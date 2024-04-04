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


include_once "helpers.php";

class negociacao{
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
	
	// function connectdb(){
		
	// 	if( $this->response['response_status']['status'] == 1 ){
			
	// 		// Carregando a resposta padrão da função
	// 		$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
	// 		$qualitor_db = $this->getconfig->searchConfigDatabase( 'ineditta' );
				
	// 		if( $qualitor_db['response_status']['status'] == 1 ){
				
	// 			$parameters = $qualitor_db['response_data'];

	// 			if( file_exists( $this->path . '/includes/php/db.mysql.php' ) ){
				
	// 				include_once( $this->path . '/includes/php/db.mysql.php');

	// 				// Criando o objeto de conexão com o banco de dados Qualitor
	// 				$apidbmysql = new apidbmysql();

	// 				$db = $apidbmysql->connection($parameters);
					
	// 				if( $db['response_status']['status'] == 1 ){
						
	// 					$this->db = $db['response_data']['connection'];
						
	// 					$this->logger->debug( $db['response_data']['connection'] );
						
	// 				}
	// 				else{
	// 					$response = $db;
	// 				}
	// 			}
	// 			else{
	// 				$response['response_status']['status']     = 0;
	// 				$response['response_status']['error_code'] = $this->error_code . __LINE__;
	// 				$response['response_status']['msg']        = 'Não foi possível encontrar o db.mysql.';
	// 			}		
	// 		}
	// 		else{
	// 			$response =  $qualitor_db;
	// 		}
	// 	}
	// 	else{
	// 		$response = $this->response;
	// 	}
		
	// 	return $response;
	// }
	
	// function getLists( $data = null ){

	// 	if( $this->response['response_status']['status'] == 1 ){
			
	// 		// Carregando a resposta padrão da função
	// 		$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
	// 		if( $response['response_status']['status'] == 1 ){
	// 			if( empty( $this->db ) ){
	// 				$connectdb = $this->connectdb();
					
	// 				if( $connectdb['response_status']['status'] == 0 ){
	// 					$response = $connectdb;
	// 				}
	// 			}
	// 		}
 
	// 		$this->logger->debug(  $connectdb );
 
	// 		if( $response['response_status']['status'] == 1 ){

	// 			//LISTA PRINCIPAL
	// 			$sqlList = "SELECT 
	// 							id_notecliente,
	// 							tipo_comentario,
	// 							id_tipo_comentario,
	// 							tipo_usuario_destino,
	// 							id_tipo_usuario_destino,
	// 							tipo_notificacao,
	// 							DATE_FORMAT(data_final, '%d/%m/%Y') AS data_final,
	// 							nc.usuario_adm_id_user,
	// 							comentario,
	// 							us.nome_usuario
	// 						FROM 
	// 							note_cliente as nc
	// 						LEFT JOIN usuario_adm as us on us.id_user = nc.usuario_adm_id_user
							
	// 			";

	// 			$resultList = mysqli_query( $this->db, $sqlList );

	// 			$html = "";
	// 			while ($obj = $resultList->fetch_object()) {

	// 				if ($obj->tipo_comentario == "laboral") {
	// 					$tipo = "Sindicato Laboral";
	// 				}elseif ($obj->tipo_comentario == "patronal") {
	// 					$tipo = "Sindicato Patronal";
	// 				}else {
	// 					$tipo = "Cláusula";
	// 				}

	// 				if ($obj->tipo_usuario_destino == "grupo") {
	// 					$tipoUser = "Grupo Econômico";
	// 				}elseif ($obj->tipo_usuario_destino == "matriz") {
	// 					$tipoUser = "Cliente Matriz";
	// 				}else {
	// 					$tipoUser = "Cliente Unidade";
	// 				}

	// 				$html .= '<tr class="odd gradeX tbl-item">';
	// 				$html .= '<td><a onclick="getById('.$obj->id_notecliente.');" style="color: #000;" data-toggle="modal" href="#myModalUpdate" data-dismiss="modal"><i class="fa fa-file-text"></i></a></td>';
	// 				$html .= '<td class="title">';
	// 				$html .= $tipo;
	// 				$html .= '</td>';
	// 				$html .= '<td class="desc">';
	// 				$html .= $tipoUser;
	// 				$html .= '</td>';
	// 				$html .= '<td>';
	// 				$html .= ($obj->tipo_notificacao == "fixa" ? "Fixa" : "Temporária");
	// 				$html .= '</td>';
	// 				$html .= '<td>';
	// 				$html .= ($obj->tipo_notificacao != "temporaria" ? "--" : $obj->data_final);
	// 				$html .= '</td>';
	// 				$html .= '<td>';
	// 				$html .= ($obj->usuario_adm_id_user == "" ? "--" : $obj->usuario_adm_id_user . " - " . $obj->nome_usuario);
	// 				$html .= '</td>';
	// 				$html .= '<td>';
	// 				$html .= $obj->comentario;
	// 				$html .= '</td>';
	// 				$html .= '</tr>';
	// 			}

	// 			$response['response_data']['listaPrincipalNotif'] = $html;
	// 			$this->logger->debug(  $html );

	// 			//LISTA SIND EMPREGADOS
	// 			$sqlEmp = "SELECT 
	// 						id_sinde
	// 						,razaosocial_sinde
	// 						,sigla_sinde
	// 						,cnpj_sinde
    //                         ,logradouro_sinde
    //                         ,email1_sinde
    //                         ,fone1_sinde
    //                         ,site_sinde
	// 					FROM 
	// 						sind_emp;
	// 			";

	// 			$resultEmp = mysqli_query( $this->db, $sqlEmp );

	// 			$html = "";
	// 			while ($objEmp = $resultEmp->fetch_object()) {
	// 				$html .= '<tr class="odd gradeX tbl-item">';
	// 				$html .= '<td><a onclick="selectTipoComentario('.$objEmp->id_sinde.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-com" data-dismiss="modal">Selecionar</a></td>';
	// 				$html .= '<td class="title">';
	// 				$html .= $objEmp->id_sinde;
	// 				$html .= '</td>';
	// 				$html .= '<td class="title">';
	// 				$html .= $objEmp->razaosocial_sinde;
	// 				$html .= '</td>';
	// 				$html .= '<td class="desc">';
	// 				$html .= $objEmp->sigla_sinde;
	// 				$html .= '</td>';
	// 				$html .= '<td>';
	// 				$html .= $objEmp->cnpj_sinde;
	// 				$html .= '</td>';
	// 				$html .= '<td>';
	// 				$html .= $objEmp->email1_sinde;
	// 				$html .= '</td>';
	// 				$html .= '<td>';
	// 				$html .= $objEmp->fone1_sinde;
	// 				$html .= '</td>';
	// 				$html .= '<td>';
	// 				$html .= $objEmp->site_sinde;
	// 				$html .= '</td>';
	// 				$html .= '</tr>';
	// 			}

	// 			$response['response_data']['sindEmp'] = $html;


	// 			//LISTA SIND PATRONAL
	// 			$sqlPatr = "SELECT 
	// 							id_sindp
	// 							,razaosocial_sp
	// 							,sigla_sp
	// 							,cnpj_sp
	// 							,logradouro_sp
	// 							,email1_sp
	// 							,fone1_sp
	// 							,site_sp
	// 						FROM 
	// 							sind_patr;		
	// 			";

	// 			$resultPatr = mysqli_query( $this->db, $sqlPatr );
	// 			$htmlPatr = "";
	// 			while ($objPatr = $resultPatr->fetch_object()) {
	// 				$htmlPatr .= '<tr class="odd gradeX tbl-item">';
	// 				$htmlPatr .= '<td><a onclick="selectTipoComentario('.$objPatr->id_sindp.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-com" data-dismiss="modal">Selecionar</a></td>';
	// 				$htmlPatr .= '<td>';
	// 				$htmlPatr .= $objPatr->id_sindp;
	// 				$htmlPatr .= '</td>';
	// 				$htmlPatr .= '<td class="title">';
	// 				$htmlPatr .= $objPatr->razaosocial_sp;
	// 				$htmlPatr .= '</td>';
	// 				$htmlPatr .= '<td class="desc">';
	// 				$htmlPatr .= $objPatr->sigla_sp;
	// 				$htmlPatr .= '</td>';
	// 				$htmlPatr .= '<td>';
	// 				$htmlPatr .= $objPatr->cnpj_sp;
	// 				$htmlPatr .= '</td>';
	// 				$htmlPatr .= '<td>';
	// 				$htmlPatr .= $objPatr->email1_sp;
	// 				$htmlPatr .= '</td>';
	// 				$htmlPatr .= '<td>';
	// 				$htmlPatr .= $objPatr->fone1_sp;
	// 				$htmlPatr .= '</td>';
	// 				$htmlPatr .= '<td>';
	// 				$htmlPatr .= $objPatr->site_sp;
	// 				$htmlPatr .= '</td>';
	// 				$htmlPatr .= '</tr>';
	// 			}
				
	// 			$response['response_data']['sindPatr'] = $htmlPatr;


	// 			//LISTA CLAUSULA GERAL

	// 			$sql4 = "SELECT 
	// 						IFNULL(GROUP_CONCAT(cg.id_clau), GROUP_CONCAT(IFNULL( cg.id_clau,null))) as id_clau
	// 						,cg.doc_sind_id_documento
	// 						,(SELECT count(aprovado) FROM clausula_geral  WHERE aprovado = 'nao' AND cg.doc_sind_id_documento = doc_sind_id_documento) as nao_aprovado
	// 						,(SELECT count(aprovado) FROM clausula_geral  WHERE aprovado = 'sim' AND cg.doc_sind_id_documento = doc_sind_id_documento) as aprovado
	// 					FROM	clausula_geral as cg 
	// 					GROUP BY cg.doc_sind_id_documento;
	// 			";

	// 			$this->logger->debug(  $sql4 );

	// 			$resultsql = mysqli_query( $this->db, $sql4 );
	// 			$htmlClau = null;
	// 			$cont = 0;

	// 			while($obj = $resultsql->fetch_object()){
	// 				$htmlClau .= '<tr class="odd gradeX tbl-item">';
	// 				$htmlClau .= '<td><a data-toggle="modal" data-dismiss="modal" href="#myModalListClausulasByDocSind" onclick="getClausulaByDoc('.$obj->doc_sind_id_documento.');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';//onclick="getByIdClausula( '.$obj->doc_sind_id_documento.');" 
	// 				$htmlClau .= '<td class="title">';
	// 				$htmlClau .= $obj->doc_sind_id_documento;
	// 				$htmlClau .= '</td>';
	// 				$htmlClau .= '<td class="desc">';
	// 				$htmlClau .= $obj->aprovado;
	// 				$htmlClau .= '</td>';
	// 				$htmlClau .= '<td>';
	// 				$htmlClau .= $obj->nao_aprovado;
	// 				$htmlClau .= '</td>';
	// 				$htmlClau .= '</tr>';

	// 				$cont += 1;
	// 			}	

	// 			$response['response_data']['listaPrincipal'] = $htmlClau;
				

	// 			//LISTA GRUPO ECONOMICO
	// 			$sql = "SELECT 
	// 						id_grupo_economico
	// 						,nome_grupoeconomico
	// 						,logo_grupo
	// 					FROM 
	// 						cliente_grupo;				
	// 			";
				
	// 			$this->logger->debug(  $sql );

	// 			$resultsql = mysqli_query( $this->db, $sql );
	// 			$htmlGrupo = null;
				
	// 			while($obj = $resultsql->fetch_object()){
	// 				$htmlGrupo .= '<tr class="odd gradeX tbl-item">';
	// 				$htmlGrupo .= '<td><a onclick="selectTipoUsuario('.$obj->id_grupo_economico.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-user" data-dismiss="modal">Selecionar</a></td>';
	// 				$htmlGrupo .= '<td>';
	// 				$htmlGrupo .= $obj->id_grupo_economico;
	// 				$htmlGrupo .= '</td>';
	// 				$htmlGrupo .= '<td class="title">';
	// 				$htmlGrupo .= $obj->nome_grupoeconomico;
	// 				$htmlGrupo .= '</td>';
	// 				$htmlGrupo .= '<td class="desc"> <img src="'.$obj->logo_grupo.'" height="200" alt="Image preview...">';
	// 				$htmlGrupo .= '</td>';
	// 				$htmlGrupo .= '</tr>';
	// 			}	

	// 			$response['response_data']['listaGrupo'] 	= $htmlGrupo;


	// 			//LISTA CLIENTE MATRIZ

	// 			$sql = "
	// 			SELECT 
	// 						cm.id_empresa AS id_empresa
	// 						,cm.nome_empresa AS nome_empresa
	// 						,cm.cnpj_empresa AS cnpj_empresa
	// 						,cm.cidade AS cidade
	// 						,cm.uf AS uf
	// 						,cm.tip_doc AS tip_doc
    //                         ,DATE_FORMAT(cm.data_inclusao,'%d/%m/%Y') AS data_inclusao
	// 						,DATE_FORMAT(cm.data_inativacao,'%d/%m/%Y') AS data_inativacao
    //                         ,gp.nome_grupoeconomico as grupo_economico
	// 					FROM 
	// 						cliente_matriz cm
	// 					INNER JOIN cliente_grupo gp WHERE gp.id_grupo_economico = cm.cliente_grupo_id_grupo_economico;								
	// 			";
				
	// 			$this->logger->debug(  $sql );

	// 			$resultsql = mysqli_query( $this->db, $sql );
	// 			$listMatriz = null;
				
	// 			while($obj = $resultsql->fetch_object()){
	// 				$listMatriz .= '<tr class="odd gradeX tbl-item">';
	// 				$listMatriz .= '<td><a onclick="selectTipoUsuario('.$obj->id_empresa.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-user" data-dismiss="modal">Selecionar</a></td>';
	// 				$listMatriz .= '<td>';
	// 				$listMatriz .= $obj->id_empresa;
	// 				$listMatriz .= '</td>';
	// 				$listMatriz .= '<td class="desc">';
	// 				$listMatriz .= $obj->grupo_economico;
	// 				$listMatriz .= '</td>';
	// 				$listMatriz .= '<td class="title">';
	// 				$listMatriz .= $obj->nome_empresa;
	// 				$listMatriz .= '</td>';
	// 				$listMatriz .= '<td>';
	// 				$listMatriz .= $obj->cnpj_empresa;
	// 				$listMatriz .= '</td>';
	// 				$listMatriz .= '<td>';
	// 				$listMatriz .= $obj->data_inclusao;
	// 				$listMatriz .= '</td>';
	// 				$listMatriz .= '<td>';
	// 				$listMatriz .= $obj->data_inativacao;
	// 				$listMatriz .= '</td>';
	// 				$listMatriz .= '<td>';
	// 				$listMatriz .= $obj->cidade;
	// 				$listMatriz .= '</td>';
	// 				$listMatriz .= '<td>';
	// 				$listMatriz .= $obj->uf;
	// 				$listMatriz .= '</td>';
	// 				$listMatriz .= '</tr>';

	// 			}	

	// 			$response['response_data']['listaMatriz'] 	= $listMatriz;

				
	// 			//LISTA CLIENTE UNIDADE

	// 			$sql = "SELECT 
	// 						cu.id_unidade AS id_unidade
	// 						,cu.nome_unidade AS nome_unidade
	// 						,cu.cnpj_unidade AS cnpj_unidade
    //                         ,DATE_FORMAT(cu.data_inclusao,'%d/%m/%Y') AS data_inclusao
    //                         ,cm.nome_empresa as nome_empresa
	// 						,tuc.tipo_negocio as tipo_negocio
	// 					FROM 
	// 						cliente_unidades cu
	// 					INNER JOIN cliente_matriz cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
	// 					INNER JOIN tipounidade_cliente tuc on tuc.id_tiponegocio = cu.tipounidade_cliente_id_tiponegocio;
	// 			";
				
	// 			$this->logger->debug(  $sql );

	// 			$resultsql = mysqli_query( $this->db, $sql );
	// 			$listUn = null;
				
	// 			while($obj = $resultsql->fetch_object()){
	// 				$listUn .= '<tr class="odd gradeX tbl-item">';
	// 				$listUn .= '<td><a onclick="selectTipoUsuario('.$obj->id_unidade.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-user" data-dismiss="modal">Selecionar</a></td>';
	// 				$listUn .= '<td>';
	// 				$listUn .= $obj->id_unidade;
	// 				$listUn .= '</td>';
	// 				$listUn .= '<td class="title">';
	// 				$listUn .= $obj->nome_unidade;
	// 				$listUn .= '</td>';
	// 				$listUn .= '<td>';
	// 				$listUn .= $obj->cnpj_unidade;
	// 				$listUn .= '</td>';
	// 				$listUn .= '<td class="desc">';
	// 				$listUn .= $obj->nome_empresa;
	// 				$listUn .= '</td>';
	// 				$listUn .= '<td>';
	// 				$listUn .= $obj->data_inclusao;
	// 				$listUn .= '</td>';
	// 				$listUn .= '<td>';
	// 				$listUn .= $obj->tipo_negocio;
	// 				$listUn .= '</td>';
	// 				$listUn .= '</tr>';
	// 			}	

	// 			$response['response_data']['listaUnidade'] 	= $listUn;
					
				
	// 		}
	// 		else{
	// 			$response = $this->response;
	// 		}			
	// 	}
	// 	else{
	// 		$response = $this->response;
	// 	}
		
	// 	return $response;
	// }


	// function getById( $data = null ){
 
	// 	if( $this->response['response_status']['status'] == 1 ){
			
	// 		// Carregando a resposta padrão da função
	// 		$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
	// 		if( $response['response_status']['status'] == 1 ){
	// 			if( empty( $this->db ) ){
	// 				$connectdb = $this->connectdb();
					
	// 				if( $connectdb['response_status']['status'] == 0 ){
	// 					$response = $connectdb;
	// 				}
	// 			}
	// 		}
  
	// 		if( $response['response_status']['status'] == 1 ){

				
	// 			$sql = "SELECT
	// 						*,
	// 						us.nome_usuario
	// 					FROM
	// 						note_cliente nc
	// 					LEFT JOIN usuario_adm as us on us.id_user = nc.usuario_adm_id_user
	// 					WHERE
	// 						id_notecliente = '{$data['id']}'			
	// 				";

	// 			$this->logger->debug( $sql);
				
	// 			if( $resultsql = mysqli_query( $this->db, $sql ) ){
					
	// 				$obj = $resultsql->fetch_object();

	// 				$this->logger->debug( $obj);

	// 				$assunto = '';

	// 				//ASSUNTO DO COMENTÁRIO
	// 				if ($obj->tipo_comentario == "clausula") {
	// 					$sql2 = "SELECT
	// 							cg.id_clau,
	// 							cg.estrutura_id_estruturaclausula,
	// 							cg.tex_clau,
	// 							est.nome_clausula
								
	// 						FROM clausula_geral as cg
	// 						LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cg.estrutura_id_estruturaclausula
	
	// 						WHERE cg.id_clau = {$obj->id_tipo_comentario}		
	
	// 					";
	
	// 					$result2 = mysqli_query($this->db, $sql2);
	
	// 					$obj2 = $result2->fetch_object();

	// 					$assunto = $obj2->nome_clausula;
	
	// 				}else if($obj->tipo_comentario == "laboral") {
	// 					$sql2 = "SELECT
	// 								id_sinde,
	// 								sigla_sinde,
	// 								denominacao_sinde
									
	// 							FROM sind_emp
	// 							WHERE id_sinde = {$obj->id_tipo_comentario}
	
	// 					";
	
	// 					$result2 = mysqli_query($this->db, $sql2);

	// 					$obj2 = $result2->fetch_object();

	// 					$assunto = "{$obj2->sigla_sinde} - {$obj2->denominacao_sinde}";
	// 				}else {
	// 					$sql2 = "SELECT
	// 								id_sindp,
	// 								sigla_sp,
	// 								denominacao_sp
									
	// 							FROM sind_patr
	// 							WHERE id_sindp = {$obj->id_tipo_comentario}

	// 					";
	
	// 					$result2 = mysqli_query($this->db, $sql2);
	
	// 					$obj2 = $result2->fetch_object();

	// 					$assunto = "{$obj2->sigla_sp} - {$obj2->denominacao_sp}";
	// 				}

	// 				//USUÁRIO DESTINO DO COMENTÁRIO
	// 				if ($obj->tipo_usuario_destino == "grupo") {
	// 					$sql3 = "SELECT
	// 								nome_grupoeconomico
	// 							FROM cliente_grupo
	// 							WHERE id_grupo_economico = {$obj->id_tipo_usuario_destino}
	// 					";

	// 					$result3 = mysqli_query($this->db, $sql3);

	// 					$obj3 = $result3->fetch_object();

	// 					$destino = $obj3->nome_grupoeconomico;

	// 				}else if($obj->tipo_usuario_destino == "matriz") {
	// 					$sql3 = "SELECT
	// 								nome_empresa
	// 							FROM cliente_matriz
	// 							WHERE id_empresa = {$obj->id_tipo_usuario_destino}
	// 					";

	// 					$result3 = mysqli_query($this->db, $sql3);

	// 					$obj3 = $result3->fetch_object();

	// 					$destino = $obj3->nome_empresa;
	// 				}else {
	// 					$sql3 = "SELECT
	// 								nome_unidade
	// 							FROM cliente_unidades
	// 							WHERE id_unidade = {$obj->id_tipo_usuario_destino}
	// 					";

	// 					$result3 = mysqli_query($this->db, $sql3);

	// 					$obj3 = $result3->fetch_object();

	// 					$destino = $obj3->nome_unidade;

	// 				}

	// 				$response['response_data']['tipo_com'] = ucfirst($obj->tipo_comentario);
	// 				$response['response_data']['assunto_up'] = $assunto;
	// 				$response['response_data']['tipo_usuario_destino'] = ucfirst($obj->tipo_usuario_destino);
	// 				$response['response_data']['destino'] = $destino;
	// 				$response['response_data']['tipo_notificacao'] = $obj->tipo_notificacao;
	// 				$response['response_data']['data_final'] = ($obj->data_final == "" ? "--" : $obj->data_final);
	// 				$response['response_data']['usuario'] = ($obj->nome_usuario == "" ? "--" : $obj->nome_usuario);
	// 				$response['response_data']['comentario'] = $obj->comentario;
						
	// 			}
	// 			else{
	// 				$this->logger->debug( $sql );
	// 				$this->logger->debug( $this->db->error );
								
	// 				$response['response_status']['status']       = 0;
	// 				$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 				$response['response_status']['msg']          = '';
	// 			}
	// 		}
	// 		else{
	// 			$response = $this->response;
	// 		}			
	// 	}
	// 	else{
	// 		$response = $this->response;
	// 	}
		
	// 	$this->logger->debug( $response['response_status']['status'] );
	// 	$this->logger->debug( $response['response_data'] );
		
	// 	return $response;
	// }	


	// function getByDocClausula( $data = null ){
 
	// 	if( $this->response['response_status']['status'] == 1 ){
			
	// 		// Carregando a resposta padrão da função
	// 		$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
	// 		if( $response['response_status']['status'] == 1 ){
	// 			if( empty( $this->db ) ){
	// 				$connectdb = $this->connectdb();
					
	// 				if( $connectdb['response_status']['status'] == 0 ){
	// 					$response = $connectdb;
	// 				}
	// 			}
	// 		}
  
	// 		if( $response['response_status']['status'] == 1 ){

	// 			$sql = "SELECT 
	// 						cg.id_clau as id_clau
	// 						,cg.doc_sind_id_documento
	// 						, cg.tex_clau as tex_clau
	// 						,cg.aprovado
	// 						,date_format(data_aprovacao, '%d/%m/%Y %H:%i') as data_aprovacao
	// 					FROM clausula_geral as cg 
						
	// 					LEFT JOIN clausula_geral_estrutura_clausula as ad on cg.id_clau = ad.clausula_geral_id_clau
	// 					LEFT JOIN estrutura_clausula as est on ad.estrutura_clausula_id_estruturaclausula = est.id_estruturaclausula
	// 					WHERE cg.doc_sind_id_documento = '{$data['id_doc']}'
	// 					GROUP BY cg.id_clau;	
	// 			";
				
	// 			if( $resultsql = mysqli_query( $this->db, $sql ) ){
					

	// 				$listClausula = [];

	// 				while ($obj = $resultsql->fetch_object()) {

	// 					if ($obj->aprovado == "sim") {
	// 						$aprovada = '<i class="fa fa-check" style="color: green;"></i>';
	// 					}else {
	// 						$aprovada = '<i class="fa fa-ban" style="color: red;"></i>';
	// 					}

	// 					$line = '<a onclick="selectTipoComentario('.$obj->id_clau.');" data-toggle="modal" type="button" class="btn btn-secondary btn-tipo-com" data-dismiss="modal">Selecionar</a>';

	// 					$obj->aprovada = $aprovada;
	// 					$obj->button = $line;
	// 					array_push($listClausula, $obj);
	// 				}

	// 				$response['response_data']['lista_clausulas'] = $listClausula;

	// 				$this->logger->debug( $listClausula);
	// 				$response['response_status']['status']       = 1;
	// 				$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 				$response['response_status']['msg']          = 'Query realizada com sucesso.';
				                   	
	// 			}
	// 			else{
	// 				$this->logger->debug( $sql );
	// 				$this->logger->debug( $this->db->error );
								
	// 				$response['response_status']['status']       = 0;
	// 				$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 				$response['response_status']['msg']          = 'A query não retornou dados, verificar!';
	// 			}
	// 		}else {

	// 			$response['response_status']['status']       = 0;
	// 			$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 			$response['response_status']['msg']          = 'Não foi possível realizar a busca no banco de dados';
	// 		}
	// 	}else {
	// 		$response['response_status']['status']       = 0;
	// 		$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 		$response['response_status']['msg']          = 'Não foi possível iniciar a requisição getByDocClausula';

	// 	}

	// 	$this->logger->debug( $response);

	// 	return $response;
	// }


	// function addNotificacao( $data = null ){
	// 	$this->logger->debug(  'entrou na classe php' );
	// 	if( $this->response['response_status']['status'] == 1 ){
			
	// 		// Carregando a resposta padrão da função
	// 		$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
	// 		if( $response['response_status']['status'] == 1 ){
	// 			if( empty( $this->db ) ){
	// 				$connectdb = $this->connectdb();
					
	// 				if( $connectdb['response_status']['status'] == 0 ){
	// 					$response = $connectdb;
	// 				}
	// 			}
	// 		}


	// 		if( $response['response_status']['status'] == 1 ){

	// 			$data_final = ($data['validade'] == "" ? "0000-00-00" : $data['validade']);

	// 			$usuario = trim(strstr($data['usuario'], "-", true));
				
	// 			$sql = "INSERT INTO note_cliente
	// 						(tipo_comentario,
	// 						id_tipo_comentario,
	// 						tipo_usuario_destino,
	// 						id_tipo_usuario_destino,
	// 						tipo_notificacao,
	// 						data_final,
	// 						usuario_adm_id_user,
	// 						comentario)
	// 					VALUES
	// 						('{$data['tipo_com']}',
	// 						'{$data['tipo_com_selected']}',
	// 						'{$data['destino']}',
	// 						'{$data['id_destino']}',
	// 						'{$data['tipo_note']}',
	// 						'{$data_final}',
	// 						'{$usuario}',
	// 						'{$data['comentario']}')
	// 			";

	// 			$this->logger->debug( $sql );

	// 			if( !mysqli_query( $this->db, $sql )  ){
											
	// 				$response['response_status']['status']       = 0;
	// 				$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 				$response['response_status']['msg']          = 'Falha ao efetuar registro.';
					
	// 				$this->logger->debug( $this->db->error );
	// 			}
	// 			else{	 
	// 				$response['response_status']['status']       = 1;
	// 				$response['response_status']['error_code']   = null;
	// 				$response['response_status']['msg']          = 'Cadastro realizado com sucesso';
	// 			}

	// 		}
	// 		else{
	// 			$response = $this->response;
	// 		}	
	// 	}
	// 	else{
	// 		$response = $this->response;
	// 	}
		
	// 	$this->logger->debug( $response['response_status']['status'] );
		
	// 	return $response;
	// }
	
	
	// function updateNotificacao( $data = null ){

	// 	if( $this->response['response_status']['status'] == 1 ){
			
	// 		// Carregando a resposta padrão da função
	// 		$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
	// 		if( $response['response_status']['status'] == 1 ){
	// 			if( empty( $this->db ) ){
	// 				$connectdb = $this->connectdb();
					
	// 				if( $connectdb['response_status']['status'] == 0 ){
	// 					$response = $connectdb;
	// 				}
	// 			}
	// 		}
 
	// 		$this->logger->debug( $data['validade'] );

	// 		// if( $response['response_status']['status'] == 1 ){

	// 			$date = !$data['validade'] ? "0000-00-00" : $data['validade'];

	// 			$sql = "UPDATE 
	// 						note_cliente
	// 					SET  
	// 						tipo_notificacao = '{$data['tipo_note']}',
	// 						data_final = '{$date}',
	// 						comentario = '{$data['comentario']}'
	// 					WHERE 
	// 						id_notecliente = {$data['id']};
	// 			";

	// 			$this->logger->debug( $sql );
	// 			if( !mysqli_query( $this->db, $sql ) ){
										
	// 				$response['response_status']['status']       = 0;
	// 				$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 				$response['response_status']['msg']          = 'Não foi possível atualizar o registro';
					
	// 				$this->logger->debug( $this->db->error );
	// 				$this->logger->debug( $response );
	// 			}
	// 			else{
	// 				$this->logger->debug( $this->db->error );
								
	// 				$response['response_status']['status']       = 1;
	// 				$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 				$response['response_status']['msg']          = 'Registro atualizado com sucesso';
	// 			}	
				
	// 		// }
	// 		// else{
	// 		// 	$response = $this->response;
	// 		// }			
	// 	}
	// 	else{
	// 		$response = $this->response;
	// 	}
		
	// 	$this->logger->debug( $response['response_status']['status'] );
		
	// 	return $response;
	// }	

	// function getTipoComentario( $data = null ){

	// 	if( $this->response['response_status']['status'] == 1 ){
			
	// 		// Carregando a resposta padrão da função
	// 		$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
			
	// 		if( $response['response_status']['status'] == 1 ){
	// 			if( empty( $this->db ) ){
	// 				$connectdb = $this->connectdb();
					
	// 				if( $connectdb['response_status']['status'] == 0 ){
	// 					$response = $connectdb;
	// 				}
	// 			}
	// 		}
 

	// 		if( $response['response_status']['status'] == 1 ){

	// 			$opt = '<option value="">--</option>';
	// 			if ($data['tipo_comentario'] == "clausula") {
	// 				$sql = "SELECT
	// 						cg.id_clau,
	// 						cg.estrutura_id_estruturaclausula,
	// 						cg.tex_clau,
	// 						est.nome_clausula
							
	// 					FROM clausula_geral as cg
	// 					LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cg.estrutura_id_estruturaclausula

	// 					GROUP BY est.nome_clausula

	// 					ORDER BY est.nome_clausula ASC

	// 				";

	// 				$result = mysqli_query($this->db, $sql);

					
	// 				while ($obj = $result->fetch_object()) {
	// 					$opt .= "<option value='{$obj->id_clau}'>{$obj->nome_clausula}</option>";
	// 				}

	// 			}else if($data['tipo_comentario'] == "laboral") {
	// 				$sql = "SELECT
	// 							id_sinde,
	// 							sigla_sinde,
	// 							denominacao_sinde
								
	// 						FROM sind_emp

	// 						ORDER BY sigla_sinde ASC

	// 				";

	// 				$result = mysqli_query($this->db, $sql);

	// 				while ($obj = $result->fetch_object()) {
	// 					$opt .= "<option value='{$obj->id_sinde}'>{$obj->sigla_sinde} - {$obj->denominacao_sinde}</option>";
	// 				}
	// 			}else {
	// 				$sql = "SELECT
	// 							id_sindp,
	// 							sigla_sp,
	// 							denominacao_sp
								
	// 						FROM sind_patr

	// 						ORDER BY sigla_sp ASC

	// 				";

	// 				$result = mysqli_query($this->db, $sql);

	// 				while ($obj = $result->fetch_object()) {
	// 					$opt .= "<option value='{$obj->id_sindp}'>{$obj->sigla_sp} - {$obj->denominacao_sp}</option>";
	// 				}
	// 			}

	// 			$response['response_data']['tipo_com'] = $opt;

	// 			$this->logger->debug( $sql );
	// 			if( !mysqli_query( $this->db, $sql ) ){
										
	// 				$response['response_status']['status']       = 0;
	// 				$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 				$response['response_status']['msg']          = 'Não foi possível atualizar o registro';
					
	// 				$this->logger->debug( $this->db->error );
	// 				$this->logger->debug( $response );
	// 			}
	// 			else{
	// 				$this->logger->debug( $this->db->error );
								
	// 				$response['response_status']['status']       = 1;
	// 				$response['response_status']['error_code']   = $this->error_code . __LINE__;
	// 				$response['response_status']['msg']          = 'Registro atualizado com sucesso';
	// 			}	
				
	// 		}
	// 		else{
	// 			$response = $this->response;
	// 		}			
	// 	}
	// 	else{
	// 		$response = $this->response;
	// 	}
		
	// 	$this->logger->debug( $response['response_status']['status'] );
		
	// 	return $response;
	// }	


	// function getDestinoNotificacao($data = null)
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

	// 			//LISTA GRUPO ECONOMICO
	// 			$sql = "SELECT 
	// 						id_grupo_economico
	// 						,nome_grupoeconomico
	// 						,logo_grupo
	// 					FROM 
	// 						cliente_grupo;				
	// 			";

	// 			$this->logger->debug($sql);

	// 			$resultsql = mysqli_query($this->db, $sql);
	// 			$htmlGrupo = "<option value='--'>--</option>";

	// 			while ($obj = $resultsql->fetch_object()) {
	// 				$htmlGrupo .= "<option value='{$obj->id_grupo_economico}'>{$obj->nome_grupoeconomico}</option>";
	// 			}

	// 			$response['response_data']['lista_grupo'] 	= $htmlGrupo;


	// 			//LISTA CLIENTE MATRIZ

	// 			$sql = "SELECT 
	// 						cm.id_empresa AS id_empresa
	// 						,cm.nome_empresa AS nome_empresa
	// 						,cm.cnpj_empresa AS cnpj_empresa
	// 						,cm.cidade AS cidade
	// 						,cm.uf AS uf
	// 						,cm.tip_doc AS tip_doc
    //                         ,DATE_FORMAT(cm.data_inclusao,'%d/%m/%Y') AS data_inclusao
	// 						,DATE_FORMAT(cm.data_inativacao,'%d/%m/%Y') AS data_inativacao
    //                         ,gp.nome_grupoeconomico as grupo_economico
	// 					FROM 
	// 						cliente_matriz cm
	// 					INNER JOIN cliente_grupo gp WHERE gp.id_grupo_economico = cm.cliente_grupo_id_grupo_economico;								
	// 			";

	// 			$this->logger->debug($sql);

	// 			$resultsql = mysqli_query($this->db, $sql);
	// 			$listMatriz = "<option value='--'>--</option>";

	// 			while ($obj = $resultsql->fetch_object()) {
	// 				$cnpj = formatCnpjCpf($obj->cnpj_empresa);
	// 				$listMatriz .= "<option value='{$obj->id_empresa}'>{$obj->nome_empresa} | CNPJ: {$cnpj}</option>";
	// 			}

	// 			$response['response_data']['lista_matriz'] 	= $listMatriz;


	// 			//LISTA CLIENTE UNIDADE

	// 			$sql = "SELECT 
	// 						cu.id_unidade AS id_unidade
	// 						,cu.nome_unidade AS nome_unidade
	// 						,cu.cnpj_unidade AS cnpj_unidade
    //                         ,DATE_FORMAT(cu.data_inclusao,'%d/%m/%Y') AS data_inclusao
    //                         ,cm.nome_empresa as nome_empresa
	// 						,tuc.tipo_negocio as tipo_negocio
	// 					FROM 
	// 						cliente_unidades cu
	// 					INNER JOIN cliente_matriz cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
	// 					INNER JOIN tipounidade_cliente tuc on tuc.id_tiponegocio = cu.tipounidade_cliente_id_tiponegocio;
	// 			";

	// 			$this->logger->debug($sql);

	// 			$resultsql = mysqli_query($this->db, $sql);

	// 			$listUn = "<option value='--'>--</option>";
	// 			while ($obj = $resultsql->fetch_object()) {
	// 				$cnpj = formatCnpjCpf($obj->cnpj_unidade);
	// 				$listUn .= "<option value='{$obj->id_unidade}'>{$obj->nome_unidade} | CNPJ: {$cnpj}</option>";
	// 			}

	// 			$response['response_data']['lista_unidade'] 	= $listUn;
	// 		} else {
	// 			$response = $this->response;
	// 		}
	// 	} else {
	// 		$response = $this->response;
	// 	}

	// 	$this->logger->debug($response['response_status']['status']);
	// 	return $response;
	// }
}
?>
