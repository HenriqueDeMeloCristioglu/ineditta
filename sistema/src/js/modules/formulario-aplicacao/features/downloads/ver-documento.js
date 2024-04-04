import { ApiService } from "../../../../core/index"
import { DocSindService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"
import PageWrapper from "../../../../utils/pages/page-wrapper"
import { MediaType } from "../../../../utils/web"

const apiService = new ApiService()
const docSindService = new DocSindService(apiService)

export async function verDocumento(id) {
  const response = await docSindService.download({ id })

  if (response.isFailure()) {
    return NotificationService.error({ title: 'Não foi possível ver o arquivo.' })
  }

  const value = response.value

  PageWrapper.preview(value.data.blob, MediaType.pdf.Accept)
}