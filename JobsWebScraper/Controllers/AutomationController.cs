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
        [HttpPost("all")]
        public async Task<IActionResult> Post()
        {
            //await _automation.RunAutomationMacgregor();
            //await _automation.RunAutomationISS(); // html changed -  to fix
            await _automation.RunAutomationAlior();

            return Ok();
        }
    }
}
 