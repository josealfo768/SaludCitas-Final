using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Common;
using SaludCitas.Domain.Entities;

namespace SaludCitas.Application.Features.Catalogo;

public record CrearMedicoCommand(int EspecialidadId, string Nombres, string Apellidos, List<int> SedeIds)
    : ICommand<int>;

public record ActualizarMedicoCommand(int MedicoId, int EspecialidadId, string Nombres, string Apellidos, List<int> SedeIds)
    : ICommand<bool>;

public record CambiarEstadoMedicoCommand(int MedicoId, bool Activo) : ICommand<bool>;

public class CrearMedicoHandler(IMedicoRepository repositorio)
    : ICommandHandler<CrearMedicoCommand, int>
{
    public async Task<int> Handle(CrearMedicoCommand command, CancellationToken cancellationToken = default)
    {
        Validar(command.Nombres, command.Apellidos, command.SedeIds);

        var medico = new Medico
        {
            EspecialidadId = command.EspecialidadId,
            Nombres = command.Nombres.Trim(),
            Apellidos = command.Apellidos.Trim(),
            SedeIds = command.SedeIds
        };
        return await repositorio.CrearAsync(medico, cancellationToken);
    }

    internal static void Validar(string nombres, string apellidos, List<int> sedeIds)
    {
        if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(apellidos))
            throw new BadRequestException("Los nombres y apellidos son obligatorios.");
        if (sedeIds is null || sedeIds.Count == 0)
            throw new BadRequestException("El médico debe atender en al menos una sede.");
    }
}

public class ActualizarMedicoHandler(IMedicoRepository repositorio)
    : ICommandHandler<ActualizarMedicoCommand, bool>
{
    public async Task<bool> Handle(ActualizarMedicoCommand command, CancellationToken cancellationToken = default)
    {
        CrearMedicoHandler.Validar(command.Nombres, command.Apellidos, command.SedeIds);

        var medico = new Medico
        {
            MedicoId = command.MedicoId,
            EspecialidadId = command.EspecialidadId,
            Nombres = command.Nombres.Trim(),
            Apellidos = command.Apellidos.Trim(),
            SedeIds = command.SedeIds
        };
        return await repositorio.ActualizarAsync(medico, cancellationToken);
    }
}

public class CambiarEstadoMedicoHandler(IMedicoRepository repositorio)
    : ICommandHandler<CambiarEstadoMedicoCommand, bool>
{
    public Task<bool> Handle(CambiarEstadoMedicoCommand command, CancellationToken cancellationToken = default)
        => repositorio.CambiarEstadoAsync(command.MedicoId, command.Activo, cancellationToken);
}