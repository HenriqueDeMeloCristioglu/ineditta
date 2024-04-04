import { MediaType } from '../../utils/web'

export class AcompanhamentoCctEtiquetaOpcaoService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = "v1/acompanhamentos-cct-etiquetas-opcoes"
  }

  async obterSelect() {
    return await this.apiService.get(this.url, null, MediaType.select)
  }
}
