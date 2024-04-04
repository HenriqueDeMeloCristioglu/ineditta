<?php

$userLogged = $_SESSION['login'];

include_once __DIR__ . "/includes/php/class.usuario.php";

$user = new usuario();

$userData = $user->validateUser($userLogged)['response_data']['user'];

$gecc = $userData->id_grupoecon;

$gecc_name = $userData->nome_gp;

$block = $userData->is_blocked;

if ($gecc == 0) {
  $gecc = "cliente_grupo_id_grupo_economico";
}

if ($userData) {
  echo "
            <script>
                sessionStorage.setItem('nome_usuario', '" . $userData->nome_usuario . "');
                sessionStorage.setItem('iduser', '" . $userData->id_user . "');
                sessionStorage.setItem('tipo', '" . $userData->tipo . "');
                sessionStorage.setItem('grupoecon', '" . $gecc . "');
                sessionStorage.setItem('grupoecon_name', '" . $gecc_name . "');
            </script>
        ";

  $modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];
  $modulosComercial = json_decode($userData->modulos_comercial) ? json_decode($userData->modulos_comercial) : [];


  if ($modulosComercial != [] || $modulosSisap != []) {
    $listaModulos = ["comercial" => $modulosComercial, "sisap" => $modulosSisap];

    //DEFININDO MÓDULOS SISAP E COMERCIAL
    $result = $user->validateModulos($listaModulos)['response_data'];

    $modulosComerciaisPermitidos = key_exists("comercial_permitido", $result) ? $result['comercial_permitido'] : [];
    $modulosSisapPermitidos = key_exists("sisap_permitido", $result) ? $result['sisap_permitido'] : [];

    //DEFINIDO URI PERMITIDAS
    $uriComercial = key_exists("comercial_uri", $result) ? $result['comercial_uri'] : [];
    $uriSisap = key_exists("sisap_uri", $result) ? $result['sisap_uri'] : [];
  } else {
    $uriComercial = [];
    $uriSisap = [];
    $modulosComerciaisPermitidos = [];
    $modulosSisapPermitidos = [];
  }

  $urlRequest = $_SERVER["REQUEST_URI"];

  $userNivelUri = $userData->nivel == "Unidade" ? "/desenvolvimento/perfil_estabelecimento.php" : "/desenvolvimento/perfil_grupo_economico.php";
  $userIneditta = $userData->nivel == "Ineditta" ?? "/desenvolvimento/perfil_estabelecimento.php";

  array_push($uriComercial, $userNivelUri);
  array_push($uriSisap, $userIneditta);

  //UNIFICANDO LISTAS DE URI PERMITIDOS
  $uriPermitido = array_merge($uriComercial, $uriSisap);

  //REMOVENDO PARAMETROS DA URI PARA CHECAGEM
  if (str_contains($urlRequest, '?')) {
    $result = explode('?', $urlRequest);
    $urlRequest = $result[0];
  }

  if (!in_array($urlRequest, $uriPermitido) || $block) {
    echo "
      <script>
        sessionStorage.setItem('rollback', 'true');
      </script>
    ";
  }
}
?>

<link rel="stylesheet" href="includes/css/buttons.css">

<style>
  .container:not(.container .row .container) {
    padding: 20px;
  }

  .versao-anterior-btn {
    margin-right: 12px;
  }

  /*========================
        FORMATAÇÃO DA LOGO
    ========================*/
  .container_logo_client {
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 15px;
    /* width: 140px; */
    padding: 0px 12px;
    background: #fff;
    height: 105px;
  }

  .container_logo_client img {
    width: 100%;
  }

  #page-content {
    background: #EAEBEC;
  }

  .img_container {
    width: 12.333333333333332%;
  }

  .content_container,
  .content_container_one {
    width: 97.66666666666666%;
  }

  /*========================
        FORMATAÇÃO DO MENU
    ========================*/
  .dropdown-menu li ul {
    list-style: none;
    display: none;
    padding: 0;
  }

  .need {
    display: block !important;
  }

  .dropdown-menu-content li {
    background-color: #595f69;
    padding: 5px 0 5px 30px;
  }

  .dropdown-menu-content li a {
    background-color: #595f69 !important;
  }

  .dropdown-toggle flexToggle span {
    text-transform: uppercase;
  }

  #sisap-menu {
    width: 280px;
  }

  /*================= menu sisap =================*/
  #sisap-menu input[type='checkbox'] {
    display: none;
  }

  /* submenu sisap */
  .menu-item a label {
    cursor: pointer;
  }

  /* EFEITOS */
  #sisap:hover ul#sisap-menu {
    display: block;
    transition: all 0.3s ease-in-out;
  }

  .sisap_menu_docsind:checked~.menu-item .sisap_drop_docsind,
  .sisap_menu_ac_cct:checked~.menu-item .sisap_drop_ac_cct,
  .sisap_menu_scrap:checked~.menu-item .sisap_drop_scrap,
  .sisap_menu_adm:checked~.menu-item .sisap_drop_adm,
  .sisap_menu_sind:checked~.menu-item .sisap_drop_sind,
  .sisap_menu_client:checked~.menu-item .sisap_drop_client {
    display: block;
    transition: all 0.3s ease-in-out;
    background-color: red;
  }

  /*================= menu comum =================*/
  .dropdown.dropItem:hover ul.dropdown-menu {
    display: block;
  }

  /*================= REMOVENDO ESPAÇOS DOS CAMPOS =================*/
  .form-group {
    margin-bottom: 10px !important;
  }

  label {
    margin-bottom: 0px !important;
  }
</style>

<header class="navbar navbar-inverse navbar-fixed-top" role="banner">
  <div class="navbar-header pull-left">
    <span class="navbar-brand">Ineditta</span>
  </div>

  <ul class="nav navbar-nav navbar-right" style="display: flex; align-items: center;">
    <button type="button" class="versao-anterior-btn" id="versao-anterior-btn">Sistema Ineditta versão anterior</button>
    <?php if ($userData->tipo != "Ineditta"): ?>
      <li><button type="button" class="btn" style="margin-right: 1rem" id="openInfoConsultorModalBtn" data-toggle="modal"
          data-target="#infoConsultorModal">Informações de contato</a></li>
    <?php endif; ?>
    <li class="username" style="color:white;" id="keyusername">
      <?= $userData->nome_usuario ?></a>
    <li><a href="#" style="color:red" id="btnLogout">Sair</a></li>
  </ul>

</header>

<?php include ('loading_component.php'); ?>

<div id="pageCtnMenu">
  <div class="hidden modal_hidden" id="infoConsultorModalHidden">
    <div id="infoConsultorModalHiddenContent">
      <div class="modal-content">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Informações de Contato</h4>
        </div>

        <div class="modal-body">
          <div id="info-consultor-container" class="col-sm-12" style="display: flex; flex-direction: column;">
          </div>
          <div class="modal-footer">
            <div class="row">
              <div class="col-sm-12" style="display: flex; justify-content: center;">
                <div class="btn-toolbar">
                  <button data-togle="modal" data-dismiss="modal" type="button" class="btn btn-primary">Fechar</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div><!-- /.modal-content -->
    </div><!-- /.modal-dialog -->
  </div><!-- /.modal -->
</div>

<nav class="navbar navbar-default yamm" role="navigation" style="position: fixed; width: 100%">
  <div class="navbar-header">
    <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-ex1-collapse">
      <i class="fa fa-bars"></i>
    </button>
  </div>
  <div class="collapse navbar-collapse navbar-ex1-collapse" id="horizontal-navbar">
    <ul class="nav navbar-nav" style="display: flex;">
      <?php if ($userData && !$block): ?>

        <li class="dropdown active flexMenu" style="margin-right: 4px;"> <a href="perfil_grupo_economico.php"><i
              style="color:#fff;" class="fa fa-home"></i></a></li>

        <?php if ($userData->tipo == "Ineditta"): ?>
          <li class="dropdown active flexMenu" id="sisap">
            <a href="#" class="dropdown-toggle flexToggle" data-toggle='dropdown' id="drop"><i class="fa fa-th"></i>
              <span>SISAP <i class="fa fa-angle-down"></i></span></a>
            <ul class="dropdown-menu" id="sisap-menu" style="background-color: #595f69;">
              <input type="checkbox" class="sisap_menu_client" id="sisap_menu_client">
              <input type="checkbox" class="sisap_menu_sind" id="sisap_menu_sind">
              <input type="checkbox" class="sisap_menu_adm" id="sisap_menu_adm">
              <input type="checkbox" class="sisap_menu_docsind" id="sisap_menu_docsind">
              <input type="checkbox" class="sisap_menu_ac_cct" id="sisap_menu_ac_cct">
              <input type="checkbox" class="sisap_menu_scrap" id="sisap_menu_scrap">

              <li class="menu-item"><a href="perfil_estabelecimento.php">Perfil Comércio</a></li>

              <li class="menu-item dropItem"><a><label for="sisap_menu_client" style="margin-bottom: 0;">Clientes</label> <i
                    class="fa fa-angle-down"></i></a>
                <ul class="dropdown-menu-content sisap_drop_client">
                  <?php if (in_array("Cliente Grupo Econômico", $modulosSisapPermitidos)): ?>
                    <li><a href="ClienteGrupo.php">- Grupo Econômico</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Cliente Matriz", $modulosSisapPermitidos)): ?>
                    <li><a href="clientematriz.php">- Empresa</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Cliente Unidade", $modulosSisapPermitidos)): ?>
                    <li><a href="clienteunidade.php">- Estabelecimento</a></li>
                  <?php endif; ?>
                </ul>
              </li>

              <li class="menu-item dropItem"><a><label for="sisap_menu_sind" style="margin-bottom: 0;">Sindicatos</label> <i
                    class="fa fa-angle-down"></i></a>
                <ul class="dropdown-menu-content sisap_drop_sind">
                  <?php if (in_array("Cadastro de Sindicato Patronal", $modulosSisapPermitidos)): ?>
                    <li><a href="sindicatopatronal.php">- Cadastro de Sindicato Patronal</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Cadastro de Sindicato Empregados", $modulosSisapPermitidos)): ?>
                    <li><a href="sindicatoempregados.php">- Cadastro de Sindicato Laboral</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Cadastro Confederações / Federações", $modulosSisapPermitidos)): ?>
                    <li><a href="associacao.php">- Cadastro Confederações / Federações</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Central Sindical", $modulosSisapPermitidos)): ?>
                    <li><a href="centralsindicaltrue.php">- Central Sindical</a></li>
                    <!-- <li><a href="centralsindical.php">- Cadastro de Central Sindical</a></li> -->
                  <?php endif; ?>
                  <?php if (in_array("Diretoria Sindical Empregados", $modulosSisapPermitidos)): ?>
                    <li><a href="diretoriaempregados.php">- Diretoria Sindical Laboral</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Diretoria Sindical Patronal", $modulosSisapPermitidos)): ?>
                    <li><a href="diretoriapatronal.php">- Diretoria Sindical Patronal</a></li>
                  <?php endif; ?>
                </ul>
              </li>

              <li class="menu-item dropItem"><a><label for="sisap_menu_adm" style="margin-bottom: 0;">Administrativo</label>
                  <i class="fa fa-angle-down"></i></a>
                <ul class="dropdown-menu-content sisap_drop_adm">
                  <?php if (in_array("Adm - Emails Enviados", $modulosSisapPermitidos)): ?>
                    <li><a href="emails_enviados.php">- Emails Enviados</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Adm - Cadastro Informação Adicional", $modulosSisapPermitidos)): ?>
                    <li><a href="adtipoinformacaoadicional.php">- Cadastro de Informação Adicional</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Adm - Jornadas", $modulosSisapPermitidos)): ?>
                    <li><a href="jornada.php">- Jornadas</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Adm - Cadastro de classe CNAE", $modulosSisapPermitidos)): ?>
                    <li><a href="classecnae.php">- Cadastro de Classe CNAE</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Adm - Fases Acompanhamento", $modulosSisapPermitidos)): ?>
                    <li><a href="fasecct.php">- Fases Acompanhamento</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Adm - Cadastro de Localização", $modulosSisapPermitidos)): ?>
                    <li><a href="localizacao.php">- Cadastro de Localização</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Adm - Cadastro de Módulos", $modulosSisapPermitidos)): ?>
                    <li><a href="modulos.php">- Cadastro de Módulos</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Adm - Grupo Cláusula", $modulosSisapPermitidos)): ?>
                    <li><a href="grupoclausula.php">- Grupo Claúsula</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Adm - Estrutura Cláusula", $modulosSisapPermitidos)): ?>
                    <li><a href="tagclausulas.php">- Estrutura da Claúsula</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Adm - Tipo Documento", $modulosSisapPermitidos)): ?>
                    <li><a href="tipodoc.php">- Tipo Documento</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Adm - Tipo Unidade Cliente", $modulosSisapPermitidos)): ?>
                    <li><a href="tipounidadecliente.php">- Tipo Unidade Cliente</a></li>
                  <?php endif; ?>
                </ul>
              </li>

              <li class="menu-item dropItem"><a><label for="sisap_menu_docsind" style="margin-bottom: 0;">Documento
                    Sindical</label> <i class="fa fa-angle-down"></i></a>
                <ul class="dropdown-menu-content sisap_drop_docsind">
                  <?php if (in_array("Documento Sindical", $modulosSisapPermitidos)): ?>
                    <li><a href="docsind.php">- Documento Sindical</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Inclusão das Cláusulas", $modulosSisapPermitidos)): ?>
                    <li><a href="ia_documento_sindical.php">- Scrap cláusulas - IA</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Inclusão das Cláusulas", $modulosSisapPermitidos)): ?>
                    <li><a href="clausula.php">- Validação das Cláusulas</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Cadastro de Informações Adicionais", $modulosSisapPermitidos)): ?>
                    <li><a href="informacoes_adiciaonais.php">- Cadastro de Informações Adicionais</a></li>
                  <?php endif; ?>
                </ul>
              </li>

              <li class="menu-item dropItem"><a><label for="sisap_menu_ac_cct" style="margin-bottom: 0;">Acompanhamento
                    CCT</label> <i class="fa fa-angle-down"></i></a>
                <ul class="dropdown-menu-content sisap_drop_ac_cct">
                  <?php if (in_array("Acompanhamento CCT", $modulosSisapPermitidos)): ?>
                    <li><a href="acompanhamento_db.php">- Inclusão</a></li>
                  <?php endif; ?>
                  <?php if (in_array("Acompanhamento CCT", $modulosSisapPermitidos)): ?>
                    <!-- <li><a href="nova_inclusao.php">- Nova Inclusão</a></li> -->
                  <?php endif; ?>
                </ul>
              </li>

              <?php if (in_array("Documento Sindical - IA", $modulosSisapPermitidos)): ?>
                <li class="menu-item dropItem"><a><label for="sisap_menu_scrap" style="margin-bottom: 0;">SCRAP</label> <i
                      class="fa fa-angle-down"></i></a>
                </li>
              <?php endif; ?>
            </ul>
          </li>
        <?php endif; ?>
      <?php endif; ?>

      <?php if ($userData && !$block): ?>
        <li class="dropdown dropItem flexMenu"> <a href="#" class="dropdown-toggle flexToggle" data-toggle='dropdown'
            id="sub-item"><i class="fa fa-th"></i> <span>Configurações <i class="fa fa-angle-down"></i></span></a>
          <ul class="dropdown-menu">
            <?php if (in_array("Cliente - Cadastro de comentários", $modulosComerciaisPermitidos)): ?>
              <li><a href="notificacao.php">Cadastro de Comentários</a></li>
            <?php endif; ?>
            <?php if (in_array("Cliente - Cadastro de usuários", $modulosComerciaisPermitidos)): ?>
              <li><a href="usuarioadm.php">Cadastro Usuário</a></li>
            <?php endif; ?>
            <?php if (in_array("Cliente - Cadastro de filial", $modulosComerciaisPermitidos)): ?>
              <!-- <li><a href="#">Unidade</a></li> -->
            <?php endif; ?>
            <?php if (in_array("Cliente - Helpdesk", $modulosComerciaisPermitidos)): ?>
              <li><a href="helpdesk.php">Gestão de Chamados</a></li>
            <?php endif; ?>
          </ul>
        </li>

        <li class="dropdown dropItem flexMenu"> <a href="#" class="dropdown-toggle flexToggle" data-toggle='dropdown'
            id="sub-item"><i class="fa fa-th"></i> <span>Sindicatos <i class="fa fa-angle-down"></i></span></a>
          <ul class="dropdown-menu">
            <?php if (in_array("Sindicatos", $modulosComerciaisPermitidos)): ?>
              <li><a href="modulo_sindicatos.php">Sindicatos</a></li>
            <?php endif; ?>
          </ul>
        </li>

        <li class="dropdown dropItem flexMenu"> <a href="#" class="dropdown-toggle flexToggle" data-toggle='dropdown'
            id="sub-item"><i class="fa fa-th"></i> <span>Documentos <i class="fa fa-angle-down"></i></span></a>
          <ul class="dropdown-menu">
            <?php if (in_array("Documentos - Inclusão", $modulosComerciaisPermitidos)): ?>
              <li><a href="documentos.php">Inclusão de Documentos</a></li>
            <?php endif; ?>
            <?php if (in_array("Documentos - Consulta", $modulosComerciaisPermitidos)): ?>
              <li><a href="consulta_documentos.php">Consulta de Documentos</a></li>
            <?php endif; ?>

          </ul>
        </li>

        <li class="dropdown dropItem flexMenu"> <a href="#" class="dropdown-toggle flexToggle" data-toggle='dropdown'
            id="sub-item"><i class="fa fa-th"></i> <span>Cláusulas <i class="fa fa-angle-down"></i></span></a>
          <ul class="dropdown-menu">
            <?php if (in_array("Cláusulas", $modulosComerciaisPermitidos)): ?>
              <li><a href="consultaclausula.php">Consulta de Cláusulas</a></li>
            <?php endif; ?>
          </ul>
        </li>

        <li class="dropdown dropItem flexMenu"> <a href="#" class="dropdown-toggle flexToggle" data-toggle='dropdown'
            id="sub-item"><i class="fa fa-th"></i> <span>Calendários <i class="fa fa-angle-down"></i></span></a>
          <ul class="dropdown-menu">
            <?php if (in_array("Calendário sindical", $modulosComerciaisPermitidos)): ?>
              <li><a href="calendario_sindical.php">Calendário Sindical</a></li>
            <?php endif; ?>
            <?php if (in_array("Calendário - Gestão de tarefas", $modulosComerciaisPermitidos)): ?>
              <li><a href="tarefas_sindicais.php">Tarefas Sindicais</a></li>
            <?php endif; ?>
          </ul>
        </li>

        <li class="dropdown dropItem flexMenu"> <a href="#" class="dropdown-toggle flexToggle" data-toggle='dropdown'
            id="sub-item"><i class="fa fa-th"></i> <span>Mapa Sindical <i class="fa fa-angle-down"></i></span></a>
          <ul class="dropdown-menu">
            <?php if (in_array("Mapa sindical - Indicadores econômicos", $modulosComerciaisPermitidos)): ?>
              <li><a href="indecon.php">Indicadores Econômicos</a></li>
            <?php endif; ?>
            <?php if (in_array("Mapa sindical - Indicadores sindicais", $modulosComerciaisPermitidos)): ?>
              <li><a href="indsindicais.php">Indicadores Sindicais</a></li>
            <?php endif; ?>
            <?php if (in_array("Mapa sindical CSV/Excel", $modulosComerciaisPermitidos)): ?>
              <li><a href="geradorCsv.php">Gerar arquivo EXCEL</a></li>
            <?php endif; ?>
            <?php if (in_array("Comparativo - Mapa Sindical", $modulosComerciaisPermitidos)): ?>
              <li><a href="comparativo.php">Comparativo</a></li>
            <?php endif; ?>
            <?php if (in_array("Formulário Aplicação", $modulosComerciaisPermitidos)): ?>
              <li><a href="formulario_comunicado.php">Formulário Aplicação</a></li>
            <?php endif; ?>
            <?php if (in_array("Mapa sindical – Busca rápida", $modulosComerciaisPermitidos)): ?>
              <li><a href="busca_rapida.php">Busca Rápida</a></li>
            <?php endif; ?>
          </ul>
        </li>

        <li class="dropdown dropItem flexMenu" id="menu_neg"> <a href="#" class="dropdown-toggle flexToggle"
            data-toggle='dropdown' id="sub-item"><i class="fa fa-th"></i> <span>Negociação <i
                class="fa fa-angle-down"></i></span></a>
          <ul class="dropdown-menu">
            <?php if (in_array("Negociacao", $modulosComerciaisPermitidos)): ?>
              <li><a href="negociacao.php">Negociação</a></li>
            <?php endif; ?>
            <!-- <?php if (in_array("Acompanhamento CCT Cliente", $modulosComerciaisPermitidos)): ?>
                            <li><a href="acompanhamento_cct.php">Acompanhamento das Negociações</a></li>
                        <?php endif; ?> -->
            <?php if (in_array("Acompanhamento Negociação sindical", $modulosComerciaisPermitidos)): ?>
              <li><a href="relatorio_negociacoes.php">Relatório de Acompanhamento CCT Ineditta</a></li>
            <?php endif; ?>
          </ul>
        </li>
      <?php else: ?>
        <li style="margin-top: 8px;">Você não possui permissões de acesso, favor contatar um administrador!</li>
      <?php endif; ?>
    </ul>
  </div>
</nav>
<br><br>