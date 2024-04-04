import DateParser from "../../utils/date/date-parser"
import { MediaType } from "../../utils/web"

export class DocSindService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/documentos-sindicais'
  }

  async obterDatatableAprovados(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/doc/${id}`, null, MediaType.json)
  }

  async obterDocSindPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterGruposEconomicosEmpresasPorId(id) {
    return await this.apiService.get(`${this.url}/${id}/grupos-empresas`, null, MediaType.json)
  }

  async obterSelect(params, withId = true) {
    const result = await this.apiService.get(`${this.url}`, params, MediaType.select)

    const optionsMap = new Map();

    result?.value?.forEach(option => {
      const description = withId ? "Id: " + option.id + " - " + option.description : option.description;

      if (!optionsMap.has(description)) {
        optionsMap.set(description, {
          id: option.id,
          description
        })
      }
    })

    return Array.from(optionsMap.values())
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterDatatableConsulta(params) {
    if (params?.dataValidadeInicial && params?.dataValidadeInicial instanceof Date) {
      params.dataValidadeInicial = DateParser.toString(params?.dataValidadeInicial)
    }

    if (params?.dataValidadeFinal && params?.dataValidadeFinal instanceof Date) {
      params.dataValidadeFinal = DateParser.toString(params?.dataValidadeFinal)
    }

    if (params?.dataAprovacaoInicial && params?.dataAprovacaoInicial instanceof Date) {
      params.dataAprovacaoInicial = DateParser.toString(params?.dataAprovacaoInicial)
    }

    if (params?.dataAprovacaoFinal && params?.dataAprovacaoFinal instanceof Date) {
      params.dataAprovacaoFinal = DateParser.toString(params?.dataAprovacaoFinal)
    }

    return await this.apiService.post(`${this.url}/consultas`, params, MediaType.dataTable)
  }

  async obterSelectAnosMeses(params = {}) {
    const result = await this.apiService.get(`${this.url}/anos-meses`, params, MediaType.json)

    if (result.isFailure() || !Array.isArray(result.value)) {
      return []
    }

    return result?.value?.sort((a, b) => {
      if (a.ano > b.ano) return -1
      if (a.ano < b.ano) return 1

      if (a.mes > b.mes) return -1
      if (a.mes < b.mes) return 1

      return 0
    })
      .map((item) => ({ id: item.mesAno, description: item.mesAno }))
  }

  async obterEncerrados(params) {
    return await this.apiService.get(`${this.url}/encerrados`, params, MediaType.dataTable)
  }

  async obterProcessados(params) {
    return await this.apiService.get(`${this.url}/processados`, params, MediaType.dataTable)
  }

  async obterNegociacoesAcumuladas(params) {
    const queryString = {
      anoInicial: params?.anoInicial,
      anoFinal: params?.anoFinal,
      gruposEconomicosIds: params?.gruposEconomicosIds,
      matrizesIds: params?.matrizesIds,
      unidadesIds: params?.unidadesIds,
      cnaesIds: params?.cnaesIds
    }

    return await this.apiService.get(`${this.url}/negociacoes-acumuladas`, queryString, MediaType.json)
  }

  async download(params) {
    return await this.apiService.download(`${this.url}/documentos`, params, MediaType.stream)
  }

  async incluir(documento) {
    return await this.apiService.post(`${this.url}`, documento, MediaType.json)
  }

  async incluirDocumentoComercial(documento) {
    return await this.apiService.post(`${this.url}/comercial`, documento, MediaType.json)
  }

  async atualizar(documento) {
    return await this.apiService.put(`${this.url}`, documento, MediaType.json)
  }

  async obterUsuariosNotificados(id) {
    return await this.apiService.get(`${this.url}/usuarios-notificacoes/${id}`, null, MediaType.json)
  }

  async aprovar(documentoId) {
    return await this.apiService.patch(`${this.url}/${documentoId}/aprovar`, null)
  }

  async liberar(id) {
    return await this.apiService.patch(`${this.url}/${id}/liberar`, null)
  }

  async atualizarSla(documentoId, novaDataSla) {
    return await this.apiService.patch(`${this.url}/${documentoId}/data-sla`, { novaDataSla }, MediaType.json)
  }

  async obterConfiguracoesEmailPorId(id) {
    return await this.apiService.get(`${this.url}/${id}/emails-configuracoes`, null, MediaType.json)
  }

  async notificarCriacaoDocumento(params) {
    return await this.apiService.post(`${this.url}/notificacao/criacao`, params, MediaType.json)
  }

  async uploadFile(params) {
    return await this.apiService.post(`${this.url}/files`, params, MediaType.multipartFormData);
  }

  async obterDocumentoAprovadoEmailDadosPorId(id) {
    return await this.apiService.get(`${this.url}/documento-aprovado-email/${id}`, null, MediaType.json)
  }

  async enviarEmailDocumentoAprovacao(data) {
    return await this.apiService.post(`${this.url}/aprovados/emails`, data, MediaType.json)
  }

  async iniciarScrap(documentoId) {
    return await this.apiService.post(`${this.url}/${documentoId}/scrap`, null, MediaType.json)
  }

  async obterSelectPorClientesCnaes(params) {
    return await this.apiService.get(`${this.url}/clientes-cnaes`, params, MediaType.select)
  }

  async obterSelectProcessados(params) {
    return await this.apiService.get(`${this.url}/processados`, params, MediaType.select)
  }
}
