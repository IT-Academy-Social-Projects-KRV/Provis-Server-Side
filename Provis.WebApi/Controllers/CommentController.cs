﻿using Microsoft.AspNetCore.Authorization;
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
        [Route("comment")]
        public async Task<IActionResult> CommentAsync([FromBody] CreateCommentDTO commentDTO)
        {
            await _commentService.CommentAsync(commentDTO, UserId);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId})]
        [Route("list")]
        public async Task<IActionResult> GetComments(int taskId)
        {
            var getComments = await _commentService.GetCommentsAsync(taskId);

            return Ok(getComments);
        }

        [Authorize]
        [HttpPut]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId})]
        [Route("edit")]
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
