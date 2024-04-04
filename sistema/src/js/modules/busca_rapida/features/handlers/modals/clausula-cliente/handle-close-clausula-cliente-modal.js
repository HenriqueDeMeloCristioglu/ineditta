import $ from 'jquery'

export function handleCloseClausulaClienteModal(context) {
  const { datas } = context

  $('#texto_clausula_cliente').val('')
  $('#nome_clausula_cliente').val('')

  datas.clausulaId = 0
  datas.clausulaCliente = null
}