namespace EconoMe_API.Models
{
    public class Categoria
    {
        public int CategoriaID { get; set; }
        public string NomeCategoria { get; set; }
        public string Tipo { get; set; } // 'Entrada' ou 'Despesa'
    }
}