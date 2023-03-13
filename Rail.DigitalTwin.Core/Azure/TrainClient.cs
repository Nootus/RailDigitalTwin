using Azure.DigitalTwins.Core;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Core.Azure
{
    public class TrainClient : TwinClient
    {
        private string _trainIDPrefix = "train-";
        private string _frontSensorName = "frontSensor";
        private string _rearSensorName = "rearSensor";

        private int _trainIndex = 0;

        private SectionClient _sectionClient;

        public TrainClient(DigitalTwinsClient client, SectionClient sectionClient) : base(client) 
        { 
            _sectionClient = sectionClient;
        }

        public async Task CreateTrainTwinAsync(TrainModel trainModel)
        {
            var section = await _sectionClient.GetSectionAsync();

            _trainIndex++;
            trainModel.TrainID = _trainIDPrefix + _trainIndex.ToString();
            trainModel.RecommendedSpeed = section.SectionModel.Speed;
            trainModel.SimulatorSpeed = trainModel.Speed;

            Location frontLocation = section.SectionModel.StartLocation;
            Location rearLocation = DistanceCalculator.GetPoint2(frontLocation, -trainModel.TrainLength);
            trainModel.FrontSensor = new LocationSensorModel(
                                            trainModel.TrainID + "-" + SensorPosition.Front.ToString(), 
                                            SensorPosition.Front, frontLocation);
            trainModel.RearSensor = new LocationSensorModel(
                                            trainModel.TrainID + "-" + SensorPosition.Rear.ToString(),
                                            SensorPosition.Rear, rearLocation);

            // creating sensor twins
            var frontSensorTwin = Mapper.MapLocationSensor(trainModel.FrontSensor);
            var rearSensorTwin = Mapper.MapLocationSensor(trainModel.RearSensor);

            var trainTwin = Mapper.MapTrain(trainModel);
            trainTwin.Contents.Add("frontSensor", frontSensorTwin);
            trainTwin.Contents.Add("rearSensor", rearSensorTwin);

            var trainTwinResponse = await _client.CreateOrReplaceDigitalTwinAsync<BasicDigitalTwin>(trainTwin.Id, trainTwin);
            trainTwin = trainTwinResponse.Value; // assigning the twin returned from Azure


            // Create relationship
            string relationName = "contains";
            string relationId = $"{section.SectionTwin.Id}-{relationName}-{trainTwin.Id}";
            var relationship = new BasicRelationship
            {
                Id = relationId,
                SourceId = section.SectionTwin.Id,
                TargetId = trainTwin.Id,
                Name = relationName

            };

            await _client.CreateOrReplaceRelationshipAsync(section.SectionTwin.Id, relationId, relationship);
        }

        public async Task<List<TrainModel>> GetTrainsAsync()
        {
            List<BasicDigitalTwin> twins = await GetTwinsAsync(ModelIDs.TrainModelID);

            List<TrainModel> models = new List<TrainModel>();
            foreach (BasicDigitalTwin tw in twins)
            {
                TrainModel model = Mapper.MapTrain(tw);
                var frontSensorTwin = (await _client.GetComponentAsync<BasicDigitalTwinComponent>(tw.Id, _frontSensorName)).Value;
                model.FrontSensor = Mapper.MapLocationSensor(frontSensorTwin);
                var rearSensorTwin = (await _client.GetComponentAsync<BasicDigitalTwinComponent>(tw.Id, _rearSensorName)).Value;
                model.RearSensor = Mapper.MapLocationSensor(rearSensorTwin);
                models.Add(model);
            }
            return models;
        }
    }
}
