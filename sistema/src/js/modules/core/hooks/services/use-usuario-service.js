import { ApiLegadoService } from "../../../../core/api-legado"
import { ApiService } from "../../../../core/index"
import { UsuarioAdmService } from "../../../../services"

export function useUsuarioService() {
  return new UsuarioAdmService(new ApiService(), new ApiLegadoService())
}