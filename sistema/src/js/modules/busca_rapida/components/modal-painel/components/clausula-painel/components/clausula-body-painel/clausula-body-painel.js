import { div } from "../../../../../../../../utils/components/elements"
import { ClausulaColuna, DataColuna, SindicatoLaboralColuna, SindicatoPatronalColuna } from "./components"

export class ClausulaBodyPainel {
  constructor({ id, data, estabelecimentoPainel }) {
    this.id = id
    this.data = data
    this.estabelecimentoPainel = estabelecimentoPainel
  }

  create() {
    const sindicatoLaboralColuna = new SindicatoLaboralColuna(this.data)
    const sindicatoLaboralColunaWrapper = sindicatoLaboralColuna.create()

    const sindicatoPatronalColuna = new SindicatoPatronalColuna(this.data)
    const sindicatoPatronalColunaWrapper = sindicatoPatronalColuna.create()

    const clausulaColuna = new ClausulaColuna(this.data)
    const clausulaColunaWrapper = clausulaColuna.create()

    const dataColuna = new DataColuna(this.data)
    const dataColunaWrapper = dataColuna.create()
  
    
    const painelBody = div({ className: 'panel-body collapse in', id: `collapseClau-${this.id}-body` })
    const mainRow = div({ className: 'row' })
    const sindicatosColunas = div({ className: 'col-lg-8 sind_column', id: `sindcolumn-${this.id}` })

    sindicatosColunas.append([sindicatoLaboralColunaWrapper, sindicatoPatronalColunaWrapper, clausulaColunaWrapper, dataColunaWrapper])
    mainRow.append([sindicatosColunas, this.estabelecimentoPainel])
    painelBody.append([mainRow])
  
    return painelBody
  }
}