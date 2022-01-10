using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Provis.Core.Entities.InviteUserEntity
{
    public class InviteUserConfiguration : IEntityTypeConfiguration<InviteUser>
    {
        public void Configure(EntityTypeBuilder<InviteUser> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Date)
                .IsRequired();

            builder
                .Property(x => x.ToUserId)
                .IsRequired();

            builder
                .Property(x => x.FromUserId)
                .IsRequired();

            builder
                .Property(x => x.WorkspaceId)
                .IsRequired();

            builder
                .HasOne(x => x.ToUser)
                .WithMany(x => x.MyInvites)
                .HasForeignKey(x => x.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder
                .HasOne(x => x.FromUser)
                .WithMany(x => x.Invites)
                .HasForeignKey(x => x.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Workspace)
                .WithMany(x => x.InviteUsers)
                .HasForeignKey(x => x.WorkspaceId);
        }
    }
}
