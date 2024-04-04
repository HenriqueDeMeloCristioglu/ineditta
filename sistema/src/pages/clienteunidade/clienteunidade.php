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

  <link rel="stylesheet" href="clienteunidade.css">
  <link rel="stylesheet" href="includes/css/styles.css">

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

  <div id="pageCtn">
    <div style="min-height: 100%;">
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
                    <td>
                      <button type="button" class="btn default-alt" data-toggle="modal"
                        data-target="#novoClientUnidadeModal" id="novoClientUnidadeModalBtn">NOVO CLIENTE
                        FILIAL</button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro de Estabelecimento</h4> <!-- Cadastro de Cliente Filial -->
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div id="grid-layout-table-1" class="box jplist">
                    <div class="box text-shadow">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered datatables" id="clienteUnidadeTb"
                        style="width: 100%;">
                      </table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div> <!-- page-container -->

  <div class="hidden" id="novoClientUnidadeHidden">
    <div id="novoClientUnidadeContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Cliente Filial</h4>
      </div>
      <div class="modal-body">
        <div class="panel panel-primary" id="panel-novo-registro">
          <div class="panel-heading">
            <h4>Novo Registro</h4>
            <div class="options">
              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
            </div>
          </div>
          <div class="panel-body collapse in">
            <form class="form-horizontal">
              <div class="row">
                <div class="col-lg-3">
                  <label class="control-label">Empresa Matriz</label>
                  <select class="form-control select2" id="em-input"></select>
                </div>

                <div class="col-lg-2">
                  <label class="control-label">Tipo de Negócio</label>
                  <select class="form-control select2" id="tn-input"></select>
                </div>

                <div class="col-lg-3">
                  <label class="control-label">Localização</label>
                  <select class="form-control select2" id="loc-input"></select>
                </div>

                <div class="col-lg-3">
                  <label class="control-label">Cnae Filial</label>
                  <select class="form-control select2" id="cnae_filial_input"></select>
                </div>

                <div class="col-lg-1">
                  <label for="cod-input" class="control-label">Código</label>
                  <input type="text" class="form-control" id="cod-input">
                </div>
              </div>

              <div class="row">
                <div class="col-lg-3">
                  <label for="nome-input" class="control-label">Nome</label>
                  <input type="text" class="form-control" id="nome-input" placeholder="">
                </div>

                <div class="col-sm-2">
                  <label for="cnpj-input" class="control-label">CNPJ</label>
                  <input type="text" class="form-control" id="cnpj-input" placeholder="00.000.000/0000-00">
                </div>

                <div class="col-sm-3">
                  <label for="end-input" class="control-label">Logradouro</label>
                  <input type="text" class="form-control" id="end-input" placeholder="">
                </div>

                <div class="col-sm-2">
                  <label for="bairro-input" class="control-label">Bairro</label>
                  <input type="text" class="form-control" id="bairro-input" placeholder="">
                </div>

                <div class="col-sm-2">
                  <label for="cep-input" class="control-label">CEP</label>
                  <input type="text" class="form-control" id="cep-input" placeholder="00000-000">
                </div>
              </div>

              <div class="row">
                <div class="col-sm-4">
                  <label for="reg-input" class="control-label">Regional</label>
                  <input type="text" class="form-control" id="reg-input" placeholder="">
                </div>

                <div class="col-sm-4" style="display: flex; gap: 10px;">
                  <div id="data_inclusao" style="width: 100%;">
                    <label for="dataina-input" class="control-label">Data
                      Inclusão</label>
                    <input type="text" class="form-control" id="data_inclusao_input" disabled>
                  </div>
                  <div style="width: 100%;">
                    <label for="dataina-input" class="control-label">Data
                      Inativação</label>
                    <input type="text" class="form-control" id="dataina-input">
                  </div>
                </div>

                <div class="col-sm-2">
                  <label for="csc-input" class="control-label">Cód. Sind.
                    Cliente</label>
                  <input type="text" class="form-control" id="csc-input" placeholder="">
                </div>

                <div class="col-sm-2">
                  <label for="csp-input" class="control-label">Cód. Sind.
                    Patronal</label>
                  <input type="text" class="form-control" id="csp-input" placeholder="">
                </div>
              </div>
            </form>
          </div>
        </div>
        <!-- PAINEL CNAES SELECIONADOS -->
        <div class="panel panel-primary" id="cnaes_selecionados">
          <div class="panel-heading">
            <h4>CNAES Selecionados</h4>
            <div class="options">
              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
            </div>
          </div>
          <div class="panel-body collapse in">
            <div class="box text-shadow">
              <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                <input type="checkbox" id="select_todos_cnaes_selecionados">
                <label for="select_todos_cnaes_selecionados">Selecionar Todos</label>
              </div>
              <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl"
                id="cnaesSelecionadosTb" data-order='[[ 1, "asc" ]]' style="width: 100% !important;"></table>
              <div class="col-sm-12">
                <button type="button" class="btn btn-primary btn-rounded" id="btn_excluir_cnaes">Excluir</button>
              </div>
            </div>
          </div>
        </div>

        <!-- PAINEL SELEÇÃO DE CNAE -->
        <div class="panel panel-primary" id="panel-select-cnaes">
          <div class="panel-heading">
            <h4>Seleção de CNAE</h4>
            <div class="options">
              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
            </div>
          </div>
          <div class="panel-body collapse in">
            <div class="box text-shadow">
              <div class="panel-body collapse in">
                <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                  <input type="checkbox" id="select_todos_cnaes">
                  <label for="select_todos_cnaes">Selecionar Todos</label>
                </div>
                <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl"
                  id="cnaesTb" data-order='[[ 1, "asc" ]]' style="width: 100% !important;"></table>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="modal-footer">
        <div class="row">
          <div class="col-lg-12" style="display: flex; justify-content:center">
            <button type="button" class="btn btn-primary btn-rounded" id="btn_submit_modal">Salvar</button>
          </div>
        </div>
      </div>
    </div>
  </div>

  <?php include 'footer.php' ?>

  <script type='text/javascript' src="./js/clienteunidade.min.js"></script>
</body>

</html>