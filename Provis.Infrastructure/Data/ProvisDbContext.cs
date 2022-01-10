﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.RefreshTokenEntity;
using Provis.Core.Entities.RoleEntity;
using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserRoleTagEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
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

            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new InviteUserConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new StatusConfiguration());
            modelBuilder.ApplyConfiguration(new StatusHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleTagConfiguration());
            modelBuilder.ApplyConfiguration(new UserTaskConfiguration());
            modelBuilder.ApplyConfiguration(new UserWorkspaceConfiguration());
            modelBuilder.ApplyConfiguration(new WorkspaceConfiguration());
            modelBuilder.ApplyConfiguration(new WorkspaceTaskConfiguration());

            // Invite User Foreign Key
            //modelBuilder.Entity<InviteUser>()
            //    .HasOne(r => r.FromUser)
            //    .WithMany()
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<InviteUser>()
            //    .HasOne(r => r.ToUser)
            //    .WithMany()
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Restrict);

            //// Status History Foreign Key
            //modelBuilder.Entity<StatusHistory>()
            //   .HasOne(r => r.Task)
            //   .WithMany()
            //   .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<StatusHistory>()
            //    .HasOne(r => r.Status)
            //    .WithMany()
            //    .OnDelete(DeleteBehavior.Restrict);

            //// Comment Foreign Key
            //modelBuilder.Entity<Comment>()
            //   .HasOne(r => r.Task)
            //   .WithMany()
            //   .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Comment>()
            //    .HasOne(r => r.User)
            //    .WithMany()
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Restrict);

            //// Task Foreign Key
            //modelBuilder.Entity<Task>()
            //    .HasOne(u => u.UserCreator)
            //    .WithMany(u => u.Tasks)
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<UserTask>()
            //    .HasOne(x => x.Task)
            //    .WithMany(x => x.UserTasks)
            //    .HasForeignKey(x => x.TaskId);

            //modelBuilder.Entity<UserTask>()
            //   .HasOne(x => x.User)
            //   .WithMany(x => x.UserTasks)
            //   .HasForeignKey(x => x.UserId);

            //modelBuilder.Entity<UserTask>()
            //    .Property(x=>x.IsUserDeleted)
            //    .HasDefaultValue(false);

            //modelBuilder.Entity<UserRoleTag>()
            //        .HasMany(x => x.UserTasks)
            //        .WithOne(x => x.UserRoleTag)
            //        .HasForeignKey(x => x.UserRoleTagId);

            //modelBuilder.Entity<UserTask>()
            //    .HasKey(x => new { x.UserId, x.TaskId });

            //// RefreshToken Foreign Key
            //modelBuilder.Entity<RefreshToken>()
            //    .HasOne(x => x.User)
            //    .WithMany(x => x.RefreshTokens)
            //    .HasForeignKey(x=>x.UserId);

            //// Add UserWorkspace table
            //modelBuilder.Entity<Workspace>()
            //        .HasMany(u => u.Users)
            //        .WithMany(w => w.Workspaces)
            //        .UsingEntity<UserWorkspace>(
            //    j => j
            //        .HasOne(ut => ut.User)
            //        .WithMany(t => t.UserWorkspaces)
            //        .HasForeignKey(ut => ut.UserId),
            //    j => j
            //        .HasOne(ut => ut.Workspace)
            //        .WithMany(t => t.UserWorkspaces)
            //        .HasForeignKey(ut => ut.WorkspaceId),
            //    j =>
            //    {
            //        j.ToTable("UserWorkspaces");
            //    });

            modelBuilder.Seed();
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<InviteUser> InviteUsers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> WorkspaceRoles { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }
        public DbSet<UserRoleTag> UserRoleTags { get; set; }
        public DbSet<UserTask> UserTask { get; set; }
        public DbSet<UserWorkspace> UserWorkspaces { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspaceTask> Tasks { get; set; }
    }
}
