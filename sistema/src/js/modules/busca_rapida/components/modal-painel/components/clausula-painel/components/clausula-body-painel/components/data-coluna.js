import { table, tbody, td, th, thead, tr } from "../../../../../../../../../utils/components/elements"
import DateFormatter from "../../../../../../../../../utils/date/date-formatter"

export class DataColuna {
  constructor(data) {
    this.data = data
  }

  create() {
    const datasTb = table({ className: 'table table-striped table-bordered' })
    
    const datasTheadRow = tr({})
    const datasTheadRowContents = ["Validade Inicial", "Validade Final", "Data Base", "Atividade Econômica"]
    datasTheadRowContents.map(content => datasTheadRow.append(th({ content })))
  
    const datasThead = thead({ content: datasTheadRow })

    const datasTbodyRow = tr({ className: "odd gradeX" })
    datasTbodyRow.append(td({ content: DateFormatter.dayMonthYear(this.data.validadeInicial) }))
    datasTbodyRow.append(td({ content: DateFormatter.dayMonthYear(this.data.validadeFinal) }))
    datasTbodyRow.append(td({ content: this.data.dataBase }))
  
    const cnaeArray = this.data.cnae.map(cnae => cnae.subclasse)
    datasTbodyRow.append(td({ content: cnaeArray.join(' / ') }))
  
    const datasTbody = tbody({ content: datasTbodyRow })
    datasTb.append([datasThead, datasTbody])

    return datasTb
  }
}