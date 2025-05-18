using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Ora.GameManaging.Server.Data.Mapping
{
    public class AppInstanceEntityConfiguration : IEntityTypeConfiguration<AppInstanceEntity>
    {
        public void Configure(EntityTypeBuilder<AppInstanceEntity> builder)
        {
            builder.HasKey(a => a.AppId);

            builder.HasMany(a => a.Rooms)
                .WithOne(r => r.AppInstance)
                .HasForeignKey(r => r.AppId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
