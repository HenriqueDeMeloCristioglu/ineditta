<?php
session_start();
if (!$_SESSION) {
    echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

/**
 * @author    {Enter5}
 * @package   {1.0.0}
 * @description	{ }
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

//Localizando usuário logado no Windows
if (!empty($_SERVER['REMOTE_USER'])) {
    $nmloginweb = $_SERVER['REMOTE_USER'];
} elseif (!empty($_SERVER['LOGON_USER'])) {
    $nmloginweb = $_SERVER['LOGON_USER'];
} elseif (!empty($_SERVER['AUTH_USER'])) {
    $nmloginweb = $_SERVER['AUTH_USER'];
}

$fileClassCalendario = $path . '/includes/php/class.clienteunidade.php';

if (file_exists($fileClassCalendario)) {

    include_once($fileClassCalendario);

    $clienteunidade = new clienteunidade();

    if ($clienteunidade->response['response_status']['status'] == 1) {

        $getClienteUnidade = $clienteunidade->getClienteUnidade();

        if ($getClienteUnidade['response_status']['status'] == 1) {

            $lista = $getClienteUnidade['response_data']['listaPrincipal'];
        } else {
            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $error_code . __LINE__;
            $response['response_status']['msg'] = $getClienteUnidade['response_status']['error_code'] . '::' . $getClienteUnidade['response_status']['msg'];
        }

        $getClienteUnidadeCampos = $clienteunidade->getClienteUnidadeCampos();

        if ($getClienteUnidadeCampos['response_status']['status'] == 1) {

            $matriz = $getClienteUnidadeCampos['response_data']['matriz'];
            // $listaMatriz = $getClienteUnidadeCampos['response_data']['listaMatriz'];
            // $listaMatrizUpdate = $getClienteUnidadeCampos['response_data']['listaMatrizUpdate'];
            $negocio = $getClienteUnidadeCampos['response_data']['negocio'];
            // $listaNegocio = $getClienteUnidadeCampos['response_data']['listaNegocio'];
            // $listaNegocioUpdate = $getClienteUnidadeCampos['response_data']['listaNegocioUpdate'];
            $localizacao = $getClienteUnidadeCampos['response_data']['localizacao'];
            // $listaLocalizacao = $getClienteUnidadeCampos['response_data']['listaLocalizacao'];
            // $listaLocalizacaoUpdate = $getClienteUnidadeCampos['response_data']['listaLocalizacaoUpdate'];
            $listaCNAEini = $getClienteUnidadeCampos['response_data']['listaCNAEini'];
        } else {
            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $error_code . __LINE__;
            $response['response_status']['msg'] = $getClienteUnidadeCampos['response_status']['error_code'] . '::' . $getClienteUnidadeCampos['response_status']['msg'];
        }
    } else {
        $response['response_status']['status'] = 0;
        $response['response_status']['error_code'] = $error_code . __LINE__;
        $response['response_status']['msg'] = $clienteusuarios->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
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
    <title>HOME</title>
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
    <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />
    <link rel="stylesheet" href="includes/plugins/datagrid/styles/jplist-custom.css">
    <link href="includes/plugins/select2/select2-4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />

    <script src="keycloak.js"></script>

    <style>
        td {
            word-break: break-all
        }

        /* .container {
            height: 100vh;
        } */

        #page-content {
            min-height: 100% !important;
        }
    </style>
</head>

<body onload="initKeycloak()" class="horizontal-nav"> <!-- onload="initKeycloak()" -->
    <?php include('menu.php'); ?>
    <div id="page-container">

        <div id="page-content" style="height: 100vh;">
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
                                        <td><a id="btn_novo" data-toggle="modal" href="#myModal"
                                                class="btn default-alt  ">NOVO CLIENTE UNIDADE</a></td>
                                    </tr>
                                </tbody>
                            </table>

                            <!-- MODAL UPDATE CADASTRO -->
                            <div class="modal fade" id="updateModal" role="dialog" aria-labelledby="myModalLabel"
                                aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal"
                                                aria-hidden="true">&times;</button>
                                            <h4 class="modal-title">Atualização da Cliente Unidade</h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="panel panel-primary">
                                                <div class="panel-heading">
                                                    <h4>Atualizar Registro</h4>
                                                    <div class="options">
                                                        <a href="javascript:;" class="panel-collapse"><i
                                                                class="fa fa-chevron-down"></i></a>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <form class="form-horizontal">
                                                        <input type="hidden" id="id-inputu" value="1">
                                                        <div class="row">

                                                            <div class="col-sm-4">
                                                                <label class="control-label">Empresa Matriz</label>
                                                                <select class="form-control select2" id="em-inputu"
                                                                    style="-webkit-appearance: none;-moz-appearance: none;text-indent: 1px; text-overflow: '';"
                                                                    disabled>
                                                                    <?= $matriz ?>
                                                                </select>
                                                            </div>


                                                            <div class="col-sm-3">
                                                                <label class="control-label">Tipo de Negócio</label>
                                                                <select class="form-control select2" id="tn-inputu"
                                                                    style="-webkit-appearance: none;-moz-appearance: none;text-indent: 1px; text-overflow: '';"
                                                                    disabled>
                                                                    <?= $negocio ?>
                                                                </select>
                                                            </div>


                                                            <div class="col-sm-3">
                                                                <label class="control-label">Localização</label>
                                                                <select class="form-control select2" id="loc-inputu"
                                                                    style="-webkit-appearance: none;-moz-appearance: none;text-indent: 1px; text-overflow: '';"
                                                                    disabled>
                                                                    <?= $localizacao ?>
                                                                </select>
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="cod-inputu"
                                                                    class="control-label">Código</label>
                                                                <input type="text" class="form-control" id="cod-inputu"
                                                                    placeholder="0000">
                                                            </div>
                                                        </div>

                                                        <div class="row">

                                                            <div class="col-sm-3">
                                                                <label for="nome-inputu"
                                                                    class="control-label">Nome</label>
                                                                <input type="text" class="form-control" id="nome-inputu"
                                                                    placeholder="">
                                                            </div>


                                                            <div class="col-sm-2">
                                                                <label for="cnpj-inputu"
                                                                    class="control-label">CNPJ</label>
                                                                <input type="text" class="form-control" id="cnpj-inputu"
                                                                    placeholder="00.000.000/0000-00">
                                                            </div>

                                                            <div class="col-sm-3">
                                                                <label for="end-inputu"
                                                                    class="control-label">Logradouro</label>
                                                                <input type="text" class="form-control" id="end-inputu"
                                                                    placeholder="">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="bairro-inputu"
                                                                    class="control-label">Bairro</label>
                                                                <input type="text" class="form-control"
                                                                    id="bairro-inputu" placeholder="">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="cep-inputu"
                                                                    class="control-label">CEP</label>
                                                                <input type="text" class="form-control" id="cep-inputu"
                                                                    placeholder="00000-000">
                                                            </div>
                                                        </div>

                                                        <div class="row">

                                                            <div class="col-sm-4">
                                                                <label for="reg-inputu"
                                                                    class="control-label">Regional</label>
                                                                <input type="text" class="form-control" id="reg-inputu"
                                                                    placeholder="">
                                                            </div>


                                                            <div class="col-sm-2">
                                                                <label for="dataclu-inputu"
                                                                    class="control-label">Inclusão</label>
                                                                <input type="text" class="form-control datepicker"
                                                                    id="dataclu-inputu" placeholder="DD/MM/AAAA"
                                                                    disabled>
                                                            </div>


                                                            <div class="col-sm-2">
                                                                <label for="dataina-inputu"
                                                                    class="control-label">Inativação</label>
                                                                <input type="text" class="form-control datepicker"
                                                                    id="dataina-inputu" placeholder="DD/MM/AAAA">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="csc-inputu" class="control-label">Cód.
                                                                    Sind. Cliente</label>
                                                                <input type="text" class="form-control" id="csc-inputu"
                                                                    placeholder="">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="csp-inputu" class="control-label">Cód.
                                                                    Sind. Patronal</label>
                                                                <input type="text" class="form-control" id="csp-inputu"
                                                                    placeholder="">
                                                            </div>
                                                        </div>
                                                    </form>
                                                </div>
                                            </div>

                                            <!-- CNAE SELECIONADO -->
                                            <div class="panel panel-primary">
                                                <div class="panel-heading">
                                                    <h4>CNAE Selecionado</h4>
                                                    <div class="options">
                                                        <a href="javascript:;" class="panel-collapse"><i
                                                                class="fa fa-chevron-down"></i></a>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <table class="table table-striped">
                                                        <thead>
                                                            <th>CNAE</th>
                                                            <th>Descrição Divisão</th>
                                                            <th>Subclasse</th>
                                                            <th>Descrição Subclasse</th>
                                                            <th>Categoria</th>
                                                        </thead>
                                                        <tbody id="cnae_selecionado">

                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>

                                            <!-- SELEÇÃO DE CNAE -->
                                            <div class="panel panel-primary">
                                                <div class="panel-heading">
                                                    <h4>Seleção de CNAE</h4>
                                                    <div class="options">
                                                        <a href="javascript:;" class="panel-collapse"><i
                                                                class="fa fa-chevron-up"></i></a>
                                                    </div>
                                                </div>
                                                <div class="panel-body" id="table_cnae" style="display: none;">
                                                    <table style="width: 100%;" class="table table-striped"
                                                        id="tabela-cnae">

                                                    </table>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="modal-footer">
                                            <div class="row">
                                                <div class="col-lg-12" style="display: flex; justify-content:center">
                                                    <a id="btn-atualizar" href="#"
                                                        class="btn btn-primary btn-rounded">Processar</a>
                                                    <a id="btn-cancelar2" href="#"
                                                        class="btn btn-danger btn-rounded">Finalizar</a>
                                                </div>
                                            </div>
                                        </div>
                                    </div><!-- /.modal-content -->
                                </div><!-- /.modal-dialog -->
                            </div><!-- /.modal -->

                            <!-- NOVO CADASTRO -->
                            <div class="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel"
                                aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal"
                                                aria-hidden="true">&times;</button>
                                            <h4 class="modal-title">Cadastro de Cliente Unidade</h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="panel panel-primary">
                                                <div class="panel-heading">
                                                    <h4>Novo Registro</h4>
                                                    <div class="options">
                                                        <a href="javascript:;" class="panel-collapse"><i
                                                                class="fa fa-chevron-down"></i></a>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <form class="form-horizontal">
                                                        <div class="row">

                                                            <div class="col-lg-4">
                                                                <label class="control-label">Empresa Matriz</label>
                                                                <select class="form-control select2" id="em-input">
                                                                    <?= $matriz ?>
                                                                </select>
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <label class="control-label">Tipo de Negócio</label>
                                                                <select class="form-control select2" id="tn-input">
                                                                    <?= $negocio ?>
                                                                </select>
                                                            </div>


                                                            <div class="col-lg-3">
                                                                <label class="control-label">Localização</label>
                                                                <select class="form-control select2" id="loc-input">
                                                                    <?= $localizacao ?>
                                                                </select>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <label for="cod-input"
                                                                    class="control-label">Código</label>
                                                                <input type="text" class="form-control" id="cod-input">
                                                            </div>
                                                        </div>

                                                        <div class="row">
                                                            <div class="col-lg-3">
                                                                <label for="nome-input"
                                                                    class="control-label">Nome</label>
                                                                <input type="text" class="form-control" id="nome-input"
                                                                    placeholder="">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="cnpj-input"
                                                                    class="control-label">CNPJ</label>
                                                                <input type="text" class="form-control" id="cnpj-input"
                                                                    placeholder="00.000.000/0000-00">
                                                            </div>

                                                            <div class="col-sm-3">
                                                                <label for="end-input"
                                                                    class="control-label">Logradouro</label>
                                                                <input type="text" class="form-control" id="end-input"
                                                                    placeholder="">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="bairro-input"
                                                                    class="control-label">Bairro</label>
                                                                <input type="text" class="form-control"
                                                                    id="bairro-input" placeholder="">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="cep-input" class="control-label">CEP</label>
                                                                <input type="text" class="form-control" id="cep-input"
                                                                    placeholder="00000-000">
                                                            </div>
                                                        </div>

                                                        <div class="row">
                                                            <div class="col-sm-4">
                                                                <label for="reg-input"
                                                                    class="control-label">Regional</label>
                                                                <input type="text" class="form-control" id="reg-input"
                                                                    placeholder="">
                                                            </div>

                                                            <!-- <div class="col-sm-2">
                                                                <label for="dataclu-input" class="control-label">Data Inclusão</label>
                                                                <input type="date" class="form-control datepicker" id="dataclu-input">
                                                            </div> -->

                                                            <div class="col-sm-2">
                                                                <label for="dataina-input" class="control-label">Data
                                                                    Inativação</label>
                                                                <input type="date" class="form-control"
                                                                    id="dataina-input">
                                                            </div>

                                                            <div class="col-sm-3">
                                                                <label for="csc-input" class="control-label">Cód. Sind.
                                                                    Cliente</label>
                                                                <input type="text" class="form-control" id="csc-input"
                                                                    placeholder="">
                                                            </div>

                                                            <div class="col-sm-3">
                                                                <label for="csp-input" class="control-label">Cód. Sind.
                                                                    Patronal</label>
                                                                <input type="text" class="form-control" id="csp-input"
                                                                    placeholder="">
                                                            </div>
                                                        </div>
                                                    </form>
                                                </div>

                                                <!-- PAINEL SELEÇÃO DE CNAE -->
                                                <div class="panel panel-primary">
                                                    <div class="panel-heading">
                                                        <h4>Seleção de CNAE</h4>
                                                        <div class="options">
                                                            <a href="javascript:;" class="panel-collapse"><i
                                                                    class="fa fa-chevron-down"></i></a>
                                                        </div>
                                                    </div>
                                                    <div class="panel-body collapse in principal-table">
                                                        <button id="select_all" type="button" class="btn btn-primary"
                                                            onclick="selecionarTodos()">Selecionar Todos</button>
                                                        <button id="unselect_all" type="button" class="btn btn-primary"
                                                            onclick="limparSelecao()">Limpar Seleção</button>
                                                        <div id="grid-layout-table-2" class="box jplist">
                                                            <div class="jplist-ios-button"><i
                                                                    class="fa fa-sort"></i>jPList Actions</div>
                                                            <div class="jplist-panel box panel-top">
                                                                <button type="button" data-control-type="reset"
                                                                    data-control-name="reset"
                                                                    data-control-action="reset"
                                                                    class="jplist-reset-btn btn btn-primary">Limpar <i
                                                                        class="fa fa-share mls"></i></button>
                                                                <div data-control-type="drop-down"
                                                                    data-control-name="paging"
                                                                    data-control-action="paging"
                                                                    class="jplist-drop-down form-control">
                                                                    <ul class="dropdown-menu">
                                                                        <li><span data-number="3"> 3 por página</span>
                                                                        </li>
                                                                        <li><span data-number="5" data-default="true"> 5
                                                                                por página</span></li>
                                                                        <li><span data-number="10"> 10 por página</span>
                                                                        </li>
                                                                        <li><span data-number="all"> ver todos</span>
                                                                        </li>
                                                                    </ul>
                                                                </div>
                                                                <div data-control-type="drop-down"
                                                                    data-control-name="sort" data-control-action="sort"
                                                                    data-datetime-format="{month}/{day}/{year}"
                                                                    class="jplist-drop-down form-control">
                                                                    <ul class="dropdown-menu">
                                                                        <li><span data-path="default">Listar por</span>
                                                                        </li>
                                                                        <li><span data-path=".title" data-order="asc"
                                                                                data-type="text">ID 0-9</span></li>
                                                                        <li><span data-path=".title" data-order="desc"
                                                                                data-type="text">ID 9-0</span></li>
                                                                        <li><span data-path=".desc" data-order="asc"
                                                                                data-type="text">Nome A-Z</span></li>
                                                                        <li><span data-path=".desc" data-order="desc"
                                                                                data-type="text">Nome Z-A</span></li>
                                                                    </ul>
                                                                </div>
                                                                <div class="text-filter-box">
                                                                    <div class="input-group"><span
                                                                            class="input-group-addon"><i
                                                                                class="fa fa-search"></i></span><input
                                                                            data-path=".title" type="text" value=""
                                                                            placeholder="Desc. Divisão"
                                                                            data-control-type="textbox"
                                                                            data-control-name="title-filter"
                                                                            data-control-action="filter"
                                                                            class="form-control" /></div>
                                                                </div>
                                                                <div class="text-filter-box">
                                                                    <div class="input-group"><span
                                                                            class="input-group-addon"><i
                                                                                class="fa fa-search"></i></span><input
                                                                            data-path=".desc" type="text" value=""
                                                                            placeholder="Desc. Subclasse"
                                                                            data-control-type="textbox"
                                                                            data-control-name="desc-filter"
                                                                            data-control-action="filter"
                                                                            class="form-control" /></div>
                                                                </div>
                                                                <div data-type="Página {current} de {pages}"
                                                                    data-control-type="pagination-info"
                                                                    data-control-name="paging"
                                                                    data-control-action="paging"
                                                                    class="jplist-label btn btn-primary"></div>
                                                                <!-- <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" class="jplist-pagination"></div> -->
                                                            </div>
                                                            <div class="box text-shadow">
                                                                <table
                                                                    class="table table-striped table-bordered demo-tbl">
                                                                    <thead>
                                                                        <tr>
                                                                            <th>Selecionar</th>
                                                                            <th>CNAE</th>
                                                                            <th>Divisão</th>
                                                                            <th>Descrição</th>
                                                                            <th>Subclasse</th>
                                                                            <th>Descrição</th>
                                                                            <th>Categoria</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody id="sel-body-add">
                                                                        <?= $listaCNAEini ?>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                            <div class="box jplist-no-results text-shadow align-center">
                                                                <p>Nenhum resultado encontrado</p>
                                                            </div>
                                                            <div class="jplist-ios-button"><i
                                                                    class="fa fa-sort"></i>jPList Actions</div>
                                                            <div class="jplist-panel box panel-bottom">
                                                                <div data-type="{start} - {end} de {all}"
                                                                    data-control-type="pagination-info"
                                                                    data-control-name="paging"
                                                                    data-control-action="paging"
                                                                    class="jplist-label btn btn-primary"></div>
                                                                <div data-control-type="pagination"
                                                                    data-control-name="paging"
                                                                    data-control-action="paging"
                                                                    data-control-animate-to-top="true"
                                                                    class="jplist-pagination"></div>
                                                            </div>
                                                        </div>


                                                    </div>
                                                </div>
                                                <input type="hidden" id="cnaes-input" value="">
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <div class="row">
                                                <div class="col-lg-12" style="display: flex; justify-content:center">
                                                    <a href="#" class="btn btn-primary btn-rounded"
                                                        onclick="addClienteUnidade();">Processar</a>
                                                    <a id="btn-cancelar" href="#"
                                                        class="btn btn-danger btn-rounded">Finalizar</a>
                                                </div>
                                            </div>
                                        </div>
                                    </div><!-- /.modal-content -->
                                </div><!-- /.modal-dialog -->
                            </div><!-- /.modal -->

                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h4>Cadastro de Cliente Unidade</h4>
                                    <div class="options">
                                        <a href="javascript:;" class="panel-collapse"><i
                                                class="fa fa-chevron-down"></i></a>
                                    </div>
                                </div>
                                <div class="panel-body collapse in principal-table">
                                    <div id="grid-layout-table-1" class="box jplist">
                                        <div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
                                        <div class="jplist-panel box panel-top">
                                            <button type="button" data-control-type="reset" data-control-name="reset"
                                                data-control-action="reset"
                                                class="jplist-reset-btn btn btn-primary">Limpar <i
                                                    class="fa fa-share mls"></i></button>
                                            <div data-control-type="drop-down" data-control-name="paging"
                                                data-control-action="paging" class="jplist-drop-down form-control">
                                                <ul class="dropdown-menu">
                                                    <li><span data-number="3"> 3 por página</span></li>
                                                    <li><span data-number="5"> 5 por página</span></li>
                                                    <li><span data-number="10" data-default="true"> 10 por página</span>
                                                    </li>
                                                    <li><span data-number="all"> ver todos</span></li>
                                                </ul>
                                            </div>
                                            <div data-control-type="drop-down" data-control-name="sort"
                                                data-control-action="sort" data-datetime-format="{month}/{day}/{year}"
                                                class="jplist-drop-down form-control">
                                                <ul class="dropdown-menu">
                                                    <li><span data-path="default">Listar por</span></li>
                                                    <li><span data-path=".title" data-order="asc" data-type="text">Nome
                                                            A-Z</span></li>
                                                    <li><span data-path=".title" data-order="desc" data-type="text">Nome
                                                            Z-A</span></li>
                                                    <li><span data-path=".desc" data-order="asc"
                                                            data-type="text">Empresa
                                                            A-Z</span></li>
                                                    <li><span data-path=".desc" data-order="desc"
                                                            data-type="text">Empresa
                                                            Z-A</span></li>
                                                </ul>
                                            </div>
                                            <div class="text-filter-box">
                                                <div class="input-group"><span class="input-group-addon"><i
                                                            class="fa fa-search"></i></span><input data-path=".title"
                                                        type="text" value="" placeholder="Filtrar por Nome"
                                                        data-control-type="textbox" data-control-name="title-filter"
                                                        data-control-action="filter" class="form-control" /></div>
                                            </div>
                                            <div class="text-filter-box">
                                                <div class="input-group"><span class="input-group-addon"><i
                                                            class="fa fa-search"></i></span><input data-path=".desc"
                                                        type="text" value="" placeholder="Filtrar por Empresa"
                                                        data-control-type="textbox" data-control-name="desc-filter"
                                                        data-control-action="filter" class="form-control" /></div>
                                            </div>
                                            <div data-type="Página {current} de {pages}"
                                                data-control-type="pagination-info" data-control-name="paging"
                                                data-control-action="paging" class="jplist-label btn btn-primary"></div>
                                            <!-- <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" class="jplist-pagination"></div> -->
                                        </div>
                                        <div class="box text-shadow">
                                            <table class="table table-striped table-bordered demo-tbl">
                                                <thead>
                                                    <tr>
                                                        <th></th>
                                                        <th>Nome Unidade</th>
                                                        <th>CNPJ</th>
                                                        <th>Empresa Matriz</th>
                                                        <th>Data de Inclusão</th>
                                                        <th>Tipo Unidade</th>
                                                        <th>UF</th>
                                                        <th>Municipio</th>
                                                        <th>Data de Inativação</th>
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
                                        <div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
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

                    </div> <!-- container -->
                </div> <!-- #wrap -->
            </div> <!-- page-content -->


            <?php include 'footer.php' ?>

        </div> <!-- page-container -->


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
        <script type='text/javascript' src='includes/plugins/jquery-editable/jquery.editable.min.js'></script>
        <script type='text/javascript' src='includes/js/placeholdr.js'></script>
        <script type='text/javascript' src='includes/demo/demo-modals.js'></script>
        <script type='text/javascript' src='includes/js/application.js'></script>
        <script type='text/javascript' src='includes/demo/demo.js'></script>

        <script src="includes/plugins/edited-datatable/jquery.dataTables.min.js"></script>
        <script src="includes/plugins/edited-datatable/datatables-bs4/js/dataTables.bootstrap4.min.js"></script>
        <script src="includes/plugins/edited-datatable/datatables-responsive/js/dataTables.responsive.min.js"></script>
        <script src="includes/plugins/edited-datatable/datatables-responsive/js/responsive.bootstrap4.min.js"></script>

        <script type='text/javascript'
            src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.15/jquery.mask.min.js"></script>
        <script
            src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.4.1/js/bootstrap-datepicker.js"></script>
        <script type="text/javascript"
            src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.9.0/moment-with-locales.js"></script>

        <script src="includes/plugins/select2/select2-4.1.0-rc.0/dist/js/select2.min.js"></script>

        <script type='text/javascript' src="includes/js/home.js"></script>

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


        <!-- <script>!window.jQuery && document.write(unescape('%3Cscript src="includes/js/jquery-1.10.2.min.js"%3E%3C/script%3E'))</script>
<script type="text/javascript">!window.jQuery.ui && document.write(unescape('%3Cscript src="includes/js/jqueryui-1.10.3.min.js'))</script>
<script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
<script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script> -->


</body>

</html>