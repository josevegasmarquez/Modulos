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

        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ProductWarehouse> Stocks { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<MovementDetail> MovementDetails { get; set; }
        public DbSet<ScheduledInventory> ScheduledInventories { get; set; }

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

            // Inventory Configuration
            builder.Entity<ProductWarehouse>(entity =>
            {
                entity.HasKey(pw => new { pw.ProductId, pw.WarehouseId });
                entity.ToTable("Stocks");

                entity.HasOne(pw => pw.Product)
                    .WithMany(p => p.Stocks)
                    .HasForeignKey(pw => pw.ProductId);

                entity.HasOne(pw => pw.Warehouse)
                    .WithMany(w => w.Stocks)
                    .HasForeignKey(pw => pw.WarehouseId);
            });

            builder.Entity<Movement>(entity =>
            {
                entity.HasOne(m => m.FromWarehouse)
                    .WithMany()
                    .HasForeignKey(m => m.FromWarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.ToWarehouse)
                    .WithMany()
                    .HasForeignKey(m => m.ToWarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.User)
                    .WithMany()
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

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
