using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ora.GameManaging.Mafia.Data.Mapping
{
    public class RoleStatusesAbilityEntityConfiguration : IEntityTypeConfiguration<RoleStatusesAbilityEntity>
    {
        public void Configure(EntityTypeBuilder<RoleStatusesAbilityEntity> builder)
        {
            builder.ToTable("RoleStatusesAbilities");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.RoleStatusId).IsRequired();
            builder.Property(e => e.AbilityId).IsRequired();

            builder.HasOne(e => e.RoleStatus)
                .WithMany(r => r.RoleStatusesAbilities)
                .HasForeignKey(e => e.RoleStatusId)
                .HasPrincipalKey(r => new { r.Id, r.RoleName })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Ability)
                .WithMany(a => a.RoleStatusesAbilities)
                .HasForeignKey(e => e.AbilityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}