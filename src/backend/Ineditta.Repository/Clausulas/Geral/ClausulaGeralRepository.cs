using System.Text;

using Ineditta.Application.Clausulas.Gerais.Dtos;
using Ineditta.Application.Clausulas.Gerais.Entities;
using Ineditta.Application.Clausulas.Gerais.Repositiories;
using Ineditta.Application.ModulosEstaticos.Entities;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.Clausulas.Geral
{
    public class ClausulaGeralRepository : IClausulaGeralRepository
    {
        private readonly InedittaDbContext _context;

        public ClausulaGeralRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(ClausulaGeral clausulaGeral)
        {
            _context.ClausulaGerals.Update(clausulaGeral);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(ClausulaGeral clausulaGeral)
        {
            await _context.ClausulaGerals.AddAsync(clausulaGeral);
        }

        public async ValueTask<IEnumerable<ClausulaGeral>?> ObterPorDocumentoEstruturaId(int estruturaId, int documentoId)
        {
            return await _context.ClausulaGerals
                .Where(c => c.EstruturaClausulaId == estruturaId && c.DocumentoSindicalId == documentoId)
                .ToListAsync();
        }

        public async ValueTask<ClausulaGeral?> ObterPorId(int id)
        {
            return await _context.ClausulaGerals.Where(c => c.Id == id).SingleOrDefaultAsync();
        }

        public async ValueTask<IEnumerable<ClausulaGeral>?> ObterTodasPorDocumentoId(int documentoId)
        {
            return await _context.ClausulaGerals.Where(c => c.DocumentoSindicalId == documentoId).ToListAsync();
        }

        public async ValueTask<IEnumerable<ObterClausulasPorEmpresaDocumentoIdDto>?> ObterTodasPorEmpresaDocumentoId(int id)
        {
            var sql = new StringBuilder(@"SELECT
                            cg.id_clau id,
	                        cg.estrutura_id_estruturaclausula estrutura_clausula_id,
	                        ec.nome_clausula estrutura_clausula_nome,
	                        cg.tex_clau texto,
                            ec.instrucao_ia,
                            ec.maximo_palavras_ia
                        FROM clausula_geral cg
                        inner join estrutura_clausula ec on cg.estrutura_id_estruturaclausula = ec.id_estruturaclausula
                        WHERE cg.doc_sind_id_documento = @id
                        AND ec.resumivel = 1
                        AND cg.consta_no_documento = 1
                        AND EXISTS (
	                        SELECT 1 from doc_sind ds,
	                        JSON_TABLE(ds.cliente_estabelecimento, '$[*]' COLUMNS (empresa INT PATH '$.m')) AS doc
	                        inner join modulos_cliente mc on doc.empresa = mc.cliente_matriz_id_empresa
	                        where mc.modulos_id_modulos = @modulo
	                        and ds.id_doc = cg.doc_sind_id_documento
                        )
                        order by cg.estrutura_id_estruturaclausula;");

            var parameters = new Dictionary<string, object>
            {
                { "@id", id },
                { "@modulo", Modulo.Comercial.BuscaRapida.Id }
            };

            var result = await _context.SelectFromRawSqlAsync<ObterClausulasPorEmpresaDocumentoIdDto>(sql.ToString(), parameters);

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async ValueTask<IEnumerable<ClausulaGeral>?> ObterTodasPorDocumentoResumivel(int id)
        {
            var sql = new StringBuilder(@"SELECT * FROM clausula_geral cg
                        inner join estrutura_clausula ec on cg.estrutura_id_estruturaclausula = ec.id_estruturaclausula
                        WHERE cg.doc_sind_id_documento = @id
                        AND ec.resumivel = 1
                        AND EXISTS (
	                        SELECT 1 from doc_sind ds,
	                        JSON_TABLE(ds.cliente_estabelecimento, '$[*]' COLUMNS (empresa INT PATH '$.m')) AS doc
	                        inner join modulos_cliente mc on doc.empresa = mc.cliente_matriz_id_empresa
	                        where mc.modulos_id_modulos = @modulo
	                        and ds.id_doc = cg.doc_sind_id_documento
                        )
                        order by cg.estrutura_id_estruturaclausula;");

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@id", id),
                new MySqlParameter("@modulo", Modulo.Comercial.BuscaRapida.Id)
            };

            var result = await _context.ClausulaGerals.FromSqlRaw(sql.ToString(), parameters.ToArray())
                .ToListAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async ValueTask<IEnumerable<ClausulaGeral>> ObterPorDocumentoId(int documentoSindicalId)
        {
            return await _context.ClausulaGerals.Where(c => c.DocumentoSindicalId == documentoSindicalId).ToListAsync();
        }
    }
}
