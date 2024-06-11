using Microsoft.EntityFrameworkCore;
using EconoMe_API.Models;

namespace EconoMe_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<InstituicaoBancaria> InstituicoesBancarias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar a precisão decimal para o campo Saldo
            modelBuilder.Entity<Conta>()
                .Property(c => c.Saldo)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Transacao>()
                .Property(c => c.Valor)
                .HasColumnType("decimal(18, 2)");

            // Seed data para instituições bancárias
            modelBuilder.Entity<InstituicaoBancaria>().HasData(
                new InstituicaoBancaria { InstituicaoBancariaID = 1, Nome = "Itau" },
                new InstituicaoBancaria { InstituicaoBancariaID = 2, Nome = "Nubank" },
                new InstituicaoBancaria { InstituicaoBancariaID = 3, Nome = "Banco do Brasil" },
                new InstituicaoBancaria { InstituicaoBancariaID = 4, Nome = "Santander" }
            );
        }
    }
}