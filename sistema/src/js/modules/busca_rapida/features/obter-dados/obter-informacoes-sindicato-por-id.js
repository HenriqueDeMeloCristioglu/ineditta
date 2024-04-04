import Result from "../../../../core/result"
import NotificationService from "../../../../utils/notifications/notification.service"
import { useSindicatoService } from "../../../core/hooks"

const sindicatoService = useSindicatoService()

export async function obterInformacoesSindicatoPorId(id, tipoSind) {
  const result = await sindicatoService.obterInfoSindical(id, tipoSind)

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro ao busca informações de sindicato', message: result.error })
    
    return Result.failure()
  }

  return Result.success(result.value)
}