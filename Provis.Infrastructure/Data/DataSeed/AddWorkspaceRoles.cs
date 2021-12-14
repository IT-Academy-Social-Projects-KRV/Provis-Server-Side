using Provis.Core.Entities;
using Provis.Core.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.Infrastructure.Data.DataSeed
{
    public class AddWorkspaceRoles
    {
        public static void AddWorkspaceRole(ProvisDbContext ctx)
        {
            if (!ctx.WorkspaceRoles.Any())
            {
                var owner = new Role() { Id = WorkSpaceRoles.OwnerId, Name = "Owner" };
                var manager = new Role() { Id = WorkSpaceRoles.ManagerId, Name = "Manager" };
                var member = new Role() { Id = WorkSpaceRoles.MemberId, Name = "Member" };
                var viewer = new Role() { Id = WorkSpaceRoles.ViewerId, Name = "Viewer" };
                ctx.WorkspaceRoles.AddRange(owner, manager, member, viewer);
                ctx.SaveChanges();
            }
        }
    }
}
