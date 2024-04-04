<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}
$sessionUser = $_SESSION['login'];
/**
 * @author      {Enter5}
 * @package     {1.0.0}
 * @description { }
 * @historic    {
        2020-08-28 13:40 ( v1.0.0 ) - 
    }
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÏ = 0



$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.'));

// Montando o c??o do erro que ser?presentado
$localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar = array("\\", "/includes/php");
$substituir = array("/", "");
$path = str_replace($localizar, $substituir, __DIR__);

$fileLocalizacao = $path . '/includes/php/class.localizacao.php';

if (file_exists($fileLocalizacao)) {

  include_once $fileLocalizacao;

  include_once __DIR__ . "/includes/php/class.usuario.php";

  $user = new usuario();
  $userData = $user->validateUser($sessionUser)['response_data']['user'];

  $modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

  $modulos = ["sisap" => $modulosSisap, "comercial" => []];

  $sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

  foreach ($sisap as $key => $value) {
    if (mb_strpos($value, "Adm - Cadastro de Localização")) {
      $indexModule = $key;
      $idModule = strstr($value, "+", true);
    }
  }

  for ($i = 0; $i < count($modulosSisap); $i++) {
    if ($modulosSisap[$i]->id == $idModule) {
      $thisModule = $modulosSisap[$i]; //module Permissions here
      break;
    }
  }

  $localizacao = new localizacao();

  if ($localizacao->response['response_status']['status'] == 1) {

    $getLocalizacao = $localizacao->getLocalizacao();

    if ($getLocalizacao['response_status']['status'] == 1) {

      $lista = $getLocalizacao['response_data']['html'];
    } else {
      $response['response_status']['status'] = 0;
      $response['response_status']['error_code'] = $error_code . __LINE__;
      $response['response_status']['msg'] = $getLocalizacao['response_status']['error_code'] . '::' . $getLocalizacao['response_status']['msg'];
    }
  } else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = $localizacao->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
  }
} else {
  $response['response_status']['status'] = 0;
  $response['response_status']['error_code'] = $error_code . __LINE__;
  $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.localizacao).';
}

if ($response['response_status']['status'] == 0) {

  print $response['response_status']['error_code'] . " :: " . $response['response_status']['msg'];
  exit();
}

?>
<!DOCTYPE html>
<html lang="pt-br">

<head>
  <meta charset="utf-8">
  <title>Ineditta</title>
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <meta name="description" content="Ineditta">
  <meta name="author" content="The Red Team">

  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>

  <!-- The following CSS are included as plugins and can be removed if unused-->
  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

  <!-- Bootstrap 3.3.7 -->
  <link rel="stylesheet" href="localizacao.css">

  <!-- Bootstrap Internal -->
  <link rel="stylesheet" href="includes/css/styles.css">


  <style type="text/css">
    #mensagem_sucesso {
      display: none;
    }

    #mensagem_error {
      display: none;
    }

    #mensagem_alterado_sucesso {
      display: none;
    }

    #mensagem_alterado_error {
      display: none;
    }

    #mensagem_sucessou {
      display: none;
    }

    #mensagem_erroru {
      display: none;
    }

    #mensagem_alterado_sucessou {
      display: none;
    }

    #mensagem_alterado_erroru {
      display: none;
    }

    #page-content {
      min-height: 100% !important;
    }
  </style>
</head>

<body class="horizontal-nav">
  <?php require 'menu.php'; ?>

  <div class="page-container" id="pageCtn">

    <div id="page-content">
      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-1 img_container"> <!-- style="transform: translate(-13px, 13px);" -->
              <div class="container_logo_client">
                <img id="imglogo" class="img-circle">
              </div>
            </div>

            <div class="col-md-11 content_container">
              <table class="table table-striped">

                <tbody>
                  <tr>
                    <?php if ($thisModule->Criar == 1): ?>
                      <td><button type="button" class="btn default-alt " data-toggle="modal"
                          data-target="#localizacaoModal" id="localizacaoBtn">Nova Localização</button></td>
                    <?php else: ?>

                    <?php endif; ?>
                  </tr>
                </tbody>
              </table>

              <div class="hidden modal_hidden" id="localizacaoModalHidden">
                <div id="localizacaoModalHiddenContent">
                  <div class="modal-content">
                    <div class="modal-header">
                      <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                      <h4 class="modal-title">Cadastro de Localizações</h4>
                    </div>
                    <div class="modal-body">
                      <div class="panel panel-primary">
                        <div class="panel-heading">
                          <h4>Novo Cadastro</h4>
                          <div class="options">
                            <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                          </div>
                        </div>
                        <div class="panel-body">
                          <form class="form-horizontal" id="formLocalizacao">
                            <input type="hidden" id="id-input">
                            <div class="form-group">
                              <label for="cod-input" class="col-sm-1 control-label">Nº País</label>
                              <div class="col-sm-1">
                                <input type="text" class="form-control" id="cod-input" placeholder="0000">
                              </div>
                              <label for="pais-input" class="col-sm-1 control-label">País</label>
                              <div class="col-sm-3">
                                <input type="text" class="form-control" id="pais-input" placeholder="">
                              </div>
                              <label for="reg-input" class="col-sm-1 control-label">NºRegião</label>
                              <div class="col-sm-1">
                                <input type="text" class="form-control" id="reg-input" placeholder="0000">
                              </div>
                              <label for="regiao-input" class="col-sm-1 control-label">Região</label>
                              <div class="col-sm-3">
                                <input type="text" class="form-control" id="regiao-input" placeholder="">
                              </div>
                            </div>
                            <div class="form-group">
                              <label for="coduf-input" class="col-sm-1 control-label">N° UF</label>
                              <div class="col-sm-1">
                                <input type="text" class="form-control" id="coduf-input" placeholder="0000">
                              </div>
                              <label for="uf-input" class="col-sm-1 control-label">UF</label>
                              <div class="col-sm-1">
                                <input type="text" class="form-control" id="uf-input" placeholder="AA">
                              </div>
                              <label for="est-input" class="col-sm-1 control-label">Estado</label>
                              <div class="col-sm-3">
                                <input type="text" class="form-control" id="est-input" placeholder="">
                              </div>
                              <label for="codmun-input" class="col-sm-1 control-label">NºMunicípio</label>
                              <div class="col-sm-3">
                                <input type="text" class="form-control" id="codmun-input" placeholder="0000">
                              </div>

                            </div>
                            <div class="form-group">
                              <label for="mun-input" class="col-sm-1 control-label">Município</label>
                              <div class="col-sm-11">
                                <input type="text" class="form-control" id="mun-input" placeholder="">
                              </div>
                            </div>
                          </form>
                        </div>
                      </div>
                    </div>
                    <div class="modal-footer">
                      <div class="row">
                        <div class="col-sm-12" style="display: flex; justify-content:center">
                          <button type="button" class="btn btn-primary btn-rounded"
                            id="localizacaoCadastrarBtn">Salvar</button>
                        </div>
                      </div>
                    </div>
                  </div><!-- /.modal-content -->
                </div><!-- /.modal-dialog -->
              </div><!-- /.modal -->
            </div>
          </div>

          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro de Localização</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <div class="box text-shadow">
                    <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="localizacaotb"
                      data-order='[[ 1, "asc" ]]'>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div> <!-- container -->
      </div> <!-- #wrap -->
    </div> <!-- page-content -->


    <?php include 'footer.php' ?>

  </div> <!-- page-container -->

  <script type='text/javascript' src="./js/localizacao.min.js"></script>
</body>

</html>