
function addFederacaoEmpregados(){
				
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "federacaoempregados", 
				"action": "addFederacaoEmpregados",
				"nome-input": $("#nome-input").val()
				
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

				$("#nome-input").val('');
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				$("#mensagem_error").fadeTo(1000, 500).slideUp(500, function(){
					$("#mensagem_error").slideUp(500);
				});

				$("#nome-input").val('');

				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}



function updateFederacaoEmpregados(id_fedemp){
			
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "federacaoempregados", 
				"action": "updateFederacaoEmpregados",
				"nome-input": $("#nome-inputu").val(),
				"id_fedemp": id_fedemp
				
			}
			,beforeSend: function(xhr){
				$("#mensagem_sucessou").hide();
			}
			,complete: function( xhr, textStatus ) {
				//$("#preload").hide();
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
				
				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}

$('#btn-atualizar').on('click', function() {
  var id = $('#id-inputu').val();
  updateFederacaoEmpregados(id);
});

function getByIdFederacaoEmpregados(id_fedemp){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "federacaoempregados", 
				"action": "getByIdFederacaoEmpregados",
				"id_fedemp": id_fedemp
				
				
			}
			,beforeSend: function(xhr){
				//$('#preload').show();
			}
			,complete: function( xhr, textStatus ) {
				//$("#preload").hide();
			}
			,success: function (data) {
				
				$("#nome-inputu").val( data.response_data.nome_fedemp );
				
				$("#id-inputu").val( data.response_data.id_fedemp );
				
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