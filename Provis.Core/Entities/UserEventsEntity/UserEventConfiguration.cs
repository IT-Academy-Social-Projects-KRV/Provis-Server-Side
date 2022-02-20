using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.UserEventsEntity
{
    public class UserEventConfiguration : IEntityTypeConfiguration<UserEvent>
    {
        public void Configure(EntityTypeBuilder<UserEvent> builder)
        {
            builder
                .HasKey(x => new { x.UserId, x.EventId });

            builder
                .Property(x => x.UserId)
                .IsRequired();

            builder
                .Property(x => x.EventId)
                .IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.UserEvents)
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne(x => x.Event)
                .WithMany(x => x.UserEvents)
                .HasForeignKey(x => x.EventId);
        }
    }
}
