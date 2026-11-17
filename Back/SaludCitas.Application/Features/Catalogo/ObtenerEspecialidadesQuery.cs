using SaludCitas.Application.Abstractions;

namespace SaludCitas.Application.Features.Catalogo;

public record ObtenerEspecialidadesQuery() : IQuery<IReadOnlyList<EspecialidadDto>>;

public class ObtenerEspecialidadesQueryHandler(IEspecialidadRepository especialidades)
    : IQueryHandler<ObtenerEspecialidadesQuery, IReadOnlyList<EspecialidadDto>>
{
    public async Task<IReadOnlyList<EspecialidadDto>> Handle(
    ObtenerEspecialidadesQuery query, CancellationToken cancellationToken = default)
    {
        var lista = await especialidades.ListarAsync(cancellationToken);

        return lista.Select(e => new EspecialidadDto(e.EspecialidadId, e.Nombre)).ToList();
    }
}