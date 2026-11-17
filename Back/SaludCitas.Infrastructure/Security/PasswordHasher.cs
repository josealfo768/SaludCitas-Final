using SaludCitas.Application.Abstractions;

namespace SaludCitas.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int FactorDeTrabajo = 11;

    public string Hash(string clave) => BCrypt.Net.BCrypt.HashPassword(clave, FactorDeTrabajo);

    public bool Verificar(string clave, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(clave, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // El hash guardado no tiene formato BCrypt: se trata como contraseña incorrecta
            return false;
        }
    }
}