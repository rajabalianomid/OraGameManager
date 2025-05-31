using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ora.GameManaging.Mafia.Data.Mapping
{
    public class RoleStatusEntityConfiguration : IEntityTypeConfiguration<RoleStatusEntity>
    {
        public void Configure(EntityTypeBuilder<RoleStatusEntity> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.ApplicationInstanceId).IsRequired().HasMaxLength(450);
            builder.Property(r => r.RoomId).IsRequired().HasMaxLength(50);
            builder.Property(r => r.UserId).IsRequired().HasMaxLength(450);
            builder.Property(r => r.RoleName).IsRequired().HasMaxLength(50);
            builder.Property(r => r.LastUpdated).IsRequired();
        }
    }
}