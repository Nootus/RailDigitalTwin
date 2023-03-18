using Azure.DigitalTwins.Core;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Core.Azure
{
    public class TrainClient : TwinClient
    {
        private readonly string _trainIDPrefix = "train-";
        private readonly string _frontSensorName = "frontSensor";
        private readonly string _rearSensorName = "rearSensor";

        private readonly SectionClient _sectionClient;
        private int _trainIndex = 0;

        private Dictionary<string, TrainModel> _trainsCache = new Dictionary<string, TrainModel>();
        private Dictionary<string, LocationSensorModel> _sensorsCache = new Dictionary<string, LocationSensorModel>();


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
                                            trainModel.TrainID + "-" + _frontSensorName, 
                                            trainModel.TrainID, SensorPosition.Front, frontLocation);
            trainModel.RearSensor = new LocationSensorModel(
                                            trainModel.TrainID + "-" + _rearSensorName,
                                            trainModel.TrainID, SensorPosition.Rear, rearLocation);

            var trainTwin = Mapper.MapTrain(trainModel);

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

            // caching
            _trainsCache.Add(trainModel.TrainID, trainModel);
            _sensorsCache.Add(trainModel.FrontSensor.SensorID, trainModel.FrontSensor);
            _sensorsCache.Add(trainModel.RearSensor.SensorID, trainModel.RearSensor);
        }

        // this is used in simulator
        public async Task<List<TrainModel>> GetTrainsAsync()
        {
            // returning from cache
            return _trainsCache.Values.OrderByDescending(t => t.TrainID).ToList();

            //List<BasicDigitalTwin> twins = await GetTwinsAsync(ModelIDs.TrainModelID);

            //List<TrainModel> models = new List<TrainModel>();
            //foreach (BasicDigitalTwin tw in twins)
            //{
            //    try
            //    {
            //        var model = await GetTrainWithSensors(tw);
            //        models.Add(model);
            //    }
            //    catch
            //    {
            //        // ignore this exception as there is a delay with SQL statement and GetDigitalTwin
            //    }
            //}
            //return models;
        }

        public async Task<TrainModel?> GetTrainAsync(string trainID)
        {
            // returning from cache
            if(_trainsCache.ContainsKey(trainID))
                return _trainsCache[trainID];

            //return cachedTrains.GetValueOrDefault(trainID);
            BasicDigitalTwin? twin = await GetTwinByIDAsync(trainID);
            if (twin == null)
                return null;
            else
                return Mapper.MapTrain(twin);
        }

        public async Task<LocationSensorModel?> GetSensorByIDAsync(string sensorID)
        {
            // returning from cache
            if (_sensorsCache.ContainsKey(sensorID))
                return _sensorsCache[sensorID];
            else
                return null;

            //string trainID = sensorID.Substring(0, sensorID.LastIndexOf("-"));
            //string componentName = sensorID.Substring(sensorID.LastIndexOf("-") + 1);
            //var sensorTwin = await _client.GetComponentAsync<BasicDigitalTwinComponent>(trainID, componentName);
            //LocationSensorModel model = Mapper.MapLocationSensor(sensorTwin);
            //return model;
        }

        // used in DigitalTwin Processing
        public async Task<TrainModel?> GetTrainWithSensors(string trainID)
        {
            // returning from cache
            if (_trainsCache.ContainsKey(trainID))
                return _trainsCache[trainID];
            else
                return null;

            //BasicDigitalTwin trainTwin = await _client.GetDigitalTwinAsync<BasicDigitalTwin>(trainID);
            //return await GetTrainWithSensors(trainTwin);
        }

        public async Task<TrainModel> GetTrainWithSensors(BasicDigitalTwin trainTwin)
        {
            TrainModel model = Mapper.MapTrain(trainTwin);
            var frontSensorTwin = await _client.GetComponentAsync<BasicDigitalTwinComponent>(trainTwin.Id, _frontSensorName);
            model.FrontSensor = Mapper.MapLocationSensor(frontSensorTwin);
            var rearSensorTwin = await _client.GetComponentAsync<BasicDigitalTwinComponent>(trainTwin.Id, _rearSensorName);
            model.RearSensor = Mapper.MapLocationSensor(rearSensorTwin);

            return model;
        }

        public async Task UpdateTrainAsync(TrainModel trainModel)
        {
            BasicDigitalTwin trainTwin = Mapper.MapTrain(trainModel);
            // make this asynchronous
            _client.CreateOrReplaceDigitalTwinAsync<BasicDigitalTwin>(trainTwin.Id, trainTwin);

            // caching
            _trainsCache[trainModel.TrainID] = trainModel;
            _sensorsCache[trainModel.FrontSensor.SensorID] = trainModel.FrontSensor;
            _sensorsCache[trainModel.RearSensor.SensorID] = trainModel.RearSensor;
        }

        public async Task SetTrainSpeedAsync(TrainSpeedModel model)
        {
            TrainModel trainModel = _trainsCache[model.TrainID];
            trainModel.Speed = model.Speed;
            trainModel.SimulatorSpeed = model.Speed;
            await UpdateTrainAsync(trainModel);
        }


        public async Task DeleteTrainsAsync()
        {
            foreach(var trainModel in _trainsCache.Values)
            {
                await DeleteTrainByIDAsync(trainModel);
            }
        }

        public async Task DeleteTrainByIDAsync(TrainModel trainModel)
        {
            // make this asynchronous
            DeleteTwinByIDAsync(trainModel.TrainID);

            // caching
            _trainsCache.Remove(trainModel.TrainID);
            _sensorsCache.Remove(trainModel.FrontSensor.SensorID);
            _sensorsCache.Remove(trainModel.RearSensor.SensorID);
        }

        public void ClearCache()
        {
            _trainsCache.Clear();
            _sensorsCache.Clear();
        }
    }
}
