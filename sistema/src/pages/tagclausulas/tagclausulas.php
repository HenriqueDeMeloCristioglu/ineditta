<?php
session_start();
if (!$_SESSION) {
    echo "<script>document.location.href='http://localhost:8080/index.php'</script>";
}
$sessionUser = $_SESSION['login'];

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

    <link rel="stylesheet" href="tagclausulas.css">
    <link rel="stylesheet" href="includes/css/styles.css">

</head>

<style>
    #page-content {
        min-height: 100% !important;
    }

    .logo-circle {
        max-width: 100%;
    }

    .panel-heading-tabs {
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .panel-heading:before, .panel-heading:after {
        display: none;
    }

    .nav-tabs {
        margin-top: -4px;
    }
</style>

<body class="horizontal-nav">
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
                                    <?php if ($thisModule->Alterar == 1): ?>
                                            <a id="btn-atualizar" href="#" class="btn btn-primary btn-rounded">Processar</a>
                                    <?php else: ?>
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
                            <img id="imglogo" class="logo-circle">
                        </div>

                        <div class="col-md-11">
                            <div class="row">
                                <div class="col-md-12">
										<div class="panel-heading panel-heading-tabs">
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
															
															<a data-toggle="modal" href="#myModal" class="btn default-alt ">Adicionar Claúsula</a>
																															
															<p></p>
															<div class="panel-body collapse in">
																<div id="grid-layout-table-1" class="box jplist">
																	<div class="box text-shadow">
																		<table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered dataTable no-footer" id="estruturasClausulasTb" data-order='[[ 0, "asc" ]]'>
																			<thead>
																				<tr>
																					<th></th>
																					<th>Nome da Cláusula</th>
																					<th>Tipo</th>
																					<th>Classe</th>
																				</tr>
																			</thead>
																			<tbody>
																			    <tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( 1);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Abono</td><td class="desc">Tabela</td><td>S</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( 2);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Abrangência</td><td class="desc">Tabela</td><td>N</td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( 3);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Acidente do trabalho</td><td class="desc">Parâmetro</td><td>N</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( 4);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adaptação de função</td><td class="desc">Tabela</td><td>N</td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( 5);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adiantamento salarial</td><td class="desc">Parâmetro</td><td>S</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( 6);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adicional aprimoramento técnico</td><td class="desc">Tabela</td><td>S</td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( 7);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adicional de assiduidade</td><td class="desc">Tabela</td><td>S</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( 8);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adicional de função</td><td class="desc">Tabela</td><td>S</td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( 9);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adicional de horas extras</td><td class="desc">Tabela</td><td>S</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( 10);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adicional de insalubridade</td><td class="desc">Tabela</td><td>S</td></tr>
                                                                            </tbody>
																		</table>
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
																		<div data-control-type="drop-down" data-control-name="paging" data-control-action="paging" class="jplist-drop-down form-control"><div class="jplist-dd-panel"> 10 por página</div>
																			<ul class="dropdown-menu">
																				<li class=""><span data-number="3"> 3 por página</span></li>
																				<li><span data-number="5"> 5 por página</span></li>
																				<li class="active"><span data-number="10" data-default="true"> 10 por página</span></li>
																				<li><span data-number="all"> ver todos</span></li>
																			</ul>
																		</div>
																		<div data-control-type="drop-down" data-control-name="sort" data-control-action="sort" data-datetime-format="{month}/{day}/{year}" class="jplist-drop-down form-control"><div class="jplist-dd-panel">Listar por</div>
																			<ul class="dropdown-menu">
																				<li class="active"><span data-path="default">Listar por</span></li>
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
																			<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".title" type="text" value="" placeholder="Filtrar por cláusula" data-control-type="textbox" data-control-name="title-filter" data-control-action="filter" class="form-control"></div>
																		</div>
																		<div class="text-filter-box">
																			<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Filtrar por informação" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control"></div>
																		</div>
																		<div data-type="Página {current} de {pages}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary">Página 1 de 20</div>
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
																																							<tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( 1);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Abono</td><td class="desc">Grupo Geral Sistema legado, Grupo Abono</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( 2);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Abrangência</td><td class="desc"></td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( 3);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Acidente do trabalho</td><td class="desc">Grupo acidente com aux. prev. e estabilidade, Grupo acidente do trabalho</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( 4);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adaptação de função</td><td class="desc"></td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( 5);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adiantamento salarial</td><td class="desc">Grupo adiantamento salarial, Grupo Geral Sistema legado</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( 6);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adicional aprimoramento técnico</td><td class="desc">Grupo adicional aprimoramento técnico, Grupo Geral Sistema legado</td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( 7);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adicional de assiduidade</td><td class="desc">Grupo adicionais e auxílios</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( 8);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adicional de função</td><td class="desc">Grupo adicionais e auxílios</td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( 9);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adicional de horas extras</td><td class="desc">Grupo hora extra e banco de horas, Grupo Geral Sistema legado</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( 10);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">Adicional de insalubridade</td><td class="desc">Grupo adicional de insalub. e peric., Grupo adicional de insalubridade, Grupo Geral Sistema legado</td></tr></tbody>
																		</table>
																	</div>
																	<div class="box jplist-no-results text-shadow align-center jplist-hidden">
																		<p>Nenhum resultado encontrado</p>
																	</div>
																	<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
																	<div class="jplist-panel box panel-bottom">
																		<div data-type="{start} - {end} de {all}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary">1 - 10 de 198</div>
																		<div data-control-type="pagination" data-control-name="paging" data-control-action="paging" data-control-animate-to-top="true" class="jplist-pagination"><div class="jplist-pagingprev jplist-hidden" data-type="pagingprev"><button type="button" class="jplist-first" data-number="0" data-type="first">«</button><button type="button" class="jplist-prev" data-type="prev" data-number="0">‹</button></div><div class="jplist-pagingmid" data-type="pagingmid"><div class="jplist-pagesbox" data-type="pagesbox"><button type="button" data-type="page" class="jplist-current" data-active="true" data-number="0">1</button> <button type="button" data-type="page" data-number="1">2</button> <button type="button" data-type="page" data-number="2">3</button> <button type="button" data-type="page" data-number="3">4</button> <button type="button" data-type="page" data-number="4">5</button> <button type="button" data-type="page" data-number="5">6</button> <button type="button" data-type="page" data-number="6">7</button> </div></div><div class="jplist-pagingnext" data-type="pagingnext"><button type="button" class="jplist-next" data-type="next" data-number="1">›</button><button type="button" class="jplist-last" data-type="last" data-number="19">»</button></div></div>
																	</div>
																</div>
															</div>
														</div>
													</div>
												</div>
												<div class="tab-pane" id="codetabs3">
													<div class="row">
														<div class="form-group">
															
																																	<a data-toggle="modal" href="#myModalAddSinonimo" class="btn default-alt ">Adicionar Sinônimos</a>
																															
															<p></p>
															<div class="panel-body collapse in">
																<div id="grid-layout-table-3" class="box jplist">
																	<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
																	<div class="jplist-panel box panel-top">
																		<button type="button" data-control-type="reset" data-control-name="reset" data-control-action="reset" class="jplist-reset-btn btn btn-primary">Limpar <i class="fa fa-share mls"></i></button>
																		<div data-control-type="drop-down" data-control-name="paging" data-control-action="paging" class="jplist-drop-down form-control"><div class="jplist-dd-panel"> 10 por página</div>
																			<ul class="dropdown-menu">
																				<li class=""><span data-number="3"> 3 por página</span></li>
																				<li><span data-number="5"> 5 por página</span></li>
																				<li class="active"><span data-number="10" data-default="true"> 10 por página</span></li>
																				<li><span data-number="all"> ver todos</span></li>
																			</ul>
																		</div>
																		<div data-control-type="drop-down" data-control-name="sort" data-control-action="sort" data-datetime-format="{month}/{day}/{year}" class="jplist-drop-down form-control"><div class="jplist-dd-panel">Listar por</div>
																			<ul class="dropdown-menu">
																				<li class="active"><span data-path="default">Listar por</span></li>
																				<li><span data-path=".title" data-order="asc" data-type="text">Título A-Z</span></li>
																				<li><span data-path=".title" data-order="desc" data-type="text">Título Z-A</span></li>
																				<li><span data-path=".desc" data-order="asc" data-type="text">Tipo A-Z</span></li>
																				<li><span data-path=".desc" data-order="desc" data-type="text">Tipo Z-A</span></li>
																			</ul>
																		</div>
																		<div class="text-filter-box">
																			<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".title" type="text" value="" placeholder="Filtrar por sinônimos" data-control-type="textbox" data-control-name="title-filter" data-control-action="filter" class="form-control"></div>
																		</div>
																		<div class="text-filter-box">
																			<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input data-path=".desc" type="text" value="" placeholder="Filtrar por cláusula" data-control-type="textbox" data-control-name="desc-filter" data-control-action="filter" class="form-control"></div>
																		</div>
																		<div data-type="Página {current} de {pages}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary">Página 1 de 9</div>
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
																																							<tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( 4457);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">ABRANGÊNCIA DOS SALÁRIOS PROFISSIONAIS</td><td class="desc">Abrangência</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( 4458);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">AMBIENTAÇÃO NO TRABALHO</td><td class="desc">Admissão</td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( 4459);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">ANDAIMES DE MADEIRA</td><td class="desc">Saúde e Segurança do trabalho</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( 4460);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">APLICÁVEL SOMENTE PARA EMPRESAS NO INTERIOR DAS INDUSTRIAS - PLR</td><td class="desc">Participação nos lucros e/ou resultados</td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( 4461);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">ASSISTÊNCIA MÉDICA E ATESTADOS</td><td class="desc">Convênio médico</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( 4462);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">ATOS ANTISSINDICAIS PRATICADOS CONTRA EMPREGADOS (A) E A FEDERAÇÃO</td><td class="desc">Atividades sindicais</td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( 4463);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">ATRASO AO LOCAL DE TRABALHO</td><td class="desc">Ausências e Faltas</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( 4464);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">ATRASOS</td><td class="desc">Ausências e Faltas</td></tr><tr class="odd gradeX tbl-item even"><td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( 4465);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">CALCULOS INDENIZATÓRIOS</td><td class="desc">Cálculo de médias</td></tr><tr class="odd gradeX tbl-item"><td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( 4466);" class="btn-default-alt" id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td><td class="title">CAPACITAÇÃO PROFISSIONAL</td><td class="desc">Qualificação profissional</td></tr></tbody>
																		</table>
																	</div>
																	<div class="box jplist-no-results text-shadow align-center jplist-hidden">
																		<p>Nenhum resultado encontrado</p>
																	</div>
																	<div class="jplist-ios-button"><i class="fa fa-sort"></i>jPList Actions</div>
																	<div class="jplist-panel box panel-bottom">
																		<div data-type="{start} - {end} de {all}" data-control-type="pagination-info" data-control-name="paging" data-control-action="paging" class="jplist-label btn btn-primary">1 - 10 de 88</div>
																		<div data-control-type="pagination" data-control-name="paging" data-control-action="paging" data-control-animate-to-top="true" class="jplist-pagination"><div class="jplist-pagingprev jplist-hidden" data-type="pagingprev"><button type="button" class="jplist-first" data-number="0" data-type="first">«</button><button type="button" class="jplist-prev" data-type="prev" data-number="0">‹</button></div><div class="jplist-pagingmid" data-type="pagingmid"><div class="jplist-pagesbox" data-type="pagesbox"><button type="button" data-type="page" class="jplist-current" data-active="true" data-number="0">1</button> <button type="button" data-type="page" data-number="1">2</button> <button type="button" data-type="page" data-number="2">3</button> <button type="button" data-type="page" data-number="3">4</button> <button type="button" data-type="page" data-number="4">5</button> <button type="button" data-type="page" data-number="5">6</button> <button type="button" data-type="page" data-number="6">7</button> </div></div><div class="jplist-pagingnext" data-type="pagingnext"><button type="button" class="jplist-next" data-type="next" data-number="1">›</button><button type="button" class="jplist-last" data-type="last" data-number="8">»</button></div></div>
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
                                            <a href="#" class="btn btn-primary btn-rounded">Processar</a>
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
                                            <?php if ($thisModule->Alterar == 1): ?>
                                                    <a href="#" class="btn btn-primary btn-rounded">Processar</a>
                                            <?php else: ?>
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
                                            <a href="#" class="btn btn-primary btn-rounded">Processar</a>
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
        <?php require_once 'modalTagClausulasPasso2.php'; ?>

    </div> <!--page-content -->

    <?php include 'footer.php' ?>

    </div> <!--page-container -->
    
    <script type='text/javascript' src="./js/tagclausulas.min.js"></script>

</body>

</html>
