import Result from "../../core/result"
import DateParser from "../../utils/date/date-parser"
import { MediaType } from "../../utils/web"

export class DiretoriaEmpregadoService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService
    this.apiLegadoService = apiLegadoService
    this.url = 'v1/diretorias-empregados'
    this.urlLegado = 'includes/php/ajax.php'
  }

  async incluir(sindicato) {
    if (sindicato && sindicato['dataini-input'] instanceof Date) {
      sindicato['dataini-input'] = DateParser.toString(sindicato['dataini-input'])
    }

    if (sindicato && sindicato['datafim-input'] instanceof Date) {
      sindicato['datafim-input'] = DateParser.toString(sindicato['datafim-input'])
    }

    const result = await this.apiLegadoService.post(this.urlLegado, sindicato, MediaType.urlencoded)

    if (result.isFailure()) {
      return result
    }

    if (result.value?.response_status?.error_code) {
      return Result.failure(result.value.response_status.msg)
    }

    return result
  }

  async editar(sindicato) {
    if (sindicato && sindicato['dataini-input'] instanceof Date) {
      sindicato['dataini-input'] = DateParser.toString(sindicato['dataini-input'])
    }

    if (sindicato && sindicato['datafim-input'] instanceof Date) {
      sindicato['datafim-input'] = DateParser.toString(sindicato['datafim-input'])
    }

    const result = await this.apiLegadoService.post(this.urlLegado, sindicato, MediaType.urlencoded)

    if (result.isFailure()) {
      return result
    }

    if (result.value?.response_status?.error_code) {
      return Result.failure(result.value.response_status.msg)
    }

    return result
  }

  async excluir(id) {
    return await this.apiService.post(`${this.urlLegado}/${id}`, MediaType.urlencoded)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }
}
