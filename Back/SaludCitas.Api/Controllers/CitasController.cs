using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Features.Citas;

namespace SaludCitas.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/citas")]
public class CitasController(
    IQueryHandler<ObtenerMisCitasQuery, IReadOnlyList<CitaDto>> misCitasHandler,
    IQueryHandler<ObtenerHorariosQuery, IReadOnlyList<DateTime>> horariosHandler,
    ICommandHandler<AgendarCitaCommand, int> agendarHandler,
    ICommandHandler<CancelarCitaCommand, bool> cancelarHandler) : ControllerBase
{
    private bool TryObtenerUsuarioId(out int usuarioId)
        => int.TryParse(User.FindFirst("sub")?.Value, out usuarioId);

    [HttpGet("mias")]
    public async Task<ActionResult<IReadOnlyList<CitaDto>>> Mias(CancellationToken cancellationToken)
    {
        if (!TryObtenerUsuarioId(out var usuarioId)) return Unauthorized();
        return Ok(await misCitasHandler.Handle(new ObtenerMisCitasQuery(usuarioId), cancellationToken));
    }

    [HttpGet("horarios")]
    public async Task<ActionResult<IReadOnlyList<DateTime>>> Horarios(
        [FromQuery] int medicoId, [FromQuery] DateTime fecha, CancellationToken cancellationToken)
        => Ok(await horariosHandler.Handle(new ObtenerHorariosQuery(medicoId, fecha), cancellationToken));

    public record AgendarRequest(int MedicoId, int SedeId, DateTime FechaHora);

    [HttpPost]
    public async Task<IActionResult> Agendar([FromBody] AgendarRequest request, CancellationToken cancellationToken)
    {
        if (!TryObtenerUsuarioId(out var usuarioId)) return Unauthorized();
        var citaId = await agendarHandler.Handle(
            new AgendarCitaCommand(usuarioId, request.MedicoId, request.SedeId, request.FechaHora), cancellationToken);
        return Created($"/api/citas/{citaId}", new { citaId });
    }

    [HttpPost("{id:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, CancellationToken cancellationToken)
    {
        if (!TryObtenerUsuarioId(out var usuarioId)) return Unauthorized();
        var ok = await cancelarHandler.Handle(new CancelarCitaCommand(usuarioId, id), cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}