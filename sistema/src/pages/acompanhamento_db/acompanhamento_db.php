<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}
$sessionUser = $_SESSION['login'];
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

$fileClassFedemp = $path . '/includes/php/class.acompanhamento_db.php';


$fileClassDis = $path . '/includes/php/class.disparoEmail.php';
if (file_exists($fileClassFedemp)) {

  //include_once($fileClassDis);

  include_once($fileClassFedemp);

  include_once __DIR__ . "/includes/php/class.usuario.php";

  $user = new usuario();
  $userData = $user->validateUser($sessionUser)['response_data']['user'];

  $modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

  $modulos = ["sisap" => $modulosSisap, "comercial" => []];

  $sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

  foreach ($sisap as $key => $value) {
    if (mb_strpos($value, "Acompanhamento CCT")) {
      $indexModule = $key;
      $idModule = strstr($value, "+", true);
    }
  }

  for ($i = 0; $i < count($modulosSisap); $i++) {
    if ($modulosSisap[$i]->id == $idModule) {
      $thisModule = $modulosSisap[$i]; //module Permissions here
      break;
    }
  }
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

  <!-- The following CSS are included as plugins and can be removed if unused-->
  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

  <!-- Bootstrap 3.3.7 -->
  <link rel="stylesheet" href="acompanhamento_db.css">

  <!-- Bootstrap Internal -->
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    .select2-container {
      width: 100% !important;
      /* or any value that fits your needs */
    }

    .swal2-select {
      display: none !important;
    }

    select {
      display: block !important;
    }

    #mensagem_sucesso {
      display: none;
    }

    #mensagem_error {
      display: none;
    }

    #mensagem_alt_sucesso {
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

    #sscript {
      display: none;
    }

    td {
      word-break: keep-all;
    }

    #semail {
      display: none;
    }

    #escript {
      display: none;
    }

    /* The heart of the matter */
    .testimonial-group>.row {
      overflow-x: auto;
      white-space: nowrap;
    }

    .testimonial-group>.row>.col-md-3 {
      display: inline-grid;
      float: none;
    }

    .trava {
      background-color: white;
      opacity: 0;
      width: 100%;
      height: 1vh;
      /* custom */
    }

    .trava:active {
      pointer-events: none;
    }

    .trava:click {
      pointer-events: none;
    }

    .coluna {
      background-color: rgba(56, 116, 214, 0.5);
      padding-right: 1vw;
      border-radius: 10px;
      min-height: 5vh;

    }

    .card {
      background-color: white;
      width: 100%;
      display: flex;
      flex-direction: column;
      padding: 0.5vw;
      margin: 0.5vw;
      border-radius: 10px;
      border-style: solid;
      border-color: #dedede;
      white-space: pre-line;
      border-width: 0.25em;
      /* custom */
    }

    .card h1,
    .card h2,
    .card h3,
    .card h4,
    .card h5 {
      margin: 0px;
      padding: 0px 0px 0.25vh 0px;
      font-family: "Noto Sans KR", sans-serif;
      font-size: 1em;
      color: #282828;
    }

    .card hr {
      display: block;
      border: none;
      height: 1.5px;
      background-color: #007bff;
      margin: 0px;
    }

    .card p {
      margin: 15px 0px 0px 0px;
      font-family: "Noto Sans KR", sans-serif;
      font-weight: 100;
      letter-spacing: 0.15px;
      line-height: 0.95;
      font-size: 0.85em;
      word-break: break-all;
      white-space: normal;
      word-wrap: normal;
      color: #282828;
    }

    .card button {
      border: none;
      background-color: #007bff;
      margin-top: 0.5vw;
      margin-bottom: 0.25vw;
      padding: 0.25vw;
      color: white;
      font-size: 0.85em;
      font-family: "Noto Sans KR", sans-serif;
      text-transform: uppercase;
      border-radius: 10px;
    }

    /* Safari 4.0 - 8.0 */
    @-webkit-keyframes line-show {
      from {
        margin: 0px 100px;
      }

      to {
        margin: 0px;
      }
    }

    /* Standard syntax */
    @keyframes line-show {
      from {
        margin: 0px 100px;
      }

      to {
        margin: 0px;
      }
    }

    /* Safari 4.0 - 8.0 */
    @-webkit-keyframes p-show {
      from {
        color: white;
      }

      to {
        color: #282828;
      }
    }

    /* Standard syntax */
    @keyframes p-show {
      from {
        color: white;
      }

      to {
        color: #282828;
      }
    }

    /* Safari 4.0 - 8.0 */
    @-webkit-keyframes shadow-show {
      from {
        box-shadow: 0px 0px 0px 0px #e0e0e0;
      }

      to {
        box-shadow: -20px -20px 0px 0px #fb968b;
      }
    }

    /* Standard syntax */
    @keyframes shadow-show {
      from {
        box-shadow: 0px 0px 0px 0px #e0e0e0;
      }

      to {
        box-shadow: -20px -20px 0px 0px #fb968b;
      }
    }

    .over {
      border: 2px dashed #000;
    }

    [draggable] {
      -moz-user-select: none;
      -khtml-user-select: none;
      -webkit-user-select: none;
      user-select: none;
    }

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

    #questionario-fase {
      display: block;
      height: 300px;
      width: 100%;
    }
  </style>

  <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='styleswitcher'>

  <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='headerswitcher'>
</head>

<style>
  #page-content {
    min-height: 100% !important;
  }
</style>

<body class="horizontal-nav">
  <?php include('menu.php'); ?>

  <div id="pageCtn" class="page-container">
    <div class="hidden modal_hidden" id="emailSindModalHidden">
      <div id="emailSindModalHiddenContent">
        <div class="modal-content">
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Enviar E-mail</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <form class="form-horizontal" id="emailForm">
                  <div class="form-group">
                    <h4>Templates:</h4>
                    <p id="sindTemplateBtn1" class="col-sm-4">
                      <a class="form-control" href="#">Ac. Assembleia Patronal</a>
                    </p>
                    <p id="sindTemplateBtn2" class="col-sm-4">
                      <a class="form-control" href="#">Negociação CCT</a>
                    </p>
                  </div>

                  <div class="form-group">
                    <h4>E-mails dos sindicatos:</h4>
                    <p class="col-sm-6">Laboral: <a class="form-control" style="cursor: pointer"
                        id="destinatario-laboral"></a>
                    </p>
                    <p class="col-sm-6">Patronal: <a class="form-control" style="cursor: pointer"
                        id="destinatario-patronal"></a>
                    </p>
                  </div>

                  <h3>Enviar Email: </h3>

                  <div class="form-group">
                    <div class="col-sm-12">
                      <label for="para-input" class="control-label">Para:</label>
                      <input type="text" class="form-control" id="para-input" placeholder="">
                    </div>

                    <div class="col-sm-12">
                      <label for="assunto-input" class="control-label">Assunto:</label>
                      <input type="text" class="form-control" id="assunto-input" placeholder="">
                    </div>

                    <div class="col-sm-12">
                      <label for="msg-input" class="control-label">Mensagem:</label>
                      <textarea class="form-control" id="msg-input" cols="30" rows="10"></textarea>
                    </div>
                  </div>
                </form>

                <div class="btn-toolbar" style="display: flex; justify-content: flex-end;">
                  <button type="button" data-toggle="modal" type="button" class="btn default-alt" data-dismiss="modal"
                    style="margin-right: 10px">Voltar</button>
                  <button type="button" id="dispararEmailsSindBtn" class="btn btn-primary btn-rounded">Enviar Email
                    Único</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="hidden modal_hidden" id="emailCliModalHidden">
      <div id="emailCliModalHiddenContent">
        <div class="modal-content">
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Enviar E-mail</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <form class="form-horizontal" id="emailCliForm">
                  <div class="form-group">
                    <h4>Templates:</h4>
                    <p id="cliTemplateBtn1" class="col-sm-4"><a class="form-control">Ata/Circular Reajuste
                        Salarial</a></p>
                    <p id="cliTemplateBtn2" class="col-sm-4"><a class="form-control">Ac. CCT Cliente
                        (Não consigo contato sindicatos)</a></p>
                    <p id="cliTemplateBtn3" class="col-sm-4"><a class="form-control">Ac. CCT
                        Cliente (tem que abrir CNPJ e nome da empresa)</a></p>
                  </div>

                  <h3>Enviar Email: </h3>

                  <div class="form-group">
                    <div class="col-sm-12">
                      <label for="para-input-cli" class="control-label">Para:</label>
                      <input type="text" class="form-control" id="para-input-cli" placeholder="">
                    </div>

                    <div class="col-sm-12">
                      <label for="assunto-input-cli" class="control-label">Assunto:</label>
                      <input type="text" class="form-control" id="assunto-input-cli" placeholder="">
                    </div>

                    <div class="col-sm-12">
                      <label for="msg-input-cli" class="control-label">Mensagem:</label>
                      <textarea class="form-control" id="msg-input-cli" cols="30" rows="10"></textarea>
                    </div>
                  </div>
                </form>
              </div>

              <div class="btn-toolbar" style="display: flex; justify-content: flex-end;">
                <button type="button" data-toggle="modal" class="btn default-alt" data-dismiss="modal"
                  style="margin-right: 10px;">Voltar</button>
                <button type="button" id="dispararEmailsCliBtn" class="btn btn-primary btn-rounded">Enviar
                  Email(s)</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="hidden modal_hidden" id="cnaeModalHidden">
      <div id="cnaeModalHiddenContent">
        <div class="modal-content">
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione o CNAE contemplado na negociação</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <div class="box text-shadow">
                  <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                    class="table table-striped table-bordered demo-tbl" id="cnaetb">
                  </table>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <div class="col-sm-12">
              <div class="btn-toolbar" style="display: flex; justify-content: flex-end;">
                <button data-toggle="modal" data-dismiss="modal" class="btn btn-secondary">Seguinte</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="hidden modal_hidden" id="empresaModalHidden">
      <div id="empresaModalContent">
        <div class="modal-content">
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione a Empresa</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <div class="box text-shadow">
                  <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                    class="table table-striped table-bordered demo-tbl" id="empresatb" data-order='[[ 1, "asc" ]]'>
                  </table>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <div class="col-sm-12">
              <div class="btn-toolbar" style="display: flex; justify-content: flex-end;">
                <button data-toggle="modal" data-dismiss="modal" class="btn btn-secondary">Seguinte</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="hidden modal_hidden" id="grupoEconomicoModalHidden">
      <div id="grupoEconomicoModalContent">
        <div class="modal-content">
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione o Grupo Ecnonômico</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <div class="box text-shadow">
                  <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                    class="table table-striped table-bordered demo-tbl" id="grupoEconomicoTb"
                    data-order='[[ 1, "asc" ]]'>
                  </table>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <div class="col-sm-12">
              <div class="btn-toolbar" style="display: flex; justify-content: flex-end;">
                <button data-toggle="modal" data-dismiss="modal" class="btn btn-secondary">Seguinte</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="hidden modal_hidden" id="localizacaoModalHidden">
      <div id="localizacaoModalHiddenContent">
        <div class="modal-content">
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione a Abrangência contemplado na negociação</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <div class="box text-shadow">
                  <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                    class="table table-striped table-bordered demo-tbl" id="localizacaotb">
                  </table>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <div class="col-sm-12">
              <div class="btn-toolbar" style="display: flex; justify-content: flex-end;">
                <button data-toggle="modal" data-dismiss="modal" class="btn btn-secondary">Seguinte</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="hidden modal_hidden" id="futurasModalHidden">
      <div id="futurasModalHiddenContent">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Acompanhamentos Agendados</h4>
          </div>

          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione as negociações a serem abertas</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <div class="row">
                  <div class="col-sm-2">
                    <label for="status-futuras-input">Prioridade</label>
                    <select class="form-control" id="status-futuras-input"></select>
                  </div>

                  <div class="col-sm-4">
                    <label for="fase-input">Fase</label>
                    <select class="form-control" id="fases-futuras-input"></select>
                  </div>

                  <div class="col-sm-4">
                    <label>Tipo Documento</label>
                    <select class="form-control select2" id="tipodoc-fut-input">
                    </select>
                  </div>
                </div>

                <div class="box text-shadow">
                  <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                    class="table table-striped table-bordered demo-tbl" id="futurastb" data-order='[[ 1, "asc" ]]'>
                  </table>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <div class="row">
              <div class="col-sm-12">
                <div class="btn-toolbar" style="display: flex; justify-content: flex-end;">
                  <button id="processarAcompanhamentosFuturosBtn" class="btn btn-primary btn-rounded"
                    style="margin-right: 5px;">Processar</button>
                  <button type="button" data-toggle="modal" class="btn btn-danger btn-rounded btn-cancelar"
                    data-dismiss="modal">Voltar</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="hidden modal_hidden" id="scriptHistModalHidden">
      <div id="scriptHistModalHiddenContent">
        <div class="modal-content">
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Histórico</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in" style="max-height: 70vh; overflow-y: auto">
                <div class="container">
                  <div class="row">
                    <div class="col-md-12">
                      <h4 class="timeline-month" id="timeline-id"><span></span></h4>
                      <ul class="timeline" id="history-timeline">
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <a data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</a>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL PARA CALENDÁRIO -->
    <div class="hidden modal_hidden" id="calendarModalHidden">
      <div id="calendarModalHiddenContent">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Cadastro de Reunião</h4>
          </div>
          <div id="mensagem_sucesso" class="alert alert-dismissable alert-success">
            Cadastro realizado com sucesso!
          </div>
          <div id="mensagem_error" class="alert alert-dismissable alert-danger">
            Não foi possível realizar essa operação!
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <form class="form-horizontal">
                <div class="panel-heading">
                  <h4>Cadastro</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>

                <div class="panel-body collapse in principal-table">
                  <div class="form-group">
                    <div class="col-sm-4">
                      <label for="tiporeu-input">Tipo de Reunião:</label>
                      <select class="form-control" id="tiporeu-input">
                        <optgroup label="SELECIONE">
                          <option value="Presencial">Presencial</option>
                          <option value="Online">Online</option>
                        </optgroup>
                      </select>
                    </div>

                    <div class="col-sm-4">
                      <label for="datareu-input">Data</label>
                      <input type="date" class="form-control" id="datareu-input">
                    </div>

                    <div class="col-sm-4">
                      <label for="horareu-input">Hora</label>
                      <input type="text" class="form-control" id="horareu-input" placeholder="00:00">
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="col-sm-12">
                      <label for="localreu-input">Local</label>
                      <input type="text" class="form-control" id="localreu-input" placeholder="">
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="col-md-12">
                      <label for="obsreu-input" class="control-label">Observação</label>
                      <textarea class="form-control" id="obsreu-input" cols="30" rows="10"></textarea>
                    </div>
                  </div>


                </div>
              </form>
              <div class="row" style="margin-top: 20px;">
                <div class="col-sm-12">
                  <div class="btn-toolbar" style="display: flex; justify-content: left;">
                    <a href="#" class="btn btn-primary btn-rounded" onclick="addCalendar();"
                      style="margin-right: 5px;">Processar</a>
                    <a id="btn-c" data-toggle="modal" href="#updateModal"
                      class="btn btn-danger btn-rounded btn-cancelar" data-dismiss="modal">Finalizar</a>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div><!-- /.modal-content -->
      </div><!-- /.modal-dialog -->
    </div><!-- /.modal -->

    <div class="hidden modal_hidden" id="cadastrarModalHidden">
      <div id="cadastrarModalHiddenContent">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Cadastro de acompanhamentos</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione o CNAE contemplado na negociação</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>

              <div class="panel-body collapse in principal-table">
                <form class="form-horizontal" id="formAdd">
                  <input type="hidden" id="id-input">
                  <div class="form-group">
                    <div class="col-sm-2">
                      <label for="status-input">Prioridade</label>
                      <select class="form-control" id="status-input"></select>
                    </div>

                    <div class="col-sm-2">
                      <label for="fase-input">Fase</label>
                      <select class="form-control" id="fase-input"></select>
                    </div>

                    <div class="col-sm-1">
                      <label for="db-input">Data Base</label>
                      <select class="form-control" id="db-input">
                        <optgroup label="SELECIONE">
                          <option value="JAN">JAN</option>
                          <option value="FEV">FEV</option>
                          <option value="MAR">MAR</option>
                          <option value="ABR">ABR</option>
                          <option value="MAI">MAI</option>
                          <option value="JUN">JUN</option>
                          <option value="JUL">JUL</option>
                          <option value="AGO">AGO</option>
                          <option value="SET">SET</option>
                          <option value="OUT">OUT</option>
                          <option value="NOV">NOV</option>
                          <option value="DEZ">DEZ</option>
                        </optgroup>
                      </select>
                    </div>
                    <div class="col-sm-1">
                      <label for="dbano-inputu">Ano</label>
                      <input type="text" id="dbano-input" class="form-control" placeholder="xxxx">
                    </div>

                    <div class="col-sm-2">
                      <label for="dataini-input">Validade inicial</label>
                      <input type="date" class="form-control" id="dataini-input">
                    </div>

                    <div class="col-sm-2">
                      <label for="dataini-input">Validade final</label>
                      <input type="date" class="form-control" id="validade-final">
                    </div>

                    <div class="col-sm-2">
                      <label for="datafim-input">Data Fim (Previsto)</label>
                      <input type="date" class="form-control" id="datafim-input" disabled>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="col-sm-3">
                      <label>Sindicato Laboral</label>
                      <select class="form-control select2" id="sind-input" multiple>
                      </select>
                    </div>

                    <div class="col-sm-3">
                      <label>Sindicato Patronal</label>
                      <select class="form-control select2" id="emp-input" multiple>
                      </select>
                    </div>

                    <div class="col-sm-3">
                      <label>Nome do Documento</label>
                      <select class="form-control select2" id="tipodoc-input">
                      </select>
                    </div>

                    <div class="col-sm-3">
                      <label>Responsável</label>
                      <select class="form-control select2" id="resp-input"></select>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="col-sm-3">
                      <label for="data-processamento-input">Data Processamento Ineditta</label>
                      <input type="text" class="form-control" id="data-processamento-input">
                    </div>

                    <div class="col-sm-4">
                      <label>Etiquetas</label>
                      <select class="form-control select2" id="etiqueta" multiple></select>
                    </div>

                    <div class="col-sm-5">
                      <label>Assuntos</label>
                      <select class="form-control select2" id="assunto" multiple></select>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="col-sm-12">
                      <label for="anotacoes-input">Anotações</label>
                      <textarea rows="5" type="text" class="form-control" id="anotacoes-input"
                        style="resize: none;"></textarea>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="col-sm-3" style="padding: 20px;">
                      <button type="button" class="btn btn-primary btn-rounded col-md-12" data-toggle="modal"
                        data-target="#cnaeModal" id="addCnaeBtn">Atividades Econômicas</button>
                    </div>

                    <div class="col-sm-3" style="padding: 20px;">
                      <button type="button" class="btn btn-primary btn-rounded col-md-12" data-toggle="modal"
                        data-target="#empresaModal" id="empresaModalBtn">Empresas</button>
                    </div>

                    <div class="col-sm-3" style="padding: 20px;">
                      <button type="button" class="btn btn-primary btn-rounded col-md-12" data-toggle="modal"
                        data-target="#grupoEconomicoModal" id="grupoEconomicoModalBtn">Grupos Econômicos</button>
                    </div>

                    <div class="col-sm-3" style="padding: 20px;">
                      <button type="button" class="btn btn-primary btn-rounded col-md-12" data-toggle="modal"
                        data-target="#localizacaoModal" id="localizacaoBtn">Abrangência</button>
                    </div>
                  </div>

                  <div id="edicaoFields">
                    <div class="form-group" style="margin-top: 30px;">
                      <div class="col-sm-6">
                        <label>Selecionar Sindicato Laboral</label>
                        <select class="form-control select2" id="sindicato-laboral-filtro">
                        </select>
                      </div>
                      <div class="col-sm-6">
                        <label>Selecionar Sindicato Patronal</label>
                        <select class="form-control select2" id="sindicato-patronal-filtro">
                        </select>
                      </div>
                    </div>
                    <div class="form-group center">
                      <div class="col-sm-2">
                        <input type="text" id="sind_laboral_email_1" class="form-control" placeholder="E-mails"
                          disabled>
                      </div>
                      <div class="col-sm-2">
                        <input type="text" id="sind_laboral_email_2" class="form-control" placeholder="E-mails"
                          disabled>
                      </div>
                      <div class="col-sm-2">
                        <input type="text" id="sind_laboral_email_3" class="form-control" placeholder="E-mails"
                          disabled>
                      </div>

                      <div class="col-md-2">
                        <input type="text" id="sind_patronal_email_1" class="form-control" placeholder="E-mails"
                          disabled>
                      </div>
                      <div class="col-md-2">
                        <input type="text" id="sind_patronal_email_2" class="form-control" placeholder="E-mails"
                          disabled>
                      </div>
                      <div class="col-md-2">
                        <input type="text" id="sind_patronal_email_3" class="form-control" placeholder="E-mails"
                          disabled>
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-sm-2">
                        <input type="text" id="sind_laboral_telefone_1" class="form-control" placeholder="Telefone"
                          disabled>
                      </div>
                      <div class="col-sm-2">
                        <input type="text" id="sind_laboral_telefone_2" class="form-control" placeholder="Telefone"
                          disabled>
                      </div>
                      <div class="col-sm-2">
                        <input type="text" id="sind_laboral_telefone_3" class="form-control" placeholder="Telefone"
                          disabled>
                      </div>

                      <div class="col-md-2">
                        <input type="text" id="sind_patronal_telefone_1" class="form-control" placeholder="Telefone"
                          disabled>
                      </div>
                      <div class="col-md-2">
                        <input type="text" id="sind_patronal_telefone_2" class="form-control" placeholder="Telefone"
                          disabled>
                      </div>
                      <div class="col-md-2">
                        <input type="text" id="sind_patronal_telefone_3" class="form-control" placeholder="Telefone"
                          disabled>
                      </div>

                    </div>

                    <div class="form-group center">
                      <div class="col-sm-6">
                        <input type="text" id="sind_laboral_link_site" class="form-control" placeholder="Link site"
                          disabled>
                      </div>

                      <div class="col-md-6">
                        <input type="text" id="sind_patronal_link_site" class="form-control" placeholder="Link site"
                          disabled>
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-sm-6">
                        <textarea class="form-control" id="sind_laboral_coment" cols="30" rows="10"
                          placeholder="Comentários" disabled></textarea>
                      </div>

                      <div class="col-md-6">
                        <textarea class="form-control" id="sind_patronal_coment" cols="30" rows="10"
                          placeholder="Comentários" disabled></textarea>
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-sm-6">
                        <textarea class="form-control" id="sind_laboral_atv" cols="30" rows="10"
                          placeholder="Comentários" disabled></textarea>
                      </div>

                      <div class="col-md-6">
                        <textarea class="form-control" id="sind_patronal_atv" cols="30" rows="10"
                          placeholder="Comentários" disabled></textarea>
                      </div>
                    </div>

                    <div class="form-group center">
                      <div class="col-md-12">
                        <textarea class="form-control" id="client-report" cols="30" rows="10"
                          placeholder="Client Report"></textarea>
                      </div>
                    </div>

                    <div class="form-group" style="padding-top: 20px;">
                      <div class="col-sm-3" style="text-align: center;">
                        <button id="btn-add-script-new" class="btn btn-primary btn-rounded"
                          style="width: 100%;">Adicionar
                          Script</button>
                      </div>

                      <div class="col-md-3">
                        <button type="button" class="btn btn-primary btn-rounded col-md-12" data-toggle="modal"
                          data-target="#emailSindModal">Enviar e-mail sindicatos</button>
                      </div>

                      <div class="col-md-3">
                        <button type="button" class="btn btn-primary btn-rounded col-md-12" data-toggle="modal"
                          data-target="#emailCliModal">Enviar e-mail clientes</button>
                      </div>

                      <div class="col-sm-3" style="text-align: center;">
                        <button type="button" class="btn btn-primary btn-rounded col-md-12" data-toggle="modal"
                          data-target="#scriptHistModal">Ver histórico de acompanhamento</button>
                      </div>
                    </div>

                  </div>
                </form>

                <div id="scriptPanel">
                  <form class="form-horizontal" id="scriptForm">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Questionário da fase</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in" style="max-height: 70vh; overflow-y: scroll">
                        <div class="form-group" id="questionario-fase"></div>
                      </div>
                    </div>

                    <div style="text-align: center;">
                      <button id="btn-save-script" class="btn btn-primary btn-rounded">Salvar Questionário</button>
                    </div>
                  </form>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <div class="row">
                <div class="col-sm-12" style="display: flex; justify-content:flex-end">
                  <button type="button" class="btn btn-primary btn-rounded" id="cadastrarBtn">Salvar</button>
                </div>
              </div>
            </div>
          </div>
        </div><!-- /.modal-content -->
      </div><!-- /.modal-dialog -->
    </div><!-- /.modal -->

    <div id="page-content">
      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex; flex-wrap: wrap;">
            <div class="col-md-6 img_container">
              <div class="container_logo_client">
                <img id="imglogo" class="img-circle">
              </div>
            </div>

            <div class="col-md-6" style="width: 87.6%;">
              <table class="table table-striped col-md-12" style="margin-bottom: 0;">
                <tbody>
                  <tr>
                    <td>
                      <?php if ($thisModule->Criar == 1): ?>
                        <button type="button" class="btn default-alt" data-toggle="modal" data-target="#cadastrarModal"
                          id="novoAcompanhamentoBtn">NOVO ACOMPANHAMENTO</button>
                        <button type="button" class="btn default-alt" data-toggle="modal" data-target="#futurasModal"
                          id="verAcompanhamentosBtn">VER ACOMPANHAMENTOS
                          AGENDADOS</button>
                      <?php else: ?>

                      <?php endif; ?>
                    </td>
                  </tr>
                </tbody>
              </table>

              <div class="col-md-12" style="background: #f7f8fa; padding: 11px 3px;">
                <div class="col-md-3" style="display: flex; align-items: center; gap: 10px;">
                  <label style="white-space: nowrap;">Negociações em Aberto:</label>
                  <p style="margin-bottom: 0;"><strong id="neg_aberto"></strong></p>
                </div>

                <div class="col-md-3" style="display: flex; align-items: center; gap: 10px;">
                  <label style="white-space: nowrap;">Sem Movimentação a 30 dias:</label>
                  <p style="margin-bottom: 0;"><strong id="sem_mov"></strong></p>
                </div>

                <div class="col-md-3" style="display: flex; align-items: center; gap: 10px;">
                  <label style="white-space: nowrap;">Qtd. Ligações do dia:</label>
                  <p style="margin-bottom: 0;"><strong id="count_ligacoes"></strong></p>
                </div>
              </div>
            </div>

            <div class="col-md-12 content_container" style="display: block; margin: 20px 0; width: 100%;">
              <!-- ACOMPANHAMENTO -->
              <div class="panel panel-primary" style="margin-top: 0;">
                <div class="panel-heading">
                  <h4>Acompanhamentos</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <div class="box text-shadow">
                    <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="acompanhamentoccttb"
                      data-order='[[ 8, "desc" ]]'>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div> <!-- container -->
      </div> <!-- #wrap -->
    </div> <!-- page-content -->
  </div> <!-- page-container -->

  <?php include 'footer.php' ?>

  <script type='text/javascript' src="./js/acompanhamento_db.min.js"></script>
</body>

</html>