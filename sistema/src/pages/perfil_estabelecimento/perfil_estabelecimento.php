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
 **/

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

  <!-- <link href="includes/less/styles.less" rel="stylesheet/less" media="all">  -->
  <link rel="stylesheet" href="perfil-estabelecimento.css">
  <link rel="stylesheet" href="includes/css/styles.css">
  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>
</head>
<style>
  .select2-container {
    width: 100% !important;
    /* or any value that fits your needs */
  }

  .especial-campo {
    position: relative;
  }

  .adesao {
    font-size: 1.3em;
    font-weight: bold;
    color: #bbb;
    margin-left: 25px;
    position: absolute;
    top: -2px;
  }

  .dom-fer {
    font-size: 1.5em;
  }

  .row_center_double {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
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
  .tbl-item td:nth-child(3) {
    height: 55px;
  }

  #page-content {
    min-height: 100% !important;
  }

  .btn-info-sindicato {
    cursor: pointer;
  }

  .info-grid {
    font-size: 20px;
  }

  .grow-1 {
    flex-grow: 1;
  }

  div.container_logo_client {
    padding: 0 !important;
    overflow: hidden !important;
  }
</style>

<body class="horizontal-nav">

  <?php include('menu.php'); ?>

  <div class="page-container">

    <div id="page-content">
      <div class="img_box">
        <img class="img_load" src="includes/img/loading.gif">
      </div>

      <div class="wrap">

        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-1 img_container"> <!-- style="transform: translate(-13px, 13px);" -->
              <div class="container_logo_client">
                <img id="imglogo" class="img-circle">
              </div>
            </div>

            <div class="grow-1">
              <label>Estabelecimento</label>
              <select multiple
                data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente, Regional" tabindex="8"
                class="form-control select2" id="unidade">
              </select>
            </div>
          </div>
          
          <div class="row">
            <div class="col-md-12">

              <section class="row">
                <div class="col-md-12" id="informacoesEstabelecimentosPanel">
                  <div class="panel panel-primary">
                    <div class="panel-heading">
                      <h4>Informações dos Estabelecimentos</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="informacoesEstabelecimentosTb" data-order='[[ 1, "asc" ]]'
                        style="width: 100%;"></table>
                    </div>
                  </div>
                </div>
              </section>

              <section class="row">
                <div class="col-md-12" id="documentosSindicaisProcessadosPanel">
                  <div class="panel panel-primary">
                    <div class="panel-heading">
                      <h4>Documentos sindicais - Processados</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body">
                      <p class="info-grid">
                        <i class="fa fa-info-circle info-icon" aria-hidden="true"></i>
                        <strong>Documentos processados: </strong>
                        <span>Instrumentos Coletivos que processam cláusulas, calendário sindical e informações de Mapa sindical.</span>
                      </p>
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="documentosSindicaisProcessadosTb" data-order='[[ 7, "desc" ]]'
                        style="width: 100%;"></table>
                    </div>
                  </div>
                </div>
              </section>

              <section class="row">
                <div class="col-md-12" id="documentosSindicaisGeraisPanel">
                  <div class="panel panel-primary">
                    <div class="panel-heading">
                      <h4>Documentos sindicais - Não processados/Biblioteca</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body">
                      <p class="info-grid">
                        <i class="fa fa-info-circle info-icon" aria-hidden="true"></i>
                        <strong>Documentos não processados: </strong>
                        <span>Documentos diversos de biblioteca que não processam cláusulas.</span>
                      </p>
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="documentosSindicaisGeraisTb" data-order='[[ 7, "desc" ]]'
                        style="width: 100%;"></table>
                    </div>
                  </div>
                </div>
              </section>

            </div>
          </div>

          <div id="info-sindicato-modal-container"></div>
        </div>
      </div>
    </div>
  </div> <!-- page-content -->

  <?php include 'footer.php' ?>

  </div> <!-- page-container -->

  <script type='text/javascript' src="./js/perfil-estabelecimento.min.js"></script>
</body>

</html>