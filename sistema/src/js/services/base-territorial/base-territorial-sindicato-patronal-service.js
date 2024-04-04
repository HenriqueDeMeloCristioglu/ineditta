import { MediaType } from "../../utils/web"

export class BaseTerritorialSindicatoPatronalService {
  constructor(apiService) {
    this.apiService = apiService;
    this.url = 'v1/bases-territoriais-sindicatos-patronais';
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable);
  }
}
