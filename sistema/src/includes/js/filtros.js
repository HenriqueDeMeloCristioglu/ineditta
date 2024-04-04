import $ from 'jquery';

/*********************************************************************
 * OBTENDO LISTAS
 ********************************************************************/

if (sessionStorage.getItem('tipo') == "Ineditta") {
  $("#matriz").prop("disabled", true)
}

if (sessionStorage.getItem('tipo') == "Cliente") {
  $("#grupo").val(sessionStorage.getItem('grupoecon'))
  $("#grupo").prop("disabled", true)
}


function getCnae() {
  $.ajax({
    url: "includes/php/ajax.php"
    , type: "post"
    , dataType: "json"
    , data: {
      "module": "filtro",
      "action": "getCnae",
      "id_grupo": ($("#grupo").val() ? $("#grupo").val()+"": ""),
      "id_matriz": ($("#matriz").val() ? $("#matriz").val()+"": ""),
      "unidades": ($("#unidade").val() ? $("#unidade").val()+"": ""),
      iduser: sessionStorage.getItem('iduser'),
      tipo: sessionStorage.getItem('tipo')
    }
    , success: function (data) {

      $("#categoria").html(data.response_data.lista_cnae)

    }
    , error: function (jqXHR, textStatus, errorThrown) {

      // Swal.fire({
      //   position: 'top-end',
      //   icon: 'error',
      //   title: 'Selecione um item da lista',
      //   showConfirmButton: false,
      //   timer: 7000
      // })
    }
  })
}

function getMatriz() {
  $.ajax({
    url: "includes/php/ajax.php"
    , type: "post"
    , dataType: "json"
    , data: {
      "module": "filtro",
      "action": "getMatriz",
      "id_grupo": ($("#grupo").val() ? $("#grupo").val()+"": "")
    }
    , success: function (data) {


      $("#matriz").prop("disabled", false)
      $("#matriz").html(data.response_data.lista_matriz)

      $("#matriz").select2({
        placeholder: 'Nome, CNPJ, Código'
      })
    }
    , error: function (jqXHR, textStatus, errorThrown) {

      // Swal.fire({
      //   position: 'top-end',
      //   icon: 'error',
      //   title: 'Selecione um item da lista',
      //   showConfirmButton: false,
      //   timer: 1500
      // })
    }
  })
}

function getUnidade(id = null) {
  $.ajax({
    url: "includes/php/ajax.php"
    , type: "post"
    , dataType: "json"
    , data: {
      "module": "filtro",
      "action": "getUnidade",
      "id_matriz": (!id ? $("#matriz").val() : id),
      iduser: sessionStorage.getItem('iduser'),
      tipo: sessionStorage.getItem('tipo'),
      "id_grupo": $("#grupo").val()
    }
    , success: function (data) {

      $("#unidade").prop("disabled", false)
      $("#unidade").html(data.response_data.lista_unidade)
    }
    , error: function (jqXHR, textStatus, errorThrown) {

      // Swal.fire({
      //   position: 'top-end',
      //   icon: 'error',
      //   title: 'Selecione um item da lista ',
      //   showConfirmButton: false,
      //   timer: 1500
      // })
    }
  })
}

// $("#unidade").prop("disabled", true)

$("#matriz").on("change", () => {
  getUnidade()
  getCnae()
  getSindicatos()
  getLocalidadeByMatrizOuUnidade();
  // $("#unidade").prop("disabled", false)
  // if($("#matriz").val()+"" == "0"){
  //   $("#unidade").prop("disabled", true)
  // }
})

//Sindicatos
function getSindicatos() {
  if ($("#localidade").val() != "") {
    $.ajax({
      url: "includes/php/ajax.php"
      , type: "post"
      , dataType: "json"
      , data: {
        "module": "filtro",
        "action": "getSindicatosByLocal",
        "localidade": $("#localidade").val(),
        "cnaes": $("#categoria").val(),
        "matriz": $("#matriz").val(),
        "unidades": $("#unidade").val(),
        "grupo": $("#grupo").val()
      }
      , success: function (data) {

        $("#sind_laboral").html(data.response_data.lista_laboral_local)
        $("#sind_patronal").html(data.response_data.lista_patronal_local)
        

      }
      , error: function (jqXHR, textStatus, errorThrown) {

        // Swal.fire({
        //   position: 'top-end',
        //   icon: 'error',
        //   title: 'Selecione um item da lista ',
        //   showConfirmButton: false,
        //   timer: 1500
        // })
      }
    })
  }
  
}

$("#localidade").on("change", () => {
  getSindicatos()
})
$("#categoria").on("change", () => {
  getSindicatos()
})
$("#unidade").on("change", () => {
  getCnae()
  getSindicatos()
  getLocalidadeByMatrizOuUnidade()
})
//clausulas
$("#clausulaList").prop("disabled", true)

function getClausulas(id = null) {
  $.ajax({
    url: "includes/php/ajax.php"
    , type: "post"
    , dataType: "json"
    , data: {
      "module": "filtro",
      "action": "getClausulas",
      "id_grupo_clausula": (!id ? $("#grupo_clausulas").val() : id)
    }
    , success: function (data) {


      $("#clausulaList").prop("disabled", false)
      $("#clausulaList").html(data.response_data.lista_clausulas)


    }
    , error: function (jqXHR, textStatus, errorThrown) {

      // Swal.fire({
      //   position: 'top-end',
      //   icon: 'warning',
      //   title: 'Selecione um item da lista ',
      //   showConfirmButton: false,
      //   timer: 1500
      // })
    }
  })
}
$("#grupo_clausulas").on("change", () => {
  getClausulas()
})

// $("#localidade").prop("disabled", true)
// $("#categoria").prop("disabled", true)

function getLocalidade() {
  //alert("LOCALIDADE ESTÁ SENDO CHAMADO")
  $.ajax({
    url: "includes/php/ajax.php"
    , type: "post"
    , dataType: "json"
    , data: {
      "module": "filtro",
      "action": "getLocalidadeByGrupo",
      "id_grupo": $("#grupo").val(),
      "id_matriz": $("#matriz").val(),
      "id_unidade": $("#unidade").val()
    }
    , success: function (data) {

      console.log(data.response_data);
      $("#localidade").html(data.response_data.optLocalizacao)
      $("#localidade").append(data.response_data.optUf)
      $("#localidade").append(data.response_data.optRegiao)

    }
    , error: function (jqXHR, textStatus, errorThrown) {

      // Swal.fire({
      //   position: 'top-end',
      //   icon: 'warning',
      //   title: 'Selecione um item da lista ',
      //   showConfirmButton: false,
      //   timer: 1500
      // })
    }
  })
}

function getLocalidadeByMatrizOuUnidade() {
  $.ajax({
    url: "includes/php/ajax.php"
    , type: "post"
    , dataType: "json"
    , data: {
      "module": "filtro",
      "action": "getLocalidade",
      "id_matriz": $("#matriz").val(),
      "id_unidade": $("#unidade").val(),
      "id_grupo": $("#grupo").val(),
    }
    , success: function (data) {

      console.log(data.response_data);
      $("#localidade").html(data.response_data.optLocalizacao)
      $("#localidade").append(data.response_data.optUf)
      $("#localidade").append(data.response_data.optRegiao)

    }
    , error: function (jqXHR, textStatus, errorThrown) {

      // Swal.fire({
      //   position: 'top-end',
      //   icon: 'warning',
      //   title: 'Selecione um item da lista ',
      //   showConfirmButton: false,
      //   timer: 1500
      // })
    }
  })
}

function getDataBase() {
  $.ajax({
    url: "includes/php/ajax.php"
    , type: "post"
    , dataType: "json"
    , data: {
      "module": "filtro",
      "action": "getDataBase",
      "sind_laboral": $("#sind_laboral").val(),
      "sind_patronal": $("#sind_patronal").val()
    }
    , success: function (data) {

      $("#data_base").html(data.response_data.data_base)

    }
    , error: function (jqXHR, textStatus, errorThrown) {

      // Swal.fire({
      //   position: 'top-end',
      //   icon: 'warning',
      //   title: 'Selecione um item da lista ',
      //   showConfirmButton: false,
      //   timer: 1500
      // })
    }
  })
}

$("#sind_laboral, #sind_patronal").on("change", () => {
  getDataBase();
})


$("#grupo").on("change", () => {
  getMatriz();
  getCnae();
  getLocalidade();
  getSindicatos()

  $("#matriz").prop("disabled", false)

  // $("#localidade").prop("disabled", false)
  // $("#categoria").prop("disabled", false)
})