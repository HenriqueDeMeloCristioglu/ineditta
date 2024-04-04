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


        redirectUri: "http://localhost:8000/formulario_aplicacao.php", //a url da pagina que está implementando,

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
            window.location.replace("http://localhost:8000/index.php");
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
    "url": "http://localhost:8000/auth/admin/realms/Ineditta-prod/users/" + key_user_id + "/logout",
    "method": "POST",
    "timeout": 0,
    "headers": {
      "Authorization": "Bearer " + access_token,
    },
  };

  $.ajax(settings).done(function (response) {
    console.log(response);
    document.location.href = 'http://localhost:8000/exit.php'

  });

}


$(function () {
  $(".select2").select2();
})



/*********************************************************************
* BOTÃO FINALIZAR
********************************************************************/

var btnCancelar = document.querySelectorAll(".btn-cancelar");

btnCancelar.forEach(btn => {
  btn.addEventListener("click", () => document.location.reload(true))
});


