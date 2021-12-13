using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.userDTO;
using Provis.Core.Interfaces.Services;
using System.Threading.Tasks;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;
        public WorkspaceController(IWorkspaceService WorkspaceService)
        {
            this._workspaceService = WorkspaceService;
        }
        [HttpPost]
        [Route("addworkspace")]
        public async Task<IActionResult> RegistrationAsync([FromBody] UserIdDTO IdDTO)
        {
            await _workspaceService.CreateWorkspase(IdDTO.Id);

            return Ok();
        }
    }
}
