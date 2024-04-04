// Libs
import jQuery from 'jquery'
import $ from 'jquery'
import 'bootstrap'

import 'datatables.net-bs5/css/dataTables.bootstrap5.css'
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css'
import 'bootstrap/dist/css/bootstrap.min.css'

// Utils
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js'

// Core
import { AuthService, ApiService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js'

// Services
import {
  DocSindService,
  ClienteUnidadeService,
  UsuarioAdmService,
  GrupoClausulaService,
  InformacaoAdicionalClienteService,
  GrupoEconomicoService,
  ClausulaService,
  TipoDocService,
  CnaeService,
  ClausulaGeralService,
  EstruturaClausulaService,
  LocalizacaoService,
  MatrizService,
  SindicatoService,
  SindicatoPatronalService,
  SindicatoLaboralService
} from '../../js/services'

import { UsuarioNivel } from '../../js/application/usuarios/constants/usuario-nivel.js'
import { stringI } from '../../js/utils/components/string-elements/string-i.js'
import { renderizarModal } from '../../js/utils/modals/modal-wrapper.js'
import { td, th, h2, thead, tr, tbody, table, div, i, a, textarea } from '../../js/utils/components/elements'
import { JOpcaoAdicional } from '../../js/utils/components/informacao-adicional/j-search-option-adicional.js'
import { InformacaoAdicionalTipo } from '../../js/application/clausulas/constants/informacao-adicional-tipo.js'
import DateParser from '../../js/utils/date/date-parser.js'
import { Generator } from '../../js/utils/generator/index.js'
import Result from '../../js/core/result.js'
import { TipoObservacaoAdicional } from '../../js/application/informacoesAdicionais/cliente/constants/tipo-obeservacao-adicional.js'


import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

import SelectWrapper from '../../js/utils/selects/select-wrapper.js'
import NotificationService from '../../js/utils/notifications/notification.service.js'
import DatepickerrangeWrapper from '../../js/utils/daterangepicker/daterangepicker-wrapper.js'
import DateFormatter from '../../js/utils/date/date-formatter.js'
import PageWrapper from '../../js/utils/pages/page-wrapper.js'
import { MediaType } from '../../js/utils/web/media-type.js'

const apiService = new ApiService()
const apiLegadoService = new ApiLegadoService()

const documentoSindicalService = new DocSindService(apiService)
const localizacaoService = new LocalizacaoService(apiService)
const cnaeService = new CnaeService(apiService)
const grupoEconomicoService = new GrupoEconomicoService(apiService)
const matrizService = new MatrizService(apiService)
const clienteUnidadeService = new ClienteUnidadeService(apiService)
const sindicatoPatronalService = new SindicatoPatronalService(apiService, apiLegadoService)
const sindicatoLaboralService = new SindicatoLaboralService(apiService, apiLegadoService)
const usuarioAdmSerivce = new UsuarioAdmService(apiService)
const clausulaGeralService = new ClausulaGeralService(apiService)
const informacaoAdicionalClienteService = new InformacaoAdicionalClienteService(apiService)
const sindicatoService = new SindicatoService(apiService)
const grupoClausulaService = new GrupoClausulaService(apiService)
const estruturaClausulaService = new EstruturaClausulaService(apiService)
const tipoDocService = new TipoDocService(apiService)
const clausulaService = new ClausulaService(apiService, apiLegadoService)

let diretoriaInfoSindTb = null
let documentoTb = null
let filtros = {}

let processamentoDp = null
let localidadeSelect = null
let atividadeEconomicaSelect = null
let grupoEconomicoSelect = null
let matrizSelect = null
let estabelecimentoSelect = null
let sindicatoPatronalSelect = null
let sindicatoLaboralSelect = null
let dataBaseSelect = null
let nomeDocumentoSelect = null
let grupoClausulaSelect = null
let estruturaClausulaSelect = null
let clausulaSelect = null

let documentoId = null
let sindicatoId = null
let tipoSind = null
let informacoesAdicionais = null
let informacoesAdicionaisCliente = null
let dadosPessoais = null

jQuery(async () => {
  new Menu()

  $('#exibirDocumentosDiv').hide()

  await AuthService.initialize()

  const result = await usuarioAdmSerivce.obterDadosPessoais()

  if (result.isFailure()) return

  dadosPessoais = result.value

  configurarModal()

  await configurarFormulario()

  await consultarUrl()

  $('.form-horizontal').on('submit', (e) => e.preventDefault())

  $('.horizontal-nav').removeClass('hide')
})

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn')

  const documentoModalHidden = document.getElementById('documentoModalHidden')
  const documentoModalContent = document.getElementById('documentoModalContent')

  const modalInfo = document.getElementById("infoSindModalHidden")
  const contentInfo = document.getElementById("infoSindModalHiddenContent")

  const modalsConfig = [
    {
      id: 'documentoModal',
      modal_hidden: documentoModalHidden,
      content: documentoModalContent,
      btnsConfigs: [
        {
          id: 'salvar_btn',
          onClick: async () => {
            informacoesAdicionaisCliente ? await atualizar() : await incluir()

            await obterInformacoesAdicionaisExistentesPorDocumento()
            montarCabecalho()
          }
        },
        {
          id: 'aprovar_btn',
          onClick: async () => {
            await aprovar()

            await obterInformacoesAdicionaisExistentesPorDocumento()
            montarCabecalho()
          }
        }
      ],
      onOpen: async () => {
        await carregarInformacoesModal()
      },
      onClose: () => {
        informacoesAdicionaisCliente = null
        informacoesAdicionais = null
      },
      styles: {
        container: {
          paddingRight: '30px',
          paddingLeft: '30px'
        },
        modal: {
          maxWidth: '1800px',
          width: '100%'
        }
      }
    },
    {
      id: "infoSindModal",
      modal_hidden: modalInfo,
      content: contentInfo,
      btnsConfigs: [],
      onOpen: async () => {
        await obterInfoSindicatoPorId(sindicatoId, tipoSind)
        await carregarDataTableInfoDiretoriaTb(sindicatoId, tipoSind)
      },
      onClose: () => {
        limparModalInfo()
      },
    }
  ]

  renderizarModal(pageCtn, modalsConfig)
}

async function configurarFormulario() {
  const isIneditta = dadosPessoais.nivel == UsuarioNivel.Ineditta
  const isGrupoEconomico = dadosPessoais.nivel == UsuarioNivel.GrupoEconomico
  const isEstabelecimento = dadosPessoais.nivel == UsuarioNivel.Estabelecimento

  const markOptionAsSelectable = dadosPessoais.nivel == 'Cliente' ? () => true : () => false

  localidadeSelect = new SelectWrapper('#localidade', {
    options: { placeholder: 'Selecione', multiple: true },
    onChange: async () => {
      sindicatoLaboralSelect && await sindicatoLaboralSelect.reload()
      sindicatoPatronalSelect && await sindicatoPatronalSelect.reload()
    },
    onOpened: async () => await obterLocalidades(),
    markOptionAsSelectable: markOptionAsSelectable
  })

  atividadeEconomicaSelect = new SelectWrapper('#categoria', {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: async () => {
      const options = {
        gruposEconomicosIds: $("#grupo").val() ?? null,
        matrizesIds: $("#matriz").val() ?? null,
        clientesUnidadesIds: $("#unidade").val() ?? null
      }
      return await cnaeService.obterSelectPorUsuario(options)
    },
    onChange: async () => {
      sindicatoLaboralSelect && await sindicatoLaboralSelect.reload()
      sindicatoPatronalSelect && await sindicatoPatronalSelect.reload()
    },
    markOptionAsSelectable: markOptionAsSelectable
  })

  grupoEconomicoSelect = new SelectWrapper('#grupo', {
    options: { placeholder: 'Selecione', multiple: true },
    onChange: async () => {
      atividadeEconomicaSelect && await atividadeEconomicaSelect.reload()
      sindicatoLaboralSelect && await sindicatoLaboralSelect.reload()
      sindicatoPatronalSelect && await sindicatoPatronalSelect.reload()
    },
    onOpened: async () => await grupoEconomicoService.obterSelectPorUsuario(),
    markOptionAsSelectable: isIneditta ? () => false : () => true
  })
  if (isIneditta) {
    grupoEconomicoSelect.enable()
  } else {
    grupoEconomicoSelect.disable()
    await grupoEconomicoSelect.loadOptions()
  }

  matrizSelect = new SelectWrapper('#matriz', {
    options: {
      placeholder: 'Selecione',
      multiple: true
    }, parentId: '#grupo',
    onChange: async () => {
      await atividadeEconomicaSelect.reload()
      await sindicatoLaboralSelect.reload()
      await sindicatoPatronalSelect.reload()
    },
    onOpened: async (grupoEconomicoId) => await matrizService.obterSelectPorUsuario(grupoEconomicoId),
    markOptionAsSelectable: isIneditta || isGrupoEconomico ? () => false : () => true
  })
  if (isIneditta || isGrupoEconomico) {
    matrizSelect.enable()
  } else {
    const options = await matrizSelect.loadOptions()
    if (!(options instanceof Array && options.length > 1) || isEstabelecimento) {
      matrizSelect.disable()
    }
    else {
      matrizSelect.config.markOptionAsSelectable = () => false;
      matrizSelect.clear();
      matrizSelect.enable();
    }
  }

  estabelecimentoSelect = new SelectWrapper('#unidade',
    {
      options: {
        placeholder: 'Selecione', multiple: true
      },
      parentId: '#matriz',
      onChange: async () => {
        await atividadeEconomicaSelect.reload()
        await sindicatoLaboralSelect.reload()
        await sindicatoPatronalSelect.reload()
      },
      onOpened: async (matrizId) => await clienteUnidadeService.obterSelectPorUsuario(matrizId),
      markOptionAsSelectable: isEstabelecimento ? () => true : () => false
    })
  if (isEstabelecimento) {
    const options = await estabelecimentoSelect.loadOptions()
    if (!(options instanceof Array && options.length > 1)) {
      estabelecimentoSelect.disable()
    }
    else {
      estabelecimentoSelect.config.markOptionAsSelectable = () => false;
      estabelecimentoSelect?.clear();
      estabelecimentoSelect.enable();
    }
  } else {
    estabelecimentoSelect.enable()
  }

  sindicatoPatronalSelect = new SelectWrapper('#sindicatoPatronal', {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: async () => {
      const options = obterParametrosParaRequisicaoDeSindicatos()
      return await sindicatoPatronalService.obterSelectPorUsuario(options)
    },
    onSelected: () => {
      dataBaseSelect.reload()
    },
    markOptionAsSelectable: markOptionAsSelectable
  })

  sindicatoLaboralSelect = new SelectWrapper('#sindicatoLaboral', {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: async () => {
      const options = obterParametrosParaRequisicaoDeSindicatos()
      return await sindicatoLaboralService.obterSelectPorUsuario(options)
    },
    onSelected: () => {
      dataBaseSelect.reload()
    },
    markOptionAsSelectable: markOptionAsSelectable
  });
  dataBaseSelect = new SelectWrapper("#dataBase", {
    onOpened: async () => {
      const requestData = {
        sindLaboralIds: sindicatoLaboralSelect.getValue(),
        sindPatronalIds: sindicatoPatronalSelect.getValue(),
      }

      const options = await clausulaService.obterDatasBases(requestData)
      options.shift()

      return options
    },
  });
  nomeDocumentoSelect = new SelectWrapper("#nome_doc", {
    options: { placeholder: "Selecione", multiple: true },
    onOpened: async () => {
      return await tipoDocService.obterSelectPorTipos({ processado: true });
    }
  });

  grupoClausulaSelect = new SelectWrapper("#grupo_clausulas", {
    onOpened: async () => (await grupoClausulaService.obterSelect()).value,
    sortable: true,
  });

  estruturaClausulaSelect = new SelectWrapper("#estrutura_clausula", {
    onOpened: async () => (await estruturaClausulaService.obterSelect()).value,
    sortable: true
  });

  clausulaSelect = new SelectWrapper("#clausulaList", {
    onOpened: async () => (await estruturaClausulaService.obterSelect()).value,
  });

  processamentoDp = new DatepickerrangeWrapper('#processamento');

  $('#limparBtn').on('click', async () => await limpar())
  $('#filtrarBtn').on('click', async () => await filtrar())

  $('#baixar_exel_btn').on('click', async () => await downloadExcel())
}

function obterParametrosParaRequisicaoDeSindicatos() {
  const campoLocalidade = $("#localidade").val();
  const localizacoesIds = campoLocalidade.map(loc => {
    if (loc.includes('municipio')) {
      return Number(loc.split(':')[1]);
    }
    return null;
  }).filter(x => x !== null);

  const ufs = campoLocalidade.map(loc => {
    if (loc.includes('uf')) {
      return loc.split(':')[1];
    }
    return null;
  }).filter(x => x !== null);

  const options = {
    gruposEconomicosIds: $("#grupo").val() ?? null,
    matrizesIds: $("#matriz").val() ?? null,
    clientesUnidadesIds: $("#unidade").val() ?? null,
    cnaesIds: $("#categoria").val() ?? null,
    localizacoesIds,
    ufs
  }

  return options;
}

async function obterLocalidades() {
  const municipios = await localizacaoService.obterSelectPorUsuario();
  const regioes = await localizacaoService.obterSelectRegioes(true);

  const localidades = municipios?.map((municipio) => ({ id: `municipio:${municipio.id}`, description: municipio.description })) ?? [];

  if (regioes?.length > 0) {
    localidades.push(...regioes.map(regiao => ({ id: `uf:${regiao.description}`, description: regiao.description })));
  }

  return localidades;
}

async function carregarDatatable() {
  if (documentoTb) return await documentoTb.reload()

  documentoTb = new DataTableWrapper('#documentoTb', {
    columns: [
      { data: 'id' },
      { data: 'nome' },
      {
        data: 'atividadesEconomicas', render: data => {
          if (!data) {
            return '';
          }

          let dado = []
          data.map(({ subclasse }) => dado.push(subclasse))

          return dado.join(', ')
        }
      },
      { data: 'siglasSindicatosLaborais' },
      { data: 'siglasSindicatosPatronais' },
      { data: 'dataLiberacao', render: (data) => DataTableWrapper.formatDate(data) },
      { data: 'dataValidadeInicial' },
      { data: 'id' },
      { data: 'id' },
    ],
    ajax: async (params) => {
      params.shortTemplate = true
      params.liberados = true
      params.informacoesAdicionais = true
      params = { ...params, ...filtros };
      return await documentoSindicalService.obterDatatableConsulta(params);
    },
    columnDefs: [
      {
        "targets": "_all",
        "defaultContent": ""
      }
    ],
    rowCallback: function (row, data) {
      const id = data?.id

      let btn = a({
        className: 'btn-update'
      })
        .attr("data-id", id)
        .html(i({
          className: 'fa fa-file-text'
        }))

      btn.on("click", function () {
        documentoId = id
        $('#documentoModalBtn').trigger('click')
      })

      $("td:eq(0)", row).html(btn)

      if (data?.sindicatosPatronais) {
        const sindicatosPatronais = data?.sindicatosPatronais
        let dindicatoTd = div({})

        sindicatosPatronais.map(sindicatoPatronal => {
          const link = $("<a>")
            .attr("data-id", sindicatoPatronal.id)
            .attr("href", "#")
            .html(sindicatoPatronal.sigla)

          link.on("click", function () {
            sindicatoId = sindicatoPatronal.id
            tipoSind = 'patronal'

            $("#openInfoSindModalBtn").trigger("click");
          })

          dindicatoTd.append(link)
        })

        $("td:eq(4)", row).html(dindicatoTd);
      }

      if (data?.sindicatosLaborais) {
        const sindicatosLaborais = data?.sindicatosLaborais
        let dindicatoTd = div({})

        sindicatosLaborais.map(sindicatoLaboral => {
          const link = $("<a>")
            .attr("data-id", sindicatoLaboral.id)
            .attr("href", "#")
            .html(sindicatoLaboral.sigla)

          link.on("click", function () {
            sindicatoId = sindicatoLaboral.id
            tipoSind = 'laboral'

            $("#openInfoSindModalBtn").trigger("click");
          })

          dindicatoTd.append(link)
        })

        $("td:eq(3)", row).html(dindicatoTd);
      }

      if (data?.dataValidadeInicial) {
        let vigencia = DataTableWrapper.formatDate(data?.dataValidadeInicial);

        vigencia += data?.dataValidadeFinal ? ` até ${DataTableWrapper.formatDate(data?.dataValidadeFinal)}` : ' até (não informado)';

        $("td:eq(6)", row).html(vigencia);
      }

      if (data?.arquivo) {
        const linkVer = div({
          content: stringI({ className: 'fa fa-file-text' })
        }).on('click', () => verDocumento(data?.id))

        $("td:eq(7)", row).html(linkVer);

        const linkBaixar = div({
          content: stringI({ className: 'fa fa-download' })
        }).on('click', () => downloadDoc(data?.id))

        $("td:eq(8)", row).html(linkBaixar);
      }
    },
  });

  await documentoTb.initialize();
}

function limparFormulario() {
  localidadeSelect?.clear();
  atividadeEconomicaSelect?.clear();
  dadosPessoais.nivel == UsuarioNivel.Ineditta && grupoEconomicoSelect?.clear();
  matrizSelect?.isEnable() && matrizSelect?.clear();
  estabelecimentoSelect?.isEnable() && estabelecimentoSelect?.clear();
  sindicatoPatronalSelect?.clear();
  sindicatoLaboralSelect?.clear();
  dataBaseSelect?.clear();
  nomeDocumentoSelect?.clear();
  grupoClausulaSelect.clear();
  clausulaSelect.clear();
  estruturaClausulaSelect.clear();
  processamentoDp.clear();

  filtros = {};
}

async function limpar() {
  limparFormulario();
  $('#exibirDocumentosDiv').hide();
}

async function filtrar() {
  filtros = {};

  if (localidadeSelect?.getValue()) {

    if (localidadeSelect.getValue().some(localidade => localidade.indexOf('municipio:') > -1)) {
      const municipios = localidadeSelect.getValue().filter(localidade => localidade.indexOf('municipio:') > -1).map(municipio => municipio.split(':')[1]);
      filtros['municipiosIds'] = Array.isArray(municipios) ? municipios : [municipios];
    }

    if (localidadeSelect.getValue().some(localidade => localidade.indexOf('uf:') > -1)) {
      const ufs = localidadeSelect.getValue().filter(localidade => localidade.indexOf('uf:') > -1).map(value => value.split(':')[1]);
      filtros['ufs'] = Array.isArray(ufs) ? ufs : [ufs];
    }
  }

  if (estabelecimentoSelect?.getValue()) {
    filtros['unidadesIds'] = Array.isArray(estabelecimentoSelect?.getValue()) ? estabelecimentoSelect?.getValue() : [estabelecimentoSelect?.getValue()];
  }

  if (matrizSelect?.getValue()) {
    filtros['matrizesIds'] = Array.isArray(matrizSelect?.getValue()) ? matrizSelect?.getValue() : [matrizSelect?.getValue()];
  }

  if (grupoEconomicoSelect?.getValue()) {
    filtros['gruposEconomicosIds'] = Array.isArray(grupoEconomicoSelect?.getValue()) ? grupoEconomicoSelect?.getValue() : [grupoEconomicoSelect?.getValue()];
  }

  if (nomeDocumentoSelect?.getValue()) {
    filtros['nomesDocumentos'] = Array.isArray(nomeDocumentoSelect?.getValue()) ? nomeDocumentoSelect?.getValue() : [nomeDocumentoSelect?.getValue()];
  }

  if (grupoClausulaSelect?.getValue()) {
    filtros['grupoClausulaIds'] = Array.isArray(grupoClausulaSelect?.getValue()) ? grupoClausulaSelect?.getValue() : [grupoClausulaSelect?.getValue()];
    filtros['grupoClausulaIds'] = filtros['grupoClausulaIds'].map(i => parseInt(i))
  }

  if (clausulaSelect?.getValue()) {
    filtros['estruturaClausulasIds'] = clausulaSelect?.getValues();
  }

  if (atividadeEconomicaSelect?.getValue()) {
    filtros['cnaesIds'] = Array.isArray(atividadeEconomicaSelect?.getValue()) ? atividadeEconomicaSelect?.getValue() : [atividadeEconomicaSelect?.getValue()];
  }

  if (sindicatoPatronalSelect?.getValue()) {
    filtros['sindicatosPatronaisIds'] = Array.isArray(sindicatoPatronalSelect?.getValue()) ? sindicatoPatronalSelect?.getValue() : [sindicatoPatronalSelect?.getValue()];
  }

  if (sindicatoLaboralSelect?.getValue()) {
    filtros['sindicatosLaboraisIds'] = Array.isArray(sindicatoLaboralSelect?.getValue()) ? sindicatoLaboralSelect?.getValue() : [sindicatoLaboralSelect?.getValue()];
  }

  if (dataBaseSelect?.getValue()) {
    filtros['datasBases'] = Array.isArray(dataBaseSelect?.getValue()) ? dataBaseSelect?.getValue() : [dataBaseSelect?.getValue()];
  }

  if (processamentoDp?.hasValue()) {
    filtros['dataAprovacaoInicial'] = processamentoDp.getBeginDate();
    filtros['dataAprovacaoFinal'] = processamentoDp.getEndDate();
  }

  filtros['tipoConsulta'] = 'Documento Processado';
  filtros['clausulaAprovada'] = true;

  $('#exibirDocumentosDiv').show();

  await carregarDatatable();
}

async function consultarUrl() {
  const queryString = window.location.search;
  const urlParams = new URLSearchParams(queryString);

  if (!urlParams.has('sindId') || !urlParams.has('tipoSind')) return

  const sindId = urlParams.get('sindId');
  const tipoSind = urlParams.get('tipoSind')
  const sigla = urlParams.get('sigla')

  const sindOption = {
    description: sigla,
    id: sindId,
  }

  if (tipoSind == 'laboral') {
    sindicatoLaboralSelect.setCurrentValue(sindOption)
  } else if (tipoSind == 'patronal') {
    sindicatoPatronalSelect.setCurrentValue(sindOption)
  }

  await filtrar()
}

async function verDocumento(id) {
  const response = await documentoSindicalService.download({ id })

  if (response.isFailure()) {
    return NotificationService.error({ title: 'Não foi possível ver o arquivo.' })
  }

  const value = response.value

  PageWrapper.preview(value.data.blob, MediaType.pdf.Accept)
}

async function downloadDoc(id) {
  const response = await documentoSindicalService.download({ id })

  if (response.isFailure()) {
    return NotificationService.error({ title: 'Não foi possível baixar o arquivo.' })
  }

  const value = response.value

  PageWrapper.download(value.data.blob, value.data.filename, MediaType.stream.Accept)
}

async function carregarInformacoesModal() {
  await obterClausulasPorId()
  await obterInformacoesAdicionaisExistentesPorDocumento()
  montarTabela()
  preencherOrientacoes()
  montarCabecalho()

  await inicializarElementos()
}

async function obterClausulasPorId() {
  const result = await clausulaGeralService.obterPorDocumento(documentoId)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter clausulas do documento', message: result.error })
  }

  informacoesAdicionais = result.value
}

async function obterInformacoesAdicionaisExistentesPorDocumento() {
  const result = await informacaoAdicionalClienteService.obterPorDocumento(documentoId)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter informações do documento', message: result.error })
  }

  informacoesAdicionaisCliente = result.value
}

function montarTabela() {
  const informacoesAdicionaisElements = criarElementos()
  $('#informacoes_adicionais').html(informacoesAdicionaisElements)

  function criarElementos() {
    const container = div({})

    if (!informacoesAdicionais || informacoesAdicionais.length < 0) return

    informacoesAdicionais = informacoesAdicionais.map((informacaoAdicional) => {
      const { nomeEstruraClausula, nomeGrupoClausula, grupos: linhas, estruturaId, numero } = informacaoAdicional
      let cabecalhoCriado = false

      const title = h2({
        className: 'form-control-title',
        content: `${numero} - ${nomeEstruraClausula} | ${nomeGrupoClausula}`,
        style: 'margin-top: 40px;'
      })

      const tabelaContainer = div({
        style: 'width: 100%; overflow-x: scroll;'
      })

      const tabela = table({
        id: '12',
        className: 'table editable-table'
      })

      const tableHead = thead({})
      const tableBody = tbody({})
      const trhs = tr({})

      informacaoAdicional.grupos = linhas.map((item) => {
        const trbs = tr({})

        item.informacoesAdicionais = item.informacoesAdicionais.map((infoItem) => {
          const { descricao, codigo, tipo, dado, id } = infoItem

          if (codigo == 310) return

          if (!cabecalhoCriado) {
            trhs.append(th({
              content: descricao,
              className: 'table-head',
              style: 'min-width: 150px;'
            }))
          }

          const jOpcaoAdicinal = new JOpcaoAdicional({
            codigo,
            tipo,
            dado: procurarDados({ id, tipo, dado })
          })

          trbs.append(td({
            content: jOpcaoAdicinal.content,
            style: 'min-width: 150px;'
          }))

          infoItem.element = jOpcaoAdicinal

          return infoItem
        })

        tableBody.append(trbs)

        cabecalhoCriado = true

        return item

        function procurarDados({ id, tipo, dado }) {
          const dadoExistente = procurarInformacaoExistente({ id, tipo })

          if (tipo == InformacaoAdicionalTipo.Select || tipo == InformacaoAdicionalTipo.SelectMultiple) {
            if (dadoExistente) {
              return {
                combo: {
                  opcoes: dado.combo.opcoes,
                  valor: dadoExistente.combo.valor
                }
              }
            }

            return dado
          }

          return dadoExistente ?? dado
        }
      })

      tableHead.append(trhs)
      tabela.append(tableHead)
      tabela.append(tableBody)

      tabelaContainer.append(tabela)
      container.append(title)
      container.append(tabelaContainer)

      const containerObservacoesAdicionais = div({ style: 'width: 100%; display: flex; flex-direction: column; height: 150px; gap: 10px; margin-top: 15px;' })

      const observacaoAdicionalStyle = 'flex: 1; padding: 10px; border-radius: 3px; border: 1px solid gray;'

      const observacoesExistentes = obterObservacoesExistente({ idClausula: estruturaId })

      let primeiraObservacaoData = ''
      let segundaObservacaoData = ''
      observacoesExistentes.map(primeiraObservacao => {
        if (!primeiraObservacao) return

        const { tipo, valor } = primeiraObservacao

        if (tipo === TipoObservacaoAdicional.ComunicadoInterno) {
          return primeiraObservacaoData = valor
        }

        if (tipo === TipoObservacaoAdicional.RegrasParaEmpresa) {
          segundaObservacaoData = valor
        }
      })

      const primeiraObservacaoAdicionalId = Generator.id()
      const segundaObservacaoAdicionalId = Generator.id()

      const primeiraObservacaoAdicional = textarea({
        style: observacaoAdicionalStyle + 'display: none;',
        id: primeiraObservacaoAdicionalId,
        content: primeiraObservacaoData,
        className: 'observacoes-adicionais',
        placeholder: 'Orientação comunicado Interno'
      })

      const segundaObservacaoAdicional = textarea({
        style: observacaoAdicionalStyle,
        id: segundaObservacaoAdicionalId,
        content: segundaObservacaoData,
        className: 'observacoes-adicionais',
        placeholder: 'Regras parâmetro para empresa'
      })

      containerObservacoesAdicionais.append(primeiraObservacaoAdicional)
      containerObservacoesAdicionais.append(segundaObservacaoAdicional)
      container.append(containerObservacoesAdicionais)

      informacaoAdicional.observacoesAdicionais = [
        {
          clausulaId: estruturaId,
          dado: primeiraObservacaoData,
          tipo: TipoObservacaoAdicional.ComunicadoInterno,
          element: {
            val: () => $(`#${primeiraObservacaoAdicionalId}`).val()
          }
        },
        {
          clausulaId: estruturaId,
          dado: segundaObservacaoData,
          tipo: TipoObservacaoAdicional.RegrasParaEmpresa,
          element: {
            val: () => $(`#${segundaObservacaoAdicionalId}`).val()
          }
        }
      ]

      cabecalhoCriado = true

      return informacaoAdicional
    })

    informacoesAdicionais = informacoesAdicionais.sort(i => i.numero)

    return container
  }

  function procurarInformacaoExistente({ id, tipo }) {
    let dado = null

    informacoesAdicionaisCliente && informacoesAdicionaisCliente.informacoesAdicionais.map(({ clausulaGeralEstruturaId, valor }) => {
      if (clausulaGeralEstruturaId == id) dado = convertValueToData({ valor, tipo })
    })

    return dado
  }

  function obterObservacoesExistente({ idClausula }) {
    let observacao = []

    if (!informacoesAdicionaisCliente) return observacao

    informacoesAdicionaisCliente && informacoesAdicionaisCliente.observacoesAdicionais.map(({ clausulaId, valor, tipo }) => {
      if (idClausula == clausulaId) observacao.push({
        valor,
        tipo
      })
    })

    return observacao
  }

  function convertValueToData({ valor, tipo }) {
    let dado = {
      data: null,
      numerico: null,
      percentual: null,
      hora: null,
      texto: null,
      combo: {
        valor: []
      },
      descricao: null
    }

    switch (tipo) {
      case InformacaoAdicionalTipo.Date:
        dado.data = valor
        break
      case InformacaoAdicionalTipo.Number:
        dado.numerico = parseFloat(valor)
        break
      case InformacaoAdicionalTipo.Percent:
        dado.percentual = parseFloat(valor)
        break
      case InformacaoAdicionalTipo.Hour:
        dado.hora = valor
        break
      case InformacaoAdicionalTipo.Text:
        dado.texto = valor
        break
      case InformacaoAdicionalTipo.Monetario:
        dado.numerico = parseFloat(valor)
        break
      case InformacaoAdicionalTipo.Select:
        dado.combo.valor = valor.split(', ')
        break
      case InformacaoAdicionalTipo.SelectMultiple:
        dado.combo.valor = valor.split(', ')
        break
      default:
        dado.descricao = valor
        break
    }

    return dado
  }
}

async function inicializarElementos() {
  if (!informacoesAdicionais || informacoesAdicionais.length < 0) return

  const promises = []

  for (let linha of informacoesAdicionais) {
    for (let itemGrupo of linha.grupos) {
      const { informacoesAdicionais } = itemGrupo

      for (let item of informacoesAdicionais) {
        if (item) {
          promises.push(item.element.init())
        }
      }

      document.getElementById('asd').click()

      await Promise.all(promises)
      
      document.getElementById('asd').click()
    }
  }

  document.getElementById('asd').click()
}

function montarCabecalho() {
  const { aprovado, nomeUsuario, dataUltimaAlteracao, informacoesAdicionais } = informacoesAdicionaisCliente

  const alterado = informacoesAdicionais && informacoesAdicionais.length > 0

  $('#aprovado_p').html(`${aprovado ? 'Aprovado' : alterado ? 'Em processamento' : 'Processado Ineditta'}`)
  $('#nome_alterador_p').html(`${nomeUsuario ? nomeUsuario : 'Sem alteração'}`)
  $('#data_alteracao_p').html(`${dataUltimaAlteracao ? DateFormatter.dayMonthYear(dataUltimaAlteracao) : 'Sem alteração'}`)
  $('#documento_status_p').html(`${alterado > 0 ? 'Alterado' : 'Original'}`)
}

function preencherOrientacoes() {
  const { orientacao, outrasInformacoes } = informacoesAdicionaisCliente

  $('#orientacoes').val(orientacao)
  $('#outras-informacoes').val(outrasInformacoes)
}

async function incluir() {
  const result = await informacaoAdicionalClienteService.incluir({
    documentoSindicalId: documentoId,
    informacoesAdicionais: obterInformacoesAdicionaisDiferentes(),
    observacoesAdicionais: obterObservacoesAdicionaisDiferentes(),
    orientacao: $("#orientacoes").val(),
    outrasInformacoes: $("#outras-informacoes").val()
  })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao incluir informação', message: result.error })
  }

  NotificationService.success({ title: 'Salvo com sucesso' })

  return Result.success()
}

async function atualizar() {
  const result = await informacaoAdicionalClienteService.atualizar({
    body: {
      documentoSindicalId: documentoId,
      informacoesAdicionais: obterInformacoesAdicionaisDiferentes(),
      observacoesAdicionais: obterObservacoesAdicionaisDiferentes(),
      orientacao: $("#orientacoes").val(),
      outrasInformacoes: $("#outras-informacoes").val()
    },
    id: documentoId
  })

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao atualizar informação', message: result.error })
  }

  NotificationService.success({ title: 'Salvo com sucesso' })

  return Result.success()
}

async function aprovar() {
  const { informacoesAdicionais, orientacao, outrasInformacoes, observacoesAdicionais } = informacoesAdicionaisCliente

  const alterado = informacoesAdicionais && informacoesAdicionais.length > 0

  let message = ''
  if (alterado) {
    message = 'Deseja aprovar com as alterações realizadas?'
  } else {
    message = 'Deseja aprovar sem ter realizado nenhuma alteração?. Informações Geradas do Sistema Ineditta, sendo responsabilidade da Empresa (Cliente) a validação para aplicação.'
  }

  return new Promise((resolve) => {
    NotificationService.success({
      title: message,
      showConfirmButton: true,
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Sim, enviar!',
      then: async (result) => {
        if (result.isConfirmed) {
          if (!alterado ||
            orientacao ||
            outrasInformacoes ||
            (observacoesAdicionais && observacoesAdicionais.length > 0)) {
            await incluir()
          }

          const result = await informacaoAdicionalClienteService.aprovar(documentoId)

          if (result.isFailure()) {
            return NotificationService.error({ title: 'Erro ao aprovar informação', message: result.error })
          }

          NotificationService.success({ title: 'Salvo com sucesso!' });
        }

        resolve()
      },
      timer: null,
    });
  });

}

async function downloadExcel() {
  const { aprovado } = informacoesAdicionaisCliente

  if (!aprovado && dadosPessoais.nivel != UsuarioNivel.Ineditta) {
    return NotificationService.error({ title: 'É necessário aprovar o formulário para gerar o Excel' })
  }

  const result = await informacaoAdicionalClienteService.relatorio(documentoId)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao baixar excel', message: result.error })
  }

  const bytes = result.value.data.blob

  PageWrapper.downloadExcel(bytes, 'formulario_aplicacao.xlsx')
}

async function obterInfoSindicatoPorId(id, tipoSind) {
  const infoResult = await sindicatoService.obterInfoSindical(id, tipoSind)

  preencherModalInfoSindical(infoResult.value, id, tipoSind)
}

function preencherModalInfoSindical(data, id, tipoSind) {
  limparModalInfo()

  $("#info-sigla").val(data.sigla)
  $("#info-cnpj").maskCNPJ().val(data.cnpj).trigger("input")
  $("#info-razao").val(data.razaoSocial)
  $("#info-denominacao").val(data.denominacao)
  $("#info-cod-sindical").val(data.codigoSindical)
  $("#info-uf").val(data.uf)
  $("#info-municipio").val(data.municipio)
  $("#info-logradouro").val(data.logradouro)
  $("#info-telefone1").maskCelPhone().val(data.telefone1).trigger("input")
  $("#info-telefone2").maskCelPhone().val(data.telefone2).trigger("input")
  $("#info-telefone3").maskCelPhone().val(data.telefone3).trigger("input")
  $("#info-ramal").val(data.ramal)
  $("#info-enquadramento").val(data.contatoEnquadramento)
  $("#info-negociador").val(data.contatoNegociador)
  $("#info-contribuicao").val(data.contatoContribuicao)
  $("#info-email1")
    .val(data.email1)
    .attr("style", data.email1 ? "cursor: pointer" : null)
  $("#info-email1-link").attr("href", `mailto:${data.email1}`)
  $("#info-email2")
    .val(data.email2)
    .attr("style", data.email2 ? "cursor: pointer" : null)
  $("#info-email2-link").attr("href", `mailto:${data.email2}`)
  $("#info-email3")
    .val(data.email3)
    .attr("style", data.email3 ? "cursor: pointer" : null)
  $("#info-email3-link").attr("href", `mailto:${data.email3}`)
  $("#info-twitter")
    .val(data.twitter)
    .attr("style", data.twitter ? "cursor: pointer" : null)
  $("#info-twitter-link").attr("href", formatarLinks(data.twitter, "twitter"))
  $("#info-facebook")
    .val(data.facebook)
    .attr("style", data.facebook ? "cursor: pointer" : null)
  $("#info-facebook-link").attr(
    "href",
    formatarLinks(data.facebook, "facebook")
  )
  $("#info-instagram")
    .val(data.instagram)
    .attr("style", data.instagram ? "cursor: pointer" : null)
  $("#info-instagram-link").attr(
    "href",
    formatarLinks(data.instagram, "instagram")
  )
  $("#info-site")
    .val(data.site)
    .attr("style", data.site ? "cursor: pointer" : null)
  $("#info-site-link").attr("href", formatarLinks(data.site, "site"))
  $("#info-data-base").val(data.dataBase)
  $("#info-ativ-econ").val(data.atividadeEconomica)
  $("#info-federacao-nome").val(data?.federacao?.nome)
  $("#info-federacao-cnpj")
    .maskCNPJ()
    .val(data?.federacao?.cnpj)
    .trigger("input")
  $("#info-confederacao-nome").val(data?.confederacao?.nome)
  $("#info-confederacao-cnpj")
    .maskCNPJ()
    .val(data?.confederacao?.cnpj)
    .trigger("input")
  $("#info-central-sind-nome").val(data?.centralSindical?.nome)
  $("#info-central-sind-cnpj")
    .maskCNPJ()
    .val(data?.centralSindical?.cnpj)
    .trigger("input")

  $("#direct-clausulas-btn").attr(
    "href",
    `consultaclausula.php?sindId=${id}&tipoSind=${tipoSind}&comparativo=${false}&sigla=${data.sigla
    }`
  )
  $("#direct-comparativo-btn").attr(
    "href",
    `consultaclausula.php?sindId=${id}&tipoSind=${tipoSind}&comparativo=${true}&sigla=${data.sigla
    }`
  )
  $("#direct-calendarios-btn").attr(
    "href",
    `calendario_sindical.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  )
  $("#direct-documentos-btn").attr(
    "href",
    `consulta_documentos.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  )
  $("#direct-formulario-aplicacao-btn").attr(
    "href",
    `formulario_comunicado.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  )

  $("#direct-gerar-excel-btn").attr(
    "href",
    `geradorCsv.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  )

  $("#direct-comparativo-mapa-btn").attr(
    "href",
    `comparativo.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  )

  $("#direct-relatorio-negociacoes-btn").attr(
    "href",
    `relatorio_negociacoes.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  )

  function formatarLinks(string, tipo) {
    if (string == null) return ""
    if (string.includes(".com")) {
      if (string.includes("http")) {
        return string
      } else {
        return `https://${string}`
      }
    }

    if (tipo === "site") {
      if (string.includes("http")) {
        return string
      } else {
        return `https://${string}`
      }
    }

    if (tipo === "twitter") return `https://twitter.com/${string}`
    if (tipo === "instagram") return `https://instagram.com/${string}`
    if (tipo === "facebook") return `https://facebook.com/${string}`
  }
}

function limparModalInfo() {
  $("#infoSindForm").trigger("reset")
}

async function carregarDataTableInfoDiretoriaTb(sindId, tipoSind) {
  if (diretoriaInfoSindTb) {
    diretoriaInfoSindTb.reload()
    return
  }

  diretoriaInfoSindTb = new DataTableWrapper("#diretoriainfosindtb", {
    ajax: async (requestData) =>
      await sindicatoService.obterInfoDiretoriaSindDatatable(
        requestData,
        sindId,
        tipoSind
      ),
    columns: [
      { title: "Dirigente", data: "nome" },
      {
        title: "Início Mandato",
        data: "inicioMandato",
      },
      { title: "Fim Mandato", data: "fimMandato" },
      { title: "Função", data: "funcao" },
    ],
    columnDefs: [
      {
        targets: [1, 2],
        render: (data) => DataTableWrapper.formatDate(data),
      },
      {
        targets: "_all",
        defaultContent: "",
      },
    ],
  })

  await diretoriaInfoSindTb.initialize()
}

function obterInformacoesAdicionaisDiferentes() {
  const items = []

  informacoesAdicionais && informacoesAdicionais.map((informacaoAdicional) => {
    const { grupos: linhas } = informacaoAdicional

    linhas.map(({ informacoesAdicionais }) => {
      informacoesAdicionais.map((item) => {
        if (!item || !item.element) return

        const { element, dado, tipo, id } = item

        const data = element.val()
        if (!data) return

        switch (tipo) {
          case InformacaoAdicionalTipo.Select:
            if (dado.combo.valor != data) createItem(id, data)
            break
          case InformacaoAdicionalTipo.SelectMultiple:
            if ((dado.combo.valor).join(', ') != (data).join(', ')) createItem(id, data.join(', '))
            break
          case InformacaoAdicionalTipo.Hour:
            if (dado.hora != data) createItem(id, data)
            break
          case InformacaoAdicionalTipo.Monetario:
            if (parseFloat(dado.numerico) != parseFloat(data)) {
              createItem(id, data)
            }
            break
          case InformacaoAdicionalTipo.Number:
            if (parseFloat(dado.numerico) != parseFloat(data)) {
              createItem(id, data)
            }
            break
          case InformacaoAdicionalTipo.Percent:
            if (parseFloat(dado.percentual) != parseFloat(data)) createItem(id, data)
            break
          case InformacaoAdicionalTipo.Text:
            if (dado.texto != data) createItem(id, data)
            break
          case InformacaoAdicionalTipo.Date:
            const value = DateParser.toString(data)

            if (dado.data != value) createItem(id, value)
            break
          default:
            if (dado.descricao != data) createItem(id, data)
            break
        }
      })
    })
  })

  return items

  function createItem(id, data) {
    items.push({
      id,
      valor: data.toString()
    })
  }
}

function obterObservacoesAdicionaisDiferentes() {
  const observacoes = []

  for (const informacaoAdicional of informacoesAdicionais) {
    if (!informacaoAdicional.observacoesAdicionais) {
      continue
    }

    for (const observacaoAdicional of informacaoAdicional.observacoesAdicionais) {
      const { clausulaId, element, dado, tipo } = observacaoAdicional
      const data = element.val()

      if (dado != data) {
        observacoes.push({
          id: clausulaId,
          valor: data,
          tipo
        })
      }
    }
  }

  return observacoes
}