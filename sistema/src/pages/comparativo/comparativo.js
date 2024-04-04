/* eslint-disable no-unused-vars */
// Libs
import "bootstrap";
import jQuery from "jquery";
import $ from "jquery";
// Css libs
import "bootstrap/dist/css/bootstrap.min.css";

// Temp
import "datatables.net-bs5/css/dataTables.bootstrap5.css";
import "datatables.net-responsive-bs5/css/responsive.bootstrap5.css";
import "datatables.net-bs5";
import "datatables.net-responsive-bs5";

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

import DataTableWrapper from "../../js/utils/datatables/datatable-wrapper.js";
import NotificationService from "../../js/utils/notifications/notification.service";

// Core
import { ApiService, AuthService } from "../../js/core/index.js";
import { ApiLegadoService } from "../../js/core/api-legado.js";

// Services
import {
  UsuarioAdmService,
  DocSindService,
  TipoDocService,
  CnaeService,
  LocalizacaoService,
  MapaSindicalService
} from "../../js/services"

import SelectWrapper from "../../js/utils/selects/select-wrapper";
import PageWrapper from "../../js/utils/pages/page-wrapper.js";

// Services
const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();

const localizacaoService = new LocalizacaoService(apiService, apiLegadoService);
const cnaeService = new CnaeService(apiService);
const docSindService = new DocSindService(apiService);
const tipoDocService = new TipoDocService(apiService);
const usuarioAdmService = new UsuarioAdmService(apiService);
const mapaSindicalService = new MapaSindicalService(apiService);

let localidadeSelect = null;
let cnaeSelect = null;
let nomeDocSelect = null;
let dataBaseSelect = null;
let grupoEconomicoSelect = null;
let matrizSelect = null;
let estabelecimentoSelect = null;

let localidadeComSelect = null;
let cnaeComSelect = null;
let nomeDocComSelect = null;
let dataBaseComSelect = null;
let grupoEconomicoComSelect = null;
let matrizComSelect = null;
let estabelecimentoComSelect = null;

let usuario = null;

let datatableDocumentosReferencia = null;
let datatableDocumentosDocumentacao = null;

let documentoReferenciaSelecionadoId = null;
let documentosComparacaoIds = [];

jQuery(async () => {
  new Menu()

  await AuthService.initialize();

  await carregarUsuario();

  await configurarSelects();

  $(".horizontal-nav").removeClass("hide");

  $("#btnAbrirModalReferencia").on("click", async () => {
    await carregarDatatableReferencia();
  });
  $("#btnAbrirModalDocumentacao").on("click", async () => {
    await carregarDatatableDocumentacao();
  });

  $("#generate_csv").on("click", () => gerarExcelComparativo());
  $("#btnLimparFiltroReferencia").on("click", () => limparFiltrosReferencia());
  $("#btnLimparFiltroDocumentacao").on("click", () =>
    limparFiltrosDocumentacao()
  );
});

async function carregarUsuario() {
  const dadosPessoais = await usuarioAdmService.obterDadosPessoais();

  if (dadosPessoais.isFailure()) {
    return;
  }

  usuario = dadosPessoais.value;
}

async function configurarSelects() {
  localidadeSelect = new SelectWrapper("#localidade", {
    onOpened: async () => await obterLocalidades(),
  });
  cnaeSelect = new SelectWrapper("#cnae", {
    onOpened: async () =>
      await cnaeService.obterSelectPorUsuario({
        porUsuario: false,
        porGrupoDoUsuario: true,
      }),
  });
  nomeDocSelect = new SelectWrapper("#nome_doc", {
    onOpened: async () =>
      await tipoDocService.obterSelectPorTipos({ processado: true }),
  });

  localidadeComSelect = new SelectWrapper("#localidade_com", {
    onOpened: async () => await obterLocalidades(),
  });
  cnaeComSelect = new SelectWrapper("#cnae_com", {
    onOpened: async () =>
      await cnaeService.obterSelectPorUsuario({
        porUsuario: false,
        porGrupoDoUsuario: true,
      }),
  });
  nomeDocComSelect = new SelectWrapper("#nome_doc_com", {
    onOpened: async () =>
      await tipoDocService.obterSelectPorTipos({ processado: true }),
  });

  dataBaseSelect = new SelectWrapper("#data_base", {
    onOpened: async () => await obterDatasBasesDocumetosSindicais(),
  });

  dataBaseComSelect = new SelectWrapper("#data_base_com", {
    onOpened: async () => await obterDatasBasesDocumetosSindicaisComparativo(),
  });
}

async function obterDatasBasesDocumetosSindicais() {
  let localidades = localidadeSelect.getValues();
  let localizacoesMunicipiosIds = [];
  let localizacoesEstadosUfs = [];

  if (localidades && localidades.length > 0) {
    localidades.forEach(localidade => {
      if (localidade?.includes("municipio")) {
        localizacoesMunicipiosIds.push(localidade.split(":")[1]);
      }

      if (localidade?.includes("uf")) {
        localizacoesEstadosUfs.push(localidade.split(":")[1]);
      }
    });
  }

  const params = {
    localizacoesEstadosUfs,
    localizacoesMunicipiosIds,
    atividadesEconomicasIds: cnaeSelect.getValues(),
    gruposEconomicosIds: null,
    matrizesIds: null,
    clientesUnidadesIds: null,
    porUsuario: true,
    processados: true
  };

  return await docSindService.obterSelectAnosMeses(params);
}

async function obterDatasBasesDocumetosSindicaisComparativo() {
  let localidades = localidadeComSelect.getValues();
  let localizacoesMunicipiosIds = [];
  let localizacoesEstadosUfs = [];

  if (localidades && localidades.length > 0) {
    localidades.forEach(localidade => {
      if (localidade?.includes("municipio")) {
        localizacoesMunicipiosIds.push(localidade.split(":")[1]);
      }

      if (localidade?.includes("uf")) {
        localizacoesEstadosUfs.push(localidade.split(":")[1]);
      }
    });
  }

  const params = {
    localizacoesEstadosUfs,
    localizacoesMunicipiosIds,
    atividadesEconomicasIds: cnaeComSelect.getValues(),
    gruposEconomicosIds: null,
    matrizesIds: null,
    clientesUnidadesIds: null,
    porUsuario: true,
    processados: true
  };

  return await docSindService.obterSelectAnosMeses(params);
}

async function obterLocalidades() {
  const municipios = await localizacaoService.obterSelectPorGrupoDoUsuario();
  const regioes =
    await localizacaoService.obterSelectPorGrupoDoUsuarioRegioes();

  let localidades =
    municipios?.map((municipio) => ({
      id: `municipio:${municipio.id}`,
      description: municipio.description,
    })) ?? [];

  if (regioes?.length > 0) {
    localidades = [
      ...regioes.map((regiao) => ({
        id: `uf:${regiao.description}`,
        description: regiao.description,
      })),
      ...localidades,
    ];
  }

  return localidades;
}

async function carregarDatatableReferencia() {
  if (datatableDocumentosReferencia) {
    await datatableDocumentosReferencia.reload();
    return;
  }

  datatableDocumentosReferencia = new DataTableWrapper(
    "#documentos_referencia",
    {
      columns: [
        { data: "id", orderable: false },
        {
          title: "Sind. Laboral",
          data: "sindicatosLaborais",
          render: (data) => {
            const siglaLaboral = Array.isArray(data) ? data?.map((item) => item?.sigla).join(", ") : "";
            return siglaLaboral;
          },
        },
        {
          title: "Sind. Patronal",
          data: "sindicatosPatronais",
          render: (data) => {
            const siglaPatronal = Array.isArray(data) ? data?.map((item) => item?.sigla).join(", ") : "";
            return siglaPatronal;
          },
        },
        {
          title: "Atividade Econômica",
          data: "atividadesEconomicas",
          render: (data) => {
            return data && Array.isArray(data) ? data.map(ativideEconomica => ativideEconomica.subclasse).join(", ") : "N/A";
          },
        },
        { data: "nome" },
        { data: "dataBase", title: "Data Base" },
        {
          title: "Vigência",
          data: null,
          render: (data) => {
            let vigencia = DataTableWrapper.formatDate(
              data?.dataValidadeInicial
            );

            vigencia += data?.dataValidadeFinal
              ? ` até ${DataTableWrapper.formatDate(data?.dataValidadeFinal)}`
              : " até (não informado)";

            return vigencia;
          }
        },
      ],
      ajax: async (params) => {

        let localidades = localidadeSelect.getValues();
        let localizacoesMunicipiosIds = [];
        let localizacoesEstadosUfs = [];

        if (localidades && localidades.length > 0) {
          localidades.forEach(localidade => {
            if (localidade?.includes("municipio")) {
              localizacoesMunicipiosIds.push(localidade.split(":")[1]);
            }

            if (localidade?.includes("uf")) {
              localizacoesEstadosUfs.push(localidade.split(":")[1]);
            }
          });
        }

        const requestParams = {
          localizacoesEstadosUfs,
          localizacoesMunicipiosIds,
          atividadesEconomicasIds: cnaeSelect.getValues(),
          nomeDocumentoIds: nomeDocSelect.getValues(),
          datasBases: dataBaseSelect.getValues(),
          ...params,
        };

        return await mapaSindicalService.obterDocumentosReferencia(
          requestParams
        );
      },
      columnDefs: [
        {
          targets: "_all",
          defaultContent: "",
        },
      ],
      rowCallback: function (row, data) {
        const checkbox = $("<input>")
          .attr("type", "checkbox")
          .addClass("chk-selecionar")
          .attr("data-id", data?.id);

        if (documentoReferenciaSelecionadoId === data?.id) {
          checkbox.prop("checked", true);
        }

        checkbox.on('change', async (el) => {
          const checked = el.target.checked;
          const id = el.target.attributes['data-id'].value;

          $(".chk-selecionar").not(this).prop("checked", false); // Desmarca os outros checkboxes

          if (checked) {
            documentoReferenciaSelecionadoId = parseInt(id);
            $(el.target).prop("checked", true);
            return;
          }

          documentoReferenciaSelecionadoId = null;

        });

        $("td:eq(0)", row).html(checkbox);
      },
    });

  await datatableDocumentosReferencia.initialize();
}

async function carregarDatatableDocumentacao() {
  if (datatableDocumentosDocumentacao) {
    await datatableDocumentosDocumentacao.reload();
    return;
  }

  datatableDocumentosDocumentacao = new DataTableWrapper(
    "#documentos_comparacao",
    {
      columns: [
        { data: "id", orderable: false },
        {
          title: "Sind. Laboral",
          data: "sindicatosLaborais",
          render: (data) => {
            const siglaLaboral = Array.isArray(data) ? data?.map((item) => item?.sigla).join(", ") : "";
            return siglaLaboral;
          },
        },
        {
          title: "Sind. Patronal",
          data: "sindicatosPatronais",
          render: (data) => {
            const siglaPatronal = Array.isArray(data) ? data?.map((item) => item?.sigla).join(", ") : "";
            return siglaPatronal;
          },
        },
        {
          title: "Atividade Econômica",
          data: "atividadesEconomicas",
          render: (data) => {
            return data && Array.isArray(data) ? data.map(ativideEconomica => ativideEconomica.subclasse).join(", ") : "N/A";
          },
        },
        { data: "nome" },
        { data: "dataBase", title: "Data Base" },
        {
          title: "Vigência",
          data: null,
          render: (data) => {
            let vigencia = DataTableWrapper.formatDate(
              data?.dataValidadeInicial
            );

            vigencia += data?.dataValidadeFinal
              ? ` até ${DataTableWrapper.formatDate(data?.dataValidadeFinal)}`
              : " até (não informado)";

            return vigencia;
          }
        }
      ],
      ajax: async (params) => {
        let localidades = localidadeComSelect.getValues();
        let localizacoesMunicipiosIds = [];
        let localizacoesEstadosUfs = [];

        if (localidades && localidades.length > 0) {
          localidades.forEach(localidade => {
            if (localidade?.includes("municipio")) {
              localizacoesMunicipiosIds.push(localidade.split(":")[1]);
            }

            if (localidade?.includes("uf")) {
              localizacoesEstadosUfs.push(localidade.split(":")[1]);
            }
          });
        }

        const ignorarDocumentoId = documentoReferenciaSelecionadoId ? documentoReferenciaSelecionadoId : null;

        const requestParams = {
          localizacoesEstadosUfs,
          localizacoesMunicipiosIds,
          atividadesEconomicasIds: cnaeComSelect.getValues(),
          nomeDocumentoIds: nomeDocComSelect.getValues(),
          datasBases: dataBaseComSelect.getValues(),
          ignorarDocumentoId,
          ...params,
        };

        return await mapaSindicalService.obterDocumentosReferencia(
          requestParams
        );
      },
      columnDefs: [
        {
          targets: "_all",
          defaultContent: "",
        },
      ],
      rowCallback: function (row, data) {
        const checkbox = $("<input>")
          .attr("type", "checkbox")
          .attr("data-id", data?.id)
          .addClass("documento-comparacao");


        if (documentosComparacaoIds.includes(data?.id)) {
          checkbox.prop("checked", true);
        }

        checkbox.on('change', (el) => {
          const checked = el.target.checked;
          const id = el.target.attributes['data-id'].value;

          if (checked) {
            documentosComparacaoIds.push(parseInt(id));
            return;
          }

          documentosComparacaoIds = documentosComparacaoIds.filter(
            (item) => item !== id
          );
        });

        $("td:eq(0)", row).html(checkbox);
      },
    });

  await datatableDocumentosDocumentacao.initialize();
}

async function gerarExcelComparativo() {
  if (!documentoReferenciaSelecionadoId) {
    NotificationService.error({
      title: "Erro",
      message: "É necessário informar um documento de referência.",
    });
    return;
  }

  if (!documentosComparacaoIds) {
    NotificationService.error({
      title: "Erro",
      message: "É necessário informar um documento para comparação.",
    });
    return;
  }

  if (documentosComparacaoIds.some((item) => item === documentoReferenciaSelecionadoId)) {
    documentosComparacaoIds.splice(documentosComparacaoIds.indexOf(documentoReferenciaSelecionadoId), 1);
  }

  const documentosComparacao = documentosComparacaoIds.filter((item, index) => {
    return documentosComparacaoIds.indexOf(item) === index;
  });

  const requestBody = {
    documentoReferenciaId: documentoReferenciaSelecionadoId,
    documentoComparacaoIds: documentosComparacao,
  };

  const result = await mapaSindicalService.obterExcelComparativo(requestBody);

  if (result.isFailure()) {
    NotificationService.error({ title: 'Não foi possível gerar o excel', message: result.error });
    return;
  }

  PageWrapper.downloadExcel(result.value.data.blob, "comparativo.xlsx");
}

function limparFiltrosReferencia() {
  localidadeSelect?.clear();
  cnaeSelect?.clear();
  nomeDocSelect?.clear();
  dataBaseSelect?.clear();

  documentoReferenciaSelecionadoId = null;
}

function limparFiltrosDocumentacao() {
  localidadeComSelect?.clear();
  cnaeComSelect?.clear();
  nomeDocComSelect?.clear();
  dataBaseComSelect?.clear();
}