<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
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

  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>
  <!-- The following CSS are included as plugins and can be removed if unused-->
  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

  <!-- Bootstrap 3.3.7 -->
  <link rel="stylesheet" href="notificacao.css">

  <!-- Bootstrap Internal -->
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    .select2-container {
      width: 100% !important;
      /* or any value that fits your needs */
    }

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
  </style>
</head>

<body class="horizontal-nav">

  <?php include('menu.php'); ?>

  <div class="page-container" id="pageCtn">
    <div id="page-content" style="min-height: 100%;">
      <!-- <div> -->
      <div id="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-11 content_container">
              <div class="row">
                <div class="col-md-12">
                  <table class="table table-striped">
                    <tbody>
                      <tr>
                        <td>
                          <button type="button" class="btn default-alt " data-toggle="modal"
                            data-target="#notificacaoModal" id="noficacaoModalBtn">Cadastrar Comentário</button>
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
              <!-- CADASTRO DE Comentário -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro de Comentário</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div id="grid-layout-table-1" class="box jplist">
                    <div class="box text-shadow">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="notificacaotb"
                        style="width: 100%; margin-top: 10px;" data-order='[[ 1, "asc" ]]'>
                        <thead>
                          <tr>
                            <th></th>
                            <th>Tipo Comentário</th>
                            <th>Tipo Usuário</th>
                            <th>Tipo Notificação</th>
                            <th>Etiqueta</th>
                            <th>Nome da cláusula</th>
                            <th>Sindicato Laboral</th>
                            <th>Sindicato Patronal</th>
                            <th>Data Final</th>
                            <th>Usuário</th>
                            <th>Comentário</th>
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


    <!-- INSERIR NOTIFICAÇÃO -->
    <div class="hidden modal_hidden" id="noticacaoModalHidden">
      <div id="noticacaoModalHiddenContent">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Adicionar Comentário</h4>
        </div>
        <div class="modal-body">
          <div class="panel panel-primary">
            <form class="form-horizontal">
              <div class="panel panel-primary" style="margin: 0;">
                <div class="panel-heading">
                  <h4>Comentário</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="form-group center">
                    <div class="col-sm-5">
                      <label for="tipo-com">Tipo do Comentário</label>
                      <select id="tipo-com" class="form-control select2">
                      </select>
                    </div>

                    <div class="col-sm-7">
                      <label for="assunto" id="assuntoTitulo">--</label>
                      <select id="assunto" class="form-control select2">
                      </select>
                    </div>

                  </div>

                  <div class="form-group center">
                    <div class="col-md-5">
                      <label for="tipo_usuario_destino">Tipo do Usuário (destino)</label>
                      <select id="tipo_usuario_destino" class="form-control">
                      </select>
                    </div>

                    <div class="col-md-7">
                      <label for="destino" id="campo_tipo">--</label>
                      <select id="destino" class="form-control">
                      </select>
                    </div>
                  </div>

                  <div class="form-group center">
                    <div class="col-sm-4">
                      <label for="tipo-note" class="control-label">Fixo ou Temporário</label>
                      <select id="tipo-note" class="form-control select2">
                      </select>
                    </div>

                    <div class="col-md-4">
                      <label for="validade">Validade</label>
                      <input type="text" id="validade" class="form-control">
                    </div>

                    <div class="col-md-4">
                      <label for="usuario">Usuário</label>
                      <input type="text" id="usuario" class="form-control" disabled>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="col-md-4">
                      <label for="tipo-etiqueta" class="control-label">Tipo de Etiqueta</label>
                      <select id="tipo-etiqueta" class="form-control select2">
                      </select>
                    </div>
                    <div class="col-md-4">
                      <label for="etiqueta" class="control-label">Etiqueta</label>
                      <select id="etiqueta" class="form-control select2">
                      </select>
                    </div>
                    <div class="col-md-4">
                      <label for="visivel" class="control-label" style="text-align: left;">Visível para outros usuários
                        com acesso a consulta de comentários?</label>
                      <select id="visivel" class="form-control select2">
                      </select>
                    </div>
                  </div>

                  <div class="form-group center">
                    <div class="col-md-12">
                      <label for="comentario">Comentário</label>
                      <textarea class="form-control" id="comentario" cols="30" rows="10"></textarea>
                    </div>
                  </div>
                  <div>
                    <input type="hidden" id="id_note" value="">
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
                <button id="notificacaoCadastrarBtn" type="button" class="btn btn-primary btn-rounded">Salvar</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div> <!--page-content -->


  <?php include 'footer.php' ?>

  </div> <!--page-container -->

  <script type='text/javascript' src="./js/notificacao.min.js"></script>
</body>

</html>