using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EconoMe_API.Migrations
{
    public partial class AddDefaultCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Inserir categorias de entrada
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Salário', 'Entrada')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Bônus', 'Entrada')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Investimentos', 'Entrada')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Reembolso', 'Entrada')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Outros', 'Entrada')");

            // Inserir categorias de despesa
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Alimentação', 'Despesa')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Transporte', 'Despesa')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Moradia', 'Despesa')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Educação', 'Despesa')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Saúde', 'Despesa')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Lazer', 'Despesa')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Roupas', 'Despesa')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Eletrônicos', 'Despesa')");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Contas e Serviços', 'Despesa') ");
            migrationBuilder.Sql("INSERT INTO Categorias (NomeCategoria, Tipo) VALUES ('Outros', 'Despesa')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover categorias de entrada
            migrationBuilder.Sql("DELETE FROM Categorias WHERE NomeCategoria IN ('Salário', 'Bônus', 'Investimentos', 'Reembolso', 'Outros') AND Tipo = 'Entrada'");

            // Remover categorias de despesa
            migrationBuilder.Sql("DELETE FROM Categorias WHERE NomeCategoria IN ('Alimentação', 'Transporte', 'Moradia', 'Educação', 'Saúde', 'Lazer', 'Roupas', 'Eletrônicos', 'Contas e Serviços', 'Outros') AND Tipo = 'Despesa'");
        }
    }
}
