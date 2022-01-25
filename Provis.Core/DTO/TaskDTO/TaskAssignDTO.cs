using Provis.Core.DTO.UserDTO;
using System.Collections.Generic;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskAssignDTO
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public UserAssignedOnTaskDTO AssignedUser { get; set; }
    }
}
