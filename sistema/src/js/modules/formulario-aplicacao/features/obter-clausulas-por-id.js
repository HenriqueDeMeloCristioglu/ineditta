import { ApiService } from "../../../core/index"
import { ClausulaGeralService } from "../../../services"
import NotificationService from "../../../utils/notifications/notification.service"

const clausulaGeralService = new ClausulaGeralService(new ApiService())

export async function obterClausulasPorId(documentoId) {
  const result = await clausulaGeralService.obterPorDocumento(documentoId)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter clausulas do documento', message: result.error })
  }

  return result.value
}