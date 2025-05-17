using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ora.GameManaging.Server.Data.Mapping
{
    public class GameEventEntityConfiguration : IEntityTypeConfiguration<GameEventEntity>
    {
        public void Configure(EntityTypeBuilder<GameEventEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.EventType).IsRequired().HasMaxLength(40);
            builder.Property(e => e.Data).HasMaxLength(4000);
            builder.Property(e => e.PlayerName).HasMaxLength(50);
        }
    }
}
