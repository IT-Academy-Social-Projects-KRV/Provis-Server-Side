using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Interfaces.Services;
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
        [HttpPost]
        [Route("addtask")]
        public async Task<IActionResult> AddTaskAsync([FromBody] TaskCreateDTO createDTO)
        {
            await _taskService.CreateTaskAsync(createDTO, UserId);

            return Ok();
        }
    }
}
