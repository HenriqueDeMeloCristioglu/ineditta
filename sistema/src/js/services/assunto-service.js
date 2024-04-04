import { MediaType } from '../utils/web'

export class AssuntoService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/assuntos'
  }

  async obterSelect() {
    return await this.apiService.get(`${this.url}`, null, MediaType.select)
  }
}
