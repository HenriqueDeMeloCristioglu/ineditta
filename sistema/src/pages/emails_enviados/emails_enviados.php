<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}
$sessionUser = $_SESSION['login'];

?>
<!DOCTYPE html>
<html lang="pt-br">

<head>
  <meta charset="utf-8">
  <title>Ineditta</title>
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <meta name="description" content="Ineditta">

  <link rel="stylesheet" href="emails_enviados.css">
  <link rel="stylesheet" href="includes/css/styles.css">
</head>

<body class="horizontal-nav">
  <?php include('menu.php'); ?>

  <div id="page-container">
    <div id="pageCtn">
      <div id="wrap">
        <div class="container">
          <div class="row">
            <div class="col-md-12">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Emails Enviados</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <div style="width: 100%; margin: 20px 0 70px 0;">
                    <button type="button" class="btn btn-secondary" id="gerarRelatorioBtn" style="float: right;">Gerar
                      Relatório</button>
                  </div>
                  <div id="grid-layout-table-1" class="box jplist">
                    <div class="box text-shadow">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="emailsEnviadosTb"
                        data-order='[[ 1, "asc" ]]' style="width: 100%;"></table>
                    </div>
                  </div>
                </div>
              </div>
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Emails Caixa de Saída</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <div style="width: 100%; margin: 20px 0 70px 0;">
                    <button type="button" class="btn btn-secondary" id="reenviarEmailsBtn"
                      style="float: right;">Reenviar
                      Emails</button>
                  </div>
                  <div id="grid-layout-table-1" class="box jplist">
                    <div class="box text-shadow">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="emailsCaixaDeSaidaTb"
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
  </div>

  <script type='text/javascript' src="./js/emails_enviados.min.js"></script>

</body>

</html>