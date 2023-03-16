using Rail.DigitalTwin.Core.Azure;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Core.AzureFunctions
{
    public static class DigitalTwinFunctions
    {
        private static readonly EventQueue<LocationSensorModel> _sensorQueue;

        static DigitalTwinFunctions()
        {
            _sensorQueue = EventQueue<LocationSensorModel>.Instance;
            //_sensorQueue.Enqueued += ProcessLocationSensorAsync;
        }

        public static async Task ProcessLocationSensorAsync(LocationSensorModel sensor)
        {
            // checking whether to clear trains
            if (RailFunctions.ClearTrains)
            {
                await RailFunctions.DeleteTrainTwinsAsync();
                RailFunctions.ClearTrains = false;
                return;
            }

            // checking in cache before continuing


            var section = await RailFunctions.GetSectionAsync();
            TrainModel? trainModel = await RailFunctions.GetTrainWithSensors(sensor.TrainID);
            if (trainModel == null)
            {
                return;
            }

            if (sensor.Position == SensorPosition.Front)
            {
                trainModel!.FrontTravelled += sensor.DistanceTravelled;
                trainModel.FrontSensor.DistanceTravelled = sensor.DistanceTravelled;
                trainModel.FrontSensor.Speed = sensor.Speed;
                trainModel.FrontSensor.Location = sensor.Location;
            }
            else
            {
                trainModel!.RearTravelled += sensor.DistanceTravelled;
                trainModel.RearSensor = sensor;
                trainModel.RearSensor.DistanceTravelled = sensor.DistanceTravelled;
                trainModel.RearSensor.Speed = sensor.Speed;
                trainModel.RearSensor.Location = sensor.Location;
            }

            trainModel.Speed = sensor.Speed;
            trainModel.RecommendedSpeed = section.Speed;

            await RailFunctions.UpdateTrainAsync(trainModel);

            // checking if the train crossed the section
            if (trainModel.FrontTravelled > section.Length &&
                trainModel.RearTravelled > section.Length)
            {
                await RailFunctions.DeleteTrainByIDAsync(trainModel);
            }
        }
    }
}
