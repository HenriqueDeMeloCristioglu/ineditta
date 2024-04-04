import { ApiService } from "../../../../../core/index"
import Result from "../../../../../core/result"
import { IADocumentoSindicalService } from "../../../../../services"
import NotificationService from "../../../../../utils/notifications/notification.service"

const iADocumentoSindicalService = new IADocumentoSindicalService(new ApiService())

export async function reprocessarDocumento(id) {
  const result = await iADocumentoSindicalService.reprocessar(id)

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro ao reprocessar documento", message: result.error })
    
    return Result.failure()
  }

  NotificationService.success({ title: "Documento reprocessar com sucesso" })
  
  return Result.success()
}