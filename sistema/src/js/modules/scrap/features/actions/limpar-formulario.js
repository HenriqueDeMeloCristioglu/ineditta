import $ from 'jquery'

export function limparFormulario(context) {
  const { inputs: { selects, datePickers } } = context

  datePickers.dataInicialDt.clear()
  datePickers.dataFinalDt.clear()
  selects.estruturaClausulaSelect.clear()
  selects.documentoSindicalSelect.clear()
  selects.sinonimoSelect.clear()

  $("#numero").val("")
  $("#texto").val("")
}
