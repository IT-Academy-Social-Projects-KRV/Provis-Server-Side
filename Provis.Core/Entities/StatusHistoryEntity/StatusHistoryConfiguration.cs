using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.StatusHistoryEntity
{
    public class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory>
    {
        public void Configure(EntityTypeBuilder<StatusHistory> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.DateOfChange)
                .IsRequired();

            builder
                .Property(x => x.StatusId)
                .IsRequired();

            builder
                .Property(x => x.TaskId)
                .IsRequired();

            builder
                .HasOne(x => x.Task)
                .WithMany(x => x.StatusHistories)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Status)
                .WithMany(x => x.StatusHistories)
                .HasForeignKey(x => x.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
