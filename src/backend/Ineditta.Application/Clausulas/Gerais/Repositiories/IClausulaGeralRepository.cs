using Ineditta.Application.Clausulas.Gerais.Dtos;
using Ineditta.Application.Clausulas.Gerais.Entities;

namespace Ineditta.Application.Clausulas.Gerais.Repositiories
{
    public interface IClausulaGeralRepository
    {
        ValueTask IncluirAsync(ClausulaGeral clausulaGeral);
        ValueTask AtualizarAsync(ClausulaGeral clausulaGeral);
        ValueTask<ClausulaGeral?> ObterPorId(int id);
        ValueTask<IEnumerable<ObterClausulasPorEmpresaDocumentoIdDto>?> ObterTodasPorEmpresaDocumentoId(int id);
        ValueTask<IEnumerable<ClausulaGeral>?> ObterTodasPorDocumentoResumivel(int id);
        ValueTask<IEnumerable<ClausulaGeral>?> ObterTodasPorDocumentoId(int documentoId);
        ValueTask<IEnumerable<ClausulaGeral>?> ObterPorDocumentoEstruturaId(int estruturaId, int documentoId);
        ValueTask<IEnumerable<ClausulaGeral>> ObterPorDocumentoId(int documentoSindicalId);
    }
}
