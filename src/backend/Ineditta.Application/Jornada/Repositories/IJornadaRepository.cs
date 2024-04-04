namespace Ineditta.Application.Jornada.Repositories
{
    public interface IJornadaRepository
    {
        ValueTask IncluirAsync(Entities.Jornada jornada);
    }
}
