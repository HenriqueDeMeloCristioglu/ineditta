import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { AcompanhamentoCctService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const acompanhamentoCctService = new AcompanhamentoCctService(new ApiService())

export async function incluirAcompanhamento(data) {  
  const result = await acompanhamentoCctService.salvar(data)
  
  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error })

    return Result.failure()
  }
  
  NotificationService.success({ title: "Sucesso", message: "Acompanhamento cadastrado com sucesso!" })
  
  return Result.success()
}
