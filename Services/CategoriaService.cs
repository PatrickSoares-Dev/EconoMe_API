using EconoMe_API.Data;
using EconoMe_API.Models;
using EconoMe_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace EconoMe_API.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly DataContext _context;

        public CategoriaService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> GetCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }

        public async Task<Categoria> GetCategoriaById(int categoriaID)
        {
            return await _context.Categorias.FindAsync(categoriaID);
        }

        public async Task<Categoria> CreateCategoria(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<Categoria> UpdateCategoria(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<bool> DeleteCategoria(int categoriaID)
        {
            var categoria = await _context.Categorias.FindAsync(categoriaID);
            if (categoria == null) return false;

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}