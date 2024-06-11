using EconoMe_API.Data;
using EconoMe_API.Models;
using EconoMe_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace EconoMe_API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DataContext _context;

        public UsuarioService(DataContext context)
        {
            _context = context;
        }

        public async Task<Usuario> GetUsuarioByFirebaseUID(string firebaseUID)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUID);
        }

        public async Task<Usuario> CreateUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> UpdateUsuario(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> DeleteUsuario(int usuarioID)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioID);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}