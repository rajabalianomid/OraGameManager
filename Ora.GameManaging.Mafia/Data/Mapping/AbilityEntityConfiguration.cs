using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ora.GameManaging.Mafia.Data.Mapping
{
    public class AbilityEntityConfiguration : IEntityTypeConfiguration<AbilityEntity>
    {
        public void Configure(EntityTypeBuilder<AbilityEntity> builder)
        {
            builder.ToTable("Abilities");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.Expression);

            builder.Property(e => e.IsNightAbility)
                .IsRequired();

            builder.Property(e => e.IsDayAbility)
                .IsRequired();

            builder.Property(e => e.RelatedPhase).HasMaxLength(50);

            builder.HasMany(e => e.GameActions).WithOne(w => w.Ability)
                .HasForeignKey(f => f.AbilityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}