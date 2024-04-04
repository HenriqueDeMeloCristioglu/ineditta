import { Chart, registerables } from 'chart.js';
import $ from 'jquery';
import jQuery from 'jquery';
import 'bootstrap';
import '../../js/utils/util.js';

import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'bootstrap/dist/css/bootstrap.min.css';

import { renderizarModal, closeModal } from '../../js/utils/modals/modal-wrapper.js';

import 'chosen-js';
import 'jquery-toggles';
import 'jquery-sparkline';
import 'easy-pie-chart';
import 'jquery.nicescroll';
import 'jquery.cookie';
import 'jquery-mask-plugin';
import 'select2';
import 'fullcalendar';

import DatatableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import SelectWrapper from '../../js/utils/selects/select-wrapper.js';

// Core
import { ApiService, AuthService } from '../../js/core/index.js';
import { ApiLegadoService } from '../../js/core/api-legado.js';

import maxBy from 'lodash/maxBy.js';

// Services
import {
  UsuarioAdmService,
  DocSindService,
  ClienteUnidadeService,
  GrupoEconomicoService,
  UFService,
  AcompanhamentoCctService,
  CnaeService,
  IndicadorEconomicoService,
  MatrizService
} from '../../js/services'
import NotificationService from "../../js/utils/notifications/notification.service.js";

import config from '../../assets/configs/config.json';
import NumberFormatter from '../../js/utils/number/number-formatter.js';
import DateFormatter from '../../js/utils/date/date-formatter.js';
import DateParser from '../../js/utils/date/date-parser.js';
import moment from 'moment';
import Config from '../../assets/configs/config.json';
import { UsuarioNivel } from '../../js/application/usuarios/constants/usuario-nivel.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import { div } from '../../js/utils/components/elements/div.js';
import { stringI } from '../../js/utils/components/string-elements/string-i.js';
import PageWrapper from '../../js/utils/pages/page-wrapper.js';
import { MediaType } from '../../js/utils/web/media-type.js';

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();

const usuarioAdmSerivce = new UsuarioAdmService(apiService);
const cnaeService = new CnaeService(apiService);
const grupoEconomicoService = new GrupoEconomicoService(apiService);
const matrizService = new MatrizService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const acompanhamentoCctService = new AcompanhamentoCctService(apiService);
const documentoSindicalService = new DocSindService(apiService);
const indicadorEconomicoService = new IndicadorEconomicoService(apiService, apiLegadoService);
const ufService = new UFService();

let atividadeEconomicaSelect = null;
let grupoEconomicoSelect = null;
let matrizSelect = null;
let estabelecimentoSelect = null;
let negociacaoEncerradasDatatable = null;
let negociacaoProcessadaSistemaTb = null;
let negociacoesAbertasChart = null;
let indicadoresEvolucaoChart = null;

let filtros = {
  numeroMeses: 12
};

// eslint-disable-next-line no-unused-vars
let nomeDocumentoSelect = null;
// eslint-disable-next-line no-unused-vars
let faseDocumentoSelect = null;
// eslint-disable-next-line no-unused-vars
let anoBaseDocumentoSelect = null;

const trsAcordosColetivos = {
  1: 'acordos-coletivos',
  2: 'acordos-coletivos-esp',
  3: 'convencoes-coletivas',
  4: 'convencoes-coletivas-esp',
  5: 'termo-aditivo-convencao-coletiva',
  default: 'termo-aditivo-acordo-coletivo'
};

jQuery(async () => {
  await AuthService.initialize();

  new Menu()

  const dadosPessoais = await usuarioAdmSerivce.obterDadosPessoais();

  if (dadosPessoais.isFailure()) {
    NotificationService.error({title: "Não foi possível carregar os dados do usuário"});
    return;
  }

  if (dadosPessoais?.value?.nivel == 'Unidade') {
    window.location.pathname = '/perfil_estabelecimento.php';
    return;
  }

  $(".horizontal-nav").removeClass("hide");

  Chart.register(...registerables);

  configurarModal();

  configurarChosenSelect();

  await configurarPagina(dadosPessoais);

  configurarDatatables();

  configurarFiltro();

  await filtrar();
});

function configurarFiltro() {
  const anoAtual = moment().year();
  const selectAnos = $("#anosNeg");

  for (let i = 0; i < 10; i++) {
    const ano = anoAtual - i;
    const option = new Option(ano, ano);
    selectAnos.append(option);
  }

  nomeDocumentoSelect = new SelectWrapper("#nomeDocNeg", {
    options: { placeholder: "Selecione", allowEmpty: true },
    onOpened: async () => (await acompanhamentoCctService.obterNomeDocumentoFiltro()).value,
    onChange: async (data) => {
      filtros.nomeDocNegId = data.id
  
      carregarChartNegociacoesAbertas()
    }
  })

  faseDocumentoSelect = new SelectWrapper("#faseDocNeg", {
    options: { placeholder: "Selecione", allowEmpty: true },
    onOpened: async () => (await acompanhamentoCctService.obterFasesFiltro()).value,
    onChange: async (data) => {
      filtros.faseDocNeg = data.id
  
      carregarChartNegociacoesAbertas()
    }
  })

  anoBaseDocumentoSelect = new SelectWrapper("#anoDocNeg", {
    options: { placeholder: "Selecione", allowEmpty: true },
    onOpened: async () => (await acompanhamentoCctService.obterAnoBaseFiltro()).value,
    onChange: async (data) => {
      filtros.anoNeg = data.id
  
      carregarChartNegociacoesAbertas()
    }
  })
}

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  const modalGrupoEconomicoEmpresa = document.getElementById('grupoEconomicoEmpresaModalHidden');
  const modalGrupoEconomicoEmpresaContent = document.getElementById('grupoEconomicoEmpresaModalHiddenContent');

  const modalGrupoEconomicoEmpresaButtonsConfig = [
    {
      id: 'grupoEconomicoEmpresaModalFechar',
      onClick: async (id, modalContainer) => {
        await Promise.resolve();
        closeModal(modalContainer);
      }
    }
  ];

  const estabelecimentoModalHidden = document.getElementById('estabelecimentoModalHidden');
  const estabelecimentoModalContent = document.getElementById('estabelecimentoHiddenContent');

  const estabelecimentoModalButtonsConfig = [
    {
      id: 'estabelecimentoModalFechar',
      onClick: async (id, modalContainer) => {
        await Promise.resolve();
        closeModal(modalContainer);
      }
    }
  ];

  const cnaeModalHidden = document.getElementById('cnaeModalHidden');
  const cnaeModalContent = document.getElementById('cnaeModalHiddenContent');

  const cnaeModalButtonsConfig = [
    {
      id: 'cnaeModalFechar',
      onClick: async (id, modalContainer) => {
        await Promise.resolve();
        closeModal(modalContainer);
      }
    }
  ];

  const modalsConfig = [
    {
      id: 'grupoEconomicoEmpresaModal',
      modal_hidden: modalGrupoEconomicoEmpresa,
      content: modalGrupoEconomicoEmpresaContent,
      btnsConfigs: modalGrupoEconomicoEmpresaButtonsConfig,
      onClose: async () => await filtrar(),
      isInIndex: true
    },
    {
      id: 'estabelecimentoModal',
      modal_hidden: estabelecimentoModalHidden,
      content: estabelecimentoModalContent,
      btnsConfigs: estabelecimentoModalButtonsConfig,
      onClose: async () => await filtrar(),
      isInIndex: true
    },
    {
      id: 'cnaeModal',
      modal_hidden: cnaeModalHidden,
      content: cnaeModalContent,
      btnsConfigs: cnaeModalButtonsConfig,
      onClose: async () => await filtrar(),
      isInIndex: true
    }
  ];

  renderizarModal(pageCtn, modalsConfig);

  //$("#linkGrupoEconomicoModalAbrir").on('click', () => $("#btnGrupoEconomicoModalAbrir").trigger('click'));
  //$("#linkEstabelecimentoModalAbrir").on('click', () => $("#btnEstabelecimentoModalAbrir").trigger('click'));
  //$("#linkCnaeModalAbrir").on('click', () => $("#btnCnaeModalAbrir").trigger('click'));
}

async function filtrar() {
  filtros = {
    numeroMeses: filtros.numeroMeses,
    anoInicial: new Date().getFullYear() - 1,
    anoFinal: new Date().getFullYear(),
    gruposEconomicosIds: grupoEconomicoSelect?.getValue(),
    matrizesIds: matrizSelect?.getValue(),
    unidadesIds: estabelecimentoSelect?.getValue(),
    cnaesIds: atividadeEconomicaSelect?.getValue(),
  };

  await Promise.all([
    carregarFases(),
    carregarFasesUfs(),
    carregarContadores(),
    carregarUltimasNegociacoesEncerradas(),
    carregarUltimasNegociacoesProcessadasSistema(),
    carregarEstruturasAcumulasPeriodo(),
    carregarChartNegociacoesAbertas(),
    carregarChartIndicadoresEvolucao(),
  ])
}

async function carregarFasesUfs() {
  const result = await acompanhamentoCctService.obterNegociacoesFasesUfs(filtros);
  const ufs = ufService.obterSelect();

  if (result.isFailure()) {
    return;
  }

  ufs.forEach(uf => {
    const ufQuantidade = result.value.find(item => item.uf === uf.id);
    $(`#num${uf.id.toLowerCase()}`).html(ufQuantidade?.quantidade ?? 0);
  });
}

async function carregarFases() {
  $("#table_fases > tbody").empty();

  const result = await acompanhamentoCctService.obterNegociacoesFases(filtros);

  if (result.isFailure()) {
    return;
  }

  let total = 0;

  const fases = result.value.filter((fase) => fase.nome !== "Concluída" && fase.nome !== "Arquivada");

  fases.forEach((fase) => total += fase.quantidade);

  $("#table_fases > tbody")
    .append(`<tr style="background-color:#4E5754"><th style="color: white">TOTAL</th><td style="color: white">${total}</td></tr>`);

  fases.sort((current, next) => current.nome.localeCompare(next.nome)).forEach((fase) => {
    $("#table_fases > tbody")
      .append(`<tr><th>${fase.nome}</th><td>${fase.quantidade}</td></tr>`);
  });
}

async function carregarContadores() {
  const result = await clienteUnidadeService.obterContadores(filtros);

  if (result.isFailure()) {
    return;
  }

  $("#num-emp").html(result.value.empresas);
  $("#num-uni").html(result.value.estabelecimentos);
  $("#num-seg").html(result.value.cnaes);
  $('#num-sinde').html(result.value.sindicatosLaborais);
  $('#num-sindp').html(result.value.sindicatosPatronais);
}

async function carregarUltimasNegociacoesEncerradas() {
  if (negociacaoEncerradasDatatable != null) {
    await negociacaoEncerradasDatatable?.reload();
    return;
  }

  negociacaoEncerradasDatatable = new DatatableWrapper('#negociacaoEncerradaTb', {
    columns: [
      { title: 'Baixar', data: 'arquivo' },
      { title: 'Sigla Doc', data: 'siglaDoc' },
      { title: 'Municípios', data: 'sindicatosLaboraisMunicipios', render: data => data?.join(', '), className: "min-width-column-lg" },
      { title: 'Sind. Laborais', data: 'sindicatosLaboraisSiglas', render: data => data?.join(', '), className: "min-width-column-lg" },
      { title: 'Sind. Patronais', data: 'sindicatosPatronaisSiglas', render: data => data?.join(', '), className: "min-width-column-lg" },
      { title: 'Data-base', data: 'dataBase', className: 'no-break' },
      { title: 'Atualizado em', data: 'dataAprovacao', render: data => DateFormatter.dayMonthYear(data) },
    ],
    fixedHeader: true,
    scrollX: true,
    autoWidth: true,
    scrollCollapse: true,
    scrollY: '240px',
    responsive: false,
    pagingType: "numbers",
    ajax: async (params) => {
      params = { ...params, ...filtros };
      return await documentoSindicalService.obterEncerrados(params);
    },
    columnDefs: [
      {
        "targets": "_all",
        "defaultContent": ""
      }
    ],
    rowCallback: function (row, data) {
      if (data?.id) {
        $(row).css('cursor', 'pointer')

        $(row).on('mouseenter', function () {
          $(this).find('td').css('color', 'blue') // Set text color to blue on hover
        }).on('mouseleave', function () {
          $(this).find('td').css('color', '') // Reset text color when not hovering
        })

        const linkBaixar = div({
          content: stringI({ className: 'fa fa-download' })
        }).on('click', () => downloadDoc(data?.id))

        $("td:eq(0)", row).html(linkBaixar)

        const siglaDoc = $("<div>")
          .on('click', () => verDocumento(data?.id))
          .html(data?.siglaDoc)

        $("td:eq(1)", row).html(siglaDoc)

        const sindicatosLaboraisMunicipios = $("<div>")
          .on('click', () => verDocumento(data?.id))
          .html(data?.sindicatosLaboraisMunicipios?.join(', '))

        $("td:eq(2)", row).html(sindicatosLaboraisMunicipios)

        const sindicatosLaboraisSiglas = $("<div>")
          .on('click', () => verDocumento(data?.id))
          .html(data?.sindicatosLaboraisSiglas?.join(', '))

        $("td:eq(3)", row).html(sindicatosLaboraisSiglas)

        const sindicatosPatronaisSiglas = $("<div>")
          .on('click', () => verDocumento(data?.id))
          .html(data?.sindicatosPatronaisSiglas?.join(', '))

        $("td:eq(4)", row).html(sindicatosPatronaisSiglas)

        const dataBase = $("<div>")
          .on('click', () => verDocumento(data?.id))
          .html(data?.dataBase)

        $("td:eq(5)", row).html(dataBase)

        const dataAprovacao = $("<div>")
          .on('click', () => verDocumento(data?.id))
          .html(DatatableWrapper.formatDate(data?.dataAprovacao))
        $("td:eq(6)", row).html(dataAprovacao)
      }
    },
  });

  await negociacaoEncerradasDatatable.initialize();
}

async function carregarUltimasNegociacoesProcessadasSistema() {
  if (negociacaoProcessadaSistemaTb != null) {
    await negociacaoProcessadaSistemaTb?.reload();
    return;
  }

  negociacaoProcessadaSistemaTb = new DatatableWrapper('#negociacaoProcessadaSistemaTb', {
    columns: [
      { title: 'Sigla Doc', data: 'siglaDoc' },
      { title: 'Municípios', data: 'sindicatosLaboraisMunicipios', className: "min-width-column-lg" },
      { title: 'Sind. Laborais', data: 'sindicatosLaboraisSiglas', className: "min-width-column-lg" },
      { title: 'Sind. Patronais', data: 'sindicatosPatronaisSiglas', className: "min-width-column-lg" },
      { title: 'Data-base', data: 'dataBase' },
      { title: 'Atualizado em', data: 'dataAprovacao' },
    ],
    order: [[0, 'desc']],
    fixedHeader: true,
    scrollX: true,
    autoWidth: true,
    scrollCollapse: true,
    scrollY: '240px',
    responsive: false,
    pagingType: "numbers",
    ajax: async (params) => {
      params = { ...params, ...filtros };
      return await documentoSindicalService.obterProcessados(params);
    },
    columnDefs: [
      {
        "targets": "_all",
        "defaultContent": ""
      }
    ],
    rowCallback: function (row, data) {
      $(row).on('click', function () {
        const url = `${config.url}consultaclausula.php?iddoc=${data?.id}`
        window.open(url, '_blank');
      })
      $(row).css('cursor', 'pointer');
      $(row).on('mouseenter', function () {
        $(this).find('td').css('color', 'blue'); // Set text color to blue on hover
      }).on('mouseleave', function () {
        $(this).find('td').css('color', ''); // Reset text color when not hovering
      });

      if (data?.dataAprovacao) {
        const dataAprovacao = DatatableWrapper.formatDate(data?.dataAprovacao);
        $("td:eq(5)", row).html(dataAprovacao);
      }

      $("td:eq(1)", row).html(data?.sindicatosLaboraisMunicipios?.join(', '));
      $("td:eq(2)", row).html(data?.sindicatosLaboraisSiglas?.join(', '));
      $("td:eq(3)", row).html(data?.sindicatosPatronaisSiglas?.join(', '));
    },
  });

  await negociacaoProcessadaSistemaTb.initialize();
}

async function carregarEstruturasAcumulasPeriodo() {
  const params = { ...filtros, anoInicial: new Date().getFullYear() - 1, anoFinal: new Date().getFullYear() };
  const result = await documentoSindicalService.obterNegociacoesAcumuladas(params);

  if (result.isFailure()) {
    return;
  }

  const maiorDataAprovacao = maxBy(result.value, x => x.ano).maiorDataAprovacao
  result.value.sort((current, next) => next.ano - current.ano)
    .forEach((estruturaAnual, index) => {
      estruturaAnual.itens.sort((current, next) => next.id - current.id)
        .forEach(item => {
          const idLinha = trsAcordosColetivos[item.id] || trsAcordosColetivos.default;
          $(`tr[id='${idLinha}'] > td:eq(0)`).html(item.nomeDocumento);
          $(`tr[id='${idLinha}'] > td:eq(${index + 1})`).html(item.quantidade);
          $(`tr[id='${idLinha}'] > td:eq(${index + 3})`).html(NumberFormatter.percentage(item.proporcao));
        });

      $(`tr[id='total-neg'] > td:eq(${index + 1})`).html(estruturaAnual.itens.reduce((total, item) => total + item.quantidade, 0));
      $(`tr[id='total-neg'] > td:eq(${index + 3})`).html(NumberFormatter.percentage(estruturaAnual.itens.reduce((total, item) => total + item.proporcao, 0)));
    });

  $('#negociacoesAcumuladasDataUltimaAtualizacao').html(DateFormatter.toString(new Date(maiorDataAprovacao)));
}

async function carregarChartNegociacoesAbertas() {
  if (!negociacoesAbertasChart) {
    negociacoesAbertasChart = criarChartNegociacoesAbertas()
  }

  const result = await acompanhamentoCctService.obterNegociacoesAbertas(filtros)

  if (result.isFailure()) return

  const labels = []
  const data = []

  result.value.sort((a, b) => a.mes - b.mes).forEach((negociacao) => {
    labels.push(`${negociacao.nomeMes} (${negociacao.quantidade})`)
    data.push(negociacao.quantidade)
  })

  negociacoesAbertasChart.data.labels = labels
  negociacoesAbertasChart.data.datasets[0].data = data

  const total = result.value.reduce((total, negociacao) => total + negociacao.quantidade, 0)

  negociacoesAbertasChart.options.plugins.title.text = "Negociações em Aberto por data-base (Total: " + total + ")"
  negociacoesAbertasChart.update()
}

function criarChartNegociacoesAbertas() {
  const chart = document.getElementById("negociacoesAbertasChart");

  if (!chart) {
    return null;
  }

  chart.getContext("2d");

  const datasets = [
    {
      label: "Negociações em Aberto",
      data: ["1", "0", "0", "0", "0", "0", "0", "0", "3", "0", "0", "0"],
      backgroundColor: ["rgba(54, 162, 235, 0.2)"],
      borderColor: ["rgba(54, 162, 235, 1)"],
      borderWidth: 1,
    },
  ];

  return new Chart(chart, {
    type: "bar",
    data: {
      labels: [
        "Jan",
        "Fev",
        "Mar",
        "Abr",
        "Mai",
        "Jun",
        "Jul",
        "Ago",
        "Set",
        "Out",
        "Nov",
        "Dez",
      ],
      datasets: datasets,
    },
    options: {
      plugins: {
        title: {
          display: true,
          text: "Negociações em Aberto por data-base",
          position: 'bottom'
        },
      },
      scales: {
        y: {
          beginAtZero: true,
        },
      },
    },
  });
}

async function carregarChartIndicadoresEvolucao() {
  limparIndices();

  if (!indicadoresEvolucaoChart) {
    indicadoresEvolucaoChart = criarChartIndicadoresEvolucao();
  }

  const result = await indicadorEconomicoService.obterHomeAsync({ numeroMeses: filtros.numeroMeses });

  if (result.isFailure()) {
    return;
  }

  const ultimoINPC = result.value.find(item => item.indicador === 'INPC' && item.tipo === 1)?.indices?.sort((a, b) => new Date(b.periodo) - new Date(a.periodo))[0];
  const ultimoIPCA = result.value.find(item => item.indicador === 'IPCA' && item.tipo === 1)?.indices?.sort((a, b) => new Date(b.periodo) - new Date(a.periodo))[0];

  if (ultimoINPC) {
    $("#calc-inpc").html(NumberFormatter.percentage(ultimoINPC.indice));
    $("#last-inpc").html(DateFormatter.monthYear(DateParser.fromString(ultimoINPC.periodo)));
  }

  if (ultimoIPCA) {
    $("#calc-ipca").html(NumberFormatter.percentage(ultimoIPCA.indice));
    $("#last-ipca").html(DateFormatter.monthYear(DateParser.fromString(ultimoIPCA.periodo)));
  }

  const anoMesInicial = new Date().subtractMonths(filtros.numeroMeses - 1);
  const anoMesFinal = new Date();
  const anosMeses = [];

  while (anoMesFinal >= anoMesInicial) {
    anosMeses.push(DateFormatter.monthYear(anoMesInicial));
    anoMesInicial.addMonths(1);
  }

  const inpcs = [];
  const ipcas = [];
  const inpcsProjetados = []
  const ipcasProjetados = [];

  anosMeses.forEach(anoMes => {
    const indice = result.value.find(item => item.indicador === 'INPC' && item.tipo === 1)?.indices?.find(item => DateFormatter.monthYear(item.periodo) === anoMes);
    if (indice) {
      inpcs.push(indice.indice);
      return;
    }
  });

  anosMeses.forEach(anoMes => {
    const indice = result.value.find(item => item.indicador === 'IPCA' && item.tipo === 1)?.indices?.find(item => DateFormatter.monthYear(item.periodo) === anoMes);
    if (indice) {
      ipcas.push(indice.indice);
      return;
    }
  });

  anosMeses.forEach(anoMes => {
    const indice = result.value.find(item => item.indicador === 'INPC' && item.tipo === 2)?.indices?.find(item => DateFormatter.monthYear(item.periodo) === anoMes);
    if (indice) {
      inpcsProjetados.push(indice.indice);
      return;
    }
  });


  anosMeses.forEach(anoMes => {
    const indice = result.value.find(item => item.indicador === 'IPCA' && item.tipo === 2)?.indices?.find(item => DateFormatter.monthYear(item.periodo) === anoMes);
    if (indice) {
      ipcasProjetados.push(indice.indice);
      return;
    }
  });

  indicadoresEvolucaoChart.data.labels = eixoXimposto(filtros.numeroMeses);
  indicadoresEvolucaoChart.data.datasets[0].data = ipcas;
  indicadoresEvolucaoChart.data.datasets[1].data = inpcs;

  const fonteInpcProjetado = result.value.find(item => item.indicador === 'INPC' && item.tipo === 2)?.fonte;
  const fonteIpcaProjetado = result.value.find(item => item.indicador === 'IPCA' && item.tipo === 2)?.fonte;

  indicadoresEvolucaoChart.options.tooltips = {
    callbacks: {
      afterLabel: function (context) {
        if (context.datasetIndex == 3) {
          return `Fonte: ${fonteInpcProjetado}`;
        }
        if (context.datasetIndex == 1) {
          return `Fonte: ${fonteIpcaProjetado}`;
        }
        if (context.datasetIndex == 0) {
          let re = 'Fonte: IBGE';
          return re;
        }
        if (context.datasetIndex == 2) {
          let re = 'Fonte: IBGE';
          return re;
        }
        return '';
      }
    }
  };

  indicadoresEvolucaoChart.update();
}

async function mudarMesIndicadoresEvolucao(quantidadeMeses) {
  filtros.numeroMeses = quantidadeMeses ?? 12;

  await carregarChartIndicadoresEvolucao();
}

function limparIndices() {
  $("#calc-inpc").val('0%');
  $("#last-inpc").val('');
  $("#calc-ipca").val('0%');
  $("#last-ipca").val('');
}

function criarChartIndicadoresEvolucao() {
  const chart = document.getElementById("indicadoresEvolucaoChart");

  if (!chart) {
    return;
  }

  chart.getContext("2d");

  return new Chart(chart, {
    type: "line",
    data: {
      labels: eixoXimposto(12),
      datasets: [
        {
          label: "IPCA real",
          data: [null, null, null, null, null, null, null, null, null, null, null, null],
          borderColor: ["rgba(54, 162, 235, 1)"],
          borderWidth: 1,
        },
        {
          label: "INPC real",
          data: [null, null, null, null, null, null, null, null, null, null, null, null],
          borderColor: ["rgba(105, 9, 222, 1)"],
          borderWidth: 1,
        }
      ]
    },
    options: {
      scales: {
        y: {
          beginAtZero: true,
          title: {
            display: true,
            text: "Percentual (%)",
            color: "000",
          }
        }
      }
    }
  });
}

async function configurarPagina(dadosPessoais) {
  const isIneditta = dadosPessoais?.value.nivel == UsuarioNivel.Ineditta;
  const isEmpresa = dadosPessoais?.value.nivel == UsuarioNivel.Empresa;
  const isGrupoEconomico = dadosPessoais?.value.nivel == UsuarioNivel.GrupoEconomico;
  const isEstabelecimento = dadosPessoais?.value.nivel == UsuarioNivel.Estabelecimento;

  const markOptionAsSelectable = dadosPessoais?.nivel == 'Cliente' ? () => true : () => false;
  const markGrupoEconomico = (isGrupoEconomico || isEmpresa || isEstabelecimento) ? () => true : () => false;
  const markEmpresa = (isEmpresa || isEstabelecimento) ? () => true : () => false;
  const markEstabelecimento = (isEstabelecimento) ? () => true : () => false;

  atividadeEconomicaSelect = new SelectWrapper('#categoria', { options: { placeholder: 'Selecione', multiple: true }, parentId: '#grupo', onOpened: async () => await obterAtividadesEconomicas(), markOptionAsSelectable: markOptionAsSelectable });
  grupoEconomicoSelect = new SelectWrapper('#grupo', { options: { placeholder: 'Selecione', multiple: true }, onOpened: async () => await grupoEconomicoService.obterSelectPorUsuario(), markOptionAsSelectable: markGrupoEconomico });
  matrizSelect = new SelectWrapper('#matriz', { options: { placeholder: 'Selecione', multiple: true }, parentId: '#grupo', onOpened: async (grupoEconomicoId) => await matrizService.obterSelectPorUsuario(grupoEconomicoId), markOptionAsSelectable: markEmpresa });
  estabelecimentoSelect = new SelectWrapper('#unidade', { options: { placeholder: 'Selecione', multiple: true }, parentId: '#matriz', onOpened: async (matrizId) => await clienteUnidadeService.obterSelectPorUsuario(matrizId, $("#grupo").val()), markOptionAsSelectable: markEstabelecimento });

  if (isIneditta) {
    grupoEconomicoSelect.enable()
    matrizSelect.enable()
    estabelecimentoSelect.enable()
  }
  else if (isGrupoEconomico) {
    await grupoEconomicoSelect.loadOptions()
    grupoEconomicoSelect.disable()

    matrizSelect.enable()
    estabelecimentoSelect.enable()
  }
  else if (isEmpresa) {
    await grupoEconomicoSelect.loadOptions()
    grupoEconomicoSelect.disable()

    const options = await matrizSelect.loadOptions()
    if (!(options instanceof Array && options.length > 1)) {
      matrizSelect.disable()
    }
    else {
      matrizSelect.config.markOptionAsSelectable = () => false;
      matrizSelect?.clear()
    }

    estabelecimentoSelect.enable()
  }
  else if (isEstabelecimento) {
    await grupoEconomicoSelect.loadOptions()
    grupoEconomicoSelect.disable()

    await matrizSelect.loadOptions()
    matrizSelect.disable()

    const options = await estabelecimentoSelect.loadOptions()
    if (!(options instanceof Array && options.length > 1)) {
      estabelecimentoSelect.disable()
    }
    else {
      estabelecimentoSelect.config.markOptionAsSelectable = () => false;
      estabelecimentoSelect?.clear()
    }
    
  }

  $('#grupo').on('change', () => {
    atividadeEconomicaSelect.reload();
    estabelecimentoSelect.loaded = false;

  })
  $('#matriz').on('change', () => atividadeEconomicaSelect.reload());
  $('#unidade').on('change', () => atividadeEconomicaSelect.reload());

  $('.anoAtual').html(new Date().getFullYear());
  $('.anoPassado').html(new Date().getFullYear() - 1);

  $('.chart-evolucao-indicadores').on('click', async function () {
    const quantidadeMeses = $(this).attr('data-quantidadeMeses');
    await mudarMesIndicadoresEvolucao(quantidadeMeses);
  });

  $('.divSindicato').on('click', function () {
    const gruposEconomicosIds = grupoEconomicoSelect?.getValue() ?? [];
    const matrizesIds = matrizSelect?.getValue() ?? [];
    const estabelecimentosIds = estabelecimentoSelect?.getValue() ?? [];
    const cnaesIds = atividadeEconomicaSelect?.getValue() ?? [];

    if (!gruposEconomicosIds.length && !matrizesIds.length && !estabelecimentosIds.length && !cnaesIds.length) {
      const url = `${Config.url}modulo_sindicatos.php`;
      window.open(url, '_blank');
      return;
    }

    const url = `${Config.url}modulo_sindicatos.php?filter=1&matrizes=${matrizesIds}&grupos=${gruposEconomicosIds}&unidades=${estabelecimentosIds}&cnaes=${cnaesIds}`;

    window.open(url, '_blank');
  });
}

async function obterAtividadesEconomicas() {
  const gruposEconomicosIds = grupoEconomicoSelect?.getValue() ?? [];
  const matrizesIds = matrizSelect?.getValue() ?? [];
  const estabelecimentosIds = estabelecimentoSelect?.getValue() ?? [];

  return await cnaeService.obterSelectPorUsuario({
    gruposEconomicosIds,
    matrizesIds,
    estabelecimentosIds
  });
}

function configurarChosenSelect() {
  $("#filter_list_dropdwn")
    .chosen()
    .change(function (e, option) {
      var parent_id =
        option.selected !== undefined ? option.selected : option.deselected;

      $("#filter_list_dropdwn option[data-parent_id=" + parent_id + "]").each(
        function () {
          $(this).prop("selected", option.selected !== undefined ? true : false);
        }
      );

      $("#filter_list_dropdwn").trigger("chosen:updated");
    });
}

function eixoXimposto(numeroMeses = 12) {
  const result = [];

  const beginDate = new Date().subtractMonths(numeroMeses - 1);
  const endDate = new Date();

  while (endDate >= beginDate) {
    result.push(DateFormatter.monthYear(beginDate));
    beginDate.addMonths(1);
  }

  return result;
}

function configurarDatatables() {
  $("#pesquisar-input").val("");
  $("#filgrupo-input").prop("checked", false);
  $("#filnome-input").prop("checked", false);
  $("#filtexto-input").prop("checked", false);
  $("#doc-fil").val("");
  $("#emp-fil").val("");
  $("#uni-fil").val("");
  $("#cat-fil").val("");
  $("#loc-fil").val("");
  $("#validade-fil").val("");
  $("#mes-fil").val("");
  $("#ano-fil").val("");
  $("#lab-fil").val("");
  $("#patr-fil").val("");

  $("#ano-fil").mask("0000");
  $(".chzn-select").chosen();

  $(".chzn-select").chosen();
}

async function verDocumento(id) {
  const response = await documentoSindicalService.download({ id })

  if (response.isFailure()) {
    return NotificationService.error({ title: 'Não foi possível baixar o arquivo.' })
  }

  PageWrapper.preview(response.value.data.blob, MediaType.pdf.Accept)
}

async function downloadDoc(id) {
  const response = await documentoSindicalService.download({ id })

  if (response.isFailure()) {
    return NotificationService.error({ title: 'Não foi possível baixar o arquivo.' })
  }

  const data = response.value.data

  PageWrapper.download(data.blob, data.filename, MediaType.stream.Accept)
}