using Provis.Core.DTO.userDTO;
using System.Collections.Generic;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskAssignDTO
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public List<AssignedUserOnTaskDTO> AssignedUsers { get; set; }
    }
}
