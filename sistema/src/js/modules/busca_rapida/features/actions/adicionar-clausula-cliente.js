import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { ClausulaClienteService } from "../../../../services"

const clausulaClienteService = new ClausulaClienteService(new ApiService())

export async function adicionarClausulaCliente({
  clausulaId,
  texto
}) {
  const result = await clausulaClienteService.incluir({
    clausulaId,
    texto
  })

  if (result.isFailure()) {
    return Result.failure(result.error)
  }

  return Result.success()
}
