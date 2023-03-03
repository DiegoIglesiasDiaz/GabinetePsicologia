using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GabinetePsicologia.Server.Data.Migrations
{
    public partial class addCamposPersona : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Psicologos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FecNacim",
                table: "Psicologos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Pacientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FecNacim",
                table: "Pacientes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Administradores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FecNacim",
                table: "Administradores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Psicologos");

            migrationBuilder.DropColumn(
                name: "FecNacim",
                table: "Psicologos");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "FecNacim",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Administradores");

            migrationBuilder.DropColumn(
                name: "FecNacim",
                table: "Administradores");
        }
    }
}
