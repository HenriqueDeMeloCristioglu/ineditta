import { MediaType } from "../../utils/web"

export class SindicatoService {
  constructor(apiService, apiLegado) {
    this.apiService = apiService
    this.url = "v1/sindicatos"
    this.apiLegadoService = apiLegado
  }

  async obterTodos() {
    return await this.apiService.get(this.url, null, MediaType.json)
  }

  async obterSindicatos(
    options = {
      matrizesIds: null,
      clientesUnidadesIds: null,
      grupoEconomicoId: null,
      localizacoesIds: null,
      ufs: null,
      regioes: null,
      cnaesIds: null,
      dataBase: null,
      sindPatronaisIds: null,
      sindLaboraisIds: null,
    }
  ) {
    const result = await this.apiService.get(this.url, options, MediaType.json)
    return result
  }

  async obterMandatosSindicais(
    options = {
      grupoEconomicoId: null,
      sindeIds: null,
      sindpIds: null,
    }
  ) {
    const result = await this.apiService.get(`${this.url}/mandatos-sindicais`, options, MediaType.json)
    return result
  }

  async obterOrganizacaoSindicalLaboralDatatable(options, sindeIds) {
    const params = {
      sindeIds,
      ...options,
    }

    return await this.apiService.get(`${this.url}/organizacao-sindical-laboral`, params, MediaType.dataTable)
  }

  async obterOrganizacaoSindicalPatronalDatatable(options, sindpIds) {
    const params = {
      sindpIds,
      ...options,
    }

    return await this.apiService.get(`${this.url}/organizacao-sindical-patronal`, params, MediaType.dataTable)
  }

  async obterDirigentesPatronalDatatable(options, grupoEconomicoId, sindpIds) {
    const params = {
      sindpIds,
      grupoEconomicoId,
      ...options,
    }

    return await this.apiService.get(`${this.url}/dirigentes-patronais`, params, MediaType.dataTable)
  }

  async obterDirigentesLaboralDatatable(options, grupoEconomicoId, sindeIds) {
    const params = {
      sindeIds,
      grupoEconomicoId,
      ...options,
    }

    return await this.apiService.get(`${this.url}/dirigentes-laborais`, params, MediaType.dataTable)
  }

  async obterInfoSindical(id, tipoSind) {
    const params = {
      sindId: id,
      tipoSind,
    }

    return await this.apiService.get(`${this.url}/infos-sindicais`, params, MediaType.json)
  }

  async obterInfoDiretoriaSindDatatable(options, sindId, tipoSind) {
    const params = {
      sindId,
      tipoSind,
      ...options,
    }

    return await this.apiService.get(`${this.url}/info-dirigentes`, params, MediaType.dataTable)
  }

  async postLegado(data) {
    return await this.apiLegadoService.post("includes/php/ajax.php", data, MediaType.urlencoded)
  }

  async obterComentariosPorUF(params) {
    return await this.apiService.get(`${this.url}/comentario-uf`, params, MediaType.dataTable)
  }

  async gerarExcelSindicatos(data) {
    return await this.apiService.download(`${this.url}/relatorio`, data, MediaType.stream)
  }

  async gerarExcelSindicatosLaborais(data) {
    return await this.apiService.download(`${this.url}/relatorio/laborais`, data, MediaType.stream)
  }

  async gerarExcelSindicatosPatronais(data) {
    return await this.apiService.download(`${this.url}/relatorio/patronais`, data, MediaType.stream)
  }
}
