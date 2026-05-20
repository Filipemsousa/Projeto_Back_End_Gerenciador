using organiza_emprego.DTOs;
using organiza_emprego.Models;

namespace organiza_emprego.Services.Interfaces
{
    public interface ICandidaturaService
    {
        Task<IEnumerable<Candidatura>> ListarPorUsuarioAsync(int usuarioId, string? empresa, string? vaga, DateTime? dataInicio, DateTime? dataFim);
        Task<Candidatura?> BuscarPorIdAsync(int id, int usuarioId);
        Task<Candidatura> CriarAsync(Candidatura candidatura, int usuarioId);
        Task<bool> AtualizarStatusAsync(int id, string novoStatus, int usuarioId);
        Task<int> DeletarPorFiltroAsync(string? empresa, string? vaga, int usuarioId);
        Task<bool> AtualizarStatusPorFiltroAsync(string empresa, string vaga, string? novoStatus, int usuarioId);
        Task<bool> AtualizarStatusAsync(int id, object status, int usuarioId);
    }
}