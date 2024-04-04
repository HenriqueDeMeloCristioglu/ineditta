import $ from 'jquery'
import { ApiService } from "../../../../core/index"
import { CnaeService, DocSindService, IAClausulaService, IADocumentoSindicalService } from "../../../../services"
import DatepickerrangeWrapper from "../../../../utils/daterangepicker/daterangepicker-wrapper"
import SelectWrapper from "../../../../utils/selects/select-wrapper"
import { cnaePorGrupoEmpresaEstabelecimentoSelect, empresaPorUsuarioSelect, grupoEconomicoPorUsuarioSelect } from "../../../core"
import { limparFiltros } from '../actions/limpar-filtros'

const cnaeService = new CnaeService(new ApiService())
const docSindService = new DocSindService(new ApiService())
const iADocumentoSindicalService = new IADocumentoSindicalService(new ApiService())
const iAClausulaService = new IAClausulaService(new ApiService())

export async function configurarFormularioFiltros(context) {
  const { inputs, dataTables } = context
  const { selects, datePickers } = inputs

  selects.grupoOperacaoFiltroSelect = null
  selects.documentoFiltroSelect = null
  selects.statusDocumentoFiltroSelect = null
  selects.statusClausulasFiltroSelect = null

  selects.grupoEconomicoFiltroSelect = grupoEconomicoPorUsuarioSelect({
    selector: '#grupoEconomicoSelect',
    isIneditta: true,
    onChange: (data) => {
      console.log(data)
      selects.atividadeEconomicaFiltroSelect?.reload()
      selects.grupoOperacaoFiltroSelect?.reload()
      selects.documentoSindicalSelect?.reload()  
    }
  })

  selects.empresaFiltroSelect = empresaPorUsuarioSelect({
    selector: '#empresaSelect',
    isIneditta: true,
    parentId: '#grupoEconomicoSelect',
    onChange: () => {
      selects.atividadeEconomicaFiltroSelect?.reload()
      selects.grupoOperacaoFiltroSelect?.reload()
      selects.documentoSindicalSelect?.reload()
    }
  })
  await selects.empresaFiltroSelect.loadOptions()
  selects.empresaFiltroSelect.enable()

  selects.atividadeEconomicaFiltroSelect = cnaePorGrupoEmpresaEstabelecimentoSelect({
    empresaSelector: '#empresaSelect',
    gruposEconomicoSelector: '#grupoEconomicoSelect',
    onChange: () => {
      selects.documentoSindicalSelect?.reload()
    }
  })

  selects.grupoOperacaoFiltroSelect = new SelectWrapper("#grupoOperacaoSelect", {
    options: { placeholder: 'Selecione' },
    onOpened: async () => {
      const options = {
        gruposEconomicosIds: $('#grupoEconomicoSelect').val() ?? null,
        matrizesIds: $('#empresaSelect').val() ?? null
      }

      return await cnaeService.obterSelectDivisaoPorUsuario(options)
    }
  })

  selects.documentoFiltroSelect = new SelectWrapper("#documentoSelect", {
    options: { placeholder: 'Selecione' },
    onOpened: async () => {
      const options = {
        gruposEconomicosIds: $('#grupoEconomicoSelect').val() ?? null,
        empresasIds: $('#empresaSelect').val() ?? null,
        atividadesEconomicasIds: $('#atividadeEconomicaSelect').val() ?? null
      }

      return (await docSindService.obterSelectPorClientesCnaes(options)).value
    }
  })

  selects.nomeDocumentoFiltroSelect = new SelectWrapper("#nomeDocumentoSelect", {
    options: { placeholder: 'Selecione' },
    onOpened: async () => (await docSindService.obterSelectProcessados()).value
  })

  selects.statusDocumentoFiltroSelect = new SelectWrapper("#statusScrapSelect", {
    options: { placeholder: 'Selecione' },
    onOpened: async () => (await iADocumentoSindicalService.obterSelect()).value
  })
 
  selects.statusClausulasFiltroSelect = new SelectWrapper("#statusClausulasSelect", {
    options: { placeholder: 'Selecione' },
    onOpened: async () => (await iAClausulaService.obterSelect()).value
  })

  datePickers.dataSlaDt = new DatepickerrangeWrapper('#dataSlaDt')

  $("#limparFiltroBtn").on('click', () => limparFiltros(context))
  $("#filtrarBtn").on('click', () => dataTables.documentosTb.reload())
}