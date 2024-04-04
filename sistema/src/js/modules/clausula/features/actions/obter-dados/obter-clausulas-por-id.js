import { ApiService } from "../../../../../core/index"
import { ClausulaGeralService } from "../../../../../services"
import NotificationService from "../../../../../utils/notifications/notification.service"

const clausulaGeralService = new ClausulaGeralService(new ApiService())

export async function obterClausulaPorId(id) {
  const result = await clausulaGeralService.obterPorId(id)

  if (result.isFailure()) return NotificationService.error({ title: 'Erro ao obter clausula', message: result.error })

  return result.value
}