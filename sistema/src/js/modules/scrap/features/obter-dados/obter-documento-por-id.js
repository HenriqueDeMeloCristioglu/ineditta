import { IADocumentoSindicalService } from "../../../../services"
import { ApiService } from "../../../../core/index"
import NotificationService from "../../../../utils/notifications/notification.service"

const iADocumentoSindicalService = new IADocumentoSindicalService(new ApiService())

export async function obterDocumentoPorId(id) {
  const result = await iADocumentoSindicalService.obterPorId(id)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter clausulas', message: result.error })
  }

  return result.value
}