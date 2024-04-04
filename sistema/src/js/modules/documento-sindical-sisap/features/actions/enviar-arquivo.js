import { ApiService } from "../../../../core/index"
import { DocumentosLocalizadosService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"

const documentosLocalizadosService = new DocumentosLocalizadosService(new ApiService())

export async function enviarArquivo({ arquivo, uf, origem }) {
  if (!arquivo) {
    return NotificationService.error({ title: "Você deve selecionar um arquivo." })
  }

  const params = {
    arquivo,
    origem,
    uf
  }

  const result = await documentosLocalizadosService.upload(params)

  if (!result.success){
    return NotificationService.error({ title: "Não foi possível realizar o upload", message: result.error })
  }

  NotificationService.success({ title: "Upload realizado com sucesso" })
}