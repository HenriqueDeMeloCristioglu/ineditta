import { MediaType } from "../utils/web"

export class EstruturaClausulaService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/estrutura-clausulas'
  }

  async obterSelect(params = null) {
    return await this.apiService.get(`${this.url}`, params, MediaType.select)
  }

  async obterSelectPorGrupo(grupoClausula, calendario = false, filters = null) {
    const params = {
      grupoClausulaId: grupoClausula,
      calendario,
      ...filters
    }
    return await this.apiService.get(`${this.url}/por-grupo`, params, MediaType.select)
  }

  async obterGruposSelect(params = null) {
    return await this.apiService.get(`${this.url}/grupos`, params, MediaType.select)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable);
  } 
}
