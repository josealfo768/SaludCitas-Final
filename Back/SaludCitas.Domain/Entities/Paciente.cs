namespace SaludCitas.Domain.Entities;

public class Paciente
{
    public int PacienteId { get; set; }
    public int UsuarioId { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}
