using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using organiza_emprego.Data;
using organiza_emprego.Models;
using System.Security.Claims;

namespace organiza_emprego.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CandidaturasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CandidaturasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Candidaturas (Listar apenas as candidaturas do usuário logado)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidatura>>> GetCandidaturas(
            [FromQuery] string? empresa,
            [FromQuery] string? vaga,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            // 1. Extrai o ID do usuário que está dentro do Token JWT recebido
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized("Usuário não identificado.");

            int usuarioId = int.Parse(usuarioIdClaim);

            // 2. Filtra a query inicial para trazer APENAS as vagas onde o UsuarioId seja igual ao do token
            var query = _context.Candidaturas.Where(c => c.UsuarioId == usuarioId);

            // 3. Aplica os outros filtros de pesquisa normalmente por cima do filtro do usuário
            if (!string.IsNullOrEmpty(empresa))
            {
                query = query.Where(c => c.Empresa.Contains(empresa));
            }

            if (!string.IsNullOrEmpty(vaga))
            {
                query = query.Where(c => c.Vaga.Contains(vaga));
            }

            if (dataInicio.HasValue)
            {
                query = query.Where(c => c.DataCandidatura >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                var dataLimite = dataFim.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.DataCandidatura <= dataLimite);
            }

            return await query.ToListAsync();
        }

        // POST: api/Candidaturas (Vincular a nova vaga automaticamente ao usuário logado)
        [HttpPost]
        public async Task<ActionResult<Candidatura>> PostCandidatura(Candidatura candidatura)
        {
            // 1. Extrai o ID do usuário logado através do Token
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized("Usuário não identificado.");

            // 2. Associa a candidatura ao usuário antes de salvar no banco
            candidatura.UsuarioId = int.Parse(usuarioIdClaim);

            _context.Candidaturas.Add(candidatura);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCandidaturas", new { id = candidatura.Id }, candidatura);
        }

        // DICA: Você deve aplicar a mesma verificação nos métodos PUT e DELETE 
        // para garantir que o usuário não mude ou delete o ID de uma vaga que pertence a outro!






        // PUT: api/Candidaturas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCandidatura(int id, Candidatura candidatura)
        {
            if (id != candidatura.Id) return BadRequest("O ID informado não coincide.");

            // 1. Extrai o ID do usuário logado através do Token JWT
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized("Usuário não identificado.");
            int usuarioId = int.Parse(usuarioIdClaim);

            // 2. Busca a candidatura original direto no banco (sem rastrear) para verificar o verdadeiro dono
            var candidaturaBanco = await _context.Candidaturas
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidaturaBanco == null) return NotFound("Candidatura não encontrada.");

            // 🔒 3. Bloqueio de segurança: Se o UsuarioId do banco for diferente do Token, barra a operação
            if (candidaturaBanco.UsuarioId != usuarioId)
            {
                return Forbid("Você não tem permissão para alterar esta candidatura.");
            }

            // 4. Garante que o ID do usuário não seja alterado na requisição
            candidatura.UsuarioId = usuarioId;

            _context.Entry(candidatura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Candidaturas.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }





        // DELETE: api/Candidaturas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidatura(int id)
        {
            // 1. Extrai o ID do usuário logado através do Token JWT
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized("Usuário não identificado.");
            int usuarioId = int.Parse(usuarioIdClaim);

            // 2. Busca a candidatura no banco de dados
            var candidatura = await _context.Candidaturas.FindAsync(id);
            if (candidatura == null) return NotFound("Candidatura não encontrada.");

            // 🔒 3. Bloqueio de segurança: Verifica se o usuário logado é realmente o dono do registro
            if (candidatura.UsuarioId != usuarioId)
            {
                return Forbid("Você não tem permissão para deletar esta candidatura.");
            }

            // 4. Se passou pela validação, remove do banco
            _context.Candidaturas.Remove(candidatura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
