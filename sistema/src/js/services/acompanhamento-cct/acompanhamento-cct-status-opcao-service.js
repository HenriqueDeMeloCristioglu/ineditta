import { MediaType } from '../../utils/web'

export class AcompanhamentoCctStatusOpcaoService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = "v1/acompanhamentos-ccts-status-opcoes"
  }

  async obterSelect() {
    return await this.apiService.get(this.url, null, MediaType.select)
  }
}
