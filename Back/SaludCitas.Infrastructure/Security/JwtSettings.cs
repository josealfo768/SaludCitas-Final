namespace SaludCitas.Infrastructure.Security;

public class JwtSettings
{
    public const string Seccion = "Jwt";

    public string Clave { get; set; } = string.Empty;
    public string Emisor { get; set; } = "SaludCitas.Api";
    public string Audiencia { get; set; } = "SaludCitas.Front";
    public int MinutosExpiracion { get; set; } = 120;
}