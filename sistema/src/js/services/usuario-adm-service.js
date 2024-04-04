import DateParser from "../utils/date/date-parser"
import { MediaType } from "../utils/web"

export class UsuarioAdmService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/usuario-adms'
    this.urlLegado = 'includes/php/ajax.php'
  }

  async incluir(usuario) {
    if (usuario && usuario['ausenciaInicio'] instanceof Date) {
      usuario['ausenciaInicio'] = DateParser.toString(usuario['ausenciaInicio'])
    }

    if (usuario && usuario['ausenciaFim'] instanceof Date) {
      usuario['ausenciaFim'] = DateParser.toString(usuario['ausenciaFim'])
    }

    return await this.apiService.post(this.url, usuario, MediaType.json)
  }

  async editar(usuario) {
    if (usuario && usuario['ausenciaInicio'] instanceof Date) {
      usuario['ausenciaInicio'] = DateParser.toString(usuario['ausenciaInicio'])
    }

    if (usuario && usuario['ausenciaFim'] instanceof Date) {
      usuario['ausenciaFim'] = DateParser.toString(usuario['ausenciaFim'])
    }

    return await this.apiService.put(`${this.url}/${usuario.id}`, usuario, MediaType.json)
  }

  async excluir(id) {
    return await this.apiService.post(`${this.urlLegado}/${id}`, MediaType.urlencoded)
  }

  async obterPorEmailOuNome(email, nome) {
    return await this.apiService.get(`${this.url}/${email}/${nome}`, null, MediaType.json)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async enviarEmailAtualizacaoCredenciais(id) {
    return await this.apiService.post(`${this.url}/${id}/email-atualizacoes-cadastrais`, null, MediaType.json)
  }

  async enviarEmailBoasVindas(email, uuid) {
    return await this.apiService.post(`${this.url}/email-boas-vindas?email=${email}`, null, MediaType.requestId(uuid))
  }

  async atualizarPermissoes(id) {
    return await this.apiService.put(`${this.url}/${id}/permissoes`, null, MediaType.json)
  }

  async obterSelectUsuariosIneditta() {
    return await this.apiService.get(`${this.url}/ineditta`, null, MediaType.select)
  }

  async obterDadosPessoais() {
    return await this.apiService.get(`${this.url}/dados-pessoais`, null, MediaType.json)
  }

  async obterPermissoes() {
    return await this.apiService.get(`${this.url}/permissoes`, null, MediaType.json)
  }

  async obterDatatablePorGrupoEconomico(params) {
    return await this.apiService.get(`${this.url}/grupo-economico`, params, MediaType.dataTable)
  }

  async obterDatatablePorDocumento(id, request) {
    return await this.apiService.get(`${this.url}/documentos/${id}`, request, MediaType.dataTable)
  }
}
