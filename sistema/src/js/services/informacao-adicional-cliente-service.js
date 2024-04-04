import { MediaType } from "../utils/web"

export class InformacaoAdicionalClienteService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService
    this.apiLegadoService = apiLegadoService
    this.url = "v1/informacoes-adicionais-clientes"
  }

  async obterPorDocumento(documentoId) {
    return await this.apiService.get(`${this.url}/${documentoId}`, null, MediaType.json)
  }

  async incluir(body) {
    return await this.apiService.post(`${this.url}`, body, MediaType.json)
  }

  async atualizar({ body, id }) {
    return await this.apiService.put(`${this.url}/${id}`, body, MediaType.json)
  }

  async aprovar(id) {
    return await this.apiService.patch(`${this.url}/${id}/aprovar`, null, MediaType.json)
  }

  async relatorio(id) {
    return await this.apiService.download(`${this.url}/relatorio/${id}`, null, MediaType.stream)
  }
}
