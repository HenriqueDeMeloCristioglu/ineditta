import $ from 'jquery'

import { gerarEventosPerguntas, gerarPerguntas } from './index'

export function configurarScripts({ jform }) {
  const data = JSON.parse(jform)
  const questionario = $("#questionario-fase")

  let idsForms = ""

  data.perguntas.map((pergunta) => {
    const perguntas = gerarPerguntas(pergunta)

    perguntas.map((item) => {
      const { id, html } = item

      questionario.append(html)

      gerarEventosPerguntas(item)

      idsForms += " " + id
    })
  })

  idsForms.trim().length === 0 && questionario.html("Não há nenhuma pergunta para esta fase.")

  return idsForms
}