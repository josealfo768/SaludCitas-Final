using SaludCitas.Domain.Entities;

namespace SaludCitas.Application.Abstractions;

public interface IMedicoRepository
{
    Task<IReadOnlyList<Medico>> ListarAsync(int? especialidadId, int? sedeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Medico>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<int> CrearAsync(Medico medico, CancellationToken cancellationToken = default);
    Task<bool> ActualizarAsync(Medico medico, CancellationToken cancellationToken = default);
    Task<bool> CambiarEstadoAsync(int medicoId, bool activo, CancellationToken cancellationToken = default);
}