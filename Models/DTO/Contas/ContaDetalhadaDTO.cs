namespace EconoMe_API.DTOs
{
    public class ContaDetalhadaDTO
    {
        public int ContaID { get; set; }
        public string NomeConta { get; set; }
        public string NomeInstituicaoBancaria { get; set; }
        public decimal Saldo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string DonoConta { get; set; }
        public IEnumerable<TransacaoDTO> Entradas { get; set; }
        public IEnumerable<TransacaoDTO> Saidas { get; set; }
        public IEnumerable<CategoriaGastoDTO> GastosPorCategoria { get; set; }
        public int QuantidadeEntradas { get; set; }
        public int QuantidadeSaidas { get; set; }
        public decimal TotalEntradas { get; set; }
        public decimal TotalSaidas { get; set; }
    }

    public class TransacaoDTO
    {
        public int TransacaoID { get; set; }
        public int CategoriaID { get; set; }
        public string NomeCategoria { get; set; }
        public string Tipo { get; set; } // 'Entrada' ou 'Despesa'
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime DataTransacao { get; set; }
    }

    public class CategoriaGastoDTO
    {
        public string NomeCategoria { get; set; }
        public decimal TotalGasto { get; set; }
    }
}