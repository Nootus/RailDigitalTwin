using Rail.DigitalTwin.Core.Azure;
using Rail.DigitalTwin.Core.AzureFunctions;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;
using System.Diagnostics;

namespace Rail.DigitalTwin.Core.Simulator
{
    public class DeviceSimulator
    {
        private double _timer = 1; // secs, this is starting

        private SectionModel? _sectionModel;

        public async Task Initialize(AzureConfig config, bool createTrain = false)
        {
            var sectionModel = new SectionModel()
            {
                Length = 5 * 1000, // meters
                Speed = 150 * (5.0 / 18.0), // meters/sec
                SafeDistance = 200, // meters
                CriticalDistance = 100, // meters
                StartLocation = new Location(17, 78), // Hyderabad location
            };
            sectionModel.EndLocation = DistanceCalculator.GetPoint2(sectionModel.StartLocation, sectionModel.Length);

            TrainModel trainModel = new TrainModel()
            {
                TrainNumber = 123,
                TrainName = "PPK",
                TrainLength = 100,
                Speed = 120 * (5.0 / 18),
            };


            RailFunctions.Connect(config);
            await RailFunctions.CleanupAsync();
            await RailFunctions.CreateModelsAsync(config.ModelDirectory);
            await RailFunctions.CreateSectionTwinAsync(sectionModel);

            if (createTrain)
            {
                await RailFunctions.CreateTrainTwinAsync(trainModel);
                //await Task.Delay(5000);
            }
        }

        public async Task SimulateTrainsAsync(bool writeTravelled = false)
        {
            // creating Timer object
            Stopwatch stopwatch = new Stopwatch();

            //getting section model
            _sectionModel = await RailFunctions.GetSectionAsync();

            List<TrainModel> trains = new List<TrainModel>();
            do
            {
                stopwatch.Restart();
                trains = await RailFunctions.GetTrainsAsync();
                foreach (TrainModel trainModel in trains)
                {
                    await SimulateSensors(trainModel);

                    // Writing block distances for debugging
                    if (writeTravelled)
                    {
                        var model = await RailFunctions.GetTrainAsync(trainModel.TrainID);
                        if (model != null)
                        {
                            Console.WriteLine($"{model.FrontTravelled:F2}m - {model.RearTravelled:F2}m : {model.Speed * 18.0 / 5:F2}kmph : {_timer:F2}sec");
                        }
                    }
                }
                // _timer = stopwatch.ElapsedMilliseconds / 1000.0;
                await Task.Delay((int)_timer * 1000);

            } while (trains.Count > 0);
        }

        private async Task SimulateSensors(TrainModel trainModel)
        {
            // slightly changing the speed to show some variance in UI
            Random random = new Random();
            double randomNumber = Math.Round(random.NextDouble() * 2 - 1, 2);
            double speed = trainModel.SimulatorSpeed + randomNumber;
            double distanceTravelled = speed * _timer;

            Location frontLocation = DistanceCalculator.GetPoint2(trainModel.FrontSensor.Location, distanceTravelled);
            Location rearLocation = DistanceCalculator.GetPoint2(trainModel.RearSensor.Location, distanceTravelled);

            await DeviceTwinFunctions.ProcessLocationSensor(new DeviceModel(trainModel.FrontSensor.SensorID, _timer,
                                                                frontLocation.Latitude, frontLocation.Longitude));
            await DeviceTwinFunctions.ProcessLocationSensor(new DeviceModel(trainModel.RearSensor.SensorID, _timer,
                                                                rearLocation.Latitude, rearLocation.Longitude));
        }
    }
}
