using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ora.GameManaging.Server.Data.Mapping
{
    public class GeneralAttributeEntityConfiguration : IEntityTypeConfiguration<GeneralAttributeEntity>
    {
        public void Configure(EntityTypeBuilder<GeneralAttributeEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Value)
                .HasMaxLength(1000);

            builder.Property(e => e.EntityKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.EntityId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .HasMaxLength(500);
        }
    }
}