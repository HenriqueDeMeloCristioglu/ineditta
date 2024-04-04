import DateParser from "../../utils/date/date-parser";
import { MediaType } from "../../utils/web"

export class IADocumentoSindicalService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = "v1/ia-documentos-sindicais"
  }

  async obterDatatable(params) {
    if (params && params['dataSlaInicio'] instanceof Date) {
      params['dataSlaInicio'] = DateParser.toString(params['dataSlaInicio'])
    }
    
    if (params && params['dataSlaFim'] instanceof Date) {
      params['dataSlaFim'] = DateParser.toString(params['dataSlaFim'])
    }

    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async aprovar(id) {
    return await this.apiService.patch(`${this.url}/aprovar/${id}`, null, MediaType.json)
  }

  async reprocessar(id) {
    return await this.apiService.patch(`${this.url}/reprocessar/${id}`, null, MediaType.json)
  }

  async obterSelect() {
    return await this.apiService.get(`${this.url}`, null, MediaType.select)
  }
}
