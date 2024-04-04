<?php
session_start();
if (!$_SESSION) {
  echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}

$userSession = $_SESSION['login'];

$sessionUser = $_SESSION['login'];

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

  <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>

  <link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

  <!-- Bootstrap 3.3.7 -->
  <link rel="stylesheet" href="modulo_sindicatos.css">

  <!-- Bootstrap Internal -->
  <link rel="stylesheet" href="includes/css/styles.css">
</head>
<style>
  body.horizontal-nav {
    padding-right: 0 !important;
  }

  .panel-body.map svg {
    max-width: 100% !important;
    max-height: 100% !important;
  }

  .select2-container {
    width: 100% !important;
  }

  .map {
    position: relative;
  }

  .info-icon {
    position: absolute;
    bottom: 8px;
    left: 8px;
    font-size: 36px;
    color: #4f8edc;
    cursor: pointer;
  }

  .map svg {
    height: 360px;
  }

  #sindChart {
    height: 100% !important;
  }

  .info-tiles.tiles-info .tiles-body {
    background: #174e92;
  }

  .info-tiles.tiles-info .tiles-heading {
    background: #3575c4;
  }

  /* hover */
  .info-tiles.tiles-info:hover .tiles-body {
    background: #0d3d79;
  }

  .info-tiles.tiles-info:hover .tiles-heading {
    background: #265da1;
  }

  #page-content {
    min-height: 100% !important;
  }

  .content_container_one {
    width: 88.666667% !important;
  }

  .img_box {
    position: absolute;
    z-index: 999;
    width: 98%;
    height: 100vh;
    background-color: rgba(255, 255, 255, 0.7);
    display: none;
  }

  .img_load {
    position: absolute;
    top: 30%;
    right: 45%;
  }

  .hide {
    display: none !important;
  }

  .show {
    display: block !important;
  }

  .chart-margin-auto {
    margin: auto;
  }

  .w-full {
    width: 100%;
  }

  .mx-auto {
    margin-left: auto;
    margin-right: auto;
  }

  .flex {
    display: flex;
  }

  .justify-center {
    justify-content: center;
  }
</style>

<body class="horizontal-nav hide">
  <?php include('menu.php'); ?>

  <div id="pageCtn" class="page-container">
    <input type="hidden" id="sind-id-input">
    <input type="hidden" id="tipo-sind-input">
    <input type="hidden" id="diretoria-id-input">
    <input type="hidden" id="unidade-id-input">
    <input type="hidden" id="nomeunidade-input">
    <input type="hidden" id="afastado-input">

    <button style="display: none" id="openInfoSindModalBtn" data-toggle="modal" data-target="#infoSindModal"></button>
    <button style="display: none" id="openEstabelecimentoModalBtn" data-toggle="modal"
      data-target="#estabelecimentoModal"></button>

    <div id="sindEstadosModalHidden" class="hidden">
      <div id="sindEstadosModalContent">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Sindicatos no Estado:</h4>
        </div>
        <div class="modal-body">
          <div class="panel panel-primary">
            <table id="sindEstadosTable" cellpadding="0" cellspacing="0" border="0"
              class="table table-striped table-bordered demo-tbl" data-order='[[ 1, "asc" ]]' style="width: 100%;">
            </table>
          </div>
        </div>
        <div class="modal-footer">
          <button data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
        </div>
      </div>
    </div>

    <div class="hidden modal_hidden" id="infoSindModalHidden">
      <div id="infoSindModalHiddenContent">
        <div class="modal-content">
          <div class="modal-header">
            <div style="display: flex; width: 100%; justify-content: space-between;">
              <h4 class="modal-title" id="infoSindModalTitle">Informações Sindicais</h4>
              <div class="dropdown" style="margin-left: 50%;">
                <button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu2" data-toggle="dropdown"
                  aria-haspopup="true" aria-expanded="false">
                  Módulos <i class="fa fa-th"></i></span>
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
                    data-dismiss="modal">Voltar</a>
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

    <div class="hidden modal_hidden" id="estabelecimentoModalHidden">
      <div id="estabelecimentoModalHiddenContent">
        <div class="modal-content">
          <div class="modal-header">
            <button id="closeModalEstaBtn" type="button" class="close" data-dismiss="modal"
              aria-hidden="true">&times;</button>
            <h4 class="modal-title">Alterar o estabelecimento do dirigente</h4>
          </div>

          <div class="modal-body">
            <form class="form-horizontal">
              <input type="hidden" id="iddirp-input" value="1">

              <div class="form-group center">
                <div class="col-md-12">
                  <label>Estabelecimento</label>
                  <select data-placeholder="Selecione" class="form-control select2" id="estabelecimento-update-input"
                    required>
                  </select>
                </div>
                <div class="col-md-4">
                  <label>Afastado para atividades sindicais</label> <!-- Filial -->
                  <select data-placeholder="Selecione" tabindex="8" class="form-control select2" id="afastado">
                    <option value="Sim">Sim</option>
                    <option value="Não">Não</option>
                  </select>
                </div>
              </div>
              <!-- Tratar butons de confirmação -->
            </form>
            <div class="row" style="margin-bottom: 1rem; margin-top: 10px;">

              <div class="col-sm-6 col-sm-offset-3">
                <div class="btn-toolbar" style="display: flex; gap: 1rem;">
                  <button id="updateEstabelecimentoBtn" class="btn btn-primary btn-rounded"
                    type="button">Processar</button>
                  <button id="limparEstabelecimentoBtn" class="btn btn-secondary btn-rounded" type="button">Limpar
                    Estabelecimento</button>
                  <button data-togle="modal" data-dismiss="modal" type="button"
                    class="btn btn-secondary">Voltar</button>
                </div>
              </div>
            </div>
          </div>
        </div><!-- /.modal-content -->
      </div><!-- /.modal-dialog -->
    </div><!-- /.modal -->

    <!-- MODAL EMPRESA -->
    <button id="filialModalOpenBtn" data-toggle="modal" data-target="#filialModal"
      class="hide">Filial</button>
    <div class="hidden" id="filialModalHidden">
      <div id="filialModalContent">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Empresa</h4>
        </div>
        <div class="modal-body">

          <!-- PAINEL DE EMPRESAS SELECIONADAS -->
          <div class="panel panel-primary">
            <div class="panel-heading">
              <h4>Empresas Selecionadas</h4>
              <div class="options">
                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
              </div>
            </div>

            <div class="panel-body collapse in">
              <div class="row selectAll" style="margin: 0px 0px 20px 0px ;" id="selecionar_todas_empresas_selecionadas-container">
                <input type="checkbox" id="selecionar_todas_empresas_selecionadas">
                <label for="selectAll">Selecionar Todos</label>
              </div>

              <table cellpadding="0" cellspacing="0" border="0"
                class="table table-striped table-bordered datatables" id="clienteUnidadeSelecionadosTb"
                data-order='[[ 0, "asc" ]]' style="width: 100%;">
              </table>

              <div id="empresas-selecionadas-actions-buttons-container">
                <button class="btn btn-primary" type="button" id="btn_remover_empresas_selecionadas">
                  Excluir
                </button>

                <button class="btn btn-primary" type="button" id="btn_atualizar_empresas_selecionadas">
                  Atualizar
                </button>
              </div>
            </div>
          </div>

          <div class="panel panel-primary" id="selecione-empresa-associada-sindicato-patronal">
            <div class="panel-heading">
              <h4>Selecione a Empresa</h4>
              <div class="options">
                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
              </div>
            </div>
            <div class="panel-body collapse in">
              <div id="grid-layout-table-2" class="box jplist">
                <div class="row selectAll" style="margin: 0px 0px 20px 0px ;">
                  <input type="checkbox" id="selecionar_todas_empresas">
                  <label for="selectAll">Selecionar Todos</label>
                </div>
                <div class="box text-shadow">
                  <table cellpadding="0" cellspacing="0" border="0"
                    class="table table-striped table-bordered demo-tbl" id="clienteUnidadeTb"
                    data-order='[[ 0, "asc" ]]' style="width: 100%;"></table>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="w-full flex justify-center">
          <button class="btn btn-primary mx-auto" type="button" id="btn_salvar_estabelecimentos_sindicato_patronal">
            Salvar
          </button>
        </div>

        <div class="modal-footer">
          <button data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary"
            data-dismiss="modal">Voltar</button>
        </div>
      </div>
    </div>


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
                      <select id="tipo-com" class="form-control select2" disabled>
                      </select>
                    </div>

                    <div class="col-sm-7">
                      <label for="assunto" id="assuntoTitulo">Sindicato</label>
                      <select id="assunto" class="form-control select2" disabled>
                      </select>
                    </div>
                  </div>

                  <div class="form-group center">
                    <div class="col-md-5">
                      <label for="tipo_usuario_destino">Tipo do Usuário (destino)</label>
                      <select id="tipo_usuario_destino" class="form-control select2" data-placeholder="Tipo Destino">
                      </select>
                    </div>

                    <div class="col-md-7">
                      <label for="destino" id="campo_tipo">--</label>
                      <select id="destino" class="form-control select2">
                      </select>
                    </div>
                  </div>

                  <div class="form-group center">
                    <div class="col-sm-4">
                      <label for="tipo-note">Fixo ou Temporário</label>
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
                      <input type="hidden" id="id_user_2">
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
                        que têm acesso a consulta de comentários</label>
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
                <button id="notificacaoCadastrarBtn" type="button"
                  class="btn btn-primary btn-rounded">Cadastrar</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div id="page-content">
      <div class="wrap">
        <div class="container">
          <div class="row" style="display: flex;">
            <div class="col-md-12">
              <section class="panel panel-primary">
                <div class="panel-heading">
                  <h4>FILTROS</h4>
                  <div class="options">
                    <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                  </div>
                </div>
                <div class="panel-body collapse in">
                  <form id="filtroForm">
                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-3">
                          <label>Grupo Econômico</label>
                          <select data-placeholder="Nome" class="form-control select2" id="grupo">
                          </select>
                        </div>

                        <div class="col-sm-3">
                          <label>Empresa</label> <!-- Matriz -->
                          <select multiple data-placeholder="Nome, Código" class="form-control select2" id="matriz">
                          </select>
                        </div>

                        <div class="col-sm-6">
                          <label>Estabelecimento</label> <!-- Filial -->
                          <select multiple data-placeholder="Nome, CNPJ" class="form-control select2" id="unidade">
                          </select>
                        </div>
                      </div>
                    </div>
                    <div class="form-group">
                      <div class="row">
                        <div class="col-sm-2">
                          <label for="data_base">Data-base</label>
                          <select multiple data-placeholder="Selecione" tabindex="8" class="form-control select2"
                            id="data_base">
                            <option value="JAN">Janeiro</option>
                            <option value="FEV">Fevereiro</option>
                            <option value="MAR">Março</option>
                            <option value="ABR">Abril</option>
                            <option value="MAI">Maio</option>
                            <option value="JUN">Junho</option>
                            <option value="JUL">Julho</option>
                            <option value="AGO">Agosto</option>
                            <option value="SET">Setembro</option>
                            <option value="OUT">Outubro</option>
                            <option value="NOV">Novembro</option>
                            <option value="DEZ">Dezembro</option>
                          </select>
                        </div>

                        <div class="col-sm-2">
                          <label>Tipo da Localidade</label>
                          <select data-placeholder="Localidade" data-minimum-results-for-search="Infinity" tabindex="8"
                            class="form-control select2" id="tipoLocalidade">
                            <option value="uf">UF</option>
                            <option value="regiao">Região</option>
                            <option value="municipio">Município</option>
                          </select>
                        </div>
                        <div class="col-sm-3">
                          <label>Localidade</label>
                          <select multiple data-placeholder="Região, UF ou Município" class="form-control select2"
                            id="localidade">
                          </select>
                        </div>

                        <div class="col-sm-5">
                          <label>Atividade Econômica</label> <!-- Categoria -->
                          <select multiple data-placeholder="CNAE" class="form-control select2" id="categoria">
                          </select>
                        </div>
                      </div>
                    </div>

                    <div class="form-group">
                      <div class="row">
                        <div class="col-lg-6">
                          <label id="label-sindicato" for="">Sindicato Laboral</label>
                          <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sind_laboral">
                          </select>
                        </div>

                        <div class="col-lg-6">
                          <label id="label-sindicato" for="">Sindicato Patronal</label>
                          <select multiple data-placeholder="Sigla, CNPJ ou Denominação" tabindex="8"
                            class="form-control select2" id="sind_patronal">
                          </select>
                        </div>
                      </div>
                    </div>

                    <div class="col-sm-12" style="display: flex; justify-content: space-between">
                      <div>
                        <button id="filtrarBtn" style="margin-top: 20px ;" type="button" class="btn btn-primary"><i
                            class="fa fa-search" style="margin-right: 10px;"></i>Filtrar</button>
                        <button id="limparFiltroBtn" style="margin-top: 20px ; margin-left:8px;" type="button"
                          class="btn btn-primary"><i class="fa fa-times-circle-o" style="margin-right: 10px;"></i>Limpar
                          Filtro</button>
                      </div>
                      <button type="button" style="margin-top: 20px;" id="gerarRelatorioBtn"
                        class="btn btn-success">Exportar registros de sindicatos</button>
                    </div>
                  </form>
                </div>
              </section>

              <section class="row cards" style="padding: 20px 0px;">
                <div class="col-lg-12 col-md-12 col-12">
                  <div class="col-lg-3 col-md-3 col-sm-12 " style="padding-left: 0;">
                    <a class="info-tiles tiles-info" href="#">
                      <div class="tiles-heading">
                        <div class="pull-left">Sindicato Laboral</div>
                      </div>
                      <div class="tiles-body">
                        <div class="pull-left"></div><!-- <i class="fa fa-filter"></i> -->
                        <div class="pull-right" id="qtd_emp">--</div>
                      </div>
                    </a>
                  </div>

                  <div class="col-lg-3 col-md-3 col-sm-12 ">
                    <a class="info-tiles tiles-info" href="#">
                      <div class="tiles-heading">
                        <div class="pull-left">Sindicatos Patronais</div>
                      </div>
                      <div class="tiles-body">
                        <div class="pull-left"></div><!-- <i class="fa fa-filter"></i> -->
                        <div class="pull-right" id="qtd_patr">--</div>
                      </div>
                    </a>
                  </div>

                  <div class="col-lg-3 col-md-3 col-sm-12 ">
                    <a class="info-tiles tiles-primary" id="goToDirLabLink">
                      <div class="tiles-heading">
                        <div class="pull-left">Mandatos Sindicais</div>
                      </div>
                      <div class="tiles-body"
                        style="font-size: 20px; display:flex; flex-direction:column; align-items:center;">
                        <p style="color:#910000;font-weight:bold;"><span style="font-size: 1.6rem; margin-right: 5px;"
                            id="mand_vencido">0</span>- Vencidos</p>
                        <p style="font-weight:bold; color: #008000;"><span style="font-size: 1.6rem; margin-right: 5px;"
                            id="mand_vigente">0</span>- Vigentes</p>
                      </div>
                    </a>
                  </div>

                  <div class="col-lg-3 col-md-3 col-sm-12" style="padding-right: 0;">
                    <a class="info-tiles tiles-primary" href="#">
                      <div class="tiles-heading">
                        <div class="pull-left">Quantidade Negociações</div>
                      </div>
                      <div class="tiles-body"
                        style="font-size: 20px; display:flex; flex-direction:column; align-items:center;">
                        <p style="color:#910000;font-weight:bold;"><span style="font-size: 1.6rem; margin-right: 5px;"
                            id="neg_vencida">0</span>- Vencidos</p>
                        <p style="font-weight:bold; color: #008000;"><span style="font-size: 1.6rem; margin-right: 5px;"
                            id="neg_vigente">0</span>- Vigentes</p>
                      </div>
                    </a>
                  </div>
                </div>
              </section>

              <section class="row map">
                <div class="col-md-8 col-lg-8" style="height: 494px;">
                  <div class="panel panel-primary">
                    <div class="panel-heading">
                      <h4>Negociações em aberto por estado</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body" style="height: 415px;">
                      <canvas id="sindChart" class="chart-margin-auto"></canvas>
                    </div>
                  </div>

                </div>
                <div class="col-lg-4 col-md-6" style="height: 494px;">
                  <div class="panel-chat panel panel-primary">
                    <div class="panel-heading">
                      <h4>Mapa de risco sindical</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body map" style="height: 415px;" id="ufs-container">
                      <?php include 'map_sindical_cores.php' ?>
                      <i class="fa fa-info-circle info-icon" aria-hidden="true"
                        title="Clique em um estado do mapa para visualizar os comentários"></i>
                    </div>
                  </div>
                </div>

              </section>

              <section class="row">
                <div class="col-lg-12">
                  <div class="panel-chat panel panel-primary">
                    <div class="panel-heading">
                      <h4>Organização Sindical Laboral </h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in">
                      <div class="box text-shadow">
                        <div style="display: flex; width: 100%; justify-content: flex-end; margin-bottom: 1rem;">
                          <button type="button" class="btn btn-success" id="exportarLaboral">Exportar registros de
                            sindicatos laborais</button>
                        </div>
                        <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                          class="table table-striped table-bordered demo-tbl" id="organizacaolaboraltb"
                          data-order='[[ 1, "asc" ]]'>
                        </table>
                      </div>
                    </div>
                  </div>
                </div>
              </section>

              <!-- 
                tabelaa para baixar excel -->
              <table style="display: none" class="table table-striped demo-tbl" id="excel-laboral">

              </table>

              <table style="display: none" class="table table-striped demo-tbl" id="excel-patronal">
              </table>

              <section class="row">
                <div class="col-lg-12">
                  <div class="panel-chat panel panel-primary">
                    <div class="panel-heading">
                      <h4>Organização Sindical Patronal</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in">
                      <div class="box text-shadow">
                        <div style="display: flex; width: 100%; justify-content: flex-end; margin-bottom: 1rem;">
                          <button type="button" class="btn btn-success" id="exportarPatronal">Exportar registros de
                            sindicatos patronais</button>
                        </div>
                        <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                          class="table table-striped table-bordered demo-tbl" id="organizacaopatronaltb"
                          data-order='[[ 1, "asc" ]]'>
                        </table>
                      </div>
                    </div>
                  </div>
                </div>
              </section>

              <section class="row">
                <div class="col-lg-12">
                  <div class="panel-chat panel panel-primary">
                    <div class="panel-heading">
                      <h4>Quantidade Centrais Sindicais</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body">
                      <canvas id="qtdChart" class="chart-margin-auto"
                        style="max-height: 500px; max-width: 1300px;"></canvas>
                    </div>
                  </div>
                </div>
              </section>

              <section class="row">
                <div class="col-lg-12">
                  <div class="panel-chat panel panel-primary" id="dirlab">
                    <div class="panel-heading">
                      <h4>Dirigentes Sindicais Laborais</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in">
                      <div class="box text-shadow">
                        <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                          class="table table-striped table-bordered demo-tbl" id="dirigenteslaboraistb"
                          data-order='[[ 1, "asc" ]]'>
                        </table>
                      </div>
                    </div>
                  </div>
                </div>
              </section>

              <section class="row">
                <div class="col-lg-12">
                  <div class="panel-chat panel panel-primary">
                    <div class="panel-heading">
                      <h4>Dirigentes Sindicais Patronais</h4>
                      <div class="options">
                        <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                      </div>
                    </div>
                    <div class="panel-body collapse in">
                      <div class="box text-shadow">
                        <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                          class="table table-striped table-bordered demo-tbl" id="dirigentespatronaistb"
                          data-order='[[ 1, "asc" ]]'>
                        </table>
                      </div>
                    </div>
                  </div>
                </div>
              </section>
            </div>
          </div>
        </div>
      </div>
    </div> <!-- page-content -->

  </div> <!-- page-container -->
  <?php include 'footer.php' ?>

  <script type='text/javascript' src="./js/modulo_sindicatos.min.js"></script>
</body>

</html>