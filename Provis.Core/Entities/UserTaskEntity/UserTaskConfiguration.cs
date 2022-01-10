using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.UserTaskEntity
{
    public class UserTaskConfiguration : IEntityTypeConfiguration<UserTask>
    {
        public void Configure(EntityTypeBuilder<UserTask> builder)
        {
            builder
                .HasKey(x => new { x.UserId, x.TaskId });

            builder
                 .Property(x => x.TaskId)
                 .IsRequired();

            builder
                 .Property(x => x.UserId)
                 .IsRequired();

            builder
                 .Property(x => x.UserRoleTagId)
                 .IsRequired();

            builder
                 .HasOne(x => x.Task)
                 .WithMany(x => x.UserTasks)
                 .HasForeignKey(x => x.TaskId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder
                 .HasOne(x => x.User)
                 .WithMany(x => x.UserTasks)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder
                 .HasOne(x => x.UserRoleTag)
                 .WithMany(x => x.UserTasks)
                 .HasForeignKey(x => x.UserRoleTagId)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
