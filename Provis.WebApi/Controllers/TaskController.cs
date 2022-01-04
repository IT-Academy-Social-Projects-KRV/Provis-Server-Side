using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using Provis.WebApi.Policy;
using System.Threading.Tasks;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        protected readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Authorize]
        [HttpGet]
        [Route("usertasks")]
        public async Task<IActionResult> GetUserTasks(string userId, int workspaceId)
        {
            var getTasks = await _taskService.GetUserTasksAsync(userId, workspaceId);

            return Ok(getTasks);
        }

        [Authorize]
        [HttpPut]
        [Route("status")]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId})]
        public async Task<IActionResult> ChangeTaskStatusAsync(ChangeTaskStatusDTO changeTaskStatus)
        {
            await _taskService.ChangeTaskStatus(changeTaskStatus);

            return Ok();
        }
    }
}
