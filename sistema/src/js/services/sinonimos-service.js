import { MediaType } from "../utils/web"

export class SinonimosService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/sinonimos'
  }

  async obterSelect() {
    return await this.apiService.get(`${this.url}`, null, MediaType.select)
  }

  async obterAssuntoPorIdSelect(id) {
    return await this.apiService.get(`${this.url}/assunto/${id}`, null, MediaType.select)
  }
}
