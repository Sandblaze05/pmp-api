using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pmp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectIdCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectsIdCounters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    LastNumber = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectsIdCounters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProjectsIdCounters",
                columns: new[] { "Id", "LastNumber" },
                values: new object[] { 1, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectsIdCounters");
        }
    }
}
