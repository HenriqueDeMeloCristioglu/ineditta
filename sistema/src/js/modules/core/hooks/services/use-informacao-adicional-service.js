import { ApiService } from "../../../../core/index"
import { InformacaoAdicionalService } from "../../../../services"

export function useInformacaoAdicionalService() {
  return new InformacaoAdicionalService(new ApiService())
}
