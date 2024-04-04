<?php

/**
 * @author    {Rafael P. Cruz}
 * @package   {1.0.0}
 * @description	{ }
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


// inclui as classes do PHPMailer
require(__DIR__ . '/PHPMailer.php');
require(__DIR__ . '/SMTP.php');


date_default_timezone_set('America/Sao_Paulo');

class disparo_email
{

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

	private $path;

	function __construct()
	{

		//Iniciando resposta padrão do construtor.
		$this->response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Classe ' . __CLASS__ . ' iniciada com sucesso.'));

		// Montando o código do erro que será apresentado
		$localizar  = array(strtolower(__DIR__), "/", "\\", ".php", ".");
		$substituir = array("", "", "", "", "-");
		$this->error_code = strtoupper(str_replace($localizar, $substituir,  strtolower(__FILE__))) . "-";

		// Declarando os caminhos principais do sistema.
		$localizar 	= array("\\", "/includes/php");
		$substituir	= array("/", "");
		$this->path 		= str_replace($localizar, $substituir, __DIR__);

		$fileLogApi = $this->path . '/includes/php/log4php/Logger.php';

		if (file_exists($fileLogApi)) {

			include_once($fileLogApi);

			$fileLogConfig = $this->path . '/includes/config/config.log.xml';

			if (file_exists($fileLogConfig)) {
				//Informado as configuracoes do log4php
				Logger::configure($fileLogConfig);

				//Indica qual bloco do XML corresponde as configuracoes
				$this->logger = Logger::getLogger('config.log');
			} else {
				$this->response['response_status']['status'] 		= 0;
				$this->response['response_status']['error_code'] 	= $this->error_code . __LINE__;
				$this->response['response_status']['msg']			= "Não foi possível localizar as configurações do log.";
			}
		} else {
			$this->response['response_status']['status']     = 0;
			$this->response['response_status']['error_code'] = $this->error_code . __LINE__;
			$this->response['response_status']['msg']        = 'Não foi possível encontrar o plugins log4php.';
		}

		if ($this->response['response_status']['status'] == 1) {

			$fileGetConfig = $this->path . "/includes/config/config.get.php";

			// Carregando as configuração do Mirrada
			if (file_exists($fileGetConfig)) {

				include_once($fileGetConfig);

				$this->getconfig = new getconfig();

				if ($this->getconfig->response['response_status']['status'] == 0) {
					$this->response = $this->getconfig->response;
				}
			} else {
				$this->response['response_status']['status']       = 0;
				$this->response['response_status']['error_code']   = $this->error_code . __LINE__;
				$this->response['response_status']['msg']          = 'Não foi possível localizar o arquivo de configuração (mirada-config).';
			}
		}
	}

	function connectdb()
	{

		if ($this->response['response_status']['status'] == 1) {

			// Carregando a resposta padrão da função
			$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

			$qualitor_db = $this->getconfig->searchConfigDatabase('ineditta');

			if ($qualitor_db['response_status']['status'] == 1) {

				$parameters = $qualitor_db['response_data'];

				if (file_exists($this->path . '/includes/php/db.mysql.php')) {

					include_once($this->path . '/includes/php/db.mysql.php');

					// Criando o objeto de conexão com o banco de dados Qualitor
					$apidbmysql = new apidbmysql();

					$db = $apidbmysql->connection($parameters);

					if ($db['response_status']['status'] == 1) {

						$this->db = $db['response_data']['connection'];

						$this->logger->debug($db['response_data']['connection']);
					} else {
						$response = $db;
					}
				} else {
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível encontrar o db.mysql.';
				}
			} else {
				$response =  $qualitor_db;
			}
		} else {
			$response = $this->response;
		}

		return $response;
	}

	function dispararEmails($data = null)
	{

		if ($this->response['response_status']['status'] == 1) {

			// Carregando a resposta padrão da função
			$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

			// Montando o c??o do erro que ser?presentado
			$localizar  = array(strtolower(__DIR__), "/", "\\", ".php", ".");
			$substituir = array("", "", "", "", "-");
			$error_code = strtoupper(str_replace($localizar, $substituir,  strtolower(__FILE__))) . "-";

			// Declarando os caminhos principais do sistema.
			$localizar 		= array("\\", "/includes/php");
			$substituir		= array("/", "");
			$path 			= str_replace($localizar, $substituir, __DIR__);

			$anexo 			= $path . '/anexos/';

			//Incluindo descrição do erro no log do sistema.
			//$this->logger->debug($data);
			$this->logger->debug("EMAIL STATUS REPONSE 1");

			if ($response['response_status']['status'] == 1) {
				// inicia a classe PHPMailer habilitando o disparo de exceções
				// inclui as classes do PHPMailer

				$enviar = 0;
				$mail = new PHPMailer\PHPMailer\PHPMailer();

				$this->logger->debug("EMAIL STATUS REPONSE 2");

				try {
					// habilita o debug
					// 0 = em mensagens de debug
					// 1 = mensagens do cliente SMTP
					// 2 = mensagens do cliente e do servidor SMTP
					// 3 = igual o 2, incluindo detalhes da conexão
					// 4 = igual o 3, inlcuindo mensagens de debug baixo-nível
					$mail->SMTPDebug = 0;

					// utilizar SMTP
					$mail->isSMTP();

					// habilita autenticação SMTP
					$mail->SMTPAuth = true;

					// servidor SMTP
					$mail->Host = 'smtp.gmail.com';

					// usuário, senha e porta do SMTP
					$mail->Username = "{$data['email_remetente']}"; //email enviando emails
					$mail->Password = ($data['email_remetente'] == "no-reply@ineditta.com.br" ? 'mmzpdogaqaqwnhij' : "{$data['senha']}"); // senha
					$mail->Port = $data['porta']; //587 para tls 465 para ssl
					// tipo de criptografia: "tls" ou "ssl"
					$mail->SMTPSecure = "{$data['cripto']}";

					$mensagem = utf8_decode("{$data['nome']}");// nome do remetente

					$this->logger->debug("EMAIL STATUS REPONSE 3 " . "{$data['nome']}");
					//s.login("muralha77de77concreto@gmail.com", "rgakatrgmhkxkjyv")
					// email e nome do remetente
					$mail->setFrom('cct@ineditta.com.br', "{$mensagem}");

					if ($enviar ==	0) {
						$enviar	= 1;
					}

					//CASO SEJAM MULTIPLOS ENDEREÇOS ENVIA STRING SEPARADA POR VIRGULA
					$to_multi  = explode(",", $data['to_multi']);

					if($data['to']){
						$mail->addAddress($data['to']);
					}else {
						// $mail->addAddress($to_multi[0]);
						//BCC
						for ($i=0; $i < count($to_multi) ; $i++) { 
							$mail->addBCC($to_multi[$i]);
						}
						
					}
					

					$this->logger->debug("EMAIL STATUS REPONSE 4" . "{$data['to']}");

					//$mail->addReplyTo('muralha77de77concreto@gmail.com', 'ULTIMATE PHP');

					$mail->isHTML(true);

					// codificação UTF-8
					// $mail->Charset = 'UTF-8';

					// assunto do email
					$mail->Subject = utf8_decode("{$data['assunto']}");

					$this->logger->debug("EMAIL STATUS REPONSE 5 " . "{$data['assunto']}");


					$mail->Body    = utf8_decode("

					{$data['msg']}

					");

					$this->logger->debug("EMAIL STATUS REPONSE 6" . "{$data['msg']}");

					$mail->send();

					$response['response_status']['status']      	= 1;
					$response['response_status']['error_code']  	= null;
					$response['response_status']['msg']         	= 'Disparos enviados com sucesso!';
				} catch (Exception $e) {

					$this->logger->debug($mail->ErrorInfo . PHP_EOL);

					$response['response_status']['status']      	= 0;
					$response['response_status']['error_code']  	= $e;
					$response['response_status']['msg']         	= 'Falha ao enviar email. Entre em contato com o administrador do sistema!';
				}
			}
		}else {
			$response = $this->response;
		}

		// $r = json_encode($response);
		$this->logger->debug("RETORNANDO" . $response);

		return $response;
	}
}
