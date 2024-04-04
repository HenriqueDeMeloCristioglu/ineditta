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

//Localizando usuário logado no Windows
if (!empty($_SERVER['REMOTE_USER'])) {
  $nmloginweb = $_SERVER['REMOTE_USER'];
} elseif (!empty($_SERVER['LOGON_USER'])) {
  $nmloginweb = $_SERVER['LOGON_USER'];
} elseif (!empty($_SERVER['AUTH_USER'])) {
  $nmloginweb = $_SERVER['AUTH_USER'];
}

$filePatro = $path . '/includes/php/class.federacaopatro.php';

if (file_exists($filePatro)) {

  include_once($filePatro);

  $federacaopatro = new federacaopatro();

  if ($federacaopatro->response['response_status']['status'] == 1) {

    $getFederacaoPatro = $federacaopatro->getFederacaoPatro();

    if ($getFederacaoPatro['response_status']['status'] == 1) {

      $lista = $getFederacaoPatro['response_data']['html'];
    } else {
      $response['response_status']['status'] = 0;
      $response['response_status']['error_code'] = $error_code . __LINE__;
      $response['response_status']['msg'] = $getFederacaoPatro['response_status']['error_code'] . '::' . $getFederacaoPatro['response_status']['msg'];
    }
  } else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = $federacaopatro->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
  }
} else {
  $response['response_status']['status'] = 0;
  $response['response_status']['error_code'] = $error_code . __LINE__;
  $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.federacao_patro).';
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

  <!-- <link href="includes/less/styles.less" rel="stylesheet/less" media="all">  -->
  <link rel="stylesheet" href="includes/css/styles.css">
  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>
  <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css">
  <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='styleswitcher'>
  <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='headerswitcher'>

  <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries. Placeholdr.js enables the placeholder attribute -->
  <!--[if lt IE 9]>
        <script type="text/javascript" src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
        <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/respond.js/1.1.0/respond.min.js"></script>
        <script type="text/javascript" src="includes/plugins/charts-flot/excanvas.min.js"></script>
    <![endif]-->

  <!-- The following CSS are included as plugins and can be removed if unused-->
  <script src="includes/js/jquery-3.4.1.min.js"></script>
  <link rel='stylesheet' type='text/css' href='includes/plugins/form-toggle/toggles.css' />
  <!-- <script type="text/javascript" src="includes/js/less.js"></script> -->

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

  <?php include('menu.php'); ?>


  <div id="page-container">

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
                    <td><a data-toggle="modal" href="#myModal" class="btn default-alt  ">NOVA FEDERACÃO</a></td>
                  </tr>
                </tbody>
              </table>

              <div class="modal fade" id="updateModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                aria-hidden="true">
                <div class="modal-dialog">
                  <div class="modal-content">
                    <div class="modal-header">
                      <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                      <h4 class="modal-title">Atualização da Federação</h4>
                    </div>
                    <div id="mensagem_alterado_sucessou" class="alert alert-dismissable alert-success">
                      Atualização realizada com sucesso!
                    </div>
                    <div id="mensagem_alterado_erroru" class="alert alert-dismissable alert-danger">
                      Não foi possível atualizar essa operação!
                    </div>
                    <div class="modal-body">
                      <div class="panel panel-primary">
                        <br> <!--Modal de Update = Atualizar -->
                        <form class="form-horizontal">
                          <input type="hidden" id="id-inputu" value="1">
                          <div class="form-group">
                            <label for="patro-inputu" class="col-sm-3 control-label">Federação Patrimonial</label>
                            <div class="col-sm-8">
                              <input type="text" class="form-control" id="patro-inputu" placeholder="">
                            </div>
                          </div>
                          <!-- Tratar butons de confirmação -->
                        </form>
                        <div class="row">
                          <div class="col-sm-6 col-sm-offset-3">
                            <div class="btn-toolbar">
                              <a id="btn-atualizar" href="#" class="btn btn-primary btn-rounded">Processar</a>
                              <a id="btn-cancelar2" href="#" class="btn btn-primary btn-rounded">Finalizar</a>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                    <!-- 
                    <div class="modal-footer">
                      <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                      <button type="button" class="btn btn-primary">Save changes</button>
                    </div> -->
                  </div><!-- /.modal-content -->
                </div><!-- /.modal-dialog -->
              </div><!-- /.modal -->

              <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                aria-hidden="true">
                <div class="modal-dialog">
                  <div class="modal-content">
                    <div class="modal-header">
                      <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                      <h4 class="modal-title">Cadastro de Federações</h4>
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
                            <label for="patro-inputc" class="col-sm-3 control-label">Federação Patrimonial</label>
                            <div class="col-sm-8">
                              <input type="text" class="form-control" id="patro-inputc" placeholder="">
                            </div>
                          </div>
                        </form>
                        <!-- Tratar buttons de validações -->
                        <div class="row">
                          <div class="col-sm-6 col-sm-offset-3">
                            <div class="btn-toolbar">
                              <a href="#" class="btn btn-primary btn-rounded" onclick="addFedePatro();">Processar</a>
                              <a id="btn-cancelar" href="#" class="btn btn-primary btn-rounded">Finalizar</a>
                            </div>
                          </div>
                        </div>




                      </div>

                    </div>
                    <!-- 
                    <div class="modal-footer">
                      <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                      <button type="button" class="btn btn-primary">Save changes</button>
                    </div> -->
                  </div><!-- /.modal-content -->
                </div><!-- /.modal-dialog -->
              </div><!-- /.modal -->

              <div class="panel panel-sky">
                <div class="panel-heading">
                  <h4>Cadastro de Federações Padrimoniais</h4>
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
                        <th>Numero da Federação</th>
                        <th>Federação Patrimonial</th>
                      </tr>
                    </thead>
                    <tbody>
                      <?php print $lista; ?>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>

        </div> <!-- container -->
      </div> <!-- #wrap -->
    </div> <!-- page-content -->

    <?php include 'footer.php' ?>

  </div> <!-- page-container -->
  <script type='text/javascript' src="includes/js/federacaopatro.js"></script>




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
  <script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.min.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/TableTools.js'></script>
  <script type='text/javascript' src='includes/plugins/jquery-editable/jquery.editable.min.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.min.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/dataTables.bootstrap.js'></script>
  <script type='text/javascript' src='includes/demo/demo-datatables.js'></script>
  <script type='text/javascript' src='includes/js/placeholdr.js'></script>
  <script type='text/javascript' src='includes/demo/demo-modals.js'></script>
  <script type='text/javascript' src='includes/js/application.js'></script>
  <script type='text/javascript' src='includes/demo/demo.js'></script>

  <!-- <script>!window.jQuery && document.write(unescape('%3Cscript src="includes/js/jquery-1.10.2.min.js"%3E%3C/script%3E'))</script>
<script type="text/javascript">!window.jQuery.ui && document.write(unescape('%3Cscript src="includes/js/jqueryui-1.10.3.min.js'))</script>
<script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
<script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script> -->


</body>

</html>