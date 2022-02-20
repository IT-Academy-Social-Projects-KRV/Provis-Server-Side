using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.SprintDTO;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using Provis.WebApi.Policy;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintController : ControllerBase
    {
        private readonly ISprintService _sprintService;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public SprintController(ISprintService sprintService)
        {
            _sprintService = sprintService;
        }

        [HttpPost]
        [Authorize]
        [Route("workspace/{workspaceId}")]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId})]
        public async Task<IActionResult> AddSprintAsync([FromBody] ChangeSprintInfoDTO changeSprintInfo,
            [FromRoute] int workspaceId)
        {
            var res = await _sprintService.AddSprintAsync(changeSprintInfo, workspaceId);

            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [Route("workspace/{workspaceId}/sprints")]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId})]
        public async Task<IActionResult> GetSprintListAsync([FromRoute] int workspaceId)
        {
            var res = await _sprintService.GetSprintListAsync(workspaceId);

            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [Route("workspace/{workspaceId}/sprint/{sprintId}")]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId})]
        public async Task<IActionResult> GetSprintByIdAsync([FromRoute] int sprintId)
        {
            var res = await _sprintService.GetSprintById(sprintId);

            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("workspace/{workspaceId}/sprint/{sprintId}")]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId})]
        public async Task<IActionResult> UpdateSprintAsync([FromBody] ChangeSprintInfoDTO changeSprintInfo,
            [FromRoute] int sprintId)
        {
            await _sprintService.UpdateSprintAsync(changeSprintInfo, sprintId);

            return Ok();
        }

    }
}
