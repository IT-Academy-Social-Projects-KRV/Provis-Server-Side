using Provis.Core.DTO.UserDTO;
using System;
using System.Collections.Generic;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset DateOfEnd { get; set; }
        public int WorkspaceId { get; set; }
        public int StatusId { get; set; }
        public int? StoryPoints { get; set; }
        public int? SprintId { get; set; }
        public List<UserAssignedOnTaskDTO>  AssignedUsers { get; set; }
    }
}
