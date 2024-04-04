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

// inclui as classes do PHPMailer
require(__DIR__ . '/PHPMailer.php');
require(__DIR__ . '/SMTP.php');


class usuarioadm
{

	// Retorno do construtor aa
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








	function getUsuarioAdmCampos($data = null)
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

				//LISTA GRUPO ECONOMICO
				$sql = "SELECT
							id_grupo_economico,
							nome_grupoeconomico
						FROM cliente_grupo
				";

				$result = mysqli_query($this->db, $sql);

				$optGrupo = "<option value='0'>-</option>";
				while ($obj = $result->fetch_object()) {

					$optGrupo .= '<option value="' . $obj->id_grupo_economico . '">';
					$optGrupo .= $obj->nome_grupoeconomico;
					$optGrupo .= '</option>';
				}

				$response['response_data']['grupo_economico'] = $optGrupo;



				//TABELA DE SUPERIOR

				$sql = "
					SELECT 
					id_user as id_usuario
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
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><button onclick="selectSup( ' . $obj->id_usuario . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$html .= '<td class="title">';
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


						$htmlupdate .= '<tr class="odd gradeX tbl-item">';
						$htmlupdate .= '<td><button onclick="selectSup( ' . $obj->id_usuario . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td class="title">';
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


				//LISTA EMPRESA
				$sess = $_SESSION['grupoecon'];
				if($sess == "cliente_grupo_id_grupo_economico"){
					$sess = 'ma.cliente_grupo_id_grupo_economico';
				}
				$sql = "SELECT
							clt.id_unidade,
							clt.nome_unidade as filial,
							ma.nome_empresa as matriz,
							gp.nome_grupoeconomico as grupo
						FROM cliente_unidades as clt
						LEFT JOIN cliente_matriz as ma on ma.id_empresa = clt.cliente_matriz_id_empresa
						LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = ma.cliente_grupo_id_grupo_economico
						WHERE ma.cliente_grupo_id_grupo_economico = {$sess};	
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);
				$html = "";
				while ($obj = $result->fetch_object()) {
					$html .= "<tr class='tbl-item'>";
					$html .= "<td><input type='checkbox' id='empresa{$obj->id_unidade}' onclick='selectEmpresa({$obj->id_unidade})'></td>";
					$html .= "<td>{$obj->grupo}</td>";
					$html .= "<td class='desc'>{$obj->matriz}</td>";
					$html .= "<td class='title'>{$obj->filial}</td>";
					$html .= "</tr>";
				}

				$response['response_data']['lista_empresa'] = $html;






				$sql = "
				SELECT 
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





				$sql = "
				select distinct divisao_cnae, descricao_divisão from classe_cnae							
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value="null"></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->divisao_cnae;
						$grupos .= '">';
						$grupos .= $obj->descricao_divisão;
						$grupos .= '</option>';
					}

					$response['response_data']['cnaes'] 	= $grupos;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}


				$sql = "
				SELECT 
							cu.id_unidade
							,concat(cu.nome_unidade,\" | CNPJ: \", cu.cnpj_unidade) as nome_unidade
							,cm.nome_empresa
							,cg.nome_grupoeconomico
						FROM 
							cliente_unidades cu INNER JOIN cliente_matriz cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							INNER JOIN cliente_grupo cg WHERE cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
							ORDER BY cg.nome_grupoeconomico;								
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value="null"></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->id_unidade;
						$grupos .= '">';
						$grupos .= $obj->nome_grupoeconomico;
						$grupos .= ', ';
						$grupos .= $obj->nome_empresa;
						$grupos .= ', ';
						$grupos .= $obj->nome_unidade;
						$grupos .= '</option>';
					}

					$response['response_data']['umge'] 	= $grupos;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}






				$sql = "
				SELECT 
							*
						FROM 
							grupo_clausula;								
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value="null"></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->idgrupo_clausula;
						$grupos .= '">';
						$grupos .= $obj->nome_grupo;
						$grupos .= '</option>';
					}

					$response['response_data']['gruc'] 	= $grupos;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}






				$sql = "
				SELECT 
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





				if ($_SESSION['tipo'] == "Ineditta") {
					$sql = "
					SELECT 
								id_modulos
								,modulos
							FROM 
								modulos WHERE tipo = 'SISAP';								
					";
				} else {
					$sql = "
					SELECT 
								id_modulos
								,modulos
							FROM 
								modulos WHERE tipo = 'SISAP' and id_modulos = 0;								
					";
				}

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$htmlupdate = null;

					while ($obj = $resultsql->fetch_object()) {
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td class="title">';
						$html .= $obj->modulos;
						$html .= '</td>';
						$html .= '<td><input class="form-check-input c' . $obj->id_modulos . '" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Criar\');" value="1" id="checkCriar' . $obj->id_modulos . '" ></td>';
						$html .= '<td><input class="form-check-input c' . $obj->id_modulos . '" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="checkConsultar' . $obj->id_modulos . '"></td>';
						$html .= '<td><input class="form-check-input c' . $obj->id_modulos . '" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="checkComentar' . $obj->id_modulos . '" ></td>';
						$html .= '<td><input class="form-check-input c' . $obj->id_modulos . '" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="checkAlterar' . $obj->id_modulos . '" ></td>';
						$html .= '<td><input class="form-check-input c' . $obj->id_modulos . '" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="checkExcluir' . $obj->id_modulos . '" ></td>';
						$html .= '<td><input class="form-check-input c' . $obj->id_modulos . '" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="checkAprovar' . $obj->id_modulos . '" ></td>';
						$html .= '<td><input class="form-check-input c' . $obj->id_modulos . '" type="checkbox" onclick="todosaddModSISAP(' . $obj->id_modulos . ');" value="1" id="checkTodos' . $obj->id_modulos . '"></td>';
						$html .= '</tr>';


						$htmlupdate .= '<tr class="odd gradeX tbl-item">';
						$htmlupdate .= '<td class="title">';
						$htmlupdate .= $obj->modulos;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td><input class="form-check-input cu' . $obj->id_modulos . '" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Criar\');" value="1" id="updatecheckCriar' . $obj->id_modulos . '"></td>';
						$htmlupdate .= '<td><input class="form-check-input cu' . $obj->id_modulos . '" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="updatecheckConsultar' . $obj->id_modulos . '" ></td>';
						$htmlupdate .= '<td><input class="form-check-input cu' . $obj->id_modulos . '" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="updatecheckComentar' . $obj->id_modulos . '" ></td>';
						$htmlupdate .= '<td><input class="form-check-input cu' . $obj->id_modulos . '" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="updatecheckAlterar' . $obj->id_modulos . '" ></td>';
						$htmlupdate .= '<td><input class="form-check-input cu' . $obj->id_modulos . '" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="updatecheckExcluir' . $obj->id_modulos . '" ></td>';
						$htmlupdate .= '<td><input class="form-check-input cu' . $obj->id_modulos . '" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="updatecheckAprovar' . $obj->id_modulos . '" ></td>';
						$htmlupdate .= '<td><input class="form-check-input cu' . $obj->id_modulos . '" type="checkbox" onclick="todosupdateModSISAP(' . $obj->id_modulos . ');" value="1" id="updatecheckTodos' . $obj->id_modulos . '"></td>';
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


				//att pls


				if ($_SESSION['tipo'] == "Ineditta") {
					$sql = "
				SELECT 
							id_modulos
							,modulos
						FROM 
							modulos WHERE tipo = 'Comercial';								
				";
				} else {
					$sql = "SELECT 
							m.*
						FROM 
							modulos as m WHERE m.id_modulos IN 
								( SELECT modulos_id_modulos FROM modulos_cliente as mc WHERE mc.cliente_matriz_id_empresa IN 
									(select id_empresa from cliente_matriz WHERE cliente_grupo_id_grupo_economico = {$_SESSION['grupoecon']}
										)) 
										AND tipo = 'Comercial';
				";
				}



				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					$htmlupdate = null;

					while ($obj = $resultsql->fetch_object()) {
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td class="title">';
						$html .= $obj->modulos;
						$html .= '</td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Criar\');" value="1" id="comcheckCriar' . $obj->id_modulos . '" disabled></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="comcheckConsultar' . $obj->id_modulos . '" disabled></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="comcheckComentar' . $obj->id_modulos . '" disabled></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="comcheckAlterar' . $obj->id_modulos . '" disabled></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="comcheckExcluir' . $obj->id_modulos . '" disabled></td>';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="comcheckAprovar' . $obj->id_modulos . '" disabled></td>';
						$html .= '<td><input class="form-check-input c' . $obj->id_modulos . '" type="checkbox" onclick="todosaddModComercial(' . $obj->id_modulos . ');" value="1" id="comcheckTodos' . $obj->id_modulos . '"></td>';
						$html .= '</tr>';


						$htmlupdate .= '<tr class="odd gradeX tbl-item">';
						$htmlupdate .= '<td class="title">';
						$htmlupdate .= $obj->modulos;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Criar\');" value="1" id="comupdatecheckCriar' . $obj->id_modulos . '" disabled></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="comupdatecheckConsultar' . $obj->id_modulos . '" disabled></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="comupdatecheckComentar' . $obj->id_modulos . '" disabled></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="comupdatecheckAlterar' . $obj->id_modulos . '" disabled></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="comupdatecheckExcluir' . $obj->id_modulos . '" disabled></td>';
						$htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="comupdatecheckAprovar' . $obj->id_modulos . '" disabled></td>';
						$htmlupdate .= '<td><input class="form-check-input cu' . $obj->id_modulos . '" type="checkbox" onclick="todosupdateModComercial(' . $obj->id_modulos . ');" value="1" id="comupdatecheckTodos' . $obj->id_modulos . '"></td>';
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




						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><button onclick="selectJor( ' . $obj->id_jornada . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
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


						$htmlupdate .= '<tr class="odd gradeX tbl-item">';
						$htmlupdate .= '<td><button onclick="selectJor( ' . $obj->id_jornada . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td class="title">';
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




				$sql = "
				SELECT distinct 
							cod_uf,uf
						FROM 
							localizacao ORDER BY uf ASC;									
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$grupos = '<option value="null"></option>';
					while ($obj = $resultsql->fetch_object()) {
						$grupos .= '<option value="';
						$grupos .= $obj->cod_uf;
						$grupos .= '">';
						$grupos .= $obj->uf;
						$grupos .= '</option>';
					}

					$response['response_data']['localizacao'] 	= $grupos;
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










	function getUsuarioAdm($data = null)
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

				$sess = $_SESSION['grupoecon'];
				if($sess == "cliente_grupo_id_grupo_economico"){
					$sess == 'id_grupoecon';
				}

				if ($_SESSION['tipo'] == "Ineditta") {
					$sql = "
					SELECT 
					id_user as id_usuario
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
				} else {
					$sql = "
					SELECT 
					id_user as id_usuario
                    ,nome_usuario
                    ,email_usuario
					,cargo
					,telefone
					,ramal
					,id_user_superior
					,departamento
					FROM 
						usuario_adm WHERE id_grupoecon = {$sess};				
				";
				}



				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;

					while ($obj = $resultsql->fetch_object()) {
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdUsuarioAdm( ' . $obj->id_usuario . ');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td class="title">';
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

	function getByIdUsuarioAdm($data = null)
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

			$this->logger->debug($connectdb);

			if ($response['response_status']['status'] == 1) {

				$sql = "
					SELECT 
                    id_user
					, nome_usuario
					, email_usuario
					, cargo
					, telefone
					, ramal
					, id_user_superior
					, departamento
					, foto
					, is_blocked
					, documento_restrito
					, tipo
					, nivel
					, id_jornada_jornada as jor
					, ids_localidade as loc
					, ids_cnae as cat
					, notifica_email as ne
					, notifica_whatsapp as nw
					, modulos_sisap
					, modulos_comercial
					, ids_matrizes as matrizes
					, id_grupoecon as gecon
					, ids_fmge as 	umge
					, ids_gruc as gruc
					, date_format(ausencia_inicio, \"%d/%m/%Y\") as dataini
					, date_format(ausencia_fim, \"%d/%m/%Y\") as datafim
					
						FROM 
							usuario_adm
						WHERE
                        id_user = {$data['id_usuario']};
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {
					$obj = $resultsql->fetch_object();
					$response['response_data']['id_usuario'] 	= $obj->id_user;
					$response['response_data']['nome_usuario'] 	= $obj->nome_usuario;
					$response['response_data']['email_usuario'] 	= $obj->email_usuario;
					$response['response_data']['cargo'] 	= $obj->cargo;
					$response['response_data']['jor'] 	= $obj->jor;
					$response['response_data']['cel'] 	= $obj->telefone;
					$response['response_data']['fone'] 	= $obj->ramal;
					$response['response_data']['depto'] 	= $obj->departamento;
					$response['response_data']['sup'] 	= $obj->id_user_superior;
					$response['response_data']['dataini'] 	= $obj->dataini;
					$response['response_data']['datafim'] 	= $obj->datafim;
					$response['response_data']['blo'] 	= $obj->is_blocked;
					$response['response_data']['dr'] 	= $obj->documento_restrito;
					$response['response_data']['ne'] 	= $obj->ne;
					$response['response_data']['nw'] 	= $obj->nw;
					$response['response_data']['modulos_sisap'] 	= $obj->modulos_sisap;
					$response['response_data']['modulos_comercial'] 	= $obj->modulos_comercial;
					$response['response_data']['cat'] 	= $obj->cat;
					$response['response_data']['loc'] 	= $obj->loc;
					$response['response_data']['umge'] 	= $obj->umge;
					$response['response_data']['filiais'] 	= is_array(json_decode($obj->umge)) ? " " . implode(" ", json_decode($obj->umge)) : "";
					$response['response_data']['gruc'] 	= $obj->gruc;
					$response['response_data']['tipo'] 	= $obj->tipo;
					$response['response_data']['nivel'] 	= $obj->nivel;
					$response['response_data']['foto'] 	= $obj->foto;
					$response['response_data']['gecon'] 	= $obj->gecon;
					$response['response_data']['matrizes'] 	= $obj->matrizes;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				//LISTA EMPRESA

				$geccc = $grupoecon;
				if($geccc == "cliente_grupo_id_grupo_economico"){
					$geccc = "ma.cliente_grupo_id_grupo_economico";
				}
				$sql = "SELECT
							clt.id_unidade,
							clt.cnpj_unidade as cnpj,
							clt.nome_unidade as filial,
							ma.nome_empresa as matriz,
							gp.nome_grupoeconomico as grupo,
							JSON_CONTAINS(
						(select ids_fmge from usuario_adm WHERE id_user = {$data['id_usuario']}),
						CONCAT('\"',clt.id_unidade, '\"'),'$') as batem
						FROM cliente_unidades as clt
						LEFT JOIN cliente_matriz as ma on ma.id_empresa = clt.cliente_matriz_id_empresa
						LEFT JOIN cliente_grupo as gp on gp.id_grupo_economico = ma.cliente_grupo_id_grupo_economico
						WHERE ma.cliente_grupo_id_grupo_economico = {$geccc};	
				";

				$result = mysqli_query($this->db, $sql);
				$html = "";
				$listaTable = [];
				while ($obj = $result->fetch_object()) {
					$html .= "<tr class='tbl-item'>";
					if ($obj->batem) {
						// $html .= "<td><input type='checkbox' id='empresa{$obj->id_unidade}' onclick='selectEmpresa({$obj->id_unidade})' checked></td>";
						$input = "<input type='checkbox' id='empresa{$obj->id_unidade}' onclick='selectEmpresa({$obj->id_unidade})' checked>";
					} else {
						// $html .= "<td><input type='checkbox' id='empresa{$obj->id_unidade}' onclick='selectEmpresa({$obj->id_unidade})'></td>";
						$input = "<input type='checkbox' id='empresa{$obj->id_unidade}' onclick='selectEmpresa({$obj->id_unidade})'>";
					}
					$html .= "<td>{$obj->grupo}</td>";
					$cnp = formatCnpjCpf($obj->cnpj);

					$html .= "<td>{$obj->matriz}</td>";
					$html .= "<td class='desc'>{$cnp}</td>";
					$html .= "<td class='title'>{$obj->filial}</td>";
					$html .= "</tr>";

					$newObj = new stdClass();
					$newObj->input = $input;
					$newObj->cnpj = $cnp;
					$newObj->grupo = $obj->grupo;
					$newObj->filial = $obj->filial;
					$newObj->matriz = $obj->matriz;

					array_push($listaTable, $newObj);
				}

				$response['response_data']['lista_empresa_up'] = $html;
				$response['response_data']['lista_table'] = $listaTable;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);

		return $response;
	}













	function getModulosDisp($data = null)
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

				$filiais = implode(",", (explode(" ", trim($data['filiais']))));

				if ($data['nivel-input'] == 'Ineditta') {
					$sql = "SELECT 
					m.*
				FROM 
					modulos as m WHERE tipo = 'Comercial'";
				} else {
					$sql = "SELECT 
							m.*
						FROM 
							modulos as m WHERE m.id_modulos IN 
								( SELECT modulos_id_modulos FROM modulos_cliente as mc WHERE mc.cliente_matriz_id_empresa IN 
									(select id_empresa from cliente_matriz WHERE id_empresa IN
										(select cliente_matriz_id_empresa from cliente_unidades WHERE id_unidade IN ({$filiais})))) 
										AND tipo = 'Comercial';
				";
				}



				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {
					$modulos_disp = [];

					while ($obj = $resultsql->fetch_object()) {
						$modu = new stdClass();
						$modu->id = $obj->id_modulos;
						$modu->criar = $obj->criar;
						$modu->consultar = $obj->consultar;
						$modu->comentar = $obj->comentar;
						$modu->alterar = $obj->alterar;
						$modu->excluir = $obj->excluir;
						$modu->aprovar = $obj->aprovar;
						array_push($modulos_disp, $modu);
					}

					$response['response_data']['modulos_disp'] 	= $modulos_disp;
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

	function getModulosDisp2($data = null)
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

				if ($data['nivel-input'] == 'Ineditta') {
					$sql = "SELECT 
					m.*
				FROM 
					modulos as m WHERE tipo = 'Comercial'";
				} else {
					$sql = "SELECT 
							m.*
						FROM 
							modulos as m WHERE m.id_modulos IN 
								( SELECT modulos_id_modulos FROM modulos_cliente as mc WHERE mc.cliente_matriz_id_empresa IN 
									(select id_empresa from cliente_matriz WHERE id_empresa IN
										(select cliente_matriz_id_empresa from cliente_unidades WHERE id_unidade IN ()))) 
										AND tipo = 'Comercial';
				";
				}



				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {
					$modulos_disp = [];

					while ($obj = $resultsql->fetch_object()) {
						$modu = new stdClass();
						$modu->id = $obj->id_modulos;
						$modu->criar = $obj->criar;
						$modu->consultar = $obj->consultar;
						$modu->comentar = $obj->comentar;
						$modu->alterar = $obj->alterar;
						$modu->excluir = $obj->excluir;
						$modu->aprovar = $obj->aprovar;
						array_push($modulos_disp, $modu);
					}

					$response['response_data']['modulos_disp'] 	= $modulos_disp;
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

















	function addUsuarioAdm($data = null)
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
				$pass = password_hash($data['senha-input'], PASSWORD_DEFAULT, ['cost' => 10]);
				$sai = $data['sisapadd-input'];
				if ($sai == '') {
					$sai = '[]';
				}
				$cai = $data['comercialadd-input'];
				if ($cai == '') {
					$cai = '[]';
				}

				$sup = $data['sup-input'];
				if ($sup == '') {
					$sup = 'null';
				}

				$grup = $data['grupo'];
				if ($grup == '') {
					$grup = 'null';
				}

				$sql = "insert into usuario_adm
				( foto
				,nome_usuario
				,email_usuario
				,cargo
				,telefone
				,ramal
				,id_user_superior
				,departamento
				,senha_adm
				,is_blocked
				,documento_restrito
				,ausencia_inicio
				,ausencia_fim
				,tipo
				,nivel
				,notifica_whatsapp
				,notifica_email
				,ids_fmge
				,ids_matrizes
				,id_grupoecon
				,id_jornada_jornada
				,ids_localidade
				,ids_cnae
				,ids_gruc
				,modulos_sisap
				,modulos_comercial)
			values
								('{$data['foto-input']}','{$data['nome-input']}', '{$data['email-input']}',
								 '{$data['cargo-input']}', '{$data['cel-input']}',
								 '{$data['fone-input']}', {$sup},
								 '{$data['depto-input']}',
								 '{$pass}',{$data['blo-input']},{$data['dr-input']}
								,STR_TO_DATE('{$data['dataini-input']}', '%d/%m/%Y')
								,STR_TO_DATE('{$data['datafim-input']}', '%d/%m/%Y'), '{$data['tipo-input']}'
								, '{$data['nivel-input']}', {$data['nw-input']}, {$data['ne-input']} , '{$data['umge-input']}', '{$data['matriz']}'
								, {$grup}, {$data['jor-input']}, '{$data['loc-input']}'
							, '{$data['cat-input']}', '{$data['gruc-input']}', '{$sai}', '{$cai}');
				";
				//AQUIIIII
				// select btse.sind_empregados_id_sinde1, cu.nome_unidade from base_territorialsindemp as btse INNER JOIN
				//  cliente_unidades as cu ON cu.localizacao_id_localizacao = btse.localizacao_id_localizacao1 INNER JOIN
				//  cnae_emp as ce ON ce.cliente_unidades_id_unidade = cu.id_unidade WHERE cu.id_unidade = 1341;
				// 				$this->logger->debug($sql);
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

	function updateUsuarioAdm($data = null)
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
				$superior = $data['sup-input'];
				$gecon = $data['grupou'];
				$jornad = $data['jor-input'];
				if (("" . $data['grupou']) == "") {
					$superior = 'null';
				}
				if (("" . $data['sup-input']) == "") {
					$superior = 'null';
				}
				if (("" . $data['jor-input']) == "") {
					$jornad = 'null';
				}
				if (("" . $data['grupou']) == "") {
					$gecon = 'null';
				}

				$sai = $data['sisapadd-input'];
				if ('' . $sai == '') {
					$sai = '[]';
				}
				$cai = $data['comercialadd-input'];
				if ('' . $cai == '') {
					$cai = '[]';
				}

				$filiais = $data['umge-input'];



				if ($data['foto-input'] == "~") {
					$sql = " UPDATE usuario_adm  
									SET  nome_usuario = '{$data['nome-input']}',
                                         email_usuario = '{$data['email-input']}',
										cargo = '{$data['cargo-input']}',	
										telefone = '{$data['cel-input']}',	
										ramal = '{$data['fone-input']}',	
										departamento = '{$data['depto-input']}',	
										id_user_superior = {$superior},
										is_blocked = {$data['blo-input']},
										documento_restrito = {$data['dr-input']},
										ausencia_inicio = STR_TO_DATE('{$data['dataini-input']}', '%d/%m/%Y'),
										ausencia_fim = STR_TO_DATE('{$data['datafim-input']}', '%d/%m/%Y')
										,tipo = '{$data['tipo-input']}'
										,nivel = '{$data['nivel-input']}'
										,notifica_whatsapp = {$data['nw-input']}
										,notifica_email = {$data['ne-input']}
										,ids_fmge = '{$filiais}'
										,ids_matrizes = '{$data['matrizu']}'
										,id_grupoecon = {$gecon}
										,ids_gruc = '{$data['gruc-input']}'
										,id_jornada_jornada = {$jornad}
										,ids_localidade = '{$data['loc-input']}'
										,ids_cnae = '{$data['cat-input']}'
										,modulos_sisap = '{$sai}'
										,modulos_comercial = '{$cai}'
									WHERE 
                                        id_user = {$data['id_usuario']};
						";
				} else {
					$sql = " UPDATE usuario_adm  
					SET  nome_usuario = '{$data['nome-input']}',
						 email_usuario = '{$data['email-input']}',	
						foto = '{$data['foto-input']}',
						cargo = '{$data['cargo-input']}',	
						telefone = '{$data['cel-input']}',	
						ramal = '{$data['fone-input']}',	
						departamento = '{$data['depto-input']}',	
						id_user_superior = {$superior},
						is_blocked = {$data['blo-input']},
						documento_restrito = {$data['dr-input']},
						ausencia_inicio = STR_TO_DATE('{$data['dataini-input']}', '%d/%m/%Y'),
						ausencia_fim = STR_TO_DATE('{$data['datafim-input']}', '%d/%m/%Y')
						,tipo = '{$data['tipo-input']}'
						,nivel = '{$data['nivel-input']}'
						,notifica_whatsapp = {$data['nw-input']}
						,notifica_email = {$data['ne-input']}
						,ids_fmge = '{$filiais}'
						,ids_matrizes = '{$data['matrizu']}'
						,id_grupoecon = {$gecon}
						,ids_gruc = '{$data['gruc-input']}'
						,id_jornada_jornada = {$jornad}
						,ids_localidade = '{$data['loc-input']}'
						,ids_cnae = '{$data['cat-input']}'
						,modulos_sisap = '{$sai}'
						,modulos_comercial = '{$cai}'
					WHERE 
						id_user = {$data['id_usuario']};
		";
				}


				$this->logger->debug($sql);
				if (!mysqli_query($this->db, $sql)) {

					mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao atualizar';

					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);
					$this->logger->debug($response);
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Atualizado com sucesso';
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
						WHERE cliente_grupo_id_grupo_economico = '{$data['id_grupo']}'
				";

				$result = mysqli_query($this->db, $sql);

				$opt = "<option value=''></option>";
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

				$matriz = json_decode($data['id_matriz']);

				$opt = "<option value=''></option>";
				for ($i = 0; $i < count($matriz); $i++) {
					$sql = "SELECT
							id_unidade,
							nome_unidade,
							cnpj_unidade,
							codigo_unidade,
							cod_sindcliente,
							regional
						FROM cliente_unidades
						WHERE cliente_matriz_id_empresa = '{$matriz[$i]}'
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
				$opt = "<option value=''></option>";
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

	function boasVindas($data = null)
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
					$mail->SMTPDebug = 1;

					// utilizar SMTP
					$mail->isSMTP();

					// habilita autenticação SMTP
					$mail->SMTPAuth = true;

					// servidor SMTP
					$mail->Host = 'smtp.gmail.com';

					// usuário, senha e porta do SMTP
					$mail->Username = 'no-reply@ineditta.com.br';
					$mail->Password = 'oysnduyjipawezbl';
					$mail->Port = 465;

					// tipo de criptografia: "tls" ou "ssl"
					$mail->SMTPSecure = 'ssl';

					$mensagem = utf8_decode("Bem-vindo!");

					$this->logger->debug("EMAIL STATUS REPONSE 3 " . "Bem-vindo!");
					// email e nome do remetente
					$mail->setFrom('no-reply@ineditta.com.br', "Acesso Ineditta");

					if ($enviar ==	0) {
						$enviar	= 1;
					}

					$mail->addAddress("{$data['email']}", '');

					$this->logger->debug("EMAIL STATUS REPONSE 4" . "{$data['email']}");

					//$mail->addReplyTo('muralha77de77concreto@gmail.com', 'ULTIMATE PHP');

					$mail->isHTML(true);

					// codificação UTF-8
					$mail->Charset = 'UTF-8';

					// assunto do email
					$mail->Subject = utf8_decode("Acesso Sistema Ineditta!");

					$this->logger->debug("EMAIL STATUS REPONSE 5 " . "Bem-vindo!");


					$mail->Body    = utf8_decode("

					<p>Prezado(a) Usuário(a),</p>

					<p>Encaminhamos abaixo o seu acesso para o Sistema Ineditta.</p>
					<p><strong>Link de acesso:</strong> <a target='_blank' href='http://localhost:8080/index.php'>http://localhost:8080/index.php</a></p>
					<p><strong>Usuário:</strong> {$data['username']}</p>
          <p><strong>E-mail:</strong> {$data['email']}</p>
					<p><strong>Senha:</strong> Ineditta123@</p>

					<p>Ao clicar no <strong>link</strong> acima, você acessa a página inicial onde valida por e-mail seu usuário, e só após efetua o seu primeiro acesso na página principal, o sistema exige que você altere a senha para uma de uso pessoal.</p>

					<p>O padrão da senha exige no mínimo 6 caracteres, sendo 1 letra e 1 número.</p>

					<p><strong>Observações:</strong> Para acessar o sistema recomendamos o navegador Google Chrome.</p>
          
          <p>Em caso de dúvidas, estamos à disposição.</p>

					<p>Atenciosamente,</p>

					<p><strong>Ineditta Consultoria Sindical</strong></p>


				");

					$this->logger->debug("EMAIL STATUS REPONSE 6" . "{$data['msg']}");

					$this->logger->debug($mail);

					$mail->send();

					$this->logger->debug("EMAIL STATUS REPONSE 7" . $mail);

					//echo 'Mensagem enviada com sucesso!' . PHP_EOL;

					$response['response_status']['status']      	= 1;
					$response['response_status']['error_code']  	= null;
					$response['response_status']['msg']         	= 'Disparos enviados com sucesso!';
				} catch (Exception $e) {
					//echo 'Falha ao enviar email.' . PHP_EOL;
					//echo 'Erro: ' . $mail->ErrorInfo . PHP_EOL;

					$this->logger->debug($mail->ErrorInfo . PHP_EOL);

					$response['response_status']['status']      	= 1;
					$response['response_status']['error_code']  	= null;
					$response['response_status']['msg']         	= 'Falha ao enviar email. Entre em contato com o administrador do sistema!';
				}
			} else {
				$response = $this->response;
			}

			$this->logger->debug($response['response_status']['status']);
			return $response;
		}
	}
}
