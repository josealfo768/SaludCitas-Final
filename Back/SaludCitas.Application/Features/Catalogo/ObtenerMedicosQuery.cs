using SaludCitas.Application.Abstractions;

namespace SaludCitas.Application.Features.Catalogo;

public record ObtenerMedicosQuery(int? EspecialidadId, int? SedeId) : IQuery<IReadOnlyList<MedicoDto>>;
public class ObtenerMedicosHandler(IMedicoRepository repositorio)
    : IQueryHandler<ObtenerMedicosQuery, IReadOnlyList<MedicoDto>>
{
    public async Task<IReadOnlyList<MedicoDto>> Handle(ObtenerMedicosQuery query, CancellationToken cancellationToken)
    {
        var medicos = await repositorio.ListarAsync(query.EspecialidadId, query.SedeId, cancellationToken);
        return medicos
            .Select(m => new MedicoDto(m.MedicoId, m.EspecialidadId, m.Especialidad, m.Nombres, m.Apellidos, m.SedeIds))
            .ToList();
    }
}