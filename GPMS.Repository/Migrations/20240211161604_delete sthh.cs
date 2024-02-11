using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GPMS.Repository.Migrations
{
    public partial class deletesthh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Departments_DepartmentId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DepartmentId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DeptID",
                table: "Projects",
                column: "DeptID");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Departments_DeptID",
                table: "Projects",
                column: "DeptID",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Departments_DeptID",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DeptID",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DepartmentId",
                table: "Projects",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Departments_DepartmentId",
                table: "Projects",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
