using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.inviteUserDTO;
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
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public WorkspaceController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        [Authorize]
        [HttpPost]
        [Route("addworkspace")]
        public async Task<IActionResult> AddWorkspaceAsync([FromBody] WorkspaceCreateDTO createDTO)
        {
            await _workspaceService.CreateWorkspace(createDTO, UserId);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("denyinvitation")]
        public async Task<IActionResult> DenyInviteUserAsync([FromBody] InviteUserDTO inviteUserDenyDTO)
        {
            await _workspaceService.DenyInviteAsync(inviteUserDenyDTO, UserId);

            return Ok();
        }
    }
}
