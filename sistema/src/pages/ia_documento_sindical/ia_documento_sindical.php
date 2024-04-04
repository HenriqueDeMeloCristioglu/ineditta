<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

$userSession = $_SESSION['login'];

header('Content-type: text/html; charset=UTF-8');
header('Cache-Control: no-cache');

?>
<!DOCTYPE html>
<html lang="pt-br">

<head>
  <meta charset="utf-8">
  <title>Ineditta</title>
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <meta name="description" content="Ineditta">

  <link rel="stylesheet" href="ia_documento_sindical.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    #page-content {
      min-height: 100% !important;
    }

    .actions-wrapper {
      display: flex;
      justify-content: space-between;
      align-items: center;
      width: 100%;
      margin: 10px 0 5px 0;
    }

    .actions-wrapper button:nth-child(1) {
      margin-left: 20px;
    }

    .actions-wrapper div {
      margin-right: 20px;
    }

    .lista_clausulas_body {
      display: flex;
      gap: 10px;
      margin-top: 10px;
      padding-bottom: 20px;
    }

    .ver_documento {
      width: 100%;
      max-width: 600px;
      min-height: 80vh;
      height: 100%;
    }

    .ver_documento_header {
      background-color: #4f8edc;
      width: 100%;
      display: flex;
      justify-content: center;
      align-items: center;
      height: 40px;
    }

    .ver_documento_header embed {
      margin-top: 20px;
    }

    .ver_documento_header h4 {
      color: white;
      font-weight: bold;
      text-align: center;
    }

    .lista_clausulas {
      width: 100%;
      height: 70vh;
      overflow-y: scroll;
      padding-bottom: 20px;
    }

    #lista_clausulas {
      width: 100%;
      display: flex;
      flex-direction: column;
      gap: 20px;
    }

    .clausula_tabela {
      width: 100%;
      margin-top: 10px;
    }

    .clausula_tabela thead {
      background: #4f8edc;
    }

    .clausula_tabela thead tr th {
      color: white;
      font-size: 17px;
      text-align: center;
      width: 25%;
    }

    .clausula_tabela th {
      font-size: 17px;
      color: white;
      padding: 10px 15px;
    }

    .clausula_tabela td {
      font-size: 15px;
      padding: 20px 15px;
      text-align: center;
      width: 25%;
    }

    .clausula_tabela .status {
      width: 100%;
    }

    .clausula_ia textarea {
      width: 100%;
      height: 200px;
      padding: 10px;
    }

    .clausula_ia .clausula_footer {
      margin-top: 10px;
      display: flex;
      gap: 20px;
    }

    #ver_documento_btn {
      padding: 10px 25px;
      text-transform: uppercase;
    }

    #embed_pdf {
      min-height: 70vh;
      height: 100%;
    }

    .circle {
      width: 20px;
      height: 20px;
      border-radius: 50%;
    }

    .circle-green {
      background-color: green;
    }

    .circle-danger {
      background-color: red;
    }

    .filtro_header {
      width: 100%;
      background: #4f8edc;
      text-align: center;
      color: white;
      font-size: 20px;
      padding: 3px 0;
      border-radius: 2px;
      border: 1px solid rgba(0, 0, 0, 0.2);
    }

    .filtro_header p {
      margin: 0;
    }

    .filtro_documento_grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 20px;
      margin-top: 15px;
    }

    .select2-container--default {
      width: 100% !important;
    }

    .scroll-none {
      max-height: 40vh;
      overflow-y: scroll;
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
      max-height: 800px;
      overflow: auto;
      border-collapse: separate;

    }

    .info-adicional,
    .info-adicional-grupo,
    .info-adicional-grupo-update {
      width: 100%;
    }

    .scroll-true {
      border-collapse: separate;
    }

    .scroll-true td {
      min-width: 200px;
    }

    .dp-none {
      display: none;
    }

    #scrap table tr td {
      word-break: break-all;
    }

    #page-content {
      min-height: 100% !important;
    }
  </style>
</head>

<body class="horizontal-nav">
  <?php include ('menu.php'); ?>

  <div class="page-container" id="pageCtn">
    <div style="min-height: 100%;">
      <div id="wrap">
        <div class="container">
          <div class="row">
            <div class="col-sm-12">
              <section class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Filtros - Cadastro de Cláusulas</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <form id="filtrosForm" class="filtros_form">
                    <div class="form-group">
                      <div class="row" style="display: flex; gap: 20px;">
                        <div class="col-sm-4">
                          <div class="row">
                            <div class="filtro_header">
                              <p>Clientes</p>
                            </div>

                            <div style="margin-top: 15px;">
                              <label>Grupo Econômico</label>
                              <select data-placeholder="Nome" class="form-control" id="grupoEconomicoSelect"
                                multiple></select>
                            </div>

                            <div style="margin-top: 10px;">
                              <label>Empresa</label> <!-- Matriz -->
                              <select multiple data-placeholder="Nome, Código" class="form-control" id="empresaSelect">
                              </select>
                            </div>
                          </div>
                        </div>
                        <div class="col-sm-8">
                          <div>
                            <div class="filtro_header">
                              <p>Documento</p>
                            </div>
                            <div class="filtro_documento_grid">
                              <div class="filtro_documento_grid_item">
                                <label>Grupo Operação</label>
                                <select data-placeholder="Grupo Operação" class="form-control"
                                  id="grupoOperacaoSelect"></select>
                              </div>
                              <div class="filtro_documento_grid_item">
                                <label>ID Doc</label>
                                <select data-placeholder="ID Doc" class="form-control" id="documentoSelect"></select>
                              </div>
                              <div class="filtro_documento_grid_item">
                                <label>Atividade econômica</label>
                                <select data-placeholder="Atividade econômica" multiple class="form-control"
                                  id="atividadeEconomicaSelect"></select>
                              </div>
                              <div class="filtro_documento_grid_item">
                                <label>Nome Documento</label>
                                <select data-placeholder="Nome Documento" class="form-control"
                                  id="nomeDocumentoSelect"></select>
                              </div>
                            </div>
                            <div class="filtro_documento_grid">
                              <div class="filtro_documento_grid_item">
                                <label>Data SLA entrega</label>
                                <input type="text" class="form-control" id="dataSlaDt"
                                  placeholder="dd/mm/aaaa - dd/mm/aaaa">
                              </div>
                              <div class="filtro_documento_grid_item">
                                <div>
                                  <div>
                                    <label>Status Scrap</label>
                                    <select data-placeholder="Status Scrap" class="form-control"
                                      id="statusScrapSelect"></select>
                                  </div>
                                  <div style="margin-top: 10px;">
                                    <label>Status Cláusulas</label>
                                    <select data-placeholder="Status Cláusulas" class="form-control"
                                      id="statusClausulasSelect"></select>
                                  </div>
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                    <div class="form-group">
                      <div class="row" style="">
                        <div class="col-sm-12" style="display: flex; justify-content: space-between; align-items: end;">
                          <div>
                            <button id="filtrarBtn" style="margin-top: 20px ;" type="button" class="btn btn-primary"><i
                                class="fa fa-search" style="margin-right: 10px;"></i>Filtrar</button>
                            <button id="limparFiltroBtn" style="margin-top: 20px ; margin-left:8px;" type="button"
                              class="btn btn-primary"><i class="fa fa-times-circle-o"
                                style="margin-right: 10px;"></i>Limpar
                              Filtro</button>
                          </div>
                        </div>
                      </div>
                    </div>
                  </form>
                </div>
              </section>
            </div>
          </div>
          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <!-- TELA INICIAL -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro de Cláusula</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div id="grid-layout-table-1" class="box jplist">
                    <div class="box text-shadow">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="documentosTb"
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
  </div> <!--page-content -->

  <!-- LISTAR CLÁUSULAS -->
  <button type="button" class="hidden" data-toggle="modal" data-target="#listaClausulasModal"
    id="listaClausulasModalBtn"></button>
  <div class="hidden" id="listaClausulasModalHidden">
    <div class="modal-content" id="listaClausulasModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Documento</h4>
      </div>
      <div class="actions-wrapper">
        <button type="button" id="ver_documento_btn" class="btn btn-primary btn-rounded">Ver</button>
        <div>
          <button type="button" id="adicionar_clausula_btn" data-toggle="modal" data-target="#clausulaModal"
            class="btn btn-primary btn-rounded">Incluir
            cláusula</button>
          <button type="button" id="aprovar_documento_btn" class="btn btn-primary btn-rounded">Aprovar
            Documento</button>
          <button type="button" id="reprocessar_documento_btn" class="btn btn-primary btn-rounded">Reprocessar
            Scrap</button>
        </div>
      </div>
      <div class="modal-body">
        <div class="ver_documento_header">
          <h4>Lista de Cláusulas do Documento</h4>
        </div>
        <div class="lista_clausulas_body">
          <div class="ver_documento" id="ver_documento_ctn">
            <div class="ver_documento_header">
              <h4>Vizualização do Documento</h4>
            </div>
            <embed id="embed_pdf" src="" type="application/pdf" width="100%" height="100%">
          </div>
          <div class="lista_clausulas">
            <div class="ver_documento_header">
              <h4>Lista de Cláusulas Identificadas - SCRAP</h4>
            </div>
            <div class="clausula_ia" id="lista_clausulas">
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- INSERIR CLÁUSULA -->
  <button type="button" class="hidden" data-toggle="modal" data-target="#clausulaModal" id="clausulaBtn"></button>
  <div class="hidden" id="clausulaModalHidden">
    <div class="modal-content" id="clausulaModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Cláusula</h4>
      </div>
      <div class="modal-body">
        <form class="form-horizontal">
          <div class="panel panel-primary">
            <div class="panel-heading">
              <h4>Classificação de Cláusula</h4>
              <div class="options">
                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
              </div>
            </div>
            <div class="panel-body collapse in principal-table">
              <div class="form-group center">
                <div class="col-sm-3">
                  <label for="info-inputc" class="control-label">Documento Sindical</label>
                  <select class="js-example-basic-single select2" id="documento_sindical"></select>
                </div>

                <div class="col-sm-3">
                  <label for="info-inputc" class="control-label select2">Classificação da Cláusula</label>
                  <select class="js-example-basic-single" id="lista_clausula"></select>
                </div>

                <div class="col-sm-3">
                  <label for="info-inputc" class="control-label">Sinônimo</label>
                  <select id="sinonimo_select" class="js-example-basic-single"></select>
                </div>

                <div class="col-sm-3">
                  <label for="numero" class="control-label">Número da Cláusula</label>
                  <input type="number" class="form-control" id="numero">
                </div>
              </div>
              <div class="form-group">
                <div class="col-sm-2">
                  <label for="vigencia_inicial" class="control-label">Vigência Inicial</label>
                  <input type="text" class="form-control" id="vigencia_inicial" disabled>
                </div>

                <div class="col-sm-2">
                  <label for="vigencia_final" class="control-label">Vigência Final</label>
                  <input type="text" class="form-control" id="vigencia_final" disabled>
                </div>
              </div>

              <div class="form-group center">
                <div class="col-sm-12">
                  <textarea class="form-control" id="texto" cols="30" rows="30" style="max-height: 55vh;"></textarea>
                </div>
              </div>
            </div>
          </div>

          <div class="form-group" id="informacao_adicional_grupo_painel">
            <div class="panel panel-primary" style="margin:0 10px ;">
              <div class="panel-heading rounded-bottom">
                <h4>Seleção de Informações Adicionais</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-up"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table">
                <div class="row">
                  <div class="col-lg-6 scroll-none">
                    <table class="table table-striped">
                      <thead>
                        <th>Seleção</th>
                        <th>Informação Adicional</th>
                      </thead>
                      <tbody id="infoAdicionalGrupoSelecao"></tbody>
                    </table>
                  </div>

                  <div class="col-lg-6 scroll-none">
                    <table class="table table-striped">
                      <thead>
                        <th>Preencher Informações Adicionais</th>
                        <th></th>
                        <th></th>
                      </thead>
                      <tbody id="infoAdicional_grupo_lista_selecao">
                        <tr id="informacao_adicional_placeholder" style="color:#bbb ;">
                          <td>Selecione a classificação da cláusula e as informações desejadas.</td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="form-group" id="table-grupo-add">
            <div class="panel panel-primary" style="margin:0 10px ;">
              <div class="panel-heading">
                <h4>Informação Grupo</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table fixTableHead">
                <table class="table scroll-true" id="table_grupo"></table>
                <div style="float: left;">
                  <input type="text" style="width: 0; height: 0; background: transparent; border: none;"
                    id="focus_input">
                  <button id="remover_linha_lista_informacoes_adicionais_btn" type="button" class="btn btn-danger"><i
                      class="fa fa-minus"></i>
                    Remover Linha</button>
                  <button id="adicionar_linha_lista_informacoes_adicionais_btn" type="button" class="btn btn-primary"><i
                      class="fa fa-plus"></i>
                    Adicionar Linha</button>
                </div>
              </div>
            </div>
          </div>
        </form>
      </div>
      <div class="modal-footer">
        <div style="width: 100%;">
          <div class="btn-toolbar" style="display: flex; justify-content: center; gap: 10px;">
            <button type="button" id="btn_salvar_clausula" class="btn btn-primary btn-rounded">Salvar</button>
          </div>
          <button type="button" class="btn btn-primary btn-rounded" style="float: right; margin-top: -40px;"
            id="btn_voltar_modal_upsert">Voltar</button>
        </div>
      </div>
    </div>
  </div>

  <?php include 'footer.php' ?>

  </div> <!--page-container -->

  <!-- MODELO DE MODAL -->
  <script type='text/javascript' src="./js/ia_documento_sindical.min.js"></script>
</body>

</html>