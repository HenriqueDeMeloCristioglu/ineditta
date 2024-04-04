<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

$userSession = $_SESSION['login'];

/**
 * @author    {Enter5}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
    2022-07-12 09:54 ( v1.0.0 ) -
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

$fileNegociacao = $path . '/includes/php/class.negociacao.php';

if (file_exists($fileNegociacao)) {
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

  <link rel="stylesheet" href="calendariosindicato.css">
  <link rel="stylesheet" href="includes/css/styles.css">
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

  <div id="pageCtn">
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
                    <td><a data-toggle="modal" href="#myModal" class="btn default-alt  ">NOVO REGISTRO DE CALENDÁRIO</a>
                    </td>
                  </tr>
                </tbody>
              </table>

              <div class="modal fade" id="updateModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                aria-hidden="true">
                <div class="modal-dialog">
                  <div class="modal-content">
                    <div class="modal-header">
                      <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                      <h4 class="modal-title">Atualização do Registro de calendário</h4>
                    </div>
                    <div id="mensagem_alterado_sucessou" class="alert alert-dismissable alert-success">
                      Atualização realizada com sucesso!
                    </div>
                    <div id="mensagem_alterado_erroru" class="alert alert-dismissable alert-danger">
                      Não foi possível atualizar essa operação!
                    </div>
                    <div class="modal-body">
                      <div class="panel panel-primary">
                        <br>
                        <form class="form-horizontal">
                          <input type="hidden" id="id-inputu" value="1">
                          <div class="form-group">
                            <div class="col-sm-3">
                              <label for="clau-inputu" class="control-label">Cláusula</label>
                              <input type="text" class="form-control" id="clau-inputu" placeholder="">
                            </div>
                            <div class="col-sm-6">
                              <label for="ass-inputu" class="control-label">Assunto</label>
                              <input type="text" class="form-control" id="ass-inputu" placeholder="">
                            </div>
                          </div>

                          <div class="form-group">
                            <div class="col-sm-2">
                              <label for="data-inputu" class="control-label">Data da ocorrência</label>
                              <input type="text" class="form-control" id="data-inputu" placeholder="DD/MM/AAAA">
                            </div>
                            <div class="col-sm-6">
                              <label for="rec-inputu" class="control-label">Recorrência</label>
                              <input type="text" class="form-control" id="rec-inputu" placeholder="">
                            </div>
                          </div>

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
                  </div><!-- /.modal-content -->
                </div><!-- /.modal-dialog -->
              </div><!-- /.modal -->

              <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"
                aria-hidden="true">
                <div class="modal-dialog">
                  <div class="modal-content">
                    <div class="modal-header">
                      <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                      <h4 class="modal-title">Cadastro de Registro de Calendário</h4>
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
                        <form class="form-horizontal">
                          <div class="form-group">
                            <div class="col-sm-3">
                              <label for="clau-input" class="control-label">Cláusula</label>
                              <input type="text" class="form-control" id="clau-input" placeholder="">
                            </div>
                            <div class="col-sm-6">
                              <label for="ass-input" class="control-label">Assunto</label>
                              <input type="text" class="form-control" id="ass-input" placeholder="">
                            </div>
                          </div>

                          <div class="form-group">
                            <div class="col-sm-2">
                              <label for="data-input" class="control-label">Data da ocorrência</label>
                              <input type="text" class="form-control" id="data-input" placeholder="DD/MM/AAAA">
                            </div>
                            <div class="col-sm-6">
                              <label for="rec-input" class="control-label">Recorrência</label>
                              <input type="text" class="form-control" id="rec-input" placeholder="">
                            </div>
                          </div>

                        </form>
                        <div class="row">
                          <div class="col-sm-6 col-sm-offset-3">
                            <div class="btn-toolbar">
                              <a href="#" class="btn btn-primary btn-rounded"
                                onclick="addCalendarioSindicato();">Processar</a>
                              <a id="btn-cancelar" href="#" class="btn btn-primary btn-rounded">Finalizar</a>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div><!-- /.modal-content -->
                </div><!-- /.modal-dialog -->
              </div><!-- /.modal -->

              <div class="panel panel-sky">
                <div class="panel-heading">
                  <h4>Cadastro de Registro de Calendário</h4>
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
                        <th>Claúsula</th>
                        <th>Assunto</th>
                        <th>Data da ocorrência</th>
                        <th>Recorrência</th>
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

  <script type='text/javascript' src="./js/calendariosindicato.min.js"></script>

</body>

</html>