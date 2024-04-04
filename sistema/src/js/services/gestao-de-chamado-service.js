import { MediaType } from "../utils/web"

export class GestaoDeChamadoService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/gestoes-chamados'
  }
  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }
}
