using CSharpFunctionalExtensions;

namespace Ineditta.Application.Documentos.Sindicais.Repositories
{
    public interface IDocumentoSindicalSindicatosRepository
    {
        ValueTask<Result> RefazerDocumentosSindicatos();
        ValueTask LimparRelacionamentosDocumentoSindicato();
    }
}
