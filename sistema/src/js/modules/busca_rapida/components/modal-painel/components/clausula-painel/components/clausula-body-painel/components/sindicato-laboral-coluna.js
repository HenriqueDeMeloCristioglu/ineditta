import { table, tbody, td, th, thead, tr } from "../../../../../../../../../utils/components/elements"

export class SindicatoLaboralColuna {
  constructor(data) {
    this.data = data
  }

  create() {
    const sindLaboralTb = table({ className: 'table table-striped table-bordered' })
    const sindLaboralTr = tr({ content: th({ content: 'Sigla Sindicato Laboral / Denominação / UF' }) })
    const sindLaboralThead = thead({ content: sindLaboralTr })
    const sindLaboralTbody = tbody({})
  
    this.data.sindLaboral?.forEach((sind) => {
      const sindRow = tr({ content: td({ content: `${sind.sigla} / ${sind.denominacao} / ${sind.uf}` }) })
  
      sindLaboralTbody.append(sindRow)
    })

    sindLaboralTb.append([sindLaboralThead, sindLaboralTbody])

    return sindLaboralTb
  }
}