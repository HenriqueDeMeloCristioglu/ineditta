import { generateJfaseOptions } from "../../../../utils/components/jfase/jfase-options"
import { createJOption } from "../../../../utils/components/jfase/jform"
import { Generator } from "../../../../utils/generator"

export function gerarPerguntas(pergunta) {
  const { tipo, texto: text, opcoes, adicionais } = pergunta

  let html = ""
  let opcs = ""

  let perguntas = []

  let id = Generator.id()

  if (opcoes && opcoes.length > 0) {
    opcoes.map((opcao) => {
      opcs += createJOption(opcao)
    })
  }

  // Pergunta
  const jfaseOptions = generateJfaseOptions({ text, options: opcs, id })
  html += findJOption({ tipo, options: jfaseOptions })

  perguntas.push({
    ...pergunta,
    id: html.length > 0 ? id : "",
    html,
  })

  // Perguntas Adicionais
  if (adicionais) {
    adicionais.map((adicional) => {
      const { tipo, texto: text, opcoes } = adicional

      let idAdicional = Generator.id()
      let opsAdicionais = ""
      let htmlAdicional = ""

      if (opcoes && opcoes.length > 0) {
        opcoes.map((opcao) => {
          opsAdicionais += createJOption(opcao)
        })
      }

      const jfaseOptions = generateJfaseOptions({
        text,
        options: opsAdicionais,
        id: idAdicional,
        className: `${id}-adicional`,
      })
      htmlAdicional += findJOption({ tipo, options: jfaseOptions })

      perguntas.push({
        ...adicional,
        id: html.length > 0 ? idAdicional : "",
        parentId: id,
        html: htmlAdicional,
      })
    })
  }

  return perguntas

  function findJOption({ tipo, options }) {
    let htmlContent = ""
    options.map(
      ({ type, content }) => tipo === type && (htmlContent += content)
    )

    return htmlContent
  }
}