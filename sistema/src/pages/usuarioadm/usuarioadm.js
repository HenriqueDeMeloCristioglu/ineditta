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
  GrupoClausulaService,
  GrupoEconomicoService,
  JornadaService,
  UsuarioAdmService,
  ClienteUnidadeService,
  CnaeService,
  LocalizacaoService,
  ModuloService,
  EventosCalendarioService
} from '../../js/services'

// Utils
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import DatepickerWrapper from '../../js/utils/datepicker/datepicker-wrapper.js';
import NotificationService from '../../js/utils/notifications/notification.service.js';
import { renderizarModal, closeModal } from '../../js/utils/modals/modal-wrapper.js';

// Core
import { AuthService, ApiService, UserInfoService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js';

import SelectWrapper from '../../js/utils/selects/select-wrapper.js';
import Result from '../../js/core/result.js';
import Masker from '../../js/utils/masks/masker.js';
import { Generator } from '../../js/utils/util.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import { Button } from 'bootstrap';

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const usuarioadmService = new UsuarioAdmService(apiService, apiLegadoService);
const jornadaService = new JornadaService(apiService);
const grupoEconomicoService = new GrupoEconomicoService(apiService, apiLegadoService);
const grupoClausulaService = new GrupoClausulaService(apiService);
const cnaeService = new CnaeService(apiService);
const localizacaoService = new LocalizacaoService(apiService, apiLegadoService);
const moduloService = new ModuloService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const eventoCalendarioService = new EventosCalendarioService(apiService, apiLegadoService);

let usuarioAdmTb = null;
let jornadaTb = null;
let superiorTb = null;
let moduloSisapTb = null;
let moduloComercialTb = null;
let empresasTb = null;
let configurarCalendarioSindicalTb = null;

let datainiInput = null;
let datafimInput = null;

let atividadeEconomicaSelect = null;
let localidadeSelect = null;
let grupoEconomicoSelect = null;
let grupoClausulaSelect = null;
let nivelSelect = null;
let tipoUsuarioSelect = null;
let tipoNumeroSelect = null;
let definirAntesSelect = null;

let modulosSisap = [];
let modulosComerciais = [];
let estabelecimentosIds = [];
let usuariosTiposEventos = [];

let calendarioConfig = {};
let tipoIdSelecionado = null;
let subtipoIdSelecionado = null;

let requestId = Generator.id();

jQuery(async () => {
  new Menu()

  await AuthService.initialize();

  configurarModal();

  configurarFormulario();

  await carregarDatatable();
});

async function carregarDatatable() {
  usuarioAdmTb = new DataTableWrapper('#usuarioAdmTb', {
    ajax: async (requestData) => await usuarioadmService.obterDatatable(requestData),
    columns: [
      { "data": 'id' },
      { "data": "nome" },
      { "data": "email" },
      { "data": "cargo" },
      { "data": "telefone", render: (data) => Masker.phone(data) },
      { "data": "ramal" },
      { "data": "departamento" },
      { "data": "dataCriacao", render: (data) => Masker.dateTime(data) },
      { "data": "nomeUserCriador" }
    ],
    rowCallback: function (row, data) {
      const icon = $("<i>").addClass("fa fa-file-text");
      let button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon);
      button.on("click", function () {
        const id = $(this).attr("data-id");
        $('#usuarioId').val(id);
        $('#usuarioAdmBtn').trigger('click'); // Abrir modal
        alternarFuncionalidades(false)
      });
      $("td:eq(0)", row).html(button);
    },
    columnDefs: [{
      targets: "_all",
      defaultContent: ""
    }]
  });

  await usuarioAdmTb.initialize();
}

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  const modalNovoUsuario = document.getElementById('novoUsuarioModalHidden');
  const contentNovoUsuario = document.getElementById('novoUsuarioModalHiddenContent');

  const buttonsCadastrarConfig = [
    {
      id: 'btn_cadastrar',
      onClick: async (id, modalContainer) => {
        const result = await upsert();
        if (result.isSuccess()) {
          closeModal(modalContainer);
        }
      }
    }
  ]

  const modalJornada = document.getElementById('jornadaModalHidden');
  const contentJornada = document.getElementById('jornadaModalContent');

  const modalSuperior = document.getElementById('superiorModalHidden');
  const contentSuperior = document.getElementById('superiorModalContent');

  const moduloSisapModal = document.getElementById('moduloSisapModalHidden');
  const moduloSisapModalContent = document.getElementById('moduloSisapModalHiddenContent');

  const moduloComercialModal = document.getElementById('moduloComercialModalHidden');
  const moduloComercialModalContent = document.getElementById('moduloComercialModalHiddenContent');

  const empresasModal = document.getElementById('empresasModalHidden');
  const empresasModalContent = document.getElementById('empresasModalHiddenContent');

  const configurarCalendarioSindicalModal = document.getElementById('configurarCalendarioSindicalModalHidden');
  const configurarCalendarioSindicalModalContent = document.getElementById('configurarCalendarioSindicalModalHiddenContent');

  const definirNotificarAntesModal = document.getElementById('definirNotificarAntesModalHidden');
  const definirNotificarAntesModalContent = document.getElementById('definirNotificarAntesModalHiddenContent');


  const modalsConfig = [
    {
      id: 'novoUsuarioModal',
      modal_hidden: modalNovoUsuario,
      content: contentNovoUsuario,
      btnsConfigs: buttonsCadastrarConfig,
      onOpen: async () => {
        const id = $('#usuarioId').val();
        if (id) {
          await obterPorId(id);
        }
      },
      onClose: () => limpar(),
    },
    {
      id: 'jornadaModal',
      modal_hidden: modalJornada,
      content: contentJornada,
      btnsConfigs: [],
      onOpen: async () => await carregarJornada(),
      onClose: () => null,
      isInIndex: true
    },
    {
      id: 'superiorModal',
      modal_hidden: modalSuperior,
      content: contentSuperior,
      btnsConfigs: [],
      onOpen: async () => await carregarSuperior(),
      onClose: () => null,
      isInIndex: true
    }, {
      id: 'moduloSisapModal',
      modal_hidden: moduloSisapModal,
      content: moduloSisapModalContent,
      btnsConfigs: [],
      onOpen: async () => await carregarModulosSisap(),
      onClose: () => null,
      isInIndex: true
    }, {
      id: 'moduloComercialModal',
      modal_hidden: moduloComercialModal,
      content: moduloComercialModalContent,
      btnsConfigs: [],
      onOpen: async () => await carregarModulosComercial(),
      onClose: () => null,
      isInIndex: true
    }, {
      id: 'empresasModal',
      modal_hidden: empresasModal,
      content: empresasModalContent,
      btnsConfigs: [],
      onOpen: async () => {
        const gruposEconomicosIds = Array.isArray(grupoEconomicoSelect.getValue()) ?
        grupoEconomicoSelect.getValue().map(item => parseInt(item)) :
          [parseInt(grupoEconomicoSelect.getValue())];
        
        const params = {
          estabelecimentoIds: estabelecimentosIds,
          gruposIds: gruposEconomicosIds
        };
        const resultEstabelecimentosSelecionados = (await clienteUnidadeService.obterEstabelecimentosSelecionados(params)).value
        estabelecimentosIds = resultEstabelecimentosSelecionados.map((x) => x.id);
        return await carregarEmpresas();
      },
      onClose: () => null,
      isInIndex: true
    },
    {
      id: 'configurarCalendarioSindicalModal',
      modal_hidden: configurarCalendarioSindicalModal,
      content: configurarCalendarioSindicalModalContent,
      btnsConfigs: [],
      onOpen: async () => await carregarTiposSubtipos(),
      onClose: () => null,
      isInIndex: true
    },
    {
      id: 'definirNotificarAntesModal',
      modal_hidden: definirNotificarAntesModal,
      content: definirNotificarAntesModalContent,
      btnsConfigs: [],
      onOpen: async () => {
        $("#definirAntesSelect").val(usuariosTiposEventos.find(ute => ute.tipo == tipoIdSelecionado && ute.subtipo == subtipoIdSelecionado)?.notificarAntes ?? 5);
      },
      onClose: () => {
        if(calendarioConfig[tipoIdSelecionado+"-"+subtipoIdSelecionado]) {
          calendarioConfig[tipoIdSelecionado+"-"+subtipoIdSelecionado].notificarAntes = Number($("#definirAntesSelect").val());
        }
        else {
          calendarioConfig[tipoIdSelecionado+"-"+subtipoIdSelecionado] = {};
          calendarioConfig[tipoIdSelecionado+"-"+subtipoIdSelecionado].tipoId = tipoIdSelecionado;
          calendarioConfig[tipoIdSelecionado+"-"+subtipoIdSelecionado].subtipoId = subtipoIdSelecionado;
          calendarioConfig[tipoIdSelecionado+"-"+subtipoIdSelecionado].notificarAntes = Number($("#definirAntesSelect").val());
        }
        console.log(calendarioConfig);
        tipoIdSelecionado = null;
        subtipoIdSelecionado = null;
      },
      isInIndex: true
    }
  ];

  renderizarModal(pageCtn, modalsConfig);
}

function configurarFormulario() {
  alternarFuncionalidades(false)
  $('#celular').prop('disabled', true)
  $("#ramal").maskCustom('000000');
  $("#ausenciaInicio").maskDate();
  $("#ausenciaFim").maskDate();

  datainiInput = new DatepickerWrapper('#ausenciaInicio');
  datafimInput = new DatepickerWrapper('#ausenciaFim');

  atividadeEconomicaSelect = new SelectWrapper('#cnaes', { options: { multiple: true }, onOpened: async () => (await cnaeService.obterSelect()).value });
  localidadeSelect = new SelectWrapper('#localidade', { options: { multiple: true }, onOpened: async () => (await localizacaoService.obterSelect()).value });
  grupoEconomicoSelect = new SelectWrapper('#grupoEconomico', {
    onOpened: async () => {
      $("#empresasModalBtn").prop("disabled", false);
      return await grupoEconomicoService.obterSelectPorUsuario()
    }
  });
  grupoClausulaSelect = new SelectWrapper('#grupoClausulas', { options: { multiple: true }, onOpened: async () => (await grupoClausulaService.obterSelect()).value });
  tipoUsuarioSelect = new SelectWrapper('#tipo', {
    onOpened: async () => {
      const result = await carregarTipos();
      nivelSelect.enable();
      return result;
    },
    onSelected: (value) => tipoUsuarioSelecionado(value)
  });
  nivelSelect = new SelectWrapper('#nivel', { onOpened: async (tipoId) => await carregarNiveis(tipoId), parentId: '#tipo' });
  tipoNumeroSelect = new SelectWrapper('#tipo_numero_select', {
    onOpened: async () => {
      $("#celular").prop('disabled', false);
      $('#celular').maskPhone();
      return await carregarTipoNumero();
    },
    onSelected: (item) => {
      ($('#celular')).prop('disabled', false)

      if (item.id == 'telefone') return $('#celular').maskPhone()

      return $('#celular').maskCelPhone()
    }
  });



  if (UserInfoService.getTipo() !== 'Ineditta') {
    $("#moduloSisapModalAbrirBtn").attr("style", "display: none;")
    $("#jornadaSelecionarBtn").prop("disabled", true);
    $("#cnaes").prop("disabled", true);
    $("#localidade").prop("disabled", true);
  }
}

function tipoUsuarioSelecionado(item) {
  if (item?.id === 'Cliente') {
    nivelSelect.clear();
    nivelSelect.enable();

    $('#jornadaId').val('');
    $('#jornadaSelecionarBtn').attr('disabled', 'disabled');

    atividadeEconomicaSelect.clear();
    atividadeEconomicaSelect.disable();

    localidadeSelect.clear();
    localidadeSelect.disable();

    grupoEconomicoSelect.clear();
    grupoEconomicoSelect.single();

    return;
  }

  nivelSelect.clear();
  nivelSelect.disable();
  nivelSelect.addOption({ id: 'Ineditta', description: 'Ineditta' }, true);

  atividadeEconomicaSelect.clear();
  atividadeEconomicaSelect.enable();

  localidadeSelect.clear();
  localidadeSelect.enable();

  grupoEconomicoSelect.clear();
  grupoEconomicoSelect.multiple();

  $('#jornadaId').val('');
  $('#jornadaSelecionarBtn').removeAttr('disabled');
  $('#btnEnviarEmailAtualizacaoCredenciais').on('click', enviarEmailAtualizacaoCredenciais);
  $('#btnEnviarEmailBoasVindas').on('click', enviarEmailBoasVindas);
  $('#btnAtualizarPermissoes').on('click', atualizarPermissoes);
}

async function upsert() {
  const id = $('#usuarioId').val();

  const calendarioConfigArray = usuariosTiposEventos.map(ute => {
    return {
      id: ute.id,
      tipoId: ute.tipo,
      subtipoId: ute.subtipo,
      notificarEmail: ute.notificarEmail,
      notificarWhatsapp: ute.notificarWhatsapp,
      usuarioId: Number($('#usuarioId').val())
    }
  });

  return id ? await alterar(id, calendarioConfigArray) : await incluir(calendarioConfigArray);
}

async function incluir(calendarioConfigArray) {
  const request = {
    nome: $("#nome").val(),
    username: $("#username").val(),
    email: $("#email").val(),
    cargo: $("#cargo").val(),
    celular: $("#celular").val(),
    ramal: $("#ramal").val(),
    superiorId: $("#superiorId").val(),
    jornadaId: $("#jornadaId").val(),
    departamento: $("#departamento").val(),
    bloqueado: $("#bloqueado").is(":checked"),
    documentoRestrito: $("#documentoRestrito").is(":checked"),
    ausenciaInicio: datainiInput.getValue(),
    ausenciaFim: datafimInput.getValue(),
    tipo: tipoUsuarioSelect?.getValue(),
    nivel: nivelSelect?.getValue(),
    notificarWhatsapp: $("#notificarWhatsapp").is(":checked"),
    notificarEmail: $("#notificarEmail").is(":checked"),
    grupoEconomicoId: Array.isArray(grupoEconomicoSelect?.getValue()) ? grupoEconomicoSelect?.getValue()[0] : grupoEconomicoSelect?.getValue(),
    localidadesIds: localidadeSelect?.getValue()?.map(item => parseInt(item)),
    cnaesIds: atividadeEconomicaSelect?.getValue()?.map(item => parseInt(item)),
    gruposClausulasIds: grupoClausulaSelect?.getValue(),
    modulosSisap,
    modulosComerciais,
    estabelecimentosIds,
    calendarioConfig: calendarioConfigArray,
  }

  const result = await usuarioadmService.incluir(request);

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao realizar o cadastro.',
      message: result.error.response.data.errors[0].message
    });

    return;
  }

  return new Promise((resolve) => {
    NotificationService.success({
      title: 'Deseja enviar email de boas vindas?',
      showConfirmButton: true,
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Sim, enviar!',
      then: async (result) => {
        if (!result.isConfirmed) {
          NotificationService.success({
            title: "Sucesso",
            message: "Cadastro realizado com sucesso!",
            timer: 2000,
          });

          limpar();
          return resolve(Result.success());
        }

        await enviarEmailBoasVindas();

        NotificationService.success({ title: 'Email enviado com sucesso!' });

        limpar();
        resolve(Result.success());
      },
      timer: null,
    });
  });
}

async function alterar(id, calendarioConfigArray) {
  const request = {
    id: id,
    nome: $("#nome").val(),
    username: $("#username").val(),
    email: $("#email").val(),
    cargo: $("#cargo").val(),
    celular: $("#celular").val(),
    ramal: $("#ramal").val(),
    superiorId: $("#superiorId").val(),
    jornadaId: $("#jornadaId").val(),
    departamento: $("#departamento").val(),
    bloqueado: $("#bloqueado").is(":checked"),
    documentoRestrito: $("#documentoRestrito").is(":checked"),
    ausenciaInicio: datainiInput.getValue(),
    ausenciaFim: datafimInput.getValue(),
    tipo: tipoUsuarioSelect?.getValue(),
    nivel: nivelSelect?.getValue(),
    notificarWhatsapp: $("#notificarWhatsapp").is(":checked"),
    notificarEmail: $("#notificarEmail").is(":checked"),
    grupoEconomicoId: Array.isArray(grupoEconomicoSelect?.getValue()) ? grupoEconomicoSelect?.getValue()[0] : grupoEconomicoSelect?.getValue(),
    localidadesIds: localidadeSelect?.getValue()?.map(item => parseInt(item)),
    cnaesIds: atividadeEconomicaSelect?.getValue()?.map(item => parseInt(item)),
    gruposClausulasIds: grupoClausulaSelect?.getValue()?.map(item => parseInt(item)),
    modulosSisap,
    modulosComerciais,
    estabelecimentosIds,
    calendarioConfig: calendarioConfigArray
  };

  const result = await usuarioadmService.editar(request);

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao realizar o cadastro.',
      message: result.error.response.data.errors[0].message
    });

    return;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Cadastro atualizado com sucesso",
  });

  limpar();

  return Result.success();
}

async function obterPorId(id) {
  const result = await usuarioadmService.obterPorId(id);

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao obter o usuário.',
      message: result.error.response.data.errors[0].message
    });

    return;
  }

  const usuario = result.value;

  $('#nome').val(usuario.nome);
  $('#username').val(usuario.username);
  $('#email').val(usuario.email);
  $('#cargo').val(usuario.cargo);
  $('#celular').val(usuario.celular);
  $('#ramal').val(usuario.ramal);
  $('#departamento').val(usuario.departamento);
  $('#bloqueado').prop('checked', usuario.bloqueado);
  $('#documentoRestrito').prop('checked', usuario.documentoRestrito);
  $('#notificarWhatsapp').prop('checked', usuario.notificarWhatsapp);
  $('#notificarEmail').prop('checked', usuario.notificarEmail);

  datainiInput.setValue(usuario.ausenciaInicio);
  datafimInput.setValue(usuario.ausenciaFim);

  modulosSisap = usuario.modulosSisap ?? [];
  modulosComerciais = usuario.modulosComerciais ?? [];
  estabelecimentosIds = usuario.estabelecimentosIds ?? [];
  usuariosTiposEventos = usuario.usuariosTiposEventosCalendario ?? [];

  const tipos = await carregarTipos();
  const niveis = await carregarNiveis(usuario.tipo);

  if (usuario.tipo) {
    tipoUsuarioSelect.setCurrentValue({ id: usuario.tipo, description: tipos.find(item => item.id === usuario.tipo).description });
    tipoUsuarioSelecionado(usuario.tipo);
  }

  if (usuario.nivel) {
    nivelSelect.setCurrentValue({ id: usuario.nivel, description: niveis.find(item => item.id === usuario.nivel).description });
  }

  if (usuario.jornada) {
    inserirValorSelect("#jornadaId", usuario.jornada.id, usuario.jornada.description);
  }

  if (usuario.superior) {
    inserirValorSelect("#superiorId", usuario.superior.id, usuario.superior.description);
  }

  if (usuario.localidades) {
    localidadeSelect.setCurrentValue(usuario.localidades.map(item => ({ id: item.id, description: item.description })));
  }

  if (usuario.grupoEconomico) {
    grupoEconomicoSelect.setCurrentValue({ id: usuario.grupoEconomico.id, description: usuario.grupoEconomico.description });
  }

  if (usuario.gruposEconomicosConsultores) {
    grupoEconomicoSelect.clear();
    grupoEconomicoSelect.setCurrentValue(usuario.gruposEconomicosConsultores.map(item => ({ id: item.id, description: item.description })));
  }

  if (usuario.cnaes) {
    atividadeEconomicaSelect.setCurrentValue(usuario.cnaes.map(item => ({ id: item.id, description: item.description })));
  }

  if (usuario.gruposClausulas) {
    grupoClausulaSelect.setCurrentValue(usuario.gruposClausulas.map(item => ({ id: item.id, description: item.description })));
  }

  alternarFuncionalidades(true);
}

function limparFormulario() {
  datainiInput.setValue(null);
  datafimInput.setValue(null);

  atividadeEconomicaSelect.clear();
  localidadeSelect.clear();
  grupoEconomicoSelect.clear();
  grupoEconomicoSelect.single();
  grupoClausulaSelect.clear();
  nivelSelect.clear();
  tipoNumeroSelect?.clear();
  tipoUsuarioSelect.clear();

  $('#nome').val('');
  $('#username').val('');
  $('#email').val('');
  $('#cargo').val('');
  $('#celular').val('');
  $('#ramal').val('');
  $('#superiorId').val('');
  $('#jornadaId').val('');
  $('#departamento').val('');
  $('#bloqueado').prop('checked', false);
  $('#documentoRestrito').prop('checked', false);
  $('#notificarWhatsapp').prop('checked', false);
  $('#notificarEmail').prop('checked', false);
  $('#usuarioId').val('');

  modulosSisap = [];
  modulosComerciais = [];
  estabelecimentosIds = [];
  requestId = Generator.id();

  alternarFuncionalidades(false);
}

function alternarFuncionalidades(exibir) {
  if (exibir) {
    // Alterar
    $('#username').attr('disabled', true);

    $("#btnEnviarEmailAtualizacaoCredenciais").show();
    $("#btnAtualizarPermissoes").show();
    $("#btnEnviarEmailBoasVindas").show();
    return;
  }

  // Cadastrar
  $("#btnEnviarEmailBoasVindas").hide();
  $("#btnEnviarEmailAtualizacaoCredenciais").hide();
  $("#btnAtualizarPermissoes").hide();

  $('#username').attr('disabled', false);
}

function limpar() {
  limparFormulario();
  usuarioAdmTb.reload();
}

async function carregarNiveis(tipoId) {
  return tipoId == 'Ineditta' ?
    await Promise.resolve([{ id: 'Ineditta', description: 'Ineditta' }]) :
    await Promise.resolve([
      { id: 'Unidade', description: 'Estabelecimento' },
      { id: 'Matriz', description: 'Empresa' },
      { id: 'Grupo Econômico', description: 'Grupo Econômico' }
    ]);
}

async function carregarTipoNumero() {
  return Promise.resolve([
    {
      id: 'telefone',
      description: 'Telefone'
    },
    {
      id: 'celular',
      description: 'Celular'
    }
  ])
}

async function carregarTipos() {
  if (UserInfoService.getTipo() !== 'Ineditta') {
    return await Promise.resolve([
      { id: 'Cliente', description: 'Cliente' },
    ])
  }

  return await Promise.resolve([
    { id: 'Cliente', description: 'Cliente' },
    { id: 'Ineditta', description: 'Ineditta' },
  ]);
}

async function carregarJornada() {
  if (jornadaTb) {
    jornadaTb.reload();
    return;
  }

  jornadaTb = new DataTableWrapper('#jornadaTb', {
    ajax: async (requestData) => await jornadaService.obterDatatable(requestData),
    columns: [
      { "data": "id" },
      { "data": "descricao" },
      {
        "data": "jornada", render: (data) => {
          const rows = handleJornadaRows(JSON.parse(data))

          return "<table cellpadding='0' cellspacing='0' border='0' class='table table-striped table-bordered demo-tbl' style='width: 100%;'><thead><tr><th>Dia</th><th>Início</th><th>Fim</th></tr></thead><tbody>" + rows + "</tbody></table>"
        }
      }
    ],
    rowCallback: function (row, data) {
      const button = $(`<button type="button" data-dismiss="modal" data-id='${data.id}' data-nome='${data.descricao}' class="btn btn-secondary">Selecionar</button>`);

      button.on('click', (el) => {
        const id = el.target.attributes['data-id'].value;
        const nome = el.target.attributes['data-nome'].value;

        inserirValorSelect("#jornadaId", id, nome);
      });

      $("td:eq(0)", row).html(button);
    },
  });

  await jornadaTb.initialize();
}

function handleJornadaRows(data) {
  return '<tr><td>Segunda-feira</td><td>' + data.SEGUNDA.INICIO + '</td><td>' + data.SEGUNDA.FIM + '</td></tr><tr><td>Terça-feira</td><td>' + data.TERCA.INICIO + '</td><td>' + data.TERCA.FIM + '</td></tr><tr><td>Quarta-feira</td><td>' + data.QUARTA.INICIO + '</td><td>' + data.QUARTA.FIM + '</td></tr><tr><td>Quinta-feira</td><td>' + data.QUINTA.INICIO + '</td><td>' + data.QUINTA.FIM + '</td></tr><tr><td>Quinta-feira</td><td>' + data.SEXTA.INICIO + '</td><td>' + data.SEXTA.FIM + '</td></tr>';
}

async function carregarSuperior() {
  if (superiorTb) {
    await superiorTb.reload();
    return;
  }

  superiorTb = new DataTableWrapper('#superiorTb', {
    ajax: async (requestData) => await usuarioadmService.obterDatatable(requestData),
    columns: [
      { "data": 'id' },
      { "data": "nome" },
      { "data": "email" },
      { "data": "cargo" },
      { "data": "telefone" },
      { "data": "ramal" },
      { "data": "departamento" }
    ],
    rowCallback: function (row, data) {
      const button = $(`<button type="button" data-dismiss="modal" data-id='${data.id}' data-nome='${data.nome}' class="btn btn-secondary">Selecionar</button>`);

      button.on('click', (el) => {
        const id = el.target.attributes['data-id'].value;
        const nome = el.target.attributes['data-nome'].value;

        inserirValorSelect("#superiorId", id, nome);
      });

      $("td:eq(0)", row).html(button);
    },
  });

  await superiorTb.initialize();
}

async function carregarModulosSisap() {
  if (moduloSisapTb) {
    await moduloSisapTb.reload();
    return;
  }

  moduloSisapTb = new DataTableWrapper('#moduloSisapTb', {
    ajax: async (requestData) => await moduloService.obterSISAPDatatable(requestData),
    columns: [
      { "data": "nome" },
      { "data": "criar" },
      { "data": "consultar" },
      { "data": "comentar" },
      { "data": "alterar" },
      { "data": "excluir" },
      { "data": "aprovar" },
      { "data": "id" }
    ],
    rowCallback: function (row, data) {
      const acoes = ['criar', 'consultar', 'comentar', 'alterar', 'excluir', 'aprovar'];

      acoes.forEach((acao, index) => {
        const checkbox = $(`<input type="checkbox" data-id='${data.id}' data-acao='${acao}' class='form-check-input c chkitem' />`);

        if (moduloSisapSelecionado(data.id, acao)) {
          checkbox.prop('checked', true);
        }

        checkbox.on('change', (el) => {
          const checked = el.target.checked;
          const id = el.target.attributes['data-id'].value;
          const acao = el.target.attributes['data-acao'].value;

          atualizarModuloSisap(id, acao, checked);
        });

        $(`td:eq(${index + 1})`, row).html(checkbox);
      });

      const checkboxTodos = $(`<input type="checkbox" data-id='${data.id}' data-acao='todos' class='form-check-input c' />`);

      checkboxTodos.on('change', (el) => {
        const checked = el.target.checked;
        $(row).find('.chkitem').prop('checked', checked);
        $(row).find('.chkitem').trigger('change');
      });

      $(`td:eq(7)`, row).html(checkboxTodos);
    },
  });

  await moduloSisapTb.initialize();
}

async function carregarModulosComercial() {
  if (moduloComercialTb) {
    await moduloComercialTb.reload();
    return;
  }

  moduloComercialTb = new DataTableWrapper('#moduloComercialTb', {
    ajax: async (requestData) => await moduloService.obterComercialDatatable(requestData),
    columns: [
      { "data": "nome" },
      { "data": "criar" },
      { "data": "consultar" },
      { "data": "comentar" },
      { "data": "alterar" },
      { "data": "excluir" },
      { "data": "aprovar" },
      { "data": "id" }
    ],
    rowCallback: function (row, data) {
      const acoes = ['criar', 'consultar', 'comentar', 'alterar', 'excluir', 'aprovar'];

      acoes.forEach((acao, index) => {
        const checkbox = $(`<input type="checkbox" data-id='${data.id}' data-acao='${acao}' class='form-check-input c chkitem' />`);

        if (moduloComercialSelecionado(data.id, acao)) {
          checkbox.prop('checked', true);
        }

        checkbox.on('change', (el) => {
          const checked = el.target.checked;
          const id = el.target.attributes['data-id'].value;
          const acao = el.target.attributes['data-acao'].value;

          atualizarModuloComercial(id, acao, checked);
        });

        $(`td:eq(${index + 1})`, row).html(checkbox);
      });

      const checkboxTodos = $(`<input type="checkbox" data-id='${data.id}' data-acao='todos' class='form-check-input c' />`);

      checkboxTodos.on('change', (el) => {
        const checked = el.target.checked;
        $(row).find('.chkitem').prop('checked', checked);
        $(row).find('.chkitem').trigger('change');
      });

      $(`td:eq(7)`, row).html(checkboxTodos);
    },
  });

  await moduloComercialTb.initialize();
}

async function carregarTiposSubtipos() {
  if (configurarCalendarioSindicalTb) {
    await configurarCalendarioSindicalTb.reload();
    return;
  }

  configurarCalendarioSindicalTb = new DataTableWrapper('#configurarCalendarioSindicalTb', {
    ajax: async (requestData) => {
      const result = await eventoCalendarioService.obterTiposSubtiposEvento(requestData);
      return result;
    },
    columns: [
      { "data": "nome" },
      { "data": "id" },
      { "data": "id" }
    ],
    rowCallback: function (row, data) {
      const tipoId = data.tipoAssociado ? data.tipoAssociado : data.id;
      const subtipoId = data.tipoAssociado ? data.id : null;

      const acoes = ['notificarEmail', 'notificarWhatsapp'];

      acoes.forEach((acao, index) => {
        const checkbox = $(`<input type="checkbox" data-tipo='${tipoId}' data-subtipo='${subtipoId}' data-acao='${acao}' class='form-check-input c chkitem' />`);

        const usuarioTipoEventoConfig = usuariosTiposEventos.find(ute => ute.tipo == tipoId && ute.subtipo == subtipoId);

        if (usuarioTipoEventoConfig) {
          checkbox.prop('checked', usuarioTipoEventoConfig[acao]);
        }

        checkbox.on('change', (el) => {
          const checked = el.target.checked;
          const tipoId = el.target.attributes['data-tipo'].value;
          const subtipoId = el.target.attributes['data-subtipo'].value == 'null' ? null : el.target.attributes['data-subtipo'].value;
          const acao = el.target.attributes['data-acao'].value;

          atualizarCalendarioConfig(tipoId, subtipoId, acao, checked);
        });

        $(`td:eq(${index + 1})`, row).html(checkbox);

        // data-toggle="modal" data-target="#definirNotificarAntesModal"
      });
    },
  });

  await configurarCalendarioSindicalTb.initialize();
}

async function carregarEmpresas() {
  if (empresasTb) {
    await empresasTb.reload();
    return;
  }

  empresasTb = new DataTableWrapper('#empresasTb', {
    columns: [
      { "data": "id", orderable: false, title: "Selecionar" },
      { "data": "nomeGrupoEconomico", title: "Grupo" },
      { "data": "filial", title: "Empresa" },
      { "data": "cnpj", title: "CNPJ", render: (data) => Masker.CNPJ(data) },
      { "data": "nome", title: "Estabelecimento" }
    ],
    scrollY: 500,
    scrollCollapse: true,
    ajax: async (requestData) => {
      $('#select_all_empresas').val(false).prop('checked', false);

      const gruposEconomicosIds = Array.isArray(grupoEconomicoSelect.getValue()) ?
        grupoEconomicoSelect.getValue().map(item => parseInt(item)) :
        [parseInt(grupoEconomicoSelect.getValue())];

      requestData.SortColumn = 'id'
      requestData.Columns = "id,nome,filial,grupo,cnpj,nomeGrupoEconomico"
      requestData.GruposEconomicosIds = gruposEconomicosIds
      return await clienteUnidadeService.obterDatatable(requestData)
    },
    rowCallback: function (row, data) {
      const checkbox = $("<input>").attr("type", "checkbox").attr("data-id", data?.id).attr('checked', false).addClass('empresas')

      estabelecimentosIds.map(item => {
        if (item == data?.id) {
          checkbox.prop('checked', true)
          checkbox.trigger('change')
        }
      })

      checkbox.on('change', (el) => {
        const checked = el.target.checked;
        const id = el.target.attributes['data-id'].value;

        atualizarEstabelecimentos(id, checked);
      });

      $("td:eq(0)", row).html(checkbox);
    },
  });

  await empresasTb.initialize();

  $("#select_all_empresas").on("click", (event) => {
    if (event.currentTarget.checked) {
      $('.empresas').prop('checked', true);
      $('.empresas').trigger('change');
    } else {
      $('.empresas').prop('checked', false);
      $('.empresas').trigger('change');
    }
  })
}

function atualizarModuloSisap(id, acao, checked) {
  let modulo = modulosSisap.find(o => o.id == id);

  if (modulo) {
    modulo[acao] = checked;
    return;
  }

  modulo = { id: id };
  modulo[acao] = checked;
  modulosSisap.push(modulo);
}

function moduloSisapSelecionado(id, acao) {
  const modulo = modulosSisap.find(o => o.id == id);

  return modulo ? modulo[acao] : false;
}

function atualizarModuloComercial(id, acao, checked) {
  let modulo = modulosComerciais.find(o => o.id == id);

  if (modulo) {
    modulo[acao] = checked;
    return;
  }

  modulo = { id: id };
  modulo[acao] = checked;
  modulosComerciais.push(modulo);
}

function atualizarCalendarioConfig(tipoId, subtipoId, acao, checked) {
  const config = usuariosTiposEventos.find(ute => ute.tipo == tipoId && ute.subtipo == subtipoId);
  
  if (config) {
    config[acao] = checked; 
  }
  else {
    const newConfig = {
      tipo: Number(tipoId),
      subtipo: isNaN(parseInt(subtipoId)) ? null : parseInt(subtipoId),
    }
    newConfig[acao] = checked;
    usuariosTiposEventos.push(newConfig);
  }

  console.log(usuariosTiposEventos);
}

function moduloComercialSelecionado(id, acao) {
  const modulo = modulosComerciais.find(o => o.id == id);

  return modulo ? modulo[acao] : false;
}

function atualizarEstabelecimentos(id, checked) {
  // let estabelecimento = estabelecimentosIds.find(estabelecimentoId => estabelecimentoId == id)
  if (checked == true) {
    const estabelecimento = estabelecimentosIds.find(estabelecimentoId => estabelecimentoId == id);
    if (!estabelecimento) {
      estabelecimentosIds.push(id);
    }
    return;
  }

  estabelecimentosIds = estabelecimentosIds.filter(estabelecimentoId => estabelecimentoId != id)
}

function inserirValorSelect(input, id, value) {
  $(input).empty();

  if (id) {
    $(input)
      .append(`<option selected="selected" value="${id}">${value}</option>`);

    $(input).val(id);
    $(input).prop("disabled", true);
  }
}

async function enviarEmailAtualizacaoCredenciais() {
  const usuarioId = $('#usuarioId').val();

  if (!usuarioId) {
    return;
  }

  const result = await usuarioadmService.enviarEmailAtualizacaoCredenciais(usuarioId);

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao enviar e-mail.',
      message: result.error.response.data.errors[0].message
    });

    return;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "E-mail enviado com sucesso!",
  });
}

async function enviarEmailBoasVindas() {
  const email = $('#email').val();
  const username = $('#username').val();

  if (!email || !username) {
    return NotificationService.warning({
      title: 'Campo em branco!'
    });
  }

  const result = await usuarioadmService.enviarEmailBoasVindas(email, requestId);

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao enviar e-mail.',
      message: result.error.response.data.errors[0].message
    });
    return;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "E-mail enviado com sucesso!",
  });

  requestId = Generator.id();
}

async function atualizarPermissoes() {
  const usuarioId = $('#usuarioId').val();

  if (!usuarioId) {
    return;
  }

  const result = await usuarioadmService.atualizarPermissoes(usuarioId);

  if (result.isFailure()) {
    NotificationService.error({
      title: 'Erro ao atualizar permissões.',
      message: result.error.response.data.errors[0].message
    });

    return;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Permissões atualizadas com sucesso!",
  });
}