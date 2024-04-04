<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

$userSession = $_SESSION['login'];

ini_set("memory_limit", "800M");
ini_set("max_execution_time", "800");

$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.'));

// Montando o c??o do erro que ser?presentado
$localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar = array("\\", "/includes/php");
$substituir = array("/", "");
$path = str_replace($localizar, $substituir, __DIR__);

$fileConsultaClausula = $path . '/includes/php/class.consultaclausula.php';

if (file_exists($fileConsultaClausula)) {

  include_once ($fileConsultaClausula);
  include_once __DIR__ . "/includes/php/class.usuario.php";
  include_once __DIR__ . "/includes/php/class.filtro.php";

  $consultaclausula = new consultaclausula();

  $user = (new usuario())->validateUser($userSession)['response_data']['user'];

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

  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>

  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

  <!-- Bootstrap 3.3.7 -->
  <link rel="stylesheet" href="busca_rapida.css">

  <!-- Bootstrap Internal -->
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    button:disabled {
      cursor: not-allowed;
      pointer-events: all !important;
    }

    td {
      word-break: keep-all
    }

    .img_box {
      position: absolute;
      z-index: 999;
      width: 100%;
      height: 100%;
      background-color: rgba(255, 255, 255, 0.7);
      display: none;
    }

    .img_load {
      position: absolute;
      top: 30%;
      right: 45%;
    }

    .tex-clau {
      text-align: justify;
      text-justify: inter-word;
      white-space: pre-line;
    }

    .hei {
      max-height: 50vh;
      overflow: auto;
    }

    div.dataTables_wrapper div.dataTables_filter input {
      width: 500px;
    }

    .dataTables_filter {
      display: flex;
      justify-content: flex-end;
    }

    .table-info {
      border-collapse: separate;
      border-spacing: 0;
    }

    #cabecalho th {
      position: sticky;
      top: 0;
      background-color: #fff;
      border-bottom: 2px solid #e6e7e8 !important;
      z-index: 10;
    }

    .fixTableHead {
      padding: 0px 20px 20px 20px !important;
      max-height: 35vh;
      overflow: scroll;
      border-collapse: separate;

    }

    .select2-container {
      width: 100% !important;
    }

    .novoFragmento {
      white-space: pre-wrap;
      background: #fff;
      border: #0ce783 solid;
      border-width: 0px 0px 0px 0.5em;
      border-radius: 0.5em;
      font-family: sans-serif;
      font-size: 88%;
      line-height: 1.6;
      box-shadow: 2px 2px 2px #ddd;
      padding: 1em;
      margin: 0;
    }

    .nadaDeNovo {
      white-space: pre-wrap;
      background: #fff;
      border: #777 solid;
      border-width: 0px 0px 0px 0.5em;
      border-radius: 0.5em;
      font-family: sans-serif;
      font-size: 88%;
      line-height: 1.6;
      box-shadow: 2px 2px 2px #ddd;
      padding: 1em;
      margin: 0;
    }

    .novoInserted {
      font-weight: bold;
      background-color: #0ce783 !important;
      color: #222;
      border-radius: 0.25em;
      padding: 0.2em 1px;
    }

    .dataTables_scroll .dataTables_scrollHead .dataTables_scrollHeadInner {
      width: 100% !important;
    }

    .dataTables_scroll .dataTables_scrollHead .dataTables_scrollHeadInner .table {
      width: 100% !important;
    }

    /* .wikEdDiffNoChange {
            display: none !important;
        } */

    /* .left-text:has( .wikEdDiffNoChange ), .right-text:has( .wikEdDiffNoChange ) {
            display: none !important;
        } */

    /* .picadiff .right,
        .picadiff .left {
            margin: 0;
            width: 100%;
        } */

    .picadiff .picadiff-content .right .equal {
      background-color: #fff;
      display: inline;
    }

    .picadiff .picadiff-content .right .insertion {
      background-color: lightcoral;
    }

    .picadiff .right .equal,
    .picadiff .left .equal {
      background-color: #fff;
      display: inline;
    }

    .picadiff .right .deletion,
    .picadiff .left .deletion {
      background-color: greenyellow;
    }

    .picadiff .right .insertion,
    .picadiff .left .insertion {
      background-color: lightcoral;
    }

    #page-content {
      min-height: 100% !important;
    }

    .texto-link:hover {
      text-decoration: underline;
      transition: all 0.3s ease-in-out;
    }

    .table.table-striped.table-bordered {
      margin-bottom: 0;
    }

    .filiais_abrangidas table tr.odd.gradeX {
      height: 41px;
    }

    #box_textos_2 textarea {
      margin-top: 8px;
    }

    .picadiff .right,
    .picadiff .left {
      width: 50%;
    }

    #sobre_documento {
      width: 100%;
    }

    #sobre_documento tr td {
      padding: 8px;
    }

    .clausula_box {
      box-shadow: -4px -4px 0px 0px rgba(122, 122, 122, 0.66);
      -webkit-box-shadow: -4px -4px 0px 0px rgba(122, 122, 122, 0.66);
      -moz-box-shadow: -4px -4px 0px 0px rgba(122, 122, 122, 0.66);
      height: fit-content;
      padding-right: 4px;
      padding-left: 0;
    }

    .clausula_box_added {
      box-shadow: -4px -4px 0px 0px rgba(98, 240, 98, 1);
      -webkit-box-shadow: -4px -4px 0px 0px rgba(98, 240, 98, 1);
      -moz-box-shadow: -4px -4px 0px 0px rgba(98, 240, 98, 1);
      height: fit-content;
      padding-right: 4px;
      padding-left: 0;
    }

    .clausula_box_removed {
      box-shadow: -4px -4px 0px 0px rgba(250, 145, 145, 1);
      -webkit-box-shadow: -4px -4px 0px 0px rgba(250, 145, 145, 1);
      height: fit-content;
      padding-right: 4px;
      padding-left: 0;
    }

    .clausula_null {
      color: #8a6d3b;
      background-color: #fcf8e3;
      border-color: #faebcc;
      border-radius: 4px;
      height: fit-content;
      text-align: center;
    }

    .texto_clausula {
      color: black;
      cursor: pointer;
    }

    .texto_clausula:hover {
      text-decoration: underline;
    }

    mark {
      padding: 0;
    }

    .title-coluna-nova,
    .title-coluna-antiga {
      padding: 0.5rem 1rem;
      margin-bottom: 1rem;
    }

    .w-full {
      width: 100%;
    }

    .mt-2 {
      margin-top: 8px;
    }

    .hide {
      display: none;
    }

    #documentoModal,
    #infoAdicionalModal {
      z-index: 1200;
    }

    .swal2-container {
      z-index: 1250;
    }

    #documentoModal.fade.in,
    #infoAdicionalModal.fade.in {
      display: flex !important;
      flex-direction: column;
      justify-content: center;
    }

    #documentosTb_filter {
      display: none;
    }

    .flex {
      display: flex;
    }

    .align-stretch {
      align-items: stretch;
    }

    .form-control-title {
      margin-bottom: 0;
      background-color: #4f8edc;
      color: #fff;
      font-weight: 700;
      text-align: center;
      margin-top: 20px;
      font-size: 18px;
      margin-bottom: 10px;
      padding: 10px 0;
    }

    .btn-info-adicional {
      margin: auto;
      display: block;
      margin-top: 0px;
      margin-left: 0px;
    }

    #infoAdicionalModal-content {
      max-height: 80vh;
      overflow-y: scroll;
      overflow-x: hidden;
    }

    #documentoModal-content,
    {
    max-height: 90vh;
    overflow-y: scroll;
    }

    #infoAdicionalModal .modal-dialog {
      width: 95vw;
    }

    .gerar-pdf-btn-grid {
      margin-left: 8px;
    }

    .justify-center {
      justify-content: center;
    }

    .mt-3 {
      margin-top: 12px;
    }
  </style>
</head>

<body class="horizontal-nav hide">
  <?php include ('menu.php'); ?>

  <div id="pageCtn" class="page-container">
    <div id="page-content" style="min-height: 100%;">
      <input type="hidden" id="sind-id-input">
      <input type="hidden" id="tipo-sind-input">

      <!-- MODAL INFORMAÇÕES SINDICATOS -->
      <button style="display: none" id="openInfoSindModalBtn" data-toggle="modal" data-target="#infoSindModal"></button>
      <div class="hidden modal_hidden" id="infoSindModalHidden">
        <div id="infoSindModalHiddenContent">
          <div class="modal-content">
            <div class="modal-header">
              <div style="display: flex; width: 100%; justify-content: space-between;">
                <h4 class="modal-title" id="infoSindModalTitle">Informações Sindicais</h4>
                <div class="dropdown" style="margin-left: 50%;">
                  <button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu2"
                    data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                    Módulos <i class="fa fa-th"></i>
                  </button>
                  <ul class="dropdown-menu" aria-labelledby="dropdownMenu2">
                    <li><a href="#" id="direct-comparativo-btn">Comparar Cláusulas</a></li>
                    <li><a href="#" id="direct-calendarios-btn">Calendário Sindical</a></li>
                    <li><a href="#" id="direct-documentos-btn">Consulta de documentos</a></li>
                    <li><a href="#" id="direct-gerar-excel-btn">Mapa Sindical (Excel)</a></li>
                    <li><a href="#" id="direct-formulario-aplicacao-btn">Mapa sindical (Formulário Aplicação)</a></li>
                    <li><a href="#" id="direct-comparativo-mapa-btn">Mapa sindical (Comparativo)</a></li>
                    <li><a href="#" id="direct-relatorio-negociacoes-btn">Negociação (Acompanhamento CCT Ineditta)</a>
                    </li>
                  </ul>
                </div>
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
              </div>
            </div>

            <div class="modal-body">
              <form id="infoSindForm">
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Dados cadastrais</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse" id="collapseDadosCadastrais"><i
                          class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in" id="collapseDadosCadastraisBody">
                    <div class="form-group">
                      <div class="col-sm-3">
                        <label for="info-sigla">Sigla</label>
                        <input class="col-sm-9 form-control" type="text" id="info-sigla" disabled>
                      </div>

                      <div class="col-sm-3">
                        <label for="info-cnpj">CNPJ</label>
                        <input class="col-sm-9 form-control" type="text" id="info-cnpj" disabled>
                      </div>

                      <div class="col-sm-6">
                        <label for="info-razao">Razão Social</label>
                        <input class="col-sm-8 form-control" type="text" id="info-razao" disabled>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-denominacao">Denominação</label>
                        <input class="col-sm-9 form-control" type="text" id="info-denominacao" disabled>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-cod-sindical">Código Sindical</label>
                        <input class="col-sm-8 form-control" type="text" id="info-cod-sindical" disabled>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Localização</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse" id="collapseLocalizacao"><i
                          class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in" id="collapseLocalizacaoBody">
                    <div class="form-group">
                      <div class="col-sm-2">
                        <label for="info-uf">UF</label>
                        <input class="col-sm-8 form-control" type="text" id="info-uf" disabled>
                      </div>

                      <div class="col-sm-3">
                        <label for="info-municipio">Município</label>
                        <input class="col-sm-9 form-control" type="text" id="info-municipio" disabled>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-logradouro">Logradouro</label>
                        <input class="col-sm-9 form-control" type="text" id="info-logradouro" disabled>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Informações de Contato</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse" id="collapseContato"><i
                          class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in" id="collapseContatoBody">
                    <div class="form-group">
                      <div class="col-sm-3">
                        <label for="info-telefone1">Telefone</label>
                        <input class="col-sm-8 form-control" type="text" id="info-telefone1" disabled>
                      </div>

                      <div class="col-sm-3">
                        <label for="info-telefone2">Telefone 2</label>
                        <input class="col-sm-8 form-control" type="text" id="info-telefone2" disabled>
                      </div>

                      <div class="col-sm-3">
                        <label for="info-telefone3">Telefone 3</label>
                        <input class="col-sm-8 form-control" type="text" id="info-telefone3" disabled>
                      </div>

                      <div class="col-sm-3">
                        <label for="info-ramal">Ramal</label>
                        <input class="col-sm-9 form-control" type="text" id="info-ramal" disabled>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-enquadramento">Contato Enquadramento</label>
                        <input class="col-sm-8 form-control" type="text" id="info-enquadramento" disabled>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-negociador">Contato Negociador</label>
                        <input class="col-sm-8 form-control" type="text" id="info-negociador" disabled>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-contribuicao">Contato Contribuição</label>
                        <input class="col-sm-8 form-control" type="text" id="info-contribuicao" disabled>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-email1">Email</label>
                        <a id="info-email1-link" style="display: flex;" target="_blank">
                          <input class="col-sm-9 form-control" type="text" id="info-email1" readonly>
                        </a>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-email2">Email 2</label>
                        <a id="info-email2-link" style="display: flex;" target="_blank">
                          <input class="col-sm-9 form-control" type="text" id="info-email2" readonly>
                        </a>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-email3">Email 3</label>
                        <a id="info-email3-link" style="display: flex;" target="_blank">
                          <input class="col-sm-9 form-control" type="text" id="info-email3" readonly>
                        </a>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-twitter">Twitter</label>
                        <a id="info-twitter-link" style="display: flex;" target="_blank">
                          <input class="col-sm-9 form-control" type="text" id="info-twitter" readonly>
                        </a>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-facebook">Facebook</label>
                        <a id="info-facebook-link" style="display: flex;" target="_blank">
                          <input class="col-sm-9 form-control" type="text" id="info-facebook" readonly>
                        </a>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-instagram">Instagram</label>
                        <a id="info-instagram-link" style="display: flex;" target="_blank">
                          <input class="col-sm-9 form-control" type="text" id="info-instagram" readonly>
                        </a>
                      </div>

                      <div class="col-sm-4">
                        <label for="info-site">Site</label>
                        <a id="info-site-link" style="display: flex;" target="_blank">
                          <input class="col-sm-9 form-control" type="text" id="info-site" readonly>
                        </a>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Associações</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse" id="collapseAssociacoes"><i
                          class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in" id="collapseAssociacoesBody">
                    <div class="form-group">
                      <div class="col-sm-6">
                        <label for="info-federacao-nome">Nome Federação</label>
                        <input class="col-sm-9 form-control" type="text" id="info-federacao-nome" disabled>
                      </div>

                      <div class="col-sm-6">
                        <label for="info-federacao-cnpj">CNPJ Federação</label>
                        <input class="col-sm-9 form-control" type="text" id="info-federacao-cnpj" disabled>
                      </div>

                      <div class="col-sm-6">
                        <label for="info-confederacao-nome">Nome Confederação</label>
                        <input class="col-sm-9 form-control" type="text" id="info-confederacao-nome" disabled>
                      </div>

                      <div class="col-sm-6">
                        <label for="info-confederacao-cnpj">CNPJ Confederação</label>
                        <input class="col-sm-9 form-control" type="text" id="info-confederacao-cnpj" disabled>
                      </div>

                      <div class="col-sm-6">
                        <label for="info-central-sind-nome">Nome Central Sindical</label>
                        <input class="col-sm-9 form-control" type="text" id="info-central-sind-nome" disabled>
                      </div>

                      <div class="col-sm-6">
                        <label for="info-central-sind-cnpj">CNPJ Central Sindical</label>
                        <input class="col-sm-9 form-control" type="text" id="info-central-sind-cnpj" disabled>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Diretoria</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse" id="collapseDiretoria"><i
                          class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in" id="collapseDiretoriaBody">
                    <div class="box text-shadow">
                      <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="diretoriainfosindtb"
                        data-order='[[ 1, "asc" ]]'>
                      </table>
                    </div>
                  </div>
                </div>
              </form>
            </div>
            <div class="modal-footer">
              <div class="row">
                <div class="col-sm-12">
                  <div class="btn-toolbar" style="display: flex; justify-content: flex-end;">
                    <button type="button" data-toggle="modal" class="btn btn-danger btn-rounded btn-cancelar"
                      data-dismiss="modal">Voltar</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
        </div>
      </div>

      <!-- MODAL EXIBIÇÃO CLAUSULA -->
      <button style="display: none" id="openClausulaModalBtn" data-toggle="modal" data-target="#clausulaModal"></button>
      <div class="hidden" id="clausulaModalHidden">
        <div id="clausulaModalHiddenContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Exibição das Cláusulas</h4>
          </div>
          <div class="modal-body">

            <div class="flex align-stretch justify-center mt-3" style="display: flex; gap: 15px;">
              <button style="width: 200px;" class="btn btn-primary" id="gerarPDF"><i class="fa fa-file-pdf"></i> Gerar
                PDF</button>

              <button class="btn btn-success" id="gerarExcelBtn2">
                <i class="fa fa-file-pdf"></i>
                Gerar
                relatório de cláusulas (Excel)
              </button>
            </div>
            <div id="clausulaModalContainer"></div>
            <button style="display: none" id="openCommentModalBtn" data-toggle="modal"
              data-target="#comentarioModal"></button>
          </div>
        </div><!-- /.modal-dialog -->
      </div><!-- /.modal -->

      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Busca rápida</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <form>
                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-3">
                          <label>Grupo Econômico</label>
                          <select data-placeholder="Nome" class="form-control select2" id="grupo">
                          </select>
                        </div>

                        <div class="col-sm-3">
                          <label>Empresa</label> <!-- Matriz -->
                          <select multiple data-placeholder="Código, CNPJ, Nome" class="form-control select2"
                            id="matriz">
                          </select>
                        </div>

                        <div class="col-sm-6">
                          <label>Estabelecimento</label> <!-- Filial -->
                          <select multiple data-placeholder="Nome, CNPJ" class="form-control select2" id="unidade">
                          </select>
                        </div>
                      </div>
                    </div>
                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-4">
                          <label>Localidade do Estabelecimento</label>
                          <select multiple data-placeholder="Região, UF ou Município" class="form-control select2"
                            id="localidade">
                          </select>
                        </div>

                        <div class="col-sm-5">
                          <label>Atividade Econômica</label>
                          <select multiple data-placeholder="CNAE" class="form-control select2" id="cnaes">
                          </select>
                        </div>
                      </div>
                    </div>

                    <div class="form-group">
                      <div class="row">
                        <div class="col-lg-5">
                          <label id="label-sindicato" for="">Sindicato Laboral</label>
                          <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sind_laboral">
                          </select>
                        </div>

                        <div class="col-lg-5">
                          <label id="label-sindicato" for="">Sindicato Patronal</label>
                          <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sind_patronal">
                          </select>
                        </div>

                        <div class="col-sm-2">
                          <label for="data_base">Data-base/Ano</label>
                          <select data-placeholder="Mês/Ano" class="form-control select2" id="data_base">
                          </select>
                        </div>
                      </div>
                    </div>

                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-4">
                          <label for="tipo_doc">Nome do Documento</label>
                          <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="tipo_documento">
                          </select>
                        </div>

                        <div class="col-sm-3">
                          <label for="tipo_doc">Grupo Cláusulas</label>
                          <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="grupo_clausulas">
                          </select>
                        </div>

                        <div class="col-sm-3">
                          <label for="tipo_doc">Seleção de Cláusulas</label>
                          <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="estrutura_clausula">
                          </select>
                        </div>
                      </div>
                    </div>

                    <div class="row">
                      <div class="col-sm-3">
                        <label>Data do Processamento Ineditta</label>
                        <input type="text" placeholder="dd/mm/aaaa - dd/mm/aaaa"
                          class="form-control float-right date_format" id="processamento">
                      </div>

                      <div class="col-sm-12" style="display: flex; justify-content: space-between">
                        <div>
                          <button id="filterBtn" style="margin-top: 20px ;" type="button" class="btn btn-primary"><i
                              class="fa fa-search" style="margin-right: 10px;"></i>Filtrar</button>
                          <button id="limparFiltroBtn" style="margin-top: 20px ; margin-left:8px;" type="button"
                            class="btn btn-primary"><i class="fa fa-times-circle-o"
                              style="margin-right: 10px;"></i>Limpar Filtro</button>
                        </div>
                      </div>
                    </div>
                  </form>
                </div>
              </div>

              <div class="panel panel-primary" id="painel_lista_clausula">
                <div class="panel-heading">
                  <h4>Lista de Cláusulas</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <input type="hidden" class="form-control" id="ids-input" placeholder="">

                  <div class="row" style="margin-bottom: 16px;">
                    <div class="col-lg-12 flex">
                      <a id="abrirClausulaBtn" data-toggle="modal" href="#updateModal" class="btn btn-primary">ABRIR
                        CLÁUSULAS</a>

                      <button class="btn btn-primary gerar-pdf-btn-grid" id="gerarPDFBtn"><i class="fa fa-file-pdf"></i>
                        Gerar PDF</button>

                      <button class="btn btn-success gerar-pdf-btn-grid" id="gerarExcelBtn"><i
                          class="fa fa-file-pdf"></i> Gerar relatório de cláusulas (Excel)</button>
                    </div>
                  </div>

                  <div class="box text-shadow" id="table-container">
                    <div id="selectAllDiv" style="display: none; margin-bottom: 10px;" class="selectAll"
                      style="margin: 0px 0px 20px 0px ;">
                      <input type="checkbox" id="selectAllInput">
                      <label for="selectAllInput">Selecionar Todos</label>
                    </div>
                    <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="clausulasTb" data-order='[[ 1, "asc" ]]'>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div> <!-- container -->
      </div> <!-- #wrap -->

      <!-- MODAL CLAUSULA CLIENTE -->
      <button style="display: none" id="openClausulaClienteModalBtn" data-toggle="modal"
        data-target="#clausulaClienteModal"></button>
      <div class="hidden" id="clausulaClienteModalHidden">
        <div class="modal-content" id="clausulaClienteModalContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Cláusula</h4>
          </div>
          <div class="modal-body">
            <form class="form-horizontal">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Adicionar Regra Empresa</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="form-group center">
                    <div class="col-sm-3">
                      <label for="info-inputc" class="control-label">Nome da Cláusula</label>
                      <input type="text" class="form-control" id="nome_clausula_cliente" disabled>
                    </div>
                    <div class="col-sm-12">
                      <label for="info-inputc" class="control-label">Texto</label>
                      <textarea class="form-control" id="texto_clausula_cliente" cols="30" rows="30"
                        style="max-height: 55vh; resize:none;"></textarea>
                    </div>
                  </div>
                </div>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <div class="row">
              <div class="col-lg-12">
                <div class="btn-toolbar" style="display: flex; justify-content:center;">
                  <button type="button" id="btn_add_clausula_cliente"
                    class="btn btn-primary btn-rounded">Salvar</button>
                </div>
              </div>
              <button type="button" class="btn btn-primary" data-dismiss="modal" data-toggle="modal">voltar</button>
            </div>
          </div>
        </div>
      </div>
    </div><!-- /.content -->
  </div> <!-- page-content -->


  <?php include 'footer.php' ?>

  </div> <!-- page-container -->

  <script type='text/javascript' src="./js/busca_rapida.min.js"></script>
</body>

</html>