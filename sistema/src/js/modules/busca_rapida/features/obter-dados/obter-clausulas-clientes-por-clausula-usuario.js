import Result from "../../../../core/result"
import { useClausulaClienteService } from "../../../core/hooks"

const clausulaClienteService = useClausulaClienteService()

export async function obterClausulasClientesPorClausulaUsuario({
  clausulaId,
  grupoEconomicoId
}) {
  const result = await clausulaClienteService.obterTodos({
    clausulaId,
    grupoEconomicoId
  })

  if (result.isFailure()) {
    return Result.failure(result.error)
  }

  return Result.success(result.value)
}