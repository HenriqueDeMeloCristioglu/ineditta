import { MediaType } from "../../utils/web"

export class SindicatoLaboralService {
  constructor(apiService, apiLegado) {
    this.apiService = apiService
    this.apiLegadoService = apiLegado
    this.url = 'v1/sindicatos-laborais'
  }

  async obterTodos() {
    return await this.apiService.get(this.url, null, MediaType.json)
  }

  async incluir(params) {
    return await this.apiService.post(this.url, params, MediaType.json)
  }

  async editar(params) {
    return await this.apiService.post(this.url, params, MediaType.json)
  }

  async excluir(id) {
    return await this.apiService.delete(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterPorId(id, params) {
    return await this.apiService.get(`${this.url}/${id}`, params, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterSelect() {
    return await this.apiService.get(`${this.url}`, null, MediaType.select)
  }

  async obterSelectFiltroSindicatos(idGrupoEconomico, matrizes, unidades, localizacoes, tipoLocalidade, idCnaes) {
    const paramsMatrizes = matrizes != null ? matrizes.map(n => `idMatriz=${n}`).join('&') : ''
    const paramsUnidades = unidades != null ? unidades.map(n => `idUnidade=${n}`).join('&') : ''
    const paramsLocalizacoes = localizacoes != null ? localizacoes.map(n => `localizacoes=${n}`).join('&') : ''
    const paramsCnaes = idCnaes != null ? idCnaes.map(n => `idCnaes=${n}`).join('&') : ''

    let url = `${this.url}/filtros-sindicatos?`
    tipoLocalidade == null ? null : url += `tipoLocalidade=${tipoLocalidade}&`
    idGrupoEconomico == null ? null : url += `idGrupoEconomico=${idGrupoEconomico}&`
    paramsUnidades == '' ? null : url += `${paramsUnidades}&`
    paramsMatrizes == '' ? null : url += `${paramsMatrizes}&`
    paramsLocalizacoes == '' ? null : url += `${paramsLocalizacoes}&`
    paramsCnaes == '' ? null : url += `${paramsCnaes}&`

    return await this.apiService.get(url, null, MediaType.select)
  }

  async obterSelectPorUsuario(options = { matrizesIds: null, clientesUnidadesIds: null, grupoEconomicoId: null, localizacoesIds: null, ufs: null, regioes: null, cnaesIds: null }) {
    const params = {
      porUsuario: true,
      columns: 'id,sigla,cnpj,razaoSocial',
      ...options
    }

    const result = await this.apiService.get(this.url, params, MediaType.json)

    return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(item => ({ id: item.id, description: `${item.sigla} / ${item.cnpj} / ${item.razaoSocial}` })) ?? [] : [];
  }

  async obterTodosComBaseExistente() {
    return await this.apiService.get(`${this.url}/bases-existentes`, null, MediaType.json)
  }

  async obterSelectUfs() {
    return await this.apiService.get(`${this.url}/ufs`, null, MediaType.select)
  }
}
