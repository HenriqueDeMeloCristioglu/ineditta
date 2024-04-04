import { ApiService } from "../../../../../core/index"
import Result from "../../../../../core/result"
import { IAClausulaService } from "../../../../../services"
import NotificationService from "../../../../../utils/notifications/notification.service"

const iAClausulaService = new IAClausulaService(new ApiService())

export async function atualizarClausula({
  id,
  texto,
  documentoSindicalId,
  estruturaClausulaId,
  numero,
  sinonimoId,
  status
}) {
  const result = await iAClausulaService.atualizar({
    id, 
    texto,
    documentoSindicalId,
    estruturaClausulaId,
    numero,
    sinonimoId,
    status
  })

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro ao atualizar cláusula", message: result.error })
    
    return Result.failure()
  }

  NotificationService.success({ title: "Cláusula atualizada com sucesso" })

  return Result.success()
}
