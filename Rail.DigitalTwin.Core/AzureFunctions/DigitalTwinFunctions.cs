using Rail.DigitalTwin.Core.Azure;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Core.AzureFunctions
{
    public static class DigitalTwinFunctions
    {
        private static RailClient? _client;
        private static readonly EventQueue<LocationSensorModel> _sensorQueue;

        static DigitalTwinFunctions()
        {
            _sensorQueue = EventQueue<LocationSensorModel>.Instance;
            _sensorQueue.Enqueued += ProcessLocationSensorAsync;
        }

        #region Initialize
        public static void Connect(AzureConfig azureConfig)
        {
            _client = new RailClient(azureConfig);
            Console.WriteLine("Client created");
        }

        public static async Task CreateModelsAsync(string modelDirectory)
        {
            await _client!.CreateModelsAsync(modelDirectory);
            Console.WriteLine("Models created");
        }

        public static async Task CreateSectionTwinAsync(SectionModel sectionModel)
        {
            await _client!.CreateSectionTwinAsync(sectionModel);
            Console.WriteLine("Section Twin Created");
        }
        #endregion

        #region Cleanup
        public static async Task CleanupAsync()
        {
            // delete all Twins and Models
            await DeleteTwinsAsync();
            await DeleteModelsAsync();
        }

        public static async Task DeleteModelsAsync()
        {
            await _client!.DeleteModelsAsync();
            Console.WriteLine("Models deleted");
        }

        public static async Task DeleteTwinsAsync()
        {
            await _client!.DeleteTwinsAsync();
            Console.WriteLine("All Twins Deleted");
        }

        public static async Task DeleteTrainTwins()
        {
            await _client!.DeleteTrainTwinsAsync();
            Console.WriteLine("Train Twins Deleted");
        }
        #endregion

        public static async Task CreateTrainTwinAsync(TrainModel model)
        {
            await _client!.CreateTrainTwinAsync(model);
            Console.WriteLine("Train Twin Created");
        }

        // used in simulator
        public static async Task<List<TrainModel>> GetTrainsAsync()
        {
            return await _client!.GetTrainsAsync();
        }

        // used in simulator
        public static async Task<TrainModel?> GetTrainAsync(string trainID)
        {
            return await _client!.GetTrainAsync(trainID);
        }

        public static async Task<LocationSensorModel> GetSensorByIDAsync(string sensorID)
        {
            return await _client!.GetSensorByIDAsync(sensorID);
        }

        public static async Task<SectionModel> GetSectionAsync()
        {
            return await _client!.GetSectionAsync();
        }

        public static async Task ProcessLocationSensorAsync(LocationSensorModel sensorModel)
        {
            await _client!.ProcessLocationSensorAsync(sensorModel);
        }
    }
}
