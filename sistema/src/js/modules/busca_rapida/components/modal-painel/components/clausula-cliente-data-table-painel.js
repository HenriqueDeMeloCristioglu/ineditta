import DateFormatter from '../../../../../utils/date/date-formatter'
import { a, div, h4, i, table, tbody, td, th, thead, tr } from '../../../../../utils/components/elements'
import { obterClausulasClientesPorClausulaUsuario } from '../../../features'

export class ClausulaClienteDataTablePainel {
  constructor({ id, grupoEconomicoId, data }) {
    this.data = data
    this.id = id
    this.grupoEconomicoId = grupoEconomicoId
  }

  async create() {
    const result = await obterClausulasClientesPorClausulaUsuario({
      clausulaId: this.id,
      grupoEconomicoId: this.grupoEconomicoId
    })

    if(result.isFailure()) {
      return
    }
    const clausulasCliente = result.value

    const data = this.data

    const clausulaClienteClausulaPainel = div({ className: 'panel panel-primary' })

    const clausulaClienteClausulaPainelHeading = div({ className: 'panel-heading' })
    const clausulaClienteClausulaTitulo = h4({ content: `Regras Parâmetros Empresa: ${data.nomeClausula}` })
    const clausulaClienteClausulaOptions = div({ className: 'options' })
    const clausulaClienteClausulaCloseBtn = a({
      href: 'javascript:',
      id: `collapseClauComm-${data.id}`,
      className: 'panel-collapse',
      content: i({ className: 'fa fa-chevron-down' })
    })

    clausulaClienteClausulaOptions.append(clausulaClienteClausulaCloseBtn)
    clausulaClienteClausulaPainelHeading.append([clausulaClienteClausulaTitulo, clausulaClienteClausulaOptions])

    const clausulaClienteClausulaPainelBody = div({
      className: 'panel-body collapse in',
      id: `collapseClauText-${data.id}-body`
    })

    const clausulaClienteTb = table({ className: 'table table-striped' })
    const clausulaClienteTheadRow = tr({ className: 'table table-striped' })
    
    clausulaClienteTheadRow.append(th({ content: "Usuário" }))
    clausulaClienteTheadRow.append(th({ content: "Texto" }))
    clausulaClienteTheadRow.append(th({ content: "Data Inclusão" }))

    const clausulaClienteThead = thead({ content: clausulaClienteTheadRow })
    const clausulaClienteTbody = tbody({})
  
    clausulasCliente?.map(clausulaCliente => {
      const commRow = tr({})
      
      commRow.append(td({ content: clausulaCliente.nomeUsuario }))
      commRow.append(td({ content: clausulaCliente.texto }))
      commRow.append(td({ content: DateFormatter.dayMonthYear(clausulaCliente.dataInclusao) }))
  
      clausulaClienteTbody.append(commRow)
    })
      
    clausulaClienteTb.append([clausulaClienteThead, clausulaClienteTbody])
    clausulaClienteClausulaPainelBody.append([clausulaClienteTb])
    clausulaClienteClausulaPainel.append([clausulaClienteClausulaPainelHeading, clausulaClienteClausulaPainelBody])

    return clausulaClienteClausulaPainel
  }
}
