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

date_default_timezone_set('America/Sao_Paulo');

require_once __DIR__ . "/helpers.php";

class sindicatopatronal{
	
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


	function getSindicatoPatronalCampos( $data = null ){

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
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					$htmlu = null;
					//CNAE
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addCNAE('.$obj->id_cnae.');" value="1" id="inicheck'.$obj->id_cnae.'"></td>';
						$html .= '<td>';
						$html .= $obj->divisao_cnae;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->descricao_divisão;
						$html .= '</td>';
						$html .= '<td class="title">';
						$html .= $obj->subclasse_cnae;
						$html .= '</td>';
						$html .= '<td class="desc">';
						$html .= $obj->descricao_subclasse;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->categoria;
						$html .= '</td>';
						$html .= '</tr>';

						$htmlu .= '<tr class="odd gradeX tbl-item">';
						$htmlu .= '<td><input class="form-check-input" type="checkbox" onclick="addCNAEu('.$obj->id_cnae.');" value="1" id="uinicheck'.$obj->id_cnae.'"></td>';
						$htmlu .= '<td>';
						$htmlu .= $obj->divisao_cnae;
						$htmlu .= '</td>';
						$htmlu .= '<td>';
						$htmlu .= $obj->descricao_divisão;
						$htmlu .= '</td>';
						$htmlu .= '<td class="title">';
						$htmlu .= $obj->subclasse_cnae;
						$htmlu .= '</td>';
						$htmlu .= '<td class="desc">';
						$htmlu .= $obj->descricao_subclasse;
						$htmlu .= '</td>';
						$htmlu .= '<td>';
						$htmlu .= $obj->categoria;
						$htmlu .= '</td>';
						$htmlu .= '</tr>';

					}	

					$response['response_data']['listaCNAEini'] 	= $html;
					$response['response_data']['listaCNAEiniu'] 	= $htmlu;

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}		

				$sql = "
				SELECT 
							id_associacao
							,sigla
							,cnpj
                            ,area_geoeconomica as area
                            ,telefone
							,grau
							,grupo
						FROM 
							associacao
						WHERE grau = 'confederação';		";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					$htmlupdate = null;
					//CONFEDERAÇÃO
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><button onclick="selectAssociacao( '.$obj->id_associacao.');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$html .= '<td class="title">';
						$html .= $obj->sigla;
						$html .= '</td>';
						$html .= '<td class="cnpj_format desc2">';
						$html .= $obj->cnpj;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->area;
						$html .= '</td>';
						$html .= '<td>';
						$html .= formatPhoneNumber($obj->telefone);
						$html .= '</td>';
						$html .= '<td class="desc">';
						$html .= $obj->grupo;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->grau;
						$html .= '</td>';
						$html .= '</tr>';


						$htmlupdate .= '<tr class="odd gradeX tbl-item">';
						$htmlupdate .= '<td><button onclick="selectAssociacao( '.$obj->id_associacao.');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td class="title">';
						$htmlupdate .= $obj->sigla;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td class="cnpj_format desc2">';
						$htmlupdate .= $obj->cnpj;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->area;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= formatPhoneNumber($obj->telefone);
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td class="desc">';
						$htmlupdate .= $obj->grupo;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->grau;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
					}	

					$response['response_data']['listaAssociacao'] 	= $html;
					$response['response_data']['listaAssociacaoUpdate'] 	= $htmlupdate;



				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}


				$sql = "
				SELECT 
							id_associacao
							,sigla
							,cnpj
                            ,area_geoeconomica as area
                            ,telefone
							,grau
							,grupo
						FROM 
							associacao
						WHERE grau = 'federação';		";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					//FEDERAÇÃO
					$html = null;
					$htmlupdate = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><button onclick="selectAssociacao1( '.$obj->id_associacao.');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$html .= '<td class="title">';
						$html .= $obj->sigla;
						$html .= '</td>';
						$html .= '<td class="cnpj_format desc2">';
						$html .= $obj->cnpj;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->area;
						$html .= '</td>';
						$html .= '<td>';
						$html .= formatPhoneNumber($obj->telefone);
						$html .= '</td>';
						$html .= '<td class="desc">';
						$html .= $obj->grupo;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->grau;
						$html .= '</td>';
						$html .= '</tr>';


						$htmlupdate .= '<tr class="odd gradeX tbl-item">';
						$htmlupdate .= '<td><button onclick="selectAssociacao1( '.$obj->id_associacao.');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td class="title">';
						$htmlupdate .= $obj->sigla;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td class="cnpj_format desc2">';
						$htmlupdate .= $obj->cnpj;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->area;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= formatCnae($obj->telefone);
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td class="desc">';
						$htmlupdate .= $obj->grupo;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->grau;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
					}	

					$response['response_data']['listaAssociacao1'] 	= $html;
					$response['response_data']['listaAssociacaoUpdate1'] 	= $htmlupdate;



				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}








				
				$sql = "
				SELECT 
							id_tiponegocio
							,tipo_negocio
						FROM 
							tipounidade_cliente;							
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					$htmlupdate = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX">';
						$html .= '<td><button onclick="selectNegocio( '.$obj->id_tiponegocio.');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$html .= '<td>';
						$html .= $obj->tipo_negocio;
						$html .= '</td>';
						$html .= '</tr>';



						$htmlupdate .= '<tr class="odd gradeX">';
						$htmlupdate .= '<td><button onclick="selectNegocio( '.$obj->id_tiponegocio.');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->tipo_negocio;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
					}	

					$response['response_data']['listaNegocio'] 	= $html;
					$response['response_data']['listaNegocioUpdate'] 	= $htmlupdate;



				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
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
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					//LOCALIZAÇÃO
					$html = null;
					$htmlupdate = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><button onclick="selectLocalizacao( '.$obj->id_localizacao.');" data-toggle="modal" href="#defBaseModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
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
						$html .= '<td class="title">';
						$html .= $obj->estado;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->uf;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->cod_municipio;
						$html .= '</td>';
						$html .= '<td class="desc">';
						$html .= $obj->municipio;
						$html .= '</td>';
						$html .= '</tr>';



						$htmlupdate .= '<tr class="odd gradeX tbl-item">';
						$htmlupdate .= '<td><button onclick="selectLocalizacao( '.$obj->id_localizacao.');" data-toggle="modal" href="#defBaseModalUpdate" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
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
						$htmlupdate .= '<td class="title">';
						$htmlupdate .= $obj->estado;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->uf;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->cod_municipio;
						$htmlupdate .= '</td>';
						$htmlupdate .= '<td class="desc">';
						$htmlupdate .= $obj->municipio;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';
					}	

					$response['response_data']['listaLocalizacao'] 	= $html;
					$response['response_data']['listaLocalizacao2'] 	= $htmlupdate;



				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}








				$sql = "
				SELECT 
							id_associacao
						    ,sigla
						FROM 
							associacao;									
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$grupos = '<option value=""></option>';
					while($obj = $resultsql->fetch_object()){
						$grupos .= '<option value="';
						$grupos .= $obj->id_associacao;
						$grupos .= '">';
						$grupos .= $obj->sigla;
						$grupos .= '</option>';


					}	

					$response['response_data']['associacao'] 	= $grupos;

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}
				
				
				$sql = "
				SELECT 
							id_centralsindical
						    ,sigla
						FROM 
							central_sindical;									
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$grupos = '<option value=""></option>';
					$html = null;
					$htmlupdate = null;
					while($obj = $resultsql->fetch_object()){
						$grupos .= '<option value="';
						$grupos .= $obj->id_centralsindical;
						$grupos .= '">';
						$grupos .= $obj->sigla;
						$grupos .= '</option>';

						
						$html .= '<tr class="odd gradeX">';
						$html .= '<td><button onclick="selectCentralSindical( '.$obj->id_centralsindical.');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$html .= '<td>';
						$html .= $obj->sigla;
						$html .= '</td>';
						$html .= '</tr>';



						$htmlupdate .= '<tr class="odd gradeX">';
						$htmlupdate .= '<td><button onclick="selectCentralSindical( '.$obj->id_centralsindical.');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
						$htmlupdate .= '<td>';
						$htmlupdate .= $obj->sigla;
						$htmlupdate .= '</td>';
						$htmlupdate .= '</tr>';

					}	

					$response['response_data']['centralsindical'] 	= $grupos;

					$response['response_data']['listaCentralSindical'] 	= $html;

					$response['response_data']['listaCentralSindicalUpdate'] 	= $htmlupdate;

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}



				
				$sql = "
				SELECT 
							id_localizacao
						    ,municipio
						FROM 
							localizacao;									
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$grupos = '<option value=""></option>';
					while($obj = $resultsql->fetch_object()){
						$grupos .= '<option value="';
						$grupos .= $obj->id_localizacao;
						$grupos .= '">';
						$grupos .= $obj->municipio;
						$grupos .= '</option>';

					}	

					$response['response_data']['localizacao'] 	= $grupos;

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







	
	function getSindicatoPatronal( $data = null ){

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
							id_sindp
							,sigla_sp
							,cnpj_sp
                            ,logradouro_sp
                            ,email1_sp
                            ,fone1_sp
                            ,site_sp
							,municipio_sp
							,uf_sp
						FROM 
							sind_patr;		
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdSindicatoPatronal( '.$obj->id_sindp.');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td class="title">';
						$html .= $obj->sigla_sp;
						$html .= '</td>';
						$html .= '<td class="cnpj_format desc">';
						$html .= $obj->cnpj_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->email1_sp;
						$html .= '</td>';
						$html .= '<td class="fone_format">';
						$html .= $obj->fone1_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->municipio_sp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->uf_sp;
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

	function getByIdSindicatoPatronal( $data = null ){

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
							*
						FROM 
							sind_patr
						WHERE
							id_sindp = {$data['id_sindp']};
				";
				
				$this->logger->debug(  mysqli_query( $this->db, $sql ) );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					
					$obj = $resultsql->fetch_object();
					$response['response_data']['id_sp'] 	= $obj->id_sindp;
					$response['response_data']['sigla_sp'] 	= $obj->sigla_sp;
					$response['response_data']['cnpj_sp'] 	= $obj->cnpj_sp;
					$response['response_data']['razaosocial_sp'] 	= $obj->razaosocial_sp;
					$response['response_data']['denominacao_sp'] 	= $obj->denominacao_sp;
					$response['response_data']['codigo_sp'] 	= $obj->codigo_sp;
					$response['response_data']['situacaocadastro_sp'] 	= $obj->situacaocadastro_sp;
					$response['response_data']['endereco_sp'] 	= $obj->logradouro_sp;
					$response['response_data']['municipio_sp'] 	= $obj->municipio_sp;
					$response['response_data']['uf_sp'] 	= $obj->uf_sp;
					$response['response_data']['fone1_sp'] 	= $obj->fone1_sp;
					$response['response_data']['fone2_sp'] 	= $obj->fone2_sp;
					$response['response_data']['fone3_sp'] 	= $obj->fone3_sp;
					$response['response_data']['ramal_sp'] 	= $obj->ramal_sp;
					$response['response_data']['negociador_sp'] 	= $obj->negociador_sp;
					$response['response_data']['enquadramento_sp'] 	= $obj->enquadramento_sp;
					$response['response_data']['contribuicao_sp'] 	= $obj->contribuicao_sp;
					$response['response_data']['email1_sp'] 	= $obj->email1_sp;
					$response['response_data']['email2_sp'] 	= $obj->email2_sp;
					$response['response_data']['email3_sp'] 	= $obj->email3_sp;
					$response['response_data']['twit_sp'] 	= $obj->twitter_sp;
					$response['response_data']['face_sp'] 	= $obj->facebook_sp;
					$response['response_data']['insta_sp'] 	= $obj->instagram_sp;
					$response['response_data']['site_sp'] 	= $obj->site_sp;
					$response['response_data']['grau_sp'] 	= $obj->grau_sp;
					$response['response_data']['status'] 	= $obj->status;
					// $response['response_data']['localizacao_id_localizacao'] 	= $obj->localizacao_id_localizacao;
					$response['response_data']['confederacao_id_associacao'] 	= $obj->confederacao_id_associacao;		
					$response['response_data']['federacao_id_associacao'] 	= $obj->federacao_id_associacao;		
					$this->logger->debug(  $response['response_data']  );
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}		
				
				$sql = "
				select b.idbase_territorialSindPatro as id,se.sigla_sp,
				DATE_FORMAT(b.data_inicial,'%d/%m/%Y')  AS data_inicio,
    if(DATE_FORMAT(b.data_final,'%d/%m/%Y') = DATE_FORMAT(0,'%d/%m/%Y'), \"Vigente\", DATE_FORMAT(b.data_final,'%d/%m/%Y') ) as data_fim,
				loc.municipio,
                cc.descricao_subclasse
				from base_territorialsindpatro as b
				inner join sind_patr as se ON se.id_sindp = {$data['id_sindp']}
				inner join localizacao as loc on loc.id_localizacao = b.localizacao_id_localizacao1
                inner join classe_cnae as cc 
				where cc.id_cnae = b.classe_cnae_idclasse_cnae and b.sind_patronal_id_sindp = {$data['id_sindp']} and DATE_FORMAT(b.data_final,'%d/%m/%Y') = DATE_FORMAT(0,'%d/%m/%Y')
				order by data_fim desc;			
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					
					if($ini = $resultsql->fetch_object()){
						$html = null;
					$html = '<div class="panel panel-primary">
					<div class="panel-heading">
						<h4>Remover registros da Base Territorial de ';
					$html .= $ini->sigla_sp;
					$html .= '</h4>
						<div class="options">   
							<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
						</div>
					</div>
					<div class="panel-body collapse in">
					
						<table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables" id="example">
							<thead>
								<tr>
									<th></th>
									<th>Localização</th>
									<th>Descrição Subclasse</th>
									<th>Data de Início</th>
								</tr>
							</thead>
							<tbody>';
				

							$html .= '<tr class="odd gradeX">';
							$html .= '<td><input class="form-check-input" type="checkbox" onclick="addBase('.$ini->id.');" value="1" id="basecheck'.$ini->id.'"></td>';
							$html .= '<td>';
							$html .= $ini->municipio;
							$html .= '</td>';
							$html .= '<td>';
							$html .= $ini->descricao_subclasse;
							$html .= '</td>';
							$html .= '<td>';
							$html .= $ini->data_inicio;
							$html .= '</td>';
							$html .= '</tr>';
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX">';
						$html .= '<td><input class="form-check-input" type="checkbox" onclick="addBase('.$obj->id.');" value="1" id="basecheck'.$obj->id.'"></td>';
						$html .= '<td>';
						$html .= $obj->municipio;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->descricao_subclasse;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->data_inicio;
						$html .= '</td>';
						$html .= '</tr>';


					}	
					$html .= '</tbody>
					</table>
				</div>
			</div>';

					$response['response_data']['listaRemover'] 	= $html;

					}
					

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$response['response_data']['listaRemover'] 	= null;
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				






				

				


				$sql = "
				select se.sigla_sp,
				DATE_FORMAT(b.data_inicial,'%d/%m/%Y')  AS data_inicio,
    if(DATE_FORMAT(b.data_final,'%d/%m/%Y') = DATE_FORMAT(0,'%d/%m/%Y'), \"Vigente\", DATE_FORMAT(b.data_final,'%d/%m/%Y') ) as data_fim,
				loc.municipio,
                cc.descricao_subclasse
				from base_territorialsindpatro as b
				inner join sind_patr as se ON se.id_sindp = {$data['id_sindp']}
				inner join localizacao as loc on loc.id_localizacao = b.localizacao_id_localizacao1
                inner join classe_cnae as cc 
				where cc.id_cnae = b.classe_cnae_idclasse_cnae and b.sind_patronal_id_sindp = {$data['id_sindp']}
				order by data_fim desc;			
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					if($ini = $resultsql->fetch_object()){
						$html = null;
					$html = '<div class="panel panel-primary">
					<div class="panel-heading">
						<h4>Histórico da Base Territorial de ';
					$html .= $ini->sigla_sp;
					$html .= '</h4>
						<div class="options">   
							<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
						</div>
					</div>
					<div class="panel-body collapse in">
					
						<table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables" id="example">
							<thead>
								<tr>
									<th>Localização</th>
									<th>Descrição Subclasse</th>
									<th>Data de Início</th>
									<th>Data de Fim</th>
								</tr>
							</thead>
							<tbody>';
				

							$html .= '<tr class="odd gradeX">';
							$html .= '<td>';
							$html .= $ini->municipio;
							$html .= '</td>';
							$html .= '<td>';
							$html .= $ini->descricao_subclasse;
							$html .= '</td>';
							$html .= '<td>';
							$html .= $ini->data_inicio;
							$html .= '</td>';
							$html .= '<td>';
							$html .= $ini->data_fim;
							$html .= '</td>';
							$html .= '</tr>';
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX">';
						$html .= '<td>';
						$html .= $obj->municipio;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->descricao_subclasse;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->data_inicio;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->data_fim;
						$html .= '</td>';
						$html .= '</tr>';


					}	
					$html .= '</tbody>
					</table>
				</div>
			</div>';

					$response['response_data']['listaHistorico'] 	= $html;

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
	
	function addSindicatoPatronal( $data = null ){
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

				$sql = "insert into sind_patr ( sigla_sp
						,cnpj_sp
						,razaosocial_sp
						,denominacao_sp
						,codigo_sp
						,situacaocadastro_sp
						,logradouro_sp
						,municipio_sp
						,uf_sp
						,fone1_sp							
						,fone2_sp
						,fone3_sp
						,ramal_sp
						,enquadramento_sp
						,negociador_sp
						,contribuicao_sp
						,email1_sp
						,email2_sp
						,email3_sp
						,twitter_sp
						,facebook_sp
						,instagram_sp
						,site_sp
						,grau_sp
						,status
						,confederacao_id_associacao
						,federacao_id_associacao) 
						values ('{$data['sigla-input']}','{$data['cnpj-input']}', '{$data['razaosocial-input']}', '{$data['denominacao-input']}', '{$data['cod-input']}', '{$data['situacao-input']}' , '{$data['endereco-input']}', '{$data['municipio-input']}','{$data['uf-input']}',
						'{$data['fone1-input']}', '{$data['fone2-input']}','{$data['fone3-input']}', '{$data['ramal-input']}', '{$data['enq-input']}', '{$data['neg-input']}','{$data['con-input']}', '{$data['email1-input']}',
						'{$data['email2-input']}', '{$data['email3-input']}', '{$data['twit-input']}', '{$data['face-input']}', '{$data['insta-input']}', '{$data['site-input']}', '{$data['grau-input']}', '{$data['status-input']}',
						'{$data['ass-input']}','{$data['ass1-input']}')
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
					$response['response_status']['msg']          = '';
				}	

				
				
				$bases = explode(" ", $data['cidades-cnaes-input']);
				if ($bases[0] != "") {
					foreach ($bases as &$base) {
						$today = date('Y-m-d');

						$loc_cnaes = explode(":", $base);
						$this->logger->debug( $loc_cnaes );
						$loc = $loc_cnaes[0];
						$cnaes = explode(",", $loc_cnaes[1]);

						$this->logger->debug( $loc_cnaes[1] );

						foreach($cnaes as &$cnae ){
							$sql = "INSERT INTO base_territorialsindpatro
							(data_inicial, data_final, classe_cnae_idclasse_cnae, localizacao_id_localizacao1, sind_patronal_id_sindp)
							select 
							STR_TO_DATE('{$today}', '%Y-%m-%d')
							, STR_TO_DATE('0000-00-00', '%Y-%m-%d')
							,cc.id_cnae
							,{$loc}
							,se.id_sindp
							from sind_patr as se
							INNER JOIN classe_cnae as cc where se.cnpj_sp = '{$data['cnpj-input']}' and cc.id_cnae = {$cnae};
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
								$response['response_status']['msg']          = 'Cadastro realizado!';
							}
						}

						
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
	
	function updateSindicatoPatronal( $data = null ){
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

				$sql = " UPDATE sind_patr  
									SET  
									sigla_sp = '{$data['sigla-input']}'
									,cnpj_sp = '{$data['cnpj-input']}'
									,razaosocial_sp = '{$data['razaosocial-input']}'
									,denominacao_sp = '{$data['denominacao-input']}'
									,codigo_sp = '{$data['cod-input']}'
									,situacaocadastro_sp = '{$data['situacao-input']}'
									,logradouro_sp = '{$data['endereco-input']}'
									,municipio_sp = '{$data['municipio-input']}'
									,uf_sp = '{$data['uf-input']}'
									,fone1_sp  ='{$data['fone1-input']}'
									,fone2_sp  ='{$data['fone2-input']}'
									,fone3_sp = '{$data['fone3-input']}'
									,ramal_sp = '{$data['ramal-input']}'
									,enquadramento_sp = '{$data['enq-input']}'
									,negociador_sp = '{$data['neg-input']}'
									,contribuicao_sp = '{$data['con-input']}'
									,email1_sp = '{$data['email1-input']}'
									,email2_sp = '{$data['email2-input']}'
									,email3_sp = '{$data['email3-input']}'
									,twitter_sp = '{$data['twit-input']}'
									,facebook_sp = '{$data['face-input']}'
									,instagram_sp = '{$data['insta-input']}'
									,site_sp = '{$data['site-input']}'      
									,grau_sp = '{$data['grau-input']}'
									,status = '{$data['status-input']}'
									,confederacao_id_associacao = {$data['ass-input']} 
									,federacao_id_associacao = {$data['ass1-input']} 
									WHERE 
										id_sindp = {$data['id_sindp']}
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
				
				
					
				$bases = explode(" ", $data['cidades-cnaes-input']);

				$this->logger->debug( $bases );

				if (key_exists(1, $bases)) {
					foreach ($bases as &$base) {
						$today = date('Y-m-d');

						$loc_cnaes = explode(":", $base);
						$loc = $loc_cnaes[0];
						$cnaes = $loc_cnaes[1] ? explode(",", $loc_cnaes[1]) : "";

						$this->logger->debug( $cnaes );

						if (is_array($cnaes)) {
							foreach($cnaes as &$cnae ){
								$sql = "INSERT INTO base_territorialsindpatro
								(data_inicial, data_final, classe_cnae_idclasse_cnae, localizacao_id_localizacao1, sind_patronal_id_sindp)
								select 
								STR_TO_DATE('{$today}', '%Y-%m-%d')
								, STR_TO_DATE('0000-00-00', '%Y-%m-%d')
								,cc.id_cnae
								,{$loc}
								,se.id_sindp
								from sind_patr as se
								INNER JOIN classe_cnae as cc where se.cnpj_sp = '{$data['cnpj-input']}' and cc.id_cnae = {$cnae};
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
									$response['response_status']['msg']          = '';
								}
							}
						}

						
						
					}
				}
				
				$rbases = explode(" ", $data['remover-input']);

				$this->logger->debug( $rbases );

				if ($rbases[0] != "") {
					foreach ($rbases as &$rbase) {
						$today = date('Y-m-d');

						$sql = "UPDATE base_territorialsindpatro
						SET  data_final = STR_TO_DATE('{$today}', '%Y-%m-%d')
						WHERE 
						idbase_territorialSindPatro = $rbase";
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
							$response['response_status']['msg']          = '';
						}
						
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