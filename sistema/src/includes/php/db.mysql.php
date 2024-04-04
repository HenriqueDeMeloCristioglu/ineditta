<?php
/**
 * apidbmssql class
 * @author    {Rafael P. Cruz}
 * @copyright {Enter5® 2021}
 * @package   {1.0.1}
 * @description	{ }
 * @historic {
		2021/07/02 15:36 ( v1.0.0 ) - xx
	}
**/

class apidbmysql{
	
	// Retorno do construtor
    public $response;
    
	// Código do erro
    public $error_code;
	
	// Instancia do log4php
    private $logger;
		
	function __construct(){
		
		// Iniciando resposta padrão do construtor.
		$this->response = array( 'response_status' => array( 'status' => 1, 'error_code' => null, 'msg' => 'Classe ' . __CLASS__ . ' iniciada com sucesso.' ) );
		
		// Montando o código do erro que será apresentado
        $localizar  = array( strtolower(__DIR__), "/", "\\", ".php", ".");
        $substituir = array( "", "", "", "", "-" );
        $this->error_code = strtoupper( str_replace( $localizar, $substituir,  strtolower( __FILE__  ) ) ) . "-";
		
		// Declarando os caminhos principais do sistema.
		$localizar 	= array( "\\", "/includes/php" );	
		$substituir	= array( "/", "" );
		$path 		= str_replace( $localizar, $substituir, __DIR__);
		
		// Incluindo os script de apoio
		if( $this->response['response_status']['status'] == 1 ){
            
			$fileLogApi = $path . '/includes/php/log4php/Logger.php';
			
			if( file_exists( $fileLogApi ) ){
                
                include_once( $fileLogApi );

				$fileLogConfig = $path . '/includes/config/config.log.xml';
				
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
        }
	}
	
	function connection( $parameters = null ){
		
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( !isset( $parameters->database->host  ) || empty( $parameters->database->host ) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro ( parameters > database > host ) é obrigatório.';
			}
			
			if( !isset( $parameters->database->user  ) || empty( $parameters->database->user ) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro ( parameters > database > user ) é obrigatório.';
			}
			
			if( !isset( $parameters->database->password  ) || empty( $parameters->database->password ) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro ( parameters > database > password ) é obrigatório.';
			}
			
			if( !isset( $parameters->database->dbName  ) || empty( $parameters->database->dbName ) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro ( parameters > database > dbName ) é obrigatório.';
			}
			
			// Iniciando a rotina para listar os chamados para atualização.
			if( $response['response_status']['status'] == 1 ){
								
				$connection = new mysqli( $parameters->database->host, $parameters->database->user, $parameters->database->password, $parameters->database->dbName );
					
				if( $connection ) {
					
					$response['response_status']['status']      = 1;
					$response['response_status']['error_code']  = null;
					$response['response_status']['msg']         = 'Conexão com o banco de dados ' . $parameters->name . ' estabelecida com sucesso.';
					$response['response_data']['connection']	= $connection;
				}
				else{
					// Capturando o erro da conexão com o banco de dados
					$errors = mysqli_connect_errno();
					
					// Resposta da solicitação
					$response['response_status']['status']		= 0;
					$response['response_status']['error_code']	= $this->error_code . __LINE__;
					$response['response_status']['msg']       	= 'Ocorreu um erro ao tentar estabelecer a conexão com banco de dados ' . $parameters->name . '.';
										 
					// Gravando no log as informações
					$this->logger->debug( $parameters );
					$this->logger->debug( $errors );
					$this->logger->debug( $response );
				}
			}
			else{
				$this->logger->debug( $parameters );
			}
		}
		else{
            $response = $this->response;
        }
				
        return $response;
	}
	
}
?>