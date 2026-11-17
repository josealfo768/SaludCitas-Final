using SaludCitas.Application.Abstractions;
using SaludCitas.Application.Common;

namespace SaludCitas.Application.Features.Citas;

public record AgendarCitaCommand(int UsuarioId, int MedicoId, int SedeId, DateTime FechaHora) : ICommand<int>;

public class AgendarCitaHandler(ICitaRepository repositorio)
    : ICommandHandler<AgendarCitaCommand, int>
{
    public async Task<int> Handle(AgendarCitaCommand command, CancellationToken cancellationToken = default)
    {
        var pacienteId = await repositorio.ObtenerPacienteIdPorUsuarioAsync(command.UsuarioId, cancellationToken)
            ?? throw new BadRequestException("El usuario no tiene un paciente asociado.");

        var f = command.FechaHora;
        var enBloque = f.Second == 0 && f.Minute % 30 == 0
                       && f.TimeOfDay >= TimeSpan.FromHours(8) && f.TimeOfDay < TimeSpan.FromHours(17);
        if (!enBloque)
            throw new BadRequestException("La hora debe estar entre 8:00 y 17:00 en bloques de 30 minutos.");

        if (f <= DateTime.Now)
            throw new BadRequestException("La cita debe ser en una fecha y hora futuras.");

        if (await repositorio.ExisteCitaProgramadaAsync(command.MedicoId, f, cancellationToken))
            throw new ConflictException("Ese horario ya está ocupado.");

        return await repositorio.CrearAsync(pacienteId, command.MedicoId, command.SedeId, f, cancellationToken);
    }
}