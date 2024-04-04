using System.Text;

using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.Usuarios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly InedittaDbContext _context;

        public UsuarioRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(Usuario usuario)
        {
            _context.UsuarioAdms.Update(usuario);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(Usuario usuario)
        {
            _context.UsuarioAdms.Add(usuario);

            await Task.CompletedTask;
        }

        public async ValueTask<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _context.UsuarioAdms.SingleOrDefaultAsync(u => u.Email.Valor == email);
        }

        public async ValueTask<Usuario?> ObterPorIdAsync(long id)
        {
            return await _context.UsuarioAdms.SingleOrDefaultAsync(u => u.Id == (int)id);
        }

        public async ValueTask<IEnumerable<Usuario>?> ObterPorListaIdsAsync(IEnumerable<long> ids)
        {
            return await _context.UsuarioAdms.Where(ua => ids.Any(id => id == ua.Id)).ToListAsync();
        }

        public async ValueTask<IEnumerable<Usuario>?> ObterPorUnidadesGrupoIds(IEnumerable<int> unidadesIds, IEnumerable<int> gruposEconomicosIds)
        {
            var resultQuery =  await _context.UsuarioAdms
                .ToListAsync();

            return resultQuery.Where(uam => unidadesIds.Intersect(uam.EstabelecimentosIds ?? Array.Empty<int>()).Any() || 
                (gruposEconomicosIds.ToList().Contains(uam.GrupoEconomicoId ?? -1) && uam.Nivel == Nivel.GrupoEconomico)
            );
        }

        public async ValueTask<IEnumerable<Usuario>?> ObterPorDocumentoId(long documentoId)
        {
            var parameters = new List<MySqlParameter>();

            var query = new StringBuilder(@"
                SELECT * FROM usuario_adm ua 
                WHERE EXISTS (
	                SELECT 1 FROM doc_sind ds 
	                WHERE ds.id_doc = @docId
	                      AND CASE WHEN ua.nivel = 'Grupo Econômico' THEN JSON_CONTAINS(ds.cliente_estabelecimento, CONCAT('{{""g"": ', ua.id_grupoecon,'}}'))
	                               ELSE EXISTS (
      		                            SELECT 1 FROM cliente_unidades cut
      		               	            WHERE EXISTS (
      		               		            SELECT 
								                jt.*
								            FROM 
								                JSON_TABLE(
								                    ds.cliente_estabelecimento,
								                    ""$[*]"" COLUMNS (
								                        g INT PATH ""$.g"",
								                        m INT PATH ""$.m"",
								                        u INT PATH ""$.u"",
								                        nome_unidade VARCHAR(255) PATH ""$.nome_unidade""
								                    )
								                ) AS jt
								             WHERE jt.u = cut.id_unidade AND JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(cut.id_unidade))
      		               	            )	
                                  )
	                          END
                ) AND ua.nivel <> 'Ineditta'
            ");

            parameters.Add(new MySqlParameter("@docId", documentoId));

            var usuarios = await _context.UsuarioAdms.FromSqlRaw(query.ToString(), parameters.ToArray()).ToListAsync();

            return usuarios;
        }

        public async ValueTask<IEnumerable<Usuario>?> ObterPorDocumentoId(long documentoId, IEnumerable<int>? usuariosIds)
        {
            var parameters = new List<MySqlParameter>();
            var parametersCount = 0;

            var query = new StringBuilder(@"
                SELECT * FROM usuario_adm ua 
                WHERE EXISTS (
	                SELECT 1 FROM doc_sind ds 
	                WHERE ds.id_doc = @docId
	                      AND CASE WHEN ua.nivel = 'Grupo Econômico' THEN JSON_CONTAINS(ds.cliente_estabelecimento, CONCAT('{{""g"": ', ua.id_grupoecon,'}}'))
	                               ELSE EXISTS (
      		                            SELECT 1 FROM cliente_unidades cut
      		               	            WHERE EXISTS (
      		               		            SELECT 
								                jt.*
								            FROM 
								                JSON_TABLE(
								                    ds.cliente_estabelecimento,
								                    ""$[*]"" COLUMNS (
								                        g INT PATH ""$.g"",
								                        m INT PATH ""$.m"",
								                        u INT PATH ""$.u"",
								                        nome_unidade VARCHAR(255) PATH ""$.nome_unidade""
								                    )
								                ) AS jt
								             WHERE jt.u = cut.id_unidade AND JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(cut.id_unidade))
      		               	            )	
                                  )
	                          END
                )
                AND ua.nivel <> 'Ineditta'
            ");

            if (usuariosIds is not null && usuariosIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, usuariosIds, "ua.id_user", parameters, ref parametersCount);
            }

            parameters.Add(new MySqlParameter("@docId", documentoId));

            return await _context.UsuarioAdms.FromSqlRaw(query.ToString(), parameters.ToArray()).ToListAsync();
        }
    }
}

