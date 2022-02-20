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
            var commentInfo = await _commentService.AddCommentAsync(commentDTO, UserId);

            return Ok(commentInfo);
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

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("comment/{commentId}/workspace/{workspaceId}/attachments")]
        public async Task<IActionResult> GetCommentAttachmentsAsync(int commentId)
        {
            var res = await _commentService.GetCommentAttachmentsAsync(commentId);

            return Ok(res);
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("comment/workspace/{workspaceId}/attachment/{attachmentId}")]
        public async Task<IActionResult> GetCommentAttachmentAsync(int attachmentId)
        {
            var file = await _commentService.GetCommentAttachmentAsync(attachmentId);

            return File(file.Content, file.ContentType, file.Name);
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId, WorkSpaceRoles.MemberId })]
        [Route("comment/workspace/{workspaceId}/attachment/{attachmentId}/preview")]
        public async Task<IActionResult> GetCommentAttachmentPreviewAsync(int attachmentId)
        {
            var file = await _commentService.GetCommentAttachmentPreviewAsync(attachmentId);

            return File(file.Content, file.ContentType, file.Name);
        }

        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("comment/workspace/{workspaceId}/attachment/{attachmentId}")]
        public async Task<IActionResult> DeleteCommentAttachmentAsync(int attachmentId)
        {
            await _commentService.DeleteCommentAttachmentAsync(attachmentId);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId })]
        [Route("comment/attachments")]
        public async Task<IActionResult> SendCommentAttachmentsAsync([FromForm] CommentAttachmentsDTO commentAttachmentsDTO)
        {
            var result = await _commentService.SendCommentAttachmentsAsync(commentAttachmentsDTO);

            return Ok(result);
        }
    }
}
