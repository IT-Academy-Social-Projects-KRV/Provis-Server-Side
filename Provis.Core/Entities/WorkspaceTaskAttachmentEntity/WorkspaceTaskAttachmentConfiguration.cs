using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.WorkspaceTaskAttachmentEntity
{
    public class WorkspaceTaskAttachmentConfiguration : IEntityTypeConfiguration<WorkspaceTaskAttachment>
    {
        public void Configure(EntityTypeBuilder<WorkspaceTaskAttachment> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.AttachmentUrl)
                .IsRequired();

            builder
                .Property(x => x.TaskId)
                .IsRequired();
            
            builder
                .HasOne(x => x.Task)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.TaskId);      
        }      
    }
}

