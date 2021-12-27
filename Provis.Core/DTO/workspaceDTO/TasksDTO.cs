using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.Core.DTO.workspaceDTO
{
    class TasksDTO
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime Deadline { get; set; }
        public string CreatorName { get; set; }
    }
}
