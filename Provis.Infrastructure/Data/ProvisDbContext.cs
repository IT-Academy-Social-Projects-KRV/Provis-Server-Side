using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Provis.Core.Entities;
using Provis.Infrastructure.Data.SeedData;

namespace Provis.Infrastructure.Data
{
    public class ProvisDbContext: IdentityDbContext<User>
    {
        public ProvisDbContext(DbContextOptions<ProvisDbContext> options): base(options) 
        {
            //Database.EnsureCreated();
        }

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

            // Add UsersTasks table
            modelBuilder.Entity<Task>()
                    .HasMany(t => t.Users)
                    .WithMany(u => u.Tasks)
                    .UsingEntity<UsersTasks>(
                j => j
                    .HasOne(ut=>ut.User)
                    .WithMany(t => t.UsersTasks)
                    .HasForeignKey(ut => ut.UserId),
                j => j
                    .HasOne(ut => ut.Task)
                    .WithMany(t => t.UsersTasks)
                    .HasForeignKey(ut => ut.TaskId),
                j =>
                {
                    j.Property(pt => pt.IsDeleted).HasDefaultValue(false);
                    j.ToTable("UsersTasks");
                });

            modelBuilder.Seed();
                
            // RefreshToken Foreign Key
            modelBuilder.Entity<RefreshToken>()
                .HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x=>x.UserId);

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

            modelBuilder.Seed();
        }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Role> WorkspaceRoles { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<InviteUser> InviteUsers { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
