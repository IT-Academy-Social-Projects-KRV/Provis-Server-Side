using System.Collections.Generic;

namespace Provis.Core.DTO.workspaceDTO
{
    class TasksUserDTO
    {
        public int IdUser { get; set; }
        public int IdWorkspace { get; set; }
        public List<TasksDTO> UserTasks { get; set; }
    }
}
