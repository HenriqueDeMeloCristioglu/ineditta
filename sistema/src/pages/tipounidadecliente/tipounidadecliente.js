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
				
				
				redirectUri: "http://localhost:8000/tipounidadecliente.php", //a url da pagina que está implementando,
				
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
	imported.src = 'includes/js/bootbox/bootbox.min.js';
	document.head.appendChild(imported); 

function addTipoUnidadeCliente(){

	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "tipounidadecliente", 
				"action": "addTipoUnidadeCliente",
				"nome-input": $("#nome-input").val()
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
					title: 'Não foi posssível realizar o cadastro' + textStatus,
					showConfirmButton: false,
					timer: 2000
				})
				
			}
		}
	)
}



function updateTipoUnidadeCliente(id_tiponegocio){
			
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "tipounidadecliente", 
				"action": "updateTipoUnidadeCliente",
				"nome-input": $("#nome-inputu").val(),
				"id_tiponegocio": id_tiponegocio
				
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

				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				$("mensagem_alterado_erroru").fadeTo(1000, 500).slideUp(500, function(){
					$("mensagem_alterado_erroru").slideUp(500);
				});

			}
		}
	)
}

$('#btn-atualizar').on('click', function() {
  var id = $('#id-inputu').val();
  updateTipoUnidadeCliente(id);
});

function getByIdTipoUnidadeCliente(id_tiponegocio){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "tipounidadecliente", 
				"action": "getByIdTipoUnidadeCliente",
				"id_tiponegocio": id_tiponegocio
				
				
			}
			,success: function (data) {
				

				$("#nome-inputu").val( data.response_data.tipo_negocio );
				$("#id-inputu").val( data.response_data.id_tiponegocio );
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi posssível obter os dados!' + textStatus,
					showConfirmButton: false,
					timer: 2000
				})
				
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
