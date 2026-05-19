using organiza_emprego.DTOs;
using organiza_emprego.Models;

namespace organiza_emprego.Services.Interfaces
{
    public interface ICandidaturaService
    {
        Task<IEnumerable<Candidatura>> ListarPorUsuarioAsync(int usuarioId, string? empresa, string? vaga, DateTime? dataInicio, DateTime? dataFim);
        Task<Candidatura?> BuscarPorIdAsync(int id, int usuarioId);
        Task<Candidatura> CriarAsync(Candidatura candidatura, int usuarioId);
        Task<bool> AtualizarAsync(int id, Candidatura candidatura, int usuarioId);
        Task<int> DeletarPorFiltroAsync(string? empresa, string? vaga, int usuarioId);
    }
}