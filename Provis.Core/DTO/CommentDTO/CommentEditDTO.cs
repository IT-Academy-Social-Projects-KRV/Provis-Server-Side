using System;

namespace Provis.Core.DTO.CommentsDTO
{
    public class CommentEditDTO
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public int WorkspaceId { get; set; }
    }
}
