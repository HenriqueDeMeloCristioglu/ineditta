import { ApiLegadoService } from "../../../../core/api-legado"
import { ApiService } from "../../../../core/index"
import { AcompanhamentoCctService } from "../../../../services"

export function useAcompanhamentoCctService() {
  return new AcompanhamentoCctService(new ApiService(), new ApiLegadoService())
}