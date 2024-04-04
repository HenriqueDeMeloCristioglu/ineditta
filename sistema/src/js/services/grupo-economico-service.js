import { MediaType } from "../utils/web"

export class GrupoEconomicoService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/grupos-economicos'
  }

  async obterSelect(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.select)
  }

  async obterSelectPorUsuario() {
    const result = await this.apiService.get(`${this.url}?porUsuario=true`, null, MediaType.json)
    return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(item => ({ id: item.id, description: item.nome })) ?? [] : []
  }

  async obterDataTable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }
}
