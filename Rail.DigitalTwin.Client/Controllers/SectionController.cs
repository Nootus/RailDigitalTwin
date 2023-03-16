using Microsoft.AspNetCore.Mvc;
using Rail.DigitalTwin.Core.Azure;
using Rail.DigitalTwin.Core.Models;

namespace Rail.DigitalTwin.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        [HttpGet]
        public async Task<SectionModel> Get()
        {
            return await RailFunctions.GetSectionAsync();
        }
    }
}
