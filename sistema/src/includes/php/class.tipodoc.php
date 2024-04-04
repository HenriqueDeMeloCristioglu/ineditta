<?php
/**
 * @author    {Rafael P. Cruz}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2021-07-02 15:39 ( v1.0.0 ) - 
	}
**/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


class tipodoc{
	
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
	
	function getTipoDoc( $data = null ){

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
 
			$this->logger->debug( $connectdb );
 
			if( $response['response_status']['status'] == 1 ){

				$sql = "
					SELECT 
						idtipo_doc	
						,tipo_doc
						,sigla_doc
						,nome_doc
					FROM 
						tipo_doc;				
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#updateTipoDoc" onclick="getByIdTipoDeDocum( '.$obj->idtipo_doc.' );" class="btn-default-alt"><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td>';
						$html .= $obj->sigla_doc;
						$html .= '</td>';
						$html .= '<td class="title">';
						$html .= $obj->tipo_doc;
						$html .= '</td>';
						$html .= '<td class="desc">';
						$html .= $obj->nome_doc;
						$html .= '</td>';
						$html .= '</tr>';
						$this->logger->debug( $obj );
					}	
					$this->logger->debug( $resultsql->fetch_object() );
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

	function getByIdTipoDeDocum( $data = null ){

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
							*
						FROM 
							tipo_doc
						WHERE
						    idtipo_doc = {$data['idtipo_doc']};
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$obj = $resultsql->fetch_object();

					$response['response_data']['idtipo_doc'] 	= $obj->idtipo_doc;

					$response['response_data']['tipo_doc'] 	= $obj->tipo_doc;
					$response['response_data']['sigla_doc'] 	= $obj->sigla_doc;
					$response['response_data']['nome_doc'] 	= $obj->nome_doc;
					$response['response_data']['modulo_cadastro'] 	= $obj->processado;
					
					$response['response_data']['val_inicial'] 	= $obj->validade_inicial;
					$response['response_data']['sind_laboral'] 	= $obj->sind_laboral;
					$response['response_data']['tipo_unidade'] 	= $obj->tipo_unid_cliente;
					$response['response_data']['assunto'] 	= $obj->refer_assunto;
					
					$response['response_data']['val_final'] 	= $obj->validade_final;
					$response['response_data']['sind_patronal'] 	= $obj->sind_patronal;
					$response['response_data']['abrangencia'] 	= $obj->abrangencia;
					$response['response_data']['descricao'] 	= $obj->descricao_doc;
					
					$response['response_data']['origem'] 	= $obj->origem;
					$response['response_data']['data_base'] 	= $obj->data_base;
					$response['response_data']['atividade_eco'] 	= $obj->atividade_economica;
					$response['response_data']['num_legislacao'] 	= $obj->numero_leg;
					
					$response['response_data']['versao'] 	= $obj->versao;
					$response['response_data']['permitir_compar'] 	= $obj->permitir_compartilhar;
					$response['response_data']['estabele'] 	= $obj->estabelecimento;
					$response['response_data']['fonte_lesgi'] 	= $obj->fonte_leg;
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
	
	function addTipoDoc( $data = null ){
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
				$sql = "INSERT INTO tipo_doc
							(tipo_doc
							,nome_doc
							,sigla_doc
							,processado
							
							,validade_inicial
							,sind_laboral
							,tipo_unid_cliente
							,refer_assunto
							
							,validade_final
							,sind_patronal
							,abrangencia
							,descricao_doc
							
							,origem
							,data_base
							,atividade_economica
							,numero_leg
							
							,versao
							,permitir_compartilhar
							,estabelecimento
							,fonte_leg)
						VALUES					

							('{$data['tipo-input']}'
							,'{$data['nome-input']}'
							,'{$data['sigla-input']}'
							,'{$data['modulo-input']}'

							,'{$data['val-inicial-input']}'
							,'{$data['sind-laboral-input']}'
							,'{$data['tipo-unidade-input']}'
							,'{$data['assunto-input']}'

							,'{$data['val-final-input']}'
							,'{$data['sind-patronal-input']}'
							,'{$data['abrangencia-input']}'
							,'{$data['descricao-input']}'

							,'{$data['origem-input']}'
							,'{$data['data-base-input']}'
							,'{$data['atividade-eco-input']}'
							,'{$data['num-legislacao-input']}'

							,'{$data['versao-input']}'
							,'{$data['permitir-compar-input']}'
							,'{$data['estabele-input']}'
							,'{$data['fonte-lesgi-input']}')
				";

				$this->logger->debug( $sql );
				if( !mysqli_query( $this->db, $sql ) ){
										
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Falha ao efetuar registro.';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}else{
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = null;
					$response['response_status']['msg']          = '';
				}			

			}else{
				$response = $this->response;
			}			
		}else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}	
	
	function updateTipoDoc( $data = null ){
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
				$sql = " UPDATE tipo_doc  
									SET  
									 tipo_doc = '{$data['tipo-input-up']}'
									,sigla_doc = '{$data['sigla-input-up']}'
									,nome_doc = '{$data['nome-input-up']}'
									,processado = '{$data['modulo-input-up']}'

									,validade_inicial = '{$data['val-inicial-input-up']}'
									,sind_laboral = '{$data['sind-laboral-input-up']}'
									,tipo_unid_cliente = '{$data['tipo-unidade-input-up']}'
									,refer_assunto = '{$data['assunto-input-up']}'

									,validade_final = '{$data['val-final-input-up']}'
									,sind_patronal = '{$data['sind-patronal-input-up']}'
									,abrangencia = '{$data['abrangencia-input-up']}'
									,descricao_doc = '{$data['descricao-input-up']}'

									,origem = '{$data['origem-input-up']}'
									,data_base = '{$data['data-base-input-up']}'
									,atividade_economica = '{$data['atividade-eco-input-up']}'
									,numero_leg = '{$data['num-legislacao-input-up']}'

									,versao = '{$data['versao-input-up']}'
									,permitir_compartilhar = '{$data['permitir-compar-input-up']}'
									,estabelecimento = '{$data['estabele-input-up']}'
									,fonte_leg = '{$data['fonte-lesgi-input-up']}'
									WHERE 
										idtipo_doc = {$data['idtipo_doc']};
						";
				$this->logger->debug( $sql );
				if( !mysqli_query( $this->db, $sql ) ){
										
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Falha ao efetuar registro.';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}else{
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = null;
					$response['response_status']['msg']          = '';
				}			

			}else{
				$response = $this->response;
			}			
		}else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}
}

?>