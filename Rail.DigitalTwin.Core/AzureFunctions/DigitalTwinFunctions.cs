using Rail.DigitalTwin.Core.Azure;
using Rail.DigitalTwin.Core.Models;

namespace Rail.DigitalTwin.Core.AzureFunctions
{
    public static class DigitalTwinFunctions
    {
        private static RailClient? _client;

        public static void Initialize(AzureConfig azureConfig)
        {
            _client = new RailClient(azureConfig);
        }

        public static async Task CreateModelsAsync(string modelDirectory)
        {
            await _client!.CreateModelsAsync(modelDirectory);
        }

        public static async Task DeleteModelsAsync()
        {
            await _client!.DeleteModelsAsync();
        }

    }
}
