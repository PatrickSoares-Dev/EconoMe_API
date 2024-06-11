using EconoMe_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EconoMe_API.Services.IServices
{
    public interface IContaService
    {
        Task<IEnumerable<Conta>> GetContasByFirebaseUID(string firebaseUID);
        Task<Conta> GetContaById(int id);
        Task<Conta> CreateConta(Conta conta);
        Task<Conta> UpdateConta(Conta conta);
        Task<bool> DeleteConta(int id);
        string GetNomeInstituicaoBancaria(int instituicaoBancariaID);
    }
}