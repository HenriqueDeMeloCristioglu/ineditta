import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';
import 'datatables.net-responsive-bs5';

import JQuery from 'jquery';
import $ from 'jquery';

import { AuthService } from '../../js/core/auth.js';
import SelectWrapper from '../../js/utils/selects/select-wrapper.js';
import { renderizarModal } from '../../js/utils/modals/modal-wrapper.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

let sindicatoLaboralSelect = null;
let tipoAcordoSelect = null;
let matrizSelect = null;
let filialSelect = null;

let scriptSelect = null;
let pautaSelect = null;
let premissasSelect = null;

JQuery(async function () {
  new Menu()

  await AuthService.initialize()

  configurarModal()

  configurarFormulario()

  configurarButtonsActions()
})



function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  // Negociacao Act
  const negociacaoActModalHidden = document.getElementById('negociacaoActHidden');
  const negociacaoActModalHiddenContent = document.getElementById('negociacaoActContent');

  // Acompanhamento CCT
  const acompanhamentoCctModalHidden = document.getElementById('acompanhamentoCctHidden');
  const acompanhamentoCctModalHiddenContent = document.getElementById('acompanhamentoCctContent');

  // Select Acompanhamento
  const selectAcompanhamentoModalHidden = document.getElementById('selectAcompanhamentoHidden');
  const selectAcompanhamentoModalHiddenContent = document.getElementById('selectAcompanhamentoContent');

  // Editar Acompanhamento
  const editarAcompanhamentoModalHidden = document.getElementById('editarAcompanhamentoHidden');
  const editarAcompanhamentoModalHiddenContent = document.getElementById('editarAcompanhamentoContent');

  const modalsConfig = [
    {
      id: 'negociacaoActModal',
      modal_hidden: negociacaoActModalHidden,
      content: negociacaoActModalHiddenContent,
      btnsConfigs: [],
      onOpen: null,
      onClose: null
    },
    {
      id: 'acompanhamentoCctModal',
      modal_hidden: acompanhamentoCctModalHidden,
      content: acompanhamentoCctModalHiddenContent,
      btnsConfigs: [],
      onOpen: null,
      onClose: null
    },
    {
      id: 'selectAcompanhamentoModal',
      modal_hidden: selectAcompanhamentoModalHidden,
      content: selectAcompanhamentoModalHiddenContent,
      btnsConfigs: [],
      onOpen: null,
      onClose: null
    },
    {
      id: 'editarAcompanhamentoModal',
      modal_hidden: editarAcompanhamentoModalHidden,
      content: editarAcompanhamentoModalHiddenContent,
      btnsConfigs: [],
      onOpen: null,
      onClose: null
    },
  ];

  renderizarModal(pageCtn, modalsConfig);
}

function configurarFormulario() {
  sindicatoLaboralSelect = new SelectWrapper('#sindicato_laboral', { onOpened: async () => await obterSelectMock() });
  tipoAcordoSelect = new SelectWrapper('#tipo_acordo', { onOpened: async () => await obterSelectMock() });
  matrizSelect = new SelectWrapper('#matriz', { onOpened: async () => await obterSelectMock() });
  filialSelect = new SelectWrapper('#filial', { onOpened: async () => await obterSelectMock() });

  scriptSelect = new SelectWrapper('#script_select', { onOpened: async () => await obterSelectScript() });
  pautaSelect = new SelectWrapper('#pauta_select', { onOpened: async () => await obterSelectPauta() });
  premissasSelect = new SelectWrapper('#premissas_select', { onOpened: async () => await obterSelectPremissas() });
}

function configurarButtonsActions() {
  $("#tab-script").hide()
  $("#tab-pauta").hide()
  $("#view_pauta").hide()
  $("#view_comparar_pauta").hide()
  $("#tab-dirigentes").hide()
  $("#tab-premissas").hide()
  $("#tab-calculadora").hide()

  $("#btn-script").on('click', function () {
    $("#tab-script").show()

    $("#tab-pauta").hide()
    $("#view_pauta").hide()
    $("#view_comparar_pauta").hide()
    $("#tab-dirigentes").hide()
    $("#tab-premissas").hide()
    $("#tab-calculadora").hide()
  });

  $("#btn-pauta").on('click', function () {
    $("#tab-pauta").show()
    $("#view_pauta").show()

    $("#tab-script").hide()
    $("#view_comparar_pauta").hide()
    $("#tab-dirigentes").hide()
    $("#tab-premissas").hide()
    $("#tab-calculadora").hide()
  });

  $("#btn-comparar-pauta").on('click', function () {
    $("#view_comparar_pauta").show()

    $("#tab-script").hide()
    $("#view_pauta").hide()
    $("#tab-dirigentes").hide()
    $("#tab-premissas").hide()
    $("#tab-calculadora").hide()
  });

  $("#btn-dirigentes").on('click', function () {
    $("#tab-dirigentes").show()

    $("#tab-script").hide()
    $(".tab-pauta").hide()
    $("#view_pauta").hide()
    $("#view_comparar_pauta").hide()
    $("#tab-premissas").hide()
    $("#tab-calculadora").hide()
  });

  $("#btn-premissas").on('click', function () {
    $("#tab-premissas").show()

    $("#tab-script").hide()
    $("#tab-pauta").hide()
    $("#view_pauta").hide()
    $("#view_comparar_pauta").hide()
    $("#tab-dirigentes").hide()
    $("#tab-calculadora").hide()
  });

  $("#btn-calculadora").on('click', function () {
    $("#tab-calculadora").show()

    $("#tab-script").hide()
    $("#tab-pauta").hide()
    $("#view_pauta").hide()
    $("#view_comparar_pauta").hide()
    $("#tab-dirigentes").hide()
    $("#tab-premissas").hide()
  });
}

async function obterSelectMock() {
  const data = [{
    id: '',
    description: '--'
  }];

  return await Promise.resolve(data);
}

async function obterSelectScript() {
  const data = [{
    id: '',
    description: 'Fases'
  }];

  return await Promise.resolve(data);
}

async function obterSelectPauta() {
  const data = [{
    id: '',
    description: 'Histórico de Pautas'
  }];

  return await Promise.resolve(data);
}

async function obterSelectPremissas() {
  const data = [{
    id: '',
    description: 'Histórico de Premissa'
  }];

  return await Promise.resolve(data);
}