import { ApiService } from "../../../../core/index"
import { ClausulaGeralService } from "../../../../services"
import DateFormatter from "../../../../utils/date/date-formatter"
import NotificationService from "../../../../utils/notifications/notification.service"
import PageWrapper from "../../../../utils/pages/page-wrapper"

const clausulaGeralService = new ClausulaGeralService(new ApiService())

export async function gerarExcel({ clausulasIds }) {
  const result = await clausulaGeralService.gerarExcelBuscaRapida({ clausulasIds })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao baixar excel', message: result.error })
  }

  const today = DateFormatter.dayMonthYear(new Date()).replace('-','_')
  PageWrapper.downloadExcel(result.value.data.blob, `relatório_busca_rápida_${today}.xlsx`)
}