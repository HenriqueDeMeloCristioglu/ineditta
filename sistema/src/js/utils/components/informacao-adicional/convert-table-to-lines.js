import { InformacaoAdicionalTipo } from "../../../application/clausulas/constants/informacao-adicional-tipo"
import DateParser from "../../date/date-parser"


export function convertTableToLines(table, { estruturaId, documentoId, grupoId }) {
  const lines = []

  table.map((row, i) => {
    row.content.map((line, k) => {
      const { command, codigo, type } = line

      let value = command.val()
      let nome = parseInt(row.content[0].command.val()[0].id)

      const isTextArea = (
        type != InformacaoAdicionalTipo.Text &&
        type !== InformacaoAdicionalTipo.Select &&
        type !== InformacaoAdicionalTipo.SelectMultiple &&
        type !== InformacaoAdicionalTipo.Hour &&
        type !== InformacaoAdicionalTipo.Number &&
        type !== InformacaoAdicionalTipo.Date &&
        type !== InformacaoAdicionalTipo.Monetario &&
        type !== InformacaoAdicionalTipo.Percent
      )

      if (codigo === 170) {
        const { description, id } = value[0]
        value = description
        nome = parseInt(id)
      }

      lines.push({
        id: line.codigoId || 0,
        sequenciaLinha: `${i + 1}`,
        sequenciaItem: k + 1,
        grupoId,
        estruturaId,
        documentoId,
        codigo: parseInt(codigo),
        nome,
        texto: type == InformacaoAdicionalTipo.Text ? value : null,
        combo: (type === InformacaoAdicionalTipo.Select || type === InformacaoAdicionalTipo.SelectMultiple) ? value : null,
        hora: type === InformacaoAdicionalTipo.Hour ? value : null,
        percentual: type === InformacaoAdicionalTipo.Percent ? parseFloat(value) : null,
        data: type === InformacaoAdicionalTipo.Date && value ? DateParser.toString(value) : null,
        descricao: isTextArea ? value : null,
        numerico: type === InformacaoAdicionalTipo.Monetario ? parseFloat(value) : type === InformacaoAdicionalTipo.Number ? parseFloat(value) : null
      })
    })
  })

  return lines
}