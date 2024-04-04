import $ from 'jquery'

export function handleCloseClausulaModal(context) {
  const { datas } = context

  datas.clausulaClicada = null
  $("#clausulaModalContainer").html(null)
}
