import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';

import JQuery from 'jquery';
import $ from 'jquery';
import 'moment';
import 'daterangepicker';
import "../../js/utils/masks/jquery-mask-extensions.js";

//import 'fullcalendar';
import 'sweetalert2';
import 'chosen-js';

// Temp
import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'datatables.net-bs5';
import 'datatables.net-responsive-bs5';
import moment from "moment";
import "mark.js/dist/jquery.mark.es6.js";

import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import DateFormatter from '../../js/utils/date/date-formatter.js';
import { td, th, h2, thead, tr, tbody, table, div, i, a } from '../../js/utils/components/elements';

import { InformacaoAdicionalTipo } from '../../js/application/clausulas/constants/informacao-adicional-tipo.js';
import { TipoObservacaoAdicional } from '../../js/application/informacoesAdicionais/cliente/constants/tipo-obeservacao-adicional.js';
import { UsuarioNivel } from '../../js/application/usuarios/constants/usuario-nivel';
import { JOpcaoAdicional } from '../../js/utils/components/informacao-adicional/j-search-option-adicional.js';

// Services
import {
  ClausulaService,
  UsuarioAdmService,
  EventosCalendarioService,
  DocSindService,
  ClienteUnidadeService,
  GrupoEconomicoService,
  ClausulaGeralService,
  TipoDocService,
  AcompanhamentoCctService,
  CnaeService,
  EstruturaClausulaService,
  LocalizacaoService,
  MatrizService,
  SindicatoService,
  SindicatoPatronalService,
  SindicatoLaboralService
} from "../../js/services"

// Core
import { AuthService, ApiService } from '../../js/core/index.js';
import { ApiLegadoService } from '../../js/core/api-legado.js';

// Components
import Notification from '../../js/utils/notifications/notification.service.js'
import DatepickerrangeWrapper from '../../js/utils/daterangepicker/daterangepicker-wrapper.js';
import SelectWrapper from '../../js/utils/selects/select-wrapper';

import {
  closeModal,
  renderizarModal,
} from "../../js/utils/modals/modal-wrapper.js";

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import NotificationService from '../../js/utils/notifications/notification.service.js';
import Result from '../../js/core/result.js';
import { ModalInfoSindicato } from '../../js/utils/components/modalInfoSindicatos/modal-info-sindicatos.js';

// Services
const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();

const acompanhamentoCctService = new AcompanhamentoCctService(apiService);
const grupoEconomicoService = new GrupoEconomicoService(apiService);
const matrizService = new MatrizService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const sindicatoPatronalService = new SindicatoPatronalService(apiService, apiLegadoService);
const sindicatoLaboralService = new SindicatoLaboralService(apiService, apiLegadoService);
const localizacaoService = new LocalizacaoService(apiService, apiLegadoService);
const cnaeService = new CnaeService(apiService);
const docSindService = new DocSindService(apiService);
const tipoDocumentoService = new TipoDocService(apiService);
const usuarioAdmSerivce = new UsuarioAdmService(apiService);
const estruturaClausulaService = new EstruturaClausulaService(apiService);
const eventosCalendarioService = new EventosCalendarioService(apiService, apiLegadoService);
const sindicatoService = new SindicatoService(apiService, apiLegadoService);
const clausulaService = new ClausulaService(apiService, apiLegadoService);
const clausulaGeralService = new ClausulaGeralService(apiService);

let localidadeSelect = null;
let assuntosSelect = null;
let nomeDocumentoSelect = null;
let atividadeEconomicaSelect = null;
let periodo = null;
let grupoEconomicoSelect = null;
let matrizSelect = null;
let estabelecimentoSelect = null;
let sindicatoPatronalSelect = null;
let sindicatoLaboralSelect = null;
let origemSelect = null;
let gruposAssuntoSelect = null;
let tipoEventoSelect = null;
let repetirSelect = null;
let dadosClausulasPorId = null;
let lembreteSelect = null;

let eventosTable = null;
let agendaEventosTable = null;
let vencimentosMandatosSindicaisPatronaisTb = null;
let vencimentosMandatosSindicaisLaboraisTb = null;
let trintidioTb = null;
let eventosDeClausulaTb = null;
let diretoriaInfoSindTb = null;
let assembleiaReuniaoTb = null;

let eventoClausulaSelecionadoId = null;
let documentoDetalhesSelecionadoId = null;
let assembleiaSelecionadaId = null;
let assembleiaSelecionada = null;
let reuniaoSelecionadaId = null;
let reuniaoSelecionada = null;

let informacoesAdicionais = null
let informacoesAdicionaisCliente = null

let filtros = {};

const TiposEventos = {
  VencimentoDocumento: 'Vencimento de Documentos',
  VencimentoMandatoLaboral: 'Vencimento de Mandato Sindical Laboral',
  VencimentoMandatoPatronal: 'Vencimento de Mandato Sindical Patronal',
  Trintidio: 'Trintidio',
  EventosClausulas: 'Eventos de Clausulas',
  AgendaEventos: 'Agenda de Eventos',
  AssembleiaPatronalReuniaoSindical: 'Assembléias Patronais ou Reuniões Sindicais'
}

JQuery(async function () {
  new Menu()

  await AuthService.initialize();

  const dadosPessoais = await usuarioAdmSerivce.obterDadosPessoais();

  const permissoesUsuario = (await usuarioAdmSerivce.obterPermissoes()).value;

  if (dadosPessoais.isFailure()) {
    return;
  }

  const [permissoesModulo] = permissoesUsuario.filter(p => p.modulo_id == 5);

  configurarModal();

  await configurarSelects(dadosPessoais.value);

  $("#calendario").hide();
  $("#vencimento_mandato_patronal_panel").hide();
  $("#vencimento_mandato_laboral_panel").hide();
  $("#trintidio_panel").hide();
  $("#eventos_clausulas_panel").hide();
  $("#row-validade-repeticao-evento").hide();
  $("#agenda_eventos_panel").hide();
  $("#assembleia_reuniao_panel").hide();

  $("#form-filtros").on('submit', (e) => e.preventDefault());

  $("#btnLimparFiltro").on('click', () => limparFiltros(dadosPessoais.value));

  $("#btnFiltrar").on('click', async () => {
    if (periodo?.getBeginDate() && periodo?.getEndDate()) {
      await carregarDatatables();
    } else {
      alert("Você deve preencher o campo 'Período'.");
    }
  });

  if (permissoesModulo.criar != 1) {
    $("#btnAdicionarEventoSalvar").hide();
  }
  else {
    $("#btnAdicionarEventoSalvar").on('click', async () => {
      await criarEvento();
      await limparModalAdicionarEvento();
      await carregarAgendaEventos();
    });
  }

  await consultarUrl();

  $(".horizontal-nav").removeClass("hide");
});

async function configurarSelects(dadosPessoais) {
  const isIneditta = dadosPessoais?.nivel == UsuarioNivel.Ineditta;
  const isGrupoEconomico = dadosPessoais?.nivel == UsuarioNivel.GrupoEconomico;
  const isEstabelecimento = dadosPessoais?.nivel == UsuarioNivel.Estabelecimento;

  const markOptionAsSelectable = dadosPessoais?.nivel == 'Cliente' ? () => true : () => false;

  tipoEventoSelect = new SelectWrapper('#tipo_evento', {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: () => {
      return obterTipoEventos();
    }
  });

  localidadeSelect = new SelectWrapper('#localidade', {
    options: { placeholder: 'Selecione', multiple: true },
    onChange: async () => {
      await sindicatoLaboralSelect.reload();
      await sindicatoPatronalSelect.reload();
    },
    onOpened: async () => await obterLocalidades(),
  });

  gruposAssuntoSelect = new SelectWrapper('#grupo_clausulas', {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: async () => (await estruturaClausulaService.obterGruposSelect({ calendario: true }))?.value
  });

  assuntosSelect = new SelectWrapper('#assuntos', {
    options: { placeholder: 'Selecione', multiple: true },
    parentId: '#grupo_clausulas',
    onOpened: async () => {
      const gruposSelecionados = gruposAssuntoSelect?.getValue();
      return (await estruturaClausulaService.obterSelectPorGrupo(gruposSelecionados, true))?.value
    }
  });

  nomeDocumentoSelect = new SelectWrapper('#nome_doc', {
    options: { placeholder: 'Selecione', multiple: true }, onOpened: async () => {
      const isProcessado = true;
      return (await tipoDocumentoService.obterSelect({ processado: isProcessado, filtrarSelectType: true })).value
    }
  });

  atividadeEconomicaSelect = new SelectWrapper('#cnae', {
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
      await sindicatoPatronalSelect.reload();
    }
  });

  grupoEconomicoSelect = new SelectWrapper('#grupo', {
    options: { placeholder: 'Selecione', multiple: true },
    onChange: async () => {
      await atividadeEconomicaSelect?.reload();
      await sindicatoLaboralSelect?.reload();
      await sindicatoPatronalSelect?.reload();
    },
    onOpened: async () => await grupoEconomicoService.obterSelectPorUsuario(),
    markOptionAsSelectable: isIneditta ? () => false : () => true
  });
  if (isIneditta) {
    grupoEconomicoSelect.enable()
  } else {
    grupoEconomicoSelect.disable()
    await grupoEconomicoSelect.loadOptions()
  }

  matrizSelect = new SelectWrapper('#matriz', {
    options: {
      placeholder: 'Selecione',
      multiple: true
    }, parentId: '#grupo',
    onChange: async () => {
      await atividadeEconomicaSelect?.reload();
      await sindicatoLaboralSelect?.reload();
      await sindicatoPatronalSelect?.reload();
    },
    onOpened: async (grupoEconomicoId) => await matrizService.obterSelectPorUsuario(grupoEconomicoId),
    markOptionAsSelectable: isIneditta || isGrupoEconomico ? () => false : () => true
  });
  if (isIneditta || isGrupoEconomico) {
    matrizSelect.enable()
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

  estabelecimentoSelect = new SelectWrapper('#unidade',
    {
      options: {
        placeholder: 'Selecione', multiple: true
      },
      parentId: '#matriz',
      onChange: async () => {
        await atividadeEconomicaSelect?.reload();
        await sindicatoLaboralSelect?.reload();
        await sindicatoPatronalSelect?.reload();
      },
      onOpened: async (matrizId) => await clienteUnidadeService.obterSelectPorUsuario(matrizId),
      markOptionAsSelectable: isEstabelecimento ? () => true : () => false
    });
  if (isEstabelecimento) {
    const options = await estabelecimentoSelect.loadOptions()
    if (!(options instanceof Array && options.length > 1)) {
      estabelecimentoSelect.disable()
    }
    else {
      estabelecimentoSelect.config.markOptionAsSelectable = () => false;
      estabelecimentoSelect?.clear();
      estabelecimentoSelect.enable();
    }
  } else {
    estabelecimentoSelect.enable()
  }

  sindicatoPatronalSelect = new SelectWrapper('#sindicatoPatronal', {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: async () => {
      const options = obterParametrosParaRequisicaoDeSindicatos();
      return await sindicatoPatronalService.obterSelectPorUsuario(options);
    },
    markOptionAsSelectable: markOptionAsSelectable
  });

  sindicatoLaboralSelect = new SelectWrapper('#sindicatoLaboral', {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: async () => {
      const options = obterParametrosParaRequisicaoDeSindicatos();
      return await sindicatoLaboralService.obterSelectPorUsuario(options)
    },
    markOptionAsSelectable: markOptionAsSelectable
  });

  origemSelect = new SelectWrapper('#origem', {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: () => obterOrigens(),
  });

  repetirSelect = new SelectWrapper('#repetir_evento', {
    options: { placeholder: 'Selecione o tempo', multiple: false },
    onOpened: () => obterRepeticaoOpcoes(),
    onChange: () => {
      if (repetirSelect?.getValue() && repetirSelect?.getValue() > 1) {
        $("#row-validade-repeticao-evento").show();
      }
      else {
        $("#row-validade-repeticao-evento").hide();
      }
    }
  });

  lembreteSelect = new SelectWrapper('#lembrete_evento', {
    options: { placeholder: 'Selecione o tempo', multiple: false },
    onOpened: () => obterLembreteOpcoes()
  });

  periodo = new DatepickerrangeWrapper('#reservation');
}

function obterTipoEventos() {
  const tiposEventos = [
    {
      id: TiposEventos.AgendaEventos,
      description: TiposEventos.AgendaEventos
    },
    // {
    //   id: TiposEventos.AssembleiaPatronalReuniaoSindical,
    //   description: TiposEventos.AssembleiaPatronalReuniaoSindical
    // },
    {
      id: TiposEventos.Trintidio,
      description: "Indenização adicional que antecede a data-base (Trintídio)"
    },
    {
      id: TiposEventos.EventosClausulas,
      description: "Obrigações sindicais das cláusulas"
    },
    {
      id: TiposEventos.VencimentoDocumento,
      description: TiposEventos.VencimentoDocumento
    },
    {
      id: TiposEventos.VencimentoMandatoLaboral,
      description: TiposEventos.VencimentoMandatoLaboral
    },
    {
      id: TiposEventos.VencimentoMandatoPatronal,
      description: TiposEventos.VencimentoMandatoPatronal
    }
  ];

  return tiposEventos;
}

function obterRepeticaoOpcoes() {
  const repeticaoOpcoes = [
    {
      id: 1,
      description: "Não repetir"
    },
    {
      id: 2,
      description: "Toda semana"
    },
    {
      id: 3,
      description: "A cada 15 dias"
    },
    {
      id: 8,
      description: "Mensalmente"
    },
    {
      id: 4,
      description: "A cada 2 meses"
    },
    {
      id: 5,
      description: "A cada 3 meses"
    },
    {
      id: 6,
      description: "A cada 6 meses"
    },
    {
      id: 7,
      description: "Anualmente"
    },
  ]

  return repeticaoOpcoes;
}

function obterLembreteOpcoes() {
  const repeticaoOpcoes = [
    {
      id: 5 * 60,
      description: "5 minutos"
    },
    {
      id: 10 * 60,
      description: "10 minutos"
    },
    {
      id: 15 * 60,
      description: "15 minutos"
    },
    {
      id: 30 * 60,
      description: "30 minutos"
    },
    {
      id: 1 * 60 * 60,
      description: "1 hora"
    },
    {
      id: 2 * 60 * 60,
      description: "2 horas"
    },
    {
      id: 24 * 60 * 60,
      description: "1 dia"
    },
    {
      id: 2 * 24 * 60 * 60,
      description: "2 dias"
    },
    {
      id: 7 * 24 * 60 * 60,
      description: "1 semana"
    },
  ]

  return repeticaoOpcoes;
}

async function criarEvento() {
  const evento = {
    titulo: $("#titulo_evento").val(),
    dataHora: $("#data_hora_evento").val(),
    local: $("#local_evento").val(),
    comentarios: $("#comentarios_evento").val(),
    visivel: $("#visivel_evento").is(":checked"),
    notificarAntes: $("#lembrete_evento").val() ? Number($("#lembrete_evento").val()) : null,
    recorrencia: repetirSelect.getValue() ? Number(repetirSelect.getValue()) : null,
    validadeRecorrencia: $("#validade_repeticao_evento").val() ? $("#validade_repeticao_evento").val() : null
  }

  const result = await eventosCalendarioService.criarEventoUsuario(evento);

  if (result.isFailure()) {
    Notification.error({
      title: "Não foi possível cadastrar o evento"
    })
    return;
  }

  Notification.success({
    title: "Evento cadastrado com sucesso!"
  })
  
  limparFormCriarEvento();
}

function limparFormCriarEvento() {
  $("#titulo_evento").val(null);
  $("#data_hora_evento").val(null);
  $("#local_evento").val(null);
  $("#comentarios_evento").val(null);
  $("#visivel_evento").prop("checked", false);
  $("#lembrete_evento").val(null);
  repetirSelect?.clear();
  $("#validade_repeticao_evento").val(null);
  $("#row-validade-repeticao-evento").hide();
}

async function obterLocalidades() {
  const params = {
    gruposEconomicosIds: grupoEconomicoSelect?.getValue(),
    matrizesIds: matrizSelect?.getValue(),
    clientesUnidadesIds: estabelecimentoSelect?.getValue()
  }
  const municipios = await localizacaoService.obterSelectPorUsuario(params);
  const regioes = await localizacaoService.obterSelectRegioes(true);

  let localidades = municipios?.map((municipio) => ({ id: `municipio:${municipio.id}`, description: municipio.description })) ?? [];

  if (regioes?.length > 0) {
    localidades = [...regioes.map(regiao => ({ id: `uf:${regiao.description}`, description: regiao.description })), ...localidades];
  }

  return localidades;
}

function obterOrigens() {
  const origens = [
    {
      id: 1,
      description: 'Ineditta'
    },
    {
      id: 2,
      description: 'Cliente'
    }
  ];

  return origens;
}

function obterParametrosParaRequisicaoDeSindicatos() {
  const campoLocalidade = $("#localidade").val();
  const localizacoesIds = campoLocalidade.map(loc => {
    if (loc.includes('municipio')) {
      return Number(loc.split(':')[1]);
    }
    return null;
  }).filter(x => x !== null);

  const ufs = campoLocalidade.map(loc => {
    console.log(loc);
    if (loc.includes('uf')) {
      return loc.split(':')[1];
    }
    return null;
  }).filter(x => x !== null);

  const options = {
    gruposEconomicosIds: $("#grupo").val() ?? null,
    matrizesIds: $("#matriz").val() ?? null,
    clientesUnidadesIds: $("#unidade").val() ?? null,
    cnaesIds: $("#cnae").val() ?? null,
    localizacoesIds,
    ufs
  }

  return options;
}

async function carregarDatatable() {
  let tipoEventosSelecionados = tipoEventoSelect?.getValue();

  if (tipoEventosSelecionados instanceof Array && tipoEventosSelecionados.length > 0 && !tipoEventosSelecionados.find(t => t == TiposEventos.VencimentoDocumento)) {
    $("#calendario").hide();
    return;
  }

  $("#calendario").show();

  if (eventosTable) {
    await eventosTable.reload();
    return;
  }

  eventosTable = new DataTableWrapper('#eventosTb', {
    columns: [
      { data: 'data' },
      { data: 'nomeDocumento' },
      { data: 'origem' },
      { data: 'atividadesEconomicas' },
      { data: 'sindicatosLaborais' },
      { data: 'sindicatosPatronais' },
      { data: 'chaveReferenciaId' }
    ],
    ajax: async (params) => {
      let localidade = localidadeSelect.getValue();
      let localizacaoMunicipioId = null;
      let localizacaoEstadoUf = null;

      if (localidadeSelect.getValue().some(localidade => localidade.indexOf('municipio:') > -1)) {
        const municipios = localidadeSelect.getValue().filter(localidade => localidade.indexOf('municipio:') > -1).map(municipio => municipio.split(':')[1]);
        localizacaoMunicipioId = Array.isArray(municipios) ? municipios : [municipios];
      }

      if (localidadeSelect.getValue().some(localidade => localidade.indexOf('uf:') > -1)) {
        const ufs = localidadeSelect.getValue().filter(localidade => localidade.indexOf('uf:') > -1).map(value => value.split(':')[1]);
        localizacaoEstadoUf = Array.isArray(ufs) ? ufs : [ufs];
      }

      const nomesDocumentosIds = nomeDocumentoSelect?.getValue();
      const atividadesEconomicasIds = atividadeEconomicaSelect?.getValue();
      const gruposEconomicosIds = grupoEconomicoSelect?.getValue();
      const matrizesIds = matrizSelect?.getValue();
      const estabelecimentosIds = estabelecimentoSelect?.getValue();
      const sindicatosLaboraisIds = sindicatoLaboralSelect?.getValue();
      const sindicatosPatronaisIds = sindicatoPatronalSelect?.getValue();
      const dataInicial = periodo?.getBeginDate();
      const dataFinal = periodo?.getEndDate();
      const origem = origemSelect?.getValue();
      const gruposAssuntoIds = gruposAssuntoSelect?.getValue();
      const assuntosIds = assuntosSelect?.getValue();

      params = {
        ...params,
        nomesDocumentosIds,
        atividadesEconomicasIds,
        gruposEconomicosIds,
        matrizesIds,
        estabelecimentosIds,
        sindicatosLaboraisIds,
        sindicatosPatronaisIds,
        dataInicial,
        dataFinal,
        origem,
        gruposAssuntoIds,
        assuntosIds,
        municipiosIds: localizacaoMunicipioId,
        ufs: localizacaoEstadoUf
      };

      params.Columns = "data,nomeDocumento,origem,atividadesEconomicas,sindicatosLaborais,sindicatosPatronais";
      return await eventosCalendarioService.obterVencimentosDocumentosDatatable(params);
    },
    columnDefs: [
      {
        "targets": "_all",
        "defaultContent": ""
      }
    ],
    rowCallback: function (row, data) {
      let linksLaborais = [];
      let linksPatronais = [];

      if (data?.sindicatosLaborais instanceof Array) {
        linksLaborais = data.sindicatosLaborais.map(sindicatoLaboral => {
          let linkLaboral = $("<a>")
            .attr("data-id", sindicatoLaboral.id) //data?.idSindicatoLaboral
            .addClass("cursor-pointer")
            .html(sindicatoLaboral.sigla);

          linkLaboral.on("click", function () {
            const id = $(this).attr("data-id");
            $("#sind-id-input").val(id);
            $("#tipo-sind-input").val("laboral");
            $("#openInfoSindModalBtn").trigger("click");
          });

          return linkLaboral;
        });
      }

      if(data?.sindicatosPatronais instanceof Array) {
        linksPatronais = data.sindicatosPatronais.map(sindicatoPatronal => {
          let linkPatronal = $("<a>")
            .attr("data-id", sindicatoPatronal.id) //data?.idSindicatoLaboral
            .addClass("cursor-pointer")
            .html(sindicatoPatronal.sigla);

          linkPatronal.on("click", function () {
            const id = $(this).attr("data-id");
            $("#sind-id-input").val(id);
            $("#tipo-sind-input").val("patronal");
            $("#openInfoSindModalBtn").trigger("click");
          });

          return linkPatronal;
        });
      }

      $("td:eq(4)", row).html("");
      linksLaborais.forEach((link, index) => {
        $("td:eq(4)", row).append(link);
        if (index < linksLaborais.length - 1) {
          $("td:eq(4)", row).append(",");
        }
      })

      $("td:eq(5)", row).html("");
      linksPatronais.forEach((link, index) => {
        $("td:eq(5)", row).append(link);
        if (index < linksPatronais.length - 1) {
          $("td:eq(5)", row).append(",");
        }
      })

      const id = data?.chaveReferenciaId

      let btn = a({
        className: 'btn-update'
      })
        .attr("data-id", id)
        .html(i({
          className: 'fa fa-file-text'
        }))

      btn.on("click", function () {
        documentoDetalhesSelecionadoId = id
        $('#vencimentoDocumentoBtn').trigger('click')
      })

      $("td:eq(6)", row).html(btn);

      $("td:eq(0)", row).html(DataTableWrapper.formatDate(data?.data));

      if (data?.atividadesEconomicas instanceof Array) {
        $("td:eq(3)", row).html(data.atividadesEconomicas.map(x => x.subclasse).join('; '));
      }

      (Array.isArray(data?.siglasSindicatosLaborais)) && $("td:eq(4)", row).html(data?.siglasSindicatosLaborais?.join(', '));
      (Array.isArray(data?.siglasSindicatosPatronais)) && $("td:eq(5)", row).html(data?.siglasSindicatosPatronais?.join(', '));
    },
  });

  await eventosTable.initialize();
}

async function carregarAgendaEventos() {
  let tipoEventosSelecionados = tipoEventoSelect?.getValue();
  let origensSelecionadas = origemSelect?.getValue();

  if (tipoEventosSelecionados instanceof Array && tipoEventosSelecionados.length > 0 && !tipoEventosSelecionados.find(t => t == TiposEventos.AgendaEventos)) {
    $("#agenda_eventos_panel").hide();
    return;
  }

  if (origensSelecionadas instanceof Array && origensSelecionadas.length > 0 && !origensSelecionadas.find(o => o == 2)) {
    $("#agenda_eventos_panel").hide();
    return;
  }

  $("#agenda_eventos_panel").show();

  if (agendaEventosTable) {
    await agendaEventosTable.reload();
    return;
  }

  agendaEventosTable = new DataTableWrapper('#agendaEventosTb', {
    columns: [
      { data: 'data' },
      { data: 'titulo' },
      {
        data: 'recorrencia', render: (data) => {
          if (data) {
            return obterRepeticaoOpcoes().find(r => r.id == data).description;
          }
          return data;
        }
      },
      { data: 'validadeRecorrencia' },
      { data: 'local' },
      { data: 'comentario' },
      {
        data: 'visivel', render: (data) => {
          if (data) return "Sim"
          return "Não"
        }
      }
    ],
    ajax: async (params) => {
      const dataInicial = periodo?.getBeginDate();
      const dataFinal = periodo?.getEndDate();

      const customParams = {
        ...params,
        dataInicial,
        dataFinal,
      }
  
      return await eventosCalendarioService.obterAgendaEvento(customParams);
    },
    columnDefs: [
      {
        "targets": "_all",
        "defaultContent": ""
      }
    ],
    rowCallback: function (row, data) {
      $("td:eq(0)", row).html(DataTableWrapper.formatDateTime(data?.data));
      $("td:eq(3)", row).html(data?.validadeRecorrencia ? DataTableWrapper.formatDateTime(data?.validadeRecorrencia) : "");
    },
  });

  await agendaEventosTable.initialize();
}

async function carregarVencimentoAssembleiaReuniao() {
  
  let tipoEventosSelecionados = tipoEventoSelect?.getValue();

  if (tipoEventosSelecionados instanceof Array && tipoEventosSelecionados.length > 0 && !tipoEventosSelecionados.find(t => t == TiposEventos.AssembleiaPatronalReuniaoSindical)) {
    $("#assembleia_reuniao_panel").hide();
    return;
  }

  $("#assembleia_reuniao_panel").show();

  if (assembleiaReuniaoTb) {
    await assembleiaReuniaoTb.reload();
    return;
  }

  assembleiaReuniaoTb = new DataTableWrapper('#assembleiaReuniaoTb', {
    columns: [
      { data: 'dataHora' },
      { data: 'tipoEvento' },
      { data: 'sindicatosLaborais', render: data => {
        const siglas = data.map(s => s.sigla)

        return siglas.join(', ')
      } },
      { data: 'sindicatosPatronais', render: data => {
        const siglas = data.map(s => s.sigla)

        return siglas.join(', ')
      } },
      { data: 'atividadesEconomicas' },
      { data: 'dataBaseNegociacao' },
      { data: 'nomeDocumento' },
      { data: 'fase' },
      { data: 'chaveReferenciaId'}
    ],
    ajax: async (params) => {
      params.Columns = 'dataHora,tipoEvento,siglasSindicatosLaborais,siglasSindicatosPatronais,atividadesEconomicas,dataBaseNegociacao,nomeDocumento,fase';
      const customParams = obterParamsRequestVencimentosMandatosSindicais(params);
      return await eventosCalendarioService.obterAssembleiaReuniaoDatatable(customParams);
    },
    columnDefs: [
      {
        "targets": "_all",
        "defaultContent": ""
      }
    ],
    rowCallback: function (row, data) {
      $("td:eq(0)", row).html(DataTableWrapper.formatDateTime(data?.dataHora));

      const id = data?.chaveReferenciaId

      let btn = $("<button>")
        .addClass("btn-update btn-view-grid")
        .attr("type", "button")
        .attr("data-id", id)
        .html(i({
          className: 'fa fa-file-text'
        }))
        
      if (data?.tipoEvento == "Assembleia patronal com as empresas"){
        btn.on("click", function () {
          assembleiaSelecionadaId = id
          assembleiaSelecionada = data
          $('#assembleiaBtn').trigger('click')
        })
      }
      else {
        btn.on("click", function () {
          reuniaoSelecionadaId = id
          reuniaoSelecionada = data
          $('#reuniaoBtn').trigger('click')
        })
      }
  
      $("td:eq(8)", row).html(btn);
    },
  });

  await assembleiaReuniaoTb.initialize();
}

async function carregarVencimentoMandatosPatronais() {
  let tipoEventosSelecionados = tipoEventoSelect?.getValue();

  if (tipoEventosSelecionados instanceof Array && tipoEventosSelecionados.length > 0 && !tipoEventosSelecionados.find(t => t == TiposEventos.VencimentoMandatoPatronal)) {
    $("#vencimento_mandato_patronal_panel").hide();
    return;
  }

  $("#vencimento_mandato_patronal_panel").show();

  if (vencimentosMandatosSindicaisPatronaisTb) {
    await vencimentosMandatosSindicaisPatronaisTb.reload();
    return;
  }

  vencimentosMandatosSindicaisPatronaisTb = new DataTableWrapper('#vencimentosMandatosSindicaisPatronaisTb', {
    columns: [
      { data: 'data' },
      { data: 'origem' },
      { data: 'siglaSindicato' }
    ],
    ajax: async (params) => {
      const customParams = obterParamsRequestVencimentosMandatosSindicais(params);
      return await eventosCalendarioService.obterVencimentosMandatosPatronaisDatatable(customParams);
    },
    columnDefs: [
      {
        "targets": "_all",
        "defaultContent": ""
      }
    ],
    rowCallback: function (row, data) {
      let link = $("<a>")
        .attr("data-id", data?.sindId) //data?.idSindicatoLaboral
        .addClass("cursor-pointer")
        .html(data?.siglaSindicato);
      link.on("click", function () {
        const id = $(this).attr("data-id");
        $("#sind-id-input").val(id);
        $("#tipo-sind-input").val("patronal");
        $("#openInfoSindModalBtn").trigger("click");
      });
      $("td:eq(2)", row).html(link);

      $("td:eq(0)", row).html(DataTableWrapper.formatDate(data?.data));

      (Array.isArray(data?.siglaSindicato)) && $("td:eq(4)", row).html(data?.siglaSindicato?.join(', '));
    },
  });

  await vencimentosMandatosSindicaisPatronaisTb.initialize();
}

async function carregarVencimentoMandatosLaborais() {
  let tipoEventosSelecionados = tipoEventoSelect?.getValue();

  if (tipoEventosSelecionados instanceof Array && tipoEventosSelecionados.length > 0 && !tipoEventosSelecionados.find(t => t == TiposEventos.VencimentoMandatoLaboral)) {
    $("#vencimento_mandato_laboral_panel").hide();
    return;
  }

  $("#vencimento_mandato_laboral_panel").show();

  if (vencimentosMandatosSindicaisLaboraisTb) {
    await vencimentosMandatosSindicaisLaboraisTb.reload();
    return;
  }

  vencimentosMandatosSindicaisLaboraisTb = new DataTableWrapper('#vencimentosMandatosSindicaisLaboraisTb', {
    columns: [
      { data: 'data' },
      { data: 'origem' },
      { data: 'siglaSindicato' }
    ],
    ajax: async (params) => {
      const customParams = obterParamsRequestVencimentosMandatosSindicais(params);
      return await eventosCalendarioService.obterVencimentosMandatosLaboraisDatatable(customParams);
    },
    columnDefs: [
      {
        "targets": "_all",
        "defaultContent": ""
      }
    ],
    rowCallback: function (row, data) {
      let link = $("<a>")
        .attr("data-id", data?.sindId) //data?.idSindicatoLaboral
        .addClass("cursor-pointer")
        .html(data?.siglaSindicato);
      link.on("click", function () {
        const id = $(this).attr("data-id");
        $("#sind-id-input").val(id);
        $("#tipo-sind-input").val("laboral");
        $("#openInfoSindModalBtn").trigger("click");
      });
      $("td:eq(2)", row).html(link);

      $("td:eq(0)", row).html(DataTableWrapper.formatDate(data?.data));

      (Array.isArray(data?.siglaSindicato)) && $("td:eq(4)", row).html(data?.siglaSindicato?.join(', '));
    },
  });

  await vencimentosMandatosSindicaisLaboraisTb.initialize();
}

async function carregarTrintidios() {
  let tipoEventosSelecionados = tipoEventoSelect?.getValue();

  if (tipoEventosSelecionados instanceof Array && tipoEventosSelecionados.length > 0 && !tipoEventosSelecionados.find(t => t == TiposEventos.Trintidio)) {
    $("#trintidio_panel").hide();
    return;
  }

  $("#trintidio_panel").show();

  if (trintidioTb) {
    await trintidioTb.reload();
    return;
  }

  trintidioTb = new DataTableWrapper('#trintidioTb', {
    columns: [
      { data: 'data' },
      { data: 'nomeDocumento' },
      { data: 'atividadesEconomicas' },
      { data: 'siglasSindicatosLaborais' },
      { data: 'siglasSindicatosPatronais' },
      { data: 'origem' },
      { data: 'dataBase' },
      { title: 'Vigência do evento' },
      { data: 'chaveReferenciaId' }
    ],
    ajax: async (params) => {
      const customParams = obterParamsRequestVencimentosMandatosSindicais(params);
      return await eventosCalendarioService.obterTrintidioDatatable(customParams);
    },
    columnDefs: [
      {
        "targets": "_all",
        "defaultContent": ""
      }
    ],
    rowCallback: function (row, data) {
      let linksLaborais = [];
      let linksPatronais = [];

      if (data?.sindicatosLaborais instanceof Array) {
        linksLaborais = data.sindicatosLaborais.map(sindicatoLaboral => {
          let linkLaboral = $("<a>")
            .attr("data-id", sindicatoLaboral.id) //data?.idSindicatoLaboral
            .addClass("cursor-pointer")
            .html(sindicatoLaboral.sigla);

          linkLaboral.on("click", function () {
            const id = $(this).attr("data-id");
            $("#sind-id-input").val(id);
            $("#tipo-sind-input").val("laboral");
            $("#openInfoSindModalBtn").trigger("click");
          });

          return linkLaboral;
        });
      }

      if(data?.sindicatosPatronais instanceof Array) {
        linksPatronais = data.sindicatosPatronais.map(sindicatoPatronal => {
          let linkPatronal = $("<a>")
            .attr("data-id", sindicatoPatronal.id) //data?.idSindicatoLaboral
            .addClass("cursor-pointer")
            .html(sindicatoPatronal.sigla);

          linkPatronal.on("click", function () {
            const id = $(this).attr("data-id");
            $("#sind-id-input").val(id);
            $("#tipo-sind-input").val("patronal");
            $("#openInfoSindModalBtn").trigger("click");
          });

          return linkPatronal;
        });
      }

      $("td:eq(3)", row).html("");
      linksLaborais.forEach((link, index) => {
        $("td:eq(3)", row).append(link);
        if (index < linksLaborais.length - 1) {
          $("td:eq(3)", row).append(",");
        }
      })

      $("td:eq(4)", row).html("");
      linksPatronais.forEach((link, index) => {
        $("td:eq(4)", row).append(link);
        if (index < linksPatronais.length - 1) {
          $("td:eq(4)", row).append(",");
        }
      })

      const id = data?.chaveReferenciaId

      let btn = a({
        className: 'btn-update'
      })
        .attr("data-id", id)
        .html(i({
          className: 'fa fa-file-text'
        }))

      btn.on("click", function () {
        documentoDetalhesSelecionadoId = id
        $('#trintidioBtn').trigger('click')
      })

      $("td:eq(8)", row).html(btn);

      $("td:eq(0)", row).html(DataTableWrapper.formatDate(data?.data));
      $("td:eq(6)", row).html(DataTableWrapper.formatDate(data?.dataBase));

      if (data?.validadeInicial) {
        let vigencia = DataTableWrapper.formatDate(data?.validadeInicial);

        vigencia += data?.validadeFinal ? ` até ${DataTableWrapper.formatDate(data?.validadeFinal)}` : ' até (não informado)';

        $("td:eq(7)", row).html(vigencia);
      }

      (Array.isArray(data?.siglasSindicatos)) && $("td:eq(4)", row).html(data?.siglasSindicatos?.join(', '));
    },
  });

  await trintidioTb.initialize();
}

async function carregarEventosDeClausulas() {
  let tipoEventosSelecionados = tipoEventoSelect?.getValue();

  if (tipoEventosSelecionados instanceof Array && tipoEventosSelecionados.length > 0 && !tipoEventosSelecionados.find(t => t == TiposEventos.EventosClausulas)) {
    $("#eventos_clausulas_panel").hide();
    return;
  }

  $("#eventos_clausulas_panel").show();

  if (eventosDeClausulaTb) {
    await eventosDeClausulaTb.reload();
    return;
  }

  eventosDeClausulaTb = new DataTableWrapper('#eventosDeClausulasTb', {
    columns: [
      { data: 'data' },
      { data: 'nomeDocumento' },
      { data: 'atividadesEconomicas' },
      { data: 'siglasSindicatosLaborais' },
      { data: 'siglasSindicatosPatronais' },
      { data: 'nomeEvento' },
      { data: 'grupoClausulas' },
      { data: 'nomeClausula' },
      { data: 'origem' },
      { data: 'id', title: '' }
    ],
    ajax: async (params) => {
      const customParams = obterParamsRequestVencimentosMandatosSindicais(params);
      return await eventosCalendarioService.obterEventosClausulasDatatable(customParams);
    },
    columnDefs: [
      {
        "targets": "_all",
        "defaultContent": ""
      }
    ],
    rowCallback: function (row, data) {
      let link = $("<a>")
        .attr("data-id", data?.sindicatoLaboralId) //data?.idSindicatoLaboral
        .attr("href", "#")
        .html(data?.siglasSindicatosLaborais);
      link.on("click", function () {
        const id = $(this).attr("data-id");
        $("#sind-id-input").val(id);
        $("#tipo-sind-input").val("laboral");
        $("#openInfoSindModalBtn").trigger("click");
      });
      $("td:eq(3)", row).html(link);

      let linkPatronal = $("<a>")
        .attr("data-id", data?.sindicatoPatronalId) //data?.idSindicatoLaboral
        .attr("href", "#")
        .html(data?.siglasSindicatosPatronais);
      linkPatronal.on("click", function () {
        const id = $(this).attr("data-id");
        $("#sind-id-input").val(id);
        $("#tipo-sind-input").val("patronal");
        $("#openInfoSindModalBtn").trigger("click");
      });
      $("td:eq(4)", row).html(linkPatronal);

      const id = data?.clausulaGeralId

      let btn = a({
        className: 'btn-update'
      })
        .attr("data-id", id)
        .html(i({
          className: 'fa fa-file-text'
        }))

      btn.on("click", function () {
        eventoClausulaSelecionadoId = id
        $('#detalheClausulaBtn').trigger('click')
      })

      $("td:eq(9)", row).html(btn)

      $("td:eq(0)", row).html(DataTableWrapper.formatDate(data?.data));

      (Array.isArray(data?.siglasSindicatosLaborais)) && $("td:eq(3)", row).html(data?.siglasSindicatosLaborais?.join(', '));
      (Array.isArray(data?.siglasSindicatosPatronais)) && $("td:eq(4)", row).html(data?.siglasSindicatosPatronais?.join(', '));
    },
  });

  await eventosDeClausulaTb.initialize();
}

async function carregarDatatables() {
  await carregarDatatable();
  await carregarVencimentoMandatosPatronais();
  await carregarVencimentoMandatosLaborais();
  await carregarTrintidios();
  await carregarEventosDeClausulas();
  await carregarAgendaEventos();
  await carregarVencimentoAssembleiaReuniao();
}

function limparFiltros(dadosPessoais) {
  const isIneditta = dadosPessoais?.nivel == UsuarioNivel.Ineditta;

  localidadeSelect?.clear();
  gruposAssuntoSelect?.clear();
  assuntosSelect?.clear();
  nomeDocumentoSelect?.clear();
  atividadeEconomicaSelect?.clear();
  isIneditta && grupoEconomicoSelect?.clear();
  matrizSelect?.isEnable() && matrizSelect?.clear();
  estabelecimentoSelect?.isEnable() && estabelecimentoSelect?.clear();
  sindicatoPatronalSelect?.clear();
  sindicatoLaboralSelect?.clear();
  origemSelect?.clear();
  periodo?.clear();

  filtros = {};

  $("#calendario").hide();
  $("#agenda_eventos_panel").hide();
  $("#vencimento_mandato_patronal_panel").hide();
  $("#vencimento_mandato_laboral_panel").hide();
  $("#trintidio_panel").hide();
  $("#eventos_clausulas_panel").hide();
}

function limparModalAdicionarEvento() {
  $("titulo_evento").val("");

}

function obterParamsRequestVencimentosMandatosSindicais(params) {
  let localidade = localidadeSelect.getValue();
  let localizacaoMunicipioId = null;
  let localizacaoEstadoUf = null;

  if (localidadeSelect.getValue().some(localidade => localidade.indexOf('municipio:') > -1)) {
    const municipios = localidadeSelect.getValue().filter(localidade => localidade.indexOf('municipio:') > -1).map(municipio => municipio.split(':')[1]);
    localizacaoMunicipioId = Array.isArray(municipios) ? municipios : [municipios];
  }

  if (localidadeSelect.getValue().some(localidade => localidade.indexOf('uf:') > -1)) {
    const ufs = localidadeSelect.getValue().filter(localidade => localidade.indexOf('uf:') > -1).map(value => value.split(':')[1]);
    localizacaoEstadoUf = Array.isArray(ufs) ? ufs : [ufs];
  }

  const nomesDocumentosIds = nomeDocumentoSelect?.getValue();
  const atividadesEconomicasIds = atividadeEconomicaSelect?.getValue();
  const gruposEconomicosIds = grupoEconomicoSelect?.getValue();
  const matrizesIds = matrizSelect?.getValue();
  const estabelecimentosIds = estabelecimentoSelect?.getValue();
  const sindicatosLaboraisIds = sindicatoLaboralSelect?.getValue();
  const sindicatosPatronaisIds = sindicatoPatronalSelect?.getValue();
  const dataInicial = periodo?.getBeginDate();
  const dataFinal = periodo?.getEndDate();
  const origem = origemSelect?.getValue();
  const gruposAssuntoIds = gruposAssuntoSelect?.getValue();
  const assuntosIds = assuntosSelect?.getValue();

  const customParams = {
    ...params,
    nomesDocumentosIds,
    atividadesEconomicasIds,
    gruposEconomicosIds,
    matrizesIds,
    estabelecimentosIds,
    sindicatosLaboraisIds,
    sindicatosPatronaisIds,
    dataInicial,
    dataFinal,
    origem,
    gruposAssuntoIds,
    assuntosIds,
    municipiosIds: localizacaoMunicipioId,
    ufs: localizacaoEstadoUf
  };

  return customParams;
}

function configurarModal() {
  $("#infoSindForm").on('submit', (e) => e.preventDefault());

  const pageCtn = document.getElementById("pageCtn");

  // DetalhesEventoClausula
  const detalheClausulaHidden = document.getElementById('detalheClausulaModalHidden')
  const detalheClausulaContent = document.getElementById('detalheClausulaModalContent')

  // DetalhesEventoVencimentoDocumento
  const vencimentoDocumentoHidden = document.getElementById('vencimentoDocumentoModalHidden')
  const vencimentoDocumentoContent = document.getElementById('vencimentoDocumentoModalContent')

  // DetalhesEventoTrintidio
  const trintidioHidden = document.getElementById('trintidioModalHidden')
  const trintidioContent = document.getElementById('trintidioModalContent')

  // Detalhes Assembleia Patronal
  const assembleiaHidden = document.getElementById('assembleiaModalHidden')
  const assembleiaContent = document.getElementById('assembleiaModalContent')

  // Detalhes Reunião Sindical
  const reuniaoHidden = document.getElementById('reuniaoModalHidden');
  const reuniaoContent = document.getElementById('reuniaoModalContent');

  // Adicionar Novo Evento Modal
  const novoEventoHidden = document.getElementById('novoEventoModalHidden');
  const novoEventoContent = document.getElementById('novoEventoModalContent');

  const modalClausula = document.getElementById("clausulaModalHidden");
  const contentClausula = document.getElementById("clausulaModalHiddenContent");

  const modalsConfig = [
    {
      id: 'reuniaoModal',
      modal_hidden: reuniaoHidden,
      content: reuniaoContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarDetalhesReuniao();
      },
      onClose: () => null
    },
    {
      id: 'assembleiaModal',
      modal_hidden: assembleiaHidden,
      content: assembleiaContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarDetalhesAssembleia();
      },
      onClose: () => null
    },
    {
      id: 'detalheClausulaModal',
      modal_hidden: detalheClausulaHidden,
      content: detalheClausulaContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarTextoClausula()
      },
      onClose: () => null
    },
    {
      id: "clausulaModal",
      modal_hidden: modalClausula,
      content: contentClausula,
      btnsConfigs: [],
      onOpen: async () => {
        const result = await clausulaService.obterPorId([eventoClausulaSelecionadoId]);
        const comentarios = await clausulaService.obterComentariosPorId([
          eventoClausulaSelecionadoId
        ]);
        if (result.isFailure() || !result.value || result.value?.length == 0) {
          NotificationService.error({title: "Não foi possível carregar os detalhes da claúsula", message: result.error});
          closeModal({id: "clausulaModal"});
          return;
        }
        preencherModalClausula(result.value, comentarios.value);
        await carregarInformacoesModal();
        $("#informacoes_adicionais .form-control").attr("disabled", true);
      },
      onClose: () => {
        eventoClausulaSelecionadoId = null;
        $("#clausulaModalContainer").html(null);
        $("#informacoes_adicionais").html(null);
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
          maxWidth: "1200px",
          width: "100%",
        },
      },
    },
    {
      id: 'vencimentoDocumentoModal',
      modal_hidden: vencimentoDocumentoHidden,
      content: vencimentoDocumentoContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarDetalhesVencimentoDocumento()
      },
      onClose: () => null
    },
    {
      id: 'trintidioModal',
      modal_hidden: trintidioHidden,
      content: trintidioContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarDetalhesTrintidio()
      },
      onClose: () => null
    },
    {
      id: 'novoEventoModal',
      modal_hidden: novoEventoHidden,
      content: novoEventoContent,
      btnsConfigs: [],
      onClose: () => null
    }
  ];

  renderizarModal(pageCtn, modalsConfig);

  configurarInformacaoSindicatoService();
}

function preencherModalClausula(dataArray, comentariosArray) {
  dadosClausulasPorId = [];
  $(".modal-backdrop").removeClass("in");
  const clausulasContainer = $("#clausulaModalContainer");
  dataArray.forEach((data) => {
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
      $("<tr>").append($("<th>").text("Sindicato Laboral / UF"))
    );
    const sindLaboralTbody = $("<tbody>");
    data.sindLaboral.forEach((sind) => {
      const sindRow = $("<tr>").append(
        $("<td>").text(`${sind.sigla} / ${sind.uf}`)
      );
      sindLaboralTbody.append(sindRow);
    });
    sindLaboralTb.append([sindLaboralThead, sindLaboralTbody]);
    const sindPatronalTb = $("<table>").addClass(
      "table table-striped table-bordered"
    );
    const sindPatronalThead = $("<thead>").append(
      $("<tr>").append($("<th>").text("Sindicato Patronal / UF"))
    );
    const sindPatronalTbody = $("<tbody>");
    data.sindPatronal.forEach((sind) => {
      const sindRow = $("<tr>").append(
        $("<td>").text(`${sind.sigla} / ${sind.uf}`)
      );
      sindPatronalTbody.append(sindRow);
    });
    sindPatronalTb.append([sindPatronalThead, sindPatronalTbody]);

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
    clauTbodyRow.append($("<td>").text(convertDate(data.dataAprovacao)));
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
    datesTbodyRow.append($("<td>").text(data.cnae[0]?.subclasse));
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

    painelBody.append(mainRow);
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
    const textoClausulaButtons = $("<div>")
      .attr("style", "margin-bottom: 1rem; display: flex; gap: 10px")
      .addClass("clausula_text_toolbar")
      .append([copyBtn]);

    const textoClausula = $("<p>")
      .attr("style", "text-align: justify; white-space: pre-line")
      .text(data.textoClausula)

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
    comentariosArray
      .filter((comm) => comm.idClausula == data.id)
      .forEach((comentario) => {
        const commRow = $("<tr>");
        commRow.append($("<td>").text(comentario.nomeUsuario));
        commRow.append(
          $("<td>").text(convertDateTime(comentario.dataRegistro))
        );
        commRow.append($("<td>").text(comentario.etiqueta));
        commRow.append($("<td>").text(comentario.comentario));
        comentariosTbody.append(commRow);
      });

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

    const unidadesData = data.unidade.filter((uni) => {
      const deveAparecer =  grupoEconomicoSelect.getValue()?.some(g => Number(g) === uni.g);
      return deveAparecer;
    });

    const estabelecimentosDataTable = $(
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

function convertDate(date) {
  if (!date) {
    return "";
  }
  return moment(date, "YYYY-MM-DD").format("DD/MM/YYYY");
}

async function carregarDetalhesAssembleia() {
  const INDICE_TIPO_ASSEMBLEIA = 10;
  const INDICE_ENDERECO_ASSEMBLEIA = 12;
  const INDICE_COMENTARIO_ASSEMBLEIA = 13;
  const INDICE_DATA_ASSEMBLEIA = 9;
  const INDICE_HORA_ASSEMBLEIA = 11;

  const result = await acompanhamentoCctService.obterPerguntasRespostasFases(assembleiaSelecionadaId);

  if (result.isFailure()){
    NotificationService.error({title: "Não foi possível carregar os dados da assembléia.", message: result.error});
    return;
  }

  if (result.value == null || (result.value instanceof Array && result.value.length == 0)){
    NotificationService.error({title: "Não foram encontrados dados do acompanhamento selecionado"});
    return;
  }

  let infosAcompanhamentos = result.value?.map(info => {
    return {
      ...info, 
      respostas: JSON.parse(info.respostas)
    } 
  });

  infosAcompanhamentos = infosAcompanhamentos.filter(r => {
    if (!r.respostas[INDICE_DATA_ASSEMBLEIA]) return false;
    const dataHoraString = r.respostas[INDICE_DATA_ASSEMBLEIA] + " " + r.respostas[INDICE_HORA_ASSEMBLEIA];
    const dataHoraAssembleiaSelecionada = new Date(assembleiaSelecionada?.dataHora).toISOString();
    const dataHoraResposta = dataHoraString !== " " && new Date(dataHoraString).toISOString(); 
    return r.fase == assembleiaSelecionada?.fase && dataHoraAssembleiaSelecionada == dataHoraResposta
  })

  const ultimoIndice = infosAcompanhamentos.length - 1;
  const respostas = infosAcompanhamentos[ultimoIndice].respostas;

  $("#tipo_assembleia_patronal").html("<strong>Tipo de assembléia patronal: </strong> " + respostas[INDICE_TIPO_ASSEMBLEIA]);
  $("#data_hora_assembleia_patronal").html("<strong>Data e hora da assembléia patronal: </strong> " + DateFormatter.dayMonthYear(respostas[INDICE_DATA_ASSEMBLEIA]) + (respostas[INDICE_HORA_ASSEMBLEIA] ? " " + respostas[INDICE_HORA_ASSEMBLEIA] : ""));
  $("#endereco_assembleia_patronal").html("<strong>Endereço da assembleia patronal: </strong> " + respostas[INDICE_ENDERECO_ASSEMBLEIA]);
  $("#comentario_assembleia_patronal").html("<strong>Comentários da assembleia patronal: </strong> " + respostas[INDICE_COMENTARIO_ASSEMBLEIA]);
}

async function carregarDetalhesReuniao() {
  const INDICE_TIPO_REUNIAO = 15;
  const INDICE_DATA_REUNIAO = 16;
  const INDICE_HORA_REUNIAO = 17;

  const result = await acompanhamentoCctService.obterPerguntasRespostasFases(reuniaoSelecionadaId);

  if (result.isFailure()){
    NotificationService.error({title: "Não foi possível carregar os dados da reunião.", message: result.error});
    return;
  }

  if (result.value == null || (result.value instanceof Array && result.value.length == 0)){
    NotificationService.error({title: "Não foram encontrados dados do acompanhamento selecionado"});
    return;
  }

  let infosAcompanhamentos = result.value?.map(info => {
    return {
      ...info, 
      respostas: JSON.parse(info.respostas)
    } 
  });

  infosAcompanhamentos = infosAcompanhamentos.filter(r => {
    const dataHoraString = r.respostas[INDICE_DATA_REUNIAO] + " " + r.respostas[INDICE_HORA_REUNIAO];
    const dataHoraReuniaoSelecionada = new Date(reuniaoSelecionada?.dataHora).toISOString();
    const dataHoraResposta = dataHoraString !== " " && new Date(dataHoraString).toISOString(); 
    return r.fase == reuniaoSelecionada?.fase && dataHoraReuniaoSelecionada == dataHoraResposta
  })

  const ultimoIndice = infosAcompanhamentos.length - 1;
  const respostas = infosAcompanhamentos[ultimoIndice].respostas;

  $("#tipo_reuniao").html("<strong>Tipo de assembléia patronal: </strong> " + respostas[INDICE_TIPO_REUNIAO]);
  $("#data_hora_reuniao").html("<strong>Data e hora da assembléia patronal: </strong> " + DateFormatter.dayMonthYear(respostas[INDICE_DATA_REUNIAO]) + (respostas[INDICE_HORA_REUNIAO] ? " " + respostas[INDICE_HORA_REUNIAO] : ""));
}

async function carregarTextoClausula() {
  let result = await eventosCalendarioService.obterTextoClausulaPorEventoId(eventoClausulaSelecionadoId);
  if (result.value == null) {
    $("#textoClausula").html("Nenhum texto encontrado para a cláusula");
    return;
  }

  let [textoClausula] = result.value;
  if (textoClausula == null) {
    $("#textoClausula").html("Nenhum texto encontrado para a cláusula");
    return;
  }

  $("#textoClausula").html(textoClausula);
}

async function carregarDetalhesDocumento() {
  let result = await docSindService.obterDocSindPorId(documentoDetalhesSelecionadoId ?? 0);
  
  if (result.isFailure()){
    NotificationService.error({title: "Não foi possível carregar os detalhes do evento", message: result.error});
    return Result.failure();
  }

  let [documentoSindical] = result.value;

  documentoSindical.clienteEstabelecimento = grupoEconomicoSelect?.getValue()?.length > 0 ? 
    documentoSindical.clienteEstabelecimento?.filter(ce => grupoEconomicoSelect?.getValue()?.some(g => Number(g) == ce.g)) : 
    documentoSindical.clienteEstabelecimento
  
  documentoSindical.clienteUnidades = documentoSindical.clienteUnidades?.filter(u => documentoSindical.clienteEstabelecimento instanceof Array && documentoSindical.clienteEstabelecimento.some(ce => ce.u == u.id));
  
  let nomeDocumento = "<strong>Nome Documento: </strong>" + documentoSindical?.nome;
  let dataInicial = "<strong>Vigência Inicial: </strong>" + DateFormatter.dayMonthYear(documentoSindical?.validadeInicial);
  let dataFinal = "<strong>Vigência Final: </strong>" + DateFormatter.dayMonthYear(documentoSindical?.validadeFinal);
  let abrangencias = "<strong>Abrangências: </strong>" + documentoSindical?.abrangencia.map(ds => ds.municipio + "/" + ds.uf).join(", ");
  let estabelecimentos = "<strong>Estabelecimentos: </strong><ul>" + 
    documentoSindical?.clienteUnidades?.map(u => "<li>" + 
      "<strong>Nome: </strong>" + u.nome + " | " + 
      "<strong>CNPJ: </strong>" + u.cnpj?.value?.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5') + 
      (u.codigoSindicatoCliente ? " | <strong>Código Sindicato cliente: </strong>" + u.codigoSindicatoCliente : "") + 
    "</li>").join("") + "</ul>";
  
  return Result.success({
    dataInicial,
    dataFinal,
    abrangencias,
    estabelecimentos,
    nomeDocumento
  })
}

async function carregarDetalhesVencimentoDocumento() {
  const result = await carregarDetalhesDocumento();
  
  if (result.isFailure()){
    NotificationService.error({title: "Não foi possível carregar os detalhes do evento", message: result.error});
    return;
  }

  const {dataInicial, dataFinal, abrangencias, estabelecimentos, nomeDocumento} = result.value;

  $("#nome_documento_detalhes").html(nomeDocumento);
  $("#vigencia_inicial_detalhes").html(dataInicial);
  $("#vigencia_final_detalhes").html(dataFinal);
  $("#abrangencia_detalhes").html(abrangencias);
  $("#estabelecimento_detalhes").html(estabelecimentos);
}

async function carregarDetalhesTrintidio() {
  const result = await carregarDetalhesDocumento();
  
  if (result.isFailure()){
    NotificationService.error({title: "Não foi possível carregar os detalhes do evento", message: result.error});
    return;
  }

  const {dataInicial, dataFinal, abrangencias, estabelecimentos, nomeDocumento} = result.value;

  $("#nome_documento_detalhes_trintidio").html(nomeDocumento);
  $("#vigencia_inicial_detalhes_trintidio").html(dataInicial);
  $("#vigencia_final_detalhes_trintidio").html(dataFinal);
  $("#abrangencia_detalhes_trintidio").html(abrangencias);
  $("#estabelecimento_detalhes_trintidio").html(estabelecimentos);
}

async function carregarInformacoesModal() {
  await obterClausulasPorId()
  montarTabela()
}

async function obterClausulasPorId() {
  const result = await clausulaGeralService.obterInformacoesAdicionaisPorClausulaId(eventoClausulaSelecionadoId)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao obter clausulas do documento', message: result.error })
  }

  informacoesAdicionais = [result.value]
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
          const { descricao, codigo, tipo, dado, id } = infoItem

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

  function procurarInformacaoExistente({ id, tipo }) {
    let dado = null

    informacoesAdicionaisCliente && informacoesAdicionaisCliente.informacoesAdicionais.map(({ clausulaGeralEstruturaId, valor }) => {
      if (clausulaGeralEstruturaId == id) dado = convertValueToData({ valor, tipo })
    })

    return dado
  }

  function convertValueToData({ valor, tipo }) {
    let dado = {
      data: null,
      numerico: null,
      percentual: null,
      hora: null,
      texto: null,
      combo: {
        valor: []
      },
      descricao: null
    }

    switch (tipo) {
      case InformacaoAdicionalTipo.Date:
        dado.data = valor
        break
      case InformacaoAdicionalTipo.Number:
        dado.numerico = parseFloat(valor)
        break
      case InformacaoAdicionalTipo.Percent:
        dado.percentual = parseFloat(valor)
        break
      case InformacaoAdicionalTipo.Hour:
        dado.hora = valor
        break
      case InformacaoAdicionalTipo.Text:
        dado.texto = valor
        break
      case InformacaoAdicionalTipo.Monetario:
        dado.numerico = parseFloat(valor)
        break
      case InformacaoAdicionalTipo.Select:
        dado.combo.valor = valor.split(', ')
        break
      case InformacaoAdicionalTipo.SelectMultiple:
        dado.combo.valor = valor.split(', ')
        break
      default:
        dado.descricao = valor
        break
    }

    return dado
  }
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
      sindicatoPatronalSelect?.disable();

      const options = await sindicatoPatronalSelect?.loadOptions();
      const sindicatoSelecionadoOption = options.find(o => o.id == sindicatoId);

      if (sindicatoSelecionadoOption) {
        sindicatoPatronalSelect?.clear();
        sindicatoPatronalSelect?.setCurrentValue(sindicatoSelecionadoOption);
      }
      else {
        NotificationService.error({title: "Sindicato patronal não encontrado entre as opções."});
      }

      sindicatoPatronalSelect?.enable();
    }
  }

  $("#btnFiltrar").trigger("click");
}

function configurarInformacaoSindicatoService() {
  const modalInfoSindicato = new ModalInfoSindicato(renderizarModal,sindicatoService,DataTableWrapper);
  modalInfoSindicato.initialize("info-modal-sindicato-container");
}
