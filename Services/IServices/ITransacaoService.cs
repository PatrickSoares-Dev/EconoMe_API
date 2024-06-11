using EconoMe_API.Models;
using EconoMe_API.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EconoMe_API.Services.IServices
{
    public interface ITransacaoService
    {
        Task<IEnumerable<Transacao>> GetTransacoesByFirebaseUID(string firebaseUID);
        Task<IEnumerable<Transacao>> GetTransacoesByContaId(int contaID);
        Task<Transacao> GetTransacaoById(int id);
        Task<IEnumerable<GastoPorCategoriaDTO>> GetGastosPorCategoria(string firebaseUID);
        Task<EntradasESaidasDTO> GetEntradasESaidas(string firebaseUID);
        Task<IEnumerable<Transacao>> GetEntradasByFirebaseUID(string firebaseUID);
        Task<IEnumerable<Transacao>> GetSaidasByFirebaseUID(string firebaseUID);
        Task<IEnumerable<Transacao>> GetEntradasByContaId(int contaID);
        Task<IEnumerable<Transacao>> GetSaidasByContaId(int contaID);
        Task<bool> DeleteTransacoesByContaId(int contaID);
        Task<Transacao> CreateTransacao(Transacao transacao);
        Task<Transacao> UpdateTransacao(Transacao transacao);
        Task<bool> DeleteTransacao(int id);

    }
}