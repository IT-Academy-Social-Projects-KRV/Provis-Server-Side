using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.WorkspaceTaskEntity
{
    public class WorkspaceTaskConfiguration : IEntityTypeConfiguration<WorkspaceTask>
    {
        public void Configure(EntityTypeBuilder<WorkspaceTask> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Name)
                .IsRequired();

            builder
                .Property(x => x.DateOfCreate)
                .IsRequired();

            builder
                .Property(x => x.StatusId)
                .IsRequired();

            builder
                .Property(x => x.WorkspaceId)
                .IsRequired();

            builder
                .Property(x => x.TaskCreatorId)
                .IsRequired();

            builder
                .HasOne(x => x.Status)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.StatusId);

            builder
                .HasOne(x => x.Workspace)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.WorkspaceId);

            builder
                .HasOne(x => x.TaskCreator)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.TaskCreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Sprint)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.SprintId);
                
            builder
                .Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
