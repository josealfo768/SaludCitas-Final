using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Common;

namespace SaludCitas.Application.Features.Catalogo;

public record CrearEspecialidadCommand(string Nombre) : ICommand<int>;

public record ActualizarEspecialidadCommand(int EspecialidadId, string Nombre) : ICommand<bool>;

public record EliminarEspecialidadCommand(int EspecialidadId) : ICommand<bool>;

public class CrearEspecialidadHandler(IEspecialidadRepository repositorio)
    : ICommandHandler<CrearEspecialidadCommand, int>
{
    public async Task<int> Handle(CrearEspecialidadCommand command, CancellationToken cancellationToken = default)
    {
        var nombre = (command.Nombre ?? string.Empty).Trim();
        if (nombre.Length == 0)
            throw new BadRequestException("El nombre de la especialidad es obligatorio.");
        if (await repositorio.ExisteNombreAsync(nombre, null, cancellationToken))
            throw new ConflictException("Ya existe un registro con ese nombre.");

        return await repositorio.CrearAsync(nombre, cancellationToken);
    }
}

public class ActualizarEspecialidadHandler(IEspecialidadRepository repositorio)
    : ICommandHandler<ActualizarEspecialidadCommand, bool>
{
    public async Task<bool> Handle(ActualizarEspecialidadCommand command, CancellationToken cancellationToken = default)
    {
        var nombre = (command.Nombre ?? string.Empty).Trim();
        if (nombre.Length == 0)
            throw new BadRequestException("El nombre de la especialidad es obligatorio.");
        if (await repositorio.ExisteNombreAsync(nombre, command.EspecialidadId, cancellationToken))
            throw new ConflictException("Ya existe un registro con ese nombre.");

        return await repositorio.ActualizarAsync(command.EspecialidadId, nombre, cancellationToken);
    }
}

public class EliminarEspecialidadHandler(IEspecialidadRepository repositorio)
    : ICommandHandler<EliminarEspecialidadCommand, bool>
{
    public async Task<bool> Handle(EliminarEspecialidadCommand command, CancellationToken cancellationToken = default)
    {
        if (await repositorio.TieneMedicosAsync(command.EspecialidadId, cancellationToken))
            throw new ConflictException("No se puede eliminar: hay médicos con esta especialidad.");

        return await repositorio.EliminarAsync(command.EspecialidadId, cancellationToken);
    }
}