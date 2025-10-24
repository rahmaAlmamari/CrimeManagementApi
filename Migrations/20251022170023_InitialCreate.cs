using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CrimeManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "deletion_status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EvidenceId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deletion_status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emailsettings",
                columns: table => new
                {
                    SmtpServer = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    SenderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SenderEmail = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    EnableSsl = table.Column<bool>(type: "boolean", nullable: false),
                    EnableEmailNotifications = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ClearanceLevel = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AreaCity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CaseType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClearanceLevel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cases_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ReferenceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReferenceId = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_comments_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "crime_reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AreaCity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReportDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReportedByUserId = table.Column<int>(type: "integer", nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric(10,8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(11,8)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crime_reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_crime_reports_users_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "case_assignees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AssignedRole = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ProgressStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_assignees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_case_assignees_cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_case_assignees_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "case_comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UserRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_case_comments_cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_case_comments_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evidence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MimeType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AddedByUserId = table.Column<int>(type: "integer", nullable: true),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_evidence_cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_evidence_users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CaseId = table.Column<int>(type: "integer", nullable: true),
                    AddedByUserId = table.Column<int>(type: "integer", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_participants_cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "cases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_participants_users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "case_reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    ReportId = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LinkedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_case_reports_cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_case_reports_crime_reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "crime_reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "evidence_audit_logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EvidenceId = table.Column<int>(type: "integer", nullable: false),
                    ActedByUserId = table.Column<int>(type: "integer", nullable: true),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ActedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidence_audit_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_evidence_audit_logs_evidence_EvidenceId",
                        column: x => x.EvidenceId,
                        principalTable: "evidence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_evidence_audit_logs_users_ActedByUserId",
                        column: x => x.ActedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "case_participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    ParticipantId = table.Column<int>(type: "integer", nullable: false),
                    AddedByUserId = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    LinkedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_case_participants_cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_case_participants_participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "ClearanceLevel", "CreatedAt", "Email", "FullName", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, "Critical", new DateTime(2025, 10, 22, 17, 0, 23, 189, DateTimeKind.Utc).AddTicks(285), "admin@crimeapi.com", "System Administrator", "$2a$11$7h2FN78Yk/PVvVNzbitQRuvcCUw8qLzhIrkajAI6CGy39i8IFyRBO", "Admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_case_assignees_CaseId",
                table: "case_assignees",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_case_assignees_UserId",
                table: "case_assignees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_case_comments_CaseId",
                table: "case_comments",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_case_comments_UserId",
                table: "case_comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_case_participants_CaseId_ParticipantId",
                table: "case_participants",
                columns: new[] { "CaseId", "ParticipantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_case_participants_ParticipantId",
                table: "case_participants",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_case_reports_CaseId",
                table: "case_reports",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_case_reports_ReportId",
                table: "case_reports",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_cases_CaseNumber",
                table: "cases",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cases_CreatedByUserId",
                table: "cases",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_cases_Status",
                table: "cases",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_comments_CreatedAt",
                table: "comments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_comments_UserId",
                table: "comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_crime_reports_ReportedByUserId",
                table: "crime_reports",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_evidence_AddedByUserId",
                table: "evidence",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_evidence_CaseId",
                table: "evidence",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_evidence_Type",
                table: "evidence",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_evidence_audit_logs_ActedByUserId",
                table: "evidence_audit_logs",
                column: "ActedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_evidence_audit_logs_EvidenceId",
                table: "evidence_audit_logs",
                column: "EvidenceId");

            migrationBuilder.CreateIndex(
                name: "IX_participants_AddedByUserId",
                table: "participants",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_participants_CaseId",
                table: "participants",
                column: "CaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "case_assignees");

            migrationBuilder.DropTable(
                name: "case_comments");

            migrationBuilder.DropTable(
                name: "case_participants");

            migrationBuilder.DropTable(
                name: "case_reports");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "deletion_status");

            migrationBuilder.DropTable(
                name: "emailsettings");

            migrationBuilder.DropTable(
                name: "evidence_audit_logs");

            migrationBuilder.DropTable(
                name: "participants");

            migrationBuilder.DropTable(
                name: "crime_reports");

            migrationBuilder.DropTable(
                name: "evidence");

            migrationBuilder.DropTable(
                name: "cases");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
