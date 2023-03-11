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

DigitalTwinFunctions.Initialize(azureConfig);
//await DigitalTwinFunctions.DeleteModelsAsync();
//await DigitalTwinFunctions.CreateModelsAsync(modelDirectory);

//DeviceSimulator simulator = new DeviceSimulator();
//List<TrainModel> trainTwins = simulator.Initialize();

//while (trainTwins.Count > 0)
//{
//    await simulator.SimulateTrainsAsync();
//}


