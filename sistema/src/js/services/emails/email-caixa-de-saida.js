import { MediaType } from "../../utils/web"

export class EmailCaixaDeSaidaService {
  constructor(apiService) {
      this.apiService = apiService
      this.url = 'v1/emails-caixas-de-saida'
  }

  async obterDatatable(params) {
      return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }
  
  async reenviarEmails() {
    return await this.apiService.post(`${this.url}/reenviar-emails`, {}, MediaType.json);
  }
}