using Rail.DigitalTwin.Core.AzureFunctions;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Core.Simulator
{
    public class DeviceSimulator
    {
        private int _timer = 1; // secs

        private SectionModel? _sectionModel;

        public int TimeDelayms { get { return _timer * 1000; } }

        public async Task SimulateTrainsAsync()
        {
            //getting section model
            _sectionModel = await DigitalTwinFunctions.GetSectionAsync();

            List<TrainModel> trains = new List<TrainModel>();
            do
            {
                trains = await DigitalTwinFunctions.GetTrainsAsync();
                foreach (TrainModel train in trains)
                {
                    await SimulateSensors(train);
                }
                await Task.Delay(TimeDelayms);
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

            // Writing block distances for debugging
            var model = await DigitalTwinFunctions.GetTrainAsync(trainModel.TrainID);
            if(model != null)
            {
                Console.WriteLine($"{model.FrontTravelled:F2} - {model.RearTravelled:F2} : {model.Speed * 18.0 / 5:F2}");
            }
        }
    }
}
