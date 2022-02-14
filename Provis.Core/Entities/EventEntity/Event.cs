using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserEventsEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Provis.Core.Entities.EventEntity
{
    public class Event : IBaseEntity
    {
        public int Id { get; set; }

        public string EventName { get; set; }
        public string EventMessage { get; set; }

        public DateTimeOffset DateOfStart { get; set; }
        public DateTimeOffset? DateOfEnd { get; set; }

        public string CreatorId { get; set; }
        public User User { get; set; }

        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }

        public bool IsCreatorExist { get; set; }

        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}
