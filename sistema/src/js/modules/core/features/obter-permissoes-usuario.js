import { ApiService } from "../../../core/index"
import { UsuarioAdmService } from "../../../services"

const usuarioAdmService = new UsuarioAdmService(new ApiService())

export async function obterPermissoesUsuario() {
  const modulos = (await usuarioAdmService.obterPermissoes()).value

  return modulos
}