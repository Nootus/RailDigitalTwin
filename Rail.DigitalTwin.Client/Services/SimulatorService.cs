using Microsoft.Extensions.Options;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Simulator;

namespace Rail.DigitalTwin.Client.Services
{
    public class SimulatorService : BackgroundService
    {
        private readonly AzureConfig _config;

        public SimulatorService(IOptions<AzureConfig> config) 
        { 
            _config = config.Value;
            _config.ModelDirectory = AppContext.BaseDirectory + _config.ModelDirectory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DeviceSimulator simulator = new DeviceSimulator();
            await simulator.Initialize(_config, false); ;

            while (!stoppingToken.IsCancellationRequested)
            {
                await simulator.SimulateTrainsAsync(true);
            }
        }
    }
}
