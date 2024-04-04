// Libs
import 'bootstrap';
import jQuery from 'jquery';
import $ from 'jquery';

// Temp
import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'datatables.net-bs5';
import 'datatables.net-responsive-bs5';

// Css libs
import 'bootstrap/dist/css/bootstrap.min.css';

// Services
import {
  ClienteUnidadeService,
  DocSindService,
  TipoDocService,
  CnaeService,
  GrupoEconomicoService,
  EstruturaClausulaService,
  IndicesSindicaisService,
  LocalizacaoService,
  MatrizService,
  SindicatoLaboralService,
  SindicatoPatronalService
} from '../../js/services'

// Utils
import NotificationService from '../../js/utils/notifications/notification.service.js';
import DatepickerrangeWrapper from '../../js/utils/daterangepicker/daterangepicker-wrapper';
import SelectWrapper from '../../js/utils/selects/select-wrapper';

// Core
import { AuthService, ApiService } from '../../js/core/index.js';
import { ApiLegadoService } from '../../js/core/api-legado.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

// Services
const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const indicesSindicaisService = new IndicesSindicaisService(apiService, apiLegadoService);
const matrizService = new MatrizService(apiService);
const grupoEconomico = new GrupoEconomicoService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const localizacaoService = new LocalizacaoService(apiService, apiLegadoService);
const cnaeService = new CnaeService(apiService);
const sindicatoLaboralService = new SindicatoLaboralService(apiService, apiLegadoService);
const sindicatoPatronalService = new SindicatoPatronalService(apiService, apiLegadoService);
const docSindService = new DocSindService(apiService);
const tipoDocService = new TipoDocService(apiService);
const estruturaClausulaService = new EstruturaClausulaService(apiService);

let grupoEconomicoSelect = null;
let tipoAcordoSelect = null;
let matrizSelect = null;
let unidadeSelect = null;
let localidadeSelect = null;
let categoriaSelect = null;
let sindicatoLaboralSelect = null;
let sindPatronalSelect = null;
let dataBaseSelect = null;
let nomeDocSelect = null;
let clausulaSelect = null;


jQuery(async ($) => {
  new Menu()

  await AuthService.initialize();

  new DatepickerrangeWrapper('#periodo');

  $("#visualizar").hide()
  $('#btn_filtrar').on('click', gerarTabela)

  configurarSelects()

  $('.horizontal-nav').removeClass('hide');
})

function configurarSelects() {
  grupoEconomicoSelect = new SelectWrapper('#grupo', { onOpened: async () => (await grupoEconomico.obterSelect()).value });
  matrizSelect = new SelectWrapper('#matriz', { onOpened: async () => (await matrizService.obterSelectTodos()).value });
  unidadeSelect = new SelectWrapper('#unidade', { onOpened: async () => (await clienteUnidadeService.obterSelect()).value });
  localidadeSelect = new SelectWrapper('#localidade', { onOpened: async () => (await localizacaoService.obterSelect()).value });
  categoriaSelect = new SelectWrapper('#categoria', { onOpened: async () => (await cnaeService.obterSelect()).value });
  sindicatoLaboralSelect = new SelectWrapper('#sind_laboral', { onOpened: async () => (await sindicatoLaboralService.obterSelect()).value });
  sindPatronalSelect = new SelectWrapper('#sind_patronal', { onOpened: async () => (await sindicatoPatronalService.obterSelect()).value });
  dataBaseSelect = new SelectWrapper('#data_base', { onOpened: async () => (await docSindService.obterSelect()).value });
  nomeDocSelect = new SelectWrapper('#nome_doc', { onOpened: async () => (await tipoDocService.obterSelect()).value });
  clausulaSelect = new SelectWrapper('#clausulaList', { onOpened: async () => (await estruturaClausulaService.obterSelect()).value });
}

async function gerarTabela() {
  if (!$("#clausulaList").val()) {
    return NotificationService.warning({
      title: 'Selecione a cláusula desejada!'
    })
  }

  const requestData = {
    "module": "indsindicais",
    "action": "gerarTabela",
    "nome_doc": $("#nome_doc").val(),
    "categoria": $("#categoria").val(),
    "localidade": $("#localidade").val(),
    "sindicato_laboral": $("#sind_laboral").val(),
    "sindicato_patronal": $("#sind_patronal").val(),
    "data_base": $("#data_base").val(),
    "lista_clausula": $("#clausulaList").val(),
    //novos campos
    "grupo": $("#grupo").val(),
    "matriz": $("#matriz").val(),
    "unidade": $("#unidade").val(),
    "periodo": $("#periodo").val()
  }

  const result = await indicesSindicaisService.generateTable(requestData)

  if (!result.value.response_data) {
    return NotificationService.warning({
      title: 'Por favor selecione um filtro!'
    })
  }

  NotificationService.success({
    title: 'Tabela gerada com sucesso!'
  })

  $("#visualizar").show()
}