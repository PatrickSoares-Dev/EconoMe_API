using EconoMe_API.Models;

namespace EconoMe_API.Services.IServices
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuarioByFirebaseUID(string firebaseUID);
        Task<Usuario> CreateUsuario(Usuario usuario);
        Task<Usuario> UpdateUsuario(Usuario usuario);
        Task<bool> DeleteUsuario(int usuarioID);
    }
}