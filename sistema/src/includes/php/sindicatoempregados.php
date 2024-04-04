<?php


/**
 * @author    {Enter5}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2020-08-28 13:40 ( v1.0.0 ) - 
	}
**/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÏ = 0



//header('charset=UTF-8; Content-type: text/html; Cache-Control: no-cache');

header ('Content-type: text/html; charset=UTF-8');
header('Cache-Control: no-cache');

$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Iniciando o processo.' ) );

// Montando o c??o do erro que ser?presentado
$localizar  = array( strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array( "", "", "", "", "-" );
$error_code = strtoupper( str_replace( $localizar, $substituir,  strtolower( __FILE__  ) ) ) . "-";

// Declarando os caminhos principais do sistema.
$localizar 	= array( "\\", "/includes/php" );	
$substituir	= array( "/", "" );
$path 		= str_replace( $localizar, $substituir, __DIR__);

//Localizando usuário logado no Windows
if (!empty($_SERVER['REMOTE_USER'])) {
    $nmloginweb = $_SERVER['REMOTE_USER'];
} elseif (!empty($_SERVER['LOGON_USER'])) {
    $nmloginweb = $_SERVER['LOGON_USER'];
} elseif (!empty($_SERVER['AUTH_USER'])) {
    $nmloginweb = $_SERVER['AUTH_USER'];
}

$fileClassSindical = $path . '/includes/php/class.sindicatoempregados.php';

if( file_exists( $fileClassSindical ) ){
	
	include_once( $fileClassSindical );
	
	$sindicatoempregados = new sindicatoempregados();

	if( $sindicatoempregados->response['response_status']['status'] == 1 ){
		
		$getSindicatoEmpregados = $sindicatoempregados->getSindicatoEmpregados();
		
		if( $getSindicatoEmpregados['response_status']['status'] == 1 ){
			
			$listaSindical = $getSindicatoEmpregados['response_data']['html'];
		}
		else{
			$response['response_status']['status']     = 0;
			$response['response_status']['error_code'] = $error_code . __LINE__;
			$response['response_status']['msg']        = $getSindical['response_status']['error_code'] . '::' . $getSindical['response_status']['msg'];
		}
	}
	else{
		$response['response_status']['status']     = 0;
		$response['response_status']['error_code'] = $error_code . __LINE__;
		$response['response_status']['msg']        = $sindicatoempregados->response['response_status']['error_code'] . '::' . $configform->response['response_status']['msg'];
	}
				
}
else{
	$response['response_status']['status']     = 0;
	$response['response_status']['error_code'] = $error_code . __LINE__;
	$response['response_status']['msg']        = 'Não foi possível encontrar o arquivo (class.sindicatoempregados).';
}

if( $response['response_status']['status'] == 0 ){
	
	print $response['response_status']['error_code'] . " :: " . $response['response_status']['msg'];
	exit();
}  

?>
<!DOCTYPE html>
<html lang="en">
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
    
    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries. Placeholdr.js enables the placeholder attribute -->
    <!--[if lt IE 9]>
        <script type="text/javascript" src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
        <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/respond.js/1.1.0/respond.min.js"></script>
        <script type="text/javascript" src="includes/plugins/charts-flot/excanvas.min.js"></script>
    <![endif]-->

    <!-- The following CSS are included as plugins and can be removed if unused-->
	<script src="includes/js/jquery-3.4.1.min.js"></script>
    <link rel='stylesheet' type='text/css' href='includes/plugins/form-toggle/toggles.css' /> 
    <!-- <script type="text/javascript" src="includes/js/less.js"></script> -->
	
	       <style type="text/css">
            ul li{
                float: left;
                position: relative; /* Necessário para o item filho se posicionar a LI que é o pai */
            }
             
            ul li ul{
                display: none;
                position: absolute;
            }
			
            ul li:hover ul{ /* Quando passar o mouse sobre a LI aparecerá o menu filho */
                width: 110px; /* Tamanho do menu filho */
                display: block; 
            }
            ul li ul li{
                width: 110px;
                cursor: pointer;
            }
            ul li ul li:hover > a{
                background-color: #0099FF;
                color: #FFFFFF;
            }
            ul li ul li ul li{
                display: none;
            }
            ul li ul li ul{
                top: 0; /* Para deixar o sub-menu na mesma linha do menu pai */
            }
            ul li ul li:hover ul li{
                display: block;
                width: 130px;
                left: 100%; /* Para deixar o sub-menu ao lado do menu pai */
            }
        </style>
</head>

<body class="horizontal-nav">
<?php include('menu.php'); ?>

    <div id="page-container">

<div id="page-content">
    <div id="wrap">
        <div class="container">
            <div class="row">
                <div class="col-md-1">
					<img id="imglogo"  class="img-circle"  >
                </div>

				<div class="col-md-11">
					<table class="table table-striped">
                         
                        <tbody>
                            <tr>
								<td><a data-toggle="modal" href="#myModal" class="btn default-alt  ">NOVO SINDICATO</a></td>
                            </tr>
                        </tbody>
                    </table>
					
					<div class="modal fade" id="updateModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
								<div class="modal-dialog">
									<div class="modal-content">
										<div class="modal-header">
											<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
											<h4 class="modal-title">Atualização do sindicato</h4>
										</div>
										<div class="modal-body">
				  <div class="panel panel-primary">
			 			<br>
						<form class="form-horizontal">
						<input type="hidden" id="id-inputu" value="10">
						  <div class="form-group">
						    <label for="sigla-inputu" class="col-sm-3 control-label">Sigla</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="sigla-inputu" placeholder="SIGLA">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="cnpj-inputu" class="col-sm-3 control-label">CNPJ</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="cnpj-inputu" placeholder="00.000.000-0000/00">
						    </div>
						  </div>
						  <div class="form-group">
						  	<label for="razaosocial-textareau" class="col-sm-3 control-label">Razão Social</label>
						  	<div class="col-sm-8"><textarea name="txtarea1" id="razaosocial-textareau" cols="50" rows="4" class="form-control"></textarea></div>
						  </div>
						  <div class="form-group">
						    <label for="cod-inputu" class="col-sm-3 control-label">Código</label>
						    <div class="col-sm-8">
						      <input type="number" class="form-control" id="cod-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="situacao-inputu" class="col-sm-3 control-label">Situação</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="situacao-inputu" placeholder="Normal">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="endereco-inputu" class="col-sm-3 control-label">Endereço</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="endereco-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="ddd-inputu" class="col-sm-3 control-label">DDD</label>
						    <div class="col-sm-8">
						      <input type="number" class="form-control" id="ddd-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="fone1-inputu" class="col-sm-3 control-label">Telefone 1</label>
						    <div class="col-sm-8">
						      <input type="number" class="form-control" id="fone1-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="fone2-inputu" class="col-sm-3 control-label">Telefone 2</label>
						    <div class="col-sm-8">
						      <input type="number" class="form-control" id="fone2-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="fone3-inputu" class="col-sm-3 control-label">Telefone 3</label>
						    <div class="col-sm-8">
						      <input type="number" class="form-control" id="fone3-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="denominacao-inputu" class="col-sm-3 control-label">Denominação</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="denominacao-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="enq-inputu" class="col-sm-3 control-label">Enquadramento</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="enq-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="email-inputu" class="col-sm-3 control-label">E-mail</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="email-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="site-inputu" class="col-sm-3 control-label">Site</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="site-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="pgdoc-inputu" class="col-sm-3 control-label">Page Doc</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="pgdoc-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="status-inputu" class="col-sm-3 control-label">Status</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="status-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="neg-inputu" class="col-sm-3 control-label">Negociador</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="neg-inputu" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="con-inputu" class="col-sm-3 control-label">Contribuição</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="con-inputu" placeholder="">
						    </div>
						  </div>
						  
				
						</form>
				      <div class="row">
				      		<div class="col-sm-6 col-sm-offset-3">
				      			<div class="btn-toolbar">
									<a id="btn-atualizar" href="#" class="btn btn-primary btn-rounded">Processar</a>
					      			<button class="btn-default btn">Cancelar</button>
				      			</div>
				      		</div>
				      	</div>
						
				       
				      	
				     
				  </div>

										</div>
										<!-- 
										<div class="modal-footer">
											<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
											<button type="button" class="btn btn-primary">Save changes</button>
										</div> -->
									</div><!-- /.modal-content -->
								</div><!-- /.modal-dialog -->
							</div><!-- /.modal -->

						<div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
								<div class="modal-dialog">
									<div class="modal-content">
										<div class="modal-header">
											<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
											<h4 class="modal-title">Cadastro de sindicatos</h4>
										</div>
										<div class="modal-body">
				  <div class="panel panel-primary">
			 			<br>
						<form class="form-horizontal">
						  <div class="form-group">
						    <label for="sigla-input" class="col-sm-3 control-label">Sigla</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="sigla-input" placeholder="SIGLA">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="cnpj-input" class="col-sm-3 control-label">CNPJ</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="cnpj-input" placeholder="00.000.000-0000/00">
						    </div>
						  </div>
						  <div class="form-group">
						  	<label for="razaosocial-textarea" class="col-sm-3 control-label">Razão Social</label>
						  	<div class="col-sm-8"><textarea name="txtarea1" id="razaosocial-textarea" cols="50" rows="4" class="form-control"></textarea></div>
						  </div>
						  <div class="form-group">
						    <label for="cod-input" class="col-sm-3 control-label">Código</label>
						    <div class="col-sm-8">
						      <input type="number" class="form-control" id="cod-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="situacao-input" class="col-sm-3 control-label">Situação</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="situacao-input" placeholder="Normal">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="endereco-input" class="col-sm-3 control-label">Endereço</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="endereco-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="ddd-input" class="col-sm-3 control-label">DDD</label>
						    <div class="col-sm-8">
						      <input type="number" class="form-control" id="ddd-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="fone1-input" class="col-sm-3 control-label">Telefone 1</label>
						    <div class="col-sm-8">
						      <input type="number" class="form-control" id="fone1-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="fone2-input" class="col-sm-3 control-label">Telefone 2</label>
						    <div class="col-sm-8">
						      <input type="number" class="form-control" id="fone2-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="fone3-input" class="col-sm-3 control-label">Telefone 3</label>
						    <div class="col-sm-8">
						      <input type="number" class="form-control" id="fone3-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="denominacao-input" class="col-sm-3 control-label">Denominação</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="denominacao-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="enq-input" class="col-sm-3 control-label">Enquadramento</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="enq-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="email-input" class="col-sm-3 control-label">E-mail</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="email-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="site-input" class="col-sm-3 control-label">Site</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="site-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="pgdoc-input" class="col-sm-3 control-label">Page Doc</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="pgdoc-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="status-input" class="col-sm-3 control-label">Status</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="status-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="neg-input" class="col-sm-3 control-label">Negociador</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="neg-input" placeholder="">
						    </div>
						  </div>
						  <div class="form-group">
						    <label for="con-input" class="col-sm-3 control-label">Contribuição</label>
						    <div class="col-sm-8">
						      <input type="text" class="form-control" id="con-input" placeholder="">
						    </div>
						  </div>
						  
				
						</form>
				      <div class="row">
				      		<div class="col-sm-6 col-sm-offset-3">
				      			<div class="btn-toolbar">
									<a href="#" class="btn btn-primary btn-rounded" onclick="addSindicatoEmpregados();">Processar</a>
					      			<button class="btn-default btn">Cancelar</button>
				      			</div>
				      		</div>
				      	</div>
						
				       
				      	
				     
				  </div>

										</div>
										<!-- 
										<div class="modal-footer">
											<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
											<button type="button" class="btn btn-primary">Save changes</button>
										</div> -->
									</div><!-- /.modal-content -->
								</div><!-- /.modal-dialog -->
							</div><!-- /.modal -->

                    <div class="panel panel-sky">
                        <div class="panel-heading">
                            <h4>Cadastro de Sindicatos</h4>
                            <div class="options">   
                                <a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
                            </div>
                        </div>
                        <div class="panel-body collapse in">
                            <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables" id="example">
                                <thead>
                                    <tr>
                                        <th></th>
                                        <th>Sigla</th>
                                        <th>CNPJ</th>
                                        <th>Endereço</th>
                                        <th>E-mail</th>
                                        <th>Telefone</th>
                                        <th>Site</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <?php print $listaSindical; ?>
                                </tbody>
                            </table>
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
<script type='text/javascript' src="includes/js/sindicatoempregados.js"></script>




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
<script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.min.js'></script> 
<script type='text/javascript' src='includes/plugins/datatables/TableTools.js'></script> 
<script type='text/javascript' src='includes/plugins/jquery-editable/jquery.editable.min.js'></script> 
<script type='text/javascript' src='includes/plugins/datatables/jquery.dataTables.min.js'></script> 
<script type='text/javascript' src='includes/plugins/datatables/dataTables.bootstrap.js'></script> 
<script type='text/javascript' src='includes/demo/demo-datatables.js'></script> 
<script type='text/javascript' src='includes/js/placeholdr.js'></script> 
<script type='text/javascript' src='includes/demo/demo-modals.js'></script> 
<script type='text/javascript' src='includes/js/application.js'></script> 
<script type='text/javascript' src='includes/demo/demo.js'></script> 

<!-- <script>!window.jQuery && document.write(unescape('%3Cscript src="includes/js/jquery-1.10.2.min.js"%3E%3C/script%3E'))</script>
<script type="text/javascript">!window.jQuery.ui && document.write(unescape('%3Cscript src="includes/js/jqueryui-1.10.3.min.js'))</script>
<script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
<script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script> -->


</body>
</html>