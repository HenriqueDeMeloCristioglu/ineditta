import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { InformacaoAdicionalService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const informacaoAdicionalService = new InformacaoAdicionalService(new ApiService())

export async function obterInformacoesAdicionaisGrupoPorClausula(id) {
  const result = await informacaoAdicionalService.obterInformacoesAdicionaisPorClausula(id)

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro ao busca informações adicionais por cláusula', message: result.error })

    return Result.failure()
  }

  return Result.success(result.value)
}