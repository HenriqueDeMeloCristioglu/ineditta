import { MediaType } from "../../utils/web"

export class SindicatoPatronalService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService
    this.apiLegadoService = apiLegadoService
    this.url = 'v1/sindicatos-patronais'
    this.urlLegado = 'includes/php/ajax.php'
  }

  async incluir(params) {
    return await this.apiService.post(this.url, params, MediaType.json)
  }

  async editar(params) {
    return await this.apiService.post(this.url, params, MediaType.json)
  }

  async excluir(id) {
    return await this.apiService.post(`${this.urlLegado}/${id}`, MediaType.urlencoded)
  }

  async obterPorId(id, params) {
    return await this.apiService.get(`${this.url}/${id}`, params, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterTodos() {
    return await this.apiService.get(this.url, null, MediaType.json)
  }

  async obterSelect() {
    return await this.apiService.get(`${this.url}`, null, MediaType.select)
  }

  async obterSelectPorUsuario(options = { matrizesIds: null, clientesUnidadesIds: null, grupoEconomicoId: null, localizacoesIds: null, ufs: null, regioes: null, cnaesIds: null }) {
    const params = {
      porUsuario: true,
      columns: 'id,sigla,cnpj,razaoSocial',
      ...options
    }

    const result = await this.apiService.get(this.url, params);
    return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(item => ({ id: item.id, description: `${item.sigla} / ${item.cnpj} / ${item.razaoSocial}` })) ?? [] : [];
  }

  async obterTodosComBaseExistente() {
    return await this.apiService.get(`${this.url}/bases-existentes`, null, MediaType.json)
  }

  async obterSelectUfs() {
    return await this.apiService.get(`${this.url}/ufs`, null, MediaType.select)
  }
}
