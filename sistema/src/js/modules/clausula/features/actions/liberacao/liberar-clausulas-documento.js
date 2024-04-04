import { ApiService } from "../../../../../core/index"
import { ClausulaGeralService } from "../../../../../services"
import Result from "../../../../../core/result"

const clausulaService = new ClausulaGeralService(new ApiService())

export async function liberarClausulasDocumento(documentoId) {
  const result = await clausulaService.liberar(documentoId)

  if (result.isFailure()) {
    return result
  }

  return Result.success()
}