using SaludCitas.Domain.Entities;

namespace SaludCitas.Application.Abstractions;

public interface ISedeRepository
{
    Task<IReadOnlyList<Sede>> ListarAsync(CancellationToken cancellationToken = default);
    Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken cancellationToken = default);
    Task<bool> TieneMedicosAsync(int sedeId, CancellationToken cancellationToken = default);
    Task<int> CrearAsync(string nombre, string direccion, CancellationToken cancellationToken = default);
    Task<bool> ActualizarAsync(int sedeId, string nombre, string direccion, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int sedeId, CancellationToken cancellationToken = default);
}