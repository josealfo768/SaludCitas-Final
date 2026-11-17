namespace SaludCitas.Domain.Entities;

public class Medico
{
    public int MedicoId { get; set; }
    public int EspecialidadId { get; set; }
    public string Especialidad { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public List<int> SedeIds { get; set; } = new();
}