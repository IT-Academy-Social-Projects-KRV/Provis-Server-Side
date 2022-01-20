using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Interfaces;
using System;

namespace Provis.Core.Entities.EventEntity
{
    public class Calendar : IBaseEntity
    {
        public int Id { get; set; }

        public string EventName { get; set; }
        public string EventMessage { get; set; }

        public DateTime DateTime { get; set; }

        public string CreatorId { get; set; }
        public User User { get; set; }

        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }

        public bool IsGeneral { get; set; }
    }
}
