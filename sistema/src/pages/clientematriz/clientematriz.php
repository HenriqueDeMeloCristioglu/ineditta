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

    <link rel="stylesheet" href="clientematriz.css">
    <link rel="stylesheet" href="includes/css/styles.css">

    <style>
        td {
            word-break: break-all
        }

        .hide {
            display: none;
        }

        #page-content {
            min-height: 100% !important;
        }

        .text-start {
            text-align: start !important;
        }
    </style>
</head>

<body class="horizontal-nav hide">
    <?php require 'menu.php'; ?>

    <div id="pageCtn">

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
                                        <td><button id="btn_novo" data-target="#novoClientMatrizModal" data-toggle="modal" class="btn default-alt">NOVO CLIENTE MATRIZ</button></td>
                                    </tr>
                                </tbody>
                            </table>

                            <!-- HISTÓRICO -->
                            <div id="myModalHistorico" class="modal" tabindex="-1" role="dialog">
                                <div class="modal-dialog" role="document">
                                    <div class="modal-content">
                                        <div id="tabela-historico"></div>

                                        <div class="modal-footer">
                                            <button data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- MODAL UPDATE -->
                            <div class="modal fade" id="updateModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                            <h4 class="modal-title">Atualização de Cliente Matriz</h4>
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
                                                        <input type="hidden" id="modulos_update">
                                                        <input type="hidden" id="id-inputu" value="1">
                                                        <div class="row" style="display: flex; align-items:flex-end">
                                                            <div class="col-lg-3">
                                                                <!-- <label for="logo-inputu" class="control-label">Logo</label> -->

                                                                <p>Logo atual: </p><img id="logo-update" height="100" alt="...">
                                                                <input type="file" class="form-control-file" id="logo-inputu">
                                                            </div>
                                                            <div class="col-lg-3">
                                                                <label class="control-label">Grupo Econômico</label>
                                                                <select class="form-control select2" id="ge-inputu">
                                                                    <?php print $listaGupdate ?>
                                                                </select>
                                                            </div>

                                                            <div class="col-lg-5">
                                                                <label for="rs-inputu" class="control-label">Razão Social</label>
                                                                <input type="text" class="form-control" id="rs-inputu" placeholder="">
                                                            </div>

                                                            <div class="col-lg-1">
                                                                <label for="cod-inputu" class="control-label">Código</label>
                                                                <input type="text" class="form-control" id="cod-inputu" placeholder="">
                                                            </div>
                                                        </div>

                                                        <div class="row">

                                                            <div class="col-lg-8">
                                                                <label for="nome-inputu" class="control-label">Nome</label>
                                                                <input type="text" class="form-control" id="nome-inputu" placeholder="">
                                                            </div>

                                                            <div class="col-lg-4">
                                                                <label for="cnpj-inputu" class="control-label">CNPJ</label>
                                                                <input type="text" class="form-control" id="cnpj-inputu" placeholder="00.000.000/0000-00">
                                                            </div>
                                                        </div>
                                                        <div class="row">

                                                            <div class="col-lg-3">
                                                                <label for="end-inputu" class="control-label">Logradouro</label>
                                                                <input type="text" class="form-control" id="end-inputu" placeholder="">
                                                            </div>
                                                            <div class="col-lg-3">
                                                                <label for="bairro-inputu" class="control-label">Bairro</label>
                                                                <input type="text" class="form-control" id="bairro-inputu" placeholder="">
                                                            </div>
                                                            <div class="col-lg-2">
                                                                <label for="cep-inputu" class="control-label">CEP</label>
                                                                <input type="text" class="form-control" id="cep-inputu" placeholder="00000-000">
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <label for="cid-inputu" class="control-label">Cidade</label>
                                                                <input type="text" class="form-control" id="cid-inputu" placeholder="">
                                                            </div>


                                                            <div class="col-lg-1">
                                                                <label for="uf-inputu" class="control-label">UF</label>
                                                                <select class="form-control" id="uf-inputu">
                                                                    <option value="AC">AC</option>
                                                                    <option value="AL">AL</option>
                                                                    <option value="AM">AM</option>
                                                                    <option value="AP">AP</option>
                                                                    <option value="BA">BA</option>
                                                                    <option value="CE">CE</option>
                                                                    <option value="DF">DF</option>
                                                                    <option value="ES">ES</option>
                                                                    <option value="GO">GO</option>
                                                                    <option value="MA">MA</option>
                                                                    <option value="MG">MG</option>
                                                                    <option value="MS">MS</option>
                                                                    <option value="MT">MT</option>
                                                                    <option value="PA">PA</option>
                                                                    <option value="PB">PB</option>
                                                                    <option value="PE">PE</option>
                                                                    <option value="PI">PI</option>
                                                                    <option value="PR">PR</option>
                                                                    <option value="RJ">RJ</option>
                                                                    <option value="RN">RN</option>
                                                                    <option value="RO">RO</option>
                                                                    <option value="RR">RR</option>
                                                                    <option value="RS">RS</option>
                                                                    <option value="SC">SC</option>
                                                                    <option value="SE">SE</option>
                                                                    <option value="SP">SP</option>
                                                                    <option value="TO">TO</option>
                                                                </select>
                                                            </div>

                                                        </div>

                                                        <div class="row">

                                                            <div class="col-lg-3">
                                                                <label for="an-inputu" class="control-label">Definição Abertura da Negociação</label>
                                                                <input type="text" class="col-sm-1 form-control" id="an-inputu" placeholder="00">
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <label class="control-label">SLA Prioridade Liberação</label>
                                                                <select class="form-control" id="pri-inputu">
                                                                    <option value="Documento Divulgado">Documento Divulgado</option>
                                                                    <option value="Processa com Mapa Sindical">Processa com Mapa Sindical</option>
                                                                    <option value="Processa com Comparativo de Cláusulas">Processa com Comparativo de Cláusulas</option>
                                                                    <option value="Processa com Formulário">Processa com Formulário</option>
                                                                </select>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <label for="cla-inputu" class="control-label">Classe Documento</label>
                                                                <input type="text" class="col-sm-1 form-control" id="cla-inputu" placeholder="">
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <label class="control-label">Tipo Documentação</label>
                                                                <select class="form-control" id="td-inputu">
                                                                    <optgroup label="SELECIONE">
                                                                        <option value="CCT">CCT</option>
                                                                        <option value="ACT">ACT</option>
                                                                        <option value="CCT e ACT">CCT e ACT</option>
                                                                    </optgroup>
                                                                </select>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <label for="proc-inputu" class="control-label">Processamento</label>
                                                                <select class="form-control" id="proc-inputu">
                                                                    <option value="assinado ou registrado">assinado ou registrado</option>
                                                                    <option value="sem assinatura">sem assinatura</option>
                                                                    <option value="somente registrado">somente registrado</option>
                                                                </select>
                                                            </div>
                                                        </div>

                                                        <div class="row">
                                                            <div class="col-lg-4">
                                                                <label for="ent-inputu" class="control-label">SLA Entrega</label>
                                                                <select class="form-control" id="ent-inputu">
                                                                    <optgroup label="SELECIONE">
                                                                        <option value="02">02 dias úteis</option>
                                                                        <option value="03">03 dias úteis</option>
                                                                        <option value="04">03 dias úteis até 3 documentos ,
                                                                            acima de 4 até 04 dias úteis</option>
                                                                        <option value="10">10 dias úteis, Garantindo
                                                                            fechamento FOPAG</option>
                                                                    </optgroup>
                                                                </select>
                                                            </div>

                                                            <div class="col-lg-4">
                                                                <label for="corte-inputu" class="control-label">Data de
                                                                    Corte FOPAG</label>
                                                                <select class="form-control" id="corte-inputu">
                                                                    <optgroup label="SELECIONE">
                                                                        <option value="01">01</option>
                                                                        <option value="02">02</option>
                                                                        <option value="03">03</option>
                                                                        <option value="04">04</option>
                                                                        <option value="05">05</option>
                                                                        <option value="06">06</option>
                                                                        <option value="07">07</option>
                                                                        <option value="08">08</option>
                                                                        <option value="09">09</option>
                                                                        <option value="10">10</option>
                                                                        <option value="11">11</option>
                                                                        <option value="12">12</option>
                                                                        <option value="13">13</option>
                                                                        <option value="14">14</option>
                                                                        <option value="15">15</option>
                                                                        <option value="16">16</option>
                                                                        <option value="17">17</option>
                                                                        <option value="18">18</option>
                                                                        <option value="19">19</option>
                                                                        <option value="20">20</option>
                                                                        <option value="21">21</option>
                                                                        <option value="22">22</option>
                                                                        <option value="23">23</option>
                                                                        <option value="24">24</option>
                                                                        <option value="25">25</option>
                                                                        <option value="26">26</option>
                                                                        <option value="27">27</option>
                                                                        <option value="28">28</option>
                                                                        <option value="29">29</option>
                                                                        <option value="30">30</option>
                                                                        <option value="31">31</option>                                                                        
                                                                    </optgroup>
                                                                </select>
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="dataclu-inputu" class="control-label">Data Inclusão</label>
                                                                <input type="text" disabled class="form-control datepicker" id="dataclu-inputu" placeholder="DD/MM/AAAA">
                                                            </div>

                                                            <div class="col-sm-2">
                                                                <label for="dataina-inputu" class="control-label">Inativação</label>
                                                                <input type="text" class="form-control datepicker" id="dataina-inputu" placeholder="DD/MM/AAAA">
                                                            </div>

                                                            <!-- <div class="col-sm-2">
                                                                <label for="logo-inputu" class="control-label">Logo</label>
                                                                <input type="file" class="form-control-file" id="logo-inputu">
                                                                <p>Logo atual: </p><img id="logo-update" height="25" alt="...">
                                                            </div> -->


                                                        </div>
                                                    </form>
                                                </div>

                                                <!-- Seleção de módulos -->
                                                <div id="teste-tabela">
                                                    <!--INNER HTML PLS-->
                                                </div>

                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <div class="row">
                                                <div class="col-sm-12" style="display: flex; justify-content:center">
                                                    <div class="btn-toolbar">
                                                        <?php if ($thisModule->Alterar == 1): ?>
                                                                      <a id="btn-atualizar" href="#" class="btn btn-primary btn-rounded">Processar</a>
                                                        <?php else: ?>
                                                        <?php endif; ?>
                                                        <a id="btn-cancelar2" href="#" class="btn btn-danger btn-rounded">Finalizar</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div><!-- /.modal-content -->
                                </div><!-- /.modal-dialog -->
                            </div><!-- /.modal -->

                            <!-- MODAL DE CADASTRO -->
                            <button class="hide" id="btn_open_novo_cliente_matriz_modal" data-target="#novoClientMatrizModal" data-toggle="modal" class="btn default-alt"></button>
                            <div id="novoClientMatrizHidden">
                                <div id="novoClientMatrizContent">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                        <h4 class="modal-title">Cadastro de Cliente Matriz</h4>
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
                                                <form class="form-horizontal" id="upsert-form">
                                                    <div class="row">
                                                        <div class="col-lg-3 required">
                                                            <label class="control-label" for="ge-input">Grupo Econômico</label>
                                                            <select class="form-control select2" id="ge-input">
                                                            </select>
                                                        </div>

                                                        <div class="col-sm-5 required">
                                                            <label for="nome-input" class="control-label">Nome</label>
                                                            <input type="text" class="form-control" id="nome-input" placeholder="">
                                                        </div>

                                                        <div class="col-lg-2 required">
                                                            <label for="cod-input" class="control-label">Código</label>
                                                            <input type="text" class="form-control" id="cod-input" placeholder="">
                                                        </div>

                                                        <div class="col-sm-2">
                                                            <label for="dataina-input" class="control-label">Data Inativação</label>
                                                            <input type="text" class="form-control datepicker" id="dataina-input" disabled>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-sm-4 required">
                                                            <label for="an-input" class="control-label">Definição Abertura da Negociação</label>
                                                            <input type="number" class="col-sm-1 form-control" id="an-input" placeholder="00" min="0" max="360">
                                                        </div>

                                                        <div class="col-sm-4">
                                                            <label for="corte-input" class="control-label">Data de Corte FOPAG</label>
                                                            <input class="form-control" id="corte-input" type="number" min="0" max="31">
                                                        </div>

                                                        <div class="col-sm-4">
                                                            <label class="control-label">SLA Prioridade Liberação</label>
                                                            <select class="form-control" id="pri-input"></select>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-sm-4">
                                                            <label class="control-label">Tipo Documentação</label>
                                                            <select multiple class="form-control" id="td-input">
                                                            </select>
                                                        </div>

                                                        <div class="col-sm-4">
                                                            <label for="proc-input" class="control-label">Processamento</label>
                                                            <select class="form-control" id="proc-input">
                                                            </select>
                                                        </div>

                                                        <div class="col-sm-4">
                                                            <label for="logo-input" class="col-sm-4 control-label text-start">Logo</label>
                                                            <input type="file" class="form-control-file" id="logo-input" name="logotipoMatriz">
                                                        </div>
                                                    </div>
                                                </form>
                                            </div>

                                            <input type="hidden" id="modulos-input" value="">
                                        </div>

                                        <!-- Seleção de módulos -->
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                                <h4>Seleção de Módulos</h4>
                                                <div class="options">
                                                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                                                </div>
                                            </div>
                                            <div class="panel-body collapse in">
                                                <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                                                    <input type="checkbox" id="seleciona_todos_modulos">
                                                    <label for="seleciona_todos_modulos">Selecionar Todos</label>
                                                </div>
                                                <div id="grid-layout-table-2" class="box jplist">
                                                    <div class="box text-shadow">
                                                    <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl"
                                                        id="modulosTb" data-order='[[ 0, "asc" ]]' style="width: 100% !important;"></table>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <div class="row">
                                            <div class="col-sm-12" style="display: flex; justify-content:center">
                                                <button id="btn-upsert" class="btn btn-primary btn-rounded">Salvar</button>
                                                <button id="btn-inativar-ativar" class="btn btn-primary btn-rounded"></button>
                                            </div>
                                        </div>
                                    </div>
                                </div><!-- /.modal-content -->
                            </div><!-- /.modal-dialog -->
                        </div>
                    </div>

                    <div class="row" style="display: flex;">
                        <div class="col-md-12">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h4>Cliente Matriz</h4>
                                    <div class="options">
                                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                                    </div>
                                </div>
                                <div class="panel-body collapse in">
                                    <div id="grid-layout-table-1" class="box jplist">
                                        <div class="box text-shadow">
                                        <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl"
                                            id="matrizesTb" data-order='[[ 0, "asc" ]]' style="width: 100% !important;"></table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div> <!-- container -->
            </div> <!-- #wrap -->
        </div> <!-- page-content -->

    </div> <!-- page-container -->

    <?php include 'footer.php' ?>                                                       
    <script type='text/javascript' src="./js/clientematriz.min.js"></script>                                                        
</body>

</html>
