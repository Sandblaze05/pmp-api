using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pmp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectWizardData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EffortsJson",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TasksJson",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WizardDetailsJson",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffortsJson",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TasksJson",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "WizardDetailsJson",
                table: "Projects");
        }
    }
}
