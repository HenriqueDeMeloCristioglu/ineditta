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
    public class SindicatoLaboralBuilder
    {
        private readonly SindicatoLaboralRelatorioFactory _sindicatoLaboralFactory;

        public SindicatoLaboralBuilder(SindicatoLaboralRelatorioFactory sindicatoLaboralFactory)
        {
            _sindicatoLaboralFactory = sindicatoLaboralFactory;
        }

        public async ValueTask<Result<byte[], Error>> HandleAsync(SindicatosRequest request, CancellationToken cancellationToken = default)
        {
            var sindicatos = await _sindicatoLaboralFactory.CriarAsync(request);

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

            var worksheet = workbook.Worksheets.Add("Sindicatos Laborais");

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
                worksheet.Cell(linhaAtual, 1).SetValue(sindicato.SindLaboralSituacao);
                worksheet.Cell(linhaAtual, 2).SetValue(CNPJ.Formatar(sindicato.SindLaboralCnpj!));
                worksheet.Cell(linhaAtual, 3).SetValue(CodigoSindical.Formatar(sindicato.SindLaboralCodigo!));
                worksheet.Cell(linhaAtual, 4).SetValue(sindicato.SindLaboralSigla);
                worksheet.Cell(linhaAtual, 5).SetValue(sindicato.SindLaboralRazao);
                worksheet.Cell(linhaAtual, 6).SetValue(sindicato.SindLaboralDenominacao);
                worksheet.Cell(linhaAtual, 7).SetValue(sindicato.SindLaboralLogradouro);
                worksheet.Cell(linhaAtual, 8).SetValue(sindicato.SindLaboralMunicipio);
                worksheet.Cell(linhaAtual, 9).SetValue(sindicato.SindLaboralUf);
                worksheet.Cell(linhaAtual, 10).SetValue(sindicato.SindLaboralDataNegociacao);
                worksheet.Cell(linhaAtual, 11).SetValue(Telefone.Formatar(sindicato.SindLaboralFone1!));
                worksheet.Cell(linhaAtual, 12).SetValue(Telefone.Formatar(sindicato.SindLaboralFone2!));
                worksheet.Cell(linhaAtual, 13).SetValue(Telefone.Formatar(sindicato.SindLaboralFone3!));
                worksheet.Cell(linhaAtual, 14).SetValue(sindicato.SindLaboralRamal);
                worksheet.Cell(linhaAtual, 15).SetValue(sindicato.SindLaboralNegociador);
                worksheet.Cell(linhaAtual, 16).SetValue(sindicato.SindLaboralContribuicao);
                worksheet.Cell(linhaAtual, 17).SetValue(sindicato.SindLaboralEnquadramento);
                worksheet.Cell(linhaAtual, 18).SetValue(sindicato.SindLaboralEmail1);
                worksheet.Cell(linhaAtual, 19).SetValue(sindicato.SindLaboralEmail2);
                worksheet.Cell(linhaAtual, 20).SetValue(sindicato.SindLaboralEmail3);
                worksheet.Cell(linhaAtual, 21).SetValue(sindicato.SindLaboralSite);
                worksheet.Cell(linhaAtual, 22).SetValue(sindicato.SindLaboralTwitter);
                worksheet.Cell(linhaAtual, 23).SetValue(sindicato.SindLaboralFacebook);
                worksheet.Cell(linhaAtual, 24).SetValue(sindicato.SindLaboralInstagram);
                worksheet.Cell(linhaAtual, 25).SetValue(sindicato.SindLaboralGrau);
                worksheet.Cell(linhaAtual, 26).SetValue(sindicato.SindLaboralStatus);
                worksheet.Cell(linhaAtual, 27).SetValue(CNPJ.Formatar(sindicato.CentralSindicalCnpj!));
                worksheet.Cell(linhaAtual, 28).SetValue(sindicato.CentralSindicalSigla);
                worksheet.Cell(linhaAtual, 29).SetValue(sindicato.CentralSindicalNome);
                worksheet.Cell(linhaAtual, 30).SetValue(CNPJ.Formatar(sindicato.FederacaoLaboralCnpj!));
                worksheet.Cell(linhaAtual, 31).SetValue(sindicato.FederacaoLaboralSigla);
                worksheet.Cell(linhaAtual, 32).SetValue(sindicato.FederacaoLaboralNome);
                worksheet.Cell(linhaAtual, 33).SetValue(CNPJ.Formatar(sindicato.ConfederacaoLaboralCnpj!));
                worksheet.Cell(linhaAtual, 34).SetValue(sindicato.ConfederacaoLaboralSigla);
                worksheet.Cell(linhaAtual, 35).SetValue(sindicato.ConfederacaoLaboralNome);

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
                "Situação Cadastro Sindicato Laboral",
                "CNPJ Sindicato Laboral",
                "Código Sindical Sindicato Laboral",
                "Sigla Sindicato Laboral",
                "Razão Social Sindicato Laboral",
                "Denominação Sindicato Laboral",
                "Endereço e Número Sindicato Laboral",
                "Município Sindicato Laboral",
                "UF Sindicato Laboral",
                "Data-base Sindicato Laboral",
                "Fone1 Sindicato Laboral",
                "Fone2 Sindicato Laboral",
                "Fone3 Sindicato Laboral",
                "Ramal Sindicato Laboral",
                "Respons. Negociação Sindicato Laboral",
                "Respons. Contribuição Sindicato Laboral",
                "Respons. Enquadramento Sindicato Laboral",
                "Email1 Sindicato Laboral",
                "Email2 Sindicato Laboral",
                "Email3 Sindicato Laboral",
                "Site Sindicato Laboral",
                "Twitter Sindicato Laboral",
                "Facebook Sindicato Laboral",
                "Instagram Sindicato Laboral",
                "Grau Sindicato Laboral",
                "Status Sindicato Laboral",
                "CNPJ Central Sindical Sindicato Laboral",
                "Sigla Central Sindical Sindicato Laboral",
                "Nome Central Sindical Sindicato Laboral",
                "CNPJ Federação Sindicato Laboral",
                "Sigla Federação Sindicato Laboral",
                "Nome Federação Sindicato Laboral",
                "CNPJ Confederação Sindicato Laboral",
                "Sigla Confederação Sindicato Laboral",
                "Nome Confederação Sindicato Laboral"
            };
        }
    }
}
