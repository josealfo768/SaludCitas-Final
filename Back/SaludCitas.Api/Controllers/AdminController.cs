using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Features.Catalogo;

namespace SaludCitas.Api.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin")]
public class AdminController(
    IQueryHandler<ObtenerTodosMedicosQuery, IReadOnlyList<MedicoAdminDto>> todosMedicosHandler,
    ICommandHandler<CrearMedicoCommand, int> crearMedicoHandler,
    ICommandHandler<ActualizarMedicoCommand, bool> actualizarMedicoHandler,
    ICommandHandler<CambiarEstadoMedicoCommand, bool> estadoMedicoHandler,
    ICommandHandler<CrearEspecialidadCommand, int> crearEspecialidadHandler,
    ICommandHandler<ActualizarEspecialidadCommand, bool> actualizarEspecialidadHandler,
    ICommandHandler<EliminarEspecialidadCommand, bool> eliminarEspecialidadHandler,
    ICommandHandler<CrearSedeCommand, int> crearSedeHandler,
    ICommandHandler<ActualizarSedeCommand, bool> actualizarSedeHandler,
    ICommandHandler<EliminarSedeCommand, bool> eliminarSedeHandler) : ControllerBase
{
    public record MedicoRequest(int EspecialidadId, string Nombres, string Apellidos, List<int> SedeIds);
    public record EstadoRequest(bool Activo);
    public record EspecialidadRequest(string Nombre);
    public record SedeRequest(string Nombre, string Direccion);

    // ---------- Médicos ----------
    [HttpGet("medicos")]
    public async Task<ActionResult<IReadOnlyList<MedicoAdminDto>>> Medicos(CancellationToken cancellationToken)
        => Ok(await todosMedicosHandler.Handle(new ObtenerTodosMedicosQuery(), cancellationToken));

    [HttpPost("medicos")]
    public async Task<IActionResult> CrearMedico([FromBody] MedicoRequest r, CancellationToken cancellationToken)
    {
        var id = await crearMedicoHandler.Handle(
            new CrearMedicoCommand(r.EspecialidadId, r.Nombres, r.Apellidos, r.SedeIds), cancellationToken);
        return Created($"/api/admin/medicos/{id}", new { medicoId = id });
    }

    [HttpPut("medicos/{id:int}")]
    public async Task<IActionResult> ActualizarMedico(int id, [FromBody] MedicoRequest r, CancellationToken cancellationToken)
    {
        var ok = await actualizarMedicoHandler.Handle(
            new ActualizarMedicoCommand(id, r.EspecialidadId, r.Nombres, r.Apellidos, r.SedeIds), cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpPatch("medicos/{id:int}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] EstadoRequest r, CancellationToken cancellationToken)
    {
        var ok = await estadoMedicoHandler.Handle(new CambiarEstadoMedicoCommand(id, r.Activo), cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    // ---------- Especialidades ----------
    [HttpPost("especialidades")]
    public async Task<IActionResult> CrearEspecialidad([FromBody] EspecialidadRequest r, CancellationToken cancellationToken)
    {
        var id = await crearEspecialidadHandler.Handle(new CrearEspecialidadCommand(r.Nombre), cancellationToken);
        return Created($"/api/admin/especialidades/{id}", new { especialidadId = id });
    }

    [HttpPut("especialidades/{id:int}")]
    public async Task<IActionResult> ActualizarEspecialidad(int id, [FromBody] EspecialidadRequest r, CancellationToken cancellationToken)
    {
        var ok = await actualizarEspecialidadHandler.Handle(new ActualizarEspecialidadCommand(id, r.Nombre), cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("especialidades/{id:int}")]
    public async Task<IActionResult> EliminarEspecialidad(int id, CancellationToken cancellationToken)
    {
        var ok = await eliminarEspecialidadHandler.Handle(new EliminarEspecialidadCommand(id), cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    // ---------- Sedes ----------
    [HttpPost("sedes")]
    public async Task<IActionResult> CrearSede([FromBody] SedeRequest r, CancellationToken cancellationToken)
    {
        var id = await crearSedeHandler.Handle(new CrearSedeCommand(r.Nombre, r.Direccion), cancellationToken);
        return Created($"/api/admin/sedes/{id}", new { sedeId = id });
    }

    [HttpPut("sedes/{id:int}")]
    public async Task<IActionResult> ActualizarSede(int id, [FromBody] SedeRequest r, CancellationToken cancellationToken)
    {
        var ok = await actualizarSedeHandler.Handle(new ActualizarSedeCommand(id, r.Nombre, r.Direccion), cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("sedes/{id:int}")]
    public async Task<IActionResult> EliminarSede(int id, CancellationToken cancellationToken)
    {
        var ok = await eliminarSedeHandler.Handle(new EliminarSedeCommand(id), cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}