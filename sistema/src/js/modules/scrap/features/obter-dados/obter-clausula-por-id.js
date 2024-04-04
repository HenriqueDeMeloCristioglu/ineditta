import { IAClausulaService } from "../../../../services"
import { ApiService } from "../../../../core/index"
import NotificationService from "../../../../utils/notifications/notification.service"

const iAClausulaService = new IAClausulaService(new ApiService())

export async function obterClausulaPorId(id) {
  const result = await iAClausulaService.obterPorId(id)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter clausula', message: result.error })
  }

  return result.value
}