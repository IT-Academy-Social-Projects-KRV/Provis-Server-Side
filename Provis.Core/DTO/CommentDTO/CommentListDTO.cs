using System;

namespace Provis.Core.DTO.CommentDTO
{
    public class CommentListDTO
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public DateTime dateTime { get; set; }
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
