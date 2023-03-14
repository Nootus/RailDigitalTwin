using Azure.DigitalTwins.Core;
using Rail.DigitalTwin.Core.Models;

namespace Rail.DigitalTwin.Core.Azure
{
    internal class SensorClient : TwinClient
    {
        private readonly TrainClient _trainClient;
        private readonly SectionClient _sectionClient;

        public SensorClient(DigitalTwinsClient client, TrainClient trainClient, SectionClient sectionClient) : base(client) 
        { 
            _trainClient = trainClient;
            _sectionClient = sectionClient;
        }

        public async Task ProcessLocationSensorAsync(LocationSensorModel sensor)
        {
            var section = await _sectionClient.GetSectionAsync();
            TrainModel trainModel = await _trainClient.GetTrainWithSensors(sensor.TrainID);
            //TrainModel trainModel = await _trainClient.GetTrainBySensorIDAsync(sensor.SensorID);

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
            trainModel.RecommendedSpeed = section.SectionModel.Speed;

            await _trainClient.UpdateTrainAsync(trainModel);

            // checking if the train crossed the section
            if (trainModel.FrontTravelled > section.SectionModel.Length &&
                trainModel.RearTravelled > section.SectionModel.Length)
            {
                await _trainClient.DeleteTrainByIDAsync(trainModel);
            }
        }
    }
}
