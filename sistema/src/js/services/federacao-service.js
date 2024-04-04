import { MediaType } from "../utils/web"

export class FederacaoService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/federacoes'
  }

  async obterTodos() {
    return await this.apiService.get(this.url, null, MediaType.json)
  }

  async salvar(federacao) {
    return await this.apiService.post(`${this.url}`, federacao, MediaType.json)
  }

  async editar(federacao) {
    return await this.apiService.put(`${this.url}/${federacao.id}`, federacao, MediaType.json)
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
