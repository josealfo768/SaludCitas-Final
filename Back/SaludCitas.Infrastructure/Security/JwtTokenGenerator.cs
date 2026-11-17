using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SaludCitas.Application.Abstractions;
using SaludCitas.Domain.Entities;

namespace SaludCitas.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(JwtSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Clave) || settings.Clave.Length < 32)
        {
            throw new InvalidOperationException("La clave JWT debe tener al menos 32 caracteres");
        }

        _settings = settings;
    }

    public string Generar(Usuario usuario, Paciente? paciente)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Correo),
            new("role", usuario.Rol)
        };

        if (paciente is not null)
        {
            claims.Add(new Claim("pacienteId", paciente.PacienteId.ToString()));
        }

        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Clave));
        var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Emisor,
            audience: _settings.Audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.MinutosExpiracion),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}