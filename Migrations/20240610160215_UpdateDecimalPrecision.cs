using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EconoMe_API.Migrations
{
    public partial class UpdateDecimalPrecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contas_InstituicoesBancarias_InstituicaoBancariaID",
                table: "Contas");

            migrationBuilder.DropIndex(
                name: "IX_Contas_InstituicaoBancariaID",
                table: "Contas");

            migrationBuilder.AlterColumn<decimal>(
                name: "Saldo",
                table: "Contas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Saldo",
                table: "Contas",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

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
    }
}
