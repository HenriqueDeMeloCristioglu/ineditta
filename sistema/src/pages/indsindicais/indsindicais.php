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

  <link rel="stylesheet" href="indsindicais.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    .hide {
      display: none;
    }
  </style>
</head>

<body class="horizontal-nav hide">

  <?php include('menu.php'); ?>

  <div class="page-container">
    <div id="page-content" style="min-height: 100%;">
      <div id="wrap">
        <div class="container">
          <!-- TELA INICICAL -->
          <div class="panel panel-primary col-md-12">
            <div class="panel-heading">
              <h4>Indicadores Sindicais</h4>
              <div class="options">
                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
              </div>
            </div>
            <div class="panel-body">
              <form>
                <div class="form-group">
                  <div class="row">
                    <div class="col-sm-3">
                      <label>Grupo Econômico</label>
                      <select data-placeholder="Nome" class="form-control select2" id="grupo"></select>
                    </div>

                    <div class="col-sm-9">
                      <label for="tipo_doc">Empresa</label>
                      <select multiple="multiple" data-placeholder="Nome, Código" class="form-control select2"
                        id="matriz" disable></select>
                    </div>
                  </div>
                </div>

                <div class="form-group">
                  <div class="row">
                    <div class="col-sm-12">
                      <label for="tipo_doc">Unidade</label>
                      <select multiple="multiple"
                        data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente, Regional" tabindex="8"
                        class="form-control select2" id="unidade"></select>
                    </div>
                  </div>
                </div>

                <div class="form-group" style="z-index:100;">
                  <div class="row">
                    <div class="col-sm-6">
                      <label for="tipo_doc">Localidade</label>
                      <select multiple="multiple" data-placeholder="Região, UF, Município" class="form-control select2"
                        id="localidade"></select>
                    </div>

                    <div class="col-sm-6">
                      <label for="tipo_doc">Atividade Econômica</label>
                      <select multiple="multiple" data-placeholder="CNAE" class="form-control select2"
                        id="categoria"></select>
                    </div>
                  </div>
                </div>

                <div class="form-group">
                  <div class="row">
                    <div class="col-sm-5">
                      <label id="label-sindicato" for="">Sindicato Laboral</label>
                      <select multiple="multiple" data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                        class="form-control select2" id="sind_laboral"></select>
                    </div>

                    <div class="col-sm-5">
                      <label id="label-sindicato" for="">Sindicato Patronal</label>
                      <select multiple="multiple" data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                        class="form-control select2" id="sind_patronal"></select>
                    </div>

                    <div class="col-sm-2">
                      <label for="tipo_doc">Data-base</label>
                      <select multiple="multiple" data-placeholder="Mês/Ano" tabindex="8" class="form-control select2"
                        id="data_base"></select>
                    </div>
                  </div>
                </div>

                <div class="form-group">
                  <div class="row">
                    <div class="col-sm-2">
                      <label>Período</label>
                      <div class="input-group-prepend">
                        <span class="input-group-text">
                          <i class="far fa-calendar-alt"></i>
                        </span>
                      </div>
                      <input type="text" class="form-control float-right date_format" id="periodo">
                    </div>

                    <div class="col-sm-4">
                      <label for="tipo_doc">Nome do Documento</label>
                      <select multiple="multiple" data-placeholder="Selecione" tabindex="8" class="form-control select2"
                        id="nome_doc"></select>
                    </div>

                    <div class="col-sm-6">
                      <label for="tipo_doc">Nome Cláusula</label>
                      <select data-placeholder="Nome" tabindex="8" class="form-control select2"
                        id="clausulaList"></select>
                    </div>
                  </div>
                </div>

                <div class="row">
                  <div class="col-sm-12">
                    <button type="button" id='btn_filtrar' class="btn btn-primary"><i class="fa fa-search"
                        style="margin-right: 10px;"></i>Filtrar</button>
                  </div>
                </div>
              </form>
            </div>

            <!-- TELA DE VISUALIZAÇÃO TABELA -->
            <div id="visualizar" class="panel panel-primary col-md-12" style="width: 100%;">
              <div class="panel-heading">
                <h4>Visualização</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table">
                <div class="row">
                  <button type="button" id="btn_csv" class="btn btn-primary">CSV</button>
                  <button type="button" id="btn_exel" class="btn btn-primary">Exel</button>
                </div>
                <div class="row">
                  <table class="table" id="example1">

                  </table>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  </div> <!--page-content -->


  <?php include 'footer.php' ?>

  </div> <!--page-container -->

  <script type='text/javascript' src="./js/indsindicais.min.js"></script>
</body>

</html>