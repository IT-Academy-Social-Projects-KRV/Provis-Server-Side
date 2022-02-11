using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;
using Provis.Core.Entities.EventEntity;

namespace Provis.Core.Entities.WorkspaceEntity
{
    public class Workspace : IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset DateOfCreate { get; set; }


        public List<UserWorkspace> UserWorkspaces { get; set; } = new List<UserWorkspace>();
        public List<WorkspaceTask> Tasks { get; set; } = new List<WorkspaceTask>();
        public List<InviteUser> InviteUsers { get; set; } = new List<InviteUser>();
        public List<Event> Events { get; set; } = new List<Event>();
    }
}
