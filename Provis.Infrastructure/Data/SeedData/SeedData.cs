using Microsoft.EntityFrameworkCore;
using Provis.Core.Entities;
using Provis.Core.Roles;
using Provis.Core.Statuses;

namespace Provis.Infrastructure.Data.SeedData
{
    public static class SeedData
    {
        public static void Seed(this ModelBuilder builder)
        {
            SeedWorkspaceRole(builder);
            SeedTaskStatuses(builder);
        }

        private static void SeedTaskStatuses(ModelBuilder builder) =>
            builder.Entity<Status>().HasData(
                new Status()
                {
                    Id = (int)TaskStatuses.ToDoId,
                    StatusName = "TO DO"
                },
                new Status()
                { 
                    Id = (int)TaskStatuses.InProgressId,
                    StatusName = "In progress"
                },
                new Status()
                {
                    Id = (int)TaskStatuses.InReviewId,
                    StatusName = "In review"
                },
                new Status()
                {
                    Id = (int)TaskStatuses.OnHoldId,
                    StatusName = "On hold"
                },
                new Status()
                {
                    Id = (int)TaskStatuses.CompleatedId,
                    StatusName = "Compleated"
                },
                new Status()
                {
                    Id = (int)TaskStatuses.ClosedId,
                    StatusName = "Closed"
                });

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
    }
}
