using Microsoft.AspNetCore.Mvc;
using Rail.DigitalTwin.Core.Azure;
using Rail.DigitalTwin.Core.Models;

namespace Rail.DigitalTwin.Client.Controllers
{
    public class SimulatorController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateTrainTwin([FromBody] TrainModel model)
        {
            model.SimulatorSpeed = model.Speed;
            await RailFunctions.CreateTrainTwinAsync(model);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> SetTrainSpeed([FromBody] TrainSpeedModel model)
        {
            await RailFunctions.SetTrainSpeedAsync(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ClearTrainTwins()
        {
            RailFunctions.ClearTrains = true;
            return Ok();
        }
    }
}
