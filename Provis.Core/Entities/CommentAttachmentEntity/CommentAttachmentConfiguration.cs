using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.CommentAttachmentEntity
{
    public class CommentAttachmentConfiguration : IEntityTypeConfiguration<CommentAttachment>
    {
        public void Configure(EntityTypeBuilder<CommentAttachment> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.AttachmentPath)
                .IsRequired();

            builder
                .Property(x => x.CommentId)
                .IsRequired();

            builder
                .HasOne(x => x.Comment)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.CommentId);
        }
    }
}

