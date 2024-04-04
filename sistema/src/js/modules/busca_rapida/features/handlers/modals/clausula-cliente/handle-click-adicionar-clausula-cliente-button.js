import { closeModal } from "../../../../../../utils/modals/modal-wrapper"
import NotificationService from "../../../../../../utils/notifications/notification.service"
import { adicionarClausulaCliente } from "../../../actions"
import $ from 'jquery'

export async function handleClickAdicionarClausulaCliente({ modalContainer, context }) {
  const { datas } = context

  const result = await adicionarClausulaCliente({ clausulaId: datas.clausulaId, texto: $('#texto_clausula_cliente').val() })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao cadastrar cláusula cliente', error: result.error })
  }

  NotificationService.success({ title: 'Cadastrado com sucesso' })

  closeModal(modalContainer)

  $('#openClausulaModalBtn').trigger('click')
}