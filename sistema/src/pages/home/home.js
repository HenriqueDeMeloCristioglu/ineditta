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
				
				
				redirectUri: "http://localhost:8000/clienteunidade.php", //a url da pagina que está implementando,
				
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

	  document.location.reload(true);
	});
  
}

$(document).ready(function(){
	$.fn.datepicker.dates['pt-BR'] = {
		format: 'dd/mm/yyyy',
		days: ["Domingo", "Segunda", "Terรงa", "Quarta", "Quinta", "Sexta", "Sรกbado", "Domingo"],
		daysShort: ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sรกb", "Dom"],
		daysMin: ["Do", "Se", "Te", "Qu", "Qu", "Se", "Sa", "Do"],
		months: ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"],
		monthsShort: ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
		today: "Hoje",
		suffix: [],
		meridiem: []
			};
	$(".datepicker").datepicker({
	autoclose: true,
	todayHighlight: true,
	toggleActive: true,
	format: 'dd/mm/yyyy',
	language: 'pt-BR'
	}).on('changeDate', function(selected){
	updateDate($(this).closest('form').find('.datepicker'), selected);
	});

	$("#cnpj-input").mask('00.000.000/0000-00');
	$("#cnpj-inputu").mask('00.000.000/0000-00');

	$("#sind-input").mask('00000000');
	$("#sind-inputu").mask('00000000');

	$("#cep-input").mask('00000-000');
	$("#cep-inputu").mask('00000-000');

  });

  function updateDate(inputs, selected){
	var minDate = new Date(selected.date.valueOf());
	$(inputs[1]).datepicker('setStartDate', minDate);
	$(inputs[0]).datepicker('setEndDate', minDate);
 }

$("#btn_novo").on("click", () => {
	setTimeout(() => {
		$(".select2").select2()
	}, 200)
})

function addClienteUnidade(){
				
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "clienteunidade", 
				"action": "addClienteUnidade",
				"codigo": $("#cod-input").val(),
				"cnpj": $("#cnpj-input").val(),
				"nome": $("#nome-input").val(),
				"endereco": $("#end-input").val(),
				"regional": $("#reg-input").val(),
				"bairro": $("#bairro-input").val(),
				"cep": $("#cep-input").val(),
				"data_inativa": $("#dataina-input").val(),
				"data_inclu": $("#dataclu-input").val(),
				"tipo_neg": $("#tn-input").val(),
				"localizacao": $("#loc-input").val(),
				"matriz": $("#em-input").val(),
				"cod_sind_cliente": $("#csc-input").val(),
				"cod_sind_patronal": $("#csp-input").val(),
				"cnaes-input": $("#cnaes-input").val()
				
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
					title: 'Não foi posssível realizar o cadastro' + textStatus,
					showConfirmButton: false,
					timer: 2000
				})
			}
		}
	)
}

function updateClienteUnidade(id_unidade){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "clienteunidade", 
				"action": "updateClienteUnidade",
				"cod-input": $("#cod-inputu").val(),
				"cnpj-input": $("#cnpj-inputu").val(),
				"nome-input": $("#nome-inputu").val(),
				"end-input": $("#end-inputu").val(),
				"reg-input": $("#reg-inputu").val(),
				"bairro-input": $("#bairro-inputu").val(),
				"cep-input": $("#cep-inputu").val(),
				"dataina-input": $("#dataina-inputu").val(),
				"csc-input": $("#csc-inputu").val(),
				"csp-input": $("#csp-inputu").val(),
				"dataclu-input": $("#dataclu-inputu").val(),
				"tn-input": $("#tn-inputu").val(),
				"loc-input": $("#loc-inputu").val(),
				"em-input": $("#em-inputu").val(),
				"id_unidade": id_unidade
				
			}
			,beforeSend: function(xhr){
				//$("#mensagem_sucessou").hide();
			}
			,complete: function( xhr, textStatus ) {
				$("#preload").hide();
			}
			,success: function (data) {

				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'Atualização realizada com sucesso!',
					showConfirmButton: false,
					timer: 2000
				})

				//$("#ia-inputu").val('');
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi posssível realizar a atualização' + textStatus,
					showConfirmButton: false,
					timer: 2000
				})

			}
		}
	)
}

$('#btn-atualizar').on('click', function() {
  var id = $('#id-inputu').val();
  updateClienteUnidade(id);
});

function getByIdClienteUnidade(id_unidade){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "clienteunidade", 
				"action": "getByIdClienteUnidade",
				"id_unidade": id_unidade
			}
			,success: function (data) {
				
				$("#id-inputu").val( data.response_data.id_unidade );
				$("#cod-inputu").val( data.response_data.codigo_unidade);
				$("#cnpj-inputu").val( data.response_data.cnpj_unidade);
				$("#nome-inputu").val( data.response_data.nome_unidade);
				$("#end-inputu").val( data.response_data.endereco);
			    $("#reg-inputu").val( data.response_data.regional);
				$("#bairro-inputu").val( data.response_data.bairro);
				$("#cep-inputu").val( data.response_data.cep);
				$("#dataina-inputu").val( data.response_data.data_inativo);
				$("#csc-inputu").val(data.response_data.cod_sindcliente);
				$("#csp-inputu").val(data.response_data.cod_sindpatrocliente);
				$("#dataclu-inputu").val(data.response_data.data_inclusao);
				$("#tn-inputu").val(data.response_data.tipounidade_cliente_id_tiponegocio);
				$("#loc-inputu").val(data.response_data.localizacao_id_localizacao);
				$("#em-inputu").val(data.response_data.cliente_matriz_id_empresa);

				$("#cnae_selecionado").html(data.response_data.listaCnaeSelecionado)

				//GERANDO TABELA CNAE UPDATE
				obj = [data.response_data.tabelaCnae];
				console.log(data)

				var dataSet = [];

				for (let i = 0; i < obj[0].length; i++) {
					dataSet[i] = [obj[0][i].td, obj[0][i].id_cnae, obj[0][i].divisao_cnae, obj[0][i].descricao_divisão, obj[0][i].subclasse_cnae, obj[0][i].descricao_subclasse, obj[0][i].categoria];
				}

				tabelaClass = $('#tabela-cnae').DataTable({
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
						{ title: "CNAE" },
						{ title: "Descrição Divisão" },
						{ title: "Subclasse" },
						{ title: "Descrição Subclasse" },
						{ title: "Categoria" }
					]
				});
				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi posssível obter os dados do registro! Status: ' + textStatus,
					showConfirmButton: false,
					timer: 2000
				})
			}
		}
	)
}

function addCNAE(id_cnaes){
	check = null;
	if (document.getElementById('inicheck'+id_cnaes+'').checked) 
	{
		$("#cnaes-input").val( $("#cnaes-input").val() + " " + id_cnaes );
		
		
	} else 
	{
		$("#cnaes-input").val( ($("#cnaes-input").val()+"").replace(' '+id_cnaes,'') );
	}	
	
	var campo = $("#cnaes-input").val()

	if (!campo) {
		$("#select_all").prop("disabled", false)
	}else {
		$("#select_all").prop("disabled", true)
	}

	console.log($("#cnaes-input").val());
	
}

function selecionarTodos() {
	var check = document.querySelectorAll(".form-check-input")
	console.log(check)

	$(check).prop("checked", true)

	var listaId = []
	check.forEach(element => {
		var id = element.getAttribute("data-id")
		
		listaId.push(id)
	});

	//console.log(listaId)
	$("#cnaes-input").val(listaId)

	console.log($("#cnaes-input").val())
}

function limparSelecao() {
	var check = document.querySelectorAll(".form-check-input")
	console.log(check)

	$(check).prop("checked", false)

	
	$("#cnaes-input").val('')

	console.log($("#cnaes-input").val())

	$("#select_all").prop("disabled", false)
}

function saveCNAEChange(id_unidade, id_cnaes){
	check = null;
	if (document.getElementById('check'+id_cnaes+'').checked) 
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
				"module": "clienteunidade", 
				"action": "saveCNAEChange",
				"id_unidade": id_unidade,
				"id_cnaes": id_cnaes,
				"check": check
				
			}
			,success: function (data) {

				Swal.fire({
					position: 'top-end',
					icon: 'success',
					title: 'CNAE atualizado com sucesso ',
					showConfirmButton: false,
					timer: 2000
				})

				
			}
			,error: function (jqXHR, textStatus, errorThrown) {
				
				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Falha ao atualizar o CNAE! Status: ' +textStatus,
					showConfirmButton: false,
					timer: 2000
				})
			}
		}
	)
}


$('.btn-cancelar').click(function(){

	document.location.reload(true);
	
});
