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


class profile
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

	//Perfil do usuario logado
	public static $perfil;

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
				// Logger::configure($fileLogConfig);

				//Indica qual bloco do XML corresponde as configuracoes
				// $this->logger = Logger::getLogger('config.log');
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

						// $this->logger->debug($db['response_data']['connection']);
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


	function getProfileCampos($data = null)
	{

		if ($this->response['response_status']['status'] == 1) {

			// Carregando a resposta padrão da função
			$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

			if ($response['response_status']['status'] == 1) {
				if (empty($this->db)) {
					$connectdb = $this->connectdb();

					if ($connectdb['response_status']['status'] == 0) {
						$response = $connectdb;
					}
				}
			}

			//$this->logger->debug(  $connectdb );

			if ($response['response_status']['status'] == 1) {





				$sql = "
					SELECT 
					id_user as id_usuario
					,foto
                    ,nome_usuario
                    ,email_usuario
					,cargo
					,telefone
					,ramal
					,id_user_superior
					,departamento
					FROM 
						usuario_adm;				
				";

				// $this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$htmlupdate = null;

					while ($obj = $resultsql->fetch_object()) {
						$html .= '<tr class="odd gradeX">';
						$html .= '<td><button onclick="selectSup( ' . $obj->id_usuario . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$html .= '<td> <img src="' . $obj->foto . '" height="100" alt="Image preview...">';
						$html .= '<td>';
						$html .= $obj->nome_usuario;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->email_usuario;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->departamento;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->cargo;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->telefone;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->ramal;
						$html .= '</td>';
						$html .= '</tr>';


						$htmlupdate .= '<tr class="odd gradeX">';
						$htmlupdate .= '<td><button onclick="selectSup( ' . $obj->id_usuario . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td> <img src="' . $obj->foto . '" height="100" alt="Image preview...">';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->nome_usuario;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->email_usuario;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->departamento;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->cargo;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->telefone;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->ramal;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
					}

					$response['response_data']['listaSup'] 	= $html;
					$response['response_data']['listaSupupdate'] 	= $htmlupdate;
				} else {
					// $this->logger->debug($sql);
					// $this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}


				$sql = "
				SELECT 
				id_user as id_usuario
				,nome_usuario
				FROM 
					usuario_adm;				
				";

				// $this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value=""></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->id_usuario;
						$grupos .= '">';
						$grupos .= $obj->nome_usuario;
						$grupos .= '</option>';
					}

					$response['response_data']['superiores'] 	= $grupos;
				} else {
					// $this->logger->debug($sql);
					// $this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		// $this->logger->debug($response['response_status']['status']);

		return $response;
	}


	function setProfile($data = null)
	{
		if ($this->response['response_status']['status'] == 1) {

			// Carregando a resposta padrão da função
			$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

			if ($response['response_status']['status'] == 1) {
				if (empty($this->db)) {
					$connectdb = $this->connectdb();

					if ($connectdb['response_status']['status'] == 0) {
						$response = $connectdb;
					}
				}
			}

			// $this->logger->debug(json_decode($data['obj']));
		}



		// $obj = json_decode($data['perfil']);
		// self::$perfil = $obj->email;

		// session_start();
		if (!$_SESSION) {
			echo "<script>document.location.href='http://localhost:8000/index.php'</script>";
		}
		// $_SESSION['login'] = $data['perfil']['firstName'];


		return $response;
	}

	// public static function getProfile()
	// {
	// 	return self::$perfil;
	// }


	function addProfile($data = null)
	{
		// $this->logger->debug('entrou na classe php');
		if ($this->response['response_status']['status'] == 1) {

			// Carregando a resposta padrão da função
			$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

			if ($response['response_status']['status'] == 1) {
				if (empty($this->db)) {
					$connectdb = $this->connectdb();

					if ($connectdb['response_status']['status'] == 0) {
						$response = $connectdb;
					}
				}
			}

			// $this->logger->debug($connectdb);
			if ($response['response_status']['status'] == 1) {
				$sql = "insert into profile (profile_semanal, descricao, is_default)
							values
								('{$data['profile']}','{$data['desc-input']}', {$data['padr-input']});
				";
				// $this->logger->debug($sql);
				if (!mysqli_query($this->db, $sql)) {

					mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';

					//$this->logger->debug($sql);
					//$this->logger->debug($this->db->error);
					//$this->logger->debug($response);
				} else {
					//$this->logger->debug($sql);
//$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		//$this->logger->debug($response['response_status']['status']);

		return $response;
	}

	function updateProfile($data = null)
	{
		//$this->logger->debug('entrou na classe php');
		if ($this->response['response_status']['status'] == 1) {

			// Carregando a resposta padrão da função
			$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

			if ($response['response_status']['status'] == 1) {
				if (empty($this->db)) {
					$connectdb = $this->connectdb();

					if ($connectdb['response_status']['status'] == 0) {
						$response = $connectdb;
					}
				}
			}

			//$this->logger->debug($connectdb);
			if ($response['response_status']['status'] == 1) {

				$sql = " UPDATE profile  
									SET  profile_semanal = '{$data['profile']}',
                                         descricao = '{$data['desc-input']}',				
										is_default = {$data['padr-input']}
									WHERE 
                                        id_profile = {$data['id_profile']};
						";


				//$this->logger->debug($sql);
				if (!mysqli_query($this->db, $sql)) {

					mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';

					
				} else {
					

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}


		return $response;
	}



	function setPersonal($data = null)
	{

		if ($this->response['response_status']['status'] == 1) {

			// Carregando a resposta padrão da função
			$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

			if ($response['response_status']['status'] == 1) {
				if (empty($this->db)) {
					$connectdb = $this->connectdb();

					if ($connectdb['response_status']['status'] == 0) {
						$response = $connectdb;
					}
				}
			}

			//$this->logger->debug(  $connectdb );

			if ($response['response_status']['status'] == 1) {





				$sql = "SELECT ge.logo_grupo, usu.email_usuario FROM cliente_grupo as ge 
						INNER JOIN usuario_adm as usu ON CASE 
							WHEN usu.nivel = 'Ineditta' then false
							ELSE usu.id_grupoecon = ge.id_grupo_economico
							END
						WHERE usu.id_user = {$data['iduser']}";

				if ($resultsql = mysqli_query($this->db, $sql)) {

					$logo = "http://localhost:8000/includes/img/ineditta_banner.jpg";

					while ($obj = $resultsql->fetch_object()) {
						if ($obj->logo_grupo) {
							$logo = $obj->logo_grupo;
						}
					}

					$response['response_data']['logo'] 	= $logo;
				} else {

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}


				$sql = "
				SELECT 
				id_user as id_usuario
				,nome_usuario
				FROM 
					usuario_adm;				
				";

				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value=""></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->id_usuario;
						$grupos .= '">';
						$grupos .= $obj->nome_usuario;
						$grupos .= '</option>';
					}

					$response['response_data']['superiores'] 	= $grupos;
				} else {
					

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}


		return $response;
	}
}
