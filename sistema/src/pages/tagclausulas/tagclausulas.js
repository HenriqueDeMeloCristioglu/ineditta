// Libs
import jQuery from "jquery";
import $ from "jquery";
import "bootstrap";
import "../../js/utils/util.js";
import "datatables.net-bs5/css/dataTables.bootstrap5.css";
import "datatables.net-responsive-bs5/css/responsive.bootstrap5.css";
import "bootstrap/dist/css/bootstrap.min.css";

import DataTableWrapper from "../../js/utils/datatables/datatable-wrapper.js";

import Masker from "../../js/utils/masks/masker.js";

// Services
import {
  ClienteUnidadeService,
  GrupoClausulaService,
  UsuarioAdmService, 
  ClausulaService,
  TipoDocService,
  CnaeService,
  GrupoEconomicoService,
  EstruturaClausulaService,
  LocalizacaoService,
  MatrizService,
  MapaSindicalService,
  SindicatoLaboralService,
  SindicatoPatronalService,
  SindicatoService
} from "../../js/services"

// Core
import { AuthService, ApiService } from "../../js/core/index.js";
import { ApiLegadoService } from "../../js/core/api-legado.js";
import { UsuarioNivel } from "../../js/application/usuarios/constants/usuario-nivel";

import DatepickerrangeWrapper from "../../js/utils/daterangepicker/daterangepicker-wrapper.js";
import SelectWrapper from "../../js/utils/selects/select-wrapper.js";
import moment from "moment";
import NotificationService from '../../js/utils/notifications/notification.service.js';
import PageWrapper from "../../js/utils/pages/page-wrapper.js";
import '../../js/utils/arrays/array.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'
import DateFormatter from '../../js/utils/date/date-formatter.js';
import { ModalInfoSindicato } from "../../js/utils/components/modalInfoSindicatos/modal-info-sindicatos.js";
import { renderizarModal } from "../../js/utils/modals/modal-wrapper.js";

// Services
const apiService = new ApiService();
const apiLegadoService = new ApiLegadoService();
const mapaSindicalService = new MapaSindicalService(apiService);

const matrizService = new MatrizService(apiService);
const grupoEconomico = new GrupoEconomicoService(apiService);
const clienteUnidadeService = new ClienteUnidadeService(apiService);
const localizacaoService = new LocalizacaoService(apiService, apiLegadoService);
const cnaeService = new CnaeService(apiService);
const sindicatoLaboralService = new SindicatoLaboralService(
  apiService,
  apiLegadoService
);
const sindicatoPatronalService = new SindicatoPatronalService(
  apiService,
  apiLegadoService
);
const clausulaService = new ClausulaService(apiService, apiLegadoService);
const grupoClausulaService = new GrupoClausulaService(apiService);
const tipoDocService = new TipoDocService(apiService);
const estruturaClausulaService = new EstruturaClausulaService(apiService);
const usuarioAdmService = new UsuarioAdmService(apiService);
const sindicatoService = new SindicatoService(apiService, apiLegadoService);
const modalInfoSindicato = new ModalInfoSindicato(renderizarModal, sindicatoService, DataTableWrapper);
const usuarioAdmSerivce = new UsuarioAdmService(apiService);

let estruturasClausulasTb = null;

jQuery(async () => {
	await AuthService.initialize();
  
	new Menu();
  
	const dadosPessoais = await usuarioAdmSerivce.obterDadosPessoais();

	await carregarEstruturasClausulasTb();
});

async function carregarEstruturasClausulasTb() {
    if (estruturasClausulasTb) {
        await estruturasClausulasTb.reload();
        return;
    }

    estruturasClausulasTb = new DataTableWrapper('#estruturasClausulasTb', {
        columns: [
            { data: 'id' },
            { data: 'nome', title: "Nome da Cláusula" },
            { data: 'tipo', title: "Tipo" },
            { data: 'classe', title: "Classe" }
        ],
        ajax: async (params) => {
            return await estruturaClausulaService.obterDatatable(params);
        },
        columnDefs: [
            {
                "targets": "_all",
                "defaultContent": ""
            }
        ],
        responsive: false,
        rowCallback: function (row, data) {
            
        },
    });

    await estruturasClausulasTb.initialize();
}

////KEYCLOACK////////////////////////////////////////////////////////////

function initKeycloak() {
	const keycloak = new Keycloak();
	keycloak
		.init({
			onLoad: "login-required",
			config: {
				url: "http://localhost:8000/auth/", 
				realm: "Ineditta-prod",
				clientId: "logineditta", 
			},
			initOptions: {
				
				
				redirectUri: "http://localhost:8000/tagclausulas.php", //a url da pagina que está implementando,
				
				checkLoginIframe: false,
			},
		})
		.then(function(authenticated) {
			//alert(
			//	authenticated ?
			//	"authenticated | TOKEN: " + keycloak.token :
			//	"not authenticated"
			//);// Garantia de acesso, comentar se confirmar a autenticação e testes não forem mais necessários
			setAccessToken(keycloak.token);
            setKeyUserId(keycloak.idTokenParsed.sub);
            $("#keyusername").html(keycloak.idTokenParsed.name);
keycloak.loadUserProfile()
            .then(function(profile) {
                // alert(JSON.stringify(profile, null, "  "));

                console.log(profile);
                console.log(sessionStorage.getItem(profile.id));

                  $('body').css("display","");

                  if (!sessionStorage.getItem(profile.id)) {
                  window.location.replace("http://localhost:8000/index.php");
                }
                

                

            }).catch(function() {
                alert('Failed to load user profile');
            });
			
			//Código para encapsular o token se necessario abaixo:
			
			//Manter nesse bloco:
			
			//setAccessToken(keycloak.token); 
			
			//Copiar para o arquivo de interesse:
			
			//let access_token = "";

			//function setAccessToken(val) {
			//  access_token = val;
			//}
													
			
		})
		.catch(function(err) {
			alert("failed to initialize" + JSON.stringify(err));

		});
}

let access_token = "";

function setAccessToken(val) {
  access_token = val;
}

let key_user_id = "";

function setKeyUserId(val) {
  key_user_id = val;
}


function logout(){
  sessionStorage.clear();
	var settings = {
	  "url": "http://localhost:8000/auth/admin/realms/Ineditta-prod/users/"+key_user_id+"/logout",
	  "method": "POST",
	  "timeout": 0,
	  "headers": {
		"Authorization":"Bearer " + access_token,
	  },
	};
	
	$.ajax(settings).done(function (response) {
	  console.log(response);
        document.location.href = 'http://localhost:8000/exit.php'

	});
  
}

/*********************************************************************
 * ADICIONANDO NOVA CLÁUSULA
 ********************************************************************/
function addTagClausulas(){
	$.ajax({
		url: "includes/php/ajax.php"
		,type: "post"
		,dataType: "json"
		,data: {
			"module": "tagclausulas", 
			"action": "addTagClausulas",
			"info-inputc": $("#info-inputc").val(),
			"infoa-inputc": $("#infoa-inputc").val(),
			"infob-inputc": $("#infob-inputc").val()
			
		}
		,success: function (data) {
			Swal.fire({
				position: 'top-end',
				icon: 'success',
				title: 'Registro realizado com sucesso!',
				showConfirmButton: false,
				timer: 2000
			})
		}
		,error: function (jqXHR, textStatus, errorThrown) {
			Swal.fire({
				position: 'top-end',
				icon: 'error',
				title: 'Não foi possível realizar o cadastro!',
				showConfirmButton: false,
				timer: 2000
			})
		}
	})
}

/*********************************************************************
 * ATRIBUINDO INFORMAÇÕES ADICIONAIS - PASSO 2
 ********************************************************************/
function addTagClausulasPasso2(){

	$.ajax({
			url: "includes/php/ajax.php"
		,type: "post"
		,dataType: "json"
		,data: {
			"module": "tagclausulas", 
			"action": "addTagClausulasPasso2",	
			"up2-inputu": $("#up2-inputu").val()
				
		}
		,success: function (data) {

			Swal.fire({
				position: 'top-end',
				icon: 'success',
				title: 'Registro realizado com sucesso!',
				showConfirmButton: false,
				timer: 2000
			})
			
		}
		,error: function (jqXHR, textStatus, errorThrown) {
			Swal.fire({
				position: 'top-end',
				icon: 'error',
				title: 'Não foi possível realizar o cadastro! Status: ' + textStatus,
				showConfirmButton: false,
				timer: 2000
			})
		}
	})
}

/*********************************************************************
 * ATUALIZANDO CLÁUSULAS
 ********************************************************************/
function updateTagClausulas(id_estruturaclausula){

	$.ajax({
			url: "includes/php/ajax.php"
		,type: "post"
		,dataType: "json"
		,data: {
			"module": "tagclausulas", 
			"action": "updateTagClausulas",
			"up1-inputu": $("#up1-inputu").val(),
			"type-select": $("#type-select").val(),
			"class-select": $("#class-select").val(),
			"id_estruturaclausula": id_estruturaclausula
		}
		,success: function (data) {
			Swal.fire({
				position: 'top-end',
				icon: 'success',
				title: 'Registro atualizado com sucesso!',
				showConfirmButton: false,
				timer: 2000
			})
		}
		,error: function (jqXHR, textStatus, errorThrown) {
			Swal.fire({
				position: 'top-end',
				icon: 'error',
				title: 'Não foi possível atualizar o cadastro! Status: ' + textStatus,
				showConfirmButton: false,
				timer: 2000
			})
		}
	})
}

$('#btn-atualizar').on('click', function() {
	var id = $('#id-inputu').val();	
	updateTagClausulas(id);
});

$('#btn-criar-Passo2').on('click', function() {
	var id = $('#id2-inputu').val();
	updateTagClausulas(id);
});

$('#btn-atualizar-Passo2').on('click', function() {
	var id = $('#id3-inputu').val();
  updateTagClausulas(id);
});

function getByIdInformacoesAdicionais(id_estruturaclausula){

	var tabela;
	$.ajax({
		method: "POST",
		dataType: "json",
		url: "includes/php/ajax.php",
		data: {
			"module": "tagclausulas", 
			"action": "getTagClausulasCampos",
			"id_estruturaclausula": id_estruturaclausula			
		}
	}).done(function (msg) {

		$("#up3-inputu").val(msg.response_data.clausulaName);

		obj = [msg.response_data.listaMod];

		var dataSet = [];

		var checkBox = document.createElement("input");
		checkBox.setAttribute("type", "checkbox");

		for (let i = 0; i < obj[0].length; i++) {
			dataSet[i] = [obj[0][i].checkBox, obj[0][i].cdtipoinformacaoadicional, obj[0][i].nmtipoinformacaoadicional];
		}    
		
		tabela = $('#table-info').DataTable({
			responsive: true,
			destroy: true,
			language: {
				"sEmptyTable": "Nenhum registro encontrado",
				"sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
				"sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
				"sInfoFiltered": "(Filtrados de _MAX_ registros)",
				"sInfoPostFix": "",
				"sInfoThousands": ".",
				"sLengthMenu": "_MENU_ resultados por página",
				"sLoadingRecords": "Carregando...",
				"sProcessing": "Processando...",
				"sZeroRecords": "Nenhum registro encontrado",
				"sSearch": "Pesquisar",
				"oPaginate": {
					"sNext": "Próximo",
					"sPrevious": "Anterior",
					"sFirst": "Primeiro",
					"sLast": "Último"
				},
				"oAria": {
					"sSortAscending": ": Ordenar colunas de forma ascendente",
					"sSortDescending": ": Ordenar colunas de forma descendente"
				}
			},
			data: dataSet,
			columns: [
				{ title: "Seleção" },
				{ title: "Id" },
				{ title: "Informação Adicional" }
			]
		});
	})
}


function getByIdTagClausulas(id_estruturaclausula){

	$.ajax({
			url: "includes/php/ajax.php"
		,type: "post"
		,dataType: "json"
		,data: {
			"module": "tagclausulas", 
			"action": "getByIdTagClausulas",
			"id_estruturaclausula": id_estruturaclausula			
		}
		,success: function (data) {
			$("#id-inputu").val( data.response_data.id_estruturaclausula);						
			$("#up1-inputu").val( data.response_data.nome_clausula );
			$("#up2-inputu").val( data.response_data.sinonimos);						
			$("#input-type").html( data.response_data.type);						
			$("#input-class").html( data.response_data.class);	
		}
		,error: function (jqXHR, textStatus, errorThrown) {
			Swal.fire({
				position: 'top-end',
				icon: 'error',
				title: 'Não foi possível atualizar o cadastro! Status: ' + textStatus,
				showConfirmButton: false,
				timer: 2000
			})
		}
	})
	
}

/*********************************************************************
 * SALVANDO INFORMAÇÕES ADICIONAIS
 ********************************************************************/
function saveModuleChange(cdtipoinformacaoadicional, id_estruturaclausula){

	check = null;
	if (document.getElementById('check'+cdtipoinformacaoadicional+'').checked) 
	{
		check = 1;
	} else 
	{
		check = 0;
	}	
	
	console.log(check)
	$.ajax({
		url: "includes/php/ajax.php"
		,type: "post"
		,dataType: "json"
		,data: {
			"module": "tagclausulas", 
			"action": "saveModuleChange",
			"cdtipoinformacaoadicional": cdtipoinformacaoadicional,
			"id_estruturaclausula": id_estruturaclausula,
			"check": check
		}
		,success: function (data) {
			Swal.fire({
				position: 'top-end',
				icon: 'success',
				title: 'Registro atualizado com sucesso!',
				showConfirmButton: false,
				timer: 2000
			})
		}
		,error: function (jqXHR, textStatus, errorThrown) {
			Swal.fire({
				position: 'top-end',
				icon: 'error',
				title: 'Não foi possível atualizar o cadastro! Status: ' + textStatus,
				showConfirmButton: false,
				timer: 2000
			})
		}
	})
}

$(".btn-cancelar").on("click", () => {
	document.location.reload(true);
})
