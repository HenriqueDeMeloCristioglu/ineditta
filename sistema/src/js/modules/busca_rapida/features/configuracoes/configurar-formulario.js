import $ from 'jquery'
import {
  handleClickAbrirClausulaPorId,
  handleClickGerarPdf,
  handleClickLimparFiltro,
  handleClickSelecionarTodasClausulas,
  handleFiltrarClausulas
} from '../handlers'
import { gerarExcel } from '../actions'
import DatepickerrangeWrapper from '../../../../utils/daterangepicker/daterangepicker-wrapper'

export function configurarFormulario(context) {
  const { datas, inputs: { datePickers } } = context

  $("#ano-fil").mask("0000")
  $("#filterBtn").on("click", async () => await handleFiltrarClausulas(context))
  $("#selectAllInput").on("change", handleClickSelecionarTodasClausulas)
  $("#limparFiltroBtn").on("click", () => handleClickLimparFiltro(context))
  $("#texto_clausula").prop("disabled", true)
  $(".horizontal-nav").removeClass("hide")
  $("#collapseDadosCadastrais").on("click", () => $("#collapseDadosCadastraisBody").collapse("toggle"))
  $("#collapseLocalizacao").on("click", () => $("#collapseLocalizacaoBody").collapse("toggle"))
  $("#collapseContato").on("click", () => $("#collapseContatoBody").collapse("toggle"))
  $("#collapseAssociacoes").on("click", () => $("#collapseAssociacoesBody").collapse("toggle"))
  $("#abrirClausulaBtn").on("click", () => handleClickAbrirClausulaPorId({ context }))
  $("#gerarPDFBtn").on("click", async () => await handleClickGerarPdf(context))
  $("#dropdownMenu2").dropdown()
  $("#gerarExcelBtn").on("click", async () => await gerarExcel({ clausulasIds: datas.clausulasSelecionadas }))
  $("#gerarExcelBtn2").on("click", async () => await gerarExcel({ clausulasIds: datas.clausulasSelecionadas }))

  datePickers.processamentoDp = new DatepickerrangeWrapper('#processamento')
}