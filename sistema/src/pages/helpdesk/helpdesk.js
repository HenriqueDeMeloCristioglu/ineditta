import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';
import 'datatables.net-responsive-bs5';

import JQuery from 'jquery';
import $ from 'jquery';

import { renderizarModal } from '../../js/utils/modals/modal-wrapper.js';
import SelectWrapper from '../../js/utils/selects/select-wrapper.js';
import NotificationService from '../../js/utils/notifications/notification.service.js';
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';

// Core
import { ApiService, AuthService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js';

// Services
import {
  ClienteUnidadeService,
  UsuarioAdmService,
  GrupoEconomicoService,
  FileStorageService,
  GestaoDeChamadoService,
  MatrizService,
  ModuloService,
  SindicatoLaboralService,
  SindicatoPatronalService
} from '../../js/services'

// Application
import { UsuarioNivel } from '../../js/application/usuarios/constants/usuario-nivel.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const gestaoDeChamadoService = new GestaoDeChamadoService(apiService);
const moduloService = new ModuloService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const usuarioAdmService = new UsuarioAdmService(apiService);
const grupoEconomicoService = new GrupoEconomicoService(apiService);
const matrizService = new MatrizService(apiService);
const sindicatoLaboralService = new SindicatoLaboralService(apiService);
const sindicatoPatronalService = new SindicatoPatronalService(apiService);
const fileStorageService = new FileStorageService(apiLegadoService);

let gestaoDeChamadoTb = null;
let grupoEconomicoSelect = null;
let matrizSelect = null;
let estabelecimentoSelect = null;
let tipoChamadoSelect = null;
let sindicatoLaboralSelect = null;
let sindicatoPatronalSelect = null;

let dadosPessoais = null;

JQuery(async function () {
  new Menu()

  await AuthService.initialize();

  const dadosPessoaisResult = await usuarioAdmService.obterDadosPessoais();

  if (dadosPessoaisResult.isFailure()) {
    return;
  }

  dadosPessoais = dadosPessoaisResult.value;

  configurarModal();

  configurarFormulario();

  await carregarDatatable();
})


function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  // Add Helpdesk
  const addHelpdesktModalHidden = document.getElementById('addHelpdeskModalHidden');
  const addHelpdesktModalHiddenContent = document.getElementById('addHelpdeskModalContent');

  // Sem Helpdesk
  const semHelpdeskModalHidden = document.getElementById('semHelpdeskModalHidden');
  const semHelpdeskModalHiddenContent = document.getElementById('semHelpdeskModalContent');

  // Timeline Helpdesk
  const timelineHelpdeskModalHidden = document.getElementById('timelineHelpdeskModalHidden');
  const timelineHelpdeskModalHiddenContent = document.getElementById('timelineHelpdeskModalContent');

  // Responsavel Helpdesk
  const responsavelHelpdeskModalHidden = document.getElementById('responsavelHelpdeskModalHidden');
  const responsavelHelpdeskModalHiddenContent = document.getElementById('responsavelHelpdeskModalContent');

  const modalsConfig = [
    {
      id: 'addHelpdesktModal',
      modal_hidden: addHelpdesktModalHidden,
      content: addHelpdesktModalHiddenContent,
      btnsConfigs: [],
      onOpen: abrirModal,
      onClose: limpar
    },
    {
      id: 'semHelpdeskModal',
      modal_hidden: semHelpdeskModalHidden,
      content: semHelpdeskModalHiddenContent,
      btnsConfigs: [],
      onOpen: null,
      onClose: null
    },
    {
      id: 'timelineHelpdeskModal',
      modal_hidden: timelineHelpdeskModalHidden,
      content: timelineHelpdeskModalHiddenContent,
      btnsConfigs: [],
      onOpen: null,
      onClose: null
    },
    {
      id: 'responsavelHelpdeskModal',
      modal_hidden: responsavelHelpdeskModalHidden,
      content: responsavelHelpdeskModalHiddenContent,
      btnsConfigs: [],
      onOpen: null,
      onClose: null
    },
  ];

  renderizarModal(pageCtn, modalsConfig);
}

async function carregarDatatable() {
  gestaoDeChamadoTb = new DataTableWrapper('#gestaoDeChamadoTb', {
    ajax: async (requestData) =>
      await gestaoDeChamadoService.obterDatatable(requestData),
    columns: [
      { "data": 'id' },
      { "data": 'id' },
      { "data": "tipo" },
      { "data": "nomeSolicitante" },
      { "data": "inicioResposta" },
      { "data": "dataAbertura" },
      { "data": "dataVencimento" },
      { "data": "nomeResponsavel" },
      { "data": "status" },
      { "data": "id" }
    ],
    rowCallback: function (row, data) {
      const docIcon = $("<i>").addClass("fa fa-file-text");
      let buttonDoc = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(docIcon)
      buttonDoc.on("click", function () {
        const id = $(this).attr("data-id");
        $('#id-input').val(id)
        $('#addHelpdesktModalBtn').trigger('click')
      });
      $("td:eq(0)", row).html(buttonDoc)

      const timelineIcon = $("<i>").addClass("fa fa-table");
      let buttonTimeline = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(timelineIcon)
      buttonTimeline.on("click", function () {
        const id = $(this).attr("data-id");
        $('#id-input').val(id)
        $('#timelineHelpdeskModalBtn').trigger('click')
      });
      $("td:eq(9)", row).html(buttonTimeline)
    },
  });

  await gestaoDeChamadoTb.initialize();
}

function configurarFormulario() {
  tipoChamadoSelect = new SelectWrapper('#tipo_chamado', { onOpened: async () => await moduloService.obterModulosPorTipo('helpdesk'), sortable: true, onSelected: tipoChamadoSelecionado });
  estabelecimentoSelect = new SelectWrapper('#estabelecimento', {
    options: {
      placeholder: 'Selecione',
      multiple: true
    },
    parentId: '#matriz',
    onOpened: async (matrizId) => await clienteUnidadeService.obterSelectPorUsuario(matrizId),
    markOptionAsSelectable: dadosPessoais.nivel == UsuarioNivel.Ineditta ? () => true : () => false
  });
  matrizSelect = new SelectWrapper('#matriz', { options: { placeholder: 'Selecione', multiple: true }, parentId: '#grupo', onOpened: async (grupoEconomicoId) => await matrizService.obterSelectPorUsuario(grupoEconomicoId), markOptionAsSelectable: dadosPessoais.nivel == UsuarioNivel.Ineditta || UsuarioNivel.GrupoEconomico ? () => true : () => false });
  grupoEconomicoSelect = new SelectWrapper('#grupo', { options: { placeholder: 'Selecione', multiple: true }, onOpened: async () => await grupoEconomicoService.obterSelectPorUsuario(), markOptionAsSelectable: dadosPessoais.nivel != UsuarioNivel.Estabelecimento ? () => true : () => false });
  sindicatoLaboralSelect = new SelectWrapper('#sind_labo', { onOpened: async () => await obterSindicatosLaborais(), sortable: true });
  sindicatoPatronalSelect = new SelectWrapper('#sind_patro', { onOpened: async () => await obterSindicatosPatronais(), sortable: true });

  document.getElementById('id_user_res').value = dadosPessoais.id;
  document.getElementById('id_user_logged').value = dadosPessoais.id;
}


async function obterSindicatosPatronais() {
  const parameters = { clientesUnidadesIds: estabelecimentoSelect.getValue() ? [estabelecimentoSelect.getValue()] : [] };
  return await sindicatoPatronalService.obterSelectPorUsuario(parameters);
}

async function obterSindicatosLaborais() {
  const parameters = { clientesUnidadesIds: estabelecimentoSelect.getValue() ? [estabelecimentoSelect.getValue()] : [] };
  return await sindicatoLaboralService.obterSelectPorUsuario(parameters);
}

async function abrirModal() {
  if (dadosPessoais.nivel != UsuarioNivel.Ineditta) {
    grupoEconomicoSelect.disable()
    grupoEconomicoSelect.single()
    await grupoEconomicoSelect.loadOptions();
  } else {
    grupoEconomicoSelect.enable()
    grupoEconomicoSelect.multiple()
  }

  if (dadosPessoais.nivel == UsuarioNivel.Empresa || dadosPessoais.nivel == UsuarioNivel.Estabelecimento) {
    matrizSelect.disable()
    await matrizSelect.loadOptions()
  } else {
    matrizSelect.enable()
  }

  if (dadosPessoais.nivel == UsuarioNivel.Estabelecimento) {
    estabelecimentoSelect.disable()
    await estabelecimentoSelect.loadOptions()
  } else {
    estabelecimentoSelect.enable()
  }
}

function limpar() {
  limparFormulario();
  gestaoDeChamadoTb?.reload();
}

function limparFormulario() {
  tipoChamadoSelect?.clear();
  estabelecimentoSelect?.clear();
  matrizSelect?.clear();
  grupoEconomicoSelect?.clear();
  sindicatoLaboralSelect?.clear();
  sindicatoPatronalSelect?.clear();
}

function tipoChamadoSelecionado(tipoChamado) {
  if (!tipoChamado) {
    return;
  }

  const regra = obterRegrasTipoChamado()[tipoChamado.text];

  if (!regra) {
    return;
  }

  if (regra['#estabelecimento'] && dadosPessoais.nivel != UsuarioNivel.Estabelecimento) {
    estabelecimentoSelect?.enable();
  }
  else {
    estabelecimentoSelect?.disable();
  }

  if (regra['#sind_labo']) {
    sindicatoLaboralSelect?.enable();
  }
  else {
    sindicatoLaboralSelect?.disable();
    sindicatoLaboralSelect?.clear();
  }

  if (regra['#sind_patro']) {
    sindicatoPatronalSelect?.enable();
  }
  else {
    sindicatoPatronalSelect?.disable();
    sindicatoPatronalSelect?.clear();
  }

  if (regra['#clausula']) {
    $('#clausula').prop('disabled', false);
  }
  else {
    $('#clausula').prop('disabled', true);
    $('#clausula').val('');
  }

  if (regra['#file']) {
    $('#file').prop('disabled', false);
  }
  else {
    $('#file').prop('disabled', true);
    $('#file').val('');
  }

  if (tipoChamado.text === 'Solicitar Enquadramento Sindical') {
    NotificationService.info({ title: 'Informação', message: 'Enviar o cartão CNPJ, Contrato Social e Relação de Cargos para Análise.', timer: 5000 });
  }
}

function obterRegrasTipoChamado() {
  return {
    'Atualizar Informações do Estabelecimento': {
      "#estabelecimento": false,
      "#sind_labo": true,
      "#sind_patro": true,
      "#clausula": true,
      "#file": false
    },
    'Atualizar Informações do Sindicato Laboral': {
      "#estabelecimento": true,
      "#sind_labo": false,
      "#sind_patro": true,
      "#clausula": true,
      "#file": false
    },
    'Atualizar Informações do Sindicato Patronal': {
      "#estabelecimento": true,
      "#sind_labo": true,
      "#sind_patro": false,
      "#clausula": true,
      "#file": false
    },
    'Inclusão de Documentos': {
      "#estabelecimento": false,
      "#sind_labo": false,
      "#sind_patro": false,
      "#clausula": true,
      "#file": false
    },
    'Dúvida Cláusula CCT': {
      "#estabelecimento": false,
      "#sind_labo": false,
      "#sind_patro": false,
      "#clausula": false,
      "#file": false
    },
    'Dúvida abertura nos Domingos': {
      "#estabelecimento": false,
      "#sind_labo": false,
      "#sind_patro": false,
      "#clausula": true,
      "#file": false
    },
    'Dúvida abertura nos Feriados': {
      "#estabelecimento": false,
      "#sind_labo": false,
      "#sind_patro": false,
      "#clausula": true,
      "#file": false
    },
    'Dúvida Acompanhamento CCT': {
      "#estabelecimento": false,
      "#sind_labo": false,
      "#sind_patro": false,
      "#clausula": true,
      "#file": false
    },
    'Solicitar Adesão REPIS': {
      "#estabelecimento": false,
      "#sind_labo": false,
      "#sind_patro": false,
      "#clausula": true,
      "#file": false
    },
    'Solicitar Negociação de Acordo Coletivo': {
      "#estabelecimento": false,
      "#sind_labo": false,
      "#sind_patro": false,
      "#clausula": true,
      "#file": false
    },
    'Solicitar Negociação de Contribuições': {
      "#estabelecimento": false,
      "#sind_labo": false,
      "#sind_patro": false,
      "#clausula": true,
      "#file": false
    },
    'Solicitar Enquadramento Sindical': {
      "#estabelecimento": false,
      "#sind_labo": false,
      "#sind_patro": false,
      "#clausula": true,
      "#file": false
    },
    'Solicitar Registro no Mediador': {
      "#estabelecimento": false,
      "#sind_labo": false,
      "#sind_patro": false,
      "#clausula": true,
      "#file": false
    },
    'Solicitar Suporte TI': {
      "#estabelecimento": false,
      "#sind_labo": true,
      "#sind_patro": true,
      "#clausula": true,
      "#file": false
    },
    'Solicitar Treinamento do Sistema': {
      "#estabelecimento": false,
      "#sind_labo": true,
      "#sind_patro": true,
      "#clausula": true,
      "#file": false
    },
    'Outras Solicitações': {
      "#estabelecimento": false,
      "#sind_labo": true,
      "#sind_patro": true,
      "#clausula": true,
      "#file": false
    }
  };
}

// eslint-disable-next-line no-unused-vars
async function incluirArquivo() {
  if (!document.getElementById('file').files[0]) {
    return;
  }

  const params = {
    'file_helpdesk': document.getElementById('file').files[0]
  };

  const result = await fileStorageService.upload(params);

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro', message: result.error ?? 'Não foi possível realizar o upload do arquivo!' });
    return;
  }

  NotificationService.success({ title: 'Sucesso', message: 'Arquivo enviado com sucesso!' });
}

// eslint-disable-next-line no-unused-vars
async function atualizarArquivo() {
  if (document.querySelector('#file_up')?.files?.length === 0) {
    return;
  }

  const params = {
    'file_helpdesk': document.getElementById('file').files[0]
  };

  const result = await fileStorageService.upload(params);

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro', message: result.error ?? 'Não foi possível realizar o upload do arquivo!' });
    return;
  }

  NotificationService.success({ title: 'Sucesso', message: 'Arquivo enviado com sucesso!' });
}