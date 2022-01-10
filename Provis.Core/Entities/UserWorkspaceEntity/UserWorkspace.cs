using Provis.Core.Entities.RoleEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Interfaces;

namespace Provis.Core.Entities.UserWorkspaceEntity
{
    public class UserWorkspace : IBaseEntity
    {
        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
