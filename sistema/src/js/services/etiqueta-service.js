import { MediaType } from "../utils/web"

export class EtiquetaService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/etiquetas'
  }

  async obterSelect({ tipoEtiquetaId }) {
    return await this.apiService.get(`${this.url}?tipoEtiquetaId=${tipoEtiquetaId}`, null, MediaType.select)
  }
}
