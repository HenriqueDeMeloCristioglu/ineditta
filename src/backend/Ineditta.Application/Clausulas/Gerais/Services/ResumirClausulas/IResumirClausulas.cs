using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Dtos.ResumirClausulas;

namespace Ineditta.Application.Clausulas.Gerais.Services.ResumirClausulas
{
    public interface IResumirClausulas
    {
        ValueTask<Result<ResumirClausulasServiceResponse>> Criar(ResumirClausulasServiceRequest request);
    }
}
