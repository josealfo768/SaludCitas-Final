using SaludCitas.Application.Abstractions;

namespace SaludCitas.Application.Features.Catalogo;

public record ObtenerSedesQuery() : IQuery<IReadOnlyList<SedeDto>>;

public class ObtenerSedesHandler(ISedeRepository repositorio)
    : IQueryHandler<ObtenerSedesQuery, IReadOnlyList<SedeDto>>
{
    public async Task<IReadOnlyList<SedeDto>> Handle(ObtenerSedesQuery query, CancellationToken cancellationToken)
    {
        var sedes = await repositorio.ListarAsync(cancellationToken);
        return sedes.Select(s => new SedeDto(s.SedeId, s.Nombre, s.Direccion)).ToList();
    }
}