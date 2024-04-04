import { InformacaoAdicionalTipo } from "../../../../application/clausulas/constants/informacao-adicional-tipo"
import DateParser from "../../../../utils/date/date-parser"

export function obterInformacoesAdicionaisDiferentes(informacoesAdicionais) {
  const items = []

  informacoesAdicionais && informacoesAdicionais.map((informacaoAdicional) => {
    const { grupos: linhas } = informacaoAdicional

    linhas.map(({ informacoesAdicionais }) => {
      informacoesAdicionais.map((item) => {
        if (!item || !item.element) return

        const { element, dado, tipo, id } = item

        const data = element.val()
        if (!data) return

        switch (tipo) {
          case InformacaoAdicionalTipo.Select:
            if (dado.combo.valor != data) createItem(id, data)
            break
          case InformacaoAdicionalTipo.SelectMultiple:
            if ((dado.combo.valor).join(', ') != (data).join(', ')) createItem(id, data.join(', '))
            break
          case InformacaoAdicionalTipo.Hour:
            if (dado.hora != data) createItem(id, data)
            break
          case InformacaoAdicionalTipo.Monetario:
            if (parseFloat(dado.numerico) != parseFloat(data)) {
              createItem(id, data)
            }
            break
          case InformacaoAdicionalTipo.Number:
            if (parseFloat(dado.numerico) != parseFloat(data)) {
              createItem(id, data)
            }
            break
          case InformacaoAdicionalTipo.Percent:
            if (parseFloat(dado.percentual) != parseFloat(data)) createItem(id, data)
            break
          case InformacaoAdicionalTipo.Text:
            if (dado.texto != data) createItem(id, data)
            break
          case InformacaoAdicionalTipo.Date:
            const value = DateParser.toString(data)

            if (dado.data != value) createItem(id, value)
            break
          default:
            if (dado.descricao != data) createItem(id, data)
            break
        }
      })
    })
  })

  return items

  function createItem(id, data) {
    items.push({
      id,
      valor: data
    })
  }
}