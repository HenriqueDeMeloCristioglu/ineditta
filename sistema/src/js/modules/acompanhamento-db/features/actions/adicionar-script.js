import NotificationService from '../../../../utils/notifications/notification.service'
import { ApiService } from '../../../../core/index'
import { AcompanhamentoCctService } from '../../../../services'

const apiService = new ApiService()
const acompanhamentoCctService = new AcompanhamentoCctService(apiService)

export async function adicionarScript(data) {
  const result = await acompanhamentoCctService.adionarScript(data)

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error })

    return result
  }

  NotificationService.success({ title: "Sucesso", message: "Script processado com sucesso!" })
}