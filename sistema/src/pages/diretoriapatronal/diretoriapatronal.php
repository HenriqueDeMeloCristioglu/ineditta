<?php
session_start();
if (!$_SESSION) {
    echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}
$sessionUser = $_SESSION['login'];

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
    <link rel="stylesheet" type='text/css' href="diretoriapatronal.css">
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
                                        <td>
                                            <button type="button" class="btn default-alt " data-toggle="modal" data-target="#diretoriaPatronalModal" id="diretoriaPatronalBtn">Nova Diretoria Patronal</button>
                                        </td>
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
                                                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                                                </div>
                                            </div>
                                            <div class="panel-body collapse in principal-table">
                                                <div class="box text-shadow">
                                                    <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl" id="sindicatoPatronalTb" style="width:100%">
                                                        <thead>
                                                            <tr>
                                                                <th>Selecionar</th>
                                                                <th>Sigla</th>
                                                                <th>CNPJ</th>
                                                                <th>Municipio</th>
                                                                <th>E-mail</th>
                                                                <th>Telefone</th>
                                                                <th>Uf</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" data-dismiss="modal" class="btn btn-secondary">Seguinte</button>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="hidden modal_hidden" id="empresaModalHidden">
                                <div id="empresaModalHiddenContent">
                                    <div class="modal-content">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                                <h4>Selecione a Empresa do dirigente</h4>
                                                <div class="options">
                                                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                                                </div>
                                            </div>
                                            <div class="panel-body collapse in principal-table">
                                                <div class="box text-shadow">
                                                    <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl" id="empresaTb" style="width:100%">
                                                        <thead>
                                                            <tr>
                                                                <th>Selecionar</th>
                                                                <th>Grupo</th>
                                                                <th>Matriz</th>
                                                                <th>Filial</th>
                                                                <th>CNPJ</th>
                                                            </tr>
                                                        </thead>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" data-dismiss="modal" class="btn btn-secondary">Seguinte</button>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="hidden modal_hidden" id="diretoriaSindicatoPatronalModalHidden">
                                <div id="diretoriaSindicatoPatronalModalHiddenContent">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                        <h4 class="modal-title">Cadastro de Diretoria Laboral</h4>
                                    </div>
                                    <div class="modal-body">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                                <h4>Novo Cadastro</h4>
                                                <div class="options">
                                                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                                                </div>
                                            </div>
                                            <div class="panel-body">
                                                <form class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="col-sm-4">
                                                            <label for="dir-input" class="control-label">Dirigente</label>
                                                            <input type="text" class="form-control" id="dir-input" placeholder="">
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <label for="func-input" class="control-label">Função</label>
                                                            <input type="text" class="form-control" id="func-input" placeholder="">
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <label for="sit-input" class="control-label">Situação</label>
                                                            <input type="text" class="form-control" id="sit-input" placeholder="">
                                                        </div>
                                                    </div>

                                                    <div class="form-group">

                                                        <div class="col-sm-3">
                                                            <label for="dataini-input">Data Início mandato</label>
                                                            <input type="text" class="form-control" id="dataini-input" placeholder="DD/MM/YYYY">
                                                        </div>

                                                        <div class="col-sm-3">
                                                            <label for="datafim-input">Data Fim mandato</label>
                                                            <input type="text" class="form-control" id="datafim-input" placeholder="DD/MM/YYYY">
                                                        </div>

                                                        <div class="col-sm-3">
                                                            <label>Sindicato</label>
                                                            <select class="form-control" id="sind-input" disabled>
                                                            </select>
                                                            <button type="button" class="btn btn-primary btn-rounded" data-toggle="modal" data-target="#sindicatoDirigenteModal">Selecionar</button>
                                                        </div>


                                                        <div class="col-sm-3">
                                                            <label>Empresa</label>
                                                            <select class="form-control" id="emp-input" disabled>
                                                            </select>
                                                            <button type="button" class="btn btn-primary btn-rounded" data-toggle="modal" data-target="#empresaModal">Selecionar</button>
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
                                                <button type="button" class="btn btn-primary btn-rounded" id="diretoriaSindicatoPatronalCadastrarBtn">Salvar</button>
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
                                    <h4>Cadastro de Diretoria Patronal</h4>
                                    <div class="options">
                                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                                    </div>
                                </div>

                                <div class="panel-body collapse in principal-table">
                                    <div class="box text-shadow">
                                        <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl" id="diretoriapatronaltb" data-order='[[ 1, "asc" ]]'>
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
    <script type='text/javascript' src="./js/diretoriapatronal.min.js"></script>
</body>

</html>