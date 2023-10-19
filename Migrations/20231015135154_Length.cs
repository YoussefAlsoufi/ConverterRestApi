using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConverterRestApi.Migrations
{
    public partial class Length : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataConverter",
                table: "DataConverter");

            migrationBuilder.RenameTable(
                name: "DataConverter",
                newName: "DataUnits");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataUnits",
                table: "DataUnits",
                column: "UnitName");

            migrationBuilder.CreateTable(
                name: "LengthUnits",
                columns: table => new
                {
                    UnitName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LengthUnits", x => x.UnitName);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LengthUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DataUnits",
                table: "DataUnits");

            migrationBuilder.RenameTable(
                name: "DataUnits",
                newName: "DataConverter");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataConverter",
                table: "DataConverter",
                column: "UnitName");
        }
    }
}
