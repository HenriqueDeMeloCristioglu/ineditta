import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { AcompanhamentoCctService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const acompanhamentoCctService = new AcompanhamentoCctService(new ApiService())

export async function atualizarAcompanhamento(data) {

  const result = await acompanhamentoCctService.editar(data)

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error })

    return Result.failure()
  }

  NotificationService.success({ title: "Sucesso", message: "Acompanhamento editado com sucesso!" })

  return Result.success()
}
