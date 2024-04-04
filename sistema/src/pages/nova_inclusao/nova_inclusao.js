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
				
				
				redirectUri: "http://localhost:8000/notificacao.php", //a url da pagina que está implementando,
				
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


$(function () {
	$(".select2").select2();

	$("#novo").on("click", function() {
		$("#up-hidden").val("")

		$("#validade").prop("disabled", true)

		$(".btn-slc").addClass("disabled")
	})

	//Botão buscar tipo comentário
	$("#assunto").prop("disabled", true)
	$("#tipo-com").on("change", function () {
		console.log('entrou');
		$.ajax(
			{
				url: "includes/php/ajax.php"
				,type: "post"
				,dataType: "json"
				,data: {
					"module": "notificacao",
					"action": "getTipoComentario",
					"tipo_comentario": $("#tipo-com").val()
				}
				,success: function (data) {

					$("#assunto").prop("disabled", false)

					$("#assunto").html(data.response_data.tipo_com)

					if ($("#tipo-com").val() == "clausula") {
						$("#assunto_title").html("Assunto")
					}else if($("#tipo-com").val() != "clausula" && $("#tipo-com").val() != "--") {
						$("#assunto_title").html("Sindicato")
					}else {
						$("#assunto_title").html("--")
						$("#assunto").html("")
						$("#assunto").prop("disabled", true)
					}

					setTimeout(() => {
						$("#assunto").select2()
					}, 300)

				}
				,error: function (jqXHR, textStatus, errorThrown) {

					Swal.fire({
						position: 'top-end',
						icon: 'error',
						title: 'Não foi possível realizar o cadastro! Status: ' + textStatus,
						showConfirmButton: false,
						timer: 7000
						})
				}
			}
		)

	})

	//Botão buscar tipo usuário
	$("#campo_destino").prop("disabled", true)

	$("#destino").on("change", () => {

		$.ajax(
		  {
			url: "includes/php/ajax.php"
			, type: "post"
			, dataType: "json"
			, data: {
			  "module": "notificacao",
			  "action": "getDestinoNotificacao"
			}
			, success: function (data) {
	  
			  $("#campo_destino").prop("disabled", false)
	  
			  if ($("#destino").val() === "matriz") {
				$("#campo_destino").html(data.response_data.lista_matriz)
				$("#campo_tipo").html("Matriz")
			  }else if ($("#destino").val() === "grupo") {
				$("#campo_destino").html(data.response_data.lista_grupo)
				$("#campo_tipo").html("Grupo Econômico")
			  }else if ($("#destino").val() === "unidade") {
				$("#campo_destino").html(data.response_data.lista_unidade)
				$("#campo_tipo").html("Filial")
			  }else {
				$("#campo_destino").html('')
				$("#campo_tipo").html("--")
				$("#campo_destino").prop("disabled", true)
			  }
	  
			  setTimeout(() => {
				$("#campo_destino").select2()
			  }, 300)
	  
			}
			, error: function (jqXHR, textStatus, errorThrown) {
	  
			  Swal.fire({
				position: 'top-end',
				icon: 'error',
				title: 'Não foi possível obter as listas! Status:' + textStatus,
				showConfirmButton: false,
				timer: 2500
			  })
	  
			}
		  }
		)
	})

	//Tipo de notificação
	$("#tipo-note").on("change", function () {

		if ($("#tipo-note").val() == "fixa" || $("#tipo-note").val() == "--") {
			$("#validade").prop("disabled", true)
			$("#validade").val("")
		}else {
			$("#validade").prop("disabled", false)
		}

		
	})

})


/*********************************************************************
 * SELECIONAR TIPO COMENTARIO
 ********************************************************************/

function selectTipoComentario(id) {
	if ($("#up-hidden").val() == "update") {
		$("#com-selected-up").val(id)
		$(".btn-tipo-com").attr("href", "#myModalUpdate")
	}else {
		$("#com-selected").val(id)
		$(".btn-tipo-com").attr("href", "#myModal")
	}
}


/*********************************************************************
 * SELECIONAR TIPO USUÁRIO
 ********************************************************************/

function selectTipoUsuario(id) {
	if ($("#up-hidden").val() == "update") {
		$("#user-selected-up").val(id)
		$(".btn-tipo-user").attr("href", "#myModalUpdate")
	}else {
		$("#user-selected").val(id)
		$(".btn-tipo-user").attr("href", "#myModal")
	}
}

/*********************************************************************
 * ADICIONANDO REGISTRO
 ********************************************************************/

function addNotificacao(){


	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "notificacao",
				"action": "addNotificacao",
				"tipo_com": $("#tipo-com").val(),
				"tipo_com_selected": $("#assunto").val(),
				"destino": $("#destino").val(),
				"id_destino": $("#campo_destino").val(),
				"tipo_note": $("#tipo-note").val(),
				"validade": $("#validade").val(),
				"usuario": $("#usuario").val(),
				"comentario": $("#comentario").val()
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
					title: 'Não foi possível realizar o cadastro! Status: ' + textStatus,
					showConfirmButton: false,
					timer: 7000
				})
			}
		}
	)
}

function getClausulaByDoc(id_doc) {

	var table;

	$.ajax({
		method: "POST",
		dataType: "json",
		url: "includes/php/ajax.php",
		data: {
			"module": "notificacao",
			"action": "getByDocClausula",
			"id_doc": id_doc
		}
	}).done(function (msg) {

		obj = [msg.response_data.lista_clausulas];

		console.log(obj);

		var dataSet = [];

		for (let i = 0; i < obj[0].length; i++) {

			dataSet[i] = [obj[0][i].button, obj[0][i].id_clau, obj[0][i].doc_sind_id_documento, obj[0][i].tex_clau.substr(0, 200) + "...", obj[0][i].aprovada, obj[0][i].data_aprovacao];
		}

		table = $('#table-list-clausulas').DataTable({
			responsive: true,
			destroy: true,
			data: dataSet,
			columns: [
				{ title: "Visualizar" },
				{ title: "ID Cláusula" },
				{ title: "ID Doc Sindical" },
				{ title: "Cláusula" },
				{ title: "Aprovação" },
				{ title: "Data Aprovação" }
				
			],
			columnDefs: [
				{ width: '20%', targets: 4 },
        
			],
		});
	})
}

/*********************************************************************
 * UPDATE
 ********************************************************************/

 function getById(id){
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "notificacao",
				"action": "getById",
				"id": id
			}
			,success: function (data) {

				if (data.response_data.tipo_notificacao == "fixa") {
					$("#validade_up").prop("disabled", true)
					$("#validade_up").val('');
					$("#tipo-note-up option[value='fixa']").prop("selected", true)
				}else {
					$("#validade_up").prop("disabled", false)
					$("#validade_up").val(data.response_data.data_final);
					$("#tipo-note-up option[value='temporaria']").prop("selected", true)
				}

				$("#tipo-note-up").on("change", function() {
					if ($("#tipo-note-up").val() == "temporaria") {
						$("#validade_up").prop("disabled", false)
					}else {
						$("#validade_up").prop("disabled", true)
						$("#validade_up").val("")
					}
				})
				
				$("#tipo-com-up").val(data.response_data.tipo_com);
				$("#assunto_up").val(data.response_data.assunto_up);
				$("#destino_up").val(data.response_data.tipo_usuario_destino);
				$("#campo_destino_up").val(data.response_data.destino);
				$("#tipo-note-up").val(data.response_data.tipo_notificacao);
				
				$("#usuario_up").val(data.response_data.usuario);
				$("#comentario_up").val(data.response_data.comentario);
				$("#up-hidden").val("update")
				$("#id_note").val(id)

				$(".btn-slc").removeClass("disabled")

				//ALTERA TITULOS
				if (data.response_data.tipo_com == "Clausula") {
					$("#assunto_titulo_up").html("Assunto")
				}else {
					$("#assunto_titulo_up").html("Sindicato")
				}

				if (data.response_data.tipo_usuario_destino == "Grupo") {
					$("#campo_tipo_up").html("Grupo Econômico")
				}else if(data.response_data.tipo_usuario_destino == "Matriz") {
					$("#campo_tipo_up").html("Matriz")
				}else {
					$("#campo_tipo_up").html("Filial")
				}

				$(".select2").select2()
				

			}
			,error: function (jqXHR, textStatus, errorThrown) {
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi possível obter os dados! Status: ' + textStatus,
					showConfirmButton: false,
					timer: 1500
				})
			}
		}
	)
}


function updateNotificacao(){

	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "notificacao",
				"action": "updateNotificacao",
				"tipo_note": $("#tipo-note-up").val(),
				"validade": $("#validade_up").val(),
				"comentario": $("#comentario_up").val(),
				"id": $("#id_note").val()
			}
			,success: function (data) {

				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'Cadastro atualizado com sucesso!',
					showConfirmButton: false,
					timer: 1500
					})

			}
			,error: function (jqXHR, textStatus, errorThrown) {

				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Falha ao atualizar registro! Status: ' + textStatus,
					showConfirmButton: false,
					timer: 3000
					})

				//window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
			}
		}
	)
}

 /*********************************************************************
 * BOTÃO FINALIZAR
 ********************************************************************/

var btnCancelar = document.querySelectorAll(".btn-cancelar");

btnCancelar.forEach(btn => {
	btn.addEventListener("click", () => document.location.reload(true))
});


