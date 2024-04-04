export function limparFiltros(context) {
  const { selects, datePickers } = context.inputs

  selects.grupoEconomicoFiltroSelect.clear()
  selects.empresaFiltroSelect.clear()
  selects.grupoOperacaoFiltroSelect.clear()
  selects.atividadeEconomicaFiltroSelect.clear()
  selects.documentoFiltroSelect.clear()
  selects.nomeDocumentoFiltroSelect.clear()
  selects.statusDocumentoFiltroSelect.clear()
  selects.statusClausulasFiltroSelect.clear()

  datePickers.dataSlaDt.clear()
}