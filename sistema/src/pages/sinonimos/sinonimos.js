function selectClausula(idClausula){
	$("#ge-input").val(idClausula);
	$("#ge-input").prop("disabled", true);
	$("#ge-inputu").val(idClausula);
	$("#ge-inputu").prop("disabled", true);
}

function selectClausulaUpdate(idClausula){
	$("#ge-input-update").val(idClausula);
	$("#ge-input-update").prop("disabled", true);
}


function addSinonimos(){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "sinonimos", 
				"action": "addSinonimos",
				"info-inputc": $("#sino-inputc").val(),
				"ge-input": $("#ge-input").val()
                
			}
			,success: function (data) {
				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'Registro cadastrado com sucesso!',
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
					timer: 2000
				})
			}
		}
	)
}

function updateSinonimos(){
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "sinonimos", 
				"action": "updateSinonimos",
				"info-input-update": $("#info-input-update").val(),
				"ge-input-update": $("#ge-input-update").val(),
				"input-id": $("#input-hide").val()
                
			}
			,success: function (data) {
				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'Registro atualizado com sucesso!',
					showConfirmButton: false,
					timer: 2000
				})
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi possível atualizar o cadastro! Status: ' + textStatus,
					showConfirmButton: false,
					timer: 2000
				})
			}
		}
	)
}

function getByIdSinonimos(id_sinonimos){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "sinonimos", 
				"action": "getByIdSinonimos",
				"id_sinonimos": id_sinonimos			
			}
			,success: function (data) {
				  
 				$("#info-input-update").val( data.response_data.nome_sinonimo );
				$("#ge-input-update").val(data.response_data.id_estruturaclausula);
				$("#input-hide").val(data.response_data.id_sinonimo);
				$("#ge-input-update").prop("disabled", true);
				$("#ge-inputu").val(data.response_data.id_estruturaclausula);
				$("#ge-inputu").prop("disabled", true);
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi possível obter os dados! Status: ' + textStatus,
					showConfirmButton: false,
					timer: 2000
				})
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

$('#btn-cancelar-clau').click(function(){

	document.location.reload(true);
	
});

$('#btn-cancelar-update').click(function(){

	document.location.reload(true);
	
});
