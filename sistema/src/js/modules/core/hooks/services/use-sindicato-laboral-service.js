import { ApiService } from "../../../../core/index"
import { ApiLegadoService } from "../../../../core/api-legado"
import { SindicatoLaboralService } from "../../../../services"

export function useSindicatoLaboralService() {
  return new SindicatoLaboralService(new ApiService(), new ApiLegadoService())
}