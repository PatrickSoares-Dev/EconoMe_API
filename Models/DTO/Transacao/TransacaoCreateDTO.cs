namespace EconoMe_API.Models.DTOs
{
    public class TransacaoCreateDTO
    {
        public int TransacaoID { get; set; }
        public int ContaID { get; set; }
        public int CategoriaID { get; set; }
        public string Tipo { get; set; } // 'Entrada' ou 'Despesa'
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime DataTransacao { get; set; } = DateTime.UtcNow;
    }
}