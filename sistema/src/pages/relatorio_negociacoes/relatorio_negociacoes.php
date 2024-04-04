<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
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

  <link rel="stylesheet" href="relatorio_negociacoes.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style type="text/css">
    #page-container {
      position: relative;
    }

    #hide-page {
      transition: all .3s ease;
      background-color: #e0e0e0;
      width: 100%;
      height: 100%;
      position: absolute;
      top: 0;
      left: 0;
      z-index: 9999999999;
    }

    .div-form {
      margin-top: 20px;
    }

    #timeline-place {
      max-height: 50vh;
      overflow-y: scroll;
    }

    .timeline-box {
      margin-top: 50px;
    }

    .update-coment:hover div {
      transition: all 0.3s ease-in-out;
      background-color: #4f8edc;
    }

    .update-coment:hover i {
      color: #fff;
      transition: all 0.3s ease-in-out;
    }

    .sub-data {
      font-size: 0.875em;
    }

    #page-content {
      min-height: 100% !important;
    }

    #principal_table {
      width: 100%;
    }

    #principal_table_wrapper .row:nth-child(1) {
      display: flex;
      flex-direction: row-reverse;
    }

    #principal_table_wrapper .row:nth-child(1) .col-md-6:nth-child(1) {
      display: flex;
      justify-content: end;
    }

    #principal_table_wrapper .row:nth-child(1) .col-md-6:nth-child(2) {
      display: flex;
      justify-content: start;
    }

    .keep-open .dropdown-menu {
      min-width: 219px;
    }

    .dropdown-menu {
      padding: 10px 0px;
    }

    a.dropdown-item {
      display: block;
      width: 100%;
      padding: 0.45rem 1rem;
      clear: both;
      font-weight: 400;
      color: #787b7c;
      text-align: inherit;
      text-decoration: none;
      white-space: nowrap;
      background-color: transparent;
      border: 0;
    }

    a.dropdown-item:focus,
    a.dropdown-item:hover {
      color: #1e2125;
      background-color: #e9ecef;
    }

    /* ICONES QUE ESTAO BUGADOS */
    i.fa-toggle-off:before {
      content: "\f204";
    }

    i.fa-toggle-on:before {
      content: "\f205";
    }

    i.bi-print:before {
      content: "\F500";
    }

    i.bi-download:before {
      content: "\F30A";
    }
  </style>
</head>

<body class="horizontal-nav">

  <?php include "menu.php"; ?>

  <div id="page-container">
    <div id="hide-page"></div>
    <div id="page-content" style="padding-bottom: 0;">
      <div id="wrap">
        <div class="container">
          <form class="col-sm-12" id="form">
            <section class="panel panel-primary">
              <div class="panel-heading">
                <h4>Relatório de Acompanhamento CCT Ineditta</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <form id="filtroForm">
                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-3">
                        <label>Grupo Econômico</label>
                        <select data-placeholder="Nome" class="form-control" id="grupo" multiple></select>
                      </div>

                      <div class="col-sm-3">
                        <label>Empresa</label> <!-- Matriz -->
                        <select multiple data-placeholder="Nome, Código" class="form-control" id="matriz">
                        </select>
                      </div>

                      <div class="col-sm-6">
                        <label>Estabelecimento</label> <!-- Filial -->
                        <select multiple data-placeholder="Nome, CNPJ" class="form-control" id="unidade">
                        </select>
                      </div>
                    </div>
                  </div>
                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-2">
                        <label>Atividades Econômicas</label>
                        <select data-placeholder="CNAE" multiple class="form-control" id="canes"></select>
                      </div>

                      <div class="col-sm-2">
                        <label>Tipo da Localidade</label>
                        <select data-placeholder="Uf" class="form-control" id="tipo_localidade">
                        </select>
                      </div>

                      <div class="col-sm-2">
                        <label>Localidade do acompanhamento</label>
                        <select data-placeholder="Região, Uf ou Município" class="form-control" id="localidade"
                          multiple>
                        </select>
                      </div>

                      <div class="col-sm-3">
                        <label>Sindicato Laboral</label>
                        <select data-placeholder="Nome, Código" multiple class="form-control" id="sind_laboral">
                        </select>
                      </div>

                      <div class="col-sm-3">
                        <label>Sindicato Patronal</label>
                        <select data-placeholder="Nome, CNPJ" multiple class="form-control" id="sind_patronal">
                        </select>
                      </div>
                    </div>
                  </div>
                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-2">
                        <label>Data-base</label>
                        <select data-placeholder="Mês/ano" class="form-control" id="data_base" multiple></select>
                      </div>

                      <div class="col-sm-4">
                        <label>Nome do Documento</label>
                        <select data-placeholder="Selecione" multiple class="form-control" id="nome_documento">
                        </select>
                      </div>

                      <div class="col-sm-3">
                        <label>Fase</label>
                        <select data-placeholder="Selecione" class="form-control" id="fase" multiple>
                        </select>
                      </div>

                      <div class="col-sm-3">
                        <label>Data Processamento Ineditta</label>
                        <input type="text" class="form-control" id="data_processamento">
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
                          <div>
                            <button class="btn btn-success" id="btn_exportar_relatorio">Exportar registros de
                              acompanhamento</button>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </form>
              </div>
            </section>
          </form>
          <div class="col-md-12">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Lista de Acompanhamento CCT Ineditta</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table">
                <div style="display: flex; justify-content: space-between; margin-bottom: 15px;">
                  <div></div>
                  <div class="dropdown" style="position: relative;" id="dropdownFiltro">
                    <div style="display: flex;">
                      <button class="btn btn-secondary dropdown-toggle" type="button" data-toggle="dropdown"
                        aria-haspopup="true" aria-expanded="false">
                        <i class="fa fa-th-list"></i>
                      </button>
                      <div class="dropdown-menu" aria-labelledby="dropdownMenuButton"
                        style="padding: 10px 15px; position: absolute; right: 0; width: 200px;">
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; gap: 3px; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="Sind. Laboral" data-column-index="0" checked
                              class="filter-column">
                            <label>Sind. Laboral</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="CNPJ Laboral" data-column-index="1" checked
                              class="filter-column">
                            <label>CNPJ Laboral</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="Sind. Patronal" data-column-index="2" checked
                              class="filter-column">
                            <label>Sind. Patronal</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="CNPJ Patronal" data-column-index="3" checked
                              class="filter-column">
                            <label>CNPJ Patronal</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="Atividade Econômica" data-column-index="4" checked
                              class="filter-column">
                            <label for="filter-table-inpc">Atividade Econômica</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="Nome documento" data-column-index="5" checked
                              class="filter-column">
                            <label for="filter-table-inpc">Nome documento</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="Data base" data-column-index="6" checked
                              class="filter-column">
                            <label>Data base</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="Período INPC" data-column-index="7" checked
                              class="filter-column">
                            <label>Período INPC</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="INPC (Real)" data-column-index="8" checked
                              class="filter-column">
                            <label>INPC (Real)</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="Fase" data-column-index="9" checked
                              class="filter-column">
                            <label>Fase</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="Observações" data-column-index="10" checked
                              class="filter-column">
                            <label>Observações</label>
                          </div>
                        </div>
                        <div class="input-group-prepend">
                          <div class="input-group-text" style="display: flex; margin-top: 4px; gap: 3px;">
                            <input type="checkbox" aria-label="Data Procesamento Ineditta" data-column-index="11"
                              checked class="filter-column">
                            <label>Data Procesamento Ineditta</label>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
                <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl"
                  id="acompanhamentoCcttb" data-order='[[ 1, "asc" ]]' style="width: 100%;"></table>
              </div>

              <!-- MODAL COMENTARIO CCT -->
              <div id="myModalUpdate" class="modal" tabindex="-1" role="dialog">
                <div class="modal-dialog" role="document">
                  <div class="modal-content">
                    <div class="modal-body">
                      <div class="panel panel-primary">
                        <form class="form-horizontal">
                          <div class="panel-heading">
                            <h4>Detalhes da Negociação</h4>
                            <div class="options">
                              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                            </div>
                          </div>
                          <div class="panel-body collapse in">
                            <div class="form-group center">
                              <input type="hidden" name="" id="id-inputu">
                              <div class="col-sm-3">
                                <label for="sinde-inputu" class="control-label">Sind. Empregados</label>
                                <input type="text" class="form-control" name="" id="sinde-inputu">
                              </div>

                              <div class="col-sm-3">
                                <label for="patr-inputu" class="control-label">Sind. Patronal</label>
                                <input type="text" class="form-control" name="" id="patr-inputu">
                              </div>

                              <div class="col-sm-3">
                                <label for="dataini-input" class="control-label">Data Inicial</label>
                                <input type="text" class="form-control" name="" id="dataini-inputu">
                              </div>

                              <div class="col-sm-3">
                                <label for="datafin-input" class="control-label">Data Final</label>
                                <input type="text" class="form-control" name="" id="datafin-inputu">
                              </div>
                            </div>

                            <div class="form-group center">
                              <div class="col-sm-3" style="margin-top: 20px;">
                                <label for="fase-inputu" class="control-label">Fase</label>
                                <input type="text" class="form-control" id="fase-inputu">
                              </div>

                              <div class="col-sm-3" style="margin-top: 20px;">
                                <label for="cnae-input" class="control-label">CNAE</label>
                                <input type="text" class="form-control" name="" id="cnae-inputu">
                              </div>

                              <div class="col-sm-3" style="margin-top: 20px;">
                                <label for="data-input" class="control-label">Data Base</label>
                                <input type="text" class="form-control" name="" id="data-inputu">
                              </div>

                              <div class="col-sm-3" style="margin-top: 20px;">
                                <label for="status-input" class="control-label">Status</label>
                                <input type="text" class="form-control" name="" id="status-inputu">
                              </div>
                            </div>

                            <div class="form-group">
                              <div class="col-md-6" style="margin-top: 20px;">
                                <label for="atual-inputu" class="control-label">Ultima Atualização</label>
                                <input type="text" class="form-control" name="" id="atual-inputu">
                              </div>
                            </div>
                          </div>
                        </form>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div> <!-- container -->
    </div> <!-- #wrap -->
  </div> <!-- page-content -->

  <?php include 'footer.php' ?>

  </div> <!-- page-container -->
  <!-- SCRIPTS -->
  <script type='text/javascript' src="./js/relatorio_negociacoes.min.js"></script>
</body>

</html>