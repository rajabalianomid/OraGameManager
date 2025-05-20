using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ora.GameManaging.Mafia.Data.Mapping
{
    public class ApplicationEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasIndex(u => new { u.UserName, u.ApplicationInstanceId }).IsUnique();
        }
    }
}
