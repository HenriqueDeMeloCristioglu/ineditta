<?php
session_start();
if (!$_SESSION) {
	echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
}
$sessionUser = $_SESSION['login'];
/**
 * @author    {Enter5}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2020-08-28 13:40 ( v1.0.0 ) - 
	}
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÏ = 0


$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.'));

// Montando o c??o do erro que ser?presentado
$localizar  = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir,  strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar 	= array("\\", "/includes/php");
$substituir	= array("/", "");
$path 		= str_replace($localizar, $substituir, __DIR__);

$fileTagClausulas = $path . '/includes/php/class.tagclausulas.php';

$fileSinonimos = $path . '/includes/php/class.sinonimos.php';

if (file_exists($fileTagClausulas)) {

	// include_once $fileTagClausulas;

	include_once($fileTagClausulas);

	include_once($fileSinonimos);

	include_once __DIR__ . "/includes/php/class.usuario.php";

	$user = new usuario();
	$userData = $user->validateUser($sessionUser)['response_data']['user'];

	$modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

	$modulos = ["sisap" => $modulosSisap, "comercial" => []];

	$sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

	foreach ($sisap as $key => $value) {
		if (mb_strpos($value, "Adm - Estrutura Cláusula")) {
			$indexModule = $key;
			$idModule = strstr($value, "+", true);
		}
	}

	for ($i = 0; $i < count($modulosSisap); $i++) {
		if ($modulosSisap[$i]->id == $idModule) {
			$thisModule = $modulosSisap[$i]; //module Permissions here
			break;
		}
	}


	$tagclausulas = new tagclausulas();

	$sinonimos = new sinonimos();
	// $tag = new tagclausulas();
	// $tag->saveModuleChange(["check" => 1, "estruturaClausulaID" => 11, "cdtipoinformacaoadicional" => 12]);

	if ($tagclausulas->response['response_status']['status'] == 1) {

		$getTagClausulas = $tagclausulas->getTagClausulas();
		$getClausulas = $getTagClausulas;

		$getSinonimos = $sinonimos->getSinonimos();
		if ($getTagClausulas['response_status']['status'] == 1) {

			$lista = $getTagClausulas['response_data']['html'];

			$listaClausulas = $getClausulas['response_data']['list2'];
			$nomesClausulas = $getClausulas['response_data']['nomesClausulas'];
			$listUpdate = $getClausulas['response_data']['listUpdate'];
			$listaSinonimos = $getSinonimos['response_data']['sinonimos'];

			$getTagClausulasInformacoesAdicionais = $tagclausulas->getTagClausulasInformacoesAdicionais();
			$getCampos = $tagclausulas->getTagClausulasCampos();

			if ($getTagClausulasInformacoesAdicionais['response_status']['status'] == 1) {

				$listaTagClausulasInformacoesAdicionais = $getTagClausulasInformacoesAdicionais['response_data']['html'];
			} else {

				$response['response_status']['status']     = 0;
				$response['response_status']['error_code'] = $error_code . __LINE__;
				$response['response_status']['msg']        = $getTagClausulasInformacoesAdicionais['response_status']['error_code'] . '::' . $getTagClausulasInformacoesAdicionais['response_status']['msg'];
			}
		} else {

			$response['response_status']['status']     = 0;
			$response['response_status']['error_code'] = $error_code . __LINE__;
			$response['response_status']['msg']        = $getTagClausulas['response_status']['error_code'] . '::' . $getTagClausulas['response_status']['msg'];
		}
	} else {
		$response['response_status']['status']     = 0;
		$response['response_status']['error_code'] = $error_code . __LINE__;
		$response['response_status']['msg']        = $tagclausulas->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
	}
} else {
	$response['response_status']['status']     = 0;
	$response['response_status']['error_code'] = $error_code . __LINE__;
	$response['response_status']['msg']        = 'Não foi possível encontrar o arquivo (class.tagclausulas).';
}

if ($response['response_status']['status'] == 0) {

	print $response['response_status']['error_code'] . " :: " . $response['response_status']['msg'];
	exit();
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

	<link rel="stylesheet" href="includes/css/styles.css">
	<link rel='stylesheet' type='text/css' href='includes/plugins/form-toggle/toggles.css' />
	<link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

	<!-- CSS datagrid -->
	<link rel="stylesheet" href="includes/plugins/datagrid/styles/jplist-custom.css">

	<script src="keycloak.js"></script>

</head>

<style>
	#page-content {
		min-height: 100% !important;
	}
</style>

<body onload="initKeycloak()" style="display: none;" class="horizontal-nav">
	<?php include('menu.php'); ?>

	<div class="modal fade" id="updateModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
		<div class="modal-dialog">
			<div class="modal-content">
				<div class="modal-header">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 class="modal-title">Atualização das Cláusulas</h4>
				</div>
				<div class="modal-body">
					<div class="panel panel-primary">
						<br> <!--Modal de Update = Atualizar -->
						<form class="form-horizontal">
							<input type="hidden" id="id-inputu" value="1">
							<div class="form-group">
								<label for="up1-inputu" class="col-sm-3 control-label">Cláusula Selecionada</label>
								<div class="col-sm-8">
									<input type="text" class="form-control" id="up1-inputu" placeholder="">
								</div>
							</div>

							<div class="form-group">
								<label for="input-type" class="col-sm-3 control-label">Tipo</label>
								<div class="col-sm-8" id="input-type">

								</div>
							</div>

							<div class="form-group">
								<label for="input-class" class="col-sm-3 control-label">Classe</label>
								<div class="col-sm-8" id="input-class">

								</div>
							</div>

							<!-- Tratar butons de confirmação -->

						</form>
						<div class="row">
							<div class="col-sm-6 col-sm-offset-3">
								<div class="btn-toolbar">
									<?php if ($thisModule->Alterar == 1) : ?>
										<a id="btn-atualizar" href="#" class="btn btn-primary btn-rounded">Processar</a>
									<?php else : ?>
									<?php endif; ?>
									<a href="#" class="btn btn-primary btn-rounded btn-cancelar">Finalizar</a>
								</div>
							</div>
						</div>
					</div>
				</div>

			</div>
		</div>
	</div>

	<div class="page-container">
		<div class="page-content" style="min-height: 1000px;">
			<!-- <div> -->
			<div id="wrap">
				<div class="container">
					<div class="row">
						<div class="col-md-1">
							<img id="imglogo" class="img-circle">
						</div>

						<div class="col-md-11">
							<div class="row">
								<div class="col-md-12">
									<div class="panel-heading">
										<div class="panel-heading">
											<h4>Assistente </h4>
											<div class="options">
												<ul class="nav nav-tabs">
													<li id="codetabsNew" class="active"><a href="#codetabs1" data-toggle="tab">1. Nova Claúsula</a></li>
													<li id="codetabsNew2"><a href="#codetabs2" data-toggle="tab">2. Informações Adicionais</a></li>
													<li id="codetabsNew3"><a href="#codetabs3" data-toggle="tab">Sinônimos</a></li>
												</ul>
											</div>
										</div>
										<div class="panel-body">
											<div class="tab-content">
												<div class="tab-pane active" id="codetabs1">
													<div class="row">
														<div class="form-group">
															<tr>
																<?php if ($thisModule->Criar == 1) : ?>
																	<td><a data-toggle="modal" href="#myModal" class="btn default-alt ">Adicionar Claúsula</a></td>
																<?php else : ?>

																<?php endif; ?>
															</tr>
															<p></p>
															<div class="panel-body collapse in">
																<div id="grid-layout-table-1" class="box jplist">
																	<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
																	<div class="jplist-panel box panel-top">
																		<button type="button" data-control-type="reset" data-control-name="reset" data-control-action="reset" class="jplist-reset-btn btn btn-primary">Limpar <i class="fa fa-share mls"></i></button>
																		<div data-control-type="drop-down" data-control-name="paging" data-control-action="paging" class="jplist-drop-down form-control">
																			<ul class="dropdown-menu">
																				<li><span data-number="3"> 3 por página</span></li>
																				<li><span data-number="5"> 5 por página</span></li>
																				<li><span data-number="10" data-default="true"> 10 por página</span></li>
																				<li><span data-number="all"> ver todos</span></li>
																			</ul>
																		</div>
																		<div data-control-type="drop-down" data-control-name="sort" data-control-action="sort" data-datetime-format="{month}/{day}/{year}" class="jplist-drop-down form-control">
																			<ul class="dropdown-menu">
																				<li><span data-path="default">Listar por</span></li>
																				<li><span data-path=".title" data-order="asc" data-type="text">Título A-Z</span></li>
																				<li><span data-path=".title" data-order="desc" data-type="text">Título Z-A</span></li>
																				<li><span data-path=".desc" data-order="asc" data-type="text">Tipo A-Z</span></li>
																				<li><span data-path=".desc" data-order="desc" data-type="text">Tipo Z-A</span></li>
																				<!-- <li><span data-path=".like" data-order="asc" data-type="number" data-default="true">Likes asc</span></li>
																					<li><span data-path=".like" data-order="desc" data-type="number">Likes desc</span></li>
																					<li><span data-path=".date" data-order="asc" data-type="datetime">Date asc</span></li>
																					<li><span data-path=".date" data-order="desc" data-type="datetime">Date desc</span></li> -->
																			</ul>
																		</div>
																		<div class="text-filter-box">
																			<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".title" type="text" value="" placeholder="Filtrar por nome" data-control-type="textbox" data-control-name="title-filter" data-control-action="filter" class="form-control" /></div>
																		</div>
																		<div class="text-filter-box">
																			<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Filtrar por tipo" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control" /></div>
																		</div>
																		<div data-type="Página {current} de {pages}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
																		<!-- <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" class="jplist-pagination"></div> -->
																	</div>
																	<div class="box text-shadow">
																		<table cellpadding="0" cellspacing="0" border="0" class="table demo-tbl">
																			<thead>
																				<tr>
																					<th></th>
																					<th>Nome da Cláusula</th>
																					<th>Tipo</th>
																					<th>Classe</th>
																				</tr>
																			</thead>
																			<tbody>
																				<?php print $lista; ?>
																			</tbody>
																		</table>
																	</div>
																	<div class="box jplist-no-results text-shadow align-center">
																		<p>Nenhum resultado encontrado</p>
																	</div>
																	<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
																	<div class="jplist-panel box panel-bottom">
																		<div data-type="{start} - {end} de {all}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
																		<div data-control-type="pagination" data-control-name="paging" data-control-action="paging" data-control-animate-to-top="true" class="jplist-pagination"></div>
																	</div>
																</div>
															</div>
														</div>
													</div>
												</div>
												<div class="tab-pane" id="codetabs2">
													<div class="row">
														<div class="form-group">
															<div class="panel-body collapse in">
																<div id="grid-layout-table-2" class="box jplist">
																	<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
																	<div class="jplist-panel box panel-top">
																		<button type="button" data-control-type="reset" data-control-name="reset" data-control-action="reset" class="jplist-reset-btn btn btn-primary">Limpar <i class="fa fa-share mls"></i></button>
																		<div data-control-type="drop-down" data-control-name="paging" data-control-action="paging" class="jplist-drop-down form-control">
																			<ul class="dropdown-menu">
																				<li><span data-number="3"> 3 por página</span></li>
																				<li><span data-number="5"> 5 por página</span></li>
																				<li><span data-number="10" data-default="true"> 10 por página</span></li>
																				<li><span data-number="all"> ver todos</span></li>
																			</ul>
																		</div>
																		<div data-control-type="drop-down" data-control-name="sort" data-control-action="sort" data-datetime-format="{month}/{day}/{year}" class="jplist-drop-down form-control">
																			<ul class="dropdown-menu">
																				<li><span data-path="default">Listar por</span></li>
																				<li><span data-path=".title" data-order="asc" data-type="text">Título A-Z</span></li>
																				<li><span data-path=".title" data-order="desc" data-type="text">Título Z-A</span></li>
																				<li><span data-path=".desc" data-order="asc" data-type="text">Tipo A-Z</span></li>
																				<li><span data-path=".desc" data-order="desc" data-type="text">Tipo Z-A</span></li>
																				<!-- <li><span data-path=".like" data-order="asc" data-type="number" data-default="true">Likes asc</span></li>
																					<li><span data-path=".like" data-order="desc" data-type="number">Likes desc</span></li>
																					<li><span data-path=".date" data-order="asc" data-type="datetime">Date asc</span></li>
																					<li><span data-path=".date" data-order="desc" data-type="datetime">Date desc</span></li> -->
																			</ul>
																		</div>
																		<div class="text-filter-box">
																			<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".title" type="text" value="" placeholder="Filtrar por cláusula" data-control-type="textbox" data-control-name="title-filter" data-control-action="filter" class="form-control" /></div>
																		</div>
																		<div class="text-filter-box">
																			<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Filtrar por informação" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control" /></div>
																		</div>
																		<div data-type="Página {current} de {pages}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
																		<!-- <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" class="jplist-pagination"></div> -->
																	</div>
																	<div class="box text-shadow">
																		<table cellpadding="0" cellspacing="0" border="0" class="table demo-tbl">
																			<thead>
																				<tr>
																					<th>#</th>
																					<th>Cláusula Atual</th>
																					<th>Nome da Informação Adicional</th>
																				</tr>
																			</thead>
																			<tbody>
																				<?php print $listaTagClausulasInformacoesAdicionais; ?>
																			</tbody>
																		</table>
																	</div>
																	<div class="box jplist-no-results text-shadow align-center">
																		<p>Nenhum resultado encontrado</p>
																	</div>
																	<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
																	<div class="jplist-panel box panel-bottom">
																		<div data-type="{start} - {end} de {all}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
																		<div data-control-type="pagination" data-control-name="paging" data-control-action="paging" data-control-animate-to-top="true" class="jplist-pagination"></div>
																	</div>
																</div>
															</div>
														</div>
													</div>
												</div>
												<div class="tab-pane" id="codetabs3">
													<div class="row">
														<div class="form-group">
															<tr>
																<?php if ($thisModule->Criar == 1) : ?>
																	<td><a data-toggle="modal" href="#myModalAddSinonimo" class="btn default-alt ">Adicionar Sinônimos</a></td>
																<?php else : ?>

																<?php endif; ?>
															</tr>
															<p></p>
															<div class="panel-body collapse in">
																<div id="grid-layout-table-3" class="box jplist">
																	<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
																	<div class="jplist-panel box panel-top">
																		<button type="button" data-control-type="reset" data-control-name="reset" data-control-action="reset" class="jplist-reset-btn btn btn-primary">Limpar <i class="fa fa-share mls"></i></button>
																		<div data-control-type="drop-down" data-control-name="paging" data-control-action="paging" class="jplist-drop-down form-control">
																			<ul class="dropdown-menu">
																				<li><span data-number="3"> 3 por página</span></li>
																				<li><span data-number="5"> 5 por página</span></li>
																				<li><span data-number="10" data-default="true"> 10 por página</span></li>
																				<li><span data-number="all"> ver todos</span></li>
																			</ul>
																		</div>
																		<div data-control-type="drop-down" data-control-name="sort" data-control-action="sort" data-datetime-format="{month}/{day}/{year}" class="jplist-drop-down form-control">
																			<ul class="dropdown-menu">
																				<li><span data-path="default">Listar por</span></li>
																				<li><span data-path=".title" data-order="asc" data-type="text">Título A-Z</span></li>
																				<li><span data-path=".title" data-order="desc" data-type="text">Título Z-A</span></li>
																				<li><span data-path=".desc" data-order="asc" data-type="text">Tipo A-Z</span></li>
																				<li><span data-path=".desc" data-order="desc" data-type="text">Tipo Z-A</span></li>
																			</ul>
																		</div>
																		<div class="text-filter-box">
																			<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".title" type="text" value="" placeholder="Filtrar por sinônimos" data-control-type="textbox" data-control-name="title-filter" data-control-action="filter" class="form-control" /></div>
																		</div>
																		<div class="text-filter-box">
																			<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Filtrar por cláusula" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control" /></div>
																		</div>
																		<div data-type="Página {current} de {pages}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
																		<!-- <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" class="jplist-pagination"></div> -->
																	</div>
																	<div class="box text-shadow">
																		<table cellpadding="0" cellspacing="0" border="0" class="table demo-tbl">
																			<thead>
																				<tr>
																					<th>#</th>
																					<th>Sinônimos</th>
																					<th>Cláusula</th>
																				</tr>
																			</thead>
																			<tbody>
																				<?php print $listaSinonimos; ?>
																			</tbody>
																		</table>
																	</div>
																	<div class="box jplist-no-results text-shadow align-center">
																		<p>Nenhum resultado encontrado</p>
																	</div>
																	<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
																	<div class="jplist-panel box panel-bottom">
																		<div data-type="{start} - {end} de {all}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
																		<div data-control-type="pagination" data-control-name="paging" data-control-action="paging" data-control-animate-to-top="true" class="jplist-pagination"></div>
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
						</div>
					</div>
				</div>
			</div>

		</div>

		<!-- Inserir sinonimos -->
		<div class="modal fade" id="myModalAddSinonimo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
			<div class="modal-dialog">
				<div class="modal-content">
					<div class="modal-header">
						<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
						<h4 class="modal-title">Adicionar Sinonimos</h4>
					</div>
					<div class="modal-body">
						<div class="panel panel-primary">
							<br>
							<!-- Modal de Insert = Inserir -->
							<form class="form-horizontal">
								<div class="form-group">
									<label for="sino-inputc" class="col-sm-3 control-label">Sinonimos</label>
									<div class="col-sm-8">
										<input type="text" class="form-control" id="sino-inputc" placeholder="">
									</div>
								</div>

								<div class="form-group">
									<label for="sino-inputc" class="col-sm-3 control-label">Cláusula</label>
									<div class="col-sm-4">
										<select class="form-control" id="ge-input" style="-webkit-appearance: none;-moz-appearance: none;text-indent: 1px; text-overflow: '';" disabled>
											<optgroup label="SELECIONE">
												<?php print $nomesClausulas; ?>
											</optgroup>
										</select>
									</div>
									<a id="btn-add-ge" data-toggle="modal" href="#myModalRegister" data-dismiss="modal" class="col-sm-2 btn btn-primary btn-rounded">Selecionar</a>
								</div>

								<!-- Tratar buttons de validações -->
								<div class="row">
									<div class="col-sm-6 col-sm-offset-3">
										<div class="btn-toolbar">
											<a href="#" class="btn btn-primary btn-rounded" onclick="addSinonimos();">Processar</a>
											<a href="#" class="btn btn-primary btn-rounded btn-cancelar">Finalizar</a>
										</div>
									</div>
								</div>
							</form>
						</div>
					</div>
				</div>
			</div>
		</div>

		<!-- Atualização de sinonimos -->
		<div class="modal fade" id="myModalUpdate" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
			<div class="modal-dialog">
				<div class="modal-content">
					<div class="modal-header">
						<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
						<h4 class="modal-title">Atualizar Sinonimos</h4>
					</div>
					<div class="modal-body">
						<div class="panel panel-primary">
							<br>
							<!-- Modal de Insert = Inserir -->
							<form class="form-horizontal">
								<input type="hidden" id="id_estrutura">
								<div class="form-group">
									<label for="info-input-update" class="col-sm-3 control-label">Sinonimos</label>
									<div class="col-sm-8">
										<input type="text" class="form-control" id="info-input-update" placeholder="">
									</div>
								</div>

								<div class="form-group">
									<label for="ge-input-update" class="col-sm-3 control-label">Cláusula</label>
									<div class="col-sm-4">
										<select class="form-control" id="ge-input-update" style="-webkit-appearance: none;-moz-appearance: none;text-indent: 1px; text-overflow: '';" disabled>
											<optgroup label="SELECIONE">
												<?php print $nomesClausulas; ?>
											</optgroup>
										</select>
									</div>
									<a id="btn-add-ge" data-toggle="modal" href="#myModalClausulaUpdate" data-dismiss="modal" class="col-sm-2 btn btn-primary btn-rounded">Selecionar</a>
								</div>

								<!-- Tratar buttons de validações -->
								<div class="row">
									<div class="col-sm-6 col-sm-offset-3">
										<div class="btn-toolbar">
											<input type="hidden" id="input-hide" value="">
											<?php if ($thisModule->Alterar == 1) : ?>
												<a href="#" class="btn btn-primary btn-rounded" onclick="updateSinonimos();">Processar</a>
											<?php else : ?>
											<?php endif; ?>
											<a href="#" class="btn btn-primary btn-rounded btn-cancelar">Finalizar</a>
										</div>
									</div>
								</div>
							</form>
						</div>
					</div>
				</div>
			</div>
		</div>

		<!-- Selecionar Cláusula -->
		<div id="myModalRegister" class="modal" tabindex="-1" role="dialog">
			<div class="modal-dialog" role="document">
				<div class="modal-content">
					<div class="panel panel-sky">
						<div class="panel-heading">
							<h4>Selecione a Cláusula de Referência</h4>
							<div class="options">
								<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
							</div>
						</div>
						<div class="panel-body collapse in">
							<div id="grid-layout-table-6" class="box jplist">
								<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
								<div class="jplist-panel box panel-top">
									<button type="button" data-control-type="reset" data-control-name="reset" data-control-action="reset" class="jplist-reset-btn btn btn-primary">Limpar <i class="fa fa-share mls"></i></button>
									<div data-control-type="drop-down" data-control-name="paging" data-control-action="paging" class="jplist-drop-down form-control">
										<ul class="dropdown-menu">
											<li><span data-number="3"> 3 por página</span></li>
											<li><span data-number="5"> 5 por página</span></li>
											<li><span data-number="10" data-default="true"> 10 por página</span></li>
											<li><span data-number="all"> ver todos</span></li>
										</ul>
									</div>
									<div data-control-type="drop-down" data-control-name="sort" data-control-action="sort" data-datetime-format="{month}/{day}/{year}" class="jplist-drop-down form-control">
										<ul class="dropdown-menu">
											<li><span data-path="default">Listar por</span></li>
											<li><span data-path=".title" data-order="asc" data-type="text">Nome A-Z</span></li>
											<li><span data-path=".title" data-order="desc" data-type="text">Nome Z-A</span></li>
											<li><span data-path=".desc" data-order="asc" data-type="text">Tipo A-Z</span></li>
											<li><span data-path=".desc" data-order="desc" data-type="text">Tipo Z-A</span></li>
										</ul>
									</div>
									<div class="text-filter-box">
										<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".title" type="text" value="" placeholder="Filtrar por nome" data-control-type="textbox" data-control-name="title-filter" data-control-action="filter" class="form-control" /></div>
									</div>
									<div class="text-filter-box">
										<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Filtrar por tipo" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control" /></div>
									</div>
									<div data-type="Página {current} de {pages}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
									<!-- <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" class="jplist-pagination"></div> -->
								</div>
								<div class="box text-shadow">
									<table cellpadding="0" cellspacing="0" border="0" class="table demo-tbl">
										<thead>
											<tr>
												<th>Selecionar</th>
												<th>Nome</th>
												<th>Tipoo</th>
												<th>Classe</th>
											</tr>
										</thead>
										<tbody>
											<?php print $listaClausulas; ?>
										</tbody>
									</table>
								</div>
								<div class="box jplist-no-results text-shadow align-center">
									<p>Nenhum resultado encontrado</p>
								</div>
								<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
								<div class="jplist-panel box panel-bottom">
									<div data-type="{start} - {end} de {all}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
									<div data-control-type="pagination" data-control-name="paging" data-control-action="paging" data-control-animate-to-top="true" class="jplist-pagination"></div>
								</div>
							</div>
						</div>
					</div>
					<div class="modal-footer">
						<button data-toggle="modal" href="#myModalAddSinonimo" type="button" class="btn btn-secondary" data-dismiss="modal">Seguinte</button>
					</div>
				</div>
			</div>
		</div>

		<!-- MODAL UPDATE SINONIMOS - SELEÇÃO DE CLÁUSULA -->
		<div id="myModalClausulaUpdate" class="modal" tabindex="-1" role="dialog">
			<div class="modal-dialog" role="document">
				<div class="modal-content">
					<div class="panel panel-sky">
						<div class="panel-heading">
							<h4>Selecione a Cláusula de Referência</h4>
							<div class="options">
								<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
							</div>
						</div>
						<div class="panel-body collapse in">
							<div id="grid-layout-table-7" class="box jplist">
								<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
								<div class="jplist-panel box panel-top">
									<button type="button" data-control-type="reset" data-control-name="reset" data-control-action="reset" class="jplist-reset-btn btn btn-primary">Limpar <i class="fa fa-share mls"></i></button>
									<div data-control-type="drop-down" data-control-name="paging" data-control-action="paging" class="jplist-drop-down form-control">
										<ul class="dropdown-menu">
											<li><span data-number="3"> 3 por página</span></li>
											<li><span data-number="5"> 5 por página</span></li>
											<li><span data-number="10" data-default="true"> 10 por página</span></li>
											<li><span data-number="all"> ver todos</span></li>
										</ul>
									</div>
									<div data-control-type="drop-down" data-control-name="sort" data-control-action="sort" data-datetime-format="{month}/{day}/{year}" class="jplist-drop-down form-control">
										<ul class="dropdown-menu">
											<li><span data-path="default">Listar por</span></li>
											<li><span data-path=".title" data-order="asc" data-type="text">Nome A-Z</span></li>
											<li><span data-path=".title" data-order="desc" data-type="text">Nome Z-A</span></li>
											<li><span data-path=".desc" data-order="asc" data-type="text">Tipo A-Z</span></li>
											<li><span data-path=".desc" data-order="desc" data-type="text">Tipo Z-A</span></li>
										</ul>
									</div>
									<div class="text-filter-box">
										<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".title" type="text" value="" placeholder="Filtrar por nome" data-control-type="textbox" data-control-name="title-filter" data-control-action="filter" class="form-control" /></div>
									</div>
									<div class="text-filter-box">
										<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Filtrar por tipo" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control" /></div>
									</div>
									<div data-type="Página {current} de {pages}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
									<!-- <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" class="jplist-pagination"></div> -->
								</div>
								<div class="box text-shadow">
									<table cellpadding="0" cellspacing="0" border="0" class="table demo-tbl">
										<thead>
											<tr>
												<th>Selecionar</th>
												<th>Nome</th>
												<th>Tipoo</th>
												<th>Classe</th>
											</tr>
										</thead>
										<tbody>
											<?php print $listUpdate; ?>
										</tbody>
									</table>
								</div>
								<div class="box jplist-no-results text-shadow align-center">
									<p>Nenhum resultado encontrado</p>
								</div>
								<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
								<div class="jplist-panel box panel-bottom">
									<div data-type="{start} - {end} de {all}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
									<div data-control-type="pagination" data-control-name="paging" data-control-action="paging" data-control-animate-to-top="true" class="jplist-pagination"></div>
								</div>
							</div>
						</div>
					</div>
					<div class="modal-footer">
						<button data-toggle="modal" href="#myModalUpdate" type="button" class="btn btn-secondary" data-dismiss="modal">Seguinte</button>
					</div>
				</div>
			</div>
		</div>

		<div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
			<div class="modal-dialog">
				<div class="modal-content">
					<div class="modal-header">
						<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
						<h4 class="modal-title">Inserir nova Cláusula</h4>
					</div>
					<div class="modal-body">
						<div class="panel panel-primary">
							<br>
							<!-- Modal de Insert = Inserir -->
							<form class="form-horizontal">
								<div class="form-group">
									<label for="info-inputc" class="col-sm-3 control-label">Nome da Cláusula</label>
									<div class="col-sm-8">
										<input type="text" class="form-control" id="info-inputc" placeholder="">
									</div>
								</div>

								<div class="form-group">
									<label for="infoa-inputc" class="col-sm-3 control-label">Tipo</label>
									<div class="col-sm-8">
										<select class="form-control" name="infoa-inputc" id="infoa-inputc">
											<option value="">Selecione --</option>
											<option value="R">Resumo</option>
											<option value="T">Tabela</option>
											<option value="P">Parâmetro</option>
										</select>
									</div>
								</div>

								<div class="form-group">
									<label for="infob-inputc" class="col-sm-3 control-label">Classe</label>
									<div class="col-sm-8">
										<select class="form-control" name="infob-inputc" id="infob-inputc">
											<option value="">Selecione --</option>
											<option value="S">Sim</option>
											<option value="N">Não</option>
										</select>
									</div>
								</div>

								<!-- Tratar buttons de validações -->
								<div class="row">
									<div class="col-sm-6 col-sm-offset-3">
										<div class="btn-toolbar">
											<a href="#" class="btn btn-primary btn-rounded" onclick="addTagClausulas();">Processar</a>
											<a href="#" class="btn btn-primary btn-rounded btn-cancelar">Finalizar</a>
										</div>
									</div>
								</div>
							</form>
						</div>
					</div>
				</div>
			</div>
		</div>

		<!-- Estrutura de Modals -->
		<?php include_once('modalTagClausulasPasso2.php'); ?>

	</div> <!--page-content -->

	<footer role="contentinfo">
		<div class="clearfix">
			<ul class="list-unstyled list-inline pull-left">
				<li>Ineditta &copy; 2022</li>
			</ul>
			<button class="pull-right btn btn-inverse-alt btn-xs hidden-print" id="back-to-top"><i class="fa fa-arrow-up"></i></button>
		</div>
	</footer>

	</div> <!--page-container -->



	<script type='text/javascript' src='includes/js/jquery-1.10.2.min.js'></script>
	<script type='text/javascript' src='includes/js/jqueryui-1.10.3.min.js'></script>
	<script type='text/javascript' src='includes/js/bootstrap.min.js'></script>
	<script type='text/javascript' src='includes/js/enquire.js'></script>
	<script type='text/javascript' src='includes/js/jquery.cookie.js'></script>
	<script type='text/javascript' src='includes/js/jquery.nicescroll.min.js'></script>
	<script type='text/javascript' src='includes/plugins/codeprettifier/prettify.js'></script>
	<script type='text/javascript' src='includes/plugins/easypiechart/jquery.easypiechart.min.js'></script>
	<script type='text/javascript' src='includes/plugins/sparklines/jquery.sparklines.min.js'></script>
	<script type='text/javascript' src='includes/plugins/form-toggle/toggle.min.js'></script>
	<script type='text/javascript' src='includes/plugins/jquery-editable/jquery.editable.min.js'></script>
	<script type='text/javascript' src='includes/plugins/form-validation/jquery.validate.min.js'></script>
	<script type='text/javascript' src='includes/plugins/form-stepy/jquery.stepy.js'></script>
	<script src="includes/plugins/edited-datatable/jquery.dataTables.min.js"></script>
	<script src="includes/plugins/edited-datatable/datatables-bs4/js/dataTables.bootstrap4.min.js"></script>
	<script src="includes/plugins/edited-datatable/datatables-responsive/js/dataTables.responsive.min.js"></script>
	<script src="includes/plugins/edited-datatable/datatables-responsive/js/responsive.bootstrap4.min.js"></script>

	<script src="includes/plugins/sweet-alert/all.js"></script>

	<script type='text/javascript' src='includes/js/placeholdr.js'></script>
	<script type='text/javascript' src='includes/demo/demo-modals.js'></script>
	<script type='text/javascript' src='includes/js/application.js'></script>
	<script type='text/javascript' src='includes/demo/demo.js'></script>
	<script type='text/javascript' src="includes/js/tagclausulas.js"></script>
	<script type='text/javascript' src="includes/js/sinonimos.js"></script>

	<!-- new datagrid -->
	<script src="includes/plugins/datagrid/script/jquery.metisMenu.js"></script>
	<script src="includes/plugins/datagrid/script/jquery.slimscroll.js"></script>
	<script src="includes/plugins/datagrid/script/jquery.flot.js"></script>
	<script src="includes/plugins/datagrid/script/jquery.flot.categories.js"></script>
	<script src="includes/plugins/datagrid/script/jquery.flot.pie.js"></script>
	<script src="includes/plugins/datagrid/script/jquery.flot.tooltip.js"></script>
	<script src="includes/plugins/datagrid/script/jquery.flot.resize.js"></script>
	<script src="includes/plugins/datagrid/script/jquery.flot.fillbetween.js"></script>
	<script src="includes/plugins/datagrid/script/jquery.flot.stack.js"></script>
	<script src="includes/plugins/datagrid/script/jquery.flot.spline.js"></script>
	<script src="includes/plugins/datagrid/script/jplist.min.js"></script>
	<script src="includes/plugins/datagrid/script/jplist.js"></script>
	<script src="includes/plugins/datagrid/script/main.js"></script>



</body>

</html>