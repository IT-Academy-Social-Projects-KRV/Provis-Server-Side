using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.CommentEntity
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.CommentText)
                .IsRequired()
                .HasMaxLength(1000);

            builder
                .Property(x => x.DateOfCreate)
                .IsRequired();

            builder
                .Property(x => x.TaskId)
                .IsRequired();

            builder
                .Property(x => x.UserId)
                .IsRequired();

            builder
                .HasOne(x => x.Task)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
