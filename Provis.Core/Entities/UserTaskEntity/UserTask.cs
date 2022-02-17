using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserRoleTagEntity;
using Provis.Core.Interfaces;

namespace Provis.Core.Entities.UserTaskEntity
{
    public class UserTask : IBaseEntity, IRowVersion
    {
        public bool IsUserDeleted { get; set; }

        public int TaskId { get; set; }
        public WorkspaceTask Task { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int UserRoleTagId { get; set; }
        public UserRoleTag UserRoleTag { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
