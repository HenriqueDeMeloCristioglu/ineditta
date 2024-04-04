import { ApiService } from "../../../../core/index"
import { GrupoEconomicoService } from "../../../../services"

export function useGrupoEconomicoService() {
  return new GrupoEconomicoService(new ApiService())
}