import { ApiService } from "../../../../../core/index"
import { SinonimosService } from "../../../../../services"
import NotificationService from "../../../../../utils/notifications/notification.service"

const sinonimosService = new SinonimosService(new ApiService())

export async function obterSinonimos(id) {
  const result = await sinonimosService.obterAssuntoPorIdSelect(id)

  if (!result.success) {
    return NotificationService.error({ title: 'Erro ao buscar assunto por sinônimo', message: result.error })
  }
}