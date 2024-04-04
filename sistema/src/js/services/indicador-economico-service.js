import { MediaType } from "../utils/web"

export class IndicadorEconomicoService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService
    this.apiLegadoService = apiLegadoService
    this.url = "v1/indicadores-economicos"
  }

  async incluirPrincipal(indice) {
    return await this.apiService.post(`${this.url}/principais`, indice, MediaType.json)
  }

  async incluirReal(indice) {
    return await this.apiService.post(`${this.url}/reais`, indice, MediaType.json)
  }

  async editarPrincipal(indice) {
    return await this.apiService.post(`${this.url}/principais`, indice, MediaType.json)
  }

  async editarReal(indice) {
    return await this.apiService.post(`${this.url}/reais`, indice, MediaType.json)
  }

  async editar(indice) {
    return await this.apiLegadoService.post("includes/php/ajax.php", indice, MediaType.urlencoded)
  }

  async excluir(data) {
    return await this.apiLegadoService.post("includes/php/ajax.php", data, MediaType.urlencoded)
  }

  async obterPrincipalPorId(id) {
    return await this.apiService.get(`${this.url}/principais/${id}`, null, MediaType.json)
  }

  async obterRealPorId(id) {
    return await this.apiService.get(`${this.url}/reais/${id}`, null, MediaType.json)
  }

  async obterDatatablePrincipal(params) {
    return await this.apiService.get(`${this.url}/principais`, params, MediaType.dataTable)
  }

  async obterDatatableReal(params) {
    return await this.apiService.get(`${this.url}/reais`, params, MediaType.dataTable)
  }

  async obterHomeAsync(params) {
    return await this.apiService.get(`${this.url}/home`, params, MediaType.json)
  }
}
