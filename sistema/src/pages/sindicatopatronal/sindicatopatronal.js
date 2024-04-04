/* eslint-disable no-unused-vars */
import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';

import $ from 'jquery';
import JQuery from 'jquery';
import '../../js/utils/masks/jquery-mask-extensions.js';
import Masker from '../../js/utils/masks/masker.js';

import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';

import { input } from '../../js/utils/components/elements';
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import SelectWrapper from '../../js/utils/selects/select-wrapper.js';
import { renderizarModal, closeModal } from '../../js/utils/modals/modal-wrapper.js';
import NotificationService from '../../js/utils/notifications/notification.service.js';

// Core
import { AuthService, ApiService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js';

// Services
import {
  AssociacaoService,
  BaseTerritorialSindicatoPatronalService,
  UFService,
  CnaeService,
  LocalizacaoService,
  SindicatoPatronalService
} from '../../js/services'

import Result from '../../js/core/result.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const sindicatoPatronalService = new SindicatoPatronalService(apiService, apiLegadoService);
const ufService = new UFService();
const cnaeService = new CnaeService(apiService);
const localizacaoService = new LocalizacaoService(apiService);
const associacaoService = new AssociacaoService(apiService);
const baseTerritorialSindicatoPatronalService = new BaseTerritorialSindicatoPatronalService(apiService);

let ufSelect = null;
let grauSelect = null;
let statusSelect = null;
let sindicatoTable = null;
let confederacaoTable = null;
let federacaoTable = null;
let baseTerritorialTable = null;
let localizacaoTable = null;
let baseTerritorialHistoricoTable = null;
let baseTerritorialSelecionadasTable = null;

let baseTerritorialSelecionadasObtidasApi = false;
const basesTerritoriaisRemoverSelecionadas = [];

let localizacoesSelecionadas = [];
let cnaesSelecionados = [];
let basesTerritoriaisSelecionadas = [];
let basesTerritoriaisParaRemoverAdicacao = [];

let localizacaoSelecionada = 0;

JQuery(async function () {
  new Menu()

  await AuthService.initialize();

  configurarModal();
  configurarFormulario();
  carregarSindicatoDatatable();

  $("#modalBaseTerritorialAdicionarBtn").on('click', async () => await adicionarBasesTerritoriais());
  $("#removerBaseTerritorialBtn").on('click', async () => await removerBasesTerritoriaisParaAdicao());
});

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  const sindicatoModal = document.getElementById('sindicatoModalHidden');
  const modalSindicatoContent = document.getElementById('sindicatoModalHiddenContent');

  const sindicatoModalButtonsConfig = [
    {
      id: 'modalSindicatoCadastrarBtn',
      onClick: async (id, modalContainer) => {
        const result = await upsert();
        if (result.isSuccess()) {
          closeModal(modalContainer);
        }
      }
    }
  ];

  const confederacaoModal = document.getElementById('confederacaoModalHidden');
  const modalConfederacaoContent = document.getElementById('confederacaoModalHiddenContent');

  const confederacaoModalButtonsConfig = [{
    id: 'modalConfederacaoCadastrarBtn',
    onClick: async (_, modalContainer) => {
      await Promise.resolve();
      closeModal(modalContainer);
    }
  }];

  const federacaoModal = document.getElementById('federacaoModalHidden');
  const modalFederacaoContent = document.getElementById('federacaoModalHiddenContent');

  const federacaoModalButtonsConfig = [{
    id: 'modalFederacaoCadastrarBtn',
    onClick: async (_, modalContainer) => {
      await Promise.resolve();
      closeModal(modalContainer);
    }
  }];

  const baseTerritorialModal = document.getElementById('baseTerritorialModalHidden');
  const modalBaseTerritorialContent = document.getElementById('baseTerritorialModalHiddenContent');

  const baseTerritorialModalButtonsConfig = [{
    id: 'modalBaseTerritorialCadastrarBtn',
    onClick: async (_, modalContainer) => {
      await Promise.resolve();
      closeModal(modalContainer);
    }
  }];

  const localizacaoModal = document.getElementById('localizacaoModalHidden');
  const modalLocalizacaoContent = document.getElementById('localizacaoModalHiddenContent');

  const localizacaoModalButtonsConfig = [{
    id: 'modalLocalizacaoCadastrarBtn',
    onClick: async (_, modalContainer) => {
      await Promise.resolve();
      closeModal(modalContainer);
    }
  }];

  const baseTerritorialHistoricoModal = document.getElementById('baseTerritorialHistoricoModalHidden');
  const modalBaseTerritorialHistoricoContent = document.getElementById('baseTerritorialHistoricoModalHiddenContent');

  const modalsConfig = [
    {
      id: 'sindicatoModal',
      modal_hidden: sindicatoModal,
      content: modalSindicatoContent,
      btnsConfigs: sindicatoModalButtonsConfig,
      onOpen: async () => {
        const id = $('#id-input').val();
        await carregarBaseTerritorialSelecionadasDatatable();
        if (id) {
          configurarFormulario();
          await obterPorId(id);
        }
      },
      onClose: () => {
        limpar()
        baseTerritorialSelecionadasObtidasApi = false
      },
    }, {
      id: 'confederacaoModal',
      modal_hidden: confederacaoModal,
      content: modalConfederacaoContent,
      btnsConfigs: confederacaoModalButtonsConfig,
      onOpen: async () => await carregarConfederacaoDatatable(),
      isInIndex: true
    }, {
      id: 'federacaoModal',
      modal_hidden: federacaoModal,
      content: modalFederacaoContent,
      btnsConfigs: federacaoModalButtonsConfig,
      onOpen: async () => await carregarFederacaoDatatable(),
      isInIndex: true
    }, {
      id: 'baseTerritorialModal',
      modal_hidden: baseTerritorialModal,
      content: modalBaseTerritorialContent,
      btnsConfigs: baseTerritorialModalButtonsConfig,
      onOpen: async () => {
        await carregarBaseTerritorialDatatable()
        await carregarLocalizacaoDatatable()
      },
      isInIndex: true
    }, {
      id: 'localizacaoModal',
      modal_hidden: localizacaoModal,
      content: modalLocalizacaoContent,
      btnsConfigs: localizacaoModalButtonsConfig,
      onOpen: async () => await carregarLocalizacaoDatatable(),
      isInIndex: true
    }, {
      id: 'baseTerritorialHistoricoModal',
      modal_hidden: baseTerritorialHistoricoModal,
      content: modalBaseTerritorialHistoricoContent,
      onOpen: async () => await carregarBaseTerritorialHistoricoDatatable(),
      isInIndex: true,
      btnsConfigs: []
    }
  ];

  renderizarModal(pageCtn, modalsConfig);

  $('i.fa-chevron-modal-inner').click(function() {
    $(this).children().toggleClass("fa-chevron-down fa-chevron-up");
    $(this).closest(".panel-heading").next().slideToggle({duration: 200});
    $(this).closest(".panel-heading").toggleClass('rounded-bottom');
    return false;
  });
}

function vincularBaseTerritorial(id) {
  basesTerritoriaisSelecionadas?.push(parseInt(id));
}

function removerBaseTerritorial(id) {
  basesTerritoriaisSelecionadas = basesTerritoriaisSelecionadas?.filter();
}

async function upsert() {
  const id = $('#id-input').val();

  return id ? await editar(id) : await incluir();
}

async function carregarSindicatoDatatable() {
  sindicatoTable = new DataTableWrapper('#sindicatotb', {
    columns: [
      { "data": "id" },
      { "data": "sigla" },
      { "data": "cnpj", render: (data) => Masker.CNPJ(data) },
      { "data": "email" },
      { "data": "telefone", render: (data) => Masker.phone(data) },
      { "data": "municipio" },
      { "data": "uf" }
    ],
    ajax: async (requestData) =>
      await sindicatoPatronalService.obterDatatable(requestData),
    rowCallback: function (row, data, index) {
      const icon = $("<i>").addClass("fa fa-file-text");
      let button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon);
      button.on("click", function () {
        const id = $(this).attr("data-id");
        $('#id-input').val(id);
        $('#sindicatoNovoBtn').trigger('click');
      });
      $("td:eq(0)", row).html(button);
    },
  });

  await sindicatoTable.initialize();
}

async function carregarConfederacaoDatatable() {
  if (confederacaoTable) {
    await confederacaoTable.reload();
    return;
  }

  confederacaoTable = new DataTableWrapper('#confederacaotb', {
    columns: [
      { "data": "id" },
      { "data": "sigla" },
      { "data": "cnpj" },
      { "data": "area" },
      { "data": "telefone" },
      { "data": "grupo" },
      { "data": "grau" }
    ],
    ajax: async (requestData) =>
      await associacaoService.obterConfederacoesDatatable(requestData),
    rowCallback: function (row, data, index) {
      const button = $("<button>")
        .attr("data-id", data?.id)
        .attr('data-nome', data?.sigla)
        .addClass("btn btn-secondary")
        .html('Selecionar');

      button.on("click", function () {
        const id = $(this).attr("data-id");
        const nome = $(this).attr("data-nome");

        inserirValorSelect('#ass-input', id, nome);
      });
      $("td:eq(0)", row).html(button);
    },
  });

  await confederacaoTable.initialize();
}

async function carregarFederacaoDatatable() {
  if (federacaoTable) {
    federacaoTable.reload();
    return;
  }

  federacaoTable = new DataTableWrapper('#federacaotb', {
    columns: [
      { "data": "id" },
      { "data": "sigla" },
      { "data": "cnpj" },
      { "data": "area" },
      { "data": "telefone" },
      { "data": "grupo" },
      { "data": "grau" }
    ],
    ajax: async (requestData) =>
      await associacaoService.obterFederacoesDatatable(requestData),
    rowCallback: function (row, data, index) {
      const button = $("<button>")
        .attr("data-id", data?.id)
        .attr('data-nome', data?.sigla)
        .addClass("btn btn-secondary")
        .html('Selecionar');

      button.on("click", function () {
        const id = $(this).attr("data-id");
        const nome = $(this).attr("data-nome");
        inserirValorSelect('#ass1-input', id, nome);
      });

      $("td:eq(0)", row).html(button);
    },
  });

  await federacaoTable.initialize();
}

async function carregarBaseTerritorialDatatable() {
  if (baseTerritorialTable) {
    baseTerritorialTable.reload();
    return;
  }

  baseTerritorialTable = new DataTableWrapper('#baseTerritorialtb', {
    columns: [
      { "data": "id" },
      { "data": "descricao" },
      { "data": "subclasse" },
      { "data": "categoria" }
    ],
    ajax: async (requestData) => {
      requestData.Columns = "id,descricao,subclasse,categoria";
      return await cnaeService.obterDatatable(requestData)
    },
    rowCallback: function (row, data, index) {
      const checkbox = $("<input>").attr("type", "checkbox").attr("data-id", data?.id);

      if (cnaesSelecionados.some(b => b?.id == data?.id)) {
        checkbox.attr("checked", true);
      }

      checkbox.on("change", function (event) {
        const id = $(this).attr("data-id");
        if (event.target.checked) {
          cnaesSelecionados?.push({
            id: parseInt(id),
            descricacaoCnae: data?.descricao,
            subclasseCnae: data?.subclasse,  
          });
          return;
        }

        cnaesSelecionados = cnaesSelecionados?.filter(c => c?.id != id);
      });

      $("td:eq(0)", row).html(checkbox);
    },
  });

  await baseTerritorialTable.initialize();
}

async function adicionarBasesTerritoriais() {
  $("#modalBaseTerritorialAdicionar").prop("disable", true);
  localizacoesSelecionadas.forEach(loc => {
    cnaesSelecionados.forEach(cnae => {
      basesTerritoriaisSelecionadas.push({
        localizacaoId: loc.id,
        pais: loc.pais, 
        regiao: loc.regiao,
        estado: loc.estado,
        codigoUf: loc.codigoUf,
        municipio: loc.municipio, 
        cnaeId: cnae.id,
        descricaoCnae: cnae.descricacaoCnae,
        subclasseCnae: cnae?.subclasseCnae,
      });
    })
  })

  localizacoesSelecionadas = [];
  cnaesSelecionados = [];

  let newBasesTerritoriaisSelecionadas = [];

  basesTerritoriaisSelecionadas.forEach((base) => {
    if (!newBasesTerritoriaisSelecionadas.some(bt => (bt.localizacaoId == base.localizacaoId && bt.cnaeId == base.cnaeId))) {
      newBasesTerritoriaisSelecionadas.push(base);
    }
  });
  basesTerritoriaisSelecionadas = [...newBasesTerritoriaisSelecionadas];
  
  await carregarBaseTerritorialSelecionadasDatatable();
  await carregarLocalizacaoDatatable();
  await carregarBaseTerritorialDatatable();

  $("#modalBaseTerritorialAdicionar").prop("disable", false);

  NotificationService.success({
    title: "Sucesso",
    message: "Base territorial adicionada"
  })
}

async function removerBasesTerritoriaisParaAdicao() {
  let newBasesTerritoriaisSelecionadas = [];

  basesTerritoriaisSelecionadas.forEach((base) => {
    if (!basesTerritoriaisParaRemoverAdicacao.some(bt => (bt.localizacaoId == base.localizacaoId && bt.cnaeId == base.cnaeId))) {
      newBasesTerritoriaisSelecionadas.push(base);
    }
  });
  basesTerritoriaisSelecionadas = [...newBasesTerritoriaisSelecionadas];

  basesTerritoriaisParaRemoverAdicacao = [];

  await carregarBaseTerritorialSelecionadasDatatable();
}

async function carregarLocalizacaoDatatable() {
  if (localizacaoTable) {
    localizacaoTable.reload();
    return;
  }

  $("#selecionar_todas_localizacoes").on("click", (event) => {
		if (event.currentTarget.checked) {
			$('.localizacao-checkbox').prop('checked', true);
			$('.localizacao-checkbox').trigger('change');
		} else {
			$('.localizacao-checkbox').prop('checked', false);
			$('.localizacao-checkbox').trigger('change');
		}
	});

  localizacaoTable = new DataTableWrapper('#localizacaotb', {
    columns: [
      { "data": "id" },
      { "data": "codigoPais" },
      { "data": "pais" },
      { "data": "codigoRegiao" },
      { "data": "regiao" },
      { "data": "codigoUf" },
      { "data": "estado" },
      { "data": "uf" },
      { "data": "codigoMunicipio" },
      { "data": "municipio" }
    ],
    ajax: async (requestData) => {
      $('#selecionar_todas_localizacoes').val(false).prop('checked', false);
      return await localizacaoService.obterDatatable(requestData)
    },
    rowCallback: function (row, data, index) {
      const checkbox = input({ type: 'checkbox', className: 'form-check-input c chkitem localizacao-checkbox' }).attr('data-id', data?.id);

      if (localizacoesSelecionadas.some(l => l.id == data?.id)) {
        checkbox.attr("checked", true);
      }

      checkbox.on("change", function (event) {
        const id = $(this).attr("data-id");
        if (event.target.checked) {
          localizacoesSelecionadas.push({
            id: Number(id), 
            pais: data?.pais, 
            regiao: data?.regiao,
            estado: data?.estado,
            codigoUf: data?.uf,
            municipio: data?.municipio 
          });
          return;
        }

        localizacoesSelecionadas = localizacoesSelecionadas?.filter(l => l.id != Number(id));
      });

      $("td:eq(0)", row).html(checkbox);
    },
  });

  await localizacaoTable.initialize();
}

async function carregarBaseTerritorialSelecionadasDatatable() {
  if (baseTerritorialSelecionadasTable) {
    baseTerritorialSelecionadasTable.reload();
    return;
  }

  $("#selecionar_todas_bases_territoriais").on("click", (event) => {
		if (event.currentTarget.checked) {
			$('.base-para-adicao-checkbox').prop('checked', true);
			$('.base-para-adicao-checkbox').trigger('change');
		} else {
			$('.base-para-adicao-checkbox').prop('checked', false);
			$('.base-para-adicao-checkbox').trigger('change');
		}
	});

  baseTerritorialSelecionadasTable = new DataTableWrapper('#baseTerritoriaisSelecionadastb', {
    columns: [
      { "data": "localizacaoId", title: "" },
      { "data": "localizacaoId", title: "Localizacao Id" },
      { "data": "pais", title: "País" },
      { "data": "regiao", title: "Região" },
      { "data": "estado", title: "Estado" },
      { "data": "codigoUf", title: "UF" },
      { "data": "municipio", title: "Municipio" },
      { "data": "descricaoCnae", title: "Descricao CNAE" },
      { "data": "subclasseCnae", title: "Subclasse" }
    ],
    ajax: async (requestData) => {
      requestData.ApenasVigentes = true;
      requestData.ShowAllRecords = true;
      $('#selecionar_todas_bases_territoriais').val(false).prop('checked', false);
      const result = await obterBasesSelecionadasDt(requestData);
      return result;
    },
    rowCallback: function (row, data, index) {
      const checkbox = input({ type: 'checkbox', className: 'form-check-input c chkitem base-para-adicao-checkbox' }).attr('data-id', data?.id);

      if (basesTerritoriaisParaRemoverAdicacao.some(l => (l?.localizacaoId == data?.localizacaoId && l?.cnaeId == data?.cnaeId))) {
        checkbox.attr("checked", true);
      }

      checkbox.on("change", function (event) {
        const id = $(this).attr("data-id");
        if (event.target.checked) {
          basesTerritoriaisParaRemoverAdicacao.push({
            localizacaoId: data?.localizacaoId,
            cnaeId: data?.cnaeId
          });
          return;
        }

        basesTerritoriaisParaRemoverAdicacao = basesTerritoriaisParaRemoverAdicacao?.filter(l => !(l.localizacaoId == data?.localizacaoId && l.cnaeId == data?.cnaeId));
      });

      $("td:eq(0)", row).html(checkbox);
    }
  });

  await baseTerritorialSelecionadasTable.initialize();
}

async function obterBasesSelecionadasDt(request) {
  request.sindicatoPatronalId = $('#id-input').val() ?? 0;
  if (!baseTerritorialSelecionadasObtidasApi){
    const baseResult = await baseTerritorialSindicatoPatronalService.obterDatatable(request);
    
    if (baseResult.isFailure()) {
      NotificationService.error({
        title: "Algo deu errado ao obter a base territorial do sindicato",
        message: baseResult.error
      });
      return;
    }

    basesTerritoriaisSelecionadas = [...basesTerritoriaisSelecionadas, ...baseResult.value.items];
    baseTerritorialSelecionadasObtidasApi = true;
  }

  const indiceInicial = (request.PageNumber) * request.PageSize;
  const indiceFinal = indiceInicial + request.PageSize;

  if (request.Filter && request.Filter != ""){
    const filterValue = request.Filter.toLowerCase();
    let basesTerritoriaisSelecionadasFiltradas = basesTerritoriaisSelecionadas?.filter(bt => 
      (bt?.localizacaoId && bt?.localizacaoId.toString().toLowerCase().includes(filterValue)) ||
      (bt?.pais && bt?.pais?.toLowerCase().includes(filterValue)) ||
      (bt?.regiao && bt?.regiao?.toLowerCase().includes(filterValue)) ||
      (bt?.estado && bt?.estado?.toLowerCase().includes(filterValue)) ||
      (bt?.codigoUf && bt?.codigoUf?.toLowerCase().includes(filterValue)) ||
      (bt?.municipio && bt?.municipio?.toLowerCase().includes(filterValue)) ||
      (bt?.descricaoCnae && bt?.descricaoCnae?.toLowerCase().includes(filterValue)) ||
      (bt?.subclasseCnae && bt?.subclasseCnae?.toString().toLowerCase().includes(filterValue))
    )
    
    return {
      value: {
        totalCount: basesTerritoriaisSelecionadasFiltradas?.length,
        items: basesTerritoriaisSelecionadasFiltradas.slice(indiceInicial, indiceFinal)
      }
    }
  }

  return {
    value: {
      totalCount: basesTerritoriaisSelecionadas?.length,
      items: basesTerritoriaisSelecionadas.slice(indiceInicial, indiceFinal)
    }
  }
}

async function carregarBaseTerritorialHistoricoDatatable() {
  if (baseTerritorialHistoricoTable) {
    baseTerritorialHistoricoTable.reload();
    return;
  }

  baseTerritorialHistoricoTable = new DataTableWrapper('#baseTerritorialHistoricotb', {
    columns: [
      { "data": "municipio" },
      { "data": "codigoUf" },
      { "data": "descricaoSubClasse" },
      { "data": "subclasseCnae" },
      { "data": "dataInicial", type: 'date', render: (data) => DataTableWrapper.formatDate(data) },
      { "data": "dataFinal", render: (data) => data && data !== '0001-01-01' ? DataTableWrapper.formatDate(data) : 'Vigente' },
    ],
    ajax: async (requestData) => {
      requestData.sindicatoPatronalId = $('#id-input').val();
      return await baseTerritorialSindicatoPatronalService.obterDatatable(requestData);
    }
  });

  await baseTerritorialHistoricoTable.initialize();
}

function configurarFormulario() {
  $("#cnpj-input").maskCNPJ();
  $("#cod-input").maskCodigoSindical();

  $("#fone1-input").maskCelPhone();
  $("#fone2-input").maskCelPhone();
  $("#fone3-input").maskCelPhone();

  $("#cep-input").maskCEP();

  ufSelect = new SelectWrapper('#uf-input', { options: { placeholder: 'UF' }, onOpened: async () => await obterEnderecos(), sortable: true });
  grauSelect = new SelectWrapper('#grau-input', { options: { placeholder: 'Grau' }, onOpened: async () => await obterGraus(), sortable: true });
  statusSelect = new SelectWrapper('#status-input', { options: { placeholder: 'Status' }, onOpened: async () => await obterStatus(), sortable: true });

  $('#baseTerritorialHistoricoModalBtn').hide();
}

async function obterEnderecos() {
  return await Promise.resolve(ufService.obterSelect());
}

async function obterGraus() {
  return await Promise.resolve([
    { id: 0, description: 'Sindicato' },
    { id: 1, description: 'Federação' },
    { id: 2, description: 'Confederação' }
  ]);
}

async function obterStatus() {
  return await Promise.resolve([
    { id: 'ativo', description: 'Ativo' },
    { id: 'inativo', description: 'Inativo' }
  ]);
}

async function incluir() {
  const status = statusSelect?.getValue() == "ativo" ? true : false;
  
  if (!$("#sigla-input").val() || $("#sigla-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "SIGLA"});
    return;
  }

  if (!$("#cnpj-input").val() || $("#cnpj-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "CNPJ"});
    return;
  }

  if (!$("#razaosocial-input").val() || $("#razaosocial-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Razão Social"});
    return;
  }

  if (!$("#denominacao-input").val() || $("#denominacao-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Denominação"});
    return;
  }

  if (!$("#cod-input").val() || $("#cod-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Código Sindical"});
    return;
  }

  if (!$("#endereco-input").val() || $("#endereco-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Logradouro"});
    return;
  }

  if (!$("#munic-input").val() || $("#munic-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Município"});
    return;
  }

  if (!ufSelect.getValue() || ufSelect.getValue() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Uf"});
    return;
  }

  if (!$("#fone1-input").val() || $("#fone1-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Telefone 1"});
    return;
  }

  if (!grauSelect.getValue() || grauSelect.getValue() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Grau"});
    return;
  }

  if (!statusSelect?.getValue() || statusSelect?.getValue() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Status"});
    return;
  }

  if (!$("#ass-input").val() || $("#ass-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Confederação"});
    return;
  }

  if (!$("#ass1-input").val() || $("#ass1-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Federação"});
    return;
  }

  if (!basesTerritoriaisSelecionadas || basesTerritoriaisSelecionadas.length == 0) {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Base Territorial"});
    return;
  }

  const request = {
    "sigla": $("#sigla-input").val(),
    "cnpj": $("#cnpj-input").val() ? $("#cnpj-input").val().replace(/[^0-9]/g, '') : null,
    "razaoSocial": $("#razaosocial-input").val(),
    "denominacao": $("#denominacao-input").val(),
    "codigoSindical": $("#cod-input").val() ? $("#cod-input").val().replace(/[^0-9]/g, '') : null,
    "situacao": $("#situacao-input").val(),
    "logradouro": $("#endereco-input").val(),
    "municipio": $("#munic-input").val(),
    "uf": ufSelect.getValue(),
    "telefone1": $("#fone1-input").val() ? $("#fone1-input").val().replace(/[^0-9]/g, '') : null,
    "telefone2": $("#fone2-input").val() ? $("#fone2-input").val().replace(/[^0-9]/g, '') : null,
    "telefone3": $("#fone3-input").val() ? $("#fone3-input").val().replace(/[^0-9]/g, '') : null,
    "ramal": $("#ramal-input").val() ? null : $("#ramal-input").val(),
    "enquadramento": $("#enq-input").val(),
    "negociador": $("#neg-input").val(),
    "contribuicao": $("#con-input").val(),
    "email1": $("#email1-input").val(),
    "email2": $("#email2-input").val(),
    "email3": $("#email3-input").val(),
    "twitter": $("#twit-input").val(),
    "facebook": $("#face-input").val(),
    "instagram": $("#insta-input").val(),
    "site": $("#site-input").val(),
    "grau": Number(grauSelect.getValue()),
    "status": status,
    "cidades-cnaes-input": `${localizacaoSelecionada}:${basesTerritoriaisSelecionadas.join(',')}`,
    "confederacaoId": $("#ass-input").val(),
    "federacaoId": $("#ass1-input").val(),
    "localizacaoId": localizacaoSelecionada,
    "cnaesIds": basesTerritoriaisSelecionadas,
    "BasesTerritoriais": basesTerritoriaisSelecionadas.map(base => {
      return {baseTerritorialId: base.id, localizacaoId: base.localizacaoId, cnaeId: base.cnaeId}
    })
  };

  const result = await sindicatoPatronalService.incluir(request);

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro', message: result.error });
    return result;
  }

  NotificationService.success({ title: 'Sucesso', message: 'Cadastro atualizado com sucesso!' });

  limpar();

  return Result.success();
}

async function editar(id) {
  const status = statusSelect?.getValue() == "ativo" ? true : false;
  
  if (!$("#sigla-input").val() || $("#sigla-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "SIGLA"});
    return;
  }

  if (!$("#cnpj-input").val() || $("#cnpj-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "CNPJ"});
    return;
  }

  if (!$("#razaosocial-input").val() || $("#razaosocial-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Razão Social"});
    return;
  }

  if (!$("#denominacao-input").val() || $("#denominacao-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Denominação"});
    return;
  }

  if (!$("#cod-input").val() || $("#cod-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Código Sindical"});
    return;
  }

  if (!$("#endereco-input").val() || $("#endereco-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Logradouro"});
    return;
  }

  if (!$("#munic-input").val() || $("#munic-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Município"});
    return;
  }

  if (!ufSelect.getValue() || ufSelect.getValue() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Uf"});
    return;
  }

  if (!$("#fone1-input").val() || $("#fone1-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Telefone 1"});
    return;
  }

  if (!grauSelect.getValue() || grauSelect.getValue() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Grau"});
    return;
  }

  if (!statusSelect?.getValue() || statusSelect?.getValue() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Status"});
    return;
  }

  if (!$("#ass-input").val() || $("#ass-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Confederação"});
    return;
  }

  if (!$("#ass1-input").val() || $("#ass1-input").val() == "") {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Federação"});
    return;
  }

  if (!basesTerritoriaisSelecionadas || basesTerritoriaisSelecionadas.length == 0) {
    NotificationService.error({title: "Campo obrigatório vazio", message: "Base Territorial"});
    return;
  }

  const request = {
    "sigla": $("#sigla-input").val(),
    "cnpj": $("#cnpj-input").val() ? $("#cnpj-input").val().replace(/[^0-9]/g, '') : null,
    "razaoSocial": $("#razaosocial-input").val(),
    "denominacao": $("#denominacao-input").val(),
    "codigoSindical": $("#cod-input").val() ? $("#cod-input").val().replace(/[^0-9]/g, '') : null,
    "situacao": $("#situacao-input").val(),
    "logradouro": $("#endereco-input").val(),
    "municipio": $("#munic-input").val(),
    "uf": ufSelect.getValue(),
    "telefone1": $("#fone1-input").val() ? $("#fone1-input").val().replace(/[^0-9]/g, '') : null,
    "telefone2": $("#fone2-input").val() ? $("#fone2-input").val().replace(/[^0-9]/g, '') : null,
    "telefone3": $("#fone3-input").val() ? $("#fone3-input").val().replace(/[^0-9]/g, '') : null,
    "ramal": $("#ramal-input").val() ? null : $("#ramal-input").val(),
    "enquadramento": $("#enq-input").val(),
    "negociador": $("#neg-input").val(),
    "contribuicao": $("#con-input").val(),
    "email1": $("#email1-input").val(),
    "email2": $("#email2-input").val(),
    "email3": $("#email3-input").val(),
    "twitter": $("#twit-input").val(),
    "facebook": $("#face-input").val(),
    "instagram": $("#insta-input").val(),
    "site": $("#site-input").val(),
    "grau": Number(grauSelect.getValue()),
    "status": status,
    "cidades-cnaes-input": `${localizacaoSelecionada}:${basesTerritoriaisSelecionadas.join(',')}`,
    "confederacaoId": $("#ass-input").val(),
    "federacaoId": $("#ass1-input").val(),
    "remover-input": `${basesTerritoriaisRemoverSelecionadas.join(',')}`,
    "BasesTerritoriais": basesTerritoriaisSelecionadas.map(base => {
      return {baseTerritorialId: base.id, localizacaoId: base.localizacaoId, cnaeId: base.cnaeId}
    }),
    "id": id
  };

  const result = await sindicatoPatronalService.editar(request);

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro', message: result.error });
    return result;
  }

  NotificationService.success({ title: 'Sucesso', message: 'Cadastro atualizado com sucesso!' });

  limpar();

  return Result.success();
}

function limpar() {
  limparFormulario();
  sindicatoTable?.reload();
  confederacaoTable?.clear();
  federacaoTable?.clear();
  baseTerritorialTable?.clear();

  baseTerritorialSelecionadasTable?.dataTable?.search("");
  baseTerritorialSelecionadasTable?.clear();

  localizacaoTable?.clear();
}

function limparFormulario() {
  $("#sigla-input").val('');
  $("#cnpj-input").val('');
  $("#razaosocial-input").val('');
  $("#denominacao-input").val('');
  $("#cod-input").val('');
  $("#situacao-input").val('');
  $("#endereco-input").val('');
  $("#munic-input").val('');
  $("#fone1-input").val('');
  $("#fone2-input").val('');
  $("#fone3-input").val('');
  $("#ramal-input").val('');
  $("#enq-input").val('');
  $("#neg-input").val('');
  $("#con-input").val('');
  $("#email1-input").val('');
  $("#email2-input").val('');
  $("#email3-input").val('');
  $("#twit-input").val('');
  $("#face-input").val('');
  $("#insta-input").val('');
  $("#status-input").val('');
  $("#ass-input").val('');
  $("#ass1-input").val('');
  $("#cidades-cnaes-input").val('');
  $("#remover-input").val('');
  $('#id-input').val('');

  ufSelect?.clear();
  grauSelect?.clear();
  statusSelect?.clear();
  basesTerritoriaisSelecionadas = [];
  basesTerritoriaisRemoverSelecionadas.splice(0, basesTerritoriaisRemoverSelecionadas.length);
  localizacaoSelecionada = null;

  $('#baseTerritorialHistoricoModalBtn').hide();
}

async function obterPorId(id) {
  limparFormulario();

  const result = await sindicatoPatronalService.obterPorId(id);

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro', message: 'Sindicato patronal não foi encontrado' });
    return;
  }

  $('#id-input').val(result.value.id);
  $("#sigla-input").val(result.value.sigla);
  $("#cnpj-input").val(result.value.cnpj?.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5'));
  $("#razaosocial-input").val(result.value.razaoSocial);
  $("#denominacao-input").val(result.value.denominacao);
  $("#cod-input").val(result.value.codigoSindical?.replace(/(\d{3})(\d{3})(\d{3})(\d{5})(\d{1})/, '$1.$2.$3.$4-$5'));
  $("#situacao-input").val(result.value.situacao);
  $("#endereco-input").val(result.value.logradouro);
  $("#munic-input").val(result.value.municipio);

  $("#fone1-input").val(result.value.telefone1.replace(/(\d{2})(\d{4})(\d{4,5})/, '($1)$2-$3'));

  $("#fone2-input").val(result.value.telefone2);
  $("#fone3-input").val(result.value.telefone3);
  $("#ramal-input").val(result.value.ramal);
  $("#enq-input").val(result.value.enquadramento);
  $("#neg-input").val(result.value.negociador);
  $("#con-input").val(result.value.contribuicao);
  $("#email1-input").val(result.value.email1);
  $("#email2-input").val(result.value.email2);
  $("#email3-input").val(result.value.email3);
  $("#twit-input").val(result.value.twitter);
  $("#face-input").val(result.value.facebook);
  $("#insta-input").val(result.value.instagram);
  $("#site-input").val(result.value.site);

  if (result.value.confederacao) {
    inserirValorSelect("#ass-input", result.value.confederacao.id, result.value.confederacao.description)
  }

  if (result.value.federacao) {
    inserirValorSelect("#ass1-input", result.value.federacao.id, result.value.federacao.description)
  }

  const ufs = ufService.obterSelect();
  const graus = await obterGraus();
  const status = await obterStatus();

  if (result.value.uf) {
    ufSelect.setCurrentValue({ id: result.value.uf, description: ufs?.find(uf => uf.id === result.value.uf)?.description });
  }

  if (result.value.grau) {
    grauSelect.setCurrentValue({ id: graus?.find(grau => grau.description === result.value.grau)?.id, description: result.value.grau });
  }

  if (result.value.status == "ativo") {
    statusSelect.setCurrentValue({ id: result.value.status, description: "Ativo" });
  } else {
    statusSelect.setCurrentValue({ id: "inativo", description: "Inativo" });
  }

  $('#baseTerritorialHistoricoModalBtn').show();
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