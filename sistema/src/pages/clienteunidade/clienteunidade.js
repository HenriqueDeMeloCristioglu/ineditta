import JQuery from 'jquery';
import $ from 'jquery';

import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';
import 'datatables.net-responsive-bs5';

import '../../js/utils/masks/jquery-mask-extensions';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

import Masker from '../../js/utils/masks/masker.js';
import SelectWrapper from '../../js/utils/selects/select-wrapper.js';
import { closeModal, renderizarModal } from '../../js/utils/modals/modal-wrapper.js';
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import DatepickerWrapper from '../../js/utils/datepicker/datepicker-wrapper.js';
import { configPanel } from '../../js/utils/panels';
import NotificationService from '../../js/utils/notifications/notification.service';
import Result from '../../js/core/result';

// Core
import { ApiService, AuthService } from '../../js/core/index.js'

// Services
import {
  UsuarioAdmService,
  ClienteUnidadeService,
  TipoUnidadeClienteService,
  CnaeService,
  LocalizacaoService,
  MatrizService
} from '../../js/services'
import DateFormatter from '../../js/utils/date/date-formatter.js';

const apiService = new ApiService()
const clienteUnidadeService = new ClienteUnidadeService(apiService)
const usuarioAdmService = new UsuarioAdmService(apiService);
const cnaeService = new CnaeService(apiService);
const matrizService = new MatrizService(apiService);
const tipoUnidadeClienteService = new TipoUnidadeClienteService(apiService);
const localizacaoService = new LocalizacaoService(apiService);

let clienteUnidade = null
let update = false

let clienteUniadeTb = null
let cnaesTb = null
let cnaesSelecionadosTb = null

let matrizSelect = null
let tipoNegocioSelect = null
let localizacaoSelect = null
let cnaeFilialSelect = null

let dataInativacaoDatePicker = null

let cnaeUnidadesIds = []
let cnaesUniadesIdsRemover = []

JQuery(async function () {
  new Menu()

  await AuthService.initialize()

  await carregarDatatable()

  configurarModal()

  configurarFormulario()

  await carregarPermissoesUsuario()
})

async function carregarDatatable() {
  clienteUniadeTb = new DataTableWrapper('#clienteUnidadeTb', {
    ajax: async (requestData) => await clienteUnidadeService.obterDatatable(requestData),
    columns: [
      { "data": 'id', title: '' },
      { "data": 'nomeGrupoEconomico', title: 'Grupo Econômico' },
      { "data": 'nome', title: 'Empresa' },
      { "data": "nomeEstabelecimento", title: 'Estabelecimento' },
      { "data": "cnpj", title: 'CNPJ', render: (data) => Masker.CNPJ(data) },
      { "data": "tipoFilial", title: 'Tipo Filial' },
      { "data": "uf", title: 'UF' },
      { "data": "municipio", title: 'Municipio' },
      { "data": "dataInativacao", title: 'Data de Inativação', render: (data) => DataTableWrapper.formatDate(data) },
      { "data": "dataAtivacao", title: 'Data de Ativação', render: (data) => Masker.dateTime(data) }
    ],
    rowCallback: function (row, data) {
      const icon = $("<i>").addClass("fa fa-file-text")

      let button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon)

      button.on("click", async function () {
        await obterPorId(data?.id)
        update = true
        $('#novoClientUnidadeModalBtn').trigger('click')
      })

      $("td:eq(0)", row).html(button)
    },
    columnDefs: [{
      targets: "_all",
      defaultContent: ""
    }]
  })

  await clienteUniadeTb.initialize()
}

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  const novoClientUnidadeModalHidden = document.getElementById('novoClientUnidadeHidden');
  const novoClientUnidadeModalHiddenContent = document.getElementById('novoClientUnidadeContent');

  const novoClienteUnidadeModalButtonsConfig = [
    {
      id: 'btn_submit_modal',
      onClick: async (_, modalContainer) => {
        if (clienteUnidade && clienteUnidade.id) {
          const result = await alterar(clienteUnidade.id)

          if (!result.isFailure()) {
            closeModal(modalContainer)
            await clienteUniadeTb.reload()
          }
        } else {
          const result = await addClienteUnidade()
          if (!result.isFailure()) {
            closeModal(modalContainer)
            await clienteUniadeTb.reload()
          }
        }
      }
    },
    {
      id: 'btn_excluir_cnaes',
      onClick: async () => {
        cnaesUniadesIdsRemover.map(id => {
          cnaeUnidadesIds = cnaeUnidadesIds.filter(item => parseInt(item.id) != parseInt(id))
        })

        cnaesUniadesIdsRemover = []
        await cnaesTb.reload()
        await cnaesSelecionadosTb.reload()

        $('#select_todos_cnaes_selecionados').prop('checked', false)
      }
    }
  ]

  const modalsConfig = [
    {
      id: 'novoClientUnidadeModal',
      modal_hidden: novoClientUnidadeModalHidden,
      content: novoClientUnidadeModalHiddenContent,
      btnsConfigs: novoClienteUnidadeModalButtonsConfig,
      onOpen: async () => {        
        configCollapsePanels()

        if (update) {
          $('#data_inclusao').show()
          $('#cnaes_selecionados').show()
          await carregarCnaesSelecionados()
        }
        
        await carregarCnaes()
      },
      onClose: () => {
        limparFormulario()
        $('#data_inclusao').hide()
        update = false
      }
    }
  ];

  renderizarModal(pageCtn, modalsConfig);
}

function configCollapsePanels() {
  const panels = ['panel-select-cnaes', 'cnaes_selecionados', 'panel-novo-registro']

  panels.map(panel => configPanel(panel))
}

async function carregarCnaesSelecionados() {
  if (cnaesSelecionadosTb) {
    await cnaesSelecionadosTb.reload();
    return;
  }

  cnaesSelecionadosTb = new DataTableWrapper('#cnaesSelecionadosTb', {
    columns: [
      { "data": "id", orderable: false, title: "Selecionar" },
      { "data": "id", title: "CNAE" },
      { "data": "divisao", title: "Divisão" },
      { "data": "subclasse", title: "Subclasse", render: data => Masker.subclasseCNAE(data) },
      { "data": "descricao", title: "Descrição" },
      { "data": "categoria", title: "Categoria" }
    ],
    ajax: async (requestData) => {
      let cnaesArray = []

      cnaeUnidadesIds.map(item => cnaesArray.push(parseInt(item.id)))
      requestData.cnaesIds = cnaesArray.length > 0 ? cnaesArray : [-1]

      return await cnaeService.obterDatatable(requestData)
    },
    rowCallback: function (row, data) {
      const checkbox = $("<input>").attr("type", "checkbox").attr("data-id", data?.id).attr('checked', false).addClass('cnaes_selecionados')

      checkbox.on('change', (el) => {
        const checked = el.target.checked;
        const id = el.target.attributes['data-id'].value;

        if (checked) {
          return cnaesUniadesIdsRemover.push(id)
        }

        cnaesUniadesIdsRemover = cnaesUniadesIdsRemover.filter(item => item !== id)
      })

      $('#select_todos_cnaes_selecionados').on('click', function () {
        if ($(row).find('.cnaes_selecionados').is(':checked')) {
          $(row).find('.cnaes_selecionados').prop('checked', false);
          return $(row).find('.cnaes_selecionados').trigger('change');
        }

        $(row).find('.cnaes_selecionados').prop('checked', true);
        $(row).find('.cnaes_selecionados').trigger('change');
      })

      $("td:eq(0)", row).html(checkbox);
    },
  });

  await cnaesSelecionadosTb.initialize();
}

async function carregarCnaes() {
  if (cnaesTb) {
    return await cnaesTb.reload()
  }

  cnaesTb = new DataTableWrapper('#cnaesTb', {
    columns: [
      { "data": "id", orderable: false, title: "Selecionar" },
      { "data": "id", title: "CNAE" },
      { "data": "divisao", title: "Divisão" },
      { "data": "subclasse", title: "Subclasse", render: data => Masker.subclasseCNAE(data) },
      { "data": "descricao", title: "Descrição" },
      { "data": "categoria", title: "Categoria" }
    ],
    ajax: async (requestData) => {
      requestData.columns = ['id', 'divisao', 'descricao', 'subclasse', 'categoria'].join(', ');
      return await cnaeService.obterDatatable(requestData)
    },
    rowCallback: function (row, data) {
      const checkbox = $("<input>").attr("type", "checkbox").attr("data-id", data?.id).attr('checked', false).addClass('cnaes')

      cnaeUnidadesIds.map(item => {
        if (item.id == data?.id) {
          checkbox.prop('checked', true)
          checkbox.trigger('change')
        }
      })

      checkbox.on('change', (el) => {
        const checked = el.target.checked;
        const id = el.target.attributes['data-id'].value;

        atualizarCnaes(id, checked);
      });

      $('#select_todos_cnaes').on('click', function () {
        if ($(row).find('.cnaes').is(':checked')) {
          $(row).find('.cnaes').prop('checked', false);
          $(row).find('.cnaes').trigger('change');
          return
        }

        $(row).find('.cnaes').prop('checked', true);
        $(row).find('.cnaes').trigger('change');
      })

      $("td:eq(0)", row).html(checkbox);
    },
  });

  await cnaesTb.initialize();
}

function atualizarCnaes(id, checked) {
  let cnaeUniades = cnaeUnidadesIds.find(cnaeUniadesId => cnaeUniadesId == id)

  if (cnaeUniades) {
    cnaeUniades = checked
    cnaeUnidadesIds = cnaeUnidadesIds.filter(cnaeUniadesId => cnaeUniadesId != id)
    return
  }

  cnaeUnidadesIds.push({ id: parseInt(id) })
}

function configurarFormulario() {
  $('#cnaes_selecionados').hide()
  $('#novoClientUnidadeModalBtn').hide()
  $('#data_inclusao').hide()

  $("#cnpj-input").maskCNPJ()
  $("#sind-input").maskCustom('00000000')
  $("#cep-input").maskCEP()

  matrizSelect = new SelectWrapper('#em-input', { onOpened: async () => (await matrizService.obterSelectTodos()).value });
  tipoNegocioSelect = new SelectWrapper('#tn-input', { onOpened: async () => (await tipoUnidadeClienteService.obterSelect()).value });
  localizacaoSelect = new SelectWrapper('#loc-input', { onOpened: async () => await obterLocalizacaoSelect() });
  cnaeFilialSelect = new SelectWrapper('#cnae_filial_input', { onOpened: async () => await obterCnaesSelect() });

  dataInativacaoDatePicker = new DatepickerWrapper('#dataina-input');
}

async function obterLocalizacaoSelect() {
  const { value } = await localizacaoService.obterTodos()

  const options = value.map(lc => {
    return {
      id: lc.idLocalizacao,
      description: lc.municipio + ' - ' + lc.uf
    }
  })

  return await Promise.resolve(options)
}

async function obterCnaesSelect() {
  const { value } = await cnaeService.obterTodos()

  const options = value.map(cn => {
    return {
      id: cn.id,
      description: Masker.subclasseCNAE(cn.subclasse) + ' - ' + cn.descricaoSubClasse
    }
  })

  return await Promise.resolve(options)
}

async function addClienteUnidade() {
  const requestData = {
    "empresaId": matrizSelect.hasValue() ? parseInt(matrizSelect.getValue()) : null,
    "tipoNegocioId": tipoNegocioSelect.hasValue() ? parseInt(tipoNegocioSelect.getValue()) : null,
    "localizacaoId": localizacaoSelect.hasValue() ? parseInt(localizacaoSelect.getValue()) : null,
    "codigo": $("#cod-input").val(),
    "nome": $("#nome-input").val(),
    "cnpj": $("#cnpj-input").val(),
    "endereco": $("#end-input").val(),
    "bairro": $("#bairro-input").val(),
    "cep": $("#cep-input").val(),
    "regiao": $("#reg-input").val(),
    "dataAusencia": dataInativacaoDatePicker.getValue(),
    "codigoSindicatoCliente": $("#csc-input").val(),
    "codigoSindicatoPatronal": $("#csp-input").val(),
    "cnaesUnidade": cnaeUnidadesIds,
    "cnaeFilial": cnaeFilialSelect.hasValue() ? parseInt(cnaeFilialSelect.getValue()) : null
  };

  const result = await clienteUnidadeService.incluir(requestData);

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao realizar editar cliente.',
      message: result.error
    });

    return;
  }

  NotificationService.success({ title: update ? 'Edição realizada com sucesso!' : 'Cadastro realizado com sucesso!' })

  clienteUniadeTb.reload()

  return Result.success();
}

async function alterar(id) {
  const requestData = {
    "id": id,
    "empresaId": matrizSelect.hasValue() ? parseInt(matrizSelect.getValue()) : null,
    "codigo": $("#cod-input").val(),
    "nome": $("#nome-input").val(),
    "cnpj": $("#cnpj-input").val(),
    "endereco": $("#end-input").val(),
    "regiao": $("#reg-input").val(),
    "bairro": $("#bairro-input").val(),
    "cep": $("#cep-input").val(),
    "dataAusencia": dataInativacaoDatePicker.getValue(),
    "codigoSindicatoCliente": $("#csc-input").val(),
    "codigoSindicatoPatronal": $("#csp-input").val(),
    "tipoNegocioId": tipoNegocioSelect.hasValue() ? parseInt(tipoNegocioSelect.getValue()) : null,
    "localizacaoId": localizacaoSelect.hasValue() ? parseInt(localizacaoSelect.getValue()) : null,
    "cnaeFilial": cnaeFilialSelect.hasValue() ? parseInt(cnaeFilialSelect.getValue()) : null,
    "cnaesUnidade": cnaeUnidadesIds
  };

  const result = await clienteUnidadeService.editar(requestData);

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao realizar editar cliente.',
      message: result.error
    });

    return;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Cliente atualizado com sucesso",
  });

  return Result.success()
}

async function obterPorId(id) {
  const result = await clienteUnidadeService.obterPorId(id);

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao obter o unidade.',
      message: result.error
    });

    return;
  }

  const data = result.value;

  clienteUnidade = data

  $("#cod-input").val(data.codigo)
  $("#nome-input").val(data.nome)
  $("#cnpj-input").val(data.cnpj)
  $("#end-input").val(data.logradouro)
  $("#bairro-input").val(data.bairro)
  $("#cep-input").val(data.cep)
  $("#reg-input").val(data.regiao)
  $("#csc-input").val(data.codigoSindicatoCliente)
  $("#csp-input").val(data.codigoSindicatoPatronal)
  $('#data_inclusao_input').val(DateFormatter.dateTime(data.dataAtivacao))
  cnaeUnidadesIds = data.cnaesUnidade ?? []

  dataInativacaoDatePicker.setValue(data.dataInativacao)

  if (data.matrizId) {
    const matrizes = (await matrizService.obterSelectTodos()).value
    matrizSelect.setCurrentValue({ id: data.matrizId, description: matrizes.find(item => item.id === data.matrizId).description })
  }

  if (data.tipoNegocioId) {
    const tipoUnidades = (await tipoUnidadeClienteService.obterSelect()).value
    tipoNegocioSelect.setCurrentValue({ id: data.tipoNegocioId, description: tipoUnidades.find(item => item.id === data.tipoNegocioId).description })
  }

  if (data.localizacaoId) {
    const localizacoes = await obterLocalizacaoSelect()
    localizacaoSelect.setCurrentValue({ id: data.localizacaoId, description: localizacoes.find(item => item.id === data.localizacaoId).description })
  }

  if (data.cnaeFilialId) {
    const cnaeFilial = await obterCnaesSelect()
    cnaeFilialSelect.setCurrentValue({ id: data.cnaeFilialId, description: cnaeFilial.find(item => item.id === data.cnaeFilialId).description })
  }
}

function limparFormulario() {
  matrizSelect.setCurrentValue({
    id: '',
    description: ''
  })
  tipoNegocioSelect.setCurrentValue({
    id: '',
    description: ''
  })
  localizacaoSelect.setCurrentValue({
    id: '',
    description: ''
  })
  cnaeFilialSelect.setCurrentValue({
    id: '',
    description: ''
  })
  $("#cod-input").val('')
  $("#nome-input").val('')
  $("#cnpj-input").val('')
  $("#end-input").val('')
  $("#bairro-input").val('')
  $("#cep-input").val('')
  $("#reg-input").val('')
  dataInativacaoDatePicker.clear()
  $("#dataclu-input").val('')
  $("#csc-input").val('')
  $("#csp-input").val('')
  cnaeUnidadesIds = []
  $('#cnae_filial_input').val('')

  update = false
  $('#cnaes_selecionados').hide()
  
  $('#data_inclusao_input').val('')

  $('#select_todos_cnaes').prop('checked', false)
  $('#select_todos_cnaes_selecionados').prop('checked', false)

  clienteUnidade = null
}

async function carregarPermissoesUsuario() {
  const result = await usuarioAdmService.obterPermissoes()

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter permições do usuário', message: result.error })
  }

  const data = result.value

  if (data.find(item => item.modulos == 'Cliente Unidade').criar == 1) {
    $('#novoClientUnidadeModalBtn').show()
  }
}
