import { ApiService } from "../../../core/index"
import { AcompanhamentoCctService } from "../../../services"
import DateFormatter from "../../../utils/date/date-formatter"
import NotificationService from "../../../utils/notifications/notification.service"
import PageWrapper from "../../../utils/pages/page-wrapper"

const apiService = new ApiService()
const acompanhamentoCctService = new AcompanhamentoCctService(apiService)

export async function gerarRelatorioExcel(params) {
  const result = await acompanhamentoCctService.downloadRelatorioNegociacoes(params)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao baixar excel', message: result.error })
  }

  const data = result.value.data

  const date = DateFormatter.dayMonthYear(new Date())
  const dataArray = date.split('/')

  const fileName = `relatório_acompanhamento_cct_ineditta_${dataArray[0]}_${dataArray[1]}_${dataArray[2]}.xlsx`

  PageWrapper.downloadExcel(data.blob, fileName)
}