import { ApiService } from "../../../../core/index"
import { ClausulaGeralService } from "../../../../services"
import DateFormatter from "../../../../utils/date/date-formatter"
import NotificationService from "../../../../utils/notifications/notification.service"
import PageWrapper from "../../../../utils/pages/page-wrapper"

const clausulaGeralService = new ClausulaGeralService(new ApiService())

export async function gerarRelatorio({ documentoId }) {
  const result = await clausulaGeralService.downloadRelatorioClausulas({ documentoId })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao baixar excel', message: result.error })
  }

  const bytes = result.value.data.blob

  const data = DateFormatter.dayMonthYear(new Date())
  const dataArray = data.split('/')

  const fileName = `relatório_clausulas_ineditta_${dataArray[0]}_${dataArray[1]}_${dataArray[2]}.xlsx`

  PageWrapper.downloadExcel(bytes, fileName)
}