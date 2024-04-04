import { ApiService } from "../../../../core/index"
import { ApiLegadoService } from "../../../../core/api-legado"
import { SindicatoPatronalService } from "../../../../services"

export function useSindicatoPatronalService() {
  return new SindicatoPatronalService(new ApiService(), new ApiLegadoService())
}