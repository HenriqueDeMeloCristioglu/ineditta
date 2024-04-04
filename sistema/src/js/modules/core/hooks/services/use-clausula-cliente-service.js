import { ApiService } from "../../../../core/index"
import { ClausulaClienteService } from "../../../../services"

export function useClausulaClienteService() {
  return new ClausulaClienteService(new ApiService())
}