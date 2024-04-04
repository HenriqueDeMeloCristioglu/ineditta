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


$fileClassSindical = $path . '/includes/php/class.sindicatoempregados.php';

if (file_exists($fileClassSindical)) {

  include_once($fileClassSindical);

  include_once __DIR__ . "/includes/php/class.usuario.php";

  $user = new usuario();
  $userData = $user->validateUser($sessionUser)['response_data']['user'];

  $modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

  $modulos = ["sisap" => $modulosSisap, "comercial" => []];

  $sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

  foreach ($sisap as $key => $value) {
    if (mb_strpos($value, "Cadastro de Sindicato Empregados")) {
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
} else {
  $response['response_status']['status'] = 0;
  $response['response_status']['error_code'] = $error_code . __LINE__;
  $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.sindicatoempregados).';
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

  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

  <!-- Bootstrap 3.3.7 -->
  <link rel="stylesheet" href="sindicatoempregados.css">

  <!-- Bootstrap Internal -->
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    td {
      word-break: break-all
    }

    #page-content {
      min-height: 100% !important;
    }

    .table {
      width: 100% !important;
      max-width: 100%;
    }

    .flex-centered {
      display: flex;
      align-items: center;
      justify-content: center;  
    }

    .flex {
      display: flex;
    }
    
    .items-end {
      align-items: end;
    }

    .flex-1 {
      flex: 1;
    }

    .w-full {
      width: 100%;
    }

    .ml-3 {
      margin-left: 12px;
    }
  </style>
</head>

<body class="horizontal-nav">

  <?php include('menu.php'); ?>

  <div class="page-container" id="pageCtn">

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
                    <?php if ($thisModule->Criar == 1): ?>
                      <td><button data-toggle="modal" data-target="#sindicatoModal" class="btn default-alt" id="sindicatoNovoBtn">NOVO SINDICATO</button></td>
                    <?php else: ?>

                    <?php endif; ?>
                  </tr>
                </tbody>
              </table>

              <!-- MODAL NOVO SINDICATO -->
              <div class="hidden modal_hidden" id="sindicatoModalHidden">
                <div id="sindicatoModalHiddenContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Novo Cadastro</h4>
                  </div>

                  <div class="modal-body">
                    <div class="panel panel-primary" style="border: 0; box-shadow: none;">
                      <br>
                      <form id="form-sindicato" class="form-horizontal">
                        <input id="id_input" type="hidden" name="id">
                        <div class="form-group">
                          <div class="col-sm-3">
                            <label for="sigla-input" class="control-label"><strong>Sigla*</strong></label>
                            <input type="text" class="form-control" id="sigla-input" placeholder="">
                          </div>
                          <div class="col-sm-4">
                            <label for="cnpj-input" class="control-label"><strong>CNPJ*</strong></label>
                            <input type="text" class="form-control" id="cnpj-input" placeholder="00.000.000-0000/00">
                          </div>
                        </div>

                        <div class="form-group">
                          <div class="col-sm-6">
                            <label for="razaosocial-input" class="control-label"><strong>Razão Social*</strong></label>
                            <input type="text" class="form-control" id="razaosocial-input" placeholder="">
                          </div>
                          <div class="col-sm-6">
                            <label for="denominacao-input" class="control-label"><strong>Denominação*</strong></label>
                            <input type="text" class="form-control" id="denominacao-input" placeholder="">
                          </div>
                        </div>

                        <div class="form-group">
                          <div class="col-sm-4">
                            <label for="cod-input" class="control-label"><strong>Código Sindical*</strong></label>
                            <input type="text" class="form-control" id="cod-input" placeholder="000.000.000.00000-0">
                          </div>
                          <div class="col-sm-8">
                            <label for="situacao-input" class="control-label">Situação</label>
                            <input type="text" class="form-control" id="situacao-input" placeholder="Normal">
                          </div>
                        </div>

                        <div class="form-group">
                          <div class="col-sm-6">
                            <label for="endereco-input" class="control-label"><strong>Logradouro*</strong></label>
                            <input type="text" class="form-control" id="endereco-input" placeholder="">
                          </div>
                          <div class="col-sm-4">
                            <label for="munic-input" class="control-label"><strong>Município*</strong></label>
                            <input type="text" class="form-control" id="munic-input" placeholder="">
                          </div>
                          <div class="col-sm-2">
                            <label for="uf-input" class="control-label"><strong>UF*</strong></label>
                            <select class="form-control" name="uf-input" id="uf-input" placeholder="UF">
                            </select>
                          </div>
                        </div>

                        <div class="form-group">
                          <div class="col-sm-3">
                            <label for="fone1-input" class="control-label"><strong>Telefone 1*</strong></label>
                            <input type="text" class="form-control" id="fone1-input" placeholder="(00) 0000-00000">
                          </div>
                          <div class="col-sm-3">
                            <label for="fone2-input" class="control-label">Telefone 2</label>
                            <input type="text" class="form-control" id="fone2-input" placeholder="(00) 0000-00000">
                          </div>
                          <div class="col-sm-3">
                            <label for="fone3-input" class="control-label">Telefone 3</label>
                            <input type="text" class="form-control" id="fone3-input" placeholder="(00) 0000-00000">
                          </div>
                          <div class="col-sm-3">
                            <label for="ramal-input" class="control-label">Ramal</label>
                            <input type="text" class="form-control" id="ramal-input" placeholder="0000">
                          </div>
                        </div>

                        <div class="form-group">
                          <div class="col-sm-4">
                            <label for="enq-input" class="control-label">Contato Enquadramento</label>
                            <input type="text" class="form-control" id="enq-input" placeholder="">
                          </div>
                          <div class="col-sm-4">
                            <label for="neg-input" class="control-label">Contato Negociador</label>
                            <input type="text" class="form-control" id="neg-input" placeholder="">
                          </div>
                          <div class="col-sm-4">
                            <label for="con-input" class="control-label">Contato Contribuição</label>
                            <input type="text" class="form-control" id="con-input" placeholder="">
                          </div>
                        </div>

                        <div class="form-group">
                          <div class="col-sm-4">
                            <label for="email1-input" class="control-label">E-mail 1</label>
                            <input type="text" class="form-control" id="email1-input" placeholder="">
                          </div>
                          <div class="col-sm-4">
                            <label for="email2-input" class="control-label">E-mail 2</label>
                            <input type="text" class="form-control" id="email2-input" placeholder="">
                          </div>
                          <div class="col-sm-4">
                            <label for="email3-input" class="control-label">E-mail 3</label>
                            <input type="text" class="form-control" id="email3-input" placeholder="">
                          </div>
                        </div>

                        <div class="form-group">
                          <div class="col-sm-2">
                            <label for="twit-input" class="control-label">Twitter</label>
                            <input type="text" class="form-control" id="twit-input" placeholder="">
                          </div>
                          <div class="col-sm-4">
                            <label for="face-input" class="control-label">Facebook</label>
                            <input type="text" class="form-control" id="face-input" placeholder="">
                          </div>
                          <div class="col-sm-2">
                            <label for="insta-input" class="control-label">Instagram</label>
                            <input type="text" class="form-control" id="insta-input" placeholder="">
                          </div>
                          <div class="col-sm-4">
                            <label for="site-input" class="control-label">Site</label>
                            <input type="text" class="form-control" id="site-input" placeholder="">
                          </div>
                        </div>

                        <div class="form-group">
                          <div class="col-sm-2">
                            <label for="grau-input" class="control-label"><strong>Grau*</strong></label>
                            <select name="combo-grau" class="form-control" id="grau-input">
                            </select>
                          </div>

                          <div class="col-sm-2">
                            <label class="control-label"><strong>Status*</strong></label>
                            <select class="form-control" id="status-input">
                            </select>
                          </div>

                          <div class="col-sm-4">
                            <label class="control-label"><strong>Confederação*</strong></label>
                            <div class="flex-centered">
                              <select class="form-control flex-1" id="ass-input" disabled>
                              </select>
                              <button id="confederacaoAbrirModalBtn" type="button" data-toggle="modal" data-target="#confederacaoModal" class="btn btn-primary btn-rounded ml-3">Selecione</button>
                            </div>
                          </div>

                          <div class="col-sm-4">
                            <label class="control-label"><strong>Federação*</strong></label>
                            <div class="flex-centered">
                              <select class="form-control flex-1" id="ass1-input" disabled>
                              </select>
                              <button id="federacaoAbrirModalBtn" type="button" data-toggle="modal" data-target="#federacaoModal" class="btn btn-primary btn-rounded ml-3">Selecione</button>
                            </div>
                          </div>
                        </div>

                        <div class="form-group flex items-end">
                          <div class="col-sm-4">
                            <label class="control-label"><strong>Central Sindical*</strong></label>
                            <div class="flex-centered">
                              <select class="form-control flex-1" id="central-sindical-input" disabled>
                              </select>
                              <button id="centralSindicalAbrirModalBtn" type="button" data-toggle="modal" data-target="#centralSindicalModal" class="btn btn-primary btn-rounded ml-3">Selecione</button>
                            </div>
                          </div>

                          <div class="col-sm-4">
                            <button id="baseTerritorialAbrirModalBtn" type="button" data-toggle="modal" data-target="#baseTerritorialModal" class="btn btn-primary btn-rounded w-full">Definir base territorial*</button>
                          </div>

                          <div class="col-sm-4">
                            <button id="baseTerritorialHistoricoModalBtn" type="button" data-toggle="modal" data-target="#baseTerritorialHistoricoModal" class="btn btn-primary btn-rounded w-full">Histórico da base territorial</button>
                          </div>
                        </div>
                      </form>
                    </div>
                  </div>

                  <div class="modal-footer">
                    <div class="row">
                      <div class="col-sm-12" style="display: flex; justify-content:center">
                        <button type="button" class="btn btn-primary btn-rounded" id="modalSindicatoCadastrarBtn">Salvar</button>
                      </div>
                    </div>
                  </div>

                </div>
              </div>
              
              <!-- MODAL CONFEDERAÇÃO -->
              <div id="confederacaoModalHidden" class="hidden modal_hidden">
                <div id="confederacaoModalHiddenContent">
                  <div class="modal-header">
                    <!-- <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button> -->
                    <h4 class="modal-title">Confederação do sindicato</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Selecione a confederação</h4>
                        <div class="options">
                          <!-- <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a> -->
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <div class="box text-shadow">
                          <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl" id="confederacaotb" data-order='[[ 1, "asc" ]]'>
                            <thead>
                              <tr>
                                <th></th>
                                <th>Sigla</th>
                                <th>CNPJ</th>
                                <th>Área Geoeconômica</th>
                                <th>Telefone</th>
                                <th>Grupo</th>
                                <th>Grau</th>
                              </tr>
                            </thead>
                          </table>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" id="modalConfederacaoCadastrarBtn" data-dismiss="modal">Seguinte</button>
                  </div>
                </div>
              </div>

              <!-- MODAL FEDERAÇÃO -->
              <div id="federacaoModalHidden" class="hidden modal_hidden">
                <div id="federacaoModalHiddenContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Federação do Sindicato</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Selecione a federação</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <div class="box text-shadow">
                          <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl" id="federacaotb">
                            <thead>
                              <tr>
                                <th></th>
                                <th>Sigla</th>
                                <th>CNPJ</th>
                                <th>Área Geoeconômica</th>
                                <th>Telefone</th>
                                <th>Grupo</th>
                                <th>Grau</th>
                              </tr>
                            </thead>
                          </table>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" id="modalFederacaoCadastrarBtn" data-dismiss="modal">Seguinte</button>
                  </div>
                </div>
              </div>

              <!-- MODAL BASE TERRITORIAL -->
              <div id="baseTerritorialModalHidden" class="hidden modal_hidden">
                <div id="baseTerritorialModalHiddenContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Definir Base Territorial</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel">
                      <form class="form-horizontal">
                        <!-- TABLE BASES TERRITORIAIS SELECIONADOS -->
                        <div class="panel panel-primary">
                          <div class="panel-heading">
                            <h4>Base territorial selecionada</h4>
                            <div class="options">
                              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down fa-chevron-modal-inner"> </i></a>
                            </div>
                          </div>
                          <div class="panel-body collapse in">
                            <div class="box text-shadow">
                              <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                                <input type="checkbox" id="selecionar_todas_bases_territoriais">
                                <label for="selectAll">Selecionar Todos</label>
                              </div>

                              <table cellpadding="0" cellspacing="0" border="0" data-order='[[ 0, "asc" ]]' class="table table-striped table-bordered demo-tbl" id="baseTerritoriaisSelecionadastb">
                              </table>

                              <button type="button" class="btn btn-primary" id="removerBaseTerritorialBtn">Remover</button>
                            </div>
                          </div>
                        </div>

                        <!-- SELETOR DO MÊS DE NEGOCIAÇÃO -->
                        <div class="form-group">
                          <div class="col-sm-2">
                            <label for="data-negociacao-input" class="control-label"><strong>Mês Negociação*</strong></label>
                            <select name="combo-data-negociacao" class="form-control" id="data-negociacao-input">
                            </select>
                          </div>
                        </div>

                        <!-- TABLE LOCALIZAÇÕES -->
                        <div class="panel panel-primary">
                          <div class="panel-heading">
                            <h4>Selecione a Localização</h4>
                            <div class="options">
                              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down fa-chevron-modal-inner"></i></a>
                            </div>
                          </div>
                          <div class="panel-body collapse in">
                            <div class="box text-shadow">
                              <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                                <input type="checkbox" id="selecionar_todas_localizacoes">
                                <label for="selectAll">Selecionar Todos</label>
                              </div>

                              <table cellpadding="0" cellspacing="0" border="0" data-order='[[ 0, "asc" ]]' class="table table-striped table-bordered demo-tbl" id="localizacaotb">
                                <thead>
                                  <tr>
                                    <th></th>
                                    <th>Código do País</th>
                                    <th>País</th>
                                    <th>Código da Região</th>
                                    <th>Região</th>
                                    <th>Código da UF</th>
                                    <th>Estado</th>
                                    <th>UF</th>
                                    <th>Código do Município</th>
                                    <th>Município</th>
                                  </tr>
                                </thead>
                              </table>
                            </div>
                          </div>
                        </div>

                        <div class="form-group">
                          <div class="col-sm-12">
                            <div class="panel panel-primary">
                              <div class="panel-heading">
                                <h4>Seleção de CNAE</h4>
                                <div class="options">
                                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down fa-chevron-modal-inner"></i></a>
                                </div>
                              </div>
                              <div class="panel-body collapse in">
                                <div class="box text-shadow">
                                  <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl" id="baseTerritorialtb" data-order='[[ 0, "asc" ]]'>
                                    <thead>
                                      <tr>
                                        <th>Selecionar</th>
                                        <th>Descrição</th>
                                        <th>Subclasse</th>
                                        <th>Categoria</th>
                                      </tr>
                                    </thead>
                                  </table>
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                        <input type="hidden" id="cnaes-input" value="">
                        <input type="hidden" id="cidades-cnaes-input" value="">
                      </form>
                    </div>
                  </div>
                  <div class="modal-footer">
                    <button type="button" class="btn btn-primary" id="modalBaseTerritorialAdicionarBtn">Adicionar Base</button>
                    <button type="button" class="btn btn-secondary" id="modalBaseTerritorialCadastrarBtn" data-dismiss="modal">Terminar e voltar</button>
                  </div>
                </div>
              </div>

              <!-- MODAL HISTORICO BASE TERRITORIAL -->
              <div id="baseTerritorialHistoricoModalHidden" class="hidden modal_hidden">
                <div id="baseTerritorialHistoricoModalHiddenContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Histórico da Base Territorial</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Histórico da Base Territorial</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down fa-chevron-modal-inner"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <div class="box text-shadow">
                          <table cellpadding="0" cellspacing="0" border="0" data-order='[[ 1, "asc" ]]' class="table table-striped table-bordered demo-tbl" id="baseTerritorialHistoricotb">
                            <thead>
                              <tr>
                                <th>Mês Negociação</th>
                                <th>Localização</th>
                                <th>Uf</th>
                                <th>Descrição Subclasse</th>
                                <th>Subclasse</th>
                                <th>Data de início</th>
                                <th>Data de fim</th>
                              </tr>
                            </thead>
                          </table>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
                  </div>
                </div>
              </div>

              <!-- MODAL CENTRAL SINDICAL -->
              <div id="centralSindicalModalHidden" class="hidden modal_hidden">
                <div id="centralSindicalModalHiddenContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Central Sindical</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Selecione a Central Sindical</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down fa-chevron-modal-inner"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <div class="box text-shadow">
                          <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl" id="centralSindicaltb" data-order='[[ 0, "asc" ]]'>
                            <thead>
                              <tr>
                                <th></th>
                                <th>Nome</th>
                                <th>Sigla</th>
                                <th>CNPJ</th>
                              </tr>
                            </thead>
                          </table>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
                  </div>
                </div>
              </div>

              <div id="centralsindicalModal" class="modal fade" tabindex="2" role="dialog" data-backdrop="static"
                aria-labelledby="associacaoModal1Label" aria-hidden="true">
                <div class="modal-dialog" role="document">
                  <div class="modal-content">
                    <div class="modal-header">
                      <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                      </button>
                      <h4 class="modal-title">Central Sindical</h4>
                    </div>
                    <div class="modal-body">
                      <div class="panel panel-primary">
                        <div class="panel-heading">
                          <h4>Selecione a Central Sindical dos empregados</h4>
                          <div class="options">
                            <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                          </div>
                        </div>
                        <div class="panel-body collapse in">
                          <div class="box text-shadow">
                            <table cellpadding="0" cellspacing="0" border="0"
                              class="table table-striped table-bordered demo-tbl" id="centralsindicaltb"
                              data-order='[[ 1, "asc" ]]' style="width: 100%"></table>
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

        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro Sindicato Laboral</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="box text-shadow">
                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="sindicatotb" data-order='[[ 1, "asc" ]]'
                      style="width: 100%">
                      <thead>
                        <tr>
                          <th>Ações</th>
                          <th>Sigla</th>
                          <th>CNPJ</th>
                          <th>E-mail</th>
                          <th>Telefone</th>
                          <th>Cidade</th>
                          <th>UF</th>
                        </tr>
                      </thead>
                    </table>
                  </div>
                </div>
              </div>
              <input type="hidden" id="id-input">
            </div>
          </div>
        </div> <!-- container -->
      </div> <!-- #wrap -->
    </div> <!-- page-content -->

    <?php include 'footer.php' ?>

  </div> <!-- page-container -->
  <script type='text/javascript' src="./js/sindicatoempregados.min.js"></script>

</body>

</html>