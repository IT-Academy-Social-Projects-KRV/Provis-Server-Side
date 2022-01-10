using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Interfaces;
using System.Collections.Generic;

namespace Provis.Core.Entities.StatusEntity
{
    public class Status : IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<StatusHistory> TaskHistories { get; set; } = new List<StatusHistory>();
        public List<WorkspaceTask> Tasks { get; set; } = new List<WorkspaceTask>();
        public List<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();
    }
}
