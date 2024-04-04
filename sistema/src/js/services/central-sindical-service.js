import { MediaType } from "../utils/web"

export class CentralSindicalService {
    constructor(apiService) {
        this.apiService = apiService
        this.url = 'v1/centrais-sindicais'
    }

    async obterTodos() {
        return await this.apiService.get(this.url, null, MediaType.json)
    }

    async salvar(centralSindical) {
        return await this.apiService.post(`${this.url}`, centralSindical, MediaType.json)
    }

    async editar(centralSindical) {
        return await this.apiService.put(`${this.url}/${centralSindical.id}`, centralSindical, MediaType.json)
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
}