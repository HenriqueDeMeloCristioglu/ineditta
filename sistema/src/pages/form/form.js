
function addTicket(){
			window.alert("Entrou");
		
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "configform", 
				"action": "addTicketByData",
				"nomeForm": $("#nomeForm").val(),
				"cdcliente": $("#cdcliente").val(),
				"cdcontato": $("#cdcontato").val(),
				"cdlocalidade": $("#cdlocalidade").val(),
				"centroResponsabilidade": $("#centroResponsabilidade").val(),
				"cdEmpresa": $("#empresa").val(),
				"dsEmpresa": $("#empresa option:selected").text(),
				"cdAtividade": $("#atividade").val(),
				"dsAtividade": $("#atividade option:selected").text(),
				"cdCcusto": $("#ccusto").val(),
				"dsCcusto": $("#ccusto option:selected").text(),
				"cdAlmoxarifado": $("#almoxarifado").val(),
				"dsAlmoxarifado": $("#almoxarifado option:selected").text(),
				"dtNecessidade": $("#dtnecessidade").val(),
				"observacao": $("#observacao").val(),
				"destino": $("#destino option:selected").text(),
				"cddemanda": $("#demanda").val(),
				"produtos": JSON.stringify( $('#tabelaProdutos').DataTable().data().toJQuery() ),
				"agregados": JSON.stringify( $('#tabelaAgregados').DataTable().data().toJQuery() )
			}
			,beforeSend: function(xhr){
				$('#preload').show();
			}
			,complete: function( xhr, textStatus ) {
				$("#preload").hide();
			}
			,success: function (data) {
				
				if( data.response_status.status == 1 ) {
					
					console.log( data.response_data.cdchamado );					
					var nrochamado = data.response_data.cdchamado;
					
					$('#fileupload').bind('fileuploadsubmit', function (e, data) {

						data.formData = { 
							"module": "configform"
							,"action": "addAttachment"
							,"cdchamado": nrochamado
						};
					});
					
//					console.log( '====================' );
					
					$('.start').trigger('click');
					
					$('#start_all').show();				
					$("#preload").hide()
					
					bootbox.alert({
						message: ( data.response_status.msg ),
						size: 'small'
					});
				} 
				else{
					bootbox.alert({
						message: ( data.response_status.error_code + ': ' + data.response_status.msg ),
						size: 'small'
					});
				}
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				bootbox.alert({
					message: ( 'Ocorreu um erro inesperado ao processar sua solicitação.' ),
					size: 'small'
				});
			}
		}
	)
}
 