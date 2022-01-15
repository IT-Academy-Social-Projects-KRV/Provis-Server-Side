using System;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskStatusHistoryDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public DateTime DateOfChange { get; set; }
    }
}
