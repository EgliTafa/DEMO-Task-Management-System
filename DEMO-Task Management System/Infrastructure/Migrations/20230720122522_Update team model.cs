using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO_Task_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class Updateteammodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "Teams",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Teams",
                newName: "TeamId");
        }
    }
}
