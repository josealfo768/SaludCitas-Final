using Dapper;
using SaludCitas.Application.Abstractions;
using SaludCitas.Domain.Entities;
using SaludCitas.Infrastructure.Data;

namespace SaludCitas.Infrastructure.Repositories;

public class MedicoRepository(IDbConnectionFactory connectionFactory) : IMedicoRepository
{
    private sealed record Relacion(int MedicoId, int SedeId);

    public async Task<IReadOnlyList<Medico>> ListarAsync(
        int? especialidadId, int? sedeId, CancellationToken cancellationToken = default)
    {
        const string sqlMedicos = @"
            SELECT m.MedicoId, m.EspecialidadId, e.Nombre AS Especialidad, m.Nombres, m.Apellidos
            FROM dbo.Medicos m
            INNER JOIN dbo.Especialidades e ON e.EspecialidadId = m.EspecialidadId
            WHERE m.Activo = 1
              AND (@EspecialidadId IS NULL OR m.EspecialidadId = @EspecialidadId)
              AND (@SedeId IS NULL OR EXISTS (
                    SELECT 1 FROM dbo.MedicoSedes ms
                    WHERE ms.MedicoId = m.MedicoId AND ms.SedeId = @SedeId))
            ORDER BY m.Apellidos, m.Nombres";

        const string sqlSedes = "SELECT MedicoId, SedeId FROM dbo.MedicoSedes";

        using var conexion = connectionFactory.CrearConexion();

        var medicos = (await conexion.QueryAsync<Medico>(new CommandDefinition(
            sqlMedicos,
            new { EspecialidadId = especialidadId, SedeId = sedeId },
            cancellationToken: cancellationToken))).ToList();

        var relaciones = await conexion.QueryAsync<Relacion>(
            new CommandDefinition(sqlSedes, cancellationToken: cancellationToken));

        var sedesPorMedico = relaciones
            .GroupBy(r => r.MedicoId)
            .ToDictionary(g => g.Key, g => g.Select(r => r.SedeId).ToList());

        foreach (var medico in medicos)
            if (sedesPorMedico.TryGetValue(medico.MedicoId, out var ids))
                medico.SedeIds = ids;

        return medicos;
    }

    public async Task<IReadOnlyList<Medico>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        const string sqlMedicos = @"
            SELECT m.MedicoId, m.EspecialidadId, e.Nombre AS Especialidad, m.Nombres, m.Apellidos, m.Activo
            FROM dbo.Medicos m
            INNER JOIN dbo.Especialidades e ON e.EspecialidadId = m.EspecialidadId
            ORDER BY m.Apellidos, m.Nombres";

        const string sqlSedes = "SELECT MedicoId, SedeId FROM dbo.MedicoSedes";

        using var conexion = connectionFactory.CrearConexion();

        var medicos = (await conexion.QueryAsync<Medico>(
            new CommandDefinition(sqlMedicos, cancellationToken: cancellationToken))).ToList();

        var relaciones = await conexion.QueryAsync<Relacion>(
            new CommandDefinition(sqlSedes, cancellationToken: cancellationToken));

        var sedesPorMedico = relaciones
            .GroupBy(r => r.MedicoId)
            .ToDictionary(g => g.Key, g => g.Select(r => r.SedeId).ToList());

        foreach (var medico in medicos)
            if (sedesPorMedico.TryGetValue(medico.MedicoId, out var ids))
                medico.SedeIds = ids;

        return medicos;
    }

    public async Task<int> CrearAsync(Medico medico, CancellationToken cancellationToken = default)
    {
        const string sqlMedico = @"
            INSERT INTO dbo.Medicos (EspecialidadId, Nombres, Apellidos, Activo)
            OUTPUT INSERTED.MedicoId
            VALUES (@EspecialidadId, @Nombres, @Apellidos, 1)";

        using var conexion = connectionFactory.CrearConexion();
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        var medicoId = await conexion.ExecuteScalarAsync<int>(new CommandDefinition(
            sqlMedico,
            new { medico.EspecialidadId, medico.Nombres, medico.Apellidos },
            transaccion,
            cancellationToken: cancellationToken));

        foreach (var sedeId in medico.SedeIds.Distinct())
        {
            await conexion.ExecuteAsync(new CommandDefinition(
                "INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (@MedicoId, @SedeId)",
                new { MedicoId = medicoId, SedeId = sedeId },
                transaccion,
                cancellationToken: cancellationToken));
        }

        transaccion.Commit();
        return medicoId;
    }

    public async Task<bool> ActualizarAsync(Medico medico, CancellationToken cancellationToken = default)
    {
        const string sqlMedico = @"
            UPDATE dbo.Medicos
            SET EspecialidadId = @EspecialidadId, Nombres = @Nombres, Apellidos = @Apellidos
            WHERE MedicoId = @MedicoId";

        using var conexion = connectionFactory.CrearConexion();
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        var filas = await conexion.ExecuteAsync(new CommandDefinition(
            sqlMedico,
            new { medico.MedicoId, medico.EspecialidadId, medico.Nombres, medico.Apellidos },
            transaccion,
            cancellationToken: cancellationToken));

        if (filas == 0)
        {
            transaccion.Rollback();
            return false;
        }

        await conexion.ExecuteAsync(new CommandDefinition(
            "DELETE FROM dbo.MedicoSedes WHERE MedicoId = @MedicoId",
            new { medico.MedicoId },
            transaccion,
            cancellationToken: cancellationToken));

        foreach (var sedeId in medico.SedeIds.Distinct())
        {
            await conexion.ExecuteAsync(new CommandDefinition(
                "INSERT INTO dbo.MedicoSedes (MedicoId, SedeId) VALUES (@MedicoId, @SedeId)",
                new { medico.MedicoId, SedeId = sedeId },
                transaccion,
                cancellationToken: cancellationToken));
        }

        transaccion.Commit();
        return true;
    }

    public async Task<bool> CambiarEstadoAsync(int medicoId, bool activo, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Medicos SET Activo = @Activo WHERE MedicoId = @MedicoId";

        using var conexion = connectionFactory.CrearConexion();
        var filas = await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { MedicoId = medicoId, Activo = activo }, cancellationToken: cancellationToken));
        return filas > 0;
    }
}