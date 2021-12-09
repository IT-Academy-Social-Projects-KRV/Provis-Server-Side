using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Provis.Core.Entities;

namespace Provis.Infrastructure.Data
{
    public class ProvisDbContext: IdentityDbContext
    {
        public ProvisDbContext(DbContextOptions<ProvisDbContext> options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Invite User Foreign Key 
            modelBuilder.Entity<InviteUser>()
                .HasOne(r => r.FromUser)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InviteUser>()
                .HasOne(r => r.ToUser)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Status History Foreign Key 
            modelBuilder.Entity<StatusHistory>()
               .HasOne(r => r.Task)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StatusHistory>()
                .HasOne(r => r.Status)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            // Comment Foreign Key 
            modelBuilder.Entity<Comment>()
               .HasOne(r => r.Task)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(r => r.User)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Task Foreign Key
            modelBuilder.Entity<Task>()
                .HasOne(u => u.UserCreator)
                .WithOne()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasMany(t => t.Users)
                .WithMany(u => u.Tasks)
                .UsingEntity(j => j.ToTable("UsersTasks"));

            // Add UserWorkspace table
            modelBuilder.Entity<Workspace>()
                    .HasMany(u => u.Users)
                    .WithMany(w => w.Workspaces)
                    .UsingEntity<UserWorkspace>(
                j => j
                    .HasOne(ut => ut.User)
                    .WithMany(t => t.UserWorkspaces)
                    .HasForeignKey(ut => ut.UserId),
                j => j
                    .HasOne(ut => ut.Workspace)
                    .WithMany(t => t.UserWorkspaces)
                    .HasForeignKey(ut => ut.WorkspaceId),
                j =>
                {
                    j.ToTable("UserWorkspaces");
                });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<InviteUser> InviteUsers { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
