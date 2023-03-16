using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Simulator;
using System.Configuration;

AzureConfig azureConfig = new AzureConfig()
{
    ADTInstanceUrl = ConfigurationManager.AppSettings["ADTInstanceUrl"]!,
    TenantID = ConfigurationManager.AppSettings["AzureTenantID"]!,
    ClientID = ConfigurationManager.AppSettings["AzureClientID"]!,
    ClientSecret = ConfigurationManager.AppSettings["AzureClientSecret"]!,
    ModelDirectory = AppContext.BaseDirectory + ConfigurationManager.AppSettings["ModelDirectory"]!
};


DeviceSimulator simulator = new DeviceSimulator();
await simulator.Initialize(azureConfig, true);
await simulator.SimulateTrainsAsync(true);
//await DigitalTwinFunctions.CleanupAsync();
