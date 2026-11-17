using SaludCitas.Domain.Entities;

namespace SaludCitas.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string clave);
    bool Verificar(string clave, string hash);
}

public interface IJwtTokenGenerator
{
    string Generar(Usuario usuario, Paciente? paciente);
}