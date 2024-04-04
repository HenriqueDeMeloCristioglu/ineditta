import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { AcompanhamentoCctService } from "../../../../services"
import { Generator } from "../../../../utils/generator"
import NotificationService from "../../../../utils/notifications/notification.service"
import { limparFormularioEmailCliente, limparFormularioEmailSindicato } from "../index"

const apiService = new ApiService()
const acompanhamentoCctService = new AcompanhamentoCctService(apiService)

export async function dispararEmails(a) {
  const { emails, template, assunto } = a
  if (!emails || !Array.isArray(emails)) return

  for (let i = 0; i < emails.length; i++) {
    const result = await acompanhamentoCctService.enviarEmailContato({
      emails: [emails[i]],
      template,
      assunto,
      idempotentToken: Generator.id(),
    })

    if (result.isFailure()) {
      NotificationService.error({ title: "Erro", message: result.error })
      return result
    }
  }


  NotificationService.success({ title: "Sucesso", message: "Enviado com sucesso!" })

  limparFormularioEmailSindicato()
  limparFormularioEmailCliente()

  return Result.success()
}