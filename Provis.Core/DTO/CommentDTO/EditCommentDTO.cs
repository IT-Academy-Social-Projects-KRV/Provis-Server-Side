using System;

namespace Provis.Core.DTO.CommentsDTO
{
    public class EditCommentDTO
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public int WorkspaceId { get; set; }
    }
}
