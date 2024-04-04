import { MediaType } from '../utils/web'

export class LocalizacaoService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService
    this.apiLegadoService = apiLegadoService
    this.url = "v1/localizacoes"
    this.urlLegado = "v1/localizacoes"
  }

  async obterTodos() {
    return await this.apiService.get(this.url)
  }

  async incluir(localizacao) {
    return await this.apiLegadoService.post(this.urlLegado, localizacao, MediaType.urlencoded)
  }

  async editar(localizacao) {
    return await this.apiLegadoService.post(this.urlLegado, localizacao, MediaType.urlencoded
    )
  }

  async excluir(id) {
    return await this.apiService.delete(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterDatatablePorListaIds(params) {
    return await this.apiService.post(`${this.url}/obter-por-lista-ids`, params, MediaType.dataTable)
  }

  async obterDatatablePorUf(params, uf) {
    return await this.apiService.get(`${this.url}/ufs/${uf}`, params, MediaType.dataTable)
  }

  async obterSelect() {
    return await this.apiService.get(`${this.url}/select`, null, MediaType.select)
  }

  async obterSelectPorTipo(matrizes, unidades, grupoId, tipoLocalidade) {
    const paramsMatrizes = matrizes != null ? matrizes.map(n => `idMatriz=${n}`).join('&') : ''
    const paramsUnidades = unidades != null ? unidades.map(n => `idUnidade=${n}`).join('&') : ''

    let url = `${this.url}/select-por-tipo?tipoLocalidade=${tipoLocalidade}`
    if (grupoId != null) {
      url += `&idGrupoEconomico=${grupoId}`
    }
    if (paramsUnidades != null) {
      url += `&${paramsUnidades}`
    }
    if (paramsMatrizes != null) {
      url += `&${paramsMatrizes}`
    }

    return await this.apiService.get(url, null, MediaType.select)
  }

  async obterSelectPorUsuario(customParams = {}) {
    const params = {
      porUsuario: true,
      columns: 'id,regiao,municipio,uf',
      ...customParams
    }
    const result = await this.apiService.get(`${this.url}`, params, MediaType.json)
    return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(localidade => ({ id: localidade.id, description: `${localidade.municipio} - ${localidade.uf} / ${localidade.regiao}` })) : []
  }

  async obterSelectPorGrupoDoUsuario() {
    const params = {
      porUsuario: false,
      porGrupoDoUsuario: true,
      columns: 'id,regiao,municipio,uf',
    }
    const result = await this.apiService.get(`${this.url}`, params, MediaType.json)
    return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(localidade => ({ id: localidade.id, description: `${localidade.municipio} - ${localidade.uf} / ${localidade.regiao}` })) : []
  }

  async obterSelectRegioes(usuario = false) {
    const result = await this.apiService.get(`${this.url}/regioes?porUsuario=${usuario}&columns=id,uf`, null, MediaType.json)
    return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(regiao => ({ id: regiao.id, description: regiao.uf })) : []
  }

  async obterSelectPorGrupoDoUsuarioRegioes() {
    const result = await this.apiService.get(`${this.url}/regioes?porGrupoDoUsuario=true&columns=id,uf`, null, MediaType.json)
    return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(regiao => ({ id: regiao.id, description: regiao.uf })) : []
  }

  async obterSelectPorTipoLocalidade(options = { matrizesIds: null, clientesUnidadesIds: null, grupoEconomicoId: null, tipoLocalidade: null }, useLocIdAsSelectId = false) {
    const params = {
      porUsuario: true,
      columns: 'id,regiao,municipio,uf',
      ...options
    }
    const result = await this.apiService.get(this.url, params, MediaType.json)

    if (options.tipoLocalidade === 'uf') {
      return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(localidade => ({ id: localidade.uf, description: localidade.uf })) : []
    }

    if (options.tipoLocalidade === 'regiao') {
      return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(localidade => ({ id: localidade.regiao, description: localidade.regiao })) : []
    }

    if (options.tipoLocalidade === 'municipio' && useLocIdAsSelectId) {
      return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(localidade => ({ id: localidade.id, description: `${localidade.municipio} - ${localidade.uf} / ${localidade.regiao}` })) : []
    }

    return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(localidade => ({ id: localidade.municipio, description: `${localidade.municipio} - ${localidade.uf} / ${localidade.regiao}` })) : []
  }
}
