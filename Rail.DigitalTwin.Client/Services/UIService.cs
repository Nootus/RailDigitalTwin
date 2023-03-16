using Microsoft.AspNetCore.SignalR;
using Rail.DigitalTwin.Client.Hubs;
using Rail.DigitalTwin.Core.Azure;
using Rail.DigitalTwin.Core.Models;

namespace Rail.DigitalTwin.Client.Services
{
    public class UIService : BackgroundService
    {
        private readonly IHubContext<TrainHub> _hubContext;

        public UIService(IHubContext<TrainHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Generate a timer event every second
                if (RailFunctions.IsConnected)
                {
                    List<TrainModel> trains = await RailFunctions.GetTrainsAsync();
                    await _hubContext.Clients.All.SendAsync("timerEvent", trains);
                }
                await Task.Delay(1000);
            }
        }
    }
}
