using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedeSocial_infnet.Service.Migrations
{
    public partial class Posts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "Posts",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                Titulo = table.Column<string>(nullable: true),
                Conteudo = table.Column<string>(nullable: true),
                CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                EditadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
            },
        constraints: table =>
        {
            table.PrimaryKey("PK_Posts", x => x.Id);
        });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
              name: "Posts");
        }

    }
}
