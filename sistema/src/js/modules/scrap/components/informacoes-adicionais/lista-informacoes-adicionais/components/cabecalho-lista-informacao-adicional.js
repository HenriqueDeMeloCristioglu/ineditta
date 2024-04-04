import { stringTh, stringThead, stringTr } from "../../../../../../utils/components/string-elements"
import { Generator } from "../../../../../../utils/generator"

export class CabecalhoListaInformacaoAdicional {
  constructor(data) {
    this.data = data
  }

  create() {
    return stringThead({
      children: stringTr({
        content: this.data.map(({ tipoInformacaoNome }) => stringTh({
          className: 'infoGrupo',
          id: Generator.id(),
          content: tipoInformacaoNome
        }))
      })
    })
  }
}
