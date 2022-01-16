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
            await _taskService.ChangeTaskStatusAsync(changeTaskStatus, UserId);

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

        [Authorize]
        [HttpGet]
        [Route("{taskId}/history")]
        public async Task<IActionResult> GetStatusHistory(int taskId)
        {
            var res = await _taskService.GetStatusHistories(taskId);
            
             return Ok(res);
        }
        
        [Authorize]
        [HttpGet]
        [Route("task/{taskId}")]
        public async Task<IActionResult> GetTaskInfoAndAssignedUsersAsync(int taskId)
        {
            var res = await _taskService.GetTaskInfoAsync(taskId);

            return Ok(res);
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        [Route("task/{taskId}/workspace/{workspaceId}/attachments")]
        public async Task<IActionResult> GetTaskAttachmentsAsync(int taskId, int workspaceId)
        {
            var res = await _taskService.GetTaskAttachmentsAsync(taskId);
            
            return Ok(res);
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        [Route("task/workspace/{workspaceId}/attachment/{attachmentId}")]
        public async Task<IActionResult> GetTaskAttachmentAsync(int workspaceId, int attachmentId)
        {
            var file = await _taskService.GetTaskAttachmentAsync(attachmentId);

            return File(file.Content, file.ContentType, file.Name);
        }
        
        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        [Route("task/workspace/{workspaceId}/attachment/{attachmentId}")]
        public async Task<IActionResult> DeleteTaskAttachmentAsync(int workspaceId, int attachmentId)
        {
            await _taskService.DeleteTaskAttachmentAsync(attachmentId);

            return Ok();
        }

        [Authorize]
        [HttpPut]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        [Route("task/attachments")]
        public async Task<IActionResult> SendTaskAttachmentsAsync([FromForm] TaskAttachmentsDTO taskAttachmentsDTO)
        {
            await _taskService.SendTaskAttachmentsAsync(taskAttachmentsDTO);

            return Ok();
        }
    }
}
