import $ from 'jquery'

export function handleClickLimparFiltro(context) {
  const { datas, dataTables, inputs: { selects, datePickers } } = context

  if (datas.isIneditta) {
    selects.gruposEconomicosSelect.reload()
    selects.empresasSelect.reload()
    selects.estabelecimentosSelect.reload()
  }

  if (datas.isGrupoEconomico) {
    selects.empresasSelect.reload()
    selects.estabelecimentosSelect.reload()
  }

  if (datas.isEmpresa) {
    selects.empresasSelect?.isEnable() && selects.empresasSelect.reload()
    selects.estabelecimentosSelect.reload()
  }

  if (datas.isEstabelecimento && selects.estabelecimentosSelect?.isEnable()){
    selects.estabelecimentosSelect.reload()
  } 

  $("#data_base").val(null).trigger("change")
  selects.localizacoesSelect.reload()
  selects.cnaeSelect.reload()
  selects.sindLaboralSelect.reload()
  selects.sindPatronalSelect.reload()
  selects.tipoDocSelect.reload()
  selects.grupoClausulaSelect.reload()
  selects.estruturaClausulaSelect.reload()
  datePickers.processamentoDp.clear()

  if (dataTables.clausulasTb) {
    dataTables.clear()
    $("#table-container").hide()
  }
}