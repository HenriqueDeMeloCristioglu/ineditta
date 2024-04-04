import { limparFormulario } from "../../../actions"
import $ from 'jquery'

export function handleCloseModalUpsertClausula(context) {
  const { datas } = context

  datas.clausulaSelecionada = null
  datas.clausulaId = null

  $('#informacao_adicional_grupo_painel').hide()
  $('#infoAdicional_grupo_lista_selecao').html('')
  $("#infoAdicionalGrupoSelecao").html('')

  $("#table-grupo-add").hide()
  $('#table_grupo').html('')

  limparFormulario(context)
}