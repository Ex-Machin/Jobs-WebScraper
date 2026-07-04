using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutomationController : ControllerBase
    {

        private readonly IAutomationService _automation;
        public AutomationController(IAutomationService automation)
        {
            _automation = automation;
        }
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            await _automation.RunAutomation();

            return Ok();
        }
    }
}
