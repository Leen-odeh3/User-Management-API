using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GPMS.Repository.Migrations
{
    public partial class addinitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "HeadOfDepartment", "Name" },
                values: new object[,]
                {
                    { 1, "Dr. Thaer Samar", "Computer Systems Engineering" },
                    { 2, "Dr. Mahmoud Ismail", "Electrical Engineering" },
                    { 3, "Dr. Nabil Al-Tanna", "Mechatronics Engineering" },
                    { 4, "Dr. Jafar Masri", "Mechanical Engineering" },
                    { 5, "Mr. Mahmoud Sawalha", "Telecommunications Engineering and Technology" },
                    { 6, "Dr. Nabil Al-Tanna", "Sustainable Energy Engineering" },
                    { 7, "Mr. Bassel Salameh", "Civil Engineering and Surveying" },
                    { 8, "Dr. Shaher Ziyod", "Architectural Engineering" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
