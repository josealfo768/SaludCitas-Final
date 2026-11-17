using SaludCitas.Application.Abstractions;

namespace SaludCitas.Application.Features.Citas;

public record ObtenerHorariosQuery(int MedicoId, DateTime Fecha) : IQuery<IReadOnlyList<DateTime>>;

public class ObtenerHorariosHandler(ICitaRepository repositorio)
    : IQueryHandler<ObtenerHorariosQuery, IReadOnlyList<DateTime>>
{
    private static readonly TimeSpan Inicio = TimeSpan.FromHours(8);
    private static readonly TimeSpan Fin = TimeSpan.FromHours(17);
    private static readonly TimeSpan Bloque = TimeSpan.FromMinutes(30);

    public async Task<IReadOnlyList<DateTime>> Handle(ObtenerHorariosQuery query, CancellationToken cancellationToken = default)
    {
        var dia = query.Fecha.Date;
        var ocupadas = (await repositorio.ListarOcupadasAsync(query.MedicoId, dia, cancellationToken)).ToHashSet();
        var ahora = DateTime.Now;

        var libres = new List<DateTime>();
        for (var hora = Inicio; hora < Fin; hora += Bloque)
        {
            var momento = dia + hora;
            if (momento > ahora && !ocupadas.Contains(momento))
                libres.Add(momento);
        }
        return libres;
    }
}