using System.Text.RegularExpressions;
using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Common;
using SaludCitas.Domain.Constants;
using SaludCitas.Domain.Entities;

namespace SaludCitas.Application.Features.Auth;

// Comando: escribe datos (crea el usuario y el paciente)
public record RegistrarPacienteCommand(
    string Nombres,
    string Apellidos,
    string Documento,
    string Telefono,
    string Correo,
    string Clave) : ICommand<AuthResponse>;

public class RegistrarPacienteCommandHandler(
    IUsuarioRepository usuarios,
    IPasswordHasher hasher,
    IJwtTokenGenerator jwt) : ICommandHandler<RegistrarPacienteCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegistrarPacienteCommand command, CancellationToken cancellationToken = default)
    {
        Validar(command);

        var correo = command.Correo.Trim().ToLowerInvariant();
        var documento = command.Documento.Trim();

        if (await usuarios.ExisteCorreoAsync(correo, cancellationToken))
        {
            throw new ConflictException("Ya existe una cuenta con ese correo");
        }

        if (await usuarios.ExisteDocumentoAsync(documento, cancellationToken))
        {
            throw new ConflictException("Ya existe un paciente con ese documento");
        }

        var usuario = new Usuario
        {
            Correo = correo,
            ClaveHash = hasher.Hash(command.Clave),
            Rol = Roles.Paciente
        };

        var paciente = new Paciente
        {
            Nombres = command.Nombres.Trim(),
            Apellidos = command.Apellidos.Trim(),
            Documento = documento,
            Telefono = command.Telefono.Trim()
        };

        var (usuarioCreado, pacienteCreado) =
            await usuarios.RegistrarPacienteAsync(usuario, paciente, cancellationToken);

        return AuthMapper.Crear(jwt.Generar(usuarioCreado, pacienteCreado), usuarioCreado, pacienteCreado);
    }

    private static void Validar(RegistrarPacienteCommand c)
    {
        if (string.IsNullOrWhiteSpace(c.Nombres) || c.Nombres.Trim().Length < 2)
            throw new BadRequestException("Los nombres son obligatorios (mínimo 2 caracteres)");

        if (string.IsNullOrWhiteSpace(c.Apellidos) || c.Apellidos.Trim().Length < 2)
            throw new BadRequestException("Los apellidos son obligatorios (mínimo 2 caracteres)");

        if (!Regex.IsMatch(c.Documento ?? string.Empty, @"^\d{6,12}$"))
            throw new BadRequestException("El documento debe tener entre 6 y 12 dígitos");

        if (!Regex.IsMatch(c.Telefono ?? string.Empty, @"^\d{10}$"))
            throw new BadRequestException("El teléfono debe tener 10 dígitos");

        if (!Regex.IsMatch(c.Correo ?? string.Empty, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new BadRequestException("El correo no es válido");

        if (string.IsNullOrEmpty(c.Clave) || c.Clave.Length < 6)
            throw new BadRequestException("La contraseña debe tener mínimo 6 caracteres");
    }
}