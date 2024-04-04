<?php

/**
 * @author    {Lucas A. R. Volpati}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2022-12-21 14:53 ( v1.0.0 ) - 
	}
 **/



include_once "class.usuario.php";
include_once "helpers.php";

class filtro
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

	function generateFilters($data = null)
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

				$user = (new usuario())->getUserData($data)['response_data']['user_data'];

				$idsFiliais = json_decode(($user->ids_fmge == '""') ? '["0"]' : $user->ids_fmge);
				//$idsMatrizes = json_decode(($user->ids_matrizes == '""') ? '["0"]' : $user->ids_matrizes);
				$idGrupo = json_decode($user->id_grupoecon);

				$grupos = [];
				$matrizes = [];
				$unidades = [];
				$localizacao = [];
				//DEFININDO LISTA DE UNIDADES
				$this->logger->debug($user);
				if ($user->nivel == "Unidade") {
					//CLIENTE UNIDADE
					foreach ($idsFiliais as $id) {

						$sql = "SELECT
									id_unidade,
									nome_unidade,
									cl.cliente_matriz_id_empresa,
									cl.localizacao_id_localizacao
								FROM cliente_unidades as cl
								LEFT JOIN cliente_matriz as mt on mt.id_empresa = cl.cliente_matriz_id_empresa
								LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = mt.cliente_grupo_id_grupo_economico
								
								WHERE cl.id_unidade = '{$id}'
						";

						$this->logger->debug($sql);

						$result = mysqli_query($this->db, $sql);

						$obj = $result->fetch_object();

						array_push($unidades, $obj->id_unidade);
						array_push($matrizes, $obj->cliente_matriz_id_empresa);
						array_push($grupos, $idGrupo);
						array_push($localizacao, $obj->localizacao_id_localizacao);
					}
				} else if ($user->nivel == "Matriz") {
					//CLIENTE MATRIZ
					foreach ($idsFiliais as $matriz) {
						$sqlMatriz = "SELECT
									id_empresa,
									nome_empresa,
									IFNULL(GROUP_CONCAT(DISTINCT cl.localizacao_id_localizacao), GROUP_CONCAT(IFNULL( cl.localizacao_id_localizacao, null))) as localizacao,								
									gp.id_grupo_economico,
									(SELECT IFNULL(GROUP_CONCAT(DISTINCT id_unidade), GROUP_CONCAT(IFNULL( id_unidade, null))) as id_unidades FROM cliente_unidades WHERE cliente_matriz_id_empresa = mz.id_empresa) as unidades
								FROM cliente_matriz as mz
								LEFT JOIN cliente_unidades as cl on cl.cliente_matriz_id_empresa = mz.id_empresa
								LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = mz.cliente_grupo_id_grupo_economico
								
								WHERE cl.id_unidade = '{$matriz}'
						";

						$this->logger->debug($sqlMatriz);

						$resultMatriz = mysqli_query($this->db, $sqlMatriz);

						$objMatriz = $resultMatriz->fetch_object();

						// $objUnidade = new stdClass();

						array_push($unidades, $objMatriz->unidades);
						array_push($matrizes, $objMatriz->id_empresa);
						!in_array($objMatriz->id_grupo_economico, $grupos) ? array_push($grupos, $objMatriz->id_grupo_economico) : '';
						array_push($localizacao, $objMatriz->localizacao);
					}
				} else {
					//GRUPO ECONOMICO OU INEDITTA
					$where = $user->nivel == "Grupo Econômico" ? "WHERE cliente_grupo_id_grupo_economico = {$idGrupo}" : "";
					$sqlGrupoEcon = "SELECT
										id_empresa
									FROM cliente_matriz
									{$where}
					";

					$this->logger->debug($sqlGrupoEcon);

					$result2 = mysqli_query($this->db, $sqlGrupoEcon);

					$listaMatriz = [];
					while ($obj3 = $result2->fetch_object()) {
						array_push($listaMatriz, $obj3->id_empresa);
					}

					foreach ($listaMatriz as $matriz2) {
						$sqlGrupo = "SELECT
										DISTINCT id_grupo_economico,
										mt.id_empresa,
										(SELECT IFNULL(GROUP_CONCAT(DISTINCT id_unidade), GROUP_CONCAT(IFNULL( id_unidade, null))) as id_unidades FROM cliente_unidades WHERE cliente_matriz_id_empresa = mt.id_empresa) as unidades,
										IFNULL(GROUP_CONCAT(DISTINCT cl.localizacao_id_localizacao), GROUP_CONCAT(IFNULL( cl.localizacao_id_localizacao, null))) as localizacao
									FROM cliente_grupo as gp
									LEFT JOIN cliente_matriz as mt on mt.cliente_grupo_id_grupo_economico = gp.id_grupo_economico
									LEFT JOIN cliente_unidades as cl on cl.cliente_matriz_id_empresa = mt.id_empresa
									
									WHERE mt.id_empresa = $matriz2
						";

						$result4 = mysqli_query($this->db, $sqlGrupo);

						$obj4 = $result4->fetch_object();

						array_push($unidades, $obj4->unidades);
						array_push($matrizes, $obj4->id_empresa);
						!in_array($obj4->id_grupo_economico, $grupos) ? array_push($grupos, $obj4->id_grupo_economico) : '';
						array_push($localizacao, $obj4->localizacao);
					}
				}

				//LISTA GRUPO ECONOMICO
				$optGrupo = "<option value=''></option>";
				foreach ($grupos as $grupo) {
					$sql1 = "SELECT
							id_grupo_economico,
							nome_grupoeconomico
						FROM cliente_grupo
						WHERE id_grupo_economico = {$grupo}
					";

					$result1 = mysqli_query($this->db, $sql1);

					while ($obj = $result1->fetch_object()) {

						if ($user->nivel != "Ineditta") {
							$optGrupo .= "<option value='{$obj->id_grupo_economico}' selected>{$obj->nome_grupoeconomico}</option>";
						} else {
							$optGrupo .= "<option value='{$obj->id_grupo_economico}'>{$obj->nome_grupoeconomico}</option>";
						}
					}
				}

				//LISTA MATRIZ
				$optMatriz = '';
				foreach ($matrizes as $matriz) {
					$sql2 = "SELECT
								id_empresa,
								razao_social,
								cnpj_empresa,
								codigo_empresa
							FROM cliente_matriz
							WHERE id_empresa = '{$matriz}'
					";

					$result2 = mysqli_query($this->db, $sql2);


					while ($obj = $result2->fetch_object()) {
						$cnpj = formatCnpjCpf($obj->cnpj_empresa);
						if ($user->nivel != "Ineditta") {
							$optMatriz .= "<option value='{$obj->id_empresa}' selected>{$obj->codigo_empresa} / {$cnpj} / {$obj->razao_social}</option>";
						} else {
							$optMatriz .= "<option value='{$obj->id_empresa}'>{$obj->codigo_empresa} / {$cnpj} / {$obj->razao_social}</option>";
						}
					}
				}

				//LISTA UNIDADE
				$optUnidade = '';
				$optLocalizacao = '';
				$listaGeralUnidades = [];
				foreach (array_filter($unidades) as $unidade) {
					$this->logger->debug($unidade);

					$listaUnidades = explode(",", $unidade);

					$listaGeralUnidades = array_merge($listaGeralUnidades, $listaUnidades);
				}

				$this->logger->debug($listaGeralUnidades);

				//LISTA LOCALIZAÇÃO
				$listaLocalizacao = [];
				foreach ($listaGeralUnidades as $unidade) {
					$this->logger->debug($unidade);
					$sql3 = "SELECT
							id_unidade,
							nome_unidade,
							cnpj_unidade,
							codigo_unidade,
							cod_sindcliente,
							regional,
							localizacao_id_localizacao,
							lc.municipio,
							lc.uf,
							lc.regiao
						FROM cliente_unidades as cl
						LEFT JOIN localizacao as lc on lc.id_localizacao = cl.localizacao_id_localizacao
						WHERE id_unidade = '{$unidade}'
					";

					$result3 = mysqli_query($this->db, $sql3);


					$obj = $result3->fetch_object();
					$cnpj = formatCnpjCpf($obj->cnpj_unidade);

					in_array($obj->municipio, $listaLocalizacao) ?? array_push($listaLocalizacao, $obj->municipio);

					if ($user->nivel != "Ineditta") {
						$optUnidade .= "<option value='{$obj->id_unidade}' selected>Cód: {$obj->codigo_unidade} / CNPJ: {$cnpj} / Nome: {$obj->nome_unidade} / Cod. Sind. Cliente: {$obj->cod_sindcliente}</option>";
					} else {
						$optUnidade .= "<option value='{$obj->id_unidade}'>Cód: {$obj->codigo_unidade} / CNPJ: {$cnpj} / Nome: {$obj->nome_unidade} / Cod. Sind. Cliente: {$obj->cod_sindcliente}</option>";
					}

					if (!in_array($obj->municipio, $listaLocalizacao)) {
						$optLocalizacao .= ($user->nivel != "Ineditta" ? "<option value='{$obj->localizacao_id_localizacao}' selected>Mun: {$obj->municipio} - {$obj->uf} / {$obj->regiao} </option>" : "<option value='{$obj->localizacao_id_localizacao}'>Mun: {$obj->municipio} - {$obj->uf} / {$obj->regiao} </option>");
					}
				}

				$response['response_data']['opt_grupo'] = $optGrupo;
				$response['response_data']['opt_matriz'] = $optMatriz;
				$response['response_data']['opt_unidade'] = $optUnidade;
				$response['response_data']['opt_local'] = $optLocalizacao;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_data']);

		return $response;
	}

	function getFilters($data = null)
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

				$user = (new usuario())->getUserData($data)['response_data']['user_data'];



				if ($user->nivel == "Grupo Econômico") {
					// $idsMatrizes = array_filter(json_decode($user->ids_matrizes == '""' ? '[]' : $user->ids_matrizes));//array
					$id = $user->id_grupoecon;
					$where = "AND gp.id_grupo_economico =";
					$tipo = "Grupo";
				} else if ($user->nivel == "Matriz") {
					// $idsMatrizes = array_filter(json_decode($user->ids_matrizes == '""' ? '[]' : $user->ids_matrizes));//array
					$id = array_filter(json_decode(($user->ids_fmge == '""') ? '[]' : $user->ids_fmge)); //array
					$where = "AND cl.id_unidade =";
					$tipo = "Matriz";
				} else if ($user->nivel == "Unidade") {
					// $idsMatrizes = array_filter(json_decode($user->ids_matrizes == '""' ? '[]' : $user->ids_matrizes));//array

					$id = array_filter(json_decode(($user->ids_fmge == '""') ? '[]' : $user->ids_fmge)); //array
					$where = "AND cl.id_unidade =";
					$tipo = "Unidade";
				} else {
					$id = "";
					$where = "";
					$tipo = "Ineditta";
				}

				$idUser = $user->id_user;

				//$idsMatrizes = array_filter(json_decode($user->ids_matrizes == '""' ? '[]' : $user->ids_matrizes)); //array

				$idsUnidades = array_filter(json_decode($user->ids_fmge == '""' ? '[]' : $user->ids_fmge)); //array

				$term = is_array($id) ? $id[0] : $id;

				$sql = "SELECT
							cl.id_unidade,
							cl.nome_unidade,
							cl.cnpj_unidade,
							cl.codigo_unidade,
							cl.cod_sindcliente,
							IFNULL(GROUP_CONCAT(DISTINCT cnae.id_cnae), GROUP_CONCAT(IFNULL( cnae.id_cnae, null))) as id_cnae,
							cnae.descricao_subclasse,
							ma.id_empresa,
							ma.cnpj_empresa,
							ma.codigo_empresa,
							ma.razao_social,
							gp.id_grupo_economico,
							gp.nome_grupoeconomico,
							lc.id_localizacao,
							lc.municipio,
							lc.uf,
							lc.regiao,
							IFNULL(GROUP_CONCAT(DISTINCT sinde.id_sinde), GROUP_CONCAT(IFNULL( sinde.id_sinde, null))) as id_sinde,
							IFNULL(GROUP_CONCAT(DISTINCT sp.id_sindp), GROUP_CONCAT(IFNULL( sp.id_sindp, null))) as id_patr,
							bem.classe_cnae_idclasse_cnae,
							bp.classe_cnae_idclasse_cnae
							
							FROM cliente_unidades cl
						LEFT JOIN localizacao as lc on cl.localizacao_id_localizacao = lc.id_localizacao
						-- Atividade Economica
						-- LEFT JOIN cnae_emp as cn ON (cn.cliente_unidades_id_unidade = cl.id_unidade AND cn.data_final = '00-00-0000')
						LEFT JOIN classe_cnae as cnae on JSON_CONTAINS(cl.cnae_unidade, CONCAT('{\"id\":', cnae.id_cnae,'}'), '$') -- cnae.id_cnae = cn.classe_cnae_idclasse_cnae
						-- Bases territoriais
						LEFT JOIN base_territorialsindemp as bem on (bem.localizacao_id_localizacao1 = lc.id_localizacao 
						AND JSON_CONTAINS(cl.cnae_unidade, CONCAT('{\"id\":', bem.classe_cnae_idclasse_cnae,'}'), '$'))
						LEFT JOIN base_territorialsindpatro as bp on (bp.localizacao_id_localizacao1 = lc.id_localizacao
						AND JSON_CONTAINS(cl.cnae_unidade, CONCAT('{\"id\":', bp.classe_cnae_idclasse_cnae,'}'), '$'))
						-- Sindicatos
						LEFT JOIN sind_emp as sinde on (sinde.id_sinde = bem.sind_empregados_id_sinde1)
						LEFT JOIN sind_patr as sp on (sp.id_sindp = bp.sind_patronal_id_sindp)
						-- Matriz e Grupo Economico
						LEFT JOIN cliente_matriz as ma on ma.id_empresa = cl.cliente_matriz_id_empresa
						LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = ma.cliente_grupo_id_grupo_economico

						WHERE bem.localizacao_id_localizacao1 IS  NOT NULL {$where} {$term} AND IF('{$tipo}' like 'Ineditta', 1 = 1,JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$idUser}),
						CONCAT('',cl.id_unidade, ''),'$'))
						
						GROUP BY cl.id_unidade
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);


				$optUnidade = '';
				$optMatriz = '';
				$optGrupo = '<option value="0">-</option>';
				$optCnae = '';
				$optLocalizacao = '';
				$optUf = '';
				$optRegiao = '';
				$listaMultiplos = [];
				$listaMultiplosMatriz = [];
				$listaIdsLaboral = [];
				$listaIdsPatronal = [];
				$listaIdsCnae = [];
				while ($obj = $result->fetch_object()) {

					$cnpjUnidade = formatCnpjCpf($obj->cnpj_unidade);
					$cnpjMatriz = formatCnpjCpf($obj->cnpj_empresa);

					//LISTA GRUPO ECONOMICO
					if (!in_array($obj->nome_grupoeconomico, $listaMultiplos)) {
						if ($tipo != "Ineditta") {
							$optGrupo = "<option value='{$obj->id_grupo_economico}' selected>{$obj->nome_grupoeconomico}</option>";
						} else {
							$optGrupo .= "<option value='{$obj->id_grupo_economico}'>{$obj->nome_grupoeconomico}</option>";
							array_push($listaMultiplos, $obj->nome_grupoeconomico);
						}
					}


					if ($tipo == 'Ineditta') {
						//ATIVIDADE ECONOMICA
						$optCnae .= !in_array($obj->descricao_subclasse, $listaMultiplos) ? "<option value='{$obj->id_cnae}'>{$obj->descricao_subclasse}</option>" : "";
						array_push($listaMultiplos, $obj->descricao_subclasse);

						//LISTA UNIDADE
						$optUnidade .= !in_array($obj->cnpj_unidade, $listaMultiplos) ? "<option value='{$obj->id_unidade}'>Cód: {$obj->codigo_unidade} / CNPJ: {$cnpjUnidade} / Nome: {$obj->nome_unidade} / Cod. Sind. Cliente: {$obj->cod_sindcliente}</option>" : "";
						array_push($listaMultiplos, $obj->cnpj_unidade);

						//LISTA MATRIZ
						$optMatriz .= !in_array($obj->razao_social, $listaMultiplos) ? "<option value='{$obj->id_empresa}'>{$obj->codigo_empresa} / {$cnpjMatriz} / {$obj->razao_social}</option>" : "";
						array_push($listaMultiplosMatriz, $obj->razao_social);
					}

					//LISTA LOCALIZAÇÃO MUNICIPIO
					if (!in_array($obj->municipio, $listaMultiplos)) {
						$optLocalizacao .= "<option value='Municipio+{$obj->municipio}'>{$obj->municipio} - {$obj->uf} / {$obj->regiao}</option>";
						array_push($listaMultiplos, $obj->municipio);
					}

					//LISTA LOCALIZAÇÃO UF
					if (!in_array($obj->uf, $listaMultiplos)) {
						$optUf .= "<option value='Uf+{$obj->uf}'>{$obj->uf}</option>";
						array_push($listaMultiplos, $obj->uf);
					}

					//LISTA LOCALIZAÇÃO REGIAO
					// if (!in_array($obj->regiao, $listaMultiplos)) {
					// 	$optRegiao .= "<option value='regiao+{$obj->regiao}'>{$obj->regiao}</option>";
					// 	array_push($listaMultiplos, $obj->regiao);
					// }

					//LISTAS LABORAL E PATRONAL
					$listaIdsLaboral = array_filter(array_unique(array_merge($listaIdsLaboral, explode(",", $obj->id_sinde))));
					$listaIdsPatronal = array_filter(array_unique(array_merge($listaIdsPatronal, explode(",", $obj->id_patr))));
					$listaIdsCnae = array_filter(array_unique(array_merge($listaIdsCnae, explode(",", $obj->id_cnae))));


					$this->logger->debug($obj);
				}

				//LISTA ATIVIDADE ECONOMICA
				if ($tipo != 'Ineditta') {

					if ($listaIdsCnae) {
						foreach ($listaIdsCnae as $id) {

							$sqlCn = "SELECT
										id_cnae,
										descricao_subclasse
									FROM classe_cnae
									WHERE id_cnae = {$id}
							";

							$this->logger->debug($sqlCn);

							$resultCn = mysqli_query($this->db, $sqlCn);

							$objCn = $resultCn->fetch_object();

							if (!in_array($objCn->descricao_subclasse, $listaMultiplos)) {
								$optCnae .= "<option value='{$objCn->id_cnae}'>{$objCn->descricao_subclasse}</option>";
								array_push($listaMultiplos, $objCn->descricao_subclasse);
							}
						}
					}
				}

				//LISTA UNIDADES
				if ($tipo != 'Ineditta') {
					if ($idsUnidades) {
						foreach ($idsUnidades as $id) {
							// if ($tipo == 'Ineditta') {
							// 	$whereUn = ""; 
							// }else {
							// 	$whereUn = "WHERE id_unidade = {$id}";
							// }

							$sqlUn = "SELECT
										id_unidade,
										nome_unidade,
										cod_sindcliente,
										codigo_unidade,
										cnpj_unidade
									FROM cliente_unidades
									WHERE id_unidade = {$id}
							";

							$this->logger->debug($sqlUn);

							$resultUn = mysqli_query($this->db, $sqlUn);

							$objUn = $resultUn->fetch_object();
							$cnpjUnidade = formatCnpjCpf($objUn->cnpj_unidade);

							if (!in_array($objUn->cnpj_unidade, $listaMultiplos)) {
								$optUnidade .= "<option value='{$objUn->id_unidade}'>Cód: {$objUn->codigo_unidade} / CNPJ: {$cnpjUnidade} / Nome: {$objUn->nome_unidade} / Cod. Sind. Cliente: {$objUn->cod_sindcliente}</option>";
								array_push($listaMultiplos, $objUn->cnpj_unidade);
							}
						}
					}
				}

				//LISTA CLIENTE MATRIZ
				if ($tipo != 'Ineditta') {
					foreach ($idsUnidades as $id) {
						if ($tipo == 'Ineditta') {
							$whereMt = "";
						} else {
							$whereMt = "WHERE cu.id_unidade = {$id}";
						}
						$sqlMt = "SELECT
								id_empresa,
								codigo_empresa,
								cnpj_empresa,
								razao_social
							FROM cliente_matriz as cm LEFT JOIN
							cliente_unidades as cu ON cu.cliente_matriz_id_empresa = cm.id_empresa
							{$whereMt}
						";

						$this->logger->debug($sqlMt);

						$result = mysqli_query($this->db, $sqlMt);

						$objMt = $result->fetch_object();

						$cnpjMatriz = formatCnpjCpf($objMt->cnpj_empresa);

						if (!in_array($objMt->cnpj_empresa, $listaMultiplosMatriz)) {
							($tipo == "Unidade" ? $optMatriz = "<option value='{$objMt->id_empresa}' selected>{$objMt->codigo_empresa} / {$cnpjMatriz} / {$objMt->razao_social}</option>" : $optMatriz .= "<option value='{$objMt->id_empresa}'>{$objMt->codigo_empresa} / {$cnpjMatriz} / {$objMt->razao_social}</option>");
							array_push($listaMultiplosMatriz, $objMt->cnpj_empresa);
							$this->logger->debug("TÁ NA LISTA");
						}

						$this->logger->debug($objMt);
					}
				}

				$response['response_data']['unidade'] = $optUnidade;
				$response['response_data']['matriz'] = $optMatriz;
				$response['response_data']['grupo'] = $optGrupo;
				$response['response_data']['cnae'] = $optCnae;
				$response['response_data']['localizacao'] = $optLocalizacao;
				$response['response_data']['uf'] = $optUf;
				$response['response_data']['regiao'] = $optRegiao;

				$this->logger->debug($optCnae);
				$this->logger->debug($optLocalizacao);
				$this->logger->debug($optUf);
				$this->logger->debug($optRegiao);

				//OBTEM LISTA DE OPTIONS LABORAL
				$optEmp = "";
				foreach ($listaIdsLaboral as $laboral) {
					$sqlEmp = "SELECT 
									id_sinde
									,razaosocial_sinde
									,sigla_sinde
									,cnpj_sinde
									,logradouro_sinde
									,email1_sinde
									,fone1_sinde
									,site_sinde
								FROM 
									sind_emp
								WHERE id_sinde = {$laboral}
					";

					$resultEmp = mysqli_query($this->db, $sqlEmp);

					$this->logger->debug($resultEmp);

					$objEmp = $resultEmp->fetch_object();
					$cnpj = formatCnpjCpf($objEmp->cnpj_sinde);
					$optEmp .= "<option value='{$objEmp->id_sinde}'>{$objEmp->sigla_sinde} / {$cnpj} / {$objEmp->razaosocial_sinde}</option>";
				}

				$response['response_data']['laboral'] = $optEmp;


				//OBTEM LISTA DE OPTIONS PATRONAL
				$optPatr = "";
				foreach ($listaIdsPatronal as $patronal) {
					$sqlPatr = "SELECT 
									id_sindp
									,razaosocial_sp
									,sigla_sp
									,cnpj_sp
									,logradouro_sp
									,email1_sp
									,fone1_sp
									,site_sp
								FROM 
									sind_patr
								WHERE 	id_sindp = {$patronal}
					";

					$this->logger->debug($sqlPatr);

					$resultPatr = mysqli_query($this->db, $sqlPatr);

					$this->logger->debug($resultPatr);

					$objPatr = $resultPatr->fetch_object();
					$cnpj = formatCnpjCpf($objPatr->cnpj_sp);
					$optPatr .= "<option value='{$objPatr->id_sindp}'>{$objPatr->sigla_sp} / {$cnpj} / {$objPatr->razaosocial_sp}</option>";
				}

				$response['response_data']['patronal'] = $optPatr;

				//OBTEM LISTA DE CLAUSULAS E GRUPO CLAUSULAS
				$opGrupoClau = "";
				$sql = "SELECT 
							* 
						FROM
							grupo_clausula
						WHERE idgrupo_clausula
						ORDER BY nome_grupo ASC
				";

				$resultGrp = mysqli_query($this->db, $sql);

				while ($objGrp = $resultGrp->fetch_object()) {
					$opGrupoClau .= "<option value='{$objGrp->idgrupo_clausula}'>{$objGrp->nome_grupo}</option>";
				}

				$response['response_data']['grupo_clausula'] = $opGrupoClau;

				//LISTA DE DATA-BASE COM DATA ATUAL SELECIONADA
				$sqlData = "SELECT 
								distinct database_doc,
								substring(database_doc,5) as ano,
    							(SELECT max(database_doc) FROM doc_sind) as maior
							FROM 
								doc_sind
							ORDER BY ano DESC
				";

				$resultData = mysqli_query($this->db, $sqlData);

				$optiondata = "";
				while ($objData = $resultData->fetch_object()) {
					

					// if ($objData->database_doc == $objData->maior ) {
					// 	$optiondata .= "<option value='{$objData->database_doc}' selected>{$objData->database_doc}</option>";
					// }else {
					// 	$optiondata .= "<option value='{$objData->database_doc}'>{$objData->database_doc}</option>";
					// }

					$optiondata .= "<option value='{$objData->database_doc}'>{$objData->database_doc}</option>";

				}

				$response['response_data']['data_base'] = $optiondata;

			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_data']);

		return $response;
	}

	function getCnae($data = null)
	{
		$iduser = $data['iduser'];
		$tipo = $data['tipo'];
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

				$unidades = $data['unidades'];
				$matriz = $data['id_matriz'];

				$opt = "";
				$sql = "SELECT
							distinct cnae.id_cnae as cnae,
							cnae.descricao_subclasse as sub,
							gp.nome_grupoeconomico
						FROM cliente_unidades as cl
						LEFT JOIN cliente_matriz as ma on cl.cliente_matriz_id_empresa = ma.id_empresa
						LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = ma.cliente_grupo_id_grupo_economico
						-- LEFT JOIN cnae_emp as ce ON (ce.cliente_unidades_id_unidade = cl.id_unidade AND ce.data_final = '00-00-0000')
						LEFT JOIN classe_cnae as cnae on JSON_CONTAINS(cl.cnae_unidade, CONCAT('{\"id\":', cnae.id_cnae,'}'), '$')
						
						WHERE  IF('{$matriz}' = '',1=1,ma.id_empresa IN (0{$matriz}))
						AND IF('{$unidades}' = '',1=1,cl.id_unidade IN (0{$unidades}))
						AND IF('{$data['id_grupo']}' like '0',1=1,gp.id_grupo_economico = '{$data['id_grupo']}') 
						AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',cl.id_unidade, ''),'$'))
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);


				while ($obj = $result->fetch_object()) {
					$opt .= "<option value='{$obj->cnae}'>{$obj->sub}</option>";
				}

				$response['response_data']['lista_cnae'] = $opt;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function getDataBase($data = null)
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

				
				

				// $this->logger->debug($laboral);
				// $this->logger->debug($patronal);

				$optionData = "";

				if (is_array($data['sind_laboral'])) {
					$laboral = implode("','", $data['sind_laboral']);

					$sql = "SELECT 
								distinct doc.database_doc as data_base,
                                se.id_sinde
							FROM 
								sind_emp as se
							LEFT JOIN doc_sind_sind_emp as de on de.sind_emp_id_sinde = se.id_sinde
							LEFT JOIN doc_sind as doc on doc.id_doc = de.doc_sind_id_doc
							
							WHERE se.id_sinde IN ('{$laboral}')

							ORDER BY doc.database_doc ASC
					";

					$this->logger->debug($sql);

					$resultEmp = mysqli_query($this->db, $sql);

					while ($obj = $resultEmp->fetch_object()) {
						if ($obj->data_base != 0) {
							$optionData .= "<option value='{$obj->data_base}'>{$obj->data_base}</option>";
						}
					}
				}

				if (is_array($data['sind_patronal'])) {
					$patronal = implode("','", $data['sind_patronal']);

					$sql = "SELECT 
								distinct doc.database_doc as data_base,
								sp.id_sindp
							FROM 
								sind_patr as sp
							LEFT JOIN doc_sind_sind_patr as dp on dp.sind_patr_id_sindp = sp.id_sindp
							LEFT JOIN doc_sind as doc on doc.id_doc = dp.doc_sind_id_doc
							
							WHERE sp.id_sindp IN ('{$patronal}')

							ORDER BY doc.database_doc ASC
					";

					$this->logger->debug($sql);

					$resultPatr = mysqli_query($this->db, $sql);

					while ($obj = $resultPatr->fetch_object()) {
						if ($obj->data_base != 0) {
							$optionData .= "<option value='{$obj->data_base}'>{$obj->data_base}</option>";
						}
					}
				}

				

				if (!is_array($data['sind_laboral']) && !is_array($data['sind_patronal'])) {
					//LISTA DE DATA-BASE COM DATA ATUAL SELECIONADA
					$sqlData = "SELECT 
									distinct database_doc,
									substring(database_doc,5) as ano,
									(SELECT max(database_doc) FROM doc_sind) as maior
								FROM 
									doc_sind
								ORDER BY ano ASC
					";

					$resultData = mysqli_query($this->db, $sqlData);

					$optionData .= "<option value='todos'>Todos</option>";
					while ($objData = $resultData->fetch_object()) {
					
						// if ($objData->database_doc == $objData->maior ) {
						// 	$optionData .= "<option value='{$objData->database_doc}' selected>{$objData->database_doc}</option>";
						// }else {
							$optionData .= "<option value='{$objData->database_doc}'>{$objData->database_doc}</option>";
						// }

					}

				}

				$response['response_data']['data_base'] = $optionData;

			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function getMatriz($data = null)
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


				$sql = "SELECT
							id_empresa,
							razao_social,
							cnpj_empresa,
							codigo_empresa
						FROM cliente_matriz
						WHERE IF('{$data['id_grupo']}' like '0',1=1,cliente_grupo_id_grupo_economico = '{$data['id_grupo']}')
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);

				$opt = "";
				while ($obj = $result->fetch_object()) {
					$cnpj = formatCnpjCpf($obj->cnpj_empresa);
					$this->logger->debug(formatCnpjCpf($obj->cnpj_empresa));
					$opt .= "<option value='{$obj->id_empresa}'>{$obj->codigo_empresa} / {$cnpj} / {$obj->razao_social}</option>";
				}

				$response['response_data']['lista_matriz'] = $opt;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function getUnidade($data = null)
	{

		$iduser = $data['iduser'];
		$tipo = $data['tipo'];

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

				$ids = $data['id_matriz'] ? $data['id_matriz'] : explode(",", $data['id_grupo']);

				$opt = "";
				for ($i = 0; $i < count($ids); $i++) {
					$where = $data['id_matriz'] ? "cliente_matriz_id_empresa = {$ids[$i]}" : "gp.id_grupo_economico = {$ids[$i]}";
					$sql = "SELECT
							id_unidade,
							nome_unidade,
							cnpj_unidade,
							codigo_unidade,
							cod_sindcliente,
							regional
						FROM cliente_unidades as cl
						LEFT JOIN cliente_matriz as mt on mt.id_empresa = cl.cliente_matriz_id_empresa
						LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = mt.cliente_grupo_id_grupo_economico
						WHERE {$where} AND IF('{$tipo}' like 'Ineditta', 1 = 1, JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$iduser}),
						CONCAT('',id_unidade, ''),'$'))
					";

					$this->logger->debug($sql);

					$result = mysqli_query($this->db, $sql);


					while ($obj = $result->fetch_object()) {
						$cnpj = formatCnpjCpf($obj->cnpj_unidade);
						$opt .= "<option value='{$obj->id_unidade}'>Cód: {$obj->codigo_unidade} / CNPJ: {$cnpj} / Nome: {$obj->nome_unidade} / Cod. Sind. Cliente: {$obj->cod_sindcliente} / Regional: {$obj->regional}</option>";
					}
				}

				$this->logger->debug($opt);

				$response['response_data']['lista_unidade'] = $opt;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function getClausulas($data = null)
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

				$idGrupo = $data['id_grupo_clausula'];
				$opt = "";
				for ($i = 0; $i < count($idGrupo); $i++) {
					$sql = "SELECT
							id_estruturaclausula,
							nome_clausula
						FROM estrutura_clausula
						WHERE grupo_clausula_idgrupo_clausula = '{$idGrupo[$i]}'
					";

					$result = mysqli_query($this->db, $sql);


					while ($obj = $result->fetch_object()) {
						$opt .= "<option value='{$obj->id_estruturaclausula}'>{$obj->nome_clausula}</option>";
					}
				}

				$response['response_data']['lista_clausulas'] = $opt;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function getSindicatosByLocal($data = null)
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

				$where = '';
				if ($data['localidade'] != "") {
					if (count($data['localidade']) > 1) {
						$string = "";
						foreach ($data['localidade'] as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);

							$string .= "'{$content}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$where .= ($where != "" ? " AND loc.{$column} IN ({$string})" : " loc.{$column} IN ({$string})");
					} else {
						$string = "";
						foreach ($data['localidade'] as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);

							$string .= "'{$content}'";
						}

						$where .= ($where != "" ? " AND loc.{$column} = {$string}" : " loc.{$column} = {$string}");
					}
				}

				$this->logger->debug($where);

				if ($data['cnaes'] != "") {
					$inbem = implode(",", $data['cnaes']);
					if ($where != "") {
						$where .= " AND bt.classe_cnae_idclasse_cnae IN ({$inbem})";
					} else {
						$where .= " bt.classe_cnae_idclasse_cnae IN ({$inbem})";
					}
				}

				$this->logger->debug($where);

				if ($data['grupo'] != "" && $data['grupo'] != 0) {
					if ($where != "") {
						$where .= " AND gp.id_grupo_economico IN ({$data['grupo']})";
					} else {
						$where .= " gp.id_grupo_economico IN ({$data['grupo']})";
					}
				}

				$this->logger->debug($where);

				if ($data['matriz'] != "") {
					$inbem = implode(",", $data['matriz']);
					if ($where != "") {
						$where .= " AND mt.id_empresa IN ({$inbem})";
					} else {
						$where .= " mt.id_empresa IN ({$inbem})";
					}
				}

				$this->logger->debug($where);

				if ($data['unidades'] != "") {
					$inbem = implode(",", $data['unidades']);
					if ($where != "") {
						$where .= " AND clt.id_unidade IN ({$inbem})";
					} else {
						$where .= " clt.id_unidade IN ({$inbem})";
					}
				}

				if (!$data['unidades'] && !$data['matriz'] && !$data['cnaes'] && !$data['localidade']) {
					$where = "gp.id_grupo_economico = {$data['grupo']}";
				}

				$this->logger->debug($where);

				//OBTEM LISTA DE OPTIONS LABORAL
				$sql = "SELECT
							sinde.id_sinde,
							bt.localizacao_id_localizacao1,
							loc.municipio,
							loc.uf,
							loc.regiao
						FROM cliente_unidades as clt
                        -- LEFT JOIN cnae_emp as cn on cn.cliente_unidades_id_unidade = clt.id_unidade
						LEFT JOIN base_territorialsindemp as bt on (JSON_CONTAINS(clt.cnae_unidade, CONCAT('{\"id\":',bt.classe_cnae_idclasse_cnae,'}'), '$') and bt.localizacao_id_localizacao1 = clt.localizacao_id_localizacao)
						LEFT JOIN localizacao as loc on loc.id_localizacao = bt.localizacao_id_localizacao1
						LEFT JOIN cliente_matriz as mt on mt.id_empresa = clt.cliente_matriz_id_empresa
						LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = mt.cliente_grupo_id_grupo_economico
                        LEFT JOIN sind_emp as sinde on sinde.id_sinde = bt.sind_empregados_id_sinde1

						WHERE sinde.id_sinde is not null and {$where}

						group by sinde.id_sinde

						order by sinde.id_sinde asc
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);
				$this->logger->debug($this->db->error);

				$listaMultiplos = [];
				$optEmp = "";

				while ($obj = $result->fetch_object()) {
					$this->logger->debug($obj);
					// $this->logger->debug($listaMultiplos);

					// if ($obj) {
						$sqlEmp = "SELECT 
										id_sinde
										,razaosocial_sinde
										,sigla_sinde
										,cnpj_sinde
										,logradouro_sinde
										,email1_sinde
										,fone1_sinde
										,site_sinde
									FROM 
										sind_emp
									WHERE id_sinde = {$obj->id_sinde}
						";

						$this->logger->debug($sqlEmp);

						$resultEmp = mysqli_query($this->db, $sqlEmp);

						$objEmp = $resultEmp->fetch_object();

						$this->logger->debug($objEmp);

						$cnpj = formatCnpjCpf($objEmp->cnpj_sinde);
						$optEmp .= "<option value='{$objEmp->id_sinde}'>{$objEmp->sigla_sinde} / {$cnpj} / {$objEmp->razaosocial_sinde}</option>";
					// 	array_push($listaMultiplos, $obj->id_sinde);
					// }
					
				}

				$response['response_data']['lista_laboral_local'] = $optEmp;
				$this->logger->debug($optEmp);


				//OBTEM LISTA DE OPTIONS PATRONAL
				$sql = "SELECT
							sp.id_sindp,
							bt.localizacao_id_localizacao1,
							loc.municipio,
							loc.uf,
							loc.regiao
						FROM cliente_unidades as clt
						-- LEFT JOIN cnae_emp as cn on cn.cliente_unidades_id_unidade = clt.id_unidade
						LEFT JOIN base_territorialsindpatro as bt on (JSON_CONTAINS(clt.cnae_unidade, CONCAT('{\"id\":',bt.classe_cnae_idclasse_cnae,'}'), '$') and bt.localizacao_id_localizacao1 = clt.localizacao_id_localizacao)
						LEFT JOIN localizacao as loc on loc.id_localizacao = bt.localizacao_id_localizacao1
						LEFT JOIN cliente_matriz as mt on mt.id_empresa = clt.cliente_matriz_id_empresa
						LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = mt.cliente_grupo_id_grupo_economico
						LEFT JOIN sind_patr as sp on sp.id_sindp = bt.sind_patronal_id_sindp

						WHERE sp.id_sindp is not null and {$where}

						group by sp.id_sindp

						order by sp.id_sindp asc
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);

				$optPatr = "";
				$listaMultiplos = [];
				while ($obj = $result->fetch_object()) {

					// if (!in_array($obj->id_sindp, $listaMultiplos)) {
						$sqlPatr = "SELECT 
										id_sindp
										,razaosocial_sp
										,sigla_sp
										,cnpj_sp
										,logradouro_sp
										,email1_sp
										,fone1_sp
										,site_sp
									FROM 
										sind_patr
									WHERE 	id_sindp = {$obj->id_sindp}
						";

						$resultPatr = mysqli_query($this->db, $sqlPatr);

						$objPatr = $resultPatr->fetch_object();

						$cnpj = formatCnpjCpf($objPatr->cnpj_sp);
						$optPatr .= "<option value='{$objPatr->id_sindp}'>{$objPatr->sigla_sp} / {$cnpj} / {$objPatr->razaosocial_sp}</option>";
					// 	array_push($listaMultiplos, $obj->id_sindp);
					// }

					
				}

				$this->logger->debug($optPatr);

				$response['response_data']['lista_patronal_local'] = $optPatr;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	// function getSindicatosByCliente($data = null)
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

	// 		if ($response['response_status']['status'] == 1) {

	// 			$where = '';
	// 			$wbem = '';
	// 			$wbp = '';
	// 			if ($data['localidade'] != "") {
	// 				if (count($data['localidade']) > 1) {
	// 					$string = "";
	// 					foreach ($data['localidade'] as $value) {
	// 						$column = strstr($value, "+", true);
	// 						$content = substr(strstr($value, "+"), 1);

	// 						$string .= "'{$content}'" . ',';
	// 					}
	// 					$string = implode(",", array_filter(explode(",", $string)));
	// 					$where .= ($where != "" ? " AND loc.{$column} IN ({$string})" : " loc.{$column} IN ({$string})");
	// 				} else {
	// 					$string = "";
	// 					foreach ($data['localidade'] as $value) {
	// 						$column = strstr($value, "+", true);
	// 						$content = substr(strstr($value, "+"), 1);

	// 						$string .= "'{$content}'";
	// 					}

	// 					$where .= ($where != "" ? " AND loc.{$column} = {$string}" : " loc.{$column} = {$string}");
	// 				}
	// 			}
	// 			if ($data['cnaes'] != "") {
	// 				$inbem = implode(",", $data['cnaes']);
	// 				if ($data['localidade'] != "") {
	// 					$wbem .= " AND bem.classe_cnae_idclasse_cnae IN ({$inbem})";
	// 					$wbp .= " AND bp.classe_cnae_idclasse_cnae IN ({$inbem})";
	// 				} else {
	// 					$wbem .= " bem.classe_cnae_idclasse_cnae IN ({$inbem})";
	// 					$wbp .= " bp.classe_cnae_idclasse_cnae IN ({$inbem})";
	// 				}
	// 			}
	// 			$this->logger->debug($where);


	// 			//OBTEM LISTA DE OPTIONS LABORAL
	// 			$sql = "SELECT
	// 						sinde.id_sinde,
	// 						bem.localizacao_id_localizacao1,
	// 						loc.municipio,
	// 						loc.uf,
	// 						loc.regiao
	// 					FROM base_territorialsindemp as bem
	// 					LEFT JOIN sind_emp as sinde on sinde.id_sinde = bem.sind_empregados_id_sinde1
	// 					LEFT JOIN localizacao as loc on loc.id_localizacao = bem.localizacao_id_localizacao1

	// 					WHERE {$where}{$wbem}
	// 			";

	// 			$this->logger->debug($sql);

	// 			$result = mysqli_query($this->db, $sql);

	// 			$listaMultiplos = [];
	// 			$optEmp = "";

	// 			while ($obj = $result->fetch_object()) {

	// 				if (!in_array($obj->id_sinde, $listaMultiplos)) {
	// 					$sqlEmp = "SELECT 
	// 									id_sinde
	// 									,razaosocial_sinde
	// 									,sigla_sinde
	// 									,cnpj_sinde
	// 									,logradouro_sinde
	// 									,email1_sinde
	// 									,fone1_sinde
	// 									,site_sinde
	// 								FROM 
	// 									sind_emp
	// 								WHERE id_sinde = {$obj->id_sinde}
	// 					";

	// 					$resultEmp = mysqli_query($this->db, $sqlEmp);

	// 					$objEmp = $resultEmp->fetch_object();

	// 					$cnpj = formatCnpjCpf($objEmp->cnpj_sinde);
	// 					$optEmp .= "<option value='{$objEmp->id_sinde}'>{$objEmp->sigla_sinde} / {$cnpj} / {$objEmp->razaosocial_sinde}</option>";
	// 				}
	// 				array_push($listaMultiplos, $obj->id_sinde);
	// 			}

	// 			$response['response_data']['lista_laboral_local'] = $optEmp;


	// 			//OBTEM LISTA DE OPTIONS PATRONAL
	// 			$sql = "SELECT
	// 						sp.id_sindp,
	// 						bp.localizacao_id_localizacao1,
	// 						loc.municipio,
	// 						loc.uf,
	// 						loc.regiao
	// 					FROM base_territorialsindpatro as bp
	// 					LEFT JOIN sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp
	// 					LEFT JOIN localizacao as loc on loc.id_localizacao = bp.localizacao_id_localizacao1

	// 					WHERE {$where}{$wbp}
	// 			";

	// 			$this->logger->debug($sql);

	// 			$result = mysqli_query($this->db, $sql);

	// 			$optPatr = "";
	// 			$listaMultiplos = [];
	// 			while ($obj = $result->fetch_object()) {

	// 				if (!in_array($obj->id_sindp, $listaMultiplos)) {
	// 					$sqlPatr = "SELECT 
	// 									id_sindp
	// 									,razaosocial_sp
	// 									,sigla_sp
	// 									,cnpj_sp
	// 									,logradouro_sp
	// 									,email1_sp
	// 									,fone1_sp
	// 									,site_sp
	// 								FROM 
	// 									sind_patr
	// 								WHERE 	id_sindp = {$obj->id_sindp}
	// 					";

	// 					$resultPatr = mysqli_query($this->db, $sqlPatr);

	// 					$objPatr = $resultPatr->fetch_object();

	// 					$cnpj = formatCnpjCpf($objPatr->cnpj_sp);
	// 					$optPatr .= "<option value='{$objPatr->id_sindp}'>{$objPatr->sigla_sp} / {$cnpj} / {$objPatr->razaosocial_sp}</option>";
	// 				}

	// 				array_push($listaMultiplos, $obj->id_sindp);
	// 			}

	// 			$response['response_data']['lista_patronal_local'] = $optPatr;
	// 		} else {
	// 			$response = $this->response;
	// 		}
	// 	} else {
	// 		$response = $this->response;
	// 	}

	// 	$this->logger->debug($response['response_status']['status']);
	// 	return $response;
	// }

	function getLocalidadeByGrupo($data = null)
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

				if ($data['id_grupo']) {
					$where = " AND gp.id_grupo_economico = {$data['id_grupo']}";
				}else if($data['id_matriz']) {
					$ids = implode(",", array_filter($data['id_matriz']));
					$where = " AND ma.id_empresa IN ({$ids})";
				}else if($data['id_unidade']) {
					$ids = implode(",", array_filter($data['id_unidade']));
					$where = " AND cl.id_unidade IN ({$ids})";
				}

				$sql = "SELECT
							gp.id_grupo_economico,
							ma.id_empresa,
							cl.id_unidade,
							loc.municipio,
							loc.uf,
							loc.regiao
						FROM cliente_grupo as gp
						LEFT JOIN cliente_matriz as ma on ma.cliente_grupo_id_grupo_economico = gp.id_grupo_economico
						LEFT JOIN cliente_unidades as cl on cl.cliente_matriz_id_empresa = ma.id_empresa
						LEFT JOIN localizacao as loc on loc.id_localizacao = cl.localizacao_id_localizacao
						
						WHERE cl.id_unidade IS NOT NULL AND cl.localizacao_id_localizacao IS NOT NULL {$where}
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);

				$listaMultiplos = [];
				$optLocalizacao = '';
				$optUf = '';
				$optRegiao = '';
				while ($obj = $result->fetch_object()) {
					$this->logger->debug($obj);

					if (!in_array($obj->municipio, $listaMultiplos)) {
						$optLocalizacao .= "<option value='municipio+{$obj->municipio}'>{$obj->municipio} - {$obj->uf} / {$obj->regiao}</option>";
						array_push($listaMultiplos, $obj->municipio);
					}

					//UF
					if (!in_array($obj->uf, $listaMultiplos)) {
						$optUf .= "<option value='uf+{$obj->uf}'>{$obj->uf}</option>";
						array_push($listaMultiplos, $obj->uf);
					}

					if (!in_array($obj->regiao, $listaMultiplos)) {
						$optRegiao .= "<option value='regiao+{$obj->regiao}'>{$obj->regiao}</option>";
						array_push($listaMultiplos, $obj->regiao);
					}
				}

				$response['response_data']['optLocalizacao'] = $optLocalizacao;
				$response['response_data']['optUf'] = $optUf;
				$response['response_data']['optRegiao'] = $optRegiao;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
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

			if ($response['response_status']['status'] == 1) {
				$where = '';
				if($data['id_matriz']) {
					$ids = implode(",", array_filter($data['id_matriz']));
					$where .= " AND ma.id_empresa IN ({$ids})";
				}

				if($data['id_unidade']) {
					$ids = implode(",", array_filter($data['id_unidade']));
					$where .= " AND cl.id_unidade IN ({$ids})";
				}

				if(!$data['id_matriz'] && !$data['id_unidade']) {
					$where .= " AND gp.id_grupo_economico = {$data['id_grupo']}";
				}



				$sql = "SELECT
							gp.id_grupo_economico,
							ma.id_empresa,
							cl.id_unidade,
							loc.municipio,
							loc.uf,
							loc.regiao
						FROM cliente_grupo as gp
						LEFT JOIN cliente_matriz as ma on ma.cliente_grupo_id_grupo_economico = gp.id_grupo_economico
						LEFT JOIN cliente_unidades as cl on cl.cliente_matriz_id_empresa = ma.id_empresa
						LEFT JOIN localizacao as loc on loc.id_localizacao = cl.localizacao_id_localizacao
						
						WHERE cl.id_unidade IS NOT NULL AND cl.localizacao_id_localizacao IS NOT NULL {$where}
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);

				$listaMultiplos = [];
				$optLocalizacao = '';
				$optUf = '';
				$optRegiao = '';
				while ($obj = $result->fetch_object()) {
					$this->logger->debug($obj);
					
					if (!in_array($obj->municipio, $listaMultiplos)) {
						$optLocalizacao .= "<option value='municipio+{$obj->municipio}'>{$obj->municipio} - {$obj->uf} / {$obj->regiao}</option>";
						array_push($listaMultiplos, $obj->municipio);
					}

					//UF
					if (!in_array($obj->uf, $listaMultiplos)) {
						$optUf .= "<option value='uf+{$obj->uf}'>{$obj->uf}</option>";
						array_push($listaMultiplos, $obj->uf);
					}

					if (!in_array($obj->regiao, $listaMultiplos)) {
						$optRegiao .= "<option value='regiao+{$obj->regiao}'>{$obj->regiao}</option>";
						array_push($listaMultiplos, $obj->regiao);
					}
				}

				$response['response_data']['optLocalizacao'] = $optLocalizacao;
				$response['response_data']['optUf'] = $optUf;
				$response['response_data']['optRegiao'] = $optRegiao;
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
