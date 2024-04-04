import { ApiService } from "../../../../core/index"
import { DocumentosLocalizadosService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const documentosLocalizadosService = new DocumentosLocalizadosService(new ApiService())

export async function obterUrlDocumento(id) {
  const response = await documentosLocalizadosService.download(id)
  
  if (response.isFailure()) {
    return NotificationService.error({ title: 'Não foi possível baixar o arquivo.' })
  }

  const blobFile = new Blob([response.value.data], { type: response.value.contentType })
  const urlFile = URL.createObjectURL(blobFile)

  return urlFile
}

export async function obterUrlDocumentoPorDocumentoSindicalId(id) {
  const response = await documentosLocalizadosService.downloadPorDocumentoSindicalId(id)
  
  if (response.isFailure()) {
    return NotificationService.error({ title: 'Não foi possível baixar o arquivo.' })
  }

  const blobFile = new Blob([response.value.data], { type: response.value.contentType })
  const urlFile = URL.createObjectURL(blobFile)

  return urlFile
}