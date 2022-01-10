using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Interfaces;
using System.Collections.Generic;

namespace Provis.Core.Entities.RoleEntity
{
    public class Role : IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<UserWorkspace> UserWorkspaces { get; set; } = new List<UserWorkspace>();
    }
}
