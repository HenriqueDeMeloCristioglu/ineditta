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
  <meta name="author" content="Threeo It Company">

  <link rel="stylesheet" href="resumo_clausulas.css">
  <link rel="stylesheet" href="includes/css/styles.css">
</head>

<body class="horizontal-nav">
  <?php include('menu.php'); ?>

  <div id="page-container">
    <div id="pageCtn">
      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <!-- TELA PRINCIPAL -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Resumo de Clausulas</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <div id="grid-layout-table-1" class="box jplist">
                    <div class="box text-shadow">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="resumoClausulasTb"
                        data-order='[[ 1, "asc" ]]' style="width: 100%;"></table>
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

  <!-- PAGE SCRIPT -->
  <script type='text/javascript' src="./js/resumo_clausulas.min.js"></script>

</body>

</html>