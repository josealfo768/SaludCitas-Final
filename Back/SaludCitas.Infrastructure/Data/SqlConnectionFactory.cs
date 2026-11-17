using System.Data;
using Microsoft.Data.SqlClient;

namespace SaludCitas.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection CrearConexion();
}

public class SqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CrearConexion() => new SqlConnection(connectionString);
}