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


class adtipoinformacaoadicional{
	
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
	
	function getadTipoInformacaoAdicional( $data = null ){

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

				$sql = "
				SELECT 
							cdtipoinformacaoadicional
							,nmtipoinformacaoadicional
							,idtipodado
                            ,DATE_FORMAT(dtultatualizacao,'%d/%m/%Y') as dtultatualizacao
						FROM 
							ad_tipoinformacaoadicional;							
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdadTipoInformacaoAdicional( '.$obj->cdtipoinformacaoadicional.');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td class="title">';
						$html .= $obj->nmtipoinformacaoadicional;
						$html .= '</td>';
						$html .= '<td class="desc">';
						

						switch ($obj->idtipodado) {
							case 'D':
								$tipo = "Data";
								break;
							case 'C':
								$tipo = "Combo";
								break;
							case 'CM':
								$tipo = "Combo Multiplo";
								break;
							case 'P':
								$tipo = "Descrição";
								break;
							case 'G':
								$tipo = "Grupo";
								break;
							case 'H':
								$tipo = "Hora";
								break;
							case 'N':
								$tipo = "Numérico";
								break;
							case 'L':
								$tipo = "Percentual";
								break;
							case 'V':
								$tipo = "Valor - R$";
							case 'T':
								$tipo = "Texto";
								break;
						}

						$html .= $tipo;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->dtultatualizacao;
						$html .= '</td>';
						$html .= '</tr>';

					}	

					$response['response_data']['html'] 	= $html;

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}	
				
				//Lista Para criar grupos
				
				
				$sqlGeral = "
				SELECT 
					idtipodado,
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
						
						$imprime	= 'true';
						
						$checkBox .= '<input class="form-check-input" onclick="selectInfoGroup( '.$obj->cdtipoinformacaoadicional.');" type="checkbox" value="0" id="check'.$obj->cdtipoinformacaoadicional.'">';
						
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
					// $this->logger->debug( $sql );
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

	function getByIdadTipoInformacaoAdicional( $data = null ){

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

				$sql = "
				SELECT 
							cdtipoinformacaoadicional
							,nmtipoinformacaoadicional
							,idtipodado
                            ,DATE_FORMAT(dtultatualizacao,'%d/%m/%Y') as dtultatualizacao
						FROM 
							ad_tipoinformacaoadicional
						WHERE
						cdtipoinformacaoadicional = {$data['cdtipoinformacaoadicional']};
				";
				
				$select = "";
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$obj = $resultsql->fetch_object();

					

					$select .= ($obj->idtipodado == 'C' ? '<option selected value="C">Combo</option>' : '<option value="C">Combo</option>' );
					$select .= ($obj->idtipodado == 'CM' ? '<option selected value="CM">Combo Multiplo</option>' : '<option value="CM">Combo Multiplo</option>' );
					$select .= ($obj->idtipodado == 'D' ? '<option selected value="D">Data</option>' : '<option value="D">Data</option>' );
					$select .= ($obj->idtipodado == 'P' ? '<option selected value="P">Descrição</option>' : '<option value="P">Descrição</option>');
					$select .= ($obj->idtipodado == 'G' ? '<option selected value="G">Grupo</option>' : '<option value="G">Grupo</option>');
					$select .= ($obj->idtipodado == 'H' ? '<option selected value="H">Hora</option>' : '<option value="H">Hora</option>');
					$select .= ($obj->idtipodado == 'N' ? '<option selected value="N">Numérico</option>' : '<option value="N">Numérico</option>');
					$select .= ($obj->idtipodado == 'L' ? '<option selected value="L">Percentual</option>' : '<option value="L">Percentual</option>');
					$select .= ($obj->idtipodado == 'T' ? '<option selected value="T">Texto</option>' : '<option value="T">Texto</option>');
					$select .= ($obj->idtipodado == 'V' ? '<option selected value="V">Valor - R$</option>' : '<option value="V">Valor - R$</option>');

					$response['response_data']['cdtipoinformacaoadicional'] 	= $obj->cdtipoinformacaoadicional;
					$response['response_data']['idtipodado'] 	= $select;
					$response['response_data']['nmtipoinformacaoadicional'] 	= $obj->nmtipoinformacaoadicional;
					//$response['response_data']['idtipodado'] 	= $obj->idtipodado;
					$response['response_data']['dtultatualizacao'] 	= $obj->dtultatualizacao;

					if ($obj->idtipodado == 'C' || $obj->idtipodado == 'CM') {
						$sqlCombo = "SELECT * FROM informacao_adicional_combo WHERE ad_tipoinformacaoadicional_id = {$data['cdtipoinformacaoadicional']}";

						$campo = "";
						if ($resultsql = mysqli_query( $this->db, $sqlCombo)) {
							$obj2 = $resultsql->fetch_object();
							$this->logger->debug( $obj2 );

							$options = explode(", ", $obj2->options);

							$campo .= '<label class="col-sm-3 control-label">Opções</label>';
							$campo .= '<div class="col-sm-6" id="combo-extra">';

							for ($i=0; $i < count($options) ; $i++) { 
								$campo .= '<input type="text" class="form-control campo" value="'.$options[$i].'">';
							}

							
							$campo .= '<button type="button" class="btn btn-primary btn-delete" onclick="addCampoUpdate();" style="bottom:0px;"><i class="fa fa-plus"></i></button>';
							$campo .= '</div>';
							
							$this->logger->debug( $campo );
						}
						$response['response_data']['camposCombo'] 	= $campo;
					}
					
					if ($obj->idtipodado == 'G') {
						$sqlGrupo = "SELECT 
											*
										FROM 
											informacao_adicional_grupo
										WHERE
											ad_tipoinformacaoadicional_id = {$data['cdtipoinformacaoadicional']};
						";
				
						$this->logger->debug(  $sqlGrupo );

						$sqlGeral = "
						SELECT 
							idtipodado,
							cdtipoinformacaoadicional, 
							nmtipoinformacaoadicional
						FROM 
							ad_tipoinformacaoadicional
								";

						$this->logger->debug( $sqlGeral );

						if( $resultsqlGeral = mysqli_query( $this->db, $sqlGeral ) ){

							$listaGrupo = [];

							while($obj = $resultsqlGeral->fetch_object()){
								$checkBox = null;

								$sqlGrupo = "SELECT 
											*
										FROM 
											informacao_adicional_grupo
										WHERE
											ad_tipoinformacaoadicional_id = {$data['cdtipoinformacaoadicional']};
								";
								
								$resultChecked = mysqli_query( $this->db, $sqlGrupo );
								
								while ($checkedQuery = $resultChecked->fetch_object()) {
									
									if ($obj->cdtipoinformacaoadicional == $checkedQuery->informacaoadicional_no_grupo) {
										$checked = 'true';
										break;
									}else {
										$checked = "";
									}

									$this->logger->debug($checkedQuery->ad_tipoinformacaoadicional_id . " info adicional " .  $obj->cdtipoinformacaoadicional . "checked " . $checked);
								}

								$imprime	= 'true';
								
								if( $checked == "true" )
								{
									$checkBox .= '<input class="form-check-input" onclick="saveModuleChangeGrupo( '.$obj->cdtipoinformacaoadicional.', '.$data['cdtipoinformacaoadicional'].');" type="checkbox" value="0" id="checkInfo'.$obj->cdtipoinformacaoadicional.'" checked>';
								}
								else{	
									$checkBox .= '<input class="form-check-input" onclick="saveModuleChangeGrupo( '.$obj->cdtipoinformacaoadicional.', '.$data['cdtipoinformacaoadicional'].');" type="checkbox" value="1" id="checkInfo'.$obj->cdtipoinformacaoadicional.'">';
									
								}
								if( $imprime == 'true' )
								{
									$obj->checkBox = $checkBox;


								
								}
								$this->logger->debug(  $obj );	
								array_push($listaGrupo, $obj);
							}					
							
							$response['response_data']['listaGrupo'] 		 = $listaGrupo;
							$this->logger->debug( $listaGrupo );

						}
						else{
							// $this->logger->debug( $sql );
							$this->logger->debug( $this->db->error );
										
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = '';
						}
					}
					
					

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
	
	function addadTipoInformacaoAdicional( $data = null ){
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

			if (key_exists("tipo", $data)) {
				$tipo = $data['tipo'];
			}else {
				$tipo = $data['tipo-input'];
			}
 
			$this->logger->debug(  $connectdb );
			if( $response['response_status']['status'] == 1 ){

				$sql = "insert into ad_tipoinformacaoadicional
							(nmtipoinformacaoadicional
							,idtipodado
							,dtultatualizacao)
						values
						('{$data['ia-input']}', '{$tipo}',
						STR_TO_DATE('{$data['data-input']}', '%d/%m/%Y'));
				";


				$this->logger->debug( $sql );


				if( !mysqli_query( $this->db, $sql ) ){
										
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Cadastro realizado com sucesso!';
				}	
				
				$lastId = mysqli_insert_id($this->db);

				

				if ($data['tipo-input'] == "G") {

					$dataGroup = explode(" ", trim($data['list-group']));

					for ($i=0; $i < count($dataGroup) ; $i++) { 

						// $sqlInfo = "SELECT idtipodado, cdtipoinformacaoadicional FROM ad_tipoinformacaoadicional WHERE cdtipoinformacaoadicional = '{$dataGroup[$i]}'";
						// $result = mysqli_query( $this->db, $sqlInfo );
						

						// while ($obj = $result->fetch_object()) {

						// 	if ($obj->idtipodado == "T") {
						// 		$this->logger->debug( $obj->idtipodado );
						// 		$this->logger->debug( $obj->cdtipoinformacaoadicional );
						// 	}
							
						// }
						

						$sequencia = ($i + 1);
						$sql2 = "INSERT INTO informacao_adicional_grupo
								(ad_tipoinformacaoadicional_id, informacaoadicional_no_grupo, sequencia)
							VALUES ('{$lastId}', '{$dataGroup[$i]}', '{$sequencia}')";


							$this->logger->debug( $sql2 );

						if( !mysqli_query( $this->db, $sql2 ) ){
																
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Não foi possível realizar o cadastro.';
						}
						else{		
							$response['response_status']['status']       = 1;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Cadastro realizado com sucesso!';
						}	
					}
				}

				if ($data['tipo-input'] == "C") {

					$dataCombo = implode(", ", array_filter($data['list-combo']));

					$sql3 = "INSERT INTO informacao_adicional_combo
								(ad_tipoinformacaoadicional_id, options)
							VALUES
								('{$lastId}', '{$dataCombo}');
					";
$this->logger->debug( $sql3 );
					if( !mysqli_query( $this->db, $sql3 ) ){
																					
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Não foi possível realizar o cadastro.';

						$this->logger->debug( $response['response_status']['error_code'] );
					}
					else{		
						$response['response_status']['status']       = 1;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Cadastro realizado com sucesso!';

						$this->logger->debug( $response['response_status']['error_code'] );
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
	
	function updateadTipoInformacaoAdicional( $data = null ){
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
				$sql = " UPDATE ad_tipoinformacaoadicional
						SET  nmtipoinformacaoadicional = '{$data['ia-input']}'
						,idtipodado = '{$data['tipo-input']}'
						,dtultatualizacao = STR_TO_DATE('{$data['data-input']}', '%d/%m/%Y')
								WHERE 
									cdtipoinformacaoadicional = {$data['cdtipoinformacaoadicional']}; 
						";
				$this->logger->debug( $sql );
				if( !mysqli_query( $this->db, $sql ) ){
					
					
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}	
				
				if ($data['tipo-input'] == "C") {

					$options = implode(", ", array_filter($data['input-combo']));

					$sql2 = "UPDATE informacao_adicional_combo
							SET 
								options = '{$options}'
							WHERE ad_tipoinformacaoadicional_id = '{$data['cdtipoinformacaoadicional']}'
							";
					$this->logger->debug( $sql2 );

					if( !mysqli_query( $this->db, $sql2 ) ){
											
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Falha na atualização do registro.';
						
						$this->logger->debug( $this->db->error );
					}
					else{
						$this->logger->debug( $this->db->error );
									
						$response['response_status']['status']       = 1;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Cadastro atualizado com sucesso';
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

	function saveModuleChangeGrupo( $data = null ){

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
									informacao_adicional_grupo 
								WHERE 
									informacaoadicional_no_grupo = {$data['id_info']} AND ad_tipoinformacaoadicional_id = {$data['id_grupo']}
					";
					$this->logger->debug( $sql );
					if( !mysqli_query( $this->db, $sql ) ){
						
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = '';
						
						$this->logger->debug( $sql );
						$this->logger->debug( $this->db->error );
						$this->logger->debug( $response );
					}
					else{
						$this->logger->debug( $sql );
						$this->logger->debug( $this->db->error );
									
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = '';
					}			

				}else{

					$sql1 = "SELECT count(*) as qtd FROM informacao_adicional_grupo WHERE ad_tipoinformacaoadicional_id = '{$data['id_grupo']}'";
					$result = mysqli_query( $this->db, $sql1 );
					$sequencia = (($result->fetch_object())->qtd + 1);

					$this->logger->debug( $sequencia );

					$sql = "INSERT INTO informacao_adicional_grupo (ad_tipoinformacaoadicional_id, informacaoadicional_no_grupo, sequencia)
							VALUES ( {$data['id_grupo']} , {$data['id_info']}, {$sequencia} );
					";
					$this->logger->debug( $sql );
					if( !mysqli_query( $this->db, $sql ) ){
												
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = '';
						
						$this->logger->debug( $sql );
						$this->logger->debug( $this->db->error );
						$this->logger->debug( $response );
					}
					else{
						$this->logger->debug( $sql );
						$this->logger->debug( $this->db->error );
									
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = '';
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