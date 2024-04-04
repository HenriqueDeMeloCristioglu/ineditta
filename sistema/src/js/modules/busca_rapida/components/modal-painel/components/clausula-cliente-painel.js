
import { a, div, h4, i, p } from "../../../../../utils/components/elements"


export class ClausulaClientePainel {
  constructor({ data, id }) {
    this.id = id
    this.data = data
  }

  create() {
    const data = this.data
    const id = this.id
    const texto = data.textoResumidoCliente
  
    const textoClausulaClienteTitulo = h4({ content: `Regra empresa da cláusula: ${data.nomeClausula}` })
    const textoClausulaClienteOptions = div({ className: "options", content: a({
        href: 'javascript:;',
        id: `collapseClauText-${id}`,
        className: 'panel-collapse',
        content: i({ className: 'fa fa-chevron-down' })
      })
    })
    const textoClausulaClientePainelHeading = div({ className: "panel-heading", content: [textoClausulaClienteTitulo, textoClausulaClienteOptions] })
  
    const textoClausulaCliente = p({ style: 'text-align: justify; white-space: pre-line;', content: texto })
    const textoClausulaClientePainelBody = div({ className: 'panel-body collapse in', id: `collapseClauText-${id}-body`, content: [textoClausulaCliente] })
    const textoClausulaClientePainel = div({ className: "panel panel-primary", content: [textoClausulaClientePainelHeading, textoClausulaClientePainelBody] })
  
    return textoClausulaClientePainel
  }
}