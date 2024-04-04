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
				
				
				redirectUri: "http://localhost:8080/perfil_estabelecimento.php", //a url da pagina que está implementando,
				
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
                  window.location.replace("http://localhost:8080/index.php");
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
	  "url": "http://localhost:8443/admin/realms/Ineditta-prod/users/"+key_user_id+"/logout",
	  "method": "POST",
	  "timeout": 0,
	  "headers": {
		"Authorization":"Bearer " + access_token,
	  },
	};
	
	$.ajax(settings).done(function (response) {
		console.log(response);
    	document.location.href = 'http://localhost:8080/exit.php'

	});
  
}



$("#sind_emp").prop("disabled", true)
$("#sind_patr").prop("disabled", true)

$("#sind_emp_cod").prop("disabled", true)
$("#sind_patr_cod").prop("disabled", true)
$(".chzn-select").chosen();
$(".select2").select2();


/*********************************************************
 * BUSCANDO LOCALIDADE
 *********************************************************/

 $("#localidade").on("change", function() {
	
	var localidade = '';
	if ($("#localidade").val() == "regiao") {
		localidade = 'regiao';
		$("#label-local").html("Região")
	}else if($("#localidade").val() == "estado") {
		localidade = 'uf';
		$("#label-local").html("Estado")
	}else if($("#localidade").val() == "municipio") {
		localidade = 'municipio';
		$("#label-local").html("Município")
	}else {
		localidade = null;
		$("#label-local").html("Selecione")
	}


	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "calendario",
				"action": "getLocalidade",
				"localidade": localidade
			}
			,success: function (data) {

				if (localidade != null) {
					$("#selecionar-local").html(data.response_data.localidade)
					
				}else {
					$("#selecionar-local").html("")
				}
				$(".chzn-select").chosen("destroy")
				$(".chzn-select").chosen()

			}
			,error: function (jqXHR, textStatus, errorThrown) {

				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi possível buscar a localidade!',
					showConfirmButton: false,
					timer: 2000
					})
			}
		}
	)
})


/*********************************************************
 * BUSCANDO SINDICATOS
 *********************************************************/

//SINDICATO LABORAL
//  $("#filter_sind_emp").on("change", function () {
// 	if ($("#filter_sind_emp").val() != "--") {

// 		var search = "";

// 		if($("#filter_sind_emp").val() == "codigo") {
// 			search = "codigo_sinde"
// 		}else if($("#filter_sind_emp").val() == "cnpj") {
// 			search = "cnpj_sinde"
// 		}else if($("#filter_sind_emp").val() == "sigla") {
// 			search = "sigla_sinde"
// 		}else {
// 			search = "denominacao_sinde"
// 		}

// 		//Buscando lista de sindicatos
// 		$.ajax({
// 			url: "includes/php/ajax.php"
// 			,type: "post"
// 			,dataType: "json"
// 			,data: {
// 				"module": "modulo_sindicato",
// 				"action": "getSindicatos",
// 				"busca": search,
// 				"sindicato": "laboral"
// 			}
// 			,success: function (data) {

// 				$("#result_emp").html(data.response_data.sindicatos)
// 				$("#result_emp").chosen("destroy")
// 				$("#result_emp").chosen()
				
// 			}
// 			,error: function (jqXHR, textStatus, errorThrown) {

// 				Swal.fire({
// 					position: 'top-end',
// 					icon: 'error',
// 					title: 'Não foi possível realizar o cadastro! Status: ' + textStatus,
// 					showConfirmButton: false,
// 					timer: 7000
// 					})
// 			}
// 		})

// 	}else {


// 		$("#result_emp").html("")
// 		$("#result_emp").chosen("destroy")
// 		$("#result_emp").chosen()
// 	}
// })

//SINDICATO PATRONAL
// $("#filter_sind_patr").on("change", function () {
// 	if ($("#filter_sind_patr").val() != "--") {

// 		var search = "";

// 		if($("#filter_sind_patr").val() == "codigo") {
// 			search = "codigo_sp"
// 		}else if($("#filter_sind_patr").val() == "cnpj") {
// 			search = "cnpj_sp"
// 		}else if($("#filter_sind_patr").val() == "sigla") {
// 			search = "sigla_sp"
// 		}else {
// 			search = "denominacao_sp"
// 		}

// 		//Buscando lista de sindicatos
// 		$.ajax({
// 			url: "includes/php/ajax.php"
// 			,type: "post"
// 			,dataType: "json"
// 			,data: {
// 				"module": "modulo_sindicato",
// 				"action": "getSindicatos",
// 				"busca": search,
// 				"sindicato": "patronal"
// 			}
// 			,success: function (data) {

// 				$("#result_patr").html(data.response_data.sindicatos)
// 				$("#result_patr").chosen("destroy")
// 				$("#result_patr").chosen()
				
// 			}
// 			,error: function (jqXHR, textStatus, errorThrown) {

// 				Swal.fire({
// 					position: 'top-end',
// 					icon: 'error',
// 					title: 'Não foi possível realizar o cadastro! Status: ' + textStatus,
// 					showConfirmButton: false,
// 					timer: 7000
// 					})
// 			}
// 		})

// 	}else {


// 		$("#result_patr").html("")
// 		$("#result_patr").chosen("destroy")
// 		$("#result_patr").chosen()
// 	}
// })

/*********************************************************************
 * BUSCANDO SINDICATOS COM COD_SINDCLIENTE
 ********************************************************************/

$("#codigo").on("change", () => {
	var cod = $("#codigo").val()

	$.ajax({
		url: "includes/php/ajax.php"
		,type: "post"
		,dataType: "json"
		,data: {
			"module": "modulo_sindicato",
			"action": "getSindicatosByCodigo",
			"busca": cod
		}
		,success: function (data) {

			// console.log(data.response_data)

			$("#sind_emp").prop("disabled", false)
			$("#sind_patr").prop("disabled", false)

			$("#sind_emp").html(data.response_data.optEmp)
			$("#sind_patr").html(data.response_data.optPatr)

			$("#sind_emp").chosen("destroy")
			$("#sind_emp").chosen()
			$("#sind_patr").chosen("destroy")
			$("#sind_patr").chosen()
		}
	})

	console.log(cod)
})


/*********************************************************************
 * BUSCANDO SINDICATOS COM CODIGO UNIDADE
 ********************************************************************/

 $("#cod_unidade").on("change", () => {
	var cod = $("#cod_unidade").val()

	$.ajax({
		url: "includes/php/ajax.php"
		,type: "post"
		,dataType: "json"
		,data: {
			"module": "modulo_sindicato",
			"action": "getSindicatosByCodUnidade",
			"busca": cod
		}
		,success: function (data) {

			// console.log(data.response_data)

			$("#sind_emp_cod").prop("disabled", false)
			$("#sind_patr_cod").prop("disabled", false)

			$("#sind_emp_cod").html(data.response_data.optEmpCod)
			$("#sind_patr_cod").html(data.response_data.optPatrCod)

			$("#sind_emp_cod").chosen("destroy")
			$("#sind_emp_cod").chosen()
			$("#sind_patr_cod").chosen("destroy")
			$("#sind_patr_cod").chosen()
		}
	})

	console.log(cod)
})


/*********************************************************************
 * GERAR TABELA
 ********************************************************************/

function gerarTabela(){

	var tabela;

	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "gera_csv",
				"action": "gerarTabela",
				"vigencia": $("#reservation").val(),			
				"nome_doc": $("#nome_doc").val(),
				"categoria": $("#categoria").val(),
				"localidade": $("#localidade").val(),
				"sindicato": $("#sindicato_tipo").val(),
				"sindicato_selecao": $("#sindicato-selecao").val(),
				"data_base": $("#data-base").val(),
				"save_filter": $("#save").val(),
				"lista_clausula": $("#clausulaList").val(),
				"grupo_clausula": $("#grupo_clausulas").val()
			}
			,beforeSend: function(xhr){
				// $("#mensagem_sucesso").hide();
			}
			,complete: function( xhr, textStatus ) {
				// $("#preload").hide();
			}
			,success: function (data) {

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

				console.log(obj)
				var dataSet = []

				for (let i = 0; i < obj.length; i++) {
					var lista = []
					lista.push(obj[i]['nome_clausula'], obj[i]['vigencia'], obj[i]['patronal'], obj[i]['laboral'], obj[i]['nome_doc'], obj[i]['categoria'], obj[i]['localidade'], obj[i]['data_base'])
					for (let o = 0; o < title.length; o++) {
						
						lista.push(obj[i][title[o]])
						
					}

					dataSet[i] = lista;
					
				}

				console.log(dataSet);
				var column = [];
				
				for (let t = 0; t < title.length; t++) {
					
					column[t] = {"title": title[t] }
					
				}
				column.unshift({"title": "Data Base"});
				column.unshift({"title": "Localidade"});
				column.unshift({"title": "Categoria"});
				column.unshift({"title": "Nome Documento"});
				column.unshift({"title": "Sind. Laboral"});
				column.unshift({"title": "Sind. Patronal"});
				column.unshift({"title": "Data de Aprovação"});
				// column.unshift({"title": "ID Documento"});
				column.unshift({"title": "Nome Cláusula"});
				// column.unshift({"title": "ID Clausula Geral"});
				
				// console.log(column)
				var tit = 'data_de_reajuste_salarial'

				tabela = $('#example1').DataTable({
					scrollX: true,
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
					columns: column,
					dom: 'Bfrtip',        // element order: NEEDS BUTTON CONTAINER (B) ****
					select: 'single',     // enable single row selection
					altEditor: true,
					buttons: ["csv", "excel"] //"copy", "pdf", "print"
				}).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');

				$(".dt-button").addClass("btn")
				$(".dt-button").addClass("btn-primary")

				$("#save-filter").prop("disabled", false)

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


/*********************************************************************
 * SALVAR FILTRO
 ********************************************************************/

function saveFilter() {
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "gera_csv",
				"action": "saveFilter",
				"vigencia": $("#reservation").val(),			
				"nome_doc": $("#nome_doc").val(),
				"categoria": $("#categoria").val(),
				"localidade": $("#localidade").val(),
				"sindicato": $("#sindicato_tipo").val(),
				"sindicato_selecao": $("#sindicato-selecao").val(),
				"data_base": $("#data-base").val(),
				"lista_clausula": $("#clausulaList").val(),
				"grupo_clausula": $("#grupo_clausulas").val()
			}
			,success: function (data) {

				Swal.fire({
				position: 'top-end',
				icon: 'success',
				title: 'Filtro salvo com sucesso!',
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


/*********************************************************************
 * INICIALIZANDO GRÁFICOS
 ********************************************************************/
 var chartSind;

 var ctxSind = document.getElementById('sindChart');
 if(ctxSind) {
	 ctxSind.getContext('2d');
	 chartSind = new Chart(ctxSind, {
		 type: 'bar',
		 data: {
			 labels: [],
			 datasets: [{
				 label: 'Abertas',
				 data: [],
				 backgroundColor: [
					 'rgba(54, 162, 235, 0.2)'
				 
				 ],
				 borderColor: [
					 'rgba(54, 162, 235, 1)'
				 ],
				 borderWidth: 1
			 }]
		 },
		 options: {
			 scales: {
				 y: {
					 beginAtZero: true
				 }
			 },
			 plugins: {
				 title: {
					 display: true,
					 text: "Negociações abertas por estado"
				 }
			 }
		 }
	 });
 }

var pieChart;
var ctxPieQtd = document.getElementById('qtdChart');

if (ctxPieQtd) {
    ctxPieQtd.getContext('2d');

	pieChart = new Chart(ctxPieQtd, {
		type: 'pie',
		data: {
			labels: [
						'null'
					],//'CENTRAL DOS TRABALHADORES E TRABALHADORAS DO BRASIL', 'CENTRAL UNICA DOS TRABALHADORES - CUT', 'FORÇA SINDICAL', 'NOVA CENTRAL SINDICAL DE TRABALHADORES - NCST','CENTRAL SINDICAL E POPULAR CONLUTAS', 'CENTRAL GERAL DOS TRABALHADORES DO BRASIL - CGTB', 'Não há declaração de filiação', 'UNIÃO GERAL DOS TRABALHADORES - UGT'
			datasets: [
				{
				label: 'Dataset 1',
				data: [1],//7,15,19,15, 1, 1, 3, 28
				backgroundColor: [
					'rgba(255, 159, 64, 0.7)',
					'rgba(54, 162, 235, 0.7)',
					'rgba(255, 99, 13, 0.7)',
					'rgba(255,99,132, 0.7)',
					'rgba(201,203,207, 0.7)',
					'rgba(153,102,255, 0.7)',
					'rgba(80,102,180, 0.7)',
					'rgba(255,205,86, 0.7)'
				]
				}
			]
		},
		options: {
			plugins: {
				// title: {
				//     display: true,
				//     text: 'Feriado 15/11 - Proclamação da República'
				// }
				legend: {
					position: 'right'
				}
			}
		}
    });
}


/*********************************************************************
 * APLICANDO FILTROS
 ********************************************************************/
 
function filter() {
	chartSind.destroy();
	pieChart.destroy();
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "modulo_sindicato",
				"action": "setFilter",
				"localidade": $("#localidade_listas").val(),
				"categoria": $("#categoria").val(),
				"data_base": $("#data_base").val(),
				"codigo": $("#codigo").val(),
				"sind_emp": $("#result_emp").val(),
				"sind_patr": $("#result_patr").val()
			}
			,success: function (data) {

				$("#qtd_emp").html(data.response_data.qtdEmp)
				$("#qtd_patr").html(data.response_data.qtdPatr)
				$("#mand_vencido").html(data.response_data.mandVencido)
				$("#mand_vigente").html(data.response_data.mandVigente)

				$("#neg_vencida").html(data.response_data.negVencida)
				$("#neg_vigente").html(data.response_data.negVigente)


				//Gráfico barras
				var dataSet = data.response_data.listaGrafico
				

				var uf = [];
				var uf_qtd = [];

				for (let i = 0; i < dataSet.length; i++) {
					uf.push(dataSet[i].uf)
					uf_qtd.push(dataSet[i].qtd)
					
				}

				console.log(uf)
				console.log(uf_qtd)

				var ctxSind = document.getElementById('sindChart');
				if(ctxSind) {
					ctxSind.getContext('2d');
					chartSind = new Chart(ctxSind, {
						type: 'bar',
						data: {
							labels: uf,
							datasets: [{
								label: 'Em aberto',
								data: uf_qtd,
								backgroundColor: [
									'rgba(54, 162, 235, 0.2)'
								
								],
								borderColor: [
									'rgba(54, 162, 235, 1)'
								],
								borderWidth: 1
							}]
						},
						options: {
							scales: {
								y: {
									beginAtZero: true
								}
							},
							plugins: {
								title: {
									display: true,
									text: "Negociações em aberto por estado"
								}
							}
						}
					});
				}

				//TABELA ORGANIZAÇÃO SINDICAL
				$("#organizacao").html(data.response_data.tabelaOrganizacao)

				$('#grid-layout-table-organizacao').jplist({

					itemsBox: '.demo-tbl tbody'
					,itemPath: '.tbl-item'
					,panelPath: '.jplist-panel'
			
					,redrawCallback: function(){
						$('.tbl-item').each(function(index, el){
							if(index%2 === 0){
								$(el).addClass('even');
							}
							else{
								$(el).addClass('odd');
							}
						});
					}
				});

				//GRÁFICO DE PIZZA QUANTIDADE DED CENTRAIS
				console.log(data.response_data.pizza)

				var dataSetPizza = data.response_data.pizza
				

				var central = [];
				var qtd_centrais = [];

				for (let i = 0; i < dataSetPizza.length; i++) {
					if (dataSetPizza[i].central == "" || dataSetPizza[i].central == null) {
						var centralName = "Não há declaração de filiação";
					}else {
						var centralName = dataSetPizza[i].central
					}
					central.push(centralName)
					qtd_centrais.push(dataSetPizza[i].qtd)
					
				}

				var ctxPieQtd = document.getElementById('qtdChart');

				if (ctxPieQtd) {
					ctxPieQtd.getContext('2d');

					pieChart = new Chart(ctxPieQtd, {
						type: 'pie',
						data: {
							labels: 
										central
									,//'CENTRAL DOS TRABALHADORES E TRABALHADORAS DO BRASIL', 'CENTRAL UNICA DOS TRABALHADORES - CUT', 'FORÇA SINDICAL', 'NOVA CENTRAL SINDICAL DE TRABALHADORES - NCST','CENTRAL SINDICAL E POPULAR CONLUTAS', 'CENTRAL GERAL DOS TRABALHADORES DO BRASIL - CGTB', 'Não há declaração de filiação', 'UNIÃO GERAL DOS TRABALHADORES - UGT'
							datasets: [
								{
								label: 'Dataset 1',
								data: qtd_centrais,//7,15,19,15, 1, 1, 3, 28
								backgroundColor: [
									'rgba(255, 159, 64, 0.7)',
									'rgba(54, 162, 235, 0.7)',
									'rgba(255, 99, 13, 0.7)',
									'rgba(255,99,132, 0.7)',
									'rgba(201,203,207, 0.7)',
									'rgba(153,102,255, 0.7)',
									'rgba(80,102,180, 0.7)',
									'rgba(255,205,86, 0.7)'
								]
								}
							]
						},
						options: {
							plugins: {
								// title: {
								//     display: true,
								//     text: 'Feriado 15/11 - Proclamação da República'
								// }
								legend: {
									position: 'right'
								}
							}
						}
					});
				}

				//TABELA DIRIGENTE SINDICAL
				$("#dirigentes").html(data.response_data.dirigentes)

				$('#grid-layout-table-dirigentes').jplist({

					itemsBox: '.demo-tbl tbody'
					,itemPath: '.tbl-item'
					,panelPath: '.jplist-panel'
			
					//save plugin state
					// ,storage: 'localstorage' //'', 'cookies', 'localstorage'
					// ,storageName: 'jplist-table-4'
			
					,redrawCallback: function(){
						$('.tbl-item').each(function(index, el){
							if(index%2 === 0){
								$(el).addClass('even');
							}
							else{
								$(el).addClass('odd');
							}
						});
					}
				});
				


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

//FILTRO

setTimeout(function() {
	//filtrar();
  }, 200)

function whereFromFiltro(unidades, matrizes, grupo) {
	let query = "";
	if (unidades) {
		query += " AND cu.id_unidade IN (" + unidades + ") ";
	}

	if (matrizes) {
		query += " AND cm.id_empresa IN (" + matrizes + ") ";
	}
	if (grupo) {
		if(grupo + "" == "0"){
			query += " AND grupo.id_grupo_economico = grupo.id_grupo_economico ";
		} else {
			query += " AND grupo.id_grupo_economico IN (" + grupo + ") ";
		}
		
	}

	return query + "";
}





function sendFilter(){
	sessionStorage.setItem("common_filter", whereFromFiltro(filiais, $('#matriz').val(), $('#grupo').val()));
	window.location.href = "http://ineditta.com/desenvolvimento/modulo_sindicatos.php?filter=1";
}






function filtrar(filiais) {
	$.ajax({
		url: "includes/php/ajax.php"
		, type: "post"
		, dataType: "json"
		, data: {
			"module": "perfil_comercio",
			"action": "getTable",
			gec: sessionStorage.getItem('grupoecon'),
			filtro: whereFromFiltro(filiais, $('#matriz').val(), $('#grupo').val())
		}, beforeSend: function () {
			$('.img_box').css("display", "block");

		}, success: function (data) {

			$('.img_box').css("display", "none");

			alert(JSON.stringify(data));

			$("#tabf").html(data.response_data.tabf);

			$('#grid-layout-table-tabf').jplist({

				itemsBox: '.demo-tbl tbody'
				, itemPath: '.tbl-item'
				, panelPath: '.jplist-panel'

				, redrawCallback: function () {
					$('.tbl-item').each(function (index, el) {
						if (index % 2 === 0) {
							$(el).addClass('even');
						}
						else {
							$(el).addClass('odd');
						}
					});
				}
			});
			//alert("1");
			// $("#unidade").trigger("chosen:updated");
			// $(".select2").select2()


		}
		, error: function (jqXHR, textStatus, errorThrown) {

			Swal.fire({
				position: 'top-end',
				icon: 'error',
				title: 'Não foi possível obter a lista de unidades! Status: ' + textStatus,
				showConfirmButton: false,
				timer: 7000
			})
		}
	})
}


