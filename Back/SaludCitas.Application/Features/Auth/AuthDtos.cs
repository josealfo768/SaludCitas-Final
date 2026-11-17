using SaludCitas.Domain.Entities;

namespace SaludCitas.Application.Features.Auth;

public record UsuarioDto(int UsuarioId, string Correo, string Rol);

public record PacienteDto(int PacienteId, int UsuarioId, string Nombres, string Apellidos, string Documento, string Telefono);

public record AuthResponse(string Token, UsuarioDto Usuario, PacienteDto? Paciente);

internal static class AuthMapper
{
    public static AuthResponse Crear(string token, Usuario usuario, Paciente? paciente)
    {
        var pacienteDto = paciente is null
            ? null
            : new PacienteDto(paciente.PacienteId, paciente.UsuarioId, paciente.Nombres,
                paciente.Apellidos, paciente.Documento, paciente.Telefono);

        return new AuthResponse(token, new UsuarioDto(usuario.UsuarioId, usuario.Correo, usuario.Rol), pacienteDto);
    }
}
