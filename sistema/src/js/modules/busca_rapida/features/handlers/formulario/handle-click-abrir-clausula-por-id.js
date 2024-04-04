import NotificationService from "../../../../../utils/notifications/notification.service"
import $ from 'jquery'

export function handleClickAbrirClausulaPorId({ context, id = null }) {
  const { datas } = context

  if (datas.clausulasSelecionadas.length === 0 && !datas.clausulaClicada) {
    return NotificationService.error({ title: "Não é possível abrir", message: "Selecione pelo menos uma cláusula!" })
  }

  if(id) {
    datas.clausulaClicada = id
  }

  $("#openClausulaModalBtn").trigger("click")
}