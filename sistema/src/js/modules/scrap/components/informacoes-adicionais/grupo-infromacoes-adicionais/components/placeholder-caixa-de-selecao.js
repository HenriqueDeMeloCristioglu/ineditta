import { stringTd, stringTr } from "../../../../../../utils/components/string-elements"

export class PlaceholderCaixaDeSelecao {
  constructor() {}

  criar() {
    return stringTr({
      id: 'informacao_adicional_placeholder',
      style: 'color:#bbb;',
      content: stringTd({ content: 'Selecione a classificação da cláusula e as informações desejadas.' })
    })
  }
}