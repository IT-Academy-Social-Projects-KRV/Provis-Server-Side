using System;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskChangeInfoDTO
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Deadline { get; set; }
        public int? StoryPoints { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
