using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pmp.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectName = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectStartDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ProjectEndDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    RevisedEndDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    RenewalDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ProjectIssueDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    EstimatedHours = table.Column<decimal>(type: "TEXT", nullable: false),
                    ActualHours = table.Column<decimal>(type: "TEXT", nullable: true),
                    OnshoreManagerName = table.Column<string>(type: "TEXT", nullable: false),
                    OffshoreManagerName = table.Column<string>(type: "TEXT", nullable: false),
                    AccountManagerName = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
