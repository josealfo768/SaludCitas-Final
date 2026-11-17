using SaludCitas.Domain.Entities;

namespace SaludCitas.Application.Abstractions;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default);
    Task<Paciente?> ObtenerPacientePorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default);
    Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken = default);
    Task<bool> ExisteDocumentoAsync(string documento, CancellationToken cancellationToken = default);
    Task<(Usuario Usuario, Paciente Paciente)> RegistrarPacienteAsync(Usuario usuario, Paciente paciente, CancellationToken cancellationToken = default);
}
