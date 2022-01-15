using System;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskChangeInfoDTO
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
    }
}
