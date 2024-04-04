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
  <link rel="stylesheet" href="includes/css/mystyle.css">
  <link rel="stylesheet" href="includes/plugins/charts-morrisjs/morris.css">
  <link rel="stylesheet" href="includes/plugins/codeprettifier/prettify.css">
  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>

  <script src="keycloak.js"></script>
</head>

<body onload="initKeycloak()" class="horizontal-nav">

  <?php include('menu.php'); ?>

  <div id="page-container">
    <div id="page-content">
      <section class="row cards" style="padding: 20px;">
        <div class="col-lg-1 col-md-12 col-12">
          <img id="imglogo" width="107" alt="" srcset="">
        </div>

        <div class="col-lg-6 col-md-12 col-12">
          <div class="col-lg-4 col-md-3 col-sm-12 ">
            <a class="info-tiles tiles-inverse" href="#">
              <div class="tiles-heading">
                <div class="pull-left">Empresas</div>
              </div>
              <div class="tiles-body">
                <div class="pull-left"><i class="fa fa-filter"></i></div>
                <div class="pull-right">115</div>
              </div>
            </a>
          </div>

          <div class="col-lg-4 col-md-3 col-sm-12 ">
            <a class="info-tiles tiles-inverse" href="#">
              <div class="tiles-heading">
                <div class="pull-left">Segmentos</div>
              </div>
              <div class="tiles-body">
                <div class="pull-left"><i class="fa fa-filter"></i></div>
                <div class="pull-right">83</div>
              </div>
            </a>
          </div>

          <div class="col-lg-4 col-md-3 col-sm-12 ">
            <a class="info-tiles tiles-inverse" href="#">
              <div class="tiles-heading">
                <div class="pull-left">Localidade/Unidade</div>
              </div>
              <div class="tiles-body">
                <div class="pull-left"><i class="fa fa-filter"></i></div>
                <div class="pull-right">237</div>
              </div>
            </a>
          </div>
        </div>

        <div class="col-lg-1 col-12">
        </div>

        <div class="col-lg-4 col-md-12 col-12">
          <div class="col-lg-6 col-md-3 col-sm-12 ">
            <a class="info-tiles tiles-orange" href="#">
              <div class="tiles-heading">
                <div class="pull-left">Sindicatos Empregados</div>
              </div>
              <div class="tiles-body">
                <div class="pull-left"><i class="fa fa-download"></i></div>
                <div class="pull-right">385</div>
              </div>
            </a>
          </div>
          <div class="col-lg-6 col-md-3 col-sm-12 ">
            <a class="info-tiles tiles-orange" href="#">
              <div class="tiles-heading">
                <div class="pull-left">Sindicatos Patronais</div>
              </div>
              <div class="tiles-body">
                <div class="pull-left"><i class="fa fa-download"></i></div>
                <div class="pull-right">513</div>
              </div>
            </a>
          </div>
        </div>
      </section>

      <section class="negocia">
        <header>
          <h1>NEGOCIAÇÕES</h1>
        </header>

        <section class="negocia-content row">
          <article class="col-lg-6">
            <div class="chat-panel panel panel-inverse">
              <div class="panel-heading">
                <h4>Últimas negociações encerradas</h4>
                <div class="pull-right"><span class="badge">27</span></div>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body">
                <ol class="chat">
                  <li>
                    Lorem ipsum dolor sit amet, bibendum
                  </li>
                  <li>
                    Lorem ipsum dolor sit amet, bibendum
                  </li>
                  <li>
                    Lorem ipsum dolor sit amet, bibendum
                  </li>
                  <li>
                    Lorem ipsum dolor sit amet, bibendum
                  </li>
                  <li>
                    Lorem ipsum dolor sit amet, bibendum
                  </li>
                </ol>
              </div>
            </div>
          </article>

          <article class="col-lg-6">
            <div class="chat-panel panel panel-inverse">
              <div class="panel-heading">
                <h4>Últimas negociações atualizadas no Sistema</h4>
                <div class="pull-right"><span class="badge">238</span></div>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body">
                <ol class="chat">
                  <li>
                    Lorem ipsum dolor sit amet, bibendum
                  </li>
                  <li>
                    Lorem ipsum dolor sit amet, bibendum
                  </li>
                  <li>
                    Lorem ipsum dolor sit amet, bibendum
                  </li>
                  <li>
                    Lorem ipsum dolor sit amet, bibendum
                  </li>
                  <li>
                    Lorem ipsum dolor sit amet, bibendum
                  </li>
                </ol>
              </div>
            </div>
          </article>
        </section>
      </section>

      <section class="row" style="padding:0 20px ;">
        <div class="col-lg-6 col-md-6 ">
          <div class="panel-chat panel panel-inverse">
            <div class="panel-heading">
              <h4>Negociações em aberto por estado</h4>
              <div class="options">
                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
              </div>
            </div>
            <div class="panel-body map">

              <div class="table-list">
                <h5>Total por fase - 130</h5>

                <p>Negociação não iniciada <span>115</span></p>
                <p>Fechada <span>81</span></p>
                <p>Paralisada <span>68</span></p>
                <p>Dissídio coletivo <span>12</span></p>
                <p>Assembléia patronal <span>3</span></p>
                <p>Fechada parcialmente <span>2</span></p>
              </div>

              <?php include 'map.php' ?>
            </div>
          </div>
        </div>
        <div class="col-md-6 col-lg-6">
          <div class="panel panel-inverse">
            <div class="panel-heading">
              <h4>Negociações em aberto por Data-Base</h4>
              <div class="options">
                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
              </div>
            </div>
            <div class="panel-body">
              <canvas id="myChart"></canvas>
            </div>
          </div>

        </div>
      </section>

      <section class="row callendary">
        <div class="col-md-12 col-lg-12">
          <div class="panel panel-inverse">
            <div class="panel-heading">
              <h4>CALENDÁRIO SINDICAL</h4>
              <div class="options">
              </div>
            </div>
            <div class="panel-body">
              <div id="bar-example"></div>
            </div>
          </div>
        </div>
      </section>

    </div> <!-- page-content -->


    <?php include 'footer.php' ?>

  </div> <!-- page-container -->

  <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.8.0/chart.min.js"></script>
  <script src="includes/js/charts.js"></script>

  <script type='text/javascript' src='includes/js/jquery-1.10.2.min.js'></script>
  <script type='text/javascript' src='includes/js/jqueryui-1.10.3.min.js'></script>
  <script type='text/javascript' src='includes/js/bootstrap.min.js'></script>
  <script type='text/javascript' src='includes/js/enquire.js'></script>
  <script type='text/javascript' src='includes/js/jquery.cookie.js'></script>
  <script type='text/javascript' src='includes/js/jquery.nicescroll.min.js'></script>
  <script type='text/javascript' src='includes/plugins/codeprettifier/prettify.js'></script>
  <script type='text/javascript' src='includes/plugins/easypiechart/jquery.easypiechart.min.js'></script>
  <script type='text/javascript' src='includes/plugins/sparklines/jquery.sparklines.min.js'></script>
  <script type='text/javascript' src='includes/plugins/form-toggle/toggle.min.js'></script>
  <script type='text/javascript' src='includes/js/placeholdr.js'></script>
  <script type='text/javascript' src='includes/js/application.js'></script>
  <script type='text/javascript' src='includes/demo/demo.js'></script>

  <script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/TableTools.js'></script>
  <script type='text/javascript' src='includes/plugins/jquery-editable/jquery.editable.min.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.js'></script>
  <script type='text/javascript' src='includes/plugins/datatables/dataTables.bootstrap.js'></script>
  <script type='text/javascript' src='includes/demo/demo-datatables.js'></script>

  <script type='text/javascript' src='includes/demo/demo-modals.js'></script>





</body>

</html>