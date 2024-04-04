<?php
/**
 * sysgetconfig class
 * @author    {Enter5}
 * @copyright {Mirada Conector® 2020}
 * @package   {1.0.0}
 * @description	{}
 * @historic {
		2019-11-26 14:24 ( v      ) - 
	}
**/

class getconfig{
    
    // Retorno do construtor
    public $response;
    
	// Código do erro
    public $error_code;
	
    // Configurações do sistema
    public $settings;

    function __construct() {
		
        //Iniciando resposta padrão do construtor.
		$this->response = array( 'response_status' => array( 'status' => 1, 'error_code' => null, 'msg' => 'Classe ' . __CLASS__ . ' iniciada com sucesso.' ) );
		
		/* Montando o código do erro que será apresentado */
        $localizar  = array( strtolower(__DIR__), "/", "\\", ".php", ".");
        $substituir = array( "", "", "", "", "-" );
        $this->error_code = strtoupper( str_replace( $localizar, $substituir,  strtolower( __FILE__  ) ) ) . "-";
		
		// Declarando os caminhos principais do sistema.
		$localizar 	= array( "\\" );	
		$substituir	= array( "/" );
		$path = str_replace( $localizar, $substituir, __DIR__ );
						
		//Iniciando a propriedade.
		$this->settings = new stdClass();

		//Validando se o arquivo de configuração existe.
		if(!file_exists( $path . '/config.json') && !getenv('CONFIG_JSON')){
			$this->response['response_status']['status']       = 0;
			$this->response['response_status']['error_code']   = $this->error_code . __LINE__;
			$this->response['response_status']['msg']          = 'Não foi possível localizar o arquivo de configuração do sistema.';

			return;
		}

		//Convertendo a scring json para array
		$arrayConfig = getEnv('CONFIG_JSON') ?
						json_decode(getEnv('CONFIG_JSON')) :
						json_decode(file_get_contents( $path . '/config.json' ));

		// Check for JSON decoding errors
		if (empty($arrayConfig)) {
			$this->response['response_status']['status']       = 0;
			$this->response['response_status']['error_code']   = $this->error_code . __LINE__;
			$this->response['response_status']['msg']          = 'Existe um erro na sintax do json no arquivo de configuração do sistema.' . json_last_error_msg();

			return;
		}

		$this->settings = $arrayConfig;
	}
	
    function searchConfigReturn() {
		
        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

			if( isset( $this->settings->standard_response ) ){
				
				$response['response_status']['msg']	= 'Parâmetros de retorno foram localizados com sucesso.';
				$response['response_data'] 			= $this->settings->standard_response; 
			}
			else{
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'Não existe parâmetros de retorno no arquivo de configuração.';
				unset($response['response_data']);
			}
        }
		else{
            $response = $this->response;
        }

        return $response;
    }
		
    function searchConfigDatabase( $name = null ) {
		
        if( $this->response['response_status']['status'] == 1 ){
			
			//Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

			//Iniciando processo de validação dos parâmetros obrigatórios.
			if( empty($name) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro name é obrigatório.';
			}
						
			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->databases ) && is_array( $this->settings->databases ) ){
					
					foreach( $this->settings->databases as $database ){

                        if( $database->name == $name ){
                            
							$response['response_status']['msg']	= 'Parâmetros de conexão com banco de dados localizados com sucesso.';
							$response['response_data'] 			= $database; 
                        }
                    }

					if( !isset( $response['response_data'] ) ){
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não existe nenhum parâmetro de conexão com banco de dados para o name ' . $name . '.';
					}
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Não existe parâmetros cadastrados para conexão com banco de dados no arquivo de configuração.';
                    unset($response['response_data']);
				}			
			}
        }
		else{
            $response = $this->response;
        }

        return $response;
    }
	
    function searchConfigWebService( $name = null ) {

        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __METHOD__ . ' iniciada com sucesso.' ) );

			//Iniciando processo de validação dos parâmetros obrigatórios.
			if( empty($name) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro name é obrigatório.';
			}
			
			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->webservices ) && is_array( $this->settings->webservices ) ){
					
					foreach( $this->settings->webservices as $webservice ){

                        if( $webservice->name == $name ){
                            
							$response['response_status']['msg']	= 'Parâmetros de conexão com o web service localizados com sucesso.';
							$response['response_data'] 			= $webservice; 
                        }
                    }
					
					if( !isset( $response['response_data'] ) ){
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não existe nenhum parâmetro de coxão com o web service para o name ' . $name . '.';
					}
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Não existe parâmetros cadastrados para conexão com web service no arquivo de configuração.';
                    unset($response['response_data']);
				}			
			}
        } 
		else{
            $response = $this->response;
        }

        return $response;
    }
    
    function searchConfigCard( $name = null ) {

        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __METHOD__ . ' iniciada com sucesso.' ) );

			//Iniciando processo de validação dos parâmetros obrigatórios.
			if( empty($name) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro name é obrigatório.';
			}
			
			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->card ) && is_array( $this->settings->card ) ){
		
					foreach( $this->settings->card as $card ){
						
                        if( $card->name == $name ){
                            
							$response['response_status']['msg']	= 'Parâmetros do card localizados com sucesso.';
							$response['response_data'] 			= $card; 
                        }
                    }
					
					if( !isset( $response['response_data'] ) ){
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não existe nenhum parâmetro cadastrado para card com o name ' . $name . '.';
					}
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Não existe parâmetros cadastrados para card no arquivo de configuração.';
                    unset($response['response_data']);
				}			
			}
        } 
		else{
            $response = $this->response;
        }
		
        return $response;
    }
	
    function searchConfigEligibility( $card = null ) {

        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __METHOD__ . ' iniciada com sucesso.' ) );

			//Iniciando processo de validação dos parâmetros obrigatórios.
			if( empty($card) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro card é obrigatório.';
			}
			
			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->eligibility ) && is_array( $this->settings->eligibility ) ){
					
					foreach( $this->settings->eligibility as $eligibility ){

                        if( $eligibility->card == $card ){
                            
							$response['response_status']['msg']	= 'Parâmetros da elegibilidade localizados com sucesso.';
							$response['response_data'] 			= $eligibility; 
                        }
                    }
					
					if( !isset( $response['response_data'] ) ){
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não existe nenhuma elegibilidade cadastrada para o card ' . $card . '.';
					}
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Não existe elegibilidade cadastradas para os card\'s no arquivo de configuração.';
                    unset($response['response_data']);
				}			
			}
        } 
		else{
            $response = $this->response;
        }

        return $response;
    }
	
    function searchConfigRouting( $gtw = null ) {

        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __METHOD__ . ' iniciada com sucesso.' ) );

			//Iniciando processo de validação dos parâmetros obrigatórios.
			if( empty($gtw) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro gtw é obrigatório.';
			}
			
			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->routing ) && is_array( $this->settings->routing ) ){
					
					foreach( $this->settings->routing as $routing ){
						
                        if( $routing->gtw == $gtw ){
							$response['response_status']['msg']	= 'Parâmetros da integração localizados com sucesso.';
							$response['response_data'] 			= $routing;
                        }
                    }
					
					if( !isset( $response['response_data'] ) ){
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = 'CONFIG-GET-002';
						$response['response_status']['msg']          = 'Não existe nenhuma integração cadastrada para o card ' . $card . '.';
					}
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = 'CONFIG-GET-001';
                    $response['response_status']['msg']          = 'Não existe nenhuma integração cadastradas no arquivo de configuração.';
                    unset($response['response_data']);
				}			
			}
        } 
		else{
            $response = $this->response;
        }

        return $response;
    }

	function searchConfigFromTo( $group = null ) {

        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __METHOD__ . ' iniciada com sucesso.' ) );

			//Iniciando processo de validação dos parâmetros obrigatórios.
			if( empty($group) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro group é obrigatório.';
			}
			
			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->fromTo ) && is_array( $this->settings->fromTo ) ){
					
					foreach( $this->settings->fromTo as $fromTo ){
						
                        if( $fromTo->group == $group ){
                            
							$response['response_status']['msg']	= 'Parâmetros do fromTo localizados com sucesso.';
							$response['response_data'] 			= $fromTo;
                        }
                    }
					
					if( !isset( $response['response_data'] ) ){
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não existe nenhum parâmetro cadastrado para fromTo com o group ' . $group . '.';
					}
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Não existe parâmetros cadastrados para fromTo no arquivo de configuração.';
                    unset($response['response_data']);
				}			
			}
        }
		else{
            $response = $this->response;
        }

        return $response;
    }

	function searchConfigTranslation( $group = null ) {

        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __METHOD__ . ' iniciada com sucesso.' ) );

			//Iniciando processo de validação dos parâmetros obrigatórios.
			if( empty($group) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro group é obrigatório.';
			}
			
			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->translation ) && is_array( $this->settings->translation ) ){
					
					foreach( $this->settings->translation as $translation ){
						
                        if( $translation->group == $group ){
                            
							$response['response_status']['msg']	= 'Parâmetros do translation localizados com sucesso.';
							$response['response_data'] 			= $translation; 
                        }
                    }
					
					if( !isset( $response['response_data'] ) ){
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não existe nenhum parâmetro cadastrado para translation com o group ' . $group . '.';
					}
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Não existe parâmetros cadastrados para translation no arquivo de configuração.';
                    unset($response['response_data']);
				}			
			}
        } 
		else{
            $response = $this->response;
        }

        return $response;
    }
	
	function searchConfigSynchronizationStatus() {

        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __METHOD__ . ' iniciada com sucesso.' ) );

			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->synchronization->service ) && !empty( $this->settings->synchronization->service ) ){
					                            
					$response['response_status']['msg']		= 'Parâmetros que define o status do serviço de synchronization foi localizado com sucesso.';
					$response['response_data']['service']	= $this->settings->synchronization->service;							
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'O status do serviço de synchronization não foi parametrizado.';
				}			
			}
        } 
		else{
            $response = $this->response;
        }

        return $response;
    }

	function searchConfigSynchronization( $profile = null ) {

        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __METHOD__ . ' iniciada com sucesso.' ) );

			//Iniciando processo de validação dos parâmetros obrigatórios.
			if( empty($profile) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro profile é obrigatório.';
			}
			
			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->synchronization->profiles ) && is_array( $this->settings->synchronization->profiles ) ){
					
					foreach( $this->settings->synchronization->profiles as $profiles ){
						
                        if( $profiles->profile == $profile ){
                            
							$response['response_status']['msg']		= 'Parâmetros da synchronization localizados com sucesso.';
							$response['response_data']['service']	= $this->settings->synchronization->service;
							$response['response_data']['profiles']	= $profiles;							
                        }
                    }
					
					if( !isset( $response['response_data'] ) ){
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não existe nenhum parâmetro cadastrado para synchronization com o profile ' . $profile . '.';
					}
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Não existe parâmetros cadastrados para synchronization no arquivo de configuração.';
                    unset($response['response_data']);
				}			
			}
        } 
		else{
            $response = $this->response;
        }

        return $response;
    }
	
	function searchConfigAdditionalInformation() {

        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __METHOD__ . ' iniciada com sucesso.' ) );

			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->additionalinformation ) ){
					
					$response['response_status']['msg']	= 'Parâmetros da synchronization localizados com sucesso.';
					$response['response_data'] 			= $this->settings->additionalinformation;
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Não existe parâmetros cadastrados para as informações adicionais no arquivo de configuração.';
                    unset($response['response_data']);
				}			
			}
        } 
		else{
            $response = $this->response;
        }

        return $response;
    }	

	function searchConfigApplication( $name = null ) {

        if( $this->response['response_status']['status'] == 1 ){
			
			//>> Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __METHOD__ . ' iniciada com sucesso.' ) );

			//Iniciando processo de validação dos parâmetros obrigatórios.
			if( empty($name) ){
				$response['response_status']['status']       = 0;
				$response['response_status']['error_code']   = $this->error_code . __LINE__;
				$response['response_status']['msg']          = 'O parâmetro name é obrigatório.';
			}
			
			//Iniciando o processamento da solicitação.
			if( $response['response_status']['status'] == 1 ){
				
				if( isset( $this->settings->application ) && is_array( $this->settings->application ) ){
					
					foreach( $this->settings->application as $application ){
						
                        if( $application->name == $name ){
                            
							$response['response_status']['msg']		= 'Parâmetros da aplicação localizados com sucesso.';
							$response['response_data']['config']	= $application;						
                        }
                    }
					
					if( !isset( $response['response_data'] ) ){
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não existe nenhum parâmetro cadastrado para aplicação com o name ' . $name . '.';
					}
				}
				else{
					$response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Não existe parâmetros cadastrados para aplicação no arquivo de configuração.';
                    unset($response['response_data']);
				}			
			}
        } 
		else{
            $response = $this->response;
        }

        return $response;
    }
}

?>