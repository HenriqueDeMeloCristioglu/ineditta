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
				
				
				redirectUri: "http://localhost:8080/calendario_sindical.php", //a url da pagina que está implementando,
				
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


/*********************************************************************
 * INICIANDO CALENDÁRIO
 ********************************************************************/
//Date range picker
$('#reservation').daterangepicker({
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
	}
})

$(".chzn-select").chosen()

/*********************************************************************
 * TRAZENDO DADOS PARA O CALENDÁRIO
 ********************************************************************/


$(function () {
	
	// var calendarEl = document.getElementById('calendar');
	// var calendar = new FullCalendar.Calendar(calendarEl, {
	// 	themeSystem: 'bootstrap',
	// 	initialView: 'dayGridMonth',
	// 	locale: 'pt-br',
	// 	headerToolbar: {
	// 		start: 'today prev,next',
	// 		center: 'title',
	// 		right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
	// 	},
	// 	buttonText: {
	// 		today:    'hoje',
	// 		month:    'mês',
	// 		week:     'semana',
	// 		day:      'dia',
	// 		list:     'lista'
	// 	},
	// 	selectable: false,
	// });

	// calendar.render();

	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "calendario",
				"action": "getCalendario"
			}
			,success: function (data) {

				var eventos = data.response_data.eventos

				console.log(eventos)

				var calendarEl = document.getElementById('calendar');
				var calendar = new FullCalendar.Calendar(calendarEl, {
					themeSystem: 'bootstrap',
					initialView: 'dayGridMonth',
					locale: 'pt-br',
					headerToolbar: {
						start: 'today prev,next',
						center: 'title',
						right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
					},
					buttonText: {
						today:    'hoje',
						month:    'mês',
						week:     'semana',
						day:      'dia',
						list:     'lista'
					},
					selectable: false,
					events: 
						eventos
					,
					dateClick: function(info) {
						// alert('Clicked on: ' + info.date);
						// alert('Coordinates: ' + info.jsEvent.pageX + ',' + info.jsEvent.pageY);
						// alert('Current view: ' + info.view.type);

						// var modal = document.getElementById("modalNovoEvento")
						// setTimeout(() => {
						// 	modal.className = 'modal fade in'
						// }, 300);
					
						// modal.style.display = 'block';
						// modal.setAttribute("aria-hidden", "false");
						// modal.setAttribute("aria-label", "true");

						$("#modalNovoEvento").modal("show")
						$("#modalNovoEvento").css("overflow-y", "auto")
						$("#modalNovoEvento").addClass("in")

						
						var startDate = dateFormat(info.date)
						$("#newEventStart").val(startDate['date'])

						$("#newAllDay").on("click", function () {
							if ($("#newAllDay").prop("checked") == true) {
							
							// $("#allDay").prop("checked", true)
							$("#newTimeStart").prop("disabled", true)
							$("#newTimeEnd").prop("disabled", true)
							$("#newEventEnd").prop("disabled", true)

							$("#newTimeStart").val("")
							$("#newTimeEnd").val("")
							$("#newEventEnd").val(dateFormat(info.date)['date'])


							}else {
								$("#newTimeStart").prop("disabled", false)
								$("#newTimeEnd").prop("disabled", false)

								$("#newEventEnd").prop("disabled", false)
								$("#newEventEnd").val("")
							}
						})

						


					},
					eventClick: function(info) {
						console.log(info);
						// window.location.href='#modalEmpregados'
						// var modal = document.getElementById("modalEvento")
						// setTimeout(() => {
						// 	modal.className = 'modal fade in'
						// }, 300);
					
						// modal.style.display = 'block';
						// modal.setAttribute("aria-hidden", "false");
						// modal.setAttribute("aria-label", "true");

						
						// var endDate = dateFormat(info.event.endStr)

						// console.log(info.event.allDay)

						// if (info.event.allDay == true) {
						// 	var btnMenu = document.querySelector("#btn-color")
						// 	btnMenu.style.backgroundColor = info.event.backgroundColor

						// 	$("#allDay").prop("checked", true)
						// 	$("#timeStart").prop("disabled", true)
						// 	$("#timeEnd").prop("disabled", true)


						// }else {
						// 	var btnMenu = document.querySelector("#btn-color")
						// 	btnMenu.style.backgroundColor = info.event.backgroundColor

						// 	$("#timeStart").val(startDate['time']);
						// 	$("#timeEnd").val(endDate['time']);
						// }
							// $("#eventStart").val(startDate['date']);
							// $("#eventEnd").val(endDate['date']);

							/**
							 * DOCSIND EVENT
							 */
							if (info.event.classNames[0] == "docsind") {

								$("#modalEvento").modal("show")
								$("#modalEvento").css("overflow-y", "auto")
								$("#modalEvento").addClass("in")
								$("#eventTitle").val(info.event.title);
								var startDate = dateFormat(info.event.start)

								//BUSCANDO DEMAIS DADOS DO DOCSIND PELO ID
								$.ajax({
									url: "includes/php/ajax.php"
									,type: "post"
									,dataType: "json"
									,data: {
										"module": "calendario",
										"action": "getById",
										"id_evento": info.event.id
									}
									,success: function (data) {
						
										console.log(data)

										$("#nome_doc_event").val(data.response_data.nome_doc)
										$("#eventStart").val(data.response_data.periodo)
										$("#cnae").val(data.response_data.cnae)
										$("#abrang").val(data.response_data.abrangencia)
										$("#laboral").val(data.response_data.laboral)
										$("#patronal").val(data.response_data.patronal)
						
									}
									,error: function (jqXHR, textStatus, errorThrown) {
						
										Swal.fire({
											position: 'top-end',
											icon: 'error',
											title: 'Não foi possível obter os dados do evento! Erro: '+ errorThrown,
											showConfirmButton: false,
											timer: 2000
											})
									}
								})
							}


							/**
							 * DIRPATRO EVENT
							 */
							if (info.event.classNames[0] == "dirpatro") {

								$("#modalEventoDirpatro").modal("show")
								$("#modalEventoDirpatro").css("overflow-y", "auto")
								$("#modalEventoDirpatro").addClass("in")
								$("#title_dir_patro").val(info.event.title);
								var startDate = dateFormat(info.event.start)

								//BUSCANDO DEMAIS DADOS DO DOCSIND PELO ID
								$.ajax({
									url: "includes/php/ajax.php"
									,type: "post"
									,dataType: "json"
									,data: {
										"module": "calendario",
										"action": "getById",
										"id_evento": info.event.id
									}
									,success: function (data) {
						
										console.log(data)

										$("#nome_sujeito").val(data.response_data.nome)
										$("#period_dir_patr").val(data.response_data.periodo)
										$("#role").val(data.response_data.role)
										$("#emprea_dir_patr").val(data.response_data.empresa)
										$("#nome_patronal").val(data.response_data.sindicato)
						
									}
									,error: function (jqXHR, textStatus, errorThrown) {
						
										Swal.fire({
											position: 'top-end',
											icon: 'error',
											title: 'Não foi possível obter os dados do evento! Erro: '+ errorThrown,
											showConfirmButton: false,
											timer: 2000
											})
									}
								})
							}


							/**
							 * DIRPATRO EVENT
							 */
							if (info.event.classNames[0] == "diremp") {

								$("#modalEventoDiremp").modal("show")
								$("#modalEventoDiremp").css("overflow-y", "auto")
								$("#modalEventoDiremp").addClass("in")
								$("#title_dir_emp").val(info.event.title);
								var startDate = dateFormat(info.event.start)

								//BUSCANDO DEMAIS DADOS DO DOCSIND PELO ID
								$.ajax({
									url: "includes/php/ajax.php"
									,type: "post"
									,dataType: "json"
									,data: {
										"module": "calendario",
										"action": "getById",
										"id_evento": info.event.id
									}
									,success: function (data) {
						
										console.log(data)

										$("#nome_sujeito_emp").val(data.response_data.nome)
										$("#period_dir_emp").val(data.response_data.periodo)
										$("#role_emp").val(data.response_data.role)
										$("#emprea_dir_emp").val(data.response_data.empresa)
										$("#nome_laboral").val(data.response_data.sindicato)
						
									}
									,error: function (jqXHR, textStatus, errorThrown) {
						
										Swal.fire({
											position: 'top-end',
											icon: 'error',
											title: 'Não foi possível obter os dados do evento! Erro: '+ errorThrown,
											showConfirmButton: false,
											timer: 2000
											})
									}
								})
							}

							
							
						}


						

				});

				calendar.render();
			}
			,error: function (jqXHR, textStatus, errorThrown) {

				Swal.fire({
					position: 'top-end',
					icon: 'error',
					title: 'Não foi possível obter os dados do calendário! Erro: ' + errorThrown,
					showConfirmButton: false,
					timer: 2000
					})
			}
		}
	)

})


function dateFormat(date) {
	let data = new Date(date);
	//verificar getMonth retornando mês anterior
	let monthProv = data.getMonth() + 1
	let day = (data.getDate() < 10 ? "0" + data.getDate() : data.getDate());
	let month = (monthProv < 10 ? "0" + monthProv : monthProv);
	let time = (data.getHours() < 10 ? "0" + data.getHours() : data.getHours()) + ":" + (data.getMinutes() < 10 ? "0" + data.getMinutes() : data.getMinutes() );

	let newDate = data.getFullYear() + "-" + month + "-" + day;

	let groupDate = [];
	groupDate['date'] = newDate
	groupDate['time'] = time

	return groupDate;
	
}

function openColors() {
	var menu = document.querySelector(".input-group-btn")

	if (menu.classList.contains("open")) {
		menu.className = "input-group-btn"
		
	}else {
		menu.className = "input-group-btn open"
	}
}

var btnColor = document.querySelectorAll(".btn-theme")

btnColor.forEach(element => {
	element.addEventListener("click", () => {
		var color = "#" + element.getAttribute("id")
		// console.log(color)
		var btnMenu = document.querySelector("#btn-color")
		btnMenu.style.backgroundColor = color

		var btnMenuAdd = document.querySelector("#new-btn-color")
		if (btnMenuAdd) {
			btnMenuAdd.style.backgroundColor = color
		}

		$("#color").val(color)
	})
});

/*********************************************************************
 * TRATANDO FILTROS
 ********************************************************************/

$("#localidade").on("change", function() {
	
	var localidade = '';
	if ($("#localidade").val() == "regiao") {
		localidade = 'regiao';
	}else if($("#localidade").val() == "estado") {
		localidade = 'uf';
	}else if($("#localidade").val() == "municipio") {
		localidade = 'municipio';
	}else {
		localidade = null;
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
			,beforeSend: function(xhr){
				// $("#mensagem_sucesso").hide();
			}
			,complete: function( xhr, textStatus ) {
				// $("#preload").hide();
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
					title: 'Não foi possível realizar o cadastro!',
					showConfirmButton: false,
					timer: 2000
					})
			}
		}
	)


})



/*********************************************************************
 * ADICIONANDO EVENTO
 ********************************************************************/

function addEvento(){

	var allday = null;
	if ($("#newAllDay").prop("checked") == true) {
		allday = 'true';
	}else {
		allday = 'false'
	}

	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "calendario",
				"action": "addEvento",
				"titulo": $("#newEventTitle").val(),
				"inicio": $("#newEventStart").val(),
				"termino": $("#newEventEnd").val(),
				"time-Start": $("#newTimeStart").val(),
				"time-End": $("#newTimeEnd").val(),
				"allDay": allday,
				"cor": $("#color").val(),
				"assunto": $("#newSubject").val()
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
					title: 'Não foi possível realizar o cadastro! Erro: ' + errorThrown + ' / Status: ' + textStatus,
					showConfirmButton: false,
					timer: 4000
				})
			}
		}
	)
}


/*********************************************************************
 * UPDATE EVENTO
 ********************************************************************/

 function getById(id){

	console.log(id)

	var tabelaInfoAdic;

	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "clausula",
				"action": "getByIdClausula",
				"id_clau": id
			}
			,beforeSend: function(xhr){
				//$('#preload').show();
			}
			,complete: function( xhr, textStatus ) {
				//$("#preload").hide();
			}
			,success: function (data) {
				


			}
			,error: function (jqXHR, textStatus, errorThrown) {
				window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
				console.log(jqXHR)
				
			}
		}
	)
}

function updateCalendario(id){
	
	$.ajax(
		{
			 url: "includes/php/ajax.php"
			,type: "post"
			,dataType: "json"
			,data: {
				"module": "calendario",
				"action": "updateCalendario",

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
					timer: 3000
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
			}
		}
	)
}


function closeModal() {
	// document.location.reload(true);
	$("#modalNovoEvento").modal("hide")
	$("#modalEvento").modal("hide")
	$("#modalEventoDirpatro").modal("hide")
	$("#modalEventoDiremp").modal("hide")
}
