<?php
/**
 * @author    {Rafael P. Cruz}
 * @package   {1.0.0}
 * @description	{ }
**/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


class tagclausulas{
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
	
	//Retorn Scrapy
	public $pdfPath;
	public $returnScrapy;

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
	
	function getTagClausulas( $data = null ){

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

				$sql = "SELECT 
							*
						FROM 
							estrutura_clausula;				
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					$list = null;
					$listUpdate = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdTagClausulas( '.$obj->id_estruturaclausula.');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td class="title">';
						$html .= $obj->nome_clausula;
						$html .= '</td>';
						$html .= '<td class="desc">';
						if ($obj->tipo_clausula == 'P') {
							$html .= "Parâmetro";
						}elseif ($obj->tipo_clausula == 'R') {
							$html .= "Resumo";
						}else {
							$html .= "Tabela";
						}
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->classe_clausula;
						$html .= '</td>';
						$html .= '</tr>';

						$list .= '<tr class="odd gradeX tbl-item">';
						$list .= '<td><button onclick="selectClausula('.$obj->id_estruturaclausula.');" data-toggle="modal" href="#myModalAddSinonimo" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$list .= '<td class="title">';
						$list .= $obj->nome_clausula;
						$list .= '</td>';
						$list .= '<td class="desc">'; 
						if ($obj->tipo_clausula == 'P') {
							$list .= "Parâmetro";
						}elseif ($obj->tipo_clausula == 'R') {
							$list .= "Resumo";
						}else {
							$list .= "Tabela";
						}
						$list .= '</td>';
						$list .= '<td>';
						$list .= $obj->classe_clausula;
						$list .= '</td>';
						$list .= '</tr>';

						$listUpdate .= '<tr class="odd gradeX tbl-item">';
						$listUpdate .= '<td><button onclick="selectClausulaUpdate('.$obj->id_estruturaclausula.');" data-toggle="modal" href="#myModalUpdate" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$listUpdate .= '<td class="title">';
						$listUpdate .= $obj->nome_clausula;
						$listUpdate .= '</td>';
						$listUpdate .= '<td class="desc">';
						if ($obj->tipo_clausula == 'P') {
							$listUpdate .= "Parâmetro";
						}elseif ($obj->tipo_clausula == 'R') {
							$listUpdate .= "Resumo";
						}else {
							$listUpdate .= "Tabela";
						}
						$listUpdate .= '</td>';
						$listUpdate .= '<td>';
						$listUpdate .= $obj->classe_clausula;
						$listUpdate .= '</td>';
						$listUpdate .= '</tr>';
					}	

					$response['response_data']['html'] 			 = $html;

					$response['response_data']['list2'] = $list;
					$response['response_data']['listUpdate'] = $listUpdate;
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				$sql = "SELECT 
							id_estruturaclausula
							,nome_clausula
						FROM 
							estrutura_clausula;									
				";
				
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$clausulas = null;
					while($obj = $resultsql->fetch_object()){
						$clausulas .= '<option value="';
						$clausulas .= $obj->id_estruturaclausula;
						$clausulas .= '">';
						$clausulas .= $obj->nome_clausula;
						$clausulas .= '</option>';
					}	

					$response['response_data']['nomesClausulas'] 	= $clausulas;
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}
			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}

	function getTagClausulasInformacoesAdicionais( $data = null ){

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
			
				$sql2 = "SELECT 
							est.id_estruturaclausula as id_estruturaclausula,
							max(est.nome_clausula) as nome_clausula,	
							IFNULL(GROUP_CONCAT(info.nmtipoinformacaoadicional), GROUP_CONCAT(IFNULL(info.nmtipoinformacaoadicional,null))) as nmtipoinformacaoadicional
						FROM estrutura_clausula as est 
						LEFT JOIN estrutura_clausulas_ad_tipoinformacaoadicional as ad on est.id_estruturaclausula = ad.estrutura_clausula_id_estruturaclausula
						LEFT JOIN ad_tipoinformacaoadicional as info on ad.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = info.cdtipoinformacaoadicional
						GROUP BY est.id_estruturaclausula;
				";
				

				if( $resultsql = mysqli_query( $this->db, $sql2 ) ){

					$html = null;
					$listInformacoes = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#addModalPasso2" onclick="getByIdInformacoesAdicionais( '.$obj->id_estruturaclausula.');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td class="title">';
						$html .= $obj->nome_clausula;
						$html .= '</td>';
						$html .= '<td class="desc">';

						$html.= implode(", ", explode(",", $obj->nmtipoinformacaoadicional));

						$html .= '</td>';
						$html .= '</tr>';
					}						

					$response['response_data']['html'] = $html;
					$this->logger->debug(  $html );

				}
				else{
					$this->logger->debug( $sql2 );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}	
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

	function getByIdTagClausulas( $data = null ){
 
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

				$sql = "SELECT 
							id_estruturaclausula
							,nome_clausula
						FROM 
							estrutura_clausula
						WHERE
							id_estruturaclausula = {$data['id_estruturaclausula']};
				";
				
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$obj = $resultsql->fetch_object();

					$response['response_data']['id_estruturaclausula'] = $obj->id_estruturaclausula;
					$response['response_data']['nome_clausula']		    = $obj->nome_clausula;

					$item = $this->infoSelected($data['id_estruturaclausula']);

					$response['response_data']['type'] = $item['response_data']['type_list'];
					$response['response_data']['class'] = $item['response_data']['class_list'];

					$this->logger->debug( $response);
				                   	
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}			

			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}	
	
	function getTagClausulasCampos( $data = null ){

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
 
			//$this->logger->debug(  $connectdb );
 
			if( $response['response_status']['status'] == 1 ){

				$sqlClausula = "SELECT 
									id_estruturaclausula
									,nome_clausula
								FROM 
									estrutura_clausula
								WHERE
									id_estruturaclausula = '{$data['id_estruturaclausula']}';
				";

				$resultClausula = mysqli_query( $this->db, $sqlClausula );

				$obj = $resultClausula->fetch_object();

				$response['response_data']['clausulaName'] = $obj->nome_clausula;
				
				$this->logger->debug(  $sqlClausula );
				$sqlGeral = "SELECT 
								cdtipoinformacaoadicional, 
								nmtipoinformacaoadicional
							FROM 
								ad_tipoinformacaoadicional
                ";

				$this->logger->debug( $sqlGeral );

				if( $resultsqlGeral = mysqli_query( $this->db, $sqlGeral ) ){

					$listaMod = [];

					while($obj = $resultsqlGeral->fetch_object()){
						$checkBox = null;
						$sqlChecked = "SELECT estrutura_clausula_id_estruturaclausula, ad_tipoinformacaoadicional_cdtipoinformacaoadicional FROM estrutura_clausulas_ad_tipoinformacaoadicional";
						$resultChecked = mysqli_query( $this->db, $sqlChecked );
						$this->logger->debug($resultChecked );
						while ($checkedQuery = $resultChecked->fetch_object()) {
							if ($obj->cdtipoinformacaoadicional == $checkedQuery->ad_tipoinformacaoadicional_cdtipoinformacaoadicional && $data['id_estruturaclausula'] == $checkedQuery->estrutura_clausula_id_estruturaclausula) {
								$checked = 'true';
								break;
							}else {
								$checked = "";
							}
						}

						$imprime	= 'true';
						
						if( $checked == "true" )
						{
							$checkBox .= '<input class="form-check-input" onclick="saveModuleChange( '.$obj->cdtipoinformacaoadicional.', '.$data['id_estruturaclausula'].');" type="checkbox" value="0" id="check'.$obj->cdtipoinformacaoadicional.'" checked>';
						}
						else{	
							$checkBox .= '<input class="form-check-input" onclick="saveModuleChange( '.$obj->cdtipoinformacaoadicional.', '.$data['id_estruturaclausula'].');" type="checkbox" value="1" id="check'.$obj->cdtipoinformacaoadicional.'">';
							
						}
						if( $imprime == 'true' )
						{
							$obj->checkBox = $checkBox;

						
						}
						$this->logger->debug(  $obj );	
						array_push($listaMod, $obj);
					}					
					
					$response['response_data']['listaMod'] 		 = $listaMod;

				}
				else{
					$this->logger->debug( $this->db->error );
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}
			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		$this->logger->debug( $response['response_data'] );
		$this->logger->debug( $response['response_status']['status'] );
								
		return $response;
	}

	function getByIdInformacoesAdicionais( $data = null ){

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

				$sql = "SELECT 
							 cdtipoinformacaoadicional
							,nmtipoinformacaoadicional  
						FROM 
							ad_tipoinformacaoadicional
						WHERE
							cdtipoinformacaoadicional = {$data['cdtipoinformacaoadicional']};
				";
				
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$obj = $resultsql->fetch_object();

					$response['response_data']['cdtipoinformacaoadicional'] = $obj->cdtipoinformacaoadicional;
					$response['response_data']['nmtipoinformacaoadicional'] = $obj->nmtipoinformacaoadicional;
					   
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}			

			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}

	function addTagClausulas( $data = null ){
		$this->logger->debug(  'entrou na classe php' );
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
 
			$this->logger->debug(  $this->db );
			if( $response['response_status']['status'] == 1 ){
				$sql = "INSERT INTO estrutura_clausula
								(nome_clausula, 
								grupo_clausula_idgrupo_clausula, 
								tipo_clausula, 
								classe_clausula)						
							VALUES
								('{$data['info-inputc']}', 
								'4', 
								'{$data['infoa-inputc']}', 
								'{$data['infob-inputc']}');
				";
				
				if( !mysqli_query( $this->db, $sql )  ){
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Não foi possível realizar o cadastro';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{		
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Cadastro realizado com sucesso';
				}			

			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}

	function infoSelected($id) {
		if($this->response['response_status']['status'] == 1) {

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

			//Função
			$sql = "SELECT id_estruturaclausula, tipo_clausula, classe_clausula FROM estrutura_clausula WHERE id_estruturaclausula = {$id}";

			if ($result = mysqli_query($this->db, $sql)) {

				while($data = $result->fetch_object()) {

					$type = "<select class='form-control' id='type-select'>";

					switch ($data->tipo_clausula) {
						case 'T':
							$item = "Tabela";
							
							$type .= "<option value='P'>";
							$type .= "Parâmetro";
							$type .= "</option>";
							$type .= "<option value='R'>";
							$type .= "Resumo";
							$type .= "</option>";
							$type .= "<option value='T' selected>";
							$type .= $item;
							$type .= "</option>";

							break;
						case 'R':
							$item = "Resumo";

							$type .= "<option value='P'>";
							$type .= "Parâmetro";
							$type .= "</option>";
							$type .= "<option value='R' selected>";
							$type .= $item;
							$type .= "</option>";
							$type .= "<option value='T'>";
							$type .= "Tabela";
							$type .= "</option>";
							break;
						case 'P':
							$item = "Parâmetro";

							$type .= "<option value='P' selected>";
							$type .= $item;
							$type .= "</option>";
							$type .= "<option value='R'>";
							$type .= "Resumo";
							$type .= "</option>";
							$type .= "<option value='T'>";
							$type .= "Tabela";
							$type .= "</option>";
							break;
						
						default:
							$type .= "<option value='P'>";
							$type .= "Parâmetro";
							$type .= "</option>";
							$type .= "<option value='R'>";
							$type .= "Resumo";
							$type .= "</option>";
							$type .= "<option value='T'>";
							$type .= "Tabela";
							$type .= "</option>";
							break;
					}

					$type .= "</select>";

					$class = "<select class='form-control' id='class-select'>";

					switch ($data->classe_clausula) {
						case 'S':
							$item = "Sim";
							
							$class .= "<option value='S' selected>";
							$class .= $item;
							$class .= "</option>";
							$class .= "<option value='N'>";
							$class .= "Não";
							$class .= "</option>";
							break;
						case 'N':
							$item = "Não";

							$class .= "<option value='S'>";
							$class .= "Sim";
							$class .= "</option>";
							$class .= "<option value='N' selected>";
							$class .= $item;
							$class .= "</option>";
							break;
						default:
							$class .= "<option>";
							$class .= "Parâmetro";
							$class .= "</option>";
							$class .= "<option>";
							$class .= "Resumo";
							$class .= "</option>";
							$class .= "<option>";
							$class .= "Tabela";
							$class .= "</option>";
							break;
					}

					$class .= "</select>";
				}

				$this->logger->debug(  $type);
				$response['response_data']['type_list'] = $type;

				$this->logger->debug(  $class);
				$response['response_data']['class_list'] = $class;
			}

			return $response;
		}
	}

	function addTagClausulasPasso2( $data = null ){
		$this->logger->debug(  'entrou na classe php' );
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
				$sql = "INSERT INTO ad_tipoinformacaoadicional
								(nmtipoinformacaoadicional)						
							VALUES
								('{$data['infoa-inputc']}');
				";
				$this->logger->debug( $sql );
				if( !mysqli_query( $this->db, $sql ) ){
					
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Não foi possível realizar o cadastro.';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{		
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Cadastro realizado com sucesso!';
				}	
			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}	
	
	function updateTagClausulas( $data = null ){

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
				$sql = " UPDATE estrutura_clausula
						SET  
							nome_clausula = '{$data['up1-inputu']}', 
							tipo_clausula = '{$data['type-select']}', 
							classe_clausula = '{$data['class-select']}'  
						WHERE id_estruturaclausula = {$data['id_estruturaclausula']};
				";
				$this->logger->debug( $sql );

				if( !mysqli_query( $this->db, $sql ) ){
										
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Não foi possível atualizar o registro';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{	
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Registro atualizado com sucesso!';
				}			

			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}	

	function saveModuleChange( $data = null ){

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

				if(($data['check']) == 0){
					$sql = "DELETE 
								FROM 
									estrutura_clausulas_ad_tipoinformacaoadicional 
								WHERE 
									estrutura_clausula_id_estruturaclausula = {$data['id_estruturaclausula']}
								AND 
									ad_tipoinformacaoadicional_cdtipoinformacaoadicional = {$data['cdtipoinformacaoadicional']};
					";
					$this->logger->debug( $sql );
					if( !mysqli_query( $this->db, $sql ) ){
						
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não foi possível excluír o registro!';
						
						$this->logger->debug( $sql );
						$this->logger->debug( $this->db->error );
						$this->logger->debug( $response );
					}
					else{
						$response['response_status']['status']       = 1;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Registro excluído com sucesso!';
					}			

				}else{

					$sql = "INSERT INTO estrutura_clausulas_ad_tipoinformacaoadicional 
								(estrutura_clausula_id_estruturaclausula, 
								ad_tipoinformacaoadicional_cdtipoinformacaoadicional)
							VALUES 
								({$data['id_estruturaclausula']} , 
								{$data['cdtipoinformacaoadicional']} );
					";
					$this->logger->debug( $sql );
					if( !mysqli_query( $this->db, $sql ) ){
												
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não foi possível realizar o cadastro.';
						
						$this->logger->debug( $sql );
						$this->logger->debug( $this->db->error );
						$this->logger->debug( $response );
					}
					else{	
						$response['response_status']['status']       = 1;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Cadastro realizado com sucesso';
					}			

				}
				
			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}
}
?>
