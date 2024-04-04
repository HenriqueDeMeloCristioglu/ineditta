import DateParser from "../../utils/date/date-parser";
import { MediaType } from "../../utils/web";

export class ClausulaGeralService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService;
    this.apiLegadoService = apiLegadoService;
    this.url = "v1/clausulas-gerais";
  }

  async obterSelect() {
    return await this.apiService.get(`${this.url}`, null, MediaType.select);
  }

  async incluir(clausula) {
    return await this.apiService.post(this.url, clausula, MediaType.json);
  }

  async editar({ id, body }) {
    return await this.apiService.put(`${this.url}/${id}`, body, MediaType.json);
  }

  async aprovar(id) {
    return await this.apiService.put(`${this.url}/aprovar/${id}`, null, MediaType.json);
  }

  async liberar(documentoId) {
    return await this.apiService.patch(`${this.url}/${documentoId}/liberar`, null, MediaType.json);
  }

  async atualizarResumo({ id, body }) {
    return await this.apiService.patch(`${this.url}/${id}/texto-resumido`, body, MediaType.json);
  }

  async excluir(data) {
    return await this.apiLegadoService.post("includes/php/ajax.php", data, MediaType.urlencoded);
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`);
  }

  async postLegado(data) {
    return await this.apiLegadoService.post("includes/php/ajax.php", data, MediaType.urlencoded);
  }

  async obterPorTipoDocumento(id) {
    return await this.apiService.get(`${this.url}/documentos/${id}`, MediaType.json);
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable);
  }

  async obterDatatableClausulasPorDocumento({ id, request }) {
    return await this.apiService.get(`${this.url}/documentos/${id}`, request, MediaType.dataTable);
  }

  async obterPorDocumento(id) {
    return await this.apiService.get(`${this.url}/documentos/${id}/informacoes-adicionais`, null, MediaType.json)
  }

  async obterInformacoesAdicionaisPorClausulaId(id) {
    return await this.apiService.get(`${this.url}/informacoes-adicionais/${id}`, null, MediaType.json)
  }

  async enviarEmailClausulaAprovada(body) {
    return await this.apiService.post(`${this.url}/email-aprovado`, body, MediaType.json);
  }

  async downloadRelatorioClausulas({ documentoId }) {
    return await this.apiService.download(`${this.url}/relatorios-clausulas`, { documentoId }, MediaType.stream)
  }

  async obterResumiveisPorDocumento(id) {
    return await this.apiService.get(`${this.url}/resumiveis/${id}`, null, MediaType.json)
  }

  async gerarResumo(id) {
    return await this.apiService.post(`${this.url}/gerar-resumo/${id}`, null, MediaType.json);
  }

  async gerarPdfBuscaRapida(data) {
    if (data?.dataProcessamentoInicial && data?.dataProcessamentoInicial instanceof Date) {
      data.dataProcessamentoInicial = DateParser.toString(data?.dataProcessamentoInicial)
    }

    if (data?.dataProcessamentoFinal && data?.dataProcessamentoFinal instanceof Date) {
      data.dataProcessamentoFinal = DateParser.toString(data?.dataProcessamentoFinal)
    }

    return await this.apiService.download(`${this.url}/relatorio-busca-rapida`, data, MediaType.pdf)
  }

  async gerarExcelBuscaRapida(data) {
    if (data?.dataProcessamentoInicial && data?.dataProcessamentoInicial instanceof Date) {
      data.dataProcessamentoInicial = DateParser.toString(data?.dataProcessamentoInicial)
    }

    if (data?.dataProcessamentoFinal && data?.dataProcessamentoFinal instanceof Date) {
      data.dataProcessamentoFinal = DateParser.toString(data?.dataProcessamentoFinal)
    }

    return await this.apiService.download(`${this.url}/relatorio-busca-rapida`, data, MediaType.stream)
  }
}
