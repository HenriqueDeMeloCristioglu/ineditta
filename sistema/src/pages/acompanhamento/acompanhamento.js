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
				
				
				redirectUri: "http://localhost:8000/acompanhamento.php", //a url da pagina que está implementando,
				
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


$(document).ready(function () {
	$("#num-inputu").mask('0000000');
	$("#num-input").mask('0000000');
});

$(function () {
	// $('#grid').gridstrap({
	//     /* default options */
	// });

	for (let i = 0; i <= 8; i++) {
		var nums = [0, 1, 2, 3, 4, 5, 6, 7]

		const idx = nums.indexOf(i);

		if (idx > -1) { // only splice array when item is found
			nums.splice(idx, 1); // 2nd parameter means remove one item only
		}

		$('#try' + i).gridstrap({
			/* default options */
		});

		$('#try' + i).mouseenter(function () {
			nums.forEach(element => {
				$('#try' + element).data('gridstrap').setAdditionalGridstrapDragTarget('#try' + i);
			});
		});
	}

	$('#right-button').click(function () {
		$('#colunas').animate({
			scrollLeft: "+=300vw"
		  }, "slow");
	});

	$('#left-button').click(function () {
		$('#colunas').animate({
			scrollLeft: "-=300vw"
		  }, "slow");
	});




	$('#try0').data('gridstrap').setAdditionalGridstrapDragTarget('#try1')
	$('#try1').data('gridstrap').setAdditionalGridstrapDragTarget('#try0');

	$('#try1').data('gridstrap').setAdditionalGridstrapDragTarget('#try2');
	$('#try2').data('gridstrap').setAdditionalGridstrapDragTarget('#try1');

	$('#try3').data('gridstrap').setAdditionalGridstrapDragTarget('#try2');
	$('#try2').data('gridstrap').setAdditionalGridstrapDragTarget('#try3');

	$('#try4').data('gridstrap').setAdditionalGridstrapDragTarget('#try3');
	$('#try3').data('gridstrap').setAdditionalGridstrapDragTarget('#try4');

	$('#try5').data('gridstrap').setAdditionalGridstrapDragTarget('#try4');
	$('#try4').data('gridstrap').setAdditionalGridstrapDragTarget('#try5');

});


function addAcompanhamento() {


	$.ajax(
		{
			url: "includes/php/ajax.php"
			, type: "post"
			, dataType: "json"
			, data: {
				"module": "acompanhamento",
				"action": "addAcompanhamento",
				"num-input": $("#num-input").val()
			}
			, beforeSend: function (xhr) {
				$("#mensagem_sucesso").hide();
			}
			, complete: function (xhr, textStatus) {
				$("#preload").hide();
			}
			, success: function (data) {

				$("#mensagem_sucesso").fadeTo(1000, 500).slideUp(500, function () {
					$("#mensagem_sucesso").slideUp(500);
				});

				$("#num-input").val('');

			}
			, error: function (jqXHR, textStatus, errorThrown) {

				$("#mensagem_error").fadeTo(1000, 500).slideUp(500, function () {
					$("#mensagem_error").slideUp(500);
				});

				$("#num-input").val('');

				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}



function updateAcompanhamento(idcnae_sinde) {


	$.ajax(
		{
			url: "includes/php/ajax.php"
			, type: "post"
			, dataType: "json"
			, data: {
				"module": "acompanhamento",
				"action": "updateAcompanhamento",
				"num-input": $("#num-inputu").val(),
				"idcnae_sinde": idcnae_sinde

			}
			, beforeSend: function (xhr) {
				$("#mensagem_sucessou").hide();
			}
			, complete: function (xhr, textStatus) {
				$("#preload").hide();
			}
			, success: function (data) {

				$("#mensagem_alterado_sucessou").fadeTo(1000, 500).slideUp(500, function () {
					$("#mensagem_alterado_sucessou").slideUp(500);
				});

				//$("#ia-inputu").val('');

			}
			, error: function (jqXHR, textStatus, errorThrown) {

				$("mensagem_alterado_erroru").fadeTo(1000, 500).slideUp(500, function () {
					$("mensagem_alterado_erroru").slideUp(500);
				});

				//$("#ia-inputu").val('');

				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}

$('#btn-atualizar').on('click', function () {
	var id = $('#id-inputu').val();
	updateAcompanhamento(id);
});

function getByIdAcompanhamento(idcnae_sinde) {

	$.ajax(
		{
			url: "includes/php/ajax.php"
			, type: "post"
			, dataType: "json"
			, data: {
				"module": "acompanhamento",
				"action": "getByIdAcompanhamento",
				"idcnae_sinde": idcnae_sinde


			}
			, beforeSend: function (xhr) {
				//$('#preload').show();
			}
			, complete: function (xhr, textStatus) {
				//$("#preload").hide();
			}
			, success: function (data) {


				$("#num-inputu").val(data.response_data.idcnae_sinde);
				$("#id-inputu").val(data.response_data.idcnae_sinde);

			}
			, error: function (jqXHR, textStatus, errorThrown) {
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

$('#btn-cancelar').click(function () {

	document.location.reload(true);

});

$('#btn-cancelar2').click(function () {

	document.location.reload(true);

});
