using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using organiza_emprego.Models;
using organiza_emprego.Services.Interfaces;

namespace organiza_emprego.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CandidaturasController : ControllerBase
    {
        private readonly ICandidaturaService _candidaturaService;

        public CandidaturasController(ICandidaturaService candidaturaService)
        {
            _candidaturaService = candidaturaService; // Injeta apenas o Service aqui
        }

        // Método auxiliar para ler o ID do Token de forma limpa
        private int GetUsuarioIdLogado()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(claim) ? 0 : int.Parse(claim);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? empresa, [FromQuery] string? vaga, [FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
        {
            int usuarioId = GetUsuarioIdLogado();
            var resultados = await _candidaturaService.ListarPorUsuarioAsync(usuarioId, empresa, vaga, dataInicio, dataFim);
            return Ok(resultados);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Candidatura candidatura)
        {
            int usuarioId = GetUsuarioIdLogado();
            var novaCandidatura = await _candidaturaService.CriarAsync(candidatura, usuarioId);
            return CreatedAtAction(nameof(Get), new { id = novaCandidatura.Id }, novaCandidatura);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Candidatura candidatura)
        {
            if (id != candidatura.Id) return BadRequest("O ID informado não coincide.");

            int usuarioId = GetUsuarioIdLogado();
            bool atualizou = await _candidaturaService.AtualizarAsync(id, candidatura, usuarioId);

            if (!atualizou) return Forbid("Operação negada ou registro inexistente.");
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string? empresa, [FromQuery] string? vaga)
        {
            if (string.IsNullOrEmpty(empresa) && string.IsNullOrEmpty(vaga))
                return BadRequest("Informe pelo menos a empresa ou a vaga.");

            int usuarioId = GetUsuarioIdLogado();
            int totalDeletado = await _candidaturaService.DeletarPorFiltroAsync(empresa, vaga, usuarioId);

            if (totalDeletado == 0) return NotFound("Nenhuma candidatura correspondente foi encontrada.");
            return Ok(new { mensagem = $"{totalDeletado} candidatura(s) removida(s) com sucesso!" });
        }
    }
}