using System.Text;

using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.ClientesUnidades.Repositories;
using Ineditta.Application.GruposEconomicos.Entities;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.ClientesUnidades
{
    public class ClienteUnidadeRepository : IClienteUnidadeRepository
    {
        private readonly InedittaDbContext _context;

        public ClienteUnidadeRepository(InedittaDbContext context)
        {
            _context = context;
        }
        public async ValueTask AtualizarAsync(ClienteUnidade clienteUnidade)
        {
            _context.Entry(clienteUnidade).Property("DataInclusao").CurrentValue = DateTime.Now;

            if (clienteUnidade.EmpresaId > 0)
            {
                _context.Entry(clienteUnidade).Property("NomeEmpresa").CurrentValue =
                    await _context.ClienteMatrizes.Where(cmt => cmt.Id == clienteUnidade.EmpresaId)
                                                  .Select(cmt => cmt.Nome)
                                                  .SingleOrDefaultAsync();

                var grupoEconomico = await (from cmt in _context.ClienteMatrizes
                                            join cgt in _context.GrupoEconomico on cmt.GrupoEconomicoId equals cgt.Id
                                            where cmt.Id == clienteUnidade.EmpresaId
                                            select new
                                            {
                                                cgt.Id,
                                                cgt.Nome,
                                            })
                                            .SingleOrDefaultAsync();

                if (grupoEconomico == null)
                {
                    return;
                }

                _context.Entry(clienteUnidade).Property("NomeGrupoEconomico").CurrentValue = grupoEconomico.Nome;
                _context.Entry(clienteUnidade).Property("GrupoEconomicoId").CurrentValue = grupoEconomico.Id;
            }

            _context.ClienteUnidades.Update(clienteUnidade);

            await Task.CompletedTask;
        }

        public async ValueTask<bool> ExistePorIdAsync(int clienteUnidadeId)
        {
            return await _context.ClienteUnidades.AnyAsync(c => c.Id == clienteUnidadeId);
        }

        public async ValueTask IncluirAsync(ClienteUnidade clienteUnidade)
        {
            _context.Entry(clienteUnidade).Property("DataInclusao").CurrentValue = DateTime.Now;

            if (clienteUnidade.EmpresaId <= 0)
            {
                return;
            }

            _context.Entry(clienteUnidade).Property("NomeEmpresa").CurrentValue =
                await _context.ClienteMatrizes.Where(cmt => cmt.Id == clienteUnidade.EmpresaId)
                                                .Select(cmt => cmt.Nome)
                                                .SingleOrDefaultAsync();

            var grupoEconomico = await (from cmt in _context.ClienteMatrizes
                                        join cgt in _context.GrupoEconomico on cmt.GrupoEconomicoId equals cgt.Id
                                        where cmt.Id == clienteUnidade.EmpresaId
                                        select new
                                        {
                                            cgt.Id,
                                            cgt.Nome,
                                        })
                                        .SingleOrDefaultAsync();

            if (grupoEconomico == null)
            {
                return;
            }

            _context.Entry(clienteUnidade).Property("NomeGrupoEconomico").CurrentValue = grupoEconomico.Nome;
            _context.Entry(clienteUnidade).Property("GrupoEconomicoId").CurrentValue = grupoEconomico.Id;

            _context.ClienteUnidades.Add(clienteUnidade);

            await Task.CompletedTask;
        }

        public async ValueTask<IEnumerable<ClienteUnidade>?> ObterClientesUnidadePorDocumentoPorUsuario(long documentoId, long usuarioId)
        {
            var parameters = new List<MySqlParameter>();

            var query = new StringBuilder(@"
                SELECT unidades.* FROM usuario_adm ua 
                INNER JOIN LATERAL (
	                SELECT cut.* FROM doc_sind ds 
	                INNER JOIN cliente_unidades cut ON JSON_CONTAINS(ds.cliente_estabelecimento, CONCAT('{{""u"": ', cut.id_unidade,'}}'))
	                WHERE ds.id_doc = @documentoId
		                    AND cut.cliente_grupo_id_grupo_economico = ua.id_grupoecon
	                        AND CASE WHEN ua.nivel = 'Grupo Econômico' THEN cut.cliente_grupo_id_grupo_economico = ua.id_grupoecon 
	                                 ELSE JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(cut.id_unidade))
	                            END
                ) unidades ON TRUE
                WHERE ua.id_user = @usuarioId
            ");

            parameters.Add(new MySqlParameter("@documentoId", documentoId));
            parameters.Add(new MySqlParameter("@usuarioId", usuarioId));

            var unidades = await _context.ClienteUnidades
                                    .FromSqlRaw(query.ToString(), parameters.ToArray())
                                    .ToListAsync();
            return unidades;
        }

        public async ValueTask<ClienteUnidade?> ObterPorIdAsync(int id)
        {
            return await _context.ClienteUnidades.FindAsync(id);
        }

        public async ValueTask<IEnumerable<ClienteUnidade>?> ObterPorListaIds(IEnumerable<int> ids)
        {
            return await _context.ClienteUnidades.Where(
                unidade => unidade != null && ids.Any(id => id == unidade.Id)
            ).ToListAsync();
        }
    }
}
