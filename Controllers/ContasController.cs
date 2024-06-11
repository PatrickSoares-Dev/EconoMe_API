using EconoMe_API.Models;
using EconoMe_API.Services.IServices;
using EconoMe_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EconoMe_API.Services;

namespace EconoMe_API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ContasController : ControllerBase
    {
        private readonly IContaService _contaService;
        private readonly ITransacaoService _transacaoService;
        private readonly IUsuarioService _usuarioService;

        public ContasController(IContaService contaService, ITransacaoService transacaoService, IUsuarioService usuarioService)
        {
            _contaService = contaService;
            _transacaoService = transacaoService;
            _usuarioService = usuarioService;
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
        /// Obtém todas as contas do usuário autenticado.
        /// </summary>
        /// <returns>Lista de contas do usuário.</returns>
        /// <response code="200">Contas recuperadas com sucesso.</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <response code="404">Nenhuma conta cadastrada.</response>
        [HttpGet]
        public async Task<IActionResult> GetContas()
        {
            try
            {
                var firebaseUID = GetFirebaseUID();
                var contas = await _contaService.GetContasByFirebaseUID(firebaseUID);
                if (contas == null || !contas.Any())
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Status = 404,
                        Message = "Nenhuma conta cadastrada",
                        Data = null
                    });
                }

                var contasDTO = contas.Select(c => new ContaDTO
                {
                    ContaID = c.ContaID,
                    NomeConta = c.NomeConta,
                    NomeInstituicaoBancaria = _contaService.GetNomeInstituicaoBancaria(c.InstituicaoBancariaID),
                    Saldo = c.Saldo,
                    DataCriacao = c.DataCriacao,
                    DataAtualizacao = c.DataAtualizacao
                });

                return Ok(new ApiResponse<IEnumerable<ContaDTO>>
                {
                    Status = 200,
                    Message = "Contas recuperadas com sucesso",
                    Data = contasDTO
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
        /// Obtém os detalhes de uma conta específica do usuário autenticado ou todas as contas detalhadas se o ID não for fornecido.
        /// </summary>
        /// <param name="id">ID da conta (opcional).</param>
        /// <returns>Detalhes da conta ou todas as contas detalhadas.</returns>
        /// <response code="200">Conta(s) detalhada(s) recuperada(s) com sucesso.</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <response code="404">Conta não encontrada ou não pertence ao usuário.</response>
        [HttpGet("detalhado/{id?}")]
        public async Task<IActionResult> GetContaDetalhada(int? id)
        {
            try
            {
                var firebaseUID = GetFirebaseUID();

                if (id == null)
                {
                    // Retornar todas as contas do usuário
                    var contas = await _contaService.GetContasByFirebaseUID(firebaseUID);
                    if (contas == null || !contas.Any())
                    {
                        return NotFound(new ApiResponse<string>
                        {
                            Status = 404,
                            Message = "Nenhuma conta cadastrada",
                            Data = null
                        });
                    }

                    var contasDTO = contas.Select(c => new ContaDTO
                    {
                        ContaID = c.ContaID,
                        NomeConta = c.NomeConta,
                        NomeInstituicaoBancaria = _contaService.GetNomeInstituicaoBancaria(c.InstituicaoBancariaID),
                        Saldo = c.Saldo,
                        DataCriacao = c.DataCriacao,
                        DataAtualizacao = c.DataAtualizacao
                    });

                    var saldoTotal = contas.Sum(c => c.Saldo);

                    return Ok(new ApiResponse<object>
                    {
                        Status = 200,
                        Message = "Contas recuperadas com sucesso",
                        Data = new
                        {
                            Contas = contasDTO,
                            SaldoTotal = saldoTotal
                        }
                    });
                }
                else
                {
                    // Retornar detalhes da conta específica
                    var conta = await _contaService.GetContaById(id.Value);
                    if (conta == null || conta.FirebaseUID != firebaseUID)
                    {
                        return NotFound(new ApiResponse<Conta>
                        {
                            Status = 404,
                            Message = "Conta não encontrada ou não pertence ao usuário",
                            Data = null
                        });
                    }

                    var usuario = await _usuarioService.GetUsuarioByFirebaseUID(firebaseUID);
                    var entradas = await _transacaoService.GetEntradasByContaId(id.Value);
                    var saidas = await _transacaoService.GetSaidasByContaId(id.Value);

                    var gastosPorCategoria = saidas
                        .GroupBy(s => s.Categoria.NomeCategoria)
                        .Select(g => new CategoriaGastoDTO
                        {
                            NomeCategoria = g.Key,
                            TotalGasto = g.Sum(s => s.Valor)
                        });

                    var contaDetalhadaDTO = new ContaDetalhadaDTO
                    {
                        ContaID = conta.ContaID,
                        NomeConta = conta.NomeConta,
                        NomeInstituicaoBancaria = _contaService.GetNomeInstituicaoBancaria(conta.InstituicaoBancariaID),
                        Saldo = conta.Saldo,
                        DataCriacao = conta.DataCriacao,
                        DataAtualizacao = conta.DataAtualizacao,
                        DonoConta = usuario.Nome,
                        Entradas = entradas.Select(e => new TransacaoDTO
                        {
                            TransacaoID = e.TransacaoID,
                            CategoriaID = e.CategoriaID,
                            NomeCategoria = e.Categoria.NomeCategoria,
                            Tipo = e.Tipo,
                            Valor = e.Valor,
                            Descricao = e.Descricao,
                            DataTransacao = e.DataTransacao
                        }),
                        Saidas = saidas.Select(s => new TransacaoDTO
                        {
                            TransacaoID = s.TransacaoID,
                            CategoriaID = s.CategoriaID,
                            NomeCategoria = s.Categoria.NomeCategoria,
                            Tipo = s.Tipo,
                            Valor = s.Valor,
                            Descricao = s.Descricao,
                            DataTransacao = s.DataTransacao
                        }),
                        GastosPorCategoria = gastosPorCategoria,
                        QuantidadeEntradas = entradas.Count(),
                        QuantidadeSaidas = saidas.Count(),
                        TotalEntradas = entradas.Sum(e => e.Valor),
                        TotalSaidas = saidas.Sum(s => s.Valor)
                    };

                    return Ok(new ApiResponse<ContaDetalhadaDTO>
                    {
                        Status = 200,
                        Message = "Conta detalhada recuperada com sucesso",
                        Data = contaDetalhadaDTO
                    });
                }
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
        /// Cria uma nova conta para o usuário autenticado.
        /// </summary>
        /// <param name="contaCreateDTO">Dados da conta a ser criada.</param>
        /// <returns>Conta criada.</returns>
        /// <response code="201">Conta criada com sucesso.</response>
        /// <response code="400">InstituicaoBancariaID é obrigatório.</response>
        /// <response code="401">Usuário não autorizado.</response>
        [HttpPost]
        public async Task<IActionResult> CreateConta([FromBody] ContaCreateDTO contaCreateDTO)
        {
            try
            {
                var firebaseUID = GetFirebaseUID();

                if (contaCreateDTO.InstituicaoBancariaID == 0)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "InstituicaoBancariaID é obrigatório",
                        Data = null
                    });
                }

                var conta = new Conta
                {
                    FirebaseUID = firebaseUID,
                    NomeConta = contaCreateDTO.NomeConta,
                    InstituicaoBancariaID = contaCreateDTO.InstituicaoBancariaID,
                    Saldo = contaCreateDTO.Saldo
                };

                var createdConta = await _contaService.CreateConta(conta);
                return CreatedAtAction(nameof(GetContas), new { id = createdConta.ContaID }, new ApiResponse<Conta>
                {
                    Status = 201,
                    Message = "Conta criada com sucesso",
                    Data = createdConta
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
        /// Atualiza uma conta existente do usuário autenticado.
        /// </summary>
        /// <param name="id">ID da conta a ser atualizada.</param>
        /// <param name="contaUpdateDTO">Dados atualizados da conta.</param>
        /// <returns>Conta atualizada.</returns>
        /// <response code="200">Conta atualizada com sucesso.</response>
        /// <response code="400">ID da conta não corresponde.</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <response code="404">Conta não encontrada ou não pertence ao usuário.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConta(int id, [FromBody] ContaUpdateDTO contaUpdateDTO)
        {
            try
            {
                var firebaseUID = GetFirebaseUID();

                var existingConta = await _contaService.GetContaById(id);
                if (existingConta == null || existingConta.FirebaseUID != firebaseUID)
                {
                    return NotFound(new ApiResponse<Conta>
                    {
                        Status = 404,
                        Message = "Conta não encontrada ou não pertence ao usuário",
                        Data = null
                    });
                }

                existingConta.NomeConta = contaUpdateDTO.NomeConta;
                existingConta.InstituicaoBancariaID = contaUpdateDTO.InstituicaoBancariaID ?? existingConta.InstituicaoBancariaID; // Manter valor anterior se não for passado
                existingConta.Saldo = contaUpdateDTO.Saldo;
                existingConta.DataAtualizacao = DateTime.UtcNow;

                var updatedConta = await _contaService.UpdateConta(existingConta);
                return Ok(new ApiResponse<Conta>
                {
                    Status = 200,
                    Message = "Conta atualizada com sucesso",
                    Data = updatedConta
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
        /// Exclui uma conta existente do usuário autenticado.
        /// </summary>
        /// <param name="id">ID da conta a ser excluída.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="204">Conta excluída com sucesso.</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <response code="404">Conta não encontrada ou não pertence ao usuário.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConta(int id)
        {
            try
            {
                var firebaseUID = GetFirebaseUID();
                var conta = await _contaService.GetContaById(id);
                if (conta == null || conta.FirebaseUID != firebaseUID)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Status = 404,
                        Message = "Conta não encontrada ou não pertence ao usuário",
                        Data = false
                    });
                }

                // Excluir todas as transações relacionadas à conta
                await _transacaoService.DeleteTransacoesByContaId(conta.ContaID);

                var result = await _contaService.DeleteConta(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Status = 404,
                        Message = "Conta não encontrada",
                        Data = false
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
    }
}