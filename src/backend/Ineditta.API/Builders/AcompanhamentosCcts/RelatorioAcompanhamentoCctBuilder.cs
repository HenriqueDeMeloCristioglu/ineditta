using System.Globalization;
using System.Text;
using System.Text.Json;

using ClosedXML.Excel;

using CSharpFunctionalExtensions;

using Ineditta.API.Builders.Worksheets;
using Ineditta.API.Configurations;
using Ineditta.API.ViewModels.AcompanhamentosCct.Requests;
using Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.Excel;
using Ineditta.Application.CctsFases.Entities;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.Repository.AcompanhamentosCct.Views.AcompanhamentosCctsRelatorios;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MySqlConnector;

namespace Ineditta.API.Builders.AcompanhamentosCcts
{
  public class RelatorioAcompanhamentoCctBuilder
  {
    private readonly IUserInfoService _userInfoService;
    private readonly InedittaDbContext _context;
    private readonly TemplateExcelConfiguration _templateExcelConfiguration;

    public RelatorioAcompanhamentoCctBuilder(IUserInfoService userInfoService, InedittaDbContext context, IOptions<TemplateExcelConfiguration> templateExcelConfiguration)
    {
      _userInfoService = userInfoService;
      _context = context;
      _templateExcelConfiguration = templateExcelConfiguration?.Value ?? throw new ArgumentNullException(nameof(templateExcelConfiguration));
    }

    public async ValueTask<Result<byte[], Error>> HandleAsync(int grupoEconomicoId, [FromQuery] AcompanhamentoCctRelatorioNegociacoesRequest request)
    {
      var acompanhamentos = await ObterTodos(grupoEconomicoId, request);

      if (acompanhamentos is null)
      {
        return Result.Failure<byte[], Error>(Errors.Http.NotFound());
      }

      var relatorioAcompanhamentos = MergeDataFactory(acompanhamentos);

      if (relatorioAcompanhamentos is null)
      {
        return Result.Failure<byte[], Error>(Errors.Http.NotFound());
      }

      using var workbook = WorksheetBuilder.ReadFrom(_templateExcelConfiguration.RelatorioAcompanhamentoCct);

      var bytes = workbook.GetFirst()
                      .Build(ws =>
                      {
                        var currentLine = 2;

                        relatorioAcompanhamentos
                                  .ToList()
                                  .ForEach(acompanhamento =>
                                  {
                                    ws.Row(currentLine).Height = 50;
                                    int currentColumn = 1;

                                    if (acompanhamento.CodigoSindicatosEstabelecimentos is not null && acompanhamento.CodigoSindicatosEstabelecimentos.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.CodigoSindicatosEstabelecimentos));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.CodigoEmpesa is not null && acompanhamento.CodigoEmpesa.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.CodigoEmpesa));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.Empresas is not null && acompanhamento.Empresas.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.Empresas));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.CodigoEstabelecimento is not null && acompanhamento.CodigoEstabelecimento.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.CodigoEstabelecimento));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.CnpjEstabelecimentos is not null && acompanhamento.CnpjEstabelecimentos.Any())
                                    {
                                      var cnpjFormatado = acompanhamento.CnpjEstabelecimentos.Select(c => CNPJ.Formatar(c));
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", cnpjFormatado));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.NomeEstabelecimentos is not null && acompanhamento.NomeEstabelecimentos.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.NomeEstabelecimentos));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.LocalizacoesEstabelecimentos is not null && acompanhamento.LocalizacoesEstabelecimentos.Any())
                                    {
                                        ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.LocalizacoesEstabelecimentos));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.SiglasSindicatosLaborais is not null && acompanhamento.SiglasSindicatosLaborais.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.SiglasSindicatosLaborais));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.CnpjSindicatosLaborais is not null && acompanhamento.CnpjSindicatosLaborais.Any())
                                    {
                                      var cnpjFormatado = acompanhamento.CnpjSindicatosLaborais.Select(c => CNPJ.Formatar(c));
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", cnpjFormatado));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.NomeSindicatosLaborais is not null && acompanhamento.NomeSindicatosLaborais.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.NomeSindicatosLaborais));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.SiglasSindicatosPatronais is not null && acompanhamento.SiglasSindicatosPatronais.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.SiglasSindicatosPatronais));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.CnpjSindicatosPatronais is not null && acompanhamento.CnpjSindicatosPatronais.Any())
                                    {
                                      var cnpjFormatado = acompanhamento.CnpjSindicatosPatronais.Select(c => CNPJ.Formatar(c));
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", cnpjFormatado));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.NomeSindicatosPatronais is not null && acompanhamento.NomeSindicatosPatronais.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.NomeSindicatosPatronais));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.Municipios is not null && acompanhamento.Municipios.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.Municipios));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.Ufs is not null && acompanhamento.Ufs.Any())
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(string.Join(", ", acompanhamento.Ufs));
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    ws.Cell(currentLine, currentColumn).SetValue(acompanhamento.AtividadeEnconomica);
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    ws.Cell(currentLine, currentColumn).SetValue(acompanhamento.NomeDocumento);
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    ws.Cell(currentLine, currentColumn).SetValue(acompanhamento.DataBase);
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    if (acompanhamento.PeriodoInpc is not null && DateOnly.TryParse(acompanhamento.PeriodoInpc, CultureInfo.InvariantCulture, out DateOnly periodoInpc))
                                    {
                                      ws.Cell(currentLine, currentColumn).SetValue(periodoInpc.ToShortDateString());
                                    }
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    ws.Cell(currentLine, currentColumn).SetValue(acompanhamento.InpcReal);
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    ws.Cell(currentLine, currentColumn).SetValue(acompanhamento.Fase);
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    ws.Cell(currentLine, currentColumn).SetValue(acompanhamento.Observacoes);
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                    currentColumn++;
                                    ws.Cell(currentLine, currentColumn).SetValue(acompanhamento.DataProcessamento is not null ? acompanhamento.DataProcessamento.Value.ToShortDateString() : "");
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, currentColumn).Style.Alignment.WrapText = true;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorderColor = XLColor.Black;
                                    ws.Cell(currentLine, currentColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    currentLine++;
                                  });
                      })
                      .ToByteArray(workbook);



      return Result.Success<byte[], Error>(bytes);
    }

    private async ValueTask<IEnumerable<AcompanhamentoCctRelatorioVw>?> ObterTodos(int grupoEconomicoId, [FromQuery] AcompanhamentoCctRelatorioNegociacoesRequest request)
    {
      var parameters = new List<MySqlParameter>();
      int parametersCount = 0;

      parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
      parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));
      parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));
      parameters.Add(new MySqlParameter("@fase", FasesCct.IndiceFaseArquivada));

      var sql = new StringBuilder(@"select DISTINCT vw.* from acompanhamento_cct_relatorio_vw as vw
                                              inner join usuario_adm uat on uat.email_usuario = @email
                                              left join tipo_doc td on vw.tipo_documento_id = td.idtipo_doc
                                              where vw.fase_id <> @fase
                                              and exists (
	                                                select * from acompanhamento_cct_estabelecimento_tb acet
                                                    WHERE acet.acompanhamento_cct_id = vw.id");

      if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
      {
        QueryBuilder.AppendListToQueryBuilder(sql, request.GruposEconomicosIds, "acet.grupo_economico_id", parameters, ref parametersCount);
      }

      if (request.EmpresasIds is not null && request.EmpresasIds.Any())
      {
        QueryBuilder.AppendListToQueryBuilder(sql, request.EmpresasIds, "acet.empresa_id", parameters, ref parametersCount);
      }

      if (request.EstabelecimentosIds is not null && request.EstabelecimentosIds.Any())
      {
        QueryBuilder.AppendListToQueryBuilder(sql, request.EstabelecimentosIds, "acet.estabelecimento_id", parameters, ref parametersCount);
      }

      sql.Append(@") ");

      if (request.AtividadesEconomicasIds is not null && request.AtividadesEconomicasIds.Any())
      {
        parameters.Add(new MySqlParameter("@atividadesEconomicasIds", JsonSerializer.Serialize(request.AtividadesEconomicasIds)));

        sql.Append(@" and JSON_OVERLAPS(vw.cnaes_ids, @atividadesEconomicasIds) ");
      }

      if (request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any())
      {
        sql.Append(@" and exists (
	                                SELECT 1 from acompanhamento_cct_sindicato_laboral_tb acslt
	                                inner join sind_emp se on acslt.sindicato_id = se.id_sinde
	                                WHERE acslt.acompanhamento_cct_id = vw.id");

        QueryBuilder.AppendListToQueryBuilder(sql, request.SindicatosLaboraisIds, "acslt.sindicato_id", parameters, ref parametersCount);

        sql.Append(@") ");
      }

      if (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any())
      {
        sql.Append(@" and exists (
	                                SELECT 1 from acompanhamento_cct_sindicato_patronal_tb acspt
	                                inner join sind_patr sp on acspt.sindicato_id = sp.id_sindp
	                                WHERE acspt.acompanhamento_cct_id = vw.id");

        QueryBuilder.AppendListToQueryBuilder(sql, request.SindicatosPatronaisIds, "acspt.sindicato_id", parameters, ref parametersCount);

        sql.Append(@") ");
      }

      if (request.DatasBases is not null && request.DatasBases.Any())
      {
        QueryBuilder.AppendListToQueryBuilder(sql, request.DatasBases, "vw.data_base", parameters, ref parametersCount);
      }

      if (request.NomeDocumento is not null && request.NomeDocumento.Any())
      {
        QueryBuilder.AppendListToQueryBuilder(sql, request.NomeDocumento, "td.idtipo_doc", parameters, ref parametersCount);
      }

      if (request.TipoLocalizacao is not null && request.Localizacoes is not null && request.Localizacoes.Any())
      {
        sql.Append(@" and exists (
                      select 1 from acompanhamento_cct_localizacao_tb aclt
                      inner join localizacao l on l.id_localizacao = aclt.localizacao_id
                      where aclt.acompanhamento_cct_id = vw.id");

        if (request.TipoLocalizacao == "uf")
        {
          QueryBuilder.AppendListToQueryBuilder(sql, request.Localizacoes, "l.uf", parameters, ref parametersCount);
        }

        if (request.TipoLocalizacao == "regiao")
        {
          QueryBuilder.AppendListToQueryBuilder(sql, request.Localizacoes, "l.regiao", parameters, ref parametersCount);
        }

        if (request.TipoLocalizacao == "municipio")
        {
          QueryBuilder.AppendListToQueryBuilder(sql, request.Localizacoes, "l.municipio", parameters, ref parametersCount);
        }

        sql.Append(@") ");
            }

        if (request.Fases is not null && request.Fases.Any())
        {
            QueryBuilder.AppendListToQueryBuilder(sql, request.Fases, "vw.fase_id", parameters, ref parametersCount);
        }

        if (request.DataProcessamentoInicial is not null && request.DataProcessamentoFinal is not null)
        {
    #pragma warning disable CA1305 // Specify IFormatProvider
            parameters.Add(new MySqlParameter("@dataProcessamentoInicial", request.DataProcessamentoInicial.Value.ToString("yyyy-MM-dd")));
            parameters.Add(new MySqlParameter("@dataProcessamentoFinal", request.DataProcessamentoFinal.Value.ToString("yyyy-MM-dd")));

            sql.Append(@" and vw.data_processamento >= @dataProcessamentoInicial AND vw.data_processamento <= @dataProcessamentoFinal");
    #pragma warning restore CA1305 // Specify IFormatProvider
        }

            var result = (await _context.AcompanhamentoCctRelatorioVw
                      .FromSqlRaw(sql.ToString(), parameters.ToArray())
                      .AsNoTracking()
                      .ToListAsync());

      if (result == null)
      {
        return null;
      }

      var acompanhamentosPorGrupo = new List<AcompanhamentoCctRelatorioVw>();

      foreach (var a in result)
      {
        if (a.Estabelecimentos is not null)
        {
          a.Estabelecimentos = FiltrarEstabelecimentosPorGrupoEconomico(a.Estabelecimentos, grupoEconomicoId);
        }

        acompanhamentosPorGrupo.Add(a);
      }

      return acompanhamentosPorGrupo;

      static IEnumerable<EstatabelecimentoRelatorio> FiltrarEstabelecimentosPorGrupoEconomico(IEnumerable<EstatabelecimentoRelatorio> estabelecimentos, int grupoEconomicoId)
      {
        var estabelecimentosFiltrados = new List<EstatabelecimentoRelatorio>();

        foreach (var estabelecimento in estabelecimentos)
        {
          if (estabelecimento.GrupoEconomicoId is not null && Int32.TryParse(estabelecimento.GrupoEconomicoId, out int grupo) && (grupoEconomicoId == grupo || grupoEconomicoId == 0))
          {
            estabelecimentosFiltrados.Add(estabelecimento);
          }
        }

        return estabelecimentosFiltrados;
      }
    }

    private static IEnumerable<RelatorioAcompanhamentoCctViewModel>? MergeDataFactory(IEnumerable<AcompanhamentoCctRelatorioVw> acompanhamentos)
    {
      var list = new List<RelatorioAcompanhamentoCctViewModel>();

      foreach (var acompanhamento in acompanhamentos)
      {
        var nomeEstabelecimentos = new List<string>();
        var cnpjEstabelecimentos = new List<string>();
        var codigoSindicatosEstabelecimentos = new List<string>();
        var codigosEmpresa = new List<string>();
        var nomesEmpresa = new List<string>();
        var siglaLaborais = new List<string>();
        var cnpjLaborais = new List<string>();
        var nomeLaborais = new List<string>();
        var siglaPatronais = new List<string>();
        var cnpjPatronais = new List<string>();
        var nomePatronais = new List<string>();
        var codigoEstabelecimentos = new List<string>();
        var localizacoesEstabelecimentos = new List<string>();

        if (acompanhamento.Estabelecimentos is not null && acompanhamento.Estabelecimentos.Any())
        {
          foreach (var estabelecimento in acompanhamento.Estabelecimentos)
          {
            if (estabelecimento.Nome is not null && estabelecimento.Nome.Any())
            {
              nomeEstabelecimentos.Add(estabelecimento.Nome);
            }

            if (estabelecimento.Cnpj is not null && estabelecimento.Cnpj.Any())
            {
              cnpjEstabelecimentos.Add(estabelecimento.Cnpj);
            }

            if (estabelecimento.CodigoSindicatoCliente is not null && estabelecimento.CodigoSindicatoCliente.Any())
            {
              codigoSindicatosEstabelecimentos.Add(estabelecimento.CodigoSindicatoCliente);
            }

            if (estabelecimento.CodigoEmpresa is not null && estabelecimento.CodigoEmpresa.Any())
            {
              codigosEmpresa.Add(estabelecimento.CodigoEmpresa);
            }

            if (estabelecimento.NomeEmpresa is not null && estabelecimento.NomeEmpresa.Any())
            {
              nomesEmpresa.Add(estabelecimento.NomeEmpresa);
            }

            if (estabelecimento.CodigoEstabelecimento is not null && estabelecimento.CodigoEstabelecimento.Any())
            {
              codigoEstabelecimentos.Add(estabelecimento.CodigoEstabelecimento);
            }

            if (!string.IsNullOrEmpty(estabelecimento.LocalizacaoEstabelecimento))
            {
              localizacoesEstabelecimentos.Add(estabelecimento.LocalizacaoEstabelecimento);
            }
          }
        }

        if (acompanhamento.SindicatosLaborais is not null && acompanhamento.SindicatosLaborais.Any())
        {
          foreach (var sindicatoLaboral in acompanhamento.SindicatosLaborais)
          {
            if (sindicatoLaboral.Sigla is not null)
            {
              siglaLaborais.Add(sindicatoLaboral.Sigla);
            }

            if (sindicatoLaboral.Cnpj is not null)
            {
              cnpjLaborais.Add(sindicatoLaboral.Cnpj);
            }

            if (sindicatoLaboral.Nome is not null)
            {
              nomeLaborais.Add(sindicatoLaboral.Nome);
            }
          }
        }

        if (acompanhamento.SindicatosPatronais is not null && acompanhamento.SindicatosPatronais.Any())
        {
          foreach (var sindicatoPatronal in acompanhamento.SindicatosPatronais)
          {
            if (sindicatoPatronal.Sigla is not null)
            {
              siglaPatronais.Add(sindicatoPatronal.Sigla);
            }

            if (sindicatoPatronal.Cnpj is not null)
            {
              cnpjPatronais.Add(sindicatoPatronal.Cnpj);
            }

            if (sindicatoPatronal.Nome is not null)
            {
              nomePatronais.Add(sindicatoPatronal.Nome);
            }
          }
        }

        var acompanhamentoItem = new RelatorioAcompanhamentoCctViewModel
        {
          NomeEstabelecimentos = nomeEstabelecimentos.Distinct(),
          CnpjEstabelecimentos = cnpjEstabelecimentos.Distinct(),
          CodigoSindicatosEstabelecimentos = codigoSindicatosEstabelecimentos.Distinct(),
          SiglasSindicatosLaborais = siglaLaborais.Distinct(),
          CnpjSindicatosLaborais = cnpjLaborais.Distinct(),
          SiglasSindicatosPatronais = siglaPatronais.Distinct(),
          CnpjSindicatosPatronais = cnpjPatronais.Distinct(),
          AtividadeEnconomica = acompanhamento.AtividadesEconomicas,
          NomeDocumento = acompanhamento.NomeDocumento,
          DataBase = acompanhamento.DataBase,
          PeriodoInpc = acompanhamento.PeriodoAnterior,
          InpcReal = acompanhamento.DadoReal,
          Fase = acompanhamento.Fase,
          Observacoes = acompanhamento.ObservacoesGerais,
          DataProcessamento = acompanhamento.DataProcessamento,
          CodigoEmpesa = codigosEmpresa.Distinct(),
          Empresas = nomesEmpresa.Distinct(),
          Ufs = acompanhamento.Ufs != null ? acompanhamento.Ufs.Distinct() : Enumerable.Empty<string>(),
          Municipios = acompanhamento.Municipios != null ? acompanhamento.Municipios.Distinct() : Enumerable.Empty<string>(),
          NomeSindicatosLaborais = nomeLaborais.Distinct(),
          NomeSindicatosPatronais = nomePatronais.Distinct(),
          CodigoEstabelecimento = codigoEstabelecimentos.Distinct(),
          LocalizacoesEstabelecimentos = localizacoesEstabelecimentos.Distinct()
        };

        list.Add(acompanhamentoItem);
      }

      return list;
    }
  }
}
