<?php
session_start();
if(!$_SESSION) {
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

  <link rel="stylesheet" href="geradorCsv.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
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

    #page-content {
      min-height: 100% !important;
    }

    .hide {
      display: none;
    }

    .flex-1 {
      flex: 1;
    }

    .btn-gerar-excel-container {
      display: flex;
      justify-content: center;
    }

    .btns-actions-container {
      display: flex;
      justify-content: flex-start;
    }

    .dataTables_filter {
      display: flex;
      flex-direction: row-reverse;
    }

    #btnGerarExcel {
      margin-right: 12px;
    }

    .btn-info-sindicato {
      display: inline-block;
      border: none;
      background: inherit;
      color: #4f8edc;
    }
  </style>
</head>

<body class="horizontal-nav hide">
  <?php include('menu.php'); ?>

  <div class="page-container">

    <div id="page-content">
      <div class="img_box">
        <img class="img_load" src="includes/img/loading.gif">
      </div>

      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <!-- TELA INICICAL -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Gerar arquivo Excel</h4>
                </div>
                <div class="panel-body collapse in principal-table">
                  <form>
                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-3">
                          <label for="grupo">Grupo Econômico</label>
                          <select data-placeholder="Nome" class="form-control select2" id="grupo"></select>
                        </div>

                        <div class="col-sm-4">
                          <label for="matriz">Empresa</label>
                          <select multiple data-placeholder="Nome, Código" class="form-control select2"
                            id="matriz"></select>
                        </div>
                        <div class="col-sm-5">
                          <label for="unidade">Estabelecimento</label>
                          <select multiple
                            data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente, Regional"
                            tabindex="8" class="form-control select2" id="unidade"></select>
                        </div>
                      </div>
                    </div>
                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-6">
                          <label for="tipo_doc">Localidade do Estabelecimento</label>
                          <select multiple data-placeholder="Região, UF, Município" class="form-control select2"
                            id="localidade"></select>
                        </div>
                        <div class="col-sm-6">
                          <label for="tipo_doc">Atividade Econômica</label>
                          <select multiple data-placeholder="CNAE" class="form-control select2" id="categoria"></select>
                        </div>
                      </div>
                    </div>

                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-5">
                          <label id="label-sindicato" for="">Sindicato Laboral</label>
                          <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sind_laboral"></select>
                        </div>

                        <div class="col-sm-5">
                          <label id="label-sindicato" for="">Sindicato Patronal</label>
                          <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sind_patronal"></select>
                        </div>

                        <div class="col-sm-2">
                          <label>Data-base</label>
                          <select multiple data-placeholder="Mês/Ano" tabindex="8" class="form-control select2"
                            id="data_base">
                          </select>
                        </div>
                      </div>
                    </div>

                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-4">
                          <label for="tipo_doc">Nome do Documento</label>
                          <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="nome_doc"></select>
                        </div>

                        <div class="col-sm-3">
                          <label for="tipo_doc">Grupo Cláusulas</label>
                          <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="grupo_clausulas"></select>
                        </div>

                        <div class="col-sm-5">
                          <label for="tipo_doc">Seleção de Cláusulas</label>
                          <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="clausulaList"></select>
                        </div>
                      </div>
                    </div>

                    <div class="form-group">
                      <div class="col-sm-2" style="padding-left: 0;">
                        <label>Data Processamento Ineditta*</label>
                        <div class="input-group-prepend">
                          <span class="input-group-text">
                            <i class="far fa-calendar-alt"></i>
                          </span>
                        </div>
                        <input type="text" class="form-control float-right date_format" id="dataProcessamento" placeholder="dd/mm/aaaa - dd/mm/aaaa">
                      </div>
                    </div>

                    <div class="row">
                      <div class="col-sm-12">
                        <div class="btn-toolbar" style="display: flex; justify-content: center;">
                          <div class="flex-1 btns-actions-container">
                            <button style="margin-top: 20px ;margin-left:8px;" type="button" id="btnObter"
                              class="btn btn-primary"><i class="fa fa-search"
                                style="margin-right: 10px;"></i>Filtrar</button>

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
              </div>

              <!-- TELA DE VISUALIZAÇÃO TABELA -->

              <div class="row" id="visualizar">
                <div class="col-md-12" id="exibirDocumentosDiv">
                  <div class="panel panel-primary">
                    <div class="panel-heading">
                      <h4>Visualização</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in" id="table-container">
                      <div id="selectAllDiv" style="display: block; margin-bottom: 10px" class="selectAll">
                        <input type="checkbox" id="selectAllInput">
                        <label for="selectAllInput">Selecionar Todos</label>
                      </div>

                      <table style="width: 100%;" class="table table-striped table-bordered" id="geradorExcelTb" data-order='[[ 4, "desc" ]]'>
                        <tbody>
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

    <!-- MODAL INFO SINDICATOS -->
    <div id="modal-info-sindicato-wrapper">
    </div>

    <?php include 'footer.php' ?>

  </div> <!--page-container -->

  <script type='text/javascript' src="./js/geradorCsv.min.js"></script>
</body>

</html>