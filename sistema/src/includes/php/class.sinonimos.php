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


class sinonimos{
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
	
	function getSinonimos( $data = null ){

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
					sin.id_sinonimo,
					sin.nome_sinonimo,
					est.nome_clausula
				FROM sinonimos as sin
				LEFT JOIN estrutura_clausula as est on id_estruturaclausula = sin.estrutura_clausula_id_estruturaclausula;				
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#myModalUpdate" onclick="getByIdSinonimos( '.$obj->id_sinonimo.');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td class="title">';
						$html .= $obj->nome_sinonimo;
						$html .= '</td>';
						$html .= '<td class="desc">';
						$html .= $obj->nome_clausula;
						$html .= '</td>';
						$html .= '</tr>';
					}	

					$response['response_data']['sinonimos'] 			 = $html;
					$this->logger->debug( $response['response_data'] );
				}
				else{
					
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

	

	function getByIdSinonimos( $data = null ){
 
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

				$sql = "
					SELECT 
						sin.id_sinonimo,
						sin.nome_sinonimo,
						est.id_estruturaclausula,
						est.nome_clausula
					FROM sinonimos as sin
					LEFT JOIN estrutura_clausula as est on id_estruturaclausula = sin.estrutura_clausula_id_estruturaclausula
					WHERE sin.id_sinonimo = {$data['id_sinonimos']};				
				";
				
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$obj = $resultsql->fetch_object();

					$response['response_data']['id_estruturaclausula'] = $obj->id_estruturaclausula;
					$response['response_data']['nome_clausula']		    = $obj->nome_clausula;
					$response['response_data']['id_sinonimo']		    = $obj->id_sinonimo;
					$response['response_data']['nome_sinonimo']		    = $obj->nome_sinonimo;

					$this->logger->debug( $response);
				                   	
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
	
	

	

	function addSinonimos( $data = null ){
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
			if( $response['response_status']['status'] == 1 ){
				$sql = "insert into sinonimos
								(nome_sinonimo, estrutura_clausula_id_estruturaclausula)						
							values
								('{$data['info-inputc']}', '{$data['ge-input']}');
				";

				$this->logger->debug( $sql );
				
				if( !mysqli_query( $this->db, $sql )  ){
										
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

		
	
	function updateSinonimos( $data = null ){

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
				$sql = " UPDATE sinonimos
									SET  nome_sinonimo = '{$data['info-input-update']}', estrutura_clausula_id_estruturaclausula = '{$data['ge-input-update']}'  
									WHERE 
                                    id_sinonimo = {$data['input-id']};
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