import { MediaType } from "../utils/web"

export class MapaSindicalService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService
    this.apiLegadoService = apiLegadoService
    this.url = "v1/mapas-sindicais"
  }

  async obterDocumentosReferencia(params = null) {
    return await this.apiService.get(`${this.url}/documentos`, params, MediaType.dataTable)
  }

  async obterExcel(params) {
    return await this.apiService.post(`${this.url}/excel`, params, MediaType.dataTable)
  }

  async downloadExcelInformacoesAdicionais(request) {
    return await this.apiService.download(`${this.url}/excel/informacoes-adicionais`, request, MediaType.excel)
  }

  async obterExcelComparativo(params) {
    return await this.apiService.download(`${this.url}/excel/comparativo`, params, MediaType.stream)
  }
}
