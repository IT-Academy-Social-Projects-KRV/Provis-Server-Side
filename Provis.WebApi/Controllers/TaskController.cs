using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using Provis.WebApi.Policy;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        protected readonly ITaskService _taskService;

        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Authorize]
        [HttpPut]
        [Route("status")]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        public async Task<IActionResult> ChangeTaskStatusAsync(TaskChangeStatusDTO changeTaskStatus)
        {
            await _taskService.ChangeTaskStatusAsync(changeTaskStatus);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        [Route("task")]
        public async Task<IActionResult> AddTaskAsync([FromBody] TaskCreateDTO createDTO)
        {
            await _taskService.CreateTaskAsync(createDTO, UserId);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("tasks")]
        public async Task<IActionResult> GetTasks(string userId, int workspaceId)
        {
            var getTasks = await _taskService.GetTasks(userId, workspaceId);

            return Ok(getTasks);
        }
        
        [Authorize]
        [HttpGet]
        [Route("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var res = await _taskService.GetTaskStatuses();

            return Ok(res);
        }

        [Authorize]
        [HttpGet]
        [Route("roles")]
        public async Task<IActionResult> GetWorkerRoles()
        {
            var res = await _taskService.GetWorkerRoles();

            return Ok(res);
        }

        [Authorize]
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] { 
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId, 
            WorkSpaceRoles.MemberId })]
        [Route("assign")]
        public async Task<IActionResult> AssignTask([FromBody] TaskAssignDTO taskAssign)
        {
            await _taskService.JoinTaskAsync(taskAssign, UserId);
            return Ok();
        }

        [Authorize]
        [HttpPut]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        [Route("task")]
        public async Task<IActionResult> ChangeTaskAsync([FromBody] TaskChangeInfoDTO taskChangeInfoDTO)
        {
            await _taskService.ChangeTaskInfoAsync(taskChangeInfoDTO, UserId);

            return Ok();
        }
    }
}
