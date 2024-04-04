import { toggleVerArquivo } from "../actions"
import { handleClickAprovarDocumento, handleClickReprocessarDocumento } from "../handlers"
import $ from 'jquery'

export function configurarFormularioModalListaClausula(context) {
  $("#ver_documento_ctn").hide()

  $("#ver_documento_btn").on('click', () => toggleVerArquivo(context))
  $("#aprovar_documento_btn").on('click', async () => await handleClickAprovarDocumento(context))
  $("#reprocessar_documento_btn").on('click', async () => await handleClickReprocessarDocumento(context))
}