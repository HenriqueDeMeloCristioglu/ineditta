import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { ClausulaGeralService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const clausulaGeralService = new ClausulaGeralService(new ApiService())

export async function atualizarResumo({ documentoId, estruturaId, texto, id }) {
  const result = await clausulaGeralService.atualizarResumo({
    id,
    body: {
      texto,
      documentoId,
      estruturaId
    }
  })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao atualizar resumo', message: result.error })
  }

  NotificationService.success({ title: 'Resumo atualizado com sucesso' })

  return Result.success()
}