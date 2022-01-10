using Microsoft.EntityFrameworkCore;
using Provis.Core.Entities.RoleEntity;
using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.UserRoleTagEntity;
using Provis.Core.Roles;
using Provis.Core.Statuses;

namespace Provis.Infrastructure.Data.SeedData
{
    public static class SeedData
    {
        public static void Seed(this ModelBuilder builder)
        {
            SeedWorkspaceRole(builder);
            SeedUserRoleTag(builder);
            SeedTaskStatuses(builder);
        }

        public static void SeedWorkspaceRole(ModelBuilder builder) =>
            builder.Entity<Role>().HasData(
                new Role()
                {
                    Id = (int)WorkSpaceRoles.OwnerId,
                    Name = "Owner",
                },
                new Role()
                {
                    Id = (int)WorkSpaceRoles.ManagerId,
                    Name = "Manager",
                },
                new Role()
                {
                    Id = (int)WorkSpaceRoles.MemberId,
                    Name = "Member",
                },
                new Role()
                {
                    Id = (int)WorkSpaceRoles.ViewerId,
                    Name = "Viewer",
                });
        public static void SeedUserRoleTag(ModelBuilder builder) =>
            builder.Entity<UserRoleTag>().HasData(
                new UserRoleTag()
                {
                    Id = (int)TaskRoles.WorkerId,
                    Name = "Worker",
                },
                new UserRoleTag()
                {
                    Id = (int)TaskRoles.SupportId,
                    Name = "Support",
                },
                new UserRoleTag()
                {
                    Id = (int)TaskRoles.ReviewerId,
                    Name = "Reviewer",
                });

         public static void SeedTaskStatuses(ModelBuilder builder) =>
             builder.Entity<Status>().HasData(
                new Status()
                {
                    Id = (int)TaskStatuses.ToDoId,
                    Name = "To do"
                },
                new Status()
                {
                    Id = (int)TaskStatuses.InProgressId,
                    Name = "In progress"
                },
                new Status()
                {
                    Id = (int)TaskStatuses.InReviewId,
                    Name = "In review"
                },
                new Status()
                {
                    Id = (int)TaskStatuses.CompleatedId,
                    Name = "Compleated"
                });
    }
}
