<?php
/**
 * @author    {Rafael P. Cruz}
 * @package   {1.0.0}
 * @description	{ }
**/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0

date_default_timezone_set('America/Sao_Paulo');

include __DIR__ . "/helpers.php";

class clientematriz{

	private static $tabela;

   public static function setTabela( $value ){
     self::$tabela = $value; //Works fine
    }

   public static function getTabela(){
     return self::$tabela;
   }

	
	// Retorno do construtor
    public $response;
	
	// Codigo de erro
    public $error_code;
	
	// Instancia do log4php
    private $logger;
	
	// Configurações do sistema
    private $getconfig;
	
	//conexão com banco de dados
	private $db;
	
	function __construct() {
		
		//Iniciando resposta padrão do construtor.
		$this->response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Classe ' . __CLASS__ . ' iniciada com sucesso.' ) );
		
		// Montando o código do erro que será apresentado
        $localizar  = array( strtolower(__DIR__), "/", "\\", ".php", ".");
        $substituir = array( "", "", "", "", "-" );
        $this->error_code = strtoupper( str_replace( $localizar, $substituir,  strtolower( __FILE__  ) ) ) . "-";
		
		// Declarando os caminhos principais do sistema.
		$localizar 	= array( "\\", "/includes/php" );	
		$substituir	= array( "/", "" );
		$this->path 		= str_replace( $localizar, $substituir, __DIR__);
		
		$fileLogApi = $this->path . '/includes/php/log4php/Logger.php';

		if( file_exists( $fileLogApi ) ){
			
			include_once( $fileLogApi );

			$fileLogConfig = $this->path . '/includes/config/config.log.xml';
			
			if( file_exists( $fileLogConfig ) ){
				//Informado as configuracoes do log4php
				Logger::configure( $fileLogConfig );	
				
				//Indica qual bloco do XML corresponde as configuracoes
				$this->logger = Logger::getLogger( 'config.log'  );
			}
			else{
				$this->response['response_status']['status'] 		= 0;
				$this->response['response_status']['error_code'] 	= $this->error_code . __LINE__;
				$this->response['response_status']['msg']			= "Não foi possível localizar as configurações do log.";
			}
		}
		else{
			$this->response['response_status']['status']     = 0;
			$this->response['response_status']['error_code'] = $this->error_code . __LINE__;
			$this->response['response_status']['msg']        = 'Não foi possível encontrar o plugins log4php.';
		}
		
		if( $this->response['response_status']['status'] == 1 ){
			
			$fileGetConfig = $this->path . "/includes/config/config.get.php";
			
			// Carregando as configuração do Mirrada
			if( file_exists( $fileGetConfig ) ){
				
				include_once( $fileGetConfig );
				
				$this->getconfig = new getconfig();
				
				if( $this->getconfig->response['response_status']['status'] == 0 ){
					$this->response = $this->getconfig->response;
				}
			}
			else{
				$this->response['response_status']['status']       = 0;
				$this->response['response_status']['error_code']   = $this->error_code . __LINE__;
				$this->response['response_status']['msg']          = 'Não foi possível localizar o arquivo de configuração (mirada-config).';
			}
		}
	}
	
	function connectdb(){
		
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			$qualitor_db = $this->getconfig->searchConfigDatabase( 'ineditta' );
				
			if( $qualitor_db['response_status']['status'] == 1 ){
				
				$parameters = $qualitor_db['response_data'];

				if( file_exists( $this->path . '/includes/php/db.mysql.php' ) ){
				
					include_once( $this->path . '/includes/php/db.mysql.php');

					// Criando o objeto de conexão com o banco de dados Qualitor
					$apidbmysql = new apidbmysql();

					$db = $apidbmysql->connection($parameters);
					
					if( $db['response_status']['status'] == 1 ){
						
						$this->db = $db['response_data']['connection'];
						
						$this->logger->debug( $db['response_data']['connection'] );
						
					}
					else{
						$response = $db;
					}
				}
				else{
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível encontrar o db.mysql.';
				}		
			}
			else{
				$response =  $qualitor_db;
			}
		}
		else{
			$response = $this->response;
		}
		
		return $response;
	}
	
	function getClienteMatriz( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
 
			$this->logger->debug(  $connectdb );
 
			if( $response['response_status']['status'] == 1 ){

				$sql = "
				SELECT 
							cm.id_empresa AS id_empresa
							,cm.nome_empresa AS nome_empresa
							,cm.cnpj_empresa AS cnpj_empresa
							,cm.cidade AS cidade
							,cm.uf AS uf
							,cm.tip_doc AS tip_doc
                            ,DATE_FORMAT(cm.data_inclusao,'%d/%m/%Y') AS data_inclusao
							,DATE_FORMAT(cm.data_inativacao,'%d/%m/%Y') AS data_inativacao
                            ,gp.nome_grupoeconomico as grupo_economico
						FROM 
							cliente_matriz cm
						INNER JOIN cliente_grupo gp WHERE gp.id_grupo_economico = cm.cliente_grupo_id_grupo_economico;								
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdClienteMatriz( '.$obj->id_empresa.');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td class="title">';
						$html .= $obj->grupo_economico;
						$html .= '</td>';
						$html .= '<td class="desc">';
						$html .= $obj->nome_empresa;
						$html .= '</td>';
						$html .= '<td>';
						$html .= formatCnpjCpf($obj->cnpj_empresa);
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->data_inclusao;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->data_inativacao;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->cidade;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->uf;
						$html .= '</td>';
						$html .= '</tr>';

					}	

					$response['response_data']['html'] 	= $html;

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}			

			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}

	function getClienteMatrizCampos( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
 
			//$this->logger->debug(  $connectdb );
 
			if( $response['response_status']['status'] == 1 ){





				$sql = "
					SELECT 
					id_grupo_economico
					,nome_grupoeconomico
                    ,logo_grupo
					FROM 
                    cliente_grupo;				
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					$htmlupdate = null;
					$optUpdate = null;
					$opt = "<option value=''> </option>";
					while($obj = $resultsql->fetch_object()){
						$opt .= "<option value='{$obj->id_grupo_economico}'>{$obj->nome_grupoeconomico}</option>";
						
						$optUpdate .= "<option value='{$obj->id_grupo_economico}'>{$obj->nome_grupoeconomico}</option>";
						
					}	

					$response['response_data']['listaG'] 	= $opt;
					$response['response_data']['listaGupdate'] 	= $optUpdate;



				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				
				$sql = "
				SELECT 
				m.id_modulos	
				,m.modulos
				,tipo
				
				FROM 
					modulos as m WHERE tipo like '%Comercial%';				
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$listaMod = null;
					
					while($obj = $resultsql->fetch_object()){
						$listaMod .= '<tr class="odd gradeX tbl-item">';
						$listaMod .= '<td><input onclick="addMod('.$obj->id_modulos.')" class="form-check-input check_modulo" data-id="'.$obj->id_modulos.'" type="checkbox" value="1" id="inicheck'.$obj->id_modulos.'"></td>';
						$listaMod .= '<td class="title">';
						$listaMod .= $obj->modulos;
						$listaMod .= '</td>';
						$listaMod .= '</tr>';
					}	

					$response['response_data']['listaModini'] 	= $listaMod;

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}









				$sql = "
				SELECT 
							id_grupo_economico
						    ,nome_grupoeconomico
						FROM 
							cliente_grupo;									
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$grupos = '<option value=""></option>';
					while($obj = $resultsql->fetch_object()){
						$grupos .= '<option value="';
						$grupos .= $obj->id_grupo_economico;
						$grupos .= '">';
						$grupos .= $obj->nome_grupoeconomico;
						$grupos .= '</option>';

					}	

					$response['response_data']['grupos'] 	= $grupos;

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}

	function getByIdClienteMatriz( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
 
			$this->logger->debug(  $connectdb );
 
			if( $response['response_status']['status'] == 1 ){

				$sql = "
				select
							id_empresa
							,codigo_empresa
							,razao_social
							,nome_empresa
							,cnpj_empresa
							,logradouro_empresa
							,bairro
							,cep
							,uf
							,cidade
							,abri_neg
							,logo_empresa
							,DATE_FORMAT(data_inclusao ,'%d/%m/%Y')  AS data_inclusao 
							,DATE_FORMAT(data_inativacao ,'%d/%m/%Y')  AS data_inativacao
							,tip_doc
							,cliente_grupo_id_grupo_economico
							,sla_entrega
							,sla_prioridade
							,tipo_processamento
							,classe_doc
							,data_cortefopag
						from 
							cliente_matriz
						where 
							id_empresa = {$data['id_empresa']};
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$obj = $resultsql->fetch_object();
					$response['response_data']['id_empresa'] 	= $obj->id_empresa;
					$response['response_data']['codigo_empresa'] 	= $obj->codigo_empresa;
					$response['response_data']['razao_social'] 	= $obj->razao_social;
					$response['response_data']['nome_empresa'] 	= $obj->nome_empresa;
					$response['response_data']['cnpj_empresa'] 	= formatCnpjCpf($obj->cnpj_empresa);
					$response['response_data']['logradouro_empresa'] 	= $obj->logradouro_empresa;
					$response['response_data']['bairro'] = $obj->bairro;
					$response['response_data']['cep'] 	= $obj->cep;
					$response['response_data']['uf'] 	= $obj->uf;
					$response['response_data']['cidade'] 	= $obj->cidade;
					$response['response_data']['abri_neg'] 	= $obj->abri_neg;
					$response['response_data']['data_inclusao'] 	= $obj->data_inclusao;
					$response['response_data']['data_inativacao'] 	= $obj->data_inativacao;
					$response['response_data']['tip_doc'] 	= $obj->tip_doc;
					$response['response_data']['logo_empresa'] 	= $obj->logo_empresa;
					$response['response_data']['sla_entrega'] 	= $obj->sla_entrega;
					$response['response_data']['sla_prioridade'] 	= $obj->sla_prioridade;
					$response['response_data']['tipo_processamento'] 	= $obj->tipo_processamento;
					$response['response_data']['classe_doc'] 	= $obj->classe_doc;
					$response['response_data']['data_cortefopag'] 	= $obj->data_cortefopag;
					$response['response_data']['cliente_grupo_id_grupo_economico'] 	= $obj->cliente_grupo_id_grupo_economico;
					
					

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}
				$idemp = $data['id_empresa'];
				$sql = "
					SELECT 
					m.id_modulos	
					,m.modulos
					,EXISTS( SELECT * FROM modulos_cliente WHERE
					data_fim <= str_to_date('00/00/00', '%d/%m/%Y') AND modulos_id_modulos =  m.id_modulos AND 
					cliente_matriz_id_empresa = {$data['id_empresa']}) as checked
					FROM 
						modulos as m WHERE tipo like '%Comercial%';				
				";
				$this->logger->debug(  $sql );
				
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$this->logger->debug(  $sql );

					$listaMod = '<div class="panel panel-primary">
					<div class="panel-heading">
						<h4>Seleção de Módulos</h4>
						<div class="options">   
							<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
						</div>
					</div>
					<div class="panel-body collapse in">
					
						<table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables" id="example">
							<thead>
								<tr>
									<th>Selecionar</th>
									
									<th>Módulo</th>
								</tr>
							</thead>
							<tbody id="sel-body">';
					
					while($obj = $resultsql->fetch_object()){
						$listaMod .= '<tr class="odd gradeX">';
						if($obj->checked){
							$listaMod .= '<td><input data-mod="'.$obj->id_modulos.'" class="form-check-input" onclick="saveModuleChange( '.$idemp.', '.$obj->id_modulos.');" type="checkbox" value="0" id="check'.$obj->id_modulos.'" checked></td>';
						} else {
							$listaMod .= '<td><input data-mod="'.$obj->id_modulos.'" class="form-check-input" onclick="saveModuleChange( '.$idemp.', '.$obj->id_modulos.');"  type="checkbox" value="1" id="check'.$obj->id_modulos.'"></td>';
						}
						
						
						$listaMod .= '<td>';
						$listaMod .= $obj->modulos;
						$listaMod .= '</td>';
						$listaMod .= '</tr>';
						$this->logger->debug(  $listaMod );
					}
					
					$listaMod .= '</tbody>
						</table>
						<a id="btn-historico" data-toggle="modal" href="#myModalHistorico" data-dismiss="modal" class="col-sm-2 btn btn-primary btn-rounded">Ver Histórico</a>
							</div>
						</div>';


					$response['response_data']['listaMod'] 	= $listaMod;
					$response['response_data']['listaMod2'] 	=  $listaMod;


				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao buscar modulos';
				}





				$sql = "
				select m.modulos,
				DATE_FORMAT(mc.data_inicio,'%d/%m/%Y')  AS data_inicio,
    if(DATE_FORMAT(mc.data_fim,'%d/%m/%Y') = DATE_FORMAT(0,'%d/%m/%Y'), \"Vigente\", DATE_FORMAT(mc.data_fim,'%d/%m/%Y') ) as data_fim,
				cm.nome_empresa
				from modulos_cliente as mc
				inner join cliente_matriz as cm ON cm.id_empresa = mc.cliente_matriz_id_empresa
				inner join modulos as m 
				where mc.modulos_id_modulos = m.id_modulos 
				and mc.cliente_matriz_id_empresa = {$data['id_empresa']}
				order by data_fim desc;			
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$ini = $resultsql->fetch_object();
					if($ini){
						$html = null;
					$html = '<div class="panel panel-primary">
					<div class="panel-heading">
						<h4>Histórico de Módulos contratados por ';
					$html .= $ini->nome_empresa;
					$html .= '</h4>
						<div class="options">   
							<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
						</div>
					</div>
					<div class="panel-body collapse in">
					
						<table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables" id="example">
							<thead>
								<tr>
									<th>Módulo</th>
									<th>Data de Início</th>
									<th>Data de Fim</th>
								</tr>
							</thead>
							<tbody>';
				

							$html .= '<tr class="odd gradeX">';
							$html .= '<td>';
							$html .= $ini->modulos;
							$html .= '</td>';
							$html .= '<td>';
							$html .= $ini->data_inicio;
							$html .= '</td>';
							$html .= '<td>';
							$html .= $ini->data_fim;
							$html .= '</td>';
							$html .= '</tr>';
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX">';
						$html .= '<td>';
						$html .= $obj->modulos;
						$html .= '</td>';
                        $html .= '<td>';
						$html .= $obj->data_inicio;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->data_fim;
						$html .= '</td>';
						$html .= '</tr>';


					}	
					$html .= '</tbody>
					</table>
				</div>
			</div>';

					$response['response_data']['listaHistorico'] 	= $html;

					}else{
						$response['response_data']['listaHistorico'] 	= '';
					}
					

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$response['response_data']['listaHistorico'] 	= '';			
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}	
	
	function addClienteMatriz( $data = null ){
		$this->logger->debug(  'entrou na classe php' );
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
 
			$this->logger->debug(  $data );
			if( $response['response_status']['status'] == 1 ){
				$dataInativacao = $data['dataina-input'] != "" ? $data['dataina-input'] : "0000-00-00";

				$sql = "INSERT INTO cliente_matriz
							(codigo_empresa
							,razao_social
							,nome_empresa
							,cnpj_empresa
							,logradouro_empresa
							,bairro
							,cep
							,uf
							,cidade
							,abri_neg
							,data_inativacao
							,tip_doc
							,cliente_grupo_id_grupo_economico
							,logo_empresa
							,sla_entrega
							,sla_prioridade
							,tipo_processamento
							,classe_doc
							,data_cortefopag)
						VALUES
							('{$data['cod-input']}',
							'{$data['rs-input']}',
							'{$data['nome-input']}', 
							'{$data['cnpj-input']}',
							'{$data['end-input']}',
							'{$data['bairro-input']}',
							'{$data['cep-input']}',
							'{$data['uf-input']}', 
							'{$data['cid-input']}',
							'{$data['an-input']}',
							'{$dataInativacao}',
							'{$data['td-input']}',
							'{$data['ge-input']}', 
							'{$data['logo-input']}', 
							'{$data['ent-input']}',
							'{$data['pri-input']}',
							'{$data['proc-input']}',
							'{$data['cla-input']}',
							'{$data['corte-input']}'
							);
				";
				$this->logger->debug( $sql );
				if( !mysqli_query( $this->db, $sql ) ){
					
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Não foi possivel realizar o cadastro!';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Cadastro realizado com sucesso';
				}

				$lastId = mysqli_insert_id($this->db);

				//CADASTRO DE MÓDULOS

				$modulos = strpos($data['modulos-input'], ",") ? explode(",", $data['modulos-input']) : explode(" ", $data['modulos-input']);

				foreach ($modulos as $modulo) {
					$today = date('Y-m-d');

					$sql = "INSERT INTO modulos_cliente
								(data_inicio, 
								data_fim, 
								modulos_id_modulos, 
								cliente_matriz_id_empresa)
							VALUES (
								'{$today}',
								'0000-00-00',
								'{$modulo}',
								'{$lastId}'
							)
								
					";
					
					if( !mysqli_query( $this->db, $sql ) ){
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = '';
						
						$this->logger->debug( $sql );
						$this->logger->debug( $this->db->error );
						$this->logger->debug( $response );
					}
					else{
						$this->logger->debug( $sql );
						$this->logger->debug( $this->db->error );
									
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = '';
					}
				}

				
				

			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}
	
	
	function saveModuleChange( $data = null ){
		$this->logger->debug(  'entrou na classe php' );
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
 
			$this->logger->debug(  $connectdb );

			
			if( $response['response_status']['status'] == 1 ){

				$today = date('Y-m-d');
				// if(($data['check']) == 0){
				
				foreach ($data['desmarcados'] as $moduloDesmarcado) {
					$sqlBusca = "SELECT * FROM modulos_cliente WHERE modulos_id_modulos = {$moduloDesmarcado} AND cliente_matriz_id_empresa = {$data['empresa']}";
					
					if (mysqli_query( $this->db, $sqlBusca )) {
						$sqlUp = "UPDATE modulos_cliente 
								SET data_fim = '{$today}'
								WHERE cliente_matriz_id_empresa = {$data['empresa']} and modulos_id_modulos = {$moduloDesmarcado};
						";
						mysqli_query( $this->db, $sqlUp );
					}
					
				}

				foreach ($data['modulos'] as $modulo) {

					$sqlBusca2 = "SELECT * FROM modulos_cliente WHERE modulos_id_modulos = '{$modulo}' AND cliente_matriz_id_empresa = '{$data['empresa']}'";
					$result = mysqli_query( $this->db, $sqlBusca2 );

					if (!$obj = $result->fetch_object()) {
						$sql = "INSERT INTO modulos_cliente
									(data_inicio,
									data_fim,
									modulos_id_modulos,
									cliente_matriz_id_empresa)
								VALUES
									('{$today}',
									'0000-00-00',
									'{$modulo}',
									'{$data['empresa']}')
						";

						if( !mysqli_query( $this->db, $sql ) ){
							
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = '';
							
							$this->logger->debug( $sql );
							$this->logger->debug( $this->db->error );
							$this->logger->debug( $response );
						}
						else{			
							$response['response_status']['status']       = 1;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Registros atualizados com sucesso';
						}
					}
				}
			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}	
	




	
	function updateClienteMatriz( $data = null ){
		$this->logger->debug(  'entrou na classe php' );
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
 
			$this->logger->debug(  $connectdb );
			if( $response['response_status']['status'] == 1 ){
				$sql = " UPDATE cliente_matriz
						SET 
							codigo_empresa = '{$data['cod-input']}'
							,razao_social = '{$data['rs-input']}'
							,nome_empresa = '{$data['nome-input']}'
							,cnpj_empresa = '{$data['cnpj-input']}'
							,logradouro_empresa = '{$data['end-input']}'
							,logo_empresa = '{$data['logo-input']}'
							,bairro = '{$data['bairro-input']}'
							,cep = '{$data['cep-input']}'
							,uf = '{$data['uf-input']}'
							,cidade = '{$data['cid-input']}'
							,abri_neg ='{$data['an-input']}'
							,data_inclusao = STR_TO_DATE('{$data['dataclu-input']}', '%d/%m/%Y')
							,data_inativacao = STR_TO_DATE('{$data['dataina-input']}', '%d/%m/%Y')
							,tip_doc = '{$data['td-input']}'
							,cliente_grupo_id_grupo_economico= {$data['ge-input']}
							,sla_entrega = '{$data['ent-input']}'
							,sla_prioridade = '{$data['pri-input']}'
							,tipo_processamento = '{$data['proc-input']}'
							,classe_doc = '{$data['cla-input']}'
							,data_cortefopag = '{$data['corte-input']}'
								WHERE 
									id_empresa = {$data['id_empresa']}; 
						";
				$this->logger->debug( $sql );
				if( !mysqli_query( $this->db, $sql ) ){
					
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}			

			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}	
	
	
}

?>