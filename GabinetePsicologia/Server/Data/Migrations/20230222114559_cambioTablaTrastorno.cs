using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GabinetePsicologia.Server.Data.Migrations
{
    public partial class cambioTablaTrastorno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Especificaciones",
                table: "Trastornos");

            migrationBuilder.DropColumn(
                name: "Gravedad",
                table: "Trastornos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Especificaciones",
                table: "Trastornos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Gravedad",
                table: "Trastornos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
