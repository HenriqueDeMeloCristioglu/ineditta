import { MediaType } from "../utils/web"

export class TipoDocService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/tipo-docs'
  }

  async incluir(tipoDocumento) {
    return await this.apiService.post(`${this.url}`, tipoDocumento, MediaType.json)
  }

  async editar(tipoDocumento) {
    return await this.apiService.put(`${this.url}/${tipoDocumento.id}`, tipoDocumento, MediaType.json)
  }

  async obterSelect(params = null) {
    return await this.apiService.get(`${this.url}`, params, MediaType.select)
  }

  async obterProcessados() {
    return await this.apiService.get(`${this.url}`, { filtrarSelectType: true, processado: true }, MediaType.select)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterSelectPorTipos({ tipos = null, tiposDocumentosIds, processado }) {
    let params = { processado }
    params = tipos ? { tipos: Array.isArray(tipos) ? tipos : null, processado } : params
    params = tiposDocumentosIds ? { tiposDocumentosIds: Array.isArray(tiposDocumentosIds) ? tiposDocumentosIds : null, processado } : params

    const result = await this.apiService.get(`${this.url}`, params)

    return result.isFailure() || !Array.isArray(result.value) ? [] : result.value?.map(tipo => ({ id: tipo.id, description: tipo.nome })) ?? []
  }

  async obterTiposSelect() {
    return await this.apiService.get(`${this.url}/tipos`, null, MediaType.select)
  }
}
