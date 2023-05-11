using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GabinetePsicologia.Server.Data.Migrations
{
    public partial class InformeTrastorno2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severidad",
                table: "Informes");

            migrationBuilder.DropColumn(
                name: "TrastornoId",
                table: "Informes");

            migrationBuilder.AddColumn<string>(
                name: "TrastornoName",
                table: "InformeTrastorno",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrastornoTipo",
                table: "InformeTrastorno",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrastornoName",
                table: "InformeTrastorno");

            migrationBuilder.DropColumn(
                name: "TrastornoTipo",
                table: "InformeTrastorno");

            migrationBuilder.AddColumn<int>(
                name: "Severidad",
                table: "Informes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TrastornoId",
                table: "Informes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
