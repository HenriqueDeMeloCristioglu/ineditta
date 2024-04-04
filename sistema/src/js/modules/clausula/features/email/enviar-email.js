import { ApiService } from "../../../../core/index"
import { ClausulaGeralService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"
import { Generator } from "../../../../utils/generator"
import Result from "../../../../core/result"

const clausulaGeralService = new ClausulaGeralService(new ApiService())

export async function enviarEmail(documentoId, usuariosIds = null) {
  const result = await clausulaGeralService.enviarEmailClausulaAprovada({
    idempotentToken: Generator.id(),
    documentoId,
    usuariosIds
  })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao enviar Email', message: result.error })
  }

  NotificationService.success({ title: 'Email enviado com sucesso' })

  return Result.success()
}