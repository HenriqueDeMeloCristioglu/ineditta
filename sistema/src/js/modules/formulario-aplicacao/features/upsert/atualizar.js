import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { InformacaoAdicionalClienteService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const informacaoAdicionalClienteService = new InformacaoAdicionalClienteService(new ApiService())

export async function atualizar({
  documentoId,
  informacoesAdicionais,
  observacoesAdicionais,
  orientacao,
  outrasInformacoes
}) {
  const result = await informacaoAdicionalClienteService.atualizar({
    body: {
      documentoSindicalId: documentoId,
      informacoesAdicionais,
      observacoesAdicionais,
      orientacao,
      outrasInformacoes
    },
    id: documentoId
  })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao atualizar informação', message: result.error })
  }

  NotificationService.success({ title: 'Salvo com sucesso' })

  return Result.success()
}