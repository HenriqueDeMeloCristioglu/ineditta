import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';

import $ from 'jquery';
import JQuery from 'jquery';
import '../../js/utils/masks/jquery-mask-extensions.js';

import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';

// Utils
import Notification from '../../js/utils/notifications/notification.service.js'

// Core
import { AuthService, ApiService } from '../../js/core/index.js';
import { ApiLegadoService } from '../../js/core/api-legado.js';

// Services
import {
  DocSindService, 
  ClienteUnidadeService,
  TipoUnidadeClienteService,
  TipoDocService,
  CnaeService,
  DocumentosLocalizadosService,
  EstruturaClausulaService,
  LocalizacaoService,
  SindicatoLaboralService,
  SindicatoPatronalService,
  UsuarioAdmService
} from '../../js/services'

import SelectWrapper from '../../js/utils/selects/select-wrapper.js';
import DatepickerWrapper from '../../js/utils/datepicker/datepicker-wrapper.js';
import DataTableWrapper from '../../js/utils/datatables/datatable-wrapper.js';
import { renderizarModal, closeModal } from '../../js/utils/modals/modal-wrapper.js';
import NotificationService from '../../js/utils/notifications/notification.service.js';
import Result from '../../js/core/result.js';
import { button, div, input } from '../../js/utils/components/elements';
import { stringI } from '../../js/utils/components/string-elements';
import DateFormatter from '../../js/utils/date/date-formatter.js';
import { documentoSindicalOptions, uf } from '../../js/utils/mocks/selects'
import { createSelectOption } from '../../js/utils/selects/create-select-option.js'
import DateParser from '../../js/utils/date/date-parser.js';


import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import { SelectSindicatoLaboralFactory, SelectSindicatoPatronalFactory } from '../../js/modules/documento-sindical-sisap';
import { enviarArquivo } from '../../js/modules/documento-sindical-sisap/features/actions/enviar-arquivo.js';
import { atualizarDataSla, documentosAprovadosOptions, enviarEmailsAprovados, obterPorId } from '../../js/modules/documento-sindical-sisap/index.js';
import { obterUrlDocumento } from '../../js/application/documento-sindical/features/obter-dados/obter-url-documento.js';

const { origem, versao } = documentoSindicalOptions

const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const docSindService = new DocSindService(apiService, apiLegadoService);
const documentosLocalizadosService = new DocumentosLocalizadosService(apiService, apiLegadoService);
const tipoUnidadeClienteService = new TipoUnidadeClienteService(apiService);
const tipoDocService = new TipoDocService(apiService);
const sindicatoLaboralService = new SindicatoLaboralService(apiService, apiLegadoService);
const sindicatoPatronalService = new SindicatoPatronalService(apiService, apiLegadoService);
const estruturaClausulaService = new EstruturaClausulaService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const cnaeService = new CnaeService(apiService);
const localizacaoService = new LocalizacaoService(apiService, apiLegadoService)
const usuarioAdmService = new UsuarioAdmService(apiService, apiLegadoService)

let documentoSindical = {}

let documentosAprovadosSelect = null
let tipoClienteUnidadeSelect = null
let tipoDocSelect = null
let sindicatosLaboraisSelect = null
let sindicatosPatronaisSelect = null
let origemSelect = null
let versaoSelect = null
let permicaoCompartilhamentoSelect = null
let referenciamentoSelect = null
let origemDocumentoLocalizadoSelect = null;
let ufDocumentoLocalizadoSelect = null;
let abrangenciaUfSelect = null;

let dataRegistroDt = null
let validadeInicialDt = null
let validadeFinalDt = null
let prorrogacaoDt = null
let dataAssinaturaDt = null
let novaDataSlaDt = null
let dataBaseDt = null

let documentoSindicalTb = null;
let documentosLocalizadosTb = null;
let clienteUnidadeTb = null;
let clienteUnidadeSelecionadosTb = null;
let atividadeEconomicaTb = null;
let abrangenciaTb = null;
let abrangenciasSelecionadasTb = null;          
let atividadesEconomicasSelecionadasTb = null;
let emailsTb = null;

let filiaisIdsSelecionadas = []
let filiaisIdsParaRemoverSelecionadas = []
let cnaesIdsSelecionados = []
let cnaesSelecionados = []
let cnaesIdsParaRemoverSelecionados = []
let emailsIds = []

let abrangenciaIds = ''
let abrangenciasParaRemoverIds  = ''
let clientesUnidadesCarregadas = null
let cnaesCarregados = null

let atualizarDataSlaIdDoc = null

JQuery(async function () {
  new Menu()

  await AuthService.initialize()

  configurarModal()

  await carregarTipoDocumentoDatatable()

  $("#documentoSindicalModalBtn").on("click", () => {
    documentoSindical = {}
    $('#showDocumentoSindicalModalBtn').trigger('click')
  })
  
  $("#btn_remover_abrangencias_selecionadas").on("click", async () => await removerSAbrangenciasSelecionadas())
  $("#btn_remover_atividades_economicas_selecionadas").on("click", async () => await removerAtividadesEconomicasSeleciondas())
  $("#btn_remover_empresas_selecionadas").on("click", async () => await removerEmpresasSeleciondas())
  $("#btn_atualizar_abrangencias_selecionadas").on("click", async () => await carregarDatatableAbrangenciasSelecionadas())
  $("#btn_atualizar_atividades_economicas_selecionadas").on("click", async () => await carregarCnaesSelecionadosDatatable())
  $("#btn_atualizar_empresas_selecionadas").on("click", async () => await carregarClienteUnidadeSelecionadosDatatable())
  $("#adicionar").on("click", async () => await handleEnviarArquivo())
  $('#documentoSindicalForm').on('submit', e => e.preventDefault())
  $('#btn_ver_arquivo_referencia').on('click', async () => await verDocumentoReferencia())
  $('#enviar_emails_aprovados_btn').on('click', async () => await handleClickEnviarEmailsAprovados())

  $('#enviar_emails_btn').on('click', async () => await enviarEmailsAprovados(documentoSindical.id))
  $('#enviar_email_selecionados_btn').on('click', () => $('#emails_aprovados_modal_btn').trigger('click'))
  $('#iniciar_scrap_btn').on('click', async () => {
    const result = await docSindService.iniciarScrap(documentoSindical.id)

    if (result.isFailure()) NotificationService.error({title: result.error});
  });

  $("#selecionar_todos_documentos_sindicais_btn").on("click", (event) => {
		if (event.currentTarget.checked) {
			$('.emailId').prop('checked', true)
			$('.emailId').trigger('change')
		} else {
			$('.emailId').prop('checked', false)
			$('.emailId').trigger('change')
		}
	})
})

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn')

  // documento sindical
  const documentoSindicalModalHidden = document.getElementById('documentoSindicalModalHidden')
  const documentoSindicalModalContent = document.getElementById('documentoSindicalModalContent')

  // atividade economica
  const atividadeEconomicaModalHidden = document.getElementById('atividadeEconomicaModalHidden')
  const atividadeEconomicaModalContent = document.getElementById('atividadeEconomicaModalContent')

  // filial
  const filialModalHidden = document.getElementById('filialModalHidden')
  const filialModalContent = document.getElementById('filialModalContent')

  // documentos localizados
  const documentosLocalizadosModalHidden = document.getElementById('documentosLocalizadosModalHidden')
  const documentosLocalizadosModalContent = document.getElementById('documentosLocalizadosModalContent')

  // abrangencia
  const abrangenciaModalHidden = document.getElementById('abrangenciaModalHidden')
  const abrangenciaModalContent = document.getElementById('abrangenciaModalContent')

  // atualizarSla
  const atualizarSlaModalHidden = document.getElementById('atualizarSLAModalHidden')
  const atualizarSlaModalContent = document.getElementById('atualizarSLAModalContent')

  // emails-aprovados
  const emailsAprovadosModalHidden = document.getElementById('emailsAprovadosModalHidden')
  const emailsAprovadosModalContent = document.getElementById('emailsAprovadosModalContent')
  
  const documentoModalButtonsConfig = [
    {
      id: 'documentoSindicalModalUpsertBtn',
      onClick: async (_, modalContainer) => {
        const result = await upsert(documentoSindical.id)
        if (result.isSuccess()) {
          closeModal(modalContainer);
        }
      }
    },
    {
      id: 'documentoSindicalModalAprovarBtn',
      onClick: async (_, modalContainer) => {
        const result = await aprovar(documentoSindical.id)
        if (result.isSuccess()) {
          closeModal(modalContainer);
        }
      }
    }
  ]

  const atualizarSlaModalButtonsConfig = [
    {
      id: 'salvarNovaDataSLABtn',
      onClick: async (_, modalContainer) => await handleClickAtualizarDataSla(modalContainer)
    },
  ]

  const modalsConfig = [
    {
      id: 'atualizarSLAModal',
      modal_hidden: atualizarSlaModalHidden,
      content: atualizarSlaModalContent,
      btnsConfigs: atualizarSlaModalButtonsConfig,
      onOpen: async () => {
        configurarFormAtualizarNovaSla()
      },
      onClose: null
    },
    {
      id: 'documentoSindicalModal',
      modal_hidden: documentoSindicalModalHidden,
      content: documentoSindicalModalContent,
      btnsConfigs: documentoModalButtonsConfig,
      onOpen: async () => {
        limparFormularioDocumentoSindical()
        await configurarFormularioModalDocumentos()
      },
      onClose: null
    },
    {
      id: 'atividadeEconomicaModal',
      modal_hidden: atividadeEconomicaModalHidden,
      content: atividadeEconomicaModalContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarCnaesSelecionadosDatatable()
        await carregarCnaesDatatable()
      },
      onClose: null
    },
    {
      id: 'filialModal',
      modal_hidden: filialModalHidden,
      content: filialModalContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarClienteUnidadeDatatable()
        await carregarClienteUnidadeSelecionadosDatatable()
      },
      onClose: null
    },
    {
      id: 'documentosLocalizadosModal',
      modal_hidden: documentosLocalizadosModalHidden,
      content: documentosLocalizadosModalContent,
      btnsConfigs: [],
      onOpen: async () => {
        await carregarFormDocumentosLocalizados()
        await carregarDocumentosLocalizadosDatatable()
      },
      onClose: () => {
        limparUpsertFormularioDocumentosLocalizados()
      }
    },
    {
      id: 'abrangenciaModal',
      modal_hidden: abrangenciaModalHidden,
      content: abrangenciaModalContent,
      btnsConfigs: [],
      onOpen: async () => {
        await configurarFormularioAbrangencia()
        await carregarDatatableAbrangenciasSelecionadas()
        await carregarDatatableAbrangencia()
        handleUfChange()

        $('#btn_add_abrangencia').on('click', function () {
          $(this).attr('disabled', true)
        })

        $('#btn_reset_abrangencia').on('click', function () {
          $('#btn_add_abrangencia').attr('disabled', false)
        })
      },
      onClose: null
    },
    {
      id: "emailsAprovadosModal",
      modal_hidden: emailsAprovadosModalHidden,
      content: emailsAprovadosModalContent,
      btnsConfigs: [],
      onOpen: async () => await handleOpenEnviarEmailsAprovadosModal(),
      onClose: () => handleCloseEnviarEmailsAprovadosModal()
    }
  ];

  renderizarModal(pageCtn, modalsConfig);
}

async function carregarTipoDocumentoDatatable() {
  if (documentoSindicalTb) return await documentoSindicalTb.reload();

  documentoSindicalTb = new DataTableWrapper('#documentoSindicalTb', {
    columns: [
      { data: "" },
      { data: "id", title: 'Id' },
      { data: "nomeDocumento", title: "Nome Documento" },
      { data: "subclasseCodigo", title: "Subclasse", render: (data) => data?.join(', ') },
      { data: "cnaeDocs", title: "CNAE Descrição", render: (data) => data?.map(cnae => cnae.subclasse)?.join(', ') },
      { data: "nomeSindicatoLaboral", title: "Sindicato Laboral", render: (data) => data?.map(sindicato => sindicato.sigla)?.join(', ') },
      { data: "nomeSindicatoPatronal", title: "Sindicato Patronal", render: (data) => data?.map(sindicato => sindicato.sigla)?.join(', ') },
      { data: "validadeInicial", title: "Validade Inicial", type: 'date', render: (data) => DataTableWrapper.formatDate(data) },
      { data: "validadeFinal", title: "Validade Final", type: 'date', render: (data) => DataTableWrapper.formatDate(data) },
      { data: "dataAprovacao", title: "Data de Aprovação", type: 'date', render: (data) => DataTableWrapper.formatDate(data) },
      { data: "dataSla", title: "Data SLA", type: 'date', render: (data) => DataTableWrapper.formatDate(data) },
      { data: "nomeUsuarioAprovador", title: "Aprovador" }
    ],
    ajax: async (requestData) => {
      requestData.processados = true
      return await docSindService.obterDatatable(requestData)
    },
    rowCallback: function (row, data) {
      const icon = $("<i>").addClass("fa fa-file-text");
      let button = $("<a>")
        .attr("data-id", data?.id)
        .addClass("btn-update")
        .html(icon);
      button.on("click", async function () {
        const id = $(this).attr("data-id");
        [documentoSindical] = await obterPorId(id);
        $('#showDocumentoSindicalModalBtn').trigger('click');
      });
      $("td:eq(0)", row).html(button);

      let link = $("<a>")
        .attr("data-id", data?.id) //data?.idSindicatoLaboral
        .attr("href", "#")
        .html(DataTableWrapper.formatDate(data?.dataSla));
      link.on("click", function () {
        const id = $(this).attr("data-id");
        atualizarDataSlaIdDoc = id;
        $("#showAtualizarSLAModalBtn").trigger("click");
      });
      $("td:eq(10)", row).html(link);
    },
  });

  await documentoSindicalTb.initialize();
}

function configurarFormAtualizarNovaSla() {
  novaDataSlaDt = new DatepickerWrapper('#nova_data_sla');
}

async function handleClickAtualizarDataSla(modalContainer) {
  const novaDataSla = DateParser.toString(novaDataSlaDt.getValue());
  const result = await atualizarDataSla({ id: atualizarDataSlaIdDoc, novaDataSla: novaDataSla })

  if (result.isSuccess()) {
    closeModal(modalContainer)

    documentoSindicalTb?.reload()
  }
}

async function carregarFormDocumentosLocalizados() {
  origemDocumentoLocalizadoSelect = new SelectWrapper('#origem_doc', { onOpened: async () => await createSelectOption(origem) });
  ufDocumentoLocalizadoSelect = new SelectWrapper('#uf_doc_localizado', { onOpened: async () => await createSelectOption(uf) });
}

async function configurarFormularioModalDocumentos() {
  // Selects
  documentosAprovadosSelect = new SelectWrapper('#doc-referencia', { onOpened: async () => await documentosAprovadosOptions() });
  tipoClienteUnidadeSelect = new SelectWrapper('#unidade_cliente', { onOpened: async () => (await tipoUnidadeClienteService.obterSelect()).value });
  tipoDocSelect = new SelectWrapper('#tipo_doc', { onOpened: async () => (await tipoDocService.obterSelect({processado: true, filtrarSelectType: true})).value });
  sindicatosLaboraisSelect = SelectSindicatoLaboralFactory.Criar(sindicatoLaboralService, "sind_laborais");
  sindicatosPatronaisSelect = SelectSindicatoPatronalFactory.Criar(sindicatoPatronalService, "sind_patronais");
  origemSelect = new SelectWrapper('#origem', { onOpened: async () => createSelectOption(origem) });
  versaoSelect = new SelectWrapper('#versao', { onOpened: async () => createSelectOption(versao) });
  permicaoCompartilhamentoSelect = new SelectWrapper('#permissao_compartilhamento', { onOpened: async () => obterSimNaoOptions() });
  referenciamentoSelect = new SelectWrapper('#referenciamento', { onOpened: async () => (await estruturaClausulaService.obterSelect()).value });

  // Datepicker
  dataRegistroDt = new DatepickerWrapper('#data_registro');
  validadeInicialDt = new DatepickerWrapper('#validade_inicial');
  validadeFinalDt = new DatepickerWrapper('#validade_final');
  prorrogacaoDt = new DatepickerWrapper('#prorrogacao');
  dataAssinaturaDt = new DatepickerWrapper('#data_assinatura');
  dataBaseDt = new DatepickerWrapper('#data_base',null,"mes-ano");

  documentosAprovadosSelect?.enable();
  if (Object.keys(documentoSindical).length > 0){
    const resultDocLocalizado = await documentosLocalizadosService.obterAprovadoPorId(documentoSindical?.idDocumentoLocalizado)
    const [docLocalizado] = resultDocLocalizado.value;
    documentosAprovadosSelect?.setCurrentValue({
      id: docLocalizado?.id,
      description: `${docLocalizado?.id}-${docLocalizado?.nome} / Aprovação: ${DateFormatter.dayMonthYear(docLocalizado?.dataAprovacao)}`
    });
    documentosAprovadosSelect?.disable();

    let tiposDocumentos = await tipoDocSelect?.loadOptions();
    if (tiposDocumentos) {
      let [tipoDocumento] = tiposDocumentos.filter(td => td.description == documentoSindical?.nome);
      tipoDocSelect.setCurrentValue(tipoDocumento);
    }

    let tiposUnidadeCliente = await tipoClienteUnidadeSelect?.loadOptions();
    if (tiposUnidadeCliente) {
      let [tipoUnidadeCliehte] = tiposUnidadeCliente.filter(tuc => tuc.id == documentoSindical?.idTipoNegocio);
      tipoClienteUnidadeSelect.setCurrentValue(tipoUnidadeCliehte);
    }

    if (documentoSindical?.validadeFinal && documentoSindical?.validadeFinal != "0001-01-01"){
      validadeFinalDt.setValue(documentoSindical.validadeFinal);
    }

    if (documentoSindical?.validadeInicial && documentoSindical?.validadeInicial != "0001-01-01"){
      validadeInicialDt.setValue(documentoSindical.validadeInicial);
    }

    if (documentoSindical?.prorrogacaoDoc && documentoSindical?.prorrogacaoDoc != "0001-01-01"){
      prorrogacaoDt.setValue(documentoSindical.prorrogacaoDoc);
    }
    
    if (documentoSindical?.dataAssinatura && documentoSindical?.dataAssinatura != "0001-01-01"){
      dataAssinaturaDt.setValue(documentoSindical.dataAssinatura);
    }

    if (documentoSindical?.dataRegMTE && documentoSindical?.dataRegMTE != "0001-01-01"){
      dataRegistroDt.setValue(documentoSindical.dataRegMTE);
    }

    if(documentoSindical?.sindLaboral) {
      const sindicatosLaboraisSelecionados = documentoSindical?.sindLaboral.map(sindicato => {
        const option = {
          id: sindicato.id,
          description: sindicato.sigla + " / " + sindicato.cnpj
        }
        return option;
      });
      sindicatosLaboraisSelect.setCurrentValue(sindicatosLaboraisSelecionados);
    }

    if(documentoSindical?.sindPatronal) {
      const sindicatosPatronaisSelecionados = documentoSindical?.sindPatronal.map(sindicato => {
        const option = {
          id: sindicato.id,
          description: sindicato.sigla + " / " + sindicato.cnpj
        }
        return option;
      });
      sindicatosPatronaisSelect.setCurrentValue(sindicatosPatronaisSelecionados);
    }

    if(documentoSindical?.referencia && documentoSindical?.referencia != "[]") {
      referenciamentoSelect?.disable();
      let referencias = documentoSindical.referencia;
      let referenciasOpcoes = await referenciamentoSelect?.loadOptions();
      let referenciasSelecionadas = referenciasOpcoes?.filter(sl => referencias.find(v => v == sl.id))
      referenciasSelecionadas = referenciasSelecionadas ? [...referenciasSelecionadas] : referenciasSelecionadas;
      
      referenciamentoSelect?.clear();
      referenciamentoSelect.setCurrentValue(referenciasSelecionadas);
      referenciamentoSelect?.enable();
    }

    let origensCliente = await origemSelect?.loadOptions();
    if (origensCliente) {
      let [origemCliente] = origensCliente.filter(o => o.id == documentoSindical?.origem);
      origemSelect.setCurrentValue(origemCliente);
    }

    let versoesOpcoes = await versaoSelect?.loadOptions();
    if (versoesOpcoes) {
      let [versaoSelecionada] = versoesOpcoes.filter(v => v.description == documentoSindical?.versaoDocumento);
      versaoSelect?.setCurrentValue(versaoSelecionada);
    }

    let permissaoOpcoes = await permicaoCompartilhamentoSelect?.loadOptions();
    if (permissaoOpcoes) {
      let [permissaoSelecionada] = permissaoOpcoes.filter(p => p.description == documentoSindical?.permissao);
      permicaoCompartilhamentoSelect?.setCurrentValue(permissaoSelecionada);
    }

    $("#numero_solicitacao").val(documentoSindical?.numeroSolicitacaoMR ?? "");
    $("#numero_registro").val(documentoSindical?.numRegMTE ?? "");
    $("#observacoes").val(documentoSindical?.observacao ?? "");
    dataBaseDt.setValue(documentoSindical?.database ? DateParser.fromDataBaseString(documentoSindical?.database) : "");

    if (documentoSindical?.docRestrito && documentoSindical?.docRestrito != "Não"){
      $("#restrito").prop("checked", true);
    }

    if (documentoSindical?.cnaeDoc) {
      const cnaesDocumento = documentoSindical.cnaeDoc;
      cnaesIdsSelecionados = cnaesDocumento?.map(c => c?.id) ?? [];
    }

    if (documentoSindical?.abrangencia) {
      const abrangenciaDocumento = documentoSindical.abrangencia;
      abrangenciaIds = " " + abrangenciaDocumento?.map(a => a?.id).join(" ") ?? '';
    }

    if (documentoSindical?.clienteEstabelecimento) {
      const filiaisDocumento = documentoSindical.clienteEstabelecimento;
      filiaisIdsSelecionadas = filiaisDocumento?.map(f => f?.u) ?? [];
    }
  }
}

function obterSimNaoOptions(){
  return [
    {
      id: 'Sim',
      description: 'Sim'
    },
    {
      id: 'Não',
      description: 'Não'
    }
  ]
}

async function configurarFormularioAbrangencia() {
  // Selects
  abrangenciaUfSelect = new SelectWrapper('#uf', { onOpened: async () => await createSelectOption(uf) });
}

async function removerSAbrangenciasSelecionadas() {
  const abrangenciasParaRemoverArray = abrangenciasParaRemoverIds.split(" ").filter(a => !Number.isNaN(a) && a != "");
  abrangenciasParaRemoverArray.forEach(abrangenciaId => {
    abrangenciaIds = (abrangenciaIds + "").replace(' ' + abrangenciaId, '');
    $('.abrangencia[data-id="' + abrangenciaId + '"]').prop("checked", false);
  })

  await carregarDatatableAbrangenciasSelecionadas();

  abrangenciasParaRemoverIds = '';
}

async function removerAtividadesEconomicasSeleciondas() {
  cnaesIdsSelecionados = cnaesIdsSelecionados.filter(cnaeId => {
    if(!cnaesIdsParaRemoverSelecionados.includes(cnaeId)) return true
    $('.cnae-modal-datatable[data-id="' + cnaeId + '"]').prop("checked", false);
  });
  await carregarCnaesSelecionadosDatatable();
  cnaesIdsParaRemoverSelecionados = [];
}

async function removerEmpresasSeleciondas() {
  let filiaisIdsSelecionadasFiltradas = filiaisIdsSelecionadas.filter(filialId => {
    if(!filiaisIdsParaRemoverSelecionadas.includes(filialId)) return true
    $('.filial-modal-datatable[data-id="' + filialId + '"]').prop("checked", false);
  });

  filiaisIdsSelecionadas.splice(0, filiaisIdsSelecionadas.length);
  filiaisIdsSelecionadas.push(...filiaisIdsSelecionadasFiltradas);

  await carregarClienteUnidadeSelecionadosDatatable();

  filiaisIdsParaRemoverSelecionadas.splice(0, filiaisIdsParaRemoverSelecionadas.length);
}

function handleUfChange() {
  $('#uf').on('change', async () => await abrangenciaTb.reload())
}

async function upsert(id) {
  const camposObrigatoriosNaoPreenchidos = [];

  if (!dataBaseDt.getValue) {
    NotificationService.error({title: "Forneça a data-base"});
    return;
  }

  if (versaoSelect?.getValue() == 'Registrado' && !$('#numero_solicitacao').val()) {
    NotificationService.error({title: "Você deve fornecer o numero de solicitação do MTE para documentos registrados."});
    return;
  }

  if (!tipoClienteUnidadeSelect?.getValue()){
    camposObrigatoriosNaoPreenchidos.push("'Tipo Unidade Cliente'");
  }

  if (!tipoDocSelect?.getValue()){
    camposObrigatoriosNaoPreenchidos.push("'Nome do Documento'");
  }

  if (!versaoSelect?.getValue()){
    camposObrigatoriosNaoPreenchidos.push("'Versão'");
  }

  if (!validadeInicialDt.getValue()) {
    camposObrigatoriosNaoPreenchidos.push("'Validade Inicial'");
  }

  if (!validadeFinalDt.getValue()) {
    camposObrigatoriosNaoPreenchidos.push("'Validade Final'");
  }

  if (!origemSelect.getValue()) {
    camposObrigatoriosNaoPreenchidos.push("'Origem'");
  }

  if (!cnaesIdsSelecionados || cnaesIdsSelecionados.length == 0){
    camposObrigatoriosNaoPreenchidos.push("'Atividades Econômicas'");
  }

  if (!abrangenciaIds || abrangenciaIds == "" || abrangenciaIds == " "){
    camposObrigatoriosNaoPreenchidos.push("'Abrangências'");
  }

  if (camposObrigatoriosNaoPreenchidos.length > 0){
    Notification.error({
      title: "Você precisa preencher os seguintes campos: " + camposObrigatoriosNaoPreenchidos.join(", ")
    })
    return Result.failure();
  }

  return id ? await atualizar(id) : await incluir();
}

async function incluir() {
  const tiposDocumentosExigemEstabelecimentos = [5,13,8,41];

  if(tiposDocumentosExigemEstabelecimentos.includes(Number(tipoDocSelect?.getValue())) && (!filiaisIdsSelecionadas || filiaisIdsSelecionadas.length == 0)) {
    Notification.error({title: "Você precisa fornecer as filiais para este tipo de documento"});
    return Result.failure();
  }

  const requestData = {
    tipoUnidadeCliente: Number(tipoClienteUnidadeSelect?.getValue()),
    tipo: tipoDocSelect.getValue() ?? 1,
    versao: versaoSelect.getValue(),
    validadeInicial: DateParser.toString(validadeInicialDt.getValue()),
    validadeFinal: DateParser.toString(validadeFinalDt.getValue()),
    origem: origemSelect.getValue(),
    cnaesIds: cnaesIdsSelecionados,
    cnaes: JSON.stringify(cnaesSelecionados),
    abrangencia: abrangenciaIds.split(' ').filter(a => a != "").map(v => Number(v)),
    
    numeroSolicitacaoMr: $('#numero_solicitacao').val(),
    dataRegistro: dataRegistroDt?.getValue() ? DateParser.toString(dataRegistroDt?.getValue()) : null,
    numeroRegistro: $('#numero_registro').val(),

    prorrogacao: prorrogacaoDt?.getValue() ? DateParser.toString(prorrogacaoDt?.getValue()) : null,
    dataAssinatura: dataAssinaturaDt?.getValue() ? DateParser.toString(dataAssinaturaDt?.getValue()) : null,
    permissao: permicaoCompartilhamentoSelect?.getValue(),

    observacao: $("#observacoes").val(),
    idDocumento: documentosAprovadosSelect?.getValue(),

    dataBase: DateParser.toDataBaseString(dataBaseDt.getValue()),
    restrito: $("#restrito").is(":checked") ? true : false,
    clienteEstabelecimento: filiaisIdsSelecionadas,
    referencia: referenciamentoSelect?.getValue(),

    sindLaboral: sindicatosLaboraisSelect?.getValue(),
    sindPatronal: sindicatosPatronaisSelect?.getValue(),
  }

  const result = await docSindService.incluir(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: 'Não foi posssível realizar o cadastro', message: result.error })
    return Result.failure();
  }

  NotificationService.success({ title: 'Cadastro realizado com sucesso!' })
  limparFormularioDocumentoSindical()
  await carregarTipoDocumentoDatatable();

  return Result.success()
}

async function atualizar(id) {
  const tiposDocumentosExigemEstabelecimentos = [5,13,8,41]

  if(tiposDocumentosExigemEstabelecimentos.includes(Number(tipoDocSelect?.getValue())) && (!filiaisIdsSelecionadas || filiaisIdsSelecionadas.length == 0)) {
    Notification.error({title: "Você precisa fornecer as filiais para este tipo de documento"});
    return Result.failure();
  }

  const requestData = {
    idDocSind: id,
    tipoUnidadeCliente: Number(tipoClienteUnidadeSelect?.getValue()),
    tipo: tipoDocSelect.getValue() ?? 1,
    versao: versaoSelect.getValue(),
    validadeInicial: DateParser.toString(validadeInicialDt.getValue()),
    validadeFinal: DateParser.toString(validadeFinalDt.getValue()),
    origem: origemSelect.getValue(),
    cnaesIds: cnaesIdsSelecionados,
    cnaes: JSON.stringify(cnaesSelecionados),
    abrangencia: abrangenciaIds.split(' ').filter(a => a != "").map(v => Number(v)),
    
    numeroSolicitacaoMr: $('#numero_solicitacao').val(),
    dataRegistro: dataRegistroDt?.getValue() ? DateParser.toString(dataRegistroDt?.getValue()) : null,
    numeroRegistro: $('#numero_registro').val(),

    prorrogacao: prorrogacaoDt?.getValue() ? DateParser.toString(prorrogacaoDt?.getValue()) : null,
    dataAssinatura: dataAssinaturaDt?.getValue() ? DateParser.toString(dataAssinaturaDt?.getValue()) : null,
    permissao: permicaoCompartilhamentoSelect?.getValue(),

    observacao: $("#observacoes").val(),
    idDocumento: documentosAprovadosSelect?.getValue(),

    dataBase: DateParser.toDataBaseString(dataBaseDt.getValue()),
    restrito: $("#restrito").is(":checked") ? true : false,
    clienteEstabelecimento: filiaisIdsSelecionadas,
    referencia: referenciamentoSelect?.getValue(),

    sindLaboral: sindicatosLaboraisSelect?.getValue(),
    sindPatronal: sindicatosPatronaisSelect?.getValue(),
  }

  const result = await docSindService.atualizar(requestData);

  if (result.isFailure()) {
    NotificationService.error({ title: 'Não foi posssível realizar o cadastro', message: result.error })
    return Result.failure();
  }

  NotificationService.success({ title: 'Cadastro realizado com sucesso!' })
  limparFormularioDocumentoSindical()
  await carregarTipoDocumentoDatatable();

  return Result.success()
}

async function aprovar(id) {
  const result = await docSindService.aprovar(id);
  if (result.isFailure()) {
    NotificationService.error({ title: 'Não foi posssível aprovar o documento', message: result.error })
    return Result.failure();
  }

  NotificationService.success({
    title: "Documento Aprovado"
  })

  await carregarTipoDocumentoDatatable()
  return Result.success();
}

async function seeFile(id) {
	const button = $(`#${id}`)
	const icon = button.find('i')
  const embed = $('#embed_pdf')

  const path = await obterUrlDocumento(id)

  const isShowEmbed = icon.hasClass('fa fa-eye')

  if (isShowEmbed) {
    embed.css("height", "1000px")
    embed.attr('src', path)
    icon.removeClass('fa fa-eye')
    return icon.addClass('fa fa-eye-slash')
  }
  
  embed.css("height", "0px")
  embed.attr('src', '')
  icon.removeClass('fa fa-eye-slash')
  return icon.addClass('fa fa-eye')
}

async function verDocumentoReferencia() {
	const icon = $('#icon_ver_arquivo_referencia')
  const embed = $('#embed_pdf_register')
  const documentoSelecionado = documentosAprovadosSelect.getValue()

  const response = await documentosLocalizadosService.download(Number(documentoSelecionado));
  
  if (response.isFailure()) {
    return NotificationService.error({ title: 'Não foi possível baixar o arquivo.' })
  }

  const blobFile = new Blob([response.value.data], { type: response.value.contentType })
  const urlFile = URL.createObjectURL(blobFile)

  const path = urlFile

  const isShowEmbed = icon.hasClass('fa fa-eye')

  if (isShowEmbed) {
    embed.css("height", "1000px")
    embed.attr('src', path)
    icon.removeClass('fa fa-eye')
    return icon.addClass('fa fa-eye-slash')
  }
  
  embed.css("height", "0px")
  embed.attr('src', '')
  icon.removeClass('fa fa-eye-slash')
  URL.revokeObjectURL(urlFile);
  return icon.addClass('fa fa-eye')
}

async function deleteFile(id) {
  return new Promise((resolve) => {
    NotificationService.success({
      title: 'Tem certeza?',
      showConfirmButton: true,
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Sim, excluir!',
      then: async (result) => {
        if (result.isConfirmed) {

          const result = await documentosLocalizadosService.deletar(id)

          if (result.isFailure()) {
            return NotificationService.error({ title: 'Erro ao deletar documento', message: result.error })
          }

          
          NotificationService.success({ title: 'Arquivo excluído com sucesso!' })

          documentosLocalizadosTb.reload()
          resolve(Result.success())
        }
      }
    })
  })
}

function approveFile(id) {
  return new Promise((resolve) => {
    NotificationService.success({
      title: 'Após a aprovação o documento será removido da lista e você será levado para a área de cadastro do documento!',
      showConfirmButton: true,
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Sim, prosseguir!',
      then: async (result) => {
        if (result.isConfirmed) {
          const result = await documentosLocalizadosService.aprovar(id)

          if (result.isFailure()) {
            return NotificationService.error({ title: 'Não foi possível aprovar o documento!', message: result.error })
          }
          
          NotificationService.success({ title: 'Documento aprovado!' })

          documentosLocalizadosTb.reload()

          closeModal({id: "documentosLocalizadosModal"});
          documentoSindical = {};
          $('#showDocumentoSindicalModalBtn').trigger('click');

          const resultDocLocalizado = await documentosLocalizadosService.obterAprovadoPorId(id)
          const [docLocalizado] = resultDocLocalizado.value;
          documentosAprovadosSelect?.setCurrentValue({
            id: docLocalizado?.id,
            description: `${docLocalizado?.id}-${docLocalizado?.nome} / Aprovação: ${DateFormatter.dayMonthYear(docLocalizado?.dataAprovacao)}`
          });
          documentosAprovadosSelect?.disable();
          
          resolve(Result.success())
        }
      }
    })
  })
}

async function handleEnviarArquivo() {
  let arquivoInput = document.querySelector('input[name="documentoLocalizadoUpload"]')
  let origem = origemDocumentoLocalizadoSelect.getValue()
  let uf = ufDocumentoLocalizadoSelect.getValue() == 0 ? null : ufDocumentoLocalizadoSelect.getValue()
  let arquivo = arquivoInput.files[0]

  await enviarArquivo({ arquivo, origem, uf })

  documentosLocalizadosTb.reload()
}

async function handleOpenEnviarEmailsAprovadosModal() {
  await carregarEmailsDatatable()
}

async function handleCloseEnviarEmailsAprovadosModal() {
  emailsIds = []
  $('#selecionar_todos_documentos_sindicais_btn').prop('checked', false)
}

async function handleClickEnviarEmailsAprovados() {
  const usuariosIds = emailsIds.map(e => parseInt(e))
  const result = await enviarEmailsAprovados(documentoSindical?.id, usuariosIds)

  if (result.isSuccess()) {
    closeModal({ id: 'emailsAprovadosModal' })
  }
}

// Data-tables
async function carregarClienteUnidadeSelecionadosDatatable() {
  if (clienteUnidadeSelecionadosTb) return clienteUnidadeSelecionadosTb.reload();

  $("#selecionar_todas_empresas_selecionadas").on("click", (event) => {
    if (event.currentTarget.checked) {
      $('.filial-selecionada-modal-datatable').prop('checked', true);
      $('.filial-selecionada-modal-datatable').trigger('change');
    } else {
      $('.filial-selecionada-modal-datatable').prop('checked', false);
      $('.filial-selecionada-modal-datatable').trigger('change');
    }
  });

  clienteUnidadeSelecionadosTb = new DataTableWrapper('#clienteUnidadeSelecionadosTb', {
    columns: [
        { data: "id" },
        { data: "nomeGrupoEconomico", title: "Grupo Econômico" },
        { data: "nome", title: "Matriz" },
        { data: "nomeEstabelecimento", title: "Filial" },
        { data: "cnpj", title: "Filial CNPJ"}
    ],
    ajax: async (requestData) => {
        $('#selecionar_todas_empresas_selecionadas').val(false).prop('checked', false);

        const customParams = {
            ...requestData,
            clienteUnidadeIds: filiaisIdsSelecionadas
        }

        clientesUnidadesCarregadas = await clienteUnidadeService.obterDatatablePorListaIds(customParams);
        return clientesUnidadesCarregadas
    },
    rowCallback: function (row, data) {
        const checkbox = input({ type: 'checkbox', className: 'form-check-input c chkitem filial-selecionada-modal-datatable' }).attr('data-id', data.id)

        if (filiaisIdsParaRemoverSelecionadas.find(filial => filial == data?.id)) {
            checkbox.prop('checked', true)
        }

        checkbox.on('change', (el) => {
            const checked = el.target.checked;
            const id = data?.id;
        
            if (checked) {
                filiaisIdsParaRemoverSelecionadas.push(id)
            } else {
                filiaisIdsParaRemoverSelecionadas = filiaisIdsParaRemoverSelecionadas.filter(item => item != id);
            }
        });

        $("td:eq(0)", row).html(checkbox);
    },
  });
  
  await clienteUnidadeSelecionadosTb.initialize();
}

async function carregarClienteUnidadeDatatable() {
  if (clienteUnidadeTb) return clienteUnidadeTb.reload();

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
        { data: "nome", title: "Matriz" },
        { data: "nomeEstabelecimento", title: "Filial" },
        { data: "cnpj", title: "Filial CNPJ"}
    ],
    ajax: async (requestData) => {
        $('#selecionar_todas_empresas').val(false).prop('checked', false);
        clientesUnidadesCarregadas = await clienteUnidadeService.obterDatatable(requestData)
        return clientesUnidadesCarregadas
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
                filiaisIdsSelecionadas.push(id)
            } else {
                filiaisIdsSelecionadas = filiaisIdsSelecionadas.filter(item => item != id);
            }
        });

        $("td:eq(0)", row).html(checkbox);
    },
  });

  await clienteUnidadeTb.initialize();
}

async function carregarCnaesSelecionadosDatatable() {
  if (atividadesEconomicasSelecionadasTb) return await atividadesEconomicasSelecionadasTb.reload()
    
  atividadesEconomicasSelecionadasTb = new DataTableWrapper('#atividadesEconomicasSelecionadasTb', {
    columns: [
      { data: "id" },
      { data: "subclasse", title: "Subclasse"},
      { data: "descricao", title: "Descrição Subclasse" },
      { data: "categoria", title: "Categoria" }
    ],
    ajax: async (requestData) => {
      const customParams = {
        ...requestData,
        cnaesIds: cnaesIdsSelecionados,
        forceGetByIds: true,
        Columns: 'id,subclasse,descricao,categoria'
      }
      cnaesCarregados = await cnaeService.obterDatatable(customParams)
      return cnaesCarregados
    },
    rowCallback: function (row, data) {
      const checkbox = input({ type: 'checkbox', className: 'form-check-input c chkitem' }).attr('data-id', data.id)

      if (cnaesIdsParaRemoverSelecionados.find(cnae => cnae == data.id)) {
        checkbox.prop('checked', true)
      }

      checkbox.on('change', (el) => {
        const checked = el.target.checked;
        const id = data?.id;

        if (checked) {
          cnaesIdsParaRemoverSelecionados.push(id);
        } else {
          cnaesIdsParaRemoverSelecionados = cnaesIdsParaRemoverSelecionados.filter(item => item != id)
        }
      });

      $("td:eq(0)", row).html(checkbox);
    },
  });

  await atividadesEconomicasSelecionadasTb.initialize();
}

async function carregarCnaesDatatable() {
  if (atividadeEconomicaTb) return await atividadeEconomicaTb.reload()
    
  atividadeEconomicaTb = new DataTableWrapper('#atividadeEconomicaTb', {
    columns: [
      { data: "id" },
      { data: "subclasse", title: "Subclasse"},
      { data: "descricao", title: "Descrição Subclasse" },
      { data: "categoria", title: "Categoria" }
    ],
    ajax: async (requestData) => {
      requestData.Columns = 'id,subclasse,descricao,categoria'
      cnaesCarregados = await cnaeService.obterDatatable(requestData)
      return cnaesCarregados
    },
    rowCallback: function (row, data) {
      const checkbox = input({ type: 'checkbox', className: 'form-check-input c chkitem cnae-modal-datatable' }).attr('data-id', data.id)

      if (cnaesIdsSelecionados.find(cnae => cnae == data.id)) {
        checkbox.prop('checked', true)
      }

      checkbox.on('change', (el) => {
        const checked = el.target.checked;
        const id = data?.id;

        if (checked) {
          const cnae = cnaesCarregados?.value?.items?.filter(cnae => cnae.id == id)[0]

          cnaesIdsSelecionados.push(id)

          cnaesSelecionados.push({
            id,
            subclasse: cnae.descricao
          })
        } else {
          cnaesSelecionados = cnaesSelecionados.filter(({ id }) => id.toString() != id.toString())
          cnaesIdsSelecionados = cnaesIdsSelecionados.filter(item => item != id)
        }
      });

      $("td:eq(0)", row).html(checkbox);
    },
  });

  await atividadeEconomicaTb.initialize();
}

async function carregarDocumentosLocalizadosDatatable() {
  if (documentosLocalizadosTb) {
    return await documentosLocalizadosTb.reload()
  }

  documentosLocalizadosTb = new DataTableWrapper('#documentosLocalizadosTb', {
    columns: [
      { data: "nome", title: "Nome Documento" },
      { data: "origem", title: "Origem" },
      { data: "dataRegistro", title: "Data de Download", render: (data) => DataTableWrapper.formatDateTime(data) },
      { data: "id", title: "" }
    ],
    ajax: async (requestData) => {
      requestData.naoAprovados = true
      return await documentosLocalizadosService.obterDatatable(requestData)
    },
    rowCallback: function (row, data) {
      const id = data?.id

      const buttonContainer = div({className: "d-flex"});
      const styleIcon = 'font-size: 1.2em;'

      const eyeButton = button({
        id,
        type: 'button',
        className: 'btn btn-primary',
        title: 'Visualizar arquivo',
        content: stringI({
          style: styleIcon,
          className: 'fa fa-eye eye_list'
        })
      })

      const trashButton = button({
        id,
        type: 'button',
        className: 'btn btn-danger',
        style: 'margin: 0 10px;',
        title: 'Excluir arquivo',
        content: stringI({
          style: styleIcon,
          className: 'fa fa-trash-o'
        })
      })

      const aproveButton = button({
        id,
        type: 'button',
        className: 'btn btn-success',
        title: 'Aprovar arquivo',
        content: stringI({
          style: styleIcon,
          className: 'fa fa-check-square-o'
        })
      })

      eyeButton.on("click", async () => await seeFile(id))
      trashButton.on("click", () => deleteFile(id))
      aproveButton.on("click", () => approveFile(id))

      buttonContainer.append(eyeButton, trashButton, aproveButton)

      $("td:eq(3)", row).html(buttonContainer)
    },
  });

  await documentosLocalizadosTb.initialize();
}

async function carregarDatatableAbrangenciasSelecionadas() {
	if (abrangenciasSelecionadasTb !== null) return await abrangenciasSelecionadasTb.reload()
  
	$("#selecionar_todos_municipios_selecionados").on("click", (event) => {
		if (event.currentTarget.checked) {
			$('.abrangenciaSelecionada').prop('checked', true)
			$('.abrangenciaSelecionada').trigger('change')
		} else {
			$('.abrangenciaSelecionada').prop('checked', false)
			$('.abrangenciaSelecionada').trigger('change')
		}
	})

	abrangenciasSelecionadasTb = new DataTableWrapper('#abrangenciasSelecionadasTb', {
		columns: [
			{ "data": "id", orderable: false, title: "Selecione" },
			{ "data": "municipio", title: "Municipio" },
      { "data": "estado", title: "Estado" },
			{ "data": "pais", title: "País" },
		],
		ajax: async (requestData) => {
			$('#selecionar_todos_municipios_selecionados').val(false).prop('checked', false)
      const ids = abrangenciaIds.split(" ")
      const customRequest = {
        abrangenciasSelecionadas: ids.filter(id => !Number.isNaN(id) && id != ""),
        ...requestData
      }
			return await localizacaoService.obterDatatablePorListaIds(customRequest)
		},
		rowCallback: function (row, data) {
			const checkbox = $("<input>").attr("type", "checkbox").attr("data-id", data?.id).addClass('abrangenciaSelecionada')
			
			$("td:eq(0)", row).html(checkbox)
			
			const dataId = $(row).find('.abrangenciaSelecionada').attr('data-id')

			$(row).find('.abrangenciaSelecionada').on('change', function () {
				if ($(row).find('.abrangenciaSelecionada').is(':checked')) {
					if(abrangenciasParaRemoverIds.split(' ').indexOf(dataId + '') === -1) {
						abrangenciasParaRemoverIds += " " + dataId
					}	
				} else {
					abrangenciasParaRemoverIds = (abrangenciasParaRemoverIds + "").replace(' ' + dataId, '')
				}
			})
		}
	})

	await abrangenciasSelecionadasTb.initialize()
}

async function carregarDatatableAbrangencia() {
	if (abrangenciaTb !== null) return await abrangenciaTb.reload()

	$("#seleciona_todas_regioes").on("click", (event) => {
		if (event.currentTarget.checked) {
			$('.abrangencia').prop('checked', true)
			$('.abrangencia').trigger('change')
		} else {
			$('.abrangencia').prop('checked', false)
			$('.abrangencia').trigger('change')
		}
	})

	abrangenciaTb = new DataTableWrapper('#abrangenciaTb', {
		columns: [
			{ "data": "id", orderable: false, title: "Selecione" },
			{ "data": "municipio", title: "Municipio" },
			{ "data": "pais", title: "País" },
		],
		ajax: async (requestData) => {
			$('#seleciona_todas_regioes').val(false).prop('checked', false)
			return await localizacaoService.obterDatatablePorUf(requestData, $('#uf').val())
		},
		rowCallback: function (row, data) {
			const checkbox = $("<input>").attr("type", "checkbox").attr("data-id", data?.id).addClass('abrangencia')
			
			$("td:eq(0)", row).html(checkbox)
			
			const dataId = $(row).find('.abrangencia').attr('data-id')

			if (abrangenciaIds) {
				const ids = abrangenciaIds.split(" ")
				const isChecked = ids.indexOf('' + dataId)
				if (isChecked >= 0) {
					$(row).find('.abrangencia').prop('checked', true)
				}
			}

			$(row).find('.abrangencia').on('change', function () {
				if ($(row).find('.abrangencia').is(':checked')) {
					if(abrangenciaIds.split(' ').indexOf(dataId + '') === -1) {
						abrangenciaIds += " " + dataId
					}	
				} else {
					abrangenciaIds = (abrangenciaIds + "").replace(' ' + dataId, '')
				}
			})
		}
	})

	await abrangenciaTb.initialize()
}

async function carregarEmailsDatatable() {
  if (emailsTb) return await emailsTb.reload()
    
  emailsTb = new DataTableWrapper('#emailsTb', {
    columns: [
      { data: "id" },
      { data: "nome", title: "Nome"},
      { data: "email", title: "E-mail" },
      { data: "nivel", title: "Nível" }
    ],
    ajax: async (requestData) => {
      requestData.modulo = 10
      return await usuarioAdmService.obterDatatablePorDocumento(documentoSindical.id, requestData)
    },
    rowCallback: function (row, data) {
      const id = data.id
      const checkbox = input({ type: 'checkbox', className: 'form-check-input c chkitem' })
        .attr('data-id', id)
        .addClass('emailId')

      if (emailsIds.find(cnae => cnae == id)) {
        checkbox.prop('checked', true)
      }

      checkbox.on('change', (el) => {
        const checked = el.target.checked

        if (checked) {
          emailsIds.push(id)
        } else {
          emailsIds = emailsIds.filter(item => item != id)
        }
      })

      $("td:eq(0)", row).html(checkbox)
    },
  })

  await emailsTb.initialize()
}

// Utils
function limparFormularioDocumentoSindical() {
  documentosAprovadosSelect?.clear()
  tipoClienteUnidadeSelect?.clear()
  tipoDocSelect?.clear()
  sindicatosLaboraisSelect?.clear()
  sindicatosPatronaisSelect?.clear()
  origemSelect?.clear()
  versaoSelect?.clear()
  permicaoCompartilhamentoSelect?.clear()
  referenciamentoSelect?.clear()

  // Datepicker
  dataRegistroDt?.clear()
  validadeInicialDt?.clear()
  validadeFinalDt?.clear()
  prorrogacaoDt?.clear()
  dataAssinaturaDt?.clear()
  dataBaseDt?.clear()

  filiaisIdsSelecionadas = []
  cnaesIdsSelecionados = []
  cnaesSelecionados = []
  cnaesIdsParaRemoverSelecionados = []
  filiaisIdsParaRemoverSelecionadas = []
  abrangenciaIds = "";

  $('#observacoes').val("")
  $('#numero_registro').val("")
  $('#numero_solicitacao').val("")

  clienteUnidadeTb?.dataTable?.search("");
  clienteUnidadeSelecionadosTb?.dataTable?.search("");

  atividadesEconomicasSelecionadasTb?.dataTable?.search("");
  atividadeEconomicaTb?.dataTable?.search("");

  abrangenciasSelecionadasTb?.dataTable?.search("");
  abrangenciaTb?.dataTable?.search("");

  documentosLocalizadosTb?.dataTable?.search("");
  abrangenciaUfSelect?.clear();

	const icon = $('#icon_ver_arquivo_referencia')
  const embed = $('#embed_pdf_register')

  if (icon.hasClass('fa fa-eye')) return
  
  embed.css("height", "0px")
  embed.attr('src', '')
  icon.removeClass('fa fa-eye-slash')
  icon.addClass('fa fa-eye')
  $('#embed_pdf').attr("src", "")
}

function limparUpsertFormularioDocumentosLocalizados() {
  ufDocumentoLocalizadoSelect?.clear();
  origemDocumentoLocalizadoSelect?.clear();

  $('#file').val(null);
  $('#embed_pdf').css("height", "0px")
  $('#embed_pdf').attr("src", "")
}
