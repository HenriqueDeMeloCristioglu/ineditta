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

  <link rel="stylesheet" href="negociacao.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    .select2-container {
      width: 100% !important;
      /* or any value that fits your needs */
    }

    .swal2-select {
      display: none !important;
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

    /*/////////////////*/
    .neg-dark {
      display: flex;
      gap: 10px;
    }

    .neg-dark :is(.col-md-2, .col-md-4) {
      text-align: center;
      background-color: #212529;
      padding: 0;
      color: #fff;
    }

    .neg-dark :is(.col-md-2 label, .col-md-4 label) {
      margin-bottom: 0;
      padding: 5px 0px;
    }

    .neg-dark :is(.col-md-2 .form-control, .col-md-4 .form-control) {
      padding: 10px;
      height: auto;
      text-align: center;
      background: #eee;
    }

    .neg-dark .col-md-2 input[type='date'] {
      height: 42px;
    }

    #editarNeg :is(.tab-script, .tab-pauta, .view_pauta, .view_comparar_pauta, .tab-dirigentes, .tab-premissas, .tab-calculadora) {
      overflow: hidden;
      height: 0;
      transition: .3s;
    }

    #editarNeg :is(.tab-script.open, .tab-pauta.open, .view_pauta.open, .view_comparar_pauta.open, .tab-dirigentes.open, .tab-premissas.open, .tab-calculadora.open) {
      height: auto;
    }

    #page-content {
      min-height: 100% !important;
    }
  </style>
</head>

<body class="horizontal-nav">

  <?php include('menu.php'); ?>

  <div class="page-container">
    <div id="pageCtn" style="min-height: 100%;">
      <!-- <div> -->
      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-11 content_container">
              <div class="row">
                <div class="col-md-12">
                  <table class="table table-striped">
                    <tbody>
                      <tr>
                        <td>
                          <button type="button" class="btn default-alt " data-toggle="modal" data-target="#acompanhamentoCctModal" id="acompanhamentoCctModalBtn">Acompanhamento CC</button>
                          <button type="button" class="btn default-alt " data-toggle="modal" data-target="#negociacaoActModal" id="negociacaoActModalBtn">Negociação ACT</button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>

          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <!-- NEGOCIAÇÕES -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Negociações</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div id="grid-layout-table-1" class="box jplist">
                      <div data-control-type="drop-down" data-control-name="sort" data-control-action="sort" data-datetime-format="{month}/{day}/{year}" class="jplist-drop-down form-control">
                        <ul class="dropdown-menu">
                          <li><span data-path="default">Listar por</span></li>
                          <li><span data-path=".title" data-order="asc" data-type="text">Tipo Com. A-Z</span></li>
                          <li><span data-path=".title" data-order="desc" data-type="text">Tipo Com. Z-A</span></li>
                          <li><span data-path=".desc" data-order="asc" data-type="text">Tipo Usuario A-Z</span></li>
                          <li><span data-path=".desc" data-order="desc" data-type="text">Tipo Usuario Z-A</span></li>
                        </ul>
                      </div>
                    </div>
                    <div class="box text-shadow">
                      <table class="table table-striped table-bordered demo-tbl">
                        <thead>
                          <tr>
                            <th>Id</th>
                            <th>Empresa</th>
                            <th>Sind. Patronal</th>
                            <th>Sind. Laboral</th>
                            <th>Data-base</th>
                            <th>Tipo negociacao</th>
                            <th>Fase</th>
                            <th>Usuário</th>
                          </tr>
                        </thead>
                        <tbody>
                          <!-- $listaPrincipal -->
                          <tr class="tbl-item">
                            <td style="text-align: center; width: 120px;">
                              <button type="button" class="btn btn-primary title" style="color: #fff;" data-toggle="modal" data-target="#editarAcompanhamentoModal">Selec.</button>
                            </td>
                            <td class="desc">--</td>
                            <td>--</td>
                            <td>--</td>
                            <td>--</td>
                            <td>--</td>
                            <td>--</td>
                            <td>--</td>
                          </tr>
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

    <!-- ACOMPANHAMENTO CCT -->
    <div class="hidden" id="acompanhamentoCctHidden">
      <div id="acompanhamentoCctContent">
        <div class="modal-body" style="padding-top: 25px;">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true" style="transform: translateY(-14px);">&times;</button>
          <div class="panel panel-primary">
            <form class="form-horizontal">
              <div class="panel panel-primary" style="margin: 0;">
                <div class="panel-heading">
                  <h4>Negociações em Acompanhamento</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row">
                    <table class="table table-striped table-bordered demo-tbl">
                      <thead>
                        <tr style="text-align: center;">
                          <th>Id</th>
                          <th>Sind. Patronal</th>
                          <th>Sind. Laboral</th>
                          <th>Data-base</th>
                          <th>Tipo negociacao</th>
                          <th>Fase</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr>
                          <td style="text-align: center; width: 120px;">
                            <button type="button" class="btn default-alt " data-toggle="modal" data-target="#selectAcompanhamentoModal">Selec.</button>
                          <td>--</td>
                          <td>--</td>
                          <td>--</td>
                          <td>--</td>
                          <td>--</td>
                        </tr>
                      </tbody>
                    </table>
                  </div>

                  <div class="row" style="text-align: end;">
                    <button data-dismiss="modal" type="button" class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
                  </div>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- SELECT ACOMPANHAMENTO -->
    <div class="hidden" id="selectAcompanhamentoHidden">
      <div id="selectAcompanhamentoContent">
        <div class="modal-body" style="padding-top: 25px;">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true" style="transform: translateY(-14px);">&times;</button>
          <div class="panel panel-primary">
            <form class="form-horizontal">
              <div class="panel panel-primary" style="margin-top: 0;">
                <div class="panel-heading">
                  <h4></h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <table class="table table-striped table-bordered demo-tbl">
                    <thead>
                      <tr>
                        <th></th>
                        <th>Matriz</th>
                        <th>Filial</th>
                      </tr>
                    </thead>
                    <tbody>
                      <!-- $listaPrincipal -->
                      <tr>
                        <td style="text-align: center; width: 80px;"><input type="checkbox" name="selectACT" id="selectACT"></td>
                        <td>--</td>
                        <td>--</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              <div class="row">
                <div class="col-lg-12">
                  <div class="btn-toolbar" style="display: flex; justify-content:center;">
                    <button type="button" class="btn btn-primary btn-rounded">Selecionar</button>
                  </div>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- EDITAR NEGOCIAÇÃO -->
    <div class="hidden" id="editarAcompanhamentoHidden">
      <div id="editarAcompanhamentoContent">
        <div class="modal-body" style="padding-top: 25px;">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true" style="transform: translateY(-14px);">&times;</button>
          <div class="panel panel-primary">
            <form class="form-horizontal">
              <div class="panel panel-primary" style="margin-top: 0;">
                <div class="panel-heading">
                  <h4></h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <form class="form-horizontal">
                    <div class="form-group center neg-dark">
                      <div class="col-md-2">
                        <label for="tipo_neg_script">Tipo Negociação</label>
                        <input type="text" name="tipo_neg_script" id="tipo_neg_script" class="form-control">
                      </div>
                      <div class="col-md-2">
                        <label for="tipo_acordo_script">Tipo de Acordo</label>
                        <input type="text" name="tipo_acordo_script" id="tipo_acordo_script" class="form-control">
                      </div>
                      <div class="col-md-2">
                        <label for="tempo_neg_script">Tempo da Negociação</label>
                        <input type="text" name="tempo_neg_script" id="tempo_neg_script" class="form-control">
                      </div>
                      <div class="col-md-2">
                        <label for="num_rodadas_script">Número de Rodadas</label>
                        <input type="text" name="num_rodadas_script" id="num_rodadas_script" class="form-control">
                      </div>
                      <div class="col-md-2">
                        <label for="reajuste_proposto_script">Reajusto Proposto</label>
                        <input type="text" name="reajuste_proposto_script" id="reajuste_proposto_script" class="form-control">
                      </div>
                      <div class="col-md-2">
                        <label for="inpc_acumulado_script">INPC Acumulado</label>
                        <input type="date" name="inpc_acumulado_script" id="inpc_acumulado_script" class="form-control">
                      </div>
                    </div>
  
                    <div class="form-group center neg-dark" style="margin-bottom: 0;">
                      <div class="col-md-4">
                        <label for="sind_patronal_script">Sind. Patronal</label>
                        <input type="text" name="sind_patronal_script" id="sind_patronal_script" class="form-control">
                      </div>
                      <div class="col-md-4">
                        <label for="sind_laboral_script">Sind. Laboral</label>
                        <input type="text" name="sind_laboral_script" id="sind_laboral_script" class="form-control">
                      </div>
                      <div class="col-md-4">
                        <label for="empresa_script">Empresa</label>
                        <input type="text" name="empresa_script" id="empresa_script" class="form-control">
                      </div>
                    </div>
                  </form>
                </div>
              </div>
  
              <div class="row">
                <div class="col-lg-12">
                  <div class="btn-toolbar" style="display: flex; justify-content:left; margin-top: 10px;">
                    <button type="button" class="btn btn-primary btn-rounded" id="btn-script" style="margin-right: 5px;">Script</button>
                    <button type="button" class="btn btn-primary btn-rounded" id="btn-pauta" style="margin-right: 5px;">Pauta</button>
                    <button type="button" class="btn btn-primary btn-rounded" id="btn-dirigentes" style="margin-right: 5px;">Dirigentes Sindicais</button>
                    <button type="button" class="btn btn-primary btn-rounded" id="btn-premissas" style="margin-right: 5px;">Premissas</button>
                    <button type="button" class="btn btn-primary btn-rounded" id="btn-calculadora" style="margin-right: 5px;">Calculadora</button>
                  </div>
                </div>
              </div>
            </form>
  
            <hr>
  
            <div id="tab-script">
              <div class="row">
                <form class="form-horizontal">
                  <div class="form-group" style="display: flex; justify-content: space-between; padding: 0px 10px;">
                    <div class="col-md-6">
                      <select id="script_select" class="form-control select2"></select>
                    </div>
  
                    <div class="col-md-6" style="text-align: right;">
                      <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;">Salvar Script</button>
                      <button id="btn-cancelar" type="button" class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
                    </div>
                  </div>
                </form>
              </div>
  
              <div class="row" style="padding: 0px 10px;">
                <div class="view-script" style="display: block; width: 100%; height: 200px; border: 1px solid gray;"></div>
              </div>
            </div>
  
            <div id="tab-pauta">
              <div class="row">
                <form class="form-horizontal">
                  <div class="form-group" style="display: flex; justify-content: space-between; padding: 0px 10px;">
                    <div class="col-md-6">
                      <select id="pauta_select" class="form-control select2"></select>
                    </div>
  
                    <div class="col-md-6" style="text-align: right;">
                      <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;">Salvar Pauta</button>
                      <button type="button" class="btn btn-primary btn-rounded" id="btn-comparar-pauta" style="margin-right: 5px;">Comparar Pauta</button>
                      <button id="btn-cancelar" type="button" class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
                    </div>
                  </div>
                </form>
              </div>
  
              <div class="row" id="view_pauta" style="padding: 0px 10px;">
                <div class="view-script" style="display: block; width: 100%; height: 200px; border: 1px solid gray;"></div>
              </div>
  
              <div class="row" id="view_comparar_pauta" style="padding: 0px 10px;">
                <div class="col-md-6" style="padding: 0;">
                  <div class="panel panel-primary" style="margin-top: 0; padding: 0;">
                    <div class="panel-heading">
                      <h4>Pauta dd/mm/aaaa - hh:mm:ss</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in principal-table" style="padding: 0;">
                      <div class="view-script" style="display: block; width: 100%; height: 200px; border: 1px solid gray;"></div>
                    </div>
                  </div>
                </div>
                <div class="col-md-6" style="padding: 0;">
                  <div class="panel panel-primary" style="margin-top: 0; padding: 0;">
                    <div class="panel-heading">
                      <h4>Pauta dd/mm/aaaa - hh:mm:ss</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in principal-table" style="padding: 0;">
                      <div class="view-script" style="display: block; width: 100%; height: 200px; border: 1px solid gray;"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
  
            <div id="tab-dirigentes">
              <div class="row">
                <form class="form-horizontal">
                  <div class="form-group" style="display: flex; justify-content: space-between; padding: 0px 10px;">
                    <div class="col-md-12" style="text-align: right;">
                      <button id="btn-cancelar" type="button" class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
                    </div>
                  </div>
                </form>
              </div>
  
              <div class="row" style="padding: 0px 10px;">
                <div class="col-md-6" style="padding: 0;">
                  <div class="panel panel-primary" style="margin-top: 0; padding: 0;">
                    <div class="panel-heading">
                      <h4>Dirigente Sindical Laboral</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in principal-table" style="padding: 0;">
                      <div class="view-script" style="display: block; width: 100%; height: 200px; border: 1px solid gray;"></div>
                    </div>
                  </div>
                </div>
                <div class="col-md-6" style="padding: 0;">
                  <div class="panel panel-primary" style="margin-top: 0; padding: 0;">
                    <div class="panel-heading">
                      <h4>Dirigente Sindical Laboral</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in principal-table" style="padding: 0;">
                      <div class="view-script" style="display: block; width: 100%; height: 200px; border: 1px solid gray;"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
  
            <div id="tab-premissas">
              <div class="row">
                <form class="form-horizontal">
                  <div class="form-group" style="display: flex; justify-content: space-between; padding: 0px 10px;">
                    <div class="col-md-6">
                      <select id="premissas_select" class="form-control select2"></select>
                    </div>
  
                    <div class="col-md-6" style="text-align: right;">
                      <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;">Salvar Premissa</button>
                      <button id="btn-cancelar" type="button" class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
                    </div>
                  </div>
                </form>
              </div>
  
              <div class="row" style="padding: 0px 10px;">
                <table class="table table-striped table-bordered demo-tbl">
                  <thead>
                    <tr>
                      <th></th>
                      <th>Assunto</th>
                      <th>Situação Atual</th>
                      <th>Objetivo</th>
                      <th>Resultado</th>
                      <th>Aproveitamento</th>
                      <th>Comentários</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td style="text-align: center;">+</td>
                      <td>--</td>
                      <td>--</td>
                      <td>--</td>
                      <td>--</td>
                      <td>--</td>
                      <td>--</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
  
            <div id="tab-calculadora">
  
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- NEGOCIAÇÃO ACT -->
    <div class="hidden" id="negociacaoActHidden">
      <div id="negociacaoActContent">
        <div class="modal-body" style="padding-top: 25px;">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true" style="transform: translateY(-14px);">&times;</button>
          <div class="panel panel-primary">
            <form class="form-horizontal">
              <input type="hidden" id="up-hidden">
              <input type="hidden" id="id_note">
              <div class="panel panel-primary" style="margin-top: 0;">
                <div class="panel-heading">
                  <h4>Negociação ACT</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="form-group center">
                    <div class="col-sm-6">
                      <label for="sindicato_laboral">Sindicato Laboral</label>
                      <select id="sindicato_laboral" class="form-control select2"></select>
                    </div>

                    <div class="col-md-6">
                      <label for="tipo_acordo">Tipo de acordo</label>
                      <select id="tipo_acordo" class="form-control select2"></select>
                    </div>
                  </div>

                  <div class="form-group center">
                    <div class="col-sm-6">
                      <label for="matriz">Matriz</label>
                      <select id="matriz" class="form-control select2"></select>
                    </div>

                    <div class="col-md-6">
                      <label for="filial">Filial</label>
                      <select id="filial" class="form-control select2"></select>
                    </div>
                  </div>

                </div>

                <div class="row">
                  <div class="col-lg-12">
                    <div class="btn-toolbar" style="display: flex; justify-content:center; margin-top: 20px;">
                      <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;">Processar</button>
                      <button id="btn-cancelar" type="button" class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
                    </div>
                  </div>
                </div>
            </form>
          </div>
        </div>
      </div>
    </div>

  </div> <!--page-content -->

  <?php include 'footer.php' ?>
  
  </div> <!--page-container -->

  <script type='text/javascript' src="./js/negociacao.min.js"></script>
</body>

</html>