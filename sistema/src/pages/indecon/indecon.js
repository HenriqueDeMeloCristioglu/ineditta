import "datatables.net-bs5/css/dataTables.bootstrap5.css";
import "datatables.net-responsive-bs5/css/responsive.bootstrap5.css";

import JQuery from "jquery";
import $ from "jquery";
import "../../js/utils/masks/jquery-mask-extensions.js";

import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap";

// Core
import { ApiService, AuthService, UserInfoService } from "../../js/core/index.js"
import { ApiLegadoService } from "../../js/core/api-legado"

// Services
import { IndicadorEconomicoService } from "../../js/services"

import moment from "moment"
import Result from "../../js/core/result.js"
import { renderizarModal } from "../../js/utils/modals/modal-wrapper.js";
import DataTableWrapper from "../../js/utils/datatables/datatable-wrapper";
import NotificationService from "../../js/utils/notifications/notification.service.js";
import DatepickerWrapper from "../../js/utils/datepicker/datepicker-wrapper.js";
import ConfirmationService from "../../js/utils/confirmation/confirmation.service.js";
import DateParser from "../../js/utils/date/date-parser.js";

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

let indicadorPrincipalTb = null;
let indicadorRealTb = null;

let datasPrincipais = [null];
let datasReais = [null];

const apiService = new ApiService();
const apiLegado = new ApiLegadoService();
const indicadorEconomicoService = new IndicadorEconomicoService(
  apiService,
  apiLegado
);

JQuery(async () => {
  new Menu()

  await AuthService.initialize();

  configurarModal();

  configurarFormulario();

  await carregarDatatablePrincipal();
  await carregarDatatableReal();

  $('.horizontal-nav').removeClass('hide');
});

function configurarFormulario() {
  datasPrincipais[0] = new DatepickerWrapper("#periodoPrincipal", null, 'mes-ano');
  datasReais[0] = new DatepickerWrapper("#periodoReal", null, 'mes-ano');

  $('#projetadoPrincipal').maskPercentage();
  $('#real').maskPercentage();
  $("#origem").val(UserInfoService.getTipo());
}

function addLinha(tipo) {
  const quantidadeLinhas = $(`#campos-tabela-${tipo} tr`).length;
  const id = tipo === 'principal' ? `periodoPrincipal${quantidadeLinhas}` : `periodoReal${quantidadeLinhas}`;
  let linha = $("<tr>");

  const td1 = $("<td>");
  let dateInput = $("<input>")
    .attr("id", id)
    .attr("type", "text")
    .attr("placeholder", "Mês/Ano")
    .addClass(`form-control linha-periodo-${tipo}`)
  td1.append(dateInput);

  const valueId = tipo === 'principal' ? `projetadoPrincipal${quantidadeLinhas}` : `real${quantidadeLinhas}`;
  let percentageInput = $("<input>")
    .attr("id", valueId)
    .attr("type", "text")
    .addClass(`form-control linha-valor-${tipo}`).maskPercentage();
  const td2 = $("<td>");
  td2.append(percentageInput);

  linha.append(td1);
  linha.append(td2);

  $(`#campos-tabela-${tipo}`).append(linha);

  if (tipo === 'principal') {
    datasPrincipais.push(new DatepickerWrapper(`#${id}`, null, 'mes-ano'));

    return
  }

  datasReais.push(new DatepickerWrapper(`#${id}`, null, 'mes-ano'));
}

function removeLinha(tipo) {
  const lastTableRow = $(`#campos-tabela-${tipo} tr:last-child`);
  lastTableRow.remove();

  if (tipo === 'principal') {
    datasPrincipais.pop();

    return;
  }

  datasReais.pop();
}

function configurarModal() {
  const pageCtn = document.getElementById("pageCtn");

  const modalPrincipal = document.getElementById("indPrincipalModalHidden");
  const contentPrincipal = document.getElementById(
    "indPrincipalModalHiddenContent"
  );

  const buttonsPrincipalConfig = [
    {
      id: "indPrincipalCadastrarBtn",
      onClick: async () => await upsertPrincipal(),
    },
    {
      id: "btnAddLinhaPrincipal",
      onClick: async () => addLinha("principal"),
    },
    {
      id: "btnRemoveLinhaPrincipal",
      onClick: async () => removeLinha("principal"),
    },
  ];

  const buttonsRealConfig = [
    {
      id: "indRealCadastrarBtn",
      onClick: async () => await upsertReal(),
    },
    {
      id: "btnAddLinhaReal",
      onClick: async () => addLinha("real"),
    },
    {
      id: "btnRemoveLinhaReal",
      onClick: async () => removeLinha("real"),
    },
  ];

  const modalReal = document.getElementById("indRealModalHidden");
  const contentReal = document.getElementById("indRealModalHiddenContent");

  const modalsConfig = [
    {
      id: "indPrincipalModal",
      modal_hidden: modalPrincipal,
      content: contentPrincipal,
      btnsConfigs: buttonsPrincipalConfig,
      onOpen: async () => {
        const id = $("#id-principal-input").val();
        if (id) {
          await obterPrincipalPorId(id);
        }
      },
      onClose: () => limparPrincipal(),
    },
    {
      id: "indRealModal",
      modal_hidden: modalReal,
      content: contentReal,
      btnsConfigs: buttonsRealConfig,
      onOpen: async () => {
        const id = $("#id-real-input").val();
        if (id) {
          await obterRealPorId(id);
        }
      },
      onClose: () => limparReal(),
    },
  ];

  renderizarModal(pageCtn, modalsConfig);
}

async function upsertPrincipal() {
  const id = $("#id-principal-input").val();

  return id ? await editarPrincipal(id) : await incluirPrincipal();
}

async function upsertReal() {
  const id = $("#id-real-input").val();

  return id ? await editarReal(id) : await incluirReal();
}

async function carregarDatatablePrincipal() {
  indicadorPrincipalTb = new DataTableWrapper("#indprincipaltb", {
    ajax: async (requestData) =>
      await indicadorEconomicoService.obterDatatablePrincipal(requestData),
    columns: [
      { data: null, orderable: false, width: "0px" },
      { title: "Indicador", data: "indicador" },
      { title: "Origem", data: "origem" },
      { title: "Fonte", data: "fonte" },
      { title: "Período", data: "data" },
      { title: "Projetado (%)", data: "dadoProjetado" },
      { title: "Remover", data: null, orderable: false, width: "0px" },
    ],
    rowCallback: function (row, data) {
      const editIcon = $("<i>").addClass("fa fa-file-text");
      let editButton = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(editIcon);
      editButton.on("click", function () {
        const id = $(this).attr("data-id");
        $("#id-principal-input").val(id);
        $("#indPrincipalBtn").trigger("click");
      });
      $("td:eq(0)", row).html(editButton);

      const removeIcon = $("<i>").addClass("fa fa-trash-o");
      let removeButton = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(removeIcon);
      removeButton.on("click", function () {
        const id = $(this).attr("data-id");
        excluirIndicador(id);
      });
      $("td:eq(6)", row).html(removeButton);
    },
  });

  await indicadorPrincipalTb.initialize();
}

async function carregarDatatableReal() {
  indicadorRealTb = new DataTableWrapper("#indrealtb", {
    ajax: async (requestData) =>
      await indicadorEconomicoService.obterDatatableReal(requestData),
    columns: [
      { data: null, orderable: false, width: "0px" },
      { title: "Indicador", data: "indicador" },
      { title: "Real (%)", data: "dadoReal" },
      { title: "Período", data: "periodoData" },
    ],
    rowCallback: function (row, data) {
      const icon = $("<i>").addClass("fa fa-file-text");
      let button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon);
      button.on("click", function () {
        const id = $(this).attr("data-id");
        $("#id-real-input").val(id);
        $("#indRealBtn").trigger("click");
      });
      $("td:eq(0)", row).html(button);
    },
  });

  await indicadorRealTb.initialize();
}

async function incluirPrincipal() {
  const periodos = obterPeriodos('principal');

  const periodoXProjetado = periodos.map((valor, index) => {
    return {
      periodo: DateParser.toString(datasPrincipais[index]?.getValue()),
      projetado: valor
    }
  })

  const dataRequest = {
    idUsuario: UserInfoService.getUserId(),
    origem: $('#origem').val(),
    fonte: $('#fonte').val(),
    periodosProjetados: periodoXProjetado,
    indicador: $('#indicador').val()
  }

  const result = await indicadorEconomicoService.incluirPrincipal(dataRequest);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Cadastro realizado com sucesso!",
  });

  limparPrincipal();

  return Result.success();
}

function obterPeriodos(tipo) {
  const linhas = $(`#campos-tabela-${tipo} .linha-valor-${tipo}`).toArray().map(input => input.value);
  return linhas;
}

async function incluirReal() {
  const periodos = obterPeriodos('real');

  const periodoXReal = periodos.map((valor, index) => {
    return {
      periodo: DateParser.toString(datasReais[index]?.getValue()),
      real: valor
    }
  })

  const requestData = {
    indicador: $("#indicador-real").val(),
    periodosReais: periodoXReal,
  };

  const result = await indicadorEconomicoService.incluirReal(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Cadastro realizado com sucesso!",
  });

  limparReal();

  return Result.success();
}

function transformarDataParaMesAno(data) {
  const date = moment(data, 'YYYY-MM-DD').toDate();
  return date;
}

async function obterPrincipalPorId(id) {
  limparPrincipal();

  const response = await indicadorEconomicoService.obterPrincipalPorId(id);

  if (!response) {
    NotificationService.error({
      title: "Erro",
      message: "Indicador não foi encontrada",
    });
    return;
  }

  $("#linhasPrincipaisToolbar").hide();

  $("#select").html(response.value.label);
  $("#select").append(response.value.periodo);
  $("#id-principal-input").val(response.value.id);
  $("#origem").val(response.value.origem);
  $("#cliente").val(response.value.cliente);
  $("#indicador").val(response.value.indicador);
  $("#fonte").val(response.value.fonte);
  $("#projetadoPrincipal").val(response.value.dadoProjetado);
  $("#periodoPrincipal").val(transformarDataParaMesAno(response.value.data));
  $("#periodoPrincipal").datepicker('setDate', transformarDataParaMesAno(response.value.data))
}

async function obterRealPorId(id) {
  limparReal();

  const response = await indicadorEconomicoService.obterRealPorId(id);

  if (!response) {
    NotificationService.error({
      title: "Erro",
      message: "Indicador não foi encontrada",
    });
    return;
  }

  $("#linhasReaisToolbar").hide();

  $("#indicador-real").val(response.value.indicador);
  $("#periodoReal").datepicker('setDate', transformarDataParaMesAno(response.value.periodoData))
  $("#real").val(response.value.dadoReal).trigger("change");
  $("#id-real-input").val(response.value.id);
}

async function editarPrincipal(id) {
  const periodos = obterPeriodos('principal');

  const periodoXProjetado = periodos.map((valor, index) => {
    return {
      periodo: DateParser.toString(datasPrincipais[index]?.getValue()),
      projetado: valor
    }
  })

  const requestData = {
    id,
    origem: $("#origem").val(),
    indicador: $("#indicador").val(),
    periodosProjetados: periodoXProjetado,
    idUsuario: UserInfoService.getUserId(),
    fonte: $("#fonte").val(),
  };

  const result = await indicadorEconomicoService.editarPrincipal(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Cadastro atualizado com sucesso!",
  });

  limparPrincipal();

  return Result.success();
}

async function editarReal(id) {
  const periodos = obterPeriodos('real');

  const periodoXReal = periodos.map((valor, index) => {
    return {
      periodo: DateParser.toString(datasReais[index]?.getValue()),
      real: valor
    }
  })


  const requestData = {
    id,
    indicador: $("#indicador-real").val(),
    periodosReais: periodoXReal,
  };

  const result = await indicadorEconomicoService.editarReal(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Cadastro atualizado com sucesso!",
  });

  limparReal();

  return Result.success();
}

function formatarData(data) {
  const momentDate = moment(data);
  const formattedDate = momentDate.format('MMM/YYYY')
  return formattedDate.charAt(0).toUpperCase() + formattedDate.slice(1);
}

async function excluirIndicador(id) {
  ConfirmationService.confirmDelete({
    message: "Você irá excluir o indicador!",
    fn: async () => {
      const requestData = {
        module: "indecon",
        action: "deleteIndicador",
        id,
      };

      const result = await indicadorEconomicoService.excluir(requestData);

      if (result.isFailure()) {
        NotificationService.error({ title: "Erro", message: result.error });
        return result;
      }

      indicadorPrincipalTb?.reload();

      NotificationService.success({
        title: "Sucesso",
        message: "Indicador excluído com sucesso!",
      });
    },
  });
}

function limparReal() {
  limparFormularioReal();

  indicadorRealTb?.reload();
}

function limparFormularioReal() {
  $("#indicadorRealForm").trigger("reset");
  $("#id-real-input").val(null);
  $("#linhasReaisToolbar").show();
}

function limparPrincipal() {
  limparFormularioPrincipal();

  indicadorPrincipalTb?.reload();
}

function limparFormularioPrincipal() {
  $("#indicadorPrincipalForm").trigger("reset");
  $("#id-principal-input").val(null);
  $("#origem").val(UserInfoService.getTipo());
  $("#linhasPrincipaisToolbar").show();
}

function toDateOnly(date) {
  return date.toISOString().split('T')[0];
}