using EconoMe_API.Models;
using EconoMe_API.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace EconoMe_API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>
        /// Obtém todas as categorias.
        /// </summary>
        /// <returns>Lista de categorias.</returns>
        /// <response code="200">Categorias encontradas com sucesso.</response>
        /// <response code="404">Nenhuma categoria encontrada.</response>
        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _categoriaService.GetCategorias();
            if (categorias == null || !categorias.Any())
            {
                return NotFound(new ApiResponse<IEnumerable<Categoria>>
                {
                    Status = 404,
                    Message = "Nenhuma categoria encontrada",
                    Data = null
                });
            }

            return Ok(new ApiResponse<IEnumerable<Categoria>>
            {
                Status = 200,
                Message = "Categorias encontradas com sucesso",
                Data = categorias
            });
        }

        /// <summary>
        /// Obtém uma categoria específica pelo ID.
        /// </summary>
        /// <param name="id">ID da categoria.</param>
        /// <returns>Dados da categoria.</returns>
        /// <response code="200">Categoria encontrada com sucesso.</response>
        /// <response code="404">Categoria não encontrada.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoria(int id)
        {
            var categoria = await _categoriaService.GetCategoriaById(id);
            if (categoria == null)
            {
                return NotFound(new ApiResponse<Categoria>
                {
                    Status = 404,
                    Message = "Categoria não encontrada",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Categoria>
            {
                Status = 200,
                Message = "Categoria encontrada com sucesso",
                Data = categoria
            });
        }

        /// <summary>
        /// Cria uma nova categoria.
        /// </summary>
        /// <param name="categoria">Dados da categoria a ser criada.</param>
        /// <returns>Categoria criada.</returns>
        /// <response code="201">Categoria criada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        public async Task<IActionResult> CreateCategoria([FromBody] Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Categoria>
                {
                    Status = 400,
                    Message = "Dados inválidos",
                    Data = null
                });
            }

            var createdCategoria = await _categoriaService.CreateCategoria(categoria);
            return CreatedAtAction(nameof(GetCategoria), new { id = createdCategoria.CategoriaID }, new ApiResponse<Categoria>
            {
                Status = 201,
                Message = "Categoria criada com sucesso",
                Data = createdCategoria
            });
        }

        /// <summary>
        /// Atualiza uma categoria existente.
        /// </summary>
        /// <param name="id">ID da categoria a ser atualizada.</param>
        /// <param name="categoria">Dados atualizados da categoria.</param>
        /// <returns>Categoria atualizada.</returns>
        /// <response code="200">Categoria atualizada com sucesso.</response>
        /// <response code="400">ID da categoria não corresponde.</response>
        /// <response code="404">Categoria não encontrada.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoria(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.CategoriaID)
            {
                return BadRequest(new ApiResponse<Categoria>
                {
                    Status = 400,
                    Message = "ID da categoria não corresponde",
                    Data = null
                });
            }

            var updatedCategoria = await _categoriaService.UpdateCategoria(categoria);
            if (updatedCategoria == null)
            {
                return NotFound(new ApiResponse<Categoria>
                {
                    Status = 404,
                    Message = "Categoria não encontrada",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Categoria>
            {
                Status = 200,
                Message = "Categoria atualizada com sucesso",
                Data = updatedCategoria
            });
        }

        /// <summary>
        /// Exclui uma categoria existente.
        /// </summary>
        /// <param name="id">ID da categoria a ser excluída.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="204">Categoria excluída com sucesso.</response>
        /// <response code="404">Categoria não encontrada.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var result = await _categoriaService.DeleteCategoria(id);
            if (!result)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Status = 404,
                    Message = "Categoria não encontrada",
                    Data = false
                });
            }

            return NoContent();
        }
    }
}