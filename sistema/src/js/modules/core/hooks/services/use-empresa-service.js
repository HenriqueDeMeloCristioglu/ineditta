import { ApiService } from "../../../../core/index"
import { MatrizService } from "../../../../services"

export function useEmpresaService() {
  return new MatrizService(new ApiService())
}