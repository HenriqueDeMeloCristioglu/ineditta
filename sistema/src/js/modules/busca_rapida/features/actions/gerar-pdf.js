import { ApiService } from "../../../../core/index"
import { ClausulaGeralService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"
import PageWrapper from "../../../../utils/pages/page-wrapper"
import { MediaType } from "../../../../utils/web"

const clausulaGeralService = new ClausulaGeralService(new ApiService())

export async function gerarPdf({ clausulasIds, dataProcessamentoInicial = null, dataProcessamentoFinal = null }) {
  const result = await clausulaGeralService.gerarPdfBuscaRapida({
    clausulasIds,
    dataProcessamentoInicial,
    dataProcessamentoFinal
  })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao baixar pdf', message: result.error })
  }

  NotificationService.success({ title: 'Baixado com sucesso', message: result.error })

  PageWrapper.download(result.value.data.blob, result.value.data.filename, MediaType.pdf["Content-Type"])
}