namespace EconoMe_API.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string FirebaseUID { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}