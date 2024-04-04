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
    public class SindicatoBuilder
    {
        private readonly SindicatoFactory _sindicatoFactory;

        public SindicatoBuilder(SindicatoFactory sindicatoFactory)
        {
            _sindicatoFactory = sindicatoFactory;
        }

        public async ValueTask<Result<byte[], Error>> HandleAsync(SindicatosRequest request, CancellationToken cancellationToken = default)
        {
            var sindicatos = await _sindicatoFactory.CriarAsync(request);

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

            var worksheet = workbook.Worksheets.Add("Sindicatos");

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
                worksheet.Cell(linhaAtual, 36).SetValue(sindicato.SindPatronalSituacao);
                worksheet.Cell(linhaAtual, 37).SetValue(CNPJ.Formatar(sindicato.SindPatronalCnpj!));
                worksheet.Cell(linhaAtual, 38).SetValue(CodigoSindical.Formatar(sindicato.SindPatronalCodigo!));
                worksheet.Cell(linhaAtual, 39).SetValue(sindicato.SindPatronalSigla);
                worksheet.Cell(linhaAtual, 40).SetValue(sindicato.SindPatronalRazao);
                worksheet.Cell(linhaAtual, 41).SetValue(sindicato.SindPatronalDenominacao);
                worksheet.Cell(linhaAtual, 42).SetValue(sindicato.SindPatronalLogradouro);
                worksheet.Cell(linhaAtual, 43).SetValue(sindicato.SindPatronalMunicipio);
                worksheet.Cell(linhaAtual, 44).SetValue(sindicato.SindPatronalUf);
                worksheet.Cell(linhaAtual, 45).SetValue(Telefone.Formatar(sindicato.SindPatronalFone1!));
                worksheet.Cell(linhaAtual, 46).SetValue(Telefone.Formatar(sindicato.SindPatronalFone2!));
                worksheet.Cell(linhaAtual, 47).SetValue(Telefone.Formatar(sindicato.SindPatronalFone3!));
                worksheet.Cell(linhaAtual, 48).SetValue(sindicato.SindPatronalRamal);
                worksheet.Cell(linhaAtual, 49).SetValue(sindicato.SindPatronalNegociador);
                worksheet.Cell(linhaAtual, 50).SetValue(sindicato.SindPatronalContribuicao);
                worksheet.Cell(linhaAtual, 51).SetValue(sindicato.SindPatronalEnquadramento);
                worksheet.Cell(linhaAtual, 52).SetValue(sindicato.SindPatronalEmail1);
                worksheet.Cell(linhaAtual, 53).SetValue(sindicato.SindPatronalEmail2);
                worksheet.Cell(linhaAtual, 54).SetValue(sindicato.SindPatronalEmail3);
                worksheet.Cell(linhaAtual, 55).SetValue(sindicato.SindPatronalSite);
                worksheet.Cell(linhaAtual, 56).SetValue(sindicato.SindPatronalTwitter);
                worksheet.Cell(linhaAtual, 57).SetValue(sindicato.SindPatronalFacebook);
                worksheet.Cell(linhaAtual, 58).SetValue(sindicato.SindPatronalInstagram);
                worksheet.Cell(linhaAtual, 59).SetValue(sindicato.SindPatronalGrau);
                worksheet.Cell(linhaAtual, 60).SetValue(sindicato.SindPatronalStatus);
                worksheet.Cell(linhaAtual, 61).SetValue(CNPJ.Formatar(sindicato.FederacaoPatronalCnpj!));
                worksheet.Cell(linhaAtual, 62).SetValue(sindicato.FederacaoPatronalSigla);
                worksheet.Cell(linhaAtual, 63).SetValue(sindicato.FederacaoPatronalNome);
                worksheet.Cell(linhaAtual, 64).SetValue(CNPJ.Formatar(sindicato.ConfederacaoPatronalCnpj!));
                worksheet.Cell(linhaAtual, 65).SetValue(sindicato.ConfederacaoPatronalSigla);
                worksheet.Cell(linhaAtual, 66).SetValue(sindicato.ConfederacaoPatronalNome);

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
                "Nome Confederação Sindicato Laboral",
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
