import { ApiService } from "../../../../../core/index"
import Result from "../../../../../core/result"
import { IADocumentoSindicalService } from "../../../../../services"
import NotificationService from "../../../../../utils/notifications/notification.service"

const iADocumentoSindicalService = new IADocumentoSindicalService(new ApiService())

export async function aprovarDocumento(id) {
  const result = await iADocumentoSindicalService.aprovar(id)

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro ao aprovar documento", message: result.error })
    
    return Result.failure()
  }

  NotificationService.success({ title: "Documento aprovado com sucesso" })
  
  return Result.success()
}