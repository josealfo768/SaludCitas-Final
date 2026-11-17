using SaludCitas.Domain.Entities;

namespace SaludCitas.Application.Abstractions;

public interface IEspecialidadRepository
{
    Task<IReadOnlyList<Especialidad>> ListarAsync(CancellationToken cancellationToken = default);
    Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken cancellationToken = default);
    Task<bool> TieneMedicosAsync(int especialidadId, CancellationToken cancellationToken = default);
    Task<int> CrearAsync(string nombre, CancellationToken cancellationToken = default);
    Task<bool> ActualizarAsync(int especialidadId, string nombre, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int especialidadId, CancellationToken cancellationToken = default);
}