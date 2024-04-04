import $ from 'jquery'

export function handleClickAdicionarRegraEmpresaBtn({ id, context }) {
  const { datas } = context

  datas.clausulaId = id

  $('#openClausulaModalBtn').trigger('click')
  $('#openClausulaClienteModalBtn').trigger('click')
}
