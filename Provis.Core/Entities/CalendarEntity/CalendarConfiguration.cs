using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.EventEntity
{
    public class CalendarConfiguration : IEntityTypeConfiguration<Calendar>
    {
        public void Configure(EntityTypeBuilder<Calendar> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.EventName)
                .IsRequired()
                .HasMaxLength(50);

            builder
                .Property(x => x.EventMessage)
                .HasMaxLength(1000);

            builder
                .Property(x => x.DateTime)
                .IsRequired();

            builder
                .Property(x => x.CreatorId)
                .IsRequired();

            builder
                .Property(x => x.WorkspaceId)
                .IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Workspace)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.WorkspaceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
