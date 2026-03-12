using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modulos.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "Apellidos", "ConcurrencyStamp", "DNI", "Direccion", "Email", "EmailConfirmed", "FechaNacimiento", "FechaRegistro", "Genero", "LockoutEnabled", "LockoutEnd", "MustChangePassword", "Nombres", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a1b2c3d4-e5f6-4g7h-8i9j-k0l1m2n3o4p5", 0, "Admin", "bc3ab74f-0433-4683-a8d5-e10d6095a7b9", "42798300", null, "superadmin@modulos.com", true, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 11, 23, 31, 1, 90, DateTimeKind.Utc).AddTicks(9679), 0, false, null, false, "Super", "SUPERADMIN@MODULOS.COM", "SUPERADMIN@MODULOS.COM", "AQAAAAIAAYagAAAAEJ9MVnOCnbzCmCIcXfo7zeTaJxG3O+RqxxQ/JvtWWlEI4iFf5ydLOYO2oXpvnoe8+A==", null, false, "40ca90d1-2e35-4a0c-8c2f-1b7daabfd7e0", false, "superadmin@modulos.com" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "a1b2c3d4-e5f6-4g7h-8i9j-k0l1m2n3o4p5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "a1b2c3d4-e5f6-4g7h-8i9j-k0l1m2n3o4p5" });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-4g7h-8i9j-k0l1m2n3o4p5");
        }
    }
}
