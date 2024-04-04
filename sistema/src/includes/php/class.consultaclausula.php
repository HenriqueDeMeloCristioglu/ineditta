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

ini_set("memory_limit", "800M");
ini_set("max_execution_time", "800");

include_once "helpers.php";
include_once __DIR__ . "/class.usuario.php";

require(__DIR__ . '/dompdf/autoload.inc.php');

use Dompdf\Dompdf;
use Dompdf\Options;



// require_once 'dompdf/autoload.inc.php'; //we've assumed that the dompdf directory is in the same directory as your PHP file. If not, adjust your autoload.inc.php i.e. first line of code accordingly.

class consultaclausula
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

	public $grupoEconLogado;

	public $idDocSindical;

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

	function getConsultaClausulaCampos($data = null)
	{

		$this->logger->debug("EIS A SESSION: ");
		$this->logger->debug($_SESSION['grupoecon']);

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

				$sql = "SELECT 
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

				$sql = "SELECT 
							id_localizacao
							,cod_pais
							,pais
							,cod_regiao
							,regiao
							,cod_uf
							,estado
							,uf
							,cod_municipio
							,municipio
						FROM 
							localizacao;				
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$htmlupdate = null;

					while ($obj = $resultsql->fetch_object()) {
						$html .= '<tr class="odd gradeX">';
						$html .= '<td><button onclick="selectLocalizacao( ' . $obj->id_localizacao . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$html .= '<td>';
						$html .= $obj->cod_pais;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->pais;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->cod_regiao;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->regiao;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->cod_uf;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->estado;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->uf;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->cod_municipio;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->municipio;
						$html .= '</td>';
						$html .= '</tr>';



						$htmlupdate .= '<tr class="odd gradeX">';
						$htmlupdate .= '<td><button onclick="selectLocalizacao( ' . $obj->id_localizacao . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->cod_pais;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->pais;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->cod_regiao;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->regiao;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->cod_uf;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->estado;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->uf;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->cod_municipio;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->municipio;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
					}

					$response['response_data']['listaLocalizacao'] 	= $html;
					$response['response_data']['listaLocalizacaoUpdate'] 	= $htmlupdate;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				$sql = "SELECT 
							id_cnae
							,divisao_cnae
							,descricao_divisão
                            ,subclasse_cnae
                            ,descricao_subclasse
							,categoria
						FROM 
							classe_cnae;								
				";
				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$htmlupdate = null;

					while ($obj = $resultsql->fetch_object()) {
						$html .= '<tr class="odd gradeX">';
						$html .= '<td><button onclick="selectCNAE( ' . $obj->id_cnae . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$html .= '<td>';
						$html .= $obj->id_cnae;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->divisao_cnae;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->descricao_divisão;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->subclasse_cnae;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->descricao_subclasse;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->categoria;
						$html .= '</td>';
						$html .= '</tr>';


						$htmlupdate .= '<tr class="odd gradeX">';
						$htmlupdate .= '<td><button onclick="selectCNAE( ' . $obj->id_cnae . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->id_cnae;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->divisao_cnae;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->descricao_divisão;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->subclasse_cnae;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->descricao_subclasse;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->categoria;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
					}

					$response['response_data']['listaCnaes'] 	= $html;
					$response['response_data']['listaCnaesU'] 	= $htmlupdate;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				//LISTA DATA BASE MES/ANO
				$sqlDate = "SELECT 
								DISTINCT database_doc
							FROM 
								doc_sind
							ORDER BY database_doc ASC		
				";

				$resultDate = mysqli_query($this->db, $sqlDate);
				$optDate = "<option value=''></option>";

				while ($objDate = $resultDate->fetch_object()) {
					$optDate .= "<option value='{$objDate->database_doc}'>{$objDate->database_doc}</option>";
				}

				$response['response_data']['data_base'] = $optDate;

				//LISTA TIPO DOC
				$sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'S' ORDER BY nome_doc ASC";
				$result = mysqli_query($this->db, $sqlTipo);

				$option = "<option value=''></option>";
				while ($obj = $result->fetch_object()) {
					$this->logger->debug($obj);

					$option .= '<option value="' . $obj->idtipo_doc . '">';
					$option .= $obj->nome_doc;
					$option .= '</option>';
				}

				$response['response_data']['nome_doc'] = $option;

				//LISTA DE ANOS CONFORME DATA BASE DE DOC SIND
				$today = (new DateTime('now'))->format("Y");
				$sqlAno = "SELECT
								DISTINCT substr(database_doc, 5, 6) as database_doc,
								DATE_FORMAT(NOW(), '%Y') as ano_atual
							FROM doc_sind
							where database_doc is not null and database_doc != ''

							ORDER BY database_doc desc limit 6

				";

				$resultAno = mysqli_query($this->db, $sqlAno);

				$optionAno = "<option value=''></option>";
				$lista = [];
				while ($objAno = $resultAno->fetch_object()) {
					$ano = $objAno->database_doc;
					if ($ano > $objAno->ano_atual) {
						(in_array($ano, $lista) ? "" : array_push($lista, $ano));
					}
				}

				$listaFiltrada = array_filter($lista);

				foreach ($listaFiltrada as $item) {
					$optionAno .= "<option value='{$item}'>{$item}</option>";
				}

				$response['response_data']['lista_ano'] = $optionAno;

				$this->logger->debug($optionAno);

				$sql = "SELECT  distinct
							cc.id_cnae
							,cc.descricao_subclasse
						FROM 
							classe_cnae as cc INNER JOIN
							 cnae_emp as ce ON ce.classe_cnae_idclasse_cnae = cc.id_cnae WHERE
                             ce.cliente_unidades_id_unidade IN (select cu.id_unidade from cliente_unidades  as cu INNER JOIN cliente_matriz as cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							 WHERE cu.cliente_matriz_id_empresa IN (SELECT id_empresa FROM 
							cliente_matriz WHERE cliente_grupo_id_grupo_economico = {$_SESSION['grupoecon']} ));								
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value="null"></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->id_cnae;
						$grupos .= '">';
						$grupos .= $obj->descricao_subclasse;
						$grupos .= '</option>';
					}

					$response['response_data']['cnaes_carre'] 	= $grupos;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				$sql = "SELECT 
							cu.id_unidade
							,cu.nome_unidade
							,cm.nome_empresa
						FROM 
							cliente_unidades as cu INNER JOIN cliente_matriz as cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							 WHERE cu.cliente_matriz_id_empresa IN (SELECT id_empresa FROM 
							cliente_matriz WHERE cliente_grupo_id_grupo_economico = {$_SESSION['grupoecon']} );							
				";

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

					$response['response_data']['unidade_carre'] 	= $grupos;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				$sql = "SELECT 
							id_empresa
							,nome_empresa
						FROM 
							cliente_matriz WHERE cliente_grupo_id_grupo_economico = {$_SESSION['grupoecon']};								
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value="null"></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->id_empresa;
						$grupos .= '">';
						$grupos .= $obj->nome_empresa;
						$grupos .= '</option>';
					}

					$response['response_data']['matriz_carre'] 	= $grupos;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				$sql = "select idtipo_doc, nome_doc, tipo_doc from tipo_doc WHERE processado = 'S' ORDER BY nome_doc ASC;			
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value="null"></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->idtipo_doc;
						$grupos .= '">';
						$grupos .= $obj->nome_doc;
						$grupos .= '</option>';
					}

					$response['response_data']['docs'] 	= $grupos;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				$sql = "SELECT 
							cu.id_unidade
							,cu.nome_unidade
							,cm.nome_empresa
							,cg.nome_grupoeconomico
						FROM 
							cliente_unidades cu INNER JOIN cliente_matriz cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							INNER JOIN cliente_grupo cg WHERE cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
							ORDER BY cg.nome_grupoeconomico;								
				";
				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$htmlupdate = null;

					while ($obj = $resultsql->fetch_object()) {
						$html .= '<tr class="odd gradeX">';
						$html .= '<td><button onclick="selectUMGE( ' . $obj->id_unidade . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$html .= '<td>';
						$html .= $obj->nome_unidade;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->nome_empresa;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->nome_grupoeconomico;
						$html .= '</td>';
						$html .= '</tr>';


						$htmlupdate .= '<tr class="odd gradeX">';
						$htmlupdate .= '<td><button onclick="selectUMGE( ' . $obj->id_unidade . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->nome_unidade;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->nome_empresa;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->nome_grupoeconomico;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
					}

					$response['response_data']['listaUMGE'] 	= $html;
					$response['response_data']['listaUMGEupdate'] 	= $htmlupdate;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}




				$sql = "SELECT 
							id_modulos
							,modulos
						FROM 
							modulos WHERE tipo = 'SISAP';								
				";
				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$htmlupdate = null;

					while ($obj = $resultsql->fetch_object()) {
						$html .= '<tr class="odd gradeX">';
						$html .= '<td>';
						$html .= $obj->modulos;
						$html .= '</td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Criar\');" value="1" id="checkCriar' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="checkConsultar' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="checkComentar' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="checkAlterar' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="checkExcluir' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="checkAprovar' . $obj->id_modulos . '"></td>';
						$html .= '</tr>';


						$htmlupdate .= '<tr class="odd gradeX">';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->modulos;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Criar\');" value="1" id="updatecheckCriar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="updatecheckConsultar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="updatecheckComentar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="updatecheckAlterar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="updatecheckExcluir' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="updatecheckAprovar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '</tr>';
					}

					$response['response_data']['listaSISAP'] 	= $html;
					$response['response_data']['listaSISAPupdate'] 	= $htmlupdate;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				$sql = "SELECT 
							id_modulos
							,modulos
						FROM 
							modulos WHERE tipo = 'Comercial';								
				";
				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$htmlupdate = null;

					while ($obj = $resultsql->fetch_object()) {
						$html .= '<tr class="odd gradeX">';
						$html .= '<td>';
						$html .= $obj->modulos;
						$html .= '</td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Criar\');" value="1" id="comcheckCriar' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="comcheckConsultar' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="comcheckComentar' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="comcheckAlterar' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="comcheckExcluir' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="comcheckAprovar' . $obj->id_modulos . '"></td>';
						$html .= '</tr>';


						$htmlupdate .= '<tr class="odd gradeX">';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->modulos;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Criar\');" value="1" id="comupdatecheckCriar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="comupdatecheckConsultar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="comupdatecheckComentar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="comupdatecheckAlterar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="comupdatecheckExcluir' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="comupdatecheckAprovar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '</tr>';
					}

					$response['response_data']['listaComercial'] 	= $html;
					$response['response_data']['listaComercialupdate'] 	= $htmlupdate;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

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
					$htmlupdate = null;

					while ($obj = $resultsql->fetch_object()) {


						$jor_sem = json_decode($obj->jornada_semanal);




						$html .= '<tr class="odd gradeX">';
						$html .= '<td><button onclick="selectJor( ' . $obj->id_jornada . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$html .= '<td>';
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


						$htmlupdate .= '<tr class="odd gradeX">';
						$htmlupdate .= '<td><button onclick="selectJor( ' . $obj->id_jornada . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->descricao;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= '<table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered">';
						$htmlupdate .= '<thead>';
						$htmlupdate .=  '<tr>';
						$htmlupdate .= 	'<th>Dia</th>';
						$htmlupdate .= 	'<th>Início</th>';
						$htmlupdate .= 	'<th>Fim</th>';
						$htmlupdate .= '</tr>';
						$htmlupdate .= '</thead>';
						$htmlupdate .= '<tbody>';
						$htmlupdate .= '<tr>';
						$htmlupdate .= '<td>';
						$htmlupdate .= '<h5>Segunda-feira</h5>';
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $jor_sem->SEGUNDA->INICIO;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $jor_sem->SEGUNDA->FIM;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
						$htmlupdate .= '<tr>';
						$htmlupdate .= '<td>';
						$htmlupdate .= '<h5>Terça-feira</h5>';
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $jor_sem->TERCA->INICIO;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $jor_sem->TERCA->FIM;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
						$htmlupdate .= '<tr>';
						$htmlupdate .= '<td>';
						$htmlupdate .= '<h5>Quarta-feira</h5>';
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $jor_sem->QUARTA->INICIO;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $jor_sem->QUARTA->FIM;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
						$htmlupdate .= '<tr>';
						$htmlupdate .= '<td>';
						$htmlupdate .= '<h5>Quinta-feira</h5>';
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $jor_sem->QUINTA->INICIO;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $jor_sem->QUINTA->FIM;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
						$htmlupdate .= '<tr>';
						$htmlupdate .= '<td>';
						$htmlupdate .= '<h5>Sexta-feira</h5>';
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $jor_sem->SEXTA->INICIO;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $jor_sem->SEXTA->FIM;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
						$htmlupdate .= '</tbody>';
						$htmlupdate .= '</table>';
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
					}

					$response['response_data']['listaJor'] 	= $html;
					$response['response_data']['listaJorupdate'] 	= $htmlupdate;
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

					$grupos = '<option value="null"></option>';
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


				//LISTA LOCALIDADE
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

				$sqlMuni = "SELECT
							distinct municipio
							
						FROM
							localizacao
						ORDER BY municipio ASC
				";
				$resultUf = mysqli_query($this->db, $sqlUf);
				$resultReg = mysqli_query($this->db, $sqlRegiao);
				$resultMun = mysqli_query($this->db, $sqlMuni);

				$opt = "<option value=''></option>";

				while ($obj = $resultReg->fetch_object()) {
					$opt .= "<option value='regiao+" . $obj->regiao . "'>" . $obj->regiao . "</option>";
				}

				while ($obj = $resultUf->fetch_object()) {
					$opt .= "<option value='uf+" . $obj->uf . "'>" . $obj->uf . "</option>";
				}

				while ($obj = $resultMun->fetch_object()) {
					$opt .= "<option value='municipio+" . $obj->municipio . "'>" . $obj->municipio . "</option>";
				}

				$response['response_data']['localizacao'] = $opt;



				$sql = "SELECT 
							id_localizacao
						    ,municipio
						FROM 
							localizacao WHERE id_localizacao IN
                            (SELECT localizacao_id_localizacao from cliente_unidades WHERE cliente_matriz_id_empresa IN (SELECT id_empresa FROM 
							cliente_matriz WHERE cliente_grupo_id_grupo_economico = {$_SESSION['grupoecon']}));								
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value="null"></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->id_localizacao;
						$grupos .= '">';
						$grupos .= $obj->municipio;
						$grupos .= '</option>';
					}

					$response['response_data']['localizacao_carre'] 	= $grupos;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				$sql = "
				SELECT 
				id_jornada
				,descricao
				FROM 
					jornada;				
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value="null"></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->id_jornada;
						$grupos .= '">';
						$grupos .= $obj->descricao;
						$grupos .= '</option>';
					}

					$response['response_data']['jornada'] 	= $grupos;
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




	function getConsultaClausula($data = null)
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
				$filtro = ";";
				$filter = [];

				$filter = array_merge($data, $filter);
				unset($filter["module"]);
				unset($filter["action"]);

				$this->logger->debug($data);



				$where = ""; //main.data_aprovacao IS NOT NULL

				if ($filter['nome_doc'] != "") {
					if (count($filter['nome_doc']) > 1) {
						$string = "";
						foreach ($filter['nome_doc'] as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$where .= ($where != "" ? " AND doc.tipo_doc_idtipo_doc IN ({$string})" : " doc.tipo_doc_idtipo_doc IN ({$string})");
					} else {

						$string = "";
						foreach ($filter['nome_doc'] as $value) {

							$string .= "'{$value}'";
						}

						$where .= ($where != "" ? " AND doc.tipo_doc_idtipo_doc = {$string}" : " doc.tipo_doc_idtipo_doc = {$string}");
					}
				}
				$this->logger->debug($where);
				if($filter['categoria'] != "") {
						$string = [];
						foreach ($filter['categoria'] as $value) {
							$id = '{"id":'.$value.'}';
	
							array_push($string, " JSON_CONTAINS(doc.cnae_doc, '{$id}', '$') ");
						}
	
						$newString = implode(" OR ", $string);
						$where .= $where != "" ? " AND {$newString}" : " {$newString}";
				}
				$this->logger->debug($where);
				if ($filter['localidade'] != "" ) {

					$string = [];
					foreach ($filter['localidade'] as $value) {
						$column = strstr($value, "+", true);
						$content = substr(strstr($value, "+"), 1);

						$id = '{"'.$column.'":"'.$content.'"}';

						array_push($string, " JSON_CONTAINS(doc.abrangencia, '{$id}', '$') ");

					}
					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND {$newString}" : " {$newString}";
				
				}
				$this->logger->debug($where);

				//FILTRO GRUPO ECONOMICO
				if ($filter['grupo'] != "" && $filter['grupo'] != 0) {
					$string = [];
					$id = '{"g":'.$filter['grupo'].'}';

					array_push($string, " JSON_CONTAINS(doc.cliente_estabelecimento, '{$id}', '$') ");
					

					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND  {$newString}" : " {$newString}";
				}

				$this->logger->debug($where);

				//FILTRO MATRIZ
				if ($filter['matriz'] != "") {
					$string = [];
					foreach ($filter['matriz'] as $value) {
						$id = '{"m":'.$value.'}';

						array_push($string, " JSON_CONTAINS(doc.cliente_estabelecimento, '{$id}', '$') ");
					}

					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND   {$newString}" : " {$newString}";
				}

				$this->logger->debug($where);

				//FILTRO UNIDADE
				if ($filter['unidade'] != "") {
					$string = [];
					foreach ($filter['unidade'] as $value) {
						$id = '{"u":'.$value.'}';

						array_push($string, " JSON_CONTAINS(doc.cliente_estabelecimento, '{$id}', '$') ");
					}

					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND   {$newString}" : "  {$newString}";
				}

				$this->logger->debug($where);

				$this->logger->debug($where);
				if ($filter['search'] != "") {

					$word = strtolower(trim($filter['search']));
					$wordUpper = strtoupper(trim($filter['search']));

					$where .= ($where != "" ? " AND main.tex_clau LIKE '%{$word}%' OR main.tex_clau LIKE '%{$wordUpper}%' collate utf8mb4_general_ci" : " main.tex_clau LIKE '%{$word}%' OR main.tex_clau LIKE '%{$wordUpper}%' collate utf8mb4_general_ci");
				}

				$this->logger->debug( $filter['sindicato_laboral']);

				if($filter['sindicato_laboral'] != "") {
					$string = [];
					foreach ($filter['sindicato_laboral'] as $value) {
						$id = '{"id":'.$value.'}';

						array_push($string, " JSON_CONTAINS(doc.sind_laboral, '{$id}', '$') ");
					}

					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND {$newString}" : " {$newString}";
				}

				$this->logger->debug( $where);

				if ($filter['sindicato_patronal'] != "") {
					$string = [];
					foreach ($filter['sindicato_patronal'] as $value) {

						$id = '{"id":'.$value.'}';

						array_push($string, " JSON_CONTAINS(doc.sind_patronal, '{$id}', '$') ");
					}
					
					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND {$newString}" : " {$newString}";
				}

				$this->logger->debug( $where);


				$this->logger->debug($where);
				$this->logger->debug($filter['data_base'][0] == "todos" ? "verdade" : "falso");
				if (!empty($filter['data_base']) && $filter['data_base'][0] != "todos") {

					if (count($filter['data_base']) > 1) {
						$string = "";
						foreach ($filter['data_base'] as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$where .= ($where != "" ? " AND doc.database_doc IN ({$string})" : " doc.database_doc IN ({$string})");
					} else {

						$string = "";
						foreach ($filter['data_base'] as $value) {

							$string .= "'{$value}'";
						}

						$where .= ($where != "" ? " AND doc.database_doc = {$string}" : " doc.database_doc = {$string}");
					}
				}

				// if(empty($filter['data_base'])) {
				// 	$where .= $where != "" ? " AND doc.database_doc = (SELECT max(dsi.database_doc) FROM clausula_geral as clau LEFT JOIN doc_sind as dsi on dsi.id_doc = clau.doc_sind_id_documento)" : " doc.database_doc = (SELECT max(dsi.database_doc) FROM clausula_geral as clau LEFT JOIN doc_sind as dsi on dsi.id_doc = clau.doc_sind_id_documento)";
				// }

				$this->logger->debug($where);

				if ($filter['grupo_clausula'] != "") {

					if (count($filter['grupo_clausula']) > 1) {
						$string = "";
						foreach ($filter['grupo_clausula'] as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$where .= ($where != "" ? " AND gc.idgrupo_clausula IN ({$string})" : " gc.idgrupo_clausula IN ({$string})");
					} else {

						$string = "";
						foreach ($filter['grupo_clausula'] as $value) {

							$string .= "'{$value}'";
						}

						$where .= ($where != "" ? " AND gc.idgrupo_clausula = {$string}" : " gc.idgrupo_clausula = {$string}");
					}
				}
				$this->logger->debug($where);

				if ($filter['lista_clausula'] != "") {

					if (count($filter['lista_clausula']) > 1) {
						$string = "";
						foreach ($filter['lista_clausula'] as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$where .= ($where != "" ? " AND ec.id_estruturaclausula IN ({$string})" : " ec.id_estruturaclausula IN ({$string})");
					} else {

						$string = "";
						foreach ($filter['lista_clausula'] as $value) {

							$string .= "'{$value}'";
						}

						$where .= ($where != "" ? " AND ec.id_estruturaclausula = {$string}" : " ec.id_estruturaclausula = {$string}");
					}
				}
				$this->logger->debug($where);

				$vigencia = $data['vigencia'];


				if ($data['vigencia']) {
					//vigencia
					$vigenIniDate = strstr($vigencia, ' -', true);
					$vigenIniDate = date("Y-m-d", strtotime(implode("-", explode("/", $vigenIniDate))));

					$separator = mb_strpos($vigencia, "-");
					$vigenEndDate = trim(substr($vigencia, $separator + 1));
					$vigenFinalDate = date("Y-m-d", strtotime(implode("-", explode("/", $vigenEndDate))));

					$where .= ($where != "" ? " AND doc.validade_inicial >= '{$vigenIniDate}' AND doc.validade_final <= '{$vigenFinalDate}'" : " doc.validade_inicial >= '{$vigenIniDate}' AND doc.validade_final <= '{$vigenFinalDate}'");
				}

				$whereInicio = "";
				if ($where == "") {
					$iniDate = (new DateTimeImmutable('now'))->sub(new DateInterval("P1Y"))->format("Y-m-d");
					$finalDate = (new DateTime('now'))->format('Y-m-d');
				}

				$wherSql = $where == "" ? "WHERE  main.liberado like 'S'" : "WHERE main.liberado like 'S' AND ";
				/** */
				if ($data['iddoc']) {
					$sql = "SELECT 
                                DISTINCT main.id_clau,
                                main.tex_clau,
                                gc.nome_grupo,
                                ec.nome_clausula,
                                JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla')) AS sigla_sp,
                                JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla')) AS sigla_sinde,
                                JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla')) AS denominacao_sp,
                                JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla')) AS denominacao_sinde,
                                DATE_FORMAT(doc.data_reg_mte,'%d/%m/%Y') AS data_registro,
                                doc.validade_final AS validade,
                                DATE_FORMAT(doc.data_aprovacao,'%d/%m/%Y') AS aprovacao,
                                main.aprovado,
                                CAST(JSON_EXTRACT(doc.sind_patronal, '$[0]')  AS UNSIGNED) AS sind_patr_id_sindp,
                                doc.database_doc,
                                doc.id_doc
                            FROM doc_sind AS doc
                            LEFT JOIN clausula_geral AS main ON main.doc_sind_id_documento = doc.id_doc
                            LEFT JOIN estrutura_clausula AS ec ON ec.id_estruturaclausula = main.estrutura_id_estruturaclausula 
                            LEFT JOIN grupo_clausula AS gc ON gc.idgrupo_clausula = ec.grupo_clausula_idgrupo_clausula 
						    WHERE main.liberado like 'S' and doc.id_doc = {$data['iddoc']} GROUP BY main.id_clau;
						";
				} else {
					$sql = "SELECT 
                                DISTINCT main.id_clau,
                                main.tex_clau,
                                gc.nome_grupo,
                                ec.nome_clausula,
								JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla')) AS sigla_sp,
                                JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla')) AS sigla_sinde,
                                JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla')) AS denominacao_sp,
                                JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla')) AS denominacao_sinde,
                                DATE_FORMAT(doc.data_reg_mte,'%d/%m/%Y') as data_registro,
                                doc.validade_final as validade,
                                DATE_FORMAT(doc.data_aprovacao,'%d/%m/%Y') as aprovacao,
                                main.aprovado,
                                CAST(JSON_EXTRACT(doc.sind_patronal, '$[0]')  AS UNSIGNED) AS sind_patr_id_sindp,
                                doc.database_doc,
                                doc.id_doc
                            FROM doc_sind AS doc
                            LEFT JOIN clausula_geral AS main on main.doc_sind_id_documento = doc.id_doc
                            LEFT JOIN estrutura_clausula as ec on ec.id_estruturaclausula = main.estrutura_id_estruturaclausula 
                            LEFT JOIN grupo_clausula as gc on gc.idgrupo_clausula = ec.grupo_clausula_idgrupo_clausula 
                            
						    {$wherSql} {$where} 
                            
                            GROUP BY main.id_clau
						
						";
				}




				$this->logger->debug($sql);
				$response['response_data']['sql'] 	= $sql;
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;

					$ids_cla = [];

					$table = [];

					$today = strtotime((new DateTime())->format("Y-m-d"));

					while ($obj = $resultsql->fetch_object()) {
						array_push($ids_cla, $obj->id_clau);

						$validadeFinal = (new DateTime($obj->validade))->format("d/m/Y");
						$validade = strtotime((new DateTime($obj->validade))->format("Y-m-d"));

						$list = new stdClass;
						$list->input = '<input type="checkbox" onclick="selectIds( ' . $obj->id_clau . ');" id="check' . $obj->id_clau . '">';
						$list->nome_grupo = $obj->nome_grupo;
						$list->nome_clausula = $obj->nome_clausula;
						$list->sigla_sinde = $obj->sigla_sinde;
						$list->sigla_sp = $obj->sigla_sp;
						$link = "<a style='color: #000;' href='#updateModal' class='texto-link' data-toggle='modal' onclick='selectIdsLink({$obj->id_clau})'>";
						$text = mb_substr($obj->tex_clau, 0, 200) . " ..." . "<i class='fa fa-external-link'></i></a>";
						// $substituido = $filter['search'] != "" ? str_ireplace($filter['search'], "<mark>{$filter['search']}</mark>", "<a style='color: #000;' data-mark='{$filter['search']}' id='link-{$obj->id_clau}' href='#updateModal' class='texto-link' data-toggle='modal' onclick='getByIdConsultaClausula({$obj->id_clau})'>" . $text) : $link . $text;
						// $list->texto = $filter['search'] != "" ? str_ireplace($filter['search'], "<mark>{$filter['search']}</mark>", "<a style='color: #000;' data-mark='{$filter['search']}' id='link-{$obj->id_clau}' href='#updateModal' class='texto-link' data-toggle='modal' onclick='getByIdConsultaClausula({$obj->id_clau})'>" . $text) : $link . $text;

						if ($filter['search'] != "") {
							// $hex = bin2hex($text);
							// $text = hex2bin($hex);

							// $low = strtolower(trim($filter['search']));
							// $upper = strtoupper(trim($filter['search']));
							// $cap = ucfirst(trim($filter['search']));
							// $this->logger->debug($low);
							// $this->logger->debug($upper);
							// $this->logger->debug($cap);
							// $substituido = str_replace($low, "<mark>{$low}</mark>", "<a style='color: #000;' data-mark='{$low}' id='link-{$obj->id_clau}' href='#updateModal' class='texto-link' data-toggle='modal' onclick='getByIdConsultaClausula({$obj->id_clau})'>" . $text);
							// $substituido = str_replace($upper, "<mark>{$upper}</mark>", $substituido);
							// $substituido = str_replace($cap, "<mark>{$upper}</mark>", $substituido);
							// $this->logger->debug($substituido);

							$search_word = $filter['search'];
							$prefix = "<mark>";
							$suffix = "</mark>";

							$substituido = preg_replace_callback('/' . preg_quote($search_word, '/') . '/iu', function ($matches) use ($prefix, $suffix) {
								return $prefix . $matches[0] . $suffix;
							}, $text);

							$list->texto = "<a style='color: #000;' data-mark='{$filter['search']}' id='link-{$obj->id_clau}' href='#updateModal' class='texto-link link-mark' data-toggle='modal' onclick='selectIdsLink({$obj->id_clau})'>" . $substituido;
						} else {
							$list->texto = $link . $text;
						}

						$list->denominacao_sp = $obj->denominacao_sp;
						$list->validade = $validade <= $today ? "<b style='color:red'>{$validadeFinal}</b>" : "<b style='color:#198754'>{$validadeFinal}</b>";

						$list->data_base = $obj->database_doc;
						array_push($table, $list);
					}

					$response['response_data']['sql'] 	= $sql;

					$response['response_data']['ids_cla'] 	= $ids_cla;

					$response['response_data']['list'] 	= $table;
					$response['response_data']['mark'] = "<mark>{$filter['search']}</mark>";
					$this->logger->debug($table);
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

	function getByIdConsultaClausula($data = null)
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

				$exibicao = "";

				$ids = $data['ids'] ? explode(" ", $data['ids']) : explode(",", $data['id_clau']); //verifica se foi clicado no texto da cláusula
				$userData = $data['user_data'];

				$this->logger->debug($userData);

				$user = (new usuario())->getUserData($userData)['response_data']['user_data'];

				$this->logger->debug($data);

				// if ($user->nivel != "Ineditta") {
				// 	$where = "AND grp.id_grupo_economico = {$user->id_grupoecon}";
				// } else {

				// 	if ($data['grupo']) {
				// 		$where = "AND grp.id_grupo_economico = {$data['grupo']}";
				// 	} else if ($data['matriz']) {
				// 		$matriz = implode(",", $data['matriz']);
				// 		$search = "mt.id_empresa IN ({$matriz})";
				// 	} else if ($data['unidade']) {
				// 		$unidade = implode(",", $data['unidades']);
				// 		$search = "cl.id_unidade IN ({$unidade})";
				// 	} else {
				// 		$where = "";
				// 	}

				// 	if ($search) {
				// 		$sql = "SELECT
				// 					gp.id_grupo_economico
				// 				FROM cliente_unidades as cl
				// 				LEFT JOIN cliente_matriz as mt on mt.id_empresa = cl.cliente_matriz_id_empresa
				// 				LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = mt.cliente_grupo_id_grupo_economico
				// 				WHERE {$search}
				// 		";

				// 		$result = mysqli_query($this->db, $sql);

				// 		$obj = $result->fetch_object();

				// 		$where = "AND grp.id_grupo_economico = {$obj->id_grupo_economico}";
				// 	}
				// }

				$cores = [
					"primary",
					"midnightblue"
				];

				$background = ["#4f8edc", "#34495e"];

				$contadorCores = 1;

				foreach ($ids as &$idx) {

					$html = '';
					$html2  = '';
					$html4  = '';
					$html5 = '';


					$id = $idx;

					if ($idx . '' == "") {
						$id = 0;
					} else {

						//exibicao

						$sql = "SELECT 
									distinct main.id_clau,
									main.tex_clau,
									gc.nome_grupo,
									ec.nome_clausula,
									ec.id_estruturaclausula,
									gc.idgrupo_clausula,
									JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].id')) AS id_sindp,
									JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla')) AS sigla_sp,
									JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].id')) AS id_sinde,
									JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla')) AS sigla_sinde,
									JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla')) AS denominacao_sp,
									JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla')) AS denominacao_sinde,
									JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].uf')) AS uf_sinde,
									JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].uf')) AS uf_sp,
									JSON_UNQUOTE(JSON_EXTRACT(doc.cnae_doc, '$[0].subclasse')) AS descricao_subclasse,
									DATE_FORMAT(doc.data_reg_mte,'%d/%m/%Y') as data_registro,
									DATE_FORMAT(doc.validade_inicial,'%d/%m/%Y') as validade_ini,
									DATE_FORMAT(doc.validade_final,'%d/%m/%Y') as validade,
									DATE_FORMAT(doc.data_aprovacao,'%d/%m/%Y') as data_pro,
									doc.cliente_estabelecimento AS unidades,
									'cnpj_unidade' as cnpj_unidade,
									'cod_sindcliente' as cod_sindcliente,
									'codigo_sp' as codigo_sp,
									'codigo_sinde' as codigo_sinde,
									td.nome_doc,
									main.aprovado,
									doc.database_doc,
									doc.id_doc,
									doc.database_doc as dataneg,
									ass.idassunto
								FROM doc_sind AS doc
								LEFT JOIN clausula_geral as main on main.doc_sind_id_documento = doc.id_doc
								LEFT JOIN estrutura_clausula as ec on ec.id_estruturaclausula = main.estrutura_id_estruturaclausula 
								LEFT JOIN grupo_clausula as gc on gc.idgrupo_clausula = ec.grupo_clausula_idgrupo_clausula 
								LEFT JOIN tipo_doc as td on td.idtipo_doc = doc.tipo_doc_idtipo_doc
								LEFT JOIN assunto as ass on ass.idassunto = main.assunto_idassunto
								
								WHERE main.id_clau = {$id};
					";


						$this->logger->debug($sql);
						if ($resultsql = mysqli_query($this->db, $sql)) {
							$obj = $resultsql->fetch_object();

							$this->logger->debug($contadorCores);

							$cor_primary = "primary";
							$cor_midnightblue = "midnightblue";

							$back_primary = "#4f8edc";
							$back_midnightblue = "#34495e";

							$this->logger->debug($cor_primary);

							$html = '
							<div class="panel panel-' . $cor_midnightblue . '"> 
								<div class="panel-heading" style="background-color: ' . $back_midnightblue . '">
									<h4>Sobre a Cláusula: ' . $obj->nome_clausula . ' </h4>
									
								</div> 
								<div class="panel-body collapse in">';
							$html .= '
							<div class="row">
								<div class="col-lg-8">
									<table class="table table-striped table-bordered">
										<thead>
											<tr>
												<th>Sindicato Laboral / UF</th>
											</tr>
										</thead>
										<tbody>
											<tr class="odd gradeX">
												<td> ' . $obj->denominacao_sinde . ' / ' . $obj->uf_sinde . ' </td>
											</tr>
										</tbody>
									</table>
								
							';

							$html .= '
							<table class="table table-striped table-bordered">
								<thead>
									<tr>
										<th>Sindicato Patronal / UF</th>
									</tr>
								</thead>
								<tbody>
									<tr class="odd gradeX">
										<td> ' . $obj->denominacao_sp . ' / ' . $obj->uf_sp . ' </td>
									</tr>
								</tbody>
							</table>
							
							';

							$html .= '
							<table class="table table-striped table-bordered">
								<thead>
									<tr>
										<th>Grupo da Cláusula</th>
										<th>Nome da Cláusula</th>
										<th>Documento</th>
										<th>Data Processamento</th>
									</tr>
								</thead>
								<tbody>
									<tr class="odd gradeX">
										<td>' . $obj->nome_grupo . '</td>
										<td>' . $obj->nome_clausula . '</td>
										<td>' . $obj->nome_doc . '</td>
										<td>' . $obj->data_pro . '</td>
									</tr>
								</tbody>
							</table>
							
							';

							$html .= '
							<table class="table table-striped table-bordered">
								<thead>
									<tr>
										<th>Validade Inicial</th>
										<th>Validade Final</th>
										<th>Data Base</th>
										<th>Atividade Econômica</th>
									</tr>
								</thead>
								<tbody>
									<tr class="odd gradeX">
										<td>' . $obj->validade_ini . '</td>
										<td>' . $obj->validade . '</td>
										<td>' . $obj->database_doc . '</td>
										<td>' . $obj->descricao_subclasse . '</td>
									</tr>
								</tbody>
							</table>
							</div>
							';

							$unidades = json_decode($obj->unidades);
							$tdFiliais = "";
							foreach ($unidades as $value) {
								$tdFiliais .= "<tr><td>{$value->nome_unidade}</td></tr>";
							}

							$html .= '
							<div class="col-lg-4">
								<div class="row filiais_abrangidas" style="max-height:440px; overflow:hidden;">
									<div class="col-lg-12">
										<table class="table table-striped table-bordered">
											<thead>
												<tr>
													<th>Estabelecimentos Abrangidos Pela Cláusula</th>
												</tr>
											</thead>
											<tbody>
												'.$tdFiliais.'
							';


							//Trazendo Notificação
							$sqlNote = "SELECT 
											nt.usuario_adm_id_user,
											us.nome_usuario,
											date_format(data_registro, '%d/%m/%Y - %H:%i') as data_registro,
											comentario,
											etiqueta
										FROM note_cliente nt
										LEFT JOIN usuario_adm as us on us.id_user = nt.usuario_adm_id_user
										WHERE id_tipo_comentario = '{$id}'
							";
							$resultNote = mysqli_query($this->db, $sqlNote);

							$lista = "";
							while ($objNote = $resultNote->fetch_object()) {
								if ($objNote) {
									$lista .= "
										<tr>
											<td>{$objNote->nome_usuario}</td>
											<td>{$objNote->data_registro}</td>
											<td>{$objNote->etiqueta}</td>
											<td>{$objNote->comentario}</td>
										</tr>
									";
								} else {
									$lista = "";
								}
							}

							$html5 = '
							<div class="panel panel-' . $cor_primary . '"> 
								<div class="panel-heading" style="background-color: ' . $back_primary . '">
									<h4>Comentários: </h4>
									 
                        		</div> 
								<div class="panel-body collapse in hei">
									<div class="row">
										<div class="col-lg-12">
											<table class="table table-striped">
												<thead>
													<th>Usuário</th>
													<th>Data</th>
													<th>Etiqueta</th>
													<th>Comentário</th>
												</thead>
												<tbody>
													' . $lista . '
												</tbody>
											</table>
										</div>
									</div>
								</div>
							</div>
							';

							$auxc = [];

							while ($obj2 = $resultsql->fetch_object()) {
								$this->logger->debug($obj2->valor);
								$cnpj = formatCnpjCpf($obj2->cnpj_unidade);

								if (!in_array($obj2->cnpj_unidade, $auxc)) {
									$html .= '<tr>';
									$html .= "<td>{$obj2->nome_unidade} | CNPJ: {$cnpj} | Cód Sind. Cliente: {$obj2->cod_sindcliente}</td>";
									$html .= '</tr>';
								}
								array_push($auxc, $obj2->cnpj_unidade);
							}

							//BUSCANDO DOCUEMNTOS PARA LEGISLAÇÃO
//							$sqlDoc = "SELECT
//											iddocumentos,
//											dc.descricao_documento,
//											tipo_doc_idtipo_doc,
//											tp.nome_doc,
//											da.estrutura_clausula_id_estruturaclausula
//										FROM documentos AS dc
//										LEFT JOIN documento_assunto AS da ON da.documentos_iddocumentos = dc.iddocumentos
//										LEFT JOIN tipo_doc AS tp ON tp.idtipo_doc = dc.tipo_doc_idtipo_doc
//
//										WHERE da.estrutura_clausula_id_estruturaclausula = {$obj->id_estruturaclausula}
//							";
                            $sqlDoc = "SELECT
                                            id_doc as iddocumentos, 
                                            descricao_documento
                                        FROM doc_sind as doc
                                        WHERE JSON_CONTAINS(doc.referencia, '\"{$obj->id_estruturaclausula}\"') AND modulo = 'COMERCIAL'";

							$this->logger->debug($sqlDoc);

							$resultDoc = mysqli_query($this->db, $sqlDoc);

							$legislacao = "";
							while ($objDoc = $resultDoc->fetch_object()) {
								$legislacao .= "<tr>";
								$legislacao .= "<td><a href='consulta_documentos.php?id_documento={$objDoc->iddocumentos}' title='Visualizar Documento'>{$objDoc->descricao_documento} <i class='fa fa-external-link'></i></a></td>";
								$legislacao .= "</tr>";
							}

							$this->logger->debug($sqlDoc);

							$html .= '
										</tbody>
									</table>
								</div>
							</div>
							<div class="row">
								<div class="col-lg-12">
									<table class="table table-striped table-bordered">
										<thead>
											<tr>
												<th>Legislação</th>
											</tr>
										</thead>
										<tbody>
											' . $legislacao . '
										</tbody>
									</table>
								</div>
							</div>
							</div>
							</div>
							</div>
							';


							$pattern = '/"/';
							$newText = preg_replace($pattern, "'", $obj->tex_clau);

							// $replacedSearch = str_replace(["<mark>", "</mark>"], ["", ""], $data['mark']);

							// $this->logger->debug($replacedSearch);

							// $textoFiltrado = $data['mark'] != "" ? str_ireplace($replacedSearch, $data['mark'], $newText) : $newText ;

							//
							if ($data['mark']) {
								$search_word = $data['mark'];
								$prefix = "<mark>";
								$suffix = "</mark>";

								$substituido = preg_replace_callback('/' . preg_quote($search_word, '/') . '/iu', function ($matches) use ($prefix, $suffix) {
									return $prefix . $matches[0] . $suffix;
								}, $newText);

								$textoFiltrado = $substituido;
							} else {
								$textoFiltrado = $newText;
							}

							//

							$this->logger->debug($data['mark']);
							$this->logger->debug($textoFiltrado);

							//LISTA DE ANOS CONFORME SIND
							$id = '{"id":'.$obj->id_sinde.'}';
							$sqlAno = "SELECT
											DISTINCT substr(database_doc, 5, 6) as database_doc,
											DATE_FORMAT(NOW(), '%Y') as ano_atual
										FROM doc_sind
										where database_doc is not null and database_doc != '' and JSON_CONTAINS(sind_laboral, '{$id}', '$')

										ORDER BY database_doc desc limit 6

							";
							$this->logger->debug($sqlAno);

							$result = mysqli_query($this->db, $sqlAno);
							$optionAno = "<option value=''></option>";
							$lista = [];
							while ($objAno = $result->fetch_object()) {
								$ano = $objAno->database_doc;
								if ($ano < substr(strstr($obj->database_doc, "/"), 1)) {
									array_push($lista, $ano);
								}
							}

							$listaFiltrada = array_filter($lista);

							foreach ($listaFiltrada as $item) {
								$optionAno .= "<option value='{$item}'>{$item}</option>";
							}

							$response['response_data']['lista_ano_do_sind'] = $optionAno;
							$this->logger->debug($optionAno);

							$html2 = '
								<div class="row">
									<div class="col-sm-12"> 
										<div class="panel panel-' . $cor_primary . '">
											<div class="panel-heading" style="background-color: ' . $back_primary . '">
												<h4>Texto da Cláusula: </h4>
												 
											</div>
											<div class="panel-body collapse in hei">
												<div style="margin-bottom:16px;">
													<a class="btn btn-' . $cor_primary . '" title="Adicionar Comentário" onclick="addComment(' . $obj->id_clau . ')" href="#modalComentario" data-toggle="modal" data-dismiss="modal"><i class="fa fa-comments-o"></i></a>
													<a class="btn btn-' . $cor_primary . '" onclick="copyText(`' . str_replace('"', '\'', $obj->tex_clau) . '`)"><i class="fa fa-copy"></i></a>
													<!--<a class="btn default-alt"  onclick="selV1(`' . str_replace('"', '\'', $obj->tex_clau) . '`)">Comparar (Versão 1)</a>
													<a class="btn default-alt"  onclick="selV2(`' . str_replace('"', '\'', $obj->tex_clau) . '`)">Comparar (Versão 2)</a> -->
													<a 
														id="clau' . $obj->id_clau . '"
														data-laboral="' . $obj->denominacao_sinde . '"
														data-patronal="' . $obj->denominacao_sp . '"
														data-grupo="' . $obj->nome_grupo . '"
														data-nome="' . $obj->nome_clausula . '"
														data-texto="' . $textoFiltrado . '"
														data-ano="' . substr(strstr($obj->database_doc, "/"), 1) . '"
														data-idsinde="' . $obj->id_sinde . '"
														data-idsindp="' . $obj->id_sindp . '"
														data-idgrupo="' . $obj->idgrupo_clausula . '"
														data-assunto="' . $obj->idassunto . '"
														data-ano-sind="'.$optionAno.'"
														
														onclick="abreComparacao(' . $obj->id_clau . ')" class="btn btn-' . $cor_primary . '" data-toggle="modal" href="#myModal" data-dismiss="modal">Comparar</a>
														<!-- <a href="geradorCsv.php?id_clau=' . $obj->id_clau . '&id_grupo_clau=' . $obj->idgrupo_clausula . '" class="btn btn-' . $cor_primary . '">Mapa Sindical</a> -->
												</div>
												<div>
													<p class="tex-clau">' . $textoFiltrado . '</p>
												</div>
											</div>
										</div>
									</div>
								</div>
							'; //abre_comparacao

							$html4 = "";
							$exibicao .= $html . $html2 . $html4 . $html5;

							$contadorCores++;

							$response['response_data']['sind_emp'] = $obj->sigla_sinde;
							$response['response_data']['sind_patr'] = $obj->sigla_sp;
							$response['response_data']['data_base'] = $obj->dataneg;
							$response['response_data']['categoria'] = $obj->descricao_subclasse;
						} else {
							$this->logger->debug($sql);
							$this->logger->debug($this->db->error);

							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = '';
						}
					}
				}

				$response['response_data']['exibicao'] 	= $exibicao;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);

		return $response;
	}

	function getConsultaClausulaByDoc($data = null)
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

				$exibicao = "";

				if ($data['data_base'] && $data['nome_doc'] && $data['sindicato_laboral']) {
					$dataBase = "'" . implode("','", $data['data_base']) . "'";
					$tipoDoc = implode(",", $data['nome_doc']);
					$laboral = implode(",", $data['sindicato_laboral']);

					$this->logger->debug($dataBase);
					$this->logger->debug($tipoDoc);
					$this->logger->debug($laboral);

					$query = "SELECT 
								id_clau
							FROM clausula_geral as cg
							LEFT JOIN doc_sind as doc on doc.id_doc = cg.doc_sind_id_documento
							LEFT JOIN tipo_doc as td on td.idtipo_doc = doc.tipo_doc_idtipo_doc
							LEFT JOIN doc_sind_sind_emp as doc_emp on doc_emp.doc_sind_id_doc = doc.id_doc 
							LEFT JOIN sind_emp as se on se.id_sinde = doc_emp.sind_emp_id_sinde
							WHERE td.idtipo_doc IN ({$tipoDoc}) 
								and doc.database_doc IN ({$dataBase}) 
								and se.id_sinde IN ({$laboral})
					";

					$this->logger->debug($query);

					$resultQuery = mysqli_query($this->db, $query);
					$ids = [];
					while ($myObj = $resultQuery->fetch_object()) {
						array_push($ids, $myObj->id_clau);
					}

					$this->logger->debug($ids);

					$userData = $data['user_data'];

					$this->logger->debug($userData);

					$user = (new usuario())->getUserData($userData)['response_data']['user_data'];

					$this->logger->debug($user);

					if ($user->nivel != "Ineditta") {
						$where = "AND grp.id_grupo_economico = {$user->id_grupoecon}";
					} else {

						if ($data['grupo']) {
							$where = "AND grp.id_grupo_economico = {$data['grupo']}";
						} else if ($data['matriz']) {
							$matriz = implode(",", $data['matriz']);
							$search = "mt.id_empresa IN ({$matriz})";
						} else if ($data['unidade']) {
							$unidade = implode(",", $data['unidades']);
							$search = "cl.id_unidade IN ({$unidade})";
						} else {
							$where = "";
						}

						if ($search) {
							$sql = "SELECT
										gp.id_grupo_economico
									FROM cliente_unidades as cl
									LEFT JOIN cliente_matriz as mt on mt.id_empresa = cl.cliente_matriz_id_empresa
									LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = mt.cliente_grupo_id_grupo_economico
									WHERE {$search}
							";

							$result = mysqli_query($this->db, $sql);

							$obj = $result->fetch_object();

							$where = "AND grp.id_grupo_economico = {$obj->id_grupo_economico}";
						}
					}

					$cores = [
						"primary",
						"midnightblue"
					];

					$background = ["#4f8edc", "#34495e"];

					$contadorCores = 1;
					$cont = 0;

					if ($dataBase && $tipoDoc && $laboral) {

						$textos = "";

						$listaDeTextos = "";
						$contador = 0;
						foreach ($ids as &$idx) {

							$html = '';
							$html2  = '';
							$html4  = '';
							$html5 = '';


							$id = $idx;

							if ($idx . '' == "") {
								$id = 0;
							} else {


								//exibicao
								$sql = "SELECT 
											distinct main.id_clau,
											main.tex_clau,
											gc.nome_grupo,
											ec.nome_clausula,
											ec.id_estruturaclausula,
											gc.idgrupo_clausula,
											JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].id')) AS id_sindp,
											JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla')) AS sigla_sp,
											JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].id')) AS id_sinde,
											JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla')) AS sigla_sinde,
											JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla')) AS denominacao_sp,
											JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla')) AS denominacao_sinde,
											JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].uf')) AS uf_sinde,
											JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].uf')) AS uf_sp,
											JSON_UNQUOTE(JSON_EXTRACT(doc.cnae_doc, '$[0].subclasse')) AS descricao_subclasse,
											DATE_FORMAT(doc.data_reg_mte,'%d/%m/%Y') as data_registro,
											DATE_FORMAT(doc.validade_inicial,'%d/%m/%Y') as validade_ini,
											DATE_FORMAT(doc.validade_final,'%d/%m/%Y') as validade,
											DATE_FORMAT(doc.data_aprovacao,'%d/%m/%Y') as data_pro,
											'cnpj_unidade' as cnpj_unidade,
											'cod_sindcliente' as cod_sindcliente,
											'codigo_sp' as codigo_sp,
											'codigo_sinde' as codigo_sinde,
											td.nome_doc,
											main.aprovado,
											doc.database_doc,
											doc.id_doc,
											doc.database_doc as dataneg,
											ass.idassunto
										FROM doc_sind AS doc
										LEFT JOIN clausula_geral as main on main.doc_sind_id_documento = doc.id_doc
										LEFT JOIN estrutura_clausula as ec on ec.id_estruturaclausula = main.estrutura_id_estruturaclausula 
										LEFT JOIN grupo_clausula as gc on gc.idgrupo_clausula = ec.grupo_clausula_idgrupo_clausula
										LEFT JOIN tipo_doc as td on td.idtipo_doc = doc.tipo_doc_idtipo_doc
										LEFT JOIN assunto as ass on ass.idassunto = main.assunto_idassunto
										WHERE main.id_clau = {$id} {$where};
								";


								$this->logger->debug($sql);
								if ($resultsql = mysqli_query($this->db, $sql)) {
									$obj = $resultsql->fetch_object();

									$cor_primary = "primary";
									$cor_midnightblue = "midnightblue";

									$back_primary = "#4f8edc";
									$back_midnightblue = "#34495e";

									$this->logger->debug($cor_primary);

									$listaDeTextos .= '
									
											<div class="row">
												<div class="col-lg-12">
													<p data-texto="' . $obj->tex_clau . '" class="left-text" style="margin: 8px 0;" id="left' . $contador . '">' . $obj->tex_clau . '</p>
												</div>
											</div>
									';

									$html = '
									<div class="panel panel-' . $cor_midnightblue . '"> 
										<div class="panel-heading" style="background-color: ' . $back_midnightblue . '">
											<h4>Dados do Documento: ' . $obj->nome_doc . ' </h4>
											
										</div> 
										<div class="panel-body collapse in">';
									$html .= '
									<div class="row">
										<div class="col-lg-12">
											<table class="table table-striped table-bordered">
												<thead>
													<tr>
														<th>Sindicato Laboral / UF</th>
													</tr>
												</thead>
												<tbody>
													<tr class="odd gradeX">
														<td> ' . $obj->denominacao_sinde . ' / ' . $obj->uf_sinde . ' </td>
													</tr>
												</tbody>
											</table>
										
									';

									$html .= '
									<table class="table table-striped table-bordered">
										<thead>
											<tr>
												<th>Sindicato Patronal / UF</th>
											</tr>
										</thead>
										<tbody>
											<tr class="odd gradeX">
												<td> ' . $obj->denominacao_sp . ' / ' . $obj->uf_sp . ' </td>
											</tr>
										</tbody>
									</table>
									
									';

									$html .= '
									<table class="table table-striped table-bordered">
										<thead>
											<tr>
												<th>Validade Inicial</th>
												<th>Validade Final</th>
												<th>Data Base</th>
												<th>Atividade Econômica</th>
												<th>Data Processamento</th>
											</tr>
										</thead>
										<tbody>
											<tr class="odd gradeX">
												<td>' . $obj->validade_ini . '</td>
												<td>' . $obj->validade . '</td>
												<td>' . $obj->database_doc . '</td>
												<td>' . $obj->descricao_subclasse . '</td>
												<td>' . $obj->data_pro . '</td>
											</tr>
										</tbody>
									</table>
									</div>
									';

									//Trazendo Notificação
									$sqlNote = "SELECT 
													nt.usuario_adm_id_user,
													us.nome_usuario,
													date_format(data_registro, '%d/%m/%Y - %H:%i') as data_registro,
													comentario,
													etiqueta
												FROM note_cliente nt
												LEFT JOIN usuario_adm as us on us.id_user = nt.usuario_adm_id_user
												WHERE id_tipo_comentario = '{$id}'
									";
									$resultNote = mysqli_query($this->db, $sqlNote);

									$lista = "";
									while ($objNote = $resultNote->fetch_object()) {
										if ($objNote) {
											$lista .= "
												<tr>
													<td>{$objNote->nome_usuario}</td>
													<td>{$objNote->data_registro}</td>
													<td>{$objNote->etiqueta}</td>
													<td>{$objNote->comentario}</td>
												</tr>
											";
										} else {
											$lista = "";
										}
									}

									if ($lista != "") {
										$html5 = '
											<div class="panel panel-' . $cor_primary . '"> 
												<div class="panel-heading" style="background-color: ' . $back_primary . '">
													<h4>Comentários: </h4>
													
												</div> 
												<div class="panel-body collapse in hei">
													<div class="row">
														<div class="col-lg-12">
															<table class="table table-striped">
																<thead>
																	<th>Usuário</th>
																	<th>Data</th>
																	<th>Etiqueta</th>
																	<th>Comentário</th>
																</thead>
																<tbody>
																	' . $lista . '
																</tbody>
															</table>
														</div>
													</div>
												</div>
											</div>
										';
									} else {
										$html5 = "";
									}

									$auxc = [];

									while ($obj2 = $resultsql->fetch_object()) {
										$this->logger->debug($obj2->valor);
										$cnpj = formatCnpjCpf($obj2->cnpj_unidade);
									}
									//BUSCANDO DOCUEMNTOS PARA LEGISLAÇÃO
                                    $sqlDoc = "SELECT
                                            id_doc as iddocumentos, 
                                            descricao_documento
                                        FROM doc_sind as doc
                                        WHERE JSON_CONTAINS(doc.referencia, '\"{$obj->id_estruturaclausula}\"') AND modulo = 'COMERCIAL'";


									$this->logger->debug($sqlDoc);

									$resultDoc = mysqli_query($this->db, $sqlDoc);

									$legislacao = "";
									while ($objDoc = $resultDoc->fetch_object()) {
										$legislacao .= "<tr>";
										$legislacao .= "<td><a href='consulta_documentos.php?id_documento={$objDoc->iddocumentos}' title='Visualizar Documento'>{$objDoc->descricao_documento} <i class='fa fa-external-link'></i></a></td>";
										$legislacao .= "</tr>";
									}

									$this->logger->debug($sqlDoc);

									$pattern = '/"/';
									$newText = preg_replace($pattern, "'", $obj->tex_clau);

									$secondPattern = "/<.*?>/";
									$newText = preg_replace($secondPattern, " ", $newText);
									$btnCompara = '
										<a
										id="doc' . $obj->id_doc . '"
										data-laboral="' . $obj->denominacao_sinde . '"
										data-patronal="' . $obj->denominacao_sp . '"
										data-ano="' . substr(strstr($obj->database_doc, "/"), 1) . '"
										data-idsinde="' . $obj->id_sinde . '"
										data-idsindp="' . $obj->id_sindp . '"
										data-assunto="' . $obj->idassunto . '"
										
										onclick="abreComparacaoDocumento(' . $obj->id_doc . ')" data-toggle="modal" href="#myModalDocumento" data-dismiss="modal">Selecione o documento para comparar</a>'; //class="btn btn-'.$cor_primary.'"
									$html2 = '
										<div class="row">
											<div class="col-sm-12"> 
												<div class="panel panel-' . $cor_primary . '">
													<div class="panel-heading" style="background-color: ' . $back_primary . '">
														<h4>Cláusula: ' . $obj->nome_clausula . ' - Grupo: ' . $obj->nome_grupo . ' </h4>
														
													</div>
													<div class="panel-body collapse in hei">
														<div style="margin-bottom:16px;">
															<a class="btn btn-' . $cor_primary . '" title="Adicionar Comentário" onclick="addComment(' . $obj->id_clau . ')" href="#modalComentario" data-toggle="modal" data-dismiss="modal"><i class="fa fa-comments-o"></i></a>
															<a class="btn btn-' . $cor_primary . '" onclick="copyText(`' . str_replace('"', '\'', $obj->tex_clau) . '`)"><i class="fa fa-copy"></i></a>
															<!--<a class="btn default-alt"  onclick="selV1(`' . str_replace('"', '\'', $obj->tex_clau) . '`)">Comparar (Versão 1)</a>
															<a class="btn default-alt"  onclick="selV2(`' . str_replace('"', '\'', $obj->tex_clau) . '`)">Comparar (Versão 2)</a> -->
															
																<!--<a href="geradorCsv.php?id_clau=' . $obj->id_clau . '&id_grupo_clau=' . $obj->idgrupo_clausula . '" class="btn btn-' . $cor_primary . '">Mapa Sindical</a>-->
														</div>
														<div>
															<p class="tex-clau">' . $obj->tex_clau . '</p>
														</div>
													</div>
												</div>
											</div>
										</div>
									'; //abre_comparacao

									// $html4 = "";
									if ($cont == 0) {
										$exibicao .= $html2 . $html4 . $html5;
										$cont += 1;
									} else {
										$exibicao .= $html2 . $html4 . $html5;
									}

									$contadorCores++;
									$contador++;

									$response['response_data']['btn_compara'] = $btnCompara;
									$response['response_data']['sind_emp'] = $obj->sigla_sinde;
									$response['response_data']['sind_patr'] = $obj->sigla_sp;
									$response['response_data']['data_base'] = $obj->dataneg;
									$response['response_data']['categoria'] = $obj->descricao_subclasse;
									$response['response_data']['lista_textos'] = $listaDeTextos;
									$response['response_data']['cabecalho'] = $html;
								} else {
									$this->logger->debug($sql);
									$this->logger->debug($this->db->error);

									$response['response_status']['status']       = 0;
									$response['response_status']['error_code']   = $this->error_code . __LINE__;
									$response['response_status']['msg']          = '';
								}
							}
						}

						$response['response_data']['exibicao'] 	= $exibicao;
					}
				} else {
					$response['response_data']['falta_filtro'] 	= true;
				}
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		$this->logger->debug($response['response_data']['exibicao']);

		return $response;
	}

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

			//$gecc = $_SESSION['grupoecon'];
			$gecc = $data['gec'];

			if ($gecc == "0") {
				$gecc = "id_grupo_economico";
			}

			if ($response['response_status']['status'] == 1) {

				//LISTA GRUPO ECONOMICO
				$sql = "SELECT 
							id_grupo_economico
							,nome_grupoeconomico
							,logo_grupo
						FROM 
							cliente_grupo
							WHERE id_grupo_economico = {$gecc};				
				";

				$this->logger->debug($sql);

				$resultsql = mysqli_query($this->db, $sql);
				$htmlGrupo = null;

				while ($obj = $resultsql->fetch_object()) {
					$htmlGrupo .= "<option value='{$obj->id_grupo_economico}'>{$obj->nome_grupoeconomico}</option>";
				}

				$response['response_data']['listaGrupo'] 	= $htmlGrupo;


				//LISTA CLIENTE MATRIZ

				$gecc = $data['gec'];

				if ($gecc == "0") {
					$gecc = "cm.cliente_grupo_id_grupo_economico";
				}

				$sql = "SELECT 
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
						INNER JOIN cliente_grupo gp WHERE gp.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
						AND cm.cliente_grupo_id_grupo_economico = {$gecc};								
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

				$gecc = $data['gec'];

				if ($gecc == "0") {
					$gecc = "cm.cliente_grupo_id_grupo_economico";
				}


				$sql = "SELECT 
							cu.id_unidade AS id_unidade
							,cu.nome_unidade AS nome_unidade
							,cu.cnpj_unidade AS cnpj_unidade
                            ,DATE_FORMAT(cu.data_inclusao,'%d/%m/%Y') AS data_inclusao
                            ,cu.nome_empresa as nome_empresa
							,tuc.tipo_negocio as tipo_negocio
						FROM 
							cliente_unidades cu
						INNER JOIN tipounidade_cliente tuc on tuc.id_tiponegocio = cu.tipounidade_cliente_id_tiponegocio
						WHERE cu.cliente_grupo_id_grupo_economico = {$gecc};
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

				// $usuario = trim(strstr($data['usuario'], "-", true));

				$idd = $data['id_destino'];
				if(!$idd){
					$idd = 0;
				}

				$sql = "INSERT INTO note_cliente
							(tipo_comentario,
							id_tipo_comentario,
							tipo_usuario_destino,
							id_tipo_usuario_destino,
							tipo_notificacao,
							data_final,
							usuario_adm_id_user,
							etiqueta,
							comentario)
						VALUES
							('{$data['tipo_com']}',
							{$data['id_clausula']},
							'{$data['destino']}',
							{$idd},
							'{$data['tipo_note']}',
							'{$data_final}',
							{$data['usuario']},
							'{$data['etiqueta']}',
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
		return $response;
	}


	function getClausulaToCompare($data = null)
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
							cg.id_clau,
							cg.estrutura_id_estruturaclausula,
							cg.tex_clau,
							sinde.denominacao_sinde,
							sindp.denominacao_sp,
							est.nome_clausula
							
						FROM clausula_geral as cg
						LEFT JOIN doc_sind as doc on doc.id_doc = cg.doc_sind_id_documento
						LEFT JOIN doc_sind_sind_emp as demp on demp.doc_sind_id_doc = doc.id_doc
						LEFT JOIN doc_sind_sind_patr as dpt on dpt.doc_sind_id_doc = doc.id_doc
						LEFT JOIN sind_emp as sinde on sinde.id_sinde = demp.sind_emp_id_sinde
						LEFT JOIN sind_patr as sindp on sindp.id_sindp = dpt.sind_patr_id_sindp
						LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cg.estrutura_id_estruturaclausula
						LEFT JOIN grupo_clausula as gc on gc.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
						LEFT JOIN assunto as ass on ass.idassunto = cg.assunto_idassunto
						
						where est.nome_clausula = '{$data['nome_clausula']}' and gc.idgrupo_clausula = {$data['id_grupo']} and ass.idassunto = {$data['assunto']} and doc.database_doc LIKE '%{$data['ano']}%'
				"; //and sinde.id_sinde = {$data['id_sinde']} and sindp.id_sindp = {$data['id_sindp']}

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);

				$obj = $result->fetch_object();

				$pattern = '/"/';
				$newText = preg_replace($pattern, "'", $obj->tex_clau);

				$secondPattern = "/<.*?>/";
				$newText = preg_replace($secondPattern, " ", $newText);


				$response['response_data']['clausula'] = $obj ? mb_convert_encoding($newText, "UTF-8") : "";
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}


	function getClausulaData($data = null)
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
							cg.id_clau,
							cg.estrutura_id_estruturaclausula,
							cg.tex_clau,
							est.nome_clausula
							
						FROM clausula_geral as cg
						LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cg.estrutura_id_estruturaclausula

						WHERE cg.id_clau = {$data['id_clau']}
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);

				$obj = $result->fetch_object();

				$response['response_data']['clausula'] = $obj->nome_clausula;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
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
					word-break: keep-all
				}
		
				.img_box {
					position: absolute;
					z-index: 999;
					width: 100%;
					height: 100%;
					background-color: rgba(255, 255, 255, 0.7);
					display: none;
				}
		
				.img_load {
					position: absolute;
					top: 30%;
					right: 45%;
				}
		
				.tex-clau {
					text-align: justify;
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
					border: #0ce783 solid;
					border-width: 0px 0px 0px 0.5em;
					border-radius: 0.5em;
					font-family: sans-serif;
					font-size: 88%;
					line-height: 1.6;
					box-shadow: 2px 2px 2px #ddd;
					padding: 1em;
					margin: 0;
				}
		
				.nadaDeNovo {
					white-space: pre-wrap;
					background: #fff;
					border: #777 solid;
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
					background-color: #0ce783 !important;
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
		
				/* .wikEdDiffNoChange {
					display: none !important;
				} */
		
				/* .left-text:has( .wikEdDiffNoChange ), .right-text:has( .wikEdDiffNoChange ) {
					display: none !important;
				} */
		
				/* .picadiff .right,
				.picadiff .left {
					margin: 0;
					width: 100%;
				} */
		
				.picadiff .picadiff-content .right .equal {
					background-color: #fff;
					display: inline;
				}
		
				.picadiff .picadiff-content .right .insertion {
					background-color: lightcoral;
				}
		
				.picadiff .right .equal,
				.picadiff .left .equal {
					background-color: #fff;
					display: inline;
				}
		
				.picadiff .right .deletion,
				.picadiff .left .deletion {
					background-color: greenyellow;
				}
		
				.picadiff .right .insertion,
				.picadiff .left .insertion {
					background-color: lightcoral;
				}
		
				#page-content {
					min-height: 100% !important;
				}
		
				.texto-link:hover {
					text-decoration: underline;
					transition: all 0.3s ease-in-out;
				}
		
				.table.table-striped.table-bordered {
					margin-bottom: 0;
				}
		
				.filiais_abrangidas table tr.odd.gradeX {
					height: 41px;
				}
		
				#box_textos_2 textarea {
					margin-top: 8px;
				}
		
				.picadiff .right, .picadiff .left{
					width: 50%;
				}
		
				#sobre_documento {
					width: 100%;
				}
		
				#sobre_documento tr td {
					padding: 8px;
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
			$this->logger->debug($dompdf);
			// Render the HTML as PDF

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
		return $response;
	}

	function getVigencia($data = null)
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

			if ($this->response['response_status']['status'] == 1) {

				$string = [];
				$array = [];

				foreach ($data['sindicato'] as $value) {
					$id = '{"id":'.$value.'}';

					array_push($array, " JSON_CONTAINS(doc.sind_laboral, '{$id}', '$') ");
				}

				$newContains = implode(" OR ", $array);

				$sql = "SELECT
							doc.id_doc,
							doc.database_doc,
							tp.nome_doc,
							tp.idtipo_doc,
							substr(doc.database_doc, 5) as ano,
							DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y') inicial,
							DATE_FORMAT(doc.validade_final, '%d/%m/%Y') final,
							CONCAT(substr(doc.database_doc, 5), '(', DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y'), ' - ', DATE_FORMAT(doc.validade_final, '%d/%m/%Y'), ')') data_string
						FROM doc_sind as doc
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
						
						WHERE tp.idtipo_doc = '{$data['documento']}' AND {$newContains}
						GROUP BY database_doc
						ORDER BY ano DESC
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);

				$option = "";
				while ($obj = $result->fetch_object()) {
					$option .= "<option value='{$obj->inicial} - {$obj->final}'>{$obj->data_string}</option>";
				}

				$this->logger->debug($option);

				$response['response_data']['data_base_list'] = $option;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function getDocData($data = null)
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

			if ($this->response['response_status']['status'] == 1) {



				if ($data['data_base']) {
					$dataBase = "'" . implode("','", explode(',', $data['data_base'])) . "'";
          if (strpos($data['documento'], ',') !== false) {
            $tipoDoc = implode(",", explode(',', $data['documento']));
          } else {
              $tipoDoc = $data['documento'];
          }


					$sql = "SELECT
								doc.id_doc,
								doc.database_doc,
								tp.nome_doc,
								tp.idtipo_doc,
								substr(doc.database_doc, 5) as ano,
								DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y') inicial,
								DATE_FORMAT(doc.validade_final, '%d/%m/%Y') final,
								CONCAT(substr(doc.database_doc, 5), '(', DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y'), ' - ', DATE_FORMAT(doc.validade_final, '%d/%m/%Y'), ')') data_string,
								JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla')) AS patronal,
								JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla')) AS laboral,
								JSON_UNQUOTE(JSON_EXTRACT(doc.cnae_doc, '$[0].subclasse')) as cnae,
								DATE_FORMAT(cg.data_aprovacao, '%d/%m/%Y') AS liberacao
							FROM doc_sind as doc
							
							LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
							LEFT JOIN clausula_geral as cg on cg.doc_sind_id_documento = doc.id_doc
							
							WHERE tp.idtipo_doc IN ({$tipoDoc}) AND doc.database_doc IN ({$dataBase})
						
					";
				}

				if ($data['vigencia'] != "") {
					$dataIni = implode("-", array_reverse(explode("/", trim(strstr($data['vigencia'], "-", true)))));
					$dataFim = implode("-", array_reverse(explode("/", trim(substr(strstr($data['vigencia'], "-"), 1)))));

					$tipoDoc = $data['documento'];

					$sql = "SELECT
								doc.id_doc,
								doc.database_doc,
								tp.nome_doc,
								tp.idtipo_doc,
								substr(doc.database_doc, 5) as ano,
								DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y') inicial,
								DATE_FORMAT(doc.validade_final, '%d/%m/%Y') final,
								CONCAT(substr(doc.database_doc, 5), '(', DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y'), ' - ', DATE_FORMAT(doc.validade_final, '%d/%m/%Y'), ')') data_string,
								JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla')) AS patronal,
								JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla')) AS laboral,
								JSON_UNQUOTE(JSON_EXTRACT(doc.cnae_doc, '$[0].subclasse')) as cnae,
								DATE_FORMAT(cg.data_aprovacao, '%d/%m/%Y') AS liberacao
							FROM doc_sind as doc
							
							LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
							LEFT JOIN clausula_geral as cg on cg.doc_sind_id_documento = doc.id_doc
							
							WHERE tp.idtipo_doc = {$tipoDoc} AND doc.validade_inicial >= '{$dataIni}' AND doc.validade_final <= '{$dataFim}'
						
					";
				}




				$this->logger->debug($sql);

				$obj = mysqli_query($this->db, $sql)->fetch_object();

				$response['response_data']['nome_doc'] = $obj->nome_doc;
				$response['response_data']['laboral'] = $obj->laboral;
				$response['response_data']['patronal'] = $obj->patronal;
				$response['response_data']['cnae'] = $obj->cnae;
				$response['response_data']['vigencia'] = $obj->data_string;
				$response['response_data']['data_base'] = $obj->database_doc;
				$response['response_data']['liberacao'] = $obj->liberacao;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

	function getFuckingComparative($data = null)
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

			if ($this->response['response_status']['status'] == 1) {

				//OBTENDO FILTRO PARA PRIMEIRO DOCUMENTO
				$dataBase = is_array($data['data_base']) ?  "'" . implode(",", $data['data_base']) . "'" : "'" . $data['data_base'] . "'";
				$tipoDoc = is_array($data['nome_doc']) ? implode(",", $data['nome_doc']) : $data['nome_doc'];
				// $laboral = is_array($data['sindicato_laboral']) ? implode(",", $data['sindicato_laboral']) : $data['sindicato_laboral'];
				$laboral = $data['sindicato_laboral'];
        $ids = explode(',', $laboral);

				$this->logger->debug($dataBase);
				$this->logger->debug($tipoDoc);
				$this->logger->debug($laboral);

				//OBTEBDO FILTRO PARA SEGUNDO DOCUMENTO
				$dataIni = implode("-", array_reverse(explode("/", trim(strstr($data['vigencia'], "-", true)))));
				$dataFim = implode("-", array_reverse(explode("/", trim(substr(strstr($data['vigencia'], "-"), 1)))));

				//Primeiro e segundo documento: odio
				$array = [];
				foreach ($ids as $id) {
					array_push($array, ' JSON_CONTAINS(doc.sind_laboral, \'{"id": '.$id.'}\', \'$\') ');
				}
				$new = implode(" OR ", $array);
				
				$sql = "SELECT 
							id_doc,
    						(SELECT id_doc FROM doc_sind WHERE validade_inicial >= '{$dataIni}' AND validade_final <= '{$dataFim}' AND tipo_doc_idtipo_doc = '{$data['id_doc']}' AND {$new} LIMIT 1) AS doc_novo
						FROM doc_sind as doc
						LEFT JOIN tipo_doc as td on td.idtipo_doc = doc.tipo_doc_idtipo_doc
						LEFT JOIN doc_sind_sind_emp as doc_emp on doc_emp.doc_sind_id_doc = doc.id_doc 
						LEFT JOIN sind_emp as se on se.id_sinde = doc_emp.sind_emp_id_sinde
						WHERE td.idtipo_doc IN ({$tipoDoc})
							and doc.database_doc IN ({$dataBase})
							and {$new}
						LIMIT 1
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);

				while ($obj = $result->fetch_object()) {
					$velho = $obj->id_doc;
					$novo = $obj->doc_novo;
				}

				//F U C K I N G QUERY DO CHAT GPT: mais ódio
				$fckQuery = "SELECT  
								distinct t1.tex_clau AS velho,
								CASE WHEN t2.tex_clau IS NULL THEN NULL ELSE t2.tex_clau END AS novo,
								t1.assunto_idassunto,
								t2.assunto_idassunto AS assunto2,
								est.nome_clausula,
								gp.nome_grupo
							FROM clausula_geral t1
								LEFT JOIN clausula_geral t2 ON (t1.assunto_idassunto = t2.assunto_idassunto) AND t2.doc_sind_id_documento = {$novo}
								LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = t1.estrutura_id_estruturaclausula
								LEFT JOIN grupo_clausula as gp on gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
								WHERE t1.doc_sind_id_documento = {$velho}  group by (t1.assunto_idassunto)
							UNION
							SELECT 
								CASE WHEN t1.tex_clau IS NULL THEN NULL ELSE t1.tex_clau END AS velho,
								CASE WHEN t2.tex_clau IS NULL THEN NULL ELSE t2.tex_clau END AS novo,
								t1.assunto_idassunto,
								t2.assunto_idassunto AS assunto2,
								est.nome_clausula,
								gp.nome_grupo
							FROM clausula_geral t1
								RIGHT JOIN clausula_geral t2 ON (t1.assunto_idassunto = t2.assunto_idassunto) AND t1.doc_sind_id_documento = {$velho}
								RIGHT JOIN estrutura_clausula as est on est.id_estruturaclausula = t1.estrutura_id_estruturaclausula
								RIGHT JOIN grupo_clausula as gp on gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
								WHERE t2.doc_sind_id_documento = {$novo} AND t1.tex_clau IS NULL  group by (t2.assunto_idassunto)
				";

				$this->logger->debug($fckQuery);

				$fckResult = mysqli_query($this->db, $fckQuery);

				$listaTextos = [];
				$cont = 0;
				while ($fckObj = $fckResult->fetch_object()) {
					$this->logger->debug($fckObj->velho);
					$this->logger->debug($fckObj->novo);

					$newList = new stdClass();
					$newList->velho = !$fckObj->velho ? "" : $fckObj->velho;
					$newList->novo = !$fckObj->novo ? "" : $fckObj->novo;
					$newList->clausula = $fckObj->nome_clausula;
					$newList->grupo = $fckObj->nome_grupo;
					array_push($listaTextos, $newList);

					$cont++;
				}

				$response['response_data']['lista_textos'] = $listaTextos;
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
