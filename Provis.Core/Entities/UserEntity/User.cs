using Microsoft.AspNetCore.Identity;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.RefreshTokenEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.EventEntity;
using Provis.Core.Entities.UserEventsEntity;

namespace Provis.Core.Entities.UserEntity
{
    public class User : IdentityUser, IBaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ImageAvatarUrl { get; set; }
        
        public DateTimeOffset CreateDate { get; set; } // DateTime UTC
        public DateTime BirthDate { get; set; }

        public List<UserTask> UserTasks { get; set; } = new List<UserTask>();
        public List<WorkspaceTask> Tasks { get; set; } = new List<WorkspaceTask>();
        public List<UserWorkspace> UserWorkspaces { get; set; } = new List<UserWorkspace>();
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<InviteUser> MyInvites { get; set; } = new List<InviteUser>();
        public List<InviteUser> Invites { get; set; } = new List<InviteUser>();
        public List<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();
        public List<Event> Events { get; set; } = new List<Event>();
        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}
