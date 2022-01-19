using System;
using System.Collections.Generic;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskInfoDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public int StatusId { get; set; }
        public List<TaskAssignedUsersDTO> AssignedUsers { get; set; }        
    }
}
