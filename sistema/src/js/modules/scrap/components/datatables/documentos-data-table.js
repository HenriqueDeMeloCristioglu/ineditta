import $ from 'jquery'
import DataTableWrapper from "../../../../utils/datatables/datatable-wrapper"
import DateFormatter from '../../../../utils/date/date-formatter'
import { a, i } from '../../../../utils/components/elements'
import { IADocumentoSindicalService } from '../../../../services'
import { ApiService } from '../../../../core/index'

const iADocumentoSindicalService = new IADocumentoSindicalService(new ApiService())

export async function documentosDataTable(context) {
  const { datas, dataTables, inputs } = context
  const { selects, datePickers } = inputs

  dataTables.documentosTb = new DataTableWrapper('#documentosTb', {
    ajax: async (requestData) => {
      const {
        grupoEconomicoFiltroSelect,
        empresaFiltroSelect,
        grupoOperacaoFiltroSelect,
        atividadeEconomicaFiltroSelect,
        documentoFiltroSelect,
        nomeDocumentoFiltroSelect,
        statusDocumentoFiltroSelect,
        statusClausulasFiltroSelect
      } = selects

      const { dataSlaDt } = datePickers

      let gruposEconomicosIds = null
      if (grupoEconomicoFiltroSelect) {
        const valor = grupoEconomicoFiltroSelect.getValues()
        console.log(valor)
        gruposEconomicosIds = valor.map(g => parseInt(g))
      }

      let empresasIds = null
      if (empresaFiltroSelect) {
        const valor = empresaFiltroSelect.getValues()
        empresasIds = valor.map(e => parseInt(e))
      }

      let atividadesEconomicasIds = null
      if (atividadeEconomicaFiltroSelect) {
        const valor = atividadeEconomicaFiltroSelect.getValues()
        atividadesEconomicasIds = valor.map(a => parseInt(a))
      }

      let grupoOperacaoId = null
      if (grupoOperacaoFiltroSelect) {
        const valor = grupoOperacaoFiltroSelect.getValue()

        if (valor) {
          grupoOperacaoId = parseInt(valor)
        }
      }

      let documentoId = null
      if (documentoFiltroSelect) {
        const valor = documentoFiltroSelect.getValue()

        if (valor) {
          documentoId = parseInt(valor)
        }
      }

      let nomeDocumentoId = null
      if (nomeDocumentoFiltroSelect) {
        const valor = nomeDocumentoFiltroSelect.getValue()

        if (valor) {
          nomeDocumentoId = parseInt(valor)
        }
      }

      let statusDocumentoId = null
      if (statusDocumentoFiltroSelect) {
        const valor = statusDocumentoFiltroSelect.getValue()

        if (valor) {
          statusDocumentoId = parseInt(valor)
        }
      }

      let statusClausulasId = null
      if (statusClausulasFiltroSelect) {
        const valor = statusClausulasFiltroSelect.getValue()
        
        if (valor) {
          statusClausulasId = parseInt(valor)
        }
      }

      let dataSlaInicio = null
      let dataSlaFim = null
      const dataSla = dataSlaDt?.hasValue()
      if (dataSla) {
        dataSlaInicio = dataSlaDt.getBeginDate()
        dataSlaFim = dataSlaDt.getEndDate()
      }

      const filtros = {
        gruposEconomicosIds,
        empresasIds,
        atividadesEconomicasIds,
        grupoOperacaoId,
        documentoId,
        nomeDocumentoId,
        statusDocumentoId,
        statusClausulasId,
        dataSlaInicio,
        dataSlaFim
      }

      const params = {
        ...filtros,
        ...requestData
      }

      return await iADocumentoSindicalService.obterDatatable(params)
    },
    columns: [
      { "data": 'id', title: '' },
      { "data": 'documentoReferenciaId', title: 'ID Documento' },
      { "data": 'nome', title: 'Nome Documento' },
      { "data": "status", title: 'Status Scrap' },
      { "data": "quantidadeClausulas", title: 'Quantidade de Cláusulas' },
      { "data": "versao", title: 'Versão o Documento' },
      { "data": "dataSla", title: 'Data Sla', render: data => DateFormatter.dayMonthYear(data) },
      { "data": "dataAprovacao", title: 'Data Aprovação', render: data => DateFormatter.dayMonthYear(data) },
      { "data": "statusGeral", title: 'Status Cláusulas' },
      { "data": "assunto", title: 'Assunto' },
      { "data": "usuarioAprovador", title: 'Usuario Aprovador' }
    ],
    rowCallback: function (row, data) {
      const id = data?.id
      const status = data?.status;
      const motivoErro = data?.motivoErro;

      if (status == "Erro") {
        let statusHtml = `Erro <span class="tooltip-custom" data-toggle="tooltip" data-placement="top" title="${motivoErro}"><i class="fa fa-exclamation-circle text-danger"></i></span>`;

        $("td:eq(3)", row).html(statusHtml);

        $('[data-toggle="tooltip"]').tooltip();
      }
      
      let btn = a({ className: 'btn-update', content: i({ className: 'fa fa-file-text' }) }).attr("data-id", id)

      btn.on("click", () => {
        datas.documentoId = id
        datas.documentoReferenciaId = data?.documentoReferenciaId

        if (status == "Erro") {
          $("#reprocessar_documento_btn").show()
        } else {
          $("#reprocessar_documento_btn").hide()
        }

        $('#listaClausulasModalBtn').trigger('click')
      })

      $("td:eq(0)", row).html(btn)
    },
    columnDefs: [{
      targets: "_all",
      defaultContent: ""
    }]
  })

  await dataTables.documentosTb.initialize()
}