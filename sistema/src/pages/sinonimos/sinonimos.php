<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

/**
 * @author    {Enter5}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
    2020-08-28 13:40 ( v1.0.0 ) - 
  }
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÏ = 0






//header('charset=UTF-8; Content-type: text/html; Cache-Control: no-cache');

header('Content-type: text/html; charset=UTF-8');
header('Cache-Control: no-cache');

$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.'));

// Montando o c??o do erro que ser?presentado
$localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar = array("\\", "/includes/php");
$substituir = array("/", "");
$path = str_replace($localizar, $substituir, __DIR__);

$fileSinonimos = $path . '/includes/php/class.sinonimos.php';

$fileTagClausulas = $path . '/includes/php/class.tagclausulas.php';
// var_dump($fileSinonimos);

if (file_exists($fileSinonimos) && file_exists($fileTagClausulas)) {

  include_once($fileTagClausulas);

  include_once($fileSinonimos);

  $sinonimos = new sinonimos();

  $clausulas = new tagclausulas();

  if ($sinonimos->response['response_status']['status'] == 1 && $clausulas->response['response_status']['status'] == 1) {

    $getSinonimos = $sinonimos->getSinonimos();

    $getClausulas = $clausulas->getTagClausulas();
    //var_dump($getTagClausulas);
    if ($getSinonimos['response_status']['status'] == 1 && $getClausulas['response_status']['status'] == 1) {

      $listaClausulas = $getClausulas['response_data']['list2'];
      $nomesClausulas = $getClausulas['response_data']['nomesClausulas'];
      $listUpdate = $getClausulas['response_data']['listUpdate'];
      $listaSinonimos = $getSinonimos['response_data']['sinonimos'];
    } else {

      $response['response_status']['status'] = 0;
      $response['response_status']['error_code'] = $error_code . __LINE__;
      $response['response_status']['msg'] = $getTagClausulas['response_status']['error_code'] . '::' . $getTagClausulas['response_status']['msg'];
    }
  } else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = $tagclausulas->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
  }
} else {
  $response['response_status']['status'] = 0;
  $response['response_status']['error_code'] = $error_code . __LINE__;
  $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.sinonimos).';
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

  <link rel="stylesheet" href="includes/css/styles.css">
  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>
  <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css">

  <!-- The following CSS are included as plugins and can be removed if unused-->
  <script src="includes/js/jquery-3.4.1.min.js"></script>
  <link rel='stylesheet' type='text/css' href='includes/plugins/form-toggle/toggles.css' />
  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

  <style>
    select {
      display: block !important;
    }

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
  <!-- <div class="page-container">
    <div class="page-content" style="min-height: 1000px;"> -->
  <?php include('menu.php'); ?>



  <div class="page-container">
    <div class="page-content" style="min-height: 1000px;">
      <!-- <div> -->
      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-1 img_container"> <!-- style="transform: translate(-13px, 13px);" -->
              <div class="container_logo_client">
                <img id="imglogo" class="img-circle">
              </div>
            </div>

            <div class="col-md-11 content_container">
              <div class="row">
                <div class="col-md-12">
                  <table class="table table-striped">

                    <tbody>
                      <tr>
                        <td><a data-toggle="modal" href="#myModal" class="btn default-alt ">ADICIONAR SINONIMOS</a></td>
                      </tr>
                    </tbody>
                  </table>
                  <div class="panel panel-sky">
                    <div class="panel-heading">
                      <h4>Cadastro de Sinonimos</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in">

                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered datatables" id="example">
                        <thead>
                          <tr>
                            <th></th>
                            <th>Sinonimos</th>
                            <th>Cláusula</th>

                          </tr>
                        </thead>
                        <tbody>
                          <?php print $listaSinonimos; ?>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

    </div>

    <!-- Inserir sinonimos -->
    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Adicionar Sinonimos</h4>
          </div>
          <div id="mensagem_sucesso" class="alert alert-dismissable alert-success">
            Cadastro realizado com sucesso!
          </div>
          <div id="mensagem_error" class="alert alert-dismissable alert-danger">
            Não foi possível realizar essa operação!
          </div>

          <div class="modal-body">
            <div class="panel panel-primary">
              <br>
              <!-- Modal de Insert = Inserir -->
              <form class="form-horizontal">
                <div class="form-group">
                  <label for="info-inputc" class="col-sm-3 control-label">Sinonimos</label>
                  <div class="col-sm-8">
                    <input type="text" class="form-control" id="info-inputc" placeholder="">
                  </div>
                </div>

                <div class="form-group">
                  <label for="info-inputc" class="col-sm-3 control-label">Cláusula</label>
                  <div class="col-sm-4">
                    <select class="form-control" id="ge-input"
                      style="-webkit-appearance: none;-moz-appearance: none;text-indent: 1px; text-overflow: '';"
                      disabled>
                      <optgroup label="SELECIONE">
                        <?php print $nomesClausulas; ?>
                      </optgroup>
                    </select>
                  </div>
                  <a id="btn-add-ge" data-toggle="modal" href="#myModalRegister" data-dismiss="modal"
                    class="col-sm-2 btn btn-primary btn-rounded">Selecionar</a>
                </div>

                <!-- Tratar buttons de validações -->
                <div class="row">
                  <div class="col-sm-6 col-sm-offset-3">
                    <div class="btn-toolbar">
                      <a href="#" class="btn btn-primary btn-rounded" onclick="addSinonimos();">Processar</a>
                      <a id="btn-cancelar" href="#" class="btn btn-primary btn-rounded">Finalizar</a>
                    </div>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Atualização de sinonimos -->
    <div class="modal fade" id="myModalUpdate" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
      aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Atualizar Sinonimos</h4>
          </div>
          <div id="mensagem_alterado_sucessou" class="alert alert-dismissable alert-success">
            Cadastro atualizado com sucesso!
          </div>
          <div id="mensagem_alterado_erroru" class="alert alert-dismissable alert-danger">
            Não foi possível realizar essa operação!
          </div>

          <div class="modal-body">
            <div class="panel panel-primary">
              <br>
              <!-- Modal de Insert = Inserir -->
              <form class="form-horizontal">
                <div class="form-group">
                  <label for="info-inputc" class="col-sm-3 control-label">Sinonimos</label>
                  <div class="col-sm-8">
                    <input type="text" class="form-control" id="info-input-update" placeholder="">
                  </div>
                </div>

                <div class="form-group">
                  <label for="ge-input-update" class="col-sm-3 control-label">Cláusula</label>
                  <div class="col-sm-4">
                    <select class="form-control" id="ge-input-update"
                      style="-webkit-appearance: none;-moz-appearance: none;text-indent: 1px; text-overflow: '';"
                      disabled>
                      <optgroup label="SELECIONE">
                        <?php print $nomesClausulas; ?>
                      </optgroup>
                    </select>
                  </div>
                  <a id="btn-add-ge" data-toggle="modal" href="#myModalClausulaUpdate" data-dismiss="modal"
                    class="col-sm-2 btn btn-primary btn-rounded">Selecionar</a>
                </div>

                <!-- Tratar buttons de validações -->
                <div class="row">
                  <div class="col-sm-6 col-sm-offset-3">
                    <div class="btn-toolbar">
                      <input type="hidden" id="input-hide" value="">
                      <a href="#" class="btn btn-primary btn-rounded" onclick="updateSinonimos();">Processar</a>
                      <a id="btn-cancelar-update" href="#" class="btn btn-primary btn-rounded">Finalizar</a>
                    </div>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Selecionar Cláusula -->

    <div id="myModalRegister" class="modal" tabindex="-1" role="dialog">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="panel panel-sky">
            <div class="panel-heading">
              <h4>Selecione a Cláusula de Referência</h4>
              <div class="options">
                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
              </div>
            </div>
            <div class="panel-body collapse in">

              <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables"
                id="example">
                <thead>
                  <tr>
                    <th>Selecionar</th>
                    <th>Nome</th>
                    <th>Tipoo</th>
                    <th>Classe</th>
                  </tr>
                </thead>
                <tbody>
                  <?php print $listaClausulas; ?>
                </tbody>
              </table>
            </div>
          </div>
          <div class="modal-footer">
            <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
              data-dismiss="modal">Seguinte</button>
          </div>
        </div>
      </div>
    </div>

    <div id="myModalClausulaUpdate" class="modal" tabindex="-1" role="dialog">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="panel panel-sky">
            <div class="panel-heading">
              <h4>Selecione a Cláusula de Referência</h4>
              <div class="options">
                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
              </div>
            </div>
            <div class="panel-body collapse in">

              <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables"
                id="example">
                <thead>
                  <tr>
                    <th>Selecionar</th>
                    <th>Nome</th>
                    <th>Tipoo</th>
                    <th>Classe</th>
                  </tr>
                </thead>
                <tbody>
                  <?php print $listUpdate; ?>
                </tbody>
              </table>
            </div>
          </div>
          <div class="modal-footer">
            <button data-toggle="modal" href="#myModalUpdate" type="button" class="btn btn-secondary"
              data-dismiss="modal">Seguinte</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Estrutura de Modals -->
    <?php include_once('modalTagClausulasPasso2.php'); ?>

  </div> <!--page-content -->

  <?php include 'footer.php' ?>

  </div> <!--page-container -->



  <script type='text/javascript' src='includes/js/jquery-1.10.2.min.js'></script>
  <script type='text/javascript' src='includes/js/jqueryui-1.10.3.min.js'></script>
  <script type='text/javascript' src='includes/js/bootstrap.min.js'></script>
  <script type='text/javascript' src='includes/js/enquire.js'></script>
  <script type='text/javascript' src='includes/js/jquery.cookie.js'></script>
  <script type='text/javascript' src='includes/js/jquery.nicescroll.min.js'></script>
  <script type='text/javascript' src='includes/plugins/codeprettifier/prettify.js'></script>
  <script type='text/javascript' src='includes/plugins/easypiechart/jquery.easypiechart.min.js'></script>
  <script type='text/javascript' src='includes/plugins/sparklines/jquery.sparklines.min.js'></script>
  <script type='text/javascript' src='includes/plugins/form-toggle/toggle.min.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.min.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/TableTools.js'></script>
  <script type='text/javascript' src='includes/plugins/jquery-editable/jquery.editable.min.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/dataTables.bootstrap.js'></script>
  <script type='text/javascript' src='includes/demo/demo-datatables.js'></script>
  <script type='text/javascript' src='includes/plugins/form-validation/jquery.validate.min.js'></script>
  <script type='text/javascript' src='includes/plugins/form-stepy/jquery.stepy.js'></script>
  <script type='text/javascript' src='includes/demo/demo-formwizard.js'></script>
  <script type='text/javascript' src='includes/js/placeholdr.js'></script>
  <script type='text/javascript' src='includes/demo/demo-modals.js'></script>
  <script type='text/javascript' src='includes/js/application.js'></script>
  <script type='text/javascript' src='includes/demo/demo.js'></script>
  <script type='text/javascript' src="includes/js/tagclausulas.js"></script>
  <script type='text/javascript' src="includes/js/sinonimos.js"></script>
</body>

</html>