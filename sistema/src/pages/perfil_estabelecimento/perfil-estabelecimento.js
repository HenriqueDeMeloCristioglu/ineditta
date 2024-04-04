import $, { param } from 'jquery';
import jQuery from 'jquery';
import 'bootstrap';
import '../../js/utils/util.js';
import Masker from '../../js/utils/masks/masker.js';

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

// Services
import {
  UsuarioAdmService,
  ClienteUnidadeService,
  DocSindService,
  SindicatoService
} from '../../js/services/index.js'
import NotificationService from "../../js/utils/notifications/notification.service.js";
import { ModalInfoSindicato } from '../../js/utils/components/modalInfoSindicatos/modal-info-sindicatos.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import { div } from '../../js/utils/components/elements/div.js';
import { stringI } from '../../js/utils/components/string-elements/string-i.js';
import PageWrapper from '../../js/utils/pages/page-wrapper.js';
import { MediaType } from '../../js/utils/web/media-type.js';

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const usuarioAdmSerivce = new UsuarioAdmService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const docSindService = new DocSindService(apiService);
const sindicatoService = new SindicatoService(apiService, apiLegadoService);

let informacoesEstabelecimentosTb = null;
let documentosSindicaisProcessadosTb = null;
let documentosSindicaisGeraisTb = null;

let estabelecimentoSelect = null;

let reloadGridsTimeout = null;

jQuery(async () => {
  await AuthService.initialize();

  new Menu()

  const dadosPessoais = await usuarioAdmSerivce.obterDadosPessoais();

  await configurarFiltro()

  await carregarInformacoesEstabelecimentosTb()
  await carregardocumentosSindicaisProcessadosTb()
  await carregardocumentosSindicaisGeraisTb()
  configurarInformacaoSindicatoModal()
});

async function configurarFiltro() {
    estabelecimentoSelect = new SelectWrapper('#unidade',
    {
        options: {
            placeholder: 'Selecione', multiple: true
        },
        onChange: async () => {
            if (reloadGridsTimeout != null) {
                clearTimeout(reloadGridsTimeout);
                reloadGridsTimeout = null;
            }
            reloadGridsTimeout = setTimeout(async () => {
                await carregarInformacoesEstabelecimentosTb();
                await carregardocumentosSindicaisGeraisTb();
                await carregardocumentosSindicaisProcessadosTb();
            }, 1500)
        },
        onOpened: async (matrizId) => await clienteUnidadeService.obterSelectPorUsuario(matrizId)
    });

    estabelecimentoSelect?.disable();
    const options = await estabelecimentoSelect?.loadOptions();
    if (options instanceof Array && options?.length == 1) {
        estabelecimentoSelect?.clear();
        estabelecimentoSelect?.setCurrentValue(options);
    }
    else {
        estabelecimentoSelect?.enable();
    }
}

async function carregarInformacoesEstabelecimentosTb() {
    if (informacoesEstabelecimentosTb) {
      informacoesEstabelecimentosTb.reload();
      return;
    }
  
    informacoesEstabelecimentosTb = new DatatableWrapper("#informacoesEstabelecimentosTb", {
        columns: [
            { data: 'codigoEstabelecimento', title: "Código" },
            { data: 'nomeEstabelecimento', title: "Estabelecimento" },
            { data: 'cnpjEstabalecimento', title: "CNPJ", render: (data) => Masker.CNPJ(data) },
            { data: 'codigoSindicatoCliente', title: "Código Sindicato"},
            { data: 'datasBases', title: "Data-base" },
            { data: 'sindicatosLaboraisSiglas', title: "Sind. Laboral" },
            { data: 'sindicatosPatronaisSiglas', title: "Sind. Patronal "}
        ],
        ajax: async (params) => {
            params.UnidadesIds = estabelecimentoSelect?.getValue();
            return await clienteUnidadeService.obterInformacoesEstabelecimentosDatatable(params);
        },
        columnDefs: [
            {
                "targets": "_all",
                "defaultContent": ""
            }
        ],
        rowCallback: function (row, data) {
            let linksSindicatosLaborais = [];
            let linksSindicatosPatronais = [];

            if (data?.sindicatosLaborais instanceof Array) {
                linksSindicatosLaborais = data?.sindicatosLaborais.map((sindicato) => {
                    let link = $("<a>")
                        .addClass("btn-info-sindicato")
                        .attr("data-id", sindicato?.id) //data?.idSindicatoLaboral
                        .html(sindicato?.sigla);
                    link.on("click", function () {
                        const id = $(this).attr("data-id");
                        $("#sind-id-input").val(id);
                        $("#tipo-sind-input").val("laboral");
                        $("#openInfoSindModalBtn").trigger("click");
                    });
                    return link;
                });
            }

            if (data?.sindicatosPatronais instanceof Array) {
                linksSindicatosPatronais = data?.sindicatosPatronais.map((sindicato) => {
                    let link = $("<a>")
                        .addClass("btn-info-sindicato")
                        .attr("data-id", sindicato?.id) //data?.idSindicatoLaboral
                        .html(sindicato?.sigla);
                    link.on("click", function () {
                        const id = $(this).attr("data-id");
                        $("#sind-id-input").val(id);
                        $("#tipo-sind-input").val("patronal");
                        $("#openInfoSindModalBtn").trigger("click");
                    });
                    return link;
                });
            }
            
            $("td:eq(5)", row).html("");
            linksSindicatosLaborais.forEach((link, index) => {
                $("td:eq(5)", row).append(link);
                if (index < linksSindicatosLaborais.length - 1) {
                    $("td:eq(5)", row).append(",");
                }
            })

            $("td:eq(6)", row).html("");
            linksSindicatosPatronais.forEach((link, index) => {
                $("td:eq(6)", row).append(link);
                if (index < linksSindicatosPatronais.length - 1) {
                    $("td:eq(6)", row).append(",");
                }
            })
        },
    });
  
    await informacoesEstabelecimentosTb.initialize();
}

async function carregardocumentosSindicaisProcessadosTb() {
    if (documentosSindicaisProcessadosTb) {
      documentosSindicaisProcessadosTb.reload();
      return;
    }
  
    documentosSindicaisProcessadosTb = new DatatableWrapper("#documentosSindicaisProcessadosTb", {
        columns: [
            { data: 'municipiosEstabelecimentos', title: "Municípios Estabelecimentos" },
            { data: 'nome', title: "Nome documento" },
            { data: 'cnpjEstabalecimento', title: "Vigência" },
            { data: 'assuntos', title: "Assuntos"},
            { data: 'comentarios', title: "Comentários" },
            { data: 'siglasSindicatosLaborais', title: "Sind. Laboral" },
            { data: 'siglasSindicatosPatronais', title: "Sind. Patronal "},
            { data: 'dataInclusao', title: "Data inclusão"},
            { data: 'id', title: "VER"},
            { data: 'id', title: "BAIXAR"}
        ],
        ajax: async (params) => {
            params.UnidadesIds = estabelecimentoSelect?.getValue();
            params.TipoConsulta = "processados";
            return await docSindService.obterDatatableConsulta(params)
        },
        columnDefs: [
            {
                "targets": "_all",
                "defaultContent": ""
            }
        ],
        responsive: false,
        scrollX: true,
        rowCallback: function (row, data) {
            let linksSindicatosLaborais = [];
            let linksSindicatosPatronais = [];

            if (data?.siglasSindicatosLaborais instanceof Array && data?.sindicatosLaboraisIds instanceof Array) {
                linksSindicatosLaborais = data?.siglasSindicatosLaborais.map((sigla, index) => {
                    let link = $("<a>")
                        .addClass("btn-info-sindicato")
                        .attr("data-id", data?.sindicatosLaboraisIds[index]) //data?.idSindicatoLaboral
                        .html(sigla);
                    link.on("click", function () {
                        const id = $(this).attr("data-id");
                        $("#sind-id-input").val(id);
                        $("#tipo-sind-input").val("laboral");
                        $("#openInfoSindModalBtn").trigger("click");
                    });
                    return link;
                });
            }

            if (data?.siglasSindicatosPatronais instanceof Array && data?.sindicatosPatronaisIds instanceof Array) {
                linksSindicatosPatronais = data?.siglasSindicatosPatronais.map((sigla, index) => {
                    let link = $("<a>")
                        .addClass("btn-info-sindicato")
                        .attr("data-id", data?.sindicatosPatronaisIds[index]) //data?.idSindicatoLaboral
                        .html(sigla);
                    link.on("click", function () {
                        const id = $(this).attr("data-id");
                        $("#sind-id-input").val(id);
                        $("#tipo-sind-input").val("patronal");
                        $("#openInfoSindModalBtn").trigger("click");
                    });
                    return link;
                });
            }

            if (data?.arquivo) {
                const icon = `<i class="fa fa-file-text"></i>`

                const link = $("<div>")
                    .on('click', () => verDocumento(data?.id))
                    .html(icon);

                $("td:eq(8)", row).html(link);

                const linkBaixar = div({
                    content: stringI({ className: 'fa fa-download' })
                }).on('click', () => downloadDoc(data?.id))

                $("td:eq(9)", row).html(linkBaixar)
            }

            if (data?.dataValidadeInicial) {
                let vigencia = DatatableWrapper.formatDate(data?.dataValidadeInicial);

                vigencia += data?.dataValidadeFinal ? ` até ${DatatableWrapper.formatDate(data?.dataValidadeFinal)}` : ' até (não informado)';

                $("td:eq(2)", row).html(vigencia);
            }

            if (data?.assuntos instanceof Array) {
                $("td:eq(3)", row).html(data.assuntos.join(", "));
            }

            if (data?.dataInclusao) {
                $("td:eq(7)", row).html(DatatableWrapper.formatDate(data?.dataInclusao));
            }
            
            $("td:eq(5)", row).html("");
            linksSindicatosLaborais.forEach((link, index) => {
                $("td:eq(5)", row).append(link);
                if (index < linksSindicatosLaborais.length - 1) {
                    $("td:eq(5)", row).append(",");
                }
            })

            $("td:eq(6)", row).html("");
            linksSindicatosPatronais.forEach((link, index) => {
                $("td:eq(6)", row).append(link);
                if (index < linksSindicatosPatronais.length - 1) {
                    $("td:eq(6)", row).append(",");
                }
            })
        },
    });
  
    await documentosSindicaisProcessadosTb.initialize();
}

async function carregardocumentosSindicaisGeraisTb() {
    if (documentosSindicaisGeraisTb) {
      documentosSindicaisGeraisTb.reload();
      return;
    }
  
    documentosSindicaisGeraisTb = new DatatableWrapper("#documentosSindicaisGeraisTb", {
        columns: [
            { data: 'municipiosEstabelecimentos', title: "Municípios Estabelecimentos" },
            { data: 'nome', title: "Nome documento" },
            { data: 'cnpjEstabalecimento', title: "Vigência" },
            { data: 'assuntos', title: "Assuntos"},
            { data: 'comentarios', title: "Comentários" },
            { data: 'siglasSindicatosLaborais', title: "Sind. Laboral" },
            { data: 'siglasSindicatosPatronais', title: "Sind. Patronal "},
            { data: 'dataInclusao', title: "Data inclusão"},
            { data: 'id', title: "VER"},
            { data: 'id', title: "BAIXAR"}
        ],
        ajax: async (params) => {
            params.UnidadesIds = estabelecimentoSelect?.getValue();
            params.TipoConsulta = "geral";
            return await docSindService.obterDatatableConsulta(params)
        },
        columnDefs: [
            {
                "targets": "_all",
                "defaultContent": ""
            }
        ],
        responsive: false,
        scrollX: true,
        rowCallback: function (row, data) {
            let linksSindicatosLaborais = [];
            let linksSindicatosPatronais = [];

            if (data?.siglasSindicatosLaborais instanceof Array && data?.sindicatosLaboraisIds instanceof Array) {
                linksSindicatosLaborais = data?.siglasSindicatosLaborais.map((sigla, index) => {
                    let link = $("<a>")
                        .addClass("btn-info-sindicato")
                        .attr("data-id", data?.sindicatosLaboraisIds[index]) //data?.idSindicatoLaboral
                        .html(sigla);
                    link.on("click", function () {
                        const id = $(this).attr("data-id");
                        $("#sind-id-input").val(id);
                        $("#tipo-sind-input").val("laboral");
                        $("#openInfoSindModalBtn").trigger("click");
                    });
                    return link;
                });
            }

            if (data?.siglasSindicatosPatronais instanceof Array && data?.sindicatosPatronaisIds instanceof Array) {
                linksSindicatosPatronais = data?.siglasSindicatosPatronais.map((sigla, index) => {
                    let link = $("<a>")
                        .addClass("btn-info-sindicato")
                        .attr("data-id", data?.sindicatosPatronaisIds[index]) //data?.idSindicatoLaboral
                        .html(sigla);
                    link.on("click", function () {
                        const id = $(this).attr("data-id");
                        $("#sind-id-input").val(id);
                        $("#tipo-sind-input").val("patronal");
                        $("#openInfoSindModalBtn").trigger("click");
                    });
                    return link;
                });
            }

            if (data?.arquivo) {
                const icon = `<i class="fa fa-file-text"></i>`

                const link = $("<div>")
                    .on('click', () => verDocumento(data?.id))
                    .html(icon);

                $("td:eq(8)", row).html(link);

                const linkBaixar = div({
                    content: stringI({ className: 'fa fa-download' })
                }).on('click', () => downloadDoc(data?.id))

                $("td:eq(9)", row).html(linkBaixar)
            }

            if (data?.dataValidadeInicial) {
                let vigencia = DatatableWrapper.formatDate(data?.dataValidadeInicial);

                vigencia += data?.dataValidadeFinal ? ` até ${DatatableWrapper.formatDate(data?.dataValidadeFinal)}` : ' até (não informado)';

                $("td:eq(2)", row).html(vigencia);
            }

            if (data?.assuntos instanceof Array) {
                $("td:eq(3)", row).html(data.assuntos.join(", "));
            }

            if (data?.dataInclusao) {
                $("td:eq(7)", row).html(DatatableWrapper.formatDate(data?.dataInclusao));
            }
            
            $("td:eq(5)", row).html("");
            linksSindicatosLaborais.forEach((link, index) => {
                $("td:eq(5)", row).append(link);
                if (index < linksSindicatosLaborais.length - 1) {
                    $("td:eq(5)", row).append(",");
                }
            })

            $("td:eq(6)", row).html("");
            linksSindicatosPatronais.forEach((link, index) => {
                $("td:eq(6)", row).append(link);
                if (index < linksSindicatosPatronais.length - 1) {
                    $("td:eq(6)", row).append(",");
                }
            })
        },
    });
  
    await documentosSindicaisGeraisTb.initialize();
}

function configurarInformacaoSindicatoModal() {
    const infoModal = new ModalInfoSindicato(renderizarModal,sindicatoService, DatatableWrapper);
    infoModal.initialize("info-sindicato-modal-container");
}

async function verDocumento(id) {
    const response = await docSindService.download({ id })

    if (response.isFailure()) {
        return NotificationService.error({ title: 'Não foi possível baixar o arquivo.' })
    }

    PageWrapper.preview(response.value.data.blob, MediaType.pdf.Accept)
}

async function downloadDoc(id) {
    const response = await docSindService.download({ id })

    if (response.isFailure()) {
        return NotificationService.error({ title: 'Não foi possível baixar o arquivo.' })
    }

    PageWrapper.download(response.value.data.blob, response.value.data.filename, response.value.contentType)
}