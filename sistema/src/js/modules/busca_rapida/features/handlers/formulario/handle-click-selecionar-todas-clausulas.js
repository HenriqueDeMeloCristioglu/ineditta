import $ from 'jquery'

export function handleClickSelecionarTodasClausulas(event) {
  if (event.currentTarget.checked) {
    $(".clausula_checkbox").prop("checked", true).trigger("change")
  } else {
    $(".clausula_checkbox").prop("checked", false).trigger("change")
  }
}