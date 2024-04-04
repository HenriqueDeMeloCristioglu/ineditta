<?php
/**
 * @author    {Lucas A. Rodrigues Volpati}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2022-06-21 16:40 ( v1.0.0 ) - 
	}
**/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


class indecon{
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
	
	function getLists( $data = null ){

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
				mysqli_query($this->db, "SET lc_time_names = 'pt_BR';");
				$sql = "SELECT DISTINCT origem, id_usuario, indicador, fonte, DATE_FORMAT(data, '%b/%Y') as periodo FROM indecon;";

				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					
					$html = "";
					while ($obj = $resultsql->fetch_object()) {
						if ($obj->indicador == "INPC") {
							$indicador = 1;
						}else {
							$indicador = 2;
						}
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#myModalUpdate" onclick="getById('.$indicador.', '.$obj->id_usuario.');" class="btn-default-alt"  ><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td class="title">'.$obj->indicador.'</td>';
						$html .= '<td class="desc">'.$obj->origem.'</td>';
						$html .= '<td>'.$obj->fonte.'</td>';
						$html .= '<td>'.$obj->periodo.'</td>';
						$html .= '</tr>';
					}

					$response['response_data']['listaPrincipal'] 	= $html;
					mysqli_query($this->db, "SET lc_time_names = 'pt_BR'");
					$sql2 = "SELECT id, indicador, dado_real, DATE_FORMAT(periodo_data, '%b/%Y') as periodo_data FROM indecon_real";
					$resultsql2 = mysqli_query( $this->db, $sql2 );

					$real = "";
					while ($obj2 = $resultsql2->fetch_object()) {
						
						$real .= '<tr class="odd gradeX tbl-item">';
						$real .= '<td><a data-toggle="modal" href="#myModalUpdateReal" onclick="getByIdReal('.$obj2->id.');" class="btn-default-alt"  ><i class="fa fa-file-text"></i></a></td>';
						$real .= '<td class="title">'.$obj2->indicador.'</td>';
						$real .= '<td>'.$obj2->dado_real.'</td>';
						$real .= '<td class="desc">'.$obj2->periodo_data.'</td>';
						$real .= '</tr>';
					
					}

					$response['response_data']['real'] = $real;

					

					$this->logger->debug( $obj);

					//LISTA GRUPO ECONOMICO
					$sql = "SELECT 
								id_grupo_economico as id,
								nome_grupoeconomico as nome
							FROM cliente_grupo
					";

					$result = mysqli_query($this->db, $sql);

					$opt = '<option value="0"></option>';
					while ($obj = $result->fetch_object()) {
						$opt .= "<option value='{$obj->id}'>{$obj->nome}</option>";
					}

					$response['response_data']['grupo_list'] = $opt;
									
				}
				else{
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Falha ao efetuar SELECT.';
				}
				
			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		return $response;
	}


	function getById( $data = null ){
 
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
  
			if( $response['response_status']['status'] == 1 ){

				if ($data['indicador'] == 1) {
					$indicador = "INPC";
				}else {
					$indicador = "IPCA";
				}
				mysqli_query($this->db, "SET lc_time_names = 'pt_BR'");
				$sql = "SELECT
							id_indecon,
							indicador, 
							id_usuario,
							origem, 
							fonte, 
							DATE_FORMAT(data, '%b/%Y') as data, 
							dado_projetado, 
							dado_real, 
							cliente_grupo_id_grupo_economico as clt,
							gp.nome_grupoeconomico
						FROM
							indecon as ind
						LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = ind.cliente_grupo_id_grupo_economico

						WHERE id_usuario = '{$data['id_usuario']}' AND indicador = '{$indicador}'
									
					";
				
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$label = '<label for="origem" class="control-label">Selecione o Período</label>';

					$select = '<select id="periodo-select" class="form-control">';
					$select .= '<option value="0">--</option>';

					while ($obj = $resultsql->fetch_object()) {
						$this->logger->debug( $obj);

						$select .= '<option value="'.$obj->id_indecon.'">'.$obj->data.'</option>';

						$response['response_data']['indicador'] = $obj->indicador;       	
						$response['response_data']['origem'] = $obj->origem;       	
						$response['response_data']['fonte'] = $obj->fonte;       	
						$response['response_data']['dado_projetado'] = $obj->dado_projetado;       	
						$response['response_data']['cliente'] = !$obj->nome_grupoeconomico ? "--" : $obj->nome_grupoeconomico;  
					}

					$select .= '</select>';
					
					$response['response_data']['label'] = $label;       	
					$response['response_data']['periodo'] = $select;  
	     	
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
		$this->logger->debug( $response['response_data'] );
		
		return $response;
	}	

	function getRegisterById( $data = null ){
 
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
  
			if( $response['response_status']['status'] == 1 ){

				$sql = "SELECT
							id_indecon,
							indicador,
							dado_projetado, 
							dado_real,
							data as periodo
						FROM
							indecon
						WHERE
						id_indecon = '{$data['id_indecon']}'
									
					";
				
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					while ($obj = $resultsql->fetch_object()) {
						$this->logger->debug( $obj);

						$response['response_data']['projetado'] = $obj->dado_projetado;       	
						$response['response_data']['id_indice'] = $obj->id_indecon;

						
						$sqlReal = "SELECT * FROM indecon_real WHERE periodo_data = '{$obj->periodo}' AND indicador = '{$obj->indicador}' ";
						$resultReal = mysqli_query( $this->db, $sqlReal );
						$objReal = $resultReal->fetch_object();

						$response['response_data']['real'] = $objReal->dado_real; 

						$this->logger->debug( $objReal);
						
					}

					     	
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
		$this->logger->debug( $response['response_data'] );
		
		return $response;
	}	


	function getByIdReal( $data = null ){
 
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
  
			if( $response['response_status']['status'] == 1 ){

				
				$sql = "SELECT
							*
						FROM
							indecon_real
						WHERE
							id = {$data['id_real']}
									
					";
				$this->logger->debug( $sql );
				
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					
					$obj = $resultsql->fetch_object();
					
					$response['response_data']['indicador_update'] = $obj->indicador;       	
					$response['response_data']['dado_real'] = $obj->dado_real;  
					$response['response_data']['periodo_update'] = $obj->periodo_data;  

					$response['response_status']['status']       = 1;


					$this->logger->debug( $obj );
					     	
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
		$this->logger->debug( $response['response_data'] );
		
		return $response;
	}	


	function addIndicadores( $data = null ){
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

 
			$this->logger->debug(  $this->db );

			$lineOne = $data['tabela-fixa'];
			$lineOthers = $data['tabela-extra'];

			if( $response['response_status']['status'] == 1 ){
				
				if ($lineOne) {

					
					// if ($data['origem-input'] == "Ineditta") {
					// 	$id_usuario = 1;
					// }else {
					// 	$id_usuario = 2;
					// }
					$cliente = $data['cliente-input'] ? $data['cliente-input'] : 0;
					
					$sql = "INSERT INTO
							indecon (indicador, id_usuario,  origem, fonte, data, dado_projetado, cliente_grupo_id_grupo_economico)
						VALUES
							('{$data['indicador-input']}', 
							'{$data['usuario']}',
							'{$data['origem-input']}', 
							'{$data['fonte-input']}', 
							'{$lineOne[0]}',
							'{$lineOne[1]}',
							'{$cliente}')
					";
	
					$this->logger->debug( $sql );
	
					if( !mysqli_query( $this->db, $sql )  ){
											
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Falha ao efetuar registro.';
						
						$this->logger->debug( $this->db->error );
					}
					else{	 
						$response['response_status']['status']       = 1;
						$response['response_status']['error_code']   = null;
						$response['response_status']['msg']          = 'Cadastro realizado com sucesso';
					}	
				}
	
				if ($lineOthers) {
	
					$list = array_chunk($lineOthers, 2);

					if ($data['origem-input'] == "Ineditta") {
						$id_usuario = 1;
					}else {
						$id_usuario = 2;
					}

					
					
					for ($i=0; $i < count($list) ; $i++) { 

						$sql2 = "INSERT INTO
									indecon (indicador, id_usuario, origem, fonte, data, dado_projetado, cliente_matriz_id_empresa)
								VALUES
									('{$data['indicador-input']}', 
									'{$id_usuario}',
									'{$data['origem-input']}', 
									'{$data['fonte-input']}', 
									'{$list[$i][0]}',
									'{$list[$i][1]}',
									'{$data['cliente-input']}')
						";
						$this->logger->debug( $sql2 );
	
						if( !mysqli_query( $this->db, $sql2 )  ){
												
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Falha ao efetuar registro.';
							
							$this->logger->debug( $this->db->error );
						}
						else{								
							$response['response_status']['status']       = 1;
							$response['response_status']['error_code']   = null;
							$response['response_status']['msg']          = 'Cadastro realizado com sucesso';
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


	function addIndicadorReal( $data = null ){
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

 
			$this->logger->debug(  $this->db );

			$lineOne = $data['tabela-fixa'];
			$lineOthers = $data['tabela-extra'];

			if( $response['response_status']['status'] == 1 ){
				
				if ($lineOne) {

					$sql = "INSERT INTO
							indecon_real (indicador, periodo_data, dado_real)
						VALUES
							('{$data['indicador-input']}',
							'{$lineOne[0]}',
							'{$lineOne[1]}')
					";
	
					$this->logger->debug( $sql );
	
					if( !mysqli_query( $this->db, $sql )  ){
											
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Falha ao efetuar registro.';
						
						$this->logger->debug( $this->db->error );
					}
					else{	 
						$response['response_status']['status']       = 1;
						$response['response_status']['error_code']   = null;
						$response['response_status']['msg']          = 'Cadastro realizado com sucesso';
					}	
				}
	
				if ($lineOthers) {
	
					$list = array_chunk($lineOthers, 2);
					
					for ($i=0; $i < count($list) ; $i++) { 

						$sql2 = "INSERT INTO
									indecon_real (indicador, periodo_data, dado_real)
								VALUES
									('{$data['indicador-input']}', 
									'{$list[$i][0]}',
									'{$list[$i][1]}')
						";
						$this->logger->debug( $sql2 );
	
						if( !mysqli_query( $this->db, $sql2 )  ){
												
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Falha ao efetuar registro.';
							
							$this->logger->debug( $this->db->error );
						}
						else{								
							$response['response_status']['status']       = 1;
							$response['response_status']['error_code']   = null;
							$response['response_status']['msg']          = 'Cadastro realizado com sucesso';
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

		
	
	function updateIndicador( $data = null ){

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
 

			if( $response['response_status']['status'] == 1 ){
				$sql = "UPDATE 
							indecon
						SET  
							fonte = '{$data['fonte-up']}', 
							dado_projetado = '{$data['projetado-up']}'
						WHERE 
							id_indecon = {$data['id_indecon']};
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
								
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Registro atualizado com sucesso';
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


	function updateIndicadorReal( $data = null ){

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
 

			if( $response['response_status']['status'] == 1 ){
				$sql = "UPDATE 
							indecon_real
						SET  
							indicador = '{$data['indicador-up']}',
							dado_real = '{$data['real-up']}',
							periodo_data = '{$data['periodo-up']}'
						WHERE 
							id = {$data['id_real']};
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
								
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Registro atualizado com sucesso';
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


	function getPrincipalListByUser( $idGrupo ){

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
  
			if( $response['response_status']['status'] == 1 ){

				$sql = "SELECT 
							DISTINCT origem, 
							id_usuario, 
							indicador,
							cliente_grupo_id_grupo_economico
						FROM indecon
						WHERE cliente_grupo_id_grupo_economico = {$idGrupo}
				";

				$result = mysqli_query($this->db, $sql);

					
				$html = "";
				while ($obj = $result->fetch_object()) {
					if ($obj->indicador == "INPC") {
						$indicador = 1;
					}else {
						$indicador = 2;
					}
					$html .= '<tr class="odd gradeX tbl-item">';
					$html .= '<td><a data-toggle="modal" href="#myModalUpdate" onclick="getById('.$indicador.', '.$obj->id_usuario.');" class="btn-default-alt"  ><i class="fa fa-file-text"></i></a></td>';
					$html .= '<td class="title">'.$obj->indicador.'</td>';
					$html .= '<td class="desc">'.$obj->origem.'</td>';
					$html .= '</tr>';
				}


			}			
		}

		$this->logger->debug( $html );
		
		return $html;
	}


	function getGrupo( $idGrupo ){

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
  
			if( $response['response_status']['status'] == 1 ){

				//LISTA GRUPO ECONOMICO
				$sql = "SELECT 
							id_grupo_economico as id,
							nome_grupoeconomico as nome
						FROM cliente_grupo
						WHERE id_grupo_economico = {$idGrupo}
				";

				$this->logger->debug( $sql );

				$result = mysqli_query($this->db, $sql);

				$obj = $result->fetch_object();

				$this->logger->debug( $obj );

				$input = "<option value='{$obj->id}'>{$obj->nome}</option>";

			}			
		}

		$this->logger->debug( $input );
		
		return $input;
	}

	function deleteIndicador( $data = null ){

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

			if( $response['response_status']['status'] == 1 ){

				$sql = "DELETE FROM indecon WHERE id_indecon = {$data['id']}";

				if (mysqli_query($this->db, $sql)) {
					$response['response_status']['status'] = 1;
					$response['response_status']['msg'] = "Registro escluído com sucesso!";

				}else {
					$response['response_status']['status'] = 0;
					$response['response_status']['msg'] = "Falha ao excluir registro!";
					$response['response_status']['error'] = $this->db->error;

				}

			}
		}

		return $response;
	}
}
?>
