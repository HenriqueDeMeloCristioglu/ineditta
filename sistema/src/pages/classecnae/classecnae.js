// Libs
import jQuery from 'jquery'
import $ from 'jquery'
import 'popper.js'

// Utils
import '../../js/utils/masks/jquery-mask-extensions.js'

// Temp
import 'datatables.net-bs5/css/dataTables.bootstrap5.css'
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css'
import 'datatables.net-bs5'
import 'datatables.net-responsive-bs5'

// Css libs
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap'

// Core
import { AuthService, ApiService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js'

// Services
import { CnaeService } from '../../js/services'

import '../../js/main.js'
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper'
import { Menu } from '../../components/menu/menu.js'
import { closeModal, renderizarModal } from '../../js/utils/modals/modal-wrapper.js'
import NotificationService from '../../js/utils/notifications/notification.service.js'
import Result from '../../js/core/result.js'

// Services
const apiService = new ApiService()
const apiLegadoService = new ApiLegadoService()
const cnaeService = new CnaeService(apiService, apiLegadoService)

let cnaesTb = null

let cnaeSelecionado = null
let id = null

jQuery(async () => {
  new Menu()

  await AuthService.initialize()

  configurarModal()

  await configurarFormulario()

  await carregarDatatable()
})

async function configurarFormulario() {
	$("#divisao").mask('00000000')
	$("#subclasse").mask('00000000')
}

function configurarModal() {
  const pageCtn = document.getElementById("pageCtn")

  const modalCadastro = document.getElementById("cadastrarModalHidden")
  const contentCadastro = document.getElementById("cadastrarModalHiddenContent")

  const buttonsCadastroConfig = [
    {
      id: "cadastrarBtn",
      onClick: async (_, modalContainer) => {
        const result = await upsert()
        if (result.isSuccess()) {
          closeModal(modalContainer)
          await cnaesTb.reload()
        }
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
        if (id) {
          await obterCnaePorId()
        }
      },
      onClose: () => {
        limparFormulario()
      },
    }
  ]

  renderizarModal(pageCtn, modalsConfig)
}

async function carregarDatatable() {
  cnaesTb = new DataTableWrapper('#cnaesTb', {
    columns: [
      { "data": "id", title: "" },
      { "data": "id", title: "CNAE" },
      { "data": "divisao", title: "Divisão" },
      { "data": "descricaoDivisao", title: "Descrição Divisão" },
      { "data": "subclasse", title: "Subclasse" },
      { "data": "descricaoSubclasse", title: "Descrição Subclasse" },
      { "data": "categoria", title: "Categoria" }
    ],
    ajax: async (requestData) => await cnaeService.obterTodosInclusao(requestData),
    rowCallback: function (row, data) {
      const editIcon = $("<i>").addClass("fa fa-file-text")
      let editButton = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(editIcon)

      editButton.on("click", async function () {
        id = parseInt($(this).attr("data-id"))

        $("#novoCnaeBtn").trigger("click")
      })

      $("td:eq(0)", row).html(editButton)
    }
  })

  await cnaesTb.initialize()
}

async function upsert() {
  const result = id ? await editar() : await incluir()

  await cnaesTb?.reload()

  return result
}

async function incluir(){
  const requestData = {
    divisao: parseInt($("#divisao").val()),
    descricaoDivisao: $("#descricao_divisao").val(),
    subClasse: parseInt($("#subclasse").val()),
    descricaoSubClasse: $("#descricao_subclasse").val(),
    categoria: $("#categoria").val()
  }

  const result = await cnaeService.salvar(requestData)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Não foi posssível realizar o cadastro', message: result.error })
  }

  NotificationService.success({ title: 'Cadastro realizado com sucesso!' })

  return Result.success()
}

async function editar(){
  const requestData = {
    id,
    divisao: parseInt($("#divisao").val()),
    descricaoDivisao: $("#descricao_divisao").val(),
    subClasse: parseInt($("#subclasse").val()),
    descricaoSubClasse: $("#descricao_subclasse").val(),
    categoria: $("#categoria").val()
  }

  const result = await cnaeService.editar(requestData)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Não foi posssível realizar a atualização', message: result.error })
  }

  NotificationService.success({ title: 'Atualização realizada com sucesso!' })

  return Result.success()
}

async function obterCnaePorId(){
  const result = await cnaeService.obterPorId(id)

  const data = result.value

  $("#divisao").val(data.divisao)
  $("#descricao_divisao").val(data.descricaoDivisao)
  $("#subclasse").val(data.subclasse)
  $("#descricao_subclasse").val(data.descricaoSubclasse)
  $("#categoria").val(data.categoria)
}

function limparFormulario() {
  $("#divisao").val("")
  $("#descricao_divisao").val("")
  $("#subclasse").val("")
  $("#descricao_subclasse").val("")
  $("#categoria").val("")

  id = null
  cnaeSelecionado = null
}
