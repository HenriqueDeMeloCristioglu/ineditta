import { MediaType } from "../utils/web"

export class GrupoClausulaService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/grupos-clausulas'
  }

  async obterSelect() {
    return await this.apiService.get(`${this.url}`, null, MediaType.select)
  }
}
