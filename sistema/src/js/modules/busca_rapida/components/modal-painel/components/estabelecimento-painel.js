import $ from 'jquery'
import { div, table } from "../../../../../utils/components/elements"

export class EstabelecimentoPainel {
  constructor({ id, unidades, grupoEconomicoId }) {
    this.id = id
    this.unidades = unidades
    this.grupoEconomicoId = grupoEconomicoId
  }

  create() {
    const estabelecimentosTb = table({
      style: 'width: 100%',
      className: 'table table-striped table-bordered demo-tbl',
      id: `estabelecimentostb-${this.id}`
    })
    .attr("cellpadding", "0")
    .attr("cellspacing", "0")
    .attr("border", "0")

    const unidadesColumn = div({ className: "col-lg-4 unidades_column" })
    unidadesColumn.append([estabelecimentosTb])

    return unidadesColumn
  }

  init() {
    let unidadesData = this.unidades
    if (!isNaN(this.grupoEconomicoId) && this.grupoEconomicoId > 0) {
      unidadesData = this.unidades.filter((uni) => this.grupoEconomicoId ? uni.g == this.grupoEconomicoId : this.unidades)
    }

    $(`#estabelecimentostb-${this.id}`).DataTable({
      data: unidadesData,
      scrollY: 320,
      columns: [
        {
          title: "Estabelecimentos Abrangidos Pela Cláusula",
          data: "nomeUnidade"
        }
      ],
      searching: false,
      paging: false,
      info: false
    })
  }
}