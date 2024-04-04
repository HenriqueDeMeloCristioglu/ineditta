<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

$userSession = $_SESSION['login'];

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

  <link rel="stylesheet" href="helpdesk.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    .select2-container {
      width: 100% !important;
      /* or any value that fits your needs */
    }

    .swal2-select {
      display: none !important;
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

    /* //////////////////////////// */
    .active_lock_user {
      position: relative;
    }

    .active_lock_user::after {
      position: absolute;
      content: '';
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;

      cursor: not-allowed;
      background-color: #edeef0;
      opacity: .2;
    }
  </style>
</head>

<body class="horizontal-nav">

  <?php include('menu.php'); ?>

  <div class="page-container">
    <div id="pageCtn" style="min-height: 100%;">
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
                          <button id="addHelpdesktModalBtn" type="button" class="btn default-alt" data-toggle="modal"
                            data-target="#addHelpdesktModal">Cadastrar Chamado</button>
                          <!-- <button type="button" class="btn default-alt" data-toggle="modal" data-target="#semHelpdeskModal">SemHelpdeskModal</button> -->
                          <button id="timelineHelpdeskModalBtn" type="button" class="btn default-alt hidden"
                            data-toggle="modal" data-target="#timelineHelpdeskModal">timelineHelpdeskModal</button>
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
              <!-- CADASTRO DE Chamado -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Gerenciar chamados</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div id="grid-layout-table-1" class="box jplist">
                    <div class="box text-shadow">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="gestaoDeChamadoTb"
                        data-order='[[ 1, "asc" ]]' style="width: 100%;">
                        <thead>
                          <tr>
                            <th></th>
                            <th>Id Chamado</th>
                            <th>Tipo de Chamado</th>
                            <th>Usuário Solicitante</th>
                            <th>Empresa</th>
                            <th>Data Abertura</th>
                            <th>Data Vencimento</th>
                            <th>Usuário Responsável</th>
                            <th>Status</th>
                            <th>Timeline</th>
                          </tr>
                        </thead>
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

    <!-- INSERIR HELPDESK -->
    <div class="hidden" id="addHelpdeskModalHidden">
      <div id="addHelpdeskModalContent">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">HelpDesk</h4>
        </div>
        <div class="modal-body">
          <div class="panel panel-primary">
            <form class="form-horizontal" enctype="multipart/form-data">
              <div class="panel panel-primary" style="margin: 0;">
                <div class="panel-heading">
                  <h4>Cadastrar Chamado</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <<<<<<< HEAD <div class="form-group center">
                    <div class="col-sm-6">
                      <label for="tipo_chamado">Tipo do Chamado</label>
                      <select data-placeholder="" class="form-control select2" id="tipo_chamado"></select>
                    </div>

                    <div class="col-sm-6">
                      <label for="estabelecimento">Estabelecimento</label>
                      <select data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente"
                        class="form-control select2" id="estabelecimento" disabled></select>
                    </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-6">
                    <label for="sind_labo">Sindicato Laboral</label>
                    <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2" id="sind_labo"
                      disabled></select>
                  </div>

                  <div class="col-md-6">
                    <label for="sind_patro">Sindicato Patronal</label>
                    <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2" id="sind_patro"
                      disabled></select>
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-6">
                    <label for="clausula">Cláusula</label>
                    <select id="clausula" class="form-control select2" disabled>
                      <?= $listaClausula ?>
                    </select>
                  </div>

                  <div class="col-md-6">
                    <label>Selecionar Documento</label>
                    <input type="file" class="form-control" id="file" disabled>
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-12">
                    <label for="comentario">Comentários</label>
                    <textarea class="form-control" id="comentario" cols="30" rows="10"></textarea>
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
            <div class="btn-toolbar" style="display: flex; justify-content:center;">
              <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;"
                onclick="uploadFile();">Processar</button>
              <button id="btn-cancelar" type="button" data-dismiss="modal"
                class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- SEM RESPONSAVEL HELPDESK -->
  <div class="hidden" id="semHelpdeskModalHidden">
    <div id="semHelpdeskModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">HelpDesk</h4>
      </div>
      <div class="modal-body">
        <div class="panel panel-primary">
          <form class="form-horizontal" enctype="multipart/form-data">
            <input type="hidden" id="id_user_res" value="<?= $id_user ?>">
            <input type="hidden" id="id_helpdesk" value="">

            <div class="panel panel-primary" style="margin-top: 0;">
              <div class="panel-heading">
                <h4>Analisar Chamado</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table">
                <div class="form-group center">
                  <div class="col-sm-6">
                    <label for="usuario_up">Usuário que realizou a chamada</label>
                    <input type="text" id="usuario_up" class="form-control" readonly>
                  </div>

                  <div class="col-sm-3">
                    <label for="data_abertura_up">Data Abertura</label>
                    <input type="text" id="data_abertura_up" class="form-control" readonly>
                  </div>

                  <div class="col-sm-3">
                    <label for="data_resposta_up">Data Resposta</label>
                    <input type="text" id="data_resposta_up" class="form-control" readonly>
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-6">
                    <label for="tipo_chamado_up">Tipo de Chamado</label>
                    <select data-placeholder="" class="form-control select2" id="tipo_chamado_up">
                      <?= $campoTipoChamado ?>
                    </select>
                  </div>

                  <div class="col-sm-6">
                    <label for="estabelecimento_up">Estabelecimento</label>
                    <select data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente"
                      class="form-control select2" id="estabelecimento_up">
                      <option value=""></option>
                      <?= $filtro['response_data']['unidade'] ?>
                    </select>
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-6">
                    <label for="sind_laboral_up">Sindicato Laboral</label>
                    <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2"
                      id="sind_laboral_up">
                      <option value=""></option>
                      <?= $filtro['response_data']['laboral'] ?>
                    </select>
                  </div>

                  <div class="col-md-6">
                    <label for="sind_patronal_up">Sindicato Patronal</label>
                    <option value=""></option>
                    <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2"
                      id="sind_patronal_up">
                      <?= $filtro['response_data']['patronal'] ?>
                    </select>
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-6">
                    <label for="clausula_up">Cláusula</label>
                    <select id="clausula_up" class="form-control select2">
                      <?= $listaClausula ?>
                    </select>
                  </div>

                  <div class="col-md-6">
                    <label>Selecionar Documento</label>
                    <input type="file" class="form-control" id="file_up">
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-12">
                    <label for="comentario_up">Comentário</label>
                    <textarea class="form-control" id="comentario_up" cols="30" rows="10"></textarea>
                  </div>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-lg-12">
                <div class="btn-toolbar" style="display: flex; justify-content:center;">
                  <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;"
                    onclick="updateUploadFile();">Processar</button>
                  <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;"
                    onclick="responderHelpdesk()">Responder</button>
                  <button id="btn-cancelar" type="button" data-dismiss="modal"
                    class="btn btn-rounded btn-danger">Finalizar</button>
                </div>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>

  <!-- TIMELINE HELPDESK -->
  <div class="hidden" id="timelineHelpdeskModalHidden">
    <div id="timelineHelpdeskModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">HelpDesk</h4>
      </div>
      <div class="modal-body">
        <div class="panel panel-primary">

          <div class="row">
            <div class="col-lg-12">
              <div class="btn-toolbar" style="display: flex; justify-content:center;">
                <button id="btn-cancelar" type="button" class="btn btn-rounded btn-danger">Finalizar</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- RESPONSAVEL HELPDESK -->
  <div class="hidden" id="responsavelHelpdeskModalHidden">
    <div id="responsavelHelpdeskModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">HelpDesk</h4>
      </div>
      <div class="modal-body">
        <div class="panel panel-primary">
          <form class="form-horizontal">
            <input type="hidden" id="id_user_logged" value="<?= $id_user ?>">
            <input type="hidden" id="id_helpdesk_res" value="">

            <div class="panel panel-primary" style="margin-top: 0;">
              <div class="panel-heading">
                <h4>Gerenciar Chamada</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table" id="responderHelpdeskModal">
                <div class="form-group center">
                  <div class="col-sm-4">
                    <label for="tipo_chamado_resposta">Tipo de Chamado</label>
                    <select data-placeholder="" class="form-control select2" id="tipo_chamado_resposta">
                      <?= $campoTipoChamado ?>
                    </select>
                  </div>

                  <div class="col-sm-3">
                    <label for="estabelecimento_resposta">Estabelecimento</label>
                    <select data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente"
                      class="form-control select2" id="estabelecimento_resposta">
                      <option value=""></option>
                      <?= $filtro['response_data']['unidade'] ?>
                    </select>
                  </div>

                  <div class="col-sm-5">
                    <label for="usuario_resposta">Usuário</label>
                    <input type="text" id="usuario_resposta" class="form-control" readonly>
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-4">
                    <label for="sind_patronal">Sindicato Patronal</label>
                    <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2"
                      id="sind_patronal_resposta">
                      <option value=""></option>
                      <?= $filtro['response_data']['patronal'] ?>
                      =======

                      <div class="form-group center">
                        <div class="row">
                          <div class="col-sm-6">
                            <label for="grupo">Grupo Econômico</label>
                            <select data-placeholder="Nome" class="form-control select2" id="grupo"></select>
                          </div>

                          <div class="col-sm-6">
                            <label for="matriz">Empresa</label>
                            <select multiple data-placeholder="Nome, Código" class="form-control select2"
                              id="matriz"></select>
                          </div>
                        </div>
                      </div>


                      <div class="form-group center">
                        <div class="col-sm-6">
                          <label for="estabelecimento">Estabelecimento</label>
                          <select data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente"
                            class="form-control select2" id="estabelecimento" disabled></select>
                        </div>
                        <div class="col-sm-6">
                          <label for="tipo_chamado">Tipo do Chamado</label>
                          <select data-placeholder="" class="form-control select2" id="tipo_chamado"></select>
                        </div>
                      </div>

                      <div class="form-group center">
                        <div class="col-md-6">
                          <label for="sind_labo">Sindicato Laboral</label>
                          <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2"
                            id="sind_labo" disabled></select>
                        </div>

                        <div class="col-md-6">
                          <label for="sind_patro">Sindicato Patronal</label>
                          <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2"
                            id="sind_patro" disabled></select>
                        </div>
                      </div>

                      <div class="form-group center">
                        <div class="col-md-6">
                          <label for="clausula">Cláusula</label>
                          <select id="clausula" class="form-control select2" disabled>
                          </select>
                        </div>

                        <div class="col-md-6">
                          <label>Selecionar Documento</label>
                          <input type="file" class="form-control" id="file" disabled>
                        </div>
                      </div>

                      <div class="form-group center">
                        <div class="col-md-12">
                          <label for="comentario">Comentários</label>
                          <textarea class="form-control" id="comentario" cols="30" rows="10"></textarea>
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
            <div class="btn-toolbar" style="display: flex; justify-content:center;">
              <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;"
                onclick="uploadFile();">Processar</button>
              <button id="btn-cancelar" type="button" data-dismiss="modal"
                class="btn btn-danger btn-rounded btn-cancelar">Finalizar</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- SEM RESPONSAVEL HELPDESK -->
  <div class="hidden" id="semHelpdeskModalHidden">
    <div id="semHelpdeskModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">HelpDesk</h4>
      </div>
      <div class="modal-body">
        <div class="panel panel-primary">
          <form class="form-horizontal" enctype="multipart/form-data">
            <input type="hidden" id="id_user_res" value="">
            <input type="hidden" id="id_helpdesk" value="">

            <div class="panel panel-primary" style="margin-top: 0;">
              <div class="panel-heading">
                <h4>Analisar Chamado</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table">
                <div class="form-group center">
                  <div class="col-sm-6">
                    <label for="usuario_up">Usuário que realizou a chamada</label>
                    <input type="text" id="usuario_up" class="form-control" readonly>
                  </div>

                  <div class="col-sm-3">
                    <label for="data_abertura_up">Data Abertura</label>
                    <input type="text" id="data_abertura_up" class="form-control" readonly>
                  </div>

                  <div class="col-sm-3">
                    <label for="data_resposta_up">Data Resposta</label>
                    <input type="text" id="data_resposta_up" class="form-control" readonly>
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-6">
                    <label for="tipo_chamado_up">Tipo de Chamado</label>
                    <select data-placeholder="" class="form-control select2" id="tipo_chamado_up">
                    </select>
                  </div>

                  <div class="col-sm-6">
                    <label for="estabelecimento_up">Estabelecimento</label>
                    <select data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente"
                      class="form-control select2" id="estabelecimento_up">
                    </select>
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-6">
                    <label for="sind_laboral_up">Sindicato Laboral</label>
                    <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2"
                      id="sind_laboral_up">
                      <option value=""></option>
                    </select>
                  </div>

                  <div class="col-md-6">
                    <label for="sind_patronal_up">Sindicato Patronal</label>
                    <option value=""></option>
                    <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2"
                      id="sind_patronal_up">
                    </select>
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-6">
                    <label for="clausula_up">Cláusula</label>
                    <select id="clausula_up" class="form-control select2">
                    </select>
                  </div>

                  <div class="col-md-6">
                    <label>Selecionar Documento</label>
                    <input type="file" class="form-control" id="file_up">
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-12">
                    <label for="comentario_up">Comentário</label>
                    <textarea class="form-control" id="comentario_up" cols="30" rows="10"></textarea>
                  </div>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-lg-12">
                <div class="btn-toolbar" style="display: flex; justify-content:center;">
                  <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;"
                    onclick="updateUploadFile();">Processar</button>
                  <button type="button" class="btn btn-primary btn-rounded" style="margin-right: 5px;"
                    onclick="responderHelpdesk()">Responder</button>
                  <button id="btn-cancelar" type="button" data-dismiss="modal"
                    class="btn btn-rounded btn-danger">Finalizar</button>
                </div>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>

  <!-- TIMELINE HELPDESK -->
  <div class="hidden" id="timelineHelpdeskModalHidden">
    <div id="timelineHelpdeskModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">HelpDesk</h4>
      </div>
      <div class="modal-body">
        <div class="panel panel-primary">

          <div class="row">
            <div class="col-lg-12">
              <div class="btn-toolbar" style="display: flex; justify-content:center;">
                <button id="btn-cancelar" type="button" class="btn btn-rounded btn-danger">Finalizar</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- RESPONSAVEL HELPDESK -->
  <div class="hidden" id="responsavelHelpdeskModalHidden">
    <div id="responsavelHelpdeskModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">HelpDesk</h4>
      </div>
      <div class="modal-body">
        <div class="panel panel-primary">
          <form class="form-horizontal">
            <input type="hidden" id="id_user_logged" value="">
            <input type="hidden" id="id_helpdesk_res" value="">

            <div class="panel panel-primary" style="margin-top: 0;">
              <div class="panel-heading">
                <h4>Gerenciar Chamada</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table" id="responderHelpdeskModal">
                <div class="form-group center">
                  <div class="col-sm-4">
                    <label for="tipo_chamado_resposta">Tipo de Chamado</label>
                    <select data-placeholder="" class="form-control select2" id="tipo_chamado_resposta">
                      <?= $campoTipoChamado ?>
                      >>>>>>> 10362b3cf024c6fe29473b17007626bffc6825fc
                    </select>
                  </div>

                  <div class="col-sm-3">
                    <<<<<<< HEAD <label for="sind_laboral">Sindicato Laboral</label>
                      <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2"
                        id="sind_laboral_resposta">
                        <option value=""></option>
                        <?= $filtro['response_data']['laboral'] ?>
                        =======
                        <label for="estabelecimento_resposta">Estabelecimento</label>
                        <select data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente"
                          class="form-control select2" id="estabelecimento_resposta">
                          <option value=""></option>
                          >>>>>>> 10362b3cf024c6fe29473b17007626bffc6825fc
                        </select>
                  </div>

                  <div class="col-sm-5">
                    <<<<<<< HEAD <label for="destinatario_resposta">Destinatário</label>
                      <input type="text" id="destinatario_resposta" class="form-control" readonly>
                      =======
                      <label for="usuario_resposta">Usuário</label>
                      <input type="text" id="usuario_resposta" class="form-control" readonly>
                      >>>>>>> 10362b3cf024c6fe29473b17007626bffc6825fc
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-4">
                    <<<<<<< HEAD <label for="clausula_resposta">Cláusula</label>
                      <select id="clausula_resposta" class="form-control select2">
                        <?= $listaClausula ?>
                      </select>
                  </div>

                  <div class="col-md-3">
                    <label>Selecionar Documento</label>
                    <input type="file" class="form-control" id="doc_input">
                  </div>

                  <div class="col-md-3">
                    <label for="data_resposta">Data da Resposta</label>
                    <input type="text" class="form-control" id="data_resposta" readonly>
                  </div>

                  <div class="col-md-2">
                    <label for="status_resposta">Status</label>
                    <input type="text" class="form-control" id="status_resposta" readonly>
                  </div>
                </div>

                <div class="form-group center">
                  <div class="col-md-7">
                    <label for="comentario_resposta">Comentário</label>
                    <textarea class="form-control" id="comentario_resposta" cols="30" rows="10"></textarea>
                  </div>
                  <div class="col-md-5">
                    <label for="resposta">Resposta</label>
                    <textarea class="form-control" id="resposta" cols="30" rows="10"></textarea>
                  </div>
                </div>
              </div>
            </div>

            <div class="row">
              <div class="col-lg-12">
                <div class="btn-toolbar" style="display: flex; justify-content:center;">
                  <button type="button" class="btn btn-primary btn-rounded" id="btn-resposta" style="margin-right: 5px;"
                    onclick="conclusaoHelpdesk()">Processar</button>
                  <button id="btn-cancelar" type="button" class="btn btn-rounded btn-danger"
                    data-dismiss="modal">Finalizar</button>
                </div>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
  </div> <!--page-content -->

  <footer role="contentinfo">
    <div class="clearfix">
      <ul class="list-unstyled list-inline pull-left">
        <li>Ineditta &copy; 2022</li>
      </ul>
      <button class="pull-right btn btn-inverse-alt btn-xs hidden-print" id="back-to-top"><i
          class="fa fa-arrow-up"></i></button>
    </div>
  </footer>

  =======
  <label for="sind_patronal">Sindicato Patronal</label>
  <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2" id="sind_patronal_resposta">
    <option value=""></option>
  </select>
  </div>

  <div class="col-sm-3">
    <label for="sind_laboral">Sindicato Laboral</label>
    <select data-placeholder="Sigla, CNPJ ou Denominação" class="form-control select2" id="sind_laboral_resposta">
      <option value=""></option>
    </select>
  </div>

  <div class="col-sm-5">
    <label for="destinatario_resposta">Destinatário</label>
    <input type="text" id="destinatario_resposta" class="form-control" readonly>
  </div>
  </div>

  <div class="form-group center">
    <div class="col-md-4">
      <label for="clausula_resposta">Cláusula</label>
      <select id="clausula_resposta" class="form-control select2">
      </select>
    </div>

    <div class="col-md-3">
      <label>Selecionar Documento</label>
      <input type="file" class="form-control" id="doc_input">
    </div>

    <div class="col-md-3">
      <label for="data_resposta">Data da Resposta</label>
      <input type="text" class="form-control" id="data_resposta" readonly>
    </div>

    <div class="col-md-2">
      <label for="status_resposta">Status</label>
      <input type="text" class="form-control" id="status_resposta" readonly>
    </div>
  </div>

  <div class="form-group center">
    <div class="col-md-7">
      <label for="comentario_resposta">Comentário</label>
      <textarea class="form-control" id="comentario_resposta" cols="30" rows="10"></textarea>
    </div>
    <div class="col-md-5">
      <label for="resposta">Resposta</label>
      <textarea class="form-control" id="resposta" cols="30" rows="10"></textarea>
    </div>
  </div>
  </div>
  </div>

  <div class="row">
    <div class="col-lg-12">
      <div class="btn-toolbar" style="display: flex; justify-content:center;">
        <button type="button" class="btn btn-primary btn-rounded" id="btn-resposta" style="margin-right: 5px;"
          onclick="conclusaoHelpdesk()">Processar</button>
        <button id="btn-cancelar" type="button" class="btn btn-rounded btn-danger"
          data-dismiss="modal">Finalizar</button>
      </div>
    </div>
  </div>
  </form>
  </div>
  </div>
  </div>
  </div>
  </div> <!--page-content -->


  <?php include 'footer.php' ?>

  >>>>>>> 10362b3cf024c6fe29473b17007626bffc6825fc
  </div> <!--page-container -->

  <script type='text/javascript' src="./js/helpdesk.min.js"></script>
</body>

</html>