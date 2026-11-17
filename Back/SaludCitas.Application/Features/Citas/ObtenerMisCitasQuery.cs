using SaludCitas.Application.Abstractions;

namespace SaludCitas.Application.Features.Citas;

public record CitaDto(
    int CitaId,
    int MedicoId,
    string Medico,
    string Especialidad,
    int SedeId,
    string Sede,
    DateTime FechaHora,
    string Estado);

public record ObtenerMisCitasQuery(int UsuarioId) : IQuery<IReadOnlyList<CitaDto>>;

public class ObtenerMisCitasHandler(ICitaRepository repositorio)
    : IQueryHandler<ObtenerMisCitasQuery, IReadOnlyList<CitaDto>>
{
    public async Task<IReadOnlyList<CitaDto>> Handle(ObtenerMisCitasQuery query, CancellationToken cancellationToken = default)
    {
        var pacienteId = await repositorio.ObtenerPacienteIdPorUsuarioAsync(query.UsuarioId, cancellationToken);
        if (pacienteId is null)
            return [];

        var citas = await repositorio.ListarPorPacienteAsync(pacienteId.Value, cancellationToken);
        return citas
            .Select(c => new CitaDto(c.CitaId, c.MedicoId, c.Medico, c.Especialidad, c.SedeId, c.Sede, c.FechaHora, c.Estado))
            .ToList();
    }
}
