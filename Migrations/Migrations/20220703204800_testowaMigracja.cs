using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrations.Migrations
{
    public partial class testowaMigracja : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Houses",
                table: "Houses");

            migrationBuilder.RenameTable(
                name: "Houses",
                newName: "Rooms");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Houses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Houses",
                table: "Houses",
                column: "Id");
        }
    }
}
