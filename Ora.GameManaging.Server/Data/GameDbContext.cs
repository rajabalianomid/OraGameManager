using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Server.Data.Mapping;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Ora.GameManaging.Server.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<AppInstanceEntity> AppInstances { get; set; }
        public DbSet<GameRoomEntity> Rooms { get; set; }
        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<GameEventEntity> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AppInstanceEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GameRoomEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GameEventEntityConfiguration());
        }
    }
}
