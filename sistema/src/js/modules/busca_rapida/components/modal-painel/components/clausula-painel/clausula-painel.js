import { div } from "../../../../../../utils/components/elements"
import { ClausulaHeaderPainel, ClausulaBodyPainel } from "./components"

export class ClausulaPainel {
  constructor(id, data, estabelecimentoPainel) {
    this.id = id
    this.data = data
    this.estabelecimentoPainel = estabelecimentoPainel
  }

  create() {
    const clausulaHeaderPainel = new ClausulaHeaderPainel({ id: this.id, data: this.data })
    const clausulaHeaderPainelWrapper = clausulaHeaderPainel.create()
    
    const clausulaBodyPainel = new ClausulaBodyPainel({ id: this.id, data: this.data, estabelecimentoPainel: this.estabelecimentoPainel })
    const clausulaBodyPainelWrapper = clausulaBodyPainel.create()
    
    return div({ className: 'panel panel-primary', content: [clausulaHeaderPainelWrapper, clausulaBodyPainelWrapper] })
  }
}