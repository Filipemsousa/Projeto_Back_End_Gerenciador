using Microsoft.EntityFrameworkCore;
using organiza_emprego.Data;
using organiza_emprego.Models;
using organiza_emprego.Services.Interfaces;

namespace organiza_emprego.Services
{
    public class CandidaturaService : ICandidaturaService
    {
        private readonly AppDbContext _context;

        public CandidaturaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Candidatura>> ListarPorUsuarioAsync(int usuarioId, string? empresa, string? vaga, DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Candidaturas.Where(c => c.UsuarioId == usuarioId);

            if (!string.IsNullOrEmpty(empresa)) query = query.Where(c => c.Empresa.Contains(empresa));
            if (!string.IsNullOrEmpty(vaga)) query = query.Where(c => c.Vaga.Contains(vaga));

            if (dataInicio.HasValue) query = query.Where(c => c.DataCandidatura >= dataInicio.Value);
            if (dataFim.HasValue)
            {
                var dataLimite = dataFim.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.DataCandidatura <= dataLimite);
            }

            return await query.ToListAsync();
        }

        public async Task<Candidatura?> BuscarPorIdAsync(int id, int usuarioId)
        {
            return await _context.Candidaturas.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
        }

        public async Task<Candidatura> CriarAsync(Candidatura candidatura, int usuarioId)
        {
            candidatura.UsuarioId = usuarioId;
            _context.Candidaturas.Add(candidatura);
            await _context.SaveChangesAsync();
            return candidatura;
        }

        public async Task<bool> AtualizarStatusAsync(int id, string novoStatus, int usuarioId)
        {
            // 💡 Buscamos direto pelo ID da candidatura. Se encontrar, fazemos a checagem do dono.
            var candidatura = await _context.Candidaturas.FirstOrDefaultAsync(c => c.Id == id);

            if (candidatura == null)
                return false;

            // Se o seu modelo usar "IdUsuario" em vez de "UsuarioId", altere a linha abaixo:
            // if (candidatura.IdUsuario != usuarioId) return false;
            if (candidatura.UsuarioId != usuarioId)
                return false;

            // Atualiza o campo exatamente com o texto vindo do front-end
            candidatura.Status = novoStatus;

            // Salva de fato no banco de dados
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> DeletarPorFiltroAsync(string? empresa, string? vaga, int usuarioId)
        {
            var query = _context.Candidaturas.Where(c => c.UsuarioId == usuarioId);

            if (!string.IsNullOrEmpty(empresa)) query = query.Where(c => c.Empresa == empresa);
            if (!string.IsNullOrEmpty(vaga)) query = query.Where(c => c.Vaga == vaga);

            var listaDeletar = await query.ToListAsync();
            if (listaDeletar.Count == 0) return 0;

            _context.Candidaturas.RemoveRange(listaDeletar);
            await _context.SaveChangesAsync();
            return listaDeletar.Count;
        }

        public Task<bool> AtualizarStatusPorFiltroAsync(string empresa, string vaga, string? novoStatus, int usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AtualizarStatusAsync(int id, object status, int usuarioId)
        {
            throw new NotImplementedException();
        }
    }
}