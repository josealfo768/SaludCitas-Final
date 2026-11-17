using Dapper;
using SaludCitas.Application.Abstractions;
using SaludCitas.Domain.Entities;
using SaludCitas.Infrastructure.Data;

namespace SaludCitas.Infrastructure.Repositories;

public class SedeRepository(IDbConnectionFactory connectionFactory) : ISedeRepository
{
    public async Task<IReadOnlyList<Sede>> ListarAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT SedeId, Nombre, Direccion
            FROM dbo.Sedes
            ORDER BY Nombre";

        using var conexion = connectionFactory.CrearConexion();
        var filas = await conexion.QueryAsync<Sede>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        return filas.ToList();
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT COUNT(1) FROM dbo.Sedes
            WHERE Nombre = @Nombre AND (@ExcluirId IS NULL OR SedeId <> @ExcluirId)";

        using var conexion = connectionFactory.CrearConexion();
        var total = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Nombre = nombre, ExcluirId = excluirId }, cancellationToken: cancellationToken));
        return total > 0;
    }

    public async Task<bool> TieneMedicosAsync(int sedeId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT (SELECT COUNT(1) FROM dbo.MedicoSedes WHERE SedeId = @SedeId)
                 + (SELECT COUNT(1) FROM dbo.Citas WHERE SedeId = @SedeId)";

        using var conexion = connectionFactory.CrearConexion();
        var total = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { SedeId = sedeId }, cancellationToken: cancellationToken));
        return total > 0;
    }

    public async Task<int> CrearAsync(string nombre, string direccion, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO dbo.Sedes (Nombre, Direccion)
            OUTPUT INSERTED.SedeId
            VALUES (@Nombre, @Direccion)";

        using var conexion = connectionFactory.CrearConexion();
        return await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Nombre = nombre, Direccion = direccion }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ActualizarAsync(int sedeId, string nombre, string direccion, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Sedes SET Nombre = @Nombre, Direccion = @Direccion WHERE SedeId = @SedeId";

        using var conexion = connectionFactory.CrearConexion();
        var filas = await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { SedeId = sedeId, Nombre = nombre, Direccion = direccion }, cancellationToken: cancellationToken));
        return filas > 0;
    }

    public async Task<bool> EliminarAsync(int sedeId, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM dbo.Sedes WHERE SedeId = @SedeId";

        using var conexion = connectionFactory.CrearConexion();
        var filas = await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { SedeId = sedeId }, cancellationToken: cancellationToken));
        return filas > 0;
    }
}