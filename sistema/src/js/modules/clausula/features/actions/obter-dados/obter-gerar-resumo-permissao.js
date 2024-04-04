import { ClausulaGeralService } from "../../../../../services"
import NotificationService from "../../../../../utils/notifications/notification.service"
import { ApiService } from "../../../../../core/index"

const clausulaGeralService = new ClausulaGeralService(new ApiService())

export async function obterGerarResumoPermissao(documentoId) {
  const result = await clausulaGeralService.obterResumiveisPorDocumento(documentoId)

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro ao obter permissão de gerar resumo', message: result.error })

    return false
  }

  return result.value?.length > 0
}