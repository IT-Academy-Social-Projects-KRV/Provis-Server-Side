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
        protected readonly IInviteUserService _inviteUserService;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public WorkspaceController(IWorkspaceService workspaceService, 
            IInviteUserService inviteUserService)
        {
            _workspaceService = workspaceService;
            _inviteUserService = inviteUserService;
        }

        [Authorize]
        [HttpPost]
        [Route("addworkspace")]
        public async Task<IActionResult> AddWorkspaceAsync([FromBody] WorkspaceCreateDTO createDTO)
        {
            await _workspaceService.CreateWorkspace(createDTO, UserId);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("inviteuser")]
        public async Task<IActionResult> SendInviteToUser([FromBody] InviteUserDTO inviteUser)
        {
            await _inviteUserService.SendInviteAsync(inviteUser, UserId);

            return Ok();
        }
    }
}
