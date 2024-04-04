import { MediaType } from '../../utils/web'

export class AcompanhamentoCctFasesService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = "v1/acompanhamentos-cct-fases"
  }

  async obterSelect() {
    return await this.apiService.get(this.url, null, MediaType.select)
  }
}
