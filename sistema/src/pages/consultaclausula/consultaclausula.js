import "bootstrap"
import JQuery from "jquery"
import $ from "jquery"
import "../../js/utils/masks/jquery-mask-extensions.js"

import "../../js/utils/util.js"

import "datatables.net-bs5/css/dataTables.bootstrap5.css"
import "datatables.net-responsive-bs5/css/responsive.bootstrap5.css"
import "datatables.net-bs5"
import "datatables.net-responsive-bs5"

// Css libs
import "bootstrap/dist/css/bootstrap.min.css"
import "mark.js/dist/jquery.mark.es6.js"

// Core
import { AuthService, ApiService, UserInfoService } from "../../js/core/index.js"
import { ApiLegadoService } from "../../js/core/api-legado.js"

// Services
import {
  GrupoClausulaService,
  GrupoEconomicoService,
  UsuarioAdmService,
  ClienteUnidadeService,
  ClausulaService,
  ComentarioService,
  TipoDocService,
  CnaeService,
  EstruturaClausulaService,
  LocalizacaoService,
  MatrizService,
  SindicatoService,
  SindicatoLaboralService,
  SindicatoPatronalService,
  TipoEtiquetaService,
  EtiquetaService,
  DocSindService,
  ClausulaGeralService
} from "../../js/services"

import moment from "moment";
import {
  closeModal,
  renderizarModal,
} from "../../js/utils/modals/modal-wrapper.js";
import NotificationService from "../../js/utils/notifications/notification.service.js";
import DatepickerWrapper from "../../js/utils/datepicker/datepicker-wrapper.js";
import DataTableWrapper from "../../js/utils/datatables/datatable-wrapper.js";
import SelectWrapper from "../../js/utils/selects/select-wrapper.js";
import { UsuarioNivel } from "../../js/application/usuarios/constants/usuario-nivel.js";

import "../../js/main.js";
import { Menu } from "../../components/menu/menu.js";

import Result from "../../js/core/result.js";
import PageWrapper from "../../js/utils/pages/page-wrapper.js";
import { BooleanType, obterBooleanSelect, obterTipoNotificacaoSelect, obterTipoUsuarioDestinoSelect, usuarioDestinoSelect } from "../../js/utils/components/selects"
import { TipoNotificacao } from "../../js/application/comentarios/constants/tipo-notificacao.js";
import { TipoComentario } from "../../js/application/comentarios/constants/tipo-comentarios.js";
import { MediaType } from "../../js/utils/web/media-type.js";

import { TipoObservacaoAdicional } from '../../js/application/informacoesAdicionais/cliente/constants/tipo-obeservacao-adicional.js';
import { JOpcaoAdicional } from '../../js/utils/components/informacao-adicional/j-search-option-adicional.js';

import { stringTextArea } from '../../js/utils/components/string-elements'
import { td, th, h2, thead, tr, tbody, table, div } from '../../js/utils/components/elements';
import DateFormatter from "../../js/utils/date/date-formatter.js";
import { ModalInfoSindicato } from "../../js/utils/components/modalInfoSindicatos/modal-info-sindicatos.js";


const apiService = new ApiService()
const apiLegadoService = new ApiLegadoService()
const grupoEconomicoService = new GrupoEconomicoService(apiService)
const usuarioAdmService = new UsuarioAdmService(apiService, apiLegadoService)
const matrizService = new MatrizService(apiService)
const clienteUnidadeService = new ClienteUnidadeService(apiService)
const localizacaoService = new LocalizacaoService(apiService, apiLegadoService)
const cnaeService = new CnaeService(apiService)
const sindLaboralService = new SindicatoLaboralService(
  apiService,
  apiLegadoService
)
const sindPatronalService = new SindicatoPatronalService(
  apiService,
  apiLegadoService
)
const tipoDocService = new TipoDocService(apiService, apiLegadoService)
const grupoClausulaService = new GrupoClausulaService(apiService)
const estruturaClausulaService = new EstruturaClausulaService(apiService)
const clausulaService = new ClausulaService(apiService, apiLegadoService)
const sindicatoService = new SindicatoService(apiService, apiLegadoService)
const tipoEtiquetaService = new TipoEtiquetaService(apiService)
const etiquetaService = new EtiquetaService(apiService)
const comentarioService = new ComentarioService(apiService)
const docSindService = new DocSindService(apiService)
const clausulaGeralService = new ClausulaGeralService(apiService)

let usuario = null;

let gruposEconomicosSelect = null;
let empresasSelect = null;
let estabelecimentosSelect = null;
let localizacoesSelect = null;
let cnaeSelect = null;
let sindLaboralSelect = null;
let sindPatronalSelect = null;
let tipoDocSelect = null;
let grupoClausulaSelect = null;
let estruturaClausulaSelect = null;
let dataBaseSelect = null;
let visivelSelect = null;
let destinoSelect = null;
let tipoUsuarioDestinoSelect = null
let tipoNotificacaoSelect = null;
let etiquetaSelect = null;
let dataValidade = null;
let grupoEconomicoFiltrado = null;

let clausulasTb = null;
let documentosTb = null;

let clausulasSelecionadas = [];
let dadosClausula = null;
let dadosComparativoClausula = [];
let clausulaClicada = null;
let dadosClausulasPorId = null;
let totalClausulasComparativo = 0;

let vigenciaAtualSelect = null;
let vigenciaAnteriorSelect = null;
let docReferenciaSelect = null;
let docAnteriorSelect = null;
let sindLaboralComparativoSelect = null;
let sindPatronalComparativoSelect = null;

let isIneditta = null;
let isGrupoEconomico = null;
let isEstabelecimento = null;
let isEmpresa = null;
let comentar = null;

let idDoc = null;
let palavraChaveSearched = null;

const documentoGridModeOptions = {
  referencia: 1,
  anterior: 2,
}
let documentoGridMode = null;
let documentoReferenciaId = null;
let documentoAnteriorId = null;

let informacoesAdicionais = null;

let dadosPessoais;

const MODULO_MAPA_SINDICAL_CSV_EXCEL_ID = 14;
let mapaSindicalCsvExcelPermissoes = null;

JQuery(async () => {
  new Menu();

  await AuthService.initialize();

  $("#ano-fil").mask("0000");

  dadosPessoais = await usuarioAdmService.obterDadosPessoais();

  if (dadosPessoais.isFailure()) {
    return;
  }

  usuario = dadosPessoais.value;

  isIneditta = usuario.nivel == UsuarioNivel.Ineditta;
  isGrupoEconomico = usuario.nivel == UsuarioNivel.GrupoEconomico;
  isEstabelecimento = usuario.nivel == UsuarioNivel.Estabelecimento;
  isEmpresa = usuario.nivel == UsuarioNivel.Empresa;

  await configurarSelects(usuario);

  await obterPermissoes();

  configurarFormulario();

  configurarModal();

  configurarInformacaoSindicatoService();

  $(".horizontal-nav").removeClass("hide");

  $("#collapseDadosCadastrais").on("click", () => {
    $("#collapseDadosCadastraisBody").collapse("toggle");
  });

  $("#collapseLocalizacao").on("click", () => {
    $("#collapseLocalizacaoBody").collapse("toggle");
  });

  $("#collapseContato").on("click", () => {
    $("#collapseContatoBody").collapse("toggle");
  });

  $("#collapseAssociacoes").on("click", () => {
    $("#collapseAssociacoesBody").collapse("toggle");
  });

  $("#abrirClausulaBtn").on("click", () => {
    abrirClausulaPorId();
  });

  $("#documentoAnteriorBtn").attr("disabled", true);
  $("#documentoAnteriorBtn").on("click", () => {
    documentoGridMode = documentoGridModeOptions.anterior;
    $("#abrirDocumentoModalBtn").trigger('click');
  });

  $("#documentoReferenciaBtn").attr("disabled", true);
  $("#documentoReferenciaBtn").on("click", () => {
    documentoGridMode = documentoGridModeOptions.referencia;
    $("#abrirDocumentoModalBtn").trigger('click');
  });

  $("#dropdownMenu2").dropdown();

  //Assuming URL has "?post=1234&action=edit"

  $("#compararClausulasBtn").on("click", async () => {
    if (sindLaboralSelect.getSelectedOptions().length <= 0) {
      NotificationService.error({
        title: "Não é possível comparar!",
        message: "Selecione um sindicato antes de abrir a comparação.",
      });

      return;
    }

    sindLaboralComparativoSelect.setCurrentValue(
      sindLaboralSelect.getSelectedOptions()[0]
    );

    $("#abrirComparativoClausulasBtn").trigger("click");
  });

  $("#gerarPDFBtn").on("click", async () => {
    await generatePdf();
  });

  $("#gerarExcelBtn").on("click", async () => {
    const result = await clausulaService.obterRelatorioClausulasExcel({
      clausulasIds: clausulasSelecionadas
    });

    if (result.isFailure()) {
      NotificationService.error({title: "Erro ao obter documento", message: result.error});
      return;
    }

    const today = DateFormatter.dayMonthYear(new Date()).replace('-','_');
    PageWrapper.downloadExcel(result.value.data.blob, `Relatório_de_cláusulas_${today}.xlsx`);
  });

  $("#gerarExcel").on("click", async () => {
    let clausulasIds = null;

    if (!clausulaClicada) {
      if(!(clausulasSelecionadas instanceof Array) || clausulasSelecionadas.length == 0) {
        NotificationService.error({
          title: "Não é possível gerar o PDF",
          message: "Selecione pelo menos uma cláusula!",
        });
        return;
      }
      
      clausulasIds = clausulasSelecionadas;
    }
    else {
      clausulasIds = clausulaClicada;
    }

    const result = await clausulaService.obterRelatorioClausulasExcel({
      clausulasIds: [clausulasIds]
    });

    if (result.isFailure()) {
      NotificationService.error({title: "Erro ao obter documento", message: result.error});
      return;
    }

    const today = DateFormatter.dayMonthYear(new Date()).replace('-','_');
    PageWrapper.downloadExcel(result.value.data.blob, `Relatório_de_cláusulas_${today}.xlsx`);
  });

  configurarSelectsComparativo();

  await consultarUrl();
});

async function consultarUrl() {
  const queryString = window.location.search;
  const urlParams = new URLSearchParams(queryString);

  const sindicatoId = urlParams.get("sindId");
  const tipoSindicato = urlParams.get("tipoSind");
  const comparativo = JSON.parse(urlParams.get("comparativo"));

  if (sindicatoId && tipoSindicato) {
    if (tipoSindicato == "laboral") {
      sindLaboralSelect?.disable();

      const options = await sindLaboralSelect?.loadOptions();
      const sindicatoSelecionadoOption = options.find(o => o.id == sindicatoId);

      if (sindicatoSelecionadoOption) {
        sindLaboralSelect?.clear();
        sindLaboralSelect?.setCurrentValue(sindicatoSelecionadoOption);
      }
      else {
        NotificationService.error({title: "Sindicato laboral não encontrado entre as opções."});
      }

      sindLaboralSelect?.enable();
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

  if (comparativo) $("#compararClausulasBtn").trigger("click");

  if (urlParams.has("iddoc")) {
    idDoc = urlParams.get("iddoc");
  }

  await filtrar();
}

function configurarSelectsComparativo() {
  $("#lista_vigencia").prop("disabled", true);

  sindLaboralComparativoSelect = new SelectWrapper(
    "#sind_laboral_comparativo",
    {
      options: { placeholder: "Selecione", multiple: false },
      onOpened: async () => {
        const cnaes = $("#categoria").val();
        const idGrupo = $("#grupo").val();
        const matrizes = $("#matriz").val();
        const unidades = $("#unidade").val();
        const localizacoes = $("#localidade").val();
        const tipoLocalidade = $("#tipoLocalidade").val();
        const arrayLocalizacoes = localizacoes.map((loc) => {
          if (tipoLocalidade == "uf" || tipoLocalidade == "regiao") {
            return;
          }
          const split = loc.split("+");
          return split[0];
        });
        const params = {
          clientesUnidadesIds: unidades,
          matrizesIds: matrizes,
          grupoEconomicoId: idGrupo,
          cnaesIds: cnaes,
        };

        if (tipoLocalidade == "uf") params.ufs = arrayLocalizacoes;
        if (tipoLocalidade == "regiao") params.regioes = arrayLocalizacoes;
        if (tipoLocalidade == "municipio")
          params.localizacoesIds = arrayLocalizacoes;

        return await sindLaboralService.obterSelectPorUsuario(params);
      },
      onChange: () => {
        comparativoFormMediator('sindLaboralComparativoSelectChange');
      },
      sortable: true,
    }
  );

  sindPatronalComparativoSelect = new SelectWrapper(
    "#sind_patronal_comparativo",
    {
      options: { placeholder: "Selecione", multiple: false, allowEmpty: true },
      onOpened: async () => {
        const sindLaboral = sindLaboralComparativoSelect.getValue();
        const params = {
          sindLaboral,
          grupoEconomicoId: gruposEconomicosSelect.getValue()
        };

        docReferenciaSelect.enable();

        return (await clausulaService.obterSindPatronalPorLaboral(params))
          .value;
      },
      onChange: () => {
        comparativoFormMediator('sindPatronalComparativoSelectChange');
      },
      sortable: true,
    }
  );

  docReferenciaSelect = new SelectWrapper("#doc_referencia", {
    options: { placeholder: "Selecione", multiple: false },
    onOpened: async () => {
      const sindLaboralId = sindLaboralComparativoSelect.getValue();
      const sindPatronalId = sindPatronalComparativoSelect.getValue();
      const params = {
        sindeId: sindLaboralId,
        sindpId: sindPatronalId,
      };

      vigenciaAtualSelect.reload();
      docAnteriorSelect.reload();
      vigenciaAnteriorSelect.reload();

      const result = (await clausulaService.obterNomeDocsPorSindicato(params))
        .value;

      if (result.length > 0) {
        vigenciaAtualSelect.enable();
      } else {
        vigenciaAtualSelect.disable();
      }

      comparativoFormMediator('docReferenciaSelectOpen');
      return result;
    },
    onChange: async () => comparativoFormMediator('docReferenciaSelectChange'),
    parentId: "#sind_laboral_comparativo",
    sortable: true,
  });

  docAnteriorSelect = new SelectWrapper("#doc_anterior", {
    options: { placeholder: "Selecione", multiple: false },
    onOpened: async () => {
      const sindLaboralId = sindLaboralComparativoSelect.getValue();
      const sindPatronalId = sindPatronalComparativoSelect.getValue();
      const params = {
        sindeId: sindLaboralId,
        sindpId: sindPatronalId,
      };

      vigenciaAnteriorSelect.reload();

      const result = (await clausulaService.obterNomeDocsPorSindicato(params))
        .value;

      if (result.length > 0) {
        vigenciaAnteriorSelect.enable();
      } else {
        vigenciaAnteriorSelect.disable();
      }

      comparativoFormMediator('docAnteriorSelectOpen')
      return result;
    },
    onChange: async () => comparativoFormMediator('docAnteriorSelectChange'),
    //parentId: "#vigencia_referencia",
    sortable: true,
  });

  vigenciaAtualSelect = new SelectWrapper("#vigencia_referencia", {
    options: { placeholder: "Selecione", multiple: false },
    onOpened: async (id) => {
      const sindLaboralId = sindLaboralComparativoSelect.getValue();
      const sindPatronalId = sindPatronalComparativoSelect.getValue();
      const params = {
        nomeDocumentoId: id,
        sindeId: sindLaboralId,
        sindpId: sindPatronalId,
      };

      docAnteriorSelect.reload();
      vigenciaAnteriorSelect.reload();

      const result = (await clausulaService.obterVigenciaPorDoc(params)).value;

      comparativoFormMediator('vigenciaAtualSelectOpen');
      return result;
    },
    onChange: async () => comparativoFormMediator('vigenciaAtualSelectChange'),
    parentId: "#doc_referencia",
    sortable: false,
  });

  vigenciaAnteriorSelect = new SelectWrapper("#vigencia_anterior", {
    options: { placeholder: "Selecione", multiple: false },
    onOpened: async (id) => {
      const vigenciaReferencia = vigenciaAtualSelect.getValue();
      const sindLaboralId = sindLaboralComparativoSelect.getValue();
      const sindPatronalId = sindPatronalComparativoSelect.getValue();
      const dataInicialReferenciaString = vigenciaReferencia.slice(6, 16).split("/").reverse().join("/");
      const params = {
        nomeDocumentoId: id,
        sindeId: sindLaboralId,
        sindpId: sindPatronalId,
        validadeInicialVigenciaReferencia: dataInicialReferenciaString,
      };

      comparativoFormMediator('vigenciaAnteriorSelectOpen');
      return (await clausulaService.obterVigenciaPorDoc(params)).value;
    },
    onChange: async () => comparativoFormMediator('vigenciaAnteriorSelectChange'),
    parentId: "#doc_anterior",
  });
}

async function configurarSelects(dadosPessoais) {
  const isIneditta = dadosPessoais.nivel == UsuarioNivel.Ineditta;
  const isGrupoEconomico = dadosPessoais.nivel == UsuarioNivel.GrupoEconomico;
  const isEstabelecimento = dadosPessoais.nivel == UsuarioNivel.Estabelecimento;

  const markOptionAsSelectable =
    dadosPessoais.nivel == "Cliente" ? () => true : () => false;

  gruposEconomicosSelect = new SelectWrapper("#grupo", {
    options: { placeholder: "Selecione" },
    onChange: async () => {
      if (cnaeSelect) {
        await cnaeSelect.reload();
      }
      if (sindLaboralSelect) {
        await sindLaboralSelect.reload();
      }
      if (sindPatronalSelect) {
        await sindPatronalSelect.reload();
      }
      if (dataBaseSelect) {
        await dataBaseSelect.reload();
      }
    },
    onOpened: async () => await grupoEconomicoService.obterSelectPorUsuario(),
    markOptionAsSelectable: isIneditta ? () => false : () => true,
  });
  if (isIneditta) {
    gruposEconomicosSelect.enable();
  } else {
    gruposEconomicosSelect.disable();
    await gruposEconomicosSelect.loadOptions();
  }

  empresasSelect = new SelectWrapper("#matriz", {
    options: {
      placeholder: "Selecione",
      multiple: true,
    },
    parentId: "#grupo",
    onChange: async () => {
      if (cnaeSelect) {
        await cnaeSelect.reload();
      }
      if (sindLaboralSelect) {
        await sindLaboralSelect.reload();
      }
      if (sindPatronalSelect) {
        await sindPatronalSelect.reload();
      }
    },
    onOpened: async (grupoEconomicoId) =>
      await matrizService.obterSelectPorUsuario(grupoEconomicoId),
    markOptionAsSelectable:
      isIneditta || isGrupoEconomico ? () => false : () => true,
  });
  if (isIneditta || isGrupoEconomico) {
    empresasSelect.enable();
  } else {
    const options = await empresasSelect.loadOptions()
    if (!(options instanceof Array && options.length > 1) || isEstabelecimento) {
      empresasSelect.disable()
    }
    else {
      empresasSelect.config.markOptionAsSelectable = () => false;
      empresasSelect.clear();
      empresasSelect.enable();
    }
  }

  estabelecimentosSelect = new SelectWrapper("#unidade", {
    options: {
      placeholder: "Selecione",
      multiple: true,
    },
    parentId: "#matriz",
    onChange: async () => {
      if (cnaeSelect) {
        await cnaeSelect.reload();
      }
      if (sindLaboralSelect) {
        await sindLaboralSelect.reload();
      }
      if (sindPatronalSelect) {
        await sindPatronalSelect.reload();
      }
    },
    onOpened: async (matrizId) =>
      await clienteUnidadeService.obterSelectPorUsuario(matrizId),
    markOptionAsSelectable: isEstabelecimento ? () => true : () => false,
  });
  if (isEstabelecimento) {
    const options = await estabelecimentosSelect.loadOptions()
    if (!(options instanceof Array && options.length > 1)) {
      estabelecimentosSelect.disable()
    }
    else {
      estabelecimentosSelect.config.markOptionAsSelectable = () => false;
      estabelecimentosSelect?.clear();
      estabelecimentosSelect.enable();
    }
  } else {
    estabelecimentosSelect.enable();
  }

  localizacoesSelect = new SelectWrapper("#localidade", {
    options: { placeholder: "Selecione", multiple: true },
    onChange: async () => {
      await sindLaboralSelect.reload();
      await sindPatronalSelect.reload();
    },
    onOpened: async () => await obterLocalidades(),
    markOptionAsSelectable: markOptionAsSelectable,
  });

  cnaeSelect = new SelectWrapper("#categoria", {
    options: { placeholder: "Selecione", multiple: true },
    onOpened: async () => {
      const options = {
        gruposEconomicosIds: $("#grupo").val() ?? null,
        matrizesIds: $("#matriz").val() ?? null,
        clientesUnidadesIds: $("#unidade").val() ?? null,
      };
      return await cnaeService.obterSelectPorUsuario(options);
    },
    onChange: async () => {
      await sindLaboralSelect.reload();
      await sindPatronalSelect.reload();
    },
    markOptionAsSelectable: markOptionAsSelectable,
  });

  sindPatronalSelect = new SelectWrapper("#sind_patronal", {
    options: { placeholder: "Selecione", multiple: true },
    onOpened: async () => {
      const options = obterParametrosParaRequisicaoDeSindicatos();
      return await sindPatronalService.obterSelectPorUsuario(options);
    },
    markOptionAsSelectable: markOptionAsSelectable,
  });
  sindLaboralSelect = new SelectWrapper("#sind_laboral", {
    options: { placeholder: "Selecione", multiple: true },
    onOpened: async () => {
      const options = obterParametrosParaRequisicaoDeSindicatos();
      return await sindLaboralService.obterSelectPorUsuario(options);
    },
    onChange: async () => {
      if (dataBaseSelect) {
        await dataBaseSelect.reload();
      }
    },
    markOptionAsSelectable: markOptionAsSelectable,
  });

  tipoDocSelect = new SelectWrapper("#nome_doc", {
    options: { placeholder: "Selecione", multiple: true },
    onOpened: async () => {
      return await tipoDocService.obterSelectPorTipos({ processado: true });
    },
  });

  grupoClausulaSelect = new SelectWrapper("#grupo_clausulas", {
    onOpened: async () => (await grupoClausulaService.obterSelect()).value,
    sortable: true,
  });

  estruturaClausulaSelect = new SelectWrapper("#estrutura_clausula", {
    onOpened: async (grupoClausula) =>
      (await estruturaClausulaService.obterSelectPorGrupo(grupoClausula)).value,
    sortable: true,
    parentId: "#grupo_clausulas",
  });

  estruturaClausulaSelect.enable();

  dataBaseSelect = new SelectWrapper("#data_base", {
    options: {allowEmpty: true},
    onOpened: async () => {
      let grupos;
      if ($("#grupo").val()) {
        grupos = [$("#grupo").val()]
      }
      const requestData = {
        sindLaboralIds: $("#sind_laboral").val(),
        sindPatronalIds: $("#sind_patronal").val(),
        grupoEconomicoIds: grupos
      };
      return await clausulaService.obterDatasBases(requestData);
    },
  });
}

async function obterPermissoes() {
  const modulos = (await usuarioAdmService.obterPermissoes()).value;

  comentar =
    modulos.length > 0
      ? modulos.find((modulo) => modulo.modulos === "Cláusulas").comentar ===
        "1"
      : false;
  
  mapaSindicalCsvExcelPermissoes = modulos.find(modulo => modulo.modulo_id === MODULO_MAPA_SINDICAL_CSV_EXCEL_ID);
}

function obterParametrosParaRequisicaoDeSindicatos() {
  const campoLocalidade = $("#localidade").val();
  const localizacoesIds = campoLocalidade
    .map((loc) => {
      if (loc.includes("municipio")) {
        return Number(loc.split(":")[1]);
      }
      return null;
    })
    .filter((x) => x !== null);

  const ufs = campoLocalidade
    .map((loc) => {
      if (loc.includes("uf")) {
        return loc.split(":")[1];
      }
      return null;
    })
    .filter((x) => x !== null);

  const options = {
    gruposEconomicosIds: $("#grupo").val() ?? null,
    matrizesIds: $("#matriz").val() ?? null,
    clientesUnidadesIds: $("#unidade").val() ?? null,
    cnaesIds: $("#categoria").val() ?? null,
    localizacoesIds,
    ufs,
  };

  return options;
}

function configurarModal() {
  const pageCtn = document.getElementById("pageCtn");

  const documentoModalHidden = document.getElementById("documentoModalHidden");
  const documentoModalContent = document.getElementById("documentoModalContent");

  const infoAdicional = document.getElementById("infoAdicionalModalHidden");
  const contentInfoAdicional = document.getElementById("infoAdicionalModalHiddenContent");

  const modalClausula = document.getElementById("clausulaModalHidden");
  const contentClausula = document.getElementById("clausulaModalHiddenContent");

  const buttonsClausulaConfig = [
    {
      id: "gerarPDF",
      onClick: async () => {
        await generatePdf();
      },
      data: null,
    },
  ];

  const modalComentario = document.getElementById("comentarioModalHidden");
  const contentComentario = document.getElementById(
    "comentarioModalHiddenContent"
  );

  const buttonsComentarioConfig = [
    {
      id: "notificacaoCadastrarBtn",
      onClick: async (id, modalContainer) => {
        const result = await incluirComentario()

        if (result.isSuccess()) {
          const ids = clausulasSelecionadas;
          const grupoEconomico = gruposEconomicosSelect.getValue();

          if (clausulaClicada) {
            const result = await clausulaService.obterPorId([clausulaClicada])
            const comentarios = await clausulaService.obterComentariosPorId([clausulaClicada], grupoEconomico)

            preencherModalClausula(result.value, comentarios.value);
          } else if (ids.length > 0) {
            const result = await clausulaService.obterPorId(ids);
            const comentarios = await clausulaService.obterComentariosPorId(
              ids,
              grupoEconomico
            );
            preencherModalClausula(result.value, comentarios.value);
          }

          closeModal(modalContainer);
        }
      },
      data: null,
    },
  ];

  const modalComparativo = document.getElementById(
    "comparativoFiltroModalHidden"
  );
  const contentComparativo = document.getElementById(
    "comparativoFiltroModalHiddenContent"
  );

  const buttonsComparativoConfig = [
    {
      id: "compararBtn",
      onClick: async () => {
        await compararClausula();
      },
      data: null,
    },
    {
      id: "gerarPDFComparativo",
      onClick: async () => {
        await generateComparativePdf();
      },
      data: null,
    },
  ];

  const modalsConfig = [
    {
      id: "documentoModal",
      modal_hidden: documentoModalHidden,
      content: documentoModalContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarDataTableDocumentosTb();
      },
      onClose: () => {
        //eventoClausulaSelecionadoId = null;
      },
      isInIndex: true,
      styles: {
        container: {
          paddingRight: '30px',
          paddingLeft: '30px'
        },
        modal: {
          maxWidth: '1800px',
          width: '100%'
        }
      }
    },
    {
      id: "infoAdicionalModal",
      modal_hidden: infoAdicional,
      content: contentInfoAdicional,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarInformacoesModal();
        $("#informacoes_adicionais .form-control").attr("disabled", true);
      },
      onClose: () => {
        $("#informacoes_adicionais").html(null);
      },
      isInIndex: true
    },
    {
      id: "clausulaModal",
      modal_hidden: modalClausula,
      content: contentClausula,
      btnsConfigs: buttonsClausulaConfig,
      onOpen: async () => {
        const ids = clausulasSelecionadas;
        const grupoEconomico = gruposEconomicosSelect.getValue()
        if (clausulaClicada) {
          const result = await clausulaService.obterPorId([clausulaClicada]);
          const comentarios = await clausulaService.obterComentariosPorId([
            clausulaClicada,
          ], grupoEconomico);
          preencherModalClausula(result.value, comentarios.value);

          return;
        }
        if (ids.length > 0) {
          const result = await clausulaService.obterPorId(ids);
          const comentarios = await clausulaService.obterComentariosPorId(ids, grupoEconomico);
          preencherModalClausula(result.value, comentarios.value);
        }
      },
      onClose: () => {
        clausulaClicada = null;
        $("#clausulaModalContainer").html(null);
        setTimeout(() => {
          $("body").attr("style", "padding-right: 0");
        }, 5);
      },
      styles: {
        container: {
          paddingRight: "30px",
          paddingLeft: "30px",
        },
        modal: {
          maxWidth: "1800px",
          width: "100%",
        },
      },
    },
    {
      id: "comentarioModal",
      modal_hidden: modalComentario,
      content: contentComentario,
      btnsConfigs: buttonsComentarioConfig,
      onOpen: async () => {
        configurarFormularioModal();
        setTimeout(() => {
          $(".modal-backdrop").removeClass("in");
        }, 5);
      },
      onClose: () => {
        $("#clausula-id-input").val(null);
        setTimeout(() => {
          $("body").addClass("modal-open").attr("style");
        }, 5);
      },
      isInIndex: true,
    },
    {
      id: "comparativoFiltroModal",
      modal_hidden: modalComparativo,
      content: contentComparativo,
      btnsConfigs: buttonsComparativoConfig,
      onOpen: async () => {
        if (sindLaboralSelect.getSelectedOptions().length === 0) {
          sindLaboralComparativoSelect.reload();
        }
        $("#only-diff").prop("checked", false);
        docReferenciaSelect.disable();
        docReferenciaSelect.reload();
        docAnteriorSelect.reload();
        docAnteriorSelect.disable();
        vigenciaAtualSelect.reload();
        vigenciaAtualSelect.disable();
        vigenciaAnteriorSelect.reload();
        vigenciaAnteriorSelect.disable();
        sindPatronalComparativoSelect.reload();
        setTimeout(() => {
          $(".modal-backdrop").removeClass("in");
        }, 5);
      },
      onClose: () => {
        documentoGridMode = null;
        documentoReferenciaId = null;
        documentoAnteriorId = null;
        $("#containerResultadoComparacao").html(null);
        $("#resultadoComparacao").attr("style", "display: none;");
      },
      styles: {
        container: {
          paddingRight: "30px",
          paddingLeft: "30px",
        },
        modal: {
          maxWidth: "1800px",
          width: "100%",
        },
      },
    },
  ];

  renderizarModal(pageCtn, modalsConfig);
}

function configurarFormularioModal() {
  $("#assunto").val(dadosClausula.nomeClausula)

  tipoUsuarioDestinoSelect = new SelectWrapper('#tipo_usuario_destino', {
    onOpened: async () => await obterTipoUsuarioDestinoSelect(),
    dropdownParent: $("#notificacaoModal")
  })
  destinoSelect = new SelectWrapper('#destino', {
    onOpened: async (tipoUsuarioId) => {
      configurarTituloTipoUsuario(tipoUsuarioId)
      return await usuarioDestinoSelect({ tipoUsuarioId, isIneditta: usuario.nivel == UsuarioNivel.Ineditta, grupoEconomicoId: usuario.grupoEconomicoId })
    },
    parentId: '#tipo_usuario_destino',
    sortable: true
  })
  tipoNotificacaoSelect = new SelectWrapper("#tipo-note", {
    onOpened: async () => await obterTipoNotificacaoSelect(),
    onSelected: ({ id }) => {
      if (id == TipoNotificacao.Fixa) {
        dataValidade.clear()
        dataValidade.disable()
      } else {
        dataValidade.enable()
      }
    },
  })
  dataValidade = new DatepickerWrapper("#validade");
  new SelectWrapper('#tipo-etiqueta', {
    onOpened: async () => (await tipoEtiquetaService.obterSelect({ tipoEtiquetaNome: 'Cláusulas' })).value,
    options: {
      allowEmpty: true
    }
  })
  etiquetaSelect = new SelectWrapper('#etiqueta', { onOpened: async (tipoEtiquetaId) => (await etiquetaService.obterSelect({ tipoEtiquetaId })).value, parentId: '#tipo-etiqueta', sortable: true })
  visivelSelect = new SelectWrapper('#visivel', { onOpened: async () => await obterBooleanSelect() })

  $("#destino").on("select2:select", (event) => {
    const value = event.currentTarget.value
    configurarTituloTipoUsuario(value)
  })

  carregarInformacoesUsuario()
}

function configurarTituloTipoUsuario(tipoUsuarioId) {
  switch (tipoUsuarioId) {
    case "grupo":
      $("#campo_tipo").html("Grupo Econômico");
      return;
    case "matriz":
      $("#campo_tipo").html("Empresa");
      return;
    case "unidade":
      $("#campo_tipo").html("Estabelecimento");
      return;
    default:
      $("#campo_tipo").html("--");
      return;
  }
}

function carregarInformacoesUsuario() {
  $("#id_user_2").val(UserInfoService.getUserId());

  const nomeUsuario =
    UserInfoService.getFirstName() + " " + UserInfoService.getLastName();

  $("#usuario").val(nomeUsuario);
}

function preencherModalClausula(dataArray, comentariosArray) {
  dadosClausulasPorId = [];
  
  $(".modal-backdrop").removeClass("in");
  const clausulasContainer = $("#clausulaModalContainer");
  dataArray.forEach((data) => {
    const buttonInfoAdicional = $("<button>")
                                .addClass("btn-info-adicional btn btn-primary " + (!data?.possuiInformacaoAdicional ? "hide" : ""))
                                .attr("id", "abrirInfoAdicionais-"+data?.id)
                                .html("<i class='fa fa-file-pdf'></i>Visualizar informações adicionais")
                                .on("click", async () => {
                                  clausulaClicada = Number(data?.id);
                                  
                                  const r = await obterClausulasPorId();
                                  if (r.isFailure()) {
                                    $("#abrirInfoAdicionais-"+data?.id).prop("disabled", true);
                                    return;
                                  }
                                  
                                  $("#abrirInfoAdicionais-main").trigger("click");
                                });

    const painel = $("<div>").addClass("panel panel-primary");
    const painelHeading = $("<div>").addClass("panel-heading");
    const titulo = $("<h4>").text(`Sobre a Cláusula: ${data.nomeClausula}`);
    const options = $("<div>").addClass("options");
    const closeBtn = $("<a>")
      .attr("href", "javascript:;")
      .attr("id", `collapseClau-${data.id}`)
      .addClass("panel-collapse")
      .html("<i class='fa fa-chevron-down'></i>");
    options.append(closeBtn);
    painelHeading.append([titulo, options]);

    const painelBody = $("<div>")
      .addClass("panel-body collapse in")
      .attr("id", `collapseClau-${data.id}-body`);

    const mainRow = $("<div>").addClass("row");
    const sindColumn = $("<div>")
      .addClass("col-lg-8 sind_column")
      .attr("id", `sindcolumn-${data.id}`);

    const sindLaboralTb = $("<table>").addClass(
      "table table-striped table-bordered"
    );
    const sindLaboralThead = $("<thead>").append(
      $("<tr>").append($("<th>").text("Sigla Sindicato Laboral / Denominação / UF"))
    );
    const sindLaboralTbody = $("<tbody>");
    data.sindLaboral?.forEach((sind) => {
      const sindRow = $("<tr>").append(
        $("<td>").text(`${sind.sigla} / ${sind.denominacao} / ${sind.uf}`)
      );
      sindLaboralTbody.append(sindRow);
    });
    sindLaboralTb.append([sindLaboralThead, sindLaboralTbody]);
    let sindPatronalTb;
    if (data.sindPatronal?.length > 0) {
      sindPatronalTb = $("<table>").addClass(
        "table table-striped table-bordered"
      );
      const sindPatronalThead = $("<thead>").append(
        $("<tr>").append($("<th>").text("Sigla Sindicato Patronal / Denominação / UF"))
      );

      const sindPatronalTbody = $("<tbody>");
      data.sindPatronal?.forEach((sind) => {
        const sindRow = $("<tr>").append(
          $("<td>").text(`${sind.sigla} / ${sind.denominacao} / ${sind.uf}`)
          );
          sindPatronalTbody.append(sindRow);
        });
      sindPatronalTb.append([sindPatronalThead, sindPatronalTbody]);
    } else {
      sindPatronalTb = null
    }

    const clauTb = $("<table>").addClass("table table-striped table-bordered");
    const clauTheadRow = $("<tr>");
    clauTheadRow.append($("<th>").text("Grupo da Cláusula"));
    clauTheadRow.append($("<th>").text("Nome da Cláusula"));
    clauTheadRow.append($("<th>").text("Documento"));
    clauTheadRow.append($("<th>").text("Data Processamento"));
    const clauThead = $("<thead>").append(clauTheadRow);
    const clauTbodyRow = $("<tr>").addClass("odd gradeX");
    clauTbodyRow.append($("<td>").text(data.grupoClausula));
    clauTbodyRow.append($("<td>").text(data.nomeClausula));
    clauTbodyRow.append($("<td>").text(data.nomeDocumento));
    clauTbodyRow.append($("<td>").text(convertDate(data.dataAprovacaoClausula)));
    const clauTbody = $("<tbody>").append(clauTbodyRow);
    clauTb.append([clauThead, clauTbody]);

    const datesTb = $("<table>").addClass("table table-striped table-bordered");
    const datesTheadRow = $("<tr>");
    datesTheadRow.append($("<th>").text("Validade Inicial"));
    datesTheadRow.append($("<th>").text("Validade Final"));
    datesTheadRow.append($("<th>").text("Data Base"));
    datesTheadRow.append($("<th>").text("Atividade Econômica"));
    const datesThead = $("<thead>").append(datesTheadRow);
    const datesTbodyRow = $("<tr>").addClass("odd gradeX");
    datesTbodyRow.append($("<td>").text(convertDate(data.validadeInicial)));
    datesTbodyRow.append($("<td>").text(convertDate(data.validadeFinal)));
    datesTbodyRow.append($("<td>").text(data.dataBase));
    const cnaeArray = data.cnae.map(cnae => cnae.subclasse);
    datesTbodyRow.append($("<td>").text(cnaeArray.join(' / ')));
    const datesTbody = $("<tbody>").append(datesTbodyRow);
    datesTb.append([datesThead, datesTbody]);

    sindColumn.append([sindLaboralTb, sindPatronalTb, clauTb, datesTb]);

    const estabelecimentosTb = $("<table>")
      .attr("style", "width: 100%")
      .attr("cellpadding", "0")
      .attr("cellspacing", "0")
      .attr("border", "0")
      .addClass("table table-striped table-bordered demo-tbl")
      .attr("id", `estabelecimentostb-${data.id}`);

    const unidadesColumn = $("<div>").addClass("col-lg-4 unidades_column");
    unidadesColumn.append([estabelecimentosTb]);

    mainRow.append([sindColumn, unidadesColumn]);

    painelBody.append([mainRow]);
    painel.append([painelHeading, painelBody]);

    const textoClausulaPainel = $("<div>").addClass("panel panel-primary");
    const textoClausulaPainelHeading = $("<div>").addClass("panel-heading");
    const textoClausulaTitulo = $("<h4>").text(
      `Texto da Cláusula: ${data.nomeClausula}`
    );
    const textoClausulaOptions = $("<div>").addClass("options");
    const textoClausulaCloseBtn = $("<a>")
      .attr("href", "javascript:;")
      .attr("id", `collapseClauText-${data.id}`)
      .addClass("panel-collapse")
      .html("<i class='fa fa-chevron-down'></i>");
    textoClausulaOptions.append(textoClausulaCloseBtn);
    textoClausulaPainelHeading.append([
      textoClausulaTitulo,
      textoClausulaOptions,
    ]);

    const textoClausulaPainelBody = $("<div>")
      .addClass("panel-body collapse in")
      .attr("id", `collapseClauText-${data.id}-body`);

    const addCommentBtn = $("<button>")
      .attr("type", "button")
      .attr("data-toggle", "modal")
      .attr("data-target", "#comentarioModal")
      .prop("disabled", !comentar)
      .addClass("btn btn-primary btn-rounded")
      .html(`<i class='fa fa-comments-o'></i> Adicionar Comentário`)
      .on("click", () => {
        $("#clausula-id-input").val(data.id);
        dadosClausula = data;
      });
    const textToCopy = data.textoClausula;
    const copyBtn = $("<button>")
      .addClass("btn btn-primary")
      .html("<i class='fa fa-copy'></i>")
      .on("click", () => {
        navigator.clipboard.writeText(textToCopy).then(() => {
          NotificationService.success({
            title: "Copiado com sucesso!",
            message: "Texto da cláusula copiado para área de transferência.",
          });
        });
      });
    // const compareBtn = $("<button>")
    //   .prop("disabled", true)
    //   .addClass("btn btn-primary")
    //   .html("Comparar");
    const textoClausulaButtons = $("<div>")
      .attr("style", "margin-bottom: 1rem; display: flex; gap: 10px")
      .addClass("clausula_text_toolbar")
      .append([addCommentBtn, copyBtn, (mapaSindicalCsvExcelPermissoes?.consultar == 1 ? buttonInfoAdicional : null)]);

    const textoClausula = $("<p>")
      .attr("style", "text-align: justify; white-space: pre-line")
      .text(data.textoClausula)
      .mark(palavraChaveSearched, {
        accuracy: "partially",
        diacritics: true,
      });

    textoClausulaPainelBody.append([textoClausulaButtons, textoClausula]);
    textoClausulaPainel.append([
      textoClausulaPainelHeading,
      textoClausulaPainelBody,
    ]);

    const comentariosClausulaPainel = $("<div>").addClass(
      "panel panel-primary"
    );
    const comentariosClausulaPainelHeading =
      $("<div>").addClass("panel-heading");
    const comentariosClausulaTitulo = $("<h4>").text(
      `Comentários da Cláusula: ${data.nomeClausula}`
    );
    const comentariosClausulaOptions = $("<div>").addClass("options");
    const comentariosClausulaCloseBtn = $("<a>")
      .attr("href", "javascript:;")
      .attr("id", `collapseClauComm-${data.id}`)
      .addClass("panel-collapse")
      .html("<i class='fa fa-chevron-down'></i>");
    comentariosClausulaOptions.append(comentariosClausulaCloseBtn);
    comentariosClausulaPainelHeading.append([
      comentariosClausulaTitulo,
      comentariosClausulaOptions,
    ]);

    const comentariosClausulaPainelBody = $("<div>")
      .addClass("panel-body collapse in")
      .attr("id", `collapseClauText-${data.id}-body`);

    const comentariosTb = $("<table>").addClass("table table-striped");
    const comentariosTheadRow = $("<tr>");
    comentariosTheadRow.append($("<th>").text("Usuário"));
    comentariosTheadRow.append($("<th>").text("Data"));
    comentariosTheadRow.append($("<th>").text("Etiqueta"));
    comentariosTheadRow.append($("<th>").text("Comentário"));
    const comentariosThead = $("<thead>").append(comentariosTheadRow);
    const comentariosTbody = $("<tbody>");
    const comentarioData = []
    comentariosArray
      ?.filter((comm) => comm.idClausula == data.id)
      ?.forEach((comentario) => {
        const commRow = $("<tr>");
        commRow.append($("<td>").text(comentario.nomeUsuario));
        commRow.append(
          $("<td>").text(convertDateTime(comentario.dataRegistro ))
        );
        commRow.append($("<td>").text(comentario.etiqueta ?? ''));
        commRow.append($("<td>").text(comentario.comentario ?? ''));
        comentariosTbody.append(commRow);
        comentarioData.push(comentario);
      });
    data.comentarios = comentarioData;
      
    comentariosTb.append([comentariosThead, comentariosTbody]);

    comentariosClausulaPainelBody.append([comentariosTb]);
    comentariosClausulaPainel.append([
      comentariosClausulaPainelHeading,
      comentariosClausulaPainelBody,
    ]);

    clausulasContainer.append([
      painel,
      textoClausulaPainel,
      comentariosClausulaPainel,
    ]);

    const unidadesData = data.unidade.filter((uni) => grupoEconomicoFiltrado ? uni.g == grupoEconomicoFiltrado : data.unidade)

    $(
      `#estabelecimentostb-${data.id}`
    ).DataTable({
      data: unidadesData,
      scrollY: 320,
      columns: [
        {
          title: "Estabelecimentos Abrangidos Pela Cláusula",
          data: "nomeUnidade",
        },
      ],
      searching: false,
      paging: false,
      info: false,
    });

    data.textoClausula = textoClausula.prop("outerHTML");
    dadosClausulasPorId.push(data);
  });
}

function configurarInformacaoSindicatoService() {
  const modalInfoSindicato = new ModalInfoSindicato(renderizarModal,sindicatoService,DataTableWrapper);
  modalInfoSindicato.initialize("info-modal-sindicato-container");
}

async function carregarDataTableDocumentosTb() {
  if (documentosTb) {
    documentosTb.reload();
    return;
  }

  documentosTb = new DataTableWrapper("#documentosTb", {
    ajax: async (requestData) => {
      let validadeInicial = null;
      let validadeFinal = null;
      let tipoDocumentoId = null;

      if (documentoGridMode == documentoGridModeOptions.referencia) {
        const vigenciaReferencia = vigenciaAtualSelect.getValue();

        const dataInicialReferenciaString = vigenciaReferencia.slice(6, 16).split("/").reverse().join("/");
        const dataFinalReferenciaString = vigenciaReferencia.slice(17,27).split("/").reverse().join("/");

        validadeInicial = new Date(dataInicialReferenciaString);
        validadeFinal = new Date(dataFinalReferenciaString);
        tipoDocumentoId = docReferenciaSelect?.getValue();
      }

      if (documentoGridMode == documentoGridModeOptions.anterior) {
        const vigenciaAnterior = vigenciaAnteriorSelect.getValue();

        const dataInicialAnteriorString = vigenciaAnterior.slice(6, 16).split("/").reverse().join("/");
        const dataFinalAnteriorString = vigenciaAnterior.slice(17,27).split("/").reverse().join("/");

        validadeInicial = new Date(dataInicialAnteriorString);
        validadeFinal = new Date(dataFinalAnteriorString);
        tipoDocumentoId = docAnteriorSelect?.getValue();
      }

      requestData.tipoConsulta = "sisap";
      requestData.sindicatosLaboraisIds = [sindLaboralComparativoSelect?.getValue()];
      requestData.sindicatosPatronaisIds = sindPatronalComparativoSelect?.getValue() ? [sindPatronalComparativoSelect?.getValue()] : null;
      requestData.tipoDocumentoId = tipoDocumentoId;
      requestData.dataValidadeInicial = validadeInicial;
      requestData.dataValidadeFinal = validadeFinal;
      requestData.UsarComparacaoEstritaNasValidades = true;
      return await docSindService.obterDatatableConsulta(requestData)
    },
    rowCallback: function (row, data) {
      const checkbox = $("<input>").attr("type", "checkbox").attr("data-id", data?.id).addClass('documentoId')

      checkbox.on('click', () => {
        $(".documentoId").prop('checked', false);
        if(documentoGridMode == documentoGridModeOptions.referencia) {
          if (documentoAnteriorId == data?.id) {
            NotificationService.error({title: "Seleção Proibida", message: "Documento já selecionado como anterior"});
            return;
          }
          documentoReferenciaId = data?.id;
          docAnteriorSelect?.enable();
        }

        if(documentoGridMode == documentoGridModeOptions.anterior) {
          if (documentoReferenciaId == data?.id) {
            NotificationService.error({title: "Seleção Proibida", message: "Documento já selecionado como referência"});
            return;
          }
          documentoAnteriorId = data?.id;
          docAnteriorSelect?.enable();
        }

        $(checkbox).prop('checked', true);
      });

      $("td:eq(0)", row).html(checkbox);

      if (documentoGridMode == documentoGridModeOptions.referencia) {
        const isChecked = documentoReferenciaId == data?.id;
        if (isChecked === true) {
          $(row).find('.documentoId').prop('checked', true);
        }
      }

      if (documentoGridMode == documentoGridModeOptions.anterior) {
        const isChecked = documentoAnteriorId == data?.id;
        if (isChecked === true) {
          $(row).find('.documentoId').prop('checked', true);
        }
      }
    },
    columns: [
      { data: "id" },
      {
        title: "Sindicato Patronal",
        data: "siglasSindicatosPatronais",
        render: (data) => {
          if (!data) return '';
          return data.join(", ");
        },
        sortable: false,
      },
      { 
        title: "Abrangência", 
        data: "abrangencia", 
        render: (data) => {
          if (!data) return ''

          const values = data.map(a => (a.Municipio ?? a.municipio) + "/" + a.Uf).join("; ")

          return stringTextArea({
            className: '',
            content: values,
            style: 'max-height: 1000px; width: 100%; background: transparent;',
            disabled: true
          })
        },
        sortable: false,
      },
      { 
        title: "Atividade Econômica", 
        data: "atividadesEconomicas", 
        render: (data) => {
          if (!data) return '';
          return data.map(a => a.subclasse).join("; ")
        },
      },
      { 
        title: "Assuntos", 
        data: "assuntos",  
        render: (data) => data ? data.join("; ") : '',
        sortable: false, 
      },
    ],
    columnDefs: [
      {
        targets: [1, 2],
        render: (data) => DataTableWrapper.formatDate(data),
      },
      {
        targets: "_all",
        defaultContent: "",
      },
    ],
  });

  await documentosTb.initialize();
}

function configurarFormulario() {
  $("#filterBtn").on("click", async () => {
    await filtrar();
  });

  $("#selectAllInput").on("change", (event) => {
    if (event.currentTarget.checked) {
      selecionarTudo();
    } else {
      deselecionarTudo();
    }
  });

  $("#limparFiltroBtn").on("click", () => {
    if (isIneditta) {
      gruposEconomicosSelect.reload();
      empresasSelect.reload();
      estabelecimentosSelect.reload();
    }

    if (isGrupoEconomico) {
      empresasSelect.reload();
      estabelecimentosSelect.reload();
    }

    if (isEmpresa) {
      empresasSelect?.isEnable() && empresasSelect.reload();
      estabelecimentosSelect.reload();
    }
  
    if (isEstabelecimento && estabelecimentosSelect?.isEnable()){
      estabelecimentosSelect.reload();
    } 

    $("#data_base").val(null).trigger("change");
    localizacoesSelect.reload();
    cnaeSelect.reload();
    sindLaboralSelect.reload();
    sindPatronalSelect.reload();
    tipoDocSelect.reload();
    grupoClausulaSelect.reload();
    estruturaClausulaSelect.reload();
    $("#search").val(null).trigger("change");

    if (clausulasTb) {
      clausulasTb.clear();
      $("#table-container").hide();
    }
  });

  $("#search").on("keypress", function (e) {
    if (e.originalEvent.key === "Enter") {
      e.preventDefault();
    }
  })
}

async function obterLocalidades() {
  const municipios = await localizacaoService.obterSelectPorUsuario();
  const regioes = await localizacaoService.obterSelectRegioes(true);

  const localidades =
    municipios?.map((municipio) => ({
      id: `municipio:${municipio.id}`,
      description: municipio.description,
    })) ?? [];

  if (regioes?.length > 0) {
    localidades.push(
      ...regioes.map((regiao) => ({
        id: `uf:${regiao.description}`,
        description: regiao.description,
      }))
    );
  }

  return localidades;
}

async function filtrar() {
  $("#table-container").show();
  clausulasSelecionadas = [];
  await carregarClausulasDataTable();
}

async function carregarClausulasDataTable() {
  if (clausulasTb) {
    clausulasTb.reload();
    return;
  }

  $("#selectAllDiv").attr("style", "display: block; margin-bottom: 10px");

  clausulasTb = new DataTableWrapper("#clausulasTb", {
    searching: false,
    ajax: async (requestData) => {
      if ($("#grupo").val() != null && $("#grupo").val() != "") {
        requestData.grupoEconomicoIds = [$("#grupo").val()];
      }
      requestData.empresaIds = $("#matriz").val();
      requestData.unidadeIds = $("#unidade").val();
      requestData.tipoDocIds = $("#nome_doc").val();
      requestData.cnaeIds = $("#categoria").val();

      if (localizacoesSelect?.getValue()) {
        if (localizacoesSelect.getValue().some(localidade => localidade.indexOf('municipio:') > -1)) {
            const municipios = localizacoesSelect.getValue().filter(localidade => localidade.indexOf('municipio:') > -1).map(municipio => municipio.split(':')[1]);
            requestData['municipiosIds'] = Array.isArray(municipios) ? municipios : [municipios];
        }

        if (localizacoesSelect.getValue().some(localidade => localidade.indexOf('uf:') > -1)) {
            const ufs = localizacoesSelect.getValue().filter(localidade => localidade.indexOf('uf:') > -1).map(value => value.split(':')[1]);
            requestData['ufs'] = Array.isArray(ufs) ? ufs : [ufs];
        }
    }

      requestData.sindLaboralIds = $("#sind_laboral").val();
      requestData.sindPatronalIds = $("#sind_patronal").val();
      requestData.dataBase = $("#data_base").val();
      requestData.grupoClausulaIds = $("#grupo_clausulas").val();
      requestData.estruturaClausulaIds = $("#estrutura_clausula").val();
      requestData.palavraChave = $("#search").val();
      palavraChaveSearched = $("#search").val();
      grupoEconomicoFiltrado = $("#grupo").val();
      if (idDoc) {
        requestData.idDoc = idDoc;
        idDoc = null;
      }

      const database = dataBaseSelect.getValue()
      
      requestData.tipoDataBase = 'data-base';
      if (database == 'vigente' || database == 'ultimo-ano') {
        requestData.tipoDataBase = database;
      }

      requestData.SortColumns = [
        {
          key: "id",
          value: "asc",
        },
      ];

      $("#selectAllInput").prop("checked", false);

      return await clausulaService.obterDatatable(requestData);
    },
    columns: [
      {
        title: "",
        orderable: false,
        data: "id",
      },
      {
        title: "Grupo da Cláusula",
        data: "grupoClausula",
      },
      { title: "Nome", data: "nomeClausula" },
      { title: "Documento", data: "nomeDocumento" },
      {
        title: "Sindicato Laboral",
        data: "sindLaboral",
      },
      {
        title: "Sindicato Patronal",
        data: "sindPatronal",
      },
      {
        title: "Texto da Cláusula",
        data: "textoClausula",
      },
      { title: "Data-base", data: "dataBase" },
      { title: "Validade Final", data: "validadeFinal" },
    ],
    rowCallback: function (row, data) {
      const checkbox = $("<input>")
        .attr("type", "checkbox")
        .attr("data-id", data?.id)
        .attr("checked", false)
        .addClass("clausula_checkbox");
      if (clausulasSelecionadas.includes(data?.id)) {
        checkbox.prop("checked", true);
        checkbox.trigger("change");
      }

      checkbox.on("change", (event) => {
        if (event.currentTarget.checked) {
          clausulasSelecionadas.includes(data?.id)
            ? null
            : clausulasSelecionadas.push(data?.id);
        } else {
          clausulasSelecionadas = clausulasSelecionadas.filter(
            (clau) => clau != data?.id
          );
        }
      });
      $("td:eq(0)", row).html(checkbox);

      const htmlPatronal = $("<span>");
      data?.sindPatronal?.map((sind, index) => {
        let linkPatronal = $("<a>")
          .attr("data-id", sind.id)
          .attr("href", "#")
          .html(sind.sigla);
        linkPatronal.on("click", function () {
          const id = $(this).attr("data-id");
          $("#sind-id-input").val(id);
          $("#tipo-sind-input").val("patronal");
          $("#openInfoSindModalBtn").trigger("click");
        });
        if (index > 0) {
          htmlPatronal.append(" / ");
        }
        htmlPatronal.append(linkPatronal);
      });
      $("td:eq(5)", row).html(htmlPatronal);

      const htmlLaboral = $("<span>");
      data?.sindLaboral?.map((sind, index) => {
        let linkLaboral = $("<a>")
          .attr("data-id", sind.id)
          .attr("href", "#")
          .html(sind.sigla);
        linkLaboral.on("click", function () {
          const id = $(this).attr("data-id");
          $("#sind-id-input").val(id);
          $("#tipo-sind-input").val("laboral");
          $("#openInfoSindModalBtn").trigger("click");
        });
        if (index > 0) {
          htmlLaboral.append(" / ");
        }
        htmlLaboral.append(linkLaboral);
      });
      $("td:eq(4)", row).html(htmlLaboral);

      const validade = renderizarValidade(convertDate(data.validadeFinal));
      $("td:eq(8)", row).html(validade);

      const outerHtmlTexto = renderizarTextoClausula(data.textoClausula);
      const htmlTexto = $("<div>")
        .attr("style", "display: flex;")
        .html(`${outerHtmlTexto}`);
      htmlTexto.on("click", function () {
        clausulaClicada = data.id;
        abrirClausulaPorId();
      });
      htmlTexto.on("hover", function () {
        this.style("color: black; text-decoration: underline;");
      });
      $("td:eq(6)", row).html(htmlTexto);
    },
    columnDefs: [
      {
        targets: "_all",
        defaultContent: "",
      },
    ],
  });

  await clausulasTb.initialize();
}

function renderizarTextoClausula(data) {
  if (!data) {
    return null;
  }

  const palavraChave = palavraChaveSearched;

  // IF NOT SEARCHED BY KEYWORD
  if (!palavraChave) {
    if (data.length <= 120) {
      const html = $("<div>").html(
        `${data} <i class="fa fa-external-link"></i>`
      );
      html.on("hover", function () {
        this.style("color: black;");
      });
      html.attr("style", "cursor: pointer");
      html.addClass("texto_clausula");
      const htmlProp = html.prop("outerHTML");
      return htmlProp;
    }
    const html = $("<div>").html(
      `${data.slice(0, 120)}... <i class="fa fa-external-link"></i>`
    );
    html.on("hover", function () {
      this.style("color: black;");
    });
    html.attr("style", "cursor: pointer");
    html.addClass("texto_clausula");
    const htmlProp = html.prop("outerHTML");
    return htmlProp;
  }

  // CREATE STRING PARTS AND HIGHLIGHT KEYWORDS
  const pattern = new RegExp(removerAcentos(palavraChave), "gi");
  const cleanedString = removerAcentos(data);
  let indices = [];
  let index = pattern.exec(cleanedString);
  while (index !== null) {
    indices.push(index.index);
    index = pattern.exec(cleanedString.slice(index.index + 1));
  }

  const stringParts = indices.map((i) => {
    const indexStart = i - 30 < 0 ? 0 : i - 30;
    const indexEnd = i + 30 > data.length - 1 ? data.length - 1 : i + 30;
    return {
      str: data.substring(indexStart, indexEnd),
      indexStart,
      indexEnd,
    };
  });
  const html = $("<div>");
  stringParts.forEach((part, i) => {
    const text = part.str;
    const cleanedStrPart = removerAcentos(text);
    const foundPattern = pattern.exec(cleanedStrPart);
    if (foundPattern === null) {
      return;
    }
    const indexString = foundPattern.index;
    const newString =
      (part.indexStart == 0 ? "" : "...") +
      text.slice(0, indexString) +
      `<span style="background-color: #f8ff00">${text.slice(
        indexString,
        indexString + palavraChave.length
      )}</span>` +
      text.slice(indexString + palavraChave.length) +
      (part.indexEnd == text.length - 1 ? "" : "...");

    if (i > 0) {
      html.append(" <strong>;</strong> ");
    }
    html.append(newString);
  });
  html.on("hover", function () {
    this.style("color: black;");
  });
  html.attr("style", "cursor: pointer");
  html.addClass("texto_clausula");
  html.append($("<i>").addClass("fa fa-external-link"));
  return html.prop("outerHTML");
}

function selecionarTudo() {
  $(".clausula_checkbox").prop("checked", true).trigger("change");
}

function deselecionarTudo() {
  $(".clausula_checkbox").prop("checked", false).trigger("change");
}

/*********************************************************************
 * ADD COMENTÁRIO
 ********************************************************************/

async function incluirComentario() {
  const isFixo = parseInt(tipoNotificacaoSelect?.getValue()) === TipoNotificacao.Fixa;

  if (dataValidade.getValue() === null && !isFixo) {
    return NotificationService.error({
      title: "Não foi possível realizar o cadastro! Erro: ",
      message: "Você precisa escolher uma validade para comentários temporários.",
    })
  }

  const requestData = {
    id: 0,
    tipo: TipoComentario.Clausula,
    valor: $("#comentario").val(),
    tipoNotificacao: parseInt(tipoNotificacaoSelect.getValue()),
    referenciaId: dadosClausula.id,
    dataValidade: dataValidade?.getValue(),
    tipoUsuarioDestino: parseInt(tipoUsuarioDestinoSelect.getValue()),
    usuarioDestionoId: parseInt(destinoSelect.getValue()),
    etiquetaId: parseInt(etiquetaSelect.getValue()),
    visivel: visivelSelect.getValue() == BooleanType.Sim ? true : null
  }

  const result = await comentarioService.incluir(requestData)

  if (result.isFailure()) {
    return NotificationService.error({
      title: "Não foi possível realizar o cadastro! Erro: ",
      message: result.error,
    })
  }

  NotificationService.success({ title: 'Notificação cadastrada com sucesso, visite "Cadastro de Notificação" para visualizar!' })

  $("#clausulaModalContainer").html(null)

  return Result.success()
}

/*********************************************************************
 * FUNÇÕES PRINCIPAIS
 ********************************************************************/

function abrirClausulaPorId() {
  if (clausulasSelecionadas.length === 0 && !clausulaClicada) {
    NotificationService.error({
      title: "Não é possível abrir",
      message: "Selecione pelo menos uma cláusula!",
    });

    return;
  }

  $("#openClausulaModalBtn").trigger("click");
}

$("#texto_clausula").prop("disabled", true);

/*********************************************************************
 * COMPARA DOCUMENTO
 ********************************************************************/

async function compararClausula() {
  totalClausulasComparativo = 0;
  dadosComparativoClausula = [];
  const vigenciaReferencia = vigenciaAtualSelect.getValue();
  const vigenciaAnterior = vigenciaAnteriorSelect.getValue();
  if (
    !sindLaboralComparativoSelect.getValue() ||
    !docReferenciaSelect.getValue() ||
    !docAnteriorSelect.getValue() ||
    !vigenciaReferencia ||
    !vigenciaAnterior
  ) {
    NotificationService.error({
      title: "Não é possível comparar",
      message: "Preencha os campos antes de comparar!",
    });

    return;
  }

  const validadeInicialReferencia = vigenciaReferencia.slice(6, 16);
  const validadeFinalReferencia = vigenciaReferencia.slice(17, 27);
  const validadeInicialAnterior = vigenciaAnterior.slice(6, 16);
  const validadeFinalAnterior = vigenciaAnterior.slice(17, 27);

  const requestData = {
    documentoReferenciaId,
    documentoAnteriorId,
    sindeId: sindLaboralComparativoSelect.getValue(),
    sindpId: sindPatronalComparativoSelect.getValue(),
    tipoDocIdReferencia: docReferenciaSelect.getValue(),
    tipoDocIdAnterior: docAnteriorSelect.getValue(),
    validadeInicialReferencia: formatDateToDateOnly(validadeInicialReferencia),
    validadeFinalReferencia: formatDateToDateOnly(validadeFinalReferencia),
    validadeInicialAnterior: formatDateToDateOnly(validadeInicialAnterior),
    validadeFinalAnterior: formatDateToDateOnly(validadeFinalAnterior),
    grupoClausula: grupoClausulaSelect.getValue(),
    estruturaClausula: estruturaClausulaSelect.getValue(),
    exibirDiferencas: $("#only-diff").prop("checked"),
  };

  const result = await clausulaService.obterComparacaoClausulas(requestData);

  if (result.isFailure()) {
    NotificationService.error({
      title: "Ocorreu um erro ao comparar",
      message: "Erro:" + result.error,
    });

    return result;
  }

  $("#containerResultadoComparacao").html(null);

  const nomeDocReferencia = docReferenciaSelect.getSelectedOptions();
  const nomeDocAnterior = docAnteriorSelect.getSelectedOptions();

  result.value.comparacaoClausulaItems.forEach((clausula) => {
    compareDifferences(
      clausula,
      nomeDocReferencia[0].description,
      nomeDocAnterior[0].description,
      validadeInicialReferencia,
      validadeFinalReferencia,
      validadeInicialAnterior,
      validadeFinalAnterior
    );
  });

  $("#clausulasComparacaoCount").text(
    `Total de ${totalClausulasComparativo} cláusulas visíveis.`
  );
}

function removerAcentos(texto) {
  return texto
    .normalize("NFD") // Normaliza os caracteres acentuados em formas separadas
    .replace(/[\u0300-\u036f]/g, ""); // Remove outros caracteres não alfanuméricos
}

function convertDate(date) {
  if (!date) {
    return "";
  }
  return moment(date, "YYYY-MM-DD").format("DD/MM/YYYY");
}

function convertDateTime(date) {
  if (!date) {
    return "";
  }
  const newDate = new Date(date);
  return moment(newDate).format("DD/MM/YYYY - HH:MM");
}

async function generatePdf() {
  if (!clausulaClicada) {
    if(!(clausulasSelecionadas instanceof Array) || clausulasSelecionadas.length == 0) {
      NotificationService.error({
        title: "Não é possível gerar o PDF",
        message: "Selecione pelo menos uma cláusula!",
      });
      return;
    }
  
    const obterClausulasPorIdsResult = await clausulaService.obterPorId(clausulasSelecionadas);
    if (obterClausulasPorIdsResult.isFailure()) {
      NotificationService.error({title: "Erro ao tentar obter informações das cláusulas.", message: obterClausulasPorIdsResult.error});
      return;
    }
  
    dadosClausulasPorId = obterClausulasPorIdsResult.value;
  }

  if (dadosClausulasPorId instanceof Array) {
    dadosClausulasPorId.sort((a,b) => {
      const regiaoA = a?.sindLaboral[0].uf + " / " + a?.sindLaboral[0].municipio;
      const regiaoB = b?.sindLaboral[0].uf + " / " + b?.sindLaboral[0].municipio;

      if (regiaoA < regiaoB) return -1;
      if (regiaoA > regiaoB) return 1;

      return 0;
    });
  }

  const requestData = {
    clausulas: dadosClausulasPorId,
  };

  const result = await clausulaService.gerarPdfClausulas(requestData);

  PageWrapper.download(result.value.data.blob, "relatorio_clausulas.pdf", MediaType.pdf["Content-Type"])
}

async function generateComparativePdf() {
  const requestData = {
    dadosComparacao: dadosComparativoClausula,
  };

  const result = await clausulaService.gerarPdfComparativoClausulas(
    requestData
  );

  PageWrapper.download(result.value.data.blob, "comparativo_clausulas.pdf", MediaType.pdf["Content-Type"])
}

function formatDateToDateOnly(date) {
  const formattedDate = moment(date, "DD/MM/YYYY").format("YYYY/MM/DD");
  return formattedDate;
}

function compareDifferences(
  clausula,
  nomeDocReferencia,
  nomeDocAnterior,
  periodoIniReferencia,
  periodoFimReferencia,
  periodoIniAnterior,
  periodoFimAnterior
) {
  const textoNovo = clausula.diferenca.newText;
  const textoAnterior = clausula.diferenca.oldText;

  const showOnlyDiff = $("#only-diff").prop("checked");

  if (
    !textoNovo.hasDifferences &&
    !textoAnterior.hasDifferences &&
    showOnlyDiff
  ) {
    return;
  }

  const diffContainer = $("<div>").attr(
    "style",
    "display: flex; align-content: center; justify-content: center; flex-direction: column; border: 1px solid #4f8edc; margin-bottom: 1rem;"
  );
  const title = $("<h4>")
    .attr("style", "color: #FFFFFF")
    .html(
      `<strong>Cláusula:</strong> ${clausula.clausula} - <strong>Grupo:</strong> ${clausula.grupo}`
    );
  const titleDiv = $("<div>")
    .attr(
      "style",
      "background-color: #4f8edc; width: 100%; padding: 0.5rem 1rem; margin-bottom: 1rem;"
    )
    .append(title);
  diffContainer.append(titleDiv);

  let colunaTextoNovo = $("<div>").addClass("col-lg-6").append();
  const titleColunaNova = $("<h5>").html(
    `<strong>Nome documento:</strong> ${nomeDocReferencia}<br><strong>Período:</strong> ${periodoIniReferencia} - ${periodoFimReferencia}`
  );
  const titleColunaNovaDiv = $("<div>")
    .addClass("title-coluna-nova")
    .attr("style", "background-color: #f5f5f5; width: 100%;")
    .append(titleColunaNova);
  const textoNovoParagrafo = $("<div>").attr("style", "padding: 5px;");
  colunaTextoNovo.append([titleColunaNovaDiv, textoNovoParagrafo]);
  let colunaTextoAntigo = $("<div>").addClass("col-lg-6");
  const titleColunaAntigo = $("<h5>").html(
    `<strong>Nome documento:</strong> ${nomeDocAnterior}<br><strong>Período:</strong> ${periodoIniAnterior} - ${periodoFimAnterior}`
  );
  const titleColunaAntigoDiv = $("<div>")
    .addClass("title-coluna-antiga")
    .attr("style", "background-color: #f5f5f5; width: 100%;")
    .append(titleColunaAntigo);
  const textoAntigoParagrafo = $("<div>").attr("style", "padding: 5px;");
  colunaTextoAntigo.append([titleColunaAntigoDiv, textoAntigoParagrafo]);

  if (clausula.tituloClausulaReferencia) {
    textoNovoParagrafo.append($("<p>").text(clausula.tituloClausulaReferencia));
  }

  if (clausula.tituloClausulaAnterior) {
    textoAntigoParagrafo.append($("<p>").text(clausula.tituloClausulaAnterior));
  }

  if (!clausula.textoAnterior && clausula.textoReferencia) {
    totalClausulasComparativo++;

    textoNovoParagrafo.append(
      $("<p>")
        .attr("style", "text-align: justify; white-space: pre-line")
        .text(clausula.textoReferencia)
    );
    colunaTextoNovo.addClass("clausula_box_added");
    textoAntigoParagrafo
      .append(
        $("<p>").text(
          "Não existem cláusulas cadastradas na base de dados correspondentes ao ano selecionado!"
        )
      )
      .addClass("clausula_null");

    const row = $("<div>")
      .addClass("row")
      .attr("style", "padding: 0 1rem; display: flex; gap: 10px;")
      .append([colunaTextoNovo, colunaTextoAntigo]);
    diffContainer.append(row);

    $("#containerResultadoComparacao").append(diffContainer);

    $("#resultadoComparacao").attr("style", "display: block");

    dadosComparativoClausula.push({
      nomeDocumentoAntigo: nomeDocReferencia,
      nomeDocumentoReferencia: nomeDocAnterior,
      periodoAntigo: `${periodoIniAnterior} - ${periodoFimAnterior}`,
      periodoNovo: `${periodoIniReferencia} - ${periodoFimReferencia}`,
      nomeClausula: clausula.clausula,
      grupoClausula: clausula.grupo,
      htmlDiferencasAntigo: colunaTextoAntigo.prop("outerHTML"),
      htmlDiferencasNovo: colunaTextoNovo.prop("outerHTML"),
    });

    return;
  }

  if (clausula.textoAnterior && !clausula.textoReferencia) {
    totalClausulasComparativo++;

    textoNovoParagrafo
      .append(
        $("<p>").text(
          "Não existem cláusulas cadastradas na base de dados correspondentes ao ano selecionado!"
        )
      )
      .addClass("clausula_null");
    textoAntigoParagrafo.append(
      $("<p>")
        .attr("style", "text-align: justify; white-space: pre-line")
        .text(clausula.textoAnterior)
    );
    colunaTextoAntigo.addClass("clausula_box_added");

    const row = $("<div>")
      .addClass("row")
      .attr("style", "padding: 0 1rem; display: flex; gap: 10px;")
      .append([colunaTextoNovo, colunaTextoAntigo]);
    diffContainer.append(row);

    $("#containerResultadoComparacao").append(diffContainer);

    $("#resultadoComparacao").attr("style", "display: block");

    dadosComparativoClausula.push({
      nomeDocumentoAntigo: nomeDocReferencia,
      nomeDocumentoReferencia: nomeDocAnterior,
      periodoAntigo: `${periodoIniAnterior} - ${periodoFimAnterior}`,
      periodoNovo: `${periodoIniReferencia} - ${periodoFimReferencia}`,
      nomeClausula: clausula.clausula,
      grupoClausula: clausula.grupo,
      htmlDiferencasAntigo: colunaTextoAntigo.prop("outerHTML"),
      htmlDiferencasNovo: colunaTextoNovo.prop("outerHTML"),
    });

    return;
  }

  if (!textoNovo.hasDifferences && !textoAnterior.hasDifferences) {
    totalClausulasComparativo++;

    textoNovoParagrafo.append(
      $("<p>")
        .attr("style", "text-align: justify; white-space: pre-line")
        .text(clausula.textoReferencia)
    );
    colunaTextoNovo.addClass("clausula_box");
    textoAntigoParagrafo.append(
      $("<p>")
        .attr("style", "text-align: justify; white-space: pre-line")
        .text(clausula.textoAnterior)
    );
    colunaTextoAntigo.addClass("clausula_box");
  } else {
    totalClausulasComparativo++;

    textoNovo.lines.forEach((line) => {
      let textoNovoLines;
      if (line.subPieces.length === 0) {
        textoNovoLines = [line.text];
        if (line.type === 2) {
          textoNovoLines = [
            `<span style="background-color: #62f062;">${line.text}</span>`,
          ];
        }
      } else {
        textoNovoLines = line.subPieces.map((sub) => {
          let text = sub.text;
          if (sub.type === 2) {
            return `<span style="background-color: #62f062;">${sub.text}</span>`;
          }
          return text;
        });
      }
      const textoClausulaNova = $("<p>")
        .attr("style", "text-align: justify; white-space: pre-line")
        .html(textoNovoLines.join("").replace(/\n/g, "<br>"));
      textoNovoParagrafo.append(textoClausulaNova);
      colunaTextoNovo.addClass("clausula_box_added");
    });

    textoAnterior.lines.forEach((line) => {
      let textoAntigoLines;
      if (line.subPieces.length === 0) {
        textoAntigoLines = [line.text];
        if (line.type === 1) {
          textoAntigoLines = [
            `<span style="background-color: #fa9191;">${line.text}</span>`,
          ];
        }
      } else {
        textoAntigoLines = line.subPieces.map((sub) => {
          let text = sub.text;
          if (sub.type === 1) {
            return `<span style="background-color: #fa9191;">${sub.text}</span>`;
          }
          return text;
        });
      }
      const textoClausulaAntiga = $("<p>")
        .attr("style", "text-align: justify; white-space: pre-line")
        .html(textoAntigoLines.join("").replace(/\n/g, "<br>"));
      textoAntigoParagrafo.append(textoClausulaAntiga);
      colunaTextoAntigo.addClass("clausula_box_removed");
    });
  }

  const row = $("<div>")
    .addClass("row")
    .attr("style", "padding: 0 1rem; display: flex; gap: 10px;")
    .append([colunaTextoNovo, colunaTextoAntigo]);
  diffContainer.append(row);

  $("#containerResultadoComparacao").append(diffContainer);

  $("#resultadoComparacao").attr("style", "display: block");

  dadosComparativoClausula.push({
    nomeDocumentoAntigo: nomeDocReferencia,
    nomeDocumentoReferencia: nomeDocAnterior,
    periodoAntigo: `${periodoIniAnterior} - ${periodoFimAnterior}`,
    periodoNovo: `${periodoIniReferencia} - ${periodoFimReferencia}`,
    nomeClausula: clausula.clausula,
    grupoClausula: clausula.grupo,
    htmlDiferencasAntigo: colunaTextoAntigo.prop("outerHTML"),
    htmlDiferencasNovo: colunaTextoNovo.prop("outerHTML"),
  });
}

function renderizarValidade(date) {
  const today = moment(new Date()).startOf("day");
  const isExpired = moment(date, "DD/MM/YYYY").isBefore(today);
  if (isExpired) {
    return $("<span>").text(date).attr("style", "color: red;");
  }

  return $("<span>").text(date).attr("style", "color: green");
}

async function comparativoFormMediator(event, params = null) {
  const reactsDictionary = {
    docReferenciaSelectChange: () => {
      documentoReferenciaBtnToggleActive();
    },
    docReferenciaSelectOpen: () => {
      if (vigenciaAtualSelect?.getValue()) {
        $("#documentoReferenciaBtn").attr("disabled", false);
      }
    },
    vigenciaAtualSelectChange: () => {
      documentoReferenciaBtnToggleActive();
    },
    vigenciaAtualSelectOpen: () => {
      if (docReferenciaSelect?.getValue()) {
        $("#documentoReferenciaBtn").attr("disabled", false);
      }
    },
    docAnteriorSelectChange: () => {
      documentoAnteriorBtnToggleActive();
    },
    docAnteriorSelectOpen: () => {
      if (vigenciaAnteriorSelect?.getValue()) {
        $("#documentoAnteriorBtn").attr("disabled", false);
      }
    },
    vigenciaAnteriorSelectChange: () => {
      documentoAnteriorBtnToggleActive();
    },
    vigenciaAnteriorSelectOpen: () => {
      if (docAnteriorSelect?.getValue()) {
        $("#documentoAnteriorBtn").attr("disabled", false);
      }
    },
    sindPatronalComparativoSelectChange: () => {
      docReferenciaSelect.reload();
      docReferenciaSelect.enable();
      docAnteriorSelect.reload();
      docAnteriorSelect.disable();
      vigenciaAnteriorSelect.reload();
      vigenciaAnteriorSelect.disable();
      vigenciaAtualSelect.reload();
      vigenciaAtualSelect.disable();

      documentoAnteriorId = null;
      documentoReferenciaId = null;
      documentoReferenciaBtnToggleActive();
      documentoAnteriorBtnToggleActive();
    },
    sindLaboralComparativoSelectChange: () => {
      docAnteriorSelect.reload();
      docAnteriorSelect.disable();
      vigenciaAnteriorSelect.reload();
      vigenciaAnteriorSelect.disable();
      vigenciaAtualSelect.reload();
      vigenciaAtualSelect.disable();
      sindPatronalComparativoSelect.reload();
      docReferenciaSelect.disable();

      documentoAnteriorId = null;
      documentoReferenciaId = null;
      documentoReferenciaBtnToggleActive();
      documentoAnteriorBtnToggleActive();
    }
  }

  const reactToNotify = reactsDictionary[event];

    if (!reactToNotify) return;

    if (reactToNotify && params) {
      await reactToNotify(params);
    }else {
      await reactToNotify();
    }
}

function documentoReferenciaBtnToggleActive() {
  if (docReferenciaSelect?.getValue() && vigenciaAtualSelect?.getValue()) {
    $("#documentoReferenciaBtn").attr("disabled", false);
  }
  else {
    $("#documentoReferenciaBtn").attr("disabled", true);
  }
}

function documentoAnteriorBtnToggleActive() {
  if (docAnteriorSelect?.getValue() && vigenciaAnteriorSelect?.getValue()) {
    $("#documentoAnteriorBtn").attr("disabled", false);
  }
  else {
    $("#documentoAnteriorBtn").attr("disabled", true);
  }
}

async function carregarInformacoesModal() {
  await obterClausulasPorId()
  montarTabela()

  document.getElementById('scroll-top').click();
}

async function obterClausulasPorId() {
  const result = await clausulaGeralService.obterInformacoesAdicionaisPorClausulaId(clausulaClicada)

  if (result.isFailure()) {
    NotificationService.error({ title: 'Erro ao obter clausulas do documento', message: result.error })
    return Result.failure("Erro ao obter clausulas do documento");
  }

  if (!result.value) {
    NotificationService.error({title: "Nenhuma informação adicional encontrada."});
    return Result.failure("Nenhum resultado encontrado"); 
  }

  informacoesAdicionais = [result.value]
  return Result.success();
}

async function montarTabela() {
  const informacoesAdicionaisElements = await new Promise(resolve => {
    const elementos = criarElementos()

    return resolve(elementos)
  })

  $('#informacoes_adicionais').html(informacoesAdicionaisElements)
  await inicializarElementos()

  $('#detalhes-clausula-modal-header').trigger('focus')

  function criarElementos() {
    const container = div({})

    if (!informacoesAdicionais || informacoesAdicionais.length < 0 || !informacoesAdicionais[0]) return

    informacoesAdicionais = informacoesAdicionais.map((informacaoAdicional) => {
      if (!informacaoAdicional) return;

      const { nomeEstruraClausula, nomeGrupoClausula, grupos: linhas, estruturaId, numero } = informacaoAdicional[0]
      let cabecalhoCriado = false

      const title = h2({
        className: 'form-control-title',
        content: `${numero} - ${nomeEstruraClausula} | ${nomeGrupoClausula}`,
        style: 'margin-top: 40px;'
      })

      const tabelaContainer = div({
        style: 'width: 100%; overflow-x: scroll;'
      })

      const tabela = table({
        id: '12',
        className: 'table editable-table'
      })

      const tableHead = thead({})
      const tableBody = tbody({})
      const trhs = tr({})

      informacaoAdicional.grupos = linhas.map((item) => {
        const trbs = tr({})

        item.informacoesAdicionais = item.informacoesAdicionais.map((infoItem) => {
          const { descricao, codigo, tipo, dado } = infoItem

          if (codigo == 310) return

          if (!cabecalhoCriado) {
            trhs.append(th({
              content: descricao,
              className: 'table-head',
              style: 'min-width: 150px;'
            }))
          }

          const jOpcaoAdicinal = new JOpcaoAdicional({
            codigo,
            tipo,
            dado: dado//procurarDados({ id, tipo, dado })
          })

          trbs.append(td({
            content: jOpcaoAdicinal.content,
            style: 'min-width: 150px;'
          }))

          infoItem.element = jOpcaoAdicinal

          return infoItem
        })

        tableBody.append(trbs)

        cabecalhoCriado = true

        return item
      })

      tableHead.append(trhs)
      tabela.append(tableHead)
      tabela.append(tableBody)

      tabelaContainer.append(tabela)
      container.append(title)
      container.append(tabelaContainer)

      let primeiraObservacaoData = ''
      let segundaObservacaoData = ''

      informacaoAdicional.observacoesAdicionais = [
        {
          clausulaId: estruturaId,
          dado: primeiraObservacaoData,
          tipo: TipoObservacaoAdicional.ComunicadoInterno,
          element: {
          }
        },
        {
          clausulaId: estruturaId,
          dado: segundaObservacaoData,
          tipo: TipoObservacaoAdicional.RegrasParaEmpresa,
          element: {
          }
        }
      ]

      cabecalhoCriado = true

      return informacaoAdicional
    })

    informacoesAdicionais = informacoesAdicionais.sort(i => i.numero)

    return container
  }

  async function inicializarElementos() {
    if (!informacoesAdicionais || informacoesAdicionais.length < 0  || !informacoesAdicionais[0]) return

    new Promise((resolve) => {
      informacoesAdicionais.map((linha) => {
        linha.grupos.map(({ informacoesAdicionais }) => {
          informacoesAdicionais.map(({ element }) => {
            element.init()
          })
          informacoesAdicionais[0].element.focus()
        })
      })

      resolve()
    })
  }
}
