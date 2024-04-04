import "datatables.net-bs5/css/dataTables.bootstrap5.css"
import "datatables.net-responsive-bs5/css/responsive.bootstrap5.css"

import JQuery from "jquery"
import $ from "jquery"
import "../../js/utils/util.js"
import "select2"

import "bootstrap/dist/css/bootstrap.min.css"
import "bootstrap"

import { AuthService } from "../../js/core/auth"
import { ApiService } from "../../js/core/api.js"

import { ApiLegadoService } from "../../js/core/api-legado"
import NotificationService from "../../js/utils/notifications/notification.service.js"
import Result from "../../js/core/result.js"
import {
  closeModal,
  renderizarModal,
} from "../../js/utils/modals/modal-wrapper.js"
import SelectWrapper from "../../js/utils/selects/select-wrapper.js"

// Services
import {
  UsuarioAdmService,
  AcompanhamentoCctService,
  SindicatoPatronalService,
  SindicatoLaboralService,
  AcompanhamentoCctStatusOpcaoService,
  AcompanhamentoCctFasesService,
  BaseTerritorialSindicatoLaboralService,
  GrupoEconomicoService,
  CnaeService,
  MatrizService,
  AcompanhamentoCctEtiquetaOpcaoService
} from "../../js/services"

import { ArrayUtils } from "../../js/utils/util.js"
import { fasesCct } from "../../js/application/acompanhamentos-cct/fases/constants/fases-cct.js"
import { stringLi, stringP, stringDiv, stringSpan, stringH4 } from "../../js/utils/components/string-elements"

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import DateFormatter from "../../js/utils/date/date-formatter.js"
import DatePickerWrapper from "../../js/utils/datepicker/datepicker-wrapper.js"
import {
  createTemplateSindicato,
  dispararEmails,
  limparFormularioEmailCliente,
  limparFormularioEmailSindicato,
  setDestinatariosEmail,
  setTemplateCliente,
  configurarScripts,
  adicionarScript,
  limparFormularioScript,
  incluirAcompanhamento,
  atualizarAcompanhamento
} from "../../js/modules/acompanhamento-db"
import DataTableWrapper from "../../js/utils/datatables/datatable-wrapper.js"
import moment from "moment"
import { EmailsAcompanhametoCct } from "../../js/modules/acompanhamento-db/features/emails/enums.js"
import { assuntoSelect } from "../../js/modules/core/index.js"

let idsForms = ""
let id = null

let sindicatosLaboraisIds = []
let sindicatosPatronaisIds = []
let empresasIds = []
let gruposEconomicosIds = []
let openedScript = false

let fasesCadastroSelect = null
let sindLaboralCadastroSelect = null
let sindPatronalCadastroSelect = null
let tiposDocsCadastroSelect = null
let responsavelCadastroSelect = null
let fasesAgendamentoSelect = null
let tiposDocsAgendamentoSelect = null
let sindicatoLaboralFiltroSelect = null
let sindicatoPatronalFiltroSelect = null
let statusSelect = null
let assuntosSelect = null
let etiquetasSelect = null

let dataProcessamentoDt = null

let acompanhamentoCctTb = null
let acompanhamentoCctFuturoTb = null
let cnaeTb = null
let localizacaoTb = null
let empresaTb = null
let grupoEconomicoTb = null

let acompanhamentoSelecionado = null
let acompanhamentosIds = []
let emailsLaboral = null
let emailsPatronal = null
let cnaesIds = []
let localizacoesIds = []
let sindPatronaisSelecionadosFilter = []
let sindLaboraisSelecionadosFilter = []

const apiService = new ApiService()
const apiLegadoService = new ApiLegadoService()
const acompanhamentoCctService = new AcompanhamentoCctService(apiService, apiLegadoService)
const sindicatoLaboralService = new SindicatoLaboralService(apiService, apiLegadoService)
const sindicatoPatronalService = new SindicatoPatronalService(apiService, apiLegadoService)
const usuarioAdmService = new UsuarioAdmService(apiService, apiLegadoService)
const acompanhamentoCctStatusOpcaoService = new AcompanhamentoCctStatusOpcaoService(apiService)
const acompanhamentoCctFasesService = new AcompanhamentoCctFasesService(apiService)
const baseTerritorialSindicatoLaboralService = new  BaseTerritorialSindicatoLaboralService(apiService)
const grupoEconomicoService = new GrupoEconomicoService(apiService)
const cnaeService = new CnaeService(apiService)
const matrizService = new MatrizService(apiService)
const acompanhamentoCctEtiquetaOpcaoService = new AcompanhamentoCctEtiquetaOpcaoService(apiService)

JQuery(async () => {
  new Menu()

  await AuthService.initialize()

  await carregarDadosIniciais()

  configurarModal()

  configurarFormulario()

  await dataTableAcompanhamento()
})

async function carregarDadosIniciais() {
  const result = await acompanhamentoCctService.obterInformacoesIniciais()

  $("#neg_aberto").text(result.value.negociacoesEmAberto)
  $("#sem_mov").text(result.value.semMovimentacao30Dias)
  $("#count_ligacoes").text(result.value.ligacoesDoDia)
}

function configurarFormulario() {
  handleToggleScriptButton({ state: false })

  $(".select2").select2()
  $("#scriptPanel").toggle("slow")

  $('#scriptForm').on('submit', e => e.preventDefault())
  $('#formAdd').on('submit', e => e.preventDefault())
  $("#btn-add-script-new").on("click", () => openedScript ? handleToggleScriptButton({ state: false }) : handleToggleScriptButton({ state: true }))
  $("#btn-save-script").on("click", async () => await handleClickAdicionarScript())

  dataProcessamentoDt = new DatePickerWrapper("#data-processamento-input")
  
  configurarSelects()
  function configurarSelects() {
    fasesCadastroSelect = new SelectWrapper("#fase-input", {
      onOpened: async () => (await acompanhamentoCctFasesService.obterSelect()).value,
      onSelected: (item) => {
        if (parseInt(item.id) === fasesCct.Concluida) {
          return $("#datafim-input").prop("disabled", false)
        }
  
        $("#datafim-input").prop("disabled", true)
      },
    })
  
    fasesAgendamentoSelect = new SelectWrapper("#fase-fut-input", { onOpened: async () => (await acompanhamentoCctFasesService.obterSelect()).value })
  
    sindLaboralCadastroSelect = new SelectWrapper("#sind-input", {
      onOpened: async () => await obterSinds("laboral"),
      onSelected: ({ id, text }) => {
        sindicatosLaboraisIds.push(id.toString())
        sindLaboraisSelecionadosFilter.push({
          id: id,
          description: `${id} - ${text}`,
        })
        sindicatoLaboralFiltroSelect.reload()
      },
      onUnSelected: (item) => {
        sindicatosLaboraisIds = sindicatosLaboraisIds.filter((id) => id != item.id.toString())
        sindLaboraisSelecionadosFilter = sindLaboraisSelecionadosFilter.filter((sindicatoItem) => sindicatoItem.id.toString() != item.id.toString())
        sindicatoLaboralFiltroSelect.reload()
      },
    })
  
    sindPatronalCadastroSelect = new SelectWrapper("#emp-input", {
      onOpened: async () => await obterSinds("patronal"),
      onSelected: ({ id, text }) => {
        sindicatosPatronaisIds.push(id.toString())
        sindPatronaisSelecionadosFilter.push({
          id: id,
          description: `${id} - ${text}`,
        })
        sindicatoPatronalFiltroSelect.reload()
      },
      onUnSelected: (item) => {
        sindicatosPatronaisIds = sindicatosPatronaisIds.filter((id) => id != item.id.toString())
        sindPatronaisSelecionadosFilter = sindPatronaisSelecionadosFilter.filter((sindicatoItem) => sindicatoItem.id.toString() != item.id.toString())
        sindicatoPatronalFiltroSelect.reload()
      },
    })
  
    sindicatoLaboralFiltroSelect = new SelectWrapper("#sindicato-laboral-filtro", {
      onOpened: async () => await Promise.resolve(sindLaboraisSelecionadosFilter),
      onSelected: async (item) => await obertSindicatoLaboralPorId(item.id),
      options: { allowEmpty: true }
    })
  
    sindicatoPatronalFiltroSelect = new SelectWrapper("#sindicato-patronal-filtro",{
      onOpened: async () => await Promise.resolve(sindPatronaisSelecionadosFilter),
      onSelected: async (item) => await obertSindicatoPatronalPorId(item.id),
      options: { allowEmpty: true }
    })
  
    tiposDocsCadastroSelect = new SelectWrapper("#tipodoc-input", { onOpened: async () => (await acompanhamentoCctService.obterSelectTiposDocs()).value })  
    tiposDocsAgendamentoSelect = new SelectWrapper("#tipodoc-fut-input", { onOpened: async () => (await acompanhamentoCctService.obterSelectTiposDocs()).value })
    responsavelCadastroSelect = new SelectWrapper("#resp-input", { onOpened: async () => (await usuarioAdmService.obterSelectUsuariosIneditta()).value })
    statusSelect = new SelectWrapper("#status-input", { onOpened: async () => (await acompanhamentoCctStatusOpcaoService.obterSelect()).value })

    new SelectWrapper("#status-futuras-input", { onOpened: async () => (await acompanhamentoCctStatusOpcaoService.obterSelect()).value })
    new SelectWrapper("#fases-futuras-input", { onOpened: async () => (await acompanhamentoCctFasesService.obterSelect()).value })

    assuntosSelect = assuntoSelect()
    etiquetasSelect = new SelectWrapper('#etiqueta',  { onOpened: async () => (await acompanhamentoCctEtiquetaOpcaoService.obterSelect()).value })
  }
}

async function obertSindicatoPatronalPorId(id) {
  const result = await sindicatoPatronalService.obterPorId(id, { cnaesIds: acompanhamentoSelecionado.cnaes.map(a => parseInt(a)) })

  if (result.isFailure()) {
    return
  }

  const sindicato = result.value

  sindicato.site
    ? $("#sind_patronal_link_site").val(sindicato.site)
    : $("#sind_patronal_link_site").val("")
  sindicato.email1
    ? $("#sind_patronal_email_1").val(sindicato.email1)
    : $("#sind_patronal_email_1").val("")
  sindicato.email2
    ? $("#sind_patronal_email_2").val(sindicato.email2)
    : $("#sind_patronal_email_2").val("")
  sindicato.email3
    ? $("#sind_patronal_email_3").val(sindicato.email3)
    : $("#sind_patronal_email_3").val("")
  sindicato.telefone1
    ? $("#sind_patronal_telefone_1").val(sindicato.telefone1)
    : $("#sind_patronal_telefone_1").val("")
  sindicato.telefone2
    ? $("#sind_patronal_telefone_2").val(sindicato.telefone2)
    : $("#sind_patronal_telefone_2").val("")
  sindicato.telefone3
    ? $("#sind_patronal_telefone_3").val(sindicato.telefone3)
    : $("#sind_patronal_telefone_3").val("")
  sindicato.comentario
    ? $("#sind_patronal_coment").val(sindicato.comentario)
    : $("#sind_patronal_coment").val("")
  
  const atividadesEconomicas = sindicato.atividadesEconomicas
  if (atividadesEconomicas && atividadesEconomicas.length > 0) {
    const atvsLaboral = atividadesEconomicas.join(", ")

    $("#sind_patronal_atv").val(atvsLaboral)
  }
}

async function obertSindicatoLaboralPorId(id) {
  const result = await sindicatoLaboralService.obterPorId(id, { cnaesIds: acompanhamentoSelecionado.cnaes.map(c => parseInt(c)) })

  if (result.isFailure()) {
    return
  }

  const sindicato = result.value

  sindicato.site
    ? $("#sind_laboral_link_site").val(sindicato.site)
    : $("#sind_laboral_link_site").val("")
  sindicato.email1
    ? $("#sind_laboral_email_1").val(sindicato.email1)
    : $("#sind_laboral_email_1").val("")
  sindicato.email2
    ? $("#sind_laboral_email_2").val(sindicato.email2)
    : $("#sind_laboral_email_2").val("")
  sindicato.email3
    ? $("#sind_laboral_email_3").val(sindicato.email3)
    : $("#sind_laboral_email_3").val("")
  sindicato.telefone1
    ? $("#sind_laboral_telefone_1").val(sindicato.telefone1)
    : $("#sind_laboral_telefone_1").val("")
  sindicato.telefone2
    ? $("#sind_laboral_telefone_2").val(sindicato.telefone2)
    : $("#sind_laboral_telefone_2").val("")
  sindicato.telefone3
    ? $("#sind_laboral_telefone_3").val(sindicato.telefone3)
    : $("#sind_laboral_telefone_3").val("")
  sindicato.comentario
    ? $("#sind_laboral_coment").val(sindicato.comentario)
    : $("#sind_laboral_coment").val("")

  const atividadesEconomicas = sindicato.atividadesEconomicas
  if (atividadesEconomicas && atividadesEconomicas.length > 0) {
    const atvsLaboral = atividadesEconomicas.join(", ")

    $("#sind_laboral_atv").val(atvsLaboral)
  }
}

async function obterSinds(tipo) {
  if (tipo === "laboral") {
    return (await sindicatoLaboralService.obterSelect()).value
  }

  return (await sindicatoPatronalService.obterSelect()).value
}

function configurarModal() {
  $("#novoAcompanhamentoBtn").on("click", () => {
    const id = $("#id-input").val()
    if (!id) {
      $("#edicaoFields").hide()
      $("#-").hide()
    }
  })

  const pageCtn = document.getElementById("pageCtn")

  const modalCadastro = document.getElementById("cadastrarModalHidden")
  const contentCadastro = document.getElementById("cadastrarModalHiddenContent")

  const empresaModalHidden = document.getElementById("empresaModalHidden")
  const empresaModalContent = document.getElementById("empresaModalContent")

  const grupoEconomicoModalHidden = document.getElementById("grupoEconomicoModalHidden")
  const grupoEconomicoModalContent = document.getElementById("grupoEconomicoModalContent")

  const buttonsCadastroConfig = [
    {
      id: "cadastrarBtn",
      onClick: async (_, modalContainer) => {
        const result = await upsert()
        if (result.isSuccess()) {
          closeModal(modalContainer)
          await acompanhamentoCctTb.reload()
        }
      },
    },
  ]

  const modalFuturas = document.getElementById("futurasModalHidden")
  const contentFuturas = document.getElementById("futurasModalHiddenContent")

  const buttonsFuturaConfig = [
    {
      id: "processarAcompanhamentosFuturosBtn",
      onClick: async () => {
        await addMultiAcompanhamento()
      },
    },
  ]

  const modalCnae = document.getElementById("cnaeModalHidden")
  const contentCnae = document.getElementById("cnaeModalHiddenContent")

  const modalLocalizacao = document.getElementById("localizacaoModalHidden")
  const contentLocalizacao = document.getElementById("localizacaoModalHiddenContent")

  const modalHist = document.getElementById("scriptHistModalHidden")
  const contentHist = document.getElementById("scriptHistModalHiddenContent")

  const modalEmailSind = document.getElementById("emailSindModalHidden")
  const contentEmailSind = document.getElementById("emailSindModalHiddenContent")

  const modalEmailCli = document.getElementById("emailCliModalHidden")
  const contentEmailCli = document.getElementById("emailCliModalHiddenContent")

  const buttonsEmailSindConfig = [
    {
      id: "dispararEmailsSindBtn",
      onClick: async () => {
        let emails = $("#para-input").val().split(",")
        emails = emails.map(email => email.trim())

        const template = $("#msg-input").val()
        const assunto = $("#assunto-input").val()

        await dispararEmails({ emails, template, assunto })
      },
    },
    {
      id: "sindTemplateBtn1",
      onClick: () => createTemplateSindicato({
        acompanhamentoSelecionado,
        template: EmailsAcompanhametoCct.Sindicatos.AssembleiaPatronal
      }),
    },
    {
      id: "sindTemplateBtn2",
      onClick: () => createTemplateSindicato({
        acompanhamentoSelecionado,
        template: EmailsAcompanhametoCct.Sindicatos.NegociaçãoCCT
      })
    },
  ]

  const buttonsEmailCliConfig = [
    {
      id: "dispararEmailsCliBtn",
      onClick: async () => {
        let emails = $("#para-input-cli").val().split(",")
        emails = emails.map(email => email.trim())

        const template = $("#msg-input-cli").val()
        const assunto = $("#assunto-input-cli").val()

        await dispararEmails({ emails, template, assunto })
      },
    },
    {
      id: "cliTemplateBtn1",
      onClick: async () => {
        setTemplateCliente({
          acompanhamentoSelecionado,
          template: EmailsAcompanhametoCct.Clientes.AtaCircularReajusteSalarial
        })
      },
    },
    {
      id: "cliTemplateBtn2",
      onClick: async () => {
        setTemplateCliente({
          acompanhamentoSelecionado,
          template: EmailsAcompanhametoCct.Clientes.ContatoSindicato
        })
      },
    },
    {
      id: "cliTemplateBtn3",
      onClick: async () => {
        setTemplateCliente({
          acompanhamentoSelecionado,
          template: EmailsAcompanhametoCct.Clientes.AbrirCnpjENomeEmpesa
        })
      },
    },
  ]

  const modalsConfig = [
    {
      id: "cadastrarModal",
      modal_hidden: modalCadastro,
      content: contentCadastro,
      btnsConfigs: buttonsCadastroConfig,
      onOpen: async () => {
        id = $("#id-input").val()

        if (id) {
          await obterAcompanhamentoPorId(id)
        }
      },
      onClose: () => {
        limparFormulario()

        handleToggleScriptButton({ state: false, openedScript: openedScript })
        idsForms = ''
        limparFormularioScript()
      },
    },
    {
      id: "futurasModal",
      modal_hidden: modalFuturas,
      content: contentFuturas,
      btnsConfigs: buttonsFuturaConfig,
      onOpen: async () => await dataTableFuturas(),
      onClose: () => {
        $("#status-fut-input").val(null)
        tiposDocsAgendamentoSelect.clear()
        fasesAgendamentoSelect.clear()
      },
    },
    {
      id: "cnaeModal",
      modal_hidden: modalCnae,
      content: contentCnae,
      btnsConfigs: [],
      onOpen: async () => await datatableCnae(),
      isInIndex: true,
    },
    {
      id: "localizacaoModal",
      modal_hidden: modalLocalizacao,
      content: contentLocalizacao,
      btnsConfigs: [],
      onOpen: async () => await dataTableLocalizacao(),
      isInIndex: true,
    },
    {
      id: "scriptHistModal",
      modal_hidden: modalHist,
      content: contentHist,
      btnsConfigs: [],
      onOpen: async () => {
        const id = $("#id-input").val()
        await carregarHistoricoScripts(id)
      },
      isInIndex: true,
    },
    {
      id: "emailSindModal",
      modal_hidden: modalEmailSind,
      content: contentEmailSind,
      btnsConfigs: buttonsEmailSindConfig,
      onOpen: async () => {
        preencherDadosEmailSind()
      },
      isInIndex: true,
    },
    {
      id: "emailCliModal",
      modal_hidden: modalEmailCli,
      content: contentEmailCli,
      btnsConfigs: buttonsEmailCliConfig,
      onOpen: async () => {
        await preencherDadosEmailCliente()
      },
      isInIndex: true,
    },
    {
      id: "empresaModal",
      modal_hidden: empresaModalHidden,
      content: empresaModalContent,
      btnsConfigs: [],
      onOpen: async () => await dataTableEmresa(),
      onClose: () => null,
      isInIndex: true,
    },
    {
      id: "grupoEconomicoModal",
      modal_hidden: grupoEconomicoModalHidden,
      content: grupoEconomicoModalContent,
      btnsConfigs: [],
      onOpen: async () => await dataTableGrupoEconomico(),
      onClose: () => null,
      isInIndex: true,
    },
  ]

  renderizarModal(pageCtn, modalsConfig)
}

function preencherDadosEmailSind() {
  const destinatarioLaboral = $("#destinatario-laboral")
  destinatarioLaboral.text(emailsLaboral)
  destinatarioLaboral.on("click", () => setDestinatariosEmail(emailsLaboral))

  const destinatarioPatronal = $("#destinatario-patronal")
  destinatarioPatronal.text(emailsPatronal)
  destinatarioPatronal.on("click", () => setDestinatariosEmail(emailsPatronal))
}

async function preencherDadosEmailCliente() {
  const result = await acompanhamentoCctService.obterUsuariosPorGruposId({
    id: acompanhamentoSelecionado.id,
    params: {
      grupoEconomico: (acompanhamentoSelecionado.gruposEconomicos && acompanhamentoSelecionado.gruposEconomicos.length > 0)
    }
  })

  if (result.isFailure()) return

  const data = result.value

  let emails = []
  data.map(({ email }) => emails.push(email))

  $("#para-input-cli").val(emails.join(', '))
}

async function carregarHistoricoScripts() {
  const timeline = $("#history-timeline")
  const result = await acompanhamentoCctService.obterPerguntasRespostasFases(acompanhamentoSelecionado.id)

  if (!result.success) {
    return NotificationService.error({ title: 'Erro ao obter histórico de acompanhamento', message: result.error })
  }

  let acompanhamento = result.value

  if (acompanhamento.length <= 0) return timeline.html(stringP({ text: 'Não foi encontrado nenhum histórico para esse acompanhamento' }))

  acompanhamento = ArrayUtils.dateTimeSort(acompanhamento, 'horario', 'desc')

  const content = acompanhamento.map(({ fase, horario, perguntas, respostas, nomeUsuario }) => {
    respostas = JSON.parse(respostas)

    if (respostas.length <= 0) return timeline.html(stringP({ text: 'Não foi encontrado nenhum histórico para esse acompanhamento' }))

    if (perguntas) perguntas = JSON.parse(perguntas)

    return stringLi({
      className: 'timeline-primary',
      content: stringDiv({
        className: 'timeline-icon'
      }) + stringDiv({
        className: 'timeline-body',
        children: stringDiv({
          className: 'timeline-header',
          children: stringSpan({
            className: 'date',
            children: DateFormatter.dateTime(horario)
          })
        }) + stringDiv({
          className: 'timeline-content',
          children: stringH4({ text: 'Fase' }) + stringP({ text: `${fase} (${nomeUsuario})` }) + stringDiv({
            className: 'row gap-2',
            children: (() => {
              const itens = []

              perguntas.perguntas.map(({ texto, adicionais }) => {
                itens.push(texto)
                if (adicionais) adicionais.map(({ texto }) => itens.push(texto))
              })

              let result = ''
              itens.map((texto, i) => {
                result += stringDiv({
                  className: 'col-sm-4',
                  children: stringH4({ text: texto }) + stringP({
                    text: (() => {
                      let value = respostas[i]

                      if (value && DateFormatter.isDateValid(value)) {
                        value = DateFormatter.dayMonthYear(value)
                      }

                      return `R: ${value}`
                    })()
                  })
                })
              })

              return result
            })()
          })
        })
      })
    })
  })

  timeline.html(content)
}

async function obterAcompanhamentoPorId(id) {
  limparFormulario()

  if (!id) return

  const result = await acompanhamentoCctService.obterPorId(id)

  acompanhamentoSelecionado = result.value

  preencherDados()

  function preencherDados() {
    const acompanhamento = acompanhamentoSelecionado
    
    const sindLaboralOption = []
    sindLaboraisSelecionadosFilter= []
    if (acompanhamento.sindicatosLaborais) {
      acompanhamento.sindicatosLaborais.map((sindicatoItem) => {
        sindicatosLaboraisIds.push(sindicatoItem.id)

        const description = `${sindicatoItem.id} - ${sindicatoItem.sigla} / ${sindicatoItem.denominacao} / ${sindicatoItem.cnpj}`

        sindLaboralOption.push({
          id: sindicatoItem.id,
          description
        })
      })
    }
    sindLaboraisSelecionadosFilter = sindLaboralOption
  
    const sindPatronalOption = []  
    sindPatronaisSelecionadosFilter = []
    if (acompanhamento.sindicatosPatronais) {
      acompanhamento.sindicatosPatronais.map((sindicatoItem) => {
        sindicatosPatronaisIds.push(sindicatoItem.id)

        const description = `${sindicatoItem.id} - ${sindicatoItem.sigla} / ${sindicatoItem.denominacao} / ${sindicatoItem.cnpj}`

        sindPatronalOption.push({
          id: sindicatoItem.id,
          description,
        })
      })
    }
    sindPatronaisSelecionadosFilter = sindPatronalOption
  
    const statusOption = {
      id: acompanhamento.status_id,
      description: acompanhamento.status,
    }

    const faseOption = {
      id: acompanhamento.fase_id,
      description: acompanhamento.fase,
    }
  
    const tipoDocOption = {
      id: acompanhamento.idTipoDocumento,
      description: acompanhamento.nomeTipoDocumento,
    }
  
    const usuarioResponsavelOption = {
      description: acompanhamento.nomeUsuario,
      id: acompanhamento.usuarioResponsavelId,
    }

    if (acompanhamento.assuntos) {
      let assuntosOprions = []
      acompanhamento.assuntos.map(assunto => {
        assuntosOprions.push({
          id: assunto.id,
          description: assunto.descricao
        })
      })

      assuntosSelect.setCurrentValue(assuntosOprions)
    }

    if (acompanhamento.etiquetas) {
      let etiquetasOprions = []
      acompanhamento.etiquetas.map(assunto => {
        etiquetasOprions.push({
          id: assunto.id,
          description: assunto.descricao
        })
      })

      etiquetasSelect.setCurrentValue(etiquetasOprions)
    }
  
    statusSelect.setCurrentValue(statusOption)
    fasesCadastroSelect.setCurrentValue(faseOption)
    $("#db-input").val(acompanhamento.dataBase.substring(0, 3))
    $("#dbano-input").val(acompanhamento.dataBase.substring(4))
    $("#dataini-input").val(acompanhamento.dataInicial)
    $("#validade-final").val(acompanhamento.validadeFinal)
    $("#datafim-input").val(acompanhamento.dataFinal)
    $("#anotacoes-input").val(acompanhamento.anotacoes)
    sindLaboralCadastroSelect.setCurrentValue(sindLaboralOption)
    sindPatronalCadastroSelect.setCurrentValue(sindPatronalOption)
    tiposDocsCadastroSelect.setCurrentValue(tipoDocOption)
    responsavelCadastroSelect.setCurrentValue(usuarioResponsavelOption)
    empresasIds = acompanhamento.empresas
    gruposEconomicosIds = acompanhamento.gruposEconomicos
    if (acompanhamento.idsCnaes) cnaesIds = acompanhamento.cnaesIds
    $("#client-report").val(acompanhamento.observacoesGerais)
    cnaesIds = acompanhamento.cnaes || []
    localizacoesIds = acompanhamento.localizacoesIds || []
    dataProcessamentoDt.setValue(acompanhamento.dataProcessamento)
    
    if (acompanhamento.jForm) idsForms = configurarScripts({ jform: acompanhamento.jForm })
    
    const emailsProperties = ["email1", "email2", "email3"]
    
    const emailsLaboralArray = []
    for (const property of emailsProperties) {
      acompanhamento.sindicatosLaborais.map(sindicato => {
        let email = sindicato[property]

        if (email && email.length > 0) {
          emailsLaboralArray.push(email)
        }
      })
    }
    
    const emailsPatronalArray = []
    for (const property of emailsProperties) {
      acompanhamento.sindicatosPatronais.map(sindicato => {
        let email = sindicato[property]

        if (email && email.length > 0) {
          emailsPatronalArray.push(email)
        }
      })
    }
  
    emailsPatronal = emailsPatronalArray.join(", ")
    emailsLaboral = emailsLaboralArray.join(", ")
  }
}

async function upsert() {
  return  id ? await handleSubmitAtualizarAcompanhamento(id) : await handleSubmitIncluirAcompanhamento()
}

async function handleSubmitIncluirAcompanhamento() {
  sindicatosLaboraisIds = sindicatosLaboraisIds.filter(id => id != 0)
  sindicatosPatronaisIds = sindicatosPatronaisIds.filter(id => id != 0)

  const assuntoValue = assuntosSelect.getValue()
  const assuntosIds = assuntoValue.map(ass => parseInt(ass))

  const etiquetaValue = etiquetasSelect.getValue()
  const etiquetasIds = etiquetaValue.map(etq => parseInt(etq))

  return await incluirAcompanhamento({
    tipoDocumentoId: $("#tipodoc-input").val(),
    statusId: parseInt(statusSelect.getValue()),
    faseId: parseInt(fasesCadastroSelect.getValue()),
    dataBase: $("#db-input").val() + "/" + $("#dbano-input").val(),
    dataInicial: $("#dataini-input").val(),
    dataFinal: $("#datafim-input").val().length > 0 ? $("#datafim-input").val() : null,
    anotacoes: $("#anotacoes-input").val(),
    usuarioResponsavelId: $("#resp-input").val(),
    cnaesIds,
    sindicatosLaboraisIds: sindicatosLaboraisIds.map(sId => parseInt(sId)),
    sindicatosPatronaisIds: sindicatosPatronaisIds.map(sId => parseInt(sId)),
    gruposEconomicosIds,
    empresasIds,
    localizacoesIds,
    dataProcessamento: dataProcessamentoDt.getValue(),
    assuntosIds,
    etiquetasIds,
    validadeFinal: $("#validade-final").val()
  })
}

async function handleSubmitAtualizarAcompanhamento(id) {
  sindicatosLaboraisIds = sindicatosLaboraisIds.filter(id => id != 0)
  sindicatosPatronaisIds = sindicatosPatronaisIds.filter(id => id != 0)

  let faseId = parseInt(fasesCadastroSelect.getValue())
  if (isNaN(faseId)) {
    faseId = acompanhamentoSelecionado.faseId
  }

  let statusId = parseInt(statusSelect.getValue())
  if (isNaN(statusId)) {
    statusId = acompanhamentoSelecionado.statusId
  }

  let usuarioResponsavelId = parseInt(responsavelCadastroSelect.getValue())
  if (isNaN(usuarioResponsavelId)) {
    usuarioResponsavelId = acompanhamentoSelecionado.usuarioResponsavelId
  }

  const assuntoValue = assuntosSelect.getValue()
  const assuntosIds = assuntoValue.map(ass => parseInt(ass))

  const etiquetaValue = etiquetasSelect.getValue()
  const etiquetasIds = etiquetaValue.map(etq => parseInt(etq))

  return await atualizarAcompanhamento({
    id,
    tipoDocumentoId: $("#tipodoc-input").val(),
    statusId,
    faseId,
    dataBase: $("#db-input").val() + "/" + $("#dbano-input").val(),
    dataInicial: $("#dataini-input").val(),
    dataFinal: $("#datafim-input").val().length > 0 ? $("#datafim-input").val() : null,
    anotacoes: $("#anotacoes-input").val(),
    usuarioResponsavelId,
    cnaesIds,
    sindicatosLaboraisIds: sindicatosLaboraisIds.map(sId => parseInt(sId)),
    sindicatosPatronaisIds: sindicatosPatronaisIds.map(sId => parseInt(sId)),
    gruposEconomicosIds,
    empresasIds,
    localizacoesIds,
    observacoesGerais: $("#client-report").val(),
    dataProcessamento: dataProcessamentoDt.getValue(),
    assuntosIds,
    etiquetasIds,
    validadeFinal: $("#validade-final").val()
  })
}

function handleToggleScriptButton({ state }) {
  if (state == true && openedScript == true) return

  if (state == false && openedScript == false) return

  if (state == true && openedScript == false) {
    openedScript = true
  }

  if (state == false && openedScript == true) {
    openedScript = false
  }

  $("#scriptPanel").toggle("slow")
}

async function handleClickAdicionarScript() {
  let ids = idsForms.split(" ")
  ids.shift()

  let respostas = []

  ids.forEach((id) => {
    if (id) {
      let val = $(`#${id}`).val()

      if (Array.isArray(val)) {
        if (val.length > 0) {
          val = val.join(', ')
        } else {
          val = ''
        }
      }

      if ($(`#${id}`).prop('type') == 'radio') {
        val = $(`#${id}`).is(':checked') ? 'Sim' : 'Não'
      }

      return respostas.push(val)
    }

    respostas.push('')
  })

  let faseId = parseInt()
  if (isNaN(faseId)) {
    faseId = acompanhamentoSelecionado.faseId
  }

  let statusId = parseInt(statusSelect.getValue())
  if (isNaN(statusId)) {
    statusId = acompanhamentoSelecionado.statusId
  }

  await adicionarScript({
    id,
    acompanhamentoSelecionado,
    faseId,
    statusId,
    respostas,
    acompanhamentosCctsIds: acompanhamentosIds
  })

  idsForms = ''
  handleToggleScriptButton({ state: false })
}

async function addMultiAcompanhamento() {
  const selecionados = $('input[name="futuras-selecionados"]:checked')

  const jsonDataCollection = selecionados.map((_, input) => {
      const jsonData = $(input).attr("data-jsondata")
      const json = JSON.parse(jsonData)

      return `${json.sindEmp}-${json.sindPatr}-${json.database}-${json.cnaeId}-${json.respId}`
    }).get()

  const inputsStr = jsonDataCollection.join(" ")

  let inputs = inputsStr.split(" ")
  inputs.forEach(async (input) => {
    if (input.length > 0) {
      let dados = input.split("-")
      let datas = getFirstDaysOfMonthAndPrev(converterIniciaisMeses(dados[2]))

      const requestData = {
        module: "acompanhamento_db",
        action: "addAcompanhamento",
        "status-input": $("#status-futuras-input").val(),
        "fase-input": $("#fases-futuras-input").val(),
        "tipodoc-input": $("#tipodoc-fut-input").val(),
        "db-input": dados[2] + "",
        "dataini-input": datas[1],
        "datafim-input": datas[0],
        "emp-input": dados[1] + "",
        "resp-input": dados[4] + "",
        "cnae-input": dados[3] + "",
        "sind-input": dados[0] + ""
      }

      const result = await acompanhamentoCctService.postLegado(requestData)

      if (result.isFailure()) {
        NotificationService.error({ title: "Erro", message: result.error })

        return result
      }

      return Result.success()
    }
  })

  NotificationService.success({ title: "Sucesso", message: "Processado com sucesso!" })

  acompanhamentoCctFuturoTb.reload()

  function getFirstDaysOfMonthAndPrev(monthName) {
    var monthIndex = new Date(Date.parse(monthName + " 1, 2000")).getMonth()
    var year = new Date().getFullYear()
    var thisMonthDate = new Date(year, monthIndex, 1)
    var prevMonthIndex = monthIndex - 1
    var prevYear = year
    if (prevMonthIndex < 0) {
      prevMonthIndex = 11 // December
      prevYear = year - 1 // previous year
    }
    var prevMonthDate = new Date(prevYear, prevMonthIndex, 1)
    var yearString = thisMonthDate.getFullYear().toString()
    var thisMonthString = ("0" + (thisMonthDate.getMonth() + 1)).slice(-2) // add leading zero
    var prevMonthString = ("0" + (prevMonthDate.getMonth() + 1)).slice(-2) // add leading zero
    var thisMonthDayString = ("0" + thisMonthDate.getDate()).slice(-2) // add leading zero
    var prevMonthDayString = ("0" + prevMonthDate.getDate()).slice(-2) // add leading zero
    var thisMonthDateString =
      yearString + "-" + thisMonthString + "-" + thisMonthDayString
    var prevMonthDateString =
      prevYear + "-" + prevMonthString + "-" + prevMonthDayString
    return [thisMonthDateString, prevMonthDateString]
  }

  function converterIniciaisMeses(mes) {
    const meses = {
      JAN: "JAN",
      FEV: "FEB",
      MAR: "MAR",
      ABR: "APR",
      MAI: "MAY",
      JUN: "JUN",
      JUL: "JUL",
      AGO: "AUG",
      SET: "SEP",
      OUT: "OCT",
      NOV: "NOV",
      DEZ: "DEC",
    }
  
    return meses[mes] || "INVÁLIDO"
  }
}

async function dataTableLocalizacao() {
  if (localizacaoTb) {
    return localizacaoTb.reload()
  }

  localizacaoTb = new DataTableWrapper("#localizacaotb", {
    ajax: async (requestData) => {
      if(sindicatosPatronaisIds && sindicatosPatronaisIds.length > 0) requestData.sindicatosPatronaisId = sindicatosPatronaisIds
      requestData.sindicatosLaboraisIds = sindicatosLaboraisIds
      return await baseTerritorialSindicatoLaboralService.obterDatatableLocalizacoes(requestData)
    },
    columns: [
      { title: "", data: 'id', width: 0, orderable: false },
      { title: "Sigla", data: "sigla" },
      { title: "Uf", data: "uf" },
      { title: "Municipio", data: "municipio" },
      { title: "Pais", data: "pais" },
    ],
    rowCallback: function (row, data) {
      const checkbox = $("<input>")
        .attr("type", "checkbox")
        .attr("data-id", data?.id)

      if (localizacoesIds.find((id) => id.toString() === data?.id.toString())) {
        checkbox.attr("checked", true)
      }

      checkbox.on("change", function (event) {
        const id = $(this).attr("data-id")

        if (event.target.checked) return localizacoesIds.push(parseInt(id))

        localizacoesIds = localizacoesIds.filter((lastIds) => parseInt(lastIds) !== parseInt(id))
      })

      $("td:eq(0)", row).html(checkbox)
    },
  })

  await localizacaoTb.initialize()
}

async function dataTableGrupoEconomico() {
  if (grupoEconomicoTb) {
    grupoEconomicoTb.reload()

    return grupoEconomicoTb
  }

  grupoEconomicoTb = new DataTableWrapper("#grupoEconomicoTb", {
    ajax: async (requestData) => await grupoEconomicoService.obterDataTable(requestData),
    columns: [
      { title: "", data: "id" },
      { title: "Nome", data: "nome" },
    ],
    rowCallback: function (row, data) {
      const checkbox = $("<input>")
        .attr("type", "checkbox")
        .attr("data-id", data?.id)

      if (gruposEconomicosIds.find((id) => parseInt(id) === parseInt(data?.id))) checkbox.attr("checked", true)

      checkbox.on("change", function (event) {
        const id = $(this).attr("data-id")

        if (event.target.checked) return gruposEconomicosIds.push(parseInt(id))

        gruposEconomicosIds = gruposEconomicosIds.filter((lastIds) => parseInt(lastIds) !== parseInt(id))
      })

      $("td:eq(0)", row).html(checkbox)
    },
  })

  await grupoEconomicoTb.initialize()
}

async function dataTableFuturas() {
  if (acompanhamentoCctFuturoTb) {
    return acompanhamentoCctFuturoTb.reload()
  }

  acompanhamentoCctFuturoTb = new DataTableWrapper("#futurastb", {
    ajax: async (requestData) =>
      await acompanhamentoCctService.obterFuturasDatatable(requestData),
    columns: [
      { data: null, orderable: false, width: "0px" },
      { title: "Sindicato Laboral", data: "siglaSinde" },
      { title: "Sindicato Patronal", data: "siglaSp" },
      { title: "Atividade Econômica", data: "descricaoSubClasse" },
      { title: "Data Base", data: "dataneg" },
      { title: "Início", data: "dataIni" },
      { title: "Responsável Identificado", data: "responsavel" },
    ],
    rowCallback: function (row, data, index) {
      const json = {
        sindPatr: data.sindPatronalIdSindp,
        sindEmp: data.sindEmpregadosIdSinde,
        database: data.dataneg,
        cnaeId: data.classeCnaeIdClasseCnae,
        respId: data.responsavelId,
      }

      let checkbox = $("<input>")
        .attr("type", "checkbox")
        .attr("data-id", index)
        .attr("name", "futuras-selecionados")
        .attr("data-jsondata", JSON.stringify(json))
        .val(data.id)

      $("td:eq(0)", row).html(checkbox)
    },
  })

  await acompanhamentoCctFuturoTb.initialize()
}

async function datatableCnae() {
  if (cnaeTb) {
    return cnaeTb.reload()
  }

  cnaeTb = new DataTableWrapper("#cnaetb", {
    ajax: async (requestData) => {
      if(sindicatosPatronaisIds && sindicatosPatronaisIds.length > 0) requestData.sindicatosPatronaisId = sindicatosPatronaisIds
      requestData.sindicatosLaboraisId = sindicatosLaboraisIds
      requestData.columns = ["divisao", "descricao", "subclasse", "categoria"].join(",")
      return await cnaeService.obterDatatable(requestData)
    },
    columns: [
      { title: "", data: null, width: 0, orderable: false },
      { title: "Divisão", data: "divisao" },
      { title: "Descrição", data: "descricao" },
      { title: "SubClasse", data: "subclasse" },
      { title: "Categorias", data: "categoria" },
    ],
    rowCallback: function (row, data) {
      const checkbox = $("<input>")
        .attr("type", "checkbox")
        .attr("data-id", data?.id)

      if (cnaesIds.find((id) => id.toString() === data?.id.toString())) {
        checkbox.attr("checked", true)
      }

      checkbox.on("change", function (event) {
        const id = $(this).attr("data-id")

        if (event.target.checked) return cnaesIds.push(id.toString())

        cnaesIds = cnaesIds.filter((lastIds) => lastIds.toString() !== id.toString())
      })

      $("td:eq(0)", row).html(checkbox)
    },
  })

  await cnaeTb.initialize()
}

async function dataTableEmresa() {
  if (empresaTb) {
    return empresaTb.reload()
  }

  empresaTb = new DataTableWrapper("#empresatb", {
    ajax: async (requestData) =>
      await matrizService.obterDataTable(requestData),
    columns: [
      { title: "", data: "id" },
      { title: "Nome", data: "nome" },
      { title: "Uf", data: "uf" },
      { title: "Cnpj", data: "cnpj" },
    ],
    rowCallback: function (row, data) {
      const checkbox = $("<input>")
        .attr("type", "checkbox")
        .attr("data-id", data?.id)

      if (empresasIds.find((id) => parseInt(id) === parseInt(data?.id))) {
        checkbox.attr("checked", true)
      }

      checkbox.on("change", function (event) {
        const id = $(this).attr("data-id")

        if (event.target.checked) {
          return empresasIds.push(parseInt(id))
        }

        empresasIds = empresasIds.filter(
          (lastIds) => parseInt(lastIds) !== parseInt(id)
        )
      })

      $("td:eq(0)", row).html(checkbox)
    },
  })

  await empresaTb.initialize()
}

async function dataTableAcompanhamento() {
  acompanhamentoCctTb = new DataTableWrapper("#acompanhamentoccttb", {
    ajax: async (requestData) => await acompanhamentoCctService.obterDatatable(requestData),
    columns: [
      { data: null, orderable: false, width: "0px" },
      { title: "", data: "id" },
      { title: "Id", data: "id" },
      { title: "Fase", data: "fase" },
      { title: "Responsável", data: "nomeUsuario" },
      { title: "Sind. Laboral", data: "siglaSindEmpregado" },
      { title: "UF Laboral", data: "ufSindEmpregado" },
      { title: "Sind. Patronal", data: "siglaSindPatronal" },
      { title: "Data-base", data: "dataBase" },
      { title: "Etiquetas", data: "etiquetas" },
      {
        title: "Prioridade",
        data: "status",
        render: (data) => {
          return data.substring(2)
        },
      },
      {
        title: "Próxima Ligação",
        data: "proxLigacao",
        render: function (data) {
          if (!data) {
            return ""
          }

          return moment(data).format("DD/MM/YYYY")
        },
      },
      {
        title: "Última Atualização",
        data: "ultimaAtualizacao",
        render: function (data) {
          if (!data) {
            return ""
          }

          return moment(data).format("DD/MM/YYYY")
        },
      },
      { title: "Nome do Documento", data: "nomeDocumento" }
    ],
    order: [[11, "asc"]],

    rowCallback: function (row, data) {
      const editIcon = $("<i>").addClass("fa fa-file-text")

      let editButton = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(editIcon)

      editButton.on("click", async function () {
        const id = $(this).attr("data-id")

        $("#edicaoFields").show()
        $("#id-input").val(id)
        $("#novoAcompanhamentoBtn").trigger("click")
      })

      $("td:eq(0)", row).html(editButton)

      const checkbox = $("<input>")
        .attr("type", "checkbox")
        .attr("data-id", data?.id)

      if (acompanhamentosIds.find((id) => parseInt(id) === parseInt(data?.id))) {
        checkbox.attr("checked", true)
      }

      checkbox.on("change", function (event) {
        const id = $(this).attr("data-id")

        if (event.target.checked) {
          return acompanhamentosIds.push(parseInt(id))
        }

        acompanhamentosIds = acompanhamentosIds.filter((lastIds) => parseInt(lastIds) !== parseInt(id))
      })

      $("td:eq(1)", row).html(checkbox)
    },
  })

  await acompanhamentoCctTb.initialize()

  return acompanhamentoCctTb
}

function limparFormulario() {
  $("#formAdd").trigger("reset")
  tiposDocsCadastroSelect.clear()
  fasesCadastroSelect.clear()
  sindLaboralCadastroSelect.clear()
  sindPatronalCadastroSelect.clear()
  responsavelCadastroSelect.clear()
  fasesAgendamentoSelect.clear()
  tiposDocsAgendamentoSelect.clear()
  sindicatoLaboralFiltroSelect.clear()
  sindicatoPatronalFiltroSelect.clear()
  statusSelect.clear()
  assuntosSelect.clear()
  etiquetasSelect.clear()

  emailsLaboral = null
  emailsPatronal = null
  cnaesIds = []
  sindicatosLaboraisIds = []
  sindicatosPatronaisIds = []

  sindPatronaisSelecionadosFilter = []
  sindLaboraisSelecionadosFilter = []
  sindicatosLaboraisIds = []
  sindicatosPatronaisIds = []
  empresasIds = []
  gruposEconomicosIds = []
  localizacoesIds = []

  $("#id-input").val("")
  $("#anotacoes-input").val("")

  limparFormularioEmailSindicato()
  limparFormularioEmailCliente()
}
