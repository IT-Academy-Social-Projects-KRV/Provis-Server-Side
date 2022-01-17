using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.CommentsDTO;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using Provis.WebApi.Policy;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        protected readonly ICommentService _commentService;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Authorize]
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetComments(int taskId)
        {
            var getComments = await _commentService.GetComments(taskId);

            return Ok(getComments);
        }

        [Authorize]
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("comment")]
        public async Task<IActionResult> Comment([FromBody] CommentDTO commentDTO)
        {
            await _
        }
    }
}
