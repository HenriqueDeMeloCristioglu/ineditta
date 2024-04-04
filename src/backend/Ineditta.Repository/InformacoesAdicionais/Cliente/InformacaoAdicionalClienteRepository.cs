using Ineditta.Application.InformacoesAdicionais.Cliente.Entities;
using Ineditta.Application.InformacoesAdicionais.Cliente.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.InformacoesAdicionais.Cliente
{
    public class InformacaoAdicionalClienteRepository : IInformacaoAdicionalClienteRepository
    {
        private readonly InedittaDbContext _context;

        public InformacaoAdicionalClienteRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(InformacaoAdicionalCliente informacaoAdicionalCliente)
        {
            _context.InformacaoAdicionalCliente.Update(informacaoAdicionalCliente);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(InformacaoAdicionalCliente informacaoAdicionalCliente)
        {
            _context.Add(informacaoAdicionalCliente);

            await Task.CompletedTask;
        }

        public async ValueTask<InformacaoAdicionalCliente?> ObterPorGrupoDocumentoAsync(int grupoEconomicoId, int documentoSindical)
        {
            return await _context.InformacaoAdicionalCliente
                .Where(ifc => ifc.GrupoEconomicoId == grupoEconomicoId && ifc.DocumentoSindicalId == documentoSindical)
                .SingleOrDefaultAsync();
        }
    }
}
