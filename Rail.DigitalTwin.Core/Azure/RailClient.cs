using Azure;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;
using System.Text.Json;

namespace Rail.DigitalTwin.Core.Azure
{
    public class RailClient
    {
        private DigitalTwinsClient _client;
        private string _sectionID = "section-1";
        private string _trainIDPrefix = "train-";

        public RailClient(AzureConfig azureConfig)
        {
            var credential =
                new ClientSecretCredential(azureConfig.TenantID, azureConfig.ClientID, azureConfig.ClientSecret);

            _client = new DigitalTwinsClient(new Uri(azureConfig.ADTInstanceUrl), credential);
        }

        public async Task CreateModelsAsync(string modelDirectory)
        {
            string sectionDtdl = File.ReadAllText(modelDirectory + @"\Section.json");
            string locationSensorDtdl = File.ReadAllText(modelDirectory + @"\LocationSensor.json");
            string trainDtdl = File.ReadAllText(modelDirectory + @"\Train.json");

            var dtdls = new List<string> { sectionDtdl, locationSensorDtdl, trainDtdl };
            await _client.CreateModelsAsync(dtdls);
        }

        public async Task<SectionModel> CreateSectionTwinAsync()
        {
            var sectionModel = new SectionModel()
            {
                SectionID = _sectionID,
                Length = 10 * 1000, // meters
                Speed = 150 * (5.0 / 18.0), // meters/sec
                SafeDistance = 200, // meters
                CriticalDistance = 100, // meters
                StartLocation = new Location(17, 78), // Hyderabad location
            };

            sectionModel.EndLocation = DistanceCalculator.GetPoint2(sectionModel.StartLocation, sectionModel.Length);

            var sectionTwin = Mapper.MapSection(sectionModel);
            await _client.CreateOrReplaceDigitalTwinAsync<BasicDigitalTwin>(sectionTwin.Id, sectionTwin);
            return sectionModel;
        }

        public async Task CreateTrainTwinAsync(TrainModel trainModel)
        {
            // checking whether section Twin exists
            SectionModel sectionModel = new SectionModel();
            var sectionTwin = await GetTwinByIDAsync(_sectionID);
            if (sectionTwin == null)
            {
                sectionModel = await CreateSectionTwinAsync();
            }
            else
            {
                sectionModel = Mapper.MapSection(sectionTwin);
            }

            trainModel.TrainID = _trainIDPrefix;

            Location frontLocation = sectionModel.StartLocation;
            Location rearLocation = DistanceCalculator.GetPoint2(frontLocation, -trainModel.TrainLength);
            trainModel.FrontSensor = new LocationSensorModel(trainModel.TrainID + "-front", SensorPosition.Front, frontLocation);
            trainModel.RearSensor = new LocationSensorModel(trainModel.TrainID + "-rear", SensorPosition.Rear, rearLocation);

            // creating sensor twins
            var frontSensorTwin = Mapper.MapLocationSensor(trainModel.FrontSensor);
            var rearSensorTwin = Mapper.MapLocationSensor(trainModel.RearSensor);

            var trainTwin = Mapper.MapTrain(trainModel);
            trainTwin.Contents.Add("frontSensor", frontSensorTwin);
            trainTwin.Contents.Add("rearSensor", rearSensorTwin);


            Console.WriteLine($"Create a digital twin with a component:{Environment.NewLine}{JsonSerializer.Serialize(trainTwin)}");

            //string component1RawText = JsonSerializer.Serialize(frontSensorTwin);
            //Console.WriteLine(component1RawText);

            await _client.CreateOrReplaceDigitalTwinAsync<BasicDigitalTwin>(trainTwin.Id, trainTwin);


            //if (_twinData.TrainTwins.Count > 0)
            //    trainTwin.TrainID = _twinData.TrainTwins.Last().TrainID + 1;
            //else
            //    trainTwin.TrainID = 1;

            //Location frontLocation = sectionModel.StartLocation;
            //Location rearLocation = DistanceCalculator.GetPoint2(frontLocation, -trainModel.TrainLength);

            //trainModel.FrontSensor = new LocationSensorModel(trainModel.TrainID + "-front", SensorPosition.Front, frontLocation);
            //trainModel.RearSensor = new LocationSensorModel(trainModel.TrainID + "-rear", SensorPosition.Rear, rearLocation);



            //_twinData.SensorTwins.Add(trainModel.FrontSensor.SensorId, trainModel.FrontSensor);
            //_twinData.SensorTwins.Add(trainModel.RearSensor.SensorId, trainModel.RearSensor);
            //_twinData.TrainTwins.Add(trainModel);

        }

        public async Task DeleteTwinsAsync()
        {
            await DeleteTrainTwinsAsync();
            await DeleteSectionTwinAsync();
        }

        public async Task DeleteSectionTwinAsync()
        {
            await DeleteAllTwinsAsync(ModelIDs.SectionModeID);
        }

        public async Task DeleteTrainTwinsAsync()
        {
            await DeleteAllTwinsAsync(ModelIDs.TrainModelID);
        }

        public async Task DeleteModelsAsync()
        {
            await DeleteModelByIDAsync(ModelIDs.TrainModelID);
            await DeleteModelByIDAsync(ModelIDs.LocationSensorModelID);
            await DeleteModelByIDAsync(ModelIDs.SectionModeID);
        }

        private async Task DeleteModelByIDAsync(string modelID)
        {
            AsyncPageable<DigitalTwinsModelData> models = _client.GetModelsAsync();
            await foreach (DigitalTwinsModelData model in models)
            {
                if(model.Id == modelID)
                {
                    await _client.DeleteModelAsync(modelID);
                    break;
                }
            }
        }

        private async Task<List<BasicDigitalTwin>> GetTwinsAsync(string modelID)
        {
            string query = $"SELECT * FROM DIGITALTWINS WHERE IS_OF_MODEL('{modelID}')";

            var twinList = new List<BasicDigitalTwin>();

            AsyncPageable<BasicDigitalTwin> queryResult = _client.QueryAsync<BasicDigitalTwin>(query);

            await foreach (BasicDigitalTwin twin in queryResult)
            {
                twinList.Add(twin);
            }

            return twinList;

        }

        private async Task<int> GetTwinsCountAsync(string modelID)
        {
            int returnCount = 0;
            string query = $"SELECT COUNT() FROM DIGITALTWINS WHERE IS_OF_MODEL('{modelID}')";

            // Execute the query and iterate over the results
            AsyncPageable<int> queryResult = _client.QueryAsync<int>(query);

            await foreach (int item in queryResult)
            {
                returnCount = item;
            }

            return returnCount;

        }


        private async Task<BasicDigitalTwin?> GetTwinByIDAsync(string twinID)
        {
            try
            {
                Response<BasicDigitalTwin> response = await _client.GetDigitalTwinAsync<BasicDigitalTwin>(twinID);
                return response.Value;
            }
            catch (RequestFailedException ex)
            {
                // If failed, check if it is because of not found error (code 404)
                if (ex.Status == 404)
                {
                    // Return null if the twin does not exist
                    return null;
                }
                else
                {
                    // Rethrow exception if it is for any other reason
                    throw;
                }
            }
        }

        private async Task DeleteAllTwinsAsync(string modelID)
        {
            List<BasicDigitalTwin> twinList = await GetTwinsAsync(modelID);
            foreach (var twin in twinList)
            {
                await DeleteTwinAsync(twin.Id);
            }
        }

        private async Task DeleteTwinAsync(string twinID)
        {
            // Delete all source relationship
            AsyncPageable<BasicRelationship> relationships = _client.GetRelationshipsAsync<BasicRelationship>(twinID);
            await foreach (BasicRelationship relationship in relationships)
            {
                await _client.DeleteRelationshipAsync(twinID, relationship.Id);
            }

            // Delete all incoming relationship
            AsyncPageable<IncomingRelationship> incomingRelationships = _client.GetIncomingRelationshipsAsync(twinID);
            await foreach (IncomingRelationship incomingRelationship in incomingRelationships)
            {
                await _client.DeleteRelationshipAsync(incomingRelationship.SourceId, incomingRelationship.RelationshipId);
            }

            // Delete the digital twin
            await _client.DeleteDigitalTwinAsync(twinID);
        }

    }
}
