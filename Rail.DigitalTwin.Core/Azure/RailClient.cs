using Azure.DigitalTwins.Core;
using Azure.Identity;
using Rail.DigitalTwin.Core.Models;
using System.Reflection;

namespace Rail.DigitalTwin.Core.Azure
{
    public class RailClient
    {
        private static readonly object _lockObj = new object ();
        private DigitalTwinsClient _client;

        // model ids
        private string _sectionModeID = "dtmi:com:nootus:Section;1";
        private string _locationSensorModelID = "dtmi:com:nootus:LocationSensor;1";
        private string _trainModelID = "dtmi:com:nootus:Train;1";

        public RailClient(AzureConfig azureConfig)
        {

            //Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", "Yyv8Q~25b5Ah1tRaJnFmYoUUFan54ot_g2aS1aoo");
            //Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", "4a2504f6-3873-427a-98da-2acbd4ddebb0");
            //Environment.SetEnvironmentVariable("AZURE_TENANT_ID", "0f679bda-5377-4857-b1cb-33cac9c03c15");

            var credential =
                new ClientSecretCredential(azureConfig.TenantID, azureConfig.ClientID, azureConfig.ClientSecret);
            //var credential = new DefaultAzureCredential();

            lock (_lockObj)
            {
                _client = new DigitalTwinsClient(new Uri(azureConfig.ADTInstanceUrl), credential);
                Console.WriteLine("Client created");
            }
        }

        public async Task CreateModelsAsync(string modelDirectory)
        {
            string sectionDtdl = File.ReadAllText(modelDirectory + @"\Section.json");
            string locationSensorDtdl = File.ReadAllText(modelDirectory + @"\LocationSensor.json");
            string trainDtdl = File.ReadAllText(modelDirectory + @"\Train.json");

            var dtdls = new List<string> { sectionDtdl, locationSensorDtdl, trainDtdl };
            await _client.CreateModelsAsync(dtdls);
            Console.WriteLine("Models created");
        }

        public async Task DeleteModelsAsync()
        {
            await _client.DeleteModelAsync(_trainModelID);
            await _client.DeleteModelAsync(_locationSensorModelID);
            await _client.DeleteModelAsync(_sectionModeID);
            Console.WriteLine("Models deleted");
        }

        //public async Task ClearTrainTwinsAsync()
        //{
        //    _client.Get
        //}
    }
}
