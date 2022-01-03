using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.Interfaces.Services;
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
    }
}
