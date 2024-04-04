import { MediaType } from '../utils/web'

export class InformacaoAdicionalService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService
    this.apiLegadoService = apiLegadoService
    this.url = "v1/informacoes-adicionais"
  }

  async obterInformacoesAdicionaisPorClausula(id) {
    return await this.apiService.get(`${this.url}/clausula/${id}`, null, MediaType.json)
  }

  async obterCamposAdicionais(tipoId, estruturaId) {
    return await this.apiService.get(`${this.url}?tipoId=${tipoId}&&estruturaId=${estruturaId}`, null, MediaType.json)
  }

  async obterDadosCamposAdicionais({ grupoId, estruturaId, clausulaId }) {
    return await this.apiService.get(`${this.url}/dados/${clausulaId}?grupoId=${grupoId}&&estruturaId=${estruturaId}`, null, MediaType.json)
  }
}
