import { MediaType } from "../utils/web"

export class TipoUnidadeClienteService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/tipos-unidades-clientes'
  }

  async obterSelect() {
    return await this.apiService.get(`${this.url}`, null, MediaType.select)
  }
}
