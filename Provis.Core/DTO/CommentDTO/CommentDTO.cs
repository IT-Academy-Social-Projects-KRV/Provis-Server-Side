using System;

namespace Provis.Core.DTO.CommentsDTO
{
    public class CommentDTO
    {
        public string CommentText { get; set; }
        public DateTime DateOfCreate { get; set; }
        public int TaskId { get; set; }
        public string UserId { get; set; }
    }
}
