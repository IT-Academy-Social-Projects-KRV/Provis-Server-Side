﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.DTO.workspaceDTO;
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
            await _taskService.ChangeTaskStatusAsync(changeTaskStatus);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId, WorkSpaceRoles.ViewerId })]
        [Route("addtask")]
        public async Task<IActionResult> AddTaskAsync([FromBody] TaskCreateDTO createDTO)
        {
            await _taskService.CreateTaskAsync(createDTO, UserId);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("gettasks")]
        public async Task<IActionResult> GetTasks(string userId, int workspaceId)
        {
            var getTasks = await _taskService.GetTasks(userId, workspaceId);

            return Ok(getTasks);
        }


        //[Authorize]
        //[HttpGet]
        //[Route("statuses")]
        //public async Task<IActionResult> GetStatuses(string userId, int workspaceId)
        //{
            //var getTasks = await _taskService.GetS(userId, workspaceId);

            //return Ok(getTasks);
        //}
    }
}
