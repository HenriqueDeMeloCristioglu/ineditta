
$(document).ready(function(){	
	$("#data-inputu").mask('00/00/0000');
	$("#data-input").mask('00/00/0000');
});
	
function addCalendarioSdf(){
		
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "calendariosdf", 
				"action": "addCalendarioSdf",
				"nome-input": $("#nome-input").val(),
				"abr-input": $("#abr-input").val(),
				"data-input": $("#data-input").val()
				
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
				$("#abr-input").val('');
				$("#data-input").val('');
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				$("#mensagem_error").fadeTo(1000, 500).slideUp(500, function(){
					$("#mensagem_error").slideUp(500);
				});

				$("#nome-input").val('');
				$("#abr-input").val('');
				$("#data-input").val('');
				
				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}



function updateCalendarioSdf(idcalendário_sdf){
		
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "calendariosdf", 
				"action": "updateCalendarioSdf",
				"nome-input": $("#nome-inputu").val(),
				"abr-input": $("#abr-inputu").val(),
				"data-input": $("#data-inputu").val(),
				"idcalendário_sdf": idcalendário_sdf
				
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
  updateCalendarioSdf(id);
});

function getByIdCalendarioSdf(idcalendário_sdf){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "calendariosdf", 
				"action": "getByIdCalendarioSdf",
				"idcalendário_sdf": idcalendário_sdf
				
				
			}
			,beforeSend: function(xhr){
				//$('#preload').show();
			}
			,complete: function( xhr, textStatus ) {
				//$("#preload").hide();
			}
			,success: function (data) {
				

				$("#nome-inputu").val( data.response_data.nome_sdf );
				$("#abr-inputu").val( data.response_data.abrang_sdf );
				$("#data-inputu").val( data.response_data.data );
				$("#id-inputu").val( data.response_data.idcalendário_sdf );
				
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
