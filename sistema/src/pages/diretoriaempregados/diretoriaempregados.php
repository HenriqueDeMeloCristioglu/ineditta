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

$fileClassSindical = $path . '/includes/php/class.diretoriaempregados.php';

if (file_exists($fileClassSindical)) {

    include_once($fileClassSindical);

    include_once __DIR__ . "/includes/php/class.usuario.php";

    $user = new usuario();
    $userData = $user->validateUser($sessionUser)['response_data']['user'];

    $modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

    $modulos = ["sisap" => $modulosSisap, "comercial" => []];

    $sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

    foreach ($sisap as $key => $value) {
        if (mb_strpos($value, "Diretoria Sindical Empregados")) {
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
} else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.diretoriaempregados).';
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

    <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='styleswitcher'>
    <link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='headerswitcher'>
    <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />
    <link rel="stylesheet" type='text/css' href="diretoriaempregados.css">
    <link rel="stylesheet" type='text/css' href="includes/css/styles.css">

    <style>
        td {
            word-break: break-all
        }

        #page-content {
            min-height: 100% !important;
        }
    </style>
</head>

<body class="horizontal-nav">

    <?php include('menu.php'); ?>

    <div class="page-container" id="pageCtn">

        <div id="page-content" style="min-height: 100%;">
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
                                            <td>
                                                <button type="button" class="btn default-alt " data-toggle="modal"
                                                    data-target="#diretoriaLaboralModal" id="diretoriaLaboralBtn">Nova
                                                    Diretoria Laboral</button>
                                            <?php else: ?>

                                            <?php endif; ?>

                                    </tr>
                                </tbody>
                            </table>

                            <div class="hidden modal_hidden" id="sindicatoDirigenteModalHidden">
                                <div id="sindicatoDirigenteModalHiddenContent">
                                    <div class="modal-content">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                                <h4>Selecione o Sindicato do Dirigente</h4>
                                                <div class="options">
                                                    <a href="javascript:;" class="panel-collapse"><i
                                                            class="fa fa-chevron-down"></i></a>
                                                </div>
                                            </div>
                                            <div class="panel-body collapse in principal-table">
                                                <div class="box text-shadow">
                                                    <table cellpadding="0" cellspacing="0" border="0"
                                                        class="table table-striped table-bordered demo-tbl"
                                                        id="sindicatoLaboralTb" style="width:100%">
                                                        <thead>
                                                            <tr>
                                                                <th>Selecionar</th>
                                                                <th>Sigla</th>
                                                                <th>CNPJ</th>
                                                                <th>Logradouro</th>
                                                                <th>E-mail</th>
                                                                <th>Telefone</th>
                                                                <th>Site</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" data-dismiss="modal"
                                                class="btn btn-secondary">Seguinte</button>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div id="empresaModalHidden" class="hidden modal_hidden" tabindex="-1" role="dialog">
                                <div id="empresaModalHiddenContent">
                                    <div class="modal-content">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                                <h4>Selecione a Empresa do dirigente</h4>
                                                <div class="options">
                                                    <a href="javascript:;" class="panel-collapse"><i
                                                            class="fa fa-chevron-down"></i></a>
                                                </div>
                                            </div>
                                            <div class="panel-body collapse in principal-table">
                                                <div class="box text-shadow">
                                                    <table cellpadding="0" cellspacing="0" border="0"
                                                        class="table table-striped table-bordered demo-tbl"
                                                        id="empresaTb" style="width:100%">
                                                        <thead>
                                                            <tr>
                                                                <th>Selecionar</th>
                                                                <th>Nome</th>
                                                                <th>CNPJ</th>
                                                                <th>Matriz</th>
                                                            </tr>
                                                        </thead>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" data-dismiss="modal"
                                                class="btn btn-secondary">Seguinte</button>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="hidden modal_hidden" id="diretoriaSindicatoLaboralModalHidden">
                                <div id="diretoriaSindicatoLaboralModalHiddenContent">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal"
                                            aria-hidden="true">&times;</button>
                                        <h4 class="modal-title">Cadastro de Diretoria Laboral</h4>
                                    </div>
                                    <div class="modal-body">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                                <h4>Novo Cadastro</h4>
                                                <div class="options">
                                                    <a href="javascript:;" class="panel-collapse"><i
                                                            class="fa fa-chevron-down"></i></a>
                                                </div>
                                            </div>
                                            <div class="panel-body">
                                                <form class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="col-sm-4">
                                                            <label for="dir-input"
                                                                class="control-label">Dirigente</label>
                                                            <input type="text" class="form-control" id="dir-input"
                                                                placeholder="">
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <label for="func-input" class="control-label">Função</label>
                                                            <input type="text" class="form-control" id="func-input"
                                                                placeholder="">
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <label for="sit-input"
                                                                class="control-label">Situação</label>
                                                            <input type="text" class="form-control" id="sit-input"
                                                                placeholder="">
                                                        </div>
                                                    </div>

                                                    <div class="form-group">

                                                        <div class="col-sm-3">
                                                            <label for="dataini-input">Data Início mandato</label>
                                                            <input type="text" class="form-control" id="dataini-input"
                                                                placeholder="DD/MM/YYYY">
                                                        </div>

                                                        <div class="col-sm-3">
                                                            <label for="datafim-input">Data Fim mandato</label>
                                                            <input type="text" class="form-control" id="datafim-input"
                                                                placeholder="DD/MM/YYYY">
                                                        </div>

                                                        <div class="col-sm-3">
                                                            <label>Sindicato</label>
                                                            <select class="form-control" id="sind-input" disabled>
                                                            </select>
                                                            <button type="button" class="btn btn-primary btn-rounded"
                                                                data-toggle="modal"
                                                                data-target="#sindicatoDirigenteModal">Selecionar</button>
                                                        </div>


                                                        <div class="col-sm-3">
                                                            <label>Empresa</label>
                                                            <select class="form-control" id="emp-input" disabled>
                                                            </select>
                                                            <button type="button" class="btn btn-primary btn-rounded"
                                                                data-toggle="modal"
                                                                data-target="#empresaModal">Selecionar</button>
                                                        </div>
                                                    </div>
                                                    <input type="hidden" id="id-input" value="">
                                                </form>
                                            </div>

                                        </div>
                                    </div>

                                    <div class="modal-footer">
                                        <div class="row">
                                            <div class="col-sm-12" style="display: flex; justify-content:center">
                                                <button type="button" class="btn btn-primary btn-rounded"
                                                    id="diretoriaSindicatoLaboralCadastrarBtn">Salvar</button>
                                            </div>
                                        </div>
                                    </div>
                                </div><!-- /.modal-content -->
                            </div>
                        </div>
                    </div>

                    <div class="row" style="display: flex;">
                        <div class="col-md-12">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h4>Cadastro de Diretoria Laboral</h4>
                                    <div class="options">
                                        <a href="javascript:;" class="panel-collapse"><i
                                                class="fa fa-chevron-down"></i></a>
                                    </div>
                                </div>

                                <div class="panel-body collapse in principal-table">
                                    <div class="box text-shadow">
                                        <table cellpadding="0" cellspacing="0" border="0"
                                            class="table table-striped table-bordered demo-tbl"
                                            id="diretoriaempregadostb" data-order='[[ 1, "asc" ]]'>
                                            <thead>
                                                <tr>
                                                    <th>Ações</th>
                                                    <th>Dirigente</th>
                                                    <th>Início mandato</th>
                                                    <th>Término mandato</th>
                                                    <th>Função</th>
                                                    <th>Situação</th>
                                                    <th>Empresa</th>
                                                    <th>Sindicato laboral</th>
                                                </tr>
                                            </thead>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div> <!-- container -->
            </div> <!-- #wrap -->
        </div> <!-- page-content -->


        <?php include 'footer.php' ?>

    </div>
    <script type='text/javascript' src="./js/diretoriaempregados.min.js"></script>
</body>

</html>