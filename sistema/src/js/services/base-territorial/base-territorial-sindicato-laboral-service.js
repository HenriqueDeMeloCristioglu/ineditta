import { MediaType } from "../../utils/web";

export class BaseTerritorialSindicatoLaboralService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/bases-territoriais-sindicatos-laborais'
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterDatatableLocalizacoes(params) {
    return await this.apiService.get(`${this.url}/localizacoes`, params, MediaType.dataTable)
  }
}
