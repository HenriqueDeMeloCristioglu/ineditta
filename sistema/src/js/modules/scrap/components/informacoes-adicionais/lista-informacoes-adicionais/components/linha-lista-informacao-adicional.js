import { stringTr } from "../../../../../../utils/components/string-elements"
import { Generator } from "../../../../../../utils/generator"
import { ItemListaInformacaoAdicional } from "./item-lista-informacao-adicional"

export class LinhaListaInformacaoAdicional {
  constructor(data) {
    this.data = data
    this.colunas = []
    this.id = Generator.id()
  }

  create() {
    return stringTr({
      id: this.id,
      content: this.data.map(({ tipoDadadoId, combo, cdInformacaoId }) => {
        const itemListaInformacaoAdicional = new ItemListaInformacaoAdicional(tipoDadadoId, cdInformacaoId, combo)

        const stringInput = itemListaInformacaoAdicional.create()

        this.colunas.push(itemListaInformacaoAdicional)

        return stringInput
      })
    })
  }

  init() {
    this.colunas.map(coluna => coluna.init())
  }
}
