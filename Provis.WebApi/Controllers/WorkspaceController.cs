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
        [Route("workspace")]
        public async Task<IActionResult> AddWorkspaceAsync([FromBody] WorkspaceCreateDTO createDTO)
        {
            await _workspaceService.CreateWorkspaceAsync(createDTO, UserId);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("invite/{inviteid}/deny")]
        public async Task<IActionResult> DenyInviteUserAsync(int inviteid)
        {
            await _workspaceService.DenyInviteAsync(inviteid, UserId);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("invite/{inviteid}/accept")]
        public async Task<IActionResult> AcceptInviteUserAsync(int inviteid)
        {
            await _workspaceService.AcceptInviteAsync(inviteid, UserId);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId })]
        [Route("invite")]
        public async Task<IActionResult> SendInviteToUser([FromBody] InviteUserDTO inviteUser)
        {
            await _workspaceService.SendInviteAsync(inviteUser, UserId);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("workspaces")]
        public async Task<IActionResult> GetWorkspaceAsync()
        {
            var getList = await _workspaceService.GetWorkspaceListAsync(UserId);

            return Ok(getList);
        }

        [HttpPut]
        [Authorize]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId })]
        [Route("role")]
        public async Task<IActionResult> GetUserChangeRoleAsync([FromBody] ChangeRoleDTO userChangeRoleDTO)
        {
            var changeRole = await _workspaceService.ChangeUserRoleAsync(UserId, userChangeRoleDTO);

            return Ok(changeRole);
        }

        [HttpPut]
        [Authorize]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId })]
        [Route("workspace")]
        public async Task<IActionResult> UpdateWorkspaceAsync([FromBody] WorkspaceUpdateDTO workspaceUpdate)
        {
            await _workspaceService.UpdateWorkspaceAsync(workspaceUpdate, UserId);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("workspaces/{workspaceid}")]
        public async Task<IActionResult> GetWorkspaceInfoAsync(int workspaceid)
        {
            var workspInfo = await _workspaceService.GetWorkspaceInfoAsync(workspaceid, UserId);

            return Ok(workspInfo);
        }

        [HttpGet]
        [Authorize]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId })]
        [Route("workspaces/{workspaceid}/active-invites")]
        public async Task<IActionResult> GetWorkspaceActiveInvitesAsync(int workspaceid)
        {
            var workspInvites = await _workspaceService.GetWorkspaceActiveInvitesAsync(workspaceid, UserId);

            return Ok(workspInvites);
        }

        [HttpGet]
        [Authorize]
        [WorkspaceRoles(new WorkSpaceRoles[]{
            WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId ,
            WorkSpaceRoles.MemberId, WorkSpaceRoles.ViewerId})]
        [Route("workspaces/{workspaceid}/members")]
        public async Task<IActionResult> GetWorkspaceMembersAsync(int workspaceid)
        {
            var members = await _workspaceService.GetWorkspaceMembersAsync(workspaceid);
            return Ok(members);
        }

        [Authorize]
        [HttpGet]
        [Route("roles")]
        public async Task<IActionResult> GetWorkerRoles()
        {
            var res = await _workspaceService.GetAllowedRoles();

            return Ok(res);
        }

        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId })]
        [Route("workspaces/{workspaceid}/users/{userid}")]
        public async Task<IActionResult> DeleteFromWorkspaceAsync(int workspaceid, string userid)
        {
            await _workspaceService.DeleteFromWorkspaceAsync(workspaceid, userid);
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        [WorkspaceRoles(new[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId })]
        [Route("workspaces/{workspaceid}/invite/{inviteid}/cancel")]
        public async Task<IActionResult> CancelInviteAsync(int inviteid, int workspaceid)
        {
            await _workspaceService.CancelInviteAsync(inviteid, workspaceid, UserId);
            return Ok();
        }
    }
}
