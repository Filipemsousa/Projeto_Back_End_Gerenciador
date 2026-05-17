using organiza_emprego.Data;
using organiza_emprego.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

        // GET: api/Candidaturas
        // Exemplo de uso: api/Candidaturas?empresa=Google&vaga=Developer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidatura>>> GetCandidaturas(
            [FromQuery] string? empresa,
            [FromQuery] string? vaga,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            
            var query = _context.Candidaturas.AsQueryable();

            // Filtro por Empresa (IgnoreCase e busca parcial usando Contains)
            if (!string.IsNullOrEmpty(empresa))
            {
                query = query.Where(c => c.Empresa.Contains(empresa));
            }

            // Filtro por Vaga (Busca parcial)
            if (!string.IsNullOrEmpty(vaga))
            {
                query = query.Where(c => c.Vaga.Contains(vaga));
            }

            // Filtro por Data Inicial (Candidaturas a partir de determinada data)
            if (dataInicio.HasValue)
            {
                query = query.Where(c => c.DataCandidatura >= dataInicio.Value);
            }

            // Filtro por Data Final (Candidaturas até determinada data)
            if (dataFim.HasValue)
            {
                // Garante que pegará o dia completo até as 23:59:59 caso venha apenas a data
                var dataLimite = dataFim.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.DataCandidatura <= dataLimite);
            }

            // Executa a query final filtrada no banco de dados
            return await query.ToListAsync();
        }

        // POST: api/Candidaturas (Criar nova candidatura)
        [HttpPost]
        public async Task<ActionResult<Candidatura>> PostCandidatura(Candidatura candidatura)
        {
            _context.Candidaturas.Add(candidatura);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCandidaturas), new { id = candidatura.Id }, candidatura);
        }

        // PUT: api/Candidaturas/5 (Atualizar status ou dados)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCandidatura(int id, Candidatura candidatura)
        {
            if (id != candidatura.Id) return BadRequest("O ID informado não coincide.");

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

        // DELETE: api/Candidaturas/5 (Remover)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidatura(int id)
        {
            var candidatura = await _context.Candidaturas.FindAsync(id);
            if (candidatura == null) return NotFound();

            _context.Candidaturas.Remove(candidatura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
