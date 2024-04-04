import { a, button, div, h4, i, p } from "../../../../../utils/components/elements"
import NotificationService from "../../../../../utils/notifications/notification.service"

export class TextoClausulaPainel {
  constructor({
    data,
    id,
    handleClickAdicionarRegraEmpresaBtn,
    showAdicionarRegrasEmpresaBtn
  }) {
    this.data = data
    this.id = id
    this.handleClickAdicionarRegraEmpresaBtn = handleClickAdicionarRegraEmpresaBtn
    this.showAdicionarRegrasEmpresaBtn = showAdicionarRegrasEmpresaBtn
  }

  create(context) {
    const texto = this.data.textoResumido
  
    const textoClausulaTitulo = h4({ content: `Resumo da cláusula: ${this.data.nomeClausula}` })
    const textoClausulaOptions = div({ className: "options", content: a({
        href: 'javascript:;',
        id: `collapseClauText-${this.id}`,
        className: 'panel-collapse',
        content: i({ className: 'fa fa-chevron-down' })
      })
    })
    const textoClausulaPainelHeading = div({ className: "panel-heading", content: [textoClausulaTitulo, textoClausulaOptions] })
  
    const copyBtn = button({ className: 'btn btn-primary', content: i({ className: 'fa fa-copy' }) })
    copyBtn.on("click", () => {
      navigator.clipboard.writeText(texto).then(() => {
        NotificationService.success({ title: "Copiado com sucesso!", message: "Texto da cláusula copiado para área de transferência." })
      })
    })
  
    const adicionarRegrasEmmpresaBtn = button({ className: 'btn btn-primary', content: 'Adicionar regra empresa' })
    adicionarRegrasEmmpresaBtn.on('click', () => this.handleClickAdicionarRegraEmpresaBtn({ id: this.id, context }))
  
    const textoClausulaButtons = div({
      style: 'margin-bottom: 1rem; display: flex; gap: 10px;',
      className: 'clausula_text_toolbar',
      content: [copyBtn, this.showAdicionarRegrasEmpresaBtn && adicionarRegrasEmmpresaBtn]
    })
    const textoClausula = p({ style: 'text-align: justify; white-space: pre-line;', content: texto })
    const textoClausulaPainelBody = div({ className: 'panel-body collapse in', id: `collapseClauText-${this.id}-body`, content: [textoClausulaButtons, textoClausula] })

    return div({ className: "panel panel-primary", content: [textoClausulaPainelHeading, textoClausulaPainelBody] })
  }
}