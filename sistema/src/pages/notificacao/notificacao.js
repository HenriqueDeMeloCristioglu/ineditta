import 'datatables.net-bs5/css/dataTables.bootstrap5.css'
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css'

import JQuery from 'jquery'
import $ from 'jquery'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap'
import 'datatables.net-responsive-bs5'

//Core
import { AuthService, ApiService, UserInfoService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js'

// Services
import {
  ComentarioService,
  EtiquetaService,
  TipoEtiquetaService,
  UsuarioAdmService
} from '../../js/services'

import { renderizarModal, closeModal } from '../../js/utils/modals/modal-wrapper.js'
import NotificationService from '../../js/utils/notifications/notification.service.js'
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js'
import Result from '../../js/core/result.js'
import SelectWrapper from '../../js/utils/selects/select-wrapper.js'
import DatepickerWrapper from '../../js/utils/datepicker/datepicker-wrapper.js'

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import { UsuarioNivel } from '../../js/application/usuarios/constants/usuario-nivel.js'
import { obterTipoComentarioSelect, obterTipoNotificacaoSelect, obterAssunto, usuarioDestinoSelect, obterBooleanSelect, obterTipoUsuarioDestinoSelect, getDescriptionTipoUsuarioDestino, getDescriptionTipoUsuarioNotificacao, BooleanType } from '../../js/utils/components/selects'
import { TipoComentario, TipoNotificacao, TipoUsuarioDestino } from '../../js/application/comentarios/constants'

const apiService = new ApiService()
const apiLegadoService = new ApiLegadoService()

const comentarioService = new ComentarioService(apiService, apiLegadoService)
const usuarioAdmService = new UsuarioAdmService(apiService, apiLegadoService)
const etiquetaService = new EtiquetaService(apiService)
const tipoEtiquetaService = new TipoEtiquetaService(apiService)

let usuario = null
let comentario = null

let notificacaoTb = null

let tipoComentarioSelect = null
let assuntoSelect = null
let tipoUsuarioDestinoSelect = null
let destinoSelect = null
let tipoNotificacaoSelect = null
let etiquetaSelect = null
let tipoEtiquetaSelect = null
let visivelSelect = null

let dataValidade = null

let permissoesUsuario = []

const MODULO_CLAUSULA_ID = 6
const MODULO_SINDICATO_ID = 15

JQuery(async function () {
  new Menu()

  await AuthService.initialize()

  await carregarUsuario()

  permissoesUsuario = (await usuarioAdmService.obterPermissoes())?.value ?? []

  configurarModal()

  configurarFormulario()

  carregarDatatable()
})

async function carregarUsuario() {
  const result = await usuarioAdmService.obterDadosPessoais()

  usuario = result.value
  return usuario
}

async function carregarDatatable() {
  notificacaoTb = new DataTableWrapper('#notificacaotb', {
    ajax: async (requestData) =>
      await comentarioService.obterDatatable(requestData),
    columns: [
      { "data": 'id', width: "0px" },
      { "data": "tipoComentario" },
      { "data": "tipoUsuarioDestino" },
      { "data": "tipoNotificacao" },
      { "data": "etiquetaNome" },
      { "data": "clausulaNome" },
      { "data": "sindicatoLaboral" },
      { "data": "sindicatoPatronal" },
      { "data": "dataFinal" },
      { "data": "usuarioNome" },
      { "data": "comentario" }
    ],
    columnDefs: [{
      targets: 8,
      render: (data) => DataTableWrapper.formatDate(data)
    },
    {
      targets: "_all",
      defaultContent: ""
    }],
    rowCallback: function (row, data) {
      const icon = $("<i>").addClass("fa fa-file-text")
      const button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon)

      button.on("click", function () {
        const id = $(this).attr("data-id")
        $('#id_note').val(id);
        $('#noficacaoModalBtn').trigger('click')
      });
      $("td:eq(0)", row).html(button)
    },
  });

  await notificacaoTb.initialize()
}

function configurarFormulario() {
  tipoComentarioSelect = new SelectWrapper('#tipo-com', {
    onOpened: async () => await obterTipoComentarioSelect({ showFilial: usuario.nivel === UsuarioNivel.Ineditta }),
    onSelected: ({ id }) => configurarTituloAssunto(id)
  })
  assuntoSelect = new SelectWrapper('#assunto', {
    onOpened: async (tipoComentarioId) => await obterAssunto(tipoComentarioId),
    parentId: '#tipo-com',
    sortable: true
  })
  tipoUsuarioDestinoSelect = new SelectWrapper('#tipo_usuario_destino', { onOpened: async () => await obterTipoUsuarioDestinoSelect() })
  destinoSelect = new SelectWrapper('#destino', { onOpened: async (tipoUsuarioId) => {
    configurarTituloTipoUsuario(tipoUsuarioId)
    return await usuarioDestinoSelect({ tipoUsuarioId, isIneditta: usuario.nivel == UsuarioNivel.Ineditta, grupoEconomicoId: usuario.grupoEconomicoId })
  }, parentId: '#tipo_usuario_destino', sortable: true })
  tipoNotificacaoSelect = new SelectWrapper('#tipo-note', {
    onOpened: async () => await obterTipoNotificacaoSelect(),
    onSelected: ({ id }) => {
      if (id == TipoNotificacao.Fixa) {
        dataValidade.clear()
        dataValidade.disable()
      } else {
        dataValidade.enable()
      }
    }
  })
  dataValidade = new DatepickerWrapper('#validade')
  tipoEtiquetaSelect = new SelectWrapper('#tipo-etiqueta', {
    onOpened: async (tipoComentarioId) => {
      const tipoEtiquetaNome = obterTipoEtiquetaPorTipoComentario(tipoComentarioId)

      return (await tipoEtiquetaService.obterSelect({ tipoEtiquetaNome })).value
    },
    parentId: '#tipo-com',
    options: {
      allowEmpty: true
    }
  })
  tipoEtiquetaSelect.enable()
  etiquetaSelect = new SelectWrapper('#etiqueta', {
    onOpened: async (tipoEtiquetaId) => (await etiquetaService.obterSelect({ tipoEtiquetaId })).value,
    parentId: '#tipo-etiqueta', sortable: true
  })
  visivelSelect = new SelectWrapper('#visivel', { onOpened: async () => await obterBooleanSelect() })

  carregarInformacoesUsuario()
}

function carregarInformacoesUsuario() {
  const nomeUsuario = UserInfoService.getFirstName() + ' ' + UserInfoService.getLastName()

  $('#usuario').val(nomeUsuario)
}

function configurarTituloAssunto(tipoComentarioId) {
  switch (parseFloat(tipoComentarioId)) {
    case TipoComentario.Clausula:
      return $('#assuntoTitulo').html('Assunto')
    case TipoComentario.SindicatoPatronal:
      return $('#assuntoTitulo').html('Sindicato')
    case TipoComentario.SindicatoLaboral:
      return $('#assuntoTitulo').html('Sindicato')
    case TipoComentario.Filial:
      return $('#assuntoTitulo').html('Filial')
    default:
      return $('#assuntoTitulo').html('Assunto')
  }
}

function configurarTituloTipoUsuario(tipoUsuarioId) {
  switch (parseFloat(tipoUsuarioId)) {
    case TipoUsuarioDestino.Grupo:
      return $('#campo_tipo').html('Grupo Econômico')
    case TipoUsuarioDestino.Matriz:
      return $('#campo_tipo').html('Empresa')
    case TipoUsuarioDestino.Unidade:
      return $('#campo_tipo').html('Estabelecimento')
    default:
      return $('#campo_tipo').html('--')
  }
}

function obterTipoEtiquetaPorTipoComentario(tipoComentarioId) {
  let tipoEtiquetaNome = ''

  switch (parseFloat(tipoComentarioId)) {
    case TipoComentario.Clausula:
      tipoEtiquetaNome = 'Cláusulas'
      break
    case TipoComentario.SindicatoLaboral:
      tipoEtiquetaNome = 'Sindicatos'
      break
    case TipoComentario.SindicatoPatronal:
      tipoEtiquetaNome = 'Sindicatos'
      break
    default:
      tipoEtiquetaNome = ''
      break
    }

    return tipoEtiquetaNome
}

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn')

  const noticacaoModalHidden = document.getElementById('noticacaoModalHidden')
  const noticacaoModalHiddenContent = document.getElementById('noticacaoModalHiddenContent')
  const noticacaoModalButtons = [
    {
      id: 'notificacaoCadastrarBtn',
      onClick: async (_, modalContainer) => {
        const result = await upsert()
        if (result.isSuccess()) {
          closeModal(modalContainer)
        }
      },
      data: null
    }
  ]

  const modalsConfig = [
    {
      id: 'notificacaoModal',
      modal_hidden: noticacaoModalHidden,
      content: noticacaoModalHiddenContent,
      btnsConfigs: noticacaoModalButtons,
      onOpen: async () => {
        const id = $('#id_note').val();
        if (id) {
          await obterPorId(id);
        }
      },
      onClose: () => limpar()
    },
  ];

  renderizarModal(pageCtn, modalsConfig);
}

async function upsert() {
  const podeComentarClausula = permissoesUsuario?.some(p => p.modulo_id == MODULO_CLAUSULA_ID && p.comentar == 1)
  const podeComentarSindicato = permissoesUsuario?.some(p => p.modulo_id == MODULO_SINDICATO_ID && p.comentar == 1)

  let notificacao = {
    title: '',
    message: ''
  }  

  if (tipoComentarioSelect?.getValue() == TipoComentario.Clausula && !podeComentarClausula) {
    notificacao.title = 'Comentário não realizado'
    notificacao.message = 'Você não possui as permissões necessárias'
  }

  if ((tipoComentarioSelect?.getValue() == TipoComentario.SindicatoLaboral || tipoComentarioSelect?.getValue() == TipoComentario.SindicatoPatronal) && !podeComentarSindicato) {
    notificacao.title = 'Comentário não realizado'
    notificacao.message = 'Você não possui as permissões necessárias'
  }

  if (!tipoComentarioSelect.getValue() || tipoComentarioSelect.getValue() == "") {
    notificacao.title = 'Campo obrigatório não preenchido.'
    notificacao.message = 'Tipo Comentário'
  }

  if (!assuntoSelect.getValue() || assuntoSelect.getValue() == "") {
    notificacao.title = 'Campo obrigatório não preenchido.'
    notificacao.message = 'Assunto/Sindicato'
  }

  if (!tipoUsuarioDestinoSelect.getValue() || tipoUsuarioDestinoSelect.getValue() == "") {
    notificacao.title = 'Campo obrigatório não preenchido.'
    notificacao.message = 'Tipo Destino'
  }

  if (!destinoSelect.getValue() || destinoSelect.getValue() == "") {
    notificacao.title = 'Campo obrigatório não preenchido.'
    notificacao.message = 'Grupo econômico/Empresa/Estabelecimento'
  }

  if (!tipoNotificacaoSelect.getValue() || tipoNotificacaoSelect.getValue() == "") {
    notificacao.title = 'Campo obrigatório não preenchido.'
    notificacao.message = 'Fixo ou temporário'
  }

  if (notificacao.title.length > 0) {
    return NotificationService.error(notificacao)
  }

  return comentario ? await editar() : await incluir()
}

async function incluir() {
  const requestData = {
    id: 0,
    tipo: parseInt(tipoComentarioSelect.getValue()),
    valor: $("#comentario").val(),
    tipoNotificacao: parseInt(tipoNotificacaoSelect.getValue()),
    referenciaId: parseInt(assuntoSelect.getValue()),
    dataValidade: dataValidade?.getValue(),
    tipoUsuarioDestino: parseInt(tipoUsuarioDestinoSelect.getValue()),
    usuarioDestionoId: parseInt(destinoSelect.getValue()),
    etiquetaId: parseInt(etiquetaSelect.getValue()),
    visivel: visivelSelect.getValue() == BooleanType.Sim ? true : null
  }

  const result = await comentarioService.incluir(requestData)

  if (result.isFailure()) {
    NotificationService.error({ title: 'Não foi possível realizar o cadastro! Erro: ', message: result.error })

    return result
  }

  NotificationService.success({ title: 'Cadastro realizado com sucesso!' })

  limpar()

  return Result.success()
}

async function editar() {
  const requestData = {
    id: comentario.id,
    tipo: parseInt(tipoComentarioSelect.getValue()),
    valor: $("#comentario").val(),
    tipoNotificacao: parseInt(tipoNotificacaoSelect.getValue()),
    referenciaId: parseInt(assuntoSelect.getValue()),
    dataValidade: dataValidade?.getValue(),
    tipoUsuarioDestino: parseInt(tipoUsuarioDestinoSelect.getValue()),
    usuarioDestionoId: parseInt(destinoSelect.getValue()),
    etiquetaId: parseInt(etiquetaSelect.getValue()),
    visivel: visivelSelect.getValue() == BooleanType.Sim ? true : null
  }


  const result = await comentarioService.editar(requestData)

  if (result.isFailure()) {
    NotificationService.error({ title: 'Não foi possível realizar a atualização! Erro: ', message: result.error })

    return result
  }

  NotificationService.success({ title: 'Editado atualizado com sucesso!' })

  limpar()

  return Result.success()
}

async function obterPorId(id) {
  limparFormulario()

  const response = await comentarioService.obterPorId(id)

  if (!response) {
    NotificationService.error({ title: 'Erro', message: 'Notificação não foi encontrada' })
    return
  }

  const data = response.value

  comentario = data

  $('#usuario').val(data.administradorNome)
  $("#comentario").val(data.comentario)

  if (data.dataFinal && data.dataFinal != "0001-01-01"){
    dataValidade?.setValue(data.dataFinal)
  }

  if (data.tipoComentario) {
    const tiposComentarios = await obterTipoComentarioSelect({ showFilial: usuario.nivel === UsuarioNivel.Ineditta })

    tipoComentarioSelect?.setCurrentValue([{ id: data.tipoComentario.id, description: tiposComentarios.find(tc => tc.id == data.tipoComentario.id)?.description }])
  }

  tipoUsuarioDestinoSelect?.setCurrentValue([{ id: data.tipoUsuarioDestino, description: getDescriptionTipoUsuarioDestino(data.tipoUsuarioDestino - 1) }])
  destinoSelect?.setCurrentValue([{ id: data.usuarioDestinoId, description: data.usuarioDestinoDescricao }])

  if (data.tipoNotificacaoId) {
    tipoNotificacaoSelect?.setCurrentValue([{ id: data.tipoNotificacaoId, description: getDescriptionTipoUsuarioNotificacao(data.tipoNotificacaoId - 1) }])
  }

  if (data.etiqueta) {
    etiquetaSelect?.setCurrentValue([{ id: data.etiqueta.id, description: data.etiqueta.nome }])
    tipoEtiquetaSelect?.setCurrentValue([{ id: data.etiqueta.tipo.id, description: data.etiqueta.tipo.nome }])
  }

  if (data.assunto) {
    assuntoSelect?.setCurrentValue([{ id: data.assunto.id, description: data.assunto.description }])
  }

  const visivel = data.visivel
  visivelSelect?.setCurrentValue([{ id: visivel, description: visivel ? 'Sim' : 'Não' }])

  configurarTituloAssunto(data.tipoComentario?.id)
  configurarTituloTipoUsuario(data.tipoUsuarioDestino?.id)
}

function limparFormulario() {
  $("#comentario").val('')
  $("#id_note").val('')

  carregarInformacoesUsuario()

  tipoComentarioSelect?.clear()
  etiquetaSelect?.clear()
  tipoNotificacaoSelect?.clear()
  tipoUsuarioDestinoSelect?.clear()
  assuntoSelect?.clear()
  destinoSelect?.clear()
  tipoEtiquetaSelect.clear()
  visivelSelect.clear()
  dataValidade?.setValue(null)

  comentario = null
}

function limpar() {
  limparFormulario();
  reloadDataTable();
}

function reloadDataTable() {
  notificacaoTb?.reload();
}
