using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO_Task_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class Assignedusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TasksId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TasksId",
                table: "AspNetUsers",
                column: "TasksId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tasks_TasksId",
                table: "AspNetUsers",
                column: "TasksId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Tasks_TasksId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TasksId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TasksId",
                table: "AspNetUsers");
        }
    }
}
