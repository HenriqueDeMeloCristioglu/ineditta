import { ApiService } from "../../../../core/index"
import { CnaeService } from "../../../../services"

export function useCnaeService() {
  return new CnaeService(new ApiService())
}