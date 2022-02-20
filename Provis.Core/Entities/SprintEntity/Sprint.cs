using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Provis.Core.Entities.SprintEntity
{
    public class Sprint: IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? DateOfStart { get; set; }
        public DateTimeOffset? DateOfEnd { get; set; }

        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }

        public List<WorkspaceTask> Tasks { get; set; } = new List<WorkspaceTask>();
    }
}
