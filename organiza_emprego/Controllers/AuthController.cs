using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using organiza_emprego.Data;
using organiza_emprego.DTOs;
using organiza_emprego.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static organiza_emprego.DTOs.UsuarioDTO;


namespace organiza_emprego.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;

    public AuthController(IConfiguration config, AppDbContext db)
    {
        _config = config;
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UsuarioCadastroDto dto)
    {
        // Validação básica para evitar e-mails duplicados
        var emailExiste = await _db.Usuarios.AnyAsync(u => u.Email == dto.Email);
        if (emailExiste)
        {
            return BadRequest("Este e-mail já está cadastrado.");
        }

        // Instancia o novo usuário mapeando os campos corretos do modelo Usuario.cs
        Usuario usuario = new Usuario
        {
            Nome = dto.Nome,       // Mapeia o Nome do DTO para o Name do seu Model
            Email = dto.Email,
            SenhaHash = dto.Senha  // Salvando em texto limpo conforme planejado
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        // Retorna o DTO de resposta preenchendo a partir do modelo salvo
        var resposta = new UsuarioRespostaDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,   // Puxa direto a string do Name
            Email = usuario.Email,
            DataCriacao = usuario.DataCriacao
        };

        return CreatedAtAction(nameof(Register), new { id = usuario.Id }, resposta);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto)
    {
        // Busca o usuário comparando Email e Senha (usando instanciamento assíncrono para melhor performance)
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email && u.SenhaHash == dto.Senha);
        if (usuario == null) return Unauthorized("E-mail ou senha incorretos.");

        var token = GenerateToken(usuario);
        return Ok(new { token, id = usuario.Id, nome = usuario.Nome });
    }

    private string GenerateToken(Usuario usuario)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key");
        var issuer = jwtSection.GetValue<string>("Issuer");
        var audience = jwtSection.GetValue<string>("Audience");

        // Garante que se a chave não for encontrada no appsettings, a API avise o erro claramente
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("A chave JWT não foi configurada corretamente.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims adaptadas para o contexto do Usuário/Candidato
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, (string)usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8), 
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }



}
