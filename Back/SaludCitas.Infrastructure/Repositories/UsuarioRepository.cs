using Dapper;
using SaludCitas.Application.Abstractions;
using SaludCitas.Domain.Entities;
using SaludCitas.Infrastructure.Data;

namespace SaludCitas.Infrastructure.Repositories;

public class UsuarioRepository(IDbConnectionFactory connectionFactory) : IUsuarioRepository
{
    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT UsuarioId, Correo, ClaveHash, Rol
            FROM dbo.Usuarios
            WHERE Correo = @Correo";

        using var conexion = connectionFactory.CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<Usuario>(
            new CommandDefinition(sql, new { Correo = correo }, cancellationToken: cancellationToken));
    }

    public async Task<Paciente?> ObtenerPacientePorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT PacienteId, UsuarioId, Nombres, Apellidos, Documento, Telefono
            FROM dbo.Pacientes
            WHERE UsuarioId = @UsuarioId";

        using var conexion = connectionFactory.CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<Paciente>(
            new CommandDefinition(sql, new { UsuarioId = usuarioId }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM dbo.Usuarios WHERE Correo = @Correo";

        using var conexion = connectionFactory.CrearConexion();
        var total = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Correo = correo }, cancellationToken: cancellationToken));
        return total > 0;
    }

    public async Task<bool> ExisteDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM dbo.Pacientes WHERE Documento = @Documento";

        using var conexion = connectionFactory.CrearConexion();
        var total = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Documento = documento }, cancellationToken: cancellationToken));
        return total > 0;
    }

    public async Task<(Usuario Usuario, Paciente Paciente)> RegistrarPacienteAsync(
        Usuario usuario, Paciente paciente, CancellationToken cancellationToken = default)
    {
        const string sqlUsuario = @"
            INSERT INTO dbo.Usuarios (Correo, ClaveHash, Rol)
            OUTPUT INSERTED.UsuarioId
            VALUES (@Correo, @ClaveHash, @Rol)";

        const string sqlPaciente = @"
            INSERT INTO dbo.Pacientes (UsuarioId, Nombres, Apellidos, Documento, Telefono)
            OUTPUT INSERTED.PacienteId
            VALUES (@UsuarioId, @Nombres, @Apellidos, @Documento, @Telefono)";

        using var conexion = connectionFactory.CrearConexion();
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            usuario.UsuarioId = await conexion.ExecuteScalarAsync<int>(
                new CommandDefinition(sqlUsuario, usuario, transaccion, cancellationToken: cancellationToken));

            paciente.UsuarioId = usuario.UsuarioId;
            paciente.PacienteId = await conexion.ExecuteScalarAsync<int>(
                new CommandDefinition(sqlPaciente, paciente, transaccion, cancellationToken: cancellationToken));

            transaccion.Commit();
            return (usuario, paciente);
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }
}