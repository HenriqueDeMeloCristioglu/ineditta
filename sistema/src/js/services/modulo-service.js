import { MediaType } from "../utils/web"

export class ModuloService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/modulos'
  }

  async obterSISAPDatatable(params = {}) {
    params ??= {}
    params['tipo'] = 'sisap'

    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterComercialDatatable(params = {}) {
    params ??= {}
    params['tipo'] = 'Comercial'

    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterModulosPorTipo(tipo) {
    const params = {
      tipo: tipo
    }

    return await this.apiService.get(`${this.url}`, params, MediaType.json)
  }
}
