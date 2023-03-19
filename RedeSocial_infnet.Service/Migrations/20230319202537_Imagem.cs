using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedeSocial_infnet.Service.Migrations
{
    public partial class Imagem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Imagem",
                table: "Posts",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Posts");
        }
    }
}
