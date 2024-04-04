import { MediaType } from '../../utils/web'

export class AcompanhamentoCctLocalizacoesService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = "v1/acompanhamentos-cct-localizacoes"
  }

  async obterUfsSelect() {
    return await this.apiService.get(`${this.url}/ufs`, null, MediaType.select)
  }
}
