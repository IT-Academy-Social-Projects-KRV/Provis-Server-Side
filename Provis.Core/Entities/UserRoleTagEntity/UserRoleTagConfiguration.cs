using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.UserRoleTagEntity
{
    public class UserRoleTagConfiguration : IEntityTypeConfiguration<UserRoleTag>
    {
        public void Configure(EntityTypeBuilder<UserRoleTag> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
