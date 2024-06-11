namespace EconoMe_API.Models
{
    public class InstituicaoBancaria
    {
        public int InstituicaoBancariaID { get; set; }
        public string Nome { get; set; }

        
        public ICollection<Conta> Contas { get; set; }
    }
}