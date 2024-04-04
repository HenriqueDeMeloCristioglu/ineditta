
import Result from "../../../../../core/result"
import { ApiService } from "../../../../../core/index"
import { DocSindService } from "../../../../../services"

const docSindService = new DocSindService(new ApiService())

export async function liberarDocumento(id) {
  const result = await docSindService.liberar(id)

  if (result.isFailure()) {
    return result
  }

  return Result.success()
}