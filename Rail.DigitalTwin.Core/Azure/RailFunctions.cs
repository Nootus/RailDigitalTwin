using Azure.DigitalTwins.Core;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Core.Azure
{
    public static class RailFunctions
    {
        private static RailClient? _railClient;
        public static bool IsConnected { get; set; }
        public static bool ClearTrains { get; set; }

        #region Initialize
        public static void Connect(AzureConfig azureConfig)
        {
            _railClient = new RailClient(azureConfig);
            IsConnected = true;
            Console.WriteLine("Client created");
        }

        public static async Task CreateModelsAsync(string modelDirectory)
        {
            await _railClient!.CreateModelsAsync(modelDirectory);
            Console.WriteLine("Models created");
        }

        public static async Task CreateSectionTwinAsync(SectionModel sectionModel)
        {
            await _railClient!.CreateSectionTwinAsync(sectionModel);
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
            await _railClient!.DeleteModelsAsync();
            Console.WriteLine("Models deleted");
        }

        public static async Task DeleteTwinsAsync()
        {
            await _railClient!.DeleteTwinsAsync();
            Console.WriteLine("All Twins Deleted");
        }

        public static async Task DeleteTrainTwinsAsync()
        {
            await _railClient!.DeleteTrainTwinsAsync();
            Console.WriteLine("Train Twins Deleted");
        }
        #endregion

        public static async Task CreateTrainTwinAsync(TrainModel model)
        {
            await _railClient!.CreateTrainTwinAsync(model);
            Console.WriteLine("Train Twin Created");
        }

        // used in simulator
        public static async Task<List<TrainModel>> GetTrainsAsync()
        {
            return await _railClient!.GetTrainsAsync();
        }

        // used in simulator
        public static async Task<TrainModel?> GetTrainAsync(string trainID)
        {
            return await _railClient!.GetTrainAsync(trainID);
        }

        public static async Task<TrainModel?> GetTrainWithSensors(string trainID)
        {
            return await _railClient!.GetTrainWithSensors(trainID);
        }

        public static async Task<LocationSensorModel?> GetSensorByIDAsync(string sensorID)
        {
            return await _railClient!.GetSensorByIDAsync(sensorID);
        }

        public static async Task<SectionModel> GetSectionAsync()
        {
            return await _railClient!.GetSectionAsync();
        }

        public static async Task UpdateTrainAsync(TrainModel trainModel)
        {
            await _railClient!.UpdateTrainAsync(trainModel);
        }

        public static async Task DeleteTrainByIDAsync(TrainModel trainModel)
        {
            await _railClient!.DeleteTrainByIDAsync(trainModel);
        }

        //public static async Task SetTrainSpeed(TrainSpeedModel model)
        //{
        //    // TODO
        //}
    }
}
