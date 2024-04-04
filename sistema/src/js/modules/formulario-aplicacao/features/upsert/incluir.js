import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { InformacaoAdicionalClienteService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const informacaoAdicionalClienteService = new InformacaoAdicionalClienteService(new ApiService())

export async function incluir({
  documentoId,
  informacoesAdicionais,
  observacoesAdicionais,
  orientacao,
  outrasInformacoes
}) {
  const result = await informacaoAdicionalClienteService.incluir({
    documentoSindicalId: documentoId,
    informacoesAdicionais,
    observacoesAdicionais,
    orientacao,
    outrasInformacoes
  })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao incluir informação', message: result.error })
  }

  NotificationService.success({ title: 'Salvo com sucesso' })

  return Result.success()
}