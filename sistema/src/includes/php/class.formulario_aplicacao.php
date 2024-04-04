<?php
/**
 * @author    {Lucas A. Rodrigues Volpati}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2022-06-21 16:40 ( v1.0.0 ) - 
	}
**/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


class formulario_aplicacao{
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

				//LISTA PRINCIPAL
				$sqlList = "SELECT 
								id_notecliente,
								tipo_comentario,
								id_tipo_comentario,
								tipo_usuario_destino,
								id_tipo_usuario_destino,
								tipo_notificacao,
								DATE_FORMAT(data_final, '%d/%m/%Y') AS data_final,
								nc.usuario_adm_id_user,
								comentario,
								us.nome_usuario
							FROM 
								note_cliente as nc
							LEFT JOIN usuario_adm as us on us.id_user = nc.usuario_adm_id_user
							
				";

				$resultList = mysqli_query( $this->db, $sqlList );

				$html = "";
				while ($obj = $resultList->fetch_object()) {

					if ($obj->tipo_comentario == "laboral") {
						$tipo = "Sindicato Laboral";
					}elseif ($obj->tipo_comentario == "patronal") {
						$tipo = "Sindicato Patronal";
					}else {
						$tipo = "Cláusula";
					}

					if ($obj->tipo_usuario_destino == "grupo") {
						$tipoUser = "Grupo Econômico";
					}elseif ($obj->tipo_usuario_destino == "matriz") {
						$tipoUser = "Cliente Matriz";
					}else {
						$tipoUser = "Cliente Unidade";
					}

					$html .= '<tr class="odd gradeX tbl-item">';
					$html .= '<td><a onclick="getById('.$obj->id_notecliente.');" style="color: #000;" data-toggle="modal" href="#myModalUpdate" data-dismiss="modal"><i class="fa fa-file-text"></i></a></td>';
					$html .= '<td class="title">';
					$html .= '</td>';
					$html .= '<td class="desc">';
					$html .= '</td>';
					$html .= '<td>';
					$html .= '</td>';
					$html .= '<td>';
					$html .= '</td>';
					$html .= '<td>';
					$html .= '</td>';
					$html .= '<td>';
					$html .= '</td>';
					$html .= '<td>';
					$html .= '</td>';
					$html .= '</tr>';
				}

				$response['response_data']['listaPrincipalNotif'] = $html;
				$this->logger->debug(  $html );

				//LISTA SIND EMPREGADOS
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
							sind_emp;
				";

				$resultEmp = mysqli_query( $this->db, $sqlEmp );

				$html = "";
				while ($objEmp = $resultEmp->fetch_object()) {
					$html .= '<tr class="odd gradeX tbl-item">';
					$html .= '<td><a onclick="selectTipoComentario('.$objEmp->id_sinde.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-com" data-dismiss="modal">Selecionar</a></td>';
					$html .= '<td class="title">';
					$html .= $objEmp->id_sinde;
					$html .= '</td>';
					$html .= '<td class="title">';
					$html .= $objEmp->razaosocial_sinde;
					$html .= '</td>';
					$html .= '<td class="desc">';
					$html .= $objEmp->sigla_sinde;
					$html .= '</td>';
					$html .= '<td>';
					$html .= $objEmp->cnpj_sinde;
					$html .= '</td>';
					$html .= '<td>';
					$html .= $objEmp->email1_sinde;
					$html .= '</td>';
					$html .= '<td>';
					$html .= $objEmp->fone1_sinde;
					$html .= '</td>';
					$html .= '<td>';
					$html .= $objEmp->site_sinde;
					$html .= '</td>';
					$html .= '</tr>';
				}

				$response['response_data']['sindEmp'] = $html;


				//LISTA SIND PATRONAL
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
								sind_patr;		
				";

				$resultPatr = mysqli_query( $this->db, $sqlPatr );
				$htmlPatr = "";
				while ($objPatr = $resultPatr->fetch_object()) {
					$htmlPatr .= '<tr class="odd gradeX tbl-item">';
					$htmlPatr .= '<td><a onclick="selectTipoComentario('.$objPatr->id_sindp.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-com" data-dismiss="modal">Selecionar</a></td>';
					$htmlPatr .= '<td>';
					$htmlPatr .= $objPatr->id_sindp;
					$htmlPatr .= '</td>';
					$htmlPatr .= '<td class="title">';
					$htmlPatr .= $objPatr->razaosocial_sp;
					$htmlPatr .= '</td>';
					$htmlPatr .= '<td class="desc">';
					$htmlPatr .= $objPatr->sigla_sp;
					$htmlPatr .= '</td>';
					$htmlPatr .= '<td>';
					$htmlPatr .= $objPatr->cnpj_sp;
					$htmlPatr .= '</td>';
					$htmlPatr .= '<td>';
					$htmlPatr .= $objPatr->email1_sp;
					$htmlPatr .= '</td>';
					$htmlPatr .= '<td>';
					$htmlPatr .= $objPatr->fone1_sp;
					$htmlPatr .= '</td>';
					$htmlPatr .= '<td>';
					$htmlPatr .= $objPatr->site_sp;
					$htmlPatr .= '</td>';
					$htmlPatr .= '</tr>';
				}
				
				$response['response_data']['sindPatr'] = $htmlPatr;


				//LISTA CLAUSULA GERAL

				$sql4 = "SELECT 
							IFNULL(GROUP_CONCAT(cg.id_clau), GROUP_CONCAT(IFNULL( cg.id_clau,null))) as id_clau
							,cg.doc_sind_id_documento
							,(SELECT count(aprovado) FROM clausula_geral  WHERE aprovado = 'nao' AND cg.doc_sind_id_documento = doc_sind_id_documento) as nao_aprovado
							,(SELECT count(aprovado) FROM clausula_geral  WHERE aprovado = 'sim' AND cg.doc_sind_id_documento = doc_sind_id_documento) as aprovado
						FROM	clausula_geral as cg 
						GROUP BY cg.doc_sind_id_documento;
				";

				$this->logger->debug(  $sql4 );

				$resultsql = mysqli_query( $this->db, $sql4 );
				$htmlClau = null;
				$cont = 0;

				while($obj = $resultsql->fetch_object()){
					$htmlClau .= '<tr class="odd gradeX tbl-item">';
					$htmlClau .= '<td><a data-toggle="modal" data-dismiss="modal" href="#myModalListClausulasByDocSind" onclick="getClausulaByDoc('.$obj->doc_sind_id_documento.');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';//onclick="getByIdClausula( '.$obj->doc_sind_id_documento.');" 
					$htmlClau .= '<td class="title">';
					$htmlClau .= $obj->doc_sind_id_documento;
					$htmlClau .= '</td>';
					$htmlClau .= '<td class="desc">';
					$htmlClau .= $obj->aprovado;
					$htmlClau .= '</td>';
					$htmlClau .= '<td>';
					$htmlClau .= $obj->nao_aprovado;
					$htmlClau .= '</td>';
					$htmlClau .= '</tr>';

					$cont += 1;
				}	

				$response['response_data']['listaPrincipal'] = $htmlClau;
				

				//LISTA GRUPO ECONOMICO
				$sql = "SELECT 
							id_grupo_economico
							,nome_grupoeconomico
							,logo_grupo
						FROM 
							cliente_grupo;				
				";
				
				$this->logger->debug(  $sql );

				$resultsql = mysqli_query( $this->db, $sql );
				$htmlGrupo = null;
				
				while($obj = $resultsql->fetch_object()){
					$htmlGrupo .= '<tr class="odd gradeX tbl-item">';
					$htmlGrupo .= '<td><a onclick="selectTipoUsuario('.$obj->id_grupo_economico.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-user" data-dismiss="modal">Selecionar</a></td>';
					$htmlGrupo .= '<td>';
					$htmlGrupo .= $obj->id_grupo_economico;
					$htmlGrupo .= '</td>';
					$htmlGrupo .= '<td class="title">';
					$htmlGrupo .= $obj->nome_grupoeconomico;
					$htmlGrupo .= '</td>';
					$htmlGrupo .= '<td class="desc"> <img src="'.$obj->logo_grupo.'" height="200" alt="Image preview...">';
					$htmlGrupo .= '</td>';
					$htmlGrupo .= '</tr>';
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
				
				$this->logger->debug(  $sql );

				$resultsql = mysqli_query( $this->db, $sql );
				$listMatriz = null;
				
				while($obj = $resultsql->fetch_object()){
					$listMatriz .= '<tr class="odd gradeX tbl-item">';
					$listMatriz .= '<td><a onclick="selectTipoUsuario('.$obj->id_empresa.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-user" data-dismiss="modal">Selecionar</a></td>';
					$listMatriz .= '<td>';
					$listMatriz .= $obj->id_empresa;
					$listMatriz .= '</td>';
					$listMatriz .= '<td class="desc">';
					$listMatriz .= $obj->grupo_economico;
					$listMatriz .= '</td>';
					$listMatriz .= '<td class="title">';
					$listMatriz .= $obj->nome_empresa;
					$listMatriz .= '</td>';
					$listMatriz .= '<td>';
					$listMatriz .= $obj->cnpj_empresa;
					$listMatriz .= '</td>';
					$listMatriz .= '<td>';
					$listMatriz .= $obj->data_inclusao;
					$listMatriz .= '</td>';
					$listMatriz .= '<td>';
					$listMatriz .= $obj->data_inativacao;
					$listMatriz .= '</td>';
					$listMatriz .= '<td>';
					$listMatriz .= $obj->cidade;
					$listMatriz .= '</td>';
					$listMatriz .= '<td>';
					$listMatriz .= $obj->uf;
					$listMatriz .= '</td>';
					$listMatriz .= '</tr>';

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
				
				$this->logger->debug(  $sql );

				$resultsql = mysqli_query( $this->db, $sql );
				$listUn = null;
				
				while($obj = $resultsql->fetch_object()){
					$listUn .= '<tr class="odd gradeX tbl-item">';
					$listUn .= '<td><a onclick="selectTipoUsuario('.$obj->id_unidade.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-user" data-dismiss="modal">Selecionar</a></td>';
					$listUn .= '<td>';
					$listUn .= $obj->id_unidade;
					$listUn .= '</td>';
					$listUn .= '<td class="title">';
					$listUn .= $obj->nome_unidade;
					$listUn .= '</td>';
					$listUn .= '<td>';
					$listUn .= $obj->cnpj_unidade;
					$listUn .= '</td>';
					$listUn .= '<td class="desc">';
					$listUn .= $obj->nome_empresa;
					$listUn .= '</td>';
					$listUn .= '<td>';
					$listUn .= $obj->data_inclusao;
					$listUn .= '</td>';
					$listUn .= '<td>';
					$listUn .= $obj->tipo_negocio;
					$listUn .= '</td>';
					$listUn .= '</tr>';
				}	

				$response['response_data']['listaUnidade'] 	= $listUn;
					
				
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
}
?>
