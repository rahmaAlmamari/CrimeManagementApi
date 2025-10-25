using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrimeManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "PhoneNumber" },
                values: new object[] { new DateTime(2025, 10, 25, 7, 6, 0, 72, DateTimeKind.Utc).AddTicks(3824), "$2a$11$zfgjlJOoPwyrl4FSWyO02O1MF8QZjFjkf3a.jrYrv28NHOpf5IHuW", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "users");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 22, 17, 0, 23, 189, DateTimeKind.Utc).AddTicks(285), "$2a$11$7h2FN78Yk/PVvVNzbitQRuvcCUw8qLzhIrkajAI6CGy39i8IFyRBO" });
        }
    }
}
