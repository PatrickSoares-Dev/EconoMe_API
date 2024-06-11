using EconoMe_API.Models;
using EconoMe_API.Services.IServices;
using EconoMe_API.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EconoMe_API.Services
{
    public class ContaService : IContaService
    {
        private readonly DataContext _context;

        public ContaService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Conta>> GetContasByFirebaseUID(string firebaseUID)
        {
            return await _context.Contas.Where(c => c.FirebaseUID == firebaseUID).ToListAsync();
        }

        public async Task<Conta> GetContaById(int id)
        {
            return await _context.Contas.FindAsync(id);
        }

        public async Task<Conta> CreateConta(Conta conta)
        {
            _context.Contas.Add(conta);
            await _context.SaveChangesAsync();
            return conta;
        }

        public async Task<Conta> UpdateConta(Conta conta)
        {
            _context.Contas.Update(conta);
            await _context.SaveChangesAsync();
            return conta;
        }

        public async Task<bool> DeleteConta(int id)
        {
            var conta = await _context.Contas.FindAsync(id);
            if (conta == null)
            {
                return false;
            }

            _context.Contas.Remove(conta);
            await _context.SaveChangesAsync();
            return true;
        }

        public string GetNomeInstituicaoBancaria(int instituicaoBancariaID)
        {
            // Implementar lógica para obter o nome da instituição bancária pelo ID
            // Exemplo:
            var instituicao = _context.InstituicoesBancarias.Find(instituicaoBancariaID);
            return instituicao?.Nome ?? "Outros";
        }
    }
}