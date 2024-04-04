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
$localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array("", "", "", "", "-");
$error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar = array("\\", "/includes/php");
$substituir = array("/", "");
$path = str_replace($localizar, $substituir, __DIR__);


$fileClassCalendario = $path . '/includes/php/class.clientematriz.php';

if (file_exists($fileClassCalendario)) {

	include_once($fileClassCalendario);

	include_once __DIR__ . "/includes/php/class.usuario.php";

	$user = new usuario();
	$userData = $user->validateUser($sessionUser)['response_data']['user'];

	$modulosSisap = json_decode($userData->modulos_sisap) ? json_decode($userData->modulos_sisap) : [];

	$modulos = ["sisap" => $modulosSisap, "comercial" => []];

	$sisap = $user->validateModulos($modulos)['response_data']['comercial_modulos_id'];

	foreach ($sisap as $key => $value) {
		if (mb_strpos($value, "Cliente Matriz")) {
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

	$clientematriz = new clientematriz();



	if ($clientematriz->response['response_status']['status'] == 1) {

		$getClienteMatriz = $clientematriz->getClienteMatriz();
		if ($getClienteMatriz['response_status']['status'] == 1) {

			$lista = $getClienteMatriz['response_data']['html'];
		} else {
			$response['response_status']['status'] = 0;
			$response['response_status']['error_code'] = $error_code . __LINE__;
			$response['response_status']['msg'] = $getClienteMatriz['response_status']['error_code'] . '::' . $getClienteMatriz['response_status']['msg'];
		}

		$getClienteMatrizCampos = $clientematriz->getClienteMatrizCampos();

		if ($getClienteMatrizCampos['response_status']['status'] == 1) {

			$grupos = $getClienteMatrizCampos['response_data']['grupos'];
			$listaModini = $getClienteMatrizCampos['response_data']['listaModini'];
			$listaG = $getClienteMatrizCampos['response_data']['listaG'];
			$listaGupdate = $getClienteMatrizCampos['response_data']['listaGupdate'];
		} else {
			$response['response_status']['status'] = 0;
			$response['response_status']['error_code'] = $error_code . __LINE__;
			$response['response_status']['msg'] = $getClienteMatrizCampos['response_status']['error_code'] . '::' . $getClienteMatrizCampos['response_status']['msg'];
		}
	} else {
		$response['response_status']['status'] = 0;
		$response['response_status']['error_code'] = $error_code . __LINE__;
		$response['response_status']['msg'] = $clienteusuarios->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
	}
} else {
	$response['response_status']['status'] = 0;
	$response['response_status']['error_code'] = $error_code . __LINE__;
	$response['response_status']['msg'] = 'Não foi possível encontrar o arquivo (class.clienteusuarios).';
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

	<!-- <link href="includes/less/styles.less" rel="stylesheet/less" media="all">  -->
	<link rel="stylesheet" href="includes/css/styles.css">
	<link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600' rel='stylesheet' type='text/css'>
	<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css">
	<link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='styleswitcher'>
	<link href='includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='headerswitcher'>
	<link rel="stylesheet" href="includes/plugins/datagrid/styles/jplist-custom.css">
	<link href="includes/plugins/select2/select2-4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />

	<!-- Plugins -->
	<script src="includes/js/jquery-3.4.1.min.js"></script>
	<link rel='stylesheet' type='text/css' href='includes/plugins/form-toggle/toggles.css' />
	<link href="includes/plugins/select2/select2-4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
	<link rel='stylesheet' type='text/css' href='includes/css/msg.css' />

	<script src="keycloak.js"></script>

	<style>
		td {
			word-break: break-all
		}

		#page-content {
			min-height: 100% !important;
		}
	</style>
</head>

<body onload="initKeycloak()" style="display: none;" class="horizontal-nav">
	<?php include('menu.php'); ?>


	<div id="page-container">

		<div id="page-content">
			<div id="wrap">
				<div class="container">
					<div class="row" style="display: flex;">
						<div class="col-md-1 img_container"> <!-- style="transform: translate(-13px, 13px);" -->
							<div class="container_logo_client">
								<img id="imglogo" class="img-circle">
							</div>
						</div>

						<div class="col-md-11 content_container">
							<table class="table table-striped">

								<tbody>
									<tr>
										<?php if ($thisModule->Criar == 1) : ?>
											<td><a id="btn_novo" data-toggle="modal" href="#myModal" class="btn default-alt  ">NOVO CLIENTE MATRIZ</a></td>
										<?php else : ?>

										<?php endif; ?>
									</tr>
								</tbody>
							</table>

							<!-- HISTÓRICO -->
							<div id="myModalHistorico" class="modal" tabindex="-1" role="dialog">
								<div class="modal-dialog" role="document">
									<div class="modal-content">
										<div id="tabela-historico"></div>

										<div class="modal-footer">
											<button data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
										</div>
									</div>
								</div>
							</div>

							<!-- MODAL UPDATE -->
							<div class="modal fade" id="updateModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
								<div class="modal-dialog">
									<div class="modal-content">
										<div class="modal-header">
											<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
											<h4 class="modal-title">Atualização de Cliente Matriz</h4>
										</div>
										<div class="modal-body">
											<div class="panel panel-primary">
												<div class="panel-heading">
													<h4>Novo Cadastro</h4>
													<div class="options">
														<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
													</div>
												</div>
												<div class="panel-body">
													<form class="form-horizontal">
														<input type="hidden" id="modulos_update">
														<input type="hidden" id="id-inputu" value="1">
														<div class="row" style="display: flex; align-items:flex-end">
															<div class="col-lg-3">
																<!-- <label for="logo-inputu" class="control-label">Logo</label> -->

																<p>Logo atual: </p><img id="logo-update" height="100" alt="...">
																<input type="file" class="form-control-file" id="logo-inputu">
															</div>
															<div class="col-lg-3">
																<label class="control-label">Grupo Econômico</label>
																<select class="form-control select2" id="ge-inputu">
																	<?php print $listaGupdate ?>
																</select>
															</div>

															<div class="col-lg-5">
																<label for="rs-inputu" class="control-label">Razão Social</label>
																<input type="text" class="form-control" id="rs-inputu" placeholder="">
															</div>

															<div class="col-lg-1">
																<label for="cod-inputu" class="control-label">Código</label>
																<input type="text" class="form-control" id="cod-inputu" placeholder="">
															</div>
														</div>

														<div class="row">

															<div class="col-lg-8">
																<label for="nome-inputu" class="control-label">Nome</label>
																<input type="text" class="form-control" id="nome-inputu" placeholder="">
															</div>

															<div class="col-lg-4">
																<label for="cnpj-inputu" class="control-label">CNPJ</label>
																<input type="text" class="form-control" id="cnpj-inputu" placeholder="00.000.000/0000-00">
															</div>
														</div>
														<div class="row">

															<div class="col-lg-3">
																<label for="end-inputu" class="control-label">Logradouro</label>
																<input type="text" class="form-control" id="end-inputu" placeholder="">
															</div>
															<div class="col-lg-3">
																<label for="bairro-inputu" class="control-label">Bairro</label>
																<input type="text" class="form-control" id="bairro-inputu" placeholder="">
															</div>
															<div class="col-lg-2">
																<label for="cep-inputu" class="control-label">CEP</label>
																<input type="text" class="form-control" id="cep-inputu" placeholder="00000-000">
															</div>

															<div class="col-lg-3">
																<label for="cid-inputu" class="control-label">Cidade</label>
																<input type="text" class="form-control" id="cid-inputu" placeholder="">
															</div>


															<div class="col-lg-1">
																<label for="uf-inputu" class="control-label">UF</label>
																<select class="form-control" id="uf-inputu">
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
																</select>
															</div>

														</div>

														<div class="row">

															<div class="col-lg-3">
																<label for="an-inputu" class="control-label">Definição Abertura da Negociação</label>
																<input type="text" class="col-sm-1 form-control" id="an-inputu" placeholder="00">
															</div>

															<div class="col-lg-3">
																<label class="control-label">SLA Prioridade Liberação</label>
																<select class="form-control" id="pri-inputu">
																	<option value="Documento Divulgado">Documento Divulgado</option>
																	<option value="Processa com Mapa Sindical">Processa com Mapa Sindical</option>
																	<option value="Processa com Comparativo de Cláusulas">Processa com Comparativo de Cláusulas</option>
																	<option value="Processa com Formulário">Processa com Formulário</option>
																</select>
															</div>

															<div class="col-lg-2">
																<label for="cla-inputu" class="control-label">Classe Documento</label>
																<input type="text" class="col-sm-1 form-control" id="cla-inputu" placeholder="">
															</div>

															<div class="col-lg-2">
																<label class="control-label">Tipo Documentação</label>
																<select class="form-control" id="td-inputu">
																	<optgroup label="SELECIONE">
																		<option value="CCT">CCT</option>
																		<option value="ACT">ACT</option>
																		<option value="CCT e ACT">CCT e ACT</option>
																	</optgroup>
																</select>
															</div>

															<div class="col-lg-2">
																<label for="proc-inputu" class="control-label">Processamento</label>
																<select class="form-control" id="proc-inputu">
																	<option value="assinado ou registrado">assinado ou registrado</option>
																	<option value="sem assinatura">sem assinatura</option>
																	<option value="somente registrado">somente registrado</option>
																</select>
															</div>
														</div>

														<div class="row">
															<div class="col-lg-4">
																<label for="ent-inputu" class="control-label">SLA Entrega</label>
																<select class="form-control" id="ent-inputu">
																	<optgroup label="SELECIONE">
																		<option value="02">02 dias úteis</option>
																		<option value="03">03 dias úteis</option>
																		<option value="04">03 dias úteis até 3 documentos ,
																			acima de 4 até 04 dias úteis</option>
																		<option value="10">10 dias úteis, Garantindo
																			fechamento FOPAG</option>
																	</optgroup>
																</select>
															</div>

															<div class="col-lg-4">
																<label for="corte-inputu" class="control-label">Data de
																	Corte FOPAG</label>
																<select class="form-control" id="corte-inputu">
																	<optgroup label="SELECIONE">
																		<option value="02">02</option>
																		<option value="05">05</option>
																		<option value="07">07</option>
																		<option value="08">08</option>
																		<option value="09">09</option>
																		<option value="10">10</option>
																		<option value="12">12</option>
																		<option value="13">13</option>
																		<option value="15">15</option>
																		<option value="18">18</option>
																		<option value="19">19</option>
																		<option value="20">20</option>
																		<option value="22">22</option>
																		<option value="23">23</option>
																		<option value="25">25</option>
																		<option value="28">28</option>
																		<option value="30">30</option>
																		<option value="15 (em Dez 13)">15 (em Dez 13)
																		</option>
																		<option value="15 a 20">15 a 20</option>
																		<option value="17 a 18">17 a 18</option>
																		<option value="17 a 20">17 a 20</option>
																		<option value="2º dia útil do mês">2º dia útil do
																			mês</option>
																		<option value="Data móvel por mês: Do dia 06">Data
																			móvel por mês: Do dia 06</option>
																		<option value="Dias finais ao pagamento">Dias finais
																			ao pagamento</option>
																		<option value="Segunda 5ª feira de cada mês">Segunda
																			5ª feira de cada mês</option>
																	</optgroup>
																</select>
															</div>

															<div class="col-sm-2">
																<label for="dataclu-inputu" class="control-label">Data Inclusão</label>
																<input type="text" disabled class="form-control datepicker" id="dataclu-inputu" placeholder="DD/MM/AAAA">
															</div>

															<div class="col-sm-2">
																<label for="dataina-inputu" class="control-label">Inativação</label>
																<input type="text" class="form-control datepicker" id="dataina-inputu" placeholder="DD/MM/AAAA">
															</div>

															<!-- <div class="col-sm-2">
																<label for="logo-inputu" class="control-label">Logo</label>
																<input type="file" class="form-control-file" id="logo-inputu">
																<p>Logo atual: </p><img id="logo-update" height="25" alt="...">
															</div> -->


														</div>
													</form>
												</div>

												<!-- Seleção de módulos -->
												<div id="teste-tabela">
													<!--INNER HTML PLS-->
												</div>

											</div>
										</div>
										<div class="modal-footer">
											<div class="row">
												<div class="col-sm-12" style="display: flex; justify-content:center">
													<div class="btn-toolbar">
														<?php if ($thisModule->Alterar == 1) : ?>
															<a id="btn-atualizar" href="#" class="btn btn-primary btn-rounded">Processar</a>
														<?php else : ?>
														<?php endif; ?>
														<a id="btn-cancelar2" href="#" class="btn btn-danger btn-rounded">Finalizar</a>
													</div>
												</div>
											</div>
										</div>
									</div><!-- /.modal-content -->
								</div><!-- /.modal-dialog -->
							</div><!-- /.modal -->

							<!-- MODAL DE CADASTRO -->
							<div class="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
								<div class="modal-dialog">
									<div class="modal-content">
										<div class="modal-header">
											<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
											<h4 class="modal-title">Cadastro de Cliente Matriz</h4>
										</div>
										<div class="modal-body">
											<div class="panel panel-primary">
												<div class="panel-heading">
													<h4>Novo Cadastro</h4>
													<div class="options">
														<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
													</div>
												</div>
												<div class="panel-body">
													<form class="form-horizontal">
														<div class="row">
															<div class="col-lg-3 required">
																<label class="control-label" for="ge-input">Grupo Econômico</label>
																<select class="form-control select2" id="ge-input">
																	<?php print $listaG ?>
																</select>
															</div>
															<div class="col-lg-7 required">
																<label for="rs-input" class="control-label">Razão Social</label>
																<input type="text" class="form-control" id="rs-input" placeholder="">
															</div>
															<div class="col-lg-2 required">
																<label for="cod-input" class="control-label">Código</label>
																<input type="text" class="form-control" id="cod-input" placeholder="">
															</div>
														</div>

														<div class="row">
															<div class="col-sm-8 required">
																<label for="nome-input" class="control-label">Nome</label>
																<input type="text" class="form-control" id="nome-input" placeholder="">
															</div>

															<div class="col-sm-4 required">
																<label for="cnpj-input" class="control-label">CNPJ</label>
																<input type="text" class="form-control" id="cnpj-input" placeholder="00.000.000/0000-00">
															</div>
														</div>

														<div class="row">

															<div class="col-sm-3 required">
																<label for="end-input" class="control-label">Logradouro</label>
																<input type="text" class="form-control" id="end-input" placeholder="">
															</div>
															<div class="col-sm-3 required">
																<label for="bairro-input" class="control-label">Bairro</label>
																<input type="text" class="form-control" id="bairro-input" placeholder="">
															</div>
															<div class="col-sm-2 required">
																<label for="cep-input" class="control-label">CEP</label>
																<input type="text" class="form-control" id="cep-input" placeholder="00000-000">
															</div>
															<div class="col-sm-3 required">
																<label for="cid-input" class="control-label">Cidade</label>
																<input type="text" class="form-control" id="cid-input" placeholder="">
															</div>

															<div class="col-sm-1">
																<label for="uf-input" class="control-label">UF</label>
																<select class="form-control" id="uf-input">
																	<option value=""></option>
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
																</select>
															</div>
														</div>

														<div class="row">
															<div class="col-sm-3 required">
																<label for="an-input" class="control-label">Definição Abertura da Negociação</label>
																<input type="text" class="col-sm-1 form-control" id="an-input" placeholder="00">
															</div>

															<div class="col-sm-3">
																<label class="control-label">SLA Prioridade Liberação</label>
																<select class="form-control" id="pri-input">
																	<option value=""></option>
																	<option value="Documento Divulgado">Documento Divulgado</option>
																	<option value="Processa com Mapa Sindical">Processa com Mapa Sindical</option>
																	<option value="Processa com Comparativo de Cláusulas"> Processa com Comparativo de Cláusulas</option>
																	<option value="Processa com Formulário">Processa com Formulário</option>
																</select>
															</div>

															<div class="col-sm-2">
																<label for="cla-input" class="control-label">Classe Documento</label>
																<input type="text" class="col-sm-1 form-control" id="cla-input" placeholder="">
															</div>

															<div class="col-sm-2">
																<label class="control-label">Tipo Documentação</label>
																<select class="form-control" id="td-input">
																	<option value=""></option>
																	<option value="CCT">CCT</option>
																	<option value="ACT">ACT</option>
																	<option value="CCT e ACT">CCT e ACT</option>
																</select>
															</div>

															<div class="col-sm-2">
																<label for="proc-input" class="control-label">Processamento</label>
																<select class="form-control" id="proc-input">
																	<option value=""></option>
																	<option value="assinado ou registrado">assinado ou registrado</option>
																	<option value="sem assinatura">sem assinatura</option>
																	<option value="somente registrado">somente registrado</option>
																</select>
															</div>
														</div>

														<div class="row">
															<div class="col-sm-4">
																<label for="ent-input" class="control-label">SLA Entrega</label>
																<select class="form-control" id="ent-input">
																	<optgroup label="SELECIONE">
																		<option value="" data-default disabled selected></option>
																		<option value="02">02 dias úteis</option>
																		<option value="03">03 dias úteis</option>
																		<option value="04">03 dias úteis até 3 documentos , acima de 4 até 04 dias úteis</option>
																		<option value="10">10 dias úteis, Garantindo fechamento FOPAG</option>
																	</optgroup>
																</select>
															</div>

															<div class="col-sm-3">
																<label for="corte-input" class="control-label">Data de Corte FOPAG</label>
																<select class="form-control" id="corte-input">
																	<option value=""></option>
																	<option value="02">02</option>
																	<option value="05">05</option>
																	<option value="07">07</option>
																	<option value="08">08</option>
																	<option value="09">09</option>
																	<option value="10">10</option>
																	<option value="12">12</option>
																	<option value="13">13</option>
																	<option value="15">15</option>
																	<option value="18">18</option>
																	<option value="19">19</option>
																	<option value="20">20</option>
																	<option value="22">22</option>
																	<option value="23">23</option>
																	<option value="25">25</option>
																	<option value="28">28</option>
																	<option value="30">30</option>
																	<option value="15 (em Dez 13)">15 (em Dez 13)</option>
																	<option value="15 a 20">15 a 20</option>
																	<option value="17 a 18">17 a 18</option>
																	<option value="17 a 20">17 a 20</option>
																	<option value="2º dia útil do mês">2º dia útil do mês</option>
																	<option value="Data móvel por mês: Do dia 06">Data móvel por mês: Do dia 06</option>
																	<option value="Dias finais ao pagamento">Dias finais ao pagamento</option>
																	<option value="Segunda 5ª feira de cada mês">Segunda 5ª feira de cada mês</option>
																</select>
															</div>

															<div class="col-sm-2">
																<label for="dataina-input" class="control-label">Data Inativação</label>
																<input type="date" class="form-control datepicker" id="dataina-input">
															</div>

															<div class="col-sm-2">
																<label for="logo-input" class="col-sm-1 control-label">Logo</label>
																<input type="file" class="form-control-file" id="logo-input">
															</div>
														</div>
													</form>
												</div>

												<!-- Seleção de módulos -->
												<div class="panel panel-primary">
													<div class="panel-heading">
														<h4>Seleção de Módulos</h4>
														<div class="options">
															<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
														</div>
													</div>
													<div class="panel-body collapse in">
														<button id="select_all" type="button" class="btn btn-primary" onclick="selecionarTodos()">Selecionar Todos</button>
														<button id="unselect_all" type="button" class="btn btn-primary" onclick="limparSelecao()">Limpar Seleção</button>
														<div id="grid-layout-table-2" class="box jplist">
															<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
															<div class="jplist-panel box panel-top">
																<button type="button" data-control-type="reset" data-control-name="reset" data-control-action="reset" class="jplist-reset-btn btn btn-primary">Limpar<i class="fa fa-share mls"></i></button>
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
																		<li><span data-path=".title" data-order="asc" data-type="text">Módulo A-Z</span></li>
																		<li><span data-path=".title" data-order="desc" data-type="text">Módulo Z-A</span></li>
																	</ul>
																</div>
																<div class="text-filter-box">
																	<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".title" type="text" value="" placeholder="Filtrar por nome do módulo" data-control-type="textbox" data-control-name="title-filter" data-control-action="filter" class="form-control" /></div>
																</div>
																<!-- <div class="text-filter-box">
																	<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Nome" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control"/></div>
																</div> -->
																<div data-type="Página {current} de {pages}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
																<!-- <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" class="jplist-pagination"></div> -->
															</div>
															<div class="box text-shadow">
																<table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered demo-tbl">
																	<thead>
																		<tr>
																			<th>Selecionar</th>
																			<th>Módulo</th>
																		</tr>
																	</thead>
																	<tbody id="sel-body-add">
																		<?php print $listaModini; ?>
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
												<input type="hidden" id="modulos-input" value="">
											</div>
										</div>
										<div class="modal-footer">
											<div class="row">
												<div class="col-sm-12" style="display: flex; justify-content:center">
													<a class="btn btn-primary btn-rounded" onclick="addClienteMatriz();">Processar</a>
													<a href="#" class="btn btn-danger btn-rounded btn_cancelar">Finalizar</a>
												</div>
											</div>
										</div>
									</div><!-- /.modal-content -->
								</div><!-- /.modal-dialog -->
							</div><!-- /.modal -->
						</div>
					</div>

					<div class="row" style="display: flex;">
						<div class="col-md-12">
							<div class="panel panel-primary">
								<div class="panel-heading">
									<h4>Cliente Matriz</h4>
									<div class="options">
										<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
									</div>
								</div>
								<div class="panel-body collapse in">
									<div id="grid-layout-table-1" class="box jplist">
										<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
										<div class="jplist-panel box panel-top">
											<button type="button" data-control-type="reset" data-control-name="reset" data-control-action="reset" class="jplist-reset-btn btn btn-primary">Limpar <i class="fa fa-share mls"></i></button>
											<div data-control-type="drop-down" data-control-name="paging" data-control-action="paging" class="jplist-drop-down form-control">
												<ul class="dropdown-menu">
													<li><span data-number="3"> 3 por página</span></li>
													<li><span data-number="5"> 5 por página</span></li>
													<li><span data-number="10" data-default="true"> 10 por página</span>
													</li>
													<li><span data-number="all"> ver todos</span></li>
												</ul>
											</div>
											<div data-control-type="drop-down" data-control-name="sort" data-control-action="sort" data-datetime-format="{month}/{day}/{year}" class="jplist-drop-down form-control">
												<ul class="dropdown-menu">
													<li><span data-path="default">Listar por</span></li>
													<li><span data-path=".title" data-order="asc" data-type="text">Grupo
															A-Z</span></li>
													<li><span data-path=".title" data-order="desc" data-type="text">Grupo Z-A</span></li>
													<li><span data-path=".desc" data-order="asc" data-type="text">Nome
															A-Z</span></li>
													<li><span data-path=".desc" data-order="desc" data-type="text">Nome
															Z-A</span></li>
												</ul>
											</div>
											<div class="text-filter-box">
												<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".title" type="text" value="" placeholder="Grupo Econômico" data-control-type="textbox" data-control-name="title-filter" data-control-action="filter" class="form-control" /></div>
											</div>
											<div class="text-filter-box">
												<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Nome" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control" /></div>
											</div>
											<div data-type="Página {current} de {pages}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary"></div>
											<!-- <div data-control-type="pagination" data-control-name="paging" data-control-action="paging" class="jplist-pagination"></div> -->
										</div>
										<div class="box text-shadow">
											<table class="table table-striped table-bordered demo-tbl">
												<thead>
													<tr>
														<th></th>
														<th>Grupo Econômico</th>
														<th>Nome</th>
														<th>CNPJ</th>
														<th>Data de Inclusão</th>
														<th>Data de Inativação</th>
														<th>Cidade</th>
														<th>UF</th>
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
				</div> <!-- container -->
			</div> <!-- #wrap -->
		</div> <!-- page-content -->


		<footer role="contentinfo">
			<div class="clearfix">
				<ul class="list-unstyled list-inline pull-left">
					<li>Ineditta &copy; 2022</li>
				</ul>
				<button class="pull-right btn btn-inverse-alt btn-xs hidden-print" id="back-to-top"><i class="fa fa-arrow-up"></i></button>
			</div>
		</footer>

	</div> <!-- page-container -->

	<!-- Template scripts -->
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
	<script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.js'></script>
	<script type='text/javascript' src='includes/plugins/datatables/TableTools.js'></script>
	<script type='text/javascript' src='includes/plugins/jquery-editable/jquery.editable.min.js'></script>
	<script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.js'></script>
	<script type='text/javascript' src='includes/plugins/datatables/dataTables.bootstrap.js'></script>
	<script type='text/javascript' src='includes/demo/demo-datatables.js'></script>
	<script type='text/javascript' src='includes/js/placeholdr.js'></script>
	<script type='text/javascript' src='includes/demo/demo-modals.js'></script>
	<script type='text/javascript' src='includes/js/application.js'></script>
	<script type='text/javascript' src='includes/demo/demo.js'></script>

	<!-- Plugins -->
	<script type='text/javascript' src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.15/jquery.mask.min.js"></script>
	<script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.4.1/js/bootstrap-datepicker.js"></script>
	<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.9.0/moment-with-locales.js"></script>
	<script src="includes/plugins/select2/select2-4.1.0-rc.0/dist/js/select2.min.js"></script>
	<script src="includes/plugins/sweet-alert/all.js"></script>


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

	<!-- Page scripts -->
	<script type='text/javascript' src="includes/js/clientematriz.js"></script>

</body>

</html>