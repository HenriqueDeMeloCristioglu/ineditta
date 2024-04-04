<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
} ?>
<!DOCTYPE html>
<html lang="pt-br">

<head>
  <meta charset="utf-8">
  <title>Ineditta</title>
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <meta name="description" content="Ineditta">
  <meta name="author" content="The Red Team">

  <link rel="stylesheet" href="usuarioadm.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    .swal2-select {
      display: none !important;
    }

    #mensagem_sucesso,
    #msg-sucesso,
    #msg-sucesso-clau,
    #msg-sucesso-clau-update {
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

    .scroll-none {
      scrollbar-width: none;
      max-height: 50vh;
      overflow-y: scroll
    }

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

    #page-content {
      min-height: 100% !important;
    }

    .form-horizontal .control-label {
      padding-top: 0 !important;
    }

    .flex {
      display: flex;
    }

    .items-center {
      align-items: center;
    }

    .mt-0 {
      margin-top: 0px;
    }

    .w-full {
      width: 100%;
    }

    #configurarCalendarioSindicalModal-content {
      max-height: 95vh;
      overflow-y: scroll;
    }

    #definirNotificarAntesModal.modal.fade.in  {
      display: flex !important;
      width: 100vw;
      height: 100vh;
      align-items: center;
      justify-content: center;      
    }

    .modal.fade.in #definirNotificarAntesModal-content {
      width: 400px;
      margin: 0 auto;
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
                    <td>
                      <button type="button" class="btn default-alt" data-toggle="modal" data-target="#novoUsuarioModal"
                        id="usuarioAdmBtn">Novo usuário</button>
                    </td>
                  </tr>
                </tbody>
              </table>

              <div class="hidden" id="moduloSisapModalHidden">
                <div id="moduloSisapModalHiddenContent">
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Selecione os Módulos SISAP</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <table cellpadding="0" cellspacing="0" border="0"
                          class="table table-striped table-bordered datatables" id="moduloSisapTb" style="width: 100%;">
                          <thead>
                            <tr>
                              <th>Módulo</th>
                              <th>Criar</th>
                              <th>Consultar</th>
                              <th>Comentar</th>
                              <th>Alterar</th>
                              <th>Inativar</th>
                              <th>Aprovar</th>
                              <th>Todos</th>
                            </tr>
                          </thead>
                        </table>

                        <input type="hidden" id="modulosSisap" value="[]" class="col-sm-12">
                      </div>
                    </div>
                    <div class="modal-footer">
                      <button data-toggle="modal" type="button" class="btn btn-secondary"
                        data-dismiss="modal">Seguinte</button>
                    </div>
                  </div>
                </div>
              </div>

              <div class="hidden" id="empresasModalHidden">
                <div id="empresasModalHiddenContent">
                  <div class="modal-content">
                    <div class="modal-body">
                      <div class="panel panel-primary">
                        <div class="panel-heading">
                          <h4>Selecione a Empresa</h4>
                          <div class="options">
                            <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                          </div>
                        </div>
                        <div class="panel-body collapse in">
                          <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                            <input type="checkbox" id="select_all_empresas">
                            <label for="selectAll">Selecionar Todos</label>
                          </div>
                          <table cellpadding="0" cellspacing="0" border="0"
                            class="table table-striped table-bordered demo-tbl" id="empresasTb"
                            data-order='[[ 1, "asc" ]]' style="width: 100% !important;"></table>
                        </div>
                      </div>
                      <div class="modal-footer">
                        <button data-toggle="modal" type="button" class="btn btn-secondary"
                          data-dismiss="modal">Seguinte</button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <button type="button" class="btn btn-primary btn-rounded hide" data-toggle="modal" data-target="#definirNotificarAntesModal"
                id="definirNotificarAntesModalBtn"></button>
              <div class="hidden" id="definirNotificarAntesModalHidden">
                <div id="definirNotificarAntesModalHiddenContent">
                  <div class="modal-content">
                    <div class="modal-body">
                      <div class="panel panel-primary">
                        <div class="panel-heading">
                          <h4>Definir Notificar Antes</h4>
                          <div class="options">
                            <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                          </div>
                        </div>
                        <div class="panel-body collapse in">
                          <input type="number" min="0" max="366" class="form-control" name="tipo" id="definirAntesSelect"></select>
                        </div>
                      </div>
                      <div class="modal-footer">
                        <button data-toggle="modal" type="button" class="btn btn-secondary"
                          data-dismiss="modal" id="definirNotificarAntesCloseModalBtn">Seguinte</button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="hidden" id="moduloComercialModalHidden">
                <div id="moduloComercialModalHiddenContent">
                  <div class="modal-content">
                    <div class="modal-body">
                      <div class="panel panel-primary">
                        <div class="panel-heading">
                          <h4>Selecione os Módulos Comerciais</h4>
                          <div class="options">
                            <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                          </div>
                        </div>
                        <div class="panel-body collapse in">
                          <table cellpadding="0" cellspacing="0" border="0"
                            class="table table-striped table-bordered datatables" id="moduloComercialTb"
                            style="width: 100%;">
                            <thead>
                              <tr>
                                <th>Módulo</th>
                                <th>Criar</th>
                                <th>Consultar</th>
                                <th>Comentar</th>
                                <th>Alterar</th>
                                <th>Inativar</th>
                                <th>Aprovar</th>
                                <th>Todos</th>
                              </tr>
                            </thead>
                          </table>
                        </div>
                      </div>
                      <div class="modal-footer">
                        <button data-toggle="modal" type="button" class="btn btn-secondary"
                          data-dismiss="modal">Seguinte</button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="hidden" id="configurarCalendarioSindicalModalHidden">
                <div id="configurarCalendarioSindicalModalHiddenContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Configurar Notificações Calendário Sindical</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Selecione os Eventos que Deseja ser Notificado</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <table cellpadding="0" cellspacing="0" border="0"
                          class="table table-striped table-bordered datatables" id="configurarCalendarioSindicalTb" style="width: 100%;">
                          <thead>
                            <tr>
                              <th>Eventos</th>
                              <th>Notificar Email</th>
                              <th>Notificar WhatsApp</th>
                            </tr>
                          </thead>
                        </table>
                      </div>
                    </div>
                    <div class="modal-footer">
                      <button data-toggle="modal" type="button" class="btn btn-secondary"
                        data-dismiss="modal">Seguinte</button>
                    </div>
                  </div>
                </div>
              </div>

              <div class="hidden" id="superiorModalHidden">
                <div id="superiorModalContent">
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Selecione o Superior Imediato</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in principal-table">
                        <div class="box text-shadow">
                          <table cellpadding="0" cellspacing="0" border="0"
                            class="table table-striped table-bordered demo-tbl" id="superiorTb"
                            data-order='[[ 1, "asc" ]]' style="width: 100%;">
                            <thead>
                              <tr>
                                <th>Ações</th>
                                <th>Nome</th>
                                <th>Email</th>
                                <th>Cargo</th>
                                <th>Telefone</th>
                                <th>Ramal</th>
                                <th>Departamento</th>
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

              <div class="hidden" id="jornadaModalHidden">
                <div id="jornadaModalContent">
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Selecione a Jornada</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in principal-table">
                        <div class="box text-shadow">
                          <table cellpadding="0" cellspacing="0" border="0"
                            class="table table-striped table-bordered demo-tbl" id="jornadaTb"
                            data-order='[[ 1, "asc" ]]' style="width: 100%;">
                            <thead>
                              <tr>
                                <th>Ações</th>
                                <th>Descrição</th>
                                <th>Jornada Semanal</th>
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

              <div class="hidden" id="novoUsuarioModalHidden">
                <div id="novoUsuarioModalHiddenContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Cadastro de Usuários</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <form class="form-horizontal">
                        <div class="panel-heading">
                          <h4>Cadastro</h4>
                          <div class="options">
                            <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                          </div>
                        </div>
                        <div class="panel-body collapse in principal-table">
                          <div class="form-group">
                            <div class="col-sm-6">
                              <label for="tipo">Tipo de Usuário</label>
                              <select class="form-control" name="tipo" id="tipo">
                              </select>
                            </div>
                            <div class="col-sm-6">
                              <label for="nivel">Nível</label>
                              <select class="form-control" name="nivel" id="nivel">
                              </select>
                            </div>
                          </div>
                          <div class="form-group">
                            <div class="col-sm-4">
                              <label for="nome">Nome</label>
                              <input type="text" class="form-control" id="nome">
                            </div>
                            <div class="col-sm-4">
                              <label for="username">Nome de usuário</label>
                              <input type="text" class="form-control" id="username">
                            </div>
                            <div class="col-sm-4">
                              <label for="email">E-mail</label>
                              <input type="email" class="form-control" id="email">
                            </div>
                          </div>
                          <div class="form-group">
                            <div class="col-sm-3">
                              <label for="departamento">Departamento</label>
                              <input type="text" class="form-control" id="departamento">
                            </div>
                            <div class="col-sm-3">
                              <label for="cargo">Cargo</label>
                              <input type="text" class="form-control" id="cargo">
                            </div>
                            <div class="form-group col-sm-5">
                              <div class="col-sm-8 gap-2">
                                <label for="celular">Tipo de Número</label>
                                <select class="form-control" id="tipo_numero_select">
                                  <optgroup label="SELECIONE">
                                  </optgroup>
                                </select>
                              </div>
                              <div class="col-sm-4">
                                <label for="celular">Número</label>
                                <input type="text" class="form-control" id="celular" placeholder="(00) 0000-0000">
                              </div>
                            </div>
                            <div class="col-sm-1">
                              <label for="ramal">Ramal</label>
                              <input type="text" class="form-control" id="ramal" placeholder="000000">
                            </div>
                          </div>
                          <div class="form-group">
                            <div class="col-sm-6">
                              <div class="row">
                                <div class="col-sm-8">
                                  <label>Jornada</label>
                                  <select class="form-control" id="jornadaId" disabled>
                                    <optgroup label="SELECIONE">
                                    </optgroup>
                                  </select>
                                </div>
                                <button type="button" data-toggle="modal" data-target="#jornadaModal"
                                  class="col-sm-4 btn btn-primary btn-rounded" style="margin-top: 20px;"
                                  id="jornadaSelecionarBtn">Selecionar</button>
                              </div>
                            </div>
                            <div class="col-sm-6">
                              <div class="row">
                                <div class="col-sm-8">
                                  <label>Superior</label>
                                  <select class="form-control" id="superiorId" disabled>
                                    <optgroup label="SELECIONE">
                                    </optgroup>
                                  </select>
                                </div>
                                <button type="button" data-toggle="modal" data-target="#superiorModal"
                                  class="col-sm-4 btn btn-primary btn-rounded" style="margin-top: 20px;"
                                  id="superiorSelecionarBtn">Selecionar</button>
                              </div>
                            </div>
                          </div>
                          <div class="form-group" id="tohide">
                            <div class="col-sm-6">
                              <label>Atividade Econômica</label>
                              <select data-placeholder="Selecione" class="form-control select2" tabindex="8"
                                name="cnaes" id="cnaes">
                              </select>
                            </div>

                            <div class="col-sm-6">
                              <label>Localidade</label>
                              <select data-placeholder="Selecione" class="form-control select2" tabindex="8"
                                name="localidade" id="localidade">
                              </select>
                            </div>
                          </div>
                          <div class="form-group">
                            <div class="col-sm-6">
                              <div class="row">
                                <div class="col-sm-8">
                                  <label>Grupo Econômico</label>
                                  <select data-placeholder="Nome" class="form-control select2" id="grupoEconomico">
                                  </select>
                                </div>
                                <button type="button" data-toggle="modal" data-target="#empresasModal"
                                  class="col-sm-4 btn btn-primary btn-rounded" style="margin-top: 20px;"
                                  id="empresasModalBtn" disabled>Empresa</button>
                              </div>
                            </div>

                            <div class="col-sm-6">
                              <label>Grupo Cláusula</label>
                              <select data-placeholder="Selecione" class="form-control  select2" tabindex="8"
                                name="grupoClausulas " id="grupoClausulas">
                              </select>
                            </div>
                          </div>

                          <div class="form-group">
                            <div class="col-sm-4 hide" style="display: flex; justify-content: center;">
                              <button type="button" id="btnEnviarEmailAtualizacaoCredenciais"
                                class="btn btn-primary btn-rounded">Enviar e-mail de atualização de
                                credenciais.</button>
                            </div>

                            <div class="col-sm-6" style="display: flex; justify-content: center;">
                              <button type="button" id="btnAtualizarPermissoes"
                                class="btn btn-primary btn-rounded">Atualizar permissões.</button>
                            </div>

                            <div class="col-sm-6" style="display: flex; justify-content: center;">
                              <button type="button" id="btnEnviarEmailBoasVindas"
                                class="btn btn-primary btn-rounded">Enviar E-mail de boas vindas.</button>
                            </div>
                          </div>
                          <h3>Configurações</h3>
                          <div class="form-group flex items-center">
                            <div class="col-sm-2" style="text-align: center;">
                              <input type="checkbox" id="bloqueado">
                              <label for="bloqueado">Bloquear usuário</label>
                            </div>
                            <div class="col-sm-2" style="text-align: center;">
                              <input type="checkbox" id="documentoRestrito">
                              <label for="documentoRestrito">Ver documento restrito</label>
                            </div>
                            <div class="col-sm-2" style="text-align: center;">
                              <input type="checkbox" id="notificarEmail">
                              <label for="notificarEmail">Notificar por E-mail</label>
                            </div>
                            <div class="col-sm-2" style="text-align: center;">
                              <input type="checkbox" id="notificarWhatsapp">
                              <label for="notificarWhatsapp">Notificar por WhatsApp</label>
                            </div>
                            <div class="col-sm-4" style="display: flex; justify-content: center; align-items: center">
                              <button type="button" data-toggle="modal" data-target="#configurarCalendarioSindicalModal"
                                class="btn btn-primary btn-rounded"
                                id="configurarCalendarioSindicalModalAbrirBtn">Config. Notificações Calendário Sindical</button>
                            </div>
                          </div>
                          <h3>Periodo de Ausência</h3>
                          <div class="form-group">
                            <div class="col-sm-6">
                              <label for="ausenciaInicio">Data de ínicio
                                ausência</label>
                              <input type="text" class="form-control datepicker" id="ausenciaInicio"
                                placeholder="DD/MM/AAAA">
                            </div>
                            <div class="col-sm-6">
                              <label for="ausenciaFim">Data fim da ausência</label>
                              <input type="text" class="form-control datepicker" id="ausenciaFim"
                                placeholder="DD/MM/AAAA">
                            </div>
                          </div>
                          <h3 data-if="exibirModulos">Módulos</h3>
                          <div data-if="exibirModulos" class="form-group">
                            <div class="col-sm-6" style="display: flex; justify-content: center;">
                              <button type="button" data-toggle="modal" data-target="#moduloSisapModal"
                                class="btn btn-primary btn-rounded" style="margin-top: 20px;"
                                id="moduloSisapModalAbrirBtn">Selecionar Módulos SISAP</button>
                            </div>
                            <div class="col-sm-6" style="display: flex; justify-content: center;">
                              <button type="button" data-toggle="modal" data-target="#moduloComercialModal"
                                class="btn btn-primary btn-rounded" style="margin-top: 20px;"
                                id="moduloComercialModalAbrirBtn">Selecionar Módulos Comerciais</button>
                            </div>
                          </div>
                        </div>
                      </form>
                      <div class="row" style="margin-top: 20px;">
                        <div class="col-sm-12">
                          <div class="btn-toolbar" style="display: flex; justify-content: center;">
                            <button type="button" class="btn btn-primary btn-rounded"
                              id="btn_cadastrar">Cadastrar</button>
                          </div>
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
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro de Usuários Ineditta</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="box text-shadow">
                    <table cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="usuarioAdmTb" data-order='[[ 1, "asc" ]]'
                      style="width: 100%;">
                      <thead>
                        <tr>
                          <th>Ações</th>
                          <th>Nome</th>
                          <th>Email</th>
                          <th>Cargo</th>
                          <th>Telefone</th>
                          <th>Ramal</th>
                          <th>Departamento</th>
                          <th>Data Criação</th>
                          <th>Usuário criador</th>
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
  <input type="hidden" id="usuarioId">
  <script type='text/javascript' src="./js/usuarioadm.min.js"></script>
</body>

</html>