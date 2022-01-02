using System;

namespace Provis.Core.DTO.workspaceDTO
{
    public class TaskCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateOfEnd { get; set; }
        public int WorkspaceID { get; set; }
        public int StatusId { get; set; }
        public AssignedUserOnTask[] assignedUser { get; set; }
    }
    public class AssignedUserOnTask
    {
        public string UserId { get; set; }
        public int TaskId { get; set; }
        public int RoleTagId { get; set; }
    }
}
