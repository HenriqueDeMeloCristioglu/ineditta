import JQuery from "jquery";
import $ from "jquery";
import "../../js/utils/masks/jquery-mask-extensions.js";

import "bootstrap";
import "../../js/utils/util.js";

import "datatables.net-bs5/css/dataTables.bootstrap5.css";
import "datatables.net-responsive-bs5/css/responsive.bootstrap5.css";
import "bootstrap/dist/css/bootstrap.min.css";

import { Chart, LinearScale, registerables } from "chart.js";
import Result from "../../js/core/result.js";

// Core
import { AuthService, ApiService, UserInfoService } from "../../js/core/index.js"
import { ApiLegadoService } from "../../js/core/api-legado"

// Services
import {
  ClausulaGeralService,
  ComentarioService,
  UsuarioAdmService,
  GrupoEconomicoService,
  ClienteUnidadeService,
  CnaeService,
  LocalizacaoService,
  MatrizService,
  SindicatoService,
  SindicatoLaboralService,
  SindicatoPatronalService,
  TipoEtiquetaService,
  EtiquetaService,
  ClienteUnidadeSindicatoPatronalService
} from "../../js/services"

import DataTableWrapper from "../../js/utils/datatables/datatable-wrapper.js";
import {
  closeModal,
  renderizarModal,
} from "../../js/utils/modals/modal-wrapper.js";
import SelectWrapper from "../../js/utils/selects/select-wrapper.js";
import NotificationService from "../../js/utils/notifications/notification.service.js";

import DatepickerWrapper from "../../js/utils/datepicker/datepicker-wrapper.js";
import Uri from "../../js/utils/urls/uri.js";
import { UsuarioNivel } from "../../js/application/usuarios/constants/usuario-nivel.js";


import '../../js/main.js'
import { input } from '../../js/utils/components/elements';
import { Menu } from '../../components/menu/menu.js'
import { BooleanType, getDescriptionTipoUsuarioDestino, getDescriptionTipoUsuarioNotificacao, obterBooleanSelect, obterLocalidades, obterTipoComentarioSelect, obterTipoNotificacaoSelect, obterTipoUsuarioDestinoSelect, usuarioDestinoSelect } from "../../js/utils/components/selects/index.js";
import { TipoComentario } from "../../js/application/comentarios/constants/tipo-comentarios.js";
import { TipoUsuarioDestino } from "../../js/application/comentarios/constants/tipo-usuario-destino.js";
import { TipoNotificacao } from "../../js/application/comentarios/constants/tipo-notificacao.js";
import PageWrapper from "../../js/utils/pages/page-wrapper.js";
import moment from "moment";

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const grupoEconomicoService = new GrupoEconomicoService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const matrizService = new MatrizService(apiService);
const localizacaoService = new LocalizacaoService(apiService);
const cnaeService = new CnaeService(apiService);
const sindLaboralService = new SindicatoLaboralService(
  apiService,
  apiLegadoService
);
const sindPatronalService = new SindicatoPatronalService(
  apiService,
  apiLegadoService
);
const usuarioAdmService = new UsuarioAdmService(apiService);
const sindicatoService = new SindicatoService(apiService, apiLegadoService);

const sindicatoPatronalService = new SindicatoPatronalService(
  apiService,
  apiLegadoService
);
const clausulaGeralService = new ClausulaGeralService(
  apiService,
  apiLegadoService
);
const comentarioService = new ComentarioService(apiService, apiLegadoService);
const tipoEtiquetaService = new TipoEtiquetaService(apiService)
const etiquetaService = new EtiquetaService(apiService)
const clienteUnidadeSindicatoPatronalService = new ClienteUnidadeSindicatoPatronalService(apiService);

let gruposEconomicosSelect = null;
let empresasSelect = null;
let estabelecimentosSelect = null;
let localizacoesSelect = null;
let cnaeSelect = null;
let sindLaboralSelect = null;
let sindPatronalSelect = null;

let estabelecimentoUpdateSelect = null;

let organizacaoSindicalLaboralTb = null;
let organizacaoSindicalPatronalTb = null;
let dirigentesSindicaisLaboraisTb = null;
let dirigentesSindicaisPatronaisTb = null;
let sindEstadosTable = null;
let selectedUF = null;
let diretoriaInfoSindTb = null;
let clienteUnidadeTb = null;
let clienteUnidadeSelecionadosTb = null;

let chartQtdCentraisSindicais = null;
let chartNegAbertoEstado = null;

let tipoComentarioSelect = null;
let tipoUsuarioDestinoSelect = null;
let tipoNotificacaoSelect = null;
let etiquetaSelect = null;
let assuntoSelect = null;
let destinoSelect = null;
let tipoEtiquetaSelect = null;
let visivelSelect = null;
let dataValidade = null

let tipoDoSindicatoSelecionado = null;
let sindicatoSelecionado = null;
let sindicatoSelecionadoId = null;

let listaSindeIds = null;
let listaSindpIds = null;

let comentar = null;
let usuario = null;

let isIneditta = null;
let isGrupoEconomico = null;
let isEstabelecimento = null;
let isEmpresa = null;

let filterParams = null;
let permissoesUsuario = [];

let filiaisIdsSelecionadas = [];
let filiaisIdsParaRemoverSelecionadas = [];
let clientesUnidadesCarregadas = null;

const MODULO_SINDICATO_ID = 15;
const podeComentarSindicatoModulo = () => permissoesUsuario?.some(p => p.modulo_id == MODULO_SINDICATO_ID && p.comentar == 1);
const podeConsultarModulo = () => permissoesUsuario?.some(p => p.modulo_id == MODULO_SINDICATO_ID && p.consultar == 1);

JQuery(async () => {
  new Menu()

  await AuthService.initialize();

  Chart.register(LinearScale, ...registerables);

  const dadosPessoais = await usuarioAdmService.obterDadosPessoais();
  const permissoesUsuarioRequest = await usuarioAdmService.obterPermissoes();

  if (dadosPessoais.isFailure()) {
    return;
  }

  if (permissoesUsuarioRequest.isFailure()) {
    NotificationService.error({ title: "Não será possível exibir a página. Erro nas permissões do usuário.", message: permissoesUsuario.error })
    return;
  }

  permissoesUsuario = permissoesUsuarioRequest.value ?? [];

  usuario = dadosPessoais.value;

  isIneditta = usuario.nivel == UsuarioNivel.Ineditta;
  isGrupoEconomico = usuario.nivel == UsuarioNivel.GrupoEconomico;
  isEstabelecimento = usuario.nivel == UsuarioNivel.Estabelecimento;
  isEmpresa = usuario.nivel == UsuarioNivel.Empresa;

  await configurarSelects();

  await configurarOptions();

  $(".horizontal-nav").removeClass("hide");

  configurarFormulario();

  configurarModal();

  configurarFormularioModal();

  $("#dropdownMenu2").dropdown();

  $("#goToDirLabLink").on("click", () => {
    rolarAte("dirlab");
  });

  initOnClickMap();

  await obterPermissoes();

  await carregarSelectUpdateEstabelecimento();

  $(".select2").select2();

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

  $("#btn_atualizar_empresas_selecionadas").on("click", async () => await carregarClienteUnidadeSelecionadosDatatable());

  $("#btn_remover_empresas_selecionadas").on("click", async () => {
    await removerEmpresasSeleciondas();
  })

  if (podeComentarSindicatoModulo()) {
    $("#btn_salvar_estabelecimentos_sindicato_patronal").on("click", async () => {
      await atualizarEstabelecimentosAssociadoSindicato()
    });
  }
  else {
    $("#btn_salvar_estabelecimentos_sindicato_patronal").hide();
  }

  await consultarUrl();

  const queryParams = Uri.getQueryParams(window.location.href);

  if (!queryParams) {
    return;
  }

  const grupos = queryParams.grupos;
  const matrizes = queryParams.matrizes;
  const unidades = queryParams.unidades;
  const cnaes = queryParams.cnaes;

  if (!grupos && !matrizes && !unidades && !cnaes) {
    return;
  }

  gruposEconomicosSelect.disable();
  empresasSelect.disable();
  estabelecimentosSelect.disable();
  cnaeSelect.disable();

  if (grupos) {
    await preencherGrupo(grupos);
  }

  //!Ineditta && Matriz ou Estabelecimento
  if (matrizes) {
    await preencherMatriz(matrizes);
  }

  if (unidades) {
    await preencherUnidades(unidades);
  }

  if (cnaes) {
    await preencherCategoria(cnaes);
  }

  gruposEconomicosSelect.enable();
  empresasSelect.enable();
  estabelecimentosSelect.enable();
  cnaeSelect.enable();

  await filter();
  $("#collapseDiretoria").on("click", () => {
    $("#collapseDiretoriaBody").collapse("toggle");
  });
});

async function preencherGrupo(grupos) {
  const gruposSelecionadosIds = grupos.split(",");
  const opcoes = await gruposEconomicosSelect.loadOptions();
  if (opcoes && !!opcoes?.length > 0) {
    const grupoOption = opcoes.filter(x => x.id == gruposSelecionadosIds[0])
    gruposEconomicosSelect.setCurrentValue(grupoOption[0]);
  }
  $("#grupo").trigger("select2:select")
  empresasSelect.disable();
}

async function preencherMatriz(matrizes) {
  const matrizesSelecionadosIds = matrizes.split(",");
  const opcoes = await empresasSelect.loadOptions();
  //empresasSelect.setCurrentId(matrizesSelecionadosIds);
  if (opcoes && !!opcoes?.length > 0) {
    const matrizesOptions = opcoes.filter(x => matrizesSelecionadosIds.includes(x.id.toString()));
    empresasSelect.setCurrentValue(matrizesOptions);
  }
  $("#matriz").trigger("change.select2");
}

async function preencherUnidades(unidades) {
  const unidadesSelecionadosIds = unidades.split(",");
  const opcoes = await estabelecimentosSelect.loadOptions();
  if (opcoes && !!opcoes?.length > 0) {
    const estabelecimentosOptions = opcoes.filter(x => unidadesSelecionadosIds.includes(x.id.toString()));
    estabelecimentosSelect.setCurrentValue(estabelecimentosOptions);
  }
  $("#unidade").trigger("select2:select")
}

async function preencherCategoria(cnaes) {
  const cnaesSelecionadosIds = cnaes.split(",");
  const opcoes = await cnaeSelect.loadOptions();

  if (opcoes && !!opcoes.length > 0) {
    cnaeSelect.setCurrentId(cnaesSelecionadosIds);
  }
  $("#categoria").trigger("change.select2")
}

async function obterPermissoes() {
  const modulos = (await usuarioAdmService.obterPermissoes()).value;

  comentar =
    modulos.length > 0
      ? modulos.find((modulo) => modulo.modulos === "Sindicatos").comentar ===
      "1"
      : false;
}

function configurarFormulario() {
  $("#exportarLaboral").hide();
  $("#exportarPatronal").hide();

  $("#limparFiltroBtn").on("click", () => {
    limparFiltro();
  });

  $("#filtrarBtn").on("click", async () => {
    await filter();
  });

  $("#gerarRelatorioBtn").on("click", async () => {
    await gerarExcelGeral();
  })

  $("#exportarLaboral").on("click", async () => {
    await gerarExcelLaboral();
  })

  $("#exportarPatronal").on("click", async () => {
    await gerarExcelPatronal();
  })
}

async function configurarOptions() {
  if (isIneditta) {
    gruposEconomicosSelect.enable();
  } else {
    gruposEconomicosSelect.disable();
    await gruposEconomicosSelect.loadOptions();
  }

  empresasSelect.reload();
  const optionsEmpresas = await empresasSelect.loadOptions();

  if (isIneditta || isGrupoEconomico) {
    empresasSelect.enable();
  } else {
    if (!(optionsEmpresas instanceof Array && optionsEmpresas.length > 1) || isEstabelecimento) {
      empresasSelect.disable();
    }
    else {
      empresasSelect.config.markOptionAsSelectable = () => false;
      empresasSelect.clear();
      empresasSelect.enable();
    }
  }

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
}


async function configurarSelects() {

  const markOptionAsSelectable =
    usuario.nivel == "Cliente" ? () => true : () => false;

  gruposEconomicosSelect = new SelectWrapper("#grupo", {
    options: { placeholder: "Selecione", allowEmpty: true },
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
      if (estabelecimentosSelect) {
        await estabelecimentosSelect.reload();
      }
      if (localizacoesSelect) {
        await localizacoesSelect.reload();
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
      if (localizacoesSelect) {
        await localizacoesSelect.reload();
      }
    },
    onOpened: async (grupoEconomicoId) =>
      await matrizService.obterSelectPorUsuario(grupoEconomicoId),
    markOptionAsSelectable:
      isIneditta || isGrupoEconomico ? () => false : () => true,
  });

  const optionsEmpresas = await empresasSelect.loadOptions();

  if (isIneditta || isGrupoEconomico) {
    empresasSelect.enable();
  } else {
    if (!(optionsEmpresas instanceof Array && optionsEmpresas.length > 1) || isEstabelecimento) {
      empresasSelect.disable();
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
      if (localizacoesSelect) {
        await localizacoesSelect.reload();
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
    onOpened: async () => await obterLocalidades({
      grupoEconomicoId: $("#grupo").val(),
      matrizesIds: $("#matriz").val(),
      clientesUnidadesIds: $("#unidade").val(),
      tipoLocalidade: $("#tipoLocalidade").val()
    }),
    markOptionAsSelectable
  })

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
  })

  sindLaboralSelect = new SelectWrapper("#sind_laboral", {
    options: { placeholder: "Selecione", multiple: true },
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
    markOptionAsSelectable: markOptionAsSelectable,
    sortable: true,
  })

  sindPatronalSelect = new SelectWrapper("#sind_patronal", {
    options: { placeholder: "Selecione", multiple: true },
    onOpened: async () => {
      const cnaes = $("#categoria").val();
      const idGrupo = $("#grupo").val();
      const matrizes = $("#matriz").val();
      const unidades = $("#unidade").val();
      const localizacoes = $("#localidade").val();
      const tipoLocalidade = $("#tipoLocalidade").val();
      const params = {
        clientesUnidadesIds: unidades,
        matrizesIds: matrizes,
        grupoEconomicoId: idGrupo,
        cnaesIds: cnaes,
      };

      if (tipoLocalidade == "uf") params.ufs = localizacoes;
      if (tipoLocalidade == "regiao") params.regioes = localizacoes;
      if (tipoLocalidade == "municipio") params.localizacoesIds = localizacoes;

      return await sindPatronalService.obterSelectPorUsuario(params);
    },
    markOptionAsSelectable: markOptionAsSelectable,
    sortable: true,
  })

  $("#tipoLocalidade").on("select2:select", () => localizacoesSelect.clear())
}

async function carregarSelectUpdateEstabelecimento() {
  if (estabelecimentoUpdateSelect) {
    await estabelecimentoUpdateSelect.loadOptions();
    return;
  }
  estabelecimentoUpdateSelect = new SelectWrapper(
    "#estabelecimento-update-input",
    {
      options: {
        placeholder: "Selecione"
      },
      onOpened: async (matrizId) =>
        await clienteUnidadeService.obterSelectPorUsuario(matrizId),
      markOptionAsSelectable: isEstabelecimento ? () => true : () => false,
    }
  );
  if (isEstabelecimento) {
    await estabelecimentoUpdateSelect.loadOptions();
    estabelecimentoUpdateSelect.disable();
  } else {
    estabelecimentoUpdateSelect.enable();
  }
}

function configurarModal() {
  $("#novoAcompanhamentoBtn").on("click", () => {
    const id = $("#id-input").val();
    if (!id) {
      $("#edicaoFields").hide();
      $("#scriptPanel").hide();
    }
  });

  const pageCtn = document.getElementById("pageCtn");

  const noticacaoModalHidden = document.getElementById("noticacaoModalHidden");
  const noticacaoModalHiddenContent = document.getElementById(
    "noticacaoModalHiddenContent"
  );
  const noticacaoModalButtons = [
    {
      id: "notificacaoCadastrarBtn",
      onClick: async (id, modalContainer) => {
        const result = await upsert();
        if (result.isSuccess()) {
          closeModal(modalContainer);
        }
      },
      data: null,
    },
  ];

  // filial
  const filialModalHidden = document.getElementById('filialModalHidden');
  const filialModalContent = document.getElementById('filialModalContent');

  //Sindicado por estado - comentários
  const sindEstadosModalHidden = document.getElementById(
    "sindEstadosModalHidden"
  );
  const sindEstadosModalContent = document.getElementById(
    "sindEstadosModalContent"
  );

  const modalInfo = document.getElementById("infoSindModalHidden");
  const contentInfo = document.getElementById("infoSindModalHiddenContent");

  const buttonsInfoConfig = [];

  const modalEstabelecimento = document.getElementById(
    "estabelecimentoModalHidden"
  );
  const contentEstabelecimento = document.getElementById(
    "estabelecimentoModalHiddenContent"
  );

  const buttonsEstabelecimentoConfig = [
    {
      id: "updateEstabelecimentoBtn",
      onClick: async (id, modalContainer) => {
        const tipoSind = $("#tipo-sind-input").val();
        const situacao = $("#afastado").val();
        if (!situacao) {
          NotificationService.error({
            title: "Não é possível processar a alteração.",
            message: "Preencha os campos!"
          })

          return;
        }
        if (tipoSind == "laboral") {
          const result = await updateEsta();
          if (result.isSuccess()) {
            closeModal(modalContainer);
            await carregarDirigentesLaboralTb();
          }
        } else if (tipoSind == "patronal") {
          const result = await updateEstaP();
          if (result.isSuccess()) {
            closeModal(modalContainer);
            await carregarDirigentesPatronalTb();
          }
        }
      },
      data: null
    },
    {
      id: "limparEstabelecimentoBtn",
      onClick: async (id, modalContainer) => {
        const tipoSind = $("#tipo-sind-input").val();
        if (tipoSind == "laboral") {
          const result = await limparEsta();
          if (result.isSuccess()) {
            closeModal(modalContainer);
            await carregarDirigentesLaboralTb();
          }
        } else if (tipoSind == "patronal") {
          const result = await limparEstaP();
          if (result.isSuccess()) {
            closeModal(modalContainer);
            await carregarDirigentesPatronalTb();
          }
        }
      },
      data: null
    },
  ];

  const modalsConfig = [
    {
      id: "infoSindModal",
      modal_hidden: modalInfo,
      content: contentInfo,
      btnsConfigs: buttonsInfoConfig,
      onOpen: async () => {
        const id = $("#sind-id-input").val();
        const tipoSind = $("#tipo-sind-input").val();
        if (id) {
          await obterInfoSindicatoPorId(id, tipoSind);
          await carregarDataTableInfoDiretoriaTb(id, tipoSind);
        }
      },
      onClose: () => {
        limparModalInfo();
      },
    },
    {
      id: "estabelecimentoModal",
      modal_hidden: modalEstabelecimento,
      content: contentEstabelecimento,
      btnsConfigs: buttonsEstabelecimentoConfig,
      onOpen: async () => {
        const unidadeId = $("#unidade-id-input").val();
        const nomeUnidade = $("#nomeunidade-input").val();
        const situacao = $("#afastado-input").val();
        if (unidadeId) {
          const estaOption = {
            description: nomeUnidade,
            id: unidadeId,
          };
          estabelecimentoUpdateSelect.setCurrentValue(estaOption);
          await estabelecimentoUpdateSelect.loadOptions();
        } else {
          await estabelecimentoUpdateSelect.reload();
        }
        if (situacao) {
          $("#afastado").val(situacao).trigger("change");
        }
      },
      onClose: () => {
        limparModalInfo();
      },
    },
    {
      id: "sindEstadosModal",
      modal_hidden: sindEstadosModalHidden,
      content: sindEstadosModalContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarSindCommentariosPorUF();
      },
      onClose: () => null,
    },
    {
      id: "notificacaoModal",
      modal_hidden: noticacaoModalHidden,
      content: noticacaoModalHiddenContent,
      btnsConfigs: noticacaoModalButtons,
      onOpen: async () => {
        const id = $("#id_note").val();
        if (id) {
          await obterPorId(id);
        }
      },
      onClose: () => limpar(),
    },
    {
      id: 'filialModal',
      modal_hidden: filialModalHidden,
      content: filialModalContent,
      btnsConfigs: [],
      onOpen: async () => {
        const result = await clienteUnidadeSindicatoPatronalService.obterPorSindicatoId(sindicatoSelecionadoId);
        filiaisIdsSelecionadas = result.value?.map(c => c.clienteUnidadeId);
        await carregarClienteUnidadeSelecionadosDatatable()
        await carregarClienteUnidadeDatatable()
      },
      onClose: () => {
        filiaisIdsParaRemoverSelecionadas = [];
        filiaisIdsSelecionadas = [];
      }
    },
  ];

  renderizarModal(pageCtn, modalsConfig);
}

async function carregarClienteUnidadeSelecionadosDatatable() {
  if (clienteUnidadeSelecionadosTb) return await clienteUnidadeSelecionadosTb.reload();

  if (!podeComentarSindicatoModulo()) {
    $("#selecionar_todas_empresas_selecionadas-container").hide();
    $("#empresas-selecionadas-actions-buttons-container").hide();
  }
  else{
    $("#selecionar_todas_empresas_selecionadas").on("click", (event) => {
      if (event.currentTarget.checked) {
        $('.filial-selecionada-modal-datatable').prop('checked', true);
        $('.filial-selecionada-modal-datatable').trigger('change');
      } else {
        $('.filial-selecionada-modal-datatable').prop('checked', false);
        $('.filial-selecionada-modal-datatable').trigger('change');
      }
    });
  }

  clienteUnidadeSelecionadosTb = new DataTableWrapper('#clienteUnidadeSelecionadosTb', {
    columns: [
      { data: "id", visible: podeComentarSindicatoModulo() },
      { data: "nomeGrupoEconomico", title: "Grupo Econômico" },
      { data: "nomeMatriz", title: "Matriz" },
      { data: "nome", title: "Filial" },
      { data: "cnpj", title: "Filial CNPJ"},
      { data: "codigoUnidadeCliente", title: "Código estabelecimento"}
    ],
    ajax: async (requestData) => {
      $('#selecionar_todas_empresas_selecionadas').val(false).prop('checked', false);
      requestData.GrupoUsuario = true;
      requestData.ApenasAssociados = true;
      requestData.SindicatoPatronalId = sindicatoSelecionadoId;
      requestData.BuscarSomentePorIds = true; 
      requestData.ClientesUnidadesIds = filiaisIdsSelecionadas;
      requestData.Columns = "id,nome,cnpj,nomeMatriz,nomeGrupoEconomico";
      let clientesUnidadesSelecionadasCarregadas = await clienteUnidadeService.obterDatatable(requestData)
      filiaisIdsSelecionadas = [...filiaisIdsSelecionadas, ]
      return clientesUnidadesSelecionadasCarregadas;
    },
    rowCallback: function (row, data) {
      if (!podeComentarSindicatoModulo()) return;

      const checkbox = input({ type: 'checkbox', className: 'form-check-input c chkitem filial-selecionada-modal-datatable' })
                        .attr('data-id', data.id);

      if (filiaisIdsParaRemoverSelecionadas.find(filial => filial == data?.id)) {
        checkbox.prop('checked', true)
      }

      checkbox.on('change', (el) => {
        const checked = el.target.checked;
        const id = data?.id;
      
        if (checked) {
          filiaisIdsParaRemoverSelecionadas.push(id)
        } else {
          filiaisIdsParaRemoverSelecionadas = filiaisIdsParaRemoverSelecionadas.filter(item => item != id)
        }
      });

      $("td:eq(0)", row).html(checkbox);
    },
  });

  await clienteUnidadeSelecionadosTb.initialize();
}

async function carregarClienteUnidadeDatatable() {
  if (!podeComentarSindicatoModulo()) {
    $("#selecione-empresa-associada-sindicato-patronal").hide();
    return;
  }

  if (clienteUnidadeTb) return await clienteUnidadeTb.reload();

  $("#selecionar_todas_empresas").on("click", (event) => {
		if (event.currentTarget.checked) {
			$('.filial-modal-datatable').prop('checked', true);
			$('.filial-modal-datatable').trigger('change');
		} else {
			$('.filial-modal-datatable').prop('checked', false);
			$('.filial-modal-datatable').trigger('change');
		}
	});

  clienteUnidadeTb = new DataTableWrapper('#clienteUnidadeTb', {
    columns: [
      { data: "id" },
      { data: "nomeGrupoEconomico", title: "Grupo Econômico" },
      { data: "nomeMatriz", title: "Matriz" },
      { data: "nome", title: "Filial" },
      { data: "cnpj", title: "Filial CNPJ"},
      { data: "codigoUnidadeCliente", title: "Código estabelecimento"}
    ],
    ajax: async (requestData) => {
      $('#selecionar_todas_empresas').val(false).prop('checked', false);
      requestData.GrupoUsuario = true;
      requestData.SindicatoPatronalId = sindicatoSelecionadoId;
      requestData.Columns = "id,nome,cnpj,nomeMatriz,nomeGrupoEconomico";
      clientesUnidadesCarregadas = await clienteUnidadeService.obterDatatable(requestData)
      return clientesUnidadesCarregadas;
    },
    rowCallback: function (row, data) {
      const checkbox = input({ type: 'checkbox', className: 'form-check-input c chkitem filial-modal-datatable' }).attr('data-id', data.id)

      if (filiaisIdsSelecionadas.find(filial => filial == data?.id)) {
        checkbox.prop('checked', true)
      }

      checkbox.on('change', (el) => {
        const checked = el.target.checked;
        const id = data?.id;
      
        if (checked) {
          const unidade = clientesUnidadesCarregadas?.value?.items?.filter(unidade => unidade.id == id)[0]
          if (unidade) filiaisIdsSelecionadas.push(id);
        } else {
          filiaisIdsSelecionadas = filiaisIdsSelecionadas.filter(item => item != id)
        }
      });

      $("td:eq(0)", row).html(checkbox);
    },
  });

  await clienteUnidadeTb.initialize();
}

async function removerEmpresasSeleciondas() {
  filiaisIdsSelecionadas = filiaisIdsSelecionadas.filter(filialId => {
    if(!filiaisIdsParaRemoverSelecionadas.includes(filialId)) return true
    $('.filial-modal-datatable[data-id="' + filialId + '"]').prop("checked", false);
  });
  await carregarClienteUnidadeSelecionadosDatatable();
  filiaisIdsParaRemoverSelecionadas = [];
}

//CONFIGURAR FORMULARIO DO MODAL
function configurarFormularioModal() {
  tipoComentarioSelect = new SelectWrapper("#tipo-com", { onOpened: async () => await obterTipoComentario() })
  assuntoSelect = new SelectWrapper("#assunto", { onOpened: async (tipoComentarioId) => await obterAssunto(tipoComentarioId), parentId: "#tipo-com", sortable: true })
  tipoUsuarioDestinoSelect = new SelectWrapper("#tipo_usuario_destino", { onOpened: async () => await obterTipoUsuarioDestinoSelect(), dropdownParent: $("#notificacaoModal") })
  destinoSelect = new SelectWrapper('#destino', { onOpened: async (tipoUsuarioId) => {
    configurarTituloTipoUsuario(tipoUsuarioId)
    return await usuarioDestinoSelect({ tipoUsuarioId, isIneditta: usuario.nivel == UsuarioNivel.Ineditta, grupoEconomicoId: usuario.grupoEconomicoId })
  }, parentId: '#tipo_usuario_destino', sortable: true })
  tipoNotificacaoSelect = new SelectWrapper("#tipo-note", { onOpened: async () => await obterTipoNotificacaoSelect() })
  dataValidade = new DatepickerWrapper("#validade")
  tipoEtiquetaSelect = new SelectWrapper('#tipo-etiqueta', {
    onOpened: async () => (await tipoEtiquetaService.obterSelect({ tipoEtiquetaNome: 'Sindicatos' })).value,
    options: {
      allowEmpty: true
    }
  })
  etiquetaSelect = new SelectWrapper('#etiqueta', { onOpened: async (tipoEtiquetaId) => (await etiquetaService.obterSelect({ tipoEtiquetaId })).value, parentId: '#tipo-etiqueta', sortable: true })
  visivelSelect = new SelectWrapper('#visivel', { onOpened: async () => await obterBooleanSelect() })

  carregarInformacoesUsuario()
}

function setarInfoFormularioSelect() {
  if (tipoDoSindicatoSelecionado == "laboral") {
    tipoComentarioSelect.setCurrentValue({
      id: TipoComentario.SindicatoLaboral,
      description: "Sindicato Laboral",
    });
  }
  if (tipoDoSindicatoSelecionado == "patronal") {
    tipoComentarioSelect.setCurrentValue({
      id: TipoComentario.SindicatoPatronal,
      description: "Sindicato Patronal",
    });
  }

  if (sindicatoSelecionado) {
    assuntoSelect.setCurrentValue({
      id: sindicatoSelecionadoId,
      description: sindicatoSelecionado,
    });
  }
}

async function obterAssunto(tipoComentarioId) {
  configurarTituloAssunto(tipoComentarioId);

  switch (tipoComentarioId) {
    case TipoComentario.Clausula:
      return (await clausulaGeralService.obterSelect()).value;
    case TipoComentario.SindicatoPatronal:
      return usuario.nivel === UsuarioNivel.Ineditta
        ? (await sindicatoPatronalService.obterSelect()).value
        : (await sindicatoPatronalService.obterSelect({ porUsuario: true }))
          .value;
    case TipoComentario.SindicatoLaboral:
      return usuario.nivel === UsuarioNivel.Ineditta
        ? (await sindLaboralService.obterSelect()).value
        : (await sindLaboralService.obterSelect({ porUsuario: true })).value;
    case TipoComentario.Filial:
      return (await clienteUnidadeService.obterSelect()).value;
    default:
      return await Promise.resolve([]);
  }
}

function configurarTituloAssunto(tipoComentarioId) {
  switch (tipoComentarioId) {
    case TipoComentario.Clausula:
      $("#assuntoTitulo").html("Assunto");
      return;
    case TipoComentario.SindicatoPatronal:
      $("#assuntoTitulo").html("Sindicato");
      return;
    case TipoComentario.SindicatoLaboral:
      $("#assuntoTitulo").html("Sindicato");
      return;
    case TipoComentario.Filial:
      $("#assuntoTitulo").html("Filial");
      return;
    default:
      $("#assuntoTitulo").html("Assunto");
      return;
  }
}

function configurarTituloTipoUsuario(tipoUsuarioId) {
  switch (tipoUsuarioId) {
    case TipoUsuarioDestino.Grupo:
      $("#campo_tipo").html("Grupo Econômico");
      return;
    case TipoUsuarioDestino.Matriz:
      $("#campo_tipo").html("Empresa");
      return;
    case TipoUsuarioDestino.Unidade:
      $("#campo_tipo").html("Estabelecimento");
      return;
    default:
      $("#campo_tipo").html("--");
      return;
  }
}

async function obterTipoComentario() {
  let data = obterTipoComentarioSelect()

  if (tipoDoSindicatoSelecionado === "laboral") {
    data = [
      {
        id: "",
        description: "--",
      },
      {
        id: TipoComentario.SindicatoLaboral,
        description: "Sindicato Laboral",
      },
    ];
  }

  if (tipoDoSindicatoSelecionado === "patronal") {
    data = [
      {
        id: "",
        description: "--",
      },
      {
        id: TipoComentario.SindicatoPatronal,
        description: "Sindicato Patronal",
      },
    ];
  }

  if (usuario.nivel === UsuarioNivel.Ineditta) {
    data.push({
      id: TipoComentario.Filial,
      description: "Estabelecimento",
    })
  }

  return await Promise.resolve(data)
}

function carregarInformacoesUsuario() {
  $("#id_user_2").val(UserInfoService.getUserId());

  const nomeUsuario =
    UserInfoService.getFirstName() + " " + UserInfoService.getLastName();

  $("#usuario").val(nomeUsuario);
}

async function obterPorId(id) {
  limparFormulario()

  const response = await comentarioService.obterPorId(id)

  if (!response) {
    return NotificationService.error({ title: "Erro", message: "Notificação não foi encontrada" })
  }

  $("#id_note").val(response.value.id);
  $("#id_user_2").val(response.value.administradorId);
  $("#usuario").val(response.value.administradorNome);
  $("#comentario").val(response.value.comentario);

  dataValidade?.setValue(response.value.dataFinal);

  if (response.value.tipoComentario) {
    const tiposComentarios = await obterTipoComentario();

    tipoComentarioSelect?.setCurrentValue([
      {
        id: response.value.tipoComentario.id,
        description: tiposComentarios.find((tc) => tc.id == response.value.tipoComentario.id)?.description,
      },
    ])
  }

  if (response.value.assunto) {
    assuntoSelect?.setCurrentValue([
      {
        id: response.value.assunto.id,
        description: response.value.assunto.description,
      },
    ])
  }

  if (response.value.tipoUsuarioDestinoSelect) {    
    tipoUsuarioDestinoSelect?.setCurrentValue([{ id: response.value.tipoUsuarioDestino, description: getDescriptionTipoUsuarioDestino(response.value.tipoUsuarioDestino - 1) }])
    destinoSelect?.setCurrentValue([{ id: response.value.usuarioDestinoId, description: response.value.usuarioDestinoDescricao }])
  }

  if (response.value.tipoNotificacaoId) {
    tipoNotificacaoSelect?.setCurrentValue([{ id: response.value.tipoNotificacaoId, description: getDescriptionTipoUsuarioNotificacao(response.value.tipoNotificacaoId - 1) }])
  }

  if (response.value.etiqueta) {
    etiquetaSelect?.setCurrentValue([{ id: response.value.etiqueta.id, description: response.value.etiqueta.nome }])
    tipoEtiquetaSelect?.setCurrentValue([{ id: response.value.etiqueta.tipo.id, description: response.value.etiqueta.tipo.nome }])
  }
  
  const visivel = response.value.visivel
  visivelSelect?.setCurrentValue([{ id: visivel, description: visivel ? 'Sim' : 'Não' }])

  configurarTituloAssunto(response.value.tipoComentario?.id)
  configurarTituloTipoUsuario(response.value.tipoUsuarioDestino?.id)
}

async function upsert() {
  const id = $("#id_note").val()
  return id ? await editar() : await incluir()
}

async function incluir() {
  const podeComentarSindicato = permissoesUsuario?.some(p => p.modulo_id == MODULO_SINDICATO_ID && p.comentar == 1);
  if (!podeComentarSindicato) {
    return NotificationService.error({ title: "Comentário não realizado", message: "Você não possui as permissões necessárias" })
  }

  const isFixo = tipoNotificacaoSelect?.getValue() == TipoNotificacao.Fixa;

  if (dataValidade.getValue() === null && !isFixo) {
    return NotificationService.error({ title: "Não foi possível realizar o cadastro! Erro: ", message: "Você precisa escolher uma validade para comentários temporários." })
  }

  const requestData = {
    id: 0,
    tipo: parseInt(tipoComentarioSelect.getValue()),
    valor: $("#comentario").val(),
    tipoNotificacao: parseInt(tipoNotificacaoSelect.getValue()),
    referenciaId: parseInt(assuntoSelect.getValue()),
    dataValidade: dataValidade?.getValue(),
    tipoUsuarioDestino: parseInt(tipoUsuarioDestinoSelect.getValue()),
    usuarioDestionoId: parseInt(destinoSelect.getValue()),
    etiquetaId: parseInt(etiquetaSelect.getValue()),
    visivel: visivelSelect.getValue() == BooleanType.Sim ? true : null
  }

  const result = await comentarioService.incluir(requestData)

  if (result.isFailure()) {
    NotificationService.error({ title: "Não foi possível realizar o cadastro! Erro: ", message: result.error })

    return result
  }

  NotificationService.success({ title: "Cadastro realizado com sucesso!", message: "" })

  limpar()

  reloadDataTable()

  await filter()

  

  return Result.success()
}

async function editar() {
  const requestData = {
    module: "notificacao",
    action: "updateNotificacao",
    tipo_note: tipoNotificacaoSelect.getValue(),
    validade: dataValidade?.getValue(),
    etiqueta: etiquetaSelect.getValue(),
    comentario: $("#comentario").val(),
    id: $("#id_note").val(),
  };

  const result = await comentarioService.editar(requestData);

  if (result.isFailure()) {
    NotificationService.error({
      title: "Não foi possível realizar a atualização! Erro: ",
      message: result.error,
    });

    return result;
  }

  NotificationService.success({
    title: "Cadastro atualizado com sucesso!",
    message: "",
  });

  limpar();

  return Result.success();
}

function limpar() {
  limparFormulario();
  reloadDataTable();
}

function reloadDataTable() {
  sindEstadosTable?.reload();
}

function limparFormulario() {
  $("#comentario").val("");
  $("#id_note").val("");

  carregarInformacoesUsuario();

  tipoComentarioSelect?.clear();
  etiquetaSelect?.clear();
  tipoNotificacaoSelect?.clear();
  tipoUsuarioDestinoSelect?.clear();
  assuntoSelect?.clear();
  destinoSelect?.clear();
  dataValidade?.setValue(null);
}

function limparModalInfo() {
  $("#infoSindForm").trigger("reset");
}

async function obterInfoSindicatoPorId(id, tipoSind) {
  const infoResult = await sindicatoService.obterInfoSindical(id, tipoSind);

  preencherModalInfoSindical(infoResult.value, id, tipoSind);
}

function preencherModalInfoSindical(data, id, tipoSind) {
  limparModalInfo();

  $("#info-sigla").val(data.sigla);
  $("#info-cnpj").maskCNPJ().val(data.cnpj).trigger("input");
  $("#info-razao").val(data.razaoSocial);
  $("#info-denominacao").val(data.denominacao);
  $("#info-cod-sindical").val(data.codigoSindical);
  $("#info-uf").val(data.uf);
  $("#info-municipio").val(data.municipio);
  $("#info-logradouro").val(data.logradouro);
  $("#info-telefone1").maskCelPhone().val(data.telefone1).trigger("input");
  $("#info-telefone2").maskCelPhone().val(data.telefone2).trigger("input");
  $("#info-telefone3").maskCelPhone().val(data.telefone3).trigger("input");
  $("#info-ramal").val(data.ramal);
  $("#info-enquadramento").val(data.contatoEnquadramento);
  $("#info-negociador").val(data.contatoNegociador);
  $("#info-contribuicao").val(data.contatoContribuicao);
  $("#info-email1")
    .val(data.email1)
    .attr("style", data.email1 ? "cursor: pointer" : null);
  $("#info-email1-link").attr("href", `mailto:${data.email1}`);
  $("#info-email2")
    .val(data.email2)
    .attr("style", data.email2 ? "cursor: pointer" : null);
  $("#info-email2-link").attr("href", `mailto:${data.email2}`);
  $("#info-email3")
    .val(data.email3)
    .attr("style", data.email3 ? "cursor: pointer" : null);
  $("#info-email3-link").attr("href", `mailto:${data.email3}`);
  $("#info-twitter")
    .val(data.twitter)
    .attr("style", data.twitter ? "cursor: pointer" : null);
  $("#info-twitter-link").attr("href", formatarLinks(data.twitter, "twitter"));
  $("#info-facebook")
    .val(data.facebook)
    .attr("style", data.facebook ? "cursor: pointer" : null);
  $("#info-facebook-link").attr(
    "href",
    formatarLinks(data.facebook, "facebook")
  );
  $("#info-instagram")
    .val(data.instagram)
    .attr("style", data.instagram ? "cursor: pointer" : null);
  $("#info-instagram-link").attr(
    "href",
    formatarLinks(data.instagram, "instagram")
  );
  $("#info-site")
    .val(data.site)
    .attr("style", data.site ? "cursor: pointer" : null);
  $("#info-site-link").attr("href", formatarLinks(data.site, "site"));
  $("#info-data-base").val(data.dataBase);
  $("#info-ativ-econ").val(data.atividadeEconomica);
  $("#info-federacao-nome").val(data?.federacao?.nome);
  $("#info-federacao-cnpj")
    .maskCNPJ()
    .val(data?.federacao?.cnpj)
    .trigger("input");
  $("#info-confederacao-nome").val(data?.confederacao?.nome);
  $("#info-confederacao-cnpj")
    .maskCNPJ()
    .val(data?.confederacao?.cnpj)
    .trigger("input");
  $("#info-central-sind-nome").val(data?.centralSindical?.nome);
  $("#info-central-sind-cnpj")
    .maskCNPJ()
    .val(data?.centralSindical?.cnpj)
    .trigger("input");

  $("#direct-clausulas-btn").attr(
    "href",
    `consultaclausula.php?sindId=${id}&tipoSind=${tipoSind}&comparativo=${false}&sigla=${data.sigla
    }`
  );
  $("#direct-comparativo-btn").attr(
    "href",
    `consultaclausula.php?sindId=${id}&tipoSind=${tipoSind}&comparativo=${true}&sigla=${data.sigla
    }`
  );
  $("#direct-calendarios-btn").attr(
    "href",
    `calendario_sindical.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  );
  $("#direct-documentos-btn").attr(
    "href",
    `consulta_documentos.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  );
  $("#direct-formulario-aplicacao-btn").attr(
    "href",
    `formulario_comunicado.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  );

  $("#direct-gerar-excel-btn").attr(
    "href",
    `geradorCsv.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  );

  $("#direct-comparativo-mapa-btn").attr(
    "href",
    `comparativo.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
  );

  $("#direct-relatorio-negociacoes-btn").attr(
    "href",
    `relatorio_negociacoes.php?sigla=${data.sigla}`
  );
}

$("#sind_emp").prop("disabled", true);
$("#sind_patr").prop("disabled", true);

$("#sind_emp_cod").prop("disabled", true);
$("#sind_patr_cod").prop("disabled", true);

let uf_encontradas = [];

function formatarLinks(string, tipo) {
  if (string == null) return "";
  if (string.includes(".com")) {
    if (string.includes("http")) {
      return string;
    } else {
      return `https://${string}`;
    }
  }

  if (tipo === "site") {
    if (string.includes("http")) {
      return string;
    } else {
      return `https://${string}`;
    }
  }

  if (tipo === "twitter") return `https://twitter.com/${string}`;
  if (tipo === "instagram") return `https://instagram.com/${string}`;
  if (tipo === "facebook") return `https://facebook.com/${string}`;
}

/*********************************************************************
 * BUSCANDO SINDICATOS COM COD_SINDCLIENTE
 ********************************************************************/

$("#codigo").on("change", () => {
  var cod = $("#codigo").val();

  $.ajax({
    url: "includes/php/ajax.php",
    type: "post",
    dataType: "json",
    data: {
      module: "modulo_sindicato",
      action: "getSindicatosByCodigo",
      busca: cod,
    },
    success: function (data) {
      $("#sind_emp").prop("disabled", false);
      $("#sind_patr").prop("disabled", false);

      $("#sind_emp").html(data.response_data.optEmp);
      $("#sind_patr").html(data.response_data.optPatr);
    },
  });
});

/*********************************************************************
 * BUSCANDO SINDICATOS COM CODIGO UNIDADE
 ********************************************************************/

$("#cod_unidade").on("change", () => {
  var cod = $("#cod_unidade").val();

  $.ajax({
    url: "includes/php/ajax.php",
    type: "post",
    dataType: "json",
    data: {
      module: "modulo_sindicato",
      action: "getSindicatosByCodUnidade",
      busca: cod,
    },
    success: function (data) {
      $("#sind_emp_cod").prop("disabled", false);
      $("#sind_patr_cod").prop("disabled", false);

      $("#sind_emp_cod").html(data.response_data.optEmpCod);
      $("#sind_patr_cod").html(data.response_data.optPatrCod);
    },
  });
});

async function carregarDataTableInfoDiretoriaTb(sindId, tipoSind) {
  if (diretoriaInfoSindTb) {
    diretoriaInfoSindTb.reload();
    return;
  }

  diretoriaInfoSindTb = new DataTableWrapper("#diretoriainfosindtb", {
    ajax: async (requestData) =>
      await sindicatoService.obterInfoDiretoriaSindDatatable(
        requestData,
        sindId,
        tipoSind
      ),
    columns: [
      { title: "Dirigente", data: "nome" },
      {
        title: "Início Mandato",
        data: "inicioMandato",
      },
      { title: "Fim Mandato", data: "fimMandato" },
      { title: "Função", data: "funcao" },
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

  await diretoriaInfoSindTb.initialize();
}

async function carregarDataTables() {
  await carregarOrganizacaoLaboralTb();
  await carregarOrganizacaoPatronalTb();
  await carregarDirigentesPatronalTb();
  await carregarDirigentesLaboralTb();
  $("#exportarLaboral").show();
  $("#exportarPatronal").show();
}

//MODAL COMENTARIOS
async function carregarSindCommentariosPorUF() {
  if (sindEstadosTable) {
    sindEstadosTable.reload();
    return;
  }

  sindEstadosTable = new DataTableWrapper("#sindEstadosTable", {
    columns: [
      { data: "id", orderable: false },
      { title: "Sigla Sindicato", data: "sigla" },
      { title: "Tipo", data: "sindTipo" },
      { title: "Comentários", data: "comentario" },
      { title: "Etiqueta", data: "etiqueta" },
      { title: "Usuário Responsável", data: "usuarioResponsavel" },
      { title: "Data do comentário", data: "criadoEm" },
    ],
    columnDefs: [
      {
        targets: 6,
        render: (data) => DataTableWrapper.formatDate(data),
      },
      {
        targets: "_all",
        defaultContent: "",
      },
    ],
    ajax: async (requestData) => {
      requestData = {
        ...requestData,
        ...filterParams,
      };
      requestData.uf = selectedUF;
      return await sindicatoService.obterComentariosPorUF(requestData);
    },
    rowCallback: function (row, data) {
      const buttonAddComment = $("<button>")
        .attr("data-toggle", "modal")
        .attr("data-target", "#notificacaoModal")
        .addClass("btn-add-comment")
        .append("<i class='fa fa-comment' aria-hidden='true'></i>")
        .on("click", () => {
          tipoDoSindicatoSelecionado = data?.sindTipo;
          sindicatoSelecionadoId = data?.sindId;
          sindicatoSelecionado = data?.sigla + " - " + data?.sindRazaoSocial;
          setarInfoFormularioSelect();
        });

      $("td:eq(0)", row).html(buttonAddComment);
    },
  });

  await sindEstadosTable.initialize();
}

async function carregarOrganizacaoLaboralTb() {
  if (organizacaoSindicalLaboralTb) {
    organizacaoSindicalLaboralTb.reload();
    return;
  }

  organizacaoSindicalLaboralTb = new DataTableWrapper("#organizacaolaboraltb", {
    ajax: async (requestData) => {
      requestData = {
        ...requestData,
        ...filterParams,
      };
      return await sindicatoService.obterOrganizacaoSindicalLaboralDatatable(
        requestData
      );
    },
    columns: [
      { title: "Localidade", data: "municipio" },
      {
        title: "Sindicato (Sigla Laboral)",
        data: "sigla",
      },
      { title: "Federação Laboral", data: "nomeFederacao" },
      { title: "Confederação Laboral", data: "nomeConfederacao" },
      { title: "Central Sindical Laboral", data: "nomeCentralSindical" },
      {
        title: "MTE",
        data: "cnpj",
        render: function (data) {
          return `<a href="http://www3.mte.gov.br/sistemas/CNES/usogeral/HistoricoEntidadeDetalhes.asp?NRCNPJ=${data}" target="_blank" class="btn-default-alt" style="display: flex; box-shadow: none; justify-content: center; border: none;"><img src="includes/img/icon-MTE.png" alt="" style="width: 20px;"></a>`;
        },
      },
    ],
    rowCallback: function (row, data) {
      let link = $("<a>")
        .attr("data-id", data?.id)
        .attr("href", "#")
        .html(data?.sigla);
      link.on("click", function () {
        const id = $(this).attr("data-id");
        $("#sind-id-input").val(id);
        $("#tipo-sind-input").val("laboral");
        $("#openInfoSindModalBtn").trigger("click");
      });
      $("td:eq(1)", row).html(link);
    },
  });

  await organizacaoSindicalLaboralTb.initialize();
}

async function carregarOrganizacaoPatronalTb() {
  if (organizacaoSindicalPatronalTb) {
    organizacaoSindicalPatronalTb.reload();
    return;
  }

  organizacaoSindicalPatronalTb = new DataTableWrapper(
    "#organizacaopatronaltb",
    {
      ajax: async (requestData) => {
        requestData = {
          ...requestData,
          ...filterParams,
        };
        return await sindicatoService.obterOrganizacaoSindicalPatronalDatatable(
          requestData
        );
      },
      columns: [
        { title: "Localidade", data: "municipio" },
        {
          title: "Sindicato (Sigla Patronal)",
          data: "sigla",
        },
        { title: "Federação Patronal", data: "nomeFederacao" },
        { title: "Confederação Patronal", data: "nomeConfederacao" },
        {
          title: "MTE",
          data: "cnpj",
          render: function (data) {
            return `<a href="http://www3.mte.gov.br/sistemas/CNES/usogeral/HistoricoEntidadeDetalhes.asp?NRCNPJ=${data}" target="_blank" class="btn-default-alt" style="display: flex; box-shadow: none; justify-content: center; border: none;"><img src="includes/img/icon-MTE.png" alt="" style="width: 20px;"></a>`;
          },
        },
        { title: "Estabelecimentos", sortable: false, visible: podeConsultarModulo() },
        { title: "Associado", data: "associado", visible: podeConsultarModulo()  }
      ],
      rowCallback: function (row, data) {
        let link = $("<a>")
          .attr("data-id", data?.id)
          .attr("href", "#")
          .html(data?.sigla);
        link.on("click", function () {
          const id = $(this).attr("data-id");
          $("#sind-id-input").val(id);
          $("#tipo-sind-input").val("patronal");
          $("#openInfoSindModalBtn").trigger("click");
        });
        $("td:eq(1)", row).html(link);

        let estaButton = $("<a>")
          .attr("data-id", data?.id)
          .attr("style", "cursor: pointer")
          .addClass("btn btn-default w-full")
          .html(`<i class="fa fa-refresh"></i>`);

        let estaButtonDiv = $("<div>").addClass("w-full").append([
          podeConsultarModulo() ? estaButton : null,
        ]);

        estaButton.on("click", function () {
          const sindicatoId = $(this).attr("data-id");
          sindicatoSelecionadoId = sindicatoId;
          $("#filialModalOpenBtn").trigger("click");
        });
        $("td:eq(5)", row).html(estaButtonDiv);
      },
    }
  );

  await organizacaoSindicalPatronalTb.initialize();
}

async function carregarDirigentesPatronalTb() {
  if (dirigentesSindicaisPatronaisTb) {
    dirigentesSindicaisPatronaisTb.reload();
    return;
  }

  dirigentesSindicaisPatronaisTb = new DataTableWrapper(
    "#dirigentespatronaistb",
    {
      ajax: async (requestData) => {
        requestData = {
          ...requestData,
          ...filterParams,
        };
        return await sindicatoService.obterDirigentesPatronalDatatable(
          requestData
        );
      },
      columns: [
        { title: "Nome", data: "nome" },
        { title: "Cargo", data: "cargo" },
        { title: "Sigla", data: "sigla" },
        { title: "Início do Mandato", data: "inicioMandato", render: (data) => DataTableWrapper.formatDate(data) },
        { title: "Término do Mandato", data: "terminoMandato", render: (data) => DataTableWrapper.formatDate(data) },
        { title: "Estabelecimento", data: null },
        { title: "Afastado para atividades", data: "situacao" },
      ],
      rowCallback: function (row, data) {
        let link = $("<a>")
          .attr("data-id", data?.sindId)
          .attr("href", "#")
          .html(data?.sigla);
        link.on("click", function () {
          const id = $(this).attr("data-id");
          $("#sind-id-input").val(id);
          $("#tipo-sind-input").val("patronal");
          $("#openInfoSindModalBtn").trigger("click");
        });
        $("td:eq(2)", row).html(link);

        let estaButton = $("<a>")
          .attr("data-id", data?.sindId)
          .attr("data-unidadeId", data?.unidadeId)
          .attr("data-dirigenteId", data?.id)
          .attr("style", "cursor: pointer")
          .addClass("btn btn-default")
          .html(`<i class="fa fa-refresh"></i>`);

        let estaButtonDiv = $("<div>").append([
          comentar ? estaButton : null,
          $("<span>").text(data?.nomeUnidade),
        ]);

        estaButton.on("click", function () {
          const id = $(this).attr("data-id");
          const unidadeId = $(this).attr("data-unidadeId");
          const dirId = $(this).attr("data-dirigenteId");
          $("#sind-id-input").val(id);
          $("#tipo-sind-input").val("patronal");
          $("#unidade-id-input").val(unidadeId);
          $("#afastado-input").val(data?.situacao);
          $("#diretoria-id-input").val(dirId);
          $("#openEstabelecimentoModalBtn").trigger("click");
        });
        $("td:eq(5)", row).html(estaButtonDiv);
      },
    }
  );

  await dirigentesSindicaisPatronaisTb.initialize();
}

async function carregarDirigentesLaboralTb() {
  if (dirigentesSindicaisLaboraisTb) {
    dirigentesSindicaisLaboraisTb.reload();
    return;
  }

  dirigentesSindicaisLaboraisTb = new DataTableWrapper(
    "#dirigenteslaboraistb",
    {
      ajax: async (requestData) => {
        requestData = {
          ...requestData,
          ...filterParams,
        };
        return await sindicatoService.obterDirigentesLaboralDatatable(
          requestData
        );
      },
      columns: [
        { title: "Nome", data: "nome" },
        { title: "Cargo", data: "cargo" },
        { title: "Sigla", data: "sigla" },
        { title: "Início do Mandato", data: "inicioMandato", render: (data) => DataTableWrapper.formatDate(data) },
        {
          title: "Término do Mandato", data: "terminoMandato", render: (data) => DataTableWrapper.formatDate(data)
        },
        { title: "Estabelecimento", data: null },
        { title: "Afastado para atividades", data: "situacao" },
      ],
      rowCallback: function (row, data) {
        let link = $("<a>")
          .attr("data-id", data?.sindId)
          .attr("href", "#")
          .html(data?.sigla);
        link.on("click", function () {
          const id = $(this).attr("data-id");
          $("#sind-id-input").val(id);
          $("#tipo-sind-input").val("laboral");
          $("#openInfoSindModalBtn").trigger("click");
        });
        $("td:eq(2)", row).html(link);

        let estaButton = $("<a>")
          .attr("data-id", data?.sindId)
          .attr("data-unidadeId", data?.unidadeId)
          .attr("data-dirigenteId", data?.id)
          .attr("style", "cursor: pointer")
          .addClass("btn btn-default")
          .html(`<i class="fa fa-refresh"></i>`);

        let estaButtonDiv = $("<div>").append([
          comentar ? estaButton : null,
          $("<span>").text(data?.nomeUnidade),
        ]);

        estaButton.on("click", function () {
          const id = $(this).attr("data-id");
          const unidadeId = $(this).attr("data-unidadeId");
          const dirId = $(this).attr("data-dirigenteId");
          $("#sind-id-input").val(id);
          $("#tipo-sind-input").val("laboral");
          $("#unidade-id-input").val(unidadeId);
          $("#afastado-input").val(data?.situacao);
          $("#diretoria-id-input").val(dirId);
          $("#openEstabelecimentoModalBtn").trigger("click");
        });
        $("td:eq(5)", row).html(estaButtonDiv);
      },
    }
  );

  await dirigentesSindicaisLaboraisTb.initialize();
}

/*********************************************************************
 * INICIALIZANDO GRÁFICOS
 ********************************************************************/

async function gerarGraficos(data) {
  let dataSet = data.negAbertoPorEstado;

  let uf = [];
  let uf_qtd = [];

  for (let i = 0; i < dataSet.length; i++) {
    //uf_encontradas.push(dataSet[i].uf);
    uf.push(dataSet[i].uf + " (" + dataSet[i].quantidade + ")");
    uf_qtd.push(dataSet[i].quantidade);
  }

  const dataSets = [
    {
      label: "Em aberto",
      data: uf_qtd,
      backgroundColor: ["rgba(54, 162, 235, 0.2)"],
      borderColor: ["rgba(54, 162, 235, 1)"],
      borderWidth: 1,
    },
  ];

  const chartNegAbertoEstadoElement = document.getElementById("sindChart");

  chartNegAbertoEstadoElement.getContext("2d");
  chartNegAbertoEstado = new Chart(chartNegAbertoEstadoElement, {
    type: "bar",
    data: {
      labels: uf,
      datasets: dataSets,
    },
    options: {
      scales: {
        y: {
          beginAtZero: true,
        },
      },
      plugins: {
        title: {
          display: true,
          text: "Negociações em aberto por estado",
        },
      },
    },
  });

  var dataSetPizza = data.qtdCentraisSindicais;

  var central = [];
  var qtd_centrais = [];

  let centralName;
  for (let i = 0; i < dataSetPizza.length; i++) {
    if (dataSetPizza[i].central == "" || dataSetPizza[i].central == null) {
      centralName = "Não há declaração de filiação";
    } else {
      centralName = dataSetPizza[i].central;
    }
    central.push(centralName);
    qtd_centrais.push(dataSetPizza[i].qtd);
  }

  const chartQtdCentraisSindicaisElement = document.getElementById("qtdChart");

  chartQtdCentraisSindicaisElement.getContext("2d");
  chartQtdCentraisSindicais = new Chart(chartQtdCentraisSindicaisElement, {
    type: "pie",
    data: {
      labels: central,
      datasets: [
        {
          label: "Quantidade",
          data: qtd_centrais,
          backgroundColor: [
            "rgba(255, 159, 64, 0.7)",
            "rgba(54, 162, 235, 0.7)",
            "rgba(255, 99, 13, 0.7)",
            "rgba(255,99,132, 0.7)",
            "rgba(201,203,207, 0.7)",
            "rgba(153,102,255, 0.7)",
            "rgba(80,102,180, 0.7)",
            "rgba(255,205,86, 0.7)",
          ],
        },
      ],
    },
    options: {
      plugins: {
        legend: {
          position: "right",
        },
      },
    },
  });

  uf_encontradas = data.estadosComentarios;

  // Faz uma requisição AJAX para a página "processar_ufs.php", passando o array de UFs como parâmetro
  let xhr = new XMLHttpRequest();
  xhr.open("POST", "map_sindical_cores.php");
  xhr.setRequestHeader("Content-Type", "application/json");
  xhr.send(JSON.stringify({ ufs: uf_encontradas }));

  xhr.onload = function () {
    let containerMap = $("#ufs-container");
    containerMap.html(xhr.responseText);

    let infoIcon = $("<i>")
      .addClass("fa fa-info-circle info-icon")
      .attr("aria-hidden", "true")
      .attr(
        "title",
        "Clique em um estado do mapa para visualizar os comentários"
      )
      .on("click", () =>
        alert("Clique em um estado do mapa para visualizar os comentários")
      );

    containerMap.append(infoIcon);
    initOnClickMap();
  };
}

async function gerarExcelGeral() {
  filterParams = {
    grupoEconomicoId: $("#grupo").val(),
    matrizesIds: $("#matriz").val(),
    clientesUnidadesIds: $("#unidade").val(),
    sindLaboraisIds: $("#sind_laboral").val(),
    sindPatronaisIds: $("#sind_patronal").val(),
    dataBase: $("#data_base").val(),
    cnaesIds: $("#categoria").val(),
  };

  const tipoLocalidade = $("#tipoLocalidade").val();
  if (tipoLocalidade === "uf") {
    filterParams.ufs = $("#localidade").val();
  } else if (tipoLocalidade === "regiao") {
    filterParams.regioes = $("#localidade").val();
  } else {
    filterParams.localizacoesIds = $("#localidade").val();
  }

  const result = await sindicatoService.gerarExcelSindicatos(filterParams);

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao baixar excel', message: result.error });
  }

  const bytes = result.value.data.blob;

  const date = moment().format("DD-MM-YYYY");

  PageWrapper.downloadExcel(bytes, `relatório_sindicatos_laboral_e_patronal_${date}.xlsx`);
}

async function gerarExcelLaboral() {
  const result = await sindicatoService.gerarExcelSindicatosLaborais(filterParams);

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao baixar excel', message: result.error });
  }

  const bytes = result.value.data.blob;

  const date = moment().format("DD-MM-YYYY");

  PageWrapper.downloadExcel(bytes, `relatorio_sindicatos_laboral_${date}.xlsx`);
}

async function gerarExcelPatronal() {
  const result = await sindicatoService.gerarExcelSindicatosPatronais(filterParams);

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao baixar excel', message: result.error });
  }

  const bytes = result.value.data.blob;

  const date = moment().format("DD-MM-YYYY");

  PageWrapper.downloadExcel(bytes, `relatorio_sindicatos_patronal_${date}.xlsx`);
}

/*********************************************************************
 * APLICANDO FILTROS
//  ********************************************************************/
async function filter(matrizes, grupo, cnaes, unidades) {
  filterParams = {
    grupoEconomicoId: $("#grupo").val(),
    matrizesIds: $("#matriz").val(),
    clientesUnidadesIds: $("#unidade").val(),
    sindLaboraisIds: $("#sind_laboral").val(),
    sindPatronaisIds: $("#sind_patronal").val(),
    dataBase: $("#data_base").val(),
    cnaesIds: $("#categoria").val(),
  };

  const tipoLocalidade = $("#tipoLocalidade").val();
  if (tipoLocalidade === "uf") {
    filterParams.ufs = $("#localidade").val();
  } else if (tipoLocalidade === "regiao") {
    filterParams.regioes = $("#localidade").val();
  } else {
    filterParams.localizacoesIds = $("#localidade").val();
  }

  const result = await sindicatoService.obterSindicatos(filterParams);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  listaSindeIds = result.value.sindicatosLaborais.map((sind) => sind.id);
  listaSindpIds = result.value.sindicatosPatronais.map((sind) => sind.id);


  const mandatosSindicais = result.value.mandatosSindicais;

  await carregarDataTables();

  if (chartNegAbertoEstado) {
    chartNegAbertoEstado.destroy();
  }
  if (chartQtdCentraisSindicais) {
    chartQtdCentraisSindicais.destroy();
  }

  await gerarGraficos(mandatosSindicais);

  $(".img_box").css("display", "none");
  $("#qtd_emp").html(listaSindeIds.length);
  $("#qtd_patr").html(listaSindpIds.length);
  $("#mand_vencido").html(mandatosSindicais.mandatosVencidos);
  $("#mand_vigente").html(mandatosSindicais.mandatosVigentes);

  $("#neg_vencida").html(mandatosSindicais.negVencidas);
  $("#neg_vigente").html(mandatosSindicais.negVigentes);

  //GERANDO TABELA DOS ESTADOS

  // loop over the 27 UFs of Brazil

  return Result.success();
}

//LIMPAR FILTRO

function limparFiltro() {
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
  $("#tipoLocalidade").val("uf").trigger("change");
  localizacoesSelect.reload();
  cnaeSelect.reload();
  sindLaboralSelect.reload();
  sindPatronalSelect.reload();
}

function rolarAte(id) {
  $("html, body").animate(
    {
      scrollTop: $("#" + id).offset().top,
    },
    1000
  );
}

//tabela por estado
function setTableUF(uf) {
  selectedUF = uf;
}

//inicializa os onclicks do mapa
function initOnClickMap() {
  $(".estado-uf").on("click", (el) => {
    setTableUF(el.currentTarget.dataset["uf"]);
  });
}

//funcoes das atualizacoes de estabelecimento

async function updateEsta() {
  const requestData = {
    module: "modulo_sindicato",
    action: "updateEsta",
    id_dir: $("#diretoria-id-input").val(),
    afasta: $("#afastado").val(),
    esta: $("#estabelecimento-update-input").val(),
  };

  const result = await sindicatoService.postLegado(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Alterado com sucesso!",
  });

  return Result.success();
}

async function limparEsta() {
  const requestData = {
    module: "modulo_sindicato",
    action: "updateEsta",
    id_dir: $("#diretoria-id-input").val(),
    afasta: null,
    esta: null,
  };

  const result = await sindicatoService.postLegado(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Alterado com sucesso!",
  });

  return Result.success();
}

async function updateEstaP() {
  const requestData = {
    module: "modulo_sindicato",
    action: "updateEstaP",
    id_dir: $("#diretoria-id-input").val(),
    afasta: $("#afastado").val(),
    esta: $("#estabelecimento-update-input").val(),
  };

  const result = await sindicatoService.postLegado(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Alterado com sucesso!",
  });

  return Result.success();
}

async function limparEstaP() {
  const requestData = {
    module: "modulo_sindicato",
    action: "updateEstaP",
    id_dir: $("#diretoria-id-input").val(),
    afasta: null,
    esta: null,
  };

  const result = await sindicatoService.postLegado(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Alterado com sucesso!",
  });

  return Result.success();
}

async function atualizarEstabelecimentosAssociadoSindicato() {
  const customParams = {
    clienteUnidadeId: filiaisIdsSelecionadas,
    sindicatoPatronalId: sindicatoSelecionadoId
  }

  const result = await clienteUnidadeSindicatoPatronalService.atualizar(customParams);

  if (result.isFailure()) {
    NotificationService.error({title: "Erro ao atualizar"});
    return;
  }
  else {
    NotificationService.success({title: "Dados atualizados com sucesso"});
  }

  await carregarOrganizacaoPatronalTb();
}

async function consultarUrl() {
  const queryString = window.location.search;
  const urlParams = new URLSearchParams(queryString);

  const sindicatoId = urlParams.get("sindId");
  const tipoSindicato = urlParams.get("tipoSind");

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

  await filter();
}
