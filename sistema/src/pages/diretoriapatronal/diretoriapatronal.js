// Libs
import 'bootstrap';
import jQuery from 'jquery';
import $ from 'jquery';
import 'jquery-mask-plugin';

// Css libs
import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'bootstrap/dist/css/bootstrap.min.css';

// JS Libs
import '../../js/utils/masks/jquery-mask-extensions.js';

// Services
import {
  ClienteUnidadeService,
  DiretoriaPatronalService,
  SindicatoPatronalService
} from '../../js/services'

// Utils
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import DatepickerWrapper from '../../js/utils/datepicker/datepicker-wrapper.js';
import DateParser from '../../js/utils/date/date-parser.js';
import NotificationService from '../../js/utils/notifications/notification.service.js';
import { renderizarModal, closeModal } from '../../js/utils/modals/modal-wrapper.js';

// Core
import Result from '../../js/core/result.js';
import { AuthService } from '../../js/core/auth.js';
import { ApiService } from '../../js/core/api.js';
import { ApiLegadoService } from '../../js/core/api-legado.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const diretoriaPatronalService = new DiretoriaPatronalService(apiService, apiLegadoService);
const sindicatoPatronalService = new SindicatoPatronalService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);


let diretoriaPatronalTb = null;
let sindicatoPatronalTb = null;
let empresaTb = null;
let dataInicial = null;
let dataFinal = null;


jQuery(async ($) => {
  new Menu()

  await AuthService.initialize();

  configurarModal();

  configurarFormulario($);

  await carregarDatatable();

});

async function carregarDatatable() {
  diretoriaPatronalTb = new DataTableWrapper('#diretoriapatronaltb', {
    ajax: async (requestData) =>
      await diretoriaPatronalService.obterDatatable(requestData),
    columns: [
      { "data": 'id' },
      { "data": "nome" },
      { "data": "inicioMandato", type: 'date' },
      { "data": "terminoMandato", type: 'date' },
      { "data": "funcao" },
      { "data": "situacao" },
      { "data": "nomeUnidade" },
      { "data": "sigla" },
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
    }],
    rowCallback: function (row, data) {
      const icon = $("<i>").addClass("fa fa-file-text");
      let button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon);
      button.on("click", function () {
        const id = $(this).attr("data-id");
        $('#id-input').val(id);
        $('#diretoriaPatronalBtn').trigger('click'); // Abrir modal
      });
      $("td:eq(0)", row).html(button);
    },
  });

  await diretoriaPatronalTb.initialize();
}

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  const modalCadastrar = document.getElementById('diretoriaSindicatoPatronalModalHidden');
  const contentCadastrar = document.getElementById('diretoriaSindicatoPatronalModalHiddenContent');

  const buttonsCadastrarConfig = [
    {
      id: 'diretoriaSindicatoPatronalCadastrarBtn',
      onClick: async (id, modalContainer) => {
        const result = await upsert();
        if (result.isSuccess()) {
          closeModal(modalContainer);
        }
      }
    }
  ];

  const modalSindicatoDirigente = document.getElementById('sindicatoDirigenteModalHidden');
  const modalSindicatoDirigenteContent = document.getElementById('sindicatoDirigenteModalHiddenContent');

  const modalEmpresa = document.getElementById('empresaModalHidden');
  const modalEmpresaContent = document.getElementById('empresaModalHiddenContent');

  const modalsConfig = [
    {
      id: 'diretoriaPatronalModal',
      modal_hidden: modalCadastrar,
      content: contentCadastrar,
      btnsConfigs: buttonsCadastrarConfig,
      onOpen: async () => {
        const id = $('#id-input').val();
        if (id) {
          await obterPorId(id);
        }
      },
      onClose: () => limpar()
    },
    {
      id: 'sindicatoDirigenteModal',
      modal_hidden: modalSindicatoDirigente,
      content: modalSindicatoDirigenteContent,
      btnsConfigs: [],
      onOpen: async () => await openModalSindicatoDirigente(),
      isInIndex: true
    },
    {
      id: 'empresaModal',
      modal_hidden: modalEmpresa,
      content: modalEmpresaContent,
      btnsConfigs: [],
      onOpen: async () => await openModalEmpresa(),
      isInIndex: true
    }
  ];

  renderizarModal(pageCtn, modalsConfig);
}

async function openModalSindicatoDirigente() {
  await carregarSindicatosDirigentes();
}

async function openModalEmpresa() {
  await carregarEmpresas();
}


async function upsert() {
  const id = $('#id-input').val();

  return id ? await editar() : await incluir();
}


async function carregarSindicatosDirigentes() {
  if (sindicatoPatronalTb) {
    sindicatoPatronalTb.reload();
    return;
  }

  sindicatoPatronalTb = new DataTableWrapper('#sindicatoPatronalTb', {
    ajax: async (requestData) =>
      await sindicatoPatronalService.obterDatatable(requestData),
    columns: [
      { "data": "id" },
      { "data": "sigla" },
      { "data": "cnpj" },
      { "data": "municipio" },
      { "data": "email" },
      { "data": "telefone" },
      { "data": "uf" }
    ],
    rowCallback: function (row, data) {
      const button = $(`<button type="button" data-dismiss="modal" data-item='${JSON.stringify(data)}' class="btn btn-secondary">Selecionar</button>`);

      button.on('click', (el) => {
        const value = el.target.attributes['data-item'].value;
        vincularSindicatoDirigente(JSON.parse(value));
      });

      $("td:eq(0)", row).html(button);
    },
  });

  await sindicatoPatronalTb.initialize();
}

async function carregarEmpresas() {
  if (empresaTb) {
    empresaTb.reload();
    return;
  }

  empresaTb = new DataTableWrapper('#empresaTb', {
    ajax: async (requestData) => {
      return await clienteUnidadeService.obterDatatable(requestData);
    },
    columns: [
      { "data": "id" },
      { "data": "nomeGrupoEconomico" },
      { "data": "nome" },
      { "data": "nomeEstabelecimento" },
      { "data": "cnpj" }
    ],
    rowCallback: function (row, data) {
      const button = $(`<button type="button" data-dismiss="modal" data-item='${JSON.stringify(data)}' class="btn btn-secondary">Selecionar</button>`);

      button.on('click', (el) => {
        const value = el.target.attributes['data-item'].value;
        vincularEmpresa(JSON.parse(value));
      });

      $("td:eq(0)", row).html(button);
    },
  });

  await empresaTb.initialize();
}

function configurarFormulario($) {

  dataInicial = new DatepickerWrapper('#dataini-input');

  dataFinal = new DatepickerWrapper('#datafim-input');

  $('#btnEmpresa').on('click', async () => await carregarEmpresas());

  $('#btn-atualizar').on('click', async () => await editar());

  $('#btn-cancelar').on('click', () => {
    limpar();
  });
}

async function incluir() {
  const request = {
    "nome": $("#dir-input").val(),
    "funcao": $("#func-input").val(),
    "situacao": $("#sit-input").val(),
    "dataInicioMandato": dataInicial?.getValue() instanceof Date && DateParser.toString(dataInicial?.getValue()), 
    "dataFimMandato": dataFinal?.getValue() instanceof Date && DateParser.toString(dataFinal?.getValue()),
    "sindicatoPatronalId": $("#sind-input").val(),
    "EstabelecimentoId": $("#emp-input").val()
  };

  const result = await diretoriaPatronalService.incluir(request);

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro', message: result.error });
    return result;
  }

  NotificationService.success({ title: 'Sucesso', message: 'Cadastro realizado com sucesso!' });

  limpar();

  return Result.success();
}

async function editar() {
  var id = $('#id-input').val();

  const request = {
    "nome": $("#dir-input").val(),
    "funcao": $("#func-input").val(),
    "situacao": $("#sit-input").val(),
    "dataInicioMandato": dataInicial?.getValue() instanceof Date && DateParser.toString(dataInicial?.getValue()), 
    "dataFimMandato": dataFinal?.getValue() instanceof Date && DateParser.toString(dataFinal?.getValue()),
    "sindicatoPatronalId": $("#sind-input").val(),
    "EstabelecimentoId": $("#emp-input").val(),
    "id": id
  };

  const result = await diretoriaPatronalService.editar(request);

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro', message: result.error });
    return result;
  }

  NotificationService.success({ title: 'Sucesso', message: 'Cadastro atualizado com sucesso!' });

  limpar();

  return Result.success();
}

async function obterPorId(id) {
  limparFormulario();

  const response = await diretoriaPatronalService.obterPorId(id);

  if (!response) {
    NotificationService.error({ title: 'Erro', message: 'Sindicato Laboral não foi encontrada' });
    return;
  }

  $("#dir-input").val(response.value.nome);
  $("#func-input").val(response.value.funcao);
  $("#sit-input").val(response.value.situacao);
  $("#sind-input").val(response.value.sindicatoDirigenteId);
  $("#emp-input").val(response.value.empresaId);
  $("#id-input").val(response.value.id);
  dataInicial?.setValue(response.value.inicioMandato);
  dataFinal?.setValue(response.value.terminoMandato);

  selectSindicatoDirigente(response.value.sindicatoDirigenteId, response.value.sindicatoDirigenteSigla);
  selectEmpresa(response.value.empresaId, response.value.empresaFilial);
}

function selectSindicatoDirigente(id, sigla) {
  $("#sind-input").val(id);
  $("#sind-input").prop("disabled", true);

  $('#sind-input')
    .empty();

  if (id) {
    $('#sind-input')
      .append(`<option selected="selected" value="${id}">${sigla}</option>`);
  }
}

function selectEmpresa(id, filial) {
  $("#emp-input").val(id);
  $("#emp-input").prop("disabled", true);

  $('#emp-input')
    .empty();

  if (id) {
    $('#emp-input')
      .append(`<option selected="selected" value="${id}">${filial}</option>`);
  }
}

function limparFormulario() {
  $("#dir-input").val(null);
  $("#func-input").val(null);
  $("#sit-input").val(null);
  $("#sind-input").val(null);
  $("#emp-input").val(null);
  $("#id-input").val(null);
  selectEmpresa(null, null);
  selectSindicatoDirigente(null, null);
  dataInicial?.setValue(null);
  dataFinal?.setValue(null);
}

function limpar() {
  limparFormulario();
  diretoriaPatronalTb?.reload();
}

function vincularSindicatoDirigente(item) {
  selectSindicatoDirigente(item?.id, item?.sigla);
}

function vincularEmpresa(item) {
  selectEmpresa(item?.id, item?.nomeEstabelecimento);
}