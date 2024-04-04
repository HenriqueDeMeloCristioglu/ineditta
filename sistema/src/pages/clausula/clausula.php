<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

$userSession = $_SESSION['login'];

header('Content-type: text/html; charset=UTF-8');
header('Cache-Control: no-cache');

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

  <link rel="stylesheet" href="clausula.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    .img_box {
      position: absolute;
      z-index: 999;
      width: 100vw;
      height: 100vh;
      background-color: rgba(255, 255, 255, 0.7);
      display: none;
    }

    .img_load {
      position: absolute;
      top: 30%;
      right: 45%;
    }

    .dialog-full {
      width: 90% !important;
    }

    .swal2-select {
      display: none !important;
    }

    .select2-container--default {
      width: 100% !important;
    }

    .scroll-none {
      max-height: 40vh;
      overflow-y: scroll;
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
      max-height: 800px;
      overflow: auto;
      border-collapse: separate;

    }

    .info-adicional,
    .info-adicional-grupo,
    .info-adicional-grupo-update {
      width: 100%;
    }

    .scroll-true {
      border-collapse: separate;
    }

    .scroll-true td {
      min-width: 200px;
    }

    .dp-none {
      display: none;
    }

    #scrap table tr td {
      word-break: break-all;
    }

    #page-content {
      min-height: 100% !important;
    }
  </style>
</head>

<body class="horizontal-nav">
  <?php include('menu.php'); ?>

  <div class="page-container" id="pageCtn">
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
              <div class="row">
                <div class="col-md-12">
                  <table class="table table-striped">
                    <tbody>
                      <tr>
                        <td>
                          <button type="button" class="btn default-alt" data-toggle="modal" data-target="#clausulaModal"
                            id="clausulaBtn">Adicionar Cláusula</button>
                          <button type="button" class="btn default-alt" data-toggle="modal"
                            data-target="#documentosAprovadosModal" id="documentosAprovadosBtn">Documentos
                            Liberados</button>
                          <button type="button" class="hidden" data-toggle="modal"
                            data-target="#clausulasDocumentoModal" id="clausulasDocumentoBtn"></button>
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
              <!-- TELA INICIAL -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Cadastro de Cláusula</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div id="grid-layout-table-1" class="box jplist">
                    <div class="box text-shadow">
                      <table cellpadding="0" cellspacing="0" border="0"
                        class="table table-striped table-bordered demo-tbl" id="documentosTb"
                        data-order='[[ 1, "asc" ]]' style="width: 100%;"></table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div> <!--page-content -->

  <!-- MODAL DOCUMENTOS APROVADOS -->
  <div class="hidden" id="documentosAprovadosModalHidden">
    <div id="documentosAprovadosModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Documentos Liberados</h4>
      </div>
      <div class="modal-body">
        <div class="panel panel-primary">
          <div class="panel-heading">
            <h4>Lista de Documentos Liberados</h4>
            <div class="options">
              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
            </div>

          </div>
          <div class="panel-body collapse in">
            <div id="grid-layout-table-2" class="box jplist">
              <div class="box text-shadow">
                <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl"
                  id="documentosAprovadosTb" data-order='[[ 1, "asc" ]]' style="width: 100%;"></table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="modal-footer">
      <button data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
    </div>
  </div>

  <!-- MODAL LISTA DE CLÁUSULAS POR DOC SIND -->
  <div class="hidden" id="documentoClausulaModalHidden">
    <div id="documentoClausulaModalContent">
      <div class="modal-header">
        <div>
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Adicionar Cláusula</h4>
        </div>

        <button type="button" id="enviar_email_selecionados_btn" class="btn btn-success btn-rounded"
          style="float: right; margin-top: 15px; margin-left: 10px; margin-right: 5px;">Reenviar e-mails</button>

        <button type="button" id="enviar_email_btn" data-id="" class="btn btn-success btn-rounded"
          style="float: right; margin-top: 15px; margin-left: 10px; margin-right: 5px;" data-dismiss="modal">Enviar
          E-mails</button>

        <button type="button" id="liberar_documento" data-id="" class="btn btn-success btn-rounded"
          style="float: right; margin-top: 15px;">Liberar
          Documento</button>
      </div>
      <div class="modal-body">
        <div class="panel panel-primary">
          <div class="panel-heading">
            <h4>Cláusulas atribuídas a este documento sindical</h4>
            <div class="options">
              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
            </div>
          </div>
          <div class="panel-body collapse in">
            <div
              style="display: flex; justify-content: flex-end; align-items: center; width: 100%; gap: 15px; margin-bottom: 20px;">
              <div>
                <button class="btn btn-success" id="btn_gerar_resumo_clausula">Gerar resumo da cláusula</button>
                <button class="btn btn-success" id="btn_extrair_relatorio">Extrair Relatório</button>
              </div>
            </div>
            <div id="grid-layout-table-clausulas" class="box jplist">
              <div class="box text-shadow">
                <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl"
                  id="clausulasTb" data-order='[[ 1, "asc" ]]' style="width: 100%;"></table>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
      </div>
    </div>
  </div>

  <!-- MODAL GRUPOS ECONOMICOS E EMPRESAS -->
  <button style="display: none" id="gruposEmpresasModalBtn" data-toggle="modal" data-target="#gruposEmpresas"></button>
  <div class="hidden modal_hidden" id="gruposEmpresasModalHidden">
    <div id="gruposEmpresasContent">
      <div class="modal-content">
        <div class="modal-header">
          <div style="display: flex; width: 100%; justify-content: space-between;">
            <h4 class="modal-title" id="infoSindModalTitle">Grupos e empresas deste documento sindical</h4>
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          </div>
        </div>

        <div class="modal-body">
          <form id="infoSindForm">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <div class="options">
                  <a href="javascript:;" class="panel-collapse" id="collapseAssociacoes"><i
                      class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in" id="collapseAssociacoesBody">
                <div class="form-group">
                  <div class="col-sm-6">
                    <label>Grupos Economicos</label>
                    <textarea class="col-sm-12" rows="10" id="doc-info-grupos-economicos" disabled></textarea>
                  </div>
                  <div class="col-sm-6">
                    <label>Empresas</label>
                    <textarea class="col-sm-12" rows="10" id="doc-info-empresas" disabled></textarea>
                  </div>
                </div>
                <div class="form-group">
                  <div class="col-sm-12" style="margin-top: 15px;">
                    <label>Abrangencia</label>
                    <textarea class="col-sm-12" rows="5" id="doc-info-abrangencia" disabled></textarea>
                  </div>
                </div>
                <div class="form-group">
                  <div class="col-sm-6" style="margin-top: 15px;">
                    <label>Siglas Laborais</label>
                    <textarea class="col-sm-12" rows="2" id="doc-info-siglas-laborais" disabled></textarea>
                  </div>
                  <div class="col-sm-6" style="margin-top: 15px;">
                    <label>Siglas Patronais</label>
                    <textarea class="col-sm-12" rows="2" id="doc-info-siglas-patronais" disabled></textarea>
                  </div>
                </div>
              </div>
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <div class="row">
            <div class="col-sm-12">
              <div class="btn-toolbar" style="display: flex; justify-content: flex-end;">
                <button type="button" data-toggle="modal" class="btn btn-danger btn-rounded btn-cancelar"
                  data-dismiss="modal">Fechar</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="modal-footer">
      <button data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
    </div>
  </div>

  <!-- INSERIR CLÁUSULA -->
  <div class="hidden" id="clausulaModalHidden">
    <div class="modal-content" id="clausulaModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Adicionar Cláusula</h4>
      </div>
      <div class="modal-body">
        <form class="form-horizontal">
          <div class="panel panel-primary">
            <div class="panel-heading">
              <h4>Classificação de Cláusula</h4>
              <div class="options">
                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
              </div>
            </div>
            <div class="panel-body collapse in principal-table">
              <div class="form-group center">
                <div class="col-sm-3">
                  <label for="info-inputc" class="control-label">Documento Sindical</label>
                  <select class="js-example-basic-single select2" id="documento_sindical"></select>
                </div>

                <div class="col-sm-3">
                  <label for="info-inputc" class="control-label select2">Classificação da Cláusula</label>
                  <select class="js-example-basic-single" id="lista_clausula"></select>
                </div>

                <div class="col-sm-3">
                  <label for="info-inputc" class="control-label">Sinônimo</label>
                  <select id="sinonimo_select" class="js-example-basic-single"></select>
                </div>

                <div class="col-sm-3">
                  <label for="numero" class="control-label">Número da Cláusula</label>
                  <input type="number" class="form-control" id="numero">
                </div>
              </div>
              <div class="form-group">
                <div class="col-sm-2">
                  <label for="vigencia_inicial" class="control-label">Vigência Inicial</label>
                  <input type="text" class="form-control" id="vigencia_inicial" disabled>
                </div>

                <div class="col-sm-2">
                  <label for="vigencia_final" class="control-label">Vigência Final</label>
                  <input type="text" class="form-control" id="vigencia_final" disabled>
                </div>
              </div>

              <div class="form-group center">
                <div class="col-sm-12">
                  <div
                    style="display: flex; justify-content: space-between; margin-bottom: 10px; align-items: flex-end;">
                    <label for="texto" class="control-label" style="line-height: 0;">Cláusula</label>

                    <button type="button" id="editar_texto" class="btn btn-success btn-rounded">Editar Texto</button>
                  </div>
                  <textarea class="form-control" id="texto" cols="30" rows="30"
                    style="max-height: 55vh; resize:none;"></textarea>
                </div>
              </div>
            </div>
          </div>

          <div class="form-group">
            <div class="panel panel-primary" style="margin:0 10px ;">
              <div class="panel-heading rounded-bottom">
                <h4>Seleção de Informações Adicionais</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-up"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table">
                <div class="row">
                  <div class="col-lg-6 scroll-none">
                    <table class="table table-striped">
                      <thead>
                        <th>Seleção</th>
                        <th>Informação Adicional</th>
                      </thead>
                      <tbody id="infoAdicionalSelect"></tbody>
                    </table>
                  </div>

                  <div class="col-lg-6 scroll-none">
                    <table class="table table-striped">
                      <thead>
                        <th>Preencher Informações Adicionais</th>
                        <th></th>
                        <th></th>
                      </thead>
                      <tbody id="infoAdicionalList">
                        <tr id="placeholder" style="color:#bbb ;">
                          <td>Selecione a classificação da cláusula e as informações desejadas.</td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="form-group" id="table-grupo-add">
            <div class="panel panel-primary" style="margin:0 10px ;">
              <div class="panel-heading">
                <h4>Informação Grupo</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in principal-table fixTableHead">
                <table class="table scroll-true" id="table_grupo"></table>
                <div style="float: left;">
                  <input type="text" style="width: 0; height: 0; background: transparent; border: none;"
                    id="focus_input">
                  <button id="remove_info_grupo" type="button" class="btn btn-danger"><i class="fa fa-minus"></i>
                    Remover Linha</button>
                  <button id="add_info_grupo" type="button" class="btn btn-primary"><i class="fa fa-plus"></i>
                    Adicionar
                    Linha</button>
                </div>
              </div>
            </div>
          </div>
        </form>
      </div>
      <div class="modal-footer">
        <div class="row">
          <div class="col-lg-12">
            <div class="btn-toolbar" style="display: flex; justify-content:center; gap: 10px;">
              <button type="button" id="btn_add_clausula" class="btn btn-primary btn-rounded"
                style="margin-right: 2.5px; margin-left: 0;">Salvar</button>
              <button type="button" id="btn_aprovar_clausula" class="btn btn-primary btn-rounded"
                style="background: green; color: white;">Aprovar</button>
            </div>
          </div>
          <button type="button" class="btn btn-primary" data-dismiss="modal" data-toggle="modal"
            id="voltar_clausulas_documento_btn">voltar</button>
        </div>
      </div>
    </div>
  </div>

  <!-- RESUMO CLÁUSULA -->
  <button style="display: none" id="resumoModalBtn" data-toggle="modal" data-target="#resumoModal"></button>
  <div class="hidden" id="resumoModalHidden">
    <div class="modal-content" id="resumoModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Resumo Cláusula</h4>
      </div>
      <div class="modal-body">
        <form class="form-horizontal">
          <div class="panel panel-primary">
            <div class="panel-heading">
              <h4>Classificação de Cláusula</h4>
              <div class="options">
                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
              </div>
            </div>
            <div class="panel-body collapse in principal-table">
              <div class="form-group center">
                <div class="col-sm-3">
                  <label for="info-inputc" class="control-label">Documento Sindical</label>
                  <input type="text" class="form-control" id="documento_sindical_resumo" disabled>
                </div>

                <div class="col-sm-3">
                  <label for="info-inputc" class="control-label select2">Classificação da Cláusula</label>
                  <input type="text" class="form-control" id="nome_clausula_resumo" disabled>
                </div>

                <div class="col-sm-3">
                  <label for="info-inputc" class="control-label">Sinônimo</label>
                  <input type="text" class="form-control" id="sinonimo_resumo" disabled>
                </div>

                <div class="col-sm-3">
                  <label for="numero" class="control-label">Número da Cláusula</label>
                  <input type="text" class="form-control" id="numero_resumo" disabled>
                </div>
              </div>
              <div class="form-group">
                <div class="col-sm-2">
                  <label for="vigencia_inicial_resumo" class="control-label">Vigência Inicial</label>
                  <input type="text" class="form-control" id="vigencia_inicial_resumo" disabled>
                </div>

                <div class="col-sm-2">
                  <label for="vigencia_final_resumo" class="control-label">Vigência Final</label>
                  <input type="text" class="form-control" id="vigencia_final_resumo" disabled>
                </div>
              </div>

              <div class="form-group center">
                <div class="row">
                  <div class="col-sm-12">
                    <button type="button" id="editar_texto_resumo" class="btn btn-success btn-rounded"
                      style="float: right;">Editar
                      Texto</button>
                  </div>
                  <div class="col-sm-6">
                    <label for="texto_original" class="control-label" style="line-height: 0;">Texto</label>
                    <textarea class="form-control" id="texto_original" cols="30" rows="30"
                      style="max-height: 55vh; resize:none;" disabled></textarea>
                  </div>
                  <div class="col-sm-6">
                    <label for="texto_resumo" class="control-label" style="line-height: 0;">Resumo</label>
                    <textarea class="form-control" id="texto_resumo" cols="30" rows="30"
                      style="max-height: 55vh; resize:none;"></textarea>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </form>
      </div>
      <div class="modal-footer">
        <div class="row">
          <div class="col-lg-12">
            <div class="btn-toolbar" style="display: flex; justify-content:center; gap: 10px;">
              <button type="button" id="btn_editar_resumo" class="btn btn-primary btn-rounded"
                style="margin-right: 2.5px; margin-left: 0;">Salvar</button>
            </div>
          </div>
          <button type="button" class="btn btn-primary" data-dismiss="modal" data-toggle="modal"
            id="voltar_resumo_clausulas_documento_btn">voltar</button>
        </div>
      </div>
    </div>
  </div>
  <!-- MODAL EMAILS APROVADOS -->
  <button type="button" class="hidden" data-toggle="modal" data-target="#emailsAprovadosModal"
    id="emails_aprovados_modal_btn"></button>
  <div class="hidden" id="emailsAprovadosModalHidden">
    <div id="emailsAprovadosModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Emails</h4>
      </div>
      <div class="modal-body">
        <div style="width: 100%; display: flex; justify-content: flex-end; align-items: center; margin-top: 20px;">
          <button id="enviar_emails_aprovados_btn" type="button" class="btn btn-primary">Enviar</button>
        </div>
        <div class="panel panel-primary">
          <div class="panel-heading">
            <h4>Selecione os emails</h4>
            <div class="options">
              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
            </div>
          </div>
          <div class="panel-body collapse in">
            <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
              <input type="checkbox" id="selecionar_todos_documentos_sindicais_btn">
              <label for="selectAll">Selecionar Todos</label>
            </div>
            <div id="grid-layout-table-2" class="box jplist">
              <div class="box text-shadow">
                <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl"
                  id="emailsTb" data-order='[[ 0, "asc" ]]' style="width: 100%;"></table>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-footer">
        <button data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
      </div>
    </div>
  </div>

  <?php include 'footer.php' ?>

  </div> <!--page-container -->

  <!-- MODELO DE MODAL -->
  <script type='text/javascript' src="./js/clausula.min.js"></script>
</body>

</html>