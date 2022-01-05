using System;

namespace Provis.Core.DTO.workspaceDTO
{
    public class TaskLastDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime Deadline { get; set; }
    }
}
