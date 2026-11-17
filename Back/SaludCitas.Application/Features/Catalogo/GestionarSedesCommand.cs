using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Common;

namespace SaludCitas.Application.Features.Catalogo;

public record CrearSedeCommand(string Nombre, string Direccion) : ICommand<int>;

public record ActualizarSedeCommand(int SedeId, string Nombre, string Direccion) : ICommand<bool>;

public record EliminarSedeCommand(int SedeId) : ICommand<bool>;

public class CrearSedeHandler(ISedeRepository repositorio)
    : ICommandHandler<CrearSedeCommand, int>
{
    public async Task<int> Handle(CrearSedeCommand command, CancellationToken cancellationToken = default)
    {
        var nombre = (command.Nombre ?? string.Empty).Trim();
        var direccion = (command.Direccion ?? string.Empty).Trim();
        if (nombre.Length == 0 || direccion.Length == 0)
            throw new BadRequestException("El nombre y la dirección de la sede son obligatorios.");
        if (await repositorio.ExisteNombreAsync(nombre, null, cancellationToken))
            throw new ConflictException("Ya existe un registro con ese nombre.");

        return await repositorio.CrearAsync(nombre, direccion, cancellationToken);
    }
}

public class ActualizarSedeHandler(ISedeRepository repositorio)
    : ICommandHandler<ActualizarSedeCommand, bool>
{
    public async Task<bool> Handle(ActualizarSedeCommand command, CancellationToken cancellationToken = default)
    {
        var nombre = (command.Nombre ?? string.Empty).Trim();
        var direccion = (command.Direccion ?? string.Empty).Trim();
        if (nombre.Length == 0 || direccion.Length == 0)
            throw new BadRequestException("El nombre y la dirección de la sede son obligatorios.");
        if (await repositorio.ExisteNombreAsync(nombre, command.SedeId, cancellationToken))
            throw new ConflictException("Ya existe un registro con ese nombre.");

        return await repositorio.ActualizarAsync(command.SedeId, nombre, direccion, cancellationToken);
    }
}

public class EliminarSedeHandler(ISedeRepository repositorio)
    : ICommandHandler<EliminarSedeCommand, bool>
{
    public async Task<bool> Handle(EliminarSedeCommand command, CancellationToken cancellationToken = default)
    {
        if (await repositorio.TieneMedicosAsync(command.SedeId, cancellationToken))
            throw new ConflictException("No se puede eliminar: hay médicos o citas asociados a esta sede.");

        return await repositorio.EliminarAsync(command.SedeId, cancellationToken);
    }
}