import { param } from "jquery";
import { MediaType } from "../utils/web";
import Result from "../core/result";

export class ClienteUnidadeSindicatoPatronalService {
    constructor(apiService) {
        this.apiService = apiService;
        this.url = 'v1/clientes-unidades-sindicatos-patronais';
    }

    async atualizar(params = null) {
        return await this.apiService.put(`${this.url}`, params, MediaType.json);
    }

    async obterPorSindicatoId(sindicatoPatronalId) {
        return await this.apiService.get(`${this.url}/sindicatos/${sindicatoPatronalId}`, null, MediaType.json);
    }
}