using EconoMe_API.Models;
using EconoMe_API.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace EconoMe_API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Obtém um usuário específico pelo FirebaseUID.
        /// </summary>
        /// <param name="firebaseUID">FirebaseUID do usuário.</param>
        /// <returns>Dados do usuário.</returns>
        /// <response code="200">Usuário encontrado com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("{firebaseUID}")]
        public async Task<IActionResult> GetUsuario(string firebaseUID)
        {
            var usuario = await _usuarioService.GetUsuarioByFirebaseUID(firebaseUID);
            if (usuario == null)
            {
                return NotFound(new ApiResponse<Usuario>
                {
                    Status = 404,
                    Message = "Usuário não encontrado",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Usuario>
            {
                Status = 200,
                Message = "Usuário encontrado com sucesso",
                Data = usuario
            });
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="usuario">Dados do usuário a ser criado.</param>
        /// <returns>Usuário criado.</returns>
        /// <response code="201">Usuário criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        public async Task<IActionResult> CreateUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Usuario>
                {
                    Status = 400,
                    Message = "Dados inválidos",
                    Data = null
                });
            }

            var createdUsuario = await _usuarioService.CreateUsuario(usuario);
            return CreatedAtAction(nameof(GetUsuario), new { firebaseUID = createdUsuario.FirebaseUID }, new ApiResponse<Usuario>
            {
                Status = 201,
                Message = "Usuário criado com sucesso",
                Data = createdUsuario
            });
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        /// <param name="id">ID do usuário a ser atualizado.</param>
        /// <param name="usuario">Dados atualizados do usuário.</param>
        /// <returns>Usuário atualizado.</returns>
        /// <response code="200">Usuário atualizado com sucesso.</response>
        /// <response code="400">ID do usuário não corresponde.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.UsuarioID)
            {
                return BadRequest(new ApiResponse<Usuario>
                {
                    Status = 400,
                    Message = "ID do usuário não corresponde",
                    Data = null
                });
            }

            var updatedUsuario = await _usuarioService.UpdateUsuario(usuario);
            if (updatedUsuario == null)
            {
                return NotFound(new ApiResponse<Usuario>
                {
                    Status = 404,
                    Message = "Usuário não encontrado",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Usuario>
            {
                Status = 200,
                Message = "Usuário atualizado com sucesso",
                Data = updatedUsuario
            });
        }

        /// <summary>
        /// Exclui um usuário existente.
        /// </summary>
        /// <param name="id">ID do usuário a ser excluído.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="204">Usuário excluído com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var result = await _usuarioService.DeleteUsuario(id);
            if (!result)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Status = 404,
                    Message = "Usuário não encontrado",
                    Data = false
                });
            }

            return NoContent();
        }
    }
}