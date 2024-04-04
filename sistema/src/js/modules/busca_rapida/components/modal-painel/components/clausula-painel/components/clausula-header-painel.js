import { a, div, h4, i } from "../../../../../../../utils/components/elements"

export class ClausulaHeaderPainel {
  constructor({ id, data }) {
    this.id = id
    this.data = data
  }

  create() {
    const painelHeading = div({ className: 'panel-heading' })
    const titulo = h4({ content: `Resumo da cláusula: ${this.data.nomeClausula}` })
    const options = div({ className: 'options' })
    const closeBtn = a({
      className: 'panel-collapse',
      href: 'javascript:;',
      id: `collapseClau-${this.id}`,
      content: i({ className: 'fa fa-chevron-down' })
    })

    options.append(closeBtn)
    painelHeading.append([titulo, options])
    
    return painelHeading
  }
}
