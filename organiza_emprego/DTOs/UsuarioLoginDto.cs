using System.ComponentModel.DataAnnotations;

namespace organiza_emprego.DTOs
{
    public class UsuarioLoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;
    }
}
