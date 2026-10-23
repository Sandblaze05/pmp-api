using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pmp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectId",
                table: "Projects",
                column: "ProjectId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectId",
                table: "Projects");
        }
    }
}
