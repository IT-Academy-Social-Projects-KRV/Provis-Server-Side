using System;
using System.Collections.Generic;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskGroupByStatusDTO
    {
        public string UserId{ get; set; }
        public Dictionary<int, List<TaskDTO>> Tasks { get; set; }
    }
}
