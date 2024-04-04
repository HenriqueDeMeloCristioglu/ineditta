import { ApiService } from "../../../../../core/index"
import { DocSindService } from "../../../../../services"
import NotificationService from "../../../../../utils/notifications/notification.service"

const docSindService = new DocSindService(new ApiService())

export async function obterConfiguracoesEmailPorDocumento({ documentoId }) {
  const result = await docSindService.obterConfiguracoesEmailPorId(documentoId)

  if (result.isFailure()) return NotificationService.error({ title: 'Erro ao buscar configurações do email', message: result.error })

  return result.value
}