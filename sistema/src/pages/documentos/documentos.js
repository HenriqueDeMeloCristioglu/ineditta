// Libs
import 'bootstrap';
import jQuery from 'jquery';
import $ from 'jquery';
import '../../js/utils/masks/jquery-mask-extensions.js';

// Temp
import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'datatables.net-bs5';
import 'datatables.net-responsive-bs5';

// Css libs
import 'bootstrap/dist/css/bootstrap.min.css';

// Services
import {
  TipoDocService,
  CnaeService,
  ClienteUnidadeService,
  UsuarioAdmService,
  LocalizacaoService,
  SindicatoLaboralService,
  SindicatoPatronalService,
  EstruturaClausulaService
} from '../../js/services'

// Core
import { AuthService, ApiService } from '../../js/core/index.js'
import { ApiLegadoService } from '../../js/core/api-legado.js'

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import { 
  CnaesTb, 
  UsuarioTb, 
  EmpresaTb, 
  EmpresasParaFiltrarUsuariosTb, 
  FormularioInclusao, 
  EmpresaModal,
  EmpresaFiltroUsuarioModal
} from '../../js/modules/documento-sindical-comercial/index.js';
import { AbrangenciasTb } from '../../js/modules/documento-sindical-comercial/components/datatables/abrangencias-datatable.js';
import { AtividadeEconomicaModal } from '../../js/modules/documento-sindical-comercial/components/modals/atividade-economica-modal.js';
import { NotificarModal } from '../../js/modules/documento-sindical-comercial/components/modals/notificar-modal.js';
import { AbrangenciaModal } from '../../js/modules/documento-sindical-comercial/components/modals/abrangencia-modal.js';

// Services
const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const sindicatoLaboralService = new SindicatoLaboralService(apiService, apiLegadoService);
const sindicatoPatronalService = new SindicatoPatronalService(apiService, apiLegadoService);
const tipoDocService = new TipoDocService(apiService);
const usuarioAdmService = new UsuarioAdmService(apiService, apiLegadoService)
const clienteUnidadeService = new ClienteUnidadeService(apiService, apiLegadoService)
const cnaeService = new CnaeService(apiService)
const localizacaoService = new LocalizacaoService(apiService, apiLegadoService)
const estruturaClausulaService = new EstruturaClausulaService(apiService);

import { pageContext } from '../../js/modules/documento-sindical-comercial/contexts/page-context.js';

jQuery(async () => {
  new Menu()

  await AuthService.initialize();

  inicializarFormulario();

  await inicializarDatatables();
  
  await configurarModals();

  $('.horizontal-nav').removeClass('hide');
})

function inicializarFormulario() {
  const formularioInclusao = new FormularioInclusao(
    pageContext, 
    estruturaClausulaService,
    sindicatoPatronalService,
    sindicatoLaboralService,
    tipoDocService
  )
  formularioInclusao.inicializar();
  pageContext.formulario.instancia = formularioInclusao;
}

async function inicializarDatatables() {
  new UsuarioTb(pageContext, usuarioAdmService);
  new EmpresaTb(pageContext, clienteUnidadeService)
  new EmpresasParaFiltrarUsuariosTb(pageContext, clienteUnidadeService)
  new CnaesTb(pageContext, cnaeService);
  new AbrangenciasTb(pageContext, localizacaoService);
}

async function configurarModals() {
  const empresaModal = new EmpresaModal(pageContext);
  empresaModal.configurarModal();

  const empresaFiltroUsuarioModal = new EmpresaFiltroUsuarioModal(pageContext);
  empresaFiltroUsuarioModal.configurarModal();

  const atividadeEconomicaModal = new AtividadeEconomicaModal(pageContext);
  atividadeEconomicaModal.configurarModal();

  const notificarModal = new NotificarModal(pageContext);
  notificarModal.configurarModal();

  const abrangenciaModal = new AbrangenciaModal(pageContext);
  abrangenciaModal.configurarModal();
}
