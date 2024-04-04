import { ApiService } from "../../../../../core/index"
import Result from "../../../../../core/result"
import { IAClausulaService } from "../../../../../services"
import NotificationService from "../../../../../utils/notifications/notification.service"

const iAClausulaService = new IAClausulaService(new ApiService())

export async function incluirClausula({
  texto,
  documentoSindicalId,
  estruturaClausulaId,
  numero,
  sinonimoId,
  status
}) {
  const result = await iAClausulaService.incluir({
    texto,
    documentoSindicalId,
    estruturaClausulaId,
    numero,
    sinonimoId,
    status
  })

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro ao incluir cláusula", message: result.error })
    
    return Result.failure()
  }

  NotificationService.success({ title: "Cláusula criada com sucesso" })

  return Result.success()
}
