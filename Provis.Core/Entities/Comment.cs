using Provis.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Provis.Core.Entities
{
    public class Comment : IBaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string CommentText { get; set; }

        public DateTime DateOfCreate { get; set; }

        public int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public Task Task { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
