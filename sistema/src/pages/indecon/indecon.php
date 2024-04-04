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

  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>

  <!-- The following CSS are included as plugins and can be removed if unused-->
  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

  <!-- Bootstrap 3.3.7 -->
  <link rel="stylesheet" href="indecon.css">

  <!-- Bootstrap Internal -->
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    .swal2-select {
      display: none !important;
    }

    .scroll-none {
      scrollbar-width: none;
      max-height: 50vh;
      overflow-y: scroll
    }

    /* .scroll-none-group {
      scrollbar-width: none;
      max-height: 35vh; 
      overflow-y: scroll;
      
    } */

    #cabecalho {
      position: sticky;
      top: 0;
      background-color: #fff;
      border-bottom: 1px solid #bbb;
      z-index: 10;
    }

    .title-group-table {
      padding: 16px 0;
    }

    #topo {
      position: sticky;
      top: 0;
      background-color: #fff;
      z-index: 10;
    }

    .fixTableHead {
      padding: 0px 20px 20px 20px !important;
      scrollbar-width: none;
      max-height: 35vh;
      overflow-y: scroll;
      border-collapse: separate;
    }

    .scroll-true {
      border-collapse: separate;
    }

    .info-adicional,
    .info-adicionalUpdate {
      resize: vertical;
    }

    .dp-none {
      display: none;
    }

    .chosen-container {
      width: 200px !important;
    }

    .chosen-choices {
      padding: 6px 9px 5px 4px;
    }

    #page-content {
      min-height: 100% !important;
    }

    .hide {
      display: none;
    }
  </style>
</head>

<body class="horizontal-nav hide">

  <?php include('menu.php'); ?>

  <div class="page-container" id="pageCtn">
    <div id="page-content" style="min-height: 100%;">
      <!-- <div> -->
      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-1 img_container">
              <div class="container_logo_client">
                <img id="imglogo" class="img-circle">
              </div>
            </div>

            <div class="col-md-11 content_container">
              <div class="row">
                <div class="col-md-12">
                  <table class="table table-striped">

                    <tbody>
                      <tr>
                        <td>
                          <?php
                          $user = (new usuario())->validateUser($sessionUser)['response_data']['user'];
                          ?>
                          <button type="button" class="btn default-alt " data-toggle="modal"
                            data-target="#indPrincipalModal" id="indPrincipalBtn">Adicionar Indicador</button>
                          <?php if ($user->tipo == "Ineditta"): ?>
                            <button type="button" class="btn default-alt " data-toggle="modal" data-target="#indRealModal"
                              id="indRealBtn">Adicionar Indicador Real - Ineditta</button>
                          <?php endif; ?>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>

          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro de Indicadores Econômicos</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <div class="box text-shadow">
                    <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="indprincipaltb"
                      data-order='[[ 1, "asc" ]]'>
                    </table>
                  </div>
                </div>
              </div>

              <?php if ($user->tipo == "Ineditta"): ?>
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Indicadores Econômicos Reais</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in">
                    <div class="box text-shadow">
                      <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="indrealtb" data-order='[[ 1, "asc" ]]'>
                      </table>
                    </div>
                  </div>
                </div>
              <?php endif; ?>
            </div>
          </div>
        </div>
      </div>

    </div>

    <!-- INSERIR INDICADOR -->
    <div class="hidden modal_hidden" id="indPrincipalModalHidden">
      <div id="indPrincipalModalHiddenContent">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Adicionar Indicador</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Descrição</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table">
                <form class="form-horizontal" id="indicadorPrincipalForm">
                  <input type="hidden" id="id-principal-input">
                  <div class="form-group center">
                    <div class="col-sm-3">
                      <label for="origem" class="control-label">Origem</label>
                      <input id="origem" type="text" class="form-control" data-user="<?php $user->id_user ?>"
                        value="<?php $user->tipo ?>" disabled>
                    </div>

                    <div class="col-sm-4">
                      <label for="cliente" class="control-label" disabled>Cliente</label>
                      <?php if ($user->tipo == "Ineditta"): ?>
                        <input type="text" id="cliente" class="form-control" disabled value="0">
                      <?php else: ?>
                        <select id="cliente" class="form-control" disabled>
                          <?php (new indecon())->getGrupo($user->id_grupoecon) ?>
                        </select>
                      <?php endif; ?>
                    </div>

                    <div class="col-sm-1">
                      <label for="indicador" class="control-label" disabled>Indicador</label>
                      <select id="indicador" class="form-control">
                        <optgroup label="Selecione">
                          <option value="" selected>--</option>
                          <option value="INPC">INPC</option>
                          <option value="IPCA">IPCA</option>
                        </optgroup>
                      </select>
                    </div>

                    <div class="col-sm-4">
                      <label for="fonte" class="control-label" disabled>Fonte</label>
                      <input type="text" id="fonte" class="form-control">
                    </div>

                  </div>

                  <div class="form-group center scroll-none">
                    <table class="table table-hover " style="width: 100%; margin: auto;">
                      <thead id="topo">
                        <th>Período</th>
                        <th>Projetado (%)</th>
                      </thead>
                      <tbody id="campos-tabela-principal">
                        <tr>
                          <td><input type="text" class="form-control linha-periodo-principal" id="periodoPrincipal"
                              placeholder="Mês/Ano"></td>
                          <td><input type="text" class="form-control linha-valor-principal" id="projetadoPrincipal">
                          </td>
                        </tr>
                      </tbody>
                    </table>

                    <div style="margin-top: 16px" id="linhasPrincipaisToolbar">
                      <button type="button" id="btnAddLinhaPrincipal" class="btn btn-primary addLine">Adicionar
                        linha</button>
                      <button type="button" id="btnRemoveLinhaPrincipal" class="btn btn-danger removeLine">Remover
                        linha</button>
                    </div>
                  </div>
                </form>
              </div>
            </div>
            <div class="modal-footer">
              <div class="row">
                <div class="col-sm-12" style="display: flex; justify-content:center">
                  <button type="button" class="btn btn-primary btn-rounded"
                    id="indPrincipalCadastrarBtn">Salvar</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- INSERIR INDICADOR REAL INEDITTA -->
    <div class="hidden modal_hidden" id="indRealModalHidden">
      <div id="indRealModalHiddenContent">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Adicionar Indicador Real - Ineditta</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Descrição</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table">
                <form class="form-horizontal" id="indicadorRealForm">
                  <input type="hidden" id="id-real-input">
                  <div class="form-group center">
                    <div class="col-sm-1">
                      <label for="indicador-real" class="control-label" disabled>Indicador</label>
                      <select id="indicador-real" class="form-control">
                        <optgroup label="Selecione">
                          <option value="" selected>--</option>
                          <option value="INPC">INPC</option>
                          <option value="IPCA">IPCA</option>
                        </optgroup>
                      </select>
                    </div>
                  </div>

                  <div class="form-group center scroll-none">
                    <table class="table table-hover " style="width: 100%; margin: auto;">
                      <thead id="topo">
                        <th>Período</th>
                        <th>Real (%)</th>
                      </thead>
                      <tbody id="campos-tabela-real">
                        <tr>
                          <td><input type="text" class="form-control linha-periodo-real" id="periodoReal"
                              placeholder="Mês/Ano"></td>
                          <td><input type="text" class="form-control linha-valor-real" id="real"></td>
                        </tr>
                      </tbody>
                    </table>

                    <div style="margin-top: 16px" id="linhasReaisToolbar">
                      <button type="button" id="btnAddLinhaReal" class="btn btn-primary ">Adicionar linha</button>
                      <button type="button" id="btnRemoveLinhaReal" class="btn btn-danger ">Remover linha</button>
                    </div>
                  </div>
                </form>
              </div>
            </div>
            <div class="modal-footer">
              <div class="row">
                <div class="col-sm-12" style="display: flex; justify-content:center">
                  <button type="button" class="btn btn-primary btn-rounded" id="indRealCadastrarBtn">Salvar</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ATUALIZAÇÃO DE INDICADOR -->
    <!-- <div class="modal fade" id="myModalUpdate" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Atualizar Indicador</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <br>
              <form class="form-horizontal">
                <input type="hidden" id="id_inputu">

                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Descrição</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in principal-table">
                    <div class="form-group center">
                      <div class="col-sm-2" id="select">

                      </div>
                    </div>
                    <div class="form-group center">

                      <div class="col-sm-3">
                        <label for="origem" class="control-label">Origem</label>
                        <input id="origem-up" type="text" class="form-control" disabled>
                      </div>

                      <div class="col-sm-4">
                        <label for="cliente" class="control-label" disabled>Cliente</label>
                        <input type="text" id="cliente-up" class="form-control">
                      </div>

                      <div class="col-sm-1">
                        <label for="indicador" class="control-label" disabled>Indicador</label>
                        <select id="indicador-up" class="form-control">
                          <optgroup label="Selecione">
                            <option value="" selected>--</option>
                            <option value="INPC">INPC</option>
                            <option value="IPCA">IPCA</option>
                          </optgroup>
                        </select>
                      </div>

                      <div class="col-sm-4">
                        <label for="fonte" class="control-label" disabled>Fonte</label>
                        <input type="text" id="fonte-up" class="form-control">
                      </div>

                    </div>

                    <div class="form-group center scroll-none">
                      <table class="table table-hover " style="width: 100%; margin: auto;">
                        <thead id="topo">
                          <th>Projetado (%)</th>
                          <th>Real (%)</th>
                        </thead>
                        <tbody id="campos-tabela">
                          <tr>
                            <td><input type="text" id="projetado-up" class="form-control percent-input"></td>
                            <td><input type="text" id="real-up" class="form-control percent-input"></td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                  </div>

                </div>

                <div class="row">
                  <div class="col-lg-12">
                    <div class="btn-toolbar" style="display: flex; justify-content:center;">
                      <button type="button" id="btn-atualizar" class="btn btn-primary btn-rounded" style="margin-right: 5px;" onclick="updateIndicador()">Processar</button>
                      <button type="button" id="btn_delete" class="btn btn-primary btn-rounded btn-danger" data-id="" style="margin-right: 5px;" onclick="deleteIndicador()">Excluir</button>
                      <button onclick="window.location.reload()" type="button" class="btn btn-rounded btn-secondary">Voltar</button>
                    </div>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div> -->

    <!-- ATUALIZAÇÃO INDICADOR REAL INEDITTA -->
    <!-- <div class="modal fade" id="myModalUpdateReal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Atualizar Indicador Real - Ineditta</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <br>
              <form class="form-horizontal">
                <div class="panel panel-primary">
                  <div class="panel-heading">
                    <h4>Descrição</h4>
                    <div class="options">
                      <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                    </div>
                  </div>
                  <div class="panel-body collapse in principal-table">
                    <div class="form-group center">
                      <div class="col-sm-2">
                      </div>
                      <div class="col-sm-3">
                        <label for="indicador-real-update" class="control-label" disabled>Indicador</label>
                        <input type="text" class="form-control" id="indicador-real-update" disabled>
                        <input type="hidden" id="id-real-update">
                      </div>

                      <div class="col-sm-2">
                        <label for="periodo-update" class="control-label" disabled>Período</label>
                        <input type="date" class="form-control mes-ano" id="periodo-update">
                      </div>

                      <div class="col-sm-3">
                        <label for="real-update" class="control-label" disabled>Dado Real (%)</label>
                        <input type="text" class="form-control percent-input" id="real-update">
                      </div>
                      <div class="col-sm-2">
                      </div>
                    </div>
                  </div>

                </div>

                <div class="row">
                  <div class="col-lg-12">
                    <div class="btn-toolbar" style="display: flex; justify-content:center;">
                      <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;" onclick="updateIndicadorReal();">Processar</button>
                      <button id="" type="button" class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
                    </div>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div> -->

  </div> <!--page-content -->


  <?php include 'footer.php' ?>

  </div> <!--page-container -->

  <?php


  ?>

  <script type='text/javascript' src="./js/indecon.min.js"></script>
</body>

</html>