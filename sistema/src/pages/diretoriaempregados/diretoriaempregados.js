/* eslint-disable no-unused-vars */
import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';

import $, { data } from 'jquery';
import jQuery from 'jquery';
import '../../js/utils/masks/jquery-mask-extensions.js';

import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';

import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import DatepickerWrapper from '../../js/utils/datepicker/datepicker-wrapper.js';

// Core
import { AuthService, ApiService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js'

// Service
import {
  DiretoriaEmpregadoService,
  SindicatoLaboralService,
  ClienteUnidadeService
} from '../../js/services'

import NotificationService from '../../js/utils/notifications/notification.service.js'; 
import { renderizarModal, closeModal } from '../../js/utils/modals/modal-wrapper.js';
import Result from '../../js/core/result.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const diretoriaEmpregadoService = new DiretoriaEmpregadoService(apiService, apiLegadoService);
const sindicatoLaboralService = new SindicatoLaboralService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);

let diretoriaEmpregadoTb = null;
let sindicatoLaboralTb = null;
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
  diretoriaEmpregadoTb = new DataTableWrapper('#diretoriaempregadostb', {
    ajax: async (requestData) =>
      await diretoriaEmpregadoService.obterDatatable(requestData),
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
    }, {
      targets: "_all",
      defaultContent: ""
    }],
    rowCallback: function (row, data, index) {
      const icon = $("<i>").addClass("fa fa-file-text");
      let button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon);
      button.on("click", function () {
        const id = $(this).attr("data-id");
        $('#id-input').val(id);
        $('#diretoriaLaboralBtn').trigger('click');
      });
      $("td:eq(0)", row).html(button);
    },
  });

  await diretoriaEmpregadoTb.initialize();
}

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  const modalCadastrar = document.getElementById('diretoriaSindicatoLaboralModalHidden');
  const contentCadastrar = document.getElementById('diretoriaSindicatoLaboralModalHiddenContent');

  const buttonsCadastrarConfig = [
    {
      id: 'diretoriaSindicatoLaboralCadastrarBtn',
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
      id: 'diretoriaLaboralModal',
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
  if (sindicatoLaboralTb) {
    sindicatoLaboralTb.reload();
    return;
  }

  sindicatoLaboralTb = new DataTableWrapper('#sindicatoLaboralTb', {
    ajax: async (requestData) =>
      await sindicatoLaboralService.obterDatatable(requestData),
    columns: [
      { "data": "id" },
      { "data": "sigla" },
      { "data": "cnpj" },
      { "data": "logradouro" },
      { "data": "email" },
      { "data": "telefone" },
      { "data": "site" }
    ],
    rowCallback: function (row, data, index) {
      const button = $(`<button type="button" data-dismiss="modal" data-item='${JSON.stringify(data)}' class="btn btn-secondary">Selecionar</button>`);

      button.on('click', (el) => {
        const value = el.target.attributes['data-item'].value;
        vincularSindicatoDirigente(JSON.parse(value));
      });

      $("td:eq(0)", row).html(button);
    },
  });

  await sindicatoLaboralTb.initialize();
}

async function carregarEmpresas() {
  if (empresaTb) {
    empresaTb.reload();
    return;
  }

  empresaTb = new DataTableWrapper('#empresaTb', {
    ajax: async (requestData) =>
      await clienteUnidadeService.obterDatatable(requestData),
    columns: [
      { "data": "id" },
      { "data": "filial" },
      { "data": "grupo" },
      { "data": "matriz" },
    ],
    rowCallback: function (row, data, index) {
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
    "module": "diretoriaempregados",
    "action": "addDiretoriaEmpregados",
    "dir-input": $("#dir-input").val(),
    "func-input": $("#func-input").val(),
    "sit-input": $("#sit-input").val(),
    "dataini-input": dataInicial?.getValue(),
    "datafim-input": dataFinal?.getValue(),
    "sind-input": $("#sind-input").val(),
    "emp-input": $("#emp-input").val()
  };

  const result = await diretoriaEmpregadoService.incluir(request);

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
    "module": "diretoriaempregados",
    "action": "updateDiretoriaEmpregados",
    "dir-input": $("#dir-input").val(),
    "func-input": $("#func-input").val(),
    "sit-input": $("#sit-input").val(),
    "dataini-input": dataInicial?.getValue(),
    "datafim-input": dataFinal?.getValue(),
    "sind-input": $("#sind-input").val(),
    "emp-input": $("#emp-input").val(),
    "id_diretoriap": id
  };

  const result = await diretoriaEmpregadoService.editar(request);

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

  const response = await diretoriaEmpregadoService.obterPorId(id);

  if (!response) {
    NotificationService.error({ title: 'Erro', message: 'Diretoria laboral não foi encontrada' });
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
  diretoriaEmpregadoTb?.reload();
}

function vincularSindicatoDirigente(item) {
  selectSindicatoDirigente(item?.id, item?.sigla);
}

function vincularEmpresa(item) {
  selectEmpresa(item?.id, item?.filial);
}