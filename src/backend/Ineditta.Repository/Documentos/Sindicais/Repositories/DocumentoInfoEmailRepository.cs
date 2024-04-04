using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Application.EstruturasClausulas.Gerais.Entities;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Documentos.Sindicais.Repositories
{
    public class DocumentoInfoEmailRepository : IDocumentoInfoEmailRepository
    {
        private readonly InedittaDbContext _dbContext;

        public DocumentoInfoEmailRepository(InedittaDbContext context)
        {
            _dbContext = context;
        }

        public async ValueTask<InfoDocumentoCriadoDto?> ObterInfoDocumentoCriadoEmail(long idDocumento)
        {
            var result = await (from ds in _dbContext.DocSinds
                                join td in _dbContext.TipoDocs on ds.TipoDocumentoId equals td.Id into _td
                                from td in _td.DefaultIfEmpty()
                                where ds.Id == (int)idDocumento
                                select new InfoDocumentoCriadoDto
                                {
                                    Estabelecimentos = ds.Estabelecimentos,
                                    Comentarios = ds.Observacao,
                                    Abrangencia = ds.Abrangencias,
                                    FonteLegislacaoSite = ds.FonteWeb,
                                    NumeroLegislacao = ds.NumeroLei,
                                    Restrito = ds.Restrito,
                                    NomeDocumento = td.Nome,
                                    SindicatosLaborais = (from dsl in _dbContext.DocumentosSindicatosLaborais
                                                          join sinde in _dbContext.SindEmps on dsl.SindicatoLaboralId equals sinde.Id
                                                          where dsl.DocumentoSindicalId == idDocumento
                                                          select new SindicatoLaboral
                                                          {
                                                              Id = sinde.Id,
                                                              Cnpj = sinde.Cnpj.Value,
                                                              Codigo = sinde.CodigoSindical.Valor,
                                                              Municipio = sinde.Municipio,
                                                              Sigla = sinde.Sigla,
                                                              Uf = sinde.Uf,
                                                              Denominacao = sinde.Denominacao
                                                          }).AsSplitQuery().ToList(),
                                    SindicatosPatronais = (from dsp in _dbContext.DocumentosSindicatosPatronais
                                                           join sindp in _dbContext.SindPatrs on dsp.SindicatoPatronalId equals sindp.Id
                                                           where dsp.DocumentoSindicalId == idDocumento
                                                           select new SindicatoPatronal
                                                           {
                                                               Id = sindp.Id,
                                                               Cnpj = sindp.Cnpj.Value,
                                                               Codigo = sindp.CodigoSindical.Valor,
                                                               Municipio = sindp.Municipio,
                                                               Sigla = sindp.Sigla,
                                                               Uf = sindp.Uf,
                                                               Denominacao = sindp.Denominacao
                                                           }).AsSplitQuery().ToList(),
                                    ValidadeFinal = ds.DataValidadeFinal,
                                    ValidadeInicial = ds.DataValidadeInicial,
                                    OrigemDocumento = ds.Origem,
                                    AssuntosIds = ds.Referencias,
                                    AtividadesEconomicas = ds.Cnaes,
                                }).FirstOrDefaultAsync();

            if (result is null) return result;

            var assuntos = await _dbContext.EstruturaClausula.ToListAsync();
            if (assuntos is not null && assuntos.Any())
            {
                var assuntosIds = result.AssuntosIds is not null ? result.AssuntosIds.Select(id => int.TryParse(id, out int idInt) ? idInt : 0) : new List<int>();
                if (assuntosIds is null || !assuntosIds.Any())
                {
                    assuntos = new List<EstruturaClausula>();
                }
                else
                {
                    assuntos = assuntos.Where(a => assuntosIds.Contains(a.Id)).ToList();
                }
            }

            result.Assuntos = assuntos is not null ? string.Join(", ", assuntos.Select(a => a.Nome)) : "";

            return result;

        }
    }
}
