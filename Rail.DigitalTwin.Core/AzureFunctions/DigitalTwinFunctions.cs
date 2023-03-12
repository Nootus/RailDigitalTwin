using Rail.DigitalTwin.Core.Azure;
using Rail.DigitalTwin.Core.Models;

namespace Rail.DigitalTwin.Core.AzureFunctions
{
    public static class DigitalTwinFunctions
    {
        private static readonly object _lockObj = new Object();
        private static RailClient? _client;

        public static void Initialize(AzureConfig azureConfig)
        {
            _client = new RailClient(azureConfig);
            Console.WriteLine("Client created");
        }

        public static async Task CreateModelsAsync(string modelDirectory)
        {
            await _client!.CreateModelsAsync(modelDirectory);
            Console.WriteLine("Models created");
        }

        public static async Task DeleteModelsAsync()
        {
            await _client!.DeleteModelsAsync();
            Console.WriteLine("Models deleted");
        }

        public static async Task<SectionModel> CreateSectionTwinAsync()
        {
            Console.WriteLine("Section Twin Created");
            return await _client!.CreateSectionTwinAsync();
        }

        public static async Task CreateTrainTwinAsync(TrainModel model)
        {
            await _client!.CreateTrainTwinAsync(model);
            Console.WriteLine("Train Twin Created");
        }

        public static async Task DeleteTrainTwins()
        {
            await _client!.DeleteTrainTwinsAsync();
            Console.WriteLine("Train Twins Deleted");
        }

        public static async Task DeleteTwinsAsync()
        {
            await _client!.DeleteTwinsAsync();
            Console.WriteLine("All Twins Deleted");
        }

        public static async Task CleanupAsync()
        {
            // delete all Twins and Models
            await DeleteTwinsAsync();
            await DeleteModelsAsync();
        }


    }
}
