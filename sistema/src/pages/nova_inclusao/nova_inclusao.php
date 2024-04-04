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

$fileInclusao = $path . '/includes/php/class.nova_inclusao.php';

if (file_exists($fileInclusao)) {

  include_once($fileInclusao);
  include_once __DIR__ . "/includes/php/class.usuario.php";

  $Inclusao = new inclusao();

  if ($Inclusao->response['response_status']['status'] == 1) {


    $getNote = $Inclusao->getLists();
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
      $response['response_status']['msg'] = $Inclusao['response_status']['error_code'] . '::' . $Inclusao['response_status']['msg'];
    }
  } else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = $Inclusao->response['response_status']['error_code'] . '::' . $Inclusao->response['response_status']['msg'];
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

    #mensagem_sucesso,
    #msg-sucesso,
    #msg-sucesso-clau,
    #msg-sucesso-clau-update {
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

    .scroll-none {
      scrollbar-width: none;
      max-height: 50vh;
      overflow-y: scroll
    }

    #cabecalho {
      position: sticky;
      top: 0;
      background-color: #fff;
      border-bottom: 1px solid #bbb;
      z-index: 10;
    }

    .title-group-table {
      padding: 16px 0;
    }

    #topo {
      position: sticky;
      top: 0;
      background-color: #fff;
      z-index: 10;
    }

    .fixTableHead {
      padding: 0px 20px 20px 20px !important;
      scrollbar-width: none;
      max-height: 35vh;
      overflow-y: scroll;
      border-collapse: separate;
    }

    .scroll-true {
      border-collapse: separate;
    }

    .info-adicional,
    .info-adicionalUpdate {
      resize: vertical;
    }

    .dp-none {
      display: none;
    }

    #page-content {
      min-height: 100% !important;
    }
  </style>
</head>

<body onload="initKeycloak()" class="horizontal-nav"> <!-- onload="initKeycloak()" -->

  <?php include('menu.php'); ?>

  <div class="page-container">
    <div id="page-content" style="min-height: 100%;">
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
                        <td>
                          <a data-toggle="modal" href="#myModal" id="novo" class="btn default-alt ">Novo Acompanhamento</a>
                        </td>
                      </tr>
                    </tbody>
                  </table>



                  <!-- ACOMPANHAMENTO -->
                  <div class="panel panel-primary">
                    <div class="panel-heading">
                      <h4>Acompanhamentos</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in principal-table">
                      <div id="grid-layout-table-1" class="box jplist">
                        <div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
                        <div class="jplist-panel box panel-top" style="display: flex; flex-direction: column;">
                          <div class="row" style="display: flex; justify-content: center;">
                            <button type="button" data-control-type="reset" data-control-name="reset" data-control-action="reset" class="jplist-reset-btn btn btn-primary">Limpar <i class="fa fa-share mls"></i></button>
                            <div data-control-type="drop-down" data-control-name="paging" data-control-action="paging" class="jplist-drop-down form-control">
                              <ul class="dropdown-menu">
                                <li><span data-number="3"> 3 por página</span></li>
                                <li><span data-number="5" data-default="true"> 5 por página</span></li>
                              </ul>
                            </div>
                            <div data-control-type="drop-down" data-control-name="sort" data-control-action="sort" data-datetime-format="{month}/{day}/{year}" class="jplist-drop-down form-control">
                              <ul class="dropdown-menu">
                                <li><span data-path="default">Listar por</span></li>
                                <li><span data-path=".title" data-order="asc" data-type="text">Tipo Com. A-Z</span></li>
                                <li><span data-path=".title" data-order="desc" data-type="text">Tipo Com. Z-A</span></li>
                                <li><span data-path=".desc" data-order="asc" data-type="text">Tipo Usuario A-Z</span></li>
                                <li><span data-path=".desc" data-order="desc" data-type="text">Tipo Usuario Z-A</span></li>
                              </ul>
                            </div>
                            <div data-type="Página {current} de {pages}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
                          </div>

                          <div class="row" style="display: flex; justify-content: center;">
                            <div class="text-filter-box">
                              <div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".title" type="text" value="" placeholder="Filtrar por Fase" data-control-type="textbox" data-control-name="title-filter" data-control-action="filter" class="form-control" /></div>
                            </div>
                            <div class="text-filter-box">
                              <div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Responsável" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control" /></div>
                            </div>
                            <div class="text-filter-box">
                              <div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Sind. Laboral" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control" /></div>
                            </div>
                            <div class="text-filter-box">
                              <div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Sind. Patronal" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control" /></div>
                            </div>
                            <div class="text-filter-box">
                              <div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Filtrar por Status" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control" /></div>
                            </div>
                          </div>

                          <!-- <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" class="jplist-pagination"></div> -->
                        </div>
                        <div class="box text-shadow">
                          <table class="table table-striped table-bordered demo-tbl">
                            <thead>
                              <tr>
                                <th></th>
                                <th>Fase</th>
                                <th>Responsável</th>
                                <th>Sind. Laboral</th>
                                <th>Sind. Patronal</th>
                                <th>Data-base</th>
                                <th>Propriedade</th>
                                <th>Última Atualização</th>
                                <th>Nome do Documento</th>
                              </tr>
                            </thead>
                            <tbody>
                              <?= $listaPrincipal ?>
                            </tbody>
                          </table>
                        </div>
                        <div class="box jplist-no-results text-shadow align-center">
                          <p>Nenhum resultado encontrado</p>
                        </div>
                        <div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
                        <div class="jplist-panel box panel-bottom">
                          <div data-type="{start} - {end} de {all}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
                          <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" data-control-animate-to-top="true" class="jplist-pagination"></div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- INSERIR ACOMPANHAMENTO -->
    <div class="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Acompanhamento CCT</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <form class="form-horizontal">
                <div class="panel panel-primary" style="margin: 0;">
                  <div class="panel-heading">
                    <h4>Cadastro de Acompanhamento CCT</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in principal-table">
                    <div class="form-group center">
                      <div class="col-sm-2">
                        <label for="add_propri">Propriedade</label>
                        <input type="text" id="add_propri" class="form-control">
                      </div>

                      <div class="col-sm-4">
                        <label for="add_fases">Fases</label>
                        <input type="text" id="add_fases" class="form-control">
                      </div>

                      <div class="col-sm-2">
                        <label for="add_data_base">Data-base</label>
                        <input type="date" id="add_data_base" class="form-control">
                      </div>

                      <div class="col-sm-2">
                        <label for="add_data_inicio">Data Início</label>
                        <input type="date" id="add_data_inicio" class="form-control">
                      </div>

                      <div class="col-sm-2">
                        <label for="add_data_fim">Data FIm (Previsto)</label>
                        <input type="date" id="add_data_fim" class="form-control">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-6">
                        <label for="add_sind_laboral">Sindicato Laboral</label>
                        <select id="add_sind_laboral" class="form-control select2">
                          <option value="--">--</option>
                        </select>
                      </div>

                      <div class="col-md-6">
                        <label for="add_sind_patronal">Sindicato Patronal</label>
                        <select id="add_sind_patronal" class="form-control select2">
                          <option value="--">--</option>
                        </select>
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-6">
                        <label for="add_sind_laboral_2">Sindicato Laboral (Adicionais)</label>
                        <select id="add_sind_laboral_2" class="form-control select2">
                          <option value="--">--</option>
                        </select>
                      </div>

                      <div class="col-md-6">
                        <label for="add_sind_patronal_2">Sindicato Patronal (Adicionais)</label>
                        <select id="add_sind_patronal_2" class="form-control select2">
                          <option value="--">--</option>
                        </select>
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-6">
                        <label for="add_responsavel">Responsável</label>
                        <select id="add_responsavel" class="form-control select2">
                          <option value="--">--</option>
                        </select>
                      </div>

                      <div class="col-md-6">
                        <label for="add_atv_economica">Atividade Econômica</label>
                        <select id="add_atv_economica" class="form-control select2">
                          <option value="--">--</option>
                        </select>
                      </div>
                    </div>
                  </div>
                </div>
              </form>
            </div>
          </div>
          <div class="modal-footer">
            <div class="row">
              <div class="col-lg-12">
                <div class="btn-toolbar" style="display: flex; justify-content:center;">
                  <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;" onclick="addInclusao();">Processar</button>
                  <button id="btn-cancelar" type="button" class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ADICIONAR SCRIPT -->
    <div class="modal fade" id="myModalUpdate" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Acompanhamento CCT</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <form class="form-horizontal">
                <input type="hidden" id="up-hidden">
                <input type="hidden" id="id_note">
                <div class="panel panel-primary" style="margin-top: 0;">
                  <div class="panel-heading">
                    <h4>Adicionar Script</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in principal-table">
                    <div class="form-group center">
                      <div class="col-sm-2">
                        <label for="add_propri">Propriedade</label>
                        <input type="text" id="add_propri" class="form-control">
                      </div>

                      <div class="col-sm-4">
                        <label for="add_fases">Fases</label>
                        <input type="text" id="add_fases" class="form-control">
                      </div>

                      <div class="col-sm-2">
                        <label for="add_data_base">Data-base</label>
                        <input type="date" id="add_data_base" class="form-control">
                      </div>

                      <div class="col-sm-2">
                        <label for="add_data_inicio">Data Início</label>
                        <input type="date" id="add_data_inicio" class="form-control">
                      </div>

                      <div class="col-sm-2">
                        <label for="add_data_fim">Data FIm (Previsto)</label>
                        <input type="date" id="add_data_fim" class="form-control">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-6">
                        <label for="destino_up">Sindicato Laboral</label>
                        <input type="text" id="sind_laboral_name" class="form-control" placeholder="Nome do Sindicato">
                      </div>

                      <div class="col-md-6">
                        <label for="campo_destino_up">Sindicato Patronal</label>
                        <input type="text" id="sind_patronal_name" class="form-control" placeholder="Nome do Sindicato">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-sm-6">
                        <input type="text" id="sind_laboral_email" class="form-control" placeholder="E-mails">
                      </div>

                      <div class="col-md-6">
                        <input type="text" id="sind_patronal_email" class="form-control" placeholder="E-mails">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-sm-3">
                        <input type="text" id="sind_laboral_telefone_1" class="form-control" placeholder="Telefone">
                      </div>

                      <div class="col-md-3">
                        <input type="text" id="sind_patronal_telefone_2" class="form-control" placeholder="Telefone">
                      </div>

                      <div class="col-sm-3">
                        <input type="text" id="sind_laboral_telefone_1" class="form-control" placeholder="Telefone">
                      </div>

                      <div class="col-md-3">
                        <input type="text" id="sind_patronal_telefone_2" class="form-control" placeholder="Telefone">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-sm-6">
                        <input type="text" id="sind_laboral_link_site" class="form-control" placeholder="Link site">
                      </div>

                      <div class="col-md-6">
                        <input type="text" id="sind_patronal_link_site" class="form-control" placeholder="Link site">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-6">
                        <label for="destino_up">Sindicato Laboral (Adicionais)</label>
                        <input type="text" id="sind_laboral_name_adicional" class="form-control" placeholder="Nome do Sindicato">
                      </div>

                      <div class="col-md-6">
                        <label for="campo_destino_up">Sindicato Patronal (Adicionais)</label>
                        <input type="text" id="sind_patronal_name_adicional" class="form-control" placeholder="Nome do Sindicato">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-6">
                        <label for="responsavel">Responsavel</label>
                        <input type="text" id="responsavel" class="form-control">
                      </div>

                      <div class="col-md-6">
                        <label for="ultima_atualizacao">Última Atualização</label>
                        <input type="text" id="ultima_atualizacao" class="form-control">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-4">
                        <button type="button" class="btn btn-primary btn-rounded" style="width: 100%;">HIstoríco de Acompanhamento</button>
                      </div>

                      <div class="col-md-4">
                        <button type="button" class="btn btn-primary btn-rounded" style="width: 100%;">Adicionar Script</button>
                      </div>

                      <div class="col-md-4">
                        <a data-toggle="modal" href="#sendEmailModal" data-dismiss="modal"><button type="button" class="btn btn-primary btn-rounded" style="width: 100%;">Mandar e-mail</button></a>
                      </div>
                    </div>

                    <div class="form-group center" style="padding: 0px 10px; margin-bottom: 0;">
                      <div class="panel panel-primary" style="margin: 0;">
                        <div class="panel panel-primary" style="margin: 0;">
                          <div class="panel-heading">
                            <h4 style="width: 97%; text-align: center; padding-left: 40px;">Questionário da Fase</h4>
                            <div class="options">
                              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                            </div>
                          </div>

                          <textarea class="form-control" id="" cols="30" rows="10"></textarea>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="row">
                  <div class="col-lg-12">
                    <div class="btn-toolbar" style="display: flex; justify-content:center;">
                      <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;" onclick="updateInclusao();">Salvar Questinário</button>
                      <button id="btn-cancelar" type="button" class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
                    </div>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ENVIAR EMAIL -->
    <div class="modal fade" id="sendEmailModal" role="dialog" aria-labelledby="sendEmailModal" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Acompanhamento CCT</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <form class="form-horizontal">
                <div class="panel panel-primary" style="margin: 0;">
                  <div class="panel-heading">
                    <h4>Enviar e-mail</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in principal-table">
                    <div class="form-group center">
                      <div class="col-sm-12">
                        <label for="destino_email">Para:</label>
                        <input type="text" id="destino_email" class="form-control">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-12">
                        <label for="subject_email">Assunto:</label>
                        <input type="text" id="subject_email" class="form-control">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-12">
                        <label for="template_email">Template:</label>
                        <input type="text" id="template_email" class="form-control">
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-12">
                        <label for="message">Mensagem:</label>
                        <textarea id="message" class="form-control" cols="30" rows="10"></textarea>
                      </div>
                    </div>

                    <div class="form-group center" style="margin-bottom: 0;">
                      <div class="col-md-4">
                        <button type="button" class="btn btn-primary btn-rounded" style="width: 100%;">Enviar Email</button>
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
  <script type='text/javascript' src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.15/jquery.mask.min.js"></script>

  <script src="includes/plugins/select2/select2-4.1.0-rc.0/dist/js/select2.min.js"></script>


  <script src="includes/plugins/edited-datatable/jquery.dataTables.min.js"></script>
  <script src="includes/plugins/edited-datatable/datatables-bs4/js/dataTables.bootstrap4.min.js"></script>
  <script src="includes/plugins/edited-datatable/datatables-responsive/js/dataTables.responsive.min.js"></script>
  <script src="includes/plugins/edited-datatable/datatables-responsive/js/responsive.bootstrap4.min.js"></script>

  <script src="includes/plugins/sweet-alert/all.js"></script>
  <script type='text/javascript' src="includes/js/nova_inclusao.js"></script>

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