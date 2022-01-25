using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Interfaces;
using System;

namespace Provis.Core.Entities.InviteUserEntity
{
    public class InviteUser : IBaseEntity
    {
        public int Id { get; set; }

        public DateTimeOffset Date { get; set; }
        public bool? IsConfirm { get; set; }

        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }

        public string FromUserId { get; set; }
        public User FromUser { get; set; }

        public string ToUserId { get; set; }
        public User ToUser { get; set; }
    }
}
