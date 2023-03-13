using Azure;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;
using System.Text.Json;

namespace Rail.DigitalTwin.Core.Azure
{
    public class RailClient: TwinClient
    {
        private SectionClient _sectionClient;
        private TrainClient _trainClient;

        #region Initialize
        public RailClient(AzureConfig azureConfig) : base(azureConfig) 
        {
            _sectionClient = new SectionClient(_client);
            _trainClient = new TrainClient(_client, _sectionClient);
        }

        public async Task CreateModelsAsync(string modelDirectory)
        {
            string sectionDtdl = File.ReadAllText(modelDirectory + @"\Section.json");
            string locationSensorDtdl = File.ReadAllText(modelDirectory + @"\LocationSensor.json");
            string trainDtdl = File.ReadAllText(modelDirectory + @"\Train.json");

            var dtdls = new List<string> { sectionDtdl, locationSensorDtdl, trainDtdl };
            await _client.CreateModelsAsync(dtdls);
        }

        public async Task<SectionModel> CreateSectionTwinAsync(SectionModel sectionModel)
        {
            return await _sectionClient.CreateSectionTwinAsync(sectionModel);
        }
        #endregion

        #region Cleanup
        public async Task DeleteModelsAsync()
        {
            await DeleteModelByIDAsync(ModelIDs.TrainModelID);
            await DeleteModelByIDAsync(ModelIDs.LocationSensorModelID);
            await DeleteModelByIDAsync(ModelIDs.SectionModeID);
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
        #endregion


        public async Task CreateTrainTwinAsync(TrainModel trainModel)
        {
            await _trainClient.CreateTrainTwinAsync(trainModel);
        }

        public async Task<List<TrainModel>> GetTrainsAsync()
        {
            return await _trainClient.GetTrainsAsync();
        }

        public async Task<SectionModel> GetSectionAsync()
        {
            var section = await _sectionClient.GetSectionAsync();
            return section.SectionModel;
        }
    }
}
