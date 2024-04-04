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


include_once "helpers.php";

require(__DIR__ . '/dompdf/autoload.inc.php');

use Dompdf\Dompdf;
use Dompdf\Options;



// require_once 'dompdf/autoload.inc.php'; //we've assumed that the dompdf directory is in the same directory as your PHP file. If not, adjust your autoload.inc.php i.e. first line of code accordingly.

class perfil1
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

	private $grupoEconLogado;

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

	




	// function getConsultaClausula($data = null)
	// {

	// 	if ($this->response['response_status']['status'] == 1) {

	// 		// Carregando a resposta padrão da função
	// 		$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

	// 		if ($response['response_status']['status'] == 1) {
	// 			if (empty($this->db)) {
	// 				$connectdb = $this->connectdb();

	// 				if ($connectdb['response_status']['status'] == 0) {
	// 					$response = $connectdb;
	// 				}
	// 			}
	// 		}

	// 		$this->logger->debug($connectdb);

	// 		if ($response['response_status']['status'] == 1) {
	// 			$filtro = ";";

	// 			// if ($data != null) {
	// 			// 	$filtro = $data['filtro'];
	// 			// }

	// 			/** */
	// 			$filter = [];

	// 			$filter = array_merge($data, $filter);
	// 			unset($filter["module"]);
	// 			unset($filter["action"]);

	// 			$this->logger->debug($data);

	// 			$where = ""; //main.data_aprovacao IS NOT NULL

	// 			if ($filter['nome_doc'] != "") {
	// 				if (count($filter['nome_doc']) > 1) {
	// 					$string = "";
	// 					foreach ($filter['nome_doc'] as $value) {

	// 						$string .= "'{$value}'" . ',';
	// 					}

	// 					$string = implode(",", array_filter(explode(",", $string)));
	// 					$where .= ($where != "" ? " AND doc.tipo_doc_idtipo_doc IN ({$string})" : " doc.tipo_doc_idtipo_doc IN ({$string})");
	// 				} else {

	// 					$string = "";
	// 					foreach ($filter['nome_doc'] as $value) {

	// 						$string .= "'{$value}'";
	// 					}

	// 					$where .= ($where != "" ? " AND doc.tipo_doc_idtipo_doc = {$string}" : " doc.tipo_doc_idtipo_doc = {$string}");
	// 				}
	// 			}
	// 			$this->logger->debug($where);
	// 			if ($filter['categoria'] != "") {

	// 				if (count($filter['categoria']) > 1) {
	// 					$string = "";
	// 					foreach ($filter['categoria'] as $value) {

	// 						$string .= "'{$value}'" . ',';
	// 					}

	// 					$string = implode(",", array_filter(explode(",", $string)));
	// 					$where .= ($where != "" ? " AND cc.id_cnae IN ({$string})" : " cc.id_cnae IN ({$string})");
	// 				} else {

	// 					$string = "";
	// 					foreach ($filter['categoria'] as $value) {

	// 						$string .= "'{$value}'";
	// 					}

	// 					$where .= ($where != "" ? " AND cc.id_cnae = {$string}" : " cc.id_cnae = {$string}");
	// 				}
	// 			}
	// 			$this->logger->debug($where);
	// 			if ($filter['localidade'] != "") {
	// 				if (count($filter['localidade']) > 1) {
	// 					$string = "";
	// 					foreach ($filter['localidade'] as $value) {
	// 						$column = strstr($value, "+", true);
	// 						$content = substr(strstr($value, "+"), 1);

	// 						$string .= "'{$content}'" . ',';
	// 					}
	// 					$string = implode(",", array_filter(explode(",", $string)));
	// 					$where .= ($where != "" ? " AND loc.{$column} IN ({$string})" : " loc.{$column} IN ({$string})");
	// 				} else {
	// 					$string = "";
	// 					foreach ($filter['localidade'] as $value) {
	// 						$column = strstr($value, "+", true);
	// 						$content = substr(strstr($value, "+"), 1);

	// 						$string .= "'{$content}'";
	// 					}

	// 					$where .= ($where != "" ? " AND loc.{$column} = {$string}" : " loc.{$column} = {$string}");
	// 				}
	// 			}
	// 			$this->logger->debug($where);

	// 			//Grupo, Matriz e unidade
	// 			$sqlGrupo = "SELECT
	// 							id_empresa
	// 						FROM cliente_matriz
	// 						WHERE cliente_grupo_id_grupo_economico = '{$filter['grupo']}'
	// 				";
	// 			$resultGrupo = mysqli_query($this->db, $sqlGrupo);

	// 			if ($filter['grupo'] != "" && $filter['matriz'] == "" && $filter['unidade'] == "") {
	// 				//Apenas Grupo

	// 				$listaUnidades = [];
	// 				while ($obj = $resultGrupo->fetch_object()) {
	// 					$sqlUnidades = "SELECT
	// 									id_unidade,
	// 									localizacao_id_localizacao
	// 								FROM cliente_unidades
	// 								WHERE cliente_matriz_id_empresa = '{$obj->id_empresa}'
	// 					";

	// 					$resultUnidades = mysqli_query($this->db, $sqlUnidades);

	// 					while ($objUn = $resultUnidades->fetch_object()) {
	// 						array_push($listaUnidades, $objUn);
	// 					}
	// 				}

	// 				$this->logger->debug($listaUnidades);

	// 				if (!empty($listaUnidades)) {
	// 					$content = "";
	// 					foreach ($listaUnidades as $value) {
	// 						$content .= "'{$value->id_unidade}'" . ',';
	// 					}
	// 					$content = implode(",", array_filter(explode(",", $content)));
	// 					$where .= ($where != "" ? " AND cu.id_unidade IN ({$content})" : " cu.id_unidade IN ({$content})");
	// 				}
	// 			} elseif ($filter['grupo'] != "" && $filter['matriz'] != "" && $filter['unidade'] == "") {
	// 				//Grupo e Matriz
	// 				$matriz = $filter['matriz'];
	// 				for ($i = 0; $i < count($matriz); $i++) {
	// 					$sqlUnidades = "SELECT
	// 									id_unidade,
	// 									localizacao_id_localizacao
	// 								FROM cliente_unidades
	// 								WHERE cliente_matriz_id_empresa = '{$matriz[$i]}'
	// 					";
	// 					$resultUnidades = mysqli_query($this->db, $sqlUnidades);

	// 					$listaUnidades = [];
	// 					while ($objUn = $resultUnidades->fetch_object()) {
	// 						array_push($listaUnidades, $objUn);
	// 					}

	// 					if (!empty($listaUnidades)) {
	// 						$content = "";
	// 						foreach ($listaUnidades as $value) {
	// 							$content .= "'{$value->id_unidade}'" . ',';
	// 						}
	// 						$content = implode(",", array_filter(explode(",", $content)));
	// 						$where .= ($where != "" ? " AND cu.id_unidade IN ({$content})" : " cu.id_unidade IN ({$content})");
	// 					}
	// 				}

	// 				$this->logger->debug($where);
	// 			} elseif ($filter['grupo'] != "" && $filter['matriz'] != "" && $filter['unidade'] != "") {
	// 				//Todos os campos
	// 				$unidade = $filter['unidade'];
	// 				$this->logger->debug($unidade);
	// 				if (!empty($unidade)) {
	// 					$content = "";
	// 					foreach ($unidade as $value) {
	// 						$content .= "'{$value}'" . ',';
	// 					}
	// 					$content = implode(",", array_filter(explode(",", $content)));

	// 					if (count($unidade) > 1) {
	// 						$where .= ($where != "" ? " AND cu.id_unidade IN ({$content})" : " cu.id_unidade IN ({$content})");
	// 					} else {
	// 						$where .= ($where != "" ? " AND cu.id_unidade = {$content}" : " cu.id_unidade = {$content}");
	// 					}
	// 				}
	// 			}

	// 			if ($filter['sindicato_laboral'] != "") {
	// 				if (count($filter['sindicato_laboral']) > 1) {
	// 					$string = "";
	// 					foreach ($filter['sindicato_laboral'] as $value) {
	// 						$string .= "'{$value}'" . ',';
	// 					}
	// 					$string = implode(",", array_filter(explode(",", $string)));
	// 					$where .= ($where != "" ? " AND doc_emp.sind_emp_id_sinde IN ({$string})" : " doc_emp.sind_emp_id_sinde IN ({$string})");
	// 				} else {
	// 					$string = "";
	// 					foreach ($filter['sindicato_laboral'] as $value) {
	// 						$string .= "'{$value}'";
	// 					}

	// 					$where .= ($where != "" ? " AND doc_emp.sind_emp_id_sinde = {$string}" : " doc_emp.sind_emp_id_sinde = {$string}");
	// 				}
	// 			}


	// 			if ($filter['sindicato_patronal'] != "") {
	// 				if (count($filter['sindicato_patronal']) > 1) {
	// 					$string = "";
	// 					foreach ($filter['sindicato_patronal'] as $value) {
	// 						$string .= "'{$value}'" . ',';
	// 					}
	// 					$string = implode(",", array_filter(explode(",", $string)));
	// 					$where .= ($where != "" ? " AND doc_patr.sind_patr_id_sindp IN ({$string})" : " doc_patr.sind_patr_id_sindp IN ({$string})");
	// 				} else {
	// 					$string = "";
	// 					foreach ($filter['sindicato_patronal'] as $value) {
	// 						$string .= "'{$value}'";
	// 					}

	// 					$where .= ($where != "" ? " AND doc_patr.sind_patr_id_sindp = {$string}" : " doc_patr.sind_patr_id_sindp = {$string}");
	// 				}
	// 			}


	// 			$this->logger->debug($where);
	// 			if ($filter['data_base'] != "") {

	// 				if (count($filter['data_base']) > 1) {
	// 					$string = "";
	// 					foreach ($filter['data_base'] as $value) {

	// 						$string .= "'{$value}'" . ',';
	// 					}

	// 					$string = implode(",", array_filter(explode(",", $string)));
	// 					$where .= ($where != "" ? " AND doc.database_doc IN ({$string})" : " doc.database_doc IN ({$string})");
	// 				} else {

	// 					$string = "";
	// 					foreach ($filter['data_base'] as $value) {

	// 						$string .= "'{$value}'";
	// 					}

	// 					$where .= ($where != "" ? " AND doc.database_doc = {$string}" : " doc.database_doc = {$string}");
	// 				}
	// 			}
	// 			$this->logger->debug($where);

	// 			if ($filter['grupo_clausula'] != "") {

	// 				if (count($filter['grupo_clausula']) > 1) {
	// 					$string = "";
	// 					foreach ($filter['grupo_clausula'] as $value) {

	// 						$string .= "'{$value}'" . ',';
	// 					}

	// 					$string = implode(",", array_filter(explode(",", $string)));
	// 					$where .= ($where != "" ? " AND gc.idgrupo_clausula IN ({$string})" : " gc.idgrupo_clausula IN ({$string})");
	// 				} else {

	// 					$string = "";
	// 					foreach ($filter['grupo_clausula'] as $value) {

	// 						$string .= "'{$value}'";
	// 					}

	// 					$where .= ($where != "" ? " AND gc.idgrupo_clausula = {$string}" : " gc.idgrupo_clausula = {$string}");
	// 				}
	// 			}
	// 			$this->logger->debug($where);

	// 			if ($filter['lista_clausula'] != "") {

	// 				if (count($filter['lista_clausula']) > 1) {
	// 					$string = "";
	// 					foreach ($filter['lista_clausula'] as $value) {

	// 						$string .= "'{$value}'" . ',';
	// 					}

	// 					$string = implode(",", array_filter(explode(",", $string)));
	// 					$where .= ($where != "" ? " AND cgec.estrutura_clausula_id_estruturaclausula IN ({$string})" : " cgec.estrutura_clausula_id_estruturaclausula IN ({$string})");
	// 				} else {

	// 					$string = "";
	// 					foreach ($filter['lista_clausula'] as $value) {

	// 						$string .= "'{$value}'";
	// 					}

	// 					$where .= ($where != "" ? " AND cgec.estrutura_clausula_id_estruturaclausula = {$string}" : " cgec.estrutura_clausula_id_estruturaclausula = {$string}");
	// 				}
	// 			}

	// 			$vigencia = $data['vigencia'];


	// 			if ($data['vigencia']) {
	// 				//vigencia
	// 				$vigenIniDate = strstr($vigencia, ' -', true);
	// 				$vigenIniDate = date("Y-m-d", strtotime(implode("-", explode("/", $vigenIniDate))));

	// 				$separator = mb_strpos($vigencia, "-");
	// 				$vigenEndDate = trim(substr($vigencia, $separator + 1));
	// 				$vigenFinalDate = date("Y-m-d", strtotime(implode("-", explode("/", $vigenEndDate))));

	// 				$where .= ($where != "" ? " AND doc.validade_inicial >= '{$vigenIniDate}' AND doc.validade_final <= '{$vigenFinalDate}'" : " doc.validade_inicial >= '{$vigenIniDate}' AND doc.validade_final <= '{$vigenFinalDate}'");
	// 			}

	// 			$whereInicio = "";
	// 			if ($where == "") {
	// 				$iniDate = (new DateTimeImmutable('now'))->sub(new DateInterval("P1Y"))->format("Y-m-d");
	// 				$finalDate = (new DateTime('now'))->format('Y-m-d');

	// 				// $whereInicio = " main.data_aprovacao >= '{$iniDate}' AND main.data_aprovacao <= '{$finalDate}'";
	// 				// $whereInicio = " loc.uf = 'SP'";
	// 			}

	// 			$wherSql = $where == "" ? "" : "WHERE";
	// 			/** */
	// 			$sql = "SELECT distinct main.id_clau,
	// 						gc.nome_grupo,
	// 						ec.nome_clausula,
	// 						sp.sigla_sp,
	// 						se.sigla_sinde,
	// 						sp.denominacao_sp,
	// 						se.denominacao_sinde,
	// 						DATE_FORMAT(doc.data_reg_mte,'%d/%m/%Y') as data_registro,
	// 						DATE_FORMAT(doc.validade_final,'%d/%m/%Y') as validade,
	// 						main.aprovado,
	// 						doc_patr.sind_patr_id_sindp
	// 					FROM clausula_geral AS main 
	// 					left join clausula_geral_estrutura_clausula as cgec on main.id_clau = cgec.clausula_geral_id_clau 
	// 					left join estrutura_clausula as ec on ec.id_estruturaclausula = cgec.estrutura_clausula_id_estruturaclausula 
	// 					left join grupo_clausula as gc on gc.idgrupo_clausula = ec.grupo_clausula_idgrupo_clausula 
	// 					left join doc_sind as doc on doc.id_doc = cgec.doc_sind_id_doc 
	// 					left join doc_sind_sind_patr as doc_patr on doc_patr.doc_sind_id_doc = doc.id_doc 
	// 					left join doc_sind_sind_emp as doc_emp on doc_emp.doc_sind_id_doc = doc.id_doc 
	// 					left join sind_emp as se on se.id_sinde = doc_emp.sind_emp_id_sinde 
	// 					left join sind_patr as sp on sp.id_sindp = doc_patr.sind_patr_id_sindp 
	// 					left join doc_sind_cliente_unidades as dscu on dscu.doc_sind_id_doc = doc.id_doc 
	// 					left join classe_cnae_doc_sind as ccds on ccds.doc_sind_id_doc = doc.id_doc 
	// 					left join base_territorialsindemp as bemp on bemp.sind_empregados_id_sinde1 = se.id_sinde 
	// 					left join classe_cnae as cc on cc.id_cnae = ccds.classe_cnae_id_cnae 
	// 					left join cnae_emp as cne on cne.classe_cnae_idclasse_cnae = cc.id_cnae 
	// 					left join cliente_unidades as cu on (cu.localizacao_id_localizacao = bemp.localizacao_id_localizacao1 and cu.id_unidade = cne.cliente_unidades_id_unidade) 
	// 					left join abrang_docsind as abr on abr.doc_sind_id_documento = doc.id_doc
	// 					left join localizacao as loc on abr.localizacao_id_localizacao = loc.id_localizacao

	// 					{$wherSql} {$where} 
	// 					";

	// 			$this->logger->debug($sql);
	// 			$response['response_data']['sql'] 	= $sql;
	// 			if ($resultsql = mysqli_query($this->db, $sql)) {

	// 				$html = null;

	// 				$ids_cla = [];

	// 				$table = [];

	// 				while ($obj = $resultsql->fetch_object()) {
	// 					array_push($ids_cla, $obj->id_clau);

	// 					$list = new stdClass;
	// 					$list->input = '<input type="checkbox" onclick="selectIds( ' . $obj->id_clau . ');" id="check' . $obj->id_clau . '">';
	// 					$list->nome_grupo = $obj->nome_grupo;
	// 					$list->nome_clausula = $obj->nome_clausula;
	// 					$list->denominacao_sinde = $obj->denominacao_sinde;
	// 					$list->denominacao_sp = $obj->denominacao_sp;
	// 					$list->validade = $obj->validade;
	// 					array_push($table, $list);
	// 				}

	// 				$response['response_data']['sql'] 	= $sql;

	// 				$response['response_data']['ids_cla'] 	= $ids_cla;

	// 				$response['response_data']['list'] 	= $table;
	// 				$this->logger->debug($table);
	// 			} else {
	// 				$this->logger->debug($sql);
	// 				$this->logger->debug($this->db->error);

	// 				$response['response_status']['status']       = 0;
	// 				$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 				$response['response_status']['msg']          = '';
	// 			}
	// 		} else {
	// 			$response = $this->response;
	// 		}
	// 	} else {
	// 		$response = $this->response;
	// 	}

	// 	$this->logger->debug($response['response_status']['status']);

	// 	return $response;
	// }




	function getDadosCarrea($data = null)
	{


		$this->logger->debug("encapsula: ");



		$grupoecon = $data['gec'];
		$iduser = $data['iduser'];
		$tipo = $data['tipo'];

		$this->logger->debug($grupoecon);

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
				$filtro = "";

				if ($data['filtro']) {
					$filtro = $data['filtro'];
				}

				$filtro_old = "";

				if ($data['filtro_old']) {
					$filtro_old = $data['filtro_old'];
				}


				$response['response_data']['grupoecon'] 	= $grupoecon;





				$sql = "
				SELECT 
							cu.id_unidade
							,cu.nome_unidade
							,cm.nome_empresa
						FROM 
							cliente_unidades as cu INNER JOIN cliente_matriz as cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							 WHERE cu.cliente_matriz_id_empresa IN (SELECT id_empresa FROM 
							cliente_matriz WHERE cliente_grupo_id_grupo_economico = {$grupoecon} );							
				";

				$this->logger->debug("OWWWW");
				$this->logger->debug($sql);

				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value="null"></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->id_unidade;
						$grupos .= '">';
						$grupos .= $obj->nome_empresa;
						$grupos .= ' - ';
						$grupos .= $obj->nome_unidade;
						$grupos .= '</option>';
					}
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}





				$sql = "SELECT distinct doc.id_doc,doc.caminho_arquivo as caminho, DATE_FORMAT(doc.data_upload, '%d/%m/%Y %H:%i:%s') as registro,
				REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_laboral, '$[*].sigla') SEPARATOR ';'), '\"', ''), '[', ''), ']', '') as siglae,
                REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_patronal, '$[*].sigla') SEPARATOR ';'), '\"', ''), '[', ''), ']', '') as siglap,
                REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_laboral, '$[*].municipio') SEPARATOR ';'), '\"', ''), '[', ''), ']', '') as municipio_sinde,
                td.nome_doc,
                DATE_FORMAT(doc.data_aprovacao,'%d/%m/%Y') as data_aprovado
                from 
                doc_sind as doc LEFT JOIN
				tipo_doc as td ON td.idtipo_doc = doc.tipo_doc_idtipo_doc 
				WHERE doc.data_aprovacao IS NOT NULL AND (isnull(doc.cliente_estabelecimento) OR
				 JSON_CONTAINS(doc.cliente_estabelecimento, '{\"g\":". ($grupoecon == "cm.cliente_grupo_id_grupo_economico" ? "0" : $grupoecon) ."}') OR
				  '".$grupoecon."' = 'cm.cliente_grupo_id_grupo_economico') AND
				   td.nome_doc IS NOT NULL and doc.data_aprovacao >= DATE_SUB(CURDATE(), INTERVAL 30 DAY) AND 
				   IF('".$tipo."' like 'Ineditta', 1 = 1, JSON_OVERLAPS((select ids_fmge from usuario_adm WHERE id_user = ".$iduser."),(JSON_EXTRACT(doc.cliente_estabelecimento, '$[*].u' )))) 
						" . $filtro .  " and doc.modulo = 'SISAP' 
						group by doc.id_doc ORDER BY doc.data_aprovacao desc 
						";

				$this->logger->debug($sql);
				$response['response_data']['sql_enc'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;

					while ($obj = $resultsql->fetch_object()) {


						$html .= '<li><a href="' . $obj->caminho . '" target="_blank"  >';
						$html .= $obj->nome_doc;
						$html .= ', ';
						$html .= $obj->municipio_sinde;
						$html .= ', ';
						$html .= $obj->siglae;
						$html .= ' x ';
						$html .= $obj->siglap;
						$html .= ', Encerrado em: ';
						$html .= $obj->data_aprovado;
						$html .= '</a></li>';
					}

					$response['response_data']['encerradas'] 	= $html;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}






				$sql = "SELECT distinct doc.id_doc,doc.caminho_arquivo as caminho,
				 DATE_FORMAT(doc.data_upload, '%d/%m/%Y %H:%i:%s') as registro,
				 DATE_FORMAT(cla.data_aprovacao,'%d/%m/%Y %H:%i:%s') as data_att, 
				REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_laboral, '$[*].sigla') SEPARATOR ';'), '\"', ''), '[', ''), ']', '') as siglae,
                REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_patronal, '$[*].sigla') SEPARATOR ';'), '\"', ''), '[', ''), ']', '') as siglap,
                REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_laboral, '$[*].municipio') SEPARATOR ';'), '\"', ''), '[', ''), ']', '') as municipio_sinde,
                td.nome_doc,
                DATE_FORMAT(doc.data_aprovacao,'%d/%m/%Y') as data_aprovado
                from 
                doc_sind as doc LEFT JOIN
				tipo_doc as td ON td.idtipo_doc = doc.tipo_doc_idtipo_doc LEFT JOIN
				clausula_geral as cla ON cla.doc_sind_id_documento = doc.id_doc
				WHERE doc.id_doc IN (SELECT doc_sind_id_documento FROM clausula_geral as c group by doc_sind_id_documento HAVING  SUM(CASE 
             WHEN c.aprovado = 'nao' THEN 1
             ELSE 0
           END) = 0) AND (isnull(doc.cliente_estabelecimento) OR
				 JSON_CONTAINS(doc.cliente_estabelecimento, '{\"g\":". ($grupoecon == "cm.cliente_grupo_id_grupo_economico" ? "0" : $grupoecon) ."}') OR
				  '".$grupoecon."' = 'cm.cliente_grupo_id_grupo_economico') AND
				   td.nome_doc IS NOT NULL and cla.data_aprovacao >= DATE_SUB(CURDATE(), INTERVAL 30 DAY) AND IF('".$tipo."' like 'Ineditta', 1 = 1, JSON_OVERLAPS(
						(select ids_fmge from usuario_adm WHERE id_user = ".$iduser."),
						(JSON_EXTRACT(doc.cliente_estabelecimento, '$[*].u' ))))
						" . $filtro .  " group by doc.id_doc ORDER BY doc.data_aprovacao desc 
						
						";

				$this->logger->debug($sql);
				$response['response_data']['sql_enc'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;

					while ($obj = $resultsql->fetch_object()) {


						$html .= '<li><a href="http://localhost:8080/consultaclausula.php?iddoc=' . $obj->id_doc . '"  >';
						$html .= $obj->nome_doc;
						$html .= ', ';
						$html .= $obj->municipio_sinde;
						$html .= ', ';
						$html .= $obj->siglae;
						$html .= ' x ';
						$html .= $obj->siglap;
						$html .= ', Atualizado em: ';
						$html .= substr($obj->data_att, 0, -9);
						$html .= '</a></li>';
					}

					$response['response_data']['atualizadas'] 	= $html;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}





				$sql = "
				SELECT count(distinct cm.id_empresa) as EMPRESAS,
				COUNT(distinct cu.id_unidade) as UNIDADES,
				COUNT(distinct ce.classe_cnae_idclasse_cnae) as SEGMENTOS,
				COUNT(distinct se.id_sinde) as LABORAIS,
				COUNT(distinct sp.id_sindp) as PATRONAIS
				from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp
				WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon}
				 AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old;

				$this->logger->debug($sql);
				$response['response_data']['sql'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;

					while ($obj = $resultsql->fetch_object()) {
						$html .= 'Resultados: ';
						$html .= $obj->EMPRESAS;
						$html .= ' Empresas, ';
						$html .= $obj->SEGMENTOS;
						$html .= ' Segmentos, ';
						$html .= $obj->UNIDADES;
						$html .= ' Unidades, ';
						$html .= $obj->LABORAIS;
						$html .= ' Sindicatos Laborais, ';
						$html .= $obj->PATRONAIS;
						$html .= ' Sindicatos Patronais';

						$response['response_data']['numemp'] 	= $obj->EMPRESAS;
						$response['response_data']['numseg'] 	= $obj->SEGMENTOS;
						$response['response_data']['numuni'] 	= $obj->UNIDADES;
						$response['response_data']['numsinde'] 	= $obj->LABORAIS;
						$response['response_data']['numsindp'] 	= $obj->PATRONAIS;
					}

					$response['response_data']['sql'] 	= $sql;

					$response['response_data']['html'] 	= $html;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}











				$sql = "






				select fase.*, (select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
 cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
 cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
 base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
 AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
 sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
 base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
 AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
 sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
 acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp)
 WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND cct.idacompanhanto_cct is
 NOT NULL AND cct.fase like fase.fase_negociacao AND IF('Ineditta' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as contagem, (select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
						cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
						cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
						base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
						AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
						sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
						base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
						AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
						sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
						acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
						WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND cct.idacompanhanto_cct is
						NOT NULL AND IF('Ineditta' like 'Ineditta', 1 = 1, JSON_CONTAINS(
											   (select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
											   CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as todas from fase_cct as fase  where fase.tipo_fase like 'cct';
				";
				$this->logger->debug("AQUIIIIIIIIIIIIIII333333");
				$this->logger->debug($sql);
				$response['response_data']['sql'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$count = 0;

					while ($obj = $resultsql->fetch_object()) {
						if ($count == 0) {
							$html .= "<tr style='background-color:#4E5754'><th style='color: white'>TOTAL</th>";
							$html .= "<td style='color: white'>";
							$html .= $obj->todas;
							$html .= "</td></tr>";
						}

						$html .= "<tr><th>{$obj->fase_negociacao}</th>";
						$html .= "<td>";
						$html .= $obj->contagem;
						$html .= "</td></tr>";



						$count++;
					}

					$response['response_data']['tf'] 	= $html;

					$response['response_data']['sql'] 	= $sql;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}














				$sql = "select distinct  IFNULL(year(doc.data_aprovacao), '0Ano nÃo informado') as ANO ,count(td.idtipo_doc) as total,
				count(CASE WHEN td.nome_doc like '%Acordo Coletivo' THEN td.idtipo_doc ELSE NULL END) as acordo_coletivo,
                count(CASE WHEN td.nome_doc like '%Acordo Coletivo Específico%' THEN td.idtipo_doc ELSE NULL END) as acordo_coletivo_esp,
				count(CASE WHEN td.nome_doc like '%Convenção Coletiva' THEN td.idtipo_doc ELSE NULL END) as convencao_coletiva,
				count(CASE WHEN td.nome_doc like '%Convenção Coletiva Específica%' THEN td.idtipo_doc ELSE NULL END) as convencao_coletiva_esp from
				doc_sind as doc JOIN
				tipo_doc as td ON td.idtipo_doc = doc.tipo_doc_idtipo_doc 
			   WHERE NOT isnull(doc.id_doc) and (year(doc.data_aprovacao) >= (YEAR(curdate()) - 1) OR ISNULL(year(doc.data_aprovacao))) GROUP BY ANO ORDER BY ANO desc;
";
				$this->logger->debug("AQUI ESTASSSSSS");
				$this->logger->debug($sql);
				$response['response_data']['sql_enc2'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = '';
					$html1 = '<td>Acordos Coletivos</td>';
					$html2 = '<td>Convenções Coletivas</td>';
					$html3 = '<td>Total</td>';
					$html4 = '<td>Acordos Coletivos Específicos</td>';
					$html5 = '<td>Convenções Coletivas Específicas</td>';
					$count = 0;

					$prop_acordo = [];
					$prop_convencao = [];
					$prop_acordo_esp = [];
					$prop_convencao_esp = [];

					while ($obj = $resultsql->fetch_object()) {

						$count += 1;
						if ($count <= 2) {
							$html .= '<th style="text-align: right;">';

							$html .= $obj->ANO;
							$html .= '</th>';

							$html1 .= '<td style="text-align: right;">';
							$html1 .= $obj->acordo_coletivo;
							$html1 .= '</td>';

							array_push($prop_acordo, number_format((100 * floatval($obj->acordo_coletivo)) / (floatval($obj->total)), 2, ',', '.'));

							$html2 .= '<td style="text-align: right;">';
							$html2 .= $obj->convencao_coletiva;
							$html2 .= '</td>';

							array_push($prop_convencao, number_format((100 * floatval($obj->convencao_coletiva)) / (floatval($obj->total)), 2, ',', '.'));


							$html4 .= '<td style="text-align: right;">';
							$html4 .= $obj->acordo_coletivo_esp;
							$html4 .= '</td>';

							array_push($prop_acordo_esp, number_format((100 * floatval($obj->acordo_coletivo_esp)) / (floatval($obj->total)), 2, ',', '.'));

							$html5 .= '<td style="text-align: right;">';
							$html5 .= $obj->convencao_coletiva_esp;
							$html5 .= '</td>';

							array_push($prop_convencao_esp, number_format((100 * floatval($obj->convencao_coletiva_esp)) / (floatval($obj->total)), 2, ',', '.'));


							$html3 .= '<td style="text-align: right;">';
							$html3 .= $obj->total;
							$html3 .= '</td>';
						}
					}

					// while ($count < 2) {
					// 	$count += 1;

					// 	$html1 .= '<td style="text-align: right;">';
					// 	$html1 .= '0';
					// 	$html1 .= '</td>';

					// 	array_push($prop_acordo, "0");

					// 	$html2 .= '<td style="text-align: right;">';
					// 	$html2 .= '0';
					// 	$html2 .= '</td>';

					// 	array_push($prop_convencao, "0");

					// 	$html3 .= '<td style="text-align: right;">';
					// 	$html3 .= '0';
					// 	$html3 .= '</td>';

					// 	array_push($prop_acordo_esp, "0");

					// 	$html4 .= '<td style="text-align: right;">';
					// 	$html4 .= '0';
					// 	$html4 .= '</td>';

					// 	array_push($prop_convencao_esp, "0");

					// 	$html5 .= '<td style="text-align: right;">';
					// 	$html5 .= '0';
					// 	$html5 .= '</td>';
					// }

					for ($i = 0; $i < ($count - 1); $i++) {
						$html3 .= '<td style="text-align: right;">';

						$html3 .= '100,00%';
						$html3 .= '</td>';

						$html2 .= '<td style="text-align: right;">';
						$html2 .= $prop_convencao[$i];
						$html2 .= '%';
						$html2 .= '</td>';

						$html1 .= '<td style="text-align: right;">';
						$html1 .= $prop_acordo[$i];
						$html1 .= '%';
						$html1 .= '</td>';

						$html5 .= '<td style="text-align: right;">';
						$html5 .= $prop_convencao_esp[$i];
						$html5 .= '%';
						$html5 .= '</td>';

						$html4 .= '<td style="text-align: right;">';
						$html4 .= $prop_acordo_esp[$i];
						$html4 .= '%';
						$html4 .= '</td>';
					}

					$response['response_data']['anosneg'] 	= $html;
					$response['response_data']['acordoscoletivos'] 	= $html1;
					$response['response_data']['convencoescoletivas'] 	= $html2;
					$response['response_data']['acordoscoletivosesp'] 	= $html4;
					$response['response_data']['convencoescoletivasesp'] 	= $html5;
					$response['response_data']['totalneg'] 	= $html3;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}













				$sql = "SELECT distinct DATE_FORMAT(max(doc.data_aprovacao),'%d/%m/%Y') as data_att
                from 
                doc_sind as doc LEFT JOIN
				tipo_doc as td ON td.idtipo_doc = doc.tipo_doc_idtipo_doc 
				WHERE doc.data_aprovacao IS NOT NULL AND (isnull(doc.cliente_estabelecimento) OR
				 JSON_CONTAINS(doc.cliente_estabelecimento, '{\"g\":". ($grupoecon == "cm.cliente_grupo_id_grupo_economico" ? "0" : $grupoecon) ."}') OR
				  '".$grupoecon."' = 'cm.cliente_grupo_id_grupo_economico') AND
				   td.nome_doc IS NOT NULL AND (td.nome_doc like '%Acordo Coletivo%'
				OR td.nome_doc like '%Convenção Coletiva%') AND IF('".$tipo."' like 'Ineditta', 1 = 1, JSON_OVERLAPS(
						(select ids_fmge from usuario_adm WHERE id_user = ".$iduser."),
						(JSON_EXTRACT(doc.cliente_estabelecimento, '$[*].u' ))))
						" . $filtro .  " group by doc.id_doc ORDER BY doc.data_aprovacao desc LIMIT 1
						
				";

				$this->logger->debug($sql);
				$response['response_data']['sql'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;

					while ($obj = $resultsql->fetch_object()) {

						$response['response_data']['ultimaatt'] 	= $obj->data_att;
					}
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}








				$sql = "select periodo_data,DATE_FORMAT(periodo_data, '%m/%Y') as periodo_data_for,
				dado_real from indecon_real WHERE indicador like '%INPC%'
	 order by periodo_data desc;";

				$this->logger->debug($sql);
				$response['response_data']['sql'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$count = 0;
					$tax = 1;

					while ($obj = $resultsql->fetch_object()) {
						if ($count == 0) {
							$html .= "Último mês: "; // Último dado
							// $html .= number_format((floatval($obj->dado_real)), 3, ',', '.');
							// $html .= "% ";
							$html .= $obj->periodo_data_for;
							$taxes = number_format((floatval($obj->dado_real)), 3, ',', '.');
						}



						if ($count <= 10) {
							$tax = $tax * (1 + (floatval($obj->dado_real / 100)));
						}
						$count += 1;
					}

					//$tax = $tax/{$grupoecon};



					$response['response_data']['lastinpc'] 	= $html;
					$response['response_data']['calcinpc'] 	= $taxes . "%";
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				$sql = "select periodo_data,DATE_FORMAT(periodo_data, '%m/%Y') as periodo_data_for,
                            dado_real from indecon_real WHERE indicador like '%IPCA%'
				 order by periodo_data DESC";

				$this->logger->debug($sql);
				$response['response_data']['sql'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$count = 0;
					$tax = 1;

					while ($obj = $resultsql->fetch_object()) {
						if ($count == 0) {
							$html .= "Último mês: "; // Último dado
							// $html .= number_format((floatval($obj->dado_real)), 3, ',', '.');
							// $html .= "% ";
							$html .= $obj->periodo_data_for;
							$taxes =  number_format((floatval($obj->dado_real)), 3, ',', '.');
						}



						if ($count < 11) {
							$tax = $tax * (1 + (floatval($obj->dado_real / 100)));
						}
						$count += 1;
					}

					//$tax = $tax/{$grupoecon};



					$response['response_data']['lastipca'] 	= $html;
					$response['response_data']['calcipca'] 	= $taxes . "%";
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}












				$sql = "SELECT 
                            DISTINCT IFNULL(periodo_data, '00/00/0000') AS periodo,
                            IFNULL(FORMAT(dado_real, 3), 0) AS ipca_real 
                        FROM indecon_real WHERE periodo_data BETWEEN  DATE_SUB(DATE_FORMAT(NOW(), '%Y-%m-01'), INTERVAL {$data['nm']} MONTH) AND DATE_FORMAT(NOW(), '%Y-%m-01') AND indicador LIKE '%IPCA%'
                        group by periodo_data ORDER BY periodo_data
                ";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$ipcaatual = [];

					while ($obj = $resultsql->fetch_object()) {
						array_push($ipcaatual, $obj->ipca_real);
					}

					//$tax = $tax/{$grupoecon};



					$response['response_data']['ipca_real'] 	= $ipcaatual;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}






				$sql = "SELECT 
                            IFNULL(cm.data, '00/00/0000') as periodo,
                            IFNULL(FORMAT(cm.dado_projetado, 3), 0) as ipca_projetado, 
                            fonte 
                        FROM indecon AS cm
                        WHERE data BETWEEN DATE_SUB(DATE_FORMAT(NOW(), '%Y-%m-01'), INTERVAL {$data['nm']} MONTH) AND DATE_ADD(DATE_FORMAT(NOW(), '%Y-%m-01'), INTERVAL {$data['nm']} MONTH)
				        and  cm.indicador like '%IPCA%'  and (IF(cm.origem like '%Cliente%' and cm.cliente_grupo_id_grupo_economico = {$grupoecon}, cm.origem like '%Cliente%' and cm.cliente_grupo_id_grupo_economico = {$grupoecon},cm.origem like '%Ineditta%')) 
                        group by cm.data ORDER BY cm.data
                ";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$ipcaatual = [];
					$fonts = [];
					$ac = 0;
					$mesinicio = "";

					while ($obj = $resultsql->fetch_object()) {
						if ($ac == 0) {

							$mesinicio = $obj->periodo;
						}
						array_push($ipcaatual, $obj->ipca_projetado);
						array_push($fonts, $obj->fonte);
					}

					//$tax = $tax/{$grupoecon};



					$response['response_data']['ipca_projetado'] 	= $ipcaatual;
					$response['response_data']['fonte_ipca_projetado'] 	= $fonts;
					$response['response_data']['mesinicio'] 	= $mesinicio;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}











				$sql = "SELECT 
                            DISTINCT IFNULL(re.periodo_data, '00/00/0000') AS periodo,
                            IFNULL(FORMAT(re.dado_real, 3), 0) AS inpc_real 
                        FROM indecon_real AS re 
                        WHERE periodo_data BETWEEN  DATE_SUB(DATE_FORMAT(NOW(), '%Y-%m-01'), INTERVAL {$data['nm']} month) AND DATE_FORMAT(NOW(), '%Y-%m-01') AND re.indicador LIKE '%INPC%' 
                        group by periodo_data ORDER BY periodo_data";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$ipcaatual = [];

					while ($obj = $resultsql->fetch_object()) {
						array_push($ipcaatual, $obj->inpc_real);
					}

					//$tax = $tax/{$grupoecon};



					$response['response_data']['inpc_real'] 	= $ipcaatual;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}






				$sql = "SELECT 
                            IFNULL(cm.data, '00/00/0000') as periodo,
                            IFNULL(FORMAT(cm.dado_projetado, 3), 0) as inpc_projetado, fonte 
                        FROM indecon AS cm  
                        WHERE cm.data between  DATE_SUB(DATE_FORMAT(NOW(), '%Y-%m-01'), INTERVAL {$data['nm']} month) and DATE_ADD(DATE_FORMAT(NOW(), '%Y-%m-01'), INTERVAL {$data['nm']} MONTH) and cm.indicador like '%INPC%'
				            AND cm.indicador like '%INPC%'  AND (IF(cm.origem like '%Cliente%' AND cm.cliente_grupo_id_grupo_economico = {$grupoecon}, cm.origem like '%Cliente%' AND cm.cliente_grupo_id_grupo_economico = {$grupoecon},cm.origem like '%Ineditta%')) 
							group by cm.data ORDER BY cm.data
				 ;";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$ipcaatual = [];
					$fontes = [];
					$ac = 0;
					$mesinicio = "";

					while ($obj = $resultsql->fetch_object()) {
						if ($ac == 0) {

							$mesinicio = $obj->periodo;
						}
						array_push($ipcaatual, $obj->inpc_projetado);
						array_push($fontes, $obj->fonte);
					}

					//$tax = $tax/{$grupoecon};



					$response['response_data']['inpc_projetado'] 	= $ipcaatual;
					$response['response_data']['fonte_inpc_projetado'] 	= $fontes;
					$response['response_data']['mesinicio'] 	= $mesinicio;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}


				$sql = "SELECT 
                            periodo_data,
                            FORMAT(dado_real, 3) as  dado_real 
                        FROM indecon_real 
                        WHERE YEAR(periodo_data) = YEAR(NOW()) - 1 AND indicador LIKE '%IPCA%' 
                        ORDER BY periodo_data
                 ";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$ipcapassado = [];

					while ($obj = $resultsql->fetch_object()) {
						array_push($ipcapassado, $obj->dado_real);
					}

					//$tax = $tax/{$grupoecon};



					$response['response_data']['passadoipca'] 	= $ipcapassado;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}















				$sql = "select (select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND cct.idacompanhanto_cct is NOT NULL) as total,
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'JAN%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as jan,
				 (select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'FEV%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as fev,
				 (select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'MAR%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as mar,
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'ABR%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as abr,
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'MAI%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as mai,
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'JUN%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as jun,
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'JUL%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as jul,
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'AGO%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as ago,
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'SET%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as 'set',
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'OUT%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as 'out',
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'NOV%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as nov,
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                        fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
				cct.idacompanhanto_cct is NOT NULL AND cct.data_base like 'DEZ%' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as dez
				;";

				$this->logger->debug($sql);
				$response['response_data']['sql'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;

					while ($obj = $resultsql->fetch_object()) {

						$response['response_data']['jan'] 	= $obj->jan;
						$response['response_data']['fev'] 	= $obj->fev;
						$response['response_data']['mar'] 	= $obj->mar;
						$response['response_data']['abr'] 	= $obj->abr;
						$response['response_data']['mai'] 	= $obj->mai;
						$response['response_data']['jun'] 	= $obj->jun;
						$response['response_data']['jul'] 	= $obj->jul;
						$response['response_data']['ago'] 	= $obj->ago;
						$response['response_data']['set'] 	= $obj->set;
						$response['response_data']['out'] 	= $obj->out;
						$response['response_data']['nov'] 	= $obj->nov;
						$response['response_data']['dez'] 	= $obj->dez;
					}

					$response['response_data']['sql'] 	= $sql;

					$response['response_data']['html'] 	= $html;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				$sql = null;



				$sql = "select (select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
				WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND cct.idacompanhanto_cct is NOT NULL AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as total,
				(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
				cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
				cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
				base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
				base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
				AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
				sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
				acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
                fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
				WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND cct.idacompanhanto_cct is NOT NULL AND se.uf_sinde like 'SP' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as SP,";

				$ufs = [
					'AC',
					'AL',
					'AP',
					'AM',
					'BA',
					'CE',
					'DF',
					'ES',
					'GO',
					'MA',
					'MS',
					'MT',
					'MG',
					'PA',
					'PB',
					'PR',
					'PE',
					'PI',
					'RJ',
					'RN',
					'RS',
					'RO',
					'RR',
					'SC',
					'SE',
					'TO',
				];

				foreach ($ufs as &$uf) {
					$sql .= "(select count(distinct cct.idacompanhanto_cct) from cliente_matriz as cm LEFT JOIN cliente_unidades as cu ON
					cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
					cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cu.id_unidade AND ce.data_final = '00-00-0000') LEFT JOIN
					base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
					AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
					sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1 LEFT JOIN
					base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
					AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
					sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp LEFT JOIN
					acompanhanto_cct as cct on (cct.sind_emp_id_sinde = se.id_sinde AND cct.sind_patr_id_sindp = sp.id_sindp) INNER JOIN
					fase_cct as fa on (fa.fase_negociacao like cct.fase and fa.tipo_fase = 'cct')
					WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon} AND
					cct.idacompanhanto_cct is NOT NULL AND se.uf_sinde like '{$uf}' AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cu.id_unidade, ''),'$'))" . $filtro_old . ") as '{$uf}',";
				}
				$sql = rtrim($sql, ",");

				$sql .= ";";


				$this->logger->debug($sql);
				$response['response_data']['sql'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;

					while ($obj = $resultsql->fetch_object()) {

						$response['response_data']['SP'] 	= $obj->SP;
						foreach ($ufs as &$uf) {
							$response['response_data'][$uf] 	= $obj->$uf;
						}
					}
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
		mysqli_close($this->db);

		return $response;
	}

	// function getByIdConsultaClausula($data = null)
	// {

	// 	if ($this->response['response_status']['status'] == 1) {

	// 		// Carregando a resposta padrão da função
	// 		$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

	// 		if ($response['response_status']['status'] == 1) {
	// 			if (empty($this->db)) {
	// 				$connectdb = $this->connectdb();

	// 				if ($connectdb['response_status']['status'] == 0) {
	// 					$response = $connectdb;
	// 				}
	// 			}
	// 		}

	// 		$this->logger->debug($connectdb);

	// 		if ($response['response_status']['status'] == 1) {

	// 			$exibicao = "";

	// 			$ids = explode(" ", $data['ids']);


	// 			foreach ($ids as &$idx) {


	// 				$id = $idx;

	// 				if ($idx . '' == "") {
	// 					$id = 0;
	// 				} else {



	// 					//exibicao
	// 					$sql = "SELECT distinct main.id_clau,
	// 								gc.idgrupo_clausula,
	// 								gc.nome_grupo,
	// 								ec.id_estruturaclausula,
	// 								ec.nome_clausula,
	// 								sp.id_sindp,
	// 								sp.sigla_sp,
	// 								se.id_sinde,
	// 								se.sigla_sinde,
	// 								se.uf_sinde,
	// 								sp.denominacao_sp,
	// 								se.denominacao_sinde,
	// 								sp.uf_sp,
	// 								DATE_FORMAT(doc.data_reg_mte,'%d/%m/%Y') as data_registro,
	// 								DATE_FORMAT(doc.validade_inicial,'%d/%m/%Y') as validade_ini,
	// 								DATE_FORMAT(doc.validade_final,'%d/%m/%Y') as validade,
	// 								main.aprovado,
	// 								td.nome_doc,
	// 								cc.descricao_divisão,
	// 								cc.descricao_subclasse,
	// 								se.codigo_sinde,
	// 								sp.codigo_sp,
	// 								bemp.dataneg,
	// 								main.tex_clau,
	// 								concat(cu.nome_unidade,\" | CNPJ: \", cu.cnpj_unidade, \" | Cód Sind. Cliente: \", cu.cod_sindcliente) as unidade,
	// 								info.nmtipoinformacaoadicional AS informacao,
	// 								cgec.sequencia,
	// 								cgec.id_info_tipo_grupo,
	// 								IFNULL(cgec.texto, IFNULL(cgec.numerico, IFNULL(cgec.descricao, IFNULL(date_format(cgec.data, '%d/%m/%Y'), IFNULL(cgec.percentual, IFNULL(cgec.hora, cgec.combo)))))) as valor,
	// 								DATE_FORMAT(main.data_aprovacao,'%d/%m/%Y') as data_pro,
	// 								doc.database_doc
	// 							FROM clausula_geral AS main 
	// 								left join clausula_geral_estrutura_clausula as cgec on main.id_clau = cgec.clausula_geral_id_clau 
	// 								left join estrutura_clausula as ec on ec.id_estruturaclausula = cgec.estrutura_clausula_id_estruturaclausula 
	// 								left join grupo_clausula as gc on gc.idgrupo_clausula = ec.grupo_clausula_idgrupo_clausula 
	// 								left join doc_sind as doc on doc.id_doc = cgec.doc_sind_id_doc 
	// 								left join doc_sind_sind_patr as doc_patr on doc_patr.doc_sind_id_doc = doc.id_doc 
	// 								left join doc_sind_sind_emp as doc_emp on doc_emp.doc_sind_id_doc = doc.id_doc 
	// 								left join sind_emp as se on se.id_sinde = doc_emp.sind_emp_id_sinde 
	// 								left join sind_patr as sp on sp.id_sindp = doc_patr.sind_patr_id_sindp 
	// 								left join doc_sind_cliente_unidades as dscu on dscu.doc_sind_id_doc = doc.id_doc 
	// 								left join classe_cnae_doc_sind as ccds on ccds.doc_sind_id_doc = doc.id_doc 
	// 								left join base_territorialsindemp as bemp on bemp.sind_empregados_id_sinde1 = se.id_sinde 
	// 								left join classe_cnae as cc on cc.id_cnae = ccds.classe_cnae_id_cnae 
	// 								left join cnae_emp as cne on cne.classe_cnae_idclasse_cnae = cc.id_cnae 
	// 								left join cliente_unidades as cu on (cu.localizacao_id_localizacao = bemp.localizacao_id_localizacao1 and cu.id_unidade = cne.cliente_unidades_id_unidade) 
	// 								left join tipo_doc as td on td.idtipo_doc = doc.tipo_doc_idtipo_doc 
	// 								left join ad_tipoinformacaoadicional as info on info.cdtipoinformacaoadicional = cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
	// 								left join informacao_adicional_grupo as ig on ig.ad_tipoinformacaoadicional_id = cgec.id_info_tipo_grupo
	// 							WHERE main.id_clau = {$id};
	// 					";


	// 					$this->logger->debug($sql);
	// 					if ($resultsql = mysqli_query($this->db, $sql)) {
	// 						$obj = $resultsql->fetch_object();

	// 						$html = '
	// 						<div class="panel panel-primary"> 
	// 							<div class="">
	// 								<h4>Sobre a Cláusula: </h4>
									
	// 							</div> 
	// 							<div class="panel-body collapse in">';
	// 						$html .= '
	// 						<div class="row">
	// 							<div class="col-lg-8">
	// 								<table class="table table-striped table-bordered">
	// 									<thead>
	// 										<tr>
	// 											<th>Sindicato Laboral / UF</th>
	// 										</tr>
	// 									</thead>
	// 									<tbody>
	// 										<tr class="odd gradeX">
	// 											<td> ' . $obj->denominacao_sinde . ' / ' . $obj->uf_sinde . ' </td>
	// 										</tr>
	// 									</tbody>
	// 								</table>
								
	// 						';

	// 						$html .= '
	// 						<table class="table table-striped table-bordered">
	// 							<thead>
	// 								<tr>
	// 									<th>Sindicato Patronal / UF</th>
	// 								</tr>
	// 							</thead>
	// 							<tbody>
	// 								<tr class="odd gradeX">
	// 									<td> ' . $obj->denominacao_sp . ' / ' . $obj->uf_sp . ' </td>
	// 								</tr>
	// 							</tbody>
	// 						</table>
							
	// 						';

	// 						$html .= '
	// 						<table class="table table-striped table-bordered">
	// 							<thead>
	// 								<tr>
	// 									<th>Grupo da Cláusula</th>
	// 									<th>Nome da Cláusula</th>
	// 									<th>Documento</th>
	// 									<th>Data Processamento</th>
	// 								</tr>
	// 							</thead>
	// 							<tbody>
	// 								<tr class="odd gradeX">
	// 									<td>' . $obj->nome_grupo . '</td>
	// 									<td>' . $obj->nome_clausula . '</td>
	// 									<td>' . $obj->nome_doc . '</td>
	// 									<td>' . $obj->data_pro . '</td>
	// 								</tr>
	// 							</tbody>
	// 						</table>
							
	// 						';

	// 						$html .= '
	// 						<table class="table table-striped table-bordered">
	// 							<thead>
	// 								<tr>
	// 									<th>Validade Inicial</th>
	// 									<th>Validade Final</th>
	// 									<th>Data Base</th>
	// 									<th>Atividade Econômica</th>
	// 								</tr>
	// 							</thead>
	// 							<tbody>
	// 								<tr class="odd gradeX">
	// 									<td>' . $obj->validade_ini . '</td>
	// 									<td>' . $obj->validade . '</td>
	// 									<td>' . $obj->dataneg . '</td>
	// 									<td>' . $obj->descricao_subclasse . '</td>
	// 								</tr>
	// 							</tbody>
	// 						</table>
	// 						</div>
	// 						';

	// 						$html .= '
	// 						<div class="col-lg-4">
	// 							<div class="row" style="max-height:440px; overflow:scroll;">
	// 								<div class="col-lg-12">
	// 									<table class="table table-striped table-bordered">
	// 										<thead>
	// 											<tr>
	// 												<th>Filiais Abrangidas Pela Cláusula</th>
	// 											</tr>
	// 										</thead>
	// 										<tbody>
	// 											<tr class="odd gradeX">
	// 												<td>' . $obj->unidade . '</td>
	// 											</tr>
	// 						';


	// 						//Trazendo Notificação
	// 						$sqlNote = "SELECT 
	// 										nt.usuario_adm_id_user,
	// 										us.nome_usuario,
	// 										date_format(data_registro, '%d/%m/%Y - %H:%i') as data_registro,
	// 										comentario
	// 									FROM note_cliente nt
	// 									LEFT JOIN usuario_adm as us on us.id_user = nt.usuario_adm_id_user
	// 									WHERE id_tipo_comentario = '{$id}'
	// 						";
	// 						$resultNote = mysqli_query($this->db, $sqlNote);
	// 						$objNote = $resultNote->fetch_object();

	// 						$html5 = '
	// 						<div class="panel panel-primary"> 
	// 							<div class="">
	// 								<h4>Comentários: </h4>
									 
    //                     		</div> 
	// 							<div class="panel-body collapse in hei">
	// 								<div class="row">
	// 									<div class="col-lg-12">
	// 										<table class="table table-striped">
	// 											<thead>
	// 												<th>Usuário</th>
	// 												<th>Data</th>
	// 												<th>Comentário</th>
	// 											</thead>
	// 											<tbody>
	// 												<tr>
	// 													<td>' . (!$objNote ? "" : $objNote->usuario_adm_id_user . " - " . $objNote->nome_usuario) . '</td>
	// 													<td>' . (!$objNote ? "" : $objNote->data_registro) . '</td>
	// 													<td>' . (!$objNote ? "" : $objNote->comentario) . '</td>
	// 												</tr>
	// 											</tbody>
	// 										</table>
	// 									</div>
	// 								</div>
	// 							</div>
	// 						</div>
	// 						';


	// 						$html4 = '
	// 						<div class="panel panel-primary"> 
	// 							<div class="">
	// 								<h4>Informações Adicionais: </h4>
									 
    //                     		</div> 
	// 							<div class="panel-body collapse in hei fixTableHead"> 
	// 								<table class="table table-striped table-info">
	// 									<thead id="cabecalho">
	// 										<tr>
	// 						';

	// 						$last = $obj->informacao;


	// 						$last1 = $obj->unidade;

	// 						$auxc = [];

	// 						$th = '
	// 							<th style="min-width:200px;">' . $obj->informacao . '</th>
	// 						';

	// 						$td = '
	// 								<td style="min-width:200px;">' . $obj->valor . '</td>
							
	// 						';

	// 						$sqlQtd = "SELECT count(ad_tipoinformacaoadicional_id) as qtd FROM informacao_adicional_grupo WHERE ad_tipoinformacaoadicional_id = '{$obj->id_info_tipo_grupo}'
	// 						";
	// 						$resultQtd = mysqli_query($this->db, $sqlQtd);

	// 						$objQtd = $resultQtd->fetch_object();

	// 						$this->logger->debug($objQtd->qtd);

	// 						// $this->logger->debug(  $resultsql->fetch_object() );
	// 						$cont = 1;
	// 						$fimTh = false;
	// 						while ($obj2 = $resultsql->fetch_object()) {
	// 							$this->logger->debug($obj2->valor);
	// 							if ($last != $obj2->informacao && $cont < $objQtd->qtd) {
	// 								if (!$fimTh) {
	// 									$th .= '
	// 										<th style="min-width:200px;" >' . $obj2->informacao . '</th>
	// 									';
	// 								}

	// 								$td .= '<td style="min-width:200px;">' . $obj2->valor . '</td>';

	// 								$this->logger->debug($obj2->valor);
	// 							}
	// 							$cont++;
	// 							$last = $obj2->informacao;



	// 							if ($cont == $objQtd->qtd) {
	// 								$fimTh = true;
	// 								$td .= '</tr>';
	// 								$cont = 0;
	// 							}

	// 							if (!in_array($obj2->unidade, $auxc)) {
	// 								$html .= '<tr>';
	// 								$html .= '<td>';
	// 								$html .= $obj2->unidade;
	// 								$html .= '</tr>';
	// 							}
	// 							array_push($auxc, $obj2->unidade);
	// 						}

	// 						$th .= "</tr>";



	// 						$html4 .= $th . '</thead><tbody>' . $td;

	// 						//BUSCANDO DOCUEMNTOS PARA LEGISLAÇÃO
	// 						$sqlDoc = "SELECT
	// 										iddocumentos,
	// 										dc.descricao_documento,
	// 										tipo_doc_idtipo_doc,
	// 										tp.nome_doc,
	// 										da.estrutura_clausula_id_estruturaclausula
	// 									FROM documentos AS dc
	// 									LEFT JOIN documento_assunto AS da ON da.documentos_iddocumentos = dc.iddocumentos
	// 									LEFT JOIN tipo_doc AS tp ON tp.idtipo_doc = dc.tipo_doc_idtipo_doc

	// 									WHERE da.estrutura_clausula_id_estruturaclausula = {$obj->id_estruturaclausula}
	// 						";

	// 						$this->logger->debug($sqlDoc);

	// 						$resultDoc = mysqli_query($this->db, $sqlDoc);

	// 						$legislacao = "";
	// 						while ($objDoc = $resultDoc->fetch_object()) {
	// 							$legislacao .= "<tr>";
	// 							$legislacao .= "<td><a href='#' title='Visualizar Documento'>{$objDoc->descricao_documento} <i class='fa fa-external-link'></i></a></td>";
	// 							$legislacao .= "</tr>";
	// 						}

	// 						$this->logger->debug($sqlDoc);



	// 						$html .= '
	// 									</tbody>
	// 								</table>
	// 							</div>
	// 						</div>
	// 						<div class="row" style="margin-top:16px;">
	// 							<div class="col-lg-12">
	// 								<table class="table table-striped table-bordered">
	// 									<thead>
	// 										<tr>
	// 											<th>Legislação</th>
	// 										</tr>
	// 									</thead>
	// 									<tbody>
	// 										' . $legislacao . '
	// 									</tbody>
	// 								</table>
	// 							</div>
	// 						</div>
	// 						</div>
	// 						</div>
	// 						</div>
	// 						';

	// 						$html4 .= '</tr></tbody></table></div></div>';

	// 						$html2 = '
	// 							<div class="row">
	// 								<div class="col-sm-12"> 
	// 									<div class="panel panel-primary">
	// 										<div class="">
	// 											<h4>Texto da Cláusula: </h4>
												 
	// 										</div>
	// 										<div class="panel-body collapse in hei">
	// 											<div style="margin-bottom:16px;">
	// 												<a class="btn btn-primary" title="Adicionar Comentário" onclick="addComment(' . $obj->id_clau . ')" href="#modalComentario" data-toggle="modal" data-dismiss="modal"><i class="fa fa-comments-o"></i></a>
	// 												<a class="btn btn-primary" onclick="copyText(`' . str_replace('"', '\'', $obj->tex_clau) . '`)"><i class="fa fa-copy"></i></a>
	// 												<!--<a class="btn default-alt"  onclick="selV1(`' . str_replace('"', '\'', $obj->tex_clau) . '`)">Comparar (Versão 1)</a>
	// 												<a class="btn default-alt"  onclick="selV2(`' . str_replace('"', '\'', $obj->tex_clau) . '`)">Comparar (Versão 2)</a> -->
	// 												<a 
	// 													id="clau' . $obj->id_clau . '"
	// 													data-laboral="' . $obj->denominacao_sinde . '"
	// 													data-patronal="' . $obj->denominacao_sp . '"
	// 													data-grupo="' . $obj->nome_grupo . '"
	// 													data-nome="' . $obj->nome_clausula . '"
	// 													data-texto="' . $obj->tex_clau . '"
	// 													data-ano="' . substr(strstr($obj->database_doc, "/"), 1) . '"
	// 													data-idsinde="' . $obj->id_sinde . '"
	// 													data-idsindp="' . $obj->id_sindp . '"
	// 													data-idgrupo="' . $obj->idgrupo_clausula . '"
														
	// 													onclick="abreComparacao(' . $obj->id_clau . ')" class="btn btn-primary" data-toggle="modal" href="#myModal" data-dismiss="modal">Comparar</a>
	// 											</div>
	// 											<div>
	// 												<p class="tex-clau">' . $obj->tex_clau . '</p>
	// 											</div>
	// 										</div>
	// 									</div>
	// 								</div>
	// 							</div>
	// 						'; //abre_comparacao


	// 						$exibicao .= $html . $html2 . $html4 . $html5;

	// 						$response['response_data']['sind_emp'] = $obj->sigla_sinde;
	// 						$response['response_data']['sind_patr'] = $obj->sigla_sp;
	// 						$response['response_data']['data_base'] = $obj->dataneg;
	// 						$response['response_data']['categoria'] = $obj->descricao_subclasse;
	// 					} else {
	// 						$this->logger->debug($sql);
	// 						$this->logger->debug($this->db->error);

	// 						$response['response_status']['status']       = 0;
	// 						$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 						$response['response_status']['msg']          = '';
	// 					}
	// 				}
	// 			}

	// 			$response['response_data']['exibicao'] 	= $exibicao;
	// 		} else {
	// 			$response = $this->response;
	// 		}
	// 	} else {
	// 		$response = $this->response;
	// 	}

	// 	$this->logger->debug($response['response_status']['status']);

	// 	return $response;
	// }

	function getDestinoNotificacao($data = null)
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

				//LISTA GRUPO ECONOMICO
				$sql = "SELECT 
							id_grupo_economico
							,nome_grupoeconomico
							,logo_grupo
						FROM 
							cliente_grupo;				
				";

				$this->logger->debug($sql);

				$resultsql = mysqli_query($this->db, $sql);
				$htmlGrupo = null;

				while ($obj = $resultsql->fetch_object()) {
					$htmlGrupo .= "<option value='{$obj->id_grupo_economico}'>{$obj->nome_grupoeconomico}</option>";
				}

				$response['response_data']['listaGrupo'] 	= $htmlGrupo;


				//LISTA CLIENTE MATRIZ

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

				$this->logger->debug($sql);

				$resultsql = mysqli_query($this->db, $sql);
				$listMatriz = null;

				while ($obj = $resultsql->fetch_object()) {
					$cnpj = formatCnpjCpf($obj->cnpj_empresa);
					$listMatriz .= "<option value='{$obj->id_empresa}'>{$obj->nome_empresa} | CNPJ: {$cnpj}</option>";
				}

				$response['response_data']['listaMatriz'] 	= $listMatriz;


				//LISTA CLIENTE UNIDADE

				$sql = "SELECT 
							cu.id_unidade AS id_unidade
							,cu.nome_unidade AS nome_unidade
							,cu.cnpj_unidade AS cnpj_unidade
                            ,DATE_FORMAT(cu.data_inclusao,'%d/%m/%Y') AS data_inclusao
                            ,cm.nome_empresa as nome_empresa
							,tuc.tipo_negocio as tipo_negocio
						FROM 
							cliente_unidades cu
						INNER JOIN cliente_matriz cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
						INNER JOIN tipounidade_cliente tuc on tuc.id_tiponegocio = cu.tipounidade_cliente_id_tiponegocio;
				";

				$this->logger->debug($sql);

				$resultsql = mysqli_query($this->db, $sql);

				$listUn = "";
				while ($obj = $resultsql->fetch_object()) {
					$cnpj = formatCnpjCpf($obj->cnpj_unidade);
					$listUn .= "<option value='{$obj->id_unidade}'>{$obj->nome_unidade} | CNPJ: {$cnpj}</option>";
				}

				$response['response_data']['listaUnidade'] 	= $listUn;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		mysqli_close($this->db);
		return $response;
	}

	function addNotificacao($data = null)
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

				$data_final = ($data['validade'] == "" ? '0000-00-00' : $data['validade']);

				$usuario = trim(strstr($data['usuario'], "-", true));

				$sql = "INSERT INTO note_cliente
							(tipo_comentario,
							id_tipo_comentario,
							tipo_usuario_destino,
							id_tipo_usuario_destino,
							tipo_notificacao,
							data_final,
							usuario_adm_id_user,
							comentario)
						VALUES
							('{$data['tipo_com']}',
							'{$data['id_clausula']}',
							'{$data['destino']}',
							'{$data['id_destino']}',
							'{$data['tipo_note']}',
							'{$data_final}',
							'{$usuario}',
							'{$data['comentario']}')
				";

				$this->logger->debug($sql);

				if (!mysqli_query($this->db, $sql)) {

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Falha ao efetuar registro.';

					$this->logger->debug($this->db->error);
				} else {
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = null;
					$response['response_status']['msg']          = 'Cadastro realizado com sucesso';
				}
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		mysqli_close($this->db);
		return $response;
	}


	function toPdf($data = null)
	{

		$this->logger->debug("entrou em toPdf");



		if ($this->response['response_status']['status'] == 1) {

			// Carregando a resposta padrão da função
			$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));


			// reference the Dompdf namespace

			// instantiate and use the dompdf class

			$options = new Options();
			$options->set('isRemoteEnabled', TRUE);


			$dompdf = new Dompdf($options);




			$tohtml = "<!DOCTYPE html>
			<html lang='pt-br'>
			
			<head>
				<meta charset='utf-8'>
				<title>Ineditta</title>
				<meta http-equiv='X-UA-Compatible' content='IE=edge'>
				<meta name='viewport' content='width=device-width, initial-scale=1.0'>
				<meta name='description' content='Ineditta'>
				<meta name='author' content='The Red Team'>
			
				<link rel='stylesheet' href='http://localhost:8080/includes/css/styles.css'>
				<link rel='stylesheet' type='text/css' href='https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css'>
				<link href='http://localhost:8080/includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='styleswitcher'>
				<link href='http://localhost:8080/includes/demo/variations/default.css' rel='stylesheet' type='text/css' media='all' id='headerswitcher'>
				<link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css'>
				
			
				<!-- The following CSS are included as plugins and can be removed if unused-->
				<link rel='stylesheet' type='text/css' href='http://localhost:8080/includes/plugins/form-toggle/toggles.css' />
				<link rel='stylesheet' type='text/css' href='http://localhost:8080/includes/css/msg.css' />
			
				<!-- Data Grid -->
				<link rel='stylesheet' href='http://localhost:8080/includes/plugins/datagrid/styles/jplist-custom.css'>
			
				<!-- Plugins -->
				<link href='http://localhost:8080/includes/plugins/select2/select2-4.1.0-rc.0/dist/css/select2.min.css' rel='stylesheet' />
				<link rel='stylesheet' href='http://localhost:8080/includes/plugins/admin-lte-plugins/daterangepicker/daterangepicker.css'>
			
			
			
			
				<style>
					button:disabled {
						cursor: not-allowed;
						pointer-events: all !important;
					}
			
					td {
						word-break: keep-all;
					}
			
					.tex-clau {
						text-justify: inter-word;
						white-space: pre-line;
					}
			
					.hei {
						max-height: 50vh;
						overflow: auto;
					}
			
					div.dataTables_wrapper div.dataTables_filter input {
						width: 500px;
					}
			
					.dataTables_filter {
						display: flex;
						justify-content: flex-end;
					}
			
					.table-info {
						border-collapse: separate;
						border-spacing: 0;
					}
			
					#cabecalho th {
						position: sticky;
						top: 0;
						background-color: #fff;
						border-bottom: 2px solid #e6e7e8 !important;
						z-index: 10;
					}
			
					.fixTableHead {
						padding: 0px 20px 20px 20px !important;
						max-height: 35vh; 
						overflow: scroll;
						border-collapse: separate;
						
					}
			
					.select2-container {
						width: 100% !important;
					}
			
					.novoFragmento {
						white-space: pre-wrap;
						background: #fff;
						border: #e35d6a solid;
						border-width: 0px 0px 0px 0.5em;
						border-radius: 0.5em;
						font-family: sans-serif;
						font-size: 88%;
						line-height: 1.6;
						box-shadow: 2px 2px 2px #ddd;
						padding: 1em;
						margin: 0;
					}
			
					.novoInserted {
						font-weight: bold;
						background-color: #f1aeb5 !important;
						color: #222;
						border-radius: 0.25em;
						padding: 0.2em 1px;
					}
			
					.dataTables_scroll .dataTables_scrollHead .dataTables_scrollHeadInner {
						width: 100% !important;
					}
			
					.dataTables_scroll .dataTables_scrollHead .dataTables_scrollHeadInner .table {
						width: 100% !important;
					}
				</style>
			</head>
			<body>
			<img src='http://localhost:8080/includes/img/ineditta_banner.jpg' width='300' alt='Ineditta' style='border:1px solid black'>";
			$tohtml .= $data['html'];

			$tohtml .= "</body>";
			$this->logger->debug("criou o html");

			libxml_use_internal_errors(true);

			$dompdf->loadHtml($tohtml);

			libxml_clear_errors();

			$this->logger->debug("deu load no html");
			// (Optional) Setup the paper size and orientation
			$dompdf->setPaper('A4', 'landscape');
			$this->logger->debug("setou papel");
			// Render the HTML as PDF

			// $f = null;
			// $l = null;
			// if (headers_sent($f, $l)) {
			// 	$this->logger->debug( $f, '<br/>', $l, '<br/>');
			// 	die('now detect line');
			// }
			$dompdf->render();
			$this->logger->debug("renderizou BBBBBBBBBBBB ");
			// Output the generated PDF to Browser
			//$dompdf->stream("dompdf_out.pdf", array("Attachment" => false));

			$this->logger->debug("voltou para o browser");

			$response['response_data']['dompdf'] =  base64_encode($dompdf->output());
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		mysqli_close($this->db);
		return $response;
	}
}
