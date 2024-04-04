import { ApiService } from "../../../../core/index"
import { DocSindService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"
import PageWrapper from "../../../../utils/pages/page-wrapper"
import { MediaType } from "../../../../utils/web"

const docSindService = new DocSindService(new ApiService())

export async function downloadDocumento(id) {
  const response = await docSindService.download({ id })

  if (response.isFailure()) {
    return NotificationService.error({ title: 'Não foi possível baixar o arquivo.' })
  }

  const value = response.value

  PageWrapper.download(value.data.blob, value.data.filename, MediaType.stream.Accept)
}
