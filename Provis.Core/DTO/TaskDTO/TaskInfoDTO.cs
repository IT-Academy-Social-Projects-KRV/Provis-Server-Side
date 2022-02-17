using System;
using System.Collections.Generic;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Deadline { get; set; }
        public int StatusId { get; set; }
        public int? StoryPoints { get; set; }
        public List<TaskAssignedUsersDTO> AssignedUsers { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
