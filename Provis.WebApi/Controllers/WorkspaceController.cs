using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.userDTO;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using Provis.WebApi.Policy;
using System.Collections.Generic;
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
            await _workspaceService.CreateWorkspaceAsync(createDTO, UserId);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("invite/{id}/deny")]
        public async Task<IActionResult> DenyInviteUserAsync(int id)
        {
            await _workspaceService.DenyInviteAsync(id, UserId);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("invite/{id}/accept")]
        public async Task<IActionResult> AcceptInviteUserAsync(int id)
        {
            await _workspaceService.AcceptInviteAsync(id, UserId);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId })]
        [Route("inviteuser")]
        public async Task<IActionResult> SendInviteToUser([FromBody] InviteUserDTO inviteUser)
        {
            await _workspaceService.SendInviteAsync(inviteUser, UserId);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("getworlspacelist")]
        public async Task<IActionResult> GetWorkspaceAsync()
        {
            var getList = await _workspaceService.GetWorkspaceListAsync(UserId);

            return Ok(getList);
        }

        [HttpPut]
        [Authorize]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId })]
        [Route("changerole")]
        public async Task<IActionResult> GetUserChangeRoleAsync([FromBody] ChangeRoleDTO userChangeRoleDTO)
        {
            var changeRole = await _workspaceService.ChangeUserRoleAsync(UserId, userChangeRoleDTO);

            return Ok(changeRole);
        }

        [HttpPut]
        [Authorize]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId })]
        [Route("updateworkspace")]
        public async Task<IActionResult> UpdateWorkspaceAsync([FromBody] WorkspaceUpdateDTO workspaceUpdate)
        {
            await _workspaceService.UpdateWorkspaceAsync(workspaceUpdate, UserId);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("getworkspace/{id}/info")]
        public async Task<IActionResult> GetWorkspaceInfoAsync(int id)
        {
            var workspInfo = await _workspaceService.GetWorkspaceInfoAsync(id, UserId);

            return Ok(workspInfo);
        }

        [HttpGet]
        [Authorize]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId })]
        [Route("{workspaceId}/invite/active")]
        public async Task<IActionResult> GetWorkspaceActiveInvitesAsync(int workspaceId)
        {
            var workspInvites = await _workspaceService.GetWorkspaceActiveInvitesAsync(workspaceId, UserId);

            return Ok(workspInvites);
        }

        [HttpGet]
        [Authorize]
        [WorkspaceRoles(new WorkSpaceRoles[]{
            WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId ,
            WorkSpaceRoles.MemberId, WorkSpaceRoles.ViewerId})]
        [Route("workspace/{workspaceId}/members")]
        public async Task<IActionResult> GetWorkspaceMembersAsync(int workspaceId)
        {
            var members = await _workspaceService.GetWorkspaceMembersAsync(workspaceId);
            return Ok(members);
        }

        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId })]
        [Route("{workspaceId}/user/{userId}")]
        public async Task<IActionResult> DeleteFromWorkspaceAsync(int workspaceId, string userId)
        {
            await _workspaceService.DeleteFromWorkspaceAsync(workspaceId, userId);
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        [WorkspaceRoles(new[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId })]
        [Route("workspace/{workspaceId}/invite/{id}/cancel")]
        public async Task<IActionResult> CancelInviteAsync(int id, int workspaceId)
        {
            await _workspaceService.CancelInviteAsync(id, workspaceId, UserId);
            return Ok();
        }
    }
}
