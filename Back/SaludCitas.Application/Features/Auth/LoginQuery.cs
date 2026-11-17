using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Common;

namespace SaludCitas.Application.Features.Auth;

// Consulta: solo lee datos y verifica credenciales, no modifica nada
public record LoginQuery(string Correo, string Clave) : IQuery<AuthResponse>;

public class LoginQueryHandler(
    IUsuarioRepository usuarios,
    IPasswordHasher hasher,
    IJwtTokenGenerator jwt) : IQueryHandler<LoginQuery, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginQuery query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Correo) || string.IsNullOrEmpty(query.Clave))
        {
            throw new BadRequestException("El correo y la contraseña son obligatorios");
        }

        var usuario = await usuarios.ObtenerPorCorreoAsync(query.Correo.Trim(), cancellationToken);

        // Mismo mensaje en ambos casos para no revelar si el correo existe
        if (usuario is null || !hasher.Verificar(query.Clave, usuario.ClaveHash))
        {
            throw new UnauthorizedException("Correo o contraseña incorrectos");
        }

        var paciente = await usuarios.ObtenerPacientePorUsuarioAsync(usuario.UsuarioId, cancellationToken);
        return AuthMapper.Crear(jwt.Generar(usuario, paciente), usuario, paciente);
    }
}
