import "datatables.net-bs5/css/dataTables.bootstrap5.css";
import "datatables.net-responsive-bs5/css/responsive.bootstrap5.css";

import JQuery from "jquery";
import $ from "jquery";
import "../../js/utils/masks/jquery-mask-extensions.js";

import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap";

// Core
import { ApiService, AuthService } from "../../js/core/index.js"
import { ApiLegadoService } from "../../js/core/api-legado"

// Services
import { LocalizacaoService } from "../../js/services"

import DataTableWrapper from "../../js/utils/datatables/datatable-wrapper";
import NotificationService from "../../js/utils/notifications/notification.service.js";
import Result from "../../js/core/result.js";
import { renderizarModal } from "../../js/utils/modals/modal-wrapper.js";

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

const apiService = new ApiService();
const apiLegado = new ApiLegadoService();
const localizacaoService = new LocalizacaoService(apiService, apiLegado);

let localizacaoTb = null;

JQuery(async ($) => {
  new Menu()

  await AuthService.initialize();

  configurarFormulario($);

  configurarModal();

  await carregarDatatable();
});

function configurarFormulario($) {
  $("#uf-input").mask("AA");
}

async function carregarDatatable() {
  localizacaoTb = new DataTableWrapper("#localizacaotb", {
    ajax: async (requestData) =>
      await localizacaoService.obterDatatable(requestData),
    columns: [
      { data: null, orderable: false, width: "0px" },
      { title: "Código do País", data: "codigoPais" },
      { title: "País", data: "pais" },
      { title: "Código da Região", data: "codigoRegiao" },
      { title: "Região", data: "regiao" },
      { title: "Código da UF", data: "codigoUf" },
      { title: "Estado", data: "estado" },
      { title: "UF", data: "uf" },
      { title: "Código do Município", data: "codigoMunicipio" },
      { title: "Município", data: "municipio" },
    ],
    rowCallback: function (row, data) {
      const icon = $("<i>").addClass("fa fa-file-text");
      let button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon);
      button.on("click", function () {
        const id = $(this).attr("data-id");
        $("#id-input").val(id);
        $("#localizacaoBtn").trigger("click");
      });
      $("td:eq(0)", row).html(button);
    },
  });

  await localizacaoTb.initialize();
}

function configurarModal() {
  const pageCtn = document.getElementById("pageCtn");

  const modalCadastrar = document.getElementById("localizacaoModalHidden");
  const contentCadastrar = document.getElementById(
    "localizacaoModalHiddenContent"
  );

  const buttonsCadastrarConfig = [
    {
      id: "localizacaoCadastrarBtn",
      onClick: async () => await upsert(),
    },
  ];

  const modalsConfig = [
    {
      id: "localizacaoModal",
      modal_hidden: modalCadastrar,
      content: contentCadastrar,
      btnsConfigs: buttonsCadastrarConfig,
      onOpen: async () => {
        const id = $("#id-input").val();
        if (id) {
          await obterPorId(id);
        }
      },
      onClose: () => limpar(),
    },
  ];

  renderizarModal(pageCtn, modalsConfig);
}

async function upsert() {
  const id = $("#id-input").val();

  return id ? await editar(id) : await incluir();
}

async function incluir() {
  const requestData = {
    module: "localizacao",
    action: "addLocalizacao",
    "cod-inputc": $("#cod-input").val(),
    "pais-inputc": $("#pais-input").val(),
    "reg-inputc": $("#reg-input").val(),
    "regiao-inputc": $("#regiao-input").val(),
    "coduf-inputc": $("#coduf-input").val(),
    "est-inputc": $("#est-input").val(),
    "uf-inputc": $("#uf-input").val(),
    "codmun-inputc": $("#codmun-input").val(),
    "mun-inputc": $("#mun-input").val(),
  };

  const result = await localizacaoService.incluir(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Cadastro realizado com sucesso!",
  });

  limpar();

  return Result.success();
}

async function editar(id) {
  const requestData = {
    module: "localizacao",
    action: "updateLocalizacao",
    "cod-inputu": $("#cod-input").val(),
    "pais-inputu": $("#pais-input").val(),
    "reg-inputu": $("#reg-input").val(),
    "regiao-inputu": $("#regiao-input").val(),
    "coduf-inputu": $("#coduf-input").val(),
    "est-inputu": $("#est-input").val(),
    "uf-inputu": $("#uf-input").val(),
    "codmun-inputu": $("#codmun-input").val(),
    "mun-inputu": $("#mun-input").val(),
    id_localizacao: id,
  };

  const result = await localizacaoService.editar(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: "Erro", message: result.error });
    return result;
  }

  NotificationService.success({
    title: "Sucesso",
    message: "Cadastro atualizado com sucesso!",
  });

  limpar();

  return Result.success();
}

$("#btn-atualizar").on("click", function () {
  var id = $("#id-inputu").val();
  editar(id);
});

async function obterPorId(id) {
  limparFormulario();

  const response = await localizacaoService.obterPorId(id);

  if (!response) {
    NotificationService.error({
      title: "Erro",
      message: "Localização não foi encontrada",
    });
    return;
  }

  $("#id-input").val(response.value.id);
  $("#cod-input").val(response.value.codigoPais);
  $("#pais-input").val(response.value.pais);
  $("#reg-input").val(response.value.codigoRegiao);
  $("#regiao-input").val(response.value.regiao);
  $("#coduf-input").val(response.value.codigoUf);
  $("#est-input").val(response.value.estado);
  $("#uf-input").val(response.value.uf);
  $("#codmun-input").val(response.value.codigoMunicipio);
  $("#mun-input").val(response.value.municipio);
}

function limpar() {
  limparFormulario();
  localizacaoTb?.reload();
}

function limparFormulario() {
  $("#formLocalizacao").trigger("reset");
}
