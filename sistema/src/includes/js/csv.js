////KEYCLOACK////////////////////////////////////////////////////////////

function initKeycloak() {
	const keycloak = new Keycloak();
	keycloak
		.init({
			onLoad: "login-required",
			config: {
				url: "http://localhost:8443/",
				realm: "Ineditta-prod",
				clientId: "logineditta",
			},
			initOptions: {


				redirectUri: "http://localhost:8080/geradorCsv.php", //a url da pagina que está implementando,

				checkLoginIframe: false,
			},
		})
		.then(function (authenticated) {
			//alert(
			//	authenticated ?
			//	"authenticated | TOKEN: " + keycloak.token :
			//	"not authenticated"
			//);// Garantia de acesso, comentar se confirmar a autenticação e testes não forem mais necessários
			setAccessToken(keycloak.token);
			setKeyUserId(keycloak.idTokenParsed.sub);
			$("#keyusername").html(keycloak.idTokenParsed.name);
			keycloak.loadUserProfile()
				.then(function (profile) {
					// alert(JSON.stringify(profile, null, "  "));

					console.log(profile);
					console.log(sessionStorage.getItem(profile.id));

					$('body').css("display", "");

					if (!sessionStorage.getItem(profile.id)) {
						window.location.replace("http://localhost:8080/index.php");
					}




				}).catch(function () {
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
		.catch(function (err) {
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


function logout() {
	sessionStorage.clear();
	var settings = {
		"url": "http://localhost:8443/admin/realms/Ineditta-prod/users/" + key_user_id + "/logout",
		"method": "POST",
		"timeout": 0,
		"headers": {
			"Authorization": "Bearer " + access_token,
		},
	};

	$.ajax(settings).done(function (response) {
		console.log(response);
		document.location.href = 'http://localhost:8080/exit.php'

	});

}


$(function () {

	//Date range picker
	$('.date_format').daterangepicker({
		"locale": {
			"format": "DD/MM/YYYY",
			"separator": " - ",
			"applyLabel": "Aplicar",
			"cancelLabel": "Cancelar",
			"fromLabel": "De",
			"toLabel": "Até",
			"customRangeLabel": "Custom",
			"daysOfWeek": [
				"Dom",
				"Seg",
				"Ter",
				"Qua",
				"Qui",
				"Sex",
				"Sáb"
			],
			"monthNames": [
				"Janeiro",
				"Fevereiro",
				"Março",
				"Abril",
				"Maio",
				"Junho",
				"Julho",
				"Agosto",
				"Setembro",
				"Outubro",
				"Novembro",
				"Dezembro"
			],
		},
		autoUpdateInput: false,
	})


		setTimeout(() => {
			//Assuming URL has "?post=1234&action=edit"
	  
			var urlParams = new URLSearchParams(window.location.search);
			if(urlParams.has('id_clau')){
				let idClau = urlParams.get('id_clau');
				let idGrupo = urlParams.get('id_grupo_clau');
				gerarTabela(idClau, idGrupo);
			}
		}, 1000)
	  
	  

	$("#btn_tipo").addClass("disabled")

	$("#preencher").prop("disabled", true)
	$("#save-filter").prop("disabled", true)

	$("#visualizar").css("display", "none")

	// $('#grupo').select2({
	// 	placeholder: "Nome"
	// });

	$(".select2").select2()


	//Buscar filtro salvo
	$.ajax(
		{
			url: "includes/php/ajax.php"
			, type: "post"
			, dataType: "json"
			, data: {
				"module": "gera_csv",
				"action": "getLists"
			}
			, success: function (data) {

				filtro = data.response_data.filtro
				filtroSalvo = data.response_data.filtro_salvo

				if (filtro == true) {
					console.log(filtroSalvo)

					$("#preencher").prop("disabled", false)

					$("#preencher").on("click", function () {

						console.log(filtroSalvo.sindicato)

						$("#reservation").val(filtroSalvo.vigencia);
						$("#nome_doc").val(filtroSalvo.nome_doc);
						$("#localidade").val(filtroSalvo.localidade);
						$("#grupo_clausulas").val(filtroSalvo.grupo_clausula);
						$("#data_base").val(filtroSalvo.data_base);
						$('#categoria').val(filtroSalvo.categoria)
						$('#grupo').val(filtroSalvo.grupo)
						$('#sind_laboral').val(filtroSalvo.sindicato_laboral)
						$('#sind_patronal').val(filtroSalvo.sindicato_patronal)
						$('#vigencia').val(filtroSalvo.vigencia)

						if (filtroSalvo.matriz) {
							// $('#matriz').prop("disabled", true)
							getMatriz()

							setTimeout(() => {
								$('#matriz').val(filtroSalvo.matriz)
								$(".select2").select2()
							}, 200)
						}

						if (filtroSalvo.unidade) {
							$('#unidade').prop("disabled", true)
							getUnidade(filtroSalvo.matriz)

							setTimeout(() => {
								$('#unidade').val(filtroSalvo.unidade)
								$(".select2").select2()
							}, 300)
						}

						if (filtroSalvo.lista_clausula) {
							$('#clausulaList').prop("disabled", true)
							getClausulas(filtroSalvo.grupo_clausula)

							setTimeout(() => {
								$('#clausulaList').val(filtroSalvo.lista_clausula)
								$(".select2").select2()
							}, 300)
						}

						// $.each(clausulas, function(idx, val) {
						// 	$('#clausulaList option[value=' + val + ']').attr("selected", true);

						// 	var elem = $('#clausulaList option[value=' + val + ']')

						// });

						$(".select2").select2()
					})
				}

			}
			, error: function (jqXHR, textStatus, errorThrown) {

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


/*********************************************************************
 * OBTENDO LISTAS
 ********************************************************************/
//Matriz
// $("#matriz").prop("disabled", true);

// function getMatriz() {
// 	$.ajax({
// 		url: "includes/php/ajax.php"
// 		,type: "post"
// 		,dataType: "json"
// 		,data: {
// 			"module": "gera_csv",
// 			"action": "getMatriz",
// 			"id_grupo": $("#grupo").val()
// 		}
// 		,success: function (data) {

// 			console.log(data);

// 			$("#matriz").prop("disabled", false)
// 			$("#matriz").html(data.response_data.lista_matriz)

// 			$("#matriz").select2({
// 				placeholder: 'Nome, CNPJ, Código'
// 			})
// 		}
// 		,error: function (jqXHR, textStatus, errorThrown) {

// 			Swal.fire({
// 				position: 'top-end',
// 				icon: 'error',
// 				title: 'Não foi possível obter a lista de clientes matriz! Status: ' + textStatus,
// 				showConfirmButton: false,
// 				timer: 7000
// 				})
// 		}
// 	})
// }

// $("#grupo").on("change", () => {
// 	getMatriz();
// })

// //Unidade
// $("#unidade").prop("disabled", true)

// function getUnidade(id = null) {
// 	$.ajax({
// 		url: "includes/php/ajax.php"
// 		,type: "post"
// 		,dataType: "json"
// 		,data: {
// 			"module": "gera_csv",
// 			"action": "getUnidade",
// 			"id_matriz": (!id ? $("#matriz").val() : id)
// 		}
// 		,success: function (data) {

// 			console.log(data);

// 			$("#unidade").prop("disabled", false)
// 			$("#unidade").html(data.response_data.lista_unidade)
// 			// $("#unidade").trigger("chosen:updated");
// 			// $(".select2").select2()


// 		}
// 		,error: function (jqXHR, textStatus, errorThrown) {

// 			Swal.fire({
// 				position: 'top-end',
// 				icon: 'error',
// 				title: 'Não foi possível obter a lista de clientes unidade! Status: ' + textStatus,
// 				showConfirmButton: false,
// 				timer: 7000
// 				})
// 		}
// 	})
// }

// $("#matriz").on("change", () => {
// 	getUnidade()
// })

// //clausulas
// $("#clausulaList").prop("disabled", true)

// function getClausulas(id = null) {
// 	$.ajax({
// 		url: "includes/php/ajax.php"
// 		,type: "post"
// 		,dataType: "json"
// 		,data: {
// 			"module": "gera_csv",
// 			"action": "getClausulas",
// 			"id_grupo_clausula": (!id ? $("#grupo_clausulas").val() : id)
// 		}
// 		,success: function (data) {

// 			console.log(data);

// 			$("#clausulaList").prop("disabled", false)
// 			$("#clausulaList").html(data.response_data.lista_clausulas)


// 		}
// 		,error: function (jqXHR, textStatus, errorThrown) {

// 			Swal.fire({
// 				position: 'top-end',
// 				icon: 'error',
// 				title: 'Não foi possível obter a lista de clientes unidade! Status: ' + textStatus,
// 				showConfirmButton: false,
// 				timer: 7000
// 				})
// 		}
// 	})
// }
// $("#grupo_clausulas").on("change", () => {
// 	getClausulas()
// })

/*********************************************************************
 * MASCARA CNPJ
 ********************************************************************/

function maskCnpj(number) {
	if (typeof number !== "string") {
		return "";
	}
	var cleanedNumber = number.replace(/[^\d]/g, ''); // Remove any non-numeric characters
	var maskedNumber = cleanedNumber != "" ? cleanedNumber.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5') : ""; // Apply the mask
	return maskedNumber;
}


/*********************************************************************
 * GERAR TABELA
 ********************************************************************/

function gerarTabela(idClau, idGrupo) {

	var tabela;

	$.ajax(
		{
			url: "includes/php/ajax.php"
			, type: "post"
			, dataType: "json"
			, data: {
				"module": "gera_csv",
				"action": "gerarTabela",
				"data_aprovacao": $("#reservation").val(),
				"nome_doc": $("#nome_doc").val(),
				"categoria": $("#categoria").val(),
				"localidade": $("#localidade").val(),
				"sindicato_laboral": $("#sind_laboral").val(),
				"sindicato_patronal": $("#sind_patronal").val(),
				"data_base": $("#data_base").val(),
				"save_filter": $("#save").val(),
				"lista_clausula": idClau ? idClau : $("#clausulaList").val(),
				"grupo_clausula": idGrupo ? idGrupo : $("#grupo_clausulas").val(),
				//novos campos
				"grupo": $("#grupo").val(),
				"matriz": $("#matriz").val(),
				"unidade": $("#unidade").val(),
				"vigencia": $("#vigencia").val(),
			},beforeSend: function() {
				$('.img_box').css("display", "block");
			},success: function(data) {
				$('.img_box').css("display", "none");

				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'Tabela gerada com sucesso!',
					showConfirmButton: false,
					timer: 2000
				})

				$("#visualizar").css("display", "block")

				var obj = data.response_data.table;
				var title = data.response_data.title;

				console.log(title);

				console.log(obj)
				var dataSet = []

				for (let i = 0; i < obj.length; i++) {
					var lista = []
					lista.push(obj[i]['laboral'], maskCnpj(obj[i]['cnpj_sinde']), obj[i]['patronal'], maskCnpj(obj[i]['cnpj_sp']), obj[i]['localidade'], obj[i]['vigencia'], obj[i]['nome_doc'], obj[i]['categoria'], obj[i]['data_base'], obj[i]['validade_final'])
					for (let o = 0; o < title.length; o++) {

						lista.push(obj[i][title[o]])
						//nome_clausula
					}

					dataSet[i] = lista;
				}

				console.log(dataSet);
				var column = [];

				for (let t = 0; t < title.length; t++) {

					column[t] = { "title": title[t] }

				}
				
				
				
				
				
				column.unshift({ "title": "Validade Final" });
				column.unshift({ "title": "Data Base" });
				column.unshift({ "title": "Atividade Econômica" });
				column.unshift({ "title": "Nome Documento" });
				column.unshift({ "title": "Data de Processamento" });
				column.unshift({ "title": "Estado" });
				column.unshift({ "title": "CNPJ Patronal" });
				column.unshift({ "title": "Sind. Patronal" });
				column.unshift({ "title": "CNPJ Laboral" });
				column.unshift({ "title": "Sind. Laboral" });

				tabela = $('#example1').DataTable({
					scrollX: true,
					destroy: true,
					filter: false,
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
					columns: column,
					dom: 'Bfrtip',        // element order: NEEDS BUTTON CONTAINER (B) ****
					select: 'single',     // enable single row selection
					altEditor: true,
					buttons: ["csv", {
						extend: 'excelHtml5',
						exportOptions: {
							columns: ':visible'
						}
					},] //"copy", "pdf", "print"
				}).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');

				$(".dt-button").addClass("btn")
				$(".dt-button").addClass("btn-primary")
				$('#example1').DataTable().columns().every(function () {
					let datas = this.data();
					//alert(datas.join("\n"));
					let vazio = true;
					let arrd = datas.join("\n").split("\n");
					arrd.forEach(data => {
						if (data) {
							vazio = false;
						}
					});
					if (vazio) {
						//alert(this.data().join("\n"));
						this.visible(false);
					}
					// ... do something with data(), or this.nodes(), etc
				});

				$("#save-filter").prop("disabled", false)

			}
			, error: function (jqXHR, textStatus, errorThrown) {

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


/*********************************************************************
 * SALVAR FILTRO
 ********************************************************************/
function saveFilter() {
	$.ajax(
		{
			url: "includes/php/ajax.php"
			, type: "post"
			, dataType: "json"
			, data: {
				"module": "gera_csv",
				"action": "saveFilter",
				"data_aprovacao": $("#reservation").val(),
				"nome_doc": $("#nome_doc").val(),
				"categoria": $("#categoria").val(),
				"localidade": $("#localidade").val(),
				"sindicato_laboral": $("#sind_laboral").val(),
				"sindicato_patronal": $("#sind_patronal").val(),
				"data_base": $("#data_base").val(),
				"lista_clausula": $("#clausulaList").val(),
				"grupo_clausula": $("#grupo_clausulas").val(),
				"grupo": $("#grupo").val(),
				"matriz": $("#matriz").val(),
				"unidade": $("#unidade").val(),
				"vigencia": $("#vigencia").val()
			}
			, success: function (data) {

				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'Filtro salvo com sucesso!',
					showConfirmButton: false,
					timer: 2000
				})
			}
			, error: function (jqXHR, textStatus, errorThrown) {

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
