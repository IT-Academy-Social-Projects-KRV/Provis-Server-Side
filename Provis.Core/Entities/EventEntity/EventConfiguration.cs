using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.EventEntity
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
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
                .Property(x => x.DateOfStart)
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
