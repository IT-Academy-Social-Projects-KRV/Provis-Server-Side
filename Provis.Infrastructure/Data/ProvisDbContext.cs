using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.CommentAttachmentEntity;
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
using Provis.Core.Entities.WorkspaceTaskAttachmentEntity;
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
            modelBuilder.ApplyConfiguration(new WorkspaceTaskAttachmentConfiguration());

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
        public DbSet<WorkspaceTaskAttachment> TaskAttachments { get; set; }
        public DbSet<CommentAttachment> CommentAttachments { get; set; }
    }
}
