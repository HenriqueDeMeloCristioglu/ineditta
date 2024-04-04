import { MediaType } from "../../utils/web"

export class ClausulaClienteService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = "v1/clausulas-clientes"
  }

  async incluir(clausula) {
    return await this.apiService.post(this.url, clausula, MediaType.json)
  }

  async obterTodos(params) {
    return await this.apiService.get(this.url, params, MediaType.json)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }
}
