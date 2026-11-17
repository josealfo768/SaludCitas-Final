using SaludCitas.Domain.Entities;

namespace SaludCitas.Application.Abstractions;

public interface ICitaRepository
{
    Task<int?> ObtenerPacienteIdPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DateTime>> ListarOcupadasAsync(int medicoId, DateTime fecha, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CitaDetalle>> ListarPorPacienteAsync(int pacienteId, CancellationToken cancellationToken = default);
    Task<bool> ExisteCitaProgramadaAsync(int medicoId, DateTime fechaHora, CancellationToken cancellationToken = default);
    Task<int> CrearAsync(int pacienteId, int medicoId, int sedeId, DateTime fechaHora, CancellationToken cancellationToken = default);
    Task<Cita?> ObtenerPorIdAsync(int citaId, CancellationToken cancellationToken = default);
    Task CancelarAsync(int citaId, CancellationToken cancellationToken = default);
}

public record CitaDetalle(
    int CitaId,
    int MedicoId,
    string Medico,
    string Especialidad,
    int SedeId,
    string Sede,
    DateTime FechaHora,
    string Estado);