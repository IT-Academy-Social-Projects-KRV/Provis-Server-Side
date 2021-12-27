using System;

namespace Provis.Core.DTO.workspaceDTO
{
    public class TaskCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateOfEnd { get; set; }
        public int WorkspaceID { get; set; }
    }
}
