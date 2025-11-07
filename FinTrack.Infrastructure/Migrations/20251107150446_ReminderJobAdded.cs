using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReminderJobAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportJobLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Period = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HangfireJobId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportJobLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportJobLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "seed-user-001",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "66676fb4-a3bc-4b93-ba86-e8881e2da722", "AQAAAAIAAYagAAAAEKHs9rZEv7/ugCX1u984zvURfNXewEt92VkvCKnvIdrYV41NAvgDsLndQxJA5cznZQ==", "1775b45a-c3aa-42ec-b4bd-5867bcb08743" });

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 1,
                column: "GeneratedAt",
                value: new DateTime(2025, 11, 7, 15, 4, 45, 734, DateTimeKind.Utc).AddTicks(5317));

            migrationBuilder.CreateIndex(
                name: "IX_ReportJobLogs_UserId",
                table: "ReportJobLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportJobLogs");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "seed-user-001",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4d1a8203-d517-4420-b3b1-bc192bd7874d", "AQAAAAIAAYagAAAAEN51L69D02/YGZGts211+i/PMRJ038eyNoX/XpabE+gR4k43i+vTC9Dy415o2rbj5Q==", "731be3d9-b5fb-467f-a4ec-83bad9857f2f" });

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "Id",
                keyValue: 1,
                column: "GeneratedAt",
                value: new DateTime(2025, 10, 25, 10, 28, 47, 723, DateTimeKind.Utc).AddTicks(5449));
        }
    }
}
