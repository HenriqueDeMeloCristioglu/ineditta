import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { DocSindService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const docSindService = new DocSindService(new ApiService())

export async function atualizarDataSla({ id, novaDataSla }) {
  const result = await docSindService.atualizarSla(id, novaDataSla)

  if (result.isFailure()) {
    return NotificationService.error({ title: "Não foi possível atualizar a data sla", message: result.error })
  }

  NotificationService.success({title: "Data SLA atualizada!"})

  return Result.success()
}