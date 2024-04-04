import { MediaType } from "../../utils/web"

export class DocumentosLocalizadosService {
  constructor(apiService, apiLegado) {
    this.apiService = apiService
    this.apiLegadoService = apiLegado
    this.url = 'v1/documentos-localizados'
    this.apiLegado = "includes/php/ajax.php"
  }

  async incluir(documento) {
    return await this.apiLegadoService.post(this.apiLegado, documento, MediaType.urlencoded)
  }

  async editar(documento) {
    return await this.apiLegadoService.post(this.apiLegado, documento, MediaType.urlencoded)
  }

  async deletar(id) {
    return await this.apiService.delete(`${this.url}/${id}`, null, MediaType.json)
  }

  async aprovar(id) {
    return await this.apiService.patch(`${this.url}/${id}/aprovar`, null, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterAprovados(params) {
    return await this.apiService.get(`${this.url}/aprovados`, params, MediaType.json)
  }

  async obterAprovadoPorId(id) {
    return await this.apiService.get(`${this.url}/aprovados/${id}`, null, MediaType.json)
  }

  async upload(params) {
    return await this.apiService.post(`${this.url}`, params, MediaType.multipartFormData)
  }

  async download(id) {
    return await this.apiService.downloadBlob(`${this.url}/${id}`, null, MediaType.stream)
  }

  async downloadPorDocumentoSindicalId(id) {
    return await this.apiService.downloadBlob(`${this.url}/documentos-sindicais/${id}`, null, MediaType.stream)
  }
}
