import $ from 'jquery'

export function handleCloseModalListaClausula(context) {
  const { datas } = context

  datas.documentoId = null
  datas.documentoReferenciaId = null
  datas.documentoSelecionado = null
  datas.documentoPath = null
  datas.ordernarPorStatusConsistente = false

  $("#lista_clausulas").html('')
  $("#ver_documento_ctn").hide()
}