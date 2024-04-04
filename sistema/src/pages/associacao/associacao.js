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
				
				
				redirectUri: "http://localhost:8000/associacao.php", //a url da pagina que está implementando,
				
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


$(document).ready(function(){

	$("#cnpj-input").mask('00.000.000/0000-00');
	$("#cnpj-inputu").mask('00.000.000/0000-00');
	$(".cnpj_format").mask('00.000.000/0000-00');

	$("#num-input").mask('0000');
	$("#num-inputu").mask('0000');

	$("#fone-input").mask('00000000000');
	$("#fone-inputu").mask('00000000000');

	$("#ram-input").mask('00000000000');
	$("#ram-inputu").mask('00000000000');

	$("#ddd-input").mask('(00)');
	$("#ddd-inputu").mask('(00)');

	$("#cep-input").mask('00000-000');
	$("#cep-inputu").mask('00000-000');

	$("#dataina-input").mask('00/00/0000');
	$("#dataina-inputu").mask('00/00/0000');
	$("#dataclu-input").mask('00/00/0000');
	$("#dataclu-inputu").mask('00/00/0000');
  });
function addAssociacao(){
				
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "associacao", 
				"action": "addAssociacao",
				"sigla-input": $("#sigla-input").val(),
				"cnpj-input": $("#cnpj-input").val(),
				"sta-input": $("#sta-input").val(),
				"grau-input": $("#grau-input").val(),
				"nome-input": $("#nome-input").val(),
				"gp-input": $("#gp-input").val(),
				"cla-input": $("#cla-input").val(),
				"cat-input": $("#cat-input").val(),
				"end-input": $("#end-input").val(),
				"compl-input": $("#compl-input").val(),
				"num-input": $("#num-input").val(),
				"bairro-input": $("#bairro-input").val(),
				"cep-input": $("#cep-input").val(),
				"est-input": $("#est-input").val(),
				"mu-input": $("#mu-input").val(),
				"mail-input": $("#mail-input").val(),
				"mail2-input": $("#mail2-input").val(),
				"mail3-input": $("#mail3-input").val(),
				"fone-input": $("#fone-input").val(),
				"ddd-input": $("#ddd-input").val(),
				"ram-input": $("#ram-input").val(),
				"site-input": $("#site-input").val(),
				"tw-input": $("#tw-input").val(),
				"insta-input": $("#insta-input").val(),
				"face-input": $("#face-input").val(),
				"area-input": $("#area-input").val()
				
			}
			,beforeSend: function(xhr){
				$("#mensagem_sucesso").hide();
			}
			,complete: function( xhr, textStatus ) {
				$("#preload").hide();
			}
			,success: function (data) {

				$("#mensagem_sucesso").fadeTo(1000, 500).slideUp(500, function(){
					$("#mensagem_sucesso").slideUp(500);
				});

				 $("#sigla-input").val('');
				 $("#cnpj-input").val('');
				 $("#sta-input").val('');
				 $("#grau-input").val('');
				 $("#nome-input").val('');
				 $("#gp-input").val('');
				 $("#cla-input").val('');
				 $("#cat-input").val('');
				 $("#end-input").val('');
				 $("#compl-input").val('');
				 $("#num-input").val('');
				 $("#bairro-input").val('');
				 $("#cep-input").val('');
				 $("#est-input").val('');
				 $("#mu-input").val('');
				 $("#mail-input").val('');
				 $("#mail2-input").val('');
				 $("#mail3-input").val('');
				 $("#fone-input").val('');
				 $("#ddd-input").val('');
				 $("#ram-input").val('');
				 $("#site-input").val('');
				 $("#tw-input").val('');
				 $("#insta-input").val('');
				 $("#face-input").val('');
				 $("#area-input").val('');
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				$("#mensagem_error").fadeTo(1000, 500).slideUp(500, function(){
					$("#mensagem_error").slideUp(500);
				});

				$("#sigla-input").val('');
				$("#cnpj-input").val('');
				$("#sta-input").val('');
				$("#grau-input").val('');
				$("#nome-input").val('');
				$("#gp-input").val('');
				$("#cla-input").val('');
				$("#cat-input").val('');
				$("#end-input").val('');
				$("#compl-input").val('');
				$("#num-input").val('');
				$("#bairro-input").val('');
				$("#cep-input").val('');
				$("#est-input").val('');
				$("#mu-input").val('');
				$("#mail-input").val('');
				$("#mail2-input").val('');
				$("#mail3-input").val('');
				$("#fone-input").val('');
				$("#ddd-input").val('');
				 $("#ram-input").val('');
				 $("#site-input").val('');
				 $("#tw-input").val('');
				 $("#insta-input").val('');
				 $("#face-input").val('');
				$("#area-input").val('');
				
				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}

function updateAssociacao(id_associacao){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "associacao", 
				"action": "updateAssociacao",
				"sigla-input": $("#sigla-inputu").val(),
				"cnpj-input": $("#cnpj-inputu").val(),
				"sta-input": $("#sta-inputu").val(),
				"grau-input": $("#grau-inputu").val(),
				"nome-input": $("#nome-inputu").val(),
				"gp-input": $("#gp-inputu").val(),
				"cla-input": $("#cla-inputu").val(),
				"cat-input": $("#cat-inputu").val(),
				"end-input": $("#end-inputu").val(),
				"compl-input": $("#compl-inputu").val(),
				"num-input": $("#num-inputu").val(),
				"bairro-input": $("#bairro-inputu").val(),
				"cep-input": $("#cep-inputu").val(),
				"est-input": $("#est-inputu").val(),
				"mu-input": $("#mu-inputu").val(),
				"mail-input": $("#mail-inputu").val(),
				"mail2-input": $("#mail2-inputu").val(),
				"mail3-input": $("#mail3-inputu").val(),
				"fone-input": $("#fone-inputu").val(),
				"ddd-input": $("#ddd-inputu").val(),
				"ram-input": $("#ram-inputu").val(),
				"site-input": $("#site-inputu").val(),
				"tw-input": $("#tw-inputu").val(),
				"insta-input": $("#insta-inputu").val(),
				"face-input": $("#face-inputu").val(),
				"area-input": $("#area-inputu").val(),
				"id_associacao": id_associacao
				
			}
			,beforeSend: function(xhr){
				$("#mensagem_sucessou").hide();
			}
			,complete: function( xhr, textStatus ) {
				$("#preload").hide();
			}
			,success: function (data) {

				$("#mensagem_alterado_sucessou").fadeTo(1000, 500).slideUp(500, function(){
					$("#mensagem_alterado_sucessou").slideUp(500);
				});

				//$("#ia-inputu").val('');
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				$("mensagem_alterado_erroru").fadeTo(1000, 500).slideUp(500, function(){
					$("mensagem_alterado_erroru").slideUp(500);
				});

				//$("#ia-inputu").val('');
				
				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}

$('#btn-atualizar').on('click', function() {
  var id = $('#id-inputu').val();
  updateAssociacao(id);
});

function getByIdAssociacao(id_associacao){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "associacao", 
				"action": "getByIdAssociacao",
				"id_associacao": id_associacao
			}
			,beforeSend: function(xhr){
				//$('#preload').show();
			}
			,complete: function( xhr, textStatus ) {
				//$("#preload").hide();
			}
			,success: function (data) {
				
				$("#id-inputu").val( data.response_data.id_associacao );
				$("#sigla-inputu").val( data.response_data.sigla );
				$("#cnpj-inputu").val(data.response_data.cnpj);
				$("#sta-inputu").val(data.response_data.status);
				$("#grau-inputu").val(data.response_data.grau);
				$("#nome-inputu").val(data.response_data.nome);
				$("#gp-inputu").val(data.response_data.grupo);
				$("#cla-inputu").val(data.response_data.classe);
				$("#cat-inputu").val(data.response_data.categoria);
				$("#end-inputu").val(data.response_data.endereco);
				$("#compl-inputu").val(data.response_data.complemento);
				$("#num-inputu").val(data.response_data.numero);
				$("#bairro-inputu").val(data.response_data.bairro);
				$("#cep-inputu").val(data.response_data.cep);
				$("#est-inputu").val(data.response_data.estado);
				$("#mu-inputu").val(data.response_data.municipio);
				$("#mail-inputu").val(data.response_data.email);
				$("#mail2-inputu").val(data.response_data.email2);
				$("#mail3-inputu").val(data.response_data.email3);
				$("#fone-inputu").val(data.response_data.telefone);
				$("#ddd-inputu").val(data.response_data.ddd);
				$("#ram-inputu").val(data.response_data.ramal);
				$("#area-inputu").val(data.response_data.area_geoeconomica);
				$("#tw-inputu").val(data.response_data.twitter);
				$("#face-inputu").val(data.response_data.facebook);
				$("#insta-inputu").val(data.response_data.instagram);
				$("#site-inputu").val(data.response_data.website);
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
				/* bootbox.alert({
					message: ( 'Ocorreu um erro inesperado ao processar sua solicitação.' ),
					size: 'small'
				}); */
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
