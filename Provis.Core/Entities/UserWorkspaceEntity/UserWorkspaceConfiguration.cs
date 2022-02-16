using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.UserWorkspaceEntity
{
    public class UserWorkspaceConfiguration : IEntityTypeConfiguration<UserWorkspace>
    {
        public void Configure(EntityTypeBuilder<UserWorkspace> builder)
        {
            builder
                .HasKey(x => new { x.UserId, x.WorkspaceId });

            builder
                .Property(x => x.UserId)
                .IsRequired();

            builder
                .Property(x => x.WorkspaceId)
                .IsRequired();

            builder
                .Property(x => x.RoleId)
                .IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.UserWorkspaces)
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne(x => x.Workspace)
                .WithMany(x => x.UserWorkspaces)
                .HasForeignKey(x => x.WorkspaceId);

            builder
                .HasOne(x => x.Role)
                .WithMany(x => x.UserWorkspaces)
                .HasForeignKey(x => x.RoleId);

            builder
                .Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
