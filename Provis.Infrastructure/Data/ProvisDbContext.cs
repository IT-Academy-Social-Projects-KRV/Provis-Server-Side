using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Provis.Core.Entities;

namespace Provis.Infrastructure.Data
{
    public class ProvisDbContext: IdentityDbContext
    {
        public ProvisDbContext(DbContextOptions<ProvisDbContext> options): base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<InviteUser> InviteUsers { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Status> Statuses { get; set; }

        public DbSet<StatusHistory> StatusHistories { get; set; }

        public DbSet<Workspace> Workspaces { get; set; }
    }
}
