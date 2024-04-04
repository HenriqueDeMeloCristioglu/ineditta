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
				
				
				redirectUri: "http://localhost:8000/clienteusuarios.php", //a url da pagina que está implementando,
				
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
	$("#ddd-inputu").mask('00');
	$("#ddd-input").mask('00');

	$("#dddf-inputu").mask('00');
	$("#dddf-input").mask('00');

	$("#data-inputu").mask('00-00-0000');
	$("#data-input").mask('00-00-0000');

	$("#cel-inputu").mask('00000-0000');
	$("#cel-input").mask('00000-0000');

	$("#celf-inputu").mask('0000-0000');
	$("#celf-input").mask('0000-0000');

	$("#perio-inputu").mask('000');
	$("#perio-input").mask('000');
});

function addClienteUsuarios(){
			

	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "clienteusuarios", 
				"action": "addClienteUsuarios",
				"nome-input": $("#nome-input").val(),
				"ddd-input": $("#ddd-input").val(),
				"cel-input": $("#cel-input").val(),
				"dddf-input": $("#dddf-input").val(),
				"celf-input": $("#celf-input").val(),
				"func-input": $("#func-input").val(),
				"email-input": $("#email-input").val(),
				"depto-input": $("#depto-input").val(),
				"senhaatual-input": $("#senhaatual-input").val(),
				"senhaantiga-input": $("#senhaantiga-input").val(),
				"data-input": $("#data-input").val(),
				"perio-input": $("#perio-input").val()
				
			}
			,beforeSend: function(xhr){
				//$("#mensagem_sucesso").hide();
			}
			,complete: function( xhr, textStatus ) {
				$("#preload").hide();
			}
			,success: function (data) {

				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'Cadastro realizado com sucesso!',
					showConfirmButton: false,
					timer: 2000
				})

				 $("#nome-input").val('');
				$("#ddd-input").val('');
				$("#cel-input").val('');
				$("#dddf-input").val('');
				$("#celf-input").val('');
				 $("#func-input").val('');
				$("#email-input").val('');
				$("#depto-input").val('');
				$("#senhaatual-input").val('');
				$("#senhaantiga-input").val('');
				$("#data-input").val('');
				$("#perio-input").val('');
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi posssível realizar o cadastro' + textStatus,
					showConfirmButton: false,
					timer: 2000
				})

				$("#ia-input").val('');
				
				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}



function updateClienteUsuarios(id_usuario){
			

	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "clienteusuarios", 
				"action": "updateClienteUsuarios",
				"nome-input": $("#nome-inputu").val(),
				"ddd-input": $("#ddd-inputu").val(),
				"cel-input": $("#cel-inputu").val(),
				"dddf-input": $("#ddd-inputu").val(),
				"celf-input": $("#cel-inputu").val(),
				"func-input": $("#func-inputu").val(),
				"email-input": $("#email-inputu").val(),
				"depto-input": $("#depto-inputu").val(),
				"senhaatual-input": $("#senhaatual-inputu").val(),
				"senhaantiga-input": $("#senhaantiga-inputu").val(),
				"data-input": $("#data-inputu").val(),
				"denominacao-input": $("#denominacao-inputu").val(),
				"perio-input": $("#perio-inputu").val(),
				"id_usuario": id_usuario
				
			}
			,beforeSend: function(xhr){
				$("#mensagem_sucessou").hide();
			}
			,complete: function( xhr, textStatus ) {
				$("#preload").hide();
			}
			,success: function (data) {

				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'Atualização realizada com sucesso!',
					showConfirmButton: false,
					timer: 2000
				})

				//$("#ia-inputu").val('');
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi posssível realizar a atualização' + textStatus,
					showConfirmButton: false,
					timer: 2000
				})

				//$("#ia-inputu").val('');
				
				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}

$('#btn-atualizar').on('click', function() {
  var id = $('#id-inputu').val();
  updateClienteUsuarios(id);
});

function getByIdClienteUsuarios(id_usuario){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "clienteusuarios", 
				"action": "getByIdClienteUsuarios",
				"id_usuario": id_usuario
				
				
			}
			,beforeSend: function(xhr){
				//$('#preload').show();
			}
			,complete: function( xhr, textStatus ) {
				//$("#preload").hide();
			}
			,success: function (data) {
				
				$("#nome-inputu").val( data.response_data.nome_usuario );
				$("#ddd-inputu").val( data.response_data.ddd_usuario );
				$("#cel-inputu").val( data.response_data.celular_usuario );
				$("#dddf-inputu").val( data.response_data.ddd_telefone );
				$("#celf-inputu").val( data.response_data.telefone_usuario );
				$("#func-inputu").val( data.response_data.funcao_usuario );
				$("#email-inputu").val( data.response_data.email_usuario );
				$("#depto-inputu").val( data.response_data.depto_usuario );
				$("#perio-inputu").val( data.response_data.periodicidade );
				$("#id-inputu").val( data.response_data.id_usuario );
				
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


$(function () {
  'use strict';
   
  // Initialize the jQuery File Upload widget:
  $('#fileupload').fileupload({
    // Uncomment the following to send cross-domain cookies:
    //xhrFields: {withCredentials: true},
    url: 'includes/php/ajax.php'
  });

  // Enable iframe cross-domain access via redirect option:
  $('#fileupload').fileupload(
    'option',
    'redirect',
    window.location.href.replace(/\/[^/]*$/, '/cors/result.html?%s')
  );

  if (window.location.hostname === 'blueimp.github.io') {
    // Demo settings:
    $('#fileupload').fileupload('option', {
      url: '//jquery-file-upload.appspot.com/',
      // Enable image resizing, except for Android and Opera,
      // which actually support image resizing, but fail to
      // send Blob objects via XHR requests:
      disableImageResize: /Android(?!.*Chrome)|Opera/.test(
        window.navigator.userAgent
      ),
      maxFileSize: 999000,
      acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i
    });
    // Upload server status check for browsers with CORS support:
    if ($.support.cors) {
      $.ajax({
        url: '//jquery-file-upload.appspot.com/',
        type: 'HEAD'
      }).fail(function () {
        $('<div class="alert alert-danger"></div>')
          .text('Upload server currently unavailable - ' + new Date())
          .appendTo('#fileupload');
      });
    }
  } else {
	
    // Load existing files:
    $('#fileupload').addClass('fileupload-processing');
    $.ajax({
      // Uncomment the following to send cross-domain cookies:
      //xhrFields: {withCredentials: true},
      url: $('#fileupload').fileupload('option', 'url'),
      dataType: 'json',
	  context: $('#fileupload')[0]
    })
      .always(function () {
        $(this).removeClass('fileupload-processing');
      })
      .done(function (result) {
        $(this)
          .fileupload('option', 'done')
          // eslint-disable-next-line new-cap
          .call(this, $.Event('done'), { result: result });
      });
  }
});


$('#btn-cancelar').click(function(){

	document.location.reload(true);
	
});

$('#btn-cancelar2').click(function(){

	document.location.reload(true);
	
});
