using System;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public int? WorkerRoleId { get; set; }
        public int CommentCount { get; set; }
    }
}
