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


$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.'));

// Montando o c??o do erro que ser?presentado
$localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar = array("\\", "/includes/php");
$substituir = array("/", "");
$path = str_replace($localizar, $substituir, __DIR__);

$fileClassTarefa = $path . '/includes/php/class.tarefas_sindicais.php';
$fileClassCltUn = $path . '/includes/php/class.clienteunidade.php';

if (file_exists($fileClassTarefa) && file_exists($fileClassCltUn)) {

  include_once($fileClassTarefa);

  include_once($fileClassCltUn);

  $docsind = new tarefas_sindicais();

  $cltUn = new clienteunidade();

  if ($docsind->response['response_status']['status'] == 1) {

    $getDoc = $docsind->getTarefa();

    $getCltUn = $cltUn->getClienteUnidade();

    $clienteUnidade = $getCltUn['response_data']['selectCltUn'];

    if ($getDoc['response_status']['status'] == 1) {

      $lista = $getDoc['response_data']['html'];
    } else {
      $response['response_status']['status'] = 0;
      $response['response_status']['error_code'] = $error_code . __LINE__;
      $response['response_status']['msg'] = $getDoc['response_status']['error_code'] . '::' . $getDoc['response_status']['msg'];
    }

    $getDocSindCampos = $docsind->getTarefasCampos();

    if ($getDocSindCampos['response_status']['status'] == 1) {

      $listaCnae = $getDocSindCampos['response_data']['listaCnae'];
      $listaSindEmp = $getDocSindCampos['response_data']['listaSindEmp'];
      $listaPatronal = $getDocSindCampos['response_data']['listaPatronal'];
      $local = $getDocSindCampos['response_data']['local'];
      $unidadeCliente = $getDocSindCampos['response_data']['unidadeCliente'];
      $optTipoDoc = $getDocSindCampos['response_data']['tipoDoc'];
      $listaArquivosDoc = $getDocSindCampos['response_data']['listaArquivosDoc'];
      $referenceList = $getDocSindCampos['response_data']['referenceList'];
      $listaEstruturaClausula = $getDocSindCampos['response_data']['listaEstruturaClausula'];
    } else {
      $response['response_status']['status'] = 0;
      $response['response_status']['error_code'] = $error_code . __LINE__;
      $response['response_status']['msg'] = $getDocSindCampos['response_status']['error_code'] . '::' . $getDocSindCampos['response_status']['msg'];
    }
  } else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = $clienteusuarios->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
  }
} else {
  $response['response_status']['status'] = 0;
  $response['response_status']['error_code'] = $error_code . __LINE__;
  $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.clienteusuarios).';
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

  <link rel="stylesheet" href="tarefas_sindicais.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    .btn-select {
      margin-top: 7px;
      width: 100%;
    }

    .fa-map-marker {
      margin-right: 10px;
    }

    .btn-box {
      margin-top: 30px;
    }

    #page-content {
      min-height: 100vh !important;
    }

    .btn-documents {
      border-radius: 50%;
      width: 32px;
      height: 32px;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 0;
      font-size: 30px;
    }

    #embed,
    #embed_register,
    #embed_update {
      height: 0px;
    }

    #checkbox_group {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 10px;
    }

    #checkbox_group label {
      transform: translateY(6px);
    }

    .form-group.buttons .row {
      display: flex;
      align-items: center;
    }

    .form-group.buttons .row a.btn.btn-primary {
      width: 200px;
      padding: 10px 0px;
      font-size: 15px;
    }

    form .row {
      padding: 0px 10px;
    }

    .table_style table tr th {
      text-align: center;
    }

    .bg-primary th {
      text-align: center;
    }

    #add_task form {
      margin-bottom: 92px;
    }

    #table_big #table-principal td:nth-child(1),
    #table_big #table-principal td:nth-child(10) {
      text-align: center;
    }

    #comments {
      max-height: 100%;
    }

    /* icon comment */
    .fa-solid.fa-comment {
      background: #4f8edc;
      padding: 5px 23px;
      color: #fff;
    }

    #page-content {
      min-height: 100% !important;
    }
  </style>
</head>

<body class="horizontal-nav">

  <?php include('menu.php'); ?>

  <div class="page-container">
    <div id="pageCtn">
      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Tarefas Sindicais</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <div id="grid-layout-table-2" class="box jplist">
                    <div class="jplist-panel" style="margin-bottom: 20px;">
                      <button type="button" class="btn btn-primary" data-toggle="modal"
                        data-target="#addTaskModal">Incluir Tarefa</button>
                    </div>
                    <div class="box text-shadow table_style">
                      <table cellpadding="0" cellspacing="0" border="0" class=" table table-bordered demo-tbl">
                        <thead>
                          <tr class="bg-primary">
                            <th></th>
                            <th>Tarefa</th>
                            <th>Assunto</th>
                            <th>Data Abertura</th>
                            <th>Data Inicio</th>
                            <th>Data Fim</th>
                            <th>Data Evento</th>
                            <th>Status</th>
                          </tr>
                        </thead>
                        <tbody>
                          <tr>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div> <!-- container -->
      </div> <!-- #wrap -->

      <!-- MODAL INCLUIR TAREFA -->
      <div class="hidden" id="addTaskModalHidden">
        <div id="addTaskModalContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Incluir Tarefa</h4>
          </div>
          <div class="modal-body">

            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Tarefa</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>

              <div class="panel-body collapse in">
                <form method="post">
                  <div class="form-group" style="margin-bottom: 20px;">
                    <div class="col-sm-6" style="padding-left: 0;">
                      <label>Nome da Tarefa</label>
                      <input type="text" id="name_task" class="form-control">
                    </div>
                    <div class="col-sm-6" style="padding-right: 0;">
                      <label>Assunto</label>
                      <input type="text" id="subject" class="form-control">
                    </div>
                  </div>
                </form>

                <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables"
                  id="table_small">
                  <thead>
                    <tr class="bg-primary">
                      <th>Data Inicio</th>
                      <th>Data Fim</th>
                      <th>Data Evento</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody id="table-principal">
                    <tr>
                      <td></td>
                      <td></td>
                      <td></td>
                      <td></td>
                    </tr>
                  </tbody>
                </table>

                <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables"
                  id="table_big">
                  <thead>
                    <tr class="bg-primary">
                      <th></th>
                      <th>Atividade</th>
                      <th>Data inicio</th>
                      <th>Data Fim</th>
                      <th>Data Evento</th>
                      <th>Status</th>
                      <th>Alerta</th>
                      <th>Recorrência</th>
                      <th>Up_load documentos</th>
                      <th>Comentários</th>
                    </tr>
                  </thead>
                  <tbody id="table-principal">
                    <tr>
                      <td><a href=""><i class="fa-solid fa-plus"></i></a></td>
                      <td></td>
                      <td></td>
                      <td></td>
                      <td></td>
                      <td></td>
                      <td></td>
                      <td></td>
                      <td></td>
                      <td>
                        <button type="button" class="btn default-alt" data-toggle="modal"
                          data-target="#comentarioModal"></button>
                      </td>
                    </tr>
                  </tbody>
                </table>

                <div id="btn-toolbar" style="display: flex; justify-content:center;">
                  <button type="button" style="margin-right: 2.5px;" class="btn btn-primary btn-rounded">Incluir</a>
                </div>
              </div>

              <div class="modal-footer">
                <button data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- MODAL COMENTARIOS -->
      <div class="hidden" id="comentarioModalHidden">
        <div id="comentarioModalContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Comentário</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4></h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables"
                  id="table_big">
                  <thead>
                    <tr class="bg-primary">
                      <th></th>
                      <th>Data</th>
                      <th>Comentário</th>
                    </tr>
                  </thead>
                  <tbody id="table-principal">
                    <tr>
                      <td><a href=""><i class="fa-solid fa-plus"></i></a></td>
                      <td></td>
                      <td></td>
                    </tr>
                  </tbody>
                </table>

                <div id="btn-toolbar" style="display: flex; justify-content:center;">
                  <a href="#" style="margin-right: 2.5px;" class="btn btn-primary btn-rounded">Processar</a>
                  <!-- addDocSind() -->
                  <a href="#" style="margin-left: 2.5px;" class="btn btn-danger btn-rounded btn-cancelar">Finalizar</a>
                </div>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
              data-dismiss="modal">Voltar</button>
          </div>
        </div>
      </div>

    </div> <!-- page-content -->

    <?php include 'footer.php' ?>

  </div> <!-- page-container -->

  <script type='text/javascript' src="./js/tarefas_sindicais.min.js"></script>
</body>

</html>