using EconoMe_API.DTOs;
using EconoMe_API.Models;
using EconoMe_API.Models.DTOs;
using EconoMe_API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace EconoMe_API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;
        private readonly IContaService _contaService;
        private readonly ICategoriaService _categoriaService;

        public TransacoesController(ITransacaoService transacaoService, IContaService contaService, ICategoriaService categoriaService)
        {
            _transacaoService = transacaoService;
            _contaService = contaService;
            _categoriaService = categoriaService;
        }

        private string GetFirebaseUID()
        {
            if (!HttpContext.Items.ContainsKey("FirebaseUID"))
            {
                throw new UnauthorizedAccessException("FirebaseUID não encontrado no contexto");
            }

            var firebaseUID = HttpContext.Items["FirebaseUID"]?.ToString();
            if (string.IsNullOrEmpty(firebaseUID))
            {
                throw new UnauthorizedAccessException("FirebaseUID é nulo ou vazio");
            }

            return firebaseUID;
        }

        /// <summary>
        /// Obtém todas as transações do usuário autenticado.
        /// </summary>
        /// <returns>Lista de transações do usuário.</returns>
        /// <response code="200">Transações recuperadas com sucesso.</response>
        /// <response code="401">Usuário não autorizado.</response>
        [HttpGet("usuario")]
        public async Task<IActionResult> GetTransacoesByUsuario()
        {
            try
            {
                var firebaseUID = GetFirebaseUID();
                var transacoes = await _transacaoService.GetTransacoesByFirebaseUID(firebaseUID);
                return Ok(new ApiResponse<IEnumerable<Transacao>>
                {
                    Status = 200,
                    Message = "Transações recuperadas com sucesso",
                    Data = transacoes
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = 401,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Obtém todas as transações de uma conta específica do usuário autenticado.
        /// </summary>
        /// <param name="contaID">ID da conta.</param>
        /// <returns>Lista de transações da conta.</returns>
        /// <response code="200">Transações recuperadas com sucesso.</response>
        /// <response code="401">Usuário não autorizado.</response>
        [HttpGet("conta/{contaID}")]
        public async Task<IActionResult> GetTransacoesByContaId(int contaID)
        {
            try
            {
                var firebaseUID = GetFirebaseUID();
                var transacoes = await _transacaoService.GetTransacoesByContaId(contaID);
                return Ok(new ApiResponse<IEnumerable<Transacao>>
                {
                    Status = 200,
                    Message = "Transações recuperadas com sucesso",
                    Data = transacoes
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = 401,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Obtém os detalhes de uma transação específica.
        /// </summary>
        /// <param name="id">ID da transação.</param>
        /// <returns>Detalhes da transação.</returns>
        /// <response code="200">Transação recuperada com sucesso.</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <response code="404">Transação não encontrada.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransacao(int id)
        {
            try
            {
                var firebaseUID = GetFirebaseUID();
                var transacao = await _transacaoService.GetTransacaoById(id);
                if (transacao == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Status = 404,
                        Message = "Transação não encontrada",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<Transacao>
                {
                    Status = 200,
                    Message = "Transação recuperada com sucesso",
                    Data = transacao
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = 401,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Cria uma nova transação.
        /// </summary>
        /// <param name="transacaoDTO">Dados da transação a ser criada.</param>
        /// <returns>Transação criada.</returns>
        /// <response code="201">Transação criada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="401">Usuário não autorizado.</response>
        [HttpPost]
        public async Task<IActionResult> CreateTransacao([FromBody] TransacaoCreateDTO transacaoDTO)
        {
            try
            {
                var firebaseUID = GetFirebaseUID();

                // Validar tipo de transação
                if (transacaoDTO.Tipo != "Despesa" && transacaoDTO.Tipo != "Entrada")
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Tipo de transação inválido. Deve ser 'Despesa' ou 'Entrada'.",
                        Data = null
                    });
                }

                // Buscar a Conta e Categoria com base nos IDs fornecidos
                var conta = await _contaService.GetContaById(transacaoDTO.ContaID);
                var categoria = await _categoriaService.GetCategoriaById(transacaoDTO.CategoriaID);

                if (conta == null || conta.FirebaseUID != firebaseUID)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Conta inválida ou não pertence ao usuário.",
                        Data = null
                    });
                }

                if (categoria == null || categoria.Tipo != transacaoDTO.Tipo)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Categoria inválida ou não corresponde ao tipo de transação.",
                        Data = null
                    });
                }

                // Mapear DTO para entidade Transacao
                var transacao = new Transacao
                {
                    ContaID = transacaoDTO.ContaID,
                    CategoriaID = transacaoDTO.CategoriaID,
                    Tipo = transacaoDTO.Tipo,
                    Valor = Math.Round(transacaoDTO.Valor, 2),
                    Descricao = transacaoDTO.Descricao,
                    DataTransacao = transacaoDTO.DataTransacao,
                    FirebaseUID = firebaseUID,
                    NomeCategoria = categoria.NomeCategoria,
                    Conta = conta,
                    Categoria = categoria
                };

                var createdTransacao = await _transacaoService.CreateTransacao(transacao);
                return CreatedAtAction(nameof(GetTransacao), new { id = createdTransacao.TransacaoID }, new ApiResponse<Transacao>
                {
                    Status = 201,
                    Message = "Transação criada com sucesso",
                    Data = createdTransacao
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = 401,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Atualiza uma transação existente.
        /// </summary>
        /// <param name="id">ID da transação a ser atualizada.</param>
        /// <param name="transacaoDTO">Dados atualizados da transação.</param>
        /// <returns>Transação atualizada.</returns>
        /// <response code="200">Transação atualizada com sucesso.</response>
        /// <response code="400">ID da transação não corresponde.</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <response code="404">Transação não encontrada ou não pertence ao usuário.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransacao(int id, [FromBody] TransacaoCreateDTO transacaoDTO)
        {
            try
            {
                var firebaseUID = GetFirebaseUID();
                if (id != transacaoDTO.TransacaoID)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "ID da transação não corresponde.",
                        Data = null
                    });
                }

                // Validar tipo de transação
                if (transacaoDTO.Tipo != "Despesa" && transacaoDTO.Tipo != "Entrada")
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Tipo de transação inválido. Deve ser 'Despesa' ou 'Entrada'.",
                        Data = null
                    });
                }

                // Buscar a Conta e Categoria com base nos IDs fornecidos
                var conta = await _contaService.GetContaById(transacaoDTO.ContaID);
                var categoria = await _categoriaService.GetCategoriaById(transacaoDTO.CategoriaID);

                if (conta == null || conta.FirebaseUID != firebaseUID)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Conta inválida ou não pertence ao usuário.",
                        Data = null
                    });
                }

                if (categoria == null || categoria.Tipo != transacaoDTO.Tipo)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Categoria inválida ou não corresponde ao tipo de transação.",
                        Data = null
                    });
                }

                // Mapear DTO para entidade Transacao
                var transacao = new Transacao
                {
                    TransacaoID = id,
                    ContaID = transacaoDTO.ContaID,
                    CategoriaID = transacaoDTO.CategoriaID,
                    Tipo = transacaoDTO.Tipo,
                    Valor = Math.Round(transacaoDTO.Valor, 2),
                    Descricao = transacaoDTO.Descricao,
                    DataTransacao = transacaoDTO.DataTransacao,
                    FirebaseUID = firebaseUID,
                    NomeCategoria = categoria.NomeCategoria,
                    Conta = conta,
                    Categoria = categoria
                };

                var updatedTransacao = await _transacaoService.UpdateTransacao(transacao);
                return Ok(new ApiResponse<Transacao>
                {
                    Status = 200,
                    Message = "Transação atualizada com sucesso",
                    Data = updatedTransacao
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = 401,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Exclui uma transação existente.
        /// </summary>
        /// <param name="id">ID da transação a ser excluída.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="204">Transação excluída com sucesso.</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <response code="404">Transação não encontrada ou não pertence ao usuário.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransacao(int id)
        {
            try
            {
                var firebaseUID = GetFirebaseUID();
                var transacao = await _transacaoService.GetTransacaoById(id);

                if (transacao == null || transacao.FirebaseUID != firebaseUID)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Status = 404,
                        Message = "Transação não encontrada ou não pertence ao usuário",
                        Data = null
                    });
                }

                var result = await _transacaoService.DeleteTransacao(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Status = 404,
                        Message = "Transação não encontrada",
                        Data = null
                    });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = 401,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Obtém os gastos por categoria do usuário autenticado.
        /// </summary>
        /// <returns>Gastos por categoria.</returns>
        /// <response code="200">Gastos por categoria recuperados com sucesso.</response>
        /// <response code="401">Usuário não autorizado.</response>        
        [HttpGet("usuario/gastos-por-categoria")]
        public async Task<IActionResult> GetGastosPorCategoria()
        {
            try
            {
                var firebaseUID = GetFirebaseUID();
                var gastosPorCategoria = await _transacaoService.GetGastosPorCategoria(firebaseUID);
                return Ok(new { success = true, data = gastosPorCategoria });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtém as entradas e saídas do usuário autenticado.
        /// </summary>
        /// <returns>Entradas e saídas.</returns>
        /// <response code="200">Entradas e saídas recuperadas com sucesso.</response>
        /// <response code="401">Usuário não autorizado.</response>
        [HttpGet("usuario/entradas-saidas")]
        public async Task<IActionResult> GetEntradasESaidas()
        {
            try
            {
                var firebaseUID = GetFirebaseUID();
                var entradasESaidas = await _transacaoService.GetEntradasESaidas(firebaseUID);
                return Ok(new ApiResponse<EntradasESaidasDTO>
                {
                    Status = 200,
                    Message = "Entradas e saídas recuperadas com sucesso",
                    Data = entradasESaidas
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = 401,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}