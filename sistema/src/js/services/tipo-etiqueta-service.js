import { MediaType } from "../utils/web"

export class TipoEtiquetaService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/tipos-etiquetas'
  }

  async obterSelect(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.select)
  }
}
