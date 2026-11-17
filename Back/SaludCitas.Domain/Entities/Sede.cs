namespace SaludCitas.Domain.Entities;

public class Sede
{
    public int SedeId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}