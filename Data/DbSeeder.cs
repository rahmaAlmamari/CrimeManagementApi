using BCrypt.Net;
using CrimeManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CrimeManagementApi.Data
{
    /// <summary>
    /// Seeds the database with a default admin account and ensures all migrations are applied.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            Console.WriteLine("============================================");
            Console.WriteLine("🔹 Starting database seeding...");

            try
            {

                // ======================================================
                // 🔹 Apply pending migrations (creates tables)
                // ======================================================
                await context.Database.MigrateAsync();
                Console.WriteLine(" Database migrations applied successfully.");

                // ======================================================
                // 🔹 Check if admin user already exists
                // ======================================================
                var adminEmail = "admin@crimeapi.com";
                var adminExists = await context.Users.AnyAsync(u => u.Email == adminEmail);

                if (!adminExists)
                {
                    var admin = new User
                    {
                        Username = "admin",
                        Email = adminEmail,
                        FullName = "System Administrator",
                        Role = "Admin",
                        ClearanceLevel = Models.Enums.ClearanceLevel.Critical,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Users.AddAsync(admin);
                    await context.SaveChangesAsync();

                    Console.WriteLine($" Default admin user created: {admin.Email}");
                }
                else
                {
                    Console.WriteLine($" Admin user already exists ({adminEmail}) — skipping seeding.");
                }

                Console.WriteLine(" Database seeding completed successfully.");
                Console.WriteLine("============================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Database seeding failed!");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("============================================");
                throw;
            }
        }
    }
}
