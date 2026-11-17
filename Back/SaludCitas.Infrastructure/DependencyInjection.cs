using Microsoft.Extensions.DependencyInjection;
using SaludCitas.Application.Abstractions;
using SaludCitas.Infrastructure.Data;
using SaludCitas.Infrastructure.Repositories;
using SaludCitas.Infrastructure.Security;

namespace SaludCitas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string connectionString, JwtSettings jwtSettings)
    {
        services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(connectionString));

        services.AddSingleton(jwtSettings);
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IEspecialidadRepository, EspecialidadRepository>();
        services.AddScoped<ISedeRepository, SedeRepository>();
        services.AddScoped<IMedicoRepository, MedicoRepository>();
        services.AddScoped<ICitaRepository, CitaRepository>();

        return services;
    }
}