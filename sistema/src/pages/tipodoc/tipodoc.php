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

$fileTipoD = $path . '/includes/php/class.tipodoc.php';

if (file_exists($fileTipoD)) {

    include_once($fileTipoD);


    include_once __DIR__ . "/includes/php/class.usuario.php";

    $user = new usuario();
    $userData = $user->validateUser($sessionUser)['response_data']['user'];

    $modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

    $modulos = ["sisap" => $modulosSisap, "comercial" => []];

    $sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

    foreach ($sisap as $key => $value) {
        if (mb_strpos($value, "Adm - Tipo Documento")) {
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



    $tipodoc = new tipodoc();

    if ($tipodoc->response['response_status']['status'] == 1) {

        $getTipoDoc = $tipodoc->getTipoDoc();

        if ($getTipoDoc['response_status']['status'] == 1) {

            $lista = $getTipoDoc['response_data']['html'];
        } else {
            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $error_code . __LINE__;
            $response['response_status']['msg'] = $getTipoDoc['response_status']['error_code'] . '::' . $getTipoDoc['response_status']['msg'];
        }
    } else {
        $response['response_status']['status'] = 0;
        $response['response_status']['error_code'] = $error_code . __LINE__;
        $response['response_status']['msg'] = $tipodoc->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
    }
} else {
    $response['response_status']['status'] = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.tipodoc).';
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

    <link rel="stylesheet" href="tipodoc.css">
    <link rel="stylesheet" href="includes/css/styles.css">

    <style>
        #page-content {
            min-height: 100% !important;
        }

        .d-flex-checkbox {
            display: flex;
            flex-direction: column;
            align-items: end;
        }

        .d-flex-checkbox .d-flex {
            display: flex;
            gap: 15px;
        }
    </style>
</head>

<body class="horizontal-nav">
    <?php include('menu.php'); ?>

    <div id="pageCtn">
        <div id="page-content">
            <div id="wrap">
                <div class="container">
                    <div class="row" style="display: flex;">
                        <div class="col-md-1 img_container">
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
                                                <button data-toggle="modal" data-target="#upsertDocumentoModal"
                                                    class="btn default-alt" id="upsertDocumentoModalBtn">Cadastrar
                                                    Tipo de Documentos</button>
                                            </td>
                                        <?php else: ?>

                                        <?php endif; ?>
                                    </tr>
                                </tbody>
                            </table>

                            <div class="hidden" id="upsertDocumentoModalHidden">
                                <div id="upsertDocumentoModalContent">
                                    <div class="modal-header" style="padding-bottom: 30px;">
                                        <button type="button" class="close" data-dismiss="modal"
                                            aria-hidden="true">&times;</button>
                                        <h4 class="modal-title"></h4>
                                    </div>
                                    <div class="modal-body">
                                        <div class="panel panel-primary">
                                            <form class="form-horizontal">
                                                <div class="panel panel-primary" style="margin: 0;">
                                                    <div class="panel-heading">
                                                        <h4>Cadastro de Tipo de Documentos</h4>
                                                        <div class="options">
                                                            <a href="javascript:;" class="panel-collapse"><i
                                                                    class="fa fa-chevron-down"></i></a>
                                                        </div>
                                                    </div>
                                                    <div class="panel-body collapse in principal-table">
                                                        <div class="form-group center">
                                                            <div class="col-sm-2">
                                                                <label for="sigla">Sigla Documento</label>
                                                                <input type="text" id="sigla" class="form-control">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="tipo-selecao">Tipo de Documento</label>
                                                                <select id="tipo-selecao"
                                                                    class="form-control select2"></select>
                                                            </div>

                                                            <div class="col-sm-2" id="tipo-existente">
                                                                <label for="tipo-documento-existente">Tipo de Documento
                                                                    exitente</label>
                                                                <select id="tipo-documento-existente"
                                                                    class="form-control select2"></select>
                                                            </div>

                                                            <div class="col-sm-2" id="novo-documento">
                                                                <label for="novo-tipo-documento">Novo tipo de
                                                                    Documento</label>
                                                                <input type="text" id="novo-tipo-documento"
                                                                    class="form-control">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="nome">Nome do Documento</label>
                                                                <input type="text" id="nome" class="form-control">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="modulo">Módulo de
                                                                    cadastro</label>
                                                                <select id="modulo"
                                                                    class="form-control select2"></select>
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="processado">Processado</label>
                                                                <select id="processado"
                                                                    class="form-control select2"></select>
                                                            </div>
                                                        </div>

                                                        <div class="form-group center hide" style="padding-top: 20px;">
                                                            <div class="col-md-3 d-flex-checkbox">
                                                                <div class="d-flex">
                                                                    <label for="validade_inicial">Validade
                                                                        Inicial</label> <input type="checkbox"
                                                                        id="validade_inicial">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="sindicato_laboral">Sindicato
                                                                        Laboral</label> <input type="checkbox"
                                                                        id="sindicato_laboral">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="tipo_unidade">Tipo Unidade
                                                                        Cliente</label> <input type="checkbox"
                                                                        id="tipo_unidade">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="assunto">Referenciamento /
                                                                        Assunto</label> <input type="checkbox"
                                                                        id="assunto">
                                                                </div>
                                                            </div>

                                                            <div class="col-md-3 d-flex-checkbox">
                                                                <div class="d-flex">
                                                                    <label for="val_final_check">Validade
                                                                        Final</label> <input type="checkbox"
                                                                        id="validade_final">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="sindicato_patronal">Sindical
                                                                        Patronal</label> <input type="checkbox"
                                                                        id="sindicato_patronal">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="abrangencia">Abrangência</label>
                                                                    <input type="checkbox" id="abrangencia">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="descricao">Descrição do
                                                                        Documento</label> <input type="checkbox"
                                                                        id="descricao">
                                                                </div>
                                                            </div>

                                                            <div class="col-md-3 d-flex-checkbox">
                                                                <div class="d-flex">
                                                                    <label for="origem">Origem</label> <input
                                                                        type="checkbox" id="origem">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="data_base">Data-base</label>
                                                                    <input type="checkbox" id="data_base">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="atividade_economica">Atividade
                                                                        Econômica</label> <input type="checkbox"
                                                                        id="atividade_economica">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="numero_legislacao">Número da
                                                                        Legislação</label> <input type="checkbox"
                                                                        id="numero_legislacao">
                                                                </div>
                                                            </div>

                                                            <div class="col-md-3 d-flex-checkbox">
                                                                <div class="d-flex">
                                                                    <label for="versao">Versão</label> <input
                                                                        type="checkbox" id="versao">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="permitir_compartilhar">Permitir
                                                                        Compartilhamento</label> <input type="checkbox"
                                                                        id="permitir_compartilhar">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="estabelecimento">Estabelecimento</label>
                                                                    <input type="checkbox" id="estabelecimento">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="fonte">Fonte da
                                                                        Legislação</label> <input type="checkbox"
                                                                        id="fonte">
                                                                </div>

                                                                <div class="d-flex">
                                                                    <label for="anuencia">Anuência</label>
                                                                    <input type="checkbox" id="anuencia">
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </form>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="btn-toolbar" style="display: flex; justify-content:left;">
                                                    <button type="button" class="btn btn-primary btn-rounded"
                                                        id="upsertDocumentoModalBtn">Salvar</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row" style="display: flex;">
                        <div class="col-md-12">
                            <!-- TELA INICIAL -->
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h4>Cadastro de Tipos de Documentos</h4>
                                    <div class="options">
                                        <a href="javascript:;" class="panel-collapse"><i
                                                class="fa fa-chevron-down"></i></a>
                                    </div>
                                </div>
                                <div class="panel-body collapse in">
                                    <div id="grid-layout-table-1" class="box jplist">
                                        <div class="box text-shadow">
                                            <table cellpadding="0" cellspacing="0" border="0"
                                                class="table table-striped table-bordered demo-tbl" id="tipodoctb"
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

    </div>

    <script type='text/javascript' src="./js/tipodoc.min.js"></script>
</body>

</html>