using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EconoMe_API.Migrations
{
    public partial class AddInstituicaoBancaria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstituicaoBancariaID",
                table: "Contas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InstituicoesBancarias",
                columns: table => new
                {
                    InstituicaoBancariaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstituicoesBancarias", x => x.InstituicaoBancariaID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "InstituicoesBancarias",
                columns: new[] { "InstituicaoBancariaID", "Nome" },
                values: new object[,]
                {
                    { 1, "Itau" },
                    { 2, "Nubank" },
                    { 3, "Banco do Brasil" },
                    { 4, "Santander" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contas_InstituicaoBancariaID",
                table: "Contas",
                column: "InstituicaoBancariaID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contas_InstituicoesBancarias_InstituicaoBancariaID",
                table: "Contas",
                column: "InstituicaoBancariaID",
                principalTable: "InstituicoesBancarias",
                principalColumn: "InstituicaoBancariaID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contas_InstituicoesBancarias_InstituicaoBancariaID",
                table: "Contas");

            migrationBuilder.DropTable(
                name: "InstituicoesBancarias");

            migrationBuilder.DropIndex(
                name: "IX_Contas_InstituicaoBancariaID",
                table: "Contas");

            migrationBuilder.DropColumn(
                name: "InstituicaoBancariaID",
                table: "Contas");
        }
    }
}
