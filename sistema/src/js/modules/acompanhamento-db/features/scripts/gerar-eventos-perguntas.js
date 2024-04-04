import $ from 'jquery'
import { JfaseInputType } from '../../../../application/jfase/constants/jfsae-input-type'

export function gerarEventosPerguntas(pergunta) {
  const { tipo, mask, adicionais, id, parentId } = pergunta
  const el = $(`#${id}`)

  const elAdicional = $(`.${parentId}-adicional`)
  if (elAdicional.length > 0) {
    elAdicional.hide()
  }

  if (tipo === JfaseInputType.Text && mask) {
    el.maskCustom(mask)
  }

  if (
    tipo === JfaseInputType.Select ||
    tipo === JfaseInputType.SelectMultiple
  ) {
    el.select2()
  }

  if (tipo === JfaseInputType.Radio && adicionais) {
    const radios = $(`input[type="radio"]`)

    radios.on("change", (el) => {
      const value = el.target.value
      const name = el.target.name

      if (value == "Sim") {
        $(`.${name}-adicional`).show()
      } else {
        $(`.${name}-adicional`).hide()
      }
    })
  }
}