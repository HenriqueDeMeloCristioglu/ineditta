import { ApiService } from "../../../../../core/index"
import Result from "../../../../../core/result"
import { IAClausulaService } from "../../../../../services"
import NotificationService from "../../../../../utils/notifications/notification.service"

const iAClausulaService = new IAClausulaService(new ApiService())

export async function deletarClausula(id) {
  const result = await iAClausulaService.excluir(id)

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro ao deletar cláusula", message: result.error })
    
    return Result.failure()
  }

  NotificationService.success({ title: "Cláusula deletada com sucesso" })

  return Result.success()
}