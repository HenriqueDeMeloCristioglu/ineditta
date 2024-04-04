import { ApiService } from "../../../../core/index"
import { ClausulaGeralService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const clausulaGeralService = new ClausulaGeralService(new ApiService())

export async function aprovarClausula(id) {
  const result = await clausulaGeralService.aprovar(id)

  if (result.isFailure()) {
    return NotificationService.error({
      title: 'Erro ao aprovar Clausula',
      message: result.error
    })
  }

  NotificationService.success({ title: 'Clausula aprovada com sucesso' })
}