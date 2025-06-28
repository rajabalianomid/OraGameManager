using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ora.GameManaging.Mafia.Model;

namespace Ora.GameManaging.Mafia.Data.Mapping
{
    public class GameActionHistoryEntityConfiguration : IEntityTypeConfiguration<GameActionHistoryEntity>
    {
        public void Configure(EntityTypeBuilder<GameActionHistoryEntity> builder)
        {
            builder.ToTable("GameActionHistories");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.ApplicationInstanceId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(e => e.RoomId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.ActorUserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(e => e.ActorRole)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.TargetUserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(e => e.ActionTime)
                .IsRequired();


            builder.Property(e => e.IsProcessed)
                .IsRequired();

            builder.Property(e => e.Result);

            builder.Property(e => e.Phase).HasMaxLength(20);
        }
    }
}