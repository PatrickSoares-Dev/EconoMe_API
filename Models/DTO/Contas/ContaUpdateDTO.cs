namespace EconoMe_API.DTOs
{
    public class ContaUpdateDTO
    {
        public string NomeConta { get; set; }
        public int? InstituicaoBancariaID { get; set; }
        public decimal Saldo { get; set; } = 0.00M;
    }
}