using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore5._0.Migrations
{
    public partial class EditKeys_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeHistories",
                table: "EmployeeHistories");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Employee",
                newName: "Id");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "EmployeeHistories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeHistories",
                table: "EmployeeHistories",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeHistories",
                table: "EmployeeHistories");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EmployeeHistories");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Employee",
                newName: "EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeHistories",
                table: "EmployeeHistories",
                column: "EmployeeId");
        }
    }
}
