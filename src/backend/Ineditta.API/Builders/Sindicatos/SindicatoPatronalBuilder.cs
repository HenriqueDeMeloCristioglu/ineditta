using CSharpFunctionalExtensions;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.API.Factories.Sindicatos;
using ClosedXML.Excel;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.Application.Sindicatos.Base.ValueObjects;
using Ineditta.API.ViewModels.Sindicatos.Dtos;
using Ineditta.API.ViewModels.Sindicatos.ViewModels;

namespace Ineditta.API.Builders.Sindicatos
{
    public class SindicatoPatronalBuilder
    {
        private readonly SindicatoPatronalRelatorioFactory _sindicatoPatronalFactory;

        public SindicatoPatronalBuilder(SindicatoPatronalRelatorioFactory sindicatoPatronalFactory)
        {
            _sindicatoPatronalFactory = sindicatoPatronalFactory;
        }

        public async ValueTask<Result<byte[], Error>> HandleAsync(SindicatosRequest request, CancellationToken cancellationToken = default)
        {
            var sindicatos = await _sindicatoPatronalFactory.CriarAsync(request);

            if (sindicatos is null)
            {
                return Result.Failure<byte[], Error>(Errors.Http.NotFound());
            }

            var excel = Criar(sindicatos);

            if (excel.IsFailure)
            {
                return excel;
            }

            return excel;
        }

        private static Result<byte[], Error> Criar(IEnumerable<RelatorioSindicatoViewModel> sindicatos)
        {
            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Sindicatos Patronais");

            worksheet.ShowGridLines = false;

            var headersPadroes = ObterColunasPadroes();

            for (int i = 0; i < headersPadroes.Count; i++)
            {
                var cell = worksheet.Cell(1, (i + 1)).SetValue(headersPadroes[i]);

                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Alignment.WrapText = true;
                cell.WorksheetColumn().Width = 30;
                cell.WorksheetRow().Height = 25;
            }

            var linhaAtual = 2;

            foreach (var sindicato in sindicatos)
            {
                worksheet.Cell(linhaAtual, 1).SetValue(sindicato.SindPatronalSituacao);
                worksheet.Cell(linhaAtual, 2).SetValue(CNPJ.Formatar(sindicato.SindPatronalCnpj!));
                worksheet.Cell(linhaAtual, 3).SetValue(CodigoSindical.Formatar(sindicato.SindPatronalCodigo!));
                worksheet.Cell(linhaAtual, 4).SetValue(sindicato.SindPatronalSigla);
                worksheet.Cell(linhaAtual, 5).SetValue(sindicato.SindPatronalRazao);
                worksheet.Cell(linhaAtual, 6).SetValue(sindicato.SindPatronalDenominacao);
                worksheet.Cell(linhaAtual, 7).SetValue(sindicato.SindPatronalLogradouro);
                worksheet.Cell(linhaAtual, 8).SetValue(sindicato.SindPatronalMunicipio);
                worksheet.Cell(linhaAtual, 9).SetValue(sindicato.SindPatronalUf);
                worksheet.Cell(linhaAtual, 10).SetValue(Telefone.Formatar(sindicato.SindPatronalFone1!));
                worksheet.Cell(linhaAtual, 11).SetValue(Telefone.Formatar(sindicato.SindPatronalFone2!));
                worksheet.Cell(linhaAtual, 12).SetValue(Telefone.Formatar(sindicato.SindPatronalFone3!));
                worksheet.Cell(linhaAtual, 13).SetValue(sindicato.SindPatronalRamal);
                worksheet.Cell(linhaAtual, 14).SetValue(sindicato.SindPatronalNegociador);
                worksheet.Cell(linhaAtual, 15).SetValue(sindicato.SindPatronalContribuicao);
                worksheet.Cell(linhaAtual, 16).SetValue(sindicato.SindPatronalEnquadramento);
                worksheet.Cell(linhaAtual, 17).SetValue(sindicato.SindPatronalEmail1);
                worksheet.Cell(linhaAtual, 18).SetValue(sindicato.SindPatronalEmail2);
                worksheet.Cell(linhaAtual, 19).SetValue(sindicato.SindPatronalEmail3);
                worksheet.Cell(linhaAtual, 20).SetValue(sindicato.SindPatronalSite);
                worksheet.Cell(linhaAtual, 21).SetValue(sindicato.SindPatronalTwitter);
                worksheet.Cell(linhaAtual, 22).SetValue(sindicato.SindPatronalFacebook);
                worksheet.Cell(linhaAtual, 23).SetValue(sindicato.SindPatronalInstagram);
                worksheet.Cell(linhaAtual, 24).SetValue(sindicato.SindPatronalGrau);
                worksheet.Cell(linhaAtual, 25).SetValue(sindicato.SindPatronalStatus);
                worksheet.Cell(linhaAtual, 26).SetValue(CNPJ.Formatar(sindicato.FederacaoPatronalCnpj!));
                worksheet.Cell(linhaAtual, 27).SetValue(sindicato.FederacaoPatronalSigla);
                worksheet.Cell(linhaAtual, 28).SetValue(sindicato.FederacaoPatronalNome);
                worksheet.Cell(linhaAtual, 29).SetValue(CNPJ.Formatar(sindicato.ConfederacaoPatronalCnpj!));
                worksheet.Cell(linhaAtual, 30).SetValue(sindicato.ConfederacaoPatronalSigla);
                worksheet.Cell(linhaAtual, 31).SetValue(sindicato.ConfederacaoPatronalNome);

                linhaAtual++;
            }

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        private static List<string> ObterColunasPadroes()
        {
            return new List<string>
            {
                "Situação Cadastro Sindicato Patronal",
                "CNPJ Sindicato Patronal",
                "Código Sindical Sindicato Patronal",
                "Sigla Sindicato Patronal",
                "Razão Social Sindicato Patronal",
                "Denominação Sindicato Patronal",
                "Endereço e Número Sindicato Patronal",
                "Município Sindicato Patronal",
                "UF Sindicato Patronal",
                "Fone1 Sindicato Patronal",
                "Fone2 Sindicato Patronal",
                "Fone3 Sindicato Patronal",
                "Ramal Sindicato Patronal",
                "Respons. Negociação Sindicato Patronal",
                "Respons. Contribuição Sindicato Patronal",
                "Respons. Enquadramento Sindicato Patronal",
                "Email1 Sindicato Patronal",
                "Email2 Sindicato Patronal",
                "Email3 Sindicato Patronal",
                "Site Sindicato Patronal",
                "Status Sindicato Patronal",
                "Twitter Sindicato Patronal",
                "Facebook Sindicato Patronal",
                "Instagram Sindicato Patronal",
                "Grau Sindicato Patronal",
                "CNPJ Federação Sindicato Patronal",
                "Sigla Federação Sindicato Patronal",
                "Nome Federação Sindicato Patronal",
                "CNPJ Confederação Sindicato Patronal",
                "Sigla Confederação Sindicato Patronal",
                "Nome Confederação Sindicato Patronal"
            };
        }
    }
}
