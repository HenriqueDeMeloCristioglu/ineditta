import { ApiService } from "../../../../core/index"
import { ApiLegadoService } from "../../../../core/api-legado"
import { ClausulaGeralService } from "../../../../services"

export function useClausulaGeralService() {
  return new ClausulaGeralService(new ApiService(), new ApiLegadoService())
}