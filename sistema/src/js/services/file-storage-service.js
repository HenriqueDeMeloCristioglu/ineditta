import { MediaType } from '../utils/web'
import Result from "../core/result"

export class FileStorageService {
    constructor(apiLegadoService, apiService) {
        this.apiLegadoService = apiLegadoService
        this.apiService = apiService
    }

    async upload(params) {
        if (!params) {
            return Result.failure("Informe o arquivo")
        }

        const data = new FormData()
        data.fullfield = true

        for (const key in params) {
            data.append(key, params[key])
        }

        const result = await this.apiLegadoService.post('includes/php/file_uploader.php', data, MediaType.multipartFormData)

        if (result.isFailure()) {
            return result
        }

        //checar a resposta para verificar se há erro

        if (result.value.response_status == 0 || result.value.response_status.status == 0) {
            return Result.failure(result.value.response_status.msg)
        }

        const responseData = {
            fileName: result.value?.response_data?.file_name,
            path: result.value?.response_data?.path
        }

        return Result.success(responseData)
    }
}