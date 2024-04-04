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

  <link rel="stylesheet" href="includes/css/styles.css">
  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>

  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />
  <link rel='stylesheet' type='text/css' href='formulario_comunicado.css' />
  <link rel="stylesheet" type='text/css' href="includes/css/styles.css" />

  <style>
    .select2-container {
      width: 100% !important;
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

    .style_check {
      display: flex;
      align-items: start;
      margin-top: 34px;
      justify-content: center;
      gap: 10px;
    }

    .table_doc_geral th,
    .table_doc_process th {
      text-align: center;
      background-color: #4f8edc;
      color: #fff;
    }

    .table_doc_geral td:nth-child(2),
    .table_doc_process td:nth-child(1) {
      text-align: center;
    }

    .img_box {
      position: absolute;
      z-index: 999;
      width: 100%;
      height: 100%;
      background-color: rgba(255, 255, 255, 0.7);
      display: none;
    }

    .img_load {
      position: absolute;
      top: 30%;
      right: 45%;
    }


    form .row {
      padding: 0px 10px;
    }

    #page-content {
      min-height: 100% !important;
    }

    .hide {
      display: none;
    }

    .table-head {
      white-space: nowrap;
      color: white;
      background-color: #4f8edc;
    }

    .form-control-title {
      margin-bottom: 0;
      background-color: #4f8edc;
      color: #fff;
      font-weight: 700;
      text-align: center;
      margin-top: 20px;
      font-size: 18px;
      margin-bottom: 10px;
      padding: 10px 0;
    }

    .box-buttons {
      margin: 20px 0;
      display: flex;
      gap: 15px;
    }

    .observacoes-adicionais {
      padding: 0;
      border-radius: 0;
    }
  </style>
</head>

<body class="horizontal-nav hide">

  <?php include('menu.php'); ?>

  <div id="pageCtn" style="min-height: 100%;">
    <div class="img_box">
      <img class="img_load" src="includes/img/loading.gif">
    </div>
    <!-- <div> -->

    <!-- MODAL INFO SINDICATOS -->
    <button style="display: none" id="openInfoSindModalBtn" data-toggle="modal" data-target="#infoSindModal"></button>
    <div class="hidden" id="infoSindModalHidden">
      <div id="infoSindModalHiddenContent">
        <div class="modal-content">
          <div class="modal-header">
            <div style="display: flex; width: 100%; justify-content: space-between;">
              <h4 class="modal-title" id="infoSindModalTitle">Informações Sindicais</h4>
              <div class="dropdown" style="margin-left: 50%;">
                <button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu2" data-toggle="dropdown"
                  aria-haspopup="true" aria-expanded="false">
                  Módulos <i class="fa fa-th"></i>
                </button>
                <ul class="dropdown-menu" aria-labelledby="dropdownMenu2">
                  <li><a href="#" id="direct-clausulas-btn">Consultar Cláusulas</a></li>
                  <li><a href="#" id="direct-comparativo-btn">Comparar Cláusulas</a></li>
                  <li><a href="#" id="direct-calendarios-btn">Calendário Sindical</a></li>
                  <li><a href="#" id="direct-documentos-btn">Consulta de documentos</a></li>
                  <li><a href="#" id="direct-gerar-excel-btn">Mapa Sindical (Excel)</a></li>
                  <li><a href="#" id="direct-formulario-aplicacao-btn">Mapa sindical (Formulário Aplicação)</a></li>
                  <li><a href="#" id="direct-comparativo-mapa-btn">Mapa sindical (Comparativo)</a></li>
                  <li><a href="#" id="direct-relatorio-negociacoes-btn">Negociação (Acompanhamento CCT Ineditta)</a>
                  </li>
                </ul>
              </div>
              <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            </div>
          </div>

          <div class="modal-body">
            <form id="infoSindForm">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Dados cadastrais</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse" id="collapseDadosCadastrais"><i
                        class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in" id="collapseDadosCadastraisBody">
                  <div class="form-group">
                    <div class="col-sm-3">
                      <label for="info-sigla">Sigla</label>
                      <input class="col-sm-9 form-control" type="text" id="info-sigla" disabled>
                    </div>

                    <div class="col-sm-3">
                      <label for="info-cnpj">CNPJ</label>
                      <input class="col-sm-9 form-control" type="text" id="info-cnpj" disabled>
                    </div>

                    <div class="col-sm-6">
                      <label for="info-razao">Razão Social</label>
                      <input class="col-sm-8 form-control" type="text" id="info-razao" disabled>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-denominacao">Denominação</label>
                      <input class="col-sm-9 form-control" type="text" id="info-denominacao" disabled>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-cod-sindical">Código Sindical</label>
                      <input class="col-sm-8 form-control" type="text" id="info-cod-sindical" disabled>
                    </div>
                  </div>
                </div>
              </div>

              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Localização</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse" id="collapseLocalizacao"><i
                        class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in" id="collapseLocalizacaoBody">
                  <div class="form-group">
                    <div class="col-sm-2">
                      <label for="info-uf">UF</label>
                      <input class="col-sm-8 form-control" type="text" id="info-uf" disabled>
                    </div>

                    <div class="col-sm-3">
                      <label for="info-municipio">Município</label>
                      <input class="col-sm-9 form-control" type="text" id="info-municipio" disabled>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-logradouro">Logradouro</label>
                      <input class="col-sm-9 form-control" type="text" id="info-logradouro" disabled>
                    </div>
                  </div>
                </div>
              </div>

              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Informações de Contato</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse" id="collapseContato"><i
                        class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in" id="collapseContatoBody">
                  <div class="form-group">
                    <div class="col-sm-3">
                      <label for="info-telefone1">Telefone</label>
                      <input class="col-sm-8 form-control" type="text" id="info-telefone1" disabled>
                    </div>

                    <div class="col-sm-3">
                      <label for="info-telefone2">Telefone 2</label>
                      <input class="col-sm-8 form-control" type="text" id="info-telefone2" disabled>
                    </div>

                    <div class="col-sm-3">
                      <label for="info-telefone3">Telefone 3</label>
                      <input class="col-sm-8 form-control" type="text" id="info-telefone3" disabled>
                    </div>

                    <div class="col-sm-3">
                      <label for="info-ramal">Ramal</label>
                      <input class="col-sm-9 form-control" type="text" id="info-ramal" disabled>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-enquadramento">Contato Enquadramento</label>
                      <input class="col-sm-8 form-control" type="text" id="info-enquadramento" disabled>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-negociador">Contato Negociador</label>
                      <input class="col-sm-8 form-control" type="text" id="info-negociador" disabled>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-contribuicao">Contato Contribuição</label>
                      <input class="col-sm-8 form-control" type="text" id="info-contribuicao" disabled>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-email1">Email</label>
                      <a id="info-email1-link" style="display: flex;" target="_blank">
                        <input class="col-sm-9 form-control" type="text" id="info-email1" readonly>
                      </a>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-email2">Email 2</label>
                      <a id="info-email2-link" style="display: flex;" target="_blank">
                        <input class="col-sm-9 form-control" type="text" id="info-email2" readonly>
                      </a>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-email3">Email 3</label>
                      <a id="info-email3-link" style="display: flex;" target="_blank">
                        <input class="col-sm-9 form-control" type="text" id="info-email3" readonly>
                      </a>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-twitter">Twitter</label>
                      <a id="info-twitter-link" style="display: flex;" target="_blank">
                        <input class="col-sm-9 form-control" type="text" id="info-twitter" readonly>
                      </a>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-facebook">Facebook</label>
                      <a id="info-facebook-link" style="display: flex;" target="_blank">
                        <input class="col-sm-9 form-control" type="text" id="info-facebook" readonly>
                      </a>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-instagram">Instagram</label>
                      <a id="info-instagram-link" style="display: flex;" target="_blank">
                        <input class="col-sm-9 form-control" type="text" id="info-instagram" readonly>
                      </a>
                    </div>

                    <div class="col-sm-4">
                      <label for="info-site">Site</label>
                      <a id="info-site-link" style="display: flex;" target="_blank">
                        <input class="col-sm-9 form-control" type="text" id="info-site" readonly>
                      </a>
                    </div>
                  </div>
                </div>
              </div>

              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Associações</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse" id="collapseAssociacoes"><i
                        class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in" id="collapseAssociacoesBody">
                  <div class="form-group">
                    <div class="col-sm-6">
                      <label for="info-federacao-nome">Nome Federação</label>
                      <input class="col-sm-9 form-control" type="text" id="info-federacao-nome" disabled>
                    </div>

                    <div class="col-sm-6">
                      <label for="info-federacao-cnpj">CNPJ Federação</label>
                      <input class="col-sm-9 form-control" type="text" id="info-federacao-cnpj" disabled>
                    </div>

                    <div class="col-sm-6">
                      <label for="info-confederacao-nome">Nome Confederação</label>
                      <input class="col-sm-9 form-control" type="text" id="info-confederacao-nome" disabled>
                    </div>

                    <div class="col-sm-6">
                      <label for="info-confederacao-cnpj">CNPJ Confederação</label>
                      <input class="col-sm-9 form-control" type="text" id="info-confederacao-cnpj" disabled>
                    </div>

                    <div class="col-sm-6">
                      <label for="info-central-sind-nome">Nome Central Sindical</label>
                      <input class="col-sm-9 form-control" type="text" id="info-central-sind-nome" disabled>
                    </div>

                    <div class="col-sm-6">
                      <label for="info-central-sind-cnpj">CNPJ Central Sindical</label>
                      <input class="col-sm-9 form-control" type="text" id="info-central-sind-cnpj" disabled>
                    </div>
                  </div>
                </div>
              </div>

              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Diretoria</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse" id="collapseDiretoria"><i
                        class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in" id="collapseDiretoriaBody">
                  <div class="box text-shadow">
                    <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                      class="table table-striped table-bordered demo-tbl" id="diretoriainfosindtb"
                      data-order='[[ 1, "asc" ]]'>
                    </table>
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
                    data-dismiss="modal">Voltar</button>
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

    <div id="wrap">
      <div class="container">
        <div class="row" style="display: flex;">
          <div class="col-md-12">
            <form class="form-horizontal">
              <div class="panel panel-primary" style="margin: 7px 0px 20px 0px;">
                <div class="panel-heading">
                  <h4>Formulário aplicação</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">

                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-3">
                        <label>Grupo Econômico</label>
                        <select data-placeholder="Nome" class="form-control select2" id="grupo">
                        </select>
                      </div>

                      <div class="col-sm-3">
                        <label>Empresa</label>
                        <select multiple data-placeholder="Nome, Código" class="form-control select2" id="matriz">
                        </select>
                      </div>

                      <div class="col-sm-6">
                        <label>Estabelecimento</label>
                        <select multiple
                          data-placeholder="Nome, CNPJ, Código Cliente, Código Sindicato Cliente, Regional" tabindex="8"
                          class="form-control select2" id="unidade">
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-5">
                        <label>Localidade do Estabelecimento</label>
                        <select multiple data-placeholder="Região, UF ou Município" class="form-control select2"
                          id="localidade">
                        </select>
                      </div>

                      <div class="col-sm-4">
                        <label>Atividade Econômica</label>
                        <select multiple data-placeholder="CNAE" class="form-control select2" id="categoria">
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-5">
                        <label id="label-sindicato" for="">Sindicato Laboral</label>
                        <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                          class="form-control select2" id="sindicatoLaboral">
                        </select>
                      </div>

                      <div class="col-sm-5">
                        <label id="label-sindicato" for="">Sindicato Patronal</label>
                        <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                          class="form-control select2" id="sindicatoPatronal">
                        </select>
                      </div>

                      <div class="col-sm-2">
                        <label>Data-base</label>
                        <select multiple data-placeholder="Mês/Ano" tabindex="8" class="form-control select2"
                          id="dataBase">
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="form-group">
                    <div class="row">
                      <div class="col-sm-4">
                        <label for="tipo_doc">Nome do Documento</label>
                        <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                          id="nome_doc">
                        </select>
                      </div>

                      <div class="col-sm-4">
                        <label for="tipo_doc">Grupo Cláusulas</label>
                        <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                          id="grupo_clausulas">
                        </select>
                      </div>

                      <div class="col-sm-4">
                        <label for="tipo_doc">Seleção de Cláusulas</label>
                        <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                          id="clausulaList"></select>
                      </div>

                    </div>
                  </div>

                  <div class="form-group">
                    <div class="col-sm-3">
                      <label>Data do Processamento Ineditta</label>
                      <div class="input-group-prepend">
                        <span class="input-group-text">
                          <i class="far fa-calendar-alt"></i>
                        </span>
                      </div>
                      <input type="text" class="form-control float-right date_format" id="processamento" placeholder="dd/mm/aaaa - dd/mm/aaaa">
                    </div>
                  </div>

                  <div class="form-group" style="margin-bottom: 0;">
                    <div class="col-sm-12" style="display: flex; gap: 3px; justify-content: left;">
                      <button type="button" id="filtrarBtn" class="btn btn-primary btn-rounded"><i class="fa fa-search"
                          style="margin-right: 10px;"></i> Filtrar</button>
                      <button type="button" id="limparBtn" class="btn btn-primary btn-rounded">Limpar Filtro</button>
                    </div>
                  </div>
                </div>

              </div>
            </form>
          </div>
        </div>
        <div class="row">
          <div class="col-md-12" id="exibirDocumentosDiv">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h4>Lista de documentos processados</h4>
                <div class="options">
                  <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                </div>
              </div>
              <div class="panel-body collapse in">
                <table style="width: 100%;" class="table table-striped table-bordered" id="documentoTb">
                  <thead>
                    <th></th>
                    <th>Nome documento</th>
                    <th>Atividade econômica</th>
                    <th>Sindicato Laboral</th>
                    <th>Sindicato Patronal</th>
                    <th>Processamento Ineditta</th>
                    <th>Vigência</th>
                    <th>Ver</th>
                    <th>Baixar</th>
                  </thead>
                  <tbody>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
        <!--page-content -->
      </div>
      <!--page-container -->
    </div>
    <button type="button" class="hidden" data-toggle="modal" data-target="#documentoModal"
      id="documentoModalBtn">Adicionar Cláusula</button>

    <div class="hidden" id="documentoModalHidden">
      <div id="documentoModalContent">
        <div class="modal-header" id="hdr">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Formulário de aplicação</h4>
          <div style="width: 100%; display: flex; gap: 20px; font-size: 16px; margin-top: 10px;">
            <div style="display: flex; gap: 6px;">
              <b id="documento_status_b">Documento: </b>
              <p id="documento_status_p"></p>
            </div>
            <div style="display: flex; gap: 6px;">
              <b id="aprovado_b">Status: </b>
              <p id="aprovado_p"></p>
            </div>
            <div style="display: flex; gap: 6px;">
              <b id="nome_alterador_b">Alterado por: </b>
              <p id="nome_alterador_p"></p>
            </div>
            <div style="display: flex; gap: 6px;">
              <b id="data_alteracao_b">Data da alteração: </b>
              <p id="data_alteracao_p"></p>
            </div>
          </div>
        </div>
        <div class="modal-body">
          <div style="margin: 20px 0; width: 100%;">
            <label for="orientacoes" style="display: none;">Orientações</label>
            <textarea id="orientacoes" rows="10" placeholder="Orientações para esse documento"
              style="width: 100%; padding: 10px; border-radius: 3px; border: 1px solid gray; margin-top: 5px; display: none;"></textarea>
          </div>
          <div id="informacoes_adicionais"></div>
          <div style="margin-top: 20px; width: 100%;">
            <label for="outras-informacoes">Outras informações</label>
            <textarea id="outras-informacoes" rows="10" placeholder="Outras informações para esse documento"
              style="width: 100%; padding: 10px; border-radius: 3px; border: 1px solid gray; margin-top: 5px;"
              data-placeholder=""></textarea>
          </div>
          <div class="box-buttons">
            <button id="salvar_btn" class="btn btn-primary btn-rounded">Salvar</button>
            <button id="aprovar_btn" class="btn btn-primary btn-rounded">Salvar e Aprovar</button>
            <button id="baixar_exel_btn" class="btn btn-primary btn-rounded">Baixar Excel</button>
          </div>
          <a href="#hdr" id="asd" style="opacity: 0;">asdsad</a>
        </div>
      </div>
    </div>
  </div>


  <?php include 'footer.php' ?>

  <script type='text/javascript' src="./js/formulario_comunicado.min.js"></script>
</body>

</html>