using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Common;

namespace SaludCitas.Application.Features.Citas;

public record CancelarCitaCommand(int UsuarioId, int CitaId) : ICommand<bool>;

public class CancelarCitaHandler(ICitaRepository repositorio)
    : ICommandHandler<CancelarCitaCommand, bool>
{
    public async Task<bool> Handle(CancelarCitaCommand command, CancellationToken cancellationToken = default)
    {
        var pacienteId = await repositorio.ObtenerPacienteIdPorUsuarioAsync(command.UsuarioId, cancellationToken)
            ?? throw new BadRequestException("El usuario no tiene un paciente asociado.");

        var cita = await repositorio.ObtenerPorIdAsync(command.CitaId, cancellationToken);
        if (cita is null || cita.PacienteId != pacienteId)
            return false;

        if (cita.Estado != "programada")
            throw new BadRequestException("Solo se pueden cancelar citas programadas.");

        if (cita.FechaHora <= DateTime.Now)
            throw new BadRequestException("Solo se pueden cancelar citas futuras.");

        await repositorio.CancelarAsync(cita.CitaId, cancellationToken);
        return true;
    }
}