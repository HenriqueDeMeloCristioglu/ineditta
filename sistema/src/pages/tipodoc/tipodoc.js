import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';

import $ from 'jquery';
import JQuery from 'jquery';
import '../../js/utils/masks/jquery-mask-extensions.js';

import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';

import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import SelectWrapper from '../../js/utils/selects/select-wrapper.js';
import { renderizarModal, closeModal } from '../../js/utils/modals/modal-wrapper.js';
import NotificationService from '../../js/utils/notifications/notification.service.js';
import Result from '../../js/core/result.js';

// Core
import { AuthService, ApiService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js'

import { TipoDocService } from '../../js/services'

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const tipoDocService = new TipoDocService(apiService, apiLegadoService);

let tipoDocumento = {}

let tipoSelecaoSelect = null
let tipoDocumentoSelect = null
let moduloSelect = null
let processadoSelect = null

let tipoDocTb = null

JQuery(async function () {
  new Menu()

  await AuthService.initialize();

  configurarModal();
  await carregarTipoDocumentoDatatable();
  configurarFormulario();
})

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  const upsertDocumentoModalHidden = document.getElementById('upsertDocumentoModalHidden');
  const upsertDocumentoModalContent = document.getElementById('upsertDocumentoModalContent');

  const documentoModalButtonsConfig = [
    {
      id: 'upsertDocumentoModalBtn',
      onClick: async (_, modalContainer) => {
        const result = await upsert(tipoDocumento.id)
        if (result.isSuccess()) {
          closeModal(modalContainer);
        }
      }
    }
  ]

  const modalsConfig = [
    {
      id: 'upsertDocumentoModal',
      modal_hidden: upsertDocumentoModalHidden,
      content: upsertDocumentoModalContent,
      btnsConfigs: documentoModalButtonsConfig,
      onOpen: async () => {
        if (tipoDocumento.id) {
          await obterPorId(tipoDocumento.id);
        }
      },
      onClose: () => limparFormulario()
    }
  ];

  renderizarModal(pageCtn, modalsConfig);
}

async function upsert(id) {
  return id ? await atualizar(id) : await incluir();
}

async function incluir() {
  const requestData = {
    nome: $("#nome").val(),
    tipo: tipoSelecaoSelect.getValue() == 0 ? $("#novo-tipo-documento").val() : tipoDocumentoSelect.getValue(),
    sigla: $("#sigla").val(),
    processado: processadoSelect.getValue(),
    modulo: Number(moduloSelect.getValue())
  }

  const result = await tipoDocService.incluir(requestData)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Não foi posssível realizar o cadastro', message: result.error })
  }

  NotificationService.success({ title: 'Cadastro realizado com sucesso!' })

  tipoDocTb.reload()
  return Result.success()
}

async function atualizar(id) {
  const requestData = {
    id: id,
    nome: $("#nome").val(),
    tipo: tipoDocumentoSelect.getValue() === null ? $("#novo-tipo-documento").val() : tipoDocumentoSelect.getValue(),
    sigla: $("#sigla").val(),
    processado: processadoSelect.getValue(),
    modulo: Number(moduloSelect.getValue())
  }

  const result = await tipoDocService.editar(requestData)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Não foi posssível realizar a atualização', message: result.error })
  }

  NotificationService.success({ title: 'Atualização realizada com sucesso!' })

  tipoDocTb.reload()
  return Result.success()
}

async function obterPorId(id) {
  const result = await tipoDocService.obterPorId(id)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter tipo de documento', message: result.error })
  }

  tipoDocumento = result.value

  $("#sigla").val(tipoDocumento.sigla);
  $("#nome").val(tipoDocumento.nome);

  const tipoDocumentosExistentes = await obterTipoDocumentoSelect()

  if (tipoDocumentosExistentes.find(item => item.id === tipoDocumento.tipo)) {
    tipoDocumentoSelect.setCurrentValue({
      id: tipoDocumento.tipo,
      description: tipoDocumento.tipo
    })
    $('#tipo-documento-existente').show()
    $('#novo-documento').hide()

    tipoSelecaoSelect.setCurrentValue({
      id: 1,
      description: 'Tipo de documento existente'
    })
  } else {
    $("#novo-documento").val(tipoDocumento.tipo);
    $('#tipo-documento-existente').hide()
    $('#novo-documento').show()
    tipoSelecaoSelect.setCurrentValue({
      id: 0,
      description: 'Novo tipo de documento'
    })
  }

  moduloSelect.setCurrentValue({
    id: tipoDocumento.modulo == 'Geral' ? 1 : 2,
    description: tipoDocumento.modulo
  })
  processadoSelect.setCurrentValue({
    id: tipoDocumento.processado,
    description: tipoDocumento.processado === 'N' ? 'Não' : 'Sim'
  })

  tipoDocumento.assunto === 'S' ? $("#assunto").prop("checked", true) : $("#assunto").prop("checked", false);
  tipoDocumento.validadeInicial === 'S' ? $("#validade_inicial").prop("checked", true) : $("#validade_inicial").prop("checked", false);
  tipoDocumento.sindicatoLaboral === 'S' ? $("#sindicato_laboral").prop("checked", true) : $("#sindicato_laboral").prop("checked", false);
  tipoDocumento.tipoUnidade === 'S' ? $("#tipo_unidade").prop("checked", true) : $("#tipo_unidade").prop("checked", false);

  tipoDocumento.descricao === 'S' ? $("#descricao").prop("checked", true) : $("#descricao").prop("checked", false);
  tipoDocumento.validadeFinal === 'S' ? $("#validade_final").prop("checked", true) : $("#validade_final").prop("checked", false);
  tipoDocumento.abrangencia === 'S' ? $("#abrangencia").prop("checked", true) : $("#abrangencia").prop("checked", false);
  tipoDocumento.sindicatoPatronal === 'S' ? $("#sindicato_patronal").prop("checked", true) : $("#sindicato_patronal").prop("checked", false);

  tipoDocumento.origem === 'S' ? $("#origem").prop("checked", true) : $("#origem").prop("checked", false);
  tipoDocumento.dataBase === 'S' ? $("#data_base").prop("checked", true) : $("#data_base").prop("checked", false);
  tipoDocumento.atividadeEconomica === 'S' ? $("#atividade_economica").prop("checked", true) : $("#atividade_economica").prop("checked", false);
  tipoDocumento.numero === 'S' ? $("#numero_legislacao").prop("checked", true) : $("#numero_legislacao").prop("checked", false);

  tipoDocumento.versao === 'S' ? $("#versao").prop("checked", true) : $("#versao").prop("checked", false);
  tipoDocumento.estabelecimento === 'S' ? $("#estabelecimento").prop("checked", true) : $("#estabelecimento").prop("checked", false);
  tipoDocumento.fonte === 'S' ? $("#fonte").prop("checked", true) : $("#fonte").prop("checked", false);
  tipoDocumento.permitirCompartilhar === 'S' ? $("#permitir_compartilhar").prop("checked", true) : $("#permitir_compartilhar").prop("checked", false);
  tipoDocumento.anuencia === 'S' ? $("#anuencia").prop("checked", true) : $("#anuencia").prop("checked", false);
}

function configurarFormulario() {
  $("#novo-documento").show()
  $("#tipo-existente").hide()

  tipoSelecaoSelect = new SelectWrapper('#tipo-selecao', {
    onOpened: async () => await obterTipoDeSelecaoSelect(), onSelected: item => {
      if (item.id === 0) {
        $("#novo-documento").show()
        $("#tipo-existente").hide()
      } else {
        $("#novo-documento").hide()
        $("#tipo-existente").show()
      }
    }, sortable: true
  });
  tipoDocumentoSelect = new SelectWrapper('#tipo-documento-existente', { onOpened: async () => await obterTipoDocumentoSelect(), sortable: true });
  moduloSelect = new SelectWrapper('#modulo', { onOpened: async () => await obterModuloSelect(), sortable: true });
  processadoSelect = new SelectWrapper('#processado', { onOpened: async () => await obterProcessadoSelect(), sortable: true });
}

async function carregarTipoDocumentoDatatable() {
  tipoDocTb = new DataTableWrapper('#tipodoctb', {
    columns: [
      { data: "id" },
      { data: "sigla", title: "Sigla" },
      { data: "tipo", title: "Tipo do documento" },
      { data: "nome", title: "Nome do documento" },
    ],
    ajax: async (requestData) => await tipoDocService.obterDatatable(requestData),
    rowCallback: function (row, data) {
      const icon = $("<i>").addClass("fa fa-file-text");
      let button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon);
      button.on("click", function () {
        const id = $(this).attr("data-id");
        tipoDocumento.id = id
        $('#upsertDocumentoModalBtn').trigger('click');
      });
      $("td:eq(0)", row).html(button);
    },
  });

  await tipoDocTb.initialize();
}


// Utils
async function obterTipoDeSelecaoSelect() {
  return await Promise.resolve([
    {
      id: 0,
      description: 'Novo tipo de documento'
    },
    {
      id: 1,
      description: 'Tipo de documento existente'
    }
  ])
}

async function obterProcessadoSelect() {
  return await Promise.resolve([
    {
      id: '',
      description: ''
    },
    {
      id: 'N',
      description: 'Não'
    },
    {
      id: 'S',
      description: 'Sim'
    }
  ])
}

async function obterTipoDocumentoSelect() {
  let tiposResult = await tipoDocService.obterTiposSelect();
  if (tiposResult.isFailure()) {
    NotificationService.error({title: "Algo deu errado ao carregar os grupos de tipos"});
    return;
  }
  return tiposResult.value;
}

async function obterModuloSelect() {
  return await Promise.resolve([
    {
      id: 1,
      description: 'Geral'
    },
    {
      id: 2,
      description: 'Processado'
    }
  ])
}

function limparFormulario() {
  $("#sigla").val("")
  $("#tipo").val("")
  $("#nome").val("")
  moduloSelect.setCurrentValue({
    id: '',
    description: ''
  })
  processadoSelect.setCurrentValue({
    id: '',
    description: ''
  })
  tipoDocumentoSelect.setCurrentValue({
    id: '',
    description: ''
  })


  $("#assunto").prop("checked", false)
  $("#validade_inicial").prop("checked", false)
  $("#sindicato_laboral").prop("checked", false)
  $("#tipo_unidade").prop("checked", false)

  $("#descricao").prop("checked", false)
  $("#validade_final").prop("checked", false)
  $("#abrangencia").prop("checked", false)
  $("#sindicato_patronal").prop("checked", false)

  $("#origem").prop("checked", false)
  $("#data_base").prop("checked", false)
  $("#atividade_economica").prop("checked", false)
  $("#numero_legislacao").prop("checked", false)

  $("#versao").prop("checked", false)
  $("#estabelecimento").prop("checked", false)
  $("#fonte").prop("checked", false)
  $("#permitir_compartilhar").prop("checked", false)
  $("#anuencia").prop("checked", false)

  tipoSelecaoSelect.setCurrentValue({
    id: '',
    description: ''
  })

  tipoDocumento = {}
}