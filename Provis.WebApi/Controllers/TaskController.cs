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
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        public async Task<IActionResult> ChangeTaskStatusAsync(TaskChangeStatusDTO changeTaskStatus)
        {
            var taskStatus = await _taskService.ChangeTaskStatusAsync(changeTaskStatus, UserId);

            return Ok(taskStatus);
        }

        [Authorize]
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("task")]
        public async Task<IActionResult> AddTaskAsync([FromBody] TaskCreateDTO createDTO)
        {
            await _taskService.CreateTaskAsync(createDTO, UserId);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("tasks")]
        public async Task<IActionResult> GetTasks(string userId, int workspaceId, int? sprintId)
        {
            var getTasks = await _taskService.GetTasks(userId, workspaceId, sprintId);

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
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("task")]
        public async Task<IActionResult> ChangeTaskAsync([FromBody] TaskChangeInfoDTO taskChangeInfoDTO)
        {
            var worskaceTask = await _taskService.ChangeTaskInfoAsync(taskChangeInfoDTO, UserId);

            return Ok(worskaceTask);
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
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("workspace/{workspaceId}/task/{taskId}")]
        public async Task<IActionResult> GetTaskInfoAndAssignedUsersAsync(int taskId)
        {
            var res = await _taskService.GetTaskInfoAsync(taskId);

            return Ok(res);
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("task/{taskId}/workspace/{workspaceId}/attachments")]
        public async Task<IActionResult> GetTaskAttachmentsAsync(int taskId)
        {
            var res = await _taskService.GetTaskAttachmentsAsync(taskId);

            return Ok(res);
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("task/workspace/{workspaceId}/attachment/{attachmentId}")]
        public async Task<IActionResult> GetTaskAttachmentAsync(int attachmentId)
        {
            var file = await _taskService.GetTaskAttachmentAsync(attachmentId);

            return File(file.Content, file.ContentType, file.Name);
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        [Route("task/workspace/{workspaceId}/attachment/{attachmentId}/preview")]
        public async Task<IActionResult> GetTaskAttachmentPreviewAsync(int attachmentId)
        {
            var file = await _taskService.GetTaskAttachmentPreviewAsync(attachmentId);

            return File(file.Content, file.ContentType, file.Name);
        }

        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("task/workspace/{workspaceId}/attachment/{attachmentId}")]
        public async Task<IActionResult> DeleteTaskAttachmentAsync(int attachmentId)
        {
            await _taskService.DeleteTaskAttachmentAsync(attachmentId);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("task/attachments")]
        public async Task<IActionResult> SendTaskAttachmentsAsync([FromForm] TaskAttachmentsDTO taskAttachmentsDTO)
        {
            var result = await _taskService.SendTaskAttachmentsAsync(taskAttachmentsDTO);

            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        [WorkspaceRoles(new WorkSpaceRoles[]{
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId})]
        [Route("change-role")]
        public async Task<IActionResult> ChangeMemberRoleAsync([FromBody] TaskChangeRoleDTO changeRoleDTO)
        {
            await _taskService.ChangeMemberRoleAsync(changeRoleDTO, UserId);

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[]{
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId})]
        [Route("task/{taskId}/workspace/{workspaceId}/disjoin/{disUserId}")]
        public async Task<IActionResult> DisjoinTaskAsync(int workspaceId, int taskId, string disUserId)
        {
            await _taskService.DisjoinTaskAsync(workspaceId, taskId, disUserId, UserId);

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        [Route("task/{taskId}/workspace/{workspaceId}")]
        public async Task<IActionResult> DeleteTaskAsync(int taskId, int workspaceId)
        {
            await _taskService.DeleteTaskAsync(workspaceId, taskId, UserId);

            return Ok();
        }

        [Authorize]
        [HttpPut]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        [Route("{taskId}/workspace/{workspaceId}")]
        public async Task<IActionResult> MoveTaskToSprintAsync([FromRoute] int taskId,
            [FromRoute] int workspaceId,
            [FromBody] ChangeSprintForTaskDTO changeSprintForTask)
        {
            await _taskService.MoveTaskToSprint(taskId, workspaceId, changeSprintForTask.SprintId);

            return Ok();
        }
    }
}
