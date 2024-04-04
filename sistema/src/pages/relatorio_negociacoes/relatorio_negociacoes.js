// Libs
import jQuery from 'jquery'
import $ from 'jquery'
import 'popper.js'

// Utils
import '../../js/utils/masks/jquery-mask-extensions.js'
import Masker from '../../js/utils/masks/masker.js'

// Temp
import 'datatables.net-bs5/css/dataTables.bootstrap5.css'
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css'
import 'datatables.net-bs5'
import 'datatables.net-responsive-bs5'

// Css libs
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap'

// Core
import { AuthService } from '../../js/core/index.js'

import '../../js/main.js'
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper'
import { Menu } from '../../components/menu/menu.js'
import DateFormatter from '../../js/utils/date/date-formatter.js'

import { UsuarioNivel } from "../../js/application/usuarios/constants/usuario-nivel.js"
import SelectWrapper from "../../js/utils/selects/select-wrapper.js"
import { gerarRelatorioExcel } from '../../js/modules/relatorio-negociacoes/features/gerar-relatorio-excel.js'
import { obterTipoLocalizacao } from '../../js/utils/components/selects/tipo-localidade.js'
import DatepickerrangeWrapper from '../../js/utils/daterangepicker/daterangepicker-wrapper.js'
import { useContentLoading } from '../../js/utils/hooks/use-content-loading.js'
import NotificationService from '../../js/utils/notifications/notification.service.js'
import {
  useAcompanhamentoCctService,
  useCnaeService,
  useEmpresaService,
  useEstabelecimentoService,
  useGrupoEconomicoService,
  useSindicatoLaboralService,
  useSindicatoPatronalService,
  useUsuarioService
} from '../../js/modules'

// Services
const acompanhamentoCctService = useAcompanhamentoCctService()
const grupoEconomicoService = useGrupoEconomicoService()
const matrizService = useEmpresaService()
const clienteUnidadeService = useEstabelecimentoService()
const cnaeService = useCnaeService()
const usuarioAdmService = useUsuarioService()
const sindicatoLaboralService = useSindicatoLaboralService()
const sindicatoPatronalService = useSindicatoPatronalService()

console.log(usuarioAdmService)

let acompanhamentoCcttb = null

let matrizSelect = null
let estabelecimentoSelect = null
let grupoEconomicoSelect = null
let cnaeSelect = null
let sindLaboralSelect = null
let sindPatronalSelect = null
let localizacoesSelect = null
let dataBaseSelect = null
let nomeDocumentoSelect = null
let faseSelect = null
let tipoLocalizacaoSelect = null

let dataProcessamentoDt = null

let usuario = null
let isIneditta = null
let isGrupoEconomico = null
let isEstabelecimento = null

jQuery(async () => {
  await useContentLoading(async () => {
    new Menu()
    
    await AuthService.initialize()
    
    const dadosPessoais = await usuarioAdmService.obterDadosPessoais()
    
    if (dadosPessoais.isFailure()) return

    usuario = dadosPessoais.value
    isIneditta = usuario.nivel == UsuarioNivel.Ineditta
    isGrupoEconomico = usuario.nivel == UsuarioNivel.GrupoEconomico
    isEstabelecimento = usuario.nivel == UsuarioNivel.Estabelecimento
    
    configurarInputs()
    
    await configurarFormulario()
    
    await carregarDatatable()

    await consultarUrl()
  })
})

function configurarInputs() {
  $("#dropdownMenuButton").dropdown()
  $('.filter-column').on('change', function () {
    const index = $(this).data('column-index')
    const isVisible = $(this).is(':checked')

    acompanhamentoCcttb.disableColumn(index, isVisible)
  })

  $("#form").on("submit", e => e.preventDefault())

  $('#btn_exportar_relatorio').on('click', async () =>  {
    let requestData = {}

    const gruposEconomicosIds = grupoEconomicoSelect.getValue()
    if (gruposEconomicosIds) requestData.gruposEconomicosIds = gruposEconomicosIds.map(i => parseInt(i))

    const empresasIds = matrizSelect.getValue()
    if (empresasIds) requestData.empresasIds = empresasIds.map(i => parseInt(i))

    const estabelecimentosIds = estabelecimentoSelect.getValue()
    if (estabelecimentosIds) requestData.estabelecimentosIds = estabelecimentosIds.map(i => parseInt(i))

    const tipoLocalizacao = tipoLocalizacaoSelect.getValue()
    if (tipoLocalizacao) requestData.tipoLocalizacao = tipoLocalizacao

    const localizacoes = localizacoesSelect.getValue()
    if (localizacoes) requestData.localizacoes = localizacoes

    const atividadesEconomicasIds = cnaeSelect.getValue()
    if (atividadesEconomicasIds) requestData.atividadesEconomicasIds = atividadesEconomicasIds.map(i => i.toString())

    const sindicatosLaboraisIds = sindLaboralSelect.getValue()
    if (sindicatosLaboraisIds) requestData.sindicatosLaboraisIds = sindicatosLaboraisIds.map(i => parseInt(i))

    const sindicatosPatronaisIds = sindPatronalSelect.getValue()
    if (sindicatosPatronaisIds) requestData.sindicatosPatronaisIds = sindicatosPatronaisIds.map(i => parseInt(i))

    const datasBases = dataBaseSelect.getValue()
    if (datasBases) requestData.datasBases = datasBases
    
    const nomeDocumento = nomeDocumentoSelect.getValue()
    if (nomeDocumento) requestData.nomeDocumento = nomeDocumento.map(i => parseInt(i))
    
    const fases = faseSelect.getValue()
    if (fases) requestData.fases = fases
    
    const dataProcessamento = dataProcessamentoDt?.hasValue()
    if (dataProcessamento) {
      requestData.dataProcessamentoInicial = dataProcessamentoDt.getBeginDate()
      requestData.dataProcessamentoFinal = dataProcessamentoDt.getEndDate()
    }
    await gerarRelatorioExcel(requestData)
  })
}

async function consultarUrl() {
  const queryString = window.location.search
  const urlParams = new URLSearchParams(queryString)

  if (!urlParams.has('sigla')) return

  const sigla = urlParams.get('sigla')

  $("#acompanhamentoCcttb_filter label input").val(sigla)

  const sindicatoId = urlParams.get("sindId");
  const tipoSindicato = urlParams.get("tipoSind");

  if (sindicatoId && tipoSindicato) {
    if (tipoSindicato == "laboral") {
      sindLaboralSelect?.disable();

      const options = await sindLaboralSelect?.loadOptions();
      const sindicatoSelecionadoOption = options.find(o => o.id == sindicatoId);

      if (sindicatoSelecionadoOption) {
        sindLaboralSelect?.clear();
        sindLaboralSelect?.setCurrentValue(sindicatoSelecionadoOption);
      }
      else {
        NotificationService.error({title: "Sindicato laboral não encontrado entre as opções."});
      }

      sindLaboralSelect?.enable();
    }
    else {
      sindPatronalSelect?.disable();

      const options = await sindPatronalSelect?.loadOptions();
      const sindicatoSelecionadoOption = options.find(o => o.id == sindicatoId);

      if (sindicatoSelecionadoOption) {
        sindPatronalSelect?.clear();
        sindPatronalSelect?.setCurrentValue(sindicatoSelecionadoOption);
      }
      else {
        NotificationService.error({title: "Sindicato patronal não encontrado entre as opções."});
      }

      sindPatronalSelect?.enable();
    }
  }

  acompanhamentoCcttb.reload()
}

async function carregarDatatable() {
  acompanhamentoCcttb = new DataTableWrapper('#acompanhamentoCcttb', {
    columns: [
      { "data": "siglasSindicatosLaborais", title: "Sind. Laboral" },
      { "data": "cnpjsLaborais", title: "CNPJ Laboral", render: (data) => {
        if (!data || !data?.split(', ')) return Masker.CNPJ(data)
        
        const datas = data?.split(', ')
        if (!datas) return Masker.CNPJ(data)
        
        const cnpjs = datas.map(cnpj => Masker.CNPJ(cnpj))

        return cnpjs.join (', ')
      } },
      { "data": "siglasSindicatoPatronais", title: "Sind. Patronal" },
      { "data": "cnpjsPatronais", title: "CNPJ Patronal", render: (data) => {
        if (!data || !data?.split(', ')) return Masker.CNPJ(data)
        
        const datas = data?.split(', ')
        if (!datas) return Masker.CNPJ(data)
        
        const cnpjs = datas.map(cnpj => Masker.CNPJ(cnpj))

        return cnpjs.join (', ')
      } },
      { "data": "atividadeEconomica", title: "Atividade Econômica" },
      { "data": "nomeDocumento", title: "Nome Documento" },
      { "data": "dataBase", title: "Data Base" },
      { "data": "periodoInpc", title: "Período INPC", render: (data) => DateFormatter.dayMonthYear(data) },
      { "data": "inpcReal", title: "INPC (Real)" },
      { "data": "fase", title: "Fase" },
      { "data": "observacoes", title: "Observações" },
      { "data": "dataProcessamento", title: "Data processamento Ineditta", render: (data) => DateFormatter.dayMonthYear(data) 
    }
    ],
    ajax: async (requestData) => {
      const gruposEconomicosIds = grupoEconomicoSelect.getValue()
      if (gruposEconomicosIds) requestData.gruposEconomicosIds = gruposEconomicosIds.map(i => parseInt(i))

      const empresasIds = matrizSelect.getValue()
      if (empresasIds) requestData.empresasIds = empresasIds.map(i => parseInt(i))

      const estabelecimentosIds = estabelecimentoSelect.getValue()
      if (estabelecimentosIds) requestData.estabelecimentosIds = estabelecimentosIds.map(i => parseInt(i))

      const tipoLocalizacao = tipoLocalizacaoSelect.getValue()
      if (tipoLocalizacao) requestData.tipoLocalizacao = tipoLocalizacao

      const localizacoes = localizacoesSelect.getValue()
      if (localizacoes) requestData.localizacoes = localizacoes

      const atividadesEconomicasIds = cnaeSelect.getValue()
      if (atividadesEconomicasIds) requestData.atividadesEconomicasIds = atividadesEconomicasIds.map(i => i.toString())

      const sindicatosLaboraisIds = sindLaboralSelect.getValue()
      if (sindicatosLaboraisIds) requestData.sindicatosLaboraisIds = sindicatosLaboraisIds.map(i => parseInt(i))

      const sindicatosPatronaisIds = sindPatronalSelect.getValue()
      if (sindicatosPatronaisIds) requestData.sindicatosPatronaisIds = sindicatosPatronaisIds.map(i => parseInt(i))

      const datasBases = dataBaseSelect.getValue()
      if (datasBases) requestData.datasBases = datasBases
      
      const nomeDocumento = nomeDocumentoSelect.getValue()
      if (nomeDocumento) requestData.nomeDocumento = nomeDocumento.map(i => parseInt(i))
      
      const fases = faseSelect.getValue()
      if (fases) requestData.fases = fases
      
      const dataProcessamento = dataProcessamentoDt?.hasValue()
      if (dataProcessamento) {
        requestData.dataProcessamentoInicial = dataProcessamentoDt.getBeginDate()
        requestData.dataProcessamentoFinal = dataProcessamentoDt.getEndDate()
      }

      return await acompanhamentoCctService.obterRelatorioDatatable(requestData)
    }
  })

  await acompanhamentoCcttb.initialize()
}

async function configurarFormulario() {
  grupoEconomicoSelect = new SelectWrapper("#grupo", {
    options: { placeholder: "Selecione" },
    onOpened: async () => await grupoEconomicoService.obterSelectPorUsuario(),
    markOptionAsSelectable: isIneditta ? () => false : () => true,
  })
  if (isIneditta) {
    grupoEconomicoSelect.enable()
  } else {
    grupoEconomicoSelect.disable()
    await grupoEconomicoSelect.loadOptions()
  }

  matrizSelect = new SelectWrapper("#matriz", {
    options: {
      placeholder: "Selecione",
    },
    parentId: "#grupo",
    onOpened: async (grupoEconomicoId) => await matrizService.obterSelectPorUsuario(grupoEconomicoId),
    markOptionAsSelectable:
      isIneditta || isGrupoEconomico ? () => false : () => true,
  })

  const optionsEmpresas = await matrizSelect.loadOptions()

  if (isIneditta || isGrupoEconomico) {
    matrizSelect.enable()
  } else {
    if (!(optionsEmpresas instanceof Array && optionsEmpresas.length > 1) || isEstabelecimento) {
      matrizSelect.disable()
    }
    else {
      matrizSelect.config.markOptionAsSelectable = () => false
      matrizSelect.clear()
      matrizSelect.enable()
    }
  }

  estabelecimentoSelect = new SelectWrapper("#unidade", {
    options: {
      placeholder: "Selecione"
    },
    parentId: "#matriz",
    onOpened: async (matrizId) => await clienteUnidadeService.obterSelectPorUsuario(matrizId),
    markOptionAsSelectable: isEstabelecimento ? () => true : () => false,
  })
  if (isEstabelecimento) {
    const options = await estabelecimentoSelect.loadOptions()

    if (!(options instanceof Array && options.length > 1)) {
      estabelecimentoSelect.disable()
    }
    else {
      estabelecimentoSelect.config.markOptionAsSelectable = () => false
      estabelecimentoSelect?.clear()
      estabelecimentoSelect.enable()
    }
  } else {
    estabelecimentoSelect.enable()
  }

  cnaeSelect = new SelectWrapper("#canes", {
    options: { placeholder: "Selecione" },
    onOpened: async () => await cnaeService.obterSelectPorUsuario()
  })

  sindLaboralSelect = new SelectWrapper("#sind_laboral", {
    options: { placeholder: "Selecione" },
    onOpened: async () => await sindicatoLaboralService.obterSelectPorUsuario(),
    sortable: true,
  })

  sindPatronalSelect = new SelectWrapper("#sind_patronal", {
    options: { placeholder: "Selecione" },
    onOpened: async () => await sindicatoPatronalService.obterSelectPorUsuario(),
    sortable: true,
  })

  tipoLocalizacaoSelect = new SelectWrapper("#tipo_localidade", {
    options: { placeholder: "Selecione" },
    onOpened: async () => await obterTipoLocalizacao(),
    onSelected: () => localizacoesSelect.reload(),
    sortable: true
  })

  localizacoesSelect = new SelectWrapper("#localidade", {
    options: { placeholder: "Selecione" },
    onOpened: async () => {
      const params = {}
      
      const gruposEconomicosIds = grupoEconomicoSelect.getValue()
      if (gruposEconomicosIds) params.gruposEconomicosIds = gruposEconomicosIds.map(i => parseInt(i))

      const empresasIds = matrizSelect.getValue()
      if (empresasIds) params.empresasIds = empresasIds.map(i => parseInt(i))

      const estabelecimentosIds = estabelecimentoSelect.getValue()
      if (estabelecimentosIds) params.estabelecimentosIds = estabelecimentosIds.map(i => parseInt(i))

      const atividadesEconomicasIds = cnaeSelect.getValue()
      if (atividadesEconomicasIds) params.atividadesEconomicasIds = atividadesEconomicasIds.map(i => i.toString())

      const sindicatosLaboraisIds = sindLaboralSelect.getValue()
      if (sindicatosLaboraisIds) params.sindicatosLaboraisIds = sindicatosLaboraisIds.map(i => parseInt(i))

      const sindicatosPatronaisIds = sindPatronalSelect.getValue()
      if (sindicatosPatronaisIds) params.sindicatosPatronaisIds = sindicatosPatronaisIds.map(i => parseInt(i))

      const datasBases = dataBaseSelect.getValue()
      if (datasBases) params.datasBases = datasBases
      
      const nomeDocumento = nomeDocumentoSelect.getValue()
      if (nomeDocumento) params.nomeDocumento = nomeDocumento.map(i => parseInt(i))
      
      const fases = faseSelect.getValue()
      if (fases) params.fases = fases
      
      const dataProcessamento = dataProcessamentoDt?.hasValue()
      if (dataProcessamento) {
        params.dataProcessamentoInicial = dataProcessamentoDt.getBeginDate()
        params.dataProcessamentoFinal = dataProcessamentoDt.getEndDate()
      }

      params.tipoLocalidade = tipoLocalizacaoSelect.getValue()

      const result = await acompanhamentoCctService.obterSelectLocalidades(params)

      return result.value
    },
    sortable: true
  })

  dataBaseSelect = new SelectWrapper("#data_base", {
    options: { placeholder: "Selecione" },
    onOpened: async () => (await acompanhamentoCctService.obterSelectDataBase()).value,
    sortable: true
  })

  nomeDocumentoSelect = new SelectWrapper("#nome_documento", {
    options: { placeholder: "Selecione" },
    onOpened: async () => (await acompanhamentoCctService.obterSelectTiposDocs({ porUsuario: true })).value,
    sortable: true
  })

  faseSelect = new SelectWrapper("#fase", {
    options: { placeholder: "Selecione" },
    onOpened: async () => (await acompanhamentoCctService.obterFasesFiltro()).value,
    sortable: true
  })

  dataProcessamentoDt = new DatepickerrangeWrapper('#data_processamento')

  $("#limparFiltroBtn").on('click', limparFiltros)
  $("#filtrarBtn").on('click', () => acompanhamentoCcttb.reload())
}

function limparFiltros() {
  isIneditta && grupoEconomicoSelect.clear()
  !isEstabelecimento && matrizSelect.clear()
  estabelecimentoSelect.clear()
  cnaeSelect.clear()
  sindLaboralSelect.clear()
  sindPatronalSelect.clear()
  localizacoesSelect.clear()
  dataBaseSelect.clear()
  nomeDocumentoSelect.clear()
  faseSelect.clear()
  dataProcessamentoDt.clear()
  tipoLocalizacaoSelect.clear()
  localizacoesSelect.clear()

  acompanhamentoCcttb.reload()
}

