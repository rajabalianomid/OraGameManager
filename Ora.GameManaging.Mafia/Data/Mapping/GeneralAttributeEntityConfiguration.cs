using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ora.GameManaging.Mafia.Data.Mapping
{
    public class GeneralAttributeEntityConfiguration : IEntityTypeConfiguration<GeneralAttributeEntity>
    {
        public void Configure(EntityTypeBuilder<GeneralAttributeEntity> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(p => p.ApplicationInstanceId).IsRequired().HasMaxLength(450);
            builder.Property(p => p.EntityId).IsRequired().HasMaxLength(100);
            builder.Property(p => p.EntityName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Key).IsRequired().HasMaxLength(200);
        }
    }
}
