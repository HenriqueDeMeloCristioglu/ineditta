// Libs
import jQuery from "jquery";
import $ from "jquery";
import "bootstrap";
import "../../js/utils/util.js";
import "datatables.net-bs5/css/dataTables.bootstrap5.css";
import "datatables.net-responsive-bs5/css/responsive.bootstrap5.css";
import "bootstrap/dist/css/bootstrap.min.css";

import DataTableWrapper from "../../js/utils/datatables/datatable-wrapper.js";

// Services
import {
  ClienteUnidadeService,
  GrupoClausulaService,
  UsuarioAdmService, 
  ClausulaService,
  TipoDocService,
  CnaeService,
  GrupoEconomicoService,
  EstruturaClausulaService,
  LocalizacaoService,
  MatrizService,
  MapaSindicalService,
  SindicatoLaboralService,
  SindicatoPatronalService,
  SindicatoService
} from "../../js/services"

// Core
import { AuthService, ApiService } from "../../js/core/index.js";
import { ApiLegadoService } from "../../js/core/api-legado.js";
import { UsuarioNivel } from "../../js/application/usuarios/constants/usuario-nivel";

import DatepickerrangeWrapper from "../../js/utils/daterangepicker/daterangepicker-wrapper.js";
import SelectWrapper from "../../js/utils/selects/select-wrapper.js";
import moment from "moment";
import NotificationService from '../../js/utils/notifications/notification.service.js';
import PageWrapper from "../../js/utils/pages/page-wrapper.js";
import '../../js/utils/arrays/array.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import DateFormatter from '../../js/utils/date/date-formatter.js';
import { ModalInfoSindicato } from "../../js/utils/components/modalInfoSindicatos/modal-info-sindicatos.js";
import { renderizarModal } from "../../js/utils/modals/modal-wrapper.js";

// Services
const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const mapaSindicalService = new MapaSindicalService(apiService);

const matrizService = new MatrizService(apiService);
const grupoEconomico = new GrupoEconomicoService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const localizacaoService = new LocalizacaoService(apiService, apiLegadoService);
const cnaeService = new CnaeService(apiService);
const sindicatoLaboralService = new SindicatoLaboralService(
  apiService,
  apiLegadoService
);
const sindicatoPatronalService = new SindicatoPatronalService(
  apiService,
  apiLegadoService
);
const clausulaService = new ClausulaService(apiService, apiLegadoService);
const grupoClausulaService = new GrupoClausulaService(apiService);
const tipoDocService = new TipoDocService(apiService);
const estruturaClausulaService = new EstruturaClausulaService(apiService);
const usuarioAdmService = new UsuarioAdmService(apiService);
const sindicatoService = new SindicatoService(apiService, apiLegadoService);
const modalInfoSindicato = new ModalInfoSindicato(renderizarModal, sindicatoService, DataTableWrapper);

let grupoEconomicoSelect = null;
let matrizSelect = null;
let unidadeSelect = null;
let localidadeSelect = null;
let categoriaSelect = null;
let sindicatoLaboralSelect = null;
let sindPatronalSelect = null;
let dataBaseSelect = null;
let grupoClausulasSelect = null;
let nomeDocSelect = null;
let clausulaSelect = null;
let geradorCsvDatatable = null;
let vigenciaSelect = null;
let dataProcessamento = null;

let usuario = null;

let documentosIds = [];

jQuery(async () => {
  new Menu()

  await AuthService.initialize();

  await carregarUsuario();

  await configurarSelects();

  configurarFormulario();

  $("#limparFiltroBtn").on("click", () => {
    limparFiltros();
  });

  $("#table-container").hide();
  $(".horizontal-nav").removeClass("hide");

  modalInfoSindicato.initialize("modal-info-sindicato-wrapper");

  await consultarUrl();
});

function configurarFormulario() {
  $("#btnObter").on("click", async () => {
    await carregarDatatable();

    const btnGerarExcelExiste = document.getElementById("btnGerarExcel");

    if (!btnGerarExcelExiste) {
      const button = $("<button>");

      button
        .addClass("btn btn-success")
        .attr("type", "button")
        .attr("id", "btnGerarExcel")
        .html("<i class='fa fa-file-excel-o' style='margin-right: 10px;'></i>Gerar excel");

      button.on('click', async (evt) => {
        evt.preventDefault();
        evt.stopPropagation();
        await downloadExcel();
      });

      $("#geradorExcelTb_filter").append(button);
    }

    $("#table-container").show();
  });
}

async function carregarDatatable() {
  documentosIds = [];

  if (geradorCsvDatatable) {
    await geradorCsvDatatable?.reload();
    return;
  }

  $("#selectAllInput").on("click", (event) => {
    if (event.currentTarget.checked) {
      $('.checkbox-documento').prop('checked', true);
      $('.checkbox-documento').trigger('change');
    } else {
      $('.checkbox-documento').prop('checked', false);
      $('.checkbox-documento').trigger('change');
    }
  });

  geradorCsvDatatable = new DataTableWrapper("#geradorExcelTb", {
    columns: [
      { title: "Gerar excel", data: "id" },
      {
        title: "Sind. Laboral",
        data: "sindLaboral",
        render: (data) => {
          const siglaLaboral = data?.map((item) => item?.sigla).join(", ");
          return siglaLaboral;
        },
      },
      {
        title: "Sind. Patronal",
        data: "sindPatronal",
        render: (data) => {
          const siglaPatronal = data?.map((item) => item?.sigla).join(", ");
          return siglaPatronal;
        },
      },
      {
        title: "Estado",
        data: "abrangencias",
        render: (data) => {
          const uf = data.map((item) => item.uf)?.distinct().join(", ");
          return uf;
        },
      },
      {
        title: "Data de Processamento",
        data: "dataLiberacao",
        render: (data) => {
          if (!data) return

          return moment(data).format("DD/MM/YYYY")
        },
      },
      { title: "Nome Documento", data: "tituloDoc" },
      {
        title: "Atividade Econômica",
        data: "cnaeDoc",
        render: (data) => {
          return data && Array.isArray(data) ? data.map(d => d.subclasse).join(", ") : "N/A";
        },
      },
      { title: "Data Base", data: "dataBase" },
      {
        title: "Validade Final",
        data: "validadeFinal",
        render: (data) => {
          if (!data) {
            return null;
          }
          const dataFormatada = moment(data).format("DD/MM/YYYY");
          return dataFormatada;
        },
      },
    ],
    ajax: async (requestData) => {
      $('#selectAllInput').val(false).prop('checked', false);

      requestData.grupoEconomicoIds = grupoEconomicoSelect?.getValues();
      requestData.empresaIds = matrizSelect?.getValues();
      requestData.unidadeIds = unidadeSelect?.getValues();
      requestData.tipoDocIds = nomeDocSelect?.getValues();
      requestData.cnaeIds = categoriaSelect?.getValues();
      requestData.sindLaboralIds = sindicatoLaboralSelect?.getValues();
      requestData.sindPatronalIds = sindPatronalSelect?.getValues();
      requestData.dataBases = dataBaseSelect?.getValues();
      requestData.grupoClausulaIds = grupoClausulasSelect?.getValues();
      requestData.estruturaClausulaIds = clausulaSelect?.getValues();

      if (localidadeSelect?.hasValue()) {

        if (localidadeSelect.getValue().some(localidade => localidade.indexOf('municipio:') > -1)) {
          const municipios = localidadeSelect.getValue().filter(localidade => localidade.indexOf('municipio:') > -1).map(municipio => municipio.split(':')[1]);
          requestData.localidadeIds = Array.isArray(municipios) ? municipios : [municipios];
        }

        if (localidadeSelect.getValue().some(localidade => localidade.indexOf('uf:') > -1)) {
          const ufs = localidadeSelect.getValue().filter(localidade => localidade.indexOf('uf:') > -1).map(value => value.split(':')[1]);
          requestData.ufs = Array.isArray(ufs) ? ufs : [ufs];
        }
      }

      requestData.palavraChave = $("#search").val();

      if (vigenciaSelect && vigenciaSelect.hasValue()) {
        requestData.vigenciaInicial = vigenciaSelect.getBeginDate();
        requestData.vigenciaFinal = vigenciaSelect.getEndDate();
      }

      if (dataProcessamento && dataProcessamento.hasValue()) {
        requestData.processamentoInedittaInicial =
          dataProcessamento.getBeginDate();
        requestData.processamentoInedittaFinal = dataProcessamento.getEndDate();
      }

      return await mapaSindicalService.obterExcel(requestData);
    },
    columnDefs: [
      {
        targets: "_all",
        defaultContent: "",
      },
    ],
    rowCallback: function (row, data) {
      let linksSindicatosLaborais = [];
      let linksSindicatosPatronais = [];

      if (data?.sindLaboral instanceof Array) {
        linksSindicatosLaborais = data?.sindLaboral.map((sindicatoLaboral) => {
          let link = $("<button>")
              .addClass("btn-info-sindicato")
              .attr("data-id", sindicatoLaboral.id) //data?.idSindicatoLaboral
              .html(sindicatoLaboral.sigla);
          link.on("click", function () {
              const id = $(this).attr("data-id");
              $("#sind-id-input").val(id);
              $("#tipo-sind-input").val("laboral");
              $("#openInfoSindModalBtn").trigger("click");
          });
          return link;
        });
      }

      if (data?.sindPatronal instanceof Array) {
        linksSindicatosPatronais = data?.sindPatronal.map((sindicatoPatronal) => {
          let link = $("<button>")
              .addClass("btn-info-sindicato")
              .attr("data-id", sindicatoPatronal.id) //data?.idSindicatoLaboral
              .html(sindicatoPatronal.sigla);
          link.on("click", function () {
              const id = $(this).attr("data-id");
              $("#sind-id-input").val(id);
              $("#tipo-sind-input").val("patronal");
              $("#openInfoSindModalBtn").trigger("click");
          });
          return link;
        });
      }

      $("td:eq(1)", row).html("");
      linksSindicatosLaborais.forEach((link, index) => {
        $("td:eq(1)", row).append(link);
        if (index < linksSindicatosLaborais.length - 1) {
          $("td:eq(1)", row).append(",");
        }
      })

      $("td:eq(2)", row).html("");
      linksSindicatosPatronais.forEach((link, index) => {
        $("td:eq(2)", row).append(link);
        if (index < linksSindicatosPatronais.length - 1) {
          $("td:eq(2)", row).append(",");
        }
      })

      const checkbox = $(`<input type="checkbox" data-id='${data.id}' class='form-check-input c chkitem checkbox-documento' />`);

      if (Array.isArray(documentosIds) && documentosIds.some(documentoId => documentoId === data.id)) {
        checkbox.prop('checked', true);
      }

      checkbox.on('change', (el) => {
        const checked = el.target.checked;
        const id = el.target.attributes['data-id'].value;

        if (checked && !documentosIds.some(documentoId => documentoId === id)) {
          documentosIds.push(id);
          return;
        }

        if (!checked && documentosIds.some(documentoId => documentoId === id)) {
          documentosIds.splice(documentosIds.indexOf(id), 1);
          return;
        }
      });

      $("td:eq(0)", row).html(checkbox);
    }
  });

  await geradorCsvDatatable.initialize();

  $('#btnGerarExcel').removeClass('hide');
}

async function downloadExcel() {
  if (!documentosIds || documentosIds.length === 0) {
    NotificationService.error({ title: 'Não foi possível gerar o excel', message: 'Selecione pelo menos um documentos para gerar o Excel' });
    return;
  }

  const request = {
    documentosIds: documentosIds,
    grupoClausulaIds: grupoClausulasSelect?.getValue(),
    estruturaClausulaIds: clausulaSelect?.getValue(),
    palavraChave: $("#search").val()
  };

  const result = await mapaSindicalService.downloadExcelInformacoesAdicionais(request);

  if (result.isFailure()) {
    NotificationService.error(result.error);
    return;
  }

  const today = DateFormatter.dayMonthYear(new Date()).replace('-','_');
  PageWrapper.downloadExcel(result.value.data.blob, `Mapa_sindical-Informações_adicionais_${today}.xlsx`);
}

async function carregarUsuario() {
  const dadosPessoais = await usuarioAdmService.obterDadosPessoais();

  if (dadosPessoais.isFailure()) {
    return;
  }

  usuario = dadosPessoais.value;
}

async function obterLocalidades() {
  const municipios = await localizacaoService.obterSelectPorUsuario();
  const regioes = await localizacaoService.obterSelectRegioes(true);

  const localidades = municipios?.map((municipio) => ({ id: `municipio:${municipio.id}`, description: municipio.description })) ?? [];

  if (regioes?.length > 0) {
    localidades.push(...regioes.map(regiao => ({ id: `uf:${regiao.description}`, description: regiao.description })));
  }

  return localidades;
}

async function configurarSelects() {
  const isIneditta = usuario.nivel == UsuarioNivel.Ineditta;
  const isGrupoEconomico = usuario.nivel == UsuarioNivel.GrupoEconomico;
  const isEstabelecimento = usuario.nivel == UsuarioNivel.Estabelecimento;

  const markOptionAsSelectable = usuario.nivel == 'Cliente' ? () => true : () => false;

  grupoEconomicoSelect = new SelectWrapper("#grupo", {
    options: { placeholder: "Selecione", allowEmpty: true },
    onOpened: async () => await grupoEconomico.obterSelectPorUsuario(),
    markOptionAsSelectable: isIneditta ? () => false : () => true,
  });
  if (isIneditta) {
    grupoEconomicoSelect.enable();
  } else {
    grupoEconomicoSelect.disable();
    await grupoEconomicoSelect.loadOptions();
  }

  matrizSelect = new SelectWrapper("#matriz", {
    onOpened: async (grupoId) =>
      await matrizService.obterSelectPorUsuario(grupoId),
    parentId: "#grupo",
    sortable: true,
    markOptionAsSelectable:
      isIneditta || isGrupoEconomico ? () => false : () => true,
  });
  if (isIneditta || isGrupoEconomico) {
    matrizSelect.enable();
  } else {
    const options = await matrizSelect.loadOptions()
    if (!(options instanceof Array && options.length > 1) || isEstabelecimento) {
      matrizSelect.disable()
    }
    else {
      matrizSelect.config.markOptionAsSelectable = () => false;
      matrizSelect.clear();
      matrizSelect.enable();
    }
  }

  unidadeSelect = new SelectWrapper("#unidade", {
    onOpened: async (empresasIds) =>
      await clienteUnidadeService.obterSelectPorUsuario(empresasIds, grupoEconomicoSelect?.getValues()),
    parentId: "#matriz",
    sortable: true,
    markOptionAsSelectable: isEstabelecimento ? () => true : () => false,
  });
  if (isEstabelecimento) {
    const options = await unidadeSelect.loadOptions()
    if (!(options instanceof Array && options.length > 1)) {
      unidadeSelect.disable()
    }
    else {
      unidadeSelect.config.markOptionAsSelectable = () => false;
      unidadeSelect?.clear();
      unidadeSelect.enable();
    }
  } else {
    unidadeSelect.enable();
  }

  vigenciaSelect = new DatepickerrangeWrapper("#vigencia");
  dataProcessamento = new DatepickerrangeWrapper("#dataProcessamento");

  localidadeSelect = new SelectWrapper('#localidade', {
    options: { placeholder: 'Selecione', multiple: true },
    onChange: async () => {
      await sindicatoLaboralSelect.reload();
      await sindPatronalSelect.reload();
    },
    onOpened: async () => await obterLocalidades(),
    markOptionAsSelectable: markOptionAsSelectable
  });

  categoriaSelect = new SelectWrapper('#categoria', {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: async () => {
      const options = {
        gruposEconomicosIds: $("#grupo").val() ?? null,
        matrizesIds: $("#matriz").val() ?? null,
        clientesUnidadesIds: $("#unidade").val() ?? null
      }
      return await cnaeService.obterSelectPorUsuario(options);
    },
    onChange: async () => {
      await sindicatoLaboralSelect.reload();
      await sindPatronalSelect.reload();
    },
    markOptionAsSelectable: markOptionAsSelectable
  });

  sindicatoLaboralSelect = new SelectWrapper("#sind_laboral", {
    onOpened: async () => await sindicatoLaboralService.obterSelectPorUsuario(),
  });
  sindPatronalSelect = new SelectWrapper("#sind_patronal", {
    onOpened: async () =>
      await sindicatoPatronalService.obterSelectPorUsuario(),
  });
  dataBaseSelect = new SelectWrapper("#data_base", {
    onOpened: async () => {
      const requestData = {
        sindLaboralIds: $("#sind_laboral").val(),
        sindPatronalIds: $("#sind_patronal").val(),
      }

      var options = await clausulaService.obterDatasBases(requestData)
      
      options.shift()

      return options
    },
    sortable: false
  });
  grupoClausulasSelect = new SelectWrapper("#grupo_clausulas", {
    onOpened: async () => (await grupoClausulaService.obterSelect()).value,
    onChange: () => clausulaSelect?.clear()
  });
  nomeDocSelect = new SelectWrapper("#nome_doc", {
    onOpened: async () => (await tipoDocService.obterProcessados()).value,
    sortable: true
  });
  clausulaSelect = new SelectWrapper("#clausulaList", {
    onOpened: async () => (await estruturaClausulaService.obterSelectPorGrupo(grupoClausulasSelect?.getValue())).value,
  });
}

function limparFiltros() {
  grupoClausulasSelect?.clear();
  matrizSelect?.clear();
  unidadeSelect?.clear();
  localidadeSelect?.clear();
  categoriaSelect?.clear();
  sindicatoLaboralSelect?.clear();
  sindPatronalSelect?.clear();
  dataBaseSelect?.clear();
  grupoClausulasSelect?.clear();
  nomeDocSelect?.clear();
  clausulaSelect?.clear();
  vigenciaSelect?.clear();
  dataProcessamento?.clear();

  $("#table-container").hide();
}

async function consultarUrl() {
  const queryString = window.location.search;
  const urlParams = new URLSearchParams(queryString);

  const sindicatoId = urlParams.get("sindId");
  const tipoSindicato = urlParams.get("tipoSind");

  if (sindicatoId && tipoSindicato) {
    if (tipoSindicato == "laboral") {
      sindicatoLaboralSelect?.disable();

      const options = await sindicatoLaboralSelect?.loadOptions();
      const sindicatoSelecionadoOption = options.find(o => o.id == sindicatoId);

      if (sindicatoSelecionadoOption) {
        sindicatoLaboralSelect?.clear();
        sindicatoLaboralSelect?.setCurrentValue(sindicatoSelecionadoOption);
      }
      else {
        NotificationService.error({title: "Sindicato laboral não encontrado entre as opções."});
      }

      sindicatoLaboralSelect?.enable();
    }
    else {
      sindPatronalSelect?.disable();

      const options = await sindPatronalSelect?.loadOptions();
      const sindicatoSelecionadoOption = options.find(o => o.id == sindicatoId);

      if (sindicatoSelecionadoOption) {
        sindPatronalSelect?.clear();
        sindPatronalSelect?.setCurrentValue(sindicatoSelecionadoOption);
      }
      else {
        NotificationService.error({title: "Sindicato patronal não encontrado entre as opções."});
      }

      sindPatronalSelect?.enable();
    }
  }

  $("#btnObter").trigger("click");
}
