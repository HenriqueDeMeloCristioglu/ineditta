import { MediaType } from '../utils/web'

export class AssociacaoService {
    constructor(apiService) {
        this.apiService = apiService;
        this.url = 'v1/associacoes';
    }

    async obterConfederacoesDatatable(params) {
        return await this.apiService.get(`${this.url}/confederacoes`, params, MediaType.dataTable);
    }

    async obterFederacoesDatatable(params) {
        return await this.apiService.get(`${this.url}/federacoes`, params, MediaType.dataTable);
    }
}