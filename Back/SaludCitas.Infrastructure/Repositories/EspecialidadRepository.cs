using Dapper;
using SaludCitas.Application.Abstractions;
using SaludCitas.Domain.Entities;
using SaludCitas.Infrastructure.Data;

namespace SaludCitas.Infrastructure.Repositories;

public class EspecialidadRepository(IDbConnectionFactory connectionFactory) : IEspecialidadRepository
{
    public async Task<IReadOnlyList<Especialidad>> ListarAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT EspecialidadId, Nombre
            FROM dbo.Especialidades
            ORDER BY Nombre";

        using var conexion = connectionFactory.CrearConexion();
        var filas = await conexion.QueryAsync<Especialidad>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        return filas.ToList();
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT COUNT(1) FROM dbo.Especialidades
            WHERE Nombre = @Nombre AND (@ExcluirId IS NULL OR EspecialidadId <> @ExcluirId)";

        using var conexion = connectionFactory.CrearConexion();
        var total = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Nombre = nombre, ExcluirId = excluirId }, cancellationToken: cancellationToken));
        return total > 0;
    }

    public async Task<bool> TieneMedicosAsync(int especialidadId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM dbo.Medicos WHERE EspecialidadId = @EspecialidadId";

        using var conexion = connectionFactory.CrearConexion();
        var total = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { EspecialidadId = especialidadId }, cancellationToken: cancellationToken));
        return total > 0;
    }

    public async Task<int> CrearAsync(string nombre, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO dbo.Especialidades (Nombre)
            OUTPUT INSERTED.EspecialidadId
            VALUES (@Nombre)";

        using var conexion = connectionFactory.CrearConexion();
        return await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Nombre = nombre }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ActualizarAsync(int especialidadId, string nombre, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Especialidades SET Nombre = @Nombre WHERE EspecialidadId = @EspecialidadId";

        using var conexion = connectionFactory.CrearConexion();
        var filas = await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { EspecialidadId = especialidadId, Nombre = nombre }, cancellationToken: cancellationToken));
        return filas > 0;
    }

    public async Task<bool> EliminarAsync(int especialidadId, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM dbo.Especialidades WHERE EspecialidadId = @EspecialidadId";

        using var conexion = connectionFactory.CrearConexion();
        var filas = await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { EspecialidadId = especialidadId }, cancellationToken: cancellationToken));
        return filas > 0;
    }
}