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

            List<TrainModel> trains = await DigitalTwinFunctions.GetTrainsAsync();
            foreach (TrainModel train in trains)
            {
                SimulateSensors(train);
            }
            await Task.Delay(TimeDelayms);
        }

        private void SimulateSensors(TrainModel trainTwin)
        {
            // Writing block distances
            Console.WriteLine($"{trainTwin.FrontTravelled:F2} - {trainTwin.RearTravelled:F2} : {trainTwin.Speed * 18.0 / 5:F2}");

            LocationSensorModel frontSensor = new LocationSensorModel(trainTwin.FrontSensor);
            LocationSensorModel rearSensor = new LocationSensorModel(trainTwin.RearSensor);
            frontSensor.TimeElapsed = _timer;
            rearSensor.TimeElapsed = _timer;

            // slightly changing the speed to show some variance in UI
            Random random = new Random();
            double randomNumber = Math.Round(random.NextDouble() * 2 - 1, 2);
            double speed = trainTwin.SimulatorSpeed + randomNumber;
            double distanceTravelled = speed * _timer;

            Location frontLocation = DistanceCalculator.GetPoint2(frontSensor.Location, distanceTravelled);
            Location rearLocation = DistanceCalculator.GetPoint2(rearSensor.Location, distanceTravelled);
            DeviceTwinFunctions.ProcessLocationSensor(frontSensor, frontLocation, _sectionModel!.StartLocation);
            DeviceTwinFunctions.ProcessLocationSensor(rearSensor, rearLocation, _sectionModel!.StartLocation);
        }
    }
}
