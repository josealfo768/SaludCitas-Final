using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SaludCitas.Application.Abstractions;

namespace SaludCitas.Application;

public static class DependencyInjection
{
    // Registra automaticamente todos los handlers de comandos y consultas
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var tiposHandler = new[] { typeof(ICommandHandler<,>), typeof(IQueryHandler<,>) };

        var registros = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && tiposHandler.Contains(i.GetGenericTypeDefinition()))
                .Select(i => (Servicio: i, Implementacion: t)));

        foreach (var (servicio, implementacion) in registros)
        {
            services.AddScoped(servicio, implementacion);
        }

        return services;
    }
}
