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

  <link rel="stylesheet" href="calendario_sindical.css">
  <link rel="stylesheet" href="includes/css/styles.css">

  <style>
    body.horizontal-nav{
      padding-right: 0 !important;
    }

    .texto-clausula {
      text-align: justify; 
      white-space: pre-line;
    }

    .chosen-container-multi .chosen-choices {
      display: flex;
      flex-wrap: wrap;
      gap: 5px;
      min-height: 34px !important;
    }

    .hide-block {
      display: none !important;
    }

    .chosen-container-multi .chosen-choices li.search-choice {
      margin: 0px 0px 0px 5px;
    }

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

    /* #calendar {
      width: 70%;
      height: 80%;

    } */

    #eventTitle,
    #newEventTitle {
      padding: 20px 10px;
      font-size: 1.5em;

    }

    #btn-color,
    #new-btn-color {
      padding: 11px 20px;
    }

    .input-group-btn li button {
      background-color: transparent;
      border: none;
    }

    .cor1,
    .cor2,
    .cor3,
    .cor4,
    .cor5,
    .cor6 {
      height: 20px;
      width: 20px;
      border: none;
      border-radius: 10px;
    }

    .cor1 {
      background-color: #2973cf;
    }

    .cor2 {
      background-color: #85c744;
    }

    .cor3 {
      background-color: #2bbce0;
    }

    .cor4 {
      background-color: #f1c40f;
    }

    .cor5 {
      background-color: #e73c3c;
    }

    .cor6 {
      background-color: #76c4ed;
    }

    .dropdown-menu-color {
      display: flex;
    }

    .timeline-box {
      height: 100vh;
      overflow-y: scroll;
    }

    .fc .fc-daygrid-day-top {
      flex-direction: row !important;
    }

    table td[class*="col-"],
    table th[class*="col-"] {
      background: #4f8edc;
      padding: 10px 0px;
      color: #fff;
    }

    .fc td,
    .fc th {
      border: 1px solid #4f8edc;
    }

    .modal-md {
      width: 800px;
    }

    .itens .row {
      margin-top: 16px;
    }

    #pageCtn {
      background: #EAEBEC;
      min-height: calc(100vh - 110px);
    }

    #page-content {
      min-height: 100% !important;
    }

    .hide{
      display: none;
    }

    .modal-dialog {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
    }

    #novoEventoModal-content {
      width: 400px;
    }

    #novoEventoModal-content > .modal-body {
      padding: 20px;
      font-size: 16px;
    }

    #novoEventoModal-content > .modal-body > .row {
      margin-bottom: 12px;
    }

    .flex-row-center {
      display: flex;
      justify-content: center;
      align-items: center;
    }

    .visivel-checkbox {
      width: 18px;
      margin: 0px 0px 0px 4px !important;
    }

    .j-left {
      justify-content: left;
    }

    .w-full {
      width: 100%;
    }

    .btn-view-grid {
      border: none;
      color: #4f8edc;
      background-color: inherit;
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

    .observacoes-adicionais {
      padding: 0;
      border-radius: 0;
    }

    #clausulaModal-content {
      height: 90vh;
      max-width: 1200px;
      overflow-y: scroll;
      overflow-x: hidden;
    }

    #vencimentoDocumentoModal-content, #trintidioModal-content {
      max-height: 90vh;
      overflow-y: scroll;
      overflow-x: hidden;
    }

    #clausulaModal-content  .modal-body {
      padding-bottom: 24px !important;
    }

    .cursor-pointer {
      cursor: pointer;
    }

  </style>
</head>

<body class="horizontal-nav hide">

  <?php include('menu.php'); ?>

  <div id="pageCtn" class="page-container">
    <input type="hidden" id="sind-id-input">
    <input type="hidden" id="tipo-sind-input">
    <input type="hidden" id="diretoria-id-input">
    <input type="hidden" id="unidade-id-input">
    <input type="hidden" id="nomeunidade-input">
   
    <div id="page-content">
      <div id="wrap">
        <div class="container">
          <div class="row">
            <div class="col-md-12">
              <!-- TELA INICICAL -->
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4>Calendário Sindical</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <form id="form-filtros">
                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-3">
                          <label>Tipo Evento</label>
                          <select multiple data-placeholder="Tipo Evento" class="form-control select2" id="tipo_evento">
                          </select>
                        </div>

                        <div class="col-sm-3">
                          <label>Período</label>
                          <div class="input-group-prepend">
                            <span class="input-group-text">
                            </span>
                          </div>
                          <input type="text" class="form-control float-right" id="reservation" placeholder="dd/mm/aaaa - dd/mm/aaaa">
                        </div>
                      </div>
                    </div>

                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-3">
                          <label>Grupo Econômico</label>
                          <select multiple data-placeholder="Nome" class="form-control select2" id="grupo">
                          </select>
                        </div>
                        <div class="col-sm-4">
                          <label>Empresa</label>
                          <select multiple data-placeholder="Nome, Código" class="form-control select2" id="matriz">
                          </select>
                        </div>
                        <div class="col-sm-5">
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
                        <div class="col-sm-4">
                          <label for="localidade">Localidade do documento</label>
                          <select multiple data-placeholder="Município, UF" id="localidade" class="form-control select2"></select>
                        </div>
                        <div class="col-sm-3">
                          <label for="nome_doc">Nome do Documento</label>
                          <select multiple data-placeholder="Nome do Documento" id="nome_doc" class="form-control select2"></select>
                        </div>
                        <div class="col-sm-3">
                          <label for="cnae">Atividade Econômica</label>
                          <select multiple data-placeholder="Atividade Econômica" id="cnae" class="form-control select2"></select>
                        </div>
                        <div class="col-sm-2">
                          <label for="origem">Origem</label>
                          <select id="origem" data-placeholder="Origem" class="form-control select2"></select>
                        </div>
                      </div>
                    </div>

                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-6">
                          <label id="label-sindicato" for="sindicatoLaboral">Sindicato Laboral</label>
                          <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sindicatoLaboral">
                          </select>
                        </div>

                        <div class="col-sm-6">
                          <label id="label-sindicato" for="sindicatoPatronal">Sindicato Patronal</label>
                          <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sindicatoPatronal">
                          </select>
                        </div>
                      </div>
                    </div>

                    <div class="form-group hide-block" >
                      <div class="row">
                        <div class="col-sm-5">
                          <label for="tipo_doc">Grupo Assunto</label>
                          <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2" id="grupo_clausulas"></select>
                        </div>
                        <div class="col-sm-7">
                          <label>Assuntos</label>
                          <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="assuntos">
                          </select>
                        </div>
                      </div>
                    </div>

                    <div class="form-group">
                      <div class="col-sm-3" style="display: flex; justify-content: left; padding-left: 0; gap: 3px;">
                        <button type="button" id="btnFiltrar" class="btn btn-primary"><i class="fa fa-search" style="margin-right: 10px;"></i>Filtrar</button>
                        <button id="btnLimparFiltro" type="button" onclick="gerarTabela();" class="btn btn-primary">Limpar Filtro</button>
                      </div>

                      <div class="col-sm-9" style="display: flex; justify-content: right; padding-right: 0; gap: 3px;">
                        <button id="btnAdicionarEvento" type="button" class="btn btn-primary"
                          data-toggle="modal" data-target="#novoEventoModal">Adicionar Evento</button>
                      </div>
                    </div>
                  </form>
                </div>
              </div>
              
              <!-- AGENDA DE EVENTOS -->
              <div id="agenda_eventos_panel" class="panel panel-primary" style="margin-top: 30px;">
                <div class="panel-heading">
                  <h4>Agenda de Eventos</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row hide-block">
                    <div class="col-sm-4" style="float: right; display: flex; justify-content: right;">
                      <button style="margin-bottom: 20px;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Evento</button>
                      <button style="margin-bottom: 20px; margin-left: 0.6em;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Tarefa</button>
                    </div>
                  </div>

                  <div class="row">
                    <div class="col-lg-12">
                      <table style="width: 100%;" class="table table-striped table-bordered" id="agendaEventosTb" data-order='[[ 0, "desc" ]]'>
                        <thead>
                          <th>Data e Hora</th>
                          <th>Título</th>
                          <th>Recorrência</th>
                          <th>Validade recorrência</th>
                          <th>Local</th>
                          <th>Comentários</th>
                          <th>Visível</th>
                        </thead>
                        <tbody>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>

              <!-- ASSEMBLÉIA OU REUNIÃO -->
              <div id="assembleia_reuniao_panel" class="panel panel-primary hide" style="margin-top: 30px;">
                <div class="panel-heading">
                  <h4>Assembleia ou Reunião</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row hide-block">
                    <div class="col-sm-4" style="float: right; display: flex; justify-content: right;">
                      <button style="margin-bottom: 20px;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Evento</button>
                      <button style="margin-bottom: 20px; margin-left: 0.6em;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Tarefa</button>
                    </div>
                  </div>

                  <div class="row">
                    <div class="col-lg-12">
                      <table style="width: 100%;" class="table table-striped table-bordered" id="assembleiaReuniaoTb" data-order='[[ 0, "desc" ]]'>
                        <thead>
                          <th>Data</th>
                          <th>Nome do Evento</th>
                          <th>Sindicatos Laborais</th>
                          <th>Sindicatos Patronais</th>
                          <th>Atividades Econômicas</th>
                          <th>Data-base da negociação</th>  
                          <th>Nome do documento</th>
                          <th>Fase da negociação</th>
                          <th></th>
                        </thead>
                        <tbody>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>

              <!-- MODAL DETALHES ASSEMBLÉIA -->
              <button type="button" class="hidden" data-toggle="modal"
                data-target="#assembleiaModal" id="assembleiaBtn"></button>
              <div class="hidden" id="assembleiaModalHidden">
                <div id="assembleiaModalContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Detalhes da Assembléia Patronal</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading" id="textoClausulaPanel">
                        <h4>Informações da Assembléia Patronal</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <input type="hidden" class="form-control" id="id_documento_detalhes_assembleia" placeholder="">   
                        <p id="tipo_assembleia_patronal" class="texto-clausula"></p>
                        <p id="data_hora_assembleia_patronal" class="texto-clausula"></p>
                        <p id="endereco_assembleia_patronal" class="texto-clausula"></p>
                        <p id="comentario_assembleia_patronal" class="texto-clausula"></p>
                      </div>
                    </div>
                  </div>

                  <div class="modal-footer">
                    <button data-toggle="modal" type="button" class="btn btn-secondary"
                      data-dismiss="modal">Voltar</button>
                  </div>
                </div>
              </div>

              <!-- MODAL DETALHES REUNIÃO -->
              <button type="button" class="hidden" data-toggle="modal"
                data-target="#reuniaoModal" id="reuniaoBtn"></button>
              <div class="hidden" id="reuniaoModalHidden">
                <div id="reuniaoModalContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Detalhes da Reunião Sindical</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading">
                        <h4>Informações da Reunião Sindical</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <input type="hidden" class="form-control" id="id_documento_detalhes_reuniao" placeholder="">   
                        <p id="tipo_reuniao" class="texto-clausula"></p>
                        <p id="data_hora_reuniao" class="texto-clausula"></p>
                      </div>
                    </div>
                  </div>

                  <div class="modal-footer">
                    <button data-toggle="modal" type="button" class="btn btn-secondary"
                      data-dismiss="modal">Voltar</button>
                  </div>
                </div>
              </div>

              <!-- TRINTÍDIOS-->
              <div id="trintidio_panel" class="panel panel-primary" style="margin-top: 30px;">
                <div class="panel-heading">
                  <h4>Indenização adicional que antecede a data-base (Trintídio)</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row hide-block">
                    <div class="col-sm-4" style="float: right; display: flex; justify-content: right;">
                      <button style="margin-bottom: 20px;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Evento</button>
                      <button style="margin-bottom: 20px; margin-left: 0.6em;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Tarefa</button>
                    </div>
                  </div>

                  <div class="row">
                    <div class="col-lg-12">
                      <table style="width: 100%;" class="table table-striped table-bordered" id="trintidioTb" data-order='[[ 0, "desc" ]]'>
                        <thead>
                          <th>Data Início</th>
                          <th>Nome do Documento</th>
                          <th>Atividades Econômicas</th>
                          <th>Sindicatos Laborais</th>
                          <th>Sindicatos Patronais</th>
                          <th>Origem</th>
                          <th>Data-base</th>
                          <th>Vigência do evento</th>
                          <th></th>
                        </thead>
                        <tbody>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>

              <!-- MODAL DETALHES TRINTIDIO -->
              <button type="button" class="hidden" data-toggle="modal"
                data-target="#trintidioModal" id="trintidioBtn"></button>
              <div class="hidden" id="trintidioModalHidden">
                <div id="trintidioModalContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Detalhes - Indenização adicional que antecede a data-base (Trintídio)</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading" id="textoClausulaPanel">
                        <h4>Informações do Documento</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <input type="hidden" class="form-control" id="id_documento_detalhes_trintidio" placeholder="">   
                        <p id="nome_documento_detalhes_trintidio" class="texto-clausula"></p>
                        <p id="vigencia_inicial_detalhes_trintidio" class="texto-clausula"></p>
                        <p id="vigencia_final_detalhes_trintidio" class="texto-clausula"></p>
                        <p id="abrangencia_detalhes_trintidio" class="texto-clausula"></p>
                        <p id="estabelecimento_detalhes_trintidio" class="texto-clausula"></p>
                      </div>
                    </div>
                  </div>

                  <div class="modal-footer">
                    <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
                      data-dismiss="modal">Voltar</button>
                  </div>
                </div>
              </div>

              <!-- EVENTOS DE CLÁUSULAS-->
              <div id="eventos_clausulas_panel" class="panel panel-primary" style="margin-top: 30px;">
                <div class="panel-heading">
                  <h4>Obrigações sindicais das cláusulas</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row hide-block">
                    <div class="col-sm-4" style="float: right; display: flex; justify-content: right;">
                      <button style="margin-bottom: 20px;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Evento</button>
                      <button style="margin-bottom: 20px; margin-left: 0.6em;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Tarefa</button>
                    </div>
                  </div>

                  <div class="row">
                    <div class="col-lg-12">
                      <table style="width: 100%;" class="table table-striped table-bordered" id="eventosDeClausulasTb" data-order='[[ 0, "desc" ]]'>
                        <thead>
                          <th>Data</th>
                          <th>Nome do Documento</th>
                          <th>Atividades Econômicas</th>
                          <th>Sindicatos Laborais</th>
                          <th>Sindicatos Patronais</th>
                          <th>Nome Evento</th>
                          <th>Grupo Cláusulas</th>
                          <th>Nome Cláusula</th>
                          <th>Origem</th>
                          <th></th>
                        </thead>
                        <tbody>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>

              <!-- MODAL EXIBIÇÃO CLAUSULA -->
              <div class="hidden modal_hidden" id="clausulaModalHidden">
                <div id="clausulaModalHiddenContent">
                  <div class="modal-content">
                    <div class="modal-header" id="detalhes-clausula-modal-header">
                      <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                      <h4 class="modal-title">Exibição das Cláusulas</h4>
                    </div>
                    <div class="modal-body">
                      <div id="clausulaModalContainer"></div>

                      <div id="informacoes_adicionais"></div>

                      <button style="display: none" id="openCommentModalBtn" data-toggle="modal" data-target="#comentarioModal"></button>
                    </div>
                  </div><!-- /.modal-content -->
                </div><!-- /.modal-dialog -->
              </div><!-- /.modal -->

              <!-- TELA DE VISUALIZAÇÃO VENCIMENTOS DE DOCUMENTOS -->
              <div id="calendario" class="panel panel-primary" style="margin-top: 30px;">
                <div class="panel-heading">
                  <h4>Vencimentos de Documentos</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row hide-block">
                    <div class="col-sm-4" style="float: right; display: flex; justify-content: right;">
                      <button style="margin-bottom: 20px;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Evento</button>
                      <button style="margin-bottom: 20px; margin-left: 0.6em;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Tarefa</button>
                    </div>
                  </div>

                  <div class="row">
                    <div class="col-lg-12">
                      <table style="width: 100%;" class="table table-striped table-bordered" id="eventosTb" data-order='[[ 0, "desc" ]]'>
                        <thead>
                          <th>Data</th>
                          <th>Nome Documento</th>
                          <th>Origem</th>
                          <th>Atividade Econômica</th>
                          <th>Sindicato Laboral</th>
                          <th>Sindicato Patronal</th>
                          <th></th>
                        </thead>
                        <tbody>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>

              <!-- MODAL DETALHES VENCIMENTO DOCUMENTOS -->
              <button type="button" class="hidden" data-toggle="modal"
                data-target="#vencimentoDocumentoModal" id="vencimentoDocumentoBtn"></button>
              <div class="hidden" id="vencimentoDocumentoModalHidden">
                <div id="vencimentoDocumentoModalContent">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Detalhes do Vencimento de Documento</h4>
                  </div>
                  <div class="modal-body">
                    <div class="panel panel-primary">
                      <div class="panel-heading" id="textoClausulaPanel">
                        <h4>Informações do Documento</h4>
                        <div class="options">
                          <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                        </div>
                      </div>
                      <div class="panel-body collapse in">
                        <input type="hidden" class="form-control" id="id_documento_detalhes" placeholder="">   
                        <p id="nome_documento_detalhes" class="texto-clausula"></p>
                        <p id="vigencia_inicial_detalhes" class="texto-clausula"></p>
                        <p id="vigencia_final_detalhes" class="texto-clausula"></p>
                        <p id="abrangencia_detalhes" class="texto-clausula"></p>
                        <p id="estabelecimento_detalhes" class="texto-clausula"></p>
                      </div>
                    </div>
                  </div>

                  <div class="modal-footer">
                    <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
                      data-dismiss="modal">Voltar</button>
                  </div>
                </div>
              </div>

              <!-- VENCIMENTOS DE MANDATOS SINDICAIS LABORAIS-->
              <div id="vencimento_mandato_laboral_panel" class="panel panel-primary" style="margin-top: 30px;">
                <div class="panel-heading">
                  <h4>Vencimentos de Mandatos Sindicais Laborais</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row hide-block">
                    <div class="col-sm-4" style="float: right; display: flex; justify-content: right;">
                      <button style="margin-bottom: 20px;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Evento</button>
                      <button style="margin-bottom: 20px; margin-left: 0.6em;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Tarefa</button>
                    </div>
                  </div>

                  <div class="row">
                    <div class="col-lg-12">
                      <table style="width: 100%;" class="table table-striped table-bordered" id="vencimentosMandatosSindicaisLaboraisTb" data-order='[[ 0, "desc" ]]'>
                        <thead>
                          <th>Data</th>
                          <th>Origem</th>
                          <th>Sindicato Laboral</th>
                        </thead>
                        <tbody>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>

              <!-- VENCIMENTOS DE MANDATOS SINDICAIS PATRONAIS -->
              <div id="vencimento_mandato_patronal_panel" class="panel panel-primary" style="margin-top: 30px;">
                <div class="panel-heading">
                  <h4>Vencimentos de Mandatos Sindicais Patronais</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row hide-block">
                    <div class="col-sm-4" style="float: right; display: flex; justify-content: right;">
                      <button style="margin-bottom: 20px;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Evento</button>
                      <button style="margin-bottom: 20px; margin-left: 0.6em;" type="button" onclick="" class="btn btn-primary"><i class="fa-solid fa-plus" style="margin-right: 10px;"></i>Adicionar Tarefa</button>
                    </div>
                  </div>

                  <div class="row">
                    <div class="col-lg-12">
                      <table style="width: 100%;" class="table table-striped table-bordered" id="vencimentosMandatosSindicaisPatronaisTb" data-order='[[ 0, "desc" ]]'>
                        <thead>
                          <th>Data</th>
                          <th>Origem</th>
                          <th>Sindicato Patronal</th>
                        </thead>
                        <tbody>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>

              <!-- SEGUNDA TELA DE VISUALIZAÇÃO CALENDÁRIO -->
              <!-- TIMELINE -->
              <div id="timeline" class="panel panel-primary" style="margin-top: 30px; display: none;">
                <div class="panel-heading">
                  <h4>Timeline </h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row">

                    <div class="col-lg-12">
                      <div class="col-lg-12 timeline-box">
                        <h4 class="timeline-month"><span>Outubro</span> <span>2022</span></h4>
                        <ul class="timeline">
                          <li class="timeline-primary">
                            <a class="update-coment" data-toggle="modal" onclick="getTimelineById('.$obj2->id.')" data-dismiss="modal" href="#myModalUpdateTimeline">
                              <div class="timeline-icon"><i class="fa fa-pencil"></i></div>
                            </a>
                            <div class="timeline-body">
                              <div class="timeline-header">
                                <span class="date">11 DE OUTUBRO DE 2022 - 19:33</span><br>
                                <p class="sub-data">Ultima atualização: 11 de outubro de 2022 - 19:33</p>
                              </div>
                              <div class="timeline-content">
                                <h3>Título: Evento de Teste</h3>
                                <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Beatae, soluta deleniti rerum quae porro velit necessitatibus eligendi, quaerat voluptate sit quis accusantium nihil impedit tenetur repellendus ea possimus quas asperiores?</p>
                              </div>
                              <div class="timeline-footer">
                                <!-- <a href="#" class="btn btn-default btn-sm pull-left">Read Full Story</a> -->
                              </div>
                            </div>
                          </li>

                          <li class="timeline-primary">
                            <a class="update-coment" data-toggle="modal" onclick="getTimelineById('.$obj2->id.')" data-dismiss="modal" href="#myModalUpdateTimeline">
                              <div class="timeline-icon"><i class="fa fa-pencil"></i></div>
                            </a>
                            <div class="timeline-body">
                              <div class="timeline-header">
                                <span class="date">11 DE OUTUBRO DE 2022 - 19:33</span><br>
                                <p class="sub-data">Ultima atualização: 11 de outubro de 2022 - 19:33</p>
                              </div>
                              <div class="timeline-content">
                                <h3>Título: Evento de Teste</h3>
                                <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Beatae, soluta deleniti rerum quae porro velit necessitatibus eligendi, quaerat voluptate sit quis accusantium nihil impedit tenetur repellendus ea possimus quas asperiores?</p>
                              </div>
                              <div class="timeline-footer">
                                <!-- <a href="#" class="btn btn-default btn-sm pull-left">Read Full Story</a> -->
                              </div>
                            </div>
                          </li>

                          <li class="timeline-primary">
                            <a class="update-coment" data-toggle="modal" onclick="getTimelineById('.$obj2->id.')" data-dismiss="modal" href="#myModalUpdateTimeline">
                              <div class="timeline-icon"><i class="fa fa-pencil"></i></div>
                            </a>
                            <div class="timeline-body">
                              <div class="timeline-header">
                                <span class="date">11 DE OUTUBRO DE 2022 - 19:33</span><br>
                                <p class="sub-data">Ultima atualização: 11 de outubro de 2022 - 19:33</p>
                              </div>
                              <div class="timeline-content">
                                <h3>Título: Evento de Teste</h3>
                                <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Beatae, soluta deleniti rerum quae porro velit necessitatibus eligendi, quaerat voluptate sit quis accusantium nihil impedit tenetur repellendus ea possimus quas asperiores?</p>
                              </div>
                              <div class="timeline-footer">
                                <!-- <a href="#" class="btn btn-default btn-sm pull-left">Read Full Story</a> -->
                              </div>
                            </div>
                          </li>

                          <li class="timeline-primary">
                            <a class="update-coment" data-toggle="modal" onclick="getTimelineById('.$obj2->id.')" data-dismiss="modal" href="#myModalUpdateTimeline">
                              <div class="timeline-icon"><i class="fa fa-pencil"></i></div>
                            </a>
                            <div class="timeline-body">
                              <div class="timeline-header">
                                <span class="date">11 DE OUTUBRO DE 2022 - 19:33</span><br>
                                <p class="sub-data">Ultima atualização: 11 de outubro de 2022 - 19:33</p>
                              </div>
                              <div class="timeline-content">
                                <h3>Título: Evento de Teste</h3>
                                <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Beatae, soluta deleniti rerum quae porro velit necessitatibus eligendi, quaerat voluptate sit quis accusantium nihil impedit tenetur repellendus ea possimus quas asperiores?</p>
                              </div>
                              <div class="timeline-footer">
                                <!-- <a href="#" class="btn btn-default btn-sm pull-left">Read Full Story</a> -->
                              </div>
                            </div>
                          </li>
                        </ul>

                        <h4 class="timeline-month"><span>Setembro</span> <span>2022</span></h4>
                        <ul class="timeline">
                          <li class="timeline-primary">
                            <a class="update-coment" data-toggle="modal" onclick="getTimelineById('.$obj2->id.')" data-dismiss="modal" href="#myModalUpdateTimeline">
                              <div class="timeline-icon"><i class="fa fa-pencil"></i></div>
                            </a>
                            <div class="timeline-body">
                              <div class="timeline-header">
                                <span class="date">11 DE OUTUBRO DE 2022 - 19:33</span><br>
                                <p class="sub-data">Ultima atualização: 11 de outubro de 2022 - 19:33</p>
                              </div>
                              <div class="timeline-content">
                                <h3>Título: Evento de Teste</h3>
                                <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Beatae, soluta deleniti rerum quae porro velit necessitatibus eligendi, quaerat voluptate sit quis accusantium nihil impedit tenetur repellendus ea possimus quas asperiores?</p>
                              </div>
                              <div class="timeline-footer">
                                <!-- <a href="#" class="btn btn-default btn-sm pull-left">Read Full Story</a> -->
                              </div>
                            </div>
                          </li>

                          <li class="timeline-primary">
                            <a class="update-coment" data-toggle="modal" onclick="getTimelineById('.$obj2->id.')" data-dismiss="modal" href="#myModalUpdateTimeline">
                              <div class="timeline-icon"><i class="fa fa-pencil"></i></div>
                            </a>
                            <div class="timeline-body">
                              <div class="timeline-header">
                                <span class="date">11 DE OUTUBRO DE 2022 - 19:33</span><br>
                                <p class="sub-data">Ultima atualização: 11 de outubro de 2022 - 19:33</p>
                              </div>
                              <div class="timeline-content">
                                <h3>Título: Evento de Teste</h3>
                                <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Beatae, soluta deleniti rerum quae porro velit necessitatibus eligendi, quaerat voluptate sit quis accusantium nihil impedit tenetur repellendus ea possimus quas asperiores?</p>
                              </div>
                              <div class="timeline-footer">
                                <!-- <a href="#" class="btn btn-default btn-sm pull-left">Read Full Story</a> -->
                              </div>
                            </div>
                          </li>

                        </ul>
                      </div>
                    </div>

                  </div>

                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
    
  <!-- MODAL DETALHES EVENTOS CLAUSULAS detalheClausulaModal-->
  <button type="button" class="hidden" data-toggle="modal"
    data-target="#clausulaModal" id="detalheClausulaBtn"></button>
  <div class="hidden" id="detalheClausulaModalHidden">
    <div id="detalheClausulaModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Detalhes da Cláusula</h4>
      </div>
      <div class="modal-body">
        <div class="panel panel-primary">
          <div class="panel-heading" id="textoClausulaPanel">
            <h4>Texto da Cláusula</h4>
            <div class="options">
              <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
            </div>
          </div>
          <div class="panel-body collapse in">
            <input type="hidden" class="form-control" id="id_empresa" placeholder="">
            <p id="textoClausula" class="texto-clausula"></p>
          </div>
        </div>
      </div>

      <div class="modal-footer">
        <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
          data-dismiss="modal">Voltar</button>
      </div>
    </div>
  </div>

  <!-- CADASTRAR NOVO EVENTO -->
  <div class="hidden" id="novoEventoModalHidden">
    <div id="novoEventoModalContent">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Adicionar Eventos</h4>
      </div>
      <div class="modal-body">
        <div class="row">
          <div class="col-sm-12">
            <label>Título:</label>
            <input type="text" class="form-control" id="titulo_evento" placeholder="Título">
          </div>
        </div>
      
        <div class="row">
          <div class="col-sm-12">
            <label>Local:</label>
            <input type="text" class="form-control" id="local_evento" placeholder="Local">
          </div>
        </div>

        <div class="row">
          <div class="col-sm-12">
            <label>Data e Hora:</label>
            <input type="datetime-local" class="form-control" id="data_hora_evento" placeholder="Data e hora">
          </div>
        </div>

        <div class="row">
          <div class="col-sm-12">
            <label>Repetir:</label>
            <select class="form-control select2" id="repetir_evento" placeholder="Período"></select>
          </div>
        </div>

        <div class="row" id="row-validade-repeticao-evento">
          <div class="col-sm-12">
            <label>Validade da Repetição:</label>
            <input type="datetime-local" class="form-control" id="validade_repeticao_evento" placeholder="Data e hora">
          </div>
        </div>

        <div class="row">
          <div class="col-sm-12">
            <label>Lembrete:</label>
            <select class="form-control select2" id="lembrete_evento" placeholder="Tempo"></select>
          </div>
        </div>

        <div class="row">
          <div class="col-sm-12">
            <label>Comentários:</label>
            <textarea type="text" class="form-control" id="comentarios_evento" placeholder="Tempo" rows="5" maxlength="300"></textarea>
          </div>
        </div>

        <div class="row">
          <div class="col-sm-12 flex-row-center j-left">
            <label>Permite visualização aos demais usuários?</label>
            <input type="checkbox" class="form-control visivel-checkbox" id="visivel_evento"></input>
          </div>
        </div>
        
        <div class="row">
          <div class="col-sm-12 flex-row-center j-left">
            <button type="button" class="btn btn-primary w-full" id="btnAdicionarEventoSalvar">Salvar</button>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- MODAL EVENTOS DOCSIND  -->
  <div class="modal fade" id="modalEvento" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-md">
      <div class="modal-content">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" onclick="closeModal()" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Visualizar Evento</h4>
        </div>
        <div class="modal-body">
          <div class="panel panel-primary">
            <form class="form-horizontal">
              <input type="hidden" id="up-hidden">
              <input type="hidden" id="id_note">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4 id="titlePanel">Evento</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row">
                    <div class="col-md-12 itens">
                      <div class="row">
                        <div class="col-md-12">
                          <label for="eventTitle">Título</label>
                          <input type="text" id="eventTitle" class="form-control">
                        </div>
                      </div>

                      <div class="row">
                        <div class="col-md-12">
                          <label for="nome_doc">Nome Documento</label>
                          <input type="text" id="nome_doc_event" class="form-control" disabled>
                        </div>
                      </div>

                      <div class="row">
                        <div class="col-lg-6">
                          <label for="eventStart">Período</label>
                          <input type="text" class="form-control" id="eventStart" disabled>
                        </div>

                        <div class="col-lg-6">
                          <label for="cnae">Atividade Econômica</label>
                          <input type="text" class="form-control" id="cnae" disabled>
                        </div>
                      </div>

                      <div class="row">
                        <div class="col-md-12">
                          <label for="abrang">Abrangência</label>
                          <input type="text" id="abrang" class="form-control" disabled>
                        </div>
                      </div>

                      <div class="row">
                        <div class="col-md-6">
                          <label for="laboral">Sindicato Laboral</label>
                          <input type="text" id="laboral" class="form-control" disabled>
                        </div>

                        <div class="col-md-6">
                          <label for="patronal">Sindicato Patronal</label>
                          <input type="text" id="patronal" class="form-control" disabled>
                        </div>
                      </div>
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
              <div style="display: flex; justify-content:center;">
                <button type="button" class="btn btn-danger btn-rounded btn-cancelar" onclick="closeModal()">Finalizar</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>


  <!-- MODAL EVENTOS DIRPATRO  -->
  <div class="modal fade" id="modalEventoDirpatro" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-md">
      <div class="modal-content">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" onclick="closeModal()" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Visualizar Evento</h4>
        </div>
        <div class="modal-body">
          <div class="panel panel-primary">
            <form class="form-horizontal">
              <input type="hidden" id="up-hidden">
              <input type="hidden" id="id_note">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4 id="titlePanel">Evento</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row">
                    <div class="col-md-12 itens">
                      <div class="row">
                        <div class="col-md-12">
                          <label for="title_dir_patro">Título</label>
                          <input type="text" id="title_dir_patro" class="form-control">
                        </div>
                      </div>

                      <div class="row">
                        <div class="col-md-12">
                          <label for="nome_sujeito">Nome Dirigente</label>
                          <input type="text" id="nome_sujeito" class="form-control" disabled>
                        </div>
                      </div>

                      <div class="row">
                        <div class="col-lg-6">
                          <label for="period_dir_patr">Período</label>
                          <input type="text" class="form-control" id="period_dir_patr" disabled>
                        </div>

                        <div class="col-lg-6">
                          <label for="role">Função</label>
                          <input type="text" class="form-control" id="role" disabled>
                        </div>
                      </div>

                      <div class="row">
                        <div class="col-md-6">
                          <label for="nome_patronal">Sindicato</label>
                          <input type="text" id="nome_patronal" class="form-control" disabled>
                        </div>

                        <div class="col-md-6">
                          <label for="emprea_dir_patr">Empresa</label>
                          <input type="text" id="emprea_dir_patr" class="form-control" disabled>
                        </div>
                      </div>
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
              <div style="display: flex; justify-content:center;">
                <button type="button" class="btn btn-danger btn-rounded btn-cancelar" onclick="closeModal()">Finalizar</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- MODAL EVENTOS DIREMP  -->
  <div class="modal fade" id="modalEventoDiremp" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-md">
      <div class="modal-content">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" onclick="closeModal()" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Visualizar Evento</h4>
        </div>
        <div class="modal-body">
          <div class="panel panel-primary">
            <form class="form-horizontal">
              <input type="hidden" id="up-hidden">
              <input type="hidden" id="id_note">
              <div class="panel panel-primary">
                <div class="panel-heading">
                  <h4 id="titlePanel">Evento</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in principal-table">
                  <div class="row">
                    <div class="col-md-12 itens">
                      <div class="row">
                        <div class="col-md-12">
                          <label for="title_dir_emp">Título</label>
                          <input type="text" id="title_dir_emp" class="form-control">
                        </div>
                      </div>

                      <div class="row">
                        <div class="col-md-12">
                          <label for="nome_sujeito_emp">Nome Dirigente</label>
                          <input type="text" id="nome_sujeito_emp" class="form-control" disabled>
                        </div>
                      </div>

                      <div class="row">
                        <div class="col-lg-6">
                          <label for="period_dir_emp">Período</label>
                          <input type="text" class="form-control" id="period_dir_emp" disabled>
                        </div>

                        <div class="col-lg-6">
                          <label for="role">Função</label>
                          <input type="text" class="form-control" id="role_emp" disabled>
                        </div>
                      </div>

                      <div class="row">
                        <div class="col-md-6">
                          <label for="nome_laboral">Sindicato</label>
                          <input type="text" id="nome_laboral" class="form-control" disabled>
                        </div>

                        <div class="col-md-6">
                          <label for="emprea_dir_emp">Empresa</label>
                          <input type="text" id="emprea_dir_emp" class="form-control" disabled>
                        </div>
                      </div>
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
              <div style="display: flex; justify-content:center;">
                <button type="button" class="btn btn-danger btn-rounded btn-cancelar" onclick="closeModal()">Finalizar</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- MODAL INFO SINDICATOS -->
  <button style="display: none" id="openInfoSindModalBtn" data-toggle="modal" data-target="#infoSindModal"></button>
  <div class="hidden" id="info-modal-sindicato-container">
  </div>

  </div> <!--page-content -->

  
  <?php include 'footer.php' ?>

  </div> <!--page-container -->

  <script type='text/javascript' src="./js/calendario_sindical.min.js"></script>

</body>

</html>