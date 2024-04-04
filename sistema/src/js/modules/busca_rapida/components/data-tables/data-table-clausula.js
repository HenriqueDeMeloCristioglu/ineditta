import moment from "moment"
import DataTableWrapper from "../../../../utils/datatables/datatable-wrapper"
import $ from 'jquery'
import DateFormatter from "../../../../utils/date/date-formatter"
import { a, div } from "../../../../utils/components/elements"
import { handleClickAbrirClausulaPorId } from "../../features"
import { useClausulaService } from "../../../core/hooks"

const clausulaService = useClausulaService()

export async function dataTableClausula(context) {
  const { dataTables, datas, inputs: { selects, datePickers } } = context

  if (dataTables.clausulasTb) {
    return dataTables.clausulasTb.reload()
  }

  $("#selectAllDiv").attr("style", "display: block margin-bottom: 10px")

  dataTables.clausulasTb = new DataTableWrapper("#clausulasTb", {
    searching: false,
    ajax: async (requestData) => {
      if ($("#grupo").val() != null && $("#grupo").val() != "") {
        requestData.grupoEconomicoIds = [$("#grupo").val()]
      }

      requestData.empresaIds = $("#matriz").val()
      requestData.unidadeIds = $("#unidade").val()
      requestData.tipoDocIds = $("#nome_doc").val()
      requestData.cnaeIds = $("#categoria").val()

      if (selects.localizacoesSelect?.getValue()) {
        if (selects.localizacoesSelect.getValue().some(localidade => localidade.indexOf('municipio:') > -1)) {
            const municipios = selects.localizacoesSelect.getValue().filter(localidade => localidade.indexOf('municipio:') > -1).map(municipio => municipio.split(':')[1])
            requestData['municipiosIds'] = Array.isArray(municipios) ? municipios : [municipios]
        }

        if (selects.localizacoesSelect.getValue().some(localidade => localidade.indexOf('uf:') > -1)) {
            const ufs = selects.localizacoesSelect.getValue().filter(localidade => localidade.indexOf('uf:') > -1).map(value => value.split(':')[1])
            requestData['ufs'] = Array.isArray(ufs) ? ufs : [ufs]
        }
      }

      if (datePickers.processamentoDp?.hasValue()) {
        requestData['dataProcessamentoInicial'] = datePickers.processamentoDp.getBeginDate()
        requestData['dataProcessamentoFinal'] = datePickers.processamentoDp.getEndDate()
      }

      requestData.sindLaboralIds = $("#sind_laboral").val()
      requestData.sindPatronalIds = $("#sind_patronal").val()
      requestData.dataBase = $("#data_base").val()
      requestData.grupoClausulaIds = $("#grupo_clausulas").val()
      requestData.estruturaClausulaIds = $("#estrutura_clausula").val()
      $("#grupo").val()

      if (datas.idDoc) {
        requestData.idDoc = datas.idDoc

        datas.idDoc = null
      }

      const database = selects.dataBaseSelect.getValue()
      
      requestData.tipoDataBase = 'data-base'
      if (database == 'vigente' || database == 'ultimo-ano') {
        requestData.tipoDataBase = database
      }

      requestData.resumivel = true
      requestData.dataProcessamentoDocumento = true
      requestData.naoConstaNoDocumento = true

      requestData.SortColumns = [
        {
          key: "id",
          value: "asc",
        },
      ]

      requestData.clausulaGrupoEconomico = true

      $("#selectAllInput").prop("checked", false)

      return await clausulaService.obterDatatable(requestData)
    },
    columns: [
      {
        title: "",
        orderable: false,
        data: "id",
      },
      {
        title: "Grupo da Cláusula",
        data: "grupoClausula",
      },
      { title: "Nome", data: "nomeClausula" },
      { title: "Documento", data: "nomeDocumento" },
      {
        title: "Sindicato Laboral",
        data: "sindLaboral",
      },
      {
        title: "Sindicato Patronal",
        data: "sindPatronal",
      },
      { title: "Processamento Ineditta", data: "dataProcessamentoDocumento", render: data => DateFormatter.dayMonthYear(data) },
      {
        title: "Resumo da Cláusula",
        data: "textoResumido",
      },
      { title: "Data-base", data: "dataBase" },
      { title: "Validade Final", data: "validadeFinal" },
    ],
    rowCallback: function (row, data) {
      const id = data?.id
      
      const checkbox = $("<input>")
        .attr("type", "checkbox")
        .attr("data-id", id)
        .attr("checked", false)
        .addClass("clausula_checkbox")

      if (datas.clausulasSelecionadas.includes(id)) {
        checkbox.prop("checked", true)
        checkbox.trigger("change")
      }

      checkbox.on("change", (event) => {
        if (event.currentTarget.checked) {
          datas.clausulasSelecionadas.includes(id) ? null : datas.clausulasSelecionadas.push(id)
        } else {
          datas.clausulasSelecionadas = datas.clausulasSelecionadas.filter((clau) => clau != id)
        }
      })
      $("td:eq(0)", row).html(checkbox)

      const htmlPatronal = $("<span>")
      data?.sindPatronal?.map((sind, index) => {
        let linkPatronal = a({ href: '#', content: sind.sigla }).attr("data-id", sind.id)

        linkPatronal.on("click", function () {
          const id = $(this).attr("data-id")

          $("#sind-id-input").val(id)
          $("#tipo-sind-input").val("patronal")
          $("#openInfoSindModalBtn").trigger("click")
        })

        if (index > 0) {
          htmlPatronal.append(" / ")
        }

        htmlPatronal.append(linkPatronal)
      })
      $("td:eq(5)", row).html(htmlPatronal)

      const htmlLaboral = $("<span>");
      data?.sindLaboral?.map((sind, index) => {
        let linkLaboral = a({ href: '#', content: sind.sigla }).attr("data-id", sind.id)

        linkLaboral.on("click", function () {
          const id = $(this).attr("data-id")

          $("#sind-id-input").val(id)
          $("#tipo-sind-input").val("laboral")
          $("#openInfoSindModalBtn").trigger("click")
        })
        if (index > 0) {
          htmlLaboral.append(" / ")
        }
        htmlLaboral.append(linkLaboral)
      })
      $("td:eq(4)", row).html(htmlLaboral)

      const validade = renderizarValidade(DateFormatter.dayMonthYear(data.validadeFinal))
      $("td:eq(9)", row).html(validade)

      const outerHtmlTexto = renderizarTextoClausula(data.textoResumido)
      const htmlTexto = div({ style: 'display: flex;', content: `${outerHtmlTexto}` })

      htmlTexto.on("click", () => handleClickAbrirClausulaPorId({ context, id: data.id }))

      htmlTexto.on("hover", () => this.style("color: black; text-decoration: underline;"))
      $("td:eq(7)", row).html(htmlTexto)
    },
    columnDefs: [
      {
        targets: "_all",
        defaultContent: "",
      },
    ],
  })

  await dataTables.clausulasTb.initialize()    

  function renderizarValidade(date) {
    const today = moment(new Date()).startOf("day")
    const isExpired = moment(date, "DD/MM/YYYY").isBefore(today)

    if (isExpired) {
      return $("<span>").text(date).attr("style", "color: red")
    }

    return $("<span>").text(date).attr("style", "color: green")
  }

  function renderizarTextoClausula(data) {
    if (!data) {
      return null
    }

    const html = $("<div>")
  
    html.on("hover", () => this.style("color: black"))
    html.attr("style", "cursor: pointer; color: black;")
    html.addClass("texto_clausula")
    html.append(data)
    html.append(" ")
    html.append($("<i>").addClass("fa fa-external-link"))
  
    return html.prop("outerHTML")
  }
}