using System;

namespace Provis.Core.DTO.CommentsDTO
{
    public class CommentCreateDTO
    {
        public string CommentText { get; set; }
        public int TaskId { get; set; }
        public int WorkspaceId { get; set; }
    }
}
