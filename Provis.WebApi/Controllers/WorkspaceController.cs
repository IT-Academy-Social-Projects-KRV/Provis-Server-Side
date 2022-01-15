using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.WorkspaceDTO;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using Provis.WebApi.Policy;
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
        [Route("invite")]
        public async Task<IActionResult> SendInviteToUser([FromBody] WorkspaceInviteUserDTO inviteUser)
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
        public async Task<IActionResult> GetUserChangeRoleAsync([FromBody] WorkspaceChangeRoleDTO userChangeRoleDTO)
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
        [Route("{workspaceId}/info")]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId
        })]
        public async Task<IActionResult> GetWorkspaceInfoAsync(int workspaceId)
        {
            var workspaceInfo = await _workspaceService.GetWorkspaceInfoAsync(workspaceId, UserId);

            return Ok(workspaceInfo);
        }

        [HttpGet]
        [Authorize]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId
        })]
        [Route("{workspaceId}/description")]
        public async Task<IActionResult> GetWorkspaceDescriptionAsync(int workspaceId)
        {
            var workspaceDescription = await _workspaceService.GetWorkspaceDescriptionAsync(workspaceId);

            return Ok(workspaceDescription);
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
        [Route("workspaces/{workspaceId}/members")]
        public async Task<IActionResult> GetWorkspaceMembersAsync(int workspaceId)
        {
            var members = await _workspaceService.GetWorkspaceMembersAsync(workspaceId);
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

        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId })]
        [Route("{workspaceId}/user")]
        public async Task<IActionResult> LeaveWorkspace(int workspaceId)
        {
            await _workspaceService.DeleteFromWorkspaceAsync(workspaceId, UserId);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("{workspaceId}/member/list")]
        public async Task<IActionResult> GetWorkspaceDetailMember(int workspaceId)
        {
            var res = await _workspaceService.GetDetailMemberAsyns(workspaceId);
            return Ok(res);
        }
    }
}
