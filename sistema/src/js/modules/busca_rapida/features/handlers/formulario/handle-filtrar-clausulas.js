import $ from 'jquery'
import { dataTableClausula } from '../../../components'

export async function handleFiltrarClausulas(context) {
  const { datas } = context

  $("#table-container").show()

  datas.clausulasSelecionadas = []

  await dataTableClausula(context)
}