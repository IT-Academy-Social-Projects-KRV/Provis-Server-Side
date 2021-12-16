using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Interfaces.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        protected readonly IWorkspaceService _workspaceService;
        protected readonly IEmailSenderService _emailSenderService;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public WorkspaceController(IWorkspaceService workspaceService, IEmailSenderService emailSenderService)
        {
            _workspaceService = workspaceService;
            _emailSenderService = emailSenderService;
        }

        [Authorize]
        [HttpPost]
        [Route("addworkspace")]
        public async Task<IActionResult> AddWorkspaceAsync([FromBody] WorkspaceCreateDTO createDTO)
        {
            await _workspaceService.CreateWorkspace(createDTO, UserId);

            return Ok();
        }

        [HttpGet]
        public IActionResult SendMessage()
        {
            _emailSenderService.Send("gorix2019@gmail.com", "Hello from Provis", "herakros");

            return Ok();
        }
    }
}
