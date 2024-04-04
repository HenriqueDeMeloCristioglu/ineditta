import { MediaType } from '../utils/web'
import Result from "../core/result"

export class IndicesSindicaisService {
    constructor(apiService, apiLegadoService) {
        this.apiService = apiService
        this.apiLegadoService = apiLegadoService
        this.urlLegado = 'includes/php/ajax.php'
    }

    async generateTable(data) {
        const result = await this.apiLegadoService.post(this.urlLegado, data, MediaType.urlencoded)

        if (result.isFailure()) {
            return result
        }

        if (result.value?.response_status?.error_code) {
            return Result.failure(result.value.response_status.msg)
        }

        return result
    }
}