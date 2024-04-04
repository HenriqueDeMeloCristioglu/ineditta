import { table, tbody, td, th, thead, tr } from "../../../../../../../../../utils/components/elements"
import DateFormatter from "../../../../../../../../../utils/date/date-formatter"

export class ClausulaColuna {
  constructor(data) {
    this.data = data
  }

  create() {
    const clausulaColuna = table({ className: 'table table-striped table-bordered' })

    const clauTheadRow = tr({})    
    const clauTheadRowContents = ["Grupo da Cláusula", "Nome da Cláusula", "Data assinatura/registro", "Documento", "Data Processamento"]
    clauTheadRowContents.map(content => clauTheadRow.append(th({ content })))
    
    const clauThead = thead({ content: clauTheadRow })
    
    const clauTbodyRow = tr({ className: 'odd gradeX' })
    const clauTbodyRowContents = [this.data.grupoClausula, this.data.nomeClausula, DateFormatter.dayMonthYear(this.data.dataAssinaturaDocumento), this.data.nomeDocumento, DateFormatter.dayMonthYear(this.data.dataAprovacaoClausula)]
    clauTbodyRowContents.map(content =>  clauTbodyRow.append(td({ content })))

    const clauTbody = tbody({ content: clauTbodyRow })

    clausulaColuna.append([clauThead, clauTbody])

    return clausulaColuna
  }
}