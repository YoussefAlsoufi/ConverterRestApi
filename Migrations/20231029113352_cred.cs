using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConverterRestApi.Migrations
{
    public partial class cred : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempertureUnits",
                columns: table => new
                {
                    UnitName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempertureUnits", x => x.UnitName);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempertureUnits");
        }
    }
}
