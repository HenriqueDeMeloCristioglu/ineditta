import NotificationService from "../../../../../utils/notifications/notification.service"
import { gerarPdf } from "../../actions"

export async function handleClickGerarPdf(context) {
  const { datas, inputs: { datePickers } } = context

  if (!datas.clausulaClicada) {
    if(!(datas.clausulasSelecionadas instanceof Array) || datas.clausulasSelecionadas.length == 0) {
      return NotificationService.error({ title: "Não é possível gerar o PDF", message: "Selecione pelo menos uma cláusula!" })
    }
  }

  const requestData = {}

  if (datePickers.processamentoDp?.hasValue()) {
    requestData['dataProcessamentoInicial'] = datePickers.processamentoDp.getBeginDate()
    requestData['dataProcessamentoFinal'] = datePickers.processamentoDp.getEndDate()
  }
  
  await gerarPdf({
    clausulasIds: datas.clausulasSelecionadas,
    dataProcessamentoInicial: requestData['dataProcessamentoInicial'],
    dataProcessamentoFinal: requestData['dataProcessamentoFinal']
  })
}