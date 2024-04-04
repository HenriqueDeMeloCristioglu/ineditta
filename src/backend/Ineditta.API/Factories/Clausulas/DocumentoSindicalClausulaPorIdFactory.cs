using Ineditta.API.ViewModels.ClausulasGerais.ViewModels;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.API.Factories.Clausulas
{
    public class DocumentoSindicalClausulaPorIdFactory
    {
        private readonly InedittaDbContext _context;

        public DocumentoSindicalClausulaPorIdFactory(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<IEnumerable<DocumentoSindicalClausulaVw>> CriarAsync(long clausulaId)
        {
#pragma warning disable CA1305 // Specify IFormatProvider
#pragma warning disable CS8604 // Possible null reference argument.
            var clausulas = await (from dct in _context.DocSinds
                                   join cgt in _context.ClausulaGerals on dct.Id equals cgt.DocumentoSindicalId
                                   join ect in _context.EstruturaClausula on cgt.EstruturaClausulaId equals ect.Id
                                   join gct in _context.GrupoClausula on ect.GrupoClausulaId equals gct.Id
                                   where cgt.Id == clausulaId
                                   select new
                                   {
                                       Id = cgt.Id,
                                       Numero = cgt.Numero,
                                       EstruturaClausulaId = cgt.Id,
                                       EstruturaClausulaNome = ect.Nome,
                                       GrupoClausulaId = gct.Id,
                                       GrupoClausulaNome = gct.Nome,
                                       DocumentoId = cgt.DocumentoSindicalId,
                                       InformacoesAdicionais = (from cgect in _context.InformacaoAdicionalSisap
                                                                join ect2 in _context.EstruturaClausula on cgect.NomeInformacaoEstruturaClausulaId equals ect2.Id into _ect2
                                                                from ect2 in _ect2.DefaultIfEmpty()
                                                                join iac in _context.InformacaoAdicionalCombos on cgect.TipoinformacaoadicionalId equals iac.AdTipoinformacaoadicionalId into _iac
                                                                from iac in _iac.DefaultIfEmpty()
                                                                join adt in _context.AdTipoinformacaoadicionals on cgect.TipoinformacaoadicionalId equals adt.Cdtipoinformacaoadicional
                                                                where cgect.ClausulaGeralId == cgt.Id
                                                                select new
                                                                {
                                                                    Id = cgect.Id,
                                                                    Linha = cgect.SequenciaLinha,
                                                                    cgect.SequenciaItem,
                                                                    TipoDado = adt.Idtipodado,
                                                                    Codigo = cgect.TipoinformacaoadicionalId,
                                                                    ValorNumero = cgect.Numerico,
                                                                    ValorTexto = cgect.Texto,
                                                                    ValorData = cgect.Data,
                                                                    ValorDescricao = cgect.Descricao,
                                                                    ValorHora = cgect.Hora,
                                                                    OpcoesCombo = iac.Options,
                                                                    ValorCombo = cgect.Combo,
                                                                    ValorPercentual = cgect.Percentual,
                                                                    NomeClasula = ect2.Nome,
                                                                    Titulo = adt.Nmtipoinformacaoadicional
                                                                })
                                                                .AsSplitQuery()
                                                                .ToList()
                                   }).ToListAsync();
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CA1305 // Specify IFormatProvider

            if (clausulas is null)
            {
                return Enumerable.Empty<DocumentoSindicalClausulaVw>();
            }

            var result = new List<DocumentoSindicalClausulaVw>();

            foreach (var clasula in clausulas)
            {
                var documentoSindicalClausula = new DocumentoSindicalClausulaVw
                {
                    DocumentoId = clasula.DocumentoId,
                    EstruturaId = clasula.EstruturaClausulaId,
                    NomeEstruraClausula = clasula.EstruturaClausulaNome,
                    NomeGrupoClausula = clasula.GrupoClausulaNome,
                    Id = clasula.Id,
                    Numero = clasula.Numero,
                    Grupos = new List<Grupos>()
                };

                if (clasula.InformacoesAdicionais is not null && clasula.InformacoesAdicionais.Any())
                {
                    var grupos = from grps in clasula.InformacoesAdicionais
                                 group grps by grps.Linha into clInfoAdicional
                                 select new Grupos
                                 {
                                     Id = clInfoAdicional.Key,
                                     Nome = clInfoAdicional.OrderBy(p => p.SequenciaItem).First().NomeClasula,
                                     InformacoesAdicionais = clInfoAdicional.OrderBy(cl => cl.SequenciaItem).Select(infoAdicional => new InformacaoAdicionalSisap
                                     {
                                         Id = infoAdicional.Id,
                                         Descricao = infoAdicional.Titulo,
                                         SequenciaItem = infoAdicional.SequenciaItem,
                                         SequenciaLinha = clInfoAdicional.Key,
                                         Codigo = infoAdicional.Codigo,
                                         Tipo = infoAdicional.TipoDado,
                                         Dado = new InformacaoAdicionalDado
                                         {
                                             Combo = infoAdicional.OpcoesCombo != null ? new Combo
                                             {
                                                 Opcoes = infoAdicional.OpcoesCombo?.Split(", ") ?? Array.Empty<string>(),
                                                 Valor = infoAdicional.ValorCombo?.Split(", ") ?? Array.Empty<string>()
                                             } : null,
                                             Data = infoAdicional.ValorData,
                                             Descricao = infoAdicional.ValorDescricao,
                                             Hora = infoAdicional.ValorHora,
                                             Nome = infoAdicional.NomeClasula,
                                             Numerico = infoAdicional.ValorNumero,
                                             Percentual = infoAdicional.ValorPercentual,
                                             Texto = infoAdicional.ValorTexto,
                                         }
                                     })
                                 };

                    documentoSindicalClausula.Grupos = grupos;
                } else
                {
                    continue;
                }

                result.Add(documentoSindicalClausula);
            }

            return result;
        }
    }
}
