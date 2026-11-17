namespace SaludCitas.Application.Features.Catalogo;

public record EspecialidadDto(int EspecialidadId, string Nombre);

public record SedeDto(int SedeId, string Nombre, string Direccion);

public record MedicoDto(
    int MedicoId,
    int EspecialidadId,
    string Especialidad,
    string Nombres,
    string Apellidos,
    IReadOnlyList<int> SedeIds);