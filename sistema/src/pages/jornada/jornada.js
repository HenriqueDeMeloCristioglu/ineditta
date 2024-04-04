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
				
				
				redirectUri: "http://localhost:8000/jornada.php", //a url da pagina que está implementando,
				
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

$(document).ready(function () {
  $.fn.datepicker.dates["pt-BR"] = {
    format: "dd/mm/yyyy",
    days: [
      "Domingo",
      "Segunda",
      "Terรงa",
      "Quarta",
      "Quinta",
      "Sexta",
      "Sรกbado",
      "Domingo",
    ],
    daysShort: ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sรกb", "Dom"],
    daysMin: ["Do", "Se", "Te", "Qu", "Qu", "Se", "Sa", "Do"],
    months: [
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
      "Dezembro",
    ],
    monthsShort: [
      "Jan",
      "Fev",
      "Mar",
      "Abr",
      "Mai",
      "Jun",
      "Jul",
      "Ago",
      "Set",
      "Out",
      "Nov",
      "Dez",
    ],
    today: "Hoje",
    suffix: [],
    meridiem: [],
  };
  $(".datepicker")
    .datepicker({
      autoclose: true,
      todayHighlight: true,
      toggleActive: true,
      format: "dd/mm/yyyy",
      language: "pt-BR",
    })
    .on("changeDate", function (selected) {
      updateDate($(this).closest("form").find(".datepicker"), selected);
    });

  $("#fone-input").mask("00000000000");
  $("#fone-inputu").mask("00000000000");

  $("#ramal-input").mask("00000000000");
  $("#ramal-inputu").mask("00000000000");

  $("#segini-input").mask("00:00");
  $("#segini-inputu").mask("00:00");
  $("#segfim-input").mask("00:00");
  $("#segfim-inputu").mask("00:00");

  $("#terini-input").mask("00:00");
  $("#terini-inputu").mask("00:00");
  $("#terfim-input").mask("00:00");
  $("#terfim-inputu").mask("00:00");

  $("#quaini-input").mask("00:00");
  $("#quaini-inputu").mask("00:00");
  $("#quafim-input").mask("00:00");
  $("#quafim-inputu").mask("00:00");

  $("#quiini-input").mask("00:00");
  $("#quiini-inputu").mask("00:00");
  $("#quifim-input").mask("00:00");
  $("#quifim-inputu").mask("00:00");

  $("#sexini-input").mask("00:00");
  $("#sexini-inputu").mask("00:00");
  $("#sexfim-input").mask("00:00");
  $("#sexfim-inputu").mask("00:00");

  $("#dataini-input").mask("00/00/0000");
  $("#dataini-inputu").mask("00/00/0000");
  $("#datafim-input").mask("00/00/0000");
  $("#datafim-inputu").mask("00/00/0000");
});
function updateDate(inputs, selected) {
  var minDate = new Date(selected.date.valueOf());
  $(inputs[1]).datepicker("setStartDate", minDate);
  $(inputs[0]).datepicker("setEndDate", minDate);
}

function isEmail(email) {
  const regexExp =
    /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/gi;

  return regexExp.test(email);
}

function addJornada() {
  let json_jor =
    `{
    "SEXTA": {
      "FIM": "` +
    $("#sexfim-input").val() +
    `",
      "INICIO": "` +
    $("#sexini-input").val() +
    `"
    },
    "TERCA": {
      "FIM": "` +
    $("#terfim-input").val() +
    `",
      "INICIO": "` +
    $("#terini-input").val() +
    `"
    },
    "QUARTA": {
      "FIM": "` +
    $("#quafim-input").val() +
    `",
      "INICIO": "` +
    $("#quaini-input").val() +
    `"
    },
    "QUINTA": {
      "FIM": "` +
    $("#quifim-input").val() +
    `",
      "INICIO": "` +
    $("#quiini-input").val() +
    `"
    },
    "SEGUNDA": {
      "FIM": "` +
    $("#segfim-input").val() +
    `",
      "INICIO": "` +
    $("#segini-input").val() +
    `"
    }
  }`;
  $.ajax({
    url: "includes/php/ajax.php",
    type: "post",
    dataType: "json",
    data: {
      module: "jornada",
      action: "addJornada",
      "desc-input": $("#desc-input").val(),
      jornada: json_jor,
      "padr-input": Number($("#padr-input").is(":checked")),
    },
    beforeSend: function (xhr) {
      //$("#mensagem_sucesso").hide();
    },
    complete: function (xhr, textStatus) {
      $("#preload").hide();
    },
    success: function (data) {
      Swal.fire({
        position: 'top-end',
        icon: 'success',
        title: 'Cadastro realizado com sucesso!',
        showConfirmButton: false,
        timer: 2000
      })

      $("#desc-input").val("");

      $("#segini-input").val("");
      $("#segfim-input").val("");

      $("#terini-input").val("");
      $("#terfim-input").val("");

      $("#quaini-input").val("");
      $("#quafim-input").val("");

      $("#quiini-input").val("");
      $("#quifim-input").val("");

      $("#sexini-input").val("");
      $("#sexfim-input").val("");
    },
    error: function (jqXHR, textStatus, errorThrown) {
      Swal.fire({
        position: 'top-end',
        icon: 'error',
        title: 'Não foi posssível realizar o cadastro' + textStatus,
        showConfirmButton: false,
        timer: 2000
      })

      //window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
    },
  });
}

function updateJornada(id_jornada) {
  let json_jor =
    `{
    "SEXTA": {
      "FIM": "` +
    $("#sexfim-inputu").val() +
    `",
      "INICIO": "` +
    $("#sexini-inputu").val() +
    `"
    },
    "TERCA": {
      "FIM": "` +
    $("#terfim-inputu").val() +
    `",
      "INICIO": "` +
    $("#terini-inputu").val() +
    `"
    },
    "QUARTA": {
      "FIM": "` +
    $("#quafim-inputu").val() +
    `",
      "INICIO": "` +
    $("#quaini-inputu").val() +
    `"
    },
    "QUINTA": {
      "FIM": "` +
    $("#quifim-inputu").val() +
    `",
      "INICIO": "` +
    $("#quiini-inputu").val() +
    `"
    },
    "SEGUNDA": {
      "FIM": "` +
    $("#segfim-inputu").val() +
    `",
      "INICIO": "` +
    $("#segini-inputu").val() +
    `"
    }
  }`;
  $.ajax({
    url: "includes/php/ajax.php",
    type: "post",
    dataType: "json",
    data: {
      module: "jornada",
      action: "updateJornada",
      "desc-input": $("#desc-inputu").val(),
      jornada: json_jor,
      "padr-input": Number($("#padr-inputu").is(":checked")),
      id_jornada: id_jornada,
    },
    beforeSend: function (xhr) {
      //$("#mensagem_sucesso").hide();
    },
    complete: function (xhr, textStatus) {
      $("#preload").hide();
    },
    success: function (data) {
      Swal.fire({
        position: 'top-end',
        icon: 'success',
        title: 'Cadastro realizado com sucesso!',
        showConfirmButton: false,
        timer: 2000
      })
    },
    error: function (jqXHR, textStatus, errorThrown) {
      Swal.fire({
        position: 'top-end',
        icon: 'error',
        title: 'Não foi posssível realizar o cadastro' + textStatus,
        showConfirmButton: false,
        timer: 2000
      })

      //$("#ia-inputu").val('');

      //window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
    },
  });
}

$("#btn-atualizar").on("click", function () {
  var id = $("#id-inputu").val();
  updateJornada(id);
});

function getByIdJornada(id_jornada) {
  $.ajax({
    url: "includes/php/ajax.php",
    type: "post",
    dataType: "json",
    data: {
      module: "jornada",
      action: "getByIdJornada",
      id_jornada: id_jornada,
    },
    beforeSend: function (xhr) {
      //$('#preload').show();
    },
    complete: function (xhr, textStatus) {
      //$("#preload").hide();
    },
    success: function (data) {
      $("#id-inputu").val(data.response_data.id_jornada);
      $("#desc-inputu").val(data.response_data.descricao);
      $("#segini-inputu").val(data.response_data.segini);
      $("#segfim-inputu").val(data.response_data.segfim);
      $("#terini-inputu").val(data.response_data.terini);
      $("#terfim-inputu").val(data.response_data.terfim);
      $("#quaini-inputu").val(data.response_data.quaini);
      $("#quafim-inputu").val(data.response_data.quafim);
      $("#quiini-inputu").val(data.response_data.quiini);
      $("#quifim-inputu").val(data.response_data.quifim);
      $("#sexini-inputu").val(data.response_data.sexini);
      $("#sexfim-inputu").val(data.response_data.sexfim);
      $("#padr-inputu").prop(
        "checked",
        Boolean(Number(data.response_data.padr))
      );
    },
    error: function (jqXHR, textStatus, errorThrown) {
      window.alert("ERRO: " + errorThrown + "    STATUS: " + textStatus);
      /* bootbox.alert({
					message: ( 'Ocorreu um erro inesperado ao processar sua solicitação.' ),
					size: 'small'
				}); */
    },
  });
}

$(function () {
  "use strict";

  // Initialize the jQuery File Upload widget:
  $("#fileupload").fileupload({
    // Uncomment the following to send cross-domain cookies:
    //xhrFields: {withCredentials: true},
    url: "includes/php/ajax.php",
  });

  // Enable iframe cross-domain access via redirect option:
  $("#fileupload").fileupload(
    "option",
    "redirect",
    window.location.href.replace(/\/[^/]*$/, "/cors/result.html?%s")
  );

  if (window.location.hostname === "blueimp.github.io") {
    // Demo settings:
    $("#fileupload").fileupload("option", {
      url: "//jquery-file-upload.appspot.com/",
      // Enable image resizing, except for Android and Opera,
      // which actually support image resizing, but fail to
      // send Blob objects via XHR requests:
      disableImageResize: /Android(?!.*Chrome)|Opera/.test(
        window.navigator.userAgent
      ),
      maxFileSize: 999000,
      acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
    });
    // Upload server status check for browsers with CORS support:
    if ($.support.cors) {
      $.ajax({
        url: "//jquery-file-upload.appspot.com/",
        type: "HEAD",
      }).fail(function () {
        $('<div class="alert alert-danger"></div>')
          .text("Upload server currently unavailable - " + new Date())
          .appendTo("#fileupload");
      });
    }
  } else {
    // Load existing files:
    $("#fileupload").addClass("fileupload-processing");
    $.ajax({
      // Uncomment the following to send cross-domain cookies:
      //xhrFields: {withCredentials: true},
      url: $("#fileupload").fileupload("option", "url"),
      dataType: "json",
      context: $("#fileupload")[0],
    })
      .always(function () {
        $(this).removeClass("fileupload-processing");
      })
      .done(function (result) {
        $(this)
          .fileupload("option", "done")
          // eslint-disable-next-line new-cap
          .call(this, $.Event("done"), { result: result });
      });
  }
});

$("#btn-cancelar").click(function () {
  document.location.reload(true);
});

$("#ausente-input").change(function () {
  //alert(""+ Number($("#ativo-input").is(':checked')));
  if (this.checked) {
    $("#dataini-input").val("");
    $("#datafim-input").val("");

    $("#dataini-input").prop("disabled", true);
    $("#datafim-input").prop("disabled", true);
  } else {
    $("#dataini-input").prop("disabled", false);
    $("#datafim-input").prop("disabled", false);
  }
});

$("#ausente-inputu").change(function () {
  if (this.checked) {
    $("#dataini-inputu").val("");
    $("#datafim-inputu").val("");

    $("#dataini-inputu").prop("disabled", true);
    $("#datafim-inputu").prop("disabled", true);
  } else {
    $("#dataini-inputu").prop("disabled", false);
    $("#datafim-inputu").prop("disabled", false);
  }
});

$("#btn-cancelar").click(function () {
  document.location.reload(true);
});

$("#btn-cancelar2").click(function () {
  document.location.reload(true);
});

function selectSup(idGrupo) {
  $("#sup-input").val(idGrupo);
  $("#sup-input").prop("disabled", true);
  $("#sup-inputu").val(idGrupo);
  $("#sup-inputu").prop("disabled", true);
}
