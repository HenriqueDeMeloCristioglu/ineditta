import { ApiService } from "../../../core/index"
import { EmailStorageManagerService } from "../../../services"
import NotificationService from "../../../utils/notifications/notification.service"
import { MediaType } from "../../../utils/web"
import PageWrapper from "../../../utils/pages/page-wrapper"

const emailStorageManagerService = new EmailStorageManagerService(new ApiService())

export async function gerarRelatorio() {
  const result = await emailStorageManagerService.obterRelatorio()

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao baixa relatório', message: result.error })
  }

  NotificationService.success({ title: 'Relatório gerado com sucesso' })

  const value = result.value

  PageWrapper.download(value.data.blob, value.data.filename, MediaType.stream.Accept)
}