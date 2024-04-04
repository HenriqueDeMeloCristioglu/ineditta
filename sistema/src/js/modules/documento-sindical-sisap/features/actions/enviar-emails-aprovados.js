import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { DocSindService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const docSindService = new DocSindService(new ApiService())

export async function enviarEmailsAprovados(id, emailsIds = null) {
  const result = await docSindService.enviarEmailDocumentoAprovacao({ documentoId: id, usuariosIds: emailsIds })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao enviar a emails', message: result.error })
  }
  
  NotificationService.success({ title: 'Notificação enviada com sucesso!' })

  return Result.success()
}
