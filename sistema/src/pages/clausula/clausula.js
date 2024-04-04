// Libs
import JQuery from 'jquery'
import $ from 'jquery'

// Css libs
import 'datatables.net-bs5/css/dataTables.bootstrap5.css'
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap'

// Core
import { ApiService, AuthService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js'

// Services
import {
  ClausulaGeralService,
  InformacaoAdicionalService,
  UsuarioAdmService,
  DocSindService,
  EstruturaClausulaService,
  SinonimosService
} from '../../js/services'

// Core
import { closeModal, renderizarModal } from '../../js/utils/modals/modal-wrapper.js'
import SelectWrapper from '../../js/utils/selects/select-wrapper.js'
import NotificationService from '../../js/utils/notifications/notification.service.js'
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js'
import { stringButton, stringI, stringTbody, stringTd, stringTh, stringThead, stringTr } from '../../js/utils/components/string-elements'
import { configOpcaoAdicional, stringSearchOpcaoAdicional } from '../../js/utils/components/informacao-adicional'
import { Generator } from '../../js/utils/generator/index.js'
import Result from '../../js/core/result.js'
import DatepickerWrapper from '../../js/utils/datepicker/datepicker-wrapper.js'
import { a, button, i, input } from '../../js/utils/components/elements'
import { convertTableToLines } from '../../js/utils/components/informacao-adicional/convert-table-to-lines.js'

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import {
  gerarRelatorio,
  obterGrupoEmpresasPorDocumento,
  obterSinonimos,
  criarInformacaoAdicionalGrupo,
  removerInformacaoAdicional,
  obterInformacoesAdicionais,
  obterClausulaPorId,
  liberar,
  aprovarClausula,
  enviarEmail,
  gerarResumoClausula,
  obterGerarResumoPermissao,
  atualizarResumo
} from '../../js/modules'
import DateFormatter from '../../js/utils/date/date-formatter.js'

const apiService = new ApiService()
const apiLegadoService = new ApiLegadoService()
const clausulaService = new ClausulaGeralService(apiService, apiLegadoService)
const docSindService = new DocSindService(apiService)
const estruturaClausulaService = new EstruturaClausulaService(apiService)
const informacaoAdicionalService = new InformacaoAdicionalService(apiService)
const usuarioAdmSerivce = new UsuarioAdmService(apiService)
const sinonimosService = new SinonimosService(apiService)
const usuarioAdmService = new UsuarioAdmService(apiService)

// Selects
let documentoSindicalSelect = null
let estruturaClausulaSelect = null
let sinonimoSelect = null

let dataInicialDt = null
let dataFinalDt = null

// Tb
let documentosTb = null
let docsAprovadosTb = null
let clausulasTb = null
let emailsTb = null

let informacoesAdiconaisOpcoes = []
let informacoesAdicionaisCampos = []
let permissoesInclusaoDeClausulas = null;

let tableFormAdicional = []
let informacoesAdicionaisDados = []
let emailsIds = []

let grupoInfoAdSelecionado = ''
let documentoSelecionado = null
let documentoSelecionadoId = 0
let clausulaSelecionada = null
let isCarregarPorId = false

JQuery(async function () {
  new Menu()

  await AuthService.initialize()

  const permissoesUsuario = (await usuarioAdmSerivce.obterPermissoes()).value

  if (permissoesUsuario instanceof Array) {
    [permissoesInclusaoDeClausulas] = permissoesUsuario.filter(permissao => permissao.modulo_id == '37')
    if (permissoesInclusaoDeClausulas.criar != '1') {
      $("#clausulaBtn")
        .addClass('disabled')
        .attr('disabled', true)
        .removeAttr('id')
        .removeAttr('data-target')
        .removeAttr('data-toggle')
      $("#documentosAprovadosBtn")
        .addClass('disabled')
        .attr('disabled', true)
        .removeAttr('id')
        .removeAttr('data-target')
        .removeAttr('data-toggle')
    }
  }

  configurarModal()
  configurarInputs()
  configurarFormularioModalClausula()
  await carregarDatatable()
})

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn')

  const clausulaModalHidden = document.getElementById('clausulaModalHidden')
  const clausulaModalContent = document.getElementById('clausulaModalContent')

  const documentosAprovadosModalHidden = document.getElementById('documentosAprovadosModalHidden')
  const documentosAprovadosModalContent = document.getElementById('documentosAprovadosModalContent')

  const documentoClausulaModalHidden = document.getElementById('documentoClausulaModalHidden')
  const documentoClausulaModalContent = document.getElementById('documentoClausulaModalContent')

  const gruposEmpresasModalHidden = document.getElementById("gruposEmpresasModalHidden")
  const gruposEmpresasContent = document.getElementById("gruposEmpresasContent")

  const resumoModalHidden = document.getElementById("resumoModalHidden")
  const resumoModalContent = document.getElementById("resumoModalContent")

  const emailsAprovadosModalHidden = document.getElementById('emailsAprovadosModalHidden')
  const emailsAprovadosModalContent = document.getElementById('emailsAprovadosModalContent')

  const modalsConfig = [
    {
      id: 'clausulaModal',
      modal_hidden: clausulaModalHidden,
      content: clausulaModalContent,
      btnsConfigs: [
        {
          id: 'btn_add_clausula',
          onClick: async (_, modalContainer) => {
            const result = await upsert(clausulaSelecionada ? clausulaSelecionada.id : null)

            if (result.isSuccess()) {
              closeModal(modalContainer)
              
              setTimeout(() => {
                $('#clausulasDocumentoBtn').trigger('click')
              }, 500)
            }
          }
        }
      ],
      onOpen: async () => {
        if (clausulaSelecionada) {
          $('#voltar_clausulas_documento_btn').show()
          $('#btn_aprovar_clausula').show()

          clausulaSelecionada = await obterClausulaPorId(clausulaSelecionada)
          await configurarInformacoes()

          $('#texto').prop('disabled', true)
        }
      },
      onClose: () => {
        clausulaSelecionada = null
        limparFormulario()
        $('#btn_aprovar_clausula').hide()
        $('#voltar_clausulas_documento_btn').hide()
        $('#texto').prop('disabled', false)
      },
      styles: {
        container: {
          paddingRight: '30px',
          paddingLeft: '30px'
        },
        modal: {
          maxWidth: '95%',
          width: '100%'
        }
      }
    },
    {
      id: 'documentosAprovadosModal',
      modal_hidden: documentosAprovadosModalHidden,
      content: documentosAprovadosModalContent,
      btnsConfigs: [],
      onOpen: async () => await carregarDatatableDocumentosAprovados(),
      onClose: () => null,
    },
    {
      id: 'clausulasDocumentoModal',
      modal_hidden: documentoClausulaModalHidden,
      content: documentoClausulaModalContent,
      btnsConfigs: [],
      onOpen: async () => {
        const result = await docSindService.obterPorId(documentoSelecionadoId)

        if (result.isFailure()) {
          return NotificationService.error({ title: 'Erro ao buscar documento', message: result.error })
        }

        documentoSelecionado = result.value

        await carregarDatatableClausualas()

        const gerarResumoPermissao = await obterGerarResumoPermissao(documentoSelecionadoId)
        gerarResumoPermissao ? $("#btn_gerar_resumo_clausula").prop('disabled', false) : $("#btn_gerar_resumo_clausula").prop('disabled', true)
      },
      onClose: () => {
        $("#btn_gerar_resumo_clausula").prop('disabled', true)
      },
    },
    {
      id: "gruposEmpresas",
      modal_hidden: gruposEmpresasModalHidden,
      content: gruposEmpresasContent,
      btnsConfigs: [],
      onOpen: async () => {
        const gruposEmpresasDocumento = await obterGrupoEmpresasPorDocumento(documentoSelecionadoId)
        carregarConteudoGruposEmpresa(gruposEmpresasDocumento)
      },
      onClose: () => {
        $('#doc-info-grupos-economicos').val('')
        $('#doc-info-empresas').val('')
      },
    },
    {
      id: "resumoModal",
      modal_hidden: resumoModalHidden,
      content: resumoModalContent,
      btnsConfigs: [
        {
          id: 'btn_editar_resumo',
          onClick: async (_, modalContainer) => {
            const result = await atualizarResumo({
              id: clausulaSelecionada.id,
              texto: $('#texto_resumo').val(),
              documentoId: clausulaSelecionada.documento.id,
              estruturaId: clausulaSelecionada.idEstruturaClausula
            })

            if (result.isSuccess()) {
              closeModal(modalContainer)
              
              setTimeout(() => {
                $('#clausulasDocumentoBtn').trigger('click')
              }, 500)
            }
          }
        }
      ],
      onOpen: async () => {
        if (clausulaSelecionada) {
          $('#voltar_clausulas_documento_btn').show()
          $('#btn_aprovar_clausula').show()

          clausulaSelecionada = await obterClausulaPorId(clausulaSelecionada)
          await configurarInformacoesResumo()

          $('#texto_resumo').prop('disabled', true)
        }
      },
      onClose: () => {
        limparFormularioResumo()

        $('#texto_resumo').prop('disabled', false)
      },
    },
    {
      id: "emailsAprovadosModal",
      modal_hidden: emailsAprovadosModalHidden,
      content: emailsAprovadosModalContent,
      btnsConfigs: [],
      onOpen: async () => await handleOpenEnviarEmailsAprovadosModal(),
      onClose: () => handleCloseEnviarEmailsAprovadosModal()
    }
  ]

  renderizarModal(pageCtn, modalsConfig);
}

function configurarInputs() {
  $('#btn_extrair_relatorio').on('click', async () => await gerarRelatorio({ documentoId: documentoSelecionadoId }))
  $('#btn_aprovar_clausula').on('click', async () =>  {
    await aprovarClausula(clausulaSelecionada.id)
    await documentosTb.reload()
  })

  $("#btn_gerar_resumo_clausula").on('click', async () => await gerarResumoClausula(documentoSelecionadoId))
  
  $('#enviar_email_btn').on('click', async () => await enviarEmail(documentoSelecionadoId))
  $('#enviar_email_selecionados_btn').on('click', () => $('#emails_aprovados_modal_btn').trigger('click'))
  $('#enviar_emails_aprovados_btn').on('click', async () => await handleClickEnviarEmailsAprovados())

  $("#selecionar_todos_documentos_sindicais_btn").on("click", (event) => {
		if (event.currentTarget.checked) {
			$('.emailId').prop('checked', true)
			$('.emailId').trigger('change')
		} else {
			$('.emailId').prop('checked', false)
			$('.emailId').trigger('change')
		}
	})
}

async function carregarDatatable() {
  documentosTb = new DataTableWrapper('#documentosTb', {
    ajax: async (requestData) => await clausulaService.obterDatatable(requestData),
    columns: [
      { "data": 'id', title: '' },
      { "data": 'id', title: 'ID Documento Sindical' },
      { "data": 'nome', title: 'Nome Documento' },
      { "data": "processados", title: 'Processadas' },
      { "data": "naoProcessados", title: 'Não Processadas' },
      { "data": "aprovados", title: 'Aprovados' },
      { "data": "naoAprovados", title: 'Não Aprovados' },
      { "data": "dataScrap", title: 'Data Scrap' },
      { "data": "dataSla", title: 'Data SLA' },
      { "data": "diasEmProcessamento", title: 'Dias em Processamento' }
    ],
    rowCallback: function (row, data) {
      const id = data?.id

      let btn = a({
        className: 'btn-update',
        content: i({
          className: 'fa fa-file-text'
        })
      }).attr("data-id", id)

      btn.on("click", function () {
        documentoSelecionadoId = id
        $('#clausulasDocumentoBtn').trigger('click')
      })

      $("td:eq(0)", row).html(btn)

      let link = a({
        href: '#',
        content: id
      }).attr("data-id", id)

      link.on("click", function () {
        const id = $(this).attr("data-id");

        documentoSelecionadoId = id

        $("#gruposEmpresasModalBtn").trigger("click");
      });

      $("td:eq(1)", row).html(link);
    },
    columnDefs: [{
      targets: 7,
      render: (data) => DataTableWrapper.formatDate(data)
    }, {
      targets: 8,
      render: (data) => DataTableWrapper.formatDate(data)
    },
    {
      targets: "_all",
      defaultContent: ""
    }]
  })

  await documentosTb.initialize()
}

async function carregarDatatableClausualas() {
  if (clausulasTb) return await clausulasTb.reload()

  clausulasTb = new DataTableWrapper('#clausulasTb', {
    ajax: async (request) => await clausulaService.obterDatatableClausulasPorDocumento({ id: documentoSelecionadoId, request }),
    columns: [
      { "data": 'id', title: 'Visualizar' },
      { "data": 'idDocumentoSindical', title: 'Id Doc' },
      { "data": "nomeGrupo", title: 'Grupo Cláusula' },
      { "data": "nomeClausula", title: 'Nome Cláusula' },
      { "data": "numero", title: 'Número Cláusula' },
      { "data": "nomeResponsavelProcessamento", title: 'Processado Por' },
      { "data": "dataAprovacao", title: 'Data Aprovação' },
      { "data": "nomeAprovador", title: 'Aprovado Por' },
      { "data": "resumivel", title: 'Resumível' },
      { "data": "resumoStatus", title: 'Resumo Status' },
      { "data": "id", title: 'Ver Resumo' }
    ],
    columnDefs: [{
      targets: 6,
      render: (data) => DataTableWrapper.formatDate(data)
    },
    {
      targets: "_all",
      defaultContent: ""
    }],
    rowCallback: function (row, data) {
      let btn = button({ className: 'btn-update', content: i({ className: 'fa fa-file-text' }) })
      btn.on("click", function () {
        clausulaSelecionada = data.id
        closeModal({ id: 'clausulasDocumentoModal' })

        setTimeout(() => {
          $('#clausulaBtn').trigger('click')
        }, 500)
      })

      $("td:eq(0)", row).html(btn)
      
      let btnResumo = button({
        className: 'btn-update',
        content: i({
          className: 'fa fa-file-text'
        })
      })

      btnResumo.on("click", function () {
        clausulaSelecionada = data.id
        closeModal({ id: 'clausulasDocumentoModal' })

        setTimeout(() => {
          $('#resumoModalBtn').trigger('click')
        }, 500)
      })
      $("td:eq(10)", row).html(btnResumo)
    },
  })

  await clausulasTb.initialize()
}

async function carregarEmailsDatatable() {
  if (emailsTb) return await emailsTb.reload()
    
  emailsTb = new DataTableWrapper('#emailsTb', {
    columns: [
      { data: "id" },
      { data: "nome", title: "Nome"},
      { data: "email", title: "E-mail" },
      { data: "nivel", title: "Nível" }
    ],
    ajax: async (requestData) => {
      requestData.modulo = 6
      return await usuarioAdmService.obterDatatablePorDocumento(documentoSelecionadoId, requestData)
    },
    rowCallback: function (row, data) {
      const id = data.id
      const checkbox = input({ type: 'checkbox', className: 'form-check-input c chkitem' })
        .attr('data-id', id)
        .addClass('emailId')

      if (emailsIds.find(cnae => cnae == id)) {
        checkbox.prop('checked', true)
      }

      checkbox.on('change', (el) => {
        const checked = el.target.checked

        if (checked) {
          emailsIds.push(id)
        } else {
          emailsIds = emailsIds.filter(item => item != id)
        }
      })

      $("td:eq(0)", row).html(checkbox)
    },
  })

  await emailsTb.initialize()
}

function configurarFormularioModalClausula() {
  documentoSindicalSelect = new SelectWrapper('#documento_sindical', {
    onOrdenable: (data) => data.sort((a, b) => a.id - b.id),
    onOpened: async () => await docSindService.obterSelect(),
    onSelected: async ({ id }) => {
      if (!id) return
      const result = await docSindService.obterPorId(id)

      if (result.isFailure()) {
        return NotificationService.error({
          title: 'Erro ao buscar documento',
          message: result.error
        })
      }

      documentoSelecionado = result.value

      dataInicialDt.setValue(documentoSelecionado.dataInicial)
      dataFinalDt.setValue(documentoSelecionado.dataFinal)
    },
    options: {
      allowEmpty: true
    }
  });
  estruturaClausulaSelect = new SelectWrapper('#lista_clausula', {
    onOpened: async () => (await estruturaClausulaService.obterSelect()).value,
    onSelected: async (value) => await changeClausula(value.id),
    options: {
      allowEmpty: true
    }
  });

  sinonimoSelect = new SelectWrapper('#sinonimo_select', {
    onOpened: async () => (await sinonimosService.obterSelect()).value,
    onSelected: async (value) => await obterSinonimos(value.id),
    options: { allowEmpty: true }
  })

  dataInicialDt = new DatepickerWrapper('#vigencia_inicial')
  dataFinalDt = new DatepickerWrapper('#vigencia_final')

  $('#table-grupo-add').hide()
  $("#aprovacao").hide()

  $('#btn_add_clausula').on('click', () => {
    $("#aprovacao").show()
  })

  $('#btn_aprovar_clausula').hide()

  $('#liberar_documento').on('click', async () => {
    await liberar({ dataLiberacaoClausulas: documentoSelecionado.dataLiberacaoClausulas, documentoSelecionadoId })
  })
  $('#liberar_documento_email').on('click', async () => {
    if (!documentoSelecionado.dataLiberacaoClausulas || documentoSelecionado.dataLiberacaoClausulas <= new Date(1950,1,1)) {
      NotificationService.error({title: "Cláusulas não liberadas."});
      return;
    }
    await enviarEmail({ documentoId: documentoSelecionadoId })
  });
  $('#editar_texto').on('click', () => $('#texto').prop('disabled', false))

  $('#editar_texto_resumo').on('click', () => $('#texto_resumo').prop('disabled', false))

  $('#add_info_grupo').on('click', () => {
    informacoesAdicionaisDados = []
    $('#table_grupo_body').append(createTableRow(informacoesAdicionaisCampos))
    initInputConfigs()

    $('#focus_input').trigger('focus')
  })

  $('#remove_info_grupo').on('click', () => {
    if (tableFormAdicional.length - 1 <= 0) return

    $(`#tb-row-${tableFormAdicional.length}`).remove()
    $('#table_grupo_body tr:last').remove()
    tableFormAdicional.pop()
  })

  $('#voltar_clausulas_documento_btn').hide()
  $('#voltar_clausulas_documento_btn').on('click', () => {
    setTimeout(() => {
      $('#clausulasDocumentoBtn').trigger('click')
    }, 400)
  })

  $('#voltar_resumo_clausulas_documento_btn').on('click', () => {
    setTimeout(() => {
      $('#clausulasDocumentoBtn').trigger('click')
    }, 400)
  })
}

async function configurarInformacoes() {
  const {
    documento,
    idEstruturaClausula,
    nome,
    numero,
    texto
  } = clausulaSelecionada

  isCarregarPorId = true

  if (clausulaSelecionada.documento) {
    dataInicialDt.setValue(documento.validadeInicial)
    dataFinalDt.setValue(documento.validadeFinal)

    documentoSindicalSelect.setCurrentValue({
      id: documento.id,
      description: documento.nome
    })
  }

  estruturaClausulaSelect.setCurrentValue({
    id: idEstruturaClausula,
    description: nome
  })

  if (clausulaSelecionada.sinonimo) {
    sinonimoSelect.setCurrentValue(clausulaSelecionada.sinonimo)
  }

  $('#numero').val(numero)
  $('#texto').val(texto)

  await changeClausula(idEstruturaClausula)

  $(".btn_informacao_adicional:first").trigger('click')
}

async function configurarInformacoesResumo() {
  const {
    documento,
    idEstruturaClausula,
    nome,
    numero,
    texto,
    textoResumido,
    sinonimo
  } = clausulaSelecionada

  isCarregarPorId = true

  if (clausulaSelecionada.documento) {
    $('#vigencia_inicial_resumo').val(DateFormatter.dayMonthYear(documento.validadeInicial))
    $('#vigencia_final_resumo').val(DateFormatter.dayMonthYear(documento.validadeInicial))
    $('#documento_sindical_resumo').val(`${documento.id} - ${documento.nome}`)
  }

  $('#nome_clausula_resumo').val(`${idEstruturaClausula} - ${nome}`)
  
  if (clausulaSelecionada.sinonimo) {
    const { id, description } = sinonimo

    $('#sinonimo_resumo').val(`${id} - ${description}`)
  }

  $('#numero_resumo').val(numero)
  $('#texto_original').val(texto)
  $('#texto_resumo').val(textoResumido)
}

async function changeClausula(id) {
  $('#infoAdicionalList').html('')
  $('#infoAdicionalList').html(stringTr({
    id: 'placeholder',
    style: 'color:#bbb;',
    content: stringTd({
      content: 'Selecione a classificação da cláusula e as informações desejadas.'
    })
  }))
  $("#infoAdicionalSelect").html('')
  $('#table_grupo').html('')
  $("#table-grupo-add").hide()
  tableFormAdicional = []

  if (!id) return

  const result = await informacaoAdicionalService.obterInformacoesAdicionaisPorClausula(id)

  const data = result.value

  $("#infoAdicionalSelect").html('')

  informacoesAdiconaisOpcoes = null
  informacoesAdiconaisOpcoes = data

  let infoAdicionais = informacoesAdiconaisOpcoes.map(info => criarInformacaoAdicionalGrupo(info))

  $("#infoAdicionalSelect").append(infoAdicionais)

  $('.btn_informacao_adicional').on('click', async (el) => {
    const id = el.target.attributes['data-id'].value

    const informacaoSelecioana = informacoesAdiconaisOpcoes.filter(infoItem => infoItem.id == id)[0]

    selectInformacaoAdicional(informacaoSelecioana)
    await handleDynamicTableForm(informacaoSelecioana)
  })
}

function selectInformacaoAdicional(informacaoSelecioana) {
  const { nomeTipoInformacao, id } = informacaoSelecioana

  $('.btn_informacao_adicional').prop('disabled', true)

  $('#placeholder').hide()

  grupoInfoAdSelecionado = id

  const dataId = id + nomeTipoInformacao

  const informacaoSelecionada = stringTr({
    id: dataId,
    className: 'infoGrupo',
    content: stringTd({
      content: nomeTipoInformacao
    }) + stringTd({}) + stringTd({
      style: 'height: 55px; display: flex; justify-content:center; align-items:center;',
      content: stringButton({
        style: 'color: red; border:none; background-color: transparent;',
        className: 'btn_remover_clausula_adicional',
        config: 'data-id="' + dataId + '"',
        content: stringI({
          className: 'fa fa-times',
          config: 'data-id="' + dataId + '"',
        })
      })
    })
  })

  $('#infoAdicionalList').append(informacaoSelecionada)

  $('.btn_remover_clausula_adicional').on('click', () => removerInformacaoAdicional(resetInformacaoAdicional))
}

function resetInformacaoAdicional() {
  tableFormAdicional = []

  grupoInfoAdSelecionado = ''

  $('#infoAdicionalList').html('')
  $('#infoAdicionalList').html(stringTr({
    id: 'placeholder',
    style: 'color:#bbb;',
    content: stringTd({
      content: 'Selecione a classificação da cláusula e as informações desejadas.'
    })
  }))
  $('.btn_informacao_adicional').prop('disabled', false)
  $('#placeholder').show()
  $('#table_grupo').html('')
  $("#table-grupo-add").hide()
}

async function carregarDatatableDocumentosAprovados() {
  if (docsAprovadosTb) return docsAprovadosTb.reload()

  docsAprovadosTb = new DataTableWrapper('#documentosAprovadosTb', {
    ajax: async (requestData) => await docSindService.obterDatatableAprovados(requestData),
    columns: [
      { "data": 'id', title: 'ID Documento' },
      { "data": 'nome', title: 'Nome Documento' },
      { "data": "dataAprovacao", title: 'Data Aprovação' },
      { "data": "dataSla", title: 'SLA Entrega' },
      { "data": "dataScrap", title: 'Data Scrap' },
      { "data": "scrap", title: 'Scrap Aprovado' },
      { "data": "nomeUsuarioResponsavel", title: 'Usuário Responsável' }
    ],
    columnDefs: [{
      targets: 2,
      render: (data) => DataTableWrapper.formatDate(data)
    }, {
      targets: 3,
      render: (data) => DataTableWrapper.formatDate(data)
    },
    {
      targets: "_all",
      defaultContent: ""
    }]
  })

  await docsAprovadosTb.initialize()
}

async function handleDynamicTableForm(informacaoSelecioana) {
  informacoesAdicionaisDados = null
  const result = await informacaoAdicionalService.obterCamposAdicionais(informacaoSelecioana.tipoInformacaoId, informacaoSelecioana.estruturaId)

  if (result.isFailure()) return

  const data = result.value

  if (clausulaSelecionada) {
    informacoesAdicionaisDados = await obterInformacoesAdicionais({ informacoesAdiconaisOpcoes, clausulaSelecionada })

    if (isCarregarPorId) {
      if (!informacoesAdicionaisDados || informacoesAdicionaisDados.length <= 0) {
        isCarregarPorId = false
        return resetInformacaoAdicional()
      }
    }

    isCarregarPorId = false
  }
  informacoesAdicionaisCampos = data

  $('#table_grupo').html('')

  $('#table_grupo').append(createTableContent(informacoesAdicionaisCampos))
  initInputConfigs()

  $("#table-grupo-add").show()
}

function initInputConfigs() {
  const dado = informacoesAdicionaisDados

  tableFormAdicional.map((row, r) => {
    row.content.map((item, c) => {
      let data = null

      if (dado) {
        data = dado.find(({ sequenciaLinha, sequenciaItem }) => sequenciaLinha.toString() == (r + 1).toString() && sequenciaItem.toString() == (c + 1).toString()) || null
        return configOpcaoAdicional(item, data)
      }

      configOpcaoAdicional(item)
    })
  })
  informacoesAdicionaisDados = null
}

function createTableContent(data) {
  let rows = ''
  let grupos = []

  if (informacoesAdicionaisDados) {
    informacoesAdicionaisDados.map(({ sequenciaLinha }) => {
      if (!grupos.includes(sequenciaLinha)) {
        grupos.push(sequenciaLinha)
      }
    })
  }

  const index = grupos.length != 0 ? grupos.length : 1
  for (let i = 0; i < index; i++) {
    const quantidade = i == 0 ? 0 : data.length * i;
    rows += createTableRow(data, quantidade)
  }

  return createTableHead(data) +
    stringTbody({
      id: 'table_grupo_body',
      children: rows
    })

  function createTableHead(data) {
    return stringThead({
      children: stringTr({
        content: data.map(({ tipoInformacaoNome }) => stringTh({
          className: 'infoGrupo',
          id: Generator.id(),
          content: tipoInformacaoNome
        }))
      })
    })
  }
}

function createTableRow(data, line = 0) {
  const rowId = Generator.id()
  const tableContent = []

  let row = ''

  row = stringTr({
    id: rowId,
    content: data.map(({ tipoDadadoId, combo, cdInformacaoId }, i) => {
      const { id, content } = stringSearchOpcaoAdicional({ input: tipoDadadoId })

      let codigoId = 0
      if (informacoesAdicionaisDados && informacoesAdicionaisDados.length > 0) {
        codigoId = informacoesAdicionaisDados[i + line].id
      }

      tableContent.push({
        id,
        codigoId,
        type: tipoDadadoId,
        codigo: cdInformacaoId,
        data: combo ?? null,
        input: null,
        command: {
          val: null
        }
      })

      return stringTd({ id: Generator.id(), content })
    })
  })

  tableFormAdicional.push({
    id: rowId,
    content: tableContent
  })

  return row
}

async function upsert(id) {
  return id ? await alterar(id) : await adicinarClausula();
}

async function adicinarClausula() {
  const sinonimo = parseInt(sinonimoSelect.getValue())
  
  if (!sinonimo) {
    return NotificationService.error({ title: 'Campo sinônimo em branco' });
  }

  const documentoId = parseInt(documentoSindicalSelect.getValue())

  const result = await clausulaService.incluir({
    texto: $("#texto").val(),
    estruturaId: parseInt(estruturaClausulaSelect.getValue()),
    documentoId,
    numero: parseInt($("#numero").val()),
    sinonimoId: sinonimo,
    informacoesAdicionais: tableFormAdicional && tableFormAdicional.length > 0 ? convertTableToLines(
      tableFormAdicional,
      {
        grupoId: parseInt(grupoInfoAdSelecionado),
        estruturaId: parseInt(estruturaClausulaSelect.getValue()),
        documentoId: parseInt(documentoId)
      }
    ) : []
  })

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao Cadastrar Clausula',
      message: result.error
    })

    return Result.failure(result.error)
  }

  NotificationService.success({
    title: 'Clausula adicionada com sucesso'
  })

  await documentosTb.reload()
  documentoSelecionadoId = documentoId

  $('#texto').prop('disabled', true)

  return Result.success()
}

async function alterar(id) {
  const sinonimo = parseInt(sinonimoSelect.getValue())
  
  if (!sinonimo) {
    return NotificationService.error({ title: 'Campo sinônimo em branco' });
  }

  const result = await clausulaService.editar({
    id,
    body: {
      texto: $("#texto").val(),
      estruturaId: parseInt(estruturaClausulaSelect.getValue()),
      documentoId: parseInt(documentoSindicalSelect.getValue()),
      numero: parseInt($("#numero").val()),
      sinonimoId: sinonimo,
      informacoesAdicionais: tableFormAdicional && tableFormAdicional.length > 0 ? convertTableToLines(
        tableFormAdicional,
        {
          grupoId: parseInt(grupoInfoAdSelecionado),
          estruturaId: parseInt(estruturaClausulaSelect.getValue()),
          documentoId: parseInt(documentoSindicalSelect.getValue())
        }
      ) : []
    }
  })

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao Editar Clausula',
      message: result.error
    })

    return Result.failure(result.error)
  }

  NotificationService.success({
    title: 'Clausula editada com sucesso'
  })

  await documentosTb.reload()
}

function limparFormulario() {
  $("#texto").val("")
  dataInicialDt.clear()
  dataFinalDt.clear()
  estruturaClausulaSelect.clear()
  documentoSindicalSelect.clear()
  sinonimoSelect.clear()
  $("#numero").val("")
  tableFormAdicional = []
  informacoesAdiconaisOpcoes = null

  $('#infoAdicionalList').html('')
  $('#infoAdicionalList').html(stringTr({
    id: 'placeholder',
    style: 'color:#bbb;',
    content: stringTd({
      content: 'Selecione a classificação da cláusula e as informações desejadas.'
    })
  }))
  $("#infoAdicionalSelect").empty()
}

function limparFormularioResumo() {  
  $('#vigencia_inicial_resumo').val('')
  $('#vigencia_final_resumo').val('')
  $('#documento_sindical_resumo').val('')
  $('#nome_clausula_resumo').val('')
  $('#sinonimo_resumo').val('')
  $('#numero_resumo').val('')
  $('#texto_original').val('')
  $('#texto_resumo').val('')

  clausulaSelecionada = null
}

function carregarConteudoGruposEmpresa(informacoesDocumento) {
  const empresas = []
  let gruposEconomicos = []
  const abrangencias = []
  const sindicatosLaborais = []
  const sindicatosPatronais = []

  const gruposEconomicosGerais = []

  informacoesDocumento.map(({ nomeGrupoEconomico, empresa, abrangencia: abrangenciaJson, sindicatosLaborais: sindicatosLaboraisJson, sindicatosPatronais: sindicatosPatronaisJson }) => {
    if (!empresas.includes(empresa)) {
      empresas.push(empresa)
    }

    if (!gruposEconomicosGerais.includes(nomeGrupoEconomico)) {
      gruposEconomicosGerais.push(nomeGrupoEconomico)
    }

    if (abrangenciaJson) {
      const abg = JSON.parse(abrangenciaJson)

      abg.map(({ Uf, Municipio }) => {
        const stringAbrangencia = `${Municipio}/${Uf}`

        if (!abrangencias.includes(stringAbrangencia)) {
          abrangencias.push(`${Municipio}/${Uf}`)
        }
      })
    }

    if (sindicatosLaboraisJson) {
      const sdcl = JSON.parse(sindicatosLaboraisJson)

      sdcl.map(({ sigla }) => {
        if (!sindicatosLaborais.includes(sigla)) {
          sindicatosLaborais.push(`${sigla}`)
        }
      })
    }

    if (sindicatosPatronaisJson) {
      const sdcp = JSON.parse(sindicatosPatronaisJson)

      sdcp.map(({ sigla }) => {
        if (!sindicatosPatronais.includes(sigla)) {
          sindicatosPatronais.push(`${sigla}`)
        }
      })
    }


    gruposEconomicos.push(nomeGrupoEconomico)
  })

  gruposEconomicos = gruposEconomicosGerais

  $('#doc-info-grupos-economicos').val(gruposEconomicos.join(', '))
  $('#doc-info-empresas').val(empresas.join(', '))
  $('#doc-info-abrangencia').val(abrangencias.join(', '))
  $('#doc-info-siglas-laborais').val(sindicatosLaborais.join(', '))
  $('#doc-info-siglas-patronais').val(sindicatosPatronais.join(', '))
}

async function handleOpenEnviarEmailsAprovadosModal() {
  await carregarEmailsDatatable()
}

async function handleCloseEnviarEmailsAprovadosModal() {
  emailsIds = []
  $('#selecionar_todos_documentos_sindicais_btn').prop('checked', false)
}

async function handleClickEnviarEmailsAprovados() {
  const usuariosIds = emailsIds.map(e => parseInt(e))
  const result = await enviarEmail(documentoSelecionadoId, usuariosIds)

  if (result.isSuccess()) {
    closeModal({ id: 'emailsAprovadosModal' })
  }
}