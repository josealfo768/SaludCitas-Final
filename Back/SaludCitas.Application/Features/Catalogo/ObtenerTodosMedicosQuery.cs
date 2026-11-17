using SaludCitas.Application.Abstractions;

namespace SaludCitas.Application.Features.Catalogo;

public record ObtenerTodosMedicosQuery() : IQuery<IReadOnlyList<MedicoAdminDto>>;

public record MedicoAdminDto(
    int MedicoId,
    int EspecialidadId,
    string Especialidad,
    string Nombres,
    string Apellidos,
    bool Activo,
    IReadOnlyList<int> SedeIds);

public class ObtenerTodosMedicosHandler(IMedicoRepository repositorio)
    : IQueryHandler<ObtenerTodosMedicosQuery, IReadOnlyList<MedicoAdminDto>>
{
    public async Task<IReadOnlyList<MedicoAdminDto>> Handle(ObtenerTodosMedicosQuery query, CancellationToken cancellationToken = default)
    {
        var medicos = await repositorio.ListarTodosAsync(cancellationToken);
        return medicos
            .Select(m => new MedicoAdminDto(m.MedicoId, m.EspecialidadId, m.Especialidad, m.Nombres, m.Apellidos, m.Activo, m.SedeIds))
            .ToList();
    }
}