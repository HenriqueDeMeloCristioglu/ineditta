<?php
session_start();
if (!$_SESSION) {
	echo "<script>document.location.href='http://localhost:8080/index.php'</script>";
}

/**
 * @author    {Rafael P. Cruz}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2021-07-02 15:39 ( v1.0.0 ) - 
	}
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


// inclui as classes do PHPMailer
require(__DIR__ . "/PHPMailer.php");
require(__DIR__ . '/SMTP.php');

setlocale(LC_TIME, 'pt_BR', 'pt_BR.utf-8', 'pt_BR.utf-8', 'portuguese');
date_default_timezone_set('America/Sao_Paulo');

class acompanhamento_cliente
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


	function getAcompanhamento($data = null)
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
			//$this->logger->debug(phpinfo());

			if ($response['response_status']['status'] == 1) {




				$sql = "
				select ac.idacompanhanto_cct
				, date_format(ac.data_inicial, \"%d/%m/%Y\") as data_inicial
				, date_format(ac.data_final, \"%d/%m/%Y\") as data_final
				, date_format(ac.ultima_atualizacao, \"%d/%m/%Y\") as ultima_atualizacao
				, ac.status
				, ua.nome_usuario
				, ac.fase
				, ac.data_base
				, sp.sigla_sp
				, se.sigla_sinde
				, cc.descricao_subclasse from acompanhanto_cct as ac
				inner join usuario_adm as ua on ua.id_user = ac.usuario_adm_id_usuario 
				inner join sind_patr as sp on sp.id_sindp = ac.sind_patr_id_sindp
				inner join sind_emp as se on se.id_sinde = ac.sind_emp_id_sinde
				inner join classe_cnae as cc on JSON_CONTAINS(
					ac.ids_cnaes,
					CONCAT('\"',cc.id_cnae, '\"'),'$')
				ORDER BY ac.fase;					
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;
					while ($obj = $resultsql->fetch_object()) {

						$sql2 = "SELECT * FROM acompanhamento_cliente WHERE id_acompanhamento_cct = {$obj->idacompanhanto_cct}";
						$resultsql2 = mysqli_query($this->db, $sql2);
						$obj2 = $resultsql2->fetch_object();
						$this->logger->debug($resultsql2);

						$icon = "";
						$edit = 0;
						if ($obj2) {
							$icon = '<i style="margin-left: 16px; color:green;" class="fa fa-edit"></i>';
							$edit = 1;
						}

						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a style="border: none;" data-toggle="modal" href="#myModalUpdate" onclick="getById(' . $obj->idacompanhanto_cct . ', ' . $edit . ');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i>' . $icon . '</a></td>'; //onclick="getByIdClausula( '.$obj->doc_sind_id_documento.');" 
						$html .= '<td class="title">';
						$html .= $obj->sigla_sp;
						$html .= '</td>';
						$html .= '<td class="desc">';
						$html .= $obj->sigla_sinde;
						$html .= '</td>';
						$html .= '<td class="desc">';
						$html .= $obj->data_base;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->fase;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->status;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->nome_usuario;
						$html .= '</td>';
						$html .= '</tr>';

						$this->logger->debug($obj);
					}

					$response['response_data']['listaPrincipal'] = $html;
				} else {
					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				// imap_close($mail_box);
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);

		return $response;
	}

	function getTimelineById($data = null)
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


				$sql = "SELECT * FROM acompanhamento_cliente WHERE id = {$data['id']} 
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					while ($obj = $resultsql->fetch_object()) {
						$this->logger->debug($obj);

						$response['response_data']['fase_up'] 	= $obj->fase;
						$response['response_data']['comentario_up'] 	= $obj->comentario;
					}

					$sqlOption = "SELECT fase_negociacao FROM fase_cct";
					$resultOption = mysqli_query($this->db, $sqlOption);

					$option = "";

					while ($obj3 = $resultOption->fetch_object()) {
						$option .= '<option value="';
						$option .= $obj3->fase_negociacao;
						$option .= '">';
						$option .= $obj3->fase_negociacao;
						$option .= '</option>';
					}

					$response['response_data']['optionFaseUpdate'] = $option;
				} else {

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao buscar os dados';
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


	function getById($data = null)
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
				

				select 
				ac.idacompanhanto_cct
				, ac.fase
				, ac.data_base
				, sp.sigla_sp
				, se.sigla_sinde
				, cc.descricao_subclasse from acompanhanto_cct as ac
				inner join sind_patr as sp on sp.id_sindp = ac.sind_patr_id_sindp
				inner join sind_emp as se on se.id_sinde = ac.sind_emp_id_sinde
				inner join classe_cnae as cc on cc.id_cnae = ac.classe_cnae_id_cnae
				WHERE ac.idacompanhanto_cct = {$data['id']}
				ORDER BY ac.fase;
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {
					$obj = $resultsql->fetch_object();
					$response['response_data']['id'] 	= $obj->idacompanhanto_cct;
					$response['response_data']['fase'] 	= $obj->fase;
					$fase = $obj->fase;
					$response['response_data']['db'] 	= $obj->data_base;
					$response['response_data']['sind'] 	= $obj->sigla_sp;
					$response['response_data']['emp'] 	= $obj->sigla_sinde;
					$response['response_data']['cnae'] 	= $obj->descricao_subclasse;

					$response['response_status']['status']       = 1;
					$response['response_status']['msg']          = 'Busca realizada com sucesso!';
					$this->logger->debug($obj);
				} else {

					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao buscar os dados';
				}

				$sqlOption = "SELECT fase_negociacao FROM fase_cct";
				$resultOption = mysqli_query($this->db, $sqlOption);

				$option = "";

				while ($obj3 = $resultOption->fetch_object()) {
					$option .= '<option value="';
					$option .= $obj3->fase_negociacao;
					$option .= '">';
					$option .= $obj3->fase_negociacao;
					$option .= '</option>';
				}

				$response['response_data']['optionFase'] = $option;





				$sql2 = "SELECT id, fase, comentario, criado_em, atualizado_em FROM acompanhamento_cliente WHERE id_acompanhamento_cct = {$data['id']};";
				$resultSql2 = mysqli_query($this->db, $sql2);


				$html = "";
				$html .= '
					<div class="col-md-12 timeline-box">
							<ul class="timeline">
				';

				while ($obj2 = $resultSql2->fetch_object()) {
					$this->logger->debug($obj2);


					$date = new DateTime($obj2->criado_em);
					$dateNew = date('d \d\e F \d\e Y - H:i', strtotime($obj2->criado_em));
					$dateUpdate = date('d \d\e F \d\e Y - H:i', strtotime($obj2->atualizado_em));

					$pattern = [
						"/(January)/",
						"/(February)/",
						"/(March)/",
						"/(April)/",
						"/(May)/",
						"/(June)/",
						"/(July)/",
						"/(August)/",
						"/(September)/",
						"/(October)/",
						"/(November)/",
						"/(December)/"
					];

					$dateNew = preg_replace($pattern, explode(" ", "janeiro fevereiro março abril maio junho julho agosto setembro outubro novembro dezembro"), $dateNew);
					$dateUpdate = preg_replace($pattern, explode(" ", "janeiro fevereiro março abril maio junho julho agosto setembro outubro novembro dezembro"), $dateUpdate);

					$html .= '
					<li class="timeline-primary">
						<a class="update-coment" data-toggle="modal" onclick="getTimelineById(' . $obj2->id . ')" data-dismiss="modal" href="#myModalUpdateTimeline"><div class="timeline-icon"><i class="fa fa-pencil"></i></div></a>
						<div class="timeline-body">
							<div class="timeline-header">
								<span class="date">' . $dateNew . '</span><br>
								<p class="sub-data">Ultima atualização: ' . $dateUpdate . '</p>
							</div>
							<div class="timeline-content">
								<h3>Fase: ' . $obj2->fase . '</h3>
								<p>' . $obj2->comentario . '</p>
							</div>
							<div class="timeline-footer">
								<!-- <a href="#" class="btn btn-default btn-sm pull-left">Read Full Story</a> -->
							</div>
						</div>
					</li>
					';
				}
				$html .= '
						</ul>
					</div>
				';



				$response['response_data']['timeline'] 	= $html;
			} else {
				$response = $this->response;
			}
		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);

		return $response;
	}



	function addAcompanhamento($data = null)
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

				$sql = "INSERT INTO acompanhamento_cliente
							(fase, comentario, id_acompanhamento_cct)
						VALUES
							('{$data['fase']}',
							'{$data['comentario']}',
							'{$data['id']}')
				";
				$this->logger->debug($sql);
				if (!mysqli_query($this->db, $sql)) {


					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao cadastrar novo registro';

					$this->logger->debug($sql);
					$this->logger->debug($this->db->error);
					$this->logger->debug($response);
				} else {


					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Cadastro realizado com sucesso!';
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

	function updateComentario($data = null)
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

				$sql = " UPDATE acompanhamento_cliente
							SET fase = '{$data['fase-update']}',
								comentario = '{$data['comentario-update']}'
						WHERE 
							id = {$data['id-update']};
						";
				$this->logger->debug($sql);
				if (!mysqli_query($this->db, $sql)) {
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao atualizar registro.';

					$this->logger->debug($this->db->error);
					$this->logger->debug($response);
				} else {

					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Registro atualizado com sucesso!';
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
