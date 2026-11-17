using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Features.Catalogo;

namespace SaludCitas.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/catalogo")]
public class CatalogoController(
    IQueryHandler<ObtenerEspecialidadesQuery, IReadOnlyList<EspecialidadDto>> especialidadesHandler,
    IQueryHandler<ObtenerSedesQuery, IReadOnlyList<SedeDto>> sedesHandler,
    IQueryHandler<ObtenerMedicosQuery, IReadOnlyList<MedicoDto>> medicosHandler) : ControllerBase
{
    [HttpGet("especialidades")]
    public async Task<ActionResult<IReadOnlyList<EspecialidadDto>>> Especialidades(CancellationToken cancellationToken)
        => Ok(await especialidadesHandler.Handle(new ObtenerEspecialidadesQuery(), cancellationToken));

    [HttpGet("sedes")]
    public async Task<ActionResult<IReadOnlyList<SedeDto>>> Sedes(CancellationToken cancellationToken)
        => Ok(await sedesHandler.Handle(new ObtenerSedesQuery(), cancellationToken));

    [HttpGet("medicos")]
    public async Task<ActionResult<IReadOnlyList<MedicoDto>>> Medicos(
        [FromQuery] int? especialidadId, [FromQuery] int? sedeId, CancellationToken cancellationToken)
        => Ok(await medicosHandler.Handle(new ObtenerMedicosQuery(especialidadId, sedeId), cancellationToken));
}