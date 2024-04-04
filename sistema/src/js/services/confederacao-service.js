import { MediaType } from "../utils/web"

export class ConfederacaoService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/confederacoes'
  }

  async obterTodos() {
    return await this.apiService.get(this.url, null, MediaType.json)
  }

  async salvar(confederacao) {
    return await this.apiService.post(`${this.url}`, confederacao, MediaType.json)
  }

  async editar(confederacao) {
    return await this.apiService.put(`${this.url}/${confederacao.id}`, confederacao, MediaType.json)
  }

  async excluir(id) {
    return await this.apiService.delete(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }
}
