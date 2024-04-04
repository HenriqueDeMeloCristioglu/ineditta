using Ineditta.Application.InformacoesAdicionais.Cliente.Entities;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.Repositories
{
    public interface IInformacaoAdicionalClienteRepository
    {
        ValueTask IncluirAsync(InformacaoAdicionalCliente informacaoAdicionalCliente);
        ValueTask AtualizarAsync(InformacaoAdicionalCliente informacaoAdicionalCliente);
        ValueTask<InformacaoAdicionalCliente?> ObterPorGrupoDocumentoAsync(int grupoEconomicoId, int documentoSindical);
    }
}
