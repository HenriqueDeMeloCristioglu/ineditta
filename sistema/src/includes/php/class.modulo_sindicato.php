<?php

/**
 * @author    {Lucas A. Rodrigues Volpati}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2022-09-30 17:19 ( v1.0.0 ) - 
	}
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


class modulo_sindicato
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

	function getLists($data = null)
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

				//LISTA TIPO DOC
				// $sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'S'";
				// $result = mysqli_query( $this->db, $sqlTipo );

				// $option = "";
				// $option .= '<option value="--">--</option>';
				// while ($obj = $result->fetch_object()) {
				// 	$this->logger->debug(  $obj );

				// 	$option .= '<option value="'.$obj->idtipo_doc.'">';
				// 	$option .= $obj->nome_doc;
				// 	$option .= '</option>';

				// }

				// $response['response_data']['optionTipo'] = $option;

				//LISTAS LOCALIDADE - UF
				$sqlUf = "SELECT
							distinct uf
							
						FROM
							localizacao
						ORDER BY uf ASC
				";

				//LISTAS LOCALIDADE - REGIAO
				$sqlRegiao = "SELECT
							distinct regiao
							
						FROM
							localizacao
						ORDER BY regiao ASC
				";

				//LISTAS LOCALIDADE - MUNICIPIO
				$sqlMuni = "SELECT
							distinct municipio
							
						FROM
							localizacao
						ORDER BY municipio ASC
				";
				$resultUf = mysqli_query($this->db, $sqlUf);
				$resultReg = mysqli_query($this->db, $sqlRegiao);
				$resultMun = mysqli_query($this->db, $sqlMuni);

				$opt = "";
				while ($obj = $resultUf->fetch_object()) {
					$opt .= "<option value='uf+" . $obj->uf . "'>" . $obj->uf . "</option>";
				}

				while ($obj = $resultReg->fetch_object()) {
					$opt .= "<option value='regiao+" . $obj->regiao . "'>" . $obj->regiao . "</option>";
				}

				while ($obj = $resultMun->fetch_object()) {
					$opt .= "<option value='municipio+" . $obj->municipio . "'>" . $obj->municipio . "</option>";
				}

				$response['response_data']['listaLocal'] = $opt;


				//LISTA CLASSE CNAE (CATEGORIA)
				$sqlCat = 'SELECT * FROM classe_cnae';
				$resultCat = mysqli_query($this->db, $sqlCat);

				$optcat = "";
				while ($objCat = $resultCat->fetch_object()) {
					$this->logger->debug($objCat);

					$optcat .= '<option value="' . $objCat->id_cnae . '">';
					$optcat .= $objCat->descricao_subclasse;
					$optcat .= '</option>';
				}

				$response['response_data']['optionCategoria'] = $optcat;

				//LISTA GRUPO CLÁUSULAS
				// $sql = "SELECT 
				// 			* 
				// 		FROM 
				// 			grupo_clausula
				// 		ORDER BY nome_grupo ASC
				// ";

				// $resultsql = mysqli_query( $this->db, $sql );

				// $group = null;

				// while($obj = $resultsql->fetch_object()){

				// 	$group .= '<option value="'.$obj->idgrupo_clausula.'">';
				// 	$group .= $obj->nome_grupo;
				// 	$group .= '</option>';
				// }

				// $response['response_data']['grupoClausulas'] = $group;

				//LISTA COD_SINDCLIENTE - CLIENTE UNIDADES
				$sqlSind1 = "SELECT 
								distinct cod_sindcliente
							FROM 
								cliente_unidades
				";

				$resultsqlSind1 = mysqli_query($this->db, $sqlSind1);

				$cod = "";
				while ($obj = $resultsqlSind1->fetch_object()) {
					$cod .= '<option value="cod_sindcliente+' . $obj->cod_sindcliente . '">';
					$cod .= $obj->cod_sindcliente;
					$cod .= '</option>';
				}

				//LISTA COD_UNIDADE - CLIENTE UNIDADES
				$sqlSind2 = "SELECT 
								distinct codigo_unidade
							FROM 
								cliente_unidades
				";

				$resultsqlSind2 = mysqli_query($this->db, $sqlSind2);

				while ($obj = $resultsqlSind2->fetch_object()) {
					$cod .= '<option value="codigo_unidade+' . $obj->codigo_unidade . '">';
					$cod .= $obj->codigo_unidade;
					$cod .= '</option>';
				}

				$response['response_data']['listaCodigos'] = $cod;

				//LISTAS SINDICATOS
				$sqlEmp = "SELECT
							id_sinde,
							codigo_sinde,
							cnpj_sinde,
							sigla_sinde,
							denominacao_sinde
						FROM sind_emp
					";

				$sqlPatr = "SELECT
							id_sindp,
							codigo_sp,
							cnpj_sp,
							sigla_sp,
							denominacao_sp
						FROM sind_patr
				";


				$resultEmp = mysqli_query($this->db, $sqlEmp);
				$resultPatr = mysqli_query($this->db, $sqlPatr);

				$optEmp = "";
				while ($obj = $resultEmp->fetch_object()) {
					$optEmp .= "<option value='codigo_sinde+" . $obj->codigo_sinde . "'>" . $obj->codigo_sinde . "</option>";
					$optEmp .= "<option value='cnpj_sinde+" . $obj->cnpj_sinde . "'>" . $obj->cnpj_sinde . "</option>";
					$optEmp .= "<option value='sigla_sinde+" . $obj->sigla_sinde . "'>" . $obj->sigla_sinde . "</option>";
					$optEmp .= "<option value='denominacao_sinde+" . $obj->denominacao_sinde . "'>" . $obj->denominacao_sinde . "</option>";
				}

				$optPatr = "";
				while ($obj = $resultPatr->fetch_object()) {
					$optPatr .= "<option value='codigo_sp+" . $obj->codigo_sp . "'>" . $obj->codigo_sp . "</option>";
					$optPatr .= "<option value='cnpj_sp+" . $obj->cnpj_sp . "'>" . $obj->cnpj_sp . "</option>";
					$optPatr .= "<option value='sigla_sp+" . $obj->sigla_sp . "'>" . $obj->sigla_sp . "</option>";
					$optPatr .= "<option value='denominacao_sp+" . $obj->denominacao_sp . "'>" . $obj->denominacao_sp . "</option>";
				}


				$response['response_data']['listaEmp'] = $optEmp;
				$response['response_data']['listaPatr'] = $optPatr;



				// Verificação de filtro salvo
				$sql = "SELECT * FROM filtro_csv";

				$result = mysqli_query($this->db, $sql);

				$obj = $result->fetch_object();

				if (!empty($obj)) {
					$response['response_data']['filtro'] = true;

					$filtro = $obj->filtro;

					$filtroDecode = json_decode($filtro);
					$this->logger->debug($filtroDecode);

					$response['response_data']['filtro_salvo'] = $filtroDecode;
				}
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		return $response;
	}


	function getLocalidade($data = null)
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

			if ($response['response_status']['status'] == 1 && $data['localidade']) {

				$term = $data['localidade'];

				$sql = "SELECT
							distinct {$term}
							
						FROM
							localizacao
						ORDER BY {$term} ASC

				";

				$this->logger->debug($sql);

				if ($result = mysqli_query($this->db, $sql)) {
					$response['response_status']['status']     = 1;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Registro salvo com sucesso!';
				} else {
					$this->logger->debug($this->db->error);
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível salvar o filtro.';
				}

				$opt = '';

				while ($obj = $result->fetch_object()) {
					$this->logger->debug($obj);

					$opt .= "<option value='{$obj->term}'>{$obj->$term}</option>";
				}

				$response['response_data']['localidade'] = $opt;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}


	function getSindicatos($data = null)
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

			if ($response['response_status']['status'] == 1) {

				$item = $data['busca'];

				if ($data['sindicato'] == "laboral") {
					$sql = "SELECT
							id_sinde,
							{$item}
						FROM sind_emp
					";
				} else {
					$sql = "SELECT
							id_sindp,
							{$item}
						FROM sind_patr
					";
				}


				$this->logger->debug($sql);

				if ($result = mysqli_query($this->db, $sql)) {
					$response['response_status']['status']     = 1;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Registro salvo com sucesso!';
				} else {
					$this->logger->debug($this->db->error);
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível salvar o filtro.';
				}

				$opt = '';
				while ($obj = $result->fetch_object()) {
					$opt .= "<option value='{$obj->id_sinde}'>{$obj->$item}</option>";
				}

				$response['response_data']['sindicatos'] = $opt;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function getSindicatosByCodigo($data = null)
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


			if ($response['response_status']['status'] == 1) {

				$busca = $data['busca'];
				$where = "";

				//Tratando busca
				for ($i = 0; $i < count($busca); $i++) {
					$column = strstr($busca[$i], "+", true);
					break;
				}

				if (count($busca) > 1) {

					$string = "";
					foreach ($busca as $value) {
						$value = substr(strstr($value, "+"), 1);
						$string .= "'{$value}'" . ',';
					}
					$string = implode(",", array_filter(explode(",", $string)));
					$where = "WHERE {$column} IN ({$string})";
				} else {
					$string = "";
					foreach ($busca as $value) {
						$value = substr(strstr($value, "+"), 1);
						$string .= "'{$value}'";
					}

					$where .= " WHERE {$column} = {$string}";
				}

				$sqlEmp = "SELECT
								cod_sindcliente,
								cu.localizacao_id_localizacao,
								bem.sind_empregados_id_sinde1,
								sinde.sigla_sinde,
								sinde.razaosocial_sinde
							FROM cliente_unidades AS cu
							LEFT JOIN base_territorialsindemp AS bem ON bem.localizacao_id_localizacao1 = cu.localizacao_id_localizacao
							LEFT JOIN sind_emp AS sinde ON sinde.id_sinde = bem.sind_empregados_id_sinde1
							
							{$where}
							
							GROUP BY bem.sind_empregados_id_sinde1
				";

				$sqlPatr = "SELECT
								cod_sindcliente,
								cu.localizacao_id_localizacao,
								bp.sind_patronal_id_sindp,
								sindp.sigla_sp,
								sindp.razaosocial_sp
							FROM cliente_unidades AS cu
							LEFT JOIN base_territorialsindpatro AS bp ON bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao
							LEFT JOIN sind_patr AS sindp ON sindp.id_sindp = bp.sind_patronal_id_sindp
							
							{$where}
							
							GROUP BY bp.sind_patronal_id_sindp
				";

				$this->logger->debug($sqlEmp);
				$this->logger->debug($sqlPatr);


				$resultEmp = mysqli_query($this->db, $sqlEmp);
				$resultPatr = mysqli_query($this->db, $sqlPatr);


				$optEmp = '';
				while ($obj = $resultEmp->fetch_object()) {
					$optEmp .= "<option value='{$obj->sind_empregados_id_sinde1}'>{$obj->razaosocial_sinde}</option>";
				}

				$response['response_data']['optEmp'] = $optEmp;

				$optPatr = '';
				while ($obj = $resultPatr->fetch_object()) {
					$optPatr .= "<option value='{$obj->sind_patronal_id_sindp}'>{$obj->razaosocial_sp}</option>";
				}

				$response['response_data']['optPatr'] = $optPatr;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function getSindicatosByCodUnidade($data = null)
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


			if ($response['response_status']['status'] == 1) {

				$busca = $data['busca'];
				$where = "";
				if (count($busca) > 1) {
					$string = "";
					foreach ($busca as $value) {

						$string .= "'{$value}'" . ',';
					}
					$string = implode(",", array_filter(explode(",", $string)));
					$where = "WHERE cu.codigo_unidade IN ({$string})";
				} else {
					$string = "";
					foreach ($busca as $value) {

						$string .= "'{$value}'";
					}

					$where .= " WHERE cu.codigo_unidade = {$string}";
				}

				$sqlEmp = "SELECT
								codigo_unidade,
								cu.localizacao_id_localizacao,
								bem.sind_empregados_id_sinde1,
								sinde.sigla_sinde,
								sinde.razaosocial_sinde
							FROM cliente_unidades AS cu
							LEFT JOIN base_territorialsindemp AS bem ON bem.localizacao_id_localizacao1 = cu.localizacao_id_localizacao
							LEFT JOIN sind_emp AS sinde ON sinde.id_sinde = bem.sind_empregados_id_sinde1
							
							{$where}
							
							GROUP BY bem.sind_empregados_id_sinde1
				";

				$sqlPatr = "SELECT
								codigo_unidade,
								cu.localizacao_id_localizacao,
								bp.sind_patronal_id_sindp,
								sindp.sigla_sp,
								sindp.razaosocial_sp
							FROM cliente_unidades AS cu
							LEFT JOIN base_territorialsindpatro AS bp ON bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao
							LEFT JOIN sind_patr AS sindp ON sindp.id_sindp = bp.sind_patronal_id_sindp
							
							{$where}
							
							GROUP BY bp.sind_patronal_id_sindp
				";

				$this->logger->debug($sqlEmp);
				$this->logger->debug($sqlPatr);


				$resultEmp = mysqli_query($this->db, $sqlEmp);
				$resultPatr = mysqli_query($this->db, $sqlPatr);


				$optEmp = '';
				while ($obj = $resultEmp->fetch_object()) {
					$optEmp .= "<option value='{$obj->sind_empregados_id_sinde1}'>{$obj->razaosocial_sinde}</option>";
				}

				$response['response_data']['optEmpCod'] = $optEmp;

				$optPatr = '';
				while ($obj = $resultPatr->fetch_object()) {
					$optPatr .= "<option value='{$obj->sind_patronal_id_sindp}'>{$obj->razaosocial_sp}</option>";
				}

				$response['response_data']['optPatrCod'] = $optPatr;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function setFilter($data = null)
	{

		if ($this->response['response_status']['status'] == 1) {

			$grupoecon = $data['gec'];

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

			if ($response['response_status']['status'] == 1) {

				$this->logger->debug($data);
				$buscaLocal = $data['localidade'];
				$buscaCateg = $data['categoria'];
				$buscaData = $data['data_base'];

				$buscaEmp = $data['sind_emp'];
				$buscaPatr = $data['sind_patr'];

				$this->logger->debug($data);

				$whereEmp = "";
				$wherePatr = "";
				$filter = 0;
				//BUSCANDO POR LOCALIDADE
				if (!empty($buscaLocal)) {
					if (count($buscaLocal) > 1) {
						$string = "";
						foreach ($buscaLocal as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);

							$string .= "'{$content}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$whereEmp .= $whereEmp != "" ? " AND lc.{$column} IN ({$string})" : "lc.{$column} IN ({$string})";
						$wherePatr .= $wherePatr != "" ? " AND lc.{$column} IN ({$string})" : "lc.{$column} IN ({$string})";
					} else {
						$string = "";
						foreach ($buscaLocal as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);

							$string .= "'{$content}'";
						}
						$whereEmp .= $whereEmp != "" ? " AND lc.{$column} = {$string}" : "lc.{$column} = {$string}";
						$wherePatr .= $wherePatr != "" ? " AND lc.{$column} = {$string}" : "lc.{$column} = {$string}";			
					}
				}

				//BUSCANDO POR CATEGORIA (CLASSE CNAE)
				if (!empty($buscaCateg)) {
					if (count($buscaCateg) > 1) {

						$string = "";
						foreach ($buscaCateg as $value) {

							$string .= "'{$value}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$whereEmp .= $whereEmp != "" ? " AND bem.classe_cnae_idclasse_cnae IN ({$string})" : "bem.classe_cnae_idclasse_cnae IN ({$string})";
						$wherePatr .= $wherePatr != "" ? " AND bp.classe_cnae_idclasse_cnae IN ({$string})" : "bp.classe_cnae_idclasse_cnae IN ({$string})";
					} else {
						$string = "";
						foreach ($buscaCateg as $value) {
							$string .= "'{$value}'";
						}

						$whereEmp .= $whereEmp != "" ? " AND bem.classe_cnae_idclasse_cnae = {$string}" : "bem.classe_cnae_idclasse_cnae = {$string}";
						$wherePatr .= $wherePatr != "" ? " AND bp.classe_cnae_idclasse_cnae = {$string}" : "bp.classe_cnae_idclasse_cnae = {$string}";
					}
				}

				//BUSCANDO POR DATA-BASE
				if (!empty($buscaData)) {
					if (count($buscaData) > 1) {

						$string = "";
						foreach ($buscaData as $value) {

							$string .= "'{$value}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$whereEmp .= $whereEmp != "" ? " AND bem.dataneg IN ({$string})" : "bem.dataneg IN ({$string})";
						$wherePatr .= $wherePatr != "" ? " AND bem.dataneg IN ({$string})" : "bem.dataneg IN ({$string})";
					} else {
						$string = "";
						foreach ($buscaData as $value) {
							$string .= "'{$value}'";
						}

						$whereEmp .= $whereEmp != "" ? " AND bem.dataneg = {$string}" : "bem.dataneg = {$string}";
						$wherePatr .= $wherePatr != "" ? " AND bem.dataneg = {$string}" : "bem.dataneg = {$string}";
					}
				}

				//FILTRO GRUPO ECONOMICO
				if ($data['grupo'] != "" && $data['grupo'] != 0) {
					$whereEmp .= ($whereEmp != "" ? " AND gp.id_grupo_economico = {$data['grupo']}" : " gp.id_grupo_economico = {$data['grupo']}");
					$wherePatr .= ($wherePatr != "" ? " AND gp.id_grupo_economico = {$data['grupo']}" : " gp.id_grupo_economico = {$data['grupo']}");
				}

				//FILTRO MATRIZ
				if ($data['matriz'] != "") {

					if (count($data['matriz']) > 1) {
						$string = "";
						foreach ($data['matriz'] as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$whereEmp .= ($whereEmp != "" ? " AND mt.id_empresa IN ({$string})" : " mt.id_empresa IN ({$string})");
						$wherePatr .= ($wherePatr != "" ? " AND mt.id_empresa IN ({$string})" : " mt.id_empresa IN ({$string})");
					} else {

						$string = "";
						foreach ($data['matriz'] as $value) {

							$string .= "'{$value}'";
						}

						$whereEmp .= ($whereEmp != "" ? " AND mt.id_empresa = {$string}" : " mt.id_empresa = {$string}");
						$wherePatr .= ($wherePatr != "" ? " AND mt.id_empresa = {$string}" : " mt.id_empresa = {$string}");
					}
				}

				//FILTRO UNIDADE
				if ($data['unidade'] != "") {

					if (count($data['unidade']) > 1) {
						$string = "";
						foreach ($data['unidade'] as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$whereEmp .= ($whereEmp != "" ? " AND cl.id_unidade IN ({$string})" : " cl.id_unidade IN ({$string})");
						$wherePatr .= ($wherePatr != "" ? " AND cl.id_unidade IN ({$string})" : " cl.id_unidade IN ({$string})");
					} else {

						$string = "";
						foreach ($data['unidade'] as $value) {

							$string .= "'{$value}'";
						}

						$whereEmp .= ($whereEmp != "" ? " AND cl.id_unidade = {$string}" : " cl.id_unidade = {$string}");
						$wherePatr .= ($wherePatr != "" ? " AND cl.id_unidade = {$string} " : " cl.id_unidade = {$string}");
					}
				}

				//BUSCANDO SIND EMP POR (CNPJ, SIGLA...)
				if (!empty($buscaEmp)) {
					if (count($buscaEmp) > 1) {
						$string = "";
						foreach ($buscaEmp as $value) {
							// $column = strstr($value, "+", true);
							// $content = substr(strstr($value, "+"), 1);

							$string .= "'{$value}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$whereEmp .= $whereEmp != "" ? " AND se.id_sinde IN ({$string})" : " se.id_sinde IN ({$string})";
						$wherePatr .= $wherePatr != "" ? " AND bemp.sind_empregados_id_sinde1 IN ({$string})" : " bemp.sind_empregados_id_sinde1 IN ({$string})";
					} else {
						$string = "";
						foreach ($buscaEmp as $value) {
							// $column = strstr($value, "+", true);
							// $content = substr(strstr($value, "+"), 1);

							$string .= "'{$value}'";
						}

						$whereEmp .= $whereEmp != "" ? " AND se.id_sinde = {$string}" : " se.id_sinde = {$string}";
						$wherePatr .= $wherePatr != "" ? " AND bemp.sind_empregados_id_sinde1 IN ({$string})" : " bemp.sind_empregados_id_sinde1 IN ({$string})";
					}
				}

				

				//BUSCANDO SIND PATR POR (CNPJ, SIGLA...)
				if (!empty($buscaPatr)) {
					if (count($buscaPatr) > 1) {
						$string = "";
						foreach ($buscaPatr as $value) {
							// $column = strstr($value, "+", true);
							// $content = substr(strstr($value, "+"), 1);

							$string .= "'{$value}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$wherePatr .= $wherePatr != "" ? " AND sp.id_sindp IN ({$string})" : " sp.id_sindp IN ({$string})";
						$whereEmp .= $whereEmp != "" ? " AND btro.sind_patronal_id_sindp IN ({$string})" : " btro.sind_patronal_id_sindp IN ({$string})";
						
					} else {
						$string = "";
						foreach ($buscaPatr as $value) {
							// $column = strstr($value, "+", true);
							// $content = substr(strstr($value, "+"), 1);

							$string .= "'{$value}'";
						}

						$wherePatr .= $wherePatr != "" ? " AND sp.id_sindp = {$string}" : " sp.id_sindp = {$string}";
						$whereEmp .= $whereEmp != "" ? " AND btro.sind_patronal_id_sindp IN ({$string})" : " btro.sind_patronal_id_sindp IN ({$string})";
					}
				}

				//QUERY SIND EMPREGADOS
				$wh = ($whereEmp ? 'WHERE' : '');
				$sqlLaboral = "SELECT
					se.id_sinde,
					se.sigla_sinde,
					se.razaosocial_sinde,
					se.municipio_sinde,
					se.uf_sinde,
					lc.uf,
					lc.regiao,
					lc.municipio,
					bem.dataneg
				FROM sind_emp as se
				LEFT JOIN base_territorialsindemp as bem on bem.sind_empregados_id_sinde1 = se.id_sinde
				LEFT join base_territorialsindpatro as btro on btro.localizacao_id_localizacao1 = bem.localizacao_id_localizacao1 and btro.classe_cnae_idclasse_cnae = bem.classe_cnae_idclasse_cnae
				LEFT JOIN localizacao as lc on lc.id_localizacao = bem.localizacao_id_localizacao1
				LEFT JOIN classe_cnae as cc on cc.id_cnae = bem.classe_cnae_idclasse_cnae

				LEFT JOIN cliente_unidades as cl on (cl.localizacao_id_localizacao = lc.id_localizacao and JSON_CONTAINS(cl.cnae_unidade, CONCAT('{\"id\":',bem.classe_cnae_idclasse_cnae,'}'), '$'))
				LEFT JOIN cliente_matriz as mt on mt.id_empresa = cl.cliente_matriz_id_empresa
				LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = mt.cliente_grupo_id_grupo_economico
				
				{$wh} {$whereEmp}
								
								
								GROUP BY se.id_sinde
				";
				$this->logger->debug($sqlLaboral);

				$wh1 = ($wherePatr ? 'WHERE' : '');
				$sqlPatronal = "SELECT
								sp.id_sindp,
								sp.sigla_sp,
								sp.razaosocial_sp,
								sp.municipio_sp,
								sp.uf_sp,
								lc.uf,
								lc.regiao,
								lc.municipio
							FROM sind_patr as sp
							LEFT JOIN base_territorialsindpatro as bp on bp.sind_patronal_id_sindp = sp.id_sindp
							LEFT join base_territorialsindemp as bemp on bp.localizacao_id_localizacao1 = bemp.localizacao_id_localizacao1 and bp.classe_cnae_idclasse_cnae = bemp.classe_cnae_idclasse_cnae
							LEFT JOIN localizacao as lc on lc.id_localizacao = bp.localizacao_id_localizacao1

							LEFT JOIN base_territorialsindemp as bem on bem.localizacao_id_localizacao1 = lc.id_localizacao and bem.classe_cnae_idclasse_cnae = bp.classe_cnae_idclasse_cnae
							LEFT JOIN classe_cnae as cc on cc.id_cnae = bem.classe_cnae_idclasse_cnae

							LEFT JOIN cliente_unidades as cl on (cl.localizacao_id_localizacao = lc.id_localizacao and JSON_CONTAINS(cl.cnae_unidade, CONCAT('{\"id\":',bem.classe_cnae_idclasse_cnae,'}'), '$'))
							LEFT JOIN cliente_matriz as mt on mt.id_empresa = cl.cliente_matriz_id_empresa
							LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = mt.cliente_grupo_id_grupo_economico
							
								{$wh1} {$wherePatr} 
								
								GROUP BY sp.id_sindp
				";
				$this->logger->debug($sqlPatronal);

				$resultEmp = mysqli_query($this->db, $sqlLaboral);
				$resultPatr = mysqli_query($this->db, $sqlPatronal);

				$qtdEmp = mysqli_num_rows($resultEmp);
				$qtdPatr = mysqli_num_rows($resultPatr);

				$response['response_data']['qtdEmp'] = $qtdEmp;
				$response['response_data']['qtdPatr'] = $qtdPatr;

				//CONSTROI OBJETO SIND EMP APÓS FILTRO
				$lista = [];
				$idLista = [];
				while ($obj = $resultEmp->fetch_object()) {
					// $this->logger->debug($obj);
					$objEmp = new stdClass();
					$objEmp->id_sinde = $obj->id_sinde;
					// $objEmp->sigla_sinde = $obj->sigla_sinde;
					// $objEmp->municipio = $obj->municipio;
					array_push($lista, $objEmp);
					array_push($idLista, $objEmp->id_sinde);
				}

				$this->logger->debug($lista);

				//CONSTROI OBJETO SIND PATR APÓS FILTRO
				$listaPatr = [];
				$idListaPatr = [];
				while ($obj = $resultPatr->fetch_object()) {
					$this->logger->debug($obj);
					$objPatr = new stdClass();
					$objPatr->id_sindp = $obj->id_sindp;
					$objPatr->sigla_sp = $obj->sigla_sp;
					$objPatr->municipio = $obj->municipio;
					array_push($listaPatr, $objPatr);
					array_push($idListaPatr, $objPatr->id_sindp);
				}

				$this->logger->debug($listaPatr);
				$vigente = 0;
				$vencido = 0;
				//MADATOS SINDICAIS EMP
				for ($i = 0; $i < count($lista); $i++) {
					$paraemp = true;
					$sqlMandEmp = "SELECT distinct
									termino_mandatoe as termino
									FROM sind_diremp
									WHERE sind_emp_id_sinde = '{$lista[$i]->id_sinde}'
					";

					$this->logger->debug($sqlMandEmp);

					$resultMandEmp = mysqli_query($this->db, $sqlMandEmp);

					while ($obj = $resultMandEmp->fetch_object()) {

						if ($paraemp) {
							if ($obj->termino >= date_format((new DateTime('now')), "Y-m-d")) {
								$vigente++;
							} else if ($obj->termino < date_format((new DateTime('now')), "Y-m-d")) {
								$vencido++;
							}
						}
						$paraemp = false;
					}
				}

				//MADATOS SINDICAIS PATR
				for ($i = 0; $i < count($listaPatr); $i++) {
					$parapatr = true;
					$sqlMandPatr = "SELECT distinct
									termino_mandatop as termino
									FROM sind_dirpatro
									WHERE sind_patr_id_sindp = '{$listaPatr[$i]->id_sindp}'
					";

					$this->logger->debug($sqlMandPatr);

					$resultMandPatr = mysqli_query($this->db, $sqlMandPatr);

					while ($obj = $resultMandPatr->fetch_object()) {
						if ($parapatr) {
							if ($obj->termino >= date_format((new DateTime('now')), "Y-m-d")) {
								$vigente++;
							} else if ($obj->termino < date_format((new DateTime('now')), "Y-m-d")) {
								$vencido++;
							}
						}
						$parapatr = false;
					}
				}

				$this->logger->debug('Vigente ' . $vigente);
				$this->logger->debug('Vencido ' . $vencido);

				$response['response_data']['mandVigente'] = $vigente;
				$response['response_data']['mandVencido'] = $vencido;


				//NEGOCIAÇÕES VIGENTES E VENCIDAS POR SIND EMP
				$negViegnte = 0;
				$negVencida = 0;
				for ($i = 0; $i < count($lista); $i++) {
					$sqlMandEmp = "SELECT
										fase
									FROM acompanhanto_cct
									WHERE sind_emp_id_sinde = '{$lista[$i]->id_sinde}'
					";

					$this->logger->debug($sqlMandEmp);

					$resultMandEmp = mysqli_query($this->db, $sqlMandEmp);

					while ($obj = $resultMandEmp->fetch_object()) {

						if ($obj->fase == "Fechada") {
							$negVencida++;
						} else {
							$negViegnte++;
						}
					}
				}

				$this->logger->debug('Neg Vigente ' . $negViegnte);
				$this->logger->debug('Neg Vencido ' . $negVencida);

				$response['response_data']['negVigente'] = $negViegnte;
				$response['response_data']['negVencida'] = $negVencida;


				//NEGOCIAÇÕES EM ABERTO POR ESTADO 
				if ($data['gec'] == "cliente_grupo_id_grupo_economico") {
					$gecc = 'cm.cliente_grupo_id_grupo_economico';
				}else {
					$gecc = $data['gec'];
				}
				
				$listaGraf = [];
				$novaLista = implode("','", $idLista);

				$sqlEst = "SELECT distinct
								lc.uf as uf,
								COUNT(DISTINCT ct.idacompanhanto_cct) as uf_qtd
							FROM acompanhanto_cct as ct
							LEFT JOIN sind_emp as se on se.id_sinde = ct.sind_emp_id_sinde
							LEFT JOIN base_territorialsindemp as be on be.sind_empregados_id_sinde1 = se.id_sinde
							LEFT JOIN localizacao as lc on lc.id_localizacao = be.localizacao_id_localizacao1
							WHERE ct.sind_emp_id_sinde IN ('{$novaLista}') AND JSON_OVERLAPS(ct.ids_cnaes,
                            CAST((SELECT distinct JSON_ARRAYAGG(CAST(ce.classe_cnae_idclasse_cnae AS CHAR)) FROM cnae_emp as ce 
                            inner join
                            classe_cnae as cc ON cc.id_cnae = ce.classe_cnae_idclasse_cnae  WHERE
                            cliente_unidades_id_unidade IN (Select cu.id_unidade from cliente_unidades as cu WHERE 
                            cliente_matriz_id_empresa IN (Select id_empresa from cliente_matriz WHERE 
                            cliente_grupo_id_grupo_economico = {$data['grupo']} ))) AS JSON)) AND ct.fase != 'Fechada'
							GROUP BY lc.uf
							ORDER BY lc.uf ASC;
				";

				$resultEst = mysqli_query($this->db, $sqlEst);


				while ($objEst = $resultEst->fetch_object()) {
					$this->logger->debug($objEst);
					$objGraf = new stdClass;
					$objGraf->uf = $objEst->uf;
					$objGraf->qtd = $objEst->uf_qtd;

					array_push($listaGraf, $objGraf);
				}

				$response['response_data']['listaGrafico'] = $listaGraf;



				//NEGOCIAÇÕES EM ABERTO TABELAS DE SINDICATO E COMENTÁRIO
				$listaTab = [
					'AC' => [],
					'AL' => [],
					'AM' => [],
					'AP' => [],
					'BA' => [],
					'CE' => [],
					'DF' => [],
					'ES' => [],
					'GO' => [],
					'MA' => [],
					'MG' => [],
					'MS' => [],
					'MT' => [],
					'PA' => [],
					'PB' => [],
					'PE' => [],
					'PI' => [],
					'PR' => [],
					'RJ' => [],
					'RN' => [],
					'RO' => [],
					'RR' => [],
					'RS' => [],
					'SC' => [],
					'SE' => [],
					'SP' => [],
					'TO' => []
				];

				$sqlEst2 = "SELECT distinct
								lc.uf as uf,
								se.id_sinde,
								se.sigla_sinde,
								group_concat(DISTINCT nc.comentario SEPARATOR '; ') as comentarios
							FROM sind_emp as se
							LEFT JOIN base_territorialsindemp as be on be.sind_empregados_id_sinde1 = se.id_sinde
							left join note_cliente as nc ON nc.id_tipo_comentario = se.id_sinde AND tipo_comentario like 'laboral'
							LEFT JOIN localizacao as lc on lc.id_localizacao = be.localizacao_id_localizacao1
							WHERE se.id_sinde IN ('{$novaLista}')
				GROUP BY lc.uf, se.id_sinde
				ORDER BY lc.uf ASC
				";

				$resultEst2 = mysqli_query($this->db, $sqlEst2);


				while ($objEst2 = $resultEst2->fetch_object()) {
					$this->logger->debug($objEst2);
					$html = "";
					$html .= '<tr class="odd gradeX tbl-item">';
					$html .= '<td><a data-toggle="modal" href="#commentModal" onclick="setCommentID(\''.$objEst2->id_sinde.'\', \''.$objEst2->sigla_sinde.'\')" class="btn-default-alt"  id="bootbox-demo-5" data-dismiss="modal"><i class="fa fa-comment"></i></a></td>';
					$html .= '<td class="title">';
					$html .= $objEst2->sigla_sinde;
					$html .= '</td>';
					$html .= '<td>';
					$html .= $objEst2->comentarios;
					$html .= '</td>';

					array_push($listaTab[$objEst2->uf], $html);
				}

				$response['response_data']['listaTab'] = $listaTab;


				//PINTAR UFS COM COMENTÁRIOS

				$sqlEst2 = "SELECT distinct
								lc.uf as uf,
								se.id_sinde,
								se.sigla_sinde,
								group_concat(DISTINCT nc.comentario SEPARATOR '; ') as comentarios
							FROM sind_emp as se
							LEFT JOIN base_territorialsindemp as be on be.sind_empregados_id_sinde1 = se.id_sinde
							left join note_cliente as nc ON nc.id_tipo_comentario = se.id_sinde AND tipo_comentario like 'laboral'
							INNER JOIN localizacao as lc on lc.id_localizacao = be.localizacao_id_localizacao1
							WHERE se.id_sinde IN ('{$novaLista}') AND nc.comentario IS NOT NULL
				GROUP BY lc.uf
				ORDER BY lc.uf ASC
				";

				$pinta_ufs = [];

				$resultEst2 = mysqli_query($this->db, $sqlEst2);


				while ($objEst2 = $resultEst2->fetch_object()) {
					$this->logger->debug($objEst2);

					array_push($pinta_ufs, $objEst2->uf);
				}

				$response['response_data']['listaPintar'] = $pinta_ufs;



				//ORGANIZAÇÃO SINDICAL LABORAL
				$organizacaoSind = [];
				for ($i = 0; $i < count($lista); $i++) {
					$sqlOrg = "SELECT distinct
									id_sinde,
									municipio_sinde,
									sigla_sinde,
									cnpj_sinde,
									ce.nome_centralsindical,
									ass.nome as confederacao,
									asf.nome as federacao
								FROM sind_emp as se
								LEFT JOIN central_sindical as ce on se.central_sindical_id_centralsindical = ce.id_centralsindical
								LEFT JOIN associacao as ass on ass.id_associacao = se.confederacao_id_associacao
								LEFT JOIN associacao as asf on asf.id_associacao = se.federacao_id_associacao
								WHERE id_sinde = '{$lista[$i]->id_sinde}'
								ORDER BY municipio_sinde ASC
					";

					$this->logger->debug($sqlOrg);

					$resultOrg = mysqli_query($this->db, $sqlOrg);

					while ($obj = $resultOrg->fetch_object()) {
						$this->logger->debug($obj);
						$orgSind = new stdClass;
						$orgSind->municipio_sinde = $obj->municipio_sinde;
						$orgSind->sigla_sinde = $obj->sigla_sinde;
						$orgSind->cnpj_sinde = $obj->cnpj_sinde;
						$orgSind->nome_centralsindical = $obj->nome_centralsindical;
						$orgSind->confederacao = $obj->confederacao;
						$orgSind->federacao = $obj->federacao;

						array_push($organizacaoSind, $orgSind);
					}
				}


				$this->logger->debug($organizacaoSind);

				//GERANO TABELA ORGANIZAÇÃO SINDICAL
				$html = "";
				$html = "
					<thead>
						<th>Localidade</th>
						<th>Sindicato (Sigla Laboral)</th>
						<th>Federação Laboral</th>
						<th>Confederação Laboral</th>
						<th>Central Sindical Laboral</th>
						<th>MTE</th>
						<th>EXCEL</th>
					</thead>
				";
				for ($i = 0; $i < count($organizacaoSind); $i++) {

					$html .= "<tr class='tbl-item'>";
					$html .= "<td class='title'>{$organizacaoSind[$i]->municipio_sinde}</td>";
					$html .= "<td class='desc'>{$organizacaoSind[$i]->sigla_sinde}</td>";
					$html .= "<td>{$organizacaoSind[$i]->federacao}</td>";
					$html .= "<td>{$organizacaoSind[$i]->confederacao}</td>";
					$html .= "<td>{$organizacaoSind[$i]->nome_centralsindical}</td>";
					$html .= '<td><a href="http://www3.mte.gov.br/sistemas/CNES/usogeral/HistoricoEntidadeDetalhes.asp?NRCNPJ=' . str_replace(str_split('./-'), "", $organizacaoSind[$i]->cnpj_sinde) . '" target="_blank" class="btn-default-alt" style="display: flex; box-shadow: none; justify-content: center; border: none;"><img src="includes/img/icon-MTE.png" alt="" style="width: 20px;"></a></td>'; // <img src="includes/img/icon-MTE.png" alt="" style="width: 20px;">
					$html .= '<td class="desc" style="display: flex; justify-content: center; align-items: center;">';
					$html .= '<a class="btn btn-primary" style="padding: 0; background: transparent; box-shadow: none;" onclick="baixarExcelLaboral(' . $organizacaoSind[$i]->cnpj_sinde . ')"><img src="includes/img/icon-excel.png" alt="" style="width: 20px;"></a>';
					$html .= '</td>';
					$html .= "</tr>";
				}

				$this->logger->debug($html);
				$response['response_data']['tabelaOrganizacao'] = $html;


				//ORGANIZAÇÃO SINDICAL PATRONAL
				$organizacaoSindp = [];
				for ($i = 0; $i < count($listaPatr); $i++) {
					$sqlOrg = "SELECT distinct
									id_sindp,
									municipio_sp,
									sigla_sp,
									cnpj_sp,
									ass.nome as confederacao,
									asf.nome as federacao
								FROM sind_patr as se
								LEFT JOIN associacao as ass on ass.id_associacao = se.confederacao_id_associacao
								LEFT JOIN associacao as asf on asf.id_associacao = se.federacao_id_associacao
								WHERE id_sindp = '{$listaPatr[$i]->id_sindp}'
								ORDER BY municipio_sp ASC
					";

					$this->logger->debug($sqlOrg);

					$resultOrgp = mysqli_query($this->db, $sqlOrg);

					while ($obj = $resultOrgp->fetch_object()) {
						$this->logger->debug($obj);
						$orgSind = new stdClass;
						$orgSind->municipio_sp = $obj->municipio_sp;
						$orgSind->sigla_sp = $obj->sigla_sp;
						$orgSind->cnpj_sp = $obj->cnpj_sp;
						$orgSind->confederacao = $obj->confederacao;
						$orgSind->federacao = $obj->federacao;

						array_push($organizacaoSindp, $orgSind);
					}
				}


				$this->logger->debug($organizacaoSindp);

				//GERANO TABELA ORGANIZAÇÃO SINDICAL PATRONAL
				$html = "";
				$html = "
					<thead>
						<th>Localidade</th>
						<th>Sindicato (Sigla Patronal)</th>
						<th>Federação Patronal</th>
						<th>Confederação Patronal</th>
						<th>MTE</th>
						<th>EXCEL</th>
					</thead>
				";
				for ($i = 0; $i < count($organizacaoSindp); $i++) {

					$html .= "<tr class='tbl-item'>";
					$html .= "<td class='title'>{$organizacaoSindp[$i]->municipio_sp}</td>";
					$html .= "<td class='desc'>{$organizacaoSindp[$i]->sigla_sp}</td>";
					$html .= "<td>{$organizacaoSindp[$i]->federacao}</td>";
					$html .= "<td>{$organizacaoSindp[$i]->confederacao}</td>";
					$html .= '<td><a href="http://www3.mte.gov.br/sistemas/CNES/usogeral/HistoricoEntidadeDetalhes.asp?NRCNPJ=' .  str_replace(str_split('./-'), "", $organizacaoSindp[$i]->cnpj_sp) . '" target="_blank" class="btn-default-alt" style="display: flex; box-shadow: none; justify-content: center; border: none;"><img src="includes/img/icon-MTE.png" alt="" style="width: 20px;"></a></td>';
					$html .= '<td class="desc" style="display: flex; justify-content: center; align-items: center;">';
					$html .= '<a class="btn btn-primary" style="padding: 0; background: transparent; box-shadow: none;" onclick="baixarExcelPatronal(' . $organizacaoSindp[$i]->cnpj_sp . ')"><img src="includes/img/icon-excel.png" alt="" style="width: 20px;"></a>';
					$html .= '</td>';
					$html .= "</tr>";
				}

				$this->logger->debug($html);
				$response['response_data']['tabelaPatr'] = $html;

				//GERANDO DADOS PARA GRÁFICO EM PIZZA
				$whereId = '';
				for ($i = 1; $i < count($idLista); $i++) {
					$whereId .= " OR id_sinde = {$idLista[$i]}";
				}

				$sqlGraf = "SELECT
								tab_1.nome_centralsindical,
								COUNT(*) as qtd_centrais
							FROM (SELECT
								id_sinde,
								municipio_sinde,
								sigla_sinde,
								ce.nome_centralsindical,
								ass.nome as confederacao,
								asf.nome as federacao
							FROM sind_emp as se
							LEFT JOIN central_sindical as ce on se.central_sindical_id_centralsindical = ce.id_centralsindical
							LEFT JOIN associacao as ass on ass.id_associacao = se.confederacao_id_associacao
							LEFT JOIN associacao as asf on asf.id_associacao = se.federacao_id_associacao
							WHERE id_sinde = '{$idLista[0]}'{$whereId})tab_1
							
							GROUP BY tab_1.nome_centralsindical
				";

				$this->logger->debug($sqlGraf);

				$resultGraf = mysqli_query($this->db, $sqlGraf);
				$listaGrafPizza = [];
				while ($obj = $resultGraf->fetch_object()) {
					$pizza = new stdClass;
					$pizza->central = $obj->nome_centralsindical;
					$pizza->qtd = $obj->qtd_centrais;

					array_push($listaGrafPizza, $pizza);
				}

				$response['response_data']['pizza'] = $listaGrafPizza;

				//TABELA DIRIGENTES SINDICAIS FULL

				
				$dirigentes = [];
				for ($i = 0; $i < count($lista); $i++) {
					$sql = "SELECT distinct
								id_diretoriae as id,
								dirigente_e as dirigente,
								funcao_e as cargo,
								IF(cm.cliente_grupo_id_grupo_economico = {$gecc}, situacao_e, '') as situacao,
								se.sigla_sinde as sigla,
								IF(cm.cliente_grupo_id_grupo_economico = {$gecc}, cu.nome_unidade, '') as esta,
								date_format(termino_mandatoe, \"%d/%m/%Y\") as termino,
								date_format(inicio_mandatoe, \"%d/%m/%Y\") as inicio,
								cm.razao_social as empresa
							FROM sind_diremp as sd
							LEFT JOIN cliente_unidades as cu on cu.id_unidade = sd.cliente_unidades_id_unidade
							LEFT JOIN cliente_matriz as cm on cm.id_empresa = cu.cliente_matriz_id_empresa
							LEFT JOIN sind_emp as se on se.id_sinde = sd.sind_emp_id_sinde 
							WHERE dirigente_e IS NOT NULL AND sind_emp_id_sinde = '{$lista[$i]->id_sinde}' group by dirigente
							ORDER BY dirigente ASC
					";

					$this->logger->debug($sql);

					$resultDir = mysqli_query($this->db, $sql);

					while ($obj = $resultDir->fetch_object()) {
						$dirSind = new stdClass;
						$dirSind->id = $obj->id;
						$dirSind->dirigente = $obj->dirigente;
						$dirSind->cargo = $obj->cargo;
						$dirSind->situacao = $obj->situacao;
						$dirSind->termino = $obj->termino;
						$dirSind->inicio = $obj->inicio;
						$dirSind->sigla = $obj->sigla;
						$dirSind->empresa = $obj->empresa;
						$dirSind->esta = $obj->esta;

						array_push($dirigentes, $dirSind);
					}
				}

				$htmlDirig = "";
				$htmlDirig = "
					<thead>
						<th>Nome</th>
						<th>Cargo</th>
						<th>Sigla</th>
						<th>Início do Mandato</th>
						<th>Término do Mandato</th>
						<th>Estabelecimento</th>
						<th>Afastado para atividades</th>
					</thead>
				";
				$htmlDirig .= "<tbody>";
				for ($i = 0; $i < count($dirigentes); $i++) {

					$htmlDirig .= "<tr class='tbl-item'>";
					$htmlDirig .= "<td class='title'>{$dirigentes[$i]->dirigente}</td>";
					$htmlDirig .= "<td class='desc'>{$dirigentes[$i]->cargo}</td>";
					$htmlDirig .= "<td class='sigla'>{$dirigentes[$i]->sigla}</td>";
					$htmlDirig .= "<td>{$dirigentes[$i]->inicio}</td>";
					$htmlDirig .= "<td>{$dirigentes[$i]->termino}</td>";
					$htmlDirig .= "<td><a data-toggle='modal' href='#estaModal' onclick='setEstaID(\"".$dirigentes[$i]->id."\")' class='btn-default-alt'  id='bootbox-demo-5' data-dismiss='modal'><i class='fa fa-refresh'></i></a> {$dirigentes[$i]->esta}</td>";
					$htmlDirig .= "<td>{$dirigentes[$i]->situacao}</td>";
					$htmlDirig .= "</tr>";
				}
				$htmlDirig .= "</tbody>";
				$this->logger->debug($htmlDirig);
				$response['response_data']['dirigentes_full'] = $htmlDirig;


				//TABELA DIRIGENTES SINDICAIS
				$dirigentes = [];
				for ($i = 0; $i < count($lista); $i++) {
					$sql = "SELECT distinct
								dirigente_e as dirigente,
								funcao_e as cargo,
								situacao_e as situacao,
								se.sigla_sinde as sigla,
								date_format(termino_mandatoe, \"%d/%m/%Y\") as termino,
								date_format(inicio_mandatoe, \"%d/%m/%Y\") as inicio,
								cm.razao_social as empresa
							FROM sind_diremp as sd
							LEFT JOIN cliente_matriz as cm on cm.id_empresa = sd.cliente_matriz_id_empresa
							LEFT JOIN sind_emp as se on se.id_sinde = sd.sind_emp_id_sinde 
							WHERE dirigente_e IS NOT NULL AND sind_emp_id_sinde = '{$lista[$i]->id_sinde}' group by dirigente
							ORDER BY dirigente ASC
					";

					$this->logger->debug($sql);

					$resultDir = mysqli_query($this->db, $sql);

					while ($obj = $resultDir->fetch_object()) {
						$dirSind = new stdClass;
						$dirSind->dirigente = $obj->dirigente;
						$dirSind->cargo = $obj->cargo;
						$dirSind->situacao = $obj->situacao;
						$dirSind->termino = $obj->termino;
						$dirSind->inicio = $obj->inicio;
						$dirSind->sigla = $obj->sigla;
						$dirSind->empresa = $obj->empresa;

						array_push($dirigentes, $dirSind);
					}
				}

				$htmlDirig = "";
				$htmlDirig = "
					<thead>
						<th>Nome</th>
						<th>Cargo</th>
						<th>Sigla</th>
						<th>Início do Mandato</th>
						<th>Término do Mandato</th>
					</thead>
				";
				$htmlDirig .= "<tbody>";
				for ($i = 0; $i < count($dirigentes); $i++) {

					$htmlDirig .= "<tr class='tbl-item'>";
					$htmlDirig .= "<td class='title'>{$dirigentes[$i]->dirigente}</td>";
					$htmlDirig .= "<td class='desc'>{$dirigentes[$i]->cargo}</td>";
					$htmlDirig .= "<td class='sigla'>{$dirigentes[$i]->sigla}</td>";
					$htmlDirig .= "<td>{$dirigentes[$i]->inicio}</td>";
					$htmlDirig .= "<td>{$dirigentes[$i]->termino}</td>";
					$htmlDirig .= "</tr>";
				}
				$htmlDirig .= "</tbody>";
				$this->logger->debug($htmlDirig);
				$response['response_data']['dirigentes'] = $htmlDirig;

				//TABELA DIRIGENTES SINDICAIS PATRONAIS FULL
				$dirigentesP = [];
				for ($i = 0; $i < count($listaPatr); $i++) {
					$sql = "SELECT distinct
								id_diretoriap as id,
								dirigente_p as dirigente,
								funcao_p as cargo,
								IF(cm.cliente_grupo_id_grupo_economico = {$gecc}, situacao_p, '') as situacao,
								sp.sigla_sp as sigla,
								IF(cm.cliente_grupo_id_grupo_economico = {$gecc}, cu.nome_unidade, '') as esta,
								date_format(termino_mandatop, \"%d/%m/%Y\") as termino,
								date_format(inicio_mandatop, \"%d/%m/%Y\") as inicio,
								cm.razao_social as empresa
							FROM sind_dirpatro as sd
							LEFT JOIN cliente_unidades as cu on cu.id_unidade = sd.cliente_unidades_id_unidade 
							LEFT JOIN cliente_matriz as cm on cm.id_empresa = cu.cliente_matriz_id_empresa
							LEFT JOIN sind_patr as sp on sp.id_sindp = sd.sind_patr_id_sindp
							WHERE dirigente_p IS NOT NULL AND sind_patr_id_sindp = '{$listaPatr[$i]->id_sindp}' group by dirigente
							ORDER BY dirigente ASC
					";

					$this->logger->debug($sql);

					$resultDir = mysqli_query($this->db, $sql);

					while ($obj = $resultDir->fetch_object()) {
						$dirSind = new stdClass;
						$dirSind->id = $obj->id;
						$dirSind->dirigente = $obj->dirigente;
						$dirSind->cargo = $obj->cargo;
						$dirSind->situacao = $obj->situacao;
						$dirSind->termino = $obj->termino;
						$dirSind->inicio = $obj->inicio;
						$dirSind->sigla = $obj->sigla;
						$dirSind->empresa = $obj->empresa;
						$dirSind->esta = $obj->esta;

						array_push($dirigentesP, $dirSind);
					}
				}

				$htmlDirig = "";
				$htmlDirig = "
					<thead>
						<th>Nome</th>
						<th>Cargo</th>
						<th>Sigla</th>
						<th>Início do Mandato</th>
						<th>Término do Mandato</th>
						<th>Estabelecimento</th>
						<th>Afastado para atividade</th>
					</thead>
				";
				$htmlDirig .= "<tbody>";
				for ($i = 0; $i < count($dirigentesP); $i++) {

					$htmlDirig .= "<tr class='tbl-item'>";
					$htmlDirig .= "<td class='title'>{$dirigentesP[$i]->dirigente}</td>";
					$htmlDirig .= "<td class='desc'>{$dirigentesP[$i]->cargo}</td>";
					$htmlDirig .= "<td class='sigla'>{$dirigentesP[$i]->sigla}</td>";
					$htmlDirig .= "<td>{$dirigentesP[$i]->inicio}</td>";
					$htmlDirig .= "<td>{$dirigentesP[$i]->termino}</td>";
					$htmlDirig .= "<td><a data-toggle='modal' href='#estaPModal' onclick='setEstaPID(\"".$dirigentesP[$i]->id."\")' class='btn-default-alt'  id='bootbox-demo-5' data-dismiss='modal'><i class='fa fa-refresh'></i></a> {$dirigentesP[$i]->esta}</td>";
					$htmlDirig .= "<td>{$dirigentesP[$i]->situacao}</td>";
					$htmlDirig .= "</tr>";
				}
				$htmlDirig .= "</tbody>";
				$this->logger->debug($htmlDirig);
				$response['response_data']['dirigentes_patr_full'] = $htmlDirig;

				//TABELA DIRIGENTES SINDICAIS PATRONAIS
				$dirigentesP = [];
				for ($i = 0; $i < count($listaPatr); $i++) {
					$sql = "SELECT distinct
								dirigente_p as dirigente,
								funcao_p as cargo,
								situacao_p as situacao,
								sp.sigla_sp as sigla,
								date_format(termino_mandatop, \"%d/%m/%Y\") as termino,
								date_format(inicio_mandatop, \"%d/%m/%Y\") as inicio,
								cm.razao_social as empresa
							FROM sind_dirpatro as sd
							LEFT JOIN cliente_matriz as cm on cm.id_empresa = sd.cliente_matriz_id_empresa
							LEFT JOIN sind_patr as sp on sp.id_sindp = sd.sind_patr_id_sindp
							WHERE dirigente_p IS NOT NULL AND sind_patr_id_sindp = '{$listaPatr[$i]->id_sindp}' group by dirigente
							ORDER BY dirigente ASC
					";

					$this->logger->debug($sql);

					$resultDir = mysqli_query($this->db, $sql);

					while ($obj = $resultDir->fetch_object()) {
						$dirSind = new stdClass;
						$dirSind->dirigente = $obj->dirigente;
						$dirSind->cargo = $obj->cargo;
						$dirSind->situacao = $obj->situacao;
						$dirSind->termino = $obj->termino;
						$dirSind->inicio = $obj->inicio;
						$dirSind->sigla = $obj->sigla;
						$dirSind->empresa = $obj->empresa;

						array_push($dirigentesP, $dirSind);
					}
				}

				$htmlDirig = "";
				$htmlDirig = "
					<thead>
						<th>Nome</th>
						<th>Cargo</th>
						<th>Sigla</th>
						<th>Início do Mandato</th>
						<th>Término do Mandato</th>
					</thead>
				";
				$htmlDirig .= "<tbody>";
				for ($i = 0; $i < count($dirigentesP); $i++) {

					$htmlDirig .= "<tr class='tbl-item'>";
					$htmlDirig .= "<td class='title'>{$dirigentesP[$i]->dirigente}</td>";
					$htmlDirig .= "<td class='desc'>{$dirigentesP[$i]->cargo}</td>";
					$htmlDirig .= "<td class='sigla'>{$dirigentesP[$i]->sigla}</td>";
					$htmlDirig .= "<td>{$dirigentesP[$i]->inicio}</td>";
					$htmlDirig .= "<td>{$dirigentesP[$i]->termino}</td>";
					$htmlDirig .= "</tr>";
				}
				$htmlDirig .= "</tbody>";
				$this->logger->debug($htmlDirig);
				$response['response_data']['dirigentes_patr'] = $htmlDirig;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}


	function saveFilter($data = null)
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

			if ($response['response_status']['status'] == 1) {

				$filter = [];
				$filter = array_merge($data, $filter);
				unset($filter["module"]);
				unset($filter["action"]);

				$saveFilter = json_encode($filter);

				$this->logger->debug($saveFilter);

				$sql = "SELECT * FROM filtro_csv";

				$result = mysqli_query($this->db, $sql);

				$obj = $result->fetch_object();

				$this->logger->debug($obj);
				if (empty($obj)) {
					$sqlSave = "INSERT INTO filtro_csv (filtro, usuario) VALUES ('{$saveFilter}', 'Teste')";
				} else {
					$sqlSave = "UPDATE filtro_csv SET filtro = '{$saveFilter}'";
				}

				if ($result = mysqli_query($this->db, $sqlSave)) {
					$response['response_status']['status']     = 1;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Registro salvo com sucesso!';
				} else {
					$this->logger->debug($this->db->error);
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível salvar o filtro.';
				}

				$this->logger->debug($sqlSave);
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}


	function tabelaExcelLaboral($data = null)
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

			if ($response['response_status']['status'] == 1) {




				$sql = "SELECT distinct
				se.*,
				ce.nome_centralsindical,
				ass.nome as confederacao,
				asf.nome as federacao
			FROM sind_emp as se
			LEFT JOIN central_sindical as ce on se.central_sindical_id_centralsindical = ce.id_centralsindical
			LEFT JOIN associacao as ass on ass.id_associacao = se.confederacao_id_associacao
			LEFT JOIN associacao as asf on asf.id_associacao = se.federacao_id_associacao
			WHERE cnpj_sinde = {$data['id_sinde']}";


				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = "<thead>
							<th>CNPJ</th>
							<th>CÓDIGO</th>
							<th>SIGLA</th>
							<th>RAZÃO SOCIAL</th>
							<th>DENOMINAÇÃO</th>
							<th>ENDEREÇO</th>
							<th>MUNICÍPIO</th>
							<th>UF</th>
							<th>TELEFONE 1º OPÇÃO</th>
							<th>TELEFONE 2º OPÇÃO</th>
							<th>TELEFONE 3º OPÇÃO</th>
							<th>E-MAIL 1º OPÇÃO</th>
							<th>E-MAIL 2º OPÇÃO</th>
							<th>E-MAIL 3º OPÇÃO</th>
							<th>SITE</th>
							<th>FEDERAÇÃO</th>
							<th>CONFEDERAÇÃO</th>
							<th>CENTRAL SINDICAL</th>
						</thead>";

					while ($obj = $resultsql->fetch_object()) {



						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td>';
						$html .= $obj->cnpj_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->codigo_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->sigla_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->razaosocial_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->denominacao_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->logradouro_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->municipio_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->uf_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->fone1_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->fone2_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->fone3_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->email1_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->email2_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->email3_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->site_sinde;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->federacao;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->confederacao;
						$html .= '</td>';

						$html .= '<td>';
						$html .= $obj->nome_centralsindical;
						$html .= '</td>';

						$html .= '</tr>';
					}

					$response['response_data']['tabela'] 	= $html;

					$this->logger->debug($obj);
				} else {
					$response = $this->response;
				}
			} else {
				$response = $this->response;
			}
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}



	function tabelaExcelPatronal($data = null)
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

			if ($response['response_status']['status'] == 1) {




				$sql = "SELECT distinct
				se.*,
				ass.nome as confederacao,
				asf.nome as federacao
			FROM sind_patr as se
			LEFT JOIN associacao as ass on ass.id_associacao = se.confederacao_id_associacao
			LEFT JOIN associacao as asf on asf.id_associacao = se.federacao_id_associacao
			WHERE cnpj_sp = {$data['id_sp']}";


				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = "<thead>
							<th>CNPJ</th>
							<th>CÓDIGO</th>
							<th>SIGLA</th>
							<th>RAZÃO SOCIAL</th>
							<th>DENOMINAÇÃO</th>
							<th>ENDEREÇO</th>
							<th>MUNICÍPIO</th>
							<th>UF</th>
							<th>TELEFONE 1º OPÇÃO</th>
							<th>TELEFONE 2º OPÇÃO</th>
							<th>TELEFONE 3º OPÇÃO</th>
							<th>E-MAIL 1º OPÇÃO</th>
							<th>E-MAIL 2º OPÇÃO</th>
							<th>E-MAIL 3º OPÇÃO</th>
							<th>SITE</th>
							<th>CONFEDERAÇÃO</th>
							<th>FEDERAÇÃO</th>
							
						</thead>";

					while ($obj = $resultsql->fetch_object()) {



						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td>';
						$html .= $obj->cnpj_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->codigo_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->sigla_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->razaosocial_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->denominacao_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->logradouro_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->municipio_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->uf_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->fone1_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->fone2_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->fone3_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->email1_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->email2_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->email3_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->site_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->confederacao;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->federacao;
						$html .= '</td>';

						$html .= '</tr>';
					}

					$response['response_data']['tabela'] 	= $html;

					$this->logger->debug($obj);
				} else {
					$response = $this->response;
				}
			} else {
				$response = $this->response;
			}
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function addComm($data = null)
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
				$gecc = $data['gec'];
				if($gecc == 'cliente_grupo_id_grupo_economico'){
					$gecc = '0';
				}
				$sql = "insert into note_cliente (tipo_comentario,
				 tipo_notificacao,
				  id_tipo_comentario,
				   tipo_usuario_destino,
				    id_tipo_usuario_destino,
					 usuario_adm_id_user,comentario)
					  values ('laboral',
					  'fixa',
					  {$data['id_sinde']}, 
					   'grupo', 
					   {$gecc},
					   {$data['uid']},
						'{$data['comm']}');
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

	function updateEsta($data = null)
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
				$sql = "update sind_diremp set cliente_unidades_id_unidade = {$data['esta']}, situacao_e = '{$data['afasta']}' 
				WHERE id_diretoriae = {$data['id_dir']} ;
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

	function updateEstaP($data = null)
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
				$sql = "update sind_dirpatro set cliente_unidades_id_unidade = {$data['esta']}, situacao_p = '{$data['afasta']}' 
				WHERE id_diretoriap = {$data['id_dir']} ;
				";
				$this->logger->debug($sql);
				if (!mysqli_query($this->db, $sql)) {


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
