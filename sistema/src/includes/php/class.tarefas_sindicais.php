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

// date_default_timezone_set('America/Sao_Paulo');
class tarefas_sindicais{
	
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
	
	function getTarefa( $data = null ){

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
							ds.id_doc
							,tp.nome_doc
							,ds.uf,
							IFNULL(GROUP_CONCAT(DISTINCT cn.classe_cnae_id_cnae), GROUP_CONCAT(IFNULL( cn.classe_cnae_id_cnae,null))) as id_cnae,
							IFNULL(GROUP_CONCAT(DISTINCT cnae.descricao_subclasse), GROUP_CONCAT(IFNULL( cnae.descricao_subclasse,null))) as descricao_subclasse
							,ds.validade_final
							,ds.data_assinatura
							,ds.data_reg_mte,
							IFNULL(GROUP_CONCAT(DISTINCT emp.razaosocial_sinde), GROUP_CONCAT(IFNULL( emp.razaosocial_sinde,null))) as sind_emp,
							IFNULL(GROUP_CONCAT(DISTINCT patr.razaosocial_sp), GROUP_CONCAT(IFNULL( patr.razaosocial_sp,null))) as sind_patr,
							ds.data_aprovacao
						FROM 
							doc_sind as ds
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = ds.tipo_doc_idtipo_doc
						LEFT JOIN classe_cnae_doc_sind as cn on cn.doc_sind_id_doc = ds.id_doc
						LEFT JOIN classe_cnae as cnae on cnae.id_cnae = cn.classe_cnae_id_cnae
						LEFT JOIN doc_sind_sind_emp as se on se.doc_sind_id_doc = ds.id_doc
						LEFT JOIN doc_sind_sind_patr as sp on sp.doc_sind_id_doc = ds.id_doc
						LEFT JOIN sind_emp as emp on emp.id_sinde = se.sind_emp_id_sinde
						LEFT JOIN sind_patr as patr on patr.id_sindp = sp.sind_patr_id_sindp
					GROUP BY ds.id_doc
			
					";
				
				$this->logger->debug(  $sql ); 
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					
					while($obj = $resultsql->fetch_object()){
						$html .= '<tr class="odd gradeX tbl-item">';
						$html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdDocSind( '.$obj->id_doc.');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
						$html .= '<td class="title">';
						$html .= $obj->id_doc;
						$html .= '</td>';
						$html .= '<td class="desc">';
						$html .= $obj->nome_doc;
						$html .= '</td>';
						// $html .= '<td>';
						// $html .= $obj->uf;
						// $html .= '</td>';
						$html .= '<td>';
						$html .= $obj->id_cnae;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->descricao_subclasse;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->sind_emp;
						$html .= '</td>';
						$html .= '<td>';
						$html .= $obj->sind_patr;
						$html .= '</td>';
						$html .= '<td>';
						$html .= ($obj->validade_final == "0000-00-00" || $obj->validade_final == null ? "--" : date('d/m/Y', strtotime($obj->validade_final)));
						$html .= '</td>';
						$html .= '<td>';
						$html .= ($obj->data_aprovacao == "0000-00-00" || $obj->data_aprovacao == null ? "--" : date('d/m/Y', strtotime($obj->data_aprovacao)));
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

	function getTarefasCampos( $data = null ){

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

				//LISTA CLIENTE UNIDADE
				$sqlClt = "SELECT 
								cu.id_unidade
								,gp.nome_grupoeconomico
								,cm.nome_empresa
								,cu.nome_unidade
							FROM 
								cliente_unidades cu
							LEFT JOIN cliente_matriz cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							LEFT JOIN cliente_grupo as gp ON gp.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
				";

				$resultClt = mysqli_query( $this->db, $sqlClt );

				$html = null;
				$selectCltUn = null;

				while($obj = $resultClt->fetch_object()){
					$selectCltUn .= "<option value='{$obj->id_unidade}'>{$obj->nome_grupoeconomico} / {$obj->nome_empresa} / {$obj->nome_unidade}</option>";

				}	

				$response['response_data']['filial'] 	= $selectCltUn;

				//LISTA CNAE
				$sql = "SELECT 
							id_cnae
                            ,descricao_subclasse
							,categoria
						FROM 
							classe_cnae;								
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					
					while($obj = $resultsql->fetch_object()){
						
						$html .= "<option value='{$obj->id_cnae}'>{$obj->descricao_subclasse} / {$obj->categoria}</option>";

					}	

					$response['response_data']['listaCnae'] 	= $html;

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}		

				//LISTA EMPREGADOS
				$sql = "SELECT 
							id_sinde
							,denominacao_sinde
							,cnpj_sinde
						FROM 
							sind_emp		
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$html = null;
					while($obj = $resultsql->fetch_object()){
						$cnpj = $this->formatCnpjCpf($obj->cnpj_sinde);
						$html .= "<option value='{$obj->id_sinde}'>{$obj->denominacao_sinde} / {$cnpj}</option>";

					}	

					$response['response_data']['listaSindEmp'] 	= $html;

				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}


				//LISTA PATRONAL
				$sql = "SELECT 
							id_sindp
							,denominacao_sp
							,cnpj_sp
						FROM 
							sind_patr
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					
					$html = null;
					while($obj = $resultsql->fetch_object()){
						$cnpj = $this->formatCnpjCpf($obj->cnpj_sp);
						$html .= "<option value='{$obj->id_sindp}'>{$obj->denominacao_sp} / {$cnpj}</option>";

					}	

					$response['response_data']['listaPatronal'] = $html;
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}


				// OPTION LOCALIZAÇÃO
				$sql = "SELECT 
							id_localizacao
						    ,municipio
						FROM 
							localizacao								
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$grupos = null;
					while($obj = $resultsql->fetch_object()){
						$grupos .= '<option value="';
						$grupos .= $obj->id_localizacao;
						$grupos .= '">';
						$grupos .= $obj->municipio;
						$grupos .= '</option>';
					}	

					$response['response_data']['local'] 	= $grupos;
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}


				// OPTION UNIDADE CLIENTE
				$sql = "SELECT 
							*
						FROM 
							tipounidade_cliente									
				";
				
				$this->logger->debug(  mysqli_query( $this->db, $sql ));
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$grupos = null;
					while($obj = $resultsql->fetch_object()){
						$grupos .= '<option value="'.$obj->id_tiponegocio.'">';
						$grupos .= $obj->tipo_negocio;
						$grupos .= '</option>';
					}	

					$response['response_data']['unidadeCliente'] 	= $grupos;
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				// OPTION TIPO DOCUMENTO
				$sql = "SELECT 
							DISTINCT idtipo_doc,
							sigla_doc,
							nome_doc
						FROM 
							tipo_doc
						WHERE processado = 'S'
						ORDER BY nome_doc ASC									
				";
				
				$this->logger->debug(  mysqli_query( $this->db, $sql ));
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$grupos = "<option value=''></option>";
					while($obj = $resultsql->fetch_object()){
						$grupos .= '<option value="'.$obj->idtipo_doc.'" data-sigla="'.$obj->sigla_doc.'">';
						$grupos .= $obj->nome_doc;
						$grupos .= '</option>';
					}	

					$response['response_data']['tipoDoc'] 	= $grupos;
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				// LISTA DE DOCUMENTOS (ARQUIVOS)
				$sql = "SELECT 
							id_documento,
							nome_documento,
							caminho,
							origem,
							date_format(data_registro, '%d/%m/%Y - %H:%i:%s') as data_registro
						FROM 
						documentos_localizados
						WHERE situacao = 'não aprovado'
				";
				
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$lista = null;
					while($obj = $resultsql->fetch_object()){
						$btnId = 'see'.$obj->id_documento.'';
						$embedId = 'embed_pdf';
						$onClick = "onclick=seeFile('{$btnId}','embed_pdf',{$obj->id_documento})";
						$path = $obj->caminho;
						$lista .= '<tr class="tbl-item">';
						$lista .= '<td class="title">'.$obj->nome_documento.'</td>';
						$lista .= '<td>'.$obj->origem.'</td>';
						$lista .= '<td class="desc">'.$obj->data_registro.'</td>';
						$lista .= "<td><button id='{$btnId}' data-path='".$path."' type='button' class='btn btn-primary' {$onClick} title='Visualizar arquivo'><i id='icon".$obj->id_documento."' style='font-size: 1.2em;'' class='fa fa-eye eye_list'></i></button>";
						$lista .= '<button type="button" class="btn btn-danger" onclick="deleteFile('.$obj->id_documento.')" title="Excluir arquivo" style="margin: 0 10px;"><i style="font-size: 1.2em;" class="fa fa-trash-o"></i></button>';
						$lista .= '<button type="button" class="btn btn-success" onclick="approveFile('.$obj->id_documento.')" title="Aprovar arquivo"><i style="font-size: 1.2em;" class="fa fa-check-square-o"></i></button></td>';
						$lista .= '</tr>';
					}	

					$response['response_data']['listaArquivosDoc'] 	= $lista;
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				// LISTA DE DOCUMENTOS (ARQUIVOS)
				$sql = "SELECT 
							id_documento,
							nome_documento,
							caminho,
							origem,
							date_format(data_registro, '%d/%m/%Y') as data_registro,
							date_format(data_aprovacao, '%d/%m/%Y') as aprovacao
						FROM 
							documentos_localizados
						WHERE situacao = 'aprovado' AND referenciado = 'não'
				";
				
				if( $resultsql = mysqli_query( $this->db, $sql ) ){

					$opt = null;
					$opt .= "<option value='--'>--</option>";
					while($obj = $resultsql->fetch_object()){
						$opt .= "<option value='{$obj->id_documento}'>{$obj->nome_documento} / Aprovação: {$obj->aprovacao}</option>";

					}	

					$response['response_data']['referenceList'] 	= $opt;
				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				//LISTA DE CLAUSULAS PARA REFERENCIAMENTO
				$sqlRef = "SELECT 
								id_estruturaclausula,
								nome_clausula
							FROM estrutura_clausula
				";

				if( $resultRef = mysqli_query( $this->db, $sqlRef ) ){

					$opt = null;
					$opt .= "<option value=''></option>";
					while($objRef = $resultRef->fetch_object()){
						$opt .= "<option value='{$objRef->id_estruturaclausula}'>{$objRef->nome_clausula}</option>";

					}	

					$response['response_data']['listaEstruturaClausula'] 	= $opt;
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
		
		$this->logger->debug( $response['response_data'] );
		
		return $response;
	}

	function getByIdDocSind( $data = null ){

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
							ds.id_doc
							,tp.nome_doc
							,tp.idtipo_doc
							,tp.sigla_doc
							,ds.uf,
							IFNULL(GROUP_CONCAT(DISTINCT cn.classe_cnae_id_cnae), GROUP_CONCAT(IFNULL( cn.classe_cnae_id_cnae,null))) as id_cnae,
							IFNULL(GROUP_CONCAT(DISTINCT cnae.descricao_subclasse), GROUP_CONCAT(IFNULL( cnae.descricao_subclasse,null))) as descricao_subclasse
							,ds.versao_documento
							,ds.origem
							,ds.numero_solicitacao_mr
							,ds.num_reg_mte
							,ds.validade_inicial
							,ds.validade_final
							,ds.prorrogacao_doc
							,ds.data_assinatura
							,ds.data_reg_mte  
							,ds.permissao
							,ds.observacao
							,ds.tipounidade_cliente_id_tiponegocio,
							dc.nome_documento,
							dc.id_documento,
							dc.caminho,
							ds.doc_restrito,
							ds.database_doc,
							IFNULL(GROUP_CONCAT(DISTINCT abd.localizacao_id_localizacao), GROUP_CONCAT(IFNULL( abd.localizacao_id_localizacao,null))) as id_municipio,
							IFNULL(GROUP_CONCAT(DISTINCT emp.sind_emp_id_sinde), GROUP_CONCAT(IFNULL( emp.sind_emp_id_sinde,null))) as id_laboral,
							IFNULL(GROUP_CONCAT(DISTINCT pt.sind_patr_id_sindp), GROUP_CONCAT(IFNULL( pt.sind_patr_id_sindp,null))) as id_patronal,
							IFNULL(GROUP_CONCAT(DISTINCT cl.cliente_unidades_id_unidade), GROUP_CONCAT(IFNULL( cl.cliente_unidades_id_unidade,null))) as id_cliente_unidades
						FROM 
							doc_sind as ds
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = ds.tipo_doc_idtipo_doc
						LEFT JOIN classe_cnae_doc_sind as cn on cn.doc_sind_id_doc = ds.id_doc
						LEFT JOIN classe_cnae as cnae on cnae.id_cnae = cn.classe_cnae_id_cnae
						LEFT JOIN abrang_docsind as abd on abd.doc_sind_id_documento = ds.id_doc

						LEFT JOIN doc_sind_sind_emp as emp on emp.doc_sind_id_doc = ds.id_doc
						LEFT JOIN doc_sind_sind_patr as pt on pt.doc_sind_id_doc = ds.id_doc
						LEFT JOIN doc_sind_cliente_unidades as cl on cl.doc_sind_id_doc = ds.id_doc
						LEFT JOIN documentos_localizados as dc on dc.id_documento = ds.documento_id_documento
						WHERE ds.id_doc = '{$data['id_doc']}'
						GROUP BY ds.id_doc
				";
				
				$this->logger->debug(  $sql );
				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$obj = $resultsql->fetch_object();
					$response['response_data']['id'] 	= $obj->id_doc;
					$response['response_data']['siglaDoc'] 	= $obj->sigla_doc;
					$response['response_data']['origem'] 	= $obj->origem;
					$response['response_data']['versao'] 	= $obj->versao_documento;
					$response['response_data']['numero'] 	= $obj->numero_solicitacao_mr;
					$response['response_data']['data_reg'] 	= ($obj->data_reg_mte == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($obj->data_reg_mte)) );
					$response['response_data']['num_reg'] 	= $obj->num_reg_mte;
					$response['response_data']['vini'] = ($obj->validade_inicial == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($obj->validade_inicial)) );
					$response['response_data']['vfim'] 	= ($obj->validade_final == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($obj->validade_final)) );
					$response['response_data']['pro'] 	= ($obj->prorrogacao_doc == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($obj->prorrogacao_doc)) );
					$response['response_data']['ass'] 	= ($obj->data_assinatura == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($obj->data_assinatura)) );
					$response['response_data']['permite'] 	= $obj->permissao;
					$response['response_data']['observa'] 	= $obj->observacao;
					$response['response_data']['documento'] 	= $obj->nome_documento;
					$response['response_data']['id_documento'] 	= $obj->id_documento;
					$response['response_data']['doc_restrito'] 	= $obj->doc_restrito;
					$response['response_data']['data_base'] 	= $obj->database_doc;					
					$response['response_data']['path'] 	= $obj->caminho;
					$uf = $obj->uf;
					
					$this->logger->debug( $response['response_data'] );

					//LISTA LABORAL
					$emp = explode(",", $obj->id_laboral);
					$patr = explode(",", $obj->id_patronal);
					$clt = explode(",", $obj->id_cliente_unidades);

					$optEmp = "";
					$sqlEmp = 'SELECT id_sinde, denominacao_sinde, cnpj_sinde FROM sind_emp';
					$resultEmp = mysqli_query( $this->db, $sqlEmp );
					
					for ($i=0; $i <= count($emp) ; $i++) { 
						while ($objEmp = $resultEmp->fetch_object()) {
							$cnpj = $this->formatCnpjCpf($objEmp->cnpj_sinde);

							if (key_exists($i, $emp)) {
								if ($emp[$i] == $objEmp->id_sinde) {
									
									$optEmp .= "<option value='{$objEmp->id_sinde}' selected='selected'>{$objEmp->denominacao_sinde} / {$cnpj}</option>";
									break;
								}
								else {
									$optEmp .= "<option value='{$objEmp->id_sinde}'>{$objEmp->denominacao_sinde} / {$cnpj}</option>";
								}
							}else {
								$optEmp .= "<option value='{$objEmp->id_sinde}'>{$objEmp->denominacao_sinde} / {$cnpj}</option>";
							}
							
						}
					}

					$response['response_data']['listaLaboral'] = $optEmp;
					$this->logger->debug( $optEmp );

					//LISTA PATRONAL
					$optPatr = "";
					$sqlPatr = 'SELECT id_sindp, denominacao_sp, cnpj_sp FROM sind_patr';
					$resultPatr = mysqli_query( $this->db, $sqlPatr );
					
					for ($i=0; $i <= count($patr) ; $i++) { 
						while ($objPatr = $resultPatr->fetch_object()) {
							$cnpj = $this->formatCnpjCpf($objPatr->cnpj_sp);

							if (key_exists($i, $patr)) {
								if ($patr[$i] == $objPatr->id_sindp) {
									$optPatr .= "<option value='{$objPatr->id_sindp}' selected='selected'>{$objPatr->denominacao_sp} / {$cnpj}</option>";
									break;
								}
								else {
									$optPatr .= "<option value='{$objPatr->id_sindp}'>{$objPatr->denominacao_sp} / {$cnpj}</option>";
								}
							}else {
								$optPatr .= "<option value='{$objPatr->id_sindp}'>{$objPatr->denominacao_sp} / {$cnpj}</option>";
							}
							
						}
					}
					$this->logger->debug( $optPatr );
					$response['response_data']['listaPatronal'] = $optPatr;

					//LISTA CLIENTE UNIDADES
					$optClt = "";
					$sqlClt = 'SELECT 
								cu.id_unidade
								,gp.nome_grupoeconomico
								,cm.nome_empresa
								,cu.nome_unidade
							FROM 
								cliente_unidades cu
							LEFT JOIN cliente_matriz cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							LEFT JOIN cliente_grupo as gp ON gp.id_grupo_economico = cm.cliente_grupo_id_grupo_economico';
					$resultClt = mysqli_query( $this->db, $sqlClt );
					$selected = 'no_selected';
					for ($i=0; $i <= count($clt) ; $i++) { 
						while ($objClt = $resultClt->fetch_object()) {


							if (key_exists($i, $clt)) {
								if ($clt[$i] == $objClt->id_unidade) {
									$optClt .= "<option value='{$objClt->id_unidade}' selected='selected'>{$objClt->nome_grupoeconomico} / {$objClt->nome_empresa} / {$objClt->nome_unidade}</option>";
									$selected = 'selected';
									break;
								}
								else {
									$optClt .= "<option value='{$objClt->id_unidade}'>{$objClt->nome_grupoeconomico} / {$objClt->nome_empresa} / {$objClt->nome_unidade}</option>";
								}
							}else {
								$optClt .= "<option value='{$objClt->id_unidade}'>{$objClt->nome_grupoeconomico} / {$objClt->nome_empresa} / {$objClt->nome_unidade}</option>";
							}
							
						}
					}

					$response['response_data']['listaClienteUnidades'] = $optClt;
					$response['response_data']['listaClienteUnidades_selected'] = $selected;
					$this->logger->debug( $optClt );

					//LISTA TIPO UNIDADE CLIENTE
					$tipoUnidadeCliente = explode(",", $obj->tipounidade_cliente_id_tiponegocio);
					$optTipoUn = "";
					$sqlTipo = 'SELECT id_tiponegocio, tipo_negocio FROM tipounidade_cliente';
					$resultTipo = mysqli_query( $this->db, $sqlTipo );
					
					for ($i=0; $i <= count($tipoUnidadeCliente) ; $i++) { 
						while ($objTipo = $resultTipo->fetch_object()) {


							if (key_exists($i, $tipoUnidadeCliente)) {
								if ($tipoUnidadeCliente[$i] == $objTipo->id_tiponegocio) {
									$optTipoUn .= '<option value="'.$objTipo->id_tiponegocio.'" selected="selected">'.$objTipo->tipo_negocio.'</option>';
									break;
								}
								else {
									$optTipoUn .= '<option value="'.$objTipo->id_tiponegocio.'">'.$objTipo->tipo_negocio.'</option>';
								}
							}else {
								$optTipoUn .= '<option value="'.$objTipo->id_tiponegocio.'">'.$objTipo->tipo_negocio.'</option>';
							}
							
						}
					}

					$response['response_data']['listaTipoUnidade'] = $optTipoUn;
					$this->logger->debug( $optTipoUn );

					//LISTA TIPO DOCUMENTO
					$tipoDocumento = explode(",", $obj->idtipo_doc);
					$optTipoDoc = "";
					$sqlTipoDoc = 'SELECT idtipo_doc, tipo_doc, sigla_doc, nome_doc FROM tipo_doc WHERE processado = "S"';
					$resultTipoDoc = mysqli_query( $this->db, $sqlTipoDoc );
					
					for ($i=0; $i <= count($tipoDocumento) ; $i++) { 
						while ($objTipoDoc = $resultTipoDoc->fetch_object()) {


							if (key_exists($i, $tipoDocumento)) {
								if ($tipoDocumento[$i] == $objTipoDoc->idtipo_doc) {
									$optTipoDoc .= '<option value="'.$objTipoDoc->idtipo_doc.'" selected="selected" data-sigla="'.$objTipoDoc->sigla_doc.'">'.$objTipoDoc->nome_doc.'</option>';
									break;
								}
								else {
									$optTipoDoc .= '<option value="'.$objTipoDoc->idtipo_doc.'" data-sigla="'.$objTipoDoc->sigla_doc.'">'.$objTipoDoc->nome_doc.'</option>';
								}
							}else {
								$optTipoDoc .= '<option value="'.$objTipoDoc->idtipo_doc.'" data-sigla="'.$objTipoDoc->sigla_doc.'">'.$objTipoDoc->nome_doc.'</option>';
							}
							
						}
					}

					$response['response_data']['listaTipoDoc'] = $optTipoDoc;
					$this->logger->debug( $optTipoDoc );

					
					//LISTA CNAE
					$cnae = explode(",", $obj->id_cnae);
					$optCnae = "";
					$sqlCnae = 'SELECT id_cnae, descricao_subclasse, categoria FROM classe_cnae';
					$resultCnae = mysqli_query( $this->db, $sqlCnae );
					
					for ($i=0; $i <= count($cnae) ; $i++) { 
						while ($objCnae = $resultCnae->fetch_object()) {


							if (key_exists($i, $cnae)) {
								if ($cnae[$i] == $objCnae->id_cnae) {
									$optCnae .= "<option value='{$objCnae->id_cnae}' selected='selected'>{$objCnae->descricao_subclasse} / {$objCnae->categoria}</option>";
									break;
								}
								else {
									$optCnae .= "<option value='{$objCnae->id_cnae}'>{$objCnae->descricao_subclasse} / {$objCnae->categoria}</option>";
								}
							}else {
								$optCnae .= "<option value='{$objCnae->id_cnae}'>{$objCnae->descricao_subclasse} / {$objCnae->categoria}</option>";
							}
							
						}
					}

					$response['response_data']['listaCnae'] = $optCnae;
					$this->logger->debug( $optCnae );


					//LISTA REFERENCIAMENTO
					//lista de id referenciados
					$sqlRef = "SELECT * FROM doc_sind_referencia WHERE doc_sind_id_doc = '{$data['id_doc']}'";
					$resultRef = mysqli_query( $this->db, $sqlRef );

					$optRef = [];
					while ($obj = $resultRef->fetch_object()) {
						array_push($optRef, $obj->estrutura_clausula_id_estruturaclausula);
						
					}
					$this->logger->debug( $optRef );
					$response['response_data']['id_referenciamento'] = $optRef;

					//Obtendo lista de clausulas
					$sqlEst = "SELECT id_estruturaclausula, nome_clausula FROM estrutura_clausula";//WHERE id_estruturaclausula = '{$obj->estrutura_clausula_id_estruturaclausula}'
					$resultEst = mysqli_query( $this->db, $sqlEst );

					$optEst = "";
					while ($objEst = $resultEst->fetch_object()) {

						$optEst .= "<option value='{$objEst->id_estruturaclausula}'>{$objEst->nome_clausula}</option>";
					}

					$response['response_data']['lista_referenciamento'] = $optEst;


					//COMPARAÇÃO ABRANGENCIA DOC SIND COM BASE TERRITORIAL EMP. E PATR.
					$sql = "SELECT
								ds.id_doc,
								IFNULL(GROUP_CONCAT(DISTINCT emp.sind_emp_id_sinde), GROUP_CONCAT(IFNULL( emp.sind_emp_id_sinde, null))) as sind_emp,
								IFNULL(GROUP_CONCAT(DISTINCT pt.sind_patr_id_sindp), GROUP_CONCAT(IFNULL( pt.sind_patr_id_sindp, null))) as sind_patr,
								IFNULL(GROUP_CONCAT(DISTINCT ab.localizacao_id_localizacao), GROUP_CONCAT(IFNULL( ab.localizacao_id_localizacao, null))) as abrang_doc,
								IFNULL(GROUP_CONCAT(DISTINCT be.localizacao_id_localizacao1), GROUP_CONCAT(IFNULL( be.localizacao_id_localizacao1, null))) as base_emp
								
							FROM doc_sind as ds
							LEFT JOIN doc_sind_sind_emp as emp on doc_sind_id_doc = ds.id_doc
							LEFT JOIN doc_sind_sind_patr as pt on pt.doc_sind_id_doc = ds.id_doc
							LEFT JOIN abrang_docsind as ab on doc_sind_id_documento = ds.id_doc
                            LEFT JOIN sind_emp as sinde on sinde.id_sinde =  emp.sind_emp_id_sinde
							LEFT JOIN base_territorialsindemp as be on be.sind_empregados_id_sinde1 = sinde.id_sinde
							WHERE id_doc = '{$data['id_doc']}'
							
							GROUP BY ds.id_doc
					";

					$result = mysqli_query( $this->db, $sql );

					$obj = $result->fetch_object();

					$sindEmp = explode(",", $obj->sind_emp);
					$sindPatr = explode(",", $obj->sind_patr);
					$abrangDoc = explode(",", $obj->abrang_doc);
					$baseEmp = explode(",", $obj->base_emp);

					$this->logger->debug( $sindEmp );
					$this->logger->debug( $sindPatr );
					$this->logger->debug( $abrangDoc );
					$this->logger->debug( $baseEmp );
					
					$listaBasePatr = [];
					for ($i=0; $i < count($sindPatr) ; $i++) { 
						$sqlBasePatr = "SELECT 
										IFNULL(GROUP_CONCAT(DISTINCT localizacao_id_localizacao1), GROUP_CONCAT(IFNULL( localizacao_id_localizacao1, null))) as base_patr
									FROM base_territorialsindpatro
									WHERE sind_patronal_id_sindp = '{$sindPatr[$i]}'
						";

						$resultPatr = mysqli_query( $this->db, $sqlBasePatr );
						$obj = $resultPatr->fetch_object();

						$array = explode(",", $obj->base_patr);
						$listaBasePatr = array_merge($listaBasePatr, $array);
						
						// array_push($listaBasePatr, $obj->base_patr);

					}

					$this->logger->debug( $listaBasePatr );

					//LOCALIZAÇÕES ABRANGENCIA
					$arrayAbrang = [];
					for ($i=0; $i < count($abrangDoc) ; $i++) { 
						$sqlAbrang = "SELECT 
										id_localizacao,
										uf,
										municipio
									FROM localizacao
									WHERE id_localizacao = '{$abrangDoc[$i]}'
						";

						$resultAbrang = mysqli_query( $this->db, $sqlAbrang );

						
						while ($obj = $resultAbrang->fetch_object()) {
							array_push($arrayAbrang, "{$obj->municipio} - {$obj->uf}");
						}
					}


					//LOCALIZAÇÕES BASE EMP
					$arrayEmp = [];
					for ($i=0; $i < count($baseEmp) ; $i++) { 
						$sqlEmp = "SELECT 
										id_localizacao,
										uf,
										municipio
									FROM localizacao
									WHERE id_localizacao = '{$baseEmp[$i]}'
						";

						$resultEmp = mysqli_query( $this->db, $sqlEmp );

						
						while ($obj = $resultEmp->fetch_object()) {
							array_push($arrayEmp, "{$obj->municipio} - {$obj->uf}");
						}
					}


					//LOCALIZAÇÕES BASE PATR
					$arrayPatr = [];
					for ($i=0; $i < count($listaBasePatr) ; $i++) { 
						$sqlPatr = "SELECT 
										id_localizacao,
										uf,
										municipio
									FROM localizacao
									WHERE id_localizacao = '{$listaBasePatr[$i]}'
						";

						$resultPatr = mysqli_query( $this->db, $sqlPatr );

						
						while ($obj = $resultPatr->fetch_object()) {
							array_push($arrayPatr, "{$obj->municipio} - {$obj->uf}");
						}
					}
					
					//COMPARAÇÃO ABRANG COM LISTA EMP E PATR
					for ($i=0; $i < count($arrayAbrang) ; $i++) { 
						
						if (in_array($arrayAbrang[$i], $arrayEmp) && in_array($arrayAbrang[$i], $arrayPatr)) {
							//UNSET EMP
							$indexEmp = array_search($arrayAbrang[$i], $arrayEmp);
							unset($arrayEmp[$indexEmp]);

							//UNSET PATR
							$indexPatr = array_search($arrayAbrang[$i], $arrayPatr);
							unset($arrayPatr[$indexPatr]);

							//UNSET ABRANG
							unset($arrayAbrang[$i]);
						}
					}

					$this->logger->debug( $arrayAbrang );
					$this->logger->debug( $arrayEmp );
					$this->logger->debug( $arrayPatr );

					//Lista Abrangencia
					$listaAbrang = "";
					natcasesort($arrayAbrang);
					foreach ($arrayAbrang as $value) {
						$listaAbrang .= "<tr>";
						$listaAbrang .= "<td>{$value}</td>";
						$listaAbrang .= "</tr>";
					}

					$response['response_data']['lista_abrang'] = $listaAbrang;

					//Lista Emp
					$listaEmp = "";
					natcasesort($arrayEmp);
					foreach ($arrayEmp as $value) {
						$listaEmp .= "<tr>";
						$listaEmp .= "<td>{$value}</td>";
						$listaEmp .= "</tr>";
					}

					$response['response_data']['lista_emp'] = $listaEmp;

					//Lista Patr
					$listaPatr = "";
					natcasesort($arrayPatr);
					foreach ($arrayPatr as $value) {
						$listaPatr .= "<tr>";
						$listaPatr .= "<td>{$value}</td>";
						$listaPatr .= "</tr>";
					}

					$response['response_data']['lista_patr'] = $listaPatr;


				}
				else{
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}

				//LISTA ABRANGENCIA: ABRANG CADASTRADA
				$sql3 = "SELECT localizacao_id_localizacao, doc_sind_id_documento FROM abrang_docsind WHERE doc_sind_id_documento = '{$data['id_doc']}'";
				$result3 = mysqli_query( $this->db, $sql3 );
				$listaAb = "";
				while ($obj = $result3->fetch_object()) {
					$sqlAb = "SELECT id_localizacao, pais, regiao, uf, municipio FROM localizacao WHERE id_localizacao = '{$obj->localizacao_id_localizacao}'";
					$resultAb = mysqli_query( $this->db, $sqlAb );
					
					
					while ($obj = $resultAb->fetch_object()) {
						$listaAb .= "<tr class='tbl-item'>";
						$listaAb .= "<td><input type='checkbox' data-doc='{$data['id_doc']}' data-id='{$obj->id_localizacao}'></td>";
						$listaAb .= "<td>{$obj->regiao}</td>";
						$listaAb .= "<td class='desc'>{$obj->uf}</td>";
						$listaAb .= "<td class='title'>{$obj->municipio}</td>";
						$listaAb .= "</tr>";
					}
				}
				
				$response['response_data']['lista_abrang_cadastrada'] = $listaAb;
				
				//LISTA DE MUNICIPIOS UPDATE

				$sql2 = "SELECT id_localizacao, pais, regiao, uf, municipio FROM localizacao WHERE uf = 'SP'";
				$resultsql2 = mysqli_query( $this->db, $sql2 );

				$listaMod = "";

				while ($obj2 = $resultsql2->fetch_object()) {
					$listaMod .= "<tr class='tbl-item'>";
					$listaMod .= '<td class="checkAbrang" id="'.$obj2->id_localizacao.'"><input class="form-check-input checkInput" type="checkbox" value="1" id="'.$obj2->id_localizacao.'"></td>';
					$listaMod .= '<td class="title">';
					$listaMod .= $obj2->municipio;
					$listaMod .= '</td>';
					$listaMod .= '</tr>';
				}
				
				// while ($obj2 = $resultsql2->fetch_object()) {

				// 	$this->logger->debug(  $obj2 );

				// 	$sql3 = "SELECT localizacao_id_localizacao, doc_sind_id_documento FROM abrang_docsind ";//WHERE doc_sind_id_documento = '{$data['id_doc']}'
				// 	$result3 = mysqli_query( $this->db, $sql3 );
				// 	$checked = "";
				// 	while($obj3 = $result3->fetch_object()) {

				// 		$this->logger->debug(  $obj3 );
				// 		if ($obj2->id_localizacao == $obj3->localizacao_id_localizacao && $data['id_doc'] == $obj3->doc_sind_id_documento) {
				// 			$checked = 'true';
				// 			break;
				// 		}else {
				// 			$checked = "false";
				// 		}
				// 	}

				// 	$this->logger->debug(  "O check está " . $checked );

				// 	$listaMod .= '<tr class="odd gradeX tbl-item">';

				// 	$imprime	= 'true';
					
				// 	if( $checked == "true" )
				// 	{
				// 		$this->logger->debug(  ' Checked ' );
				// 		$listaMod .= '<td class="checkAbrang" id="'.$obj2->id_localizacao.'"><input class="form-check-input checkInput" type="checkbox" value="0" id="'.$obj2->id_localizacao.'" checked></td>'; //onclick="saveModuleChange( '.$obj2->id_localizacao.', '.$data['id_doc'].');"
				// 	}
				// 	else{
							
				// 		$this->logger->debug(  ' No checked ' );
				// 		$listaMod .= '<td class="checkAbrang" id="'.$obj2->id_localizacao.'"><input class="form-check-input checkInput" type="checkbox" value="1" id="'.$obj2->id_localizacao.'"></td>';//onclick="saveModuleChange( '.$obj2->id_localizacao.', '.$data['id_doc'].');" 
						
				// 	}
				// 	if( $imprime == 'true' )
				// 	{
				// 		$listaMod .= '<td class="title">';
				// 		$listaMod .= $obj2->municipio;
				// 		$listaMod .= '</td>';
				// 		$listaMod .= '</tr>';
				// 	}
				// }

				
				$response['response_data']['abrangUpdate'] = $listaMod;
				// $response['response_data']['ufAbrang'] = 'SP';
				$this->logger->debug(  $response['response_data'] );
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
	
	function addDocSind( $data = null ){
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

			// $sql = "SELECT dataneg FROM base_territorialsindemp WHERE sind_empregados_id_sinde1 = '{$data['emp-input'][0]}'";
			// $result = mysqli_query( $this->db, $sql );
			// $obj = $result->fetch_object();

			// $dataMes = $obj->dataneg;
			// $dataBase = $dataMes . "/" . date_format((new DateTime('now')),'Y');

			$dataBase = $data['data_base'];

			$dataIni = ($data['vini-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['vini-input'])))) : "0000-00-00" );
			$dataFim = ($data['vfim-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['vfim-input'])))) : "0000-00-00" );
			$dataPro = ($data['pro-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['pro-input'])))) : "0000-00-00" );
			$dataReg = ($data['data-reg-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['data-reg-input'])))) : "0000-00-00" );
			$dataAss = ($data['ass-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['ass-input'])))) : "0000-00-00" );
			

			$this->logger->debug( "Ini: " . $dataIni . " Fim: " . $dataFim . " Prorr: " . $dataPro . " Reg-mte: " . $dataReg . " Ass: " . $dataAss );
 
			$this->logger->debug(  $connectdb );
			if( $response['response_status']['status'] == 1 ){

				//INSERT DOC RESTRITO
				if ($data['doc_restrito'] == "restrito") {
					
				}

				$restrito = ($data['doc_restrito'] == "restrito" ? "Sim" : "Não");
				
				$sql = "INSERT INTO doc_sind
				(tipounidade_cliente_id_tiponegocio
				,tipo_doc_idtipo_doc
				,origem
				,versao_documento
				,numero_solicitacao_mr
				,data_reg_mte
				,num_reg_mte
				,validade_inicial
				,validade_final
				,prorrogacao_doc
				,data_assinatura
				,permissao
				,observacao
				,documento_id_documento,
				database_doc,
				doc_restrito
				)
				VALUES (
					'{$data['type-un-input']}', 
					'{$data['tipodoc-input']}', 
					'{$data['ori-input']}', 
					'{$data['versao-input']}', 
					'{$data['num-input']}', 
					'{$dataReg}', 
					'{$data['num-reg-input']}',
					'{$dataIni}', 
					'{$dataFim}',  
					'{$dataPro}', 
					'{$dataAss}', 
					'{$data['perm-input']}', 
					'{$data['obs-input']}',
					'{$data['id_arquivo']}',
					'{$dataBase}',
					'{$restrito}')
				";

			
				
				$this->logger->debug( $sql );
				$this->logger->debug( "Abrangencia:" . $data['abrang-input'] );
				$this->logger->debug( "UF:" . $data['uf-input'] );
				$this->logger->debug( $data['selectCltUn'] );
				$this->logger->debug( $data['cnae-input'] );

				//INSERT DOC SIND

				if( !mysqli_query( $this->db, $sql ) ){
					
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao cadastrar';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{

					$lastId = mysqli_insert_id($this->db);
					
					$sqlDoc = "UPDATE documentos_localizados
							SET referenciado = 'sim'
							WHERE id_documento = '{$data['id_arquivo']}'
					";

					mysqli_query( $this->db, $sqlDoc );

								
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Cadastro realizado com sucesso!';
				}

				
				
				if ($lastId) {
					//INSERT EMPREGADOS e PATRONAL

					$patr =$data['patr-input'];

					$emp = $data['emp-input'];

					$this->logger->debug( $lastId );
					$this->logger->debug( $patr );

					for ($i=0; $i < count($emp); $i++) { 
						$sqlEmp = "INSERT INTO doc_sind_sind_emp (sind_emp_id_sinde, doc_sind_id_doc) VALUES('{$emp[$i]}', '{$lastId}' )";

						if( !mysqli_query( $this->db, $sqlEmp ) ){
												
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Erro ao cadastrar.';
							
							$this->logger->debug( $this->db->error );
							$this->logger->debug( $response );
						}
						else{
							$this->logger->debug( $sqlEmp );
							$this->logger->debug( $this->db->error );
										
							$response['response_status']['status']       = 1;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Cadastro realizado.';
						}
					}

					for ($i=0; $i < count($patr); $i++) { 
						$sqlPatr = "INSERT INTO doc_sind_sind_patr (sind_patr_id_sindp, doc_sind_id_doc) VALUES('{$patr[$i]}', '{$lastId}' )";

						if( !mysqli_query( $this->db, $sqlPatr ) ){
												
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Erro ao cadastrar';
							
							$this->logger->debug( $this->db->error );
							$this->logger->debug( $response );
						}
						else{
							$this->logger->debug( $sqlPatr );
							$this->logger->debug( $this->db->error );
										
							$response['response_status']['status']       = 1;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Cadastro realizado';
						}
					}


					//INSERT ABRANGENCIA TABLE

					// $abrang = array_unique(explode(" ", trim($data['abrang-input'])));
					$abrang = explode(",", implode(",", array_unique(explode(" ", trim($data['abrang-input'])))));
					$this->logger->debug( $abrang );
					$this->logger->debug( array_unique($abrang) );
					for ($i=0; $i < count($abrang) ; $i++) { 

						$sql2 = "INSERT INTO abrang_docsind (localizacao_id_localizacao, doc_sind_id_documento) VALUES('{$abrang[$i]}', '{$lastId}' )";
						$this->logger->debug( $sql2 );

						if( !mysqli_query( $this->db, $sql2 ) ){
												
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Erro ao cadastrar';
							
							$this->logger->debug( $sql2 );
							$this->logger->debug( $this->db->error );
							$this->logger->debug( $response );
						}
						else{
							$this->logger->debug( $sql2 );
							$this->logger->debug( $this->db->error );
										
							$response['response_status']['status']       = 1;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Cadastro realizado com sucesso!';
						}
					}
					
					//INSERT CLIENTE UNIDADES	

					$sql5 = "SELECT sigla_doc FROM tipo_doc WHERE idtipo_doc = (SELECT tipo_doc_idtipo_doc FROM doc_sind WHERE id_doc = {$lastId})";
					$result5 = mysqli_query( $this->db, $sql5 );
					$obj5 = $result5->fetch_object();

					if (!empty($obj5->sigla_doc) && $obj5->sigla_doc == "ACT") {
						
						
						$unidades =$data['selectCltUn'];

						$this->logger->debug( $unidades );

						foreach ($unidades as $unidade) {
							$sql3 = "INSERT INTO doc_sind_cliente_unidades (doc_sind_id_doc, cliente_unidades_id_unidade) VALUES('{$lastId}', '{$unidade}' )";
							$this->logger->debug( $sql3 );
							
							if( !mysqli_query( $this->db, $sql3 ) ){
								
								$response['response_status']['status']       = 0;
								$response['response_status']['error_code']   = $this->error_code . __LINE__;
								$response['response_status']['msg']          = 'Erro ao cadastrar';
								
								$this->logger->debug( $sql3 );
								$this->logger->debug( $this->db->error );
								$this->logger->debug( $response );
							}
							else{
								$this->logger->debug( $sql3 );
								$this->logger->debug( $this->db->error );
											
								$response['response_status']['status']       = 1;
								$response['response_status']['error_code']   = $this->error_code . __LINE__;
								$response['response_status']['msg']          = 'Cadastro realizado com sucesso!';
							}
						}
					}

					//INSERT CNAE 

					// $cnaeId = explode(" ", trim($data['cnae-input']));
					$cnaeId = $data['cnae-input'];
					$this->logger->debug( $cnaeId );

					for ($i=0; $i < count($cnaeId); $i++) { 
						
						$sql6 = "INSERT INTO classe_cnae_doc_sind (doc_sind_id_doc, classe_cnae_id_cnae) VALUES ('{$lastId}', '{$cnaeId[$i]}')";
						$this->logger->debug( $sql6 );
						
						if( !mysqli_query( $this->db, $sql6 ) ){
								
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Erro ao cadastrar!';
							
							$this->logger->debug( $sql6 );
							$this->logger->debug( $this->db->error );
							$this->logger->debug( $response );
						}
						else{
							$response['response_status']['status']       = 1;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Cadastrado com sucesso';
						}
					}

					if ($data['referenciamento']) {
						//ADD REFERENCIAMENTO
						$ref = $data['referenciamento'];
						for ($i=0; $i < count($data['referenciamento']) ; $i++) { 
							$sqlRef = "INSERT INTO doc_sind_referencia 
										(doc_sind_id_doc, 
										estrutura_clausula_id_estruturaclausula)
									VALUES
										('{$lastId}', 
										'{$ref[$i]}')
							";

							if( !mysqli_query( $this->db, $sqlRef ) ){
															
								$response['response_status']['status']       = 0;
								$response['response_status']['error_code']   = $this->error_code . __LINE__;
								$response['response_status']['msg']          = 'Erro ao cadastrar referenciamento!';
								
								$this->logger->debug( $sql6 );
								$this->logger->debug( $this->db->error );
								$this->logger->debug( $response );
							}
							else{
								$response['response_status']['status']       = 1;
								$response['response_status']['error_code']   = $this->error_code . __LINE__;
								$response['response_status']['msg']          = 'Referenciamento adastrado com sucesso';
							}
						}
					}

					
				}else{
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Não foi possível realizar prosseguir com o cadastro pois o documento não foi cadastrado!';
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
	
	function updateDocSind( $data = null ){
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

			$dataIni = ($data['vini-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['vini-input'])))) : "0000-00-00" );
			$dataFim = ($data['vfim-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['vfim-input'])))) : "0000-00-00" );
			$dataPro = ($data['pro-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['pro-input'])))) : "0000-00-00" );
			$dataReg = ($data['data-reg-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['data-reg-input'])))) : "0000-00-00" );
			$dataAss = ($data['ass-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['ass-input'])))) : "0000-00-00" );
			$docRestrito = $data['doc_restrito'] == 'restrito' ? 'Sim' : 'Não';

			if( $response['response_status']['status'] == 1 ){
				$sql = "UPDATE doc_sind
						SET 
							tipounidade_cliente_id_tiponegocio = '{$data['type-un-input']}'
							,tipo_doc_idtipo_doc = '{$data['tipo-doc-input']}'
							,uf = '{$data['uf-input']}'
							,origem = '{$data['ori-input']}'
							,versao_documento = '{$data['versao-input']}'
							,numero_solicitacao_mr = '{$data['num-input']}'
							,data_reg_mte = '{$dataReg}'
							,num_reg_mte = '{$data['num-reg-input']}'
							,validade_inicial = '{$dataIni}'
							,validade_final = '{$dataFim}'
							,prorrogacao_doc = '{$dataPro}'
							,data_assinatura = '{$dataAss}'
							,observacao = '{$data['obs-input']}'
							,permissao = '{$data['perm-input']}'
							,doc_restrito = '{$docRestrito}'

						WHERE 
							id_doc = {$data['id_doc']};
						";
				
				$this->logger->debug( $sql );

				if( !mysqli_query( $this->db, $sql )){
					
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao cadastrar!';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{	
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Cadastrado com sucesso';
				}	
				
				// $this->logger->debug( $data['abrang-input'] ) ;

				//ADD ABRANGENCIA

				$abrang = explode(",", implode(",", array_unique(explode(" ", trim($data['abrang-input'])))));
				$this->logger->debug( $abrang );

				if (!empty($data['abrang-input'])) {
					for ($i=0; $i < count($abrang); $i++) { 
						$sql2 = "INSERT INTO abrang_docsind
									(localizacao_id_localizacao, doc_sind_id_documento)
								VALUES
									('{$abrang[$i]}', '{$data['id_doc']}')
							";
						$this->logger->debug( $sql2 );
	
						if( !mysqli_query( $this->db, $sql2 )){
						
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Erro ao cadastrar!';
							
							$this->logger->debug( $sql );
							$this->logger->debug( $this->db->error );
							$this->logger->debug( $response );
						}
						else{		
							$response['response_status']['status']       = 1;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Cadastrado com sucesso';
						}
	
					}
				}

				//UPDATE ITENS MULTIPLOS
				$cltUn = $data['cliente_unidade'];
				$cnae = $data['cnae'];
				$emp = $data['sind_emp'];
				$patr = $data['sind_patr'];
				$reference = $data['referenciamento'];
				$this->logger->debug( $data['tipo-doc-sigla'] );
				//UPDATE CLIENTE UNIDADE
				if ($data['tipo-doc-sigla']) {
					$sqlDel = "DELETE FROM doc_sind_cliente_unidades WHERE doc_sind_id_doc = '{$data['id_doc']}'";
					mysqli_query( $this->db, $sqlDel );
					
					if ($data['tipo-doc-sigla'] == "ACT") {
						for ($i=0; $i < count($cltUn) ; $i++) { 
							$sql = "INSERT INTO doc_sind_cliente_unidades 
										(doc_sind_id_doc, 
										cliente_unidades_id_unidade)
									VALUES 
										('{$data['id_doc']}',
										'{$cltUn[$i]}')	
							";

							if( !mysqli_query( $this->db, $sql ) ){
													
								$response['response_status']['status']       = 0;
								$response['response_status']['error_code']   = $this->error_code . __LINE__;
								$response['response_status']['msg']          = 'Erro ao realizar cadastro';
								
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
					}
					
				}
			
				
				
				//UPDATE CNAE
				$sqlDel = "DELETE FROM classe_cnae_doc_sind WHERE doc_sind_id_doc = '{$data['id_doc']}'";
				$resultDel = mysqli_query( $this->db, $sqlDel );
				
				for ($i=0; $i < count($cnae) ; $i++) { 
					$sql = "INSERT INTO classe_cnae_doc_sind 
							(doc_sind_id_doc, 
							classe_cnae_id_cnae)
						VALUES 
							('{$data['id_doc']}',
							'{$cnae[$i]}')	
					";

					if( !mysqli_query( $this->db, $sql ) ){
											
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Erro ao realizar cadastro';
						
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

				//UPDATE SIND EMP
			
				$sqlDel = "DELETE FROM doc_sind_sind_emp WHERE doc_sind_id_doc = '{$data['id_doc']}'";
				$resultDel = mysqli_query( $this->db, $sqlDel );
				
				for ($i=0; $i < count($emp) ; $i++) { 
					$sql = "INSERT INTO doc_sind_sind_emp 
							(doc_sind_id_doc, 
							sind_emp_id_sinde)
						VALUES 
							('{$data['id_doc']}',
							'{$emp[$i]}')	
					";

					if( !mysqli_query( $this->db, $sql ) ){
											
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Erro ao realizar cadastro';
						
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

				//UPDATE SIND PATR
			
				$sqlDel = "DELETE FROM doc_sind_sind_patr WHERE doc_sind_id_doc = '{$data['id_doc']}'";
				$resultDel = mysqli_query( $this->db, $sqlDel );
				
				for ($i=0; $i < count($patr) ; $i++) { 
					$sql = "INSERT INTO doc_sind_sind_patr 
							(doc_sind_id_doc, 
							sind_patr_id_sindp)
						VALUES 
							('{$data['id_doc']}',
							'{$patr[$i]}')	
					";

					if( !mysqli_query( $this->db, $sql ) ){
											
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Erro ao realizar cadastro';
						
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

				//REFERENCIAMENTO
				if (!empty($reference)) {
					$sqlDel = "DELETE FROM doc_sind_referencia WHERE doc_sind_id_doc = '{$data['id_doc']}'";
					mysqli_query( $this->db, $sqlDel );
					
					for ($i=0; $i < count($reference) ; $i++) { 
						$sql = "INSERT INTO doc_sind_referencia 
								(doc_sind_id_doc, 
								estrutura_clausula_id_estruturaclausula)
							VALUES 
								('{$data['id_doc']}',
								'{$reference[$i]}')	
						";

						if( !mysqli_query( $this->db, $sql ) ){
												
							$response['response_status']['status']       = 0;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Erro ao atualizar cadastro';
							
							$this->logger->debug( $sql );
							$this->logger->debug( $this->db->error );
							$this->logger->debug( $response );
						}
						else{
							$response['response_status']['status']       = 1;
							$response['response_status']['error_code']   = $this->error_code . __LINE__;
							$response['response_status']['msg']          = 'Cadastro atualizado com sucesso!';
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

	function getAbrangVerify( $data = null ){

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

				$sql = "SELECT * FROM abrang_docsind WHERE doc_sind_id_documento = '{$data['id_doc']}'";

				if( !$result = mysqli_query( $this->db, $sql ) ){
						
					$response['response_data']['abrangencia'] = 0;
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{
					$obj = $result->fetch_object();
					$response['response_data']['abrangencia'] = $obj->doc_sind_id_documento;
					$this->logger->debug( $obj->doc_sind_id_documento );
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

	function setUfAbrang($data = null) {

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
			$this->logger->debug(  $data['uf-input'] );
				
			if( $response['response_status']['status'] == 1 ){

				//LISTA LOCALIZAÇÃO
				$sql = "SELECT 
							id_localizacao, 
							municipio
						FROM 
							localizacao
						WHERE uf = '{$data['uf-input']}';				
						";

				$this->logger->debug(  $sql );

				if( $resultsql = mysqli_query( $this->db, $sql ) ){
					$this->logger->debug(  $resultsql );

					$list = "";
					// $update = "";
					$listaTable = [];
					while($obj = $resultsql->fetch_object()){
						$this->logger->debug(  $obj );
						$list .= '<tr class="tbl-item">';
						$list .= '<td><input class="form-check-input check" type="checkbox" value="1" id="'.$obj->id_localizacao.'"></td>';
						$list .= '<td class="title">';
						$list .= $obj->municipio;
						$list .= '</td>';
						$list .= '</tr>';
						
						$newObj = new stdClass();
						$newObj->campo = '<input class="form-check-input checkUpdate" type="checkbox" value="1" id="'.$obj->id_localizacao.'">';
						$newObj->municipio = $obj->municipio;
						array_push($listaTable, $newObj);
					}	


					$response['response_data']['list'] 	= $list;
					$response['response_data']['lista_update'] 	= $listaTable;

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
		
		$this->logger->debug( $response );
		$this->logger->debug( $this->db->error );
		
		return $response;	
	}

	function deleteAbrang($data = null) {

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

				//LISTA LOCALIZAÇÃO
				$sql = "DELETE FROM 
							abrang_docsind 
						WHERE 
							doc_sind_id_documento = '{$data['id_doc']}';				
						";

				$this->logger->debug(  $sql );

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
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response );
		$this->logger->debug( $this->db->error );
		
		return $response;	
	}

	function deleteAbrangencia( $data = null ){

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

				$this->logger->debug(  $data );
				$idLocal = $data['id_localizacao'];
				for ($i=0; $i < count($idLocal) ; $i++) { 
					$sql = "DELETE FROM abrang_docsind 
							WHERE doc_sind_id_documento = '{$data['id_doc'][0]}' AND localizacao_id_localizacao = '{$idLocal[$i]}'
					";

					if( !mysqli_query( $this->db, $sql ) ){
																			
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Erro ao excluir o registro!';
						
						$this->logger->debug( $sql );
						$this->logger->debug( $this->db->error );
						$this->logger->debug( $response );
					}
					else{
						$this->logger->debug( $sql );		
						$response['response_status']['status']       = 1;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Registro excluído com sucesso!';
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

	function getDataBase( $data = null ){

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

				if ($data['sind_emp'][0] != "") {
					$sql = "SELECT dataneg FROM base_territorialsindemp WHERE sind_empregados_id_sinde1 = '{$data['sind_emp'][0]}'";
					$result = mysqli_query( $this->db, $sql );
					$obj = $result->fetch_object();

					$dataMes = $obj->dataneg;
					$dataBase = $dataMes . "/" . date_format((new DateTime('now')),'Y');

					$response['response_data']['data_base'] = $dataBase;
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

	function getPermissaoDoc( $data = null ){

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

				$sql = "SELECT idtipo_doc, permissao FROM tipo_doc WHERE idtipo_doc = '{$data['id_tipo_doc']}'
				";

				$result = mysqli_query( $this->db, $sql );
				$obj = $result->fetch_object();

				$response['response_data']['permissao'] = $obj->permissao;

				

				
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

	function addDocumentFile( $data = null ){

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

				$fileName = strstr($data['file_name'], "-0065", true);
				$origem = ($data['origem'] == "" ? "Não informada" : $data['origem']);

				$this->logger->debug( $fileName );
				$this->logger->debug( $data['path'] );

				$path = "https://ineditta.com" . strstr($data['path'], "/documentos_sistema");

				$sql = "INSERT INTO documentos_localizados
							(nome_documento,
							origem,
							caminho)
						VALUES
							('{$fileName}',
							'{$origem}',
							'{$path}')
				";

				if( !mysqli_query( $this->db, $sql ) ){
										
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao realizar o cadastro do documento!';
					
					$this->logger->debug( $sql );
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{		
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Cadastrado com sucesso!';
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

	function deleteDocumentFile( $data = null ){

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

				$select = "SELECT * FROM documentos_localizados WHERE id_documento = '{$data['id_documento']}'";
				$result = mysqli_query( $this->db, $select );

				$obj = $result->fetch_object();

				$filePath = $obj->caminho;

				if (unlink($filePath)) {
					$this->logger->debug('Registro excluido' );

					$sql = "DELETE FROM documentos_localizados
							WHERE id_documento = '{$data['id_documento']}'
					";

					if( !mysqli_query( $this->db, $sql ) ){
															
						$response['response_status']['status']       = 0;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Erro ao excluir registro!';
						
						$this->logger->debug( $sql );
						$this->logger->debug( $this->db->error );
						$this->logger->debug( $response );
					}
					else{		
						$response['response_status']['status']       = 1;
						$response['response_status']['error_code']   = $this->error_code . __LINE__;
						$response['response_status']['msg']          = 'Registro excluido com sucesso!';
					}
				}else {
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao excluir arquivo!';
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

	function approveDocumentFile( $data = null ){

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

				// $today = date_format(new DateTime('now'), "Y-m-d");

				$sql = "UPDATE documentos_localizados
						SET situacao = 'aprovado', data_aprovacao = '".date_format(new DateTime('now'), "Y-m-d")."'
						 WHERE id_documento = '{$data['id_documento']}'
				";

				$this->logger->debug( $sql );

				if( !mysqli_query( $this->db, $sql ) ){
														
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao atualizar registro!';
					
					
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{		
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Registro atualizado com sucesso!';

					$sql = "SELECT 
								* 
							FROM documentos_localizados 
							WHERE id_documento = '{$data['id_documento']}'
					";

					$result = mysqli_query( $this->db, $sql );

					$obj = $result->fetch_object();

					$response['response_data']['objDoc'] = $obj;
					// $response['response_data']['path'] = strstr($obj->caminho, "storage/");
					$response['response_data']['path'] = $obj->caminho;
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

	function getDocumentoById( $data = null ){

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

				$select = "SELECT * FROM documentos_localizados WHERE id_documento = '{$data['id_documento']}'";
				$result = mysqli_query( $this->db, $select );

				$obj = $result->fetch_object();

				$response['response_data']['objDoc'] = $obj;
				// $response['response_data']['path'] = strstr($obj->caminho, "storage/");
				$response['response_data']['path'] = $obj->caminho;

				
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

	function formatCnpjCpf($value){
	$CPF_LENGTH = 11;
	$cnpj_cpf = preg_replace("/\D/", '', $value);
	
	if (strlen($cnpj_cpf) === $CPF_LENGTH) {
		return preg_replace("/(\d{3})(\d{3})(\d{3})(\d{2})/", "\$1.\$2.\$3-\$4", $cnpj_cpf);
	} 
	
	return preg_replace("/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/", "\$1.\$2.\$3/\$4-\$5", $cnpj_cpf);
	}

}

?>