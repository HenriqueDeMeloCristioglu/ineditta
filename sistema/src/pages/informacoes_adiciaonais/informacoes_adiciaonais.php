<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

$userSession = $_SESSION['login'];

/**
 * @author    {J. vinicio}
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

$fileInfo_adicionais = $path . '/includes/php/class.informacoes_adiciaonais.php';

if (file_exists($fileInfo_adicionais)) {

  include_once($fileInfo_adicionais);
  include_once __DIR__ . "/includes/php/class.usuario.php";

  $info_adicionais = new informacoes_adiciaonais();

  if ($info_adicionais->response['response_status']['status'] == 1) {


    $getNote = $info_adicionais->connectdb();
    if ($getNote['response_status']['status'] == 1) {

      $listaPatronal = $getNote['response_data']['sindPatr'];
      $listaLaboral = $getNote['response_data']['sindEmp'];
      $listaClausula = $getNote['response_data']['listaPrincipal'];
      $listaGrupo = $getNote['response_data']['listaGrupo'];
      $listaMatriz = $getNote['response_data']['listaMatriz'];
      $listaUnidade = $getNote['response_data']['listaUnidade'];

      $listaPrincipal = $getNote['response_data']['listaPrincipalNotif'];
    } else {

      $response['response_status']['status'] = 0;
      $response['response_status']['error_code'] = $error_code . __LINE__;
      $response['response_status']['msg'] = $info_adicionais['response_status']['error_code'] . '::' . $info_adicionais['response_status']['msg'];
    }
  } else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = $info_adicionais->response['response_status']['error_code'] . '::' . $info_adicionais->response['response_status']['msg'];
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
  <link href="includes/plugins/select2/select2-4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />

  <!-- The following CSS are included as plugins and can be removed if unused-->
  <script src="includes/js/jquery-3.4.1.min.js"></script>
  <link rel='stylesheet' type='text/css' href='includes/plugins/form-toggle/toggles.css' />
  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

  <!-- CSS datagrid -->
  <link rel="stylesheet" href="includes/plugins/datagrid/styles/jplist-custom.css">

  <script src="keycloak.js"></script>

  <style>
    .select2-container {
      width: 100% !important;
      /* or any value that fits your needs */
    }

    .swal2-select {
      display: none !important;
    }

    .dp-none {
      display: none;
    }

    #page-content {
      min-height: 100% !important;
    }

    :is(.view_box_txt, .view_box_nome, .view_box_info) {
      width: 100%;
      height: 400px;
      border: 1px solid #000;
      overflow-y: scroll;
    }
  </style>
</head>

<body onload="initKeycloak()" class="horizontal-nav"> <!-- onload="initKeycloak()" -->

  <?php include('menu.php'); ?>

  <div class="page-container">
    <div id="page-content" style="min-height: 100%; padding-bottom: 0;">
      <!-- <div> -->
      <div id="wrap">
        <div class="container" style="padding-bottom: 0;">
          <div class="row" style="display: flex;">
            <!-- <div class="col-md-1 img_container">
              <div class="container_logo_client">
                <img id="imglogo" class="img-circle">
              </div>
            </div> -->

            <div class="col-md-12 content_container" style="width: 100% !important;">
              <div class="panel panel-primary" style="margin-top: 0;">
                <form class="form-horizontal">
                  <div class="panel panel-primary" style="margin: 0;">
                    <div class="panel-heading">
                      <h4>Cadastro de Informações Adicionais</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in principal-table">
                      <div class="form-group">
                        <div class="col-sm-4">
                          <label for="doc_sind">Documento Sindical</label>
                          <select id="doc_sind" class="form-control select2">
                            <option value="--">--</option>
                          </select>
                        </div>

                        <div class="col-sm-2">
                          <label for="vigencia_inicial">Vigência Inicial</label>
                          <input type="date" id="vigencia_inicial" class="form-control">
                        </div>

                        <div class="col-sm-2">
                          <label for="vigencia_final">Vigência Final</label>
                          <input type="date" id="vigencia_final" class="form-control">
                        </div>

                        <div class="col-sm-4">
                          <label for="classificacao">Classificação</label>
                          <select id="classificacao" class="form-control select2">
                            <option value="--">--</option>
                          </select>
                        </div>
                      </div>

                      <div class="form-group center">
                        <div class="col-md-4">
                          <label for="txt_clausula">Texto da Cláusula</label>
                          <div class="view_box_txt"></div>
                        </div>

                        <div class="col-md-4">
                          <label for="nome_variavel">Nome Variável</label>
                          <div class="view_box_nome"></div>
                        </div>

                        <div class="col-md-4">
                          <label for="info_add">Informações Adicionais</label>
                          <div class="view_box_info"></div>
                        </div>
                      </div>

                      <div class="form-group" style="margin-bottom: 0;">
                        <div class="col-sm-12" style="display: flex; justify-content: space-between;">
                          <div class="block_1" style="display:flex; gap: 3px;">
                            <button type="button" class="btn btn-primary btn-rounded" onclick="">Selecionar
                              Tudo</button>
                            <button type="button" class="btn btn-primary btn-rounded" onclick="">Limpar Seleção</button>
                          </div>

                          <div class="block_2" style="display:flex; gap: 3px;">
                            <button type="button" class="btn btn-primary btn-rounded" onclick="">Gravar</button>
                            <button type="button" class="btn btn-primary btn-rounded" onclick="">Visualizar</button>
                            <button type="button" class="btn btn-danger btn-rounded" onclick="">Finalizar</button>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>

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
  <script type='text/javascript' src='includes/plugins/jquery-editable/jquery.editable.min.js'></script>
  <script type='text/javascript' src='includes/plugins/form-validation/jquery.validate.min.js'></script>
  <script type='text/javascript' src='includes/plugins/form-stepy/jquery.stepy.js'></script>
  <script type='text/javascript' src='includes/demo/demo-formwizard.js'></script>
  <script type='text/javascript' src='includes/js/placeholdr.js'></script>
  <script type='text/javascript' src='includes/demo/demo-modals.js'></script>
  <script type='text/javascript' src='includes/js/application.js'></script>
  <script type='text/javascript' src='includes/demo/demo.js'></script>
  <script type='text/javascript'
    src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.15/jquery.mask.min.js"></script>

  <script src="includes/plugins/select2/select2-4.1.0-rc.0/dist/js/select2.min.js"></script>


  <script src="includes/plugins/edited-datatable/jquery.dataTables.min.js"></script>
  <script src="includes/plugins/edited-datatable/datatables-bs4/js/dataTables.bootstrap4.min.js"></script>
  <script src="includes/plugins/edited-datatable/datatables-responsive/js/dataTables.responsive.min.js"></script>
  <script src="includes/plugins/edited-datatable/datatables-responsive/js/responsive.bootstrap4.min.js"></script>

  <script src="includes/plugins/sweet-alert/all.js"></script>
  <script type='text/javascript' src="includes/js/informacoes_adiciaonais.js"></script>

  <!-- new datagrid -->
  <script src="includes/plugins/datagrid/script/jquery.metisMenu.js"></script>
  <script src="includes/plugins/datagrid/script/jquery.slimscroll.js"></script>
  <script src="includes/plugins/datagrid/script/jquery.flot.js"></script>
  <script src="includes/plugins/datagrid/script/jquery.flot.categories.js"></script>
  <script src="includes/plugins/datagrid/script/jquery.flot.pie.js"></script>
  <script src="includes/plugins/datagrid/script/jquery.flot.tooltip.js"></script>
  <script src="includes/plugins/datagrid/script/jquery.flot.resize.js"></script>
  <script src="includes/plugins/datagrid/script/jquery.flot.fillbetween.js"></script>
  <script src="includes/plugins/datagrid/script/jquery.flot.stack.js"></script>
  <script src="includes/plugins/datagrid/script/jquery.flot.spline.js"></script>
  <script src="includes/plugins/datagrid/script/jplist.min.js"></script>
  <script src="includes/plugins/datagrid/script/jplist.js"></script>
  <script src="includes/plugins/datagrid/script/main.js"></script>
</body>

</html>