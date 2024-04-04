import Result from "../../core/result"
import { MediaType } from "../../utils/web"

export class DocumentosService {
  constructor(apiService, apiLegado) {
    this.apiService = apiService
    this.apiLegadoService = apiLegado
    this.url = 'v1/documentos'
    this.urlLegado = 'includes/php/ajax.php'
  }

  async incluir(documentos) {
    const result = await this.apiService.post(`${this.url}/comercial`, documentos, MediaType.urlencoded)

    if (result.isFailure()) {
      return result
    }

    if (result.isSuccess() && result.value?.response_status?.status == '0') {
      return Result.failure(result.value?.response_status?.msg)
    }

    return Result.success(result.value)
  }

  async editar(documentos) {
    const result = await this.apiLegadoService.post(this.urlLegado, documentos, MediaType.urlencoded)

    if (result.isFailure()) {
      return result
    }

    if (result.isSuccess() && result.value?.response_status?.status == '0') {
      return Result.failure(result.value?.response_status?.msg)
    }

    return Result.success(result.value)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }
}