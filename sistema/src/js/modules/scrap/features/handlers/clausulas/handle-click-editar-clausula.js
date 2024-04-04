import $ from 'jquery'

export function handleClickEditarClausula({ context, id }) {
  context.datas.clausulaId = id

  $("#clausulaBtn").trigger('click')
}