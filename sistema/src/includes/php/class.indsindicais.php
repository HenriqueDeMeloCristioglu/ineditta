<?php
/**
 * @author    {Lucas A. Rodrigues Volpati}
 * @package   {1.0.0}
 * @description	{ }
**/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


include __DIR__ . "/helpers.php";

class indsindicais{
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

				
				
				//LISTA TIPO DOC
				$sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'S'";
				$result = mysqli_query( $this->db, $sqlTipo );

				$option = "<option value=''></option>";
				while ($obj = $result->fetch_object()) {
					$this->logger->debug(  $obj );

					$option .= '<option value="'.$obj->idtipo_doc.'">';
					$option .= $obj->nome_doc;
					$option .= '</option>';

				}

				$response['response_data']['optionTipo'] = $option;

				//LISTA DATA BASE MES/ANO
				$sqlDate = "SELECT 
								DISTINCT database_doc
							FROM 
								doc_sind
							ORDER BY database_doc ASC		
				";

				$resultDate = mysqli_query( $this->db, $sqlDate );
				$optDate = "<option value=''></option>";
				while ($objDate = $resultDate->fetch_object()) {
					$optDate .= "<option value='{$objDate->database_doc}'>{$objDate->database_doc}</option>";
				}

				$response['response_data']['data_base'] = $optDate;


				//LISTA CLÁUSULAS
				$sql = "SELECT
							id_estruturaclausula,
							nome_clausula
						FROM estrutura_clausula
						WHERE classe_clausula = 'S'
						ORDER BY nome_clausula ASC
				";

				$resultsql = mysqli_query( $this->db, $sql );

				$clau = "<option value=''></option>";
				while($obj = $resultsql->fetch_object()){
					$clau .= "<option value='{$obj->id_estruturaclausula}'>{$obj->nome_clausula}</option>";

				}

				$response['response_data']['nome_clausula'] = $clau;

				
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


	function gerarTabela( $data = null ){
 
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

				$this->logger->debug( $data);

				$filter = [];

				$filter = array_merge($data, $filter);
				unset($filter["module"]);
				unset($filter["action"]);

				$this->logger->debug( $filter);

				$sqlInfo = "SELECT 
								distinct ad.nmtipoinformacaoadicional as nmtipoinformacaoadicional
							FROM 
								clausula_geral_estrutura_clausula as cl 
								
							LEFT JOIN ad_tipoinformacaoadicional as ad on ad.cdtipoinformacaoadicional = cl.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
							LEFT JOIN doc_sind_sind_emp as demp on demp.doc_sind_id_doc = cl.doc_sind_id_doc
							LEFT JOIN doc_sind_sind_patr as dpatr on dpatr.doc_sind_id_doc = cl.doc_sind_id_doc
							LEFT JOIN doc_sind as doc on doc.id_doc = cl.doc_sind_id_doc
							LEFT JOIN sind_emp as sinde on sinde.id_sinde = demp.sind_emp_id_sinde
							LEFT JOIN sind_patr as sp on sp.id_sindp = dpatr.sind_patr_id_sindp
							LEFT JOIN clausula_geral as cg on cl.clausula_geral_id_clau = cg.id_clau
							
							WHERE ad.classe_ia = 'FE' OR ad.classe_ia = 'CM' AND cl.clausula_geral_id_clau is not null
							GROUP BY cl.clausula_geral_id_clau
							ORDER BY ad.nmtipoinformacaoadicional ASC
				";
				$resultInfo = mysqli_query( $this->db, $sqlInfo );

				$query = [];
				$pattern = [
					"/(á|à|ã|â|ä)/",
					"/(Á|À|Ã|Â|Ä)/",
					"/(é|è|ê|ë)/",
					"/(É|È|Ê|Ë)/",
					"/(í|ì|î|ï)/",
					"/(Í|Ì|Î|Ï)/",
					"/(ó|ò|õ|ô|ö)/",
					"/(Ó|Ò|Õ|Ô|Ö)/",
					"/(ú|ù|û|ü)/",
					"/(Ú|Ù|Û|Ü)/",
					"/(ñ)/",
					"/(Ñ)/",
					"/(ç)/",
					"/(Ç)/"
				];

				$title = [];

				while ($obj = $resultInfo->fetch_object()) {
					$column = str_replace(" ", "_", str_replace([".", "-", "?", "!", "&", "*", "=", "+", "<", ">", "(", ")", "/", "%", "#", "$", "@"], "_", $obj->nmtipoinformacaoadicional));

					$column = preg_replace($pattern, explode(" ","a A e E i I o O u U n N c C"),$column);

					// array_push($query, 'case when tab2.info_adicional = "'.$obj->nmtipoinformacaoadicional.'" then tab2.conteudo END as '.strtolower($column).'');
					array_push($query, 'IFNULL(GROUP_CONCAT(case when tab2.info_adicional = "'.$obj->nmtipoinformacaoadicional.'" then tab2.conteudo END), GROUP_CONCAT(IFNULL(case when tab2.info_adicional = "'.$obj->nmtipoinformacaoadicional.'" then tab2.conteudo END,null))) as '.strtolower($column).'');

					
					array_push($title, strtolower($column));
				}

				

				$query = implode(", ", $query);

				$this->logger->debug( $query);

				$where = "";

				if ($filter['nome_doc'] != "") {
					if (count($filter['nome_doc']) > 1) {
						$string = "";
						foreach ($filter['nome_doc'] as $value) {
						
							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$where .= ($where != "" ? " AND doc.tipo_doc_idtipo_doc IN ({$string})" : " doc.tipo_doc_idtipo_doc IN ({$string})" );
					}else {

						$string = "";
						foreach ($filter['nome_doc'] as $value) {
						
							$string .= "'{$value}'";
						}
						
						$where .= ($where != "" ? " AND doc.tipo_doc_idtipo_doc = {$string}" : " doc.tipo_doc_idtipo_doc = {$string}" );
					}
				}
				$this->logger->debug( $where);
				if ($filter['categoria'] != "") {

					if (count($filter['categoria']) > 1) {
						$string = "";
						foreach ($filter['categoria'] as $value) {
						
							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$where .= ($where != "" ? " AND cn.id_cnae IN ({$string})" : " cn.id_cnae IN ({$string})" );
					}else {

						$string = "";
						foreach ($filter['categoria'] as $value) {
						
							$string .= "'{$value}'";
						}
						
						$where .= ($where != "" ? " AND cn.id_cnae = {$string}" : " cn.id_cnae = {$string}");
					}
					
				}
				$this->logger->debug( $where);
				if ($filter['localidade'] != "" ) {
					if (count($filter['localidade']) > 1) {
						$string = "";
						foreach ($filter['localidade'] as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);
						
							$string .= "'{$content}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$where .= ($where != "" ? " AND loc.{$column} IN ({$string})" : " loc.{$column} IN ({$string})" );
					
					}else {
						$string = "";
						foreach ($filter['localidade'] as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);
						
							$string .= "'{$content}'";
						}
						
						$where .= ($where != "" ? " AND loc.{$column} = {$string}" : " loc.{$column} = {$string}" );
					}
				}
				$this->logger->debug( $where);

				//FILTRO GRUPO ECONOMICO
				if ($filter['grupo'] != "" && $filter['grupo'] != 0) {
					$where .= ($where != "" ? " AND gpr.id_grupo_economico = {$filter['grupo']}" : " gpr.id_grupo_economico = {$filter['grupo']}" );
				}

				$this->logger->debug($where);

				//FILTRO MATRIZ
				if ($filter['matriz'] != "") {

					if (count($filter['matriz']) > 1) {
						$string = "";
						foreach ($filter['matriz'] as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						// $where .= " AND mtz.id_empresa IN ({$string})";
						$where .= ($where != "" ? " AND mtz.id_empresa IN ({$string})" : " mtz.id_empresa IN ({$string})" );
					} else {

						$string = "";
						foreach ($filter['matriz'] as $value) {

							$string .= "'{$value}'";
						}

						// $where .= " AND mtz.id_empresa = {$string}";
						$where .= ($where != "" ? " AND mtz.id_empresa = {$string}" : " mtz.id_empresa = {$string}" );
					}
				}

				$this->logger->debug($where);

				//FILTRO UNIDADE
				if ($filter['unidade'] != "") {

					if (count($filter['unidade']) > 1) {
						$string = "";
						foreach ($filter['unidade'] as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						// $where .= " AND clt.id_unidade IN ({$string})";
						$where .= ($where != "" ? " AND clt.id_unidade IN ({$string})" : " clt.id_unidade IN ({$string})" );
					} else {

						$string = "";
						foreach ($filter['unidade'] as $value) {

							$string .= "'{$value}'";
						}

						// $where .= " AND clt.id_unidade = {$string}";
						$where .= ($where != "" ? " AND clt.id_unidade = {$string}" : " clt.id_unidade = {$string}" );
					}
				}

				if($filter['sindicato_laboral'] != "") {
					if (count($filter['sindicato_laboral']) > 1) {
						$string = "";
						foreach ($filter['sindicato_laboral'] as $value) {
							$string .= "'{$value}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$where .= ($where != "" ? " AND emp.sind_emp_id_sinde IN ({$string})" : " emp.sind_emp_id_sinde IN ({$string})" );
					}else {
						$string = "";
						foreach ($filter['sindicato_laboral'] as $value) {
							$string .= "'{$value}'";
						}
						
						$where .= ($where != "" ? " AND emp.sind_emp_id_sinde = {$string}" : " emp.sind_emp_id_sinde = {$string}" );
					}
				}
				

				if ($filter['sindicato_patronal'] != "") {
					if (count($filter['sindicato_patronal']) > 1) {
						$string = "";
						foreach ($filter['sindicato_patronal'] as $value) {
							$string .= "'{$value}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$where .= ($where != "" ? " AND pt.sind_patr_id_sindp IN ({$string})" : " pt.sind_patr_id_sindp IN ({$string})" );
					}else {
						$string = "";
						foreach ($filter['sindicato_patronal'] as $value) {
							$string .= "'{$value}'";
						}
						
						$where .= ($where != "" ? " AND pt.sind_patr_id_sindp = {$string}" : " pt.sind_patr_id_sindp = {$string}" );
					}
				}
				

				

				$this->logger->debug( $where);
				if ($filter['data_base'] != "") {

					if (count($filter['data_base']) > 1) {
						$string = "";
						foreach ($filter['data_base'] as $value) {
						
							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						// $where .= " AND doc.database_doc IN ({$string})";
						$where .= ($where != "" ? " AND doc.database_doc IN ({$string})" : " doc.database_doc IN ({$string})" );
					}else {

						$string = "";
						foreach ($filter['data_base'] as $value) {
						
							$string .= "'{$value}'";
						}
						
						// $where .= " AND doc.database_doc = {$string}";
						$where .= ($where != "" ? " AND doc.database_doc = {$string}" : " doc.database_doc = {$string}" );
					}
				}
				$this->logger->debug( $where);

				if ($filter['lista_clausula'] != "") {

					$where .= ($where != "" ? " AND cl.estrutura_clausula_id_estruturaclausula = {$filter['lista_clausula']}" : " cl.estrutura_clausula_id_estruturaclausula = {$filter['lista_clausula']}" );
					
				}
				
				
				$this->logger->debug( $where);

				$vigencia = $data['periodo'];


				if ($data['periodo'] && $where != "") {
					//vigencia
					$vigenIniDate = strstr($vigencia, ' -', true);
					$vigenIniDate = date("Y-m-d", strtotime(implode("-", explode("/", $vigenIniDate))));

					$separator = mb_strpos($vigencia, "-");
					$vigenEndDate = trim(substr($vigencia, $separator + 1));
					$vigenFinalDate = date("Y-m-d", strtotime(implode("-", explode("/", $vigenEndDate))));

					$where .= " AND doc.validade_inicial >= '{$vigenIniDate}' AND doc.validade_final <= '{$vigenFinalDate}'";
				}
				
				$this->logger->debug( $filter);

				// $sql = "SELECT 
				// 			tab2.id_clau,
				// 			id_doc,
				// 			tab2.nome_clausula as nome_clausula,
				// 			{$query}
				// 			,DATE_FORMAT(vigencia, '%d/%m/%Y') AS vigencia,
				// 			tipo_doc,
				// 			localidade,
				// 			laboral,
                //             patronal, 
				// 			categoria,
				// 			nome_doc,
				// 			data_base,
				// 			estrutura,
				// 			nome_grupo
				// 		FROM (
				// 			SELECT
				// 			id_clau,
				// 			nome_clausula,
				// 			info_adicional,
				// 			conteudo,
				// 			id_doc,
				// 			vigencia,
				// 			tipo_doc,
				// 			localidade,
				// 			laboral,
                //             patronal,
                //             categoria,
				// 			nome_doc,
				// 			data_base,
				// 			estrutura,
				// 			nome_grupo
				// 		FROM (
				// 			SELECT
				// 				cl.clausula_geral_id_clau as id_clau,
				// 				est.nome_clausula as nome_clausula,
				// 				ad.nmtipoinformacaoadicional as info_adicional,
				// 				case when cl.texto != '' then cl.texto 
				// 					when cl.numerico != 0 then cl.numerico
				// 					when cl.descricao != '' then cl.descricao
				// 					when cl.data then cl.data
				// 					when cl.percentual != 0 then cl.percentual
				// 					when cl.hora != '' then cl.hora
				// 					when cl.combo != '' then cl.combo else null end as conteudo,
				// 				gr.doc_sind_id_documento as id_doc,
				// 				gr.data_aprovacao as vigencia,
				// 				doc.tipo_doc_idtipo_doc as tipo_doc,
				// 				doc.uf as localidade,
				// 				sinde.razaosocial_sinde as laboral,
                //                 sindp.razaosocial_sp as patronal,
                //                 cn.descricao_subclasse as categoria,
				// 				tp.nome_doc as nome_doc,
				// 				bs.dataneg as data_base,
				// 				cl.estrutura_clausula_id_estruturaclausula as estrutura,
				// 				gp.nome_grupo
						
				// 			FROM 
				// 				clausula_geral_estrutura_clausula as cl
							
				// 				LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cl.estrutura_clausula_id_estruturaclausula AND est.tipo_clausula = 'T' OR est.tipo_clausula = 'P'
				// 				LEFT JOIN ad_tipoinformacaoadicional as ad on ad.cdtipoinformacaoadicional = cl.ad_tipoinformacaoadicional_cdtipoinformacaoadicional AND ad.classe_ia = 'FE' OR ad.classe_ia = 'CM'
				// 				LEFT JOIN clausula_geral as gr on gr.id_clau = cl.clausula_geral_id_clau
				// 				LEFT JOIN doc_sind as doc on doc.id_doc = gr.doc_sind_id_documento
                //                 LEFT JOIN classe_cnae_doc_sind as cnd on cnd.doc_sind_id_doc = cl.doc_sind_id_doc
                //                 LEFT JOIN classe_cnae as cn on cn.id_cnae = cnd.classe_cnae_id_cnae
				// 				LEFT JOIN doc_sind_sind_emp as emp on emp.doc_sind_id_doc =  cl.doc_sind_id_doc
                //                 LEFT JOIN doc_sind_sind_patr as pt on pt.doc_sind_id_doc =  cl.doc_sind_id_doc
				// 				LEFT JOIN base_territorialsindemp as bs on bs.sind_empregados_id_sinde1 = emp.sind_emp_id_sinde
				// 				LEFT JOIN sind_emp as sinde on sinde.id_sinde = emp.sind_emp_id_sinde
				// 				LEFT JOIN sind_patr as sindp on sindp.id_sindp = pt.sind_patr_id_sindp
				// 				LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
				// 				LEFT JOIN grupo_clausula as gp on gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
				// 				LEFT JOIN abrang_docsind as abr on abr.doc_sind_id_documento = doc.id_doc
                //                 LEFT JOIN localizacao as loc on abr.localizacao_id_localizacao = loc.id_localizacao
				// 				LEFT JOIN cliente_unidades as clt on clt.localizacao_id_localizacao = loc.id_localizacao
				// 				LEFT JOIN cliente_matriz as mtz on mtz.id_empresa = clt.cliente_matriz_id_empresa
				// 				LEFT JOIN cliente_grupo as gpr on gpr.id_grupo_economico = mtz.cliente_grupo_id_grupo_economico

				// 				WHERE {$where}
				// 			) tab1
						
				// 			GROUP BY tab1.id_clau, tab1.conteudo
				// 		)tab2

				// 		GROUP BY id_clau
				// ";

				$sql = "SELECT 
				tab2.id_clau,
				id_doc,
				tab2.nome_clausula as nome_clausula,
				{$query}
				,DATE_FORMAT(vigencia, '%d/%m/%Y') AS vigencia,
				tipo_doc,
				localidade,
				laboral,
				patronal, 
				categoria,
				nome_doc,
				data_base,
				estrutura,
				nome_grupo
			FROM (
				SELECT
				id_clau,
				nome_clausula,
				info_adicional,
				conteudo,
				id_doc,
				vigencia,
				tipo_doc,
				localidade,
				laboral,
				patronal,
				categoria,
				nome_doc,
				data_base,
				estrutura,
				nome_grupo
			FROM (
				SELECT
					cl.clausula_geral_id_clau as id_clau,
					est.nome_clausula as nome_clausula,
					ad.nmtipoinformacaoadicional as info_adicional,
					case when cl.texto != '' then cl.texto 
						when cl.numerico != 0 then cl.numerico
						when cl.descricao != '' then cl.descricao
						when cl.data then cl.data
						when cl.percentual != 0 then cl.percentual
						when cl.hora != '' then cl.hora
						when cl.combo != '' then cl.combo else null end as conteudo,
					gr.doc_sind_id_documento as id_doc,
					gr.data_aprovacao as vigencia,
					doc.tipo_doc_idtipo_doc as tipo_doc,
					doc.uf as localidade,
					sinde.razaosocial_sinde as laboral,
					sindp.razaosocial_sp as patronal,
					cn.descricao_subclasse as categoria,
					tp.nome_doc as nome_doc,
					doc.database_doc as data_base,
					cl.estrutura_clausula_id_estruturaclausula as estrutura,
					gp.nome_grupo
			
				FROM 
					clausula_geral_estrutura_clausula as cl
				
					LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cl.estrutura_clausula_id_estruturaclausula
					LEFT JOIN ad_tipoinformacaoadicional as ad on ad.cdtipoinformacaoadicional = cl.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
					LEFT JOIN clausula_geral as gr on gr.id_clau = cl.clausula_geral_id_clau
					LEFT JOIN doc_sind as doc on doc.id_doc = gr.doc_sind_id_documento
					LEFT JOIN classe_cnae_doc_sind as cnd on cnd.doc_sind_id_doc = cl.doc_sind_id_doc
					LEFT JOIN classe_cnae as cn on cn.id_cnae = cnd.classe_cnae_id_cnae
					LEFT JOIN doc_sind_sind_emp as emp on emp.doc_sind_id_doc =  cl.doc_sind_id_doc
					LEFT JOIN doc_sind_sind_patr as pt on pt.doc_sind_id_doc =  cl.doc_sind_id_doc
					LEFT JOIN base_territorialsindemp as bs on bs.sind_empregados_id_sinde1 = emp.sind_emp_id_sinde
					LEFT JOIN sind_emp as sinde on sinde.id_sinde = emp.sind_emp_id_sinde
					LEFT JOIN sind_patr as sindp on sindp.id_sindp = pt.sind_patr_id_sindp
					LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
					LEFT JOIN grupo_clausula as gp on gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
					LEFT JOIN abrang_docsind as abr on abr.doc_sind_id_documento = doc.id_doc
					LEFT JOIN localizacao as loc on abr.localizacao_id_localizacao = loc.id_localizacao
					LEFT JOIN cliente_unidades as clt on clt.localizacao_id_localizacao = loc.id_localizacao
					LEFT JOIN cliente_matriz as mtz on mtz.id_empresa = clt.cliente_matriz_id_empresa
					LEFT JOIN cliente_grupo as gpr on gpr.id_grupo_economico = mtz.cliente_grupo_id_grupo_economico
			
					WHERE {$where} AND est.tipo_clausula IN ('T', 'P') 
				) tab1
			
				GROUP BY tab1.id_clau, tab1.conteudo
			)tab2
			
			GROUP BY id_clau
			";

				$this->logger->debug( $sql);
				$this->logger->debug( $result = mysqli_query( $this->db, $sql ));


				if ($result = mysqli_query( $this->db, $sql )) {
					
					$content = [];
					while ($obj = $result->fetch_assoc()) {
						
						array_push($content, $obj);
						$this->logger->debug( $obj );
						
					}

					$response['response_data']['title'] = $title;
					$response['response_data']['table'] = $content;
				}else {
					
					$this->logger->debug( $this->db->error );
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível renderizar a tabela.';
				}

				$this->logger->debug( $content ); 
				
			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		// $this->logger->debug( $response['response_data'] );
		
		return $response;
	}

	function getMatriz( $data = null ){
 
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
							id_empresa,
							razao_social,
							cnpj_empresa,
							codigo_empresa
						FROM cliente_matriz
						WHERE cliente_grupo_id_grupo_economico = '{$data['id_grupo']}'
				";

				$result = mysqli_query( $this->db, $sql );

				$opt = "<option value=''></option>";
				while ($obj = $result->fetch_object()) {
					$opt .= "<option value='{$obj->id_empresa}'>{$obj->codigo_empresa} / {$obj->cnpj_empresa} / {$obj->razao_social}</option>";
				}

				$response['response_data']['lista_matriz'] = $opt;
			
				
			}else{
				$response = $this->response;
			}

		}else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		return $response;
	}

	function getUnidade( $data = null ){
 
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

				$matriz = $data['id_matriz'];

				$opt = "<option value=''></option>";
				for ($i=0; $i < count($matriz) ; $i++) { 
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

					$this->logger->debug( $sql );

					$result = mysqli_query( $this->db, $sql );

					
					while ($obj = $result->fetch_object()) {
						$opt .= "<option value='{$obj->id_unidade}'>Cód: {$obj->codigo_empresa} / CNPJ: {$obj->cnpj_unidade} / Nome: {$obj->nome_unidade} / Cod. Sind. Cliente: {$obj->cod_sindcliente} / Regional: {$obj->regional}</option>";
					}
				}

				$this->logger->debug( $opt );
					
				$response['response_data']['lista_unidade'] = $opt;
			
				
			}else{
				$response = $this->response;
			}

		}else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		return $response;
	}

	// function getClausulas( $data = null ){
 
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

	// 			$idGrupo = $data['id_grupo_clausula'];
	// 			$opt = "<option value=''></option>";
	// 			for ($i=0; $i < count($idGrupo) ; $i++) { 
	// 				$sql = "SELECT
	// 							id_estruturaclausula,
	// 							nome_clausula
	// 						FROM estrutura_clausula
	// 						WHERE grupo_clausula_idgrupo_clausula = '{$idGrupo[$i]}'
	// 				";

	// 				$result = mysqli_query( $this->db, $sql );

					
	// 				while ($obj = $result->fetch_object()) {
	// 					$opt .= "<option value='{$obj->id_estruturaclausula}'>{$obj->nome_clausula}</option>";
	// 				}
	// 			}

	// 			$response['response_data']['lista_clausulas'] = $opt;
				
	// 		}else{
	// 			$response = $this->response;
	// 		}
	// 	}else{
	// 		$response = $this->response;
	// 	}

	// 	$this->logger->debug( $response['response_status']['status'] );
	// 	return $response;
	// }


	function saveFilter( $data = null ){
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

				$filter = [];
				$filter = array_merge($data, $filter);
				unset($filter["module"]);
				unset($filter["action"]);
				
				$saveFilter = json_encode($filter);

				$this->logger->debug( $saveFilter);

				$sql = "SELECT * FROM filtro_csv";

				$result = mysqli_query( $this->db, $sql );

				$obj = $result->fetch_object();

				$this->logger->debug( $obj);
				if (empty($obj)) {
					$sqlSave = "INSERT INTO filtro_csv (filtro, usuario) VALUES ('{$saveFilter}', 'Teste')";
				}else {
					$sqlSave = "UPDATE filtro_csv SET filtro = '{$saveFilter}'";
				}

				if ($result = mysqli_query( $this->db, $sqlSave )) {
					$response['response_status']['status']     = 1;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Registro salvo com sucesso!';
				}else {
					$this->logger->debug( $this->db->error );
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível salvar o filtro.';
				}

				$this->logger->debug( $sqlSave );

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
