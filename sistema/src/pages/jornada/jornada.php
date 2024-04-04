<?php
session_start();
if (!$_SESSION) {
    echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}
$sessionUser = $_SESSION['login'];

/**
 * @author    {Enter5}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
        2020-08-28 13:40 ( v1.0.0 ) - 
    }
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÏ = 0


$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.'));

// Montando o c??o do erro que ser?presentado
$localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar = array("\\", "/includes/php");
$substituir = array("/", "");
$path = str_replace($localizar, $substituir, __DIR__);

$fileJornada = $path . '/includes/php/class.jornada.php';

if (file_exists($fileJornada)) {

    include_once($fileJornada);

    include_once __DIR__ . "/includes/php/class.usuario.php";

    $user = new usuario();
    $userData = $user->validateUser($sessionUser)['response_data']['user'];

    $modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

    $modulos = ["sisap" => $modulosSisap, "comercial" => []];

    $sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

    foreach ($sisap as $key => $value) {
        if (mb_strpos($value, "Adm - Jornadas")) {
            $indexModule = $key;
            $idModule = strstr($value, "+", true);
        }
    }

    for ($i = 0; $i < count($modulosSisap); $i++) {
        if ($modulosSisap[$i]->id == $idModule) {
            $thisModule = $modulosSisap[$i]; //module Permissions here
            break;
        }
    }


    $jornada = new jornada();



    if ($jornada->response['response_status']['status'] == 1) {

        $getJornada = $jornada->getJornada();

        if ($getJornada['response_status']['status'] == 1) {

            $lista = $getJornada['response_data']['html'];
        } else {
            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $error_code . __LINE__;
            $response['response_status']['msg'] = $getJornada['response_status']['error_code'] . '::' . $getJornada['response_status']['msg'];
        }

        $getJornadaCampos = $jornada->getJornadaCampos();

        if ($getJornadaCampos['response_status']['status'] == 1) {

            $superior = $getJornadaCampos['response_data']['superiores'];
            $listaSup = $getJornadaCampos['response_data']['listaSup'];
            $listaSupupdate = $getJornadaCampos['response_data']['listaSupupdate'];
        } else {
            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $error_code . __LINE__;
            $response['response_status']['msg'] = $getJornadaCampos['response_status']['error_code'] . '::' . $getClienteMatrizCampos['response_status']['msg'];
        }
    } else {
        $response['response_status']['status'] = 0;
        $response['response_status']['error_code'] = $error_code . __LINE__;
        $response['response_status']['msg'] = $jornada->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
    }
} else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.jornada).';
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


    <!-- The following CSS are included as plugins and can be removed if unused-->
    <script src="includes/js/jquery-3.4.1.min.js"></script>
    <link rel='stylesheet' type='text/css' href='includes/plugins/form-toggle/toggles.css' />
    <!-- <script type="text/javascript" src="includes/js/less.js"></script> -->
    <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />
    <link rel="stylesheet" href="includes/plugins/datagrid/styles/jplist-custom.css">
    <script src="keycloak.js"></script>
    <style>
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

        #page-content {
            min-height: 100% !important;
        }
    </style>
</head>

<body onload="initKeycloak()" style="display: none;" class="horizontal-nav">

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
                                        <?php if ($thisModule->Criar == 1): ?>
                                            <td><a data-toggle="modal" href="#myModal" class="btn default-alt  ">NOVA
                                                    JORNADA</a></td>
                                        <?php else: ?>

                                        <?php endif; ?>
                                    </tr>
                                </tbody>
                            </table>










                            <div id="superiorModal" class="modal" tabindex="-1" role="dialog">
                                <div class="modal-dialog" role="document">
                                    <div class="modal-content">
                                        <div class="panel panel-sky">
                                            <div class="panel-heading">
                                                <h4>Selecione o Superior Imediato</h4>
                                                <div class="options">
                                                    <a href="javascript:;" class="panel-collapse"><i
                                                            class="fa fa-chevron-down"></i></a>
                                                </div>
                                            </div>
                                            <div class="panel-body collapse in">

                                                <table cellpadding="0" cellspacing="0" border="0"
                                                    class="table table-striped table-bordered datatables" id="example">
                                                    <thead>
                                                        <tr>
                                                            <th></th>
                                                            <th>Foto</th>
                                                            <th>Nome do Usuário</th>
                                                            <th>E-mail do Usuário</th>
                                                            <th>Departamento</th>
                                                            <th>Cargo</th>
                                                            <th>Celular</th>
                                                            <th>Telefone</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <?php print $listaSup; ?>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button data-toggle="modal" href="#myModal" type="button"
                                                class="btn btn-secondary" data-dismiss="modal">Seguinte</button>
                                        </div>
                                    </div>
                                </div>
                            </div>


                            <div id="superiorModalupdate" class="modal" tabindex="-1" role="dialog">
                                <div class="modal-dialog" role="document">
                                    <div class="modal-content">
                                        <div class="panel panel-sky">
                                            <div class="panel-heading">
                                                <h4>Selecione o Superior Imediato</h4>
                                                <div class="options">
                                                    <a href="javascript:;" class="panel-collapse"><i
                                                            class="fa fa-chevron-down"></i></a>
                                                </div>
                                            </div>
                                            <div class="panel-body collapse in">

                                                <table cellpadding="0" cellspacing="0" border="0"
                                                    class="table table-striped table-bordered datatables" id="example">
                                                    <thead>
                                                        <tr>
                                                            <th></th>
                                                            <th>Foto</th>
                                                            <th>Nome do Usuário</th>
                                                            <th>E-mail do Usuário</th>
                                                            <th>Departamento</th>
                                                            <th>Cargo</th>
                                                            <th>Celular</th>
                                                            <th>Telefone</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <?php print $listaSupupdate; ?>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button data-toggle="modal" href="#updateModal" type="button"
                                                class="btn btn-secondary" data-dismiss="modal">Seguinte</button>
                                        </div>
                                    </div>
                                </div>
                            </div>











                            <div class="modal fade" id="updateModal" tabindex="-1" role="dialog"
                                aria-labelledby="myModalLabel" aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal"
                                                aria-hidden="true">&times;</button>
                                            <h4 class="modal-title">Atualização de Usuários</h4>
                                        </div>
                                        <div id="mensagem_alterado_sucessou"
                                            class="alert alert-dismissable alert-success">
                                            Atualização realizada com sucesso!
                                        </div>
                                        <div id="mensagem_alterado_erroru" class="alert alert-dismissable alert-danger">
                                            Não foi possível atualizar essa operação!
                                        </div>
                                        <div class="modal-body">
                                            <div class="panel panel-primary">
                                                <br>
                                                <!--Modal de Update = Atualizar -->
                                                <form class="form-horizontal">
                                                    <input type="hidden" id="id-inputu" value="1">
                                                    <div class="form-group">
                                                        <label for="desc-inputu"
                                                            class="col-sm-1 control-label">Descrição</label>
                                                        <div class="col-sm-11">
                                                            <input type="text" class="form-control" id="desc-inputu"
                                                                placeholder="">
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <table cellpadding="0" cellspacing="0" border="0"
                                                            class="col-sm-12 table table-striped table-bordered">
                                                            <thead>
                                                                <tr>
                                                                    <th>Dia</th>
                                                                    <th>Início</th>
                                                                    <th>Fim</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td>
                                                                        <h5>Segunda-feira</h5>
                                                                    </td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="segini-inputu" placeholder="00:00"></td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="segfim-inputu" placeholder="00:00"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <h5>Terça-feira</h5>
                                                                    </td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="terini-inputu" placeholder="00:00"></td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="terfim-inputu" placeholder="00:00"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <h5>Quarta-feira</h5>
                                                                    </td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="quaini-inputu" placeholder="00:00"></td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="quafim-inputu" placeholder="00:00"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <h5>Quinta-feira</h5>
                                                                    </td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="quiini-inputu" placeholder="00:00"></td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="quifim-inputu" placeholder="00:00"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <h5>Sexta-feira</h5>
                                                                    </td>

                                                                    <td><input type="text" class="form-control"
                                                                            id="sexini-inputu" placeholder="00:00"></td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="sexfim-inputu" placeholder="00:00"></td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>

                                                    <h3>Configurações</h3>

                                                    <div class="form-group">
                                                        <div class="col-sm-3">
                                                            <input type="checkbox" id="padr-inputu">
                                                            <label for="padr-inputu">Padrão do sistema</label>
                                                        </div>
                                                    </div>
                                                </form>
                                                <div class="row">
                                                    <div class="col-sm-6 col-sm-offset-3">
                                                        <div class="btn-toolbar">
                                                            <?php if ($thisModule->Alterar == 1): ?>
                                                                <a id="btn-atualizar" href="#"
                                                                    class="btn btn-primary btn-rounded">Processar</a>
                                                            <?php else: ?>
                                                            <?php endif; ?>
                                                            <a id="btn-cancelar2" href="#"
                                                                class="btn btn-primary btn-rounded">Finalizar</a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- 
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                            <button type="button" class="btn btn-primary">Save changes</button>
                                        </div> -->
                                    </div><!-- /.modal-content -->
                                </div><!-- /.modal-dialog -->
                            </div><!-- /.modal -->

                            <div class="modal fade" id="myModal" tabindex="-1" role="dialog"
                                aria-labelledby="myModalLabel" aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal"
                                                aria-hidden="true">&times;</button>
                                            <h4 class="modal-title">Cadastro de Jornadas</h4>
                                        </div>
                                        <div id="mensagem_sucesso" class="alert alert-dismissable alert-success">
                                            Cadastro realizado com sucesso!
                                        </div>
                                        <div id="mensagem_error" class="alert alert-dismissable alert-danger">
                                            Não foi possível realizar essa operação!
                                        </div>
                                        <div class="modal-body">
                                            <div class="panel panel-primary">
                                                <br>
                                                <!-- Modal de Insert = Inserir -->
                                                <form class="form-horizontal">
                                                    <div class="form-group">
                                                        <label for="desc-input"
                                                            class="col-sm-1 control-label">Descrição</label>
                                                        <div class="col-sm-11">
                                                            <input type="text" class="form-control" id="desc-input"
                                                                placeholder="">
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <table cellpadding="0" cellspacing="0" border="0"
                                                            class="col-sm-12 table table-striped table-bordered">
                                                            <thead>
                                                                <tr>
                                                                    <th>Dia</th>
                                                                    <th>Início</th>
                                                                    <th>Fim</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td>
                                                                        <h5>Segunda-feira</h5>
                                                                    </td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="segini-input" placeholder="00:00"></td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="segfim-input" placeholder="00:00"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <h5>Terça-feira</h5>
                                                                    </td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="terini-input" placeholder="00:00"></td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="terfim-input" placeholder="00:00"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <h5>Quarta-feira</h5>
                                                                    </td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="quaini-input" placeholder="00:00"></td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="quafim-input" placeholder="00:00"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <h5>Quinta-feira</h5>
                                                                    </td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="quiini-input" placeholder="00:00"></td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="quifim-input" placeholder="00:00"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <h5>Sexta-feira</h5>
                                                                    </td>

                                                                    <td><input type="text" class="form-control"
                                                                            id="sexini-input" placeholder="00:00"></td>
                                                                    <td><input type="text" class="form-control"
                                                                            id="sexfim-input" placeholder="00:00"></td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>

                                                    <h3>Configurações</h3>

                                                    <div class="form-group">
                                                        <div class="col-sm-3">
                                                            <input type="checkbox" id="padr-input">
                                                            <label for="padr-input">Padrão do sistema</label>
                                                        </div>
                                                    </div>
                                                </form>
                                                <!-- Tratar buttons de validações -->
                                                <div class="row">
                                                    <div class="col-sm-6 col-sm-offset-3">
                                                        <div class="btn-toolbar">
                                                            <a href="#" class="btn btn-primary btn-rounded"
                                                                onclick="addJornada();">Processar</a>
                                                            <a id="btn-cancelar" href="#"
                                                                class="btn btn-primary btn-rounded">Finalizar</a>
                                                        </div>
                                                    </div>
                                                </div>




                                            </div>
                                        </div>
                                        <!-- 
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                            <button type="button" class="btn btn-primary">Save changes</button>
                                        </div> -->
                                    </div><!-- /.modal-content -->
                                </div><!-- /.modal-dialog -->
                            </div><!-- /.modal -->
                        </div>
                    </div>

                    <div class="row" style="display: flex;">
                        <div class="col-md-12">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h4>Cadastro de Jornadas</h4>
                                    <div class="options">
                                        <a href="javascript:;" class="panel-collapse"><i
                                                class="fa fa-chevron-down"></i></a>
                                    </div>
                                </div>
                                <div class="panel-body collapse in">
                                    <div id="grid-layout-table-1" class="box jplist">
                                        <div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions
                                        </div>
                                        <div class="jplist-panel box panel-top">
                                            <button type="button" data-control-type="reset" data-control-name="reset"
                                                data-control-action="reset"
                                                class="jplist-reset-btn btn btn-primary">Limpar
                                                <i class="fa fa-share mls"></i></button>
                                            <div data-control-type="drop-down" data-control-name="paging"
                                                data-control-action="paging" class="jplist-drop-down form-control">
                                                <ul class="dropdown-menu">
                                                    <li><span data-number="3"> 3 por página</span></li>
                                                    <li><span data-number="5" data-default="true"> 5 por página</span>
                                                    </li>
                                                    <li><span data-number="10"> 10 por página</span></li>
                                                    <li><span data-number="all"> ver todos</span></li>
                                                </ul>
                                            </div>
                                            <div data-control-type="drop-down" data-control-name="sort"
                                                data-control-action="sort" data-datetime-format="{month}/{day}/{year}"
                                                class="jplist-drop-down form-control">
                                                <ul class="dropdown-menu">
                                                    <li><span data-path="default">Listar por</span></li>
                                                    <li><span data-path=".title" data-order="asc"
                                                            data-type="text">Descrição
                                                            A-Z</span></li>
                                                    <li><span data-path=".title" data-order="desc"
                                                            data-type="text">Descrição Z-A</span></li>
                                                </ul>
                                            </div>
                                            <div class="text-filter-box">
                                                <div class="input-group"><span class="input-group-addon"><i
                                                            class="fa fa-search"></i></span><input data-path=".title"
                                                        type="text" value="" placeholder="Filtrar por Descrição"
                                                        data-control-type="textbox" data-control-name="title-filter"
                                                        data-control-action="filter" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="box text-shadow">
                                            <table cellpadding="0" cellspacing="0" border="0"
                                                class="table table-striped table-bordered demo-tbl">
                                                <thead>
                                                    <tr>
                                                        <th></th>
                                                        <th>Descrição</th>
                                                        <th>Jornada semanal</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <?php print $lista; ?>
                                                </tbody>
                                            </table>
                                        </div>
                                        <div class="box jplist-no-results text-shadow align-center">
                                            <p>Nenhum resultado encontrado</p>
                                        </div>
                                        <div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions
                                        </div>
                                        <div class="jplist-panel box panel-bottom">
                                            <div data-type="{start} - {end} de {all}"
                                                data-control-type="pagination-info" data-control-name="paging"
                                                data-control-action="paging" class="jplist-label btn btn-primary"></div>
                                            <div data-control-type="pagination" data-control-name="paging"
                                                data-control-action="paging" data-control-animate-to-top="true"
                                                class="jplist-pagination"></div>
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

    <script type='text/javascript' src="includes/js/profile.js"></script>

    <script type='text/javascript' src="includes/js/jornada.js"></script>



    <script src="includes/plugins/sweet-alert/all.js"></script>
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
    <script type='text/javascript'
        src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.15/jquery.mask.min.js"></script>
    <script type='text/javascript'
        src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.15/jquery.mask.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.4.1/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript"
        src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.9.0/moment-with-locales.js"></script>



    <!-- <script>!window.jQuery && document.write(unescape('%3Cscript src="includes/js/jquery-1.10.2.min.js"%3E%3C/script%3E'))</script>
<script type="text/javascript">!window.jQuery.ui && document.write(unescape('%3Cscript src="includes/js/jqueryui-1.10.3.min.js'))</script>
<script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
<script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script> -->

    <!-- new datagrid -->
    <script src="includes/plugins/datagrid/script/jquery.metisMenu.js"></script>
    <script src="includes/plugins/datagrid/script/jquery.slimscroll.js"></script>
    <script src="includes/plugins/datagrid/script/jquery.flot.js"></script>
    <script src="includes/plugins/datagrid/script/jquery.flot.categories.js"></script>
    <script src="includes/plugins/datagrid/script/jquery.flot.pie.js"></script>
    <script src="includes/plugins/datagrid/script/jquery.flot.tooltip.js"></script>
    <script src="includes/plugins/datagrid/script/jquery.flot.resize.js"></script>
    <script src="includes/plugins/datagrid/script/jquery.flot.fillbetween.js"></script>
    <script src="includes/plugins/datagrid/script/jquery.flot.stack.js"></script>
    <script src="includes/plugins/datagrid/script/jquery.flot.spline.js"></script>
    <script src="includes/plugins/datagrid/script/jplist.min.js"></script>
    <script src="includes/plugins/datagrid/script/jplist.js"></script>
    <script src="includes/plugins/datagrid/script/main.js"></script>


</body>

</html>