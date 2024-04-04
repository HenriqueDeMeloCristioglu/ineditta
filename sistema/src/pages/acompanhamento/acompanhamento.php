<?php
session_start();
if (!$_SESSION) {
    echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

/**
 * @author    {Enter5}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
        2020-08-28 13:40 ( v1.0.0 ) - 
    }
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÏ = 0



//header('charset=UTF-8; Content-type: text/html; Cache-Control: no-cache');



$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.'));

// Montando o c??o do erro que ser?presentado
$localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar = array("\\", "/includes/php");
$substituir = array("/", "");
$path = str_replace($localizar, $substituir, __DIR__);
$fileClassFedemp = $path . '/includes/php/class.acompanhamento.php';

if (file_exists($fileClassFedemp)) {

    include_once($fileClassFedemp);

    $acompanhamento = new acompanhamento();

    if ($acompanhamento->response['response_status']['status'] == 1) {

        $getAcompanhamento = $acompanhamento->getAcompanhamento();

        if ($getAcompanhamento['response_status']['status'] == 1) {

            $lista = $getAcompanhamento['response_data']['html'];
        } else {
            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $error_code . __LINE__;
            $response['response_status']['msg'] = $getAcompanhamento['response_status']['error_code'] . '::' . $getAcompanhamento['response_status']['msg'];
        }
    } else {
        $response['response_status']['status'] = 0;
        $response['response_status']['error_code'] = $error_code . __LINE__;
        $response['response_status']['msg'] = $acompanhamento->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
    }
} else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.clienteusuarios).';
}

if ($response['response_status']['status'] == 0) {

    print $response['response_status']['error_code'] . " :: " . $response['response_status']['msg'];
    exit();
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

    <!-- <link href="includes/less/styles.less" rel="stylesheet/less" media="all">  -->
    <link rel="stylesheet" href="includes/css/styles.css">
    <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css">
    <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='styleswitcher'>
    <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='headerswitcher'>

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries. Placeholdr.js enables the placeholder attribute -->
    <!--[if lt IE 9]>
        <script type="text/javascript" src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
        <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/respond.js/1.1.0/respond.min.js"></script>
        <script type="text/javascript" src="includes/plugins/charts-flot/excanvas.min.js"></script>
    <![endif]-->

    <!-- The following CSS are included as plugins and can be removed if unused-->
    <script src="includes/js/jquery-3.4.1.min.js"></script>
    <link rel='stylesheet' type='text/css' href='includes/plugins/form-toggle/toggles.css' />
    <link rel='stylesheet' type='text/css' href='includes/plugins/gridstrap/gridstrap.css' />
    <!-- <script type="text/javascript" src="includes/js/less.js"></script> -->
    <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

    <script src="keycloak.js"></script>

    <style>
        select {
            display: block !important;
        }

        #mensagem_sucesso {
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

        /* The heart of the matter */
        .testimonial-group>.row {
            overflow-x: auto;
            white-space: nowrap;
        }

        .testimonial-group>.row>.col-md-3 {
            display: inline-grid;
            float: none;
        }

        .trava {
            background-color: white;
            opacity: 0;
            width: 100%;
            height: 1vh;
            /* custom */
        }

        .trava:active {
            pointer-events: none;
        }

        .trava:click {
            pointer-events: none;
        }

        .coluna {
            background-color: rgba(56, 116, 214, 0.5);
            padding-right: 1vw;
            border-radius: 10px;
            min-height: 5vh;
        }

        .card {
            background-color: white;
            width: 100%;
            display: flex;
            flex-direction: column;
            padding: 0.5vw;
            margin: 0.5vw;
            border-radius: 10px;
            border-style: solid;
            border-color: #dedede;
            white-space: pre-line;
            border-width: 0.25em;
            /* custom */
        }

        .card h1,
        .card h2,
        .card h3,
        .card h4,
        .card h5 {
            margin: 0px;
            padding: 0px 0px 0.25vh 0px;
            font-family: "Noto Sans KR", sans-serif;
            font-size: 1em;
            color: #282828;
        }

        .card hr {
            display: block;
            border: none;
            height: 1.5px;
            background-color: #007bff;
            margin: 0px;
        }

        .card p {
            margin: 15px 0px 0px 0px;
            font-family: "Noto Sans KR", sans-serif;
            font-weight: 100;
            letter-spacing: -0.15px;
            line-height: 0.35;
            font-size: 0.85em;
            word-break: keep-all;
            word-wrap: pre-wrap;
            color: #282828;
        }

        .card button {
            border: none;
            background-color: #007bff;
            margin-top: 0.5vw;
            margin-bottom: 0.25vw;
            padding: 0.25vw;
            color: white;
            font-size: 0.85em;
            font-family: "Noto Sans KR", sans-serif;
            text-transform: uppercase;
            border-radius: 10px;
        }

        /* Safari 4.0 - 8.0 */
        @-webkit-keyframes line-show {
            from {
                margin: 0px 100px;
            }

            to {
                margin: 0px;
            }
        }

        /* Standard syntax */
        @keyframes line-show {
            from {
                margin: 0px 100px;
            }

            to {
                margin: 0px;
            }
        }

        /* Safari 4.0 - 8.0 */
        @-webkit-keyframes p-show {
            from {
                color: white;
            }

            to {
                color: #282828;
            }
        }

        /* Standard syntax */
        @keyframes p-show {
            from {
                color: white;
            }

            to {
                color: #282828;
            }
        }

        /* Safari 4.0 - 8.0 */
        @-webkit-keyframes shadow-show {
            from {
                box-shadow: 0px 0px 0px 0px #e0e0e0;
            }

            to {
                box-shadow: -20px -20px 0px 0px #fb968b;
            }
        }

        /* Standard syntax */
        @keyframes shadow-show {
            from {
                box-shadow: 0px 0px 0px 0px #e0e0e0;
            }

            to {
                box-shadow: -20px -20px 0px 0px #fb968b;
            }
        }

        .over {
            border: 2px dashed #000;
        }

        [draggable] {
            -moz-user-select: none;
            -khtml-user-select: none;
            -webkit-user-select: none;
            user-select: none;
        }

        #page-content {
            min-height: 100% !important;
        }
    </style>
</head>

<body onload="initKeycloak()" class="horizontal-nav">
    <?php include('menu.php'); ?>

    <div id="page-container">

        <div id="page-content">
            <div id="wrap">
                <div class="container">
                    <div class="row" style="display: flex;">
                        <div class="col-md-1 img_container"> <!-- style="transform: translate(-13px, 13px);" -->
                            <div class="container_logo_client">
                                <img id="imglogo" class="img-circle">
                            </div>
                        </div>

                        <div class="col-md-11 content_container">
                            <table class="table table-striped">

                                <tbody>
                                    <tr>
                                        <td>
                                            <a data-toggle="modal" href="#myModal" class="btn default-alt  ">NOVO
                                                ACOMPANHAMENTO</a>
                                        </td>
                                        <td align="right">

                                            <a id="left-button" class="btn default-alt  "><i
                                                    class="fa fa-arrow-left"></i></a>
                                            <a id="right-button" class="btn default-alt  "><i
                                                    class="fa fa-arrow-right"></i></a>

                                        </td>
                                    </tr>
                                </tbody>
                            </table>


                            <div id="kanban" class="container testimonial-group">
                                <div id="colunas" class="row">
                                    <div class="col-md-3">
                                        <h3><b>Fechadas</b></h3>
                                        <div id="try0" class="coluna row">
                                            <div class="trava col-md-12"></div>

                                            <div class="card col-md-12" draggable="true">
                                                <h2>#177013</h2>
                                                <hr />
                                                <p>Status: <spam style="color:red">Urgente</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="trava col-md-12"></div>



                                        </div>
                                    </div>
                                    <div class="col-md-3">
                                        <h3><b>Fechada Parcialmente</b></h3>
                                        <div id="try1" class="coluna row">
                                            <div class="trava col-md-12"></div>

                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="trava col-md-12"></div>



                                        </div>
                                    </div>

                                    <div class="col-md-3">
                                        <h3><b>Em Negociação</b></h3>
                                        <div id="try2" class="coluna row">
                                            <div class="trava col-md-12"></div>

                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>

                                            <div class="trava col-md-12"></div>


                                        </div>
                                    </div>

                                    <div class="col-md-3">
                                        <h3><b>Assembleia patronal</b></h3>
                                        <div id="try3" class="coluna row">
                                            <div class="trava col-md-12"></div>



                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="trava col-md-12"></div>


                                        </div>
                                    </div>

                                    <div class="col-md-3">
                                        <h3><b> Negociação não iniciada</b></h3>
                                        <div id="try4" class="coluna row">
                                            <div class="trava col-md-12"></div>


                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="trava col-md-12"></div>



                                        </div>
                                    </div>

                                    <div class="col-md-3">
                                        <h3><b>Prorrogação CCT</b></h3>
                                        <div id="try5" class="coluna row">
                                            <div class="trava col-md-12"></div>


                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>

                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="trava col-md-12"></div>



                                        </div>
                                    </div>

                                    <div class="col-md-3">
                                        <h3><b>Dissídio Coletivo</b></h3>
                                        <div id="try6" class="coluna row">
                                            <div class="trava col-md-12"></div>


                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>

                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="trava col-md-12"></div>



                                        </div>
                                    </div>

                                    <div class="col-md-3">
                                        <h3><b>Paralisada</b></h3>
                                        <div id="try7" class="coluna row">
                                            <div class="trava col-md-12"></div>


                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>

                                            <div class="card col-md-12" draggable="true">
                                                <h2>#181228</h2>
                                                <hr />
                                                <p>Status: <spam style="color:darkgoldenrod">Padrão</spam>
                                                </p>
                                                <p>Partes envolvidas: SINDCATO1, SINDICATO2</p>
                                                <p>CNAE: 7777777</p>
                                                <p>Ínicio: 01/07/2022</p>
                                                <p>Fim previsto: 21/07/2022</p>
                                                <p>Ultima atualização: 14/07/2022</p>
                                                <p>Responsável: <b>Paulo Oliveira</b></p>
                                                <button>Editar <i class="fa fa-file-text"></i></button>
                                            </div>
                                            <div class="trava col-md-12"></div>



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
    <script type='text/javascript' src="includes/js/acompanhamento.js"></script>




    <script type='text/javascript' src='includes/js/jquery-1.10.2.min.js'></script>
    <script type='text/javascript' src='includes/js/jquery-3.4.1.min.js'></script>
    <script type='text/javascript' src='includes/js/jqueryui-1.10.3.min.js'></script>
    <script type='text/javascript' src='includes/js/bootstrap.min.js'></script>
    <script type='text/javascript' src='includes/js/enquire.js'></script>
    <script type='text/javascript' src='includes/js/jquery.cookie.js'></script>
    <script type='text/javascript' src='includes/js/jquery.nicescroll.min.js'></script>
    <script type='text/javascript' src='includes/plugins/codeprettifier/prettify.js'></script>
    <script type='text/javascript' src='includes/plugins/easypiechart/jquery.easypiechart.min.js'></script>
    <script type='text/javascript' src='includes/plugins/sparklines/jquery.sparklines.min.js'></script>
    <script type='text/javascript' src='includes/plugins/form-toggle/toggle.min.js'></script>
    <script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.js'></script>
    <script type='text/javascript' src='includes/plugins/datatables/TableTools.js'></script>
    <script type='text/javascript' src='includes/plugins/jquery-editable/jquery.editable.min.js'></script>
    <script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.js'></script>
    <script type='text/javascript' src='includes/plugins/datatables/dataTables.bootstrap.js'></script>
    <script type='text/javascript' src='includes/demo/demo-datatables.js'></script>
    <script type='text/javascript' src='includes/js/placeholdr.js'></script>
    <script type='text/javascript' src='includes/demo/demo-modals.js'></script>
    <script type='text/javascript' src='includes/js/application.js'></script>
    <script type='text/javascript' src='includes/demo/demo.js'></script>
    <script type='text/javascript' src='includes/plugins/gridstrap/gridstrap.js'></script>

    <script type='text/javascript'
        src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.15/jquery.mask.min.js"></script>



</body>

</html>