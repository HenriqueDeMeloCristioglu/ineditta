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
				
				
				redirectUri: "http://localhost:8000/adtipoinformacaoadicional.php", //a url da pagina que está implementando,
				
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

function addadTipoInformacaoAdicional(){

	//Combo
	var campos = document.querySelectorAll(".campo");

	var dataCombo = []

	for (let i = 0; i < campos.length; i++) {
		
		dataCombo[i] = $(campos[i]).val()
		
	}

	if ($("#multiple").prop("checked")) {
		var tipo = "CM";
	}

	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "adtipoinformacaoadicional", 
				"action": "addadTipoInformacaoAdicional",
				"ia-input": $("#ia-input").val(),
				"tipo-input": $("#tipo-input").val(),
				"data-input": $("#data-input").val(),
				"list-group": $("#info-group-list").val(),
				"list-combo": dataCombo,
				"tipo": tipo
				
			}
			,beforeSend: function(xhr){
				$("#mensagem_sucesso").hide();
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

			}
			,error: function (jqXHR, textStatus, errorThrown) {
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi possível realizar o cadastro!',
					showConfirmButton: false,
					timer: 2000
				})
				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}

function updateadTipoInformacaoAdicional(cdtipoinformacaoadicional){

	//Combo
	var campos = document.querySelectorAll(".campo");

	var dataCombo = []

	for (let i = 0; i < campos.length; i++) {
		
		dataCombo[i] = $(campos[i]).val()
		
	}
			
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "adtipoinformacaoadicional", 
				"action": "updateadTipoInformacaoAdicional",
				"ia-input": $("#ia-inputu").val(),
				"tipo-input": $("#tipo-inputu").val(),
				"data-input": $("#data-inputu").val(),
				"cdtipoinformacaoadicional": cdtipoinformacaoadicional,
				"input-combo": dataCombo
				
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
					title: 'Cadastro atualizado com sucesso!',
					showConfirmButton: false,
					timer: 2000
				})
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi possível atualizar o cadastro!',
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
  updateadTipoInformacaoAdicional(id);
});

function getByIdadTipoInformacaoAdicional(cdtipoinformacaoadicional){

	$("#combo-options").html("")
	
	var tabelaGrupo;
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "adtipoinformacaoadicional", 
				"action": "getByIdadTipoInformacaoAdicional",
				"cdtipoinformacaoadicional": cdtipoinformacaoadicional				
			}
			,beforeSend: function(xhr){
				//$('#preload').show();
			}
			,complete: function( xhr, textStatus ) {
				//$("#preload").hide();
			}
			,success: function (data) {
				
				$("#ia-inputu").val( data.response_data.nmtipoinformacaoadicional );
				$("#tipo-inputu").empty();
				$("#tipo-inputu").append( data.response_data.idtipodado );
				//$("#tipo-inputu").val( data.response_data.idtipodado );
				$("#data-inputu").val( data.response_data.dtultatualizacao );
				$("#id-inputu").val( data.response_data.cdtipoinformacaoadicional );

				$("#combo-options").html(data.response_data.camposCombo);

				if ($("#tipo-inputu").val() == "C" || $("#tipo-inputu").val() == "CM" || $("#tipo-inputu").val() == "G" ) {
					$("#tipo-inputu").prop("disabled", true)	
				}else {
					$("#tipo-inputu").prop("disabled", false)
				}

				console.log(data.response_data.listaGrupo)

				obj = [data.response_data.listaGrupo];

			var dataSet = [];
			var tipo;
			for (let i = 0; i < obj[0].length; i++) {

				if (obj[0][i].idtipodado == "D") {
					tipo = "Data"
				}else if(obj[0][i].idtipodado == "C") {
					tipo = "Combo"
				}else if(obj[0][i].idtipodado == "P") {
					tipo = "Descrição"
				}else if(obj[0][i].idtipodado == "G") {
					tipo = "Grupo"
				}else if(obj[0][i].idtipodado == "H") {
					tipo = "Hora"
				}else if(obj[0][i].idtipodado == "N") {
					tipo = "Numérico"
				}else if(obj[0][i].idtipodado == "L") {
					tipo = "Percentual"
				}else if(obj[0][i].idtipodado == "V") {
					tipo = "Valor - R$"
				}else{
					tipo = "Texto"
				}

				dataSet[i] = [obj[0][i].checkBox, obj[0][i].nmtipoinformacaoadicional, tipo];

			}    
						
			tabelaGrupo = $('#tabelaGrupo').DataTable({
				responsive: true,
				destroy: true,
				language: {
					"sEmptyTable": "Nenhum registro encontrado",
					"sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
					"sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
					"sInfoFiltered": "(Filtrados de _MAX_ registros)",
					"sInfoPostFix": "",
					"sInfoThousands": ".",
					"sLengthMenu": "_MENU_ resultados por página",
					"sLoadingRecords": "Carregando...",
					"sProcessing": "Processando...",
					"sZeroRecords": "Nenhum registro encontrado",
					"sSearch": "Pesquisar",
					"oPaginate": {
						"sNext": "Próximo",
						"sPrevious": "Anterior",
						"sFirst": "Primeiro",
						"sLast": "Último"
					},
					"oAria": {
						"sSortAscending": ": Ordenar colunas de forma ascendente",
						"sSortDescending": ": Ordenar colunas de forma descendente"
					}
				},
				data: dataSet,
				columns: [
					{ title: "Seleção" },
					{ title: "Informação Adicional" },
					{ title: "Tipo da Informação" }
				]
			});
				
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

$("#grupo-select").css("display", "none")
$("#multiplo-combo").css("display", "none");

$("#tipo-input").change(function() {

	if ($("#tipo-input").val() == "G") {
		$("#grupo-select").css("display", "block")
		$("#options").css("display", "none");
		$("#multiplo-combo").css("display", "none");


		var tabela;
		$.ajax({
			method: "POST",
			dataType: "json",
			url: "includes/php/ajax.php",
			data: {
				"module": "adtipoinformacaoadicional", 
				"action": "getadTipoInformacaoAdicional"		
			}
		}).done(function (msg) {
			console.log(msg.response_data.listaMod)
			obj = [msg.response_data.listaMod];

			var dataSet = [];
			var tipo;
			for (let i = 0; i < obj[0].length; i++) {

				if (obj[0][i].idtipodado == "D") {
					tipo = "Data"
				}else if(obj[0][i].idtipodado == "C") {
					tipo = "Combo"
				}else if(obj[0][i].idtipodado == "P") {
					tipo = "Descrição"
				}else if(obj[0][i].idtipodado == "G") {
					tipo = "Grupo"
				}else if(obj[0][i].idtipodado == "H") {
					tipo = "Hora"
				}else if(obj[0][i].idtipodado == "N") {
					tipo = "Numérico"
				}else if(obj[0][i].idtipodado == "L") {
					tipo = "Percentual"
				}else if(obj[0][i].idtipodado == "V") {
					tipo = "Valor - R$"
				}else{
					tipo = "Texto"
				}

				dataSet[i] = [obj[0][i].checkBox, obj[0][i].nmtipoinformacaoadicional, tipo];

			}    
						
			tabela = $('#table-info').DataTable({
				responsive: true,
				destroy: true,
				language: {
					"sEmptyTable": "Nenhum registro encontrado",
					"sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
					"sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
					"sInfoFiltered": "(Filtrados de _MAX_ registros)",
					"sInfoPostFix": "",
					"sInfoThousands": ".",
					"sLengthMenu": "_MENU_ resultados por página",
					"sLoadingRecords": "Carregando...",
					"sProcessing": "Processando...",
					"sZeroRecords": "Nenhum registro encontrado",
					"sSearch": "Pesquisar",
					"oPaginate": {
						"sNext": "Próximo",
						"sPrevious": "Anterior",
						"sFirst": "Primeiro",
						"sLast": "Último"
					},
					"oAria": {
						"sSortAscending": ": Ordenar colunas de forma ascendente",
						"sSortDescending": ": Ordenar colunas de forma descendente"
					}
				},
				data: dataSet,
				columns: [
					{ title: "Seleção" },
					{ title: "Informação Adicional" },
					{ title: "Tipo da Informação" }
				]
			});
		})
	}else if($("#tipo-input").val() == "C") {
		$("#options").css("display", "block");
		$("#multiplo-combo").css("display", "block");
		$("#grupo-select").css("display", "none")
		$("#info-group-list").val("")
	}else {
		$("#grupo-select").css("display", "none")
		$("#options").css("display", "none");
		$("#multiplo-combo").css("display", "none");
		$("#info-group-list").val("");
	}
})

function addCampo() {
	var index = document.querySelectorAll(".campo").length + 1

	var campo = document.createElement("input");

	var btnDelete = document.createElement("button");

	btnDelete.className = "btn btn-danger btn-delete";

	var iconDel = document.createElement("i");

	iconDel.className = "fa fa-times"

	campo.className = "form-control campo input" + index

	var div = document.getElementById("campos");

	btnDelete.append(iconDel);
	
	
	campo.setAttribute("id", index)

	btnDelete.setAttribute("id", index)

	div.append(campo);
	div.append(btnDelete);

	btnDelete.addEventListener('click', () => {
		var inputDel = document.querySelector(".input" + index)

		inputDel.remove();
		btnDelete.remove()
	})
	
	
	
}

function addCampoUpdate() {
	var index = document.querySelectorAll(".campo").length + 1

	var campo = document.createElement("input");

	var btnDelete = document.createElement("button");

	btnDelete.className = "btn btn-danger btn-delete";

	var iconDel = document.createElement("i");

	iconDel.className = "fa fa-times"

	campo.className = "form-control campo input" + index

	var div = document.getElementById("combo-extra");

	btnDelete.append(iconDel);
	
	
	campo.setAttribute("id", index)

	btnDelete.setAttribute("id", index)

	div.append(campo);
	div.append(btnDelete);

	btnDelete.addEventListener('click', () => {
		var inputDel = document.querySelector(".input" + index)

		inputDel.remove();
		btnDelete.remove()
	})
	
	
	
}

function selectInfoGroup(id_info) {
	

	var input = document.getElementById('check'+id_info+'')

	if ($(input).is(":checked")) 
	{
		$("#info-group-list").val( $("#info-group-list").val() + " " + id_info );
		console.log('marcado')
		
	} else 
	{
		$("#info-group-list").val( ($("#info-group-list").val()+"").replace(id_info+"",'') );
		console.log('desmarcado')
	}

	console.log($("#info-group-list").val())
}

function saveModuleChangeGrupo(id_info, id_grupo){
	check = null;
	if (document.getElementById('checkInfo'+id_info+'').checked) 
	{
		check = 1;
	} else 
	{
		check = 0;
	}
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "adtipoinformacaoadicional", 
				"action": "saveModuleChangeGrupo",
				"id_info": id_info,
				"id_grupo": id_grupo,
				"check": check
				
			}
			,beforeSend: function(xhr){
				$("#mensagem_sucesso").hide();
			}
			,complete: function( xhr, textStatus ) {
				$("#preload").hide();
			}
			,success: function (data) {

				getByIdClausula(id_clausula)

				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'Cadastro atualizado com sucesso!',
					showConfirmButton: false,
					timer: 2000
				})

				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi possível realizar o cadastro!',
					showConfirmButton: false,
					timer: 2000
				})

				
				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
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

