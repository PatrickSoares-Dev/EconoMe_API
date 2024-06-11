using System.ComponentModel.DataAnnotations.Schema;

namespace EconoMe_API.Models
{
    public class Conta
    {
        public int ContaID { get; set; }
        public string FirebaseUID { get; set; }
        public string NomeConta { get; set; }

        public int InstituicaoBancariaID { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Saldo { get; set; } = 0.00M;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
        
        public InstituicaoBancaria InstituicaoBancaria { get; set; }
    }
}