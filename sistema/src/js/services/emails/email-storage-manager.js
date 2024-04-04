import { MediaType } from "../../utils/web"

export class EmailStorageManagerService {
  constructor(apiService) {
      this.apiService = apiService
      this.url = 'v1/emails-storages-managers'
  }

  async obterDatatable(params) {
      return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterRelatorio() {
      return await this.apiService.download(`${this.url}/relatorios`, null, MediaType.stream)
  }
}