import $ from "jquery";
import "../../js/utils/masks/jquery-mask-extensions.js";

import config from "../../assets/configs/config.json";

import { ApiService } from "../../js/core/api";
import { ClienteUnidadeService } from "../../js/services/cliente-unidade-service";

import { renderizarModal } from "../../js/utils/modals/modal-wrapper";
import NotificationService from "../../js/utils/notifications/notification.service.js";
import { UsuarioAdmService } from "../../js/services/usuario-adm-service.js";
import { ApiLegadoService } from "../../js/core/api-legado.js";

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const usuarioAdmService = new UsuarioAdmService(apiService, apiLegadoService);

let dadosContato = null;

export class Menu {
  constructor() {
    this.init()
  }

  init() {
    executarLegado();

    configurarModal();

    function configurarModal() {
      const pageCtn = document.getElementById("pageCtnMenu");

      const modalInfo = document.getElementById("infoConsultorModalHidden");
      const contentInfo = document.getElementById(
        "infoConsultorModalHiddenContent"
      );

      const buttonsInfoConfig = [];

      const modalsConfig = [
        {
          id: "infoConsultorModal",
          modal_hidden: modalInfo,
          content: contentInfo,
          btnsConfigs: buttonsInfoConfig,
          onOpen: async () => {
            await carregarDadosContato();
          },
          onClose: () => { },
        },
      ];

      renderizarModal(pageCtn, modalsConfig);
    }

    async function carregarDadosContato() {
      if (dadosContato != null) {
        return;
      }

      const result = await clienteUnidadeService.obterInfoConsultor();

      dadosContato = result.value;

      if (dadosContato == null || dadosContato.length < 1) {
        $("#info-consultor-container").html(
          "<p style='text-align: center; margin-top: 1rem; font-size: 1.2rem;'>Nenhum contato encontrado!</p>"
        );
      }

      dadosContato.forEach((row) => gerarInformacoesHtml(row));

      const modulos = (await usuarioAdmService.obterPermissoes()).value;
      const gestaoDeChamados = modulos.length > 0 ? modulos.find((modulo) => modulo.modulos === "Cliente - Helpdesk") : []
      const exibirGestaoDeChamados = gestaoDeChamados?.consultar === "1" || gestaoDeChamados?.criar === "1"

      if (exibirGestaoDeChamados) {
        const gestaoChamadosLink = $("<a>")
        .attr("href", "https://app.pipefy.com/public/form/HbNWDBuA")
        .attr("target", "_blank")
        .text("Abrir um novo chamado")
        .attr("style", "padding-left: 10px; padding-top: 10px; display: block;");
        const gestaoChamadosRow = $("<div>")
        .addClass("col-sm-12")
        .append(gestaoChamadosLink);
        
        $("#info-consultor-container").append(gestaoChamadosRow);
      }
    }

    function gerarInformacoesHtml(data) {
      const nomeLabel = $("<label>")
        .addClass("col-sm-2")
        .html("<strong>Nome:</strong>");
      const nomeData = $("<span>").addClass("col-sm-10").text(data.nome);
      const nomeRow = $("<div>")
        .addClass("col-sm-12")
        .append(nomeLabel)
        .append(nomeData);

      const foneLabel = $("<label>")
        .addClass("col-sm-2")
        .html("<strong>Telefone:</strong>");
      const foneData = $("<span>").addClass("col-sm-10").text(data.telefone);
      const foneRow = $("<div>")
        .addClass("col-sm-12")
        .append(foneLabel)
        .append(foneData);

      const ramalLabel = $("<label>")
        .addClass("col-sm-2")
        .html("<strong>Ramal:</strong>");
      const ramalData = $("<span>").addClass("col-sm-10").text(data.ramal);
      const ramalRow = $("<div>")
        .addClass("col-sm-12")
        .append(ramalLabel)
        .append(ramalData);

      const group = $("<div>").attr("style", "margin-top: 1rem;");

      group.append(nomeRow).append(foneRow).append(ramalRow);

      $("#info-consultor-container").append(group);
    }

    async function redirecionarParaLegado() {
      return new Promise((resolve) => {
        NotificationService.success({
          title: 'Tem certeza?',
          message: 'Você será redirecionado para a versão anterior do Sistema Ineditta, para acessar é necessário informar o login e senha anterior!',
          showConfirmButton: true,
          showCancelButton: true,
          confirmButtonColor: '#3085d6',
          cancelButtonColor: '#d33',
          confirmButtonText: 'Sim, redirecionar',
          then: async (result) => {
            if (result.isConfirmed) {
    
              let novaAba = window.open();
              novaAba.location.href = config.sistemaLegadoUrl
    
              resolve()
            }
          }
        })
      })
    }

    function executarLegado() {
      $(".dropItem").each(function () {
        if (!$(this).html().includes("<li>")) {
          $(this).css("display", "none");
        }
      });

      $.ajax({
        url: "includes/php/ajax.php",
        type: "post",
        dataType: "json",
        data: {
          module: "profile",
          action: "setPersonal",
          iduser: sessionStorage.getItem("iduser"),
        },
        success: (data) => $("#imglogo").attr("src", data.response_data.logo),
        error: function (_, __, errorThrown) {
          NotificationService.error({
            title: "Não foi possível realizar a personalização de tela! Erro: ",
            message: errorThrown,
          });
        },
      });
    }

    $("#versao-anterior-btn").on("click", async () => await redirecionarParaLegado());
  }
}
