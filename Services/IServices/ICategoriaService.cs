using EconoMe_API.Models;

namespace EconoMe_API.Services.IServices
{
    public interface ICategoriaService
    {
        Task<IEnumerable<Categoria>> GetCategorias();
        Task<Categoria> GetCategoriaById(int categoriaID);
        Task<Categoria> CreateCategoria(Categoria categoria);
        Task<Categoria> UpdateCategoria(Categoria categoria);
        Task<bool> DeleteCategoria(int categoriaID);
    }
}