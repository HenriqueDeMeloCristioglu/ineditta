import Result from "../../core/result"
import DateParser from "../../utils/date/date-parser"
import { MediaType } from "../../utils/web"

export class DiretoriaPatronalService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService
    this.apiLegadoService = apiLegadoService
    this.url = 'v1/diretorias-patronais'
    this.urlLegado = 'includes/php/ajax.php'
  }

  async incluir(sindicato) {
    return await this.apiService.post(this.url, sindicato, MediaType.json)
  }

  async editar(sindicato) {
    return await this.apiService.post(this.url, sindicato, MediaType.json)
  }

  async excluir(id) {
    return await this.apiService.post(`${this.urlLegado}/${id}`, MediaType.urlencoded)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }
}
