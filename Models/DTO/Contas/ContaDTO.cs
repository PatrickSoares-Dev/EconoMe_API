namespace EconoMe_API.DTOs
{
    public class ContaDTO
    {
        public int ContaID { get; set; }
        public string NomeConta { get; set; }
        public string NomeInstituicaoBancaria { get; set; }
        public decimal Saldo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}