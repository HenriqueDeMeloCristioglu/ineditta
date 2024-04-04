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
        2020-08-28 13:40 ( v1.0.0 ) -
    }
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÏ = 0

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

  include_once($fileConsultaClausula);
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
  <link rel="stylesheet" href="consultaclausula.css">

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

    #documentoModal, #infoAdicionalModal {
      z-index: 1200;
    }

    .swal2-container {
      z-index: 1250;
    }

    #documentoModal.fade.in, #infoAdicionalModal.fade.in{
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

    #documentoModal-content,  {
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
  <?php include('menu.php'); ?>

  <div id="pageCtn" class="page-container">
    <div id="page-content" style="min-height: 100%;">
      <input type="hidden" id="sind-id-input">
      <input type="hidden" id="tipo-sind-input">
      <input type="hidden" id="diretoria-id-input">
      <input type="hidden" id="unidade-id-input">
      <input type="hidden" id="nomeunidade-input">
      <input type="hidden" id="clausula-id-input">

      <button style="display: none" id="openInfoSindModalBtn" data-toggle="modal" data-target="#infoSindModal"></button>
      <button style="display: none" id="openClausulaModalBtn" data-toggle="modal" data-target="#clausulaModal"></button>
      <button style="display: none" id="abrirComparativoClausulasBtn" data-toggle="modal"
        data-target="#comparativoFiltroModal"></button>

      <div id="info-modal-sindicato-container">
      </div>

      <!-- MODAL EXIBIÇÃO INFORMAÇÕES ADICIONAIS DA CLAUSULA -->
      <button class="hide" data-toggle="modal" href="#infoAdicionalModal" id="abrirInfoAdicionais-main">
        <i class="fa fa-file-pdf"></i>Visualizar informações adicionais
      </button>
      <div class="hidden" id="infoAdicionalModalHidden">
        <div id="infoAdicionalModalHiddenContent">
          <div class="modal-header" id="asd">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Informações Adicionais das Cláusulas</h4>
          </div>
          <div class="modal-body">
            <div id="informacoes_adicionais"></div>
          </div>
          <a href="#asd" id="scroll-top"></a>
        </div><!-- /.modal-dialog -->
      </div><!-- /.modal -->

      <!-- MODAL EXIBIÇÃO CLAUSULA -->
      <div class="hidden" id="clausulaModalHidden">
        <div id="clausulaModalHiddenContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Exibição das Cláusulas</h4>
          </div>
          <div class="modal-body">

            <div class="flex align-stretch justify-center mt-3">
              <button style="width: 200px;" class="btn btn-primary"
                id="gerarPDF"><i class="fa fa-file-pdf"></i> Gerar PDF</button>

              <button class="btn btn-success gerar-pdf-btn-grid"
                id="gerarExcel"><i class="fa fa-file-pdf"></i> Gerar relatório de cláusulas (Excel)</button>
            </div> 
              <div id="clausulaModalContainer">

            </div>
            <button style="display: none" id="openCommentModalBtn" data-toggle="modal"
              data-target="#comentarioModal"></button>
          </div>
        </div><!-- /.modal-dialog -->
      </div><!-- /.modal -->

      <div class="hidden" id="comentarioModalHidden">
        <div id="comentarioModalHiddenContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Adicionar Comentário</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <form class="form-horizontal">
                <div class="panel panel-primary" style="margin: 0;">
                  <div class="panel-heading">
                    <h4>Comentário</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in principal-table">
                    <div class="form-group center">
                      <div class="col-sm-5">
                        <label for="tipo-com">Tipo do Comentário</label>
                        <select id="tipo-com" class="form-control" disabled>
                          <option value="clausula">Cláusula</option>
                        </select>
                      </div>

                      <div class="col-sm-7">
                        <label for="assunto">Assunto</label>
                        <input type="text" id="assunto" class="form-control" disabled>
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-5">
                        <label for="tipo_usuario_destino">Tipo do Usuário (destino)</label>
                        <select id="tipo_usuario_destino" class="form-control" data-placeholder="Tipo Destino">
                        </select>
                      </div>

                      <div class="col-md-7">
                        <label for="destino" id="campo_tipo">--</label>
                        <select id="destino" class="form-control">
                        </select>
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-sm-4">
                        <label for="tipo-note">Fixo ou Temporário</label>
                        <select id="tipo-note" class="form-control">
                        </select>
                      </div>

                      <div class="col-md-4">
                        <label for="validade">Validade</label>
                        <input type="text" placeholder="DD/MM/DDDD" id="validade" class="form-control">
                      </div>

                      <div class="col-md-4">
                        <label for="usuario">Usuário</label>
                        <input type="text" id="usuario" class="form-control" disabled>
                        <input type="hidden" id="id_user_2">
                      </div>
                    </div>

                    <div class="form-group">
                      <div class="col-md-4">
                        <label for="tipo-etiqueta" class="control-label">Tipo de Etiqueta</label>
                        <select id="tipo-etiqueta" class="form-control select2">
                        </select>
                      </div>
                      <div class="col-md-4">
                        <label for="etiqueta" class="control-label">Etiqueta</label>
                        <select id="etiqueta" class="form-control select2">
                        </select>
                      </div>
                      <div class="col-md-4">
                        <label for="visivel" class="control-label" style="text-align: left;">Visível para outros
                          usuários que têm acesso a consulta de comentários</label>
                        <select id="visivel" class="form-control select2"></select>
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-12">
                        <label for="comentario">Comentário</label>
                        <textarea class="form-control" id="comentario" cols="30" rows="10"></textarea>
                      </div>
                    </div>
                    <div>
                      <input type="hidden" id="id_note" value="">
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
                  <button id="notificacaoCadastrarBtn" type="button"
                    class="btn btn-primary btn-rounded">Cadastrar</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Consulta cláusula</h4> <!-- Opções de pesquisa -->
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
                          <label>Atividade Econômica</label> <!-- Categoria -->
                          <select multiple data-placeholder="CNAE" class="form-control select2" id="categoria">
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
                            id="nome_doc">
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
                      <div class="col-md-4">
                        <label for="search">Palavra-chave:</label>
                        <input type="text" id="search" class="form-control">
                      </div>

                      <div class="col-sm-12" style="display: flex; justify-content: space-between">
                        <div>
                          <button id="filterBtn" style="margin-top: 20px ;" type="button" class="btn btn-primary"><i
                              class="fa fa-search" style="margin-right: 10px;"></i>Filtrar</button>
                          <button id="limparFiltroBtn" style="margin-top: 20px ; margin-left:8px;" type="button"
                            class="btn btn-primary"><i class="fa fa-times-circle-o"
                              style="margin-right: 10px;"></i>Limpar Filtro</button>
                        </div>
                        <button type="button" style="margin-top: 20px;" id="compararClausulasBtn"
                          class="btn btn-primary">COMPARATIVO DE
                          CLÁUSULAS</button>
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

                      <button class="btn btn-primary gerar-pdf-btn-grid"
                        id="gerarPDFBtn"><i class="fa fa-file-pdf"></i> Gerar PDF</button>

                      <button class="btn btn-success gerar-pdf-btn-grid"
                        id="gerarExcelBtn"><i class="fa fa-file-pdf"></i> Gerar relatório de cláusulas (Excel)</button>
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

      <!-- MODAL COMPARAÇÃO DOCUMENTO -->
      <div class="hidden modal_hidden" id="comparativoFiltroModalHidden">
        <div id="comparativoFiltroModalHiddenContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Exibição das Cláusulas</h4>

          </div>
          <div class="modal-body" id="box_geral">
            <!-- <button id="capture-btn" onclick="capture()">Capture Modal</button> -->
            <br>
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Comparativo de Cláusula</h4> <!-- Opções de pesquisa -->
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <div class="row ">
                  <input type="hidden" id="id_sinde">
                  <input type="hidden" id="id_sindp">
                  <input type="hidden" id="id_grupo">
                  <input type="hidden" id="nome_clau">
                  <input type="hidden" id="id_assunto">
                  <div>
                    <div class="row">
                      <div class="row">
                        <div class="col-lg-6" style="padding-left: 20px;">
                          <label for="sind_laboral_comparativo">Sindicato Laboral</label>
                          <select data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sind_laboral_comparativo">
                          </select>
                        </div>

                        <div class="col-lg-6" style="padding-right: 20px;">
                          <label for="sind_laboral_comparativo">Sindicato Patronal</label>
                          <select data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sind_patronal_comparativo">
                          </select>
                        </div>
                      </div>

                      <div class="col-lg-6">
                        <h4>Documento Referência</h4>

                        <div>
                          <label for="doc_referencia">Documento Referência</label>
                          <select data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="doc_referencia">
                          </select>
                        </div>

                        <div>
                          <label for="vigencia_referencia">Vigência Referência</label>
                          <select data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="vigencia_referencia">
                          </select>
                        </div>

                        <div>
                          <button id="documentoReferenciaBtn" class="btn btn-primary w-full mt-2">
                            Selecionar documento referência
                          </button>
                        </div>
                      </div>
                      <div class="col-lg-6">
                        <h4>Documento Anterior</h4>

                        <div>
                          <label for="doc_anterior">Documento Anterior</label>
                          <select data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="doc_anterior">
                          </select>
                        </div>

                        <div>
                          <label for="vigencia_anterior">Vigência Anterior</label>
                          <select data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="vigencia_anterior">
                          </select>
                        </div>

                        <div>
                          <button id="documentoAnteriorBtn" class="btn btn-primary w-full mt-2">
                            Selecionar documento anterior
                          </button>
                        </div>
                      </div>
                    </div>

                    <div
                      style="display: flex; margin: auto; padding-top: 1rem; justify-content: center; align-items: flex-start; gap: 10px;">
                      <input type="checkbox" id="only-diff" name="only-diff">
                      <label for="only-diff">Exibir apenas diferenças</label>
                    </div>

                    <div style="display: flex; justify-content: center; margin-top: 1rem;">
                      <a class="btn btn-primary" id='compararBtn' style="margin-top: 10px;">Comparar</a>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div style="display: none" class="panel panel-primary" style="margin: 0px 0px 20px 0px;"
              id="resultadoComparacao">
              <div class="panel-heading">
                <h4>COMPARAÇÃO</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <div>
                  <button style="margin: auto; display: block; width: 200px;" class="btn btn-primary"
                    id="gerarPDFComparativo"><i class="fa fa-file-pdf"></i> Gerar PDF</button>
                  <p id="clausulasComparacaoCount" style="padding-left: 10px; font-weight: 700; font-size: 1.25rem;">
                  </p>
                </div>
                <div id="containerResultadoComparacao"></div>
              </div>
            </div>
          </div>

          <div class="modal-footer" style="text-align: right;">
            <div class="row">
              <div class="col-lg-12">
                <div class="btn-toolbar" style="display: flex; justify-content:center;">
                  <button data-dismiss="modal" data-toggle="modal" type="button"
                    class="btn btn-secondary">Voltar</button>
                </div>
              </div>
            </div>
          </div>
        </div><!-- /.modal-content -->
      </div><!-- /.modal-dialog -->

      <!-- MODAL DOCUMENTOS -->
      <button id="abrirDocumentoModalBtn" type="button" class="btn btn-primary hide" data-toggle="modal"
        data-target="#documentoModal"></button>
      <div class="hidden" id="documentoModalHidden">
        <div id="documentoModalContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Documentos</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione o Documento</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <input type="hidden" class="form-control" id="id_user" placeholder="">
                <div id="grid-layout-table-3" class="box jplist">
                  <div class="box text-shadow">
                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="documentosTb" data-order='[[ 0, "asc" ]]'
                      style="width: 100%;"></table>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Seguinte</button>
          </div>
        </div>
      </div>

      <div class="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
              <h4 class="modal-title">Comparação de cláusulas</h4>
            </div>
            <div class="modal-body">
              <div>
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Comparação</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in">
                    <div class="row picadiff">
                      <input type="hidden" id="id_sinde">
                      <input type="hidden" id="id_sindp">
                      <input type="hidden" id="id_grupo">
                      <input type="hidden" id="nome_clau">
                      <input type="hidden" id="id_assunto">
                      <div class="picadiff-content">
                        <!-- Primeira cláusula -->
                        <div class="col-lg-6">

                          <table class="table table-striped">
                            <thead>
                              <th>Sindicato Laboral</th>
                            </thead>
                            <tbody>
                              <tr>
                                <td id="laboral"></td>
                              </tr>
                            </tbody>
                          </table>

                          <table class="table table-striped">
                            <thead>
                              <th>Sindicato Patronal</th>
                            </thead>
                            <tbody>
                              <tr>
                                <td id="patronal"></td>
                              </tr>
                            </tbody>
                          </table>

                          <table class="table table-striped">
                            <thead>
                              <th>Grupo Cláusula</th>
                              <th>Nome Cláusula</th>
                              <th>Ano</th>
                            </thead>
                            <tbody>
                              <tr>
                                <td id="data_grupo"></td>
                                <td id="nome"></td>
                                <td id="ano"></td>
                              </tr>
                            </tbody>
                          </table>
                          <p id="txt1" class="tex-clau left" style="width:100%; max-height: 300px; overflow:scroll"></p>

                        </div>

                        <!-- Segunda cláusula -->
                        <div class="col-lg-6">

                          <table class="table table-striped">
                            <thead>
                              <th>Sindicato Laboral</th>
                            </thead>
                            <tbody>
                              <tr>
                                <td id="laboral_comp"></td>
                              </tr>
                            </tbody>
                          </table>

                          <table class="table table-striped">
                            <thead>
                              <th>Sindicato Patronal</th>
                            </thead>
                            <tbody>
                              <tr>
                                <td id="patronal_comp"></td>
                              </tr>
                            </tbody>
                          </table>

                          <table class="table table-striped" style="margin-bottom: 6px;">
                            <thead>
                              <th>Grupo Cláusula</th>
                              <th>Nome Cláusula</th>
                              <th>Ano</th>
                            </thead>
                            <tbody>
                              <tr>
                                <td id="grupo_comp"></td>
                                <td id="nome_comp"></td>
                                <td id="ano_comp">
                                  <select id="ano_base" class="form-control">
                                    <?= $listaAno ?>
                                  </select>
                                </td>
                              </tr>
                            </tbody>
                          </table>

                          <p id="txt2" class="tex-clau right" style="width:100%; max-height: 300px; overflow:scroll">
                          </p>

                        </div>
                      </div>
                    </div>

                    <div class="form-group">

                      <a id="btn_launch" style="margin: 20px 0;" class="btn btn-primary" onclick="launch()">Comparar</a>

                    </div>


                  </div>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <a href="#updateModal" data-dismiss="modal" data-toggle="modal" class="btn btn-secondary">Voltar</a>
            </div>
          </div><!-- /.modal-content -->
        </div><!-- /.modal-dialog -->
      </div><!-- /.modal -->
    </div><!-- /.content -->
  </div> <!-- page-content -->


  <?php include 'footer.php' ?>

  </div> <!-- page-container -->

  <script type='text/javascript' src="./js/consultaclausula.min.js"></script>
</body>

</html>