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
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId})]
        public async Task<IActionResult> CommentAsync([FromBody] CreateCommentDTO commentDTO)
        {
            await _commentService.AddCommentAsync(commentDTO, UserId);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId})]
        [Route("{taskId}/workspace/{workspaceId}")]
        public async Task<IActionResult> GetCommentsListAsync(int taskId)
        {
            var getComments = await _commentService.GetCommentListsAsync(taskId);

            return Ok(getComments);
        }

        [Authorize]
        [HttpPut]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId})]
        public async Task<IActionResult> EditCommentAsync([FromBody] EditCommentDTO commentDTO)
        {
            await _commentService.EditCommentAsync(commentDTO, UserId);

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId})]
        [Route("{id}/workspace/{workspaceId}")]
        public async Task<IActionResult> DeleteCommentAsync(int id, int workspaceId)
        {
            await _commentService.DeleteCommentAsync(id, UserId, workspaceId);

            return Ok();
        }
    }
}
