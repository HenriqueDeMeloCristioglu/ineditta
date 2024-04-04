import $ from "jquery";
import 'jquery-ui';
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
				
				
				redirectUri: "http://localhost:8000/acompanhamento_cct.php", //a url da pagina que está implementando,
				
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

	  document.location.reload(true);
	});
  
}


/*********************************************************************
 * ADICIONANDO REGISTRO
 ********************************************************************/

function addAcompanhamento(){
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "acompanhamento_cliente",
				"action": "addAcompanhamento",
				"fase": $("#fase-inputu").val(),
				"comentario": $("#comentario-inputu").val(),
				"id": $("#id-inputu").val()
			}
			,beforeSend: function(xhr){
				// $("#mensagem_sucesso").hide();
			}
			,complete: function( xhr, textStatus ) {
				// $("#preload").hide();
			}
			,success: function (data) {

				Swal.fire({
				position: 'top-end',
				icon: 'success',
				title: 'Cadastro realizado com sucesso!',
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
					timer: 7000
					})
			}
		}
	)
}


/*********************************************************************
 * UPDATE
 ********************************************************************/

 function getById(id, edit){

	console.log(id)

	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "acompanhamento_cliente",
				"action": "getById",
				"id": id
			}
			,beforeSend: function(xhr){
				//$('#preload').show();
			}
			,complete: function( xhr, textStatus ) {
				//$("#preload").hide();
			}
			,success: function (data) {

				var div = document.querySelectorAll(".timeline-box")
					
					div.forEach(element => {
						element.remove();
					});

				if (edit == 1) {
					$("#adicionar").css("display", "none")

					$("#timeline-place").append(data.response_data.timeline)

				}else {
					$("#adicionar").css("display", "block")
				}


				$("#id-inputu").val(id)
				$("#sinde-inputu").val(data.response_data.emp);
				$("#patr-inputu").val(data.response_data.sind);
				$("#data-inputu").val(data.response_data.db);
				$("#categoria-inputu").val(data.response_data.cnae);
				$("#fase-inputu").html(data.response_data.optionFase);
				$("#fase-inputu").val(data.response_data.fase);
				
				$("#sinde-inputu").prop("disabled", true);
				$("#patr-inputu").prop("disabled", true);
				$("#data-inputu").prop("disabled", true);
				$("#categoria-inputu").prop("disabled", true);

			}
			,error: function (jqXHR, textStatus, errorThrown) {
				window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
				console.log(jqXHR)
				/* bootbox.alert({
					message: ( 'Ocorreu um erro inesperado ao processar sua solicitação.' ),
					size: 'small'
				}); */
			}
		}
	)
}


function getTimelineById(id){
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "acompanhamento_cliente",
				"action": "getTimelineById",
				"id": id
			}
			,beforeSend: function(xhr){
				//$('#preload').show();
			}
			,complete: function( xhr, textStatus ) {
				//$("#preload").hide();
			}
			,success: function (data) {

				$("#fase-update").html(data.response_data.optionFaseUpdate)
				$("#fase-update").val(data.response_data.fase_up)
				$("#comentario-update").val(data.response_data.comentario_up)
				$("#id-update").val(id)
			
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
				console.log(jqXHR)
				/* bootbox.alert({
					message: ( 'Ocorreu um erro inesperado ao processar sua solicitação.' ),
					size: 'small'
				}); */
			}
		}
	)
}


function updateComentario(){
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "acompanhamento_cliente",
				"action": "updateComentario",
				"fase-update": $("#fase-update").val(),
				"comentario-update": $("#comentario-update").val(),
				"id-update": $("#id-update").val()
			}
			,beforeSend: function(xhr){
				// $("#mensagem_sucessou").hide();
			}
			,complete: function( xhr, textStatus ) {
				// $("#preload").hide();
			}
			,success: function (data) {

				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'Cadastro atualizado com sucesso!',
					showConfirmButton: false,
					timer: 3000
					})

			}
			,error: function (jqXHR, textStatus, errorThrown) {

				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Falha ao atualizar registro!',
					showConfirmButton: false,
					timer: 3000
					})

				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}


$('#btn-cancelar').click(function(){

	document.location.reload(true);

});

$('#btn-cancelar2').click(function(){

	document.location.reload(true);

});
