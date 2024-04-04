import { ApiService } from "../../../../core/index"
import { EstruturaClausulaService } from "../../../../services"

export function useEstruturaClusulaService() {
  return new EstruturaClausulaService(new ApiService())
}