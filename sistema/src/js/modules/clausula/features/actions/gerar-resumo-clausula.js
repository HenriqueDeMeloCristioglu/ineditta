import { ApiService } from "../../../../core/index"
import { ClausulaGeralService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const clausulaGeralService = new ClausulaGeralService(new ApiService())

export async function gerarResumoClausula(documentoId) {
  const result = await clausulaGeralService.gerarResumo(documentoId)

  if (result.isFailure()) {
    return NotificationService.error({ title: "Erro ao gerar resumo", message: result.error })
  }

  return NotificationService.success({ title: "Resumo gerado com sucesso" })
}