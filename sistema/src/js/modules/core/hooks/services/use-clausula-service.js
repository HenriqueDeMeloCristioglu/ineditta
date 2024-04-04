import { ApiLegadoService } from "../../../../core/api-legado"
import { ApiService } from "../../../../core/index"
import { ClausulaService } from "../../../../services"

export function useClausulaService() {
  return new ClausulaService(new ApiService(), new ApiLegadoService())
}