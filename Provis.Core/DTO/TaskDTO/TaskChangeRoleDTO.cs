using System;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskChangeRoleDTO
    {
        public int WorkspaceId { get; set; }
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public int RoleId { get; set; }

    }
}
