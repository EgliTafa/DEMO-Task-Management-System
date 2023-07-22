using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO_Task_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class Updateprioritycoulmnandaddurgencycoulumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Urgency",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Urgency",
                table: "Tasks");
        }
    }
}
