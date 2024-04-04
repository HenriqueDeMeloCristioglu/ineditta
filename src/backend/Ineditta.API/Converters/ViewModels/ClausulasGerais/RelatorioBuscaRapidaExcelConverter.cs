using Ineditta.API.ViewModels.ClausulasGerais.ViewModels.Excel;

namespace Ineditta.API.Converters.ViewModels.ClausulasGerais
{
    public static class RelatorioBuscaRapidaExcelConverter
    {
        public static IEnumerable<RelatorioBuscaRapidaExcelViewModel> Criar (IEnumerable<RelatorioClausulasBuscaRapidaExcelViewModel> clausulas)
        {
            List<RelatorioBuscaRapidaExcelViewModel> documentos = new();

            RelatorioBuscaRapidaExcelViewModel novoDocumento = new();
            List<ClausulaRelatorioBuscaRapidaViewModel> novasClausulas = new();

            var documentoAtual = 0;

            foreach (var item in clausulas)
            {
                if (documentoAtual != 0 && documentoAtual != item.DocumentoId)
                {
                    novoDocumento.Clausulas = novasClausulas;
                    documentos.Add(novoDocumento);

                    novasClausulas = new List<ClausulaRelatorioBuscaRapidaViewModel>();
                }

                novoDocumento = new RelatorioBuscaRapidaExcelViewModel
                {
                    DocumentoId = item.DocumentoId,
                    CodigosSindicatoCliente = item.CodigosSindicatoCliente,
                    CodigosUnidades = item.CodigosUnidades,
                    CnpjsUnidades = item.CnpjsUnidades,
                    UfsUnidades = item.UfsUnidades,
                    MunicipiosUnidades = item.MunicipiosUnidades,
                    SiglasSindicatosLaborais = item.SiglasSindicatosLaborais,
                    CnpjsSindicatosLaborais = item.CnpjsSindicatosLaborais,
                    DenominacoesSindicatosLaborais = item.DenominacoesSindicatosLaborais,
                    SiglasSindicatosPatronais = item.SiglasSindicatosPatronais,
                    CnpjsSindicatosPatronais = item.CnpjsSindicatosPatronais,
                    DenominacoesSindicatosPatronais = item.DenominacoesSindicatosPatronais,
                    DataLiberacaoClausulas = item.DataLiberacaoClausulas,
                    NomeDocumento = item.NomeDocumento,
                    AtividadesEconomicasDocumentoString = item.AtividadesEconomicasDocumentoString,
                    ValidadeInicialDocumento = item.ValidadeInicialDocumento,
                    ValidadeFinalDocumento = item.ValidadeFinalDocumento,
                    DatabaseDocumento = item.DatabaseDocumento,
                    AbrangenciaDocumentoString = item.AbrangenciaDocumentoString,
                    DataAssinaturaDocumento = item.DataAssinaturaDocumento
                };

                var novaClausula = new ClausulaRelatorioBuscaRapidaViewModel
                {
                    Id = item.ClausulaId,
                    Nome = item.NomeClausula,
                    NomeGrupo = item.NomeGrupoClausula,
                    Numero = item.NumeroClausula,
                    Texto = item.TextoClausula,
                    EstruturaClausulaId = item.EstruturaClausulaId,
                    DocumentoId = item.DocumentoId
                };

                novasClausulas.Add(novaClausula);
                documentoAtual = item.DocumentoId;
            }

            novoDocumento.Clausulas = novasClausulas;
            documentos.Add(novoDocumento);

            return documentos;
        }
    }
}
