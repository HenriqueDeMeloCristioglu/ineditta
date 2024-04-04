import DateParser from "../utils/date/date-parser"
import { MediaType } from "../utils/web"

export class ComentarioService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/comentarios'
  }

  async obterTodos() {
    return await this.apiService.get(this.url, null, MediaType.json)
  }

  async incluir(comentario) {
    if (comentario && comentario.dataValidade instanceof Date) {
      comentario.dataValidade = DateParser.toString(comentario.dataValidade)
    }

    return await this.apiService.post(this.url, comentario, MediaType.json)
  }

  async editar(comentario) {
    if (comentario && comentario.dataValidade instanceof Date) {
      comentario.dataValidade = DateParser.toString(comentario.dataValidade)
    }

    return await this.apiService.put(`${this.url}/${comentario.id}`, comentario, MediaType.json)
  }

  async excluir(id) {
    return await this.apiService.delete(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}?porUsuario=${true}`, params, MediaType.dataTable)
  }
}
