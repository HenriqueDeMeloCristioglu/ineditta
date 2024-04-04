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


class calendario{
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

	function __construct() {
		
		//Iniciando resposta padrão do construtor.
		$this->response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Classe ' . __CLASS__ . ' iniciada com sucesso.' ) );
		
		// Montando o código do erro que será apresentado
        $localizar  = array( strtolower(__DIR__), "/", "\\", ".php", ".");
        $substituir = array( "", "", "", "", "-" );
        $this->error_code = strtoupper( str_replace( $localizar, $substituir,  strtolower( __FILE__  ) ) ) . "-";
		
		// Declarando os caminhos principais do sistema.
		$localizar 	= array( "\\", "/includes/php" );	
		$substituir	= array( "/", "" );
		$this->path 		= str_replace( $localizar, $substituir, __DIR__);
		
		$fileLogApi = $this->path . '/includes/php/log4php/Logger.php';

		if( file_exists( $fileLogApi ) ){
			
			include_once( $fileLogApi );

			$fileLogConfig = $this->path . '/includes/config/config.log.xml';
			
			if( file_exists( $fileLogConfig ) ){
				//Informado as configuracoes do log4php
				Logger::configure( $fileLogConfig );	
				
				//Indica qual bloco do XML corresponde as configuracoes
				$this->logger = Logger::getLogger( 'config.log'  );
			}
			else{
				$this->response['response_status']['status'] 		= 0;
				$this->response['response_status']['error_code'] 	= $this->error_code . __LINE__;
				$this->response['response_status']['msg']			= "Não foi possível localizar as configurações do log.";
			}
		}
		else{
			$this->response['response_status']['status']     = 0;
			$this->response['response_status']['error_code'] = $this->error_code . __LINE__;
			$this->response['response_status']['msg']        = 'Não foi possível encontrar o plugins log4php.';
		}
		
		if( $this->response['response_status']['status'] == 1 ){
			
			$fileGetConfig = $this->path . "/includes/config/config.get.php";
			
			// Carregando as configuração do Mirrada
			if( file_exists( $fileGetConfig ) ){
				
				include_once( $fileGetConfig );
				
				$this->getconfig = new getconfig();
				
				if( $this->getconfig->response['response_status']['status'] == 0 ){
					$this->response = $this->getconfig->response;
				}
			}
			else{
				$this->response['response_status']['status']       = 0;
				$this->response['response_status']['error_code']   = $this->error_code . __LINE__;
				$this->response['response_status']['msg']          = 'Não foi possível localizar o arquivo de configuração (mirada-config).';
			}
		}
	}
	
	function connectdb(){
		
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			$qualitor_db = $this->getconfig->searchConfigDatabase( 'ineditta' );
				
			if( $qualitor_db['response_status']['status'] == 1 ){
				
				$parameters = $qualitor_db['response_data'];

				if( file_exists( $this->path . '/includes/php/db.mysql.php' ) ){
				
					include_once( $this->path . '/includes/php/db.mysql.php');

					// Criando o objeto de conexão com o banco de dados Qualitor
					$apidbmysql = new apidbmysql();

					$db = $apidbmysql->connection($parameters);
					
					if( $db['response_status']['status'] == 1 ){
						
						$this->db = $db['response_data']['connection'];
						
						$this->logger->debug( $db['response_data']['connection'] );
						
					}
					else{
						$response = $db;
					}
				}
				else{
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível encontrar o db.mysql.';
				}		
			}
			else{
				$response =  $qualitor_db;
			}
		}
		else{
			$response = $this->response;
		}
		
		return $response;
	}
	
	function getLists( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
 
			$this->logger->debug(  $connectdb );
 
			if( $response['response_status']['status'] == 1 ){
				
				//LISTA CLASSE CNAE (CATEGORIA)
				$sqlCat = 'SELECT * FROM classe_cnae';
				$resultCat = mysqli_query( $this->db, $sqlCat );

				$optcat = "";
				while ($objCat = $resultCat->fetch_object()) {
					$this->logger->debug(  $objCat );

					$optcat .= '<option value="'.$objCat->id_cnae.'">';
					$optcat .= $objCat->descricao_divisão;
					$optcat .= '</option>';

				}

				$response['response_data']['optionCategoria'] = $optcat;


				//LISTA ESTRUTURA CLAUSULA
				$sql = "SELECT * FROM estrutura_clausula";

				$this->logger->debug(  $sql );

				$resultsql = mysqli_query( $this->db, $sql );

				$clau = null;
				
				while($obj = $resultsql->fetch_object()){

					$clau .= '<option value="'.$obj->id_estruturaclausula.'">';
					$clau .= $obj->nome_clausula;
					$clau .= '</option>';
				}	

				$response['response_data']['clausulas'] = $clau;


				//LISTA GRUPO CLÁUSULAS
				$sql = "SELECT 
							* 
						FROM 
							grupo_clausula
						ORDER BY nome_grupo ASC
				";

				$resultsql = mysqli_query( $this->db, $sql );

				$group = null;

				while($obj = $resultsql->fetch_object()){

					$group .= '<option value="'.$obj->idgrupo_clausula.'">';
					$group .= $obj->nome_grupo;
					$group .= '</option>';
				}

				$response['response_data']['grupoClausulas'] = $group;



				
			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		return $response;
	}


	function addEvento( $data = null ){
 
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
  
			if( $response['response_status']['status'] == 1 ){		

				$this->logger->debug($data);

				if ($data['allDay'] == 'false') {
					$start = $data['inicio'] . " " . $data['time-Start'];
					$end = $data['termino'] . " " . $data['time-End'];

					$this->logger->debug($start);
					$this->logger->debug($end);
				}else {
					$this->logger->debug('allday true');
					$start = $data['inicio'];
					$end = $data['termino'];
				}

				
				

				$sql = "INSERT INTO calendario_geral
							(titulo,
							assunto,
							data_inicial,
							data_final,
							origem,
							color,
							dia_todo)
						VALUES
							('{$data['titulo']}',
							'{$data['assunto']}',
							'{$start}',
							'{$end}',
							'ineditta',
							'{$data['cor']}',
							'{$data['allDay']}')

				";

				$this->logger->debug($sql);

				if ($result = mysqli_query( $this->db, $sql )) {
					$response['response_status']['status']     = 1;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Registro salvo com sucesso!';
				}else {
					$this->logger->debug( $this->db->error );
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível salvar o filtro.';
				}


			}else{
				$response = $this->response;
			}

		}else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		return $response;
	}

	function getCalendario( $data = null ){
 
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
  
			if( $response['response_status']['status'] == 1){

				$sql = "SELECT
							id,
							doc_sind_id_doc,
							clausula_geral_id_clau,
							acompanhamento_cct_id,
							sind_dirpatro_id,
							sind_diremp_id
						FROM
							calendario_geral_novo
				";

				$resultCal = mysqli_query($this->db, $sql);

				
				$lista = [];
				while($objCal = $resultCal->fetch_object()) {

					$this->logger->debug( $objCal );

					if ($objCal->doc_sind_id_doc) {
						/**
						 * OBTENDO DOC_SIND
						 */
						$sqlDocsind = "SELECT 
											doc.id_doc,
											doc.validade_final,
											cg.id
										FROM doc_sind as doc
										LEFT JOIN calendario_geral_novo as cg on cg.doc_sind_id_doc = doc.id_doc
										WHERE doc.id_doc = {$objCal->doc_sind_id_doc}

						";

						$resultDoc = mysqli_query($this->db, $sqlDocsind);

						//GERA EVENTOS DOC_SIND
						$obj = $resultDoc->fetch_object();

						$this->logger->debug( $obj );
						$event = new stdClass();

						$eventDate = $obj->validade_final;

						$event->id = $obj->id;
						$event->classNames = ["docsind"];
						$event->title = "Vigência final documento sindical";
						$event->start = $eventDate;
						$event->end = $eventDate;
						$event->backgroundColor = "#4f8edc";
						$event->borderColor = "#4f8edc";

						array_push($lista, $event);
					}

					if ($objCal->sind_dirpatro_id) {
						/**
						 * OBTENDO DIRIGENTE PATRONAL
						 */

						$sqlDirPatr = "SELECT
											sp.id_diretoriap,
											sp.termino_mandatop,
											cg.id
										FROM sind_dirpatro as sp
										LEFT JOIN calendario_geral_novo as cg on cg.sind_dirpatro_id = sp.id_diretoriap
										WHERE sp.id_diretoriap = {$objCal->sind_dirpatro_id}
						";

						$resultDirPatr = mysqli_query($this->db, $sqlDirPatr);

						//GERA EVENTOS DIRIGENTE PATRONAL
						$objDirPatr = $resultDirPatr->fetch_object();
						
						$this->logger->debug( $objDirPatr );

						$event = new stdClass();

						$eventDate = $objDirPatr->termino_mandatop;

						$event->id = $objDirPatr->id;
						$event->classNames = ["dirpatro"];
						$event->title = "Fim Mandato Dir. Patronal";
						$event->start = $eventDate;
						$event->end = $eventDate;
						$event->backgroundColor = "#f89b1c";
						$event->borderColor = "#f89b1c";

						array_push($lista, $event);
					}

					if ($objCal->sind_diremp_id) {
						/**
						 * OBTENDO DIRIGENTE LABORAL
						 */

						$sqlDirEmp = "SELECT
											se.id_diretoriae,
											se.termino_mandatoe,
											cg.id
										FROM sind_diremp as se
										LEFT JOIN calendario_geral_novo as cg on cg.sind_diremp_id = se.id_diretoriae
										WHERE se.id_diretoriae = {$objCal->sind_diremp_id}
						";

						$resultDirEmp = mysqli_query($this->db, $sqlDirEmp);

						//GERA EVENTOS DIRIGENTE PATRONAL
						$objDirEmp = $resultDirEmp->fetch_object();
						
						$this->logger->debug( $objDirEmp );

						$event = new stdClass();

						$eventDate = $objDirEmp->termino_mandatoe;

						$event->id = $objDirEmp->id;
						$event->classNames = ["diremp"];
						$event->title = "Fim Mandato Dir. Laboral";
						$event->start = $eventDate;
						$event->end = $eventDate;
						$event->backgroundColor = "#198754";
						$event->borderColor = "#198754";

						array_push($lista, $event);
					}
				}


				

				$this->logger->debug( $lista );

				$response['response_data']['eventos'] = $lista;

			}else{
				$response = $this->response;
			}

		}else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		return $response;
	}


	function getById( $data = null ){
 
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
  
			if( $response['response_status']['status'] == 1 ){

				

				$sqlGeral = "SELECT
							*
						FROM
							calendario_geral_novo
						WHERE id = '{$data['id_evento']}'
				";

				$this->logger->debug($sqlGeral);

				$obj = mysqli_query($this->db, $sqlGeral)->fetch_object();

				if($obj->doc_sind_id_doc) {
					//DOCSIND
					$sql = "SELECT 
								tp.nome_doc,
								CONCAT(DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y'), ' - ', DATE_FORMAT(doc.validade_final, '%d/%m/%Y')) as periodo,
								cs.descricao_subclasse,
								IFNULL(GROUP_CONCAT(DISTINCT CONCAT(loc.municipio, '-', loc.uf)), GROUP_CONCAT(IFNULL( CONCAT(loc.municipio, '-', loc.uf),null))) as abrangencia,
								IFNULL(GROUP_CONCAT(DISTINCT sinde.denominacao_sinde), GROUP_CONCAT(IFNULL( sinde.denominacao_sinde,null))) as laboral,
								IFNULL(GROUP_CONCAT(DISTINCT sp.denominacao_sp), GROUP_CONCAT(IFNULL( sp.denominacao_sp,null))) as patronal
								
							FROM doc_sind as doc
							LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
							LEFT JOIN classe_cnae_doc_sind as cn on cn.doc_sind_id_doc = doc.id_doc
							LEFT JOIN classe_cnae as cs on cs.id_cnae = cn.classe_cnae_id_cnae
							LEFT JOIN abrang_docsind as abr on abr.doc_sind_id_documento = doc.id_doc
							LEFT JOIN localizacao as loc on loc.id_localizacao = abr.localizacao_id_localizacao
							LEFT JOIN doc_sind_sind_emp as de on de.doc_sind_id_doc = doc.id_doc
							LEFT JOIN doc_sind_sind_patr as dp on dp.doc_sind_id_doc = doc.id_doc
							LEFT JOIN sind_emp as sinde on sinde.id_sinde = de.sind_emp_id_sinde
							LEFT JOIN sind_patr as sp on sp.id_sindp = dp.sind_patr_id_sindp
							WHERE doc.id_doc = {$obj->doc_sind_id_doc}
					";

					$objDoc = mysqli_query($this->db, $sql)->fetch_object();

					$response['response_data']['nome_doc'] = $objDoc->nome_doc;
					$response['response_data']['cnae'] = $objDoc->descricao_subclasse;
					$response['response_data']['abrangencia'] = $objDoc->abrangencia;
					$response['response_data']['laboral'] = $objDoc->laboral;
					$response['response_data']['patronal'] = $objDoc->patronal;
					$response['response_data']['periodo'] = $objDoc->periodo;
				}


				if($obj->sind_dirpatro_id) {
					//DIRETORIA PATRONAL
					$sql = "SELECT
								CONCAT(DATE_FORMAT(inicio_mandatop, '%d/%m/%Y'), ' - ', DATE_FORMAT(termino_mandatop, '%d/%m/%Y')) as periodo,
								dirigente_p,
								funcao_p,
								cl.nome_unidade,
								sp.denominacao_sp

							FROM sind_dirpatro as dp
							LEFT JOIN cliente_unidades as cl on cl.id_unidade = dp.cliente_unidades_id_unidade
							LEFT JOIN sind_patr as sp on sp.id_sindp = dp.sind_patr_id_sindp
							WHERE dp.id_diretoriap = {$obj->sind_dirpatro_id}
					";

					$obj = mysqli_query($this->db, $sql)->fetch_object();

					$response['response_data']['nome'] = $obj->dirigente_p;
					$response['response_data']['role'] = $obj->funcao_p;
					$response['response_data']['periodo'] = $obj->periodo;
					$response['response_data']['sindicato'] = $obj->denominacao_sp;
					$response['response_data']['empresa'] = $obj->nome_unidade;
				
				}

				if($obj->sind_diremp_id) {
					//DIRETORIA LABORAL
					$sql = "SELECT
								CONCAT(DATE_FORMAT(inicio_mandatoe, '%d/%m/%Y'), ' - ', DATE_FORMAT(termino_mandatoe, '%d/%m/%Y')) as periodo,
								dirigente_e,
								funcao_e,
								cl.nome_unidade,
								sinde.denominacao_sinde
							
							FROM sind_diremp as de
							LEFT JOIN cliente_unidades as cl on cl.id_unidade = de.cliente_unidades_id_unidade
							LEFT JOIN sind_emp as sinde on sinde.id_sinde = de.sind_emp_id_sinde
							WHERE de.id_diretoriae = {$obj->sind_diremp_id}
					";

					$obj = mysqli_query($this->db, $sql)->fetch_object();

					$response['response_data']['nome'] = $obj->dirigente_e;
					$response['response_data']['role'] = $obj->funcao_e;
					$response['response_data']['periodo'] = $obj->periodo;
					$response['response_data']['sindicato'] = $obj->denominacao_sinde;
					$response['response_data']['empresa'] = $obj->nome_unidade;
				
				}
	

			}else{
				$response = $this->response;
			}

		}else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		return $response;
	}


	function getLocalidade( $data = null ){
 
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
			if( $response['response_status']['status'] == 1 ){
				if( empty( $this->db ) ){
					$connectdb = $this->connectdb();
					
					if( $connectdb['response_status']['status'] == 0 ){
						$response = $connectdb;
					}
				}
			}
  
			if( $response['response_status']['status'] == 1 && $data['localidade'] ){

				$term = $data['localidade'];

				$sql = "SELECT
							distinct {$term}
							
						FROM
							localizacao
						ORDER BY {$term} ASC

				";

				$this->logger->debug($sql);

				if ($result = mysqli_query( $this->db, $sql )) {
					$response['response_status']['status']     = 1;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Registro salvo com sucesso!';
				}else {
					$this->logger->debug( $this->db->error );
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível salvar o filtro.';
				}

				$opt = '';

				while ($obj=$result->fetch_object()) {
					$this->logger->debug($obj);

					$opt .= "<option value='{$obj->term}'>{$obj->$term}</option>";
				}

				$response['response_data']['localidade'] = $opt;

			}else{
				$response = $this->response;
			}

		}else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		return $response;
	}
	
}
?>
