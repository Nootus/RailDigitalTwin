using Rail.DigitalTwin.Core.AzureFunctions;
using Rail.DigitalTwin.Core.Models;
using System.Configuration;

AzureConfig azureConfig = new AzureConfig()
{
    ADTInstanceUrl = ConfigurationManager.AppSettings["ADTInstanceUrl"]!,
    TenantID = ConfigurationManager.AppSettings["AzureTenantID"]!,
    ClientID = ConfigurationManager.AppSettings["AzureClientID"]!,
    ClientSecret = ConfigurationManager.AppSettings["AzureClientSecret"]!
};

string modelDirectory = AppContext.BaseDirectory + ConfigurationManager.AppSettings["ModelDirectory"]!;


TrainModel trainModel = new TrainModel()
{
    TrainNumber = 123,
    TrainName = "PPK",
    TrainLength = 350,
    Speed = 120 * (5.0 / 18),
};



DigitalTwinFunctions.Initialize(azureConfig);
await DigitalTwinFunctions.CleanupAsync();
await DigitalTwinFunctions.CreateModelsAsync(modelDirectory);
await DigitalTwinFunctions.CreateSectionTwinAsync();
await DigitalTwinFunctions.CreateTrainTwinAsync(trainModel);

//await DigitalTwinFunctions.CleanupAsync();



//DeviceSimulator simulator = new DeviceSimulator();
//List<TrainModel> trainTwins = simulator.Initialize();

//while (trainTwins.Count > 0)
//{
//    await simulator.SimulateTrainsAsync();
//}


