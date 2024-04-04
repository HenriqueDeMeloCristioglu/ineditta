<!DOCTYPE html>
<html lang="pt-br">

<head>
    <meta charset="utf-8">
    <title>Ineditta</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="Ineditta">
    <meta name="author" content="The Red Team">

    <link rel="stylesheet" href="../../includes/css/styles.css">
    <link rel="stylesheet" href="../../includes/css/mystyle.css">
    <link rel="stylesheet" href="../../includes/plugins/charts-morrisjs/morris.css">
    <link rel="stylesheet" href="../../includes/plugins/codeprettifier/prettify.css">
    <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>

</head>

<body class="horizontal-nav">

  <?php require '../components/menu.php'; ?>


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
                                <div class="pull-left">
                                  <i class="fa fa-filter"></i>
                                </div>
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
                                <div class="pull-left">
                                  <i class="fa fa-filter"></i>
                                </div>
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
                                <div class="pull-left">
                                  <i class="fa fa-filter"></i>
                                </div>
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
                                <div class="pull-left">
                                   <i class="fa fa-download"></i>
                                </div>
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

                            <?php require 'map.php' ?>
                        </div>
                    </div>
                </div>
                <div class="col-md-6 col-lg-6">
                    <!-- <div class="panel panel-inverse">
                        <div class="panel-heading">
                            <h4>Negociações em aberto por Data-Base</h4>
                        </div>
                        <div class="panel-body">
                            <canvas id="bar-chart" height="300"  width="800"></canvas>
                        </div>
                    </div> -->
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

            <section class="index row" style="padding-left: 20px ;">
                <!-- <header>
                    <h1>INDICES ECONÔMICOS (INPC e IPCA)</h1>
                </header> -->


                <div class="col-md-12 col-lg-12">
                    <div class="panel panel-inverse">
                        <div class="panel-heading">
                            <h4>Tabela 1: Quantidade e estrutura das negociações no 1º semestre (2021 e 2022) </h4>
                            <div class="options">
                                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                            </div>
                        </div>
                        <div class="panel-body">
                            <table class="table table-hover table-bordered">
                                <thead>
                                    <th>Estrutura da Negociação</th>
                                    <th>Quantidade</th>
                                    <th></th>
                                    <th>Proporção</th>
                                    <th></th>
                                </thead>
                                <thead>
                                    <th></th>
                                    <th>2020</th>
                                    <th>2021</th>
                                    <th>2020</th>
                                    <th>2021</th>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>Acordos Coletivos</td>
                                        <td>20.397</td>
                                        <td>10.056</td>
                                        <td>86,5%</td>
                                        <td>85,3%</td>
                                    </tr>
                                    <tr>
                                        <td>Convenções Coletivas</td>
                                        <td>3.175</td>
                                        <td>1.732</td>
                                        <td>13,5%</td>
                                        <td>14,7%</td>
                                    </tr>
                                    <tr>
                                        <td>Total</td>
                                        <td>23.572</td>
                                        <td>11.788</td>
                                        <td>100,0%</td>
                                        <td>100,0%</td>
                                    </tr>
                                </tbody>
                            </table>
                            <p>Fonte: Base de Dados Ineditta e Sindicatos</p>
                            <p>Elaboração: Ineditta</p>
                            <p>Data da Ultima Atualização: 04/03/2022</p>
                        </div>
                    </div>
                </div>
            </section>

            <section class="index row" style="margin-top: 50px ;">
                <header style="padding:0 10px ;">
                    <h1>INDICES ECONÔMICOS (INPC e IPCA)</h1>
                </header>
                <div class="col-lg-4 col-md-4">
                    <div class="col-lg-12 col-md-12 col-sm-12 ">
                        <a class="info-tiles tiles-inverse" href="#">
                            <div class="tiles-heading">
                                <div class="pull-left">INPC - Ultimos 12 meses</div>
                            </div>
                            <div class="tiles-body tiles-index">
                                <div class="pull-left">5,688%</div>
                                <div class="pull-right bottom-index">0,755% Fev 2022</div>

                            </div>
                        </a>
                    </div>

                    <div class="col-lg-12 col-md-12 col-sm-12 " style="margin-top: 20px;">
                        <a class="info-tiles tiles-inverse" href="#">
                            <div class="tiles-heading">
                                <div class="pull-left">IPCA - Ultimos 12 meses</div>
                            </div>
                            <div class="tiles-body tiles-index">
                                <div class="pull-left">5,688%</div>
                                <div class="pull-right bottom-index">0,755% Fev 2022</div>
                            </div>
                        </a>
                    </div>
                </div>

                <div class="col-md-8 col-lg-8">
                    <div class="panel panel-inverse">
                        <div class="panel-heading">
                            <h4>Valor do IPCA</h4>
                            <div class="options">
                                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                            </div>
                        </div>
                        <div class="panel-body">
                            <canvas id="lineChart" height="100"></canvas>
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
                                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                            </div>
                        </div>
                        <div class="panel-body">
                            <div id="bar-example"></div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- <section class="row footer-section">
                <div class="col-lg-3 col-md-6 col-sm-12">
                    <h1>Ineditta</h1>
                    <p><a href="#">Termos de uso</a></p>
                    <p>Versão 3.0.14</p>
                </div>

                <div class="col-lg-3 col-md-6 col-sm-12">
                    <h2>Canais de contato</h2>
                    <div>
                        <h3>Atendimento</h3>
                        <p><a href="#">Registrar solicitação</a></p>
                    </div>
                    <div>
                        <h3>Telefone</h3>
                        <p>(11)2233-1454</p>
                    </div>
                    <div>
                        <h3>E-mail</h3>
                        <p>suporte@ineditta.com.br</p>
                    </div>
                </div>

                <div class="col-lg-3 col-md-6 col-sm-12">
                    <h2>Links úteis</h2>
                    <p><a href="#">Acesse aqui o manual do sistema</a></p>
                </div>

                <div class="col-lg-3 col-md-6 col-sm-12">
                    <p>&copy; 2022 - ineditta.com.br</p>
                    <p>Todos os direitos reservados</p>
                </div>
            </section> -->


        </div> <!-- page-content -->

        <?php include 'footer.php' ?>

    </div> <!-- page-container -->

    <script type='text/javascript' src='../../includes/demo/demo.js'></script>
    <script type='text/javascript' src='../../includes/demo/demo-datatables.js'></script>
    <script type='text/javascript' src='../../includes/demo/demo-modals.js'></script>
    <script type='text/javascript' src='../../js/core/tools.js'></script>
    <script type='text/javascript' src="../../js/core/charts.js"></script>

</body>

</html>
