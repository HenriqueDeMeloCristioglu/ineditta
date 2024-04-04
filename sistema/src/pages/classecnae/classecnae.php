<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}
$sessionUser = $_SESSION['login'];

$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.'));

// Montando o c??o do erro que ser?presentado
$localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar = array("\\", "/includes/php");
$substituir = array("/", "");
$path = str_replace($localizar, $substituir, __DIR__);


$fileClassCalendario = $path . '/includes/php/class.classecnae.php';

if (file_exists($fileClassCalendario)) {

  include_once($fileClassCalendario);

  include_once __DIR__ . "/includes/php/class.usuario.php";

  $user = new usuario();
  $userData = $user->validateUser($sessionUser)['response_data']['user'];

  $modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

  $modulos = ["sisap" => $modulosSisap, "comercial" => []];

  $sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

  foreach ($sisap as $key => $value) {
    if (mb_strpos($value, "Adm - Cadastro de classe CNAE")) {
      $indexModule = $key;
      $idModule = strstr($value, "+", true);
    }
  }

  for ($i = 0; $i < count($modulosSisap); $i++) {
    if ($modulosSisap[$i]->id == $idModule) {
      $thisModule = $modulosSisap[$i];
      break;
    }
  }
} else {
  $response['response_status']['status'] = 0;
  $response['response_status']['error_code'] = $error_code . __LINE__;
  $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.classecnae).';
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

  <link rel="stylesheet" href="classecnae.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
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

    td {
      word-break: break-all
    }

    #page-content {
      min-height: 100% !important;
    }
  </style>
</head>

<body class="horizontal-nav">
  <?php include('menu.php'); ?>

  <div id="page-container">
    <div id="pageCtn">
      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-1 img_container">
              <div class="container_logo_client">
                <img id="imglogo" class="img-circle">
              </div>
            </div>

            <div class="col-md-11 content_container">
              <table class="table table-striped">
                <tbody>
                  <tr>
                    <?php if ($thisModule->Criar == 1): ?>
                      <td>
                        <button type="button" class="btn default-alt" data-toggle="modal" data-target="#cadastrarModal"
                          id="novoCnaeBtn">NOVA CLASSE
                          CNAE</button>
                      </td>
                    <?php else: ?>

                    <?php endif; ?>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <!-- TELA PRINCIPAL -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro de classe cnae</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <div id="grid-layout-table-1" class="box jplist">
                    <div class="box text-shadow">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="cnaesTb" data-order='[[ 1, "asc" ]]'
                        style="width: 100%;"></table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div> <!-- container -->
      </div> <!-- #wrap -->

      <div class="hidden modal_hidden" id="cadastrarModalHidden">
        <div class="modal-content" id="cadastrarModalHiddenContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Cadastro de classe cnae</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Registro Cnae</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body">
                <form class="form-horizontal">
                  <div class="row">
                    <!-- <div class="col-lg-1">
                                                          <label for="cnae-input" class="control-label">CNAE</label>
                                                          <input type="text" class="form-control" id="cnae-input" placeholder="0000000">
                                                      </div> -->
                    <div class="col-lg-2">
                      <label for="divisao" class="control-label">Divisão</label>
                      <input type="text" class="form-control" id="divisao" placeholder="0000000">
                    </div>
                    <div class="col-lg-4">
                      <label for="descricao_divisao" class="control-label">Descrição
                        Divisão</label>
                      <input type="text" class="form-control" id="descricao_divisao" placeholder="">
                    </div>
                    <div class="col-lg-1">
                      <label for="subclasse" class="control-label">Subclasse</label>
                      <input type="text" class="form-control" id="subclasse" placeholder="0000000">
                    </div>
                    <div class="col-lg-5">
                      <label for="descricao_subclasse" class="control-label">Descrição
                        subclasse</label>
                      <input type="text" class="form-control" id="descricao_subclasse" placeholder="">
                    </div>
                  </div>

                  <div class="row">
                    <div class="col-lg-12">
                      <label for="categoria" class="control-label">Categoria</label>
                      <textarea rows='5' class="form-control" id="categoria" placeholder=""></textarea>
                    </div>
                  </div>
                </form>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <div class="row">
              <div class="col-lg-12" style="display: flex; justify-content:center">
                <button type="button" class="btn btn-primary btn-rounded" id="cadastrarBtn">Salvar</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div> <!-- page-content -->

    <?php include 'footer.php' ?>
  </div> <!-- page-container -->

  <!-- PAGE SCRIPT -->
  <script type='text/javascript' src="./js/classecnae.min.js"></script>

</body>

</html>