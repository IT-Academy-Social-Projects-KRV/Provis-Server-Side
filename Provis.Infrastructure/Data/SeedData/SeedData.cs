using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Provis.Core.Entities;
using Provis.Core.Roles;
using System;

namespace Provis.Infrastructure.Data.SeedData
{
    public static class SeedData
    {
        public static void Seed(this ModelBuilder builder)
        {
            SeedWorkspaceRole(builder);
            SeedInviteInfo(builder);
            SeedAddWorkspace(builder);
        }

        public static void SeedAddWorkspace(ModelBuilder builder) =>
            builder.Entity<Workspace>().HasData(
                new Workspace()
                {
                    Id = 1,
                    Name = "workspace1",
                    Description = "workspace1"
                },
                new Workspace()
                {
                    Id = 2,
                    Name = "workspace2",
                    Description = "workspace2"
                },
                new Workspace()
                {
                    Id = 3,
                    Name = "workspace3",
                    Description = "workspace3"
                }
                );

        public static void SeedInviteInfo(ModelBuilder builder) =>
            builder.Entity<InviteUser>().HasData(
                new InviteUser()
                {
                    Id = 1,
                    Date = new DateTime(),
                    IsConfirm = null,
                    WorkspaceId = 2,
                    FromUserId = "e0baf8de-ed67-42c5-9760-4823dd664816",
                    ToUserId = "baf4ff0f-ea04-443f-80b1-044046a5dc2e"
                },
                new InviteUser()
                {
                    Id = 2,
                    Date = new DateTime(),
                    IsConfirm = true,
                    WorkspaceId = 1,
                    FromUserId = "e0baf8de-ed67-42c5-9760-4823dd664816",
                    ToUserId = "9c03ecb6-a154-445d-a66b-9cdfa67ff03e"
                },
                new InviteUser()
                {
                    Id = 3,
                    Date = new DateTime(),
                    IsConfirm = false,
                    WorkspaceId = 1,
                    FromUserId = "e0baf8de-ed67-42c5-9760-4823dd664816",
                    ToUserId = "baf4ff0f-ea04-443f-80b1-044046a5dc2e"
                }
                );

        public static void SeedWorkspaceRole(ModelBuilder builder) =>
            builder.Entity<Role>().HasData(
                new Role()
                {
                    Id = WorkSpaceRoles.OwnerId,
                    Name = "Owner",
                },
                new Role()
                {
                    Id = WorkSpaceRoles.ManagerId,
                    Name = "Manager",
                },
                new Role()
                {
                    Id = WorkSpaceRoles.MemberId,
                    Name = "Member",
                },
                new Role()
                {
                    Id = WorkSpaceRoles.ViewerId,
                    Name = "Viewer",
                });
    }
}
