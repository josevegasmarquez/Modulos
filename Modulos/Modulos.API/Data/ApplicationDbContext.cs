using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modulos.API.Models;

namespace Modulos.API.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
                entity.HasIndex(u => u.DNI).IsUnique();
            });

            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

            // Seed Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                new IdentityRole { Id = "2", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "3", Name = "User", NormalizedName = "USER" }
            );

            // Seed SuperAdmin User
            var superAdminId = "a1b2c3d4-e5f6-4g7h-8i9j-k0l1m2n3o4p5";
            var superAdmin = new ApplicationUser
            {
                Id = superAdminId,
                UserName = "superadmin@modulos.com",
                NormalizedUserName = "SUPERADMIN@MODULOS.COM",
                Email = "superadmin@modulos.com",
                NormalizedEmail = "SUPERADMIN@MODULOS.COM",
                EmailConfirmed = true,
                Nombres = "Super",
                Apellidos = "Admin",
                DNI = "42798300",
                FechaNacimiento = new DateTime(1990, 1, 1),
                Genero = TipoGenero.Masculino,
                MustChangePassword = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var hasher = new PasswordHasher<ApplicationUser>();
            superAdmin.PasswordHash = hasher.HashPassword(superAdmin, "Jlvm2612@");

            builder.Entity<ApplicationUser>().HasData(superAdmin);

            // Assign SuperAdmin Role
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "1", // SuperAdmin role ID
                UserId = superAdminId
            });
        }
    }
}
