using EconoMe_API.Data;
using EconoMe_API.Models;
using EconoMe_API.Models.DTOs;
using EconoMe_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EconoMe_API.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly DataContext _context;

        public TransacaoService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transacao>> GetTransacoesByFirebaseUID(string firebaseUID)
        {
            return await _context.Transacoes
                .Include(t => t.Conta)
                .Include(t => t.Categoria)
                .Where(t => t.Conta.FirebaseUID == firebaseUID)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> GetTransacoesByContaId(int contaID)
        {
            return await _context.Transacoes
                .Include(t => t.Conta)
                .Include(t => t.Categoria)
                .Where(t => t.ContaID == contaID)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> GetEntradasByFirebaseUID(string firebaseUID)
        {
            return await _context.Transacoes
                .Include(t => t.Categoria)
                .Where(t => t.Conta.FirebaseUID == firebaseUID && t.Tipo == "Entrada")
                .ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> GetSaidasByFirebaseUID(string firebaseUID)
        {
            return await _context.Transacoes
                .Include(t => t.Categoria)
                .Where(t => t.Conta.FirebaseUID == firebaseUID && t.Tipo == "Saida")
                .ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> GetEntradasByContaId(int contaID)
        {
            return await _context.Transacoes
                .Include(t => t.Categoria)
                .Where(t => t.ContaID == contaID && t.Tipo == "Entrada")
                .ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> GetSaidasByContaId(int contaID)
        {
            return await _context.Transacoes
                .Include(t => t.Categoria)
                .Where(t => t.ContaID == contaID && t.Tipo == "Saida")
                .ToListAsync();
        }

        public async Task<bool> DeleteTransacoesByContaId(int contaID)
        {
            var transacoes = await _context.Transacoes
                .Where(t => t.ContaID == contaID)
                .ToListAsync();

            if (transacoes == null || !transacoes.Any())
            {
                return false;
            }

            _context.Transacoes.RemoveRange(transacoes);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Transacao> GetTransacaoById(int id)
        {
            return await _context.Transacoes
                .Include(t => t.Conta)
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.TransacaoID == id);
        }

        public async Task<Transacao> CreateTransacao(Transacao transacao)
        {
            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();
            return transacao;
        }

        public async Task<Transacao> UpdateTransacao(Transacao transacao)
        {
            _context.Transacoes.Update(transacao);
            await _context.SaveChangesAsync();
            return transacao;
        }

        public async Task<bool> DeleteTransacao(int id)
        {
            var transacao = await _context.Transacoes.FindAsync(id);
            if (transacao == null)
            {
                return false;
            }

            _context.Transacoes.Remove(transacao);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<IEnumerable<GastoPorCategoriaDTO>> GetGastosPorCategoria(string firebaseUID)
        {
            return await _context.Transacoes
                .Where(t => t.Conta.FirebaseUID == firebaseUID && t.Tipo == "Despesa")
                .GroupBy(t => t.Categoria.NomeCategoria)
                .Select(g => new GastoPorCategoriaDTO
                {
                    Categoria = g.Key,
                    TotalGasto = g.Sum(t => t.Valor)
                })
                .ToListAsync();
        }

        public async Task<EntradasESaidasDTO> GetEntradasESaidas(string firebaseUID)
        {
            var entradas = await _context.Transacoes
                .Where(t => t.Conta.FirebaseUID == firebaseUID && t.Tipo == "Entrada")
                .SumAsync(t => t.Valor);

            var saidas = await _context.Transacoes
                .Where(t => t.Conta.FirebaseUID == firebaseUID && t.Tipo == "Despesa")
                .SumAsync(t => t.Valor);

            return new EntradasESaidasDTO
            {
                TotalEntradas = entradas,
                TotalSaidas = saidas
            };
        }


    }
        
}