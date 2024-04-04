import { ApiService } from "../../../core/index"
import { InformacaoAdicionalClienteService } from "../../../services"
import NotificationService from "../../../utils/notifications/notification.service"

const informacaoAdicionalClienteService = new InformacaoAdicionalClienteService(new ApiService())

export async function obterInformacoesAdicionaisExistentesPorDocumento(documentoId) {
  const result = await informacaoAdicionalClienteService.obterPorDocumento(documentoId)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter informações do documento', message: result.error })
  }

  return result.value
}