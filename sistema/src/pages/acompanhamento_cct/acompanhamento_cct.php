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

header('Content-type: text/html; charset=UTF-8');
header('Cache-Control: no-cache');

$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.'));

// Montando o c??o do erro que ser?presentado
$localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar = array("\\", "/includes/php");
$substituir = array("/", "");
$path = str_replace($localizar, $substituir, __DIR__);


$fileAcompanhamento = $path . '/includes/php/class.acompanhamento_cliente.php';

if (file_exists($fileAcompanhamento)) {

    include_once($fileAcompanhamento);

    $acompanhamento_cliente = new acompanhamento_cliente();

    if ($acompanhamento_cliente->response['response_status']['status'] == 1) {

        $getAcompanhamento = $acompanhamento_cliente->getAcompanhamento();



        if ($getAcompanhamento['response_status']['status'] == 1) {

            $listaPrincipal = $getAcompanhamento['response_data']['listaPrincipal'];
        } else {
            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $error_code . __LINE__;
            $response['response_status']['msg'] = $getAcompanhamento['response_status']['error_code'] . '::' . $getAcompanhamento['response_status']['msg'];
        }
    } else {
        $response['response_status']['status'] = 0;
        $response['response_status']['error_code'] = $error_code . __LINE__;
        $response['response_status']['msg'] = $acompanhamento_cliente->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
    }
} else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.acompanhamento_cliente).';
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

    <link rel="stylesheet" href="includes/css/styles.css">
    <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css">
    <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='styleswitcher'>
    <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='headerswitcher'>

    <!-- CSS datagrid -->
    <link rel="stylesheet" href="includes/plugins/datagrid/styles/jplist-custom.css">


    <!-- The following CSS are included as plugins and can be removed if unused-->
    <script src="includes/js/jquery-3.4.1.min.js"></script>
    <link rel='stylesheet' type='text/css' href='includes/plugins/form-toggle/toggles.css' />
    <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

    <script src="keycloak.js"></script>

    <style type="text/css">
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

        .div-form {
            margin-top: 20px;
        }

        #timeline-place {
            max-height: 50vh;
            overflow-y: scroll;
        }

        .timeline-box {
            margin-top: 50px;
        }

        .update-coment:hover div {
            transition: all 0.3s ease-in-out;
            background-color: #4f8edc;
        }

        .update-coment:hover i {
            color: #fff;
            transition: all 0.3s ease-in-out;
        }

        .sub-data {
            font-size: 0.875em;
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
                <div class="container" style="padding-bottom: 0px;">

                    <div class="row" style="display: flex;">
                        <div class="col-md-1 img_container"> <!-- style="transform: translate(-13px, 13px);" -->
                            <div class="container_logo_client">
                                <img id="imglogo" class="img-circle">
                            </div>
                        </div>

                        <div class="col-md-11 content_container">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="panel panel-primary" style="margin-top: 0px; margin-bottom: 0;">
                                        <div class="panel-heading">
                                            <h4>Acompanhamento CCT</h4>
                                            <div class="options">
                                                <a href="javascript:;" class="panel-collapse"><i
                                                        class="fa fa-chevron-down"></i></a>
                                            </div>
                                        </div>
                                        <div class="panel-body collapse in principal-table">
                                            <div id="grid-layout-table-1" class="box jplist">
                                                <div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions
                                                </div>
                                                <div class="jplist-panel box panel-top">
                                                    <button type="button" data-control-type="reset"
                                                        data-control-name="reset" data-control-action="reset"
                                                        class="jplist-reset-btn btn btn-primary">Limpar <i
                                                            class="fa fa-share mls"></i></button>
                                                    <div data-control-type="drop-down" data-control-name="paging"
                                                        data-control-action="paging"
                                                        class="jplist-drop-down form-control">
                                                        <ul class="dropdown-menu">
                                                            <li><span data-number="3"> 3 por página</span></li>
                                                            <li><span data-number="5" data-default="true"> 5 por
                                                                    página</span></li>
                                                            <li><span data-number="10"> 10 por página</span></li>
                                                            <li><span data-number="all"> ver todos</span></li>
                                                        </ul>
                                                    </div>
                                                    <div data-control-type="drop-down" data-control-name="sort"
                                                        data-control-action="sort"
                                                        data-datetime-format="{month}/{day}/{year}"
                                                        class="jplist-drop-down form-control">
                                                        <ul class="dropdown-menu">
                                                            <li><span data-path="default">Listar por</span></li>
                                                            <li><span data-path=".title" data-order="asc"
                                                                    data-type="text">S.P A-Z</span></li>
                                                            <li><span data-path=".title" data-order="desc"
                                                                    data-type="text">S.P Z-A</span></li>
                                                            <li><span data-path=".desc" data-order="asc"
                                                                    data-type="text">S.E A-Z</span></li>
                                                            <li><span data-path=".desc" data-order="desc"
                                                                    data-type="text">S.E Z-A</span></li>
                                                        </ul>
                                                    </div>
                                                    <div class="text-filter-box">
                                                        <div class="input-group"><span class="input-group-addon"><i
                                                                    class="fa fa-search"></i></span><input
                                                                data-path=".title" type="text" value=""
                                                                placeholder="Filtrar por SP" data-control-type="textbox"
                                                                data-control-name="title-filter"
                                                                data-control-action="filter" class="form-control" />
                                                        </div>
                                                    </div>
                                                    <div class="text-filter-box">
                                                        <div class="input-group"><span class="input-group-addon"><i
                                                                    class="fa fa-search"></i></span><input
                                                                data-path=".desc" type="text" value=""
                                                                placeholder="Filtrar por SE" data-control-type="textbox"
                                                                data-control-name="desc-filter"
                                                                data-control-action="filter" class="form-control" />
                                                        </div>
                                                    </div>
                                                    <div data-type="Página {current} de {pages}"
                                                        data-control-type="pagination-info" data-control-name="paging"
                                                        data-control-action="paging"
                                                        class="jplist-label btn btn-primary"></div>
                                                </div>
                                                <div class="box text-shadow">
                                                    <table cellpadding="0" cellspacing="0" border="0"
                                                        class=" table table-bordered demo-tbl">
                                                        <thead>
                                                            <tr>
                                                                <th></th>
                                                                <th>Sind. Patronal</th>
                                                                <th>Sind. Empregados</th>
                                                                <th>Data Base</th>
                                                                <th>Fase</th>
                                                                <th>Status</th>
                                                                <th>Usuário</th>

                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <?php print $listaPrincipal; ?>
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
                                                        data-control-action="paging"
                                                        class="jplist-label btn btn-primary"></div>
                                                    <div data-control-type="pagination" data-control-name="paging"
                                                        data-control-action="paging" data-control-animate-to-top="true"
                                                        class="jplist-pagination"></div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- MODAL COMENTARIO CCT -->
                                        <div id="myModalUpdate" class="modal" tabindex="-1" role="dialog">
                                            <div class="modal-dialog" role="document">
                                                <div class="modal-content">
                                                    <div class="modal-header">
                                                        <button type="button" class="close" data-dismiss="modal"
                                                            aria-hidden="true">&times;</button>
                                                        <h4 class="modal-title">Comentários</h4>
                                                    </div>
                                                    <div class="modal-body">
                                                        <div class="panel panel-primary">
                                                            <form class="form-horizontal">
                                                                <div class="panel-heading">
                                                                    <h4>Adicionar Comentário</h4>
                                                                    <div class="options">
                                                                        <a href="javascript:;" class="panel-collapse"><i
                                                                                class="fa fa-chevron-down"></i></a>
                                                                    </div>
                                                                </div>
                                                                <div class="panel-body collapse in">
                                                                    <div class="form-group center">
                                                                        <input type="hidden" name="" id="id-inputu">

                                                                        <div class="col-sm-6">
                                                                            <label for="sinde-inputu"
                                                                                class="control-label">Sind.
                                                                                Empregados</label>
                                                                            <input type="text" class="form-control"
                                                                                name="" id="sinde-inputu">
                                                                        </div>

                                                                        <div class="col-sm-6">
                                                                            <label for="patr-inputu"
                                                                                class="control-label">Sind.
                                                                                Patronal</label>
                                                                            <input type="text" class="form-control"
                                                                                name="" id="patr-inputu">
                                                                        </div>
                                                                    </div>

                                                                    <div class="form-group center">
                                                                        <div class="col-sm-4">
                                                                            <label for="data-input"
                                                                                class="control-label">Data Base</label>
                                                                            <input type="text" class="form-control"
                                                                                name="" id="data-inputu">
                                                                        </div>

                                                                        <div class="col-sm-4">
                                                                            <label for="categoria-inputu"
                                                                                class="control-label">Categoria</label>
                                                                            <input type="text" class="form-control"
                                                                                name="" id="categoria-inputu">
                                                                        </div>

                                                                        <div class="col-md-4">
                                                                            <label for="fase-inputu"
                                                                                class="control-label">Fase</label>
                                                                            <select class="form-control" name=""
                                                                                id="fase-inputu"></select>
                                                                        </div>
                                                                    </div>

                                                                    <div class="form-group center">
                                                                        <div class="col-md-12">
                                                                            <label for="comentario-inputu"
                                                                                class="control-label">Comentário</label>
                                                                            <textarea class="form-control"
                                                                                id="comentario-inputu" cols="30"
                                                                                rows="10"></textarea>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </form>
                                                        </div>
                                                    </div>
                                                    <div class="modal-footer">
                                                        <div class="row">
                                                            <div class="col-lg-12"
                                                                style="display: flex; justify-content:center;">
                                                                <button style="margin-top: 10px ;"
                                                                    class="btn btn-primary btn-rounded"
                                                                    onclick="addAcompanhamento()">Processar</button>
                                                                <button type="button" id="btn-cancelar"
                                                                    style="margin-top: 10px ;"
                                                                    class="btn btn-danger btn-rounded">Finalizar</button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- MODAL UPDATE TIMELINE -->
                                        <div id="myModalUpdateTimeline" class="modal" tabindex="-1" role="dialog">
                                            <div class="modal-dialog" role="document">
                                                <div class="modal-content">
                                                    <div class="panel panel-primary">
                                                        <form class="form-horizontal">
                                                            <div class="panel-heading">
                                                                <h4>Editar Comentário</h4>
                                                                <div class="options">
                                                                    <a href="javascript:;" class="panel-collapse"><i
                                                                            class="fa fa-chevron-down"></i></a>
                                                                </div>
                                                            </div>
                                                            <div class="panel-body collapse in">
                                                                <input type="hidden" name="" id="id-update">

                                                                <div class="form-group center">
                                                                    <div class="col-sm-12">
                                                                        <label for="sinde-inputu"
                                                                            class="control-label">Fase</label>
                                                                        <select class="form-control" name=""
                                                                            id="fase-update"></select>
                                                                    </div>
                                                                </div>

                                                                <div class="form-group center">
                                                                    <div class="col-sm-12">
                                                                        <label for="patr-inputu"
                                                                            class="control-label">Comentário</label>
                                                                        <textarea class="form-control"
                                                                            id="comentario-update" cols="30"
                                                                            rows="10"></textarea>
                                                                    </div>
                                                                </div>

                                                                <a type="button" data-toggle="modal"
                                                                    data-dismiss="modal" href="#myModalUpdate"
                                                                    style="margin-top: 10px ;"
                                                                    class="btn btn-primary btn-rounded">Finalizar</a>
                                                                <button style="margin-top: 10px ;"
                                                                    class="btn btn-primary btn-rounded"
                                                                    onclick="updateComentario()">Processar</button>
                                                            </div>
                                                        </form>

                                                    </div>
                                                    <!-- <div class="modal-footer">
                                                            <button data-toggle="modal" href="#myModalUpdate" type="button" class="btn btn-secondary" data-dismiss="modal">Seguinte</button>
                                                        </div> -->
                                                </div>
                                            </div>
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
    <!-- SCRIPTS -->
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

    <script type='text/javascript' src="includes/js/acompanhamento_cct.js"></script>

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