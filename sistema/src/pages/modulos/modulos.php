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

$fileModu = $path . '/includes/php/class.modulos.php';

if (file_exists($fileModu)) {

    include_once($fileModu);

    include_once __DIR__ . "/includes/php/class.usuario.php";

    $user = new usuario();
    $userData = $user->validateUser($sessionUser)['response_data']['user'];

    $modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

    $modulos = ["sisap" => $modulosSisap, "comercial" => []];

    $sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

    foreach ($sisap as $key => $value) {
        if (mb_strpos($value, "Adm - Cadastro de Módulos")) {
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

    $modulos = new modulos();

    if ($modulos->response['response_status']['status'] == 1) {

        $getModulos = $modulos->getModulos();

        if ($getModulos['response_status']['status'] == 1) {

            $lista = $getModulos['response_data']['html'];
        } else {
            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $error_code . __LINE__;
            $response['response_status']['msg'] = $getModulos['response_status']['error_code'] . '::' . $getModulos['response_status']['msg'];
        }
    } else {
        $response['response_status']['status'] = 0;
        $response['response_status']['error_code'] = $error_code . __LINE__;
        $response['response_status']['msg'] = $modulos->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
    }
} else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.modulos).';
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

        .container-addon {
            display: flex;
        }

        .container-addon input {
            border-radius: 1px 0px 0px 1px;
            border-right: none;
        }

        .container-addon span.extension-file-addon {
            padding: 6px 9px;
            color: #495057;
            background-color: #e9ecef;
            border: 1px solid #d2d3d6;
            border-radius: 0px 1px 1px 0px;
        }

        .bootbox.modal {
            display: none !important;
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
                                        <?php if ($thisModule->Criar == 1): ?>
                                            <td><a data-toggle="modal" href="#myModal" class="btn default-alt  ">NOVO
                                                    MÓDULO</a></td>
                                        <?php else: ?>

                                        <?php endif; ?>
                                    </tr>
                                </tbody>
                            </table>

                            <div class="modal fade" id="updateModal" tabindex="-1" role="dialog"
                                aria-labelledby="myModalLabel" aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header" style="border-bottom: none;">
                                            <button type="button" class="close" data-dismiss="modal"
                                                aria-hidden="true">&times;</button>
                                            <h4 class="modal-title"></h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="panel panel-primary">
                                                <form class="form-horizontal">
                                                    <div class="panel panel-primary" style="margin: 0;">
                                                        <div class="panel-heading">
                                                            <h4>Atualização dos Módulos</h4>
                                                            <div class="options">
                                                                <a href="javascript:;" class="panel-collapse"><i
                                                                        class="fa fa-chevron-down"></i></a>
                                                            </div>
                                                        </div>
                                                        <div class="panel-body collapse in principal-table">
                                                            <input type="hidden" id="id-inputu" value="1">

                                                            <div class="form-group">
                                                                <div class="col-sm-6">
                                                                    <label for="mod-inputu">Nome do módulo</label>
                                                                    <input type="text" class="form-control"
                                                                        id="mod-inputu" placeholder="">
                                                                </div>

                                                                <div class="col-sm-6">
                                                                    <label for="name-file">Nome do arquivo</label>
                                                                    <div class="container-addon">
                                                                        <input type="text" class="form-control"
                                                                            id="name-file-update">
                                                                        <span class="extension-file-addon">.php</span>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="form-group">
                                                                <div class="col-sm-3">
                                                                    <label for="check-insert-u">Possui
                                                                        <strong>insert</strong></label>
                                                                    <select class="form-control" name="check-insert-u"
                                                                        id="check-insert-u">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>

                                                                <div class="col-sm-3">
                                                                    <label for="check-select-u">Possui
                                                                        <strong>select</strong></label>
                                                                    <select class="form-control" name="check-select-u"
                                                                        id="check-select-u">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>

                                                                <div class="col-sm-3">
                                                                    <label for="check-delete-u">Possui
                                                                        <strong>delete</strong></label>
                                                                    <select class="form-control" name="check-delete-u"
                                                                        id="check-delete-u">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>

                                                                <div class="col-sm-3">
                                                                    <label for="check-update-u">Possui
                                                                        <strong>update</strong></label>
                                                                    <select class="form-control" name="check-update-u"
                                                                        id="check-update-u">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>
                                                            </div>

                                                            <div class="form-group">
                                                                <div class="col-sm-4">
                                                                    <label for="tipo-inputu">Tipo de menu</label>
                                                                    <select class="form-control" name="tipo-inputu"
                                                                        id="tipo-inputu">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="Comercial">Comercial</option>
                                                                            <option value="SISAP">SISAP</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>

                                                                <div class="col-sm-4">
                                                                    <label for="check-comentario-u">Possui
                                                                        <strong>comentario</strong></label>
                                                                    <select class="form-control"
                                                                        name="check-comentario-u"
                                                                        id="check-comentario-u">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>

                                                                <div class="col-sm-4">
                                                                    <label for="check-aprovar-u">Possui
                                                                        <strong>aprovar</strong></label>
                                                                    <select class="form-control" name="check-aprovar-u"
                                                                        id="check-aprovar-u">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>
                                                            </div>

                                                            <div class="form-group" style="margin-bottom: 0;">
                                                                <div class="col-sm-6 col-sm-offset-3">
                                                                    <div class="btn-toolbar"
                                                                        style="text-align: center;">
                                                                        <?php if ($thisModule->Alterar == 1): ?>
                                                                            <a id="btn-atualizar" href="#"
                                                                                class="btn btn-primary btn-rounded">Processar</a>
                                                                        <?php else: ?>
                                                                        <?php endif; ?>
                                                                        <a id="btn-cancelar2" href="#"
                                                                            class="btn btn-danger btn-rounded btn-cancelar">Finalizar</a>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <!-- Tratar butons de confirmação -->
                                                        </div>
                                                    </div>
                                                </form>
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
                                        <div class="modal-header" style="border-bottom: none;">
                                            <button type="button" class="close" data-dismiss="modal"
                                                aria-hidden="true">&times;</button>
                                            <h4 class="modal-title"></h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="panel panel-primary">
                                                <form class="form-horizontal">
                                                    <div class="panel panel-primary" style="margin: 0;">
                                                        <div class="panel-heading">
                                                            <h4>Cadastro dos Módulos</h4>
                                                            <div class="options">
                                                                <a href="javascript:;" class="panel-collapse"><i
                                                                        class="fa fa-chevron-down"></i></a>
                                                            </div>
                                                        </div>
                                                        <div class="panel-body collapse in principal-table">
                                                            <div class="form-group">
                                                                <div class="col-sm-6">
                                                                    <label for="mod-inputc">Nome do módulo</label>
                                                                    <input type="text" class="form-control"
                                                                        id="mod-inputc" placeholder="">
                                                                </div>

                                                                <div class="col-sm-6">
                                                                    <label for="name-file">Nome do arquivo</label>
                                                                    <div class="container-addon">
                                                                        <input type="text" class="form-control"
                                                                            id="name-file">
                                                                        <span class="extension-file-addon">.php</span>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="form-group">
                                                                <div class="col-sm-3">
                                                                    <label for="check-insert">Possui
                                                                        <strong>insert</strong></label>
                                                                    <select class="form-control" name="check-insert"
                                                                        id="check-insert">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>

                                                                <div class="col-sm-3">
                                                                    <label for="check-select">Possui
                                                                        <strong>select</strong></label>
                                                                    <select class="form-control" name="check-select"
                                                                        id="check-select">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>

                                                                <div class="col-sm-3">
                                                                    <label for="check-delete">Possui
                                                                        <strong>delete</strong></label>
                                                                    <select class="form-control" name="check-delete"
                                                                        id="check-delete">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>

                                                                <div class="col-sm-3">
                                                                    <label for="check-update">Possui
                                                                        <strong>update</strong></label>
                                                                    <select class="form-control" name="check-update"
                                                                        id="check-update">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>
                                                            </div>

                                                            <div class="form-group">
                                                                <div class="col-sm-4">
                                                                    <label for="tipo-input">Tipo de menu</label>
                                                                    <select class="form-control" name="tipo-input"
                                                                        id="tipo-input">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="Comercial" selected>Comercial
                                                                            </option>
                                                                            <option value="SISAP">SISAP</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>

                                                                <div class="col-sm-4">
                                                                    <label for="check-comentario">Possui
                                                                        <strong>comentario</strong></label>
                                                                    <select class="form-control" name="check-comentario"
                                                                        id="check-comentario">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>

                                                                <div class="col-sm-4">
                                                                    <label for="check-aprovar">Possui
                                                                        <strong>aprovar</strong></label>
                                                                    <select class="form-control" name="check-aprovar"
                                                                        id="check-aprovar">
                                                                        <optgroup label="SELECIONE">
                                                                            <option value="S">Sim</option>
                                                                            <option value="N">Nao Aplica</option>
                                                                        </optgroup>
                                                                    </select>
                                                                </div>
                                                            </div>

                                                            <div class="form-group" style="margin-bottom: 0;">
                                                                <div class="col-sm-6 col-sm-offset-3">
                                                                    <div class="btn-toolbar"
                                                                        style="text-align: center;">
                                                                        <a href="#" class="btn btn-primary btn-rounded"
                                                                            onclick="addMd();">Processar</a>
                                                                        <a id="btn-cancelar" href="#"
                                                                            class="btn btn-danger btn-rounded btn-cancelar">Finalizar</a>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </form>
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
                                    <h4>Cadastro de Módulos</h4>
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
                                                            data-type="text">Módulos
                                                            A-Z</span></li>
                                                    <li><span data-path=".title" data-order="desc"
                                                            data-type="text">Módulos Z-A</span></li>
                                                </ul>
                                            </div>
                                            <div class="text-filter-box">
                                                <div class="input-group"><span class="input-group-addon"><i
                                                            class="fa fa-search"></i></span><input data-path=".title"
                                                        type="text" value="" placeholder="Filtrar por Módulos"
                                                        data-control-type="textbox" data-control-name="title-filter"
                                                        data-control-action="filter" class="form-control" />
                                                </div>
                                            </div>
                                            <div class="text-filter-box">
                                                <div class="input-group"><span class="input-group-addon"><i
                                                            class="fa fa-search"></i></span><input data-path=".desc"
                                                        type="text" value="" placeholder="Filtrar por Tipo"
                                                        data-control-type="textbox" data-control-name="desc-filter"
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
                                                        <th>Módulos</th>
                                                        <th>Tipo</th>
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
    <script type='text/javascript' src="includes/js/modulos.js"></script>



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