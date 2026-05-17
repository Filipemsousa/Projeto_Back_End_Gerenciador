using System.ComponentModel.DataAnnotations;

namespace organiza_emprego.Models
{
    public class Candidatura
    {
        public int Id { get; set; }

        [Required]
        public string Empresa { get; set; } = string.Empty;

        [Required]
        public string Vaga { get; set; } = string.Empty;

        public string? LinkVaga { get; set; }

        public DateTime DataCandidatura { get; set; } = DateTime.Now;

        // Status: "Aplicado", "Entrevista", "Teste Técnico", "Aprovado", "Recusado"
        [Required]
        public string Status { get; set; } = "Aplicado";

        public string? Observacoes { get; set; }
    }
}
