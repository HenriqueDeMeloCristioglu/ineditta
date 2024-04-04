<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

$userSession = $_SESSION['login'];

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

  <link rel="stylesheet" href="comparativo.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    body {
      padding-right: 0 !important;
    }

    .btn-selecionado {
      background-color: #276ec6 !important;
    }

    .select2-container {
      width: 100% !important;
      /* or any value that fits your needs */
    }

    .swal2-select {
      display: none !important;
    }

    .header_top {
      background-color: #34495e;
      color: #fff;
    }

    .header_left {
      background-color: #ddd;
    }

    #generate_csv {
      margin: 16px 0;
    }

    .linha_nome {
      background-color: #4f8edc;
      color: #fff;
    }

    .modal-md {
      max-width: 800px !important;
    }

    #page-content {
      min-height: 100% !important;
    }

    .hide {
      display: none;
    }
  </style>
</head>

<body class="horizontal-nav hide">

  <?php include('menu.php'); ?>

  <div class="page-container">
    <div id="page-content">
      <!-- <div> -->
      <div id="wrap">
        <div class="container">
          <div class="row">
            <div class="col-sm-12">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Mapa Sindical Comparativo - Negociação referência</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-4">
                        <label for="localidade">Localidade do Estabelecimento</label>
                        <select multiple id="localidade" class="form-control select2"></select>
                      </div>
                      <div class="col-sm-3">
                        <label for="cnae">Atividade Econômica</label>
                        <select multiple id="cnae" class="form-control select2"></select>
                      </div>
                      <div class="col-sm-3">
                        <label for="nome_doc">Nome do Documento</label>
                        <select multiple id="nome_doc" class="form-control select2"></select>
                      </div>
                      <div class="col-sm-2">
                        <label for="data_base">Data-base</label>
                        <select multiple id="data_base" class="form-control select2"></select>
                      </div>
                    </div>
                  </div>
                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-12" style="margin-top: 30px;">
                        <a id="btnAbrirModalReferencia" href="#modalSelectDoc1" data-toggle="modal" class="btn btn-primary btn-rounded" style="margin-right: 5px;"><i class="fa fa-search" style="margin-right: 10px;"></i>Filtrar</a>
                        <button id="btnLimparFiltroReferencia" type="button" class="btn btn-primary btn-rounded">Limpar
                          Filtro</button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="col-sm-12">
              <div class="panel panel-primary" style="margin-top: 0; margin-bottom: 0;">
                <div class="panel-heading">
                  <h4>Mapa Sindical Comparativo - Negociação comparação</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-4">
                        <input type="hidden" id="id_docs_to_compare">
                        <label for="localidade_com">Localidade do Estabelecimento</label>
                        <select multiple id="localidade_com" class="form-control select2"></select>
                      </div>
                      <div class="col-sm-3">
                        <label for="cnae_com">Atividade Econômica</label>
                        <select multiple id="cnae_com" class="form-control select2"></select>
                      </div>

                      <div class="col-sm-3">
                        <label for="nome_doc_com">Nome do Documento</label>
                        <select multiple id="nome_doc_com" class="form-control select2"></select>
                      </div>

                      <div class="col-sm-2">
                        <label for="data_base_com">Data-base</label>
                        <select multiple id="data_base_com" class="form-control select2"></select>
                      </div>
                    </div>
                  </div>
                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-12" style="margin-top: 30px;">
                        <a id="btnAbrirModalDocumentacao" href="#modalSelectDoc2" data-toggle="modal" class="btn btn-primary btn-rounded" style="margin-right: 5px;"><i class="fa fa-search" style="margin-right: 10px;"></i>Filtrar</a>
                        <button id="btnLimparFiltroDocumentacao" type="button" class="btn btn-primary btn-rounded clear" style="margin-right: 5px;">Limpar
                          Filtro</button>
                        <a class="btn btn-primary" id="generate_csv">Gerar Excel</a>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>


            <!-- MODAL DOCS ENCONTRADOS FILTRO 1 -->
            <div class="modal fade" id="modalSelectDoc1" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
              <div class="modal-dialog" role="document">
                <div class="modal-content">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Selecione o Documento</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Lista de Documentos Encontrados</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <div id="grid-layout-table-2" class="box jplist">
                          <div class="box text-shadow">
                            <table cellpadding="0" cellspacing="0" border="0" style="width: 100%;" class="table table-striped table-bordered demo-tbl" id="documentos_referencia" data-order='[[ 0, "asc" ]]'>
                              <thead>
                                <th></th>
                                <th>Sindicato Laboral</th>
                                <th>Sindicato Patronal</th>
                                <th>Atividade econômica</th>
                                <th>Nome</th>
                                <th>Data base</th>
                                <th>Vigência</th>
                              </thead>
                              <tbody>
                              </tbody>
                            </table>
                          </div>
                          <div class="col-lg-12">
                            <button data-toggle="modal" type="button" class="btn btn-primary chk-selecionar" data-dismiss="modal">Selecionar</button>
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
            </div>
            <!-- FIM MODAL FILTRO 1-->
            <!-- MODAL DE DOCUMENTOS PARA COMPARAÇÃO -->
            <div class="modal fade" id="modalSelectDoc2" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
              <div class="modal-dialog" role="document">
                <div class="modal-content">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Selecione o Documento</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Lista de Documentos Encontrados Para Comparação</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <div class="row">
                          <div class="col-lg-12">
                            <table cellpadding="0" cellspacing="0" border="0" style="width: 100%;" class="table table-striped table-bordered demo-tbl" id="documentos_comparacao" data-order='[[ 0, "asc" ]]'>
                              <thead>
                                <th></th>
                                <th>Sindicato Laboral</th>
                                <th>Sindicato Patronal</th>
                                <th>Atividade econômica</th>
                                <th>Nome</th>
                                <th>Data base</th>
                                <th>Vigência</th>
                              </thead>
                              <tbody>
                              </tbody>
                            </table>
                          </div>
                          <div class="col-lg-12">
                            <button class="btn btn-primary" data-dismiss="modal">Selecionar</button>
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
            </div>
            <!-- FIM MODAL FILTRO 2-->
          </div>
        </div>
      </div>
    </div>
  </div> <!--page-content -->


  <?php include 'footer.php' ?>

  </div> <!--page-container -->

  <script type='text/javascript' src="./js/comparativo.min.js"></script>
</body>

</html>