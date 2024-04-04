// Libs
import jQuery from 'jquery';
import $ from 'jquery';
import 'bootstrap';

import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import "../../js/utils/masks/jquery-mask-extensions.js";

// Utils
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import SelectWrapper from '../../js/utils/selects/select-wrapper.js';
import DatepickerrangeWrapper from '../../js/utils/daterangepicker/daterangepicker-wrapper.js';

// Core
import { AuthService, ApiService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js';

// Services
import {
    ClienteUnidadeService,
    DocSindService,
    GrupoEconomicoService,
    UsuarioAdmService,
    TipoDocService,
    CnaeService,
    EstruturaClausulaService,
    LocalizacaoService,
    MatrizService,
    SindicatoPatronalService,
    SindicatoLaboralService,
    SindicatoService
} from '../../js/services'

// Application
import { UsuarioNivel } from '../../js/application/usuarios/constants/usuario-nivel.js';

import {
    renderizarModal
} from "../../js/utils/modals/modal-wrapper.js";

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import { div } from '../../js/utils/components/elements/div.js';
import { stringI } from '../../js/utils/components/string-elements/string-i.js';
import PageWrapper from '../../js/utils/pages/page-wrapper.js';
import NotificationService from '../../js/utils/notifications/notification.service.js';
import { MediaType } from '../../js/utils/web/media-type.js';
import { ModalInfoSindicato } from '../../js/utils/components/modalInfoSindicatos/modal-info-sindicatos.js';

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();

const documentoSindicalService = new DocSindService(apiService);
const tipoDocumentoService = new TipoDocService(apiService);
const localizacaoService = new LocalizacaoService(apiService);
const cnaeService = new CnaeService(apiService);
const grupoEconomicoService = new GrupoEconomicoService(apiService);
const matrizService = new MatrizService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const sindicatoPatronalService = new SindicatoPatronalService(apiService, apiLegadoService);
const sindicatoLaboralService = new SindicatoLaboralService(apiService, apiLegadoService);
const estruturaClausulaService = new EstruturaClausulaService(apiService);
const usuarioAdmSerivce = new UsuarioAdmService(apiService);
const sindicatoService = new SindicatoService(apiService, apiLegadoService);

let documentoTable = null;
let filtros = {};

let tipoConsultaSelect = null;
let tipoDocumentoSelect = null;
let localidadeSelect = null;
let atividadeEconomicaSelect = null;
let grupoEconomicoSelect = null;
let matrizSelect = null;
let estabelecimentoSelect = null;
let sindicatoPatronalSelect = null;
let sindicatoLaboralSelect = null;
let dataBaseSelect = null;
let assuntosSelect = null;
let nomeDocumentoSelect = null;
let periodo = null;

jQuery(async () => {
    new Menu()

    $('#exibirDocumentosDiv').hide();

    await AuthService.initialize();

    const dadosPesoais = await usuarioAdmSerivce.obterDadosPessoais();

    if (dadosPesoais.isFailure()) {
        return;
    }

    configurarModal();

    await configurarFormulario(dadosPesoais.value);

    await carregarValoresIniciais();

    await consultarUrl();

    $('.form-horizontal').on('submit', (e) => e.preventDefault());

    $('.horizontal-nav').removeClass('hide');
});

function configurarModal() {
    $("#infoSindForm").on('submit', (e) => e.preventDefault());
    configurarInformacaoSindicatoService();
}

async function configurarFormulario(dadosPessoais) {
    const isIneditta = dadosPessoais.nivel == UsuarioNivel.Ineditta;
    const isGrupoEconomico = dadosPessoais.nivel == UsuarioNivel.GrupoEconomico;
    const isEstabelecimento = dadosPessoais.nivel == UsuarioNivel.Estabelecimento;

    const markOptionAsSelectable = dadosPessoais.nivel == 'Cliente' ? () => true : () => false;

    tipoConsultaSelect = new SelectWrapper('#tipo_doc', { 
        options: { placeholder: 'Selecione', allowEmpty: true }, 
        onOpened: async () => await obterTiposConsultas(),
        onChange: async (value) => {
            await mediatorFiltersActions('tipoConsultaChange', value);
        },
        onSelected: tipoConsultaSelecionada, 
        markOptionAsSelectable: () => false
    });

    tipoDocumentoSelect = new SelectWrapper('#tipo_documento', { 
        options: { placeholder: 'Selecione', multiple: true }, 
        onOpened: async () => (await tipoDocumentoService.obterTiposSelect()).value, 
        onChange: async () => await mediatorFiltersActions('tipoDocumentoChange'),
        markOptionAsSelectable: markOptionAsSelectable, 
        sortable: true 
    });

    localidadeSelect = new SelectWrapper('#localidade', {
        options: { placeholder: 'Selecione', multiple: true },
        onChange: async () => {
            await mediatorFiltersActions('localidadeChange');
        },
        onOpened: async () => await obterLocalidades(),
        markOptionAsSelectable: markOptionAsSelectable
    });

    atividadeEconomicaSelect = new SelectWrapper('#categoria', {
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
            await mediatorFiltersActions('atividadeEconomicaChange');
        },
        markOptionAsSelectable: markOptionAsSelectable
    });

    grupoEconomicoSelect = new SelectWrapper('#grupo', {
        options: { placeholder: 'Selecione', multiple: true },
        onChange: async () => {
            await mediatorFiltersActions('grupoEconomicoChange');
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
            await mediatorFiltersActions('matrizChange');
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
            await mediatorFiltersActions('estabelecimentoChange');
        },
        onOpened: async (matrizId) => await clienteUnidadeService.obterSelectPorUsuario(matrizId),
        markOptionAsSelectable: isEstabelecimento ? () => true : () => false
    });
    if (isEstabelecimento) {
        const options = await estabelecimentoSelect.loadOptions()
        if (!(options instanceof Array && options.length > 1)) {
            estabelecimentoSelect.disable();
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
        onChange: async () => {
            await mediatorFiltersActions('sindicatoPatronalChange');
        },
        markOptionAsSelectable: markOptionAsSelectable
    });

    sindicatoLaboralSelect = new SelectWrapper('#sindicatoLaboral', {
        options: { placeholder: 'Selecione', multiple: true },
        onOpened: async () => {
            const options = obterParametrosParaRequisicaoDeSindicatos();
            return await sindicatoLaboralService.obterSelectPorUsuario(options)
        },
        onChange: async () => {
            await mediatorFiltersActions('sindicatoLaboralChange');
        },
        markOptionAsSelectable: markOptionAsSelectable
    });

    dataBaseSelect = new SelectWrapper('#dataBase', { options: { placeholder: 'Selecione', multiple: true }, onOpened: async () =>  await obterDatasBases()});
    assuntosSelect = new SelectWrapper('#assuntos', { options: { placeholder: 'Selecione', multiple: true }, onOpened: async () => (await estruturaClausulaService.obterSelect())?.value, markOptionAsSelectable: markOptionAsSelectable });
    nomeDocumentoSelect = new SelectWrapper('#nome_doc', {
        options: { placeholder: 'Selecione', multiple: true }, onOpened: async () => {
            const isProcessado = $("#tipo_doc").val() === 'processado';
            const tiposDocumentosIds = tipoDocumentoSelect?.getValue();
            return await tipoDocumentoService.obterSelectPorTipos({ tiposDocumentosIds, processado: isProcessado })
        }
    });
    periodo = new DatepickerrangeWrapper('#vigencia');

    $('#limparBtn').on('click', async () => await limpar());
    $('#filtrarBtn').on('click', async () => await filtrar());
}

async function obterDatasBases() {
    const customDataBaseOptions = [
        {id: "vigente", description: "Vigente"}
    ];

    const customParams = {
        sindLaboralIds: sindicatoLaboralSelect?.getValue(),
        sindPatronalIds: sindicatoPatronalSelect?.getValue(),
        porUsuario: true
    }

    const result = await documentoSindicalService.obterSelectAnosMeses(customParams);
    result.unshift(...customDataBaseOptions);
    return result;
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
        cnaesIds: $("#categoria").val() ?? null,
        localizacoesIds,
        ufs
    }

    return options;
}

async function carregarValoresIniciais() {
    // const tiposConsultas = await obterTiposConsultas();

    // tipoConsultaSelect.setCurrentValue(tiposConsultas[0]);
}

async function obterTiposConsultas() {
    return await Promise.resolve([
        { id: 'processado', description: 'Documento Processado' },
        { id: 'geral', description: 'Documento Não Processado' },
    ]);
}

function tipoConsultaSelecionada(tipoDocumento) {
    if (tipoDocumento?.id == "geral") {
        $("#restrito").removeAttr("disabled");
        $("#anu_obrigatoria").removeAttr("disabled");
        $("#data_base").attr("disabled", "disabled");
        $("#tipo_documento").prop("disabled", false)
        $("#box_tipo_doc").show();
    } else {
        $("#data_base").removeAttr("disabled");
        $("#anu_obrigatoria").attr("disabled", "disabled");

        $("#tipo_documento").prop("disabled", true);
        $("#tipo_documento").val('');
        $("#box_tipo_doc").hide();
    }

    $('#nome_doc').val('');
    $('#tipo_documento').val('');
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

async function carregarDatatable() {
    if (documentoTable) {
        await documentoTable.reload();
        return;
    }

    documentoTable = new DataTableWrapper('#documentoTb', {
        columns: [
            { data: 'nome' },
            { data: 'assuntos' },
            { data: 'siglasSindicatosLaborais' },
            { data: 'siglasSindicatosPatronais' },
            { data: 'atividadesEconomicas' },
            { data: 'dataValidadeInicial' },
            { data: 'comentarios' },
            { data: 'dataInclusao', render: (data) => DataTableWrapper.formatDate(data) },
            { data: 'id' },
            { data: 'id' },
        ],
        ajax: async (params) => {
            params = { ...params, ...filtros };
            return await documentoSindicalService.obterDatatableConsulta(params);
        },
        columnDefs: [
            {
                "targets": "_all",
                "defaultContent": ""
            }
        ],
        scrollX: true,
        responsive: false,
        rowCallback: function (row, data) {
            let linksSindicatosLaborais = [];
            let linksSindicatosPatronais = [];

            if (data?.siglasSindicatosLaborais instanceof Array && data?.sindicatosLaboraisIds instanceof Array) {
                linksSindicatosLaborais = data?.siglasSindicatosLaborais.map((sigla, index) => {
                    let link = $("<button>")
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
                    let link = $("<button>")
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
                let vigencia = DataTableWrapper.formatDate(data?.dataValidadeInicial);

                vigencia += data?.dataValidadeFinal ? ` até ${DataTableWrapper.formatDate(data?.dataValidadeFinal)}` : ' até (não informado)';

                $("td:eq(5)", row).html(vigencia);
            }

            if (data?.atividadesEconomicas instanceof Array) {
                $("td:eq(4)", row).html(data.atividadesEconomicas.map(x => x.subclasse).join('; '));
            }

            if (data?.assuntos instanceof Array) {
                $("td:eq(1)", row).html(data.assuntos.join(", "));
            }
            
            $("td:eq(2)", row).html("");
            linksSindicatosLaborais.forEach((link, index) => {
                $("td:eq(2)", row).append(link);
                if (index < linksSindicatosLaborais.length - 1) {
                    $("td:eq(2)", row).append(",");
                }
            })

            $("td:eq(3)", row).html("");
            linksSindicatosPatronais.forEach((link, index) => {
                $("td:eq(3)", row).append(link);
                if (index < linksSindicatosPatronais.length - 1) {
                    $("td:eq(3)", row).append(",");
                }
            })
        },
    });

    await documentoTable.initialize();
}

function limparFormulario() {
    tipoConsultaSelect?.clear();
    tipoDocumentoSelect?.clear();
    localidadeSelect?.clear();
    atividadeEconomicaSelect?.clear();
    grupoEconomicoSelect?.clear();
    //matrizSelect?.clear();
    //estabelecimentoSelect?.clear();
    sindicatoPatronalSelect?.clear();
    sindicatoLaboralSelect?.clear();
    dataBaseSelect?.clear();
    assuntosSelect?.clear();
    nomeDocumentoSelect?.clear();

    $('#restrito').val(false);
    $('#anu_obrigatoria').val(false);

    console.log(matrizSelect.isEnable())

    if(matrizSelect?.isEnable()) {
        matrizSelect?.clear();
    }

    if(estabelecimentoSelect?.isEnable()) {
        estabelecimentoSelect?.clear();
    }

    filtros = {};
}

async function limpar() {
    limparFormulario();
    $('#exibirDocumentosDiv').hide();

    await carregarValoresIniciais();
}

async function filtrar() {
    filtros = {};

    if (tipoDocumentoSelect?.getValue()) {
        filtros['tiposDocumentos'] = Array.isArray(tipoDocumentoSelect?.getValue()) ? tipoDocumentoSelect?.getValue() : [tipoDocumentoSelect?.getValue()];
    }

    if (nomeDocumentoSelect?.getValue()) {
        filtros['nomesDocumentos'] = nomeDocumentoSelect?.getValue();
        //filtros['nomesDocumentos'] = Array.isArray(nomeDocumentoSelect?.getValue()) ? nomeDocumentoSelect?.getValue() : [nomeDocumentoSelect?.getValue()];
    }

    if (localidadeSelect?.getValue()) {

        if (localidadeSelect.getValue().some(localidade => localidade.indexOf('municipio:') > -1)) {
            const municipios = localidadeSelect.getValue().filter(localidade => localidade.indexOf('municipio:') > -1).map(municipio => municipio.split(':')[1]);
            filtros['municipiosIds'] = Array.isArray(municipios) ? municipios : [municipios];
        }

        if (localidadeSelect.getValue().some(localidade => localidade.indexOf('uf:') > -1)) {
            const ufs = localidadeSelect.getValue().filter(localidade => localidade.indexOf('uf:') > -1).map(value => value.split(':')[1]);
            filtros['ufs'] = Array.isArray(ufs) ? ufs : [ufs];
        }
    }

    if (estabelecimentoSelect?.getValue()) {
        filtros['unidadesIds'] = Array.isArray(estabelecimentoSelect?.getValue()) ? estabelecimentoSelect?.getValue() : [estabelecimentoSelect?.getValue()];
    }

    if (matrizSelect?.getValue()) {
        filtros['matrizesIds'] = Array.isArray(matrizSelect?.getValue()) ? matrizSelect?.getValue() : [matrizSelect?.getValue()];
    }

    if (grupoEconomicoSelect?.getValue()) {
        filtros['gruposEconomicosIds'] = Array.isArray(grupoEconomicoSelect?.getValue()) ? grupoEconomicoSelect?.getValue() : [grupoEconomicoSelect?.getValue()];
    }

    if (atividadeEconomicaSelect?.getValue()) {
        filtros['cnaesIds'] = Array.isArray(atividadeEconomicaSelect?.getValue()) ? atividadeEconomicaSelect?.getValue() : [atividadeEconomicaSelect?.getValue()];
    }

    if (sindicatoPatronalSelect?.getValue()) {
        filtros['sindicatosPatronaisIds'] = Array.isArray(sindicatoPatronalSelect?.getValue()) ? sindicatoPatronalSelect?.getValue() : [sindicatoPatronalSelect?.getValue()];
    }

    if (sindicatoLaboralSelect?.getValue()) {
        filtros['sindicatosLaboraisIds'] = Array.isArray(sindicatoLaboralSelect?.getValue()) ? sindicatoLaboralSelect?.getValue() : [sindicatoLaboralSelect?.getValue()];
    }

    if (dataBaseSelect?.getValue()) {
        filtros['datasBases'] = Array.isArray(dataBaseSelect?.getValue()) ? dataBaseSelect?.getValue() : [dataBaseSelect?.getValue()];
    }

    if ($('#restrito').val()) {
        filtros['restrito'] = $('#restrito').prop("checked") === true;
    }

    if ($('#anu_obrigatoria').val()) {
        filtros['anuenciaObrigatoria'] = $('#anu_obrigatoria').prop("checked") === true;
    }

    if (assuntosSelect?.getValue()) {
        filtros['assuntosIds'] = Array.isArray(assuntosSelect?.getValue()) ? assuntosSelect?.getValue() : [assuntosSelect?.getValue()];
    }

    if (tipoConsultaSelect?.getValue()) {
        filtros['tipoConsulta'] = tipoConsultaSelect?.getValue();
    }
    else {
        NotificationService.error({title: "Você precisa fornecer o tipo de consulta"});
        return;
    }

    if (periodo?.hasValue()) {
        filtros['dataValidadeInicial'] = periodo.getBeginDate();
        filtros['dataValidadeFinal'] = periodo.getEndDate();
    }

    $('#exibirDocumentosDiv').show();

    await carregarDatatable();
}

async function consultarUrl() {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);

    if (!urlParams.has('sindId') || !urlParams.has('tipoSind')) {
        return;
    }

    const sindId = urlParams.get('sindId');
    const tipoSind = urlParams.get('tipoSind');
    const sigla = urlParams.get('sigla');

    const sindOption = {
        description: sigla,
        id: sindId,
    };

    if (tipoSind == 'laboral') {
        sindicatoLaboralSelect.setCurrentValue(sindOption);
    } else if (tipoSind == 'patronal') {
        sindicatoPatronalSelect.setCurrentValue(sindOption);
    }

    const tipoOption = {
        description: 'Documento Processado',
        id: 'processado'
    }

    tipoConsultaSelect.setCurrentValue(tipoOption);

    await filtrar();
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

    PageWrapper.download(response.value.data.blob, response.value.data.filename, response.value.contentType)
}

async function mediatorFiltersActions(event, params = null) {
    const actionsDictionary = {
        sindicatoLaboralChange: async () => {
            await dataBaseSelect?.reload();
        },
        sindicatoPatronalChange: async () => {
            await dataBaseSelect?.reload();
        },
        localidadeChange: async () => {
            await sindicatoLaboralSelect.reload();
            await sindicatoPatronalSelect.reload();
        },
        atividadeEconomicaChange: async () => {
            await sindicatoLaboralSelect.reload();
            await sindicatoPatronalSelect.reload();
        },
        grupoEconomicoChange: async () => {
            await atividadeEconomicaSelect.reload();
            await sindicatoLaboralSelect.reload();
            await sindicatoPatronalSelect.reload();
        },
        matrizChange: async () => {
            await atividadeEconomicaSelect.reload();
            await sindicatoLaboralSelect.reload();
            await sindicatoPatronalSelect.reload();
        },
        estabelecimentoChange: async () => {
            await atividadeEconomicaSelect.reload();
            await sindicatoLaboralSelect.reload();
            await sindicatoPatronalSelect.reload();
        },
        tipoDocumentoChange: async () => {
            await nomeDocumentoSelect?.reload();
        },
        tipoConsultaChange: async (value) => {
            if (value?.id == 'geral') {
                $("#box-data-base-select").hide();
                dataBaseSelect?.clear();
            }
            else $("#box-data-base-select").show();

            await nomeDocumentoSelect?.reload();
            await tipoDocumentoSelect?.reload();
        }
    }

    const action = actionsDictionary[event];

    if (!action) return;

    if (action && params) {
        await action(params);
    }else {
        await action();
    }
}

function configurarInformacaoSindicatoService() {
    const modalInfoSindicato = new ModalInfoSindicato(renderizarModal,sindicatoService,DataTableWrapper);
    modalInfoSindicato.initialize("info-modal-sindicato-container");
}