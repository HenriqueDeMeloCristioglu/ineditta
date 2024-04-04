import { ApiService } from "../../../core/index"
import { DocSindService } from "../../../services"
import NotificationService from "../../../utils/notifications/notification.service"

const docSindService = new DocSindService(new ApiService())

export async function obterPorId(id){
  const result = await docSindService.obterDocSindPorId(id)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter tipo de documento', message: result.error })
  }

  return result.value
}