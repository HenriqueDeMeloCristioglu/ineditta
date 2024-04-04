import { MediaType } from "../utils/web"

export class JornadaService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/jornada'
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.json)
  }
}
