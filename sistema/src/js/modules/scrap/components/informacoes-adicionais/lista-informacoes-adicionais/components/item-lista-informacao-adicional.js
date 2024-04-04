import { configOpcaoAdicional, stringSearchOpcaoAdicional } from "../../../../../../utils/components/informacao-adicional"
import { stringTd } from "../../../../../../utils/components/string-elements"
import { Generator } from "../../../../../../utils/generator"
import '../../../../../../utils/masks/jquery-mask-extensions'

export class ItemListaInformacaoAdicional {
  constructor(tipo, cdInformacaoId, combo = null) {
    this.item = {
      id: 0,
      type: tipo,
      codigo: cdInformacaoId,
      data: {
        options: combo?.options
      },
      input: null
    }
  }

  create() {
    const { content, id } = stringSearchOpcaoAdicional({ input: this.item.type })

    this.item.id = id

    return stringTd({ id: Generator.id(), content })
  }

  init() {
    configOpcaoAdicional(this.item)
  }
}
