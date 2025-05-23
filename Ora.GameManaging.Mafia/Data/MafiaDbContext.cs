﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data.Mapping;
using System.Reflection.Emit;

namespace Ora.GameManaging.Mafia.Data
{
    public class MafiaDbContext(DbContextOptions<MafiaDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<GeneralAttributeEntity> GeneralAttributes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            builder.ApplyConfiguration(new ApplicationEntityConfiguration());
            builder.ApplyConfiguration(new GeneralAttributeEntityConfiguration());
        }
    }
}
