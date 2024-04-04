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

  <link rel="stylesheet" href="documentos.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    div.modal-content {
      max-height: 90vh;
      overflow-y: scroll;
    }

    .modal.fade.in {
      display: flex !important;
      justify-content: center;
      align-items: center;
    }

    .flex-grow-1 {
      flex-grow: 1;
    }

    .flex-end {
      display: flex;
      align-items: end;
    }

    .btn-select {
      margin-top: 7px;
      width: 100%;
    }

    .fa-map-marker {
      margin-right: 10px;
    }

    .btn-box {
      margin-top: 30px;
    }

    #page-content {
      min-height: 100vh !important;
    }

    .btn-documents {
      border-radius: 50%;
      width: 32px;
      height: 32px;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 0;
      font-size: 30px;
    }

    #checkbox_group {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 10px;
    }

    #checkbox_group label {
      transform: translateY(6px);
    }

    .form-group.buttons .row {
      display: flex;
      align-items: flex-end;
    }

    .form-group.buttons .row .big-btn a.btn.btn-primary {
      width: 200px;
      padding: 10px 0px;
      font-size: 15px;
    }

    .form-group.buttons .row .big-btn {
      display: flex;
      justify-content: center;
    }

    form .row {
      padding: 0px 10px;
    }

    .has-warning label {
      color: #ffc107;
    }

    .has-warning .select2-container--default .select2-selection--single,
    .has-warning .select2-container--default .select2-selection--multiple {
      border-color: #ffc107;
    }

    .has-warning .btn {
      color: #ffc107;
    }

    #page-content {
      min-height: 100% !important;
    }

    .hide {
      display: none;
    }

    .aviso * {
      display: inline-block;
    }

    .aviso h5 {
      font-weight: 600px;
      font-size: 1.5rem;
      margin-top: 0;
      margin-bottom: 16px;
    }

    .aviso span {
      font-size: 1.125rem;
    }

    .mb-4 {
      margin-bottom: 16px;
    }
  </style>
</head>

<body class="horizontal-nav hide">

  <?php include('menu.php'); ?>

  <div id="pageCtn">
    <div id="page-content">
      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <!-- NOVO CADASTRO  -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro de Documentos não processados</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <form id="form_add" class="form-horizontal">
                    <input type="hidden" id="selectCltUn">
                    <div class="row aviso">
                      <h5>AVISO:&nbsp;</h5><span>Este módulo é destinado para inserir documentos de biblioteca, que <strong>não processam cláusulas.</strong></span>
                    </div>

                    <div class="form-group" id="no_reference">
                      <div class="row">
                        <div class="col-lg-6 required">
                          <label>Selecionar Documento</label>
                          <input type="file" class="form-control" id="file" name="file_documentos_sindicais" required>
                        </div>
                        <div class="col-sm-6 required" style="float: right;">
                          <label for="origem_doc">Origem do Documento</label>
                          <?php
                          $user = (new usuario())->validateUser($sessionUser)['response_data']['user'];
                          ?>
                          <input type="text" id="origem_doc" class="form-control" value="<?= $user->tipo ?>" required
                            disabled>
                        </div>
                      </div>
                    </div>
                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-4 required">
                          <label>Tipo do Documento</label>
                          <select data-placeholder="Selecione" tabindex="8" class="form-control select2" id="tipo_doc"
                            required></select>
                        </div>
                        <div class="col-sm-4 required">
                          <label>Nome do Documento</label>
                          <select data-placeholder="Selecione" tabindex="8" class="form-control select2" id="nome_doc"
                            required></select>
                        </div>
                        <div class="col-sm-4 required">
                          <label for="assunto">Assunto</label>
                          <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="assunto"></select>
                        </div>
                      </div>
                    </div>
                    <div class="form-group buttons">
                      <div class="row">
                        <div class="flex-end col-md-2 required">
                          <div class="flex-grow-1">
                            <label for="anuencia">Notificar</label>
                            <select id="anuencia" class="form-control">
                              <option value=""></option>
                              <option value="sim">Sim</option>
                              <option value="nao">Não</option>
                            </select>
                          </div>

                          <button type="button" class="btn btn-primary disabled" data-toggle="modal"
                            data-target="#anuenciaModal" id="anuenciaModalBtn">
                            <i class="fa fa-user"></i>
                          </button>
                        </div>

                        <div class="col-lg-4">
                          <label>Número da Legislação</label>
                          <input type="text" id="numero_lei" class="form-control" placeholder="Número/Ano">
                        </div>

                        <div class="col-sm-4">
                          <label for="fonte_site">Fonte Legislação Site</label>
                          <input type="text" id="fonte_site" class="form-control">
                        </div>

                        <div class="col-lg-2 required">
                          <label for="restrito">Restrito</label>
                          <select id="restrito" class="form-control">
                            <option value=""></option>
                            <option value="sim">Sim</option>
                            <option value="nao">Não</option>
                          </select>
                        </div>
                      </div>
                    </div>
                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-6">
                          <label>Sindicato Laboral</label>
                          <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sind_laboral"></select>
                        </div>

                        <div class="col-sm-6">
                          <label>Sindicato Patronal</label>
                          <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sind_patronal"></select>
                        </div>
                      </div>
                    </div>
                    <div class="form-group flex-end">
                      <div class="col-sm-3 required">
                        <label for="vigencia_inicial">Validade Inicial</label>
                        <input type="text" class="form-control datepicker" id="vigencia_inicial"
                          placeholder="dd/mm/aaaa">
                      </div>

                      <div class="col-sm-3">
                        <label for="vigencia_final">Validade Final</label>
                        <input type="text" class="form-control datepicker" id="vigencia_final" placeholder="dd/mm/aaaa">
                      </div>

                      <div class="col-sm-2 required big-btn">
                        <button type="button" class="btn btn-primary" data-toggle="modal"
                          data-target="#empresaModal">Empresa</button>
                      </div>

                      <div class="col-sm-2 required big-btn">
                        <button type="button" class="btn btn-primary" data-toggle="modal"
                          data-target="#abrangenciaModal">Abrangência</button>
                      </div>

                      <div class="col-sm-2 required big-btn">
                        <button type="button" class="btn btn-primary" data-toggle="modal"
                          data-target="#atividadeEconomicaModal">Atividade Econômica</button>
                      </div>
                    </div>
                    <div class="form-group">
                      <div class="col-sm-12">
                        <label for="comentarios">Comentários</label>
                        <textarea class="form-control" id="comentarios" cols="30" rows="10"></textarea>
                      </div>
                    </div>
                  </form>

                  <div class="row">
                    <div class="col-sm-12">
                      <button id="btn-cadastrar-doc" type="button"
                        class="btn btn-primary btn-rounded">Cadastrar</button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div> <!-- container -->
      </div> <!-- #wrap -->

      <!-- MODAL USUARIOS - ANUENCIA -->
      <div class="hidden" id="anuenciaModalHidden">
        <div id="anuenciaModalContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Usuários</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione o(s) Usuário(s) a Notitificar a Criação do Documento</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <input type="hidden" class="form-control" id="id_user" placeholder="">
                <div id="grid-layout-table-3" class="box jplist">
                  <div class="box text-shadow">
                    <div class="row">
                      <button type="button" class="btn btn-primary mb-4" data-toggle="modal"
                        data-target="#empresaFiltroUsuarioModal">Filtrar Usuários por Estabelecimentos</button>
                    </div>

                    <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                      <input type="checkbox" id="seleciona_todos_usuario">
                      <label for="selectAll">Selecionar Todas as Empresas</label>
                    </div>

                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="usuariosTb" data-order='[[ 0, "asc" ]]'
                      style="width: 100%;"></table>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Seguinte</button>
          </div>
        </div>
      </div>

      <!-- MODAL ABRANGENCIA -->
      <div class="hidden" id="abrangenciaModalHidden">
        <div id="abrangenciaModalContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Abrangência</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione a Abrangência do Documento</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <div class="row" style="margin-bottom: 20px ;">
                  <div class="col-lg-4" style="margin-top: 10px;">
                    <label for="estado-input">Selecione o Estado</label>
                    <select class="form-control uf-input-abrang" id="uf_input">
                      <optgroup>
                        <option value="0">SELECIONE</option>
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
                      </optgroup>
                    </select>
                  </div>

                  <div class="col-lg-6">
                    <div class="btn-toolbar btn-control btn-box">
                      <button style="width: 150px;" id="btn_add_abrangencia"
                        class="btn btn-primary btn-rounded">Adicionar</button>
                      <button style="width: 150px; margin-left: 10px;" id="btn_reset_abrangencia"
                        class="btn btn-primary btn-rounded">Resetar</button>
                    </div>
                  </div>
                </div>

                <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                  <input type="checkbox" id="seleciona_todas_regioes">
                  <label for="selectAll">Selecionar Todos os Municípios</label>
                </div>
                <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables"
                  id="abrangenciaTb" data-order='[[ 0, "asc" ]]' style="width: 100%;">
                  <thead>
                    <tr>Selecione</tr>
                    <tr>Município</tr>
                    <tr>País</tr>
                  </thead>
                </table>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
              data-dismiss="modal">Voltar</button>
          </div>
        </div>
      </div>

      <!-- MODAL EMPRESA -->
      <div class="hidden" id="empresaModalHidden">
        <div id="empresaModalContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Empresa</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione a Empresa</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <input type="hidden" class="form-control" id="id_empresa" placeholder="">

                <div id="grid-layout-table-2" class="box jplist">
                  <div class="box text-shadow">
                    <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                      <input type="checkbox" id="seleciona_todas_empresas">
                      <label for="selectAll">Selecionar Todas as Empresas</label>
                    </div>
                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="empresaTb" data-order='[[ 0, "asc" ]]'
                      style="width: 100%;"></table>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
              data-dismiss="modal">Voltar</button>
          </div>
        </div>
      </div>

      <!-- MODAL EMPRESA PARA FILTRAR USUÁRIOS A NOTIFICAR -->
      <div class="hidden" id="empresaFiltroUsuarioModalHidden">
        <div id="empresaFiltroUsuarioModalContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Empresa</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione a Empresa</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <input type="hidden" class="form-control" id="id_empresa" placeholder="">

                <div id="grid-layout-table-2" class="box jplist">
                  <div class="box text-shadow">
                    <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                      <input type="checkbox" id="seleciona_todas_empresas_filtro_usuario">
                      <label for="selectAll">Selecionar Todas as Empresas</label>
                    </div>
                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="empresaFiltroUsuarioTb" data-order='[[ 0, "asc" ]]'
                      style="width: 100%;"></table>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
              data-dismiss="modal">Voltar</button>
          </div>
        </div>
      </div>

      <!-- MODAL ATIVIDADE ECONOMICA -->
      <div class="hidden" id="atividadeEconomicaModalHidden">
        <div id="atividadeEconomicaModalContent">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            <h4 class="modal-title">Atividade Econômica</h4>
          </div>
          <div class="modal-body">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Selecione a Atividade Econômica</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <input type="hidden" class="form-control" id="id_cnae" placeholder="">

                <div id="grid-layout-table-1" class="box jplist">
                  <div class="box text-shadow">
                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="cnaesTb" data-order='[[ 0, "asc" ]]'
                      style="width: 100%;"></table>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
              data-dismiss="modal">Voltar</button>
          </div>
        </div>
      </div>

    </div> <!-- page-content -->

    <?php include 'footer.php' ?>

  </div> <!-- page-container -->

  <script type='text/javascript' src="./js/documentos.min.js"></script>
</body>

</html>