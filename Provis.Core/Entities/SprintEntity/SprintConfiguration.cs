using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.SprintEntity
{
    public class SprintConfiguration : IEntityTypeConfiguration<Sprint>
    {
        public void Configure(EntityTypeBuilder<Sprint> builder)
        {
            builder
                .HasKey(x=>x.Id);

            builder
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder
                .Property(x => x.Description)
                .HasMaxLength(500);

            builder
                .Property(x => x.WorkspaceId)
                .IsRequired();

            builder
                .HasOne(x => x.Workspace)
                .WithMany(x => x.Sprints)
                .HasForeignKey(x => x.WorkspaceId);
        }
    }
}
