using System.ComponentModel.DataAnnotations;

namespace organiza_emprego.Controllers
{
    public class UsuarioCadastroDto
    {
        /// <summary>
        /// Nome completo do candidato
        /// </summary>
        /// <example>Filipe Menezes</example>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(50, MinimumLength = 3)]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Endereço de e-mail válido
        /// </summary>
        /// <example>filipe@exemplo.com</example>
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha de acesso (mínimo 6 caracteres)
        /// </summary>
        /// <example>Senha#123</example>
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(20, MinimumLength = 6)]
        public string Senha { get; set; } = string.Empty;
    }
}
