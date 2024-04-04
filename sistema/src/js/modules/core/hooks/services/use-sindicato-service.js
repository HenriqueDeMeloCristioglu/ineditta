import { ApiService } from "../../../../core/index"
import { ApiLegadoService } from "../../../../core/api-legado"
import { SindicatoService } from "../../../../services"

export function useSindicatoService() {
  return new SindicatoService(new ApiService(), new ApiLegadoService())
}