import { ApiService } from "../../../../core/index"
import { ClienteUnidadeService } from "../../../../services"

export function useEstabelecimentoService() {
  return new ClienteUnidadeService(new ApiService())
}