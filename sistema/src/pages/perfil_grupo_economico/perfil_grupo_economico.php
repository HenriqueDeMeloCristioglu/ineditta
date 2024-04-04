<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}
$userSession = $_SESSION['login'];

$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.'));

// Montando o c??o do erro que ser?presentado
$localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar = array("\\", "/includes/php");
$substituir = array("/", "");
$path = str_replace($localizar, $substituir, __DIR__);

$fileConsultaClausula = $path . '/includes/php/class.perfil1.php';

if (file_exists($fileConsultaClausula)) {

  include_once($fileConsultaClausula);


  include_once __DIR__ . "/includes/php/class.usuario.php";

  include_once __DIR__ . "/includes/php/class.filtro.php";


  $consultaclausula = new perfil1();
} else {
  $response['response_status']['status'] = 0;
  $response['response_status']['error_code'] = $error_code . __LINE__;
  $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.consultaclausula).';
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
  <link rel="stylesheet" href="perfil-grupo-economico.css">
  <link rel="stylesheet" href="includes/css/styles.css">
  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>

  <style>
    #panel-body-encerrado,
    #panel-body-processados {
      height: 430px !important;
    }

    #negociacaoProcessadaSistemaTb,
    #negociacaoEncerradaTb {
      min-width: 1100px !important;
      width: 100% !important;
    }

    body {
      padding-right: 0 !important;
    }

    div.container_logo_client {
      padding: 0 !important;
      overflow: hidden !important;
    }

    .modal-backdrop {
      opacity: 0.1 !important;
    }

    .min-width-column-lg {
      min-width: 152px !important;
      max-width: 164px !important;
    }

    .dataTables_scrollBody {
      overflow-x: scroll !important;
    }

    .select2-container {
      width: 100% !important;
      /* or any value that fits your needs */
    }

    td {
      word-break: break-all;
      padding: 10px 20px !important;
    }

    .map .table-list {
      padding-right: 30px !important;
      font-size: 1.3em !important;
    }

    #anos-neg th {
      text-align: center !important;
    }

    .panel {
      margin: 0px 0 20px 0 !important;
    }

    .info-tiles.tiles-orange {
      font-weight: bolder;
    }

    .info-tiles.tiles-orange .pull-left {
      font-size: 8pt;
    }

    .cards {
      position: relative;
    }

    .cards div.scale {
      /* filtros laranja */
      position: absolute;
      transform: scale(.9);
    }

    .cards div.scale:nth-child(1) {
      left: 0;
      padding-left: 0;
      transform: scale(.9) translate(-20px, -19px);
    }

    .cards div.scale.two {
      right: 0;
      padding-right: 0;
      transform: scale(.9) translate(20px, -19px);
    }

    .map .table-list {
      /* tabela de negociações */
      border-right: none;
      width: 100%;
      padding: 0 !important;
    }

    .map {
      display: flex;
      justify-content: center;
      align-items: center;
      height: 485px;
      overflow: hidden;
    }

    section.index.row.indices_eco .col-lg-12.col-md-12.col-sm-12 {
      height: 166px;
    }

    .info-tiles.tiles-primary {
      height: 100%;
    }

    .info-tiles.tiles-primary .tiles-body {
      height: 82%;
    }

    .table_data table tr td:nth-child(2) {
      text-align: center;
    }

    .fc-toolbar-chunk:nth-child(3) .btn-group {
      display: none;
    }

    .callendary {
      margin: 0 auto 20px auto;
    }

    .fc-scrollgrid-sync-table tr:nth-child(1) .fc-day-disabled {
      display: table-cell;
    }

    .fc-day-disabled {
      display: none;
    }

    .fc-daygrid-day.fc-day.fc-day-thu.fc-day-past {
      padding: 0px 20px !important;
    }

    .fc-daygrid-day-frame.fc-scrollgrid-sync-inner {
      height: 40px;
    }

    .fc .fc-daygrid-day-top {
      justify-content: center;
    }

    /* modal */
    .modal-dialog {
      width: 595px;
    }

    .thead-dark {
      background-color: #4f8edc;
      color: #fff;
    }

    #calendar {
      height: 100% !important;
    }

    .info-tiles.tiles-info .tiles-body {
      background: #174e92;
    }

    .info-tiles.tiles-info .tiles-heading {
      background: #3575c4;
    }

    /* hover */
    .info-tiles.tiles-info:hover .tiles-body {
      background: #0d3d79;
    }

    .info-tiles.tiles-info:hover .tiles-heading {
      background: #265da1;
    }

    .fc-col-header,
    .fc-daygrid-body-unbalanced,
    .fc-scrollgrid-sync-table {
      width: 100% !important;
    }

    .map svg {
      transform: scale(1.1);
      height: 90%;
    }

    .callendary,
    .negocia,
    .count_neg {
      padding: 0 !important;
    }

    #page-content {
      min-height: 100% !important;
    }

    .indices_eco {
      padding: 0px 10px;
      position: relative;
    }

    .cards-indeces {
      position: absolute;
      padding-left: 0;
      padding-bottom: 0;
      top: 50px;
      bottom: 0;
      display: flex;
      flex-direction: column;
      justify-content: space-between;
    }

    .cards-indeces>div {
      padding: 0px;
      padding-right: 10px;
    }

    .tiles-index {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
    }

    .tiles-index-last-month {
      display: flex;
      justify-content: center;
      align-items: center;
      font-size: 0.5em;
      font-weight: 300;
    }

    .tiles-index-last-month>p {
      margin: 0 !important;
    }

    header h1 {
      font-weight: 600;
      font-size: 2em;
    }

    .panel-table-fases {
      height: 485px;
    }

    .dataTables_scrollBody .no-break {
      white-space: nowrap;
    }

    .hide {
      display: none;
    }
  </style>

</head>

<body class="horizontal-nav hide">

  <?php include('menu.php'); ?>


  <div id="pageCtn">
    <div id="page-container">
      <div class="hidden" id="grupoEconomicoEmpresaModalHidden">
        <div id="grupoEconomicoEmpresaModalHiddenContent">
          <div class="modal-content">
            <div class="modal-header">
              <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
              <h4 class="modal-title">Filtros</h4>
            </div>
            <div class="modal-body">
              <div class="panel panel-primary" id="modal-exibir">
                <form>
                  <div class="form-group" id="ttohide">
                    <div class="row">
                      <div class="col-sm-12">
                        <label>Grupo Econômico</label>
                        <select data-placeholder="Nome" class="form-control select2" id="grupo">
                        </select>
                      </div>
                    </div>
                  </div>
                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-12">
                        <label for="tipo_doc">Empresa</label>
                        <select multiple data-placeholder="Nome, Código" class="form-control select2" id="matriz">
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="row">
                    <div class="col-sm-12">
                      <button style="margin-top: 20px ;" type="button" class="btn btn-primary"
                        id="grupoEconomicoEmpresaModalFechar" data-dismiss="modal"><i class="fa fa-search"
                          style="margin-right: 10px;"></i>Filtrar</button>
                    </div>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="hidden" id="estabelecimentoModalHidden">
        <div id="estabelecimentoHiddenContent">
          <div class="modal-content">
            <div class="modal-header">
              <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
              <h4 class="modal-title">Filtros</h4>
            </div>
            <div class="modal-body">
              <div class="panel panel-primary" id="modal-exibir">
                <form>
                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-12">
                        <label>Estabelecimento</label>
                        <select multiple
                          data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente, Regional" tabindex="8"
                          class="form-control select2" id="unidade">
                        </select>
                      </div>
                    </div>
                  </div>
                  <div class="row">
                    <div class="col-sm-12">
                      <button style="margin-top: 20px ;" type="button" class="btn btn-primary"
                        id="estabelecimentoModalFechar" data-dismiss="modal"><i class="fa fa-search"
                          style="margin-right: 10px;"></i>Filtrar</button>
                    </div>
                  </div>
                </form>
              </div>
            </div>
          </div><!-- /.modal-content -->
        </div>
      </div>

      <div class="hidden" id="cnaeModalHidden">
        <div id="cnaeModalHiddenContent">
          <div class="modal-content">
            <div class="modal-header">
              <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
              <h4 class="modal-title">Filtros</h4>
            </div>
            <div class="modal-body">
              <div class="panel panel-primary" id="modal-exibir">
                <form>
                  <div class="form-group" style="z-index:100;">
                    <div class="row">

                      <div class="col-sm-12">
                        <label for="tipo_doc">Atividade Econômica</label>
                        <select multiple data-placeholder="CNAE" class="form-control select2" id="categoria">
                        </select>
                      </div>
                    </div>
                  </div>
                  <div class="row">
                    <div class="col-sm-12">
                      <button style="margin-top: 20px ;" type="button" class="btn btn-primary" id="cnaeModalFechar"
                        data-dismiss="modal"><i class="fa fa-search" style="margin-right: 10px;"></i>Filtrar</button>
                    </div>
                  </div>
                </form>
              </div>
            </div>
          </div><!-- /.modal-content -->
        </div>
      </div>

      <div id="page-content">
        <div class="wrap">
          <div class="container">
            <div class="row" style="display: flex;">
              <div class="col-md-1 img_container"> <!-- style="transform: translate(-13px, 13px);" -->
                <div class="container_logo_client">
                  <img id="imglogo" class="logo_carref">
                </div>
              </div>
              <div class="col-md-11 content_container">
                <section class="row cards" style="display: flex;align-items: center;height: 133px;">
                  <div class="col-lg-6 col-md-12 col-12 scale">
                    <div class="col-lg-4 col-md-3 col-sm-12" style="padding-left: 0;">
                      <a class="info-tiles tiles-primary" id="linkGrupoEconomicoModalAbrir" data-toggle="modal"
                        data-target="#grupoEconomicoEmpresaModal">
                        <div class="tiles-heading">
                          <div class="pull-left">Empresa</div> <!-- Matriz -->
                        </div>
                        <div class="tiles-body">
                          <div class="pull-left"><i class="fa fa-filter"></i></div>
                          <div class="pull-right" id="num-emp">0</div>
                        </div>
                      </a>
                    </div>

                    <div class="col-lg-4 col-md-3 col-sm-12 ">
                      <a class="info-tiles tiles-primary" id="linkEstabelecimentoModalAbrir" data-toggle="modal"
                        data-target="#estabelecimentoModal">
                        <div class="tiles-heading">
                          <div class="pull-left">Estabelecimento</div> <!-- Filial -->
                        </div>
                        <div class="tiles-body">
                          <div class="pull-left"><i class="fa fa-filter"></i></div>
                          <div class="pull-right" id="num-uni">0</div>
                        </div>
                      </a>
                    </div>

                    <div class="col-lg-4 col-md-3 col-sm-12 ">
                      <a class="info-tiles tiles-primary" id="linkCnaeModalAbrir" data-toggle="modal"
                        data-target="#cnaeModal">
                        <div class="tiles-heading">
                          <div class="pull-left">Atividade Econômica</div> <!-- Segmentos -->
                        </div>
                        <div class="tiles-body">
                          <div class="pull-left"><i class="fa fa-filter"></i></div>
                          <div class="pull-right" id="num-seg">0</div>
                        </div>
                      </a>
                    </div>
                  </div>

                  <div class="col-lg-2 col-12">
                  </div>

                  <div class="col-lg-4 col-md-12 col-12 scale two">
                    <div class="col-lg-6 col-md-3 col-sm-12 ">
                      <a class="info-tiles tiles-info divSindicato" href="#">
                        <div class="tiles-heading">
                          <div class="pull-left">Sindicatos Laborais</div>
                        </div>
                        <div class="tiles-body">
                          <div class="pull-left"><i class="fa fa-download"></i></div>
                          <div class="pull-right" id="num-sinde">0</div>
                        </div>
                      </a>
                    </div>
                    <div class="col-lg-6 col-md-3 col-sm-12 ">
                      <a class="info-tiles tiles-info divSindicato" href="#">
                        <div class="tiles-heading">
                          <div class="pull-left">Sindicatos Patronais</div>
                        </div>
                        <div class="tiles-body">
                          <div class="pull-left"><i class="fa fa-download"></i></div>
                          <div class="pull-right" id="num-sindp">0</div>
                        </div>
                      </a>
                    </div>
                  </div>
                </section>
              </div>
            </div>
            <div class="row" style="display: flex;">
              <div class="col-md-12">
                <section class="negocia">
                  <header class="head_neg row" style="padding:0 10px;">
                    <h1>NEGOCIAÇÕES</h1>
                  </header>

                  <section class="negocia-content row">
                    <article class="col-lg-6 neg_block">
                      <div class="chat-panel panel panel-primary">
                        <div class="panel-heading">
                          <h4>Últimas negociações encerradas</h4>
                          <!-- <div class="pull-right"><span class="badge">27</span></div> -->
                          <div class="options">
                            <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                          </div>
                        </div>
                        <div class="panel-body" id="panel-body-encerrado">
                          <table style="width: 100%;" class="table table-striped table-bordered"
                            data-order='[[ 6, "desc" ]]' id="negociacaoEncerradaTb">
                          </table>
                        </div>
                      </div>
                    </article>

                    <article class="col-lg-6 neg_block">
                      <div class="chat-panel panel panel-primary">
                        <div class="panel-heading">
                          <h4>Últimas negociações processadas no Sistema</h4>
                          <!-- <div class="pull-right"><span class="badge">238</span></div> -->
                          <div class="options">
                            <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                          </div>
                        </div>
                        <div class="panel-body" id="panel-body-processados">
                          <table class="table table-striped table-bordered" id="negociacaoProcessadaSistemaTb"
                            data-order='[[ 5, "desc" ]]'>
                          </table>
                        </div>
                      </div>
                    </article>
                  </section>
                </section>

                <section class="row neg_estado">
                  <div class="col-lg-6 col-md-6">
                    <div class="panel-chat panel panel-primary">
                      <div class="panel-heading">
                        <h4>Negociações em aberto por fase</h4> <!-- Negociações por fase -->
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body panel-table-fases">
                        <div class="table-list">
                          <table class="table table-bordered table-striped" id="table_fases">
                            <tbody></tbody>
                          </table>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="col-lg-6 col-md-6">
                    <div class="panel-chat panel panel-primary">
                      <div class="panel-heading">
                        <h4>Mapa de negociações em aberto</h4> <!-- Mapa -->
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body map">
                        <?php include 'map_carre.php' ?>
                      </div>
                    </div>
                  </div>
                </section>

                <section class="row neg_data_base">
                  <div class="col-lg-12 col-md-12">
                    <div class="panel panel-primary" style="padding-bottom: 2.5rem">
                      <div class="panel-heading">
                        <h4>Negociações em aberto por data-base</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body" id="painelNegAbertas" style="width: 100%;">
                        <div class="row">
                          <div class="col-sm-12">
                            <form class="form-horizontal" id="negociacoesAbertasForm">
                              <div class="row" style="display: flex; justify-content: center;">
                                <div class="col-sm-3">
                                  <label for="anoDocNeg">Ano-base</label>
                                  <select class="form-control select2" id="anoDocNeg">
                                  </select>
                                </div>
                                <div class="col-sm-3">
                                  <label for="nomeDocNeg">Nome Documento</label>
                                  <select class="form-control select2" id="nomeDocNeg">
                                  </select>
                                </div>
                                <div class="col-sm-3">
                                  <label for="faseDocNeg">Fase</label>
                                  <select class="form-control select2" id="faseDocNeg">
                                  </select>
                                </div>
                              </div>
                            </form>
                          </div>
                        </div>
                        <canvas style="margin: auto; max-width: 1100px; width: 100%;"
                          id="negociacoesAbertasChart"></canvas>
                      </div>
                    </div>
                  </div>
                </section>

                <section class="index row count_neg">


                  <div class="col-md-12 col-lg-12">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Quantidade e estrutura das negociações acumuladas no período</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body">
                        <table class="table table-hover table-bordered" id="negociacoesAcumuladasTb">
                          <thead>
                            <tr>
                              <th colspan="7" style="text-align: center;">Estrutura da Negociação</th>
                            </tr>
                            <tr>
                              <th></th>
                              <th colspan="2" style="text-align: center;">Quantidade</th>
                              <th colspan="2" style="text-align: center;">Proporção</th>
                            </tr>
                            <tr>
                              <th style="text-align: center;">Nome do documento</th>
                              <th style="text-align: center;" class="anoAtual"></th>
                              <th style="text-align: center;" class="anoPassado"></th>
                              <th style="text-align: center;" class="anoAtual"></th>
                              <th style="text-align: center;" class="anoPassado"></th>
                            </tr>
                            <tr id="acordos-coletivos">
                              <td>Acordos Coletivos</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0%</td>
                              <td style="text-align: center;">0%</td>
                            </tr>
                            <tr id="acordos-coletivos-esp">
                              <td>Acordos Coletivos Específicos</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0%</td>
                              <td style="text-align: center;">0%</td>
                            </tr>
                            <tr id="convencoes-coletivas">
                              <td>Convenções Coletivas</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0%</td>
                              <td style="text-align: center;">0%</td>
                            </tr>
                            <tr id="convencoes-coletivas-esp">
                              <td>Convenções Coletivas Específicas</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0%</td>
                              <td style="text-align: center;">0%</td>
                            </tr>
                            <tr id="termo-aditivo-convencao-coletiva">
                              <td>Termo Aditivo de Convenção Coletiva</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0%</td>
                              <td style="text-align: center;">0%</td>
                            </tr>
                            <tr id="termo-aditivo-acordo-coletivo">
                              <td>Termo Aditivo de Acordo Coletivo</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0%</td>
                              <td style="text-align: center;">0%</td>
                            </tr>
                            <tr id="total-neg">
                              <td>Total</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0</td>
                              <td style="text-align: center;">0%</td>
                              <td style="text-align: center;">0%</td>
                            </tr>
                          </thead>
                          <tbody>
                          </tbody>
                        </table>
                        <p>Data da Ultima Atualização: <span id="negociacoesAcumuladasDataUltimaAtualizacao"></span></p>
                      </div>
                    </div>
                  </div>
                </section>

                <section class="index row indices_eco">
                  <div class="content">
                    <header style="padding:0px 10px 0px 0px;">
                      <h1>INDICES ECONÔMICOS (INPC e IPCA)</h1>
                    </header>
                    <div class="col-lg-3 col-md-3 cards-indeces" style="width: 20.666667%;">
                      <div class="col-lg-12 col-md-12 col-sm-12 " style="height: 50%;">
                        <div class="info-tiles tiles-primary" style="cursor: pointer">
                          <div class="tiles-heading">
                            <div class="pull-left">INPC - Últimos 12 meses</div>
                          </div>
                          <div class="tiles-body tiles-index">
                            <div id="calc-inpc">5,688%</div>
                            <div class="tiles-index-last-month">
                              <p>Último mês:&nbsp;</p>
                              <div id="last-inpc">0,755% Fev 2022</div>
                            </div>
                          </div>
                        </div>
                      </div>

                      <div class="col-lg-12 col-md-12 col-sm-12 " style="margin-bottom: 20px; height: 50%;">
                        <div class="info-tiles tiles-primary" style="cursor: pointer">
                          <div class="tiles-heading">
                            <div class="pull-left">IPCA - Últimos 12 meses</div>
                          </div>
                          <div class="tiles-body tiles-index">
                            <div id="calc-ipca">5,688%</div>
                            <div class="tiles-index-last-month">
                              <p>Último mês:&nbsp;</p>
                              <div id="last-ipca">0,755% Fev 2022</div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>

                    <div class="col-md-9 col-lg-9" style="float: right; width: 80%;">
                      <div class="panel panel-primary">
                        <div class="panel-heading">
                          <h4>Evolução dos indicadores</h4>
                          <div class="options">
                            <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                          </div>
                        </div>
                        <div class="panel-body">
                          <a class="btn default-alt chart-evolucao-indicadores" data-quantidadeMeses="12">12 meses</a>
                          <a class="btn default-alt chart-evolucao-indicadores" data-quantidadeMeses="24">24 meses</a>
                          <canvas id="indicadoresEvolucaoChart" height="100"></canvas>
                        </div>
                      </div>
                    </div>
                  </div>
                </section>


              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <?php include 'footer.php' ?>

  </div> <!-- page-container -->
  <script type='text/javascript' src='./js/core.min.js'></script>
  <script type='text/javascript' src="./js/perfil-grupo-economico.min.js"></script>

  <script type='text/javascript' src='./js/demo/demo.min.js'></script>
  <script type='text/javascript' src='./js/demo/demo-datatables.min.js'></script>
  <script type='text/javascript' src='./js/demo/demo-modals.min.js'></script>

  <script type='text/javascript' src="./js/filtros.min.js"></script>
</body>

</html>