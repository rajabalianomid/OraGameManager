using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Ora.GameManaging.Server.Data.Mapping
{
    public class GameRoomEntityConfiguration : IEntityTypeConfiguration<GameRoomEntity>
    {
        public void Configure(EntityTypeBuilder<GameRoomEntity> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.RoomId).IsRequired().HasMaxLength(50);
            builder.HasMany(r => r.Players)
                   .WithOne(p => p.GameRoom)
                   .HasForeignKey(p => p.GameRoomEntityId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(r => r.Events)
                   .WithOne(e => e.GameRoom)
                   .HasForeignKey(e => e.GameRoomEntityId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Property(r => r.LastSnapshotJson).HasMaxLength(8000);
            builder.HasIndex(r => new { r.AppId, r.RoomId })
            .IsUnique();
        }
    }
}
