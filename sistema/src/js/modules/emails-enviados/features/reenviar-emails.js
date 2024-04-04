import { ApiService } from "../../../core/index"
import { EmailCaixaDeSaidaService } from "../../../services"
import NotificationService from "../../../utils/notifications/notification.service"

const emailCaixaDeSaidaService = new EmailCaixaDeSaidaService(new ApiService())

export async function reenviarEmails() {
  const result = await emailCaixaDeSaidaService.reenviarEmails()

  if (result.isFailure()) {
    return NotificationService.error({ title: "Erro ao reenviar Emails", message: result.error })
  }

  return NotificationService.success({ title: "Reenvio efetuado com sucesso" })
}