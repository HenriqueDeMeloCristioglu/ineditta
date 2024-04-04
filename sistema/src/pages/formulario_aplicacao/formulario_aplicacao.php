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
  <meta name="author" content="The Red Team">

  <link rel="stylesheet" href="includes/css/styles.css">
  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>

  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />
  <link rel='stylesheet' type='text/css' href='formulario_aplicacao.css' />
  <link rel="stylesheet" type='text/css' href="includes/css/styles.css" />

  <style>
    .select2-container {
      width: 100% !important;
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

    /* .scroll-none-group {
      scrollbar-width: none;
      max-height: 35vh; 
      overflow-y: scroll;
      
    } */

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

    .style_check {
      display: flex;
      align-items: start;
      margin-top: 34px;
      justify-content: center;
      gap: 10px;
    }

    .table_doc_geral th,
    .table_doc_process th {
      text-align: center;
      background-color: #4f8edc;
      color: #fff;
    }

    .table_doc_geral td:nth-child(2),
    .table_doc_process td:nth-child(1) {
      text-align: center;
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


    form .row {
      padding: 0px 10px;
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

  <div id="pageCtn" style="min-height: 100%;">
    <div class="img_box">
      <img class="img_load" src="includes/img/loading.gif">
    </div>
    <!-- <div> -->
    <div id="wrap">
      <div class="container">
        <div class="row" style="display: flex;">
          <div class="col-md-12">
            <form class="form-horizontal">
              <div class="panel panel-primary" style="margin: 7px 0px 20px 0px;">
                <div class="panel-heading">
                  <h4>Consulta Documento</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-5">
                        <label>Tipo de Consulta</label>
                        <select class="form-control" id="tipo_doc">
                        </select>
                      </div>

                      <div class="col-sm-3" id="box_tipo_doc">
                        <label>Tipo do Documento</label>
                        <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                          id="tipo_documento">
                        </select>
                      </div>

                      <div class="col-sm-4">
                        <label>Nome do Documento</label>
                        <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                          id="nome_doc">
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-5">
                        <label>Localidade</label>
                        <!-- <select class="form-control" id="localidade"> -->
                        <select multiple data-placeholder="Região, UF ou Município" class="form-control select2"
                          id="localidade">
                        </select>
                      </div>

                      <div class="col-sm-4">
                        <label>Atividade Econômica</label>
                        <select multiple data-placeholder="CNAE" class="form-control select2" id="categoria">
                        </select>
                      </div>

                      <div class="col-sm-1 style_check">
                        <input type="checkbox" name="restrito" id="restrito">
                        <label for="restrito">Restrito</label>
                      </div>

                      <div class="col-sm-2 style_check">
                        <input type="checkbox" name="anu_obrigatoria" id="anu_obrigatoria">
                        <label for="anu_obrigatoria">Anuência Obrigatória</label>
                      </div>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-3">
                        <label>Grupo Econômico</label>
                        <select data-placeholder="Nome" class="form-control select2" id="grupo">
                        </select>
                      </div>

                      <div class="col-sm-3">
                        <label>Empresa</label>
                        <select multiple data-placeholder="Nome, Código" class="form-control select2" id="matriz">
                        </select>
                      </div>

                      <div class="col-sm-6">
                        <label>Estabelecimento</label>
                        <select multiple
                          data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente, Regional" tabindex="8"
                          class="form-control select2" id="unidade">
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-5">
                        <label id="label-sindicato" for="">Sindicato Laboral</label>
                        <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                          class="form-control select2" id="sindicatoLaboral">
                        </select>
                      </div>

                      <div class="col-sm-5">
                        <label id="label-sindicato" for="">Sindicato Patronal</label>
                        <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                          class="form-control select2" id="sindicatoPatronal">
                        </select>
                      </div>

                      <div class="col-sm-2">
                        <label>Data-base</label>
                        <select multiple data-placeholder="Mês/Ano" tabindex="8" class="form-control select2"
                          id="dataBase">
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-4">
                        <label>Vigência</label>
                        <div class="input-group-prepend">
                          <span class="input-group-text">
                            <i class="far fa-calendar-alt"></i>
                          </span>
                        </div>
                        <input type="text" class="form-control float-right date_format" id="vigencia">
                      </div>

                      <div class="col-sm-8">
                        <label>Assuntos</label>
                        <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                          id="assuntos">
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="form-group" style="margin-bottom: 0;">
                    <div class="col-sm-12" style="display: flex; gap: 3px; justify-content: left;">
                      <button type="button" id="filtrarBtn" class="btn btn-primary btn-rounded"><i class="fa fa-search"
                          style="margin-right: 10px;"></i> Filtrar</button>
                      <button type="button" id="limparBtn" class="btn btn-primary btn-rounded">Limpar Filtro</button>
                    </div>
                  </div>
                </div>

              </div>
            </form>
          </div>
        </div>
        <div class="row">
          <div class="col-md-12" id="exibirDocumentosDiv">
            <div class="panel panel-primary" style="padding: 0px 10px;">
              <div class="panel-heading">
                <h4>Lista de Documentos</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <table style="width: 100%;" class="table table-striped table-bordered" id="documentoTb">
                  <thead>
                    <th>Nome</th>
                    <th>Sindicato Laboral</th>
                    <th>Sindicato Patronal</th>
                    <th>Data de Inclusão</th>
                    <th>Vigência</th>
                    <th>Descrição</th>
                    <th>Atividades</th>
                    <th>Ver</th>
                  </thead>
                  <tbody>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
        <!--page-content -->
      </div>
      <!--page-container -->
    </div>
  </div>


  <?php include 'footer.php' ?>

  <script type='text/javascript' src="./js/formulario_aplicacao.min.js"></script>
</body>

</html>