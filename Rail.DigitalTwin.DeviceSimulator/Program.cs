using Rail.DigitalTwin.Core.AzureFunctions;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Simulator;
using Rail.DigitalTwin.Core.Utilities;
using System.Configuration;

AzureConfig azureConfig = new AzureConfig()
{
    ADTInstanceUrl = ConfigurationManager.AppSettings["ADTInstanceUrl"]!,
    TenantID = ConfigurationManager.AppSettings["AzureTenantID"]!,
    ClientID = ConfigurationManager.AppSettings["AzureClientID"]!,
    ClientSecret = ConfigurationManager.AppSettings["AzureClientSecret"]!
};

string modelDirectory = AppContext.BaseDirectory + ConfigurationManager.AppSettings["ModelDirectory"]!;


var sectionModel = new SectionModel()
{
    Length = 10 * 1000, // meters
    Speed = 150 * (5.0 / 18.0), // meters/sec
    SafeDistance = 200, // meters
    CriticalDistance = 100, // meters
    StartLocation = new Location(17, 78), // Hyderabad location
};
sectionModel.EndLocation = DistanceCalculator.GetPoint2(sectionModel.StartLocation, sectionModel.Length);


TrainModel trainModel = new TrainModel()
{
    TrainNumber = 123,
    TrainName = "PPK",
    TrainLength = 350,
    Speed = 120 * (5.0 / 18),
};



DigitalTwinFunctions.Connect(azureConfig);
await DigitalTwinFunctions.CleanupAsync();
await DigitalTwinFunctions.CreateModelsAsync(modelDirectory);
await DigitalTwinFunctions.CreateSectionTwinAsync(sectionModel);

await DigitalTwinFunctions.CreateTrainTwinAsync(trainModel);
//await DigitalTwinFunctions.CreateTrainTwinAsync(trainModel);
//await DigitalTwinFunctions.CreateTrainTwinAsync(trainModel);

await Task.Delay(5000);
DeviceSimulator simulator = new DeviceSimulator();
await simulator.SimulateTrainsAsync();

await DigitalTwinFunctions.CleanupAsync();
