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
				
				
				redirectUri: "http://localhost:8000/modulos.php", //a url da pagina que está implementando,
				
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


var imported = document.createElement('script');
var imported = document.createElement('script');
	imported.src = 'includes/plugins/bootbox/bootbox.min.js';
	document.head.appendChild(imported); 

function addMd(){
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "html"
			,data: {
				"module": "modulos", 
				"action": "addMd",
				"mod-inputc": $("#mod-inputc").val(),
				"tipo-input": $("#tipo-input").val(),
				"name-file": $("#name-file").val() + ".php"
				
				,"check-insert": $("#check-insert").val()
				,"check-select": $("#check-select").val()
				,"check-update": $("#check-update").val()
				,"check-delete": $("#check-delete").val()
				,"check-comentario": $("#check-comentario").val()
				,"check-aprovar": $("#check-aprovar").val()
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

				$("#mod-inputc").val('');
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi posssível realizar o cadastro' + textStatus + " " + errorThrown,
					showConfirmButton: false,
					timer: 2000
				})

				$("#mod-inputc").val('');
				
				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}


function updateModulos(id_modulos){
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "modulos", 
				"action": "updateModulos",
				"mod-inputu": $("#mod-inputu").val(),
				"name-file-update": $("#name-file-update").val() + ".php",
				"id_modulos": id_modulos
				
				,"check-insert-update": $("#check-insert-u").val()
				,"check-select-update": $("#check-select-u").val()
				,"check-update-update": $("#check-update-u").val()
				,"check-delete-update": $("#check-delete-u").val()
				,"check-comentario-update": $("#check-comentario-u").val()
				,"check-aprovar-update": $("#check-aprovar-u").val()
				
			}
			,beforeSend: function(xhr){
				//$("#mensagem_sucessou").hide();
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

function getByIdMoDu(id_modulos){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "modulos", 
				"action": "getByIdMoDu",
				"id_modulos": id_modulos
				
				
			}
			,beforeSend: function(xhr){
				//$('#preload').show();
			}
			,complete: function( xhr, textStatus ) {
				//$("#preload").hide();
			}
			,success: function (data) {
				
				$("#id-inputu").val( data.response_data.id_modulos );
				$("#mod-inputu").val( data.response_data.modulos );
				$("#name-file-update").val( data.response_data.name_file_view.replace('.php', '') );
				
				( data.response_data.tipo == "SISAP" )? $("#tipo-inputu option:contains(SISAP)").attr('selected', true) : $("#tipo-inputu option:contains(Comercial)").attr('selected', true);

				( data.response_data.criar_view == "S" )? $("#check-insert-u option:contains(Sim)").attr('selected', true) : $("#check-insert-u option:contains(Nao Aplica)").attr('selected', true);
				( data.response_data.consultar_view == "S" )? $("#check-select-u option:contains(Sim)").attr('selected', true) : $("#check-select-u option:contains(Nao Aplica)").attr('selected', true);
				( data.response_data.alterar_view == "S" )? $("#check-update-u option:contains(Sim)").attr('selected', true) : $("#check-update-u option:contains(Nao Aplica)").attr('selected', true);
				( data.response_data.excluir_view == "S" )? $("#check-delete-u option:contains(Sim)").attr('selected', true) : $("#check-delete-u option:contains(Nao Aplica)").attr('selected', true);
				( data.response_data.comentar_view == "S" )? $("#check-comentario-u option:contains(Sim)").attr('selected', true) : $("#check-comentario-u option:contains(Nao Aplica)").attr('selected', true);
				( data.response_data.aprovar_view == "S" )? $("#check-aprovar-u option:contains(Sim)").attr('selected', true) : $("#check-aprovar-u option:contains(Nao Aplica)").attr('selected', true);
				
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

$('#btn-atualizar').on('click', function() {
  var id = $('#id-inputu').val();
  updateModulos(id);
});


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
