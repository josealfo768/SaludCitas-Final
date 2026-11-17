using Dapper;
using SaludCitas.Application.Abstractions;
using SaludCitas.Infrastructure.Data;
using SaludCitas.Domain.Entities;

namespace SaludCitas.Infrastructure.Repositories;

public class CitaRepository(IDbConnectionFactory connectionFactory) : ICitaRepository
{
    public async Task<int?> ObtenerPacienteIdPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT PacienteId FROM dbo.Pacientes WHERE UsuarioId = @UsuarioId";

        using var conexion = connectionFactory.CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<int?>(
            new CommandDefinition(sql, new { UsuarioId = usuarioId }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<CitaDetalle>> ListarPorPacienteAsync(int pacienteId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT c.CitaId,
                   c.MedicoId,
                   m.Nombres + ' ' + m.Apellidos AS Medico,
                   e.Nombre AS Especialidad,
                   c.SedeId,
                   s.Nombre AS Sede,
                   c.FechaHora,
                   c.Estado
            FROM dbo.Citas c
            INNER JOIN dbo.Medicos m ON m.MedicoId = c.MedicoId
            INNER JOIN dbo.Especialidades e ON e.EspecialidadId = m.EspecialidadId
            INNER JOIN dbo.Sedes s ON s.SedeId = c.SedeId
            WHERE c.PacienteId = @PacienteId
            ORDER BY c.FechaHora DESC";

        using var conexion = connectionFactory.CrearConexion();
        var filas = await conexion.QueryAsync<CitaDetalle>(
            new CommandDefinition(sql, new { PacienteId = pacienteId }, cancellationToken: cancellationToken));
        return filas.ToList();
    }

    public async Task<IReadOnlyList<DateTime>> ListarOcupadasAsync(int medicoId, DateTime fecha, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT FechaHora
            FROM dbo.Citas
            WHERE MedicoId = @MedicoId
              AND Estado = 'programada'
              AND FechaHora >= @Desde AND FechaHora < @Hasta";

        using var conexion = connectionFactory.CrearConexion();
        var filas = await conexion.QueryAsync<DateTime>(
            new CommandDefinition(sql,
                new { MedicoId = medicoId, Desde = fecha.Date, Hasta = fecha.Date.AddDays(1) },
                cancellationToken: cancellationToken));
        return filas.ToList();
    }

    public async Task<bool> ExisteCitaProgramadaAsync(int medicoId, DateTime fechaHora, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT COUNT(1) FROM dbo.Citas
            WHERE MedicoId = @MedicoId AND FechaHora = @FechaHora AND Estado = 'programada'";

        using var conexion = connectionFactory.CrearConexion();
        var total = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { MedicoId = medicoId, FechaHora = fechaHora }, cancellationToken: cancellationToken));
        return total > 0;
    }

    public async Task<int> CrearAsync(int pacienteId, int medicoId, int sedeId, DateTime fechaHora, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO dbo.Citas (PacienteId, MedicoId, SedeId, FechaHora, Estado)
            OUTPUT INSERTED.CitaId
            VALUES (@PacienteId, @MedicoId, @SedeId, @FechaHora, 'programada')";

        using var conexion = connectionFactory.CrearConexion();
        return await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql,
                new { PacienteId = pacienteId, MedicoId = medicoId, SedeId = sedeId, FechaHora = fechaHora },
                cancellationToken: cancellationToken));
    }

    public async Task<Cita?> ObtenerPorIdAsync(int citaId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT CitaId, PacienteId, MedicoId, SedeId, FechaHora, Estado FROM dbo.Citas WHERE CitaId = @CitaId";

        using var conexion = connectionFactory.CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<Cita>(
            new CommandDefinition(sql, new { CitaId = citaId }, cancellationToken: cancellationToken));
    }

    public async Task CancelarAsync(int citaId, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Citas SET Estado = 'cancelada' WHERE CitaId = @CitaId";

        using var conexion = connectionFactory.CrearConexion();
        await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { CitaId = citaId }, cancellationToken: cancellationToken));
    }
}