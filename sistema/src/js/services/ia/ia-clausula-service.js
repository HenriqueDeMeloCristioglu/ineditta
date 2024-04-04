import { MediaType } from "../../utils/web"

export class IAClausulaService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = "v1/ia-clausulas"
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async incluir(data) {
    return await this.apiService.post(this.url, data, MediaType.json)
  }

  async atualizar(data) {
    return await this.apiService.put(`${this.url}/${data.id}`, data, MediaType.json)
  }

  async excluir(id) {
    return await this.apiService.delete(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterSelect() {
    return await this.apiService.get(this.url, null, MediaType.select)
  }
}
