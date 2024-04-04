import { MediaType } from "../utils/web"

export class EventosCalendarioService {
  constructor(apiService, apiLegado) {
    this.apiService = apiService
    this.apiLegadoService = apiLegado
    this.url = 'v1/eventos-calendario'
  }

  async obterVencimentosDocumentosDatatable(params) {
    return await this.apiService.get(`${this.url}/vencimentos-documentos`, params, MediaType.dataTable)
  }

  async obterVencimentosMandatosPatronaisDatatable(params) {
    return await this.apiService.get(`${this.url}/vencimentos-mandatos-patronais`, params, MediaType.dataTable)
  }

  async obterAssembleiaReuniaoDatatable(params) {
    return await this.apiService.get(`${this.url}/assembleias-reunioes`, params, MediaType.dataTable);
  }

  async obterVencimentosMandatosLaboraisDatatable(params) {
    return await this.apiService.get(`${this.url}/vencimentos-mandatos-laborais`, params, MediaType.dataTable)
  }

  async obterTrintidioDatatable(params) {
    return await this.apiService.get(`${this.url}/trintidios`, params, MediaType.dataTable)
  }

  async obterEventosClausulasDatatable(params) {
    return await this.apiService.get(`${this.url}/eventos-clausulas`, params, MediaType.dataTable)
  }

  async obterTextoClausulaPorEventoId(eventoId) {
    return await this.apiService.get(`${this.url}/${eventoId}/texto-clausulas`, null, MediaType.json)
  }

  async criarEventoUsuario(evento) {
    return await this.apiService.post(`${this.url}/calendarios-sindicais-usuario`, evento, MediaType.json)
  }

  async obterAgendaEvento(params) {
    return await this.apiService.get(`${this.url}/agenda-eventos`, params, MediaType.dataTable)
  }

  async obterTiposSubtiposEvento(params) {
    return await this.apiService.get(`${this.url}/tipos-subtipos`, params, MediaType.dataTable)
  }
}