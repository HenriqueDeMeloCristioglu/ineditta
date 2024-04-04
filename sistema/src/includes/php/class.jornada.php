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


class jornada
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








	function getJornadaCampos($data = null)
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

				$this->logger->debug($sql);
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
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

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

				$this->logger->debug($sql);
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
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

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

		$this->logger->debug($response['response_status']['status']);

		return $response;
	}










	function getJornada($data = null)
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

			$this->logger->debug($connectdb);

			if ($response['response_status']['status'] == 1) {

				$sql = "
					SELECT 
					id_jornada
					,descricao
                    ,jornada_semanal
					FROM 
						jornada;				
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;

					while ($obj = $resultsql->fetch_object()) {

						$jor_sem = json_decode($obj->jornada_semanal);



						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdJornada( ' . $obj->id_jornada . ');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td class="title">';
						$html .= $obj->descricao;
						$html .= '</td>';
						$html .= '<td>';
						$html .= '<table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered">';
						$html .= '<thead>';
						$html .=  '<tr>';
						$html .= 	'<th>Dia</th>';
						$html .= 	'<th>Início</th>';
						$html .= 	'<th>Fim</th>';
						$html .= '</tr>';
						$html .= '</thead>';
						$html .= '<tbody>';
						$html .= '<tr>';
						$html .= '<td>';
						$html .= '<h5>Segunda-feira</h5>';
						$html .= '</td>';
						$html .= '<td>';
						$html .= $jor_sem->SEGUNDA->INICIO;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $jor_sem->SEGUNDA->FIM;
						$html .= '</td>';
						$html .= '</tr>';
						$html .= '<tr>';
						$html .= '<td>';
						$html .= '<h5>Terça-feira</h5>';
						$html .= '</td>';
						$html .= '<td>';
						$html .= $jor_sem->TERCA->INICIO;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $jor_sem->TERCA->FIM;
						$html .= '</td>';
						$html .= '</tr>';
						$html .= '<tr>';
						$html .= '<td>';
						$html .= '<h5>Quarta-feira</h5>';
						$html .= '</td>';
						$html .= '<td>';
						$html .= $jor_sem->QUARTA->INICIO;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $jor_sem->QUARTA->FIM;
						$html .= '</td>';
						$html .= '</tr>';
						$html .= '<tr>';
						$html .= '<td>';
						$html .= '<h5>Quinta-feira</h5>';
						$html .= '</td>';
						$html .= '<td>';
						$html .= $jor_sem->QUINTA->INICIO;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $jor_sem->QUINTA->FIM;
						$html .= '</td>';
						$html .= '</tr>';
						$html .= '<tr>';
						$html .= '<td>';
						$html .= '<h5>Sexta-feira</h5>';
						$html .= '</td>';
						$html .= '<td>';
						$html .= $jor_sem->SEXTA->INICIO;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $jor_sem->SEXTA->FIM;
						$html .= '</td>';
						$html .= '</tr>';
						$html .= '</tbody>';
						$html .= '</table>';
						$html .= '</td>';
						$html .= '</tr>';
					}

					$response['response_data']['html'] 	= $html;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

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

		$this->logger->debug($response['response_status']['status']);

		return $response;
	}

	function getByIdJornada($data = null)
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

			$this->logger->debug($connectdb);

			if ($response['response_status']['status'] == 1) {

				$sql = "
				SELECT 
				id_jornada
				,descricao
				,jornada_semanal
				,is_default
				FROM 
					jornada
						WHERE
                        id_jornada = {$data['id_jornada']};
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {
					$obj = $resultsql->fetch_object();
					$jor_sem = json_decode($obj->jornada_semanal);
					$response['response_data']['id_jornada'] 	= $obj->id_jornada;
					$response['response_data']['descricao'] 	= $obj->descricao;
					$response['response_data']['segini'] 	= $jor_sem->SEGUNDA->INICIO;
					$response['response_data']['segfim'] 	= $jor_sem->SEGUNDA->FIM;
					$response['response_data']['terini'] 	= $jor_sem->TERCA->INICIO;
					$response['response_data']['terfim'] 	= $jor_sem->TERCA->FIM;
					$response['response_data']['quaini'] 	= $jor_sem->QUARTA->INICIO;
					$response['response_data']['quafim'] 	= $jor_sem->QUARTA->FIM;
					$response['response_data']['quiini'] 	= $jor_sem->QUINTA->INICIO;
					$response['response_data']['quifim'] 	= $jor_sem->QUINTA->FIM;
					$response['response_data']['sexini'] 	= $jor_sem->SEXTA->INICIO;
					$response['response_data']['sexfim'] 	= $jor_sem->SEXTA->FIM;

					$response['response_data']['padr'] 	= $obj->is_default;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

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

		$this->logger->debug($response['response_status']['status']);

		return $response;
	}

	function addJornada($data = null)
	{
		$this->logger->debug('entrou na classe php');
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

			$this->logger->debug($connectdb);
			if ($response['response_status']['status'] == 1) {
				$sql = "insert into jornada (jornada_semanal, descricao, is_default)
							values
								('{$data['jornada']}','{$data['desc-input']}', {$data['padr-input']});
				";
				$this->logger->debug($sql);
				if (!mysqli_query($this->db, $sql)) {

					mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';

					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);
					$this->logger->debug($response);
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

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

		$this->logger->debug($response['response_status']['status']);

		return $response;
	}

	function updateJornada($data = null)
	{
		$this->logger->debug('entrou na classe php');
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

			$this->logger->debug($connectdb);
			if ($response['response_status']['status'] == 1) {

				$sql = " UPDATE jornada  
									SET  jornada_semanal = '{$data['jornada']}',
                                         descricao = '{$data['desc-input']}',				
										is_default = {$data['padr-input']}
									WHERE 
                                        id_jornada = {$data['id_jornada']};
						";


				$this->logger->debug($sql);
				if (!mysqli_query($this->db, $sql)) {

					mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';

					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);
					$this->logger->debug($response);
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

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

		$this->logger->debug($response['response_status']['status']);

		return $response;
	}
}
