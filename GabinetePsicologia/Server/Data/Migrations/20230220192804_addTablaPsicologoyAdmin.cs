using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GabinetePsicologia.Server.Data.Migrations
{
    public partial class addTablaPsicologoyAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pacientes_AspNetUsers_ApplicationUserId",
                table: "Pacientes");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Pacientes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administradores_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Psicologos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Psicologos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Psicologos_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_ApplicationUserId",
                table: "Administradores",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Psicologos_ApplicationUserId",
                table: "Psicologos",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pacientes_AspNetUsers_ApplicationUserId",
                table: "Pacientes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pacientes_AspNetUsers_ApplicationUserId",
                table: "Pacientes");

            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Psicologos");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Pacientes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Pacientes_AspNetUsers_ApplicationUserId",
                table: "Pacientes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
