
function buscaAdicional(){

	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "sistemabusca", 
				"action": "getTeste",
				"codcliente": $("#codcliente").val()
				
			}
			,beforeSend: function(xhr){
				$("#mensagem_sucesso").hide();
			}
			,success: function (data) {

				$("#dscliente").attr( "disabled", "disabled" );
				
				//$("#dscliente").empty();
				$("#dscliente").val( data.response_data.busca );
				//$("#dscliente").val('Te peguei nessa!');
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				$("#mensagem_error").fadeTo(1000, 500).slideUp(500, function(){
					$("#mensagem_error").slideUp(500);
				});

				$("#dscliente").val('');
				
				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}
