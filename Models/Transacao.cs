namespace EconoMe_API.Models
{
    public class Transacao
    {
        public int TransacaoID { get; set; }
        public int ContaID { get; set; }
        public int CategoriaID { get; set; }
        public string Tipo { get; set; } // 'Entrada' ou 'Despesa'
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime DataTransacao { get; set; } = DateTime.UtcNow;
        public string FirebaseUID { get; set; } // Novo campo
        public string NomeCategoria { get; set; } // Novo campo

        public Conta Conta { get; set; }
        public Categoria Categoria { get; set; }
    }
}