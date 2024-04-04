// Core
import { ApiService } from "../../../../core/index"

// Services
import { GrupoEconomicoService, ClienteUnidadeService, MatrizService } from "../../../../services"

// Application
import { TipoUsuarioDestino } from "../../../../application/comentarios/constants"

const apiService = new ApiService()
const grupoEconomicoService = new GrupoEconomicoService(apiService)
const matrizService = new MatrizService(apiService)
const clienteUnidadeService = new ClienteUnidadeService(apiService)

export async function usuarioDestinoSelect({ tipoUsuarioId, isIneditta, grupoEconomicoId }) {
  tipoUsuarioId = parseFloat(tipoUsuarioId)

  switch (tipoUsuarioId) {
    case TipoUsuarioDestino.Grupo:
      if (!isIneditta){
        return await grupoEconomicoService.obterSelectPorUsuario()
      }

      return (await grupoEconomicoService.obterSelect()).value
    case TipoUsuarioDestino.Matriz:
      return (await matrizService.obterSelectPorUsuario(isIneditta ? null : grupoEconomicoId))
    case TipoUsuarioDestino.Unidade:
      return (await clienteUnidadeService.obterSelectPorUsuario())
    default:
      return await Promise.resolve([])
  }
}