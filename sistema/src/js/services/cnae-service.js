import { MediaType } from "../utils/web"

export class CnaeService {
    constructor(apiService) {
        this.apiService = apiService
        this.url = 'v1/cnaes'
    }

    async obterTodos() {
        return await this.apiService.get(this.url, null, MediaType.json)
    }

    async obterTodosInclusao(requestData) {
        return await this.apiService.get(`${this.url}/inclusao`, requestData, MediaType.json)
    }

    async salvar(cnae) {
        return await this.apiService.post(`${this.url}`, cnae, MediaType.json)
    }

    async editar(cnae) {
        return await this.apiService.put(`${this.url}/${cnae.id}`, cnae, MediaType.json)
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

    async obterDatatableSimples(params) {
        return await this.apiService.get(`${this.url}/datatable`, params, MediaType.dataTable)
    }

    async obterSelect(query) {
        return await this.apiService.get(`${this.url}/select?${query}`, null, MediaType.select)
    }

    async obterSelectPorUsuario(options = { matrizesIds: null, clientesUnidadesIds: null, grupoEconomicoId: null, porGrupoDoUsuario: false }) {
        const params = {
            porUsuario: true,
            columns: 'id,descricaoSubClasse',
            ...options
        }

        const result = await this.apiService.get(this.url, params, MediaType.json)
        return result.isSuccess() && Array.isArray(result.value) ? result?.value?.map(cnae => ({ id: cnae.id, description: cnae.descricaoSubClasse })) : []
    }

    async obterSelectDivisaoPorUsuario(options = { matrizesIds: null, clientesUnidadesIds: null, grupoEconomicoId: null, porGrupoDoUsuario: false }) {
        const params = {
            porUsuario: true,
            columns: 'divisao,descricao',
            distinctDivisao: true,
            ...options
        }

        const result = await this.apiService.get(this.url, params, MediaType.json)
        return result.isSuccess() && Array.isArray(result.value) ? result?.value?.map(cnae => ({ id: cnae.divisao, description: cnae.descricao })) : []
    }
}