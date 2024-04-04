import { table, tbody, td, th, thead, tr } from "../../../../../../../../../utils/components/elements"

export class SindicatoPatronalColuna {
  constructor(data) {
    this.data = data
  }

  create() {
    let sindPatronalColuna = null

    if (this.data.sindPatronal?.length > 0) {
      sindPatronalColuna = table({ className: 'table table-striped table-bordered' })
  
      const sindPatronalTr = tr({ content: th({ content: 'Sigla Sindicato Patronal / Denominação / UF' }) })
      const sindPatronalThead = thead({ content: sindPatronalTr })
      const sindPatronalTbody = tbody({})
  
      this.data.sindPatronal?.forEach((sind) => {
        const sindRow = tr({ content: td({ content: `${sind.sigla} / ${sind.denominacao} / ${sind.uf}` }) })
  
        sindPatronalTbody.append(sindRow)
      })
  
      sindPatronalColuna.append([sindPatronalThead, sindPatronalTbody])
    }
    
    return sindPatronalColuna
  }
}