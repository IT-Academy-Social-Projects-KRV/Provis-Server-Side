using Provis.Core.Interfaces;
using System;

namespace Provis.Core.Entities
{
    public class Comment : IBaseEntity
    {
        public int Id { get; set; }

        public string CommentText { get; set; }

        public int TaskId { get; set; }

        public string UserId { get; set; }

        public DateTime DateOfCreate { get; set; }

        public Task Task { get; set; }

        public User User { get; set; }
    }
}
