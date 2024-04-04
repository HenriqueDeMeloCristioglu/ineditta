import DateParser from "../../utils/date/date-parser"
import { MediaType } from "../../utils/web"

export class ClausulaService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService
    this.apiLegadoService = apiLegadoService
    this.url = "v1/clausulas"
    this.urlLegado = "includes/php/ajax.php"
  }

  async obterDatatable(params) {
    if (params?.dataProcessamentoInicial && params?.dataProcessamentoInicial instanceof Date) {
      params.dataProcessamentoInicial = DateParser.toString(params?.dataProcessamentoInicial)
    }

    if (params?.dataProcessamentoFinal && params?.dataProcessamentoFinal instanceof Date) {
      params.dataProcessamentoFinal = DateParser.toString(params?.dataProcessamentoFinal)
    }

    return await this.apiService.post(`${this.url}`, params, MediaType.dataTable)
  }

  async obterDatasBases(data) {
    const result = await this.apiService.get(`${this.url}/datas-bases`, data, MediaType.select);
    const value = result.value;
    value.unshift({ id: 'ultimo-ano', description: 'Último ano'}, { id: 'vigente', description: 'Vigente'})
    return value;
  }

  async obterPorId(ids) {
    const params = {
      clausulaIds: ids,
    }
    return await this.apiService.post(`${this.url}/por-id`, params, MediaType.json)
  }

  async obterComentariosPorId(ids, grupoEconomicoId) {
    const params = {
      clausulaIds: ids,
      grupoEconomicoId
    }
    return await this.apiService.post(`${this.url}/comentarios-por-id`, params, MediaType.json)
  }

  async postLegado(data) {
    return await this.apiLegadoService.post(this.urlLegado, data, MediaType.urlencoded)
  }

  async obterVigenciaPorDoc(params) {
    return await this.apiService.get(`${this.url}/vigencia-select`, params, MediaType.select)
  }

  async obterNomeDocsPorSindicato(params) {
    return await this.apiService.get(`${this.url}/nome-doc-select`, params, MediaType.select)
  }

  async obterComparacaoClausulas(params) {
    return await this.apiService.get(`${this.url}/comparacao`, params, MediaType.json);
  }

  async gerarPdfClausulas(data) {
    return await this.apiService.download(`${this.url}/pdf-clausula`, data)
  }

  async gerarPdfComparativoClausulas(data) {
    return await this.apiService.download(`${this.url}/pdf-comparacao`, data)
  }

  async obterSindPatronalPorLaboral(params) {
    return await this.apiService.get(`${this.url}/sind-patronal-select`, params, MediaType.select)
  }

  async obterRelatorioClausulasExcel(params) {
    return await this.apiService.download(`${this.url}/relatorios`, params, MediaType.excel);
  }
}
