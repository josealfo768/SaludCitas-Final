namespace SaludCitas.Domain.Entities;

public class Cita
{
    public int CitaId { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public int SedeId { get; set; }
    public DateTime FechaHora { get; set; }
    public string Estado { get; set; } = string.Empty;
}
