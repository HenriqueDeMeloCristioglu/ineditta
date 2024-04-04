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

$fileClassCalendario = $path . '/includes/php/class.docsind.php';

if (file_exists($fileClassCalendario)) {

  include_once($fileClassCalendario);
  include_once __DIR__ . "/includes/php/class.usuario.php";

  $user = new usuario();
  $userData = $user->validateUser($sessionUser)['response_data']['user'];

  $modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

  $modulos = ["sisap" => $modulosSisap, "comercial" => []];

  $sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

  foreach ($sisap as $key => $value) {
    if (mb_strpos($value, "Documento Sindical")) {
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

  <link rel="stylesheet" href="docsind.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    .modal-body {
      overflow-y: scroll;
      max-height: 80vh;
    }

    .hidden {
      display: none;
    }

    .flex-centered {
      display: flex;
      justify-content: center;
      align-items: center;
    }

    .mx-3 {
      margin-left: 12px;
      margin-right: 12px;
    }

    .w-full {
      width: 100%;
    }

    #atualizarSLAModal-content {
      width: 650px !important;
      margin: 0 auto !important;
    }

    .d-flex {
      display: flex;
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
            <div class="col-md-1 img_container">
              <div class="container_logo_client">
                <img id="imglogo" class="img-circle">
              </div>
            </div>

            <div class="col-md-11 content_container">
              <table class="table table-striped">
                <tbody>
                  <tr>
                    <td>
                      <?php if ($thisModule->Criar == 1): ?>
                        <button class="btn default-alt" id="documentoSindicalModalBtn">NOVO DOCUMENTO SINDICAL</button>
                        <button data-toggle="modal" data-target="#documentosLocalizadosModal" class="btn default-alt"
                          id="documentosLocalizadosModalBtn">DOCUMENTOS
                          LOCALIZADOS</button>
                      <?php else: ?>
                        <button class="btn default-alt disabled" id="documentoSindicalModalBtn">NOVO DOCUMENTO
                          SINDICAL</button>
                        <button data-toggle="modal" data-target="#documentosLocalizadosModal"
                          class="btn default-alt disabled" id="documentosLocalizadosModalBtn">DOCUMENTOS
                          LOCALIZADOS</button>
                      <?php endif; ?>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- MODAL ATUALIZAR DATA SLA -->
          <button data-toggle="modal" data-target="#atualizarSLAModal" class="hidden"
            id="showAtualizarSLAModalBtn"></button>
          <div class="hidden" id="atualizarSLAModalHidden">
            <div id="atualizarSLAModalContent">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title">Data SLA</h4>
              </div>
              <div class="modal-body">
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Atualizar Data SLA</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in">
                    <div id="grid-layout-table-2" class="box jplist">
                      <div class="box text-shadow">
                        <div class="row">
                          <div class="col-sm-10">
                            <input type="text" class="form-control" id="nova_data_sla" placeholder="DD/MM/AAAA">
                          </div>
                          <div class="col-sm-2">
                            <button id="salvarNovaDataSLABtn" class="btn btn-primary btn-rounded w-full">Salvar</button>
                          </div>
                        </div>
                      </div>
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

          <!-- MODAL ABRANGENCIA -->
          <div class="hidden" id="abrangenciaModalHidden">
            <div id="abrangenciaModalContent">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title">Abrangência</h4>
              </div>
              <div class="modal-body">

                <!-- PAINEL DE ABRANGÊNCIAS SELECIONADAS -->
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Abrangências Selecionadas</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>

                  <div class="panel-body collapse in">
                    <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                      <input type="checkbox" id="selecionar_todos_municipios_selecionados">
                      <label for="selectAll">Selecionar Todos</label>
                    </div>
                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered datatables" id="abrangenciasSelecionadasTb"
                      data-order='[[ 0, "asc" ]]' style="width: 100%;">
                      <thead>
                        <tr>Selecione</tr>
                        <tr>Município</tr>
                        <tr>Estado</tr>
                        <tr>País</tr>
                      </thead>
                    </table>

                    <div>
                      <button class="btn btn-primary" type="button" id="btn_remover_abrangencias_selecionadas">
                        Excluir
                      </button>

                      <button class="btn btn-primary" type="button" id="btn_atualizar_abrangencias_selecionadas">
                        Atualizar
                      </button>
                    </div>
                  </div>
                </div>

                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Selecione a Abrangência do Documento</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in">
                    <div class="row" style="margin-bottom: 20px ;">
                      <div class="col-lg-4" style="margin-top: 10px;">
                        <label for="estado-input">Selecione o Estado</label>
                        <select class="form-control uf-input-abrang" id="uf"></select>
                      </div>

                      <div class="col-lg-6">
                        <div class="btn-toolbar btn-control btn-box" style="margin-top: 30px;">
                          <button style="width: 150px;" id="btn_add_abrangencia"
                            class="btn btn-primary btn-rounded">Adicionar</button>
                          <button style="width: 150px; margin-left: 10px;" id="btn_reset_abrangencia"
                            class="btn btn-primary btn-rounded">Resetar</button>
                        </div>
                      </div>
                    </div>

                    <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                      <input type="checkbox" id="seleciona_todas_regioes">
                      <label for="selectAll">Selecionar Todos os Municípios</label>
                    </div>
                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered datatables" id="abrangenciaTb"
                      data-order='[[ 0, "asc" ]]' style="width: 100%;">
                      <thead>
                        <tr>Selecione</tr>
                        <tr>Município</tr>
                        <tr>País</tr>
                      </thead>
                    </table>
                  </div>
                </div>
              </div>

              <div class="modal-footer">
                <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
                  data-dismiss="modal">Voltar</button>
              </div>
            </div>
          </div>

          <!-- MODAL DOCUMENTOS LOCALIZADOS -->
          <div class="hidden" id="documentosLocalizadosModalHidden">
            <div id="documentosLocalizadosModalContent">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title">Documentos Localizados</h4>
              </div>
              <div class="modal-body">
                <div class="row" style="margin: 16px 0 16px 0;">
                  <form enctype="multipart/form-data">
                    <div class="col-lg-4">
                      <input type="file" class="form-control" id="file" name="documentoLocalizadoUpload">
                    </div>
                    <div class="col-lg-3">
                      <select data-placeholder="ORIGEM DO DOCUMENTO" class="form-control select2"
                        id="origem_doc"></select>
                    </div>
                    <div class="col-lg-3">
                      <select data-placeholder="UF" class="form-control select2" id="uf_doc_localizado"></select>
                    </div>
                    <div class="col-lg-2">
                      <button class="btn btn-primary" type="button" id="adicionar"><i style="margin-right: 10px;"
                          class="fa fa-plus-circle"></i>Adicionar Arquivo</button>
                    </div>
                  </form>
                </div>
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Lista de Documentos</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in">
                    <div id="grid-layout-table-docs" class="box jplist">
                      <div class="box text-shadow">
                        <table cellpadding="0" cellspacing="0" border="0"
                          class="table table-striped table-bordered demo-tbl" id="documentosLocalizadosTb"
                          data-order='[[ 1, "asc" ]]' style="width: 100%;"></table>
                      </div>
                    </div>
                  </div>
                </div>
                <!-- VISUALIZAÇÃO DO ARQUIVO -->
                <div class="panel panel-primary" style="margin-top: 24px;">
                  <div class="panel-heading">
                    <h4>Visualização do arquivo</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in" id="embed">
                    <embed id="embed_pdf" src="" type="application/pdf" width="100%" height="100%">
                  </div>
                </div>
              </div>
              <div class="modal-footer">
                <button data-toggle="modal" href="#myModalAddSinonimo" type="button" class="btn btn-secondary"
                  data-dismiss="modal">Seguinte</button>
              </div>
            </div>
          </div>

          <!-- MODAL EMPRESA -->
          <div class="hidden" id="filialModalHidden">
            <div id="filialModalContent">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title">Empresa</h4>
              </div>
              <div class="modal-body">

                <!-- PAINEL DE EMPRESAS SELECIONADAS -->
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Empresas Selecionadas</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>

                  <div class="panel-body collapse in">
                    <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                      <input type="checkbox" id="selecionar_todas_empresas_selecionadas">
                      <label for="selectAll">Selecionar Todos</label>
                    </div>

                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered datatables" id="clienteUnidadeSelecionadosTb"
                      data-order='[[ 0, "asc" ]]' style="width: 100%;">
                    </table>

                    <div>
                      <button class="btn btn-primary" type="button" id="btn_remover_empresas_selecionadas">
                        Excluir
                      </button>

                      <button class="btn btn-primary" type="button" id="btn_atualizar_empresas_selecionadas">
                        Atualizar
                      </button>
                    </div>
                  </div>
                </div>

                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Selecione a Empresa</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in">
                    <div id="grid-layout-table-2" class="box jplist">
                      <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                        <input type="checkbox" id="selecionar_todas_empresas">
                        <label for="selectAll">Selecionar Todos</label>
                      </div>
                      <div class="box text-shadow">
                        <table cellpadding="0" cellspacing="0" border="0"
                          class="table table-striped table-bordered demo-tbl" id="clienteUnidadeTb"
                          data-order='[[ 0, "asc" ]]' style="width: 100%;"></table>
                      </div>
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

          <!-- MODAL ATIVIDADE ECONOMICA -->
          <div class="hidden" id="atividadeEconomicaModalHidden">
            <div id="atividadeEconomicaModalContent">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title">Atividade Econômica</h4>
              </div>
              <div class="modal-body">

                <!-- PAINEL DE ATIVIDADES ECONÔMICAS SELECIONADAS -->
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Atividades Econômicas Selecionadas</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>

                  <div class="panel-body collapse in">
                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered datatables" id="atividadesEconomicasSelecionadasTb"
                      data-order='[[ 0, "asc" ]]' style="width: 100%;">
                    </table>

                    <div>
                      <button class="btn btn-primary" type="button" id="btn_remover_atividades_economicas_selecionadas">
                        Excluir
                      </button>

                      <button class="btn btn-primary" type="button"
                        id="btn_atualizar_atividades_economicas_selecionadas">
                        Atualizar
                      </button>
                    </div>
                  </div>
                </div>

                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Selecione a Atividade Econômica</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in">
                    <div id="grid-layout-table-2" class="box jplist">
                      <div class="box text-shadow">
                        <table cellpadding="0" cellspacing="0" border="0"
                          class="table table-striped table-bordered demo-tbl" id="atividadeEconomicaTb"
                          data-order='[[ 0, "asc" ]]' style="width: 100%;"></table>
                      </div>
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

          <!-- Novo Cadastro Doc Sind -->
          <button data-toggle="modal" data-target="#documentoSindicalModal" class="hidden"
            id="showDocumentoSindicalModalBtn"></button>
          <div class="hidden" id="documentoSindicalModalHidden">
            <div id="documentoSindicalModalContent">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title">Cadastro de Documento Sindical</h4>
              </div>
              <div class="modal-body">
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Novo Cadastro</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in">
                    <form class="form-horizontal" id="documentoSindicalForm">
                      <div class="form-group" id="no_reference">
                        <div class="col-lg-4 required" id="group_reference">
                          <label for="doc-referencia">Selecionar Documento de Referência (Aprovado)</label>
                          <select id="doc-referencia" class="form-control"> </select>
                        </div>
                        <div class="col-lg-1">
                          <button id="btn_ver_arquivo_referencia" type="button" class="btn btn-primary"
                            title="Visualizar arquivo" style="margin-top: 20px;"><i id="icon_ver_arquivo_referencia"
                              style="font-size: 1.2em; margin-right: 0;" class="fa fa-eye"></i></button>
                        </div>

                        <div class="col-sm-2">
                          <label for="data_base" class="control-label">Data Base</label>
                          <input type="text" class="form-control" id="data_base" placeholder="MM/AAAA">
                        </div>

                        <div class="col-lg-4"
                          style="margin-top: 34px; display:flex; align-items: center; flex-direction:row;">
                          <input type="checkbox" style="margin: 0 10px;" id="restrito">
                          <label for="restrito" style="margin: 0;">Documento Restrito</label>
                        </div>
                      </div>
                      <div class="form-group">
                        <div class="col-sm-3 required">
                          <label for="unidade_cliente" class="control-label">Tipo Unidade Cliente</label>
                          <select data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="unidade_cliente"></select>
                        </div>

                        <div class="col-sm-3 required">
                          <label for="tipo_doc" class="control-label">Nome Documento</label>
                          <select data-placeholder="Selecione" class="form-control select2" id="tipo_doc"></select>
                        </div>

                        <div class="col-sm-3">
                          <button data-toggle="modal" data-target="#filialModal"
                            class="btn btn-primary btn-rounded btn-select col-sm-12"
                            style="margin-top: 27px;">Filial</button>
                        </div>

                        <div class="col-sm-3">
                          <button data-toggle="modal" data-target="#atividadeEconomicaModal"
                            class="btn btn-primary btn-rounded btn-select col-sm-12" style="margin-top: 27px;">Atividade
                            Econômica</button>
                        </div>
                      </div>

                      <div class="form-group">
                        <div class="col-sm-5">
                          <label for="sind_laborais" class="control-label">Sindicato Laboral</label>
                          <select multiple data-placeholder="Sigla / Denominação / CNPJ" tabindex="8"
                            class="form-control select2" id="sind_laborais"></select>
                        </div>

                        <div class="col-sm-5">
                          <label for="sind_patronais" class="control-label">Sindicato Patronal</label>
                          <select multiple data-placeholder="Sigla / Denominação / CNPJ" tabindex="8"
                            class="form-control select2" id="sind_patronais"></select>
                        </div>

                        <div class="col-sm-2">
                          <button data-toggle="modal" data-target="#abrangenciaModal"
                            class="btn btn-primary btn-rounded btn-select col-sm-12"
                            style="margin-top: 27px; margin-right: 5px;"><i
                              class="fa fa-map-marker"></i>Abrangência</button>
                        </div>
                      </div>
                      <div class="form-group">
                        <div class="col-sm-2 required">
                          <label for="origem" class="control-label">Origem</label>
                          <select class="form-control" id="origem"></select>
                        </div>
                        <div class="col-sm-2 required">
                          <label for="versao-input" class="control-label">Versão</label>
                          <select class="form-control" id="versao"></select>
                        </div>
                        <div class="col-sm-3">
                          <label for="numero_solicitacao" class="control-label">Nº Solicitação</label>
                          <input type="text" class="form-control" id="numero_solicitacao">
                        </div>
                        <div class="col-sm-2">
                          <label for="data-reg-input" class="control-label">Data Registro MTE</label>
                          <input type="text" class="form-control" id="data_registro" placeholder="DD/MM/AAAA">
                        </div>
                        <div class="col-sm-3">
                          <label for="numero_registro" class="control-label">Numero Registro MTE</label>
                          <input type="text" class="form-control" id="numero_registro">
                        </div>
                      </div>
                      <div class="form-group"> <!--  class="form-group" -->
                        <div class="col-sm-2 required">
                          <label for="validade_inicial" class="control-label">Validade Inicial</label>
                          <input type="text" class="form-control" id="validade_inicial" placeholder="DD/MM/AAAA">
                        </div>
                        <div class="col-sm-2 required" id="group_val_fim">
                          <label id="vfim_label" for="validade_final" class="control-label">Validade Final</label>
                          <input type="text" class="form-control" id="validade_final" placeholder="DD/MM/AAAA">
                        </div>
                        <div class="col-sm-2" id="group_prorroga">
                          <label id="pro_label" for="prorrogacao" class="control-label">Prorrogação</label>
                          <input type="text" class="form-control" id="prorrogacao" placeholder="DD/MM/AAAA">
                        </div>
                        <div class="col-sm-2">
                          <label for="data_assinatura" class="control-label">Data da Assinatura</label>
                          <input type="text" class="form-control" id="data_assinatura" placeholder="DD/MM/AAAA">
                        </div>
                        <div class="col-sm-4">
                          <label for="permissao_compartilhamento" class="control-label">Permitir o compartilhamento
                            de dados</label>
                          <select class="form-control" id="permissao_compartilhamento"></select>
                        </div>
                      </div>

                      <div class="form-group">
                        <div class="col-sm-12">
                          <label for="referenciamento" class="control-label">Referenciamento</label>
                          <select multiple data-placeholder="Selecione" id="referenciamento"
                            class="form-control select2"></select>
                        </div>
                      </div>

                      <div class="form-group">
                        <div class="col-sm-12">
                          <label for="observacoes" class="control-label">Observação</label>
                          <textarea class="form-control " id="observacoes" cols="15" rows="5"></textarea>
                        </div>
                      </div>
                    </form>

                    <div class="row">
                      <div class="col-sm-12 flex-centered">
                        <div id="btn-toolbar" style="display: flex; justify-content:center;">
                          <button id="documentoSindicalModalUpsertBtn"
                            class="btn btn-primary btn-rounded">Salvar</button>
                        </div>

                        <div id="btn-toolbar" style="display: flex; justify-content:center;">
                          <button id="documentoSindicalModalAprovarBtn"
                            class="btn btn-primary btn-rounded mx-3">Aprovar</button>
                        </div>

                        <div id="btn-toolbar" style="display: flex; justify-content:center; gap: 20px;">
                          <button type="button" class="btn btn-primary btn-rounded" id="enviar_emails_btn">Enviar
                            Emails</button>
                            
                          <button type="button" class="btn btn-primary btn-rounded"
                            id="enviar_email_selecionados_btn">Reenviar e-mails</button>

                          <button type="button" class="btn btn-primary btn-rounded"
                            id="iniciar_scrap_btn">Iniciar Scrap</button>
                        </div>
                      </div>
                    </div>
                  </div>


                </div>

                <div id="iframe" class="panel panel-primary" style="margin-top: 24px;">
                  <div class="panel-heading">
                    <h4>Visualização do arquivo</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in" id="embed_register">
                    <embed id="embed_pdf_register" src="" type="application/pdf" width="100%" height="100%">
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- MODAL EMAILS APROVADOS -->
          <button type="button" class="hidden" data-toggle="modal" data-target="#emailsAprovadosModal"
            id="emails_aprovados_modal_btn"></button>
          <div class="hidden" id="emailsAprovadosModalHidden">
            <div id="emailsAprovadosModalContent">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title">Emails</h4>
              </div>
              <div class="modal-body">
                <div
                  style="width: 100%; display: flex; justify-content: flex-end; align-items: center; margin-top: 20px;">
                  <button id="enviar_emails_aprovados_btn" type="button" class="btn btn-primary">Enviar</button>
                </div>
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Selecione os emails</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in">
                    <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                      <input type="checkbox" id="selecionar_todos_documentos_sindicais_btn">
                      <label for="selectAll">Selecionar Todos</label>
                    </div>
                    <div id="grid-layout-table-2" class="box jplist">
                      <div class="box text-shadow">
                        <table cellpadding="0" cellspacing="0" border="0"
                          class="table table-striped table-bordered demo-tbl" id="emailsTb" data-order='[[ 0, "asc" ]]'
                          style="width: 100%;"></table>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div class="modal-footer">
                <button data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
              </div>
            </div>
          </div>

          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <!-- TELA PRINCIPAL -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro de Documento Sindical</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <div id="grid-layout-table-1" class="box jplist">
                    <div class="box text-shadow">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="documentoSindicalTb"
                        data-order='[[ 1, "asc" ]]' style="width: 100%;"></table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <?php include 'footer.php' ?>

  </div> <!-- page-container -->

  <script type='text/javascript' src="./js/docsind.min.js"></script>
</body>

</html>