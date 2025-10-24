using CrimeManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CrimeManagementApi.Data
{
    /// <summary>
    /// Central EF Core DbContext for managing all entities, relationships, and configurations.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        // ==========================================================
        // 🔹 DbSets (Tables)
        // ==========================================================
        public DbSet<User> Users { get; set; }
        public DbSet<CrimeReport> CrimeReports { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseAssignee> CaseAssignees { get; set; }
        public DbSet<CaseParticipant> CaseParticipants { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<CaseReport> CaseReports { get; set; }
        public DbSet<Evidence> Evidence { get; set; }
        public DbSet<EvidenceAuditLog> EvidenceAuditLogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CaseComment> CaseComments { get; set; }
        public DbSet<EmailSettings> EmailSettings { get; set; }
        public DbSet<DeletionStatus> DeletionStatuses { get; set; }

        // ==========================================================
        // 🔹 Model Configuration
        // ==========================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================================================
            // 🔸 Global Convention — Convert all table names to lowercase
            // ======================================================
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                    entity.SetTableName(tableName.ToLower());
            }

            // ======================================================
            // 🔸 Explicit Table Mappings (snake_case)
            // ======================================================
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<CrimeReport>().ToTable("crime_reports");
            modelBuilder.Entity<Case>().ToTable("cases");
            modelBuilder.Entity<Evidence>().ToTable("evidence");
            modelBuilder.Entity<EvidenceAuditLog>().ToTable("evidence_audit_logs");
            modelBuilder.Entity<CaseAssignee>().ToTable("case_assignees");
            modelBuilder.Entity<CaseParticipant>().ToTable("case_participants");
            modelBuilder.Entity<Participant>().ToTable("participants");
            modelBuilder.Entity<CaseReport>().ToTable("case_reports");
            modelBuilder.Entity<Comment>().ToTable("comments");
            modelBuilder.Entity<CaseComment>().ToTable("case_comments");
            modelBuilder.Entity<DeletionStatus>().ToTable("deletion_status");
            modelBuilder.Entity<EmailSettings>().HasNoKey();

            // ======================================================
            // 🔸 Relationships
            // ======================================================
            modelBuilder.Entity<Case>()
                .HasOne(c => c.CreatedByUser)
                .WithMany(u => u.CreatedCases)
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evidence>()
                .HasOne(e => e.Case)
                .WithMany(c => c.EvidenceList)
                .HasForeignKey(e => e.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evidence>()
                .HasOne(e => e.AddedByUser)
                .WithMany(u => u.AddedEvidence)
                .HasForeignKey(e => e.AddedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EvidenceAuditLog>()
                .HasOne(ea => ea.Evidence)
                .WithMany(e => e.AuditLogs)
                .HasForeignKey(ea => ea.EvidenceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EvidenceAuditLog>()
                .HasOne(ea => ea.ActedByUser)
                .WithMany()
                .HasForeignKey(ea => ea.ActedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CaseAssignee>()
                .HasOne(a => a.Case)
                .WithMany(c => c.Assignees)
                .HasForeignKey(a => a.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CaseAssignee>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CaseParticipant>()
                .HasOne(cp => cp.Case)
                .WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CaseParticipant>()
                .HasOne(cp => cp.Participant)
                .WithMany(p => p.CaseParticipants)
                .HasForeignKey(cp => cp.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CaseReport>()
                .HasOne(cr => cr.Case)
                .WithMany(c => c.CaseReports)
                .HasForeignKey(cr => cr.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CaseReport>()
                .HasOne(cr => cr.Report)
                .WithMany(r => r.CaseReports)
                .HasForeignKey(cr => cr.ReportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CaseComment>()
                .HasOne(cc => cc.Case)
                .WithMany(c => c.CaseComments)
                .HasForeignKey(cc => cc.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CaseComment>()
                .HasOne(cc => cc.User)
                .WithMany(u => u.CaseComments)
                .HasForeignKey(cc => cc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================================================
            // 🔸 Indexes for Performance
            // ======================================================
            modelBuilder.Entity<Case>()
                .HasIndex(c => c.CaseNumber)
                .IsUnique();

            modelBuilder.Entity<Case>()
                .HasIndex(c => c.Status);

            modelBuilder.Entity<Evidence>()
                .HasIndex(e => e.Type);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.CreatedAt);

            modelBuilder.Entity<CaseComment>()
                .HasIndex(cc => cc.CaseId);

            modelBuilder.Entity<CaseParticipant>()
                .HasIndex(cp => new { cp.CaseId, cp.ParticipantId })
                .IsUnique();

            // 🔹 Convert Enum to String for PostgreSQL compatibility
            modelBuilder.Entity<User>()
                .Property(u => u.ClearanceLevel)
                .HasConversion<string>();

            // ======================================================
            // 🔹 Global Query Filters for Soft Delete
            // ======================================================
            modelBuilder.Entity<Evidence>()
                .HasQueryFilter(e => !e.IsDeleted);


            // You can easily extend this later:
            // modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            // modelBuilder.Entity<Case>().HasQueryFilter(c => !c.IsDeleted);

            // ======================================================
            // 🔸 Seed Default Admin
            // ======================================================
            var admin = new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@crimeapi.com",
                FullName = "System Administrator",
                Role = "Admin",
                ClearanceLevel = Models.Enums.ClearanceLevel.Critical,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                CreatedAt = DateTime.UtcNow
            };

            modelBuilder.Entity<User>().HasData(admin);
        }
    }
}
