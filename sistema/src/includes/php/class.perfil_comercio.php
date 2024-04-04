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


include __DIR__ . "/helpers.php";

class perfil_comercio{
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
				
				//LISTA TIPO DOC
				// $sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'S'";
				// $result = mysqli_query( $this->db, $sqlTipo );

				// $option = "";
				// $option .= '<option value="--">--</option>';
				// while ($obj = $result->fetch_object()) {
				// 	$this->logger->debug(  $obj );

				// 	$option .= '<option value="'.$obj->idtipo_doc.'">';
				// 	$option .= $obj->nome_doc;
				// 	$option .= '</option>';

				// }

				// $response['response_data']['optionTipo'] = $option;

				//LISTAS LOCALIDADE - UF
				$sqlUf = "SELECT
							distinct uf
							
						FROM
							localizacao
						ORDER BY uf ASC
				";

				//LISTAS LOCALIDADE - REGIAO
				$sqlRegiao = "SELECT
							distinct regiao
							
						FROM
							localizacao
						ORDER BY regiao ASC
				";

				//LISTAS LOCALIDADE - MUNICIPIO
				$sqlMuni = "SELECT
							distinct municipio
							
						FROM
							localizacao
						ORDER BY municipio ASC
				";
				$resultUf = mysqli_query( $this->db, $sqlUf );
				$resultReg = mysqli_query( $this->db, $sqlRegiao );
				$resultMun = mysqli_query( $this->db, $sqlMuni );

				$opt = "";
				while ($obj = $resultUf->fetch_object()) {
					$opt .= "<option value='uf+".$obj->uf."'>".$obj->uf."</option>";
				}

				while ($obj = $resultReg->fetch_object()) {
					$opt .= "<option value='regiao+".$obj->regiao."'>".$obj->regiao."</option>";
				}

				while ($obj = $resultMun->fetch_object()) {
					$opt .= "<option value='municipio+".$obj->municipio."'>".$obj->municipio."</option>";
				}

				$response['response_data']['listaLocal'] = $opt;


				//LISTA CLASSE CNAE (CATEGORIA)
				$sqlCat = 'SELECT * FROM classe_cnae';
				$resultCat = mysqli_query( $this->db, $sqlCat );

				$optcat = "";
				while ($objCat = $resultCat->fetch_object()) {
					$this->logger->debug(  $objCat );

					$optcat .= '<option value="'.$objCat->id_cnae.'">';
					$optcat .= $objCat->descricao_subclasse;
					$optcat .= '</option>';

				}

				$response['response_data']['optionCategoria'] = $optcat;

				//LISTA GRUPO CLÁUSULAS
				// $sql = "SELECT 
				// 			* 
				// 		FROM 
				// 			grupo_clausula
				// 		ORDER BY nome_grupo ASC
				// ";

				// $resultsql = mysqli_query( $this->db, $sql );

				// $group = null;

				// while($obj = $resultsql->fetch_object()){

				// 	$group .= '<option value="'.$obj->idgrupo_clausula.'">';
				// 	$group .= $obj->nome_grupo;
				// 	$group .= '</option>';
				// }

				// $response['response_data']['grupoClausulas'] = $group;

				//LISTA COD_SINDCLIENTE - CLIENTE UNIDADES
				$sqlSind1 = "SELECT 
								distinct cod_sindcliente
							FROM 
								cliente_unidades
				";

				$resultsqlSind1 = mysqli_query( $this->db, $sqlSind1 );

				$cod = "";
				while($obj = $resultsqlSind1->fetch_object()){
					$cod .= '<option value="cod_sindcliente+'.$obj->cod_sindcliente.'">';
					$cod .= $obj->cod_sindcliente;
					$cod .= '</option>';
				}

				//LISTA COD_UNIDADE - CLIENTE UNIDADES
				$sqlSind2 = "SELECT 
								distinct codigo_unidade
							FROM 
								cliente_unidades
				";

				$resultsqlSind2 = mysqli_query( $this->db, $sqlSind2 );

				while($obj = $resultsqlSind2->fetch_object()){
					$cod .= '<option value="codigo_unidade+'.$obj->codigo_unidade.'">';
					$cod .= $obj->codigo_unidade;
					$cod .= '</option>';
				}

				$response['response_data']['listaCodigos'] = $cod;

				//LISTAS SINDICATOS
				$sqlEmp = "SELECT
							id_sinde,
							codigo_sinde,
							cnpj_sinde,
							sigla_sinde,
							denominacao_sinde
						FROM sind_emp
					";

				$sqlPatr = "SELECT
							id_sindp,
							codigo_sp,
							cnpj_sp,
							sigla_sp,
							denominacao_sp
						FROM sind_patr
				";


				$resultEmp = mysqli_query( $this->db, $sqlEmp );
				$resultPatr = mysqli_query( $this->db, $sqlPatr );

				$optEmp = "";
				while ($obj = $resultEmp->fetch_object()) {
					$optEmp .= "<option value='codigo_sinde+".$obj->codigo_sinde."'>".$obj->codigo_sinde."</option>";
					$optEmp .= "<option value='cnpj_sinde+".$obj->cnpj_sinde."'>".$obj->cnpj_sinde."</option>";
					$optEmp .= "<option value='sigla_sinde+".$obj->sigla_sinde."'>".$obj->sigla_sinde."</option>";
					$optEmp .= "<option value='denominacao_sinde+".$obj->denominacao_sinde."'>".$obj->denominacao_sinde."</option>";
				}

				$optPatr = "";
				while ($obj = $resultPatr->fetch_object()) {
					$optPatr .= "<option value='codigo_sp+".$obj->codigo_sp."'>".$obj->codigo_sp."</option>";
					$optPatr .= "<option value='cnpj_sp+".$obj->cnpj_sp."'>".$obj->cnpj_sp."</option>";
					$optPatr .= "<option value='sigla_sp+".$obj->sigla_sp."'>".$obj->sigla_sp."</option>";
					$optPatr .= "<option value='denominacao_sp+".$obj->denominacao_sp."'>".$obj->denominacao_sp."</option>";
				}


				$response['response_data']['listaEmp'] = $optEmp;
				$response['response_data']['listaPatr'] = $optPatr;



				// Verificação de filtro salvo
				$sql = "SELECT * FROM filtro_csv";

				$result = mysqli_query( $this->db, $sql );

				$obj = $result->fetch_object();

				if (!empty($obj)) {
					$response['response_data']['filtro'] = true;

					$filtro = $obj->filtro;

					$filtroDecode = json_decode($filtro);
					$this->logger->debug( $filtroDecode);

					$response['response_data']['filtro_salvo'] = $filtroDecode;
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

	
	function getSindicatos( $data = null ){
 
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

				$item = $data['busca'];

				if ( $data['sindicato'] == "laboral") {
					$sql = "SELECT
							id_sinde,
							{$item}
						FROM sind_emp
					";
				}else {
					$sql = "SELECT
							id_sindp,
							{$item}
						FROM sind_patr
					";
				}
				

				$this->logger->debug( $sql );

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
				while ($obj = $result->fetch_object()) {
					$opt .= "<option value='{$obj->id_sinde}'>{$obj->$item}</option>";
				}

				$response['response_data']['sindicatos'] = $opt;

				
			}else{
				$response = $this->response;
			}

		}else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		return $response;
	}

	function getSindicatosByCodigo( $data = null ){
 
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

				$busca = $data['busca'];
				$where = "";

				//Tratando busca
				for ($i=0; $i < count($busca) ; $i++) { 
					$column = strstr($busca[$i], "+", true);
					break;
				}

				if (count($busca) > 1) {

					$string = "";
					foreach ($busca as $value) {
						$value = substr(strstr($value, "+"), 1);
						$string .= "'{$value}'" . ',';
					}
					$string = implode(",", array_filter(explode(",", $string)));
					$where = "WHERE {$column} IN ({$string})";
				}else {
					$string = "";
					foreach ($busca as $value) {
						$value = substr(strstr($value, "+"), 1);
						$string .= "'{$value}'";
					}
					
					$where .= " WHERE {$column} = {$string}";
				}

				$sqlEmp = "SELECT
								cod_sindcliente,
								cu.localizacao_id_localizacao,
								bem.sind_empregados_id_sinde1,
								sinde.sigla_sinde,
								sinde.razaosocial_sinde
							FROM cliente_unidades AS cu
							LEFT JOIN base_territorialsindemp AS bem ON bem.localizacao_id_localizacao1 = cu.localizacao_id_localizacao
							LEFT JOIN sind_emp AS sinde ON sinde.id_sinde = bem.sind_empregados_id_sinde1
							
							{$where}
							
							GROUP BY bem.sind_empregados_id_sinde1
				";

				$sqlPatr = "SELECT
								cod_sindcliente,
								cu.localizacao_id_localizacao,
								bp.sind_patronal_id_sindp,
								sindp.sigla_sp,
								sindp.razaosocial_sp
							FROM cliente_unidades AS cu
							LEFT JOIN base_territorialsindpatro AS bp ON bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao
							LEFT JOIN sind_patr AS sindp ON sindp.id_sindp = bp.sind_patronal_id_sindp
							
							{$where}
							
							GROUP BY bp.sind_patronal_id_sindp
				";

				$this->logger->debug( $sqlEmp );
				$this->logger->debug( $sqlPatr );

				
				$resultEmp = mysqli_query( $this->db, $sqlEmp );
				$resultPatr = mysqli_query( $this->db, $sqlPatr );
				

				$optEmp = '';
				while ($obj = $resultEmp->fetch_object()) {
					$optEmp .= "<option value='{$obj->sind_empregados_id_sinde1}'>{$obj->razaosocial_sinde}</option>";
				}

				$response['response_data']['optEmp'] = $optEmp;

				$optPatr = '';
				while ($obj = $resultPatr->fetch_object()) {
					$optPatr .= "<option value='{$obj->sind_patronal_id_sindp}'>{$obj->razaosocial_sp}</option>";
				}

				$response['response_data']['optPatr'] = $optPatr;

				
			}else{
				$response = $this->response;
			}

		}else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		return $response;
	}

	function getSindicatosByCodUnidade( $data = null ){
 
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

				$busca = $data['busca'];
				$where = "";
				if (count($busca) > 1) {
					$string = "";
					foreach ($busca as $value) {
					
						$string .= "'{$value}'" . ',';
					}
					$string = implode(",", array_filter(explode(",", $string)));
					$where = "WHERE cu.codigo_unidade IN ({$string})";
				}else {
					$string = "";
					foreach ($busca as $value) {
					
						$string .= "'{$value}'";
					}
					
					$where .= " WHERE cu.codigo_unidade = {$string}";
				}

				$sqlEmp = "SELECT
								codigo_unidade,
								cu.localizacao_id_localizacao,
								bem.sind_empregados_id_sinde1,
								sinde.sigla_sinde,
								sinde.razaosocial_sinde
							FROM cliente_unidades AS cu
							LEFT JOIN base_territorialsindemp AS bem ON bem.localizacao_id_localizacao1 = cu.localizacao_id_localizacao
							LEFT JOIN sind_emp AS sinde ON sinde.id_sinde = bem.sind_empregados_id_sinde1
							
							{$where}
							
							GROUP BY bem.sind_empregados_id_sinde1
				";

				$sqlPatr = "SELECT
								codigo_unidade,
								cu.localizacao_id_localizacao,
								bp.sind_patronal_id_sindp,
								sindp.sigla_sp,
								sindp.razaosocial_sp
							FROM cliente_unidades AS cu
							LEFT JOIN base_territorialsindpatro AS bp ON bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao
							LEFT JOIN sind_patr AS sindp ON sindp.id_sindp = bp.sind_patronal_id_sindp
							
							{$where}
							
							GROUP BY bp.sind_patronal_id_sindp
				";

				$this->logger->debug( $sqlEmp );
				$this->logger->debug( $sqlPatr );

				
				$resultEmp = mysqli_query( $this->db, $sqlEmp );
				$resultPatr = mysqli_query( $this->db, $sqlPatr );
				

				$optEmp = '';
				while ($obj = $resultEmp->fetch_object()) {
					$optEmp .= "<option value='{$obj->sind_empregados_id_sinde1}'>{$obj->razaosocial_sinde}</option>";
				}

				$response['response_data']['optEmpCod'] = $optEmp;

				$optPatr = '';
				while ($obj = $resultPatr->fetch_object()) {
					$optPatr .= "<option value='{$obj->sind_patronal_id_sindp}'>{$obj->razaosocial_sp}</option>";
				}

				$response['response_data']['optPatrCod'] = $optPatr;

				
			}else{
				$response = $this->response;
			}

		}else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		return $response;
	}

	function setFilter( $data = null ){
 
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

				$this->logger->debug($data);
				$buscaLocal = $data['localidade'];
				$buscaCateg = $data['categoria'];
				$buscaData = $data['data_base'];

				$buscaEmp = $data['sind_emp'];
				$buscaPatr = $data['sind_patr'];

				$whereEmp = "";
				$wherePatr = "";
				$filter = 0;
				//BUSCANDO POR LOCALIDADE
				if (!empty($buscaLocal)) {
					if (count($buscaLocal) > 1) {
						$filter++;
						$string = "";
						foreach ($buscaLocal as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);
						
							$string .= "'{$content}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$whereEmp .= " OR lc.{$column} IN ({$string})";
						$wherePatr .= " OR lc.{$column} IN ({$string})";
					
					}else {
						$filter++;
						$string = "";
						foreach ($buscaLocal as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);
						
							$string .= "'{$content}'";
						}
						
						$whereEmp .= " OR lc.{$column} = {$string}";
						$wherePatr .= " OR lc.{$column} = {$string}";
					}
				}

				//BUSCANDO POR CATEGORIA (CLASSE CNAE)
				if (!empty($buscaCateg)) {
					if (count($buscaCateg) > 1) {
						
						$string = "";
						foreach ($buscaCateg as $value) {
							
							$string .= "'{$value}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$whereEmp .= $filter > 0 ? " AND bem.classe_cnae_idclasse_cnae IN ({$string})" : " OR bem.classe_cnae_idclasse_cnae IN ({$string})";
						$wherePatr .= $filter > 0 ? " AND bp.classe_cnae_idclasse_cnae IN ({$string})" : " OR bp.classe_cnae_idclasse_cnae IN ({$string})";
						$filter++;
					}else {
						$string = "";
						foreach ($buscaCateg as $value) {
							$string .= "'{$value}'";
						}
						
						$whereEmp .= $filter > 0 ? " AND bem.classe_cnae_idclasse_cnae = {$string}" : " OR bem.classe_cnae_idclasse_cnae = {$string}";
						$wherePatr .= $filter > 0 ? " AND bp.classe_cnae_idclasse_cnae = {$string}" : " OR bp.classe_cnae_idclasse_cnae = {$string}";
						$filter++;
					}
				}

				//BUSCANDO POR DATA-BASE
				if (!empty($buscaData)) {
					if (count($buscaData) > 1) {
						
						$string = "";
						foreach ($buscaData as $value) {
							
							$string .= "'{$value}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$whereEmp .= $filter > 0 ? " AND bem.dataneg IN ({$string})" : " OR bem.dataneg IN ({$string})";
						$filter++;
					}else {
						$string = "";
						foreach ($buscaData as $value) {
							$string .= "'{$value}'";
						}
						
						$whereEmp .= $filter > 0 ? " AND bem.dataneg = {$string}" : " OR bem.dataneg = {$string}";
						$filter++;
					}
				}

				//BUSCANDO SIND EMP POR (CNPJ, SIGLA...)
				if (!empty($buscaEmp)) {
					if (count($buscaEmp) > 1) {
						$string = "";
						foreach ($buscaEmp as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);
						
							$string .= "'{$content}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$whereEmp .= $filter > 0 ? " AND se.{$column} IN ({$string})" : " OR se.{$column} IN ({$string})";
						$filter++;
					}else {
						$string = "";
						foreach ($buscaEmp as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);
						
							$string .= "'{$content}'";
						}
						
						$whereEmp .= $filter > 0 ? " AND se.{$column} = {$string}" : " OR se.{$column} = {$string}";
						$filter++;
					}
				}

				//BUSCANDO SIND PATR POR (CNPJ, SIGLA...)
				if (!empty($buscaPatr)) {
					if (count($buscaPatr) > 1) {
						$string = "";
						foreach ($buscaPatr as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);
						
							$string .= "'{$content}'" . ',';
						}
						$string = implode(",", array_filter(explode(",", $string)));
						$wherePatr .= $wherePatr != "" ? " AND sp.{$column} IN ({$string})" : " OR sp.{$column} IN ({$string})";
						$filter++;
					}else {
						$string = "";
						foreach ($buscaPatr as $value) {
							$column = strstr($value, "+", true);
							$content = substr(strstr($value, "+"), 1);
						
							$string .= "'{$content}'";
						}
						
						$wherePatr .= $wherePatr != "" ? " AND sp.{$column} = {$string}" : " OR sp.{$column} = {$string}";
						$filter++;
					}
				}

				//QUERY SIND EMPREGADOS
				$sqlLaboral = "SELECT
							se.id_sinde,
							se.sigla_sinde,
							se.razaosocial_sinde,
							se.municipio_sinde,
							se.uf_sinde,
							lc.regiao,
							lc.municipio,
							bem.dataneg
						FROM sind_emp as se
						LEFT JOIN base_territorialsindemp as bem on bem.sind_empregados_id_sinde1 = se.id_sinde
						LEFT JOIN localizacao as lc on lc.id_localizacao = bem.localizacao_id_localizacao1
						
						WHERE lc.uf IN ('GH','PS'){$whereEmp}
						
						GROUP BY se.id_sinde
				";
				$this->logger->debug( $sqlLaboral );

				$sqlPatronal = "SELECT
									sp.id_sindp,
									sp.sigla_sp,
									sp.razaosocial_sp,
									sp.municipio_sp,
									sp.uf_sp,
									lc.regiao,
									lc.municipio
								FROM sind_patr as sp
								LEFT JOIN base_territorialsindpatro as bp on bp.sind_patronal_id_sindp = sp.id_sindp
								LEFT JOIN localizacao as lc on lc.id_localizacao = bp.localizacao_id_localizacao1
								
								WHERE lc.uf IN ('GH','PS') {$wherePatr}
								
								GROUP BY sp.id_sindp
				";
				$this->logger->debug( $sqlPatronal );

				$resultEmp = mysqli_query( $this->db, $sqlLaboral );
				$resultPatr = mysqli_query( $this->db, $sqlPatronal );

				$qtdEmp = mysqli_num_rows($resultEmp);
				$qtdPatr = mysqli_num_rows($resultPatr);

				$response['response_data']['qtdEmp'] = $qtdEmp;
				$response['response_data']['qtdPatr'] = $qtdPatr;

				//CONSTROI OBJETO SIND EMP APÓS FILTRO
				$lista = [];
				$idLista = [];
				while ($obj = $resultEmp->fetch_object()) {
					$this->logger->debug( $obj );
					$objEmp = new stdClass();
					$objEmp->id_sinde = $obj->id_sinde;
					$objEmp->sigla_sinde = $obj->sigla_sinde;
					$objEmp->municipio_sinde = $obj->municipio_sinde;
					array_push($lista, $objEmp);
					array_push($idLista, $objEmp->id_sinde);
				}

				//CONSTROI OBJETO SIND PATR APÓS FILTRO
				$listaPatr = [];
				while ($obj = $resultPatr->fetch_object()) {
					$this->logger->debug( $obj );
					$objPatr = new stdClass();
					$objPatr->id_sindp = $obj->id_sindp;
					$objPatr->sigla_sp = $obj->sigla_sp;
					$objPatr->municipio_sp = $obj->municipio_sp;
					array_push($listaPatr, $objPatr);
				}

				$this->logger->debug( $listaPatr );
				$vigente = 0;
				$vencido = 0;
				//MADATOS SINDICAIS EMP
				for ($i=0; $i < count($lista) ; $i++) { 
					$sqlMandEmp = "SELECT
									termino_mandatoe as termino
									FROM sind_diremp
									WHERE sind_emp_id_sinde = '{$lista[$i]->id_sinde}'
					";

					$this->logger->debug( $sqlMandEmp );

					$resultMandEmp = mysqli_query( $this->db, $sqlMandEmp );
					
					while ($obj = $resultMandEmp->fetch_object()) {
						
						if ($obj->termino >= date_format((new DateTime('now')), "Y-m-d")) {
							$vigente ++;
						}else if($obj->termino < date_format((new DateTime('now')), "Y-m-d")) {
							$vencido++;
						}
						
					}
				}

				//MADATOS SINDICAIS PATR
				for ($i=0; $i < count($listaPatr) ; $i++) { 
					$sqlMandPatr = "SELECT
									termino_mandatop as termino
									FROM sind_dirpatro
									WHERE sind_patr_id_sindp = '{$listaPatr[$i]->id_sindp}'
					";

					$this->logger->debug( $sqlMandPatr );

					$resultMandPatr = mysqli_query( $this->db, $sqlMandPatr );
					
					while ($obj = $resultMandPatr->fetch_object()) {
						
						if ($obj->termino >= date_format((new DateTime('now')), "Y-m-d")) {
							$vigente ++;
						}else if($obj->termino < date_format((new DateTime('now')), "Y-m-d")) {
							$vencido++;
						}
						
					}
				}

				$this->logger->debug('Vigente ' . $vigente );
				$this->logger->debug('Vencido ' . $vencido );

				$response['response_data']['mandVigente'] = $vigente;
				$response['response_data']['mandVencido'] = $vencido;


				//NEGOCIAÇÕES VIGENTES E VENCIDAS POR SIND EMP
				$negViegnte = 0;
				$negVencida = 0;
				for ($i=0; $i < count($lista) ; $i++) { 
					$sqlMandEmp = "SELECT
										fase
									FROM acompanhanto_cct
									WHERE sind_emp_id_sinde = '{$lista[$i]->id_sinde}'
					";

					$this->logger->debug( $sqlMandEmp );

					$resultMandEmp = mysqli_query( $this->db, $sqlMandEmp );
					
					while ($obj = $resultMandEmp->fetch_object()) {
						
						if ($obj->fase == "Fechada") {
							$negVencida++;
						}else {
							$negViegnte ++;
						}
						
					}
				}

				$this->logger->debug('Neg Vigente ' . $negViegnte );
				$this->logger->debug('Neg Vencido ' . $negVencida );

				$response['response_data']['negVigente'] = $negViegnte;
				$response['response_data']['negVencida'] = $negVencida;


				//NEGOCIAÇÕES EM ABERTO POR ESTADO 
				$listaGraf = [];
				for ($i=0; $i < count($lista) ; $i++) { 
					$sqlEst = "SELECT 
								se.uf_sinde as uf,
								COUNT(se.uf_sinde) as uf_qtd
							FROM acompanhanto_cct as ct
							LEFT JOIN sind_emp as se on se.id_sinde = ct.sind_emp_id_sinde
							WHERE ct.sind_emp_id_sinde = '{$lista[$i]->id_sinde}' AND ct.fase != 'Fechada'
							GROUP BY se.uf_sinde
							ORDER BY se.uf_sinde ASC
					";

					$this->logger->debug($sqlEst );

					$resultEst = mysqli_query( $this->db, $sqlEst );

					
					while ($objEst = $resultEst->fetch_object()) {
						$this->logger->debug($objEst );
						$objGraf = new stdClass;
						$objGraf->uf = $objEst->uf;
						$objGraf->qtd = $objEst->uf_qtd;

						array_push($listaGraf, $objGraf);
					}
				}
				
				$response['response_data']['listaGrafico'] = $listaGraf;


				//ORGANIZAÇÃO SINDICAL LABORAL
				$organizacaoSind = [];
				for ($i=0; $i < count($lista) ; $i++) { 
					$sqlOrg = "SELECT
									id_sinde,
									municipio_sinde,
									sigla_sinde,
									ce.nome_centralsindical,
									ass.nome as confederacao,
									asf.nome as federacao
								FROM sind_emp as se
								LEFT JOIN central_sindical as ce on se.central_sindical_id_centralsindical = ce.id_centralsindical
								LEFT JOIN associacao as ass on ass.id_associacao = se.confederacao_id_associacao
								LEFT JOIN associacao as asf on asf.id_associacao = se.federacao_id_associacao
								WHERE id_sinde = '{$lista[$i]->id_sinde}'
								ORDER BY municipio_sinde ASC
					";

					$this->logger->debug( $sqlOrg );

					$resultOrg = mysqli_query( $this->db, $sqlOrg );
					
					while ($obj = $resultOrg->fetch_object()) {
						$this->logger->debug( $obj );
						$orgSind = new stdClass;
						$orgSind->municipio_sinde = $obj->municipio_sinde;
						$orgSind->sigla_sinde = $obj->sigla_sinde;
						$orgSind->nome_centralsindical = $obj->nome_centralsindical;
						$orgSind->confederacao = $obj->confederacao;
						$orgSind->federacao = $obj->federacao;
						
						array_push($organizacaoSind, $orgSind);
					}
				}
				

				$this->logger->debug($organizacaoSind );

				//GERANO TABELA ORGANIZAÇÃO SINDICAL
				$html = "";
				$html = "
					<thead>
						<th>Localidade</th>
						<th>Sindicato (Sigla Laboral)</th>
						<th>Federação Laboral</th>
						<th>Confederação Laboral</th>
						<th>Central Sindical Laboral</th>
					</thead>
				";
				for ($i=0; $i < count($organizacaoSind) ; $i++) { 
					
					$html .= "<tr class='tbl-item'>";
					$html .= "<td class='title'>{$organizacaoSind[$i]->municipio_sinde}</td>";
					$html .= "<td class='desc'>{$organizacaoSind[$i]->sigla_sinde}</td>";
					$html .= "<td>{$organizacaoSind[$i]->federacao}</td>";
					$html .= "<td>{$organizacaoSind[$i]->confederacao}</td>";
					$html .= "<td>{$organizacaoSind[$i]->nome_centralsindical}</td>";
					$html .= "</tr>";
				}
				
				$this->logger->debug($html );
				$response['response_data']['tabelaOrganizacao'] = $html;

				//GERANDO DADOS PARA GRÁFICO EM PIZZA
				$whereId = '';
				for ($i=1; $i < count($idLista) ; $i++) { 
					$whereId .= " OR id_sinde = {$idLista[$i]}";
				}

				$sqlGraf = "SELECT
								tab_1.nome_centralsindical,
								COUNT(*) as qtd_centrais
							FROM (SELECT
								id_sinde,
								municipio_sinde,
								sigla_sinde,
								ce.nome_centralsindical,
								ass.nome as confederacao,
								asf.nome as federacao
							FROM sind_emp as se
							LEFT JOIN central_sindical as ce on se.central_sindical_id_centralsindical = ce.id_centralsindical
							LEFT JOIN associacao as ass on ass.id_associacao = se.confederacao_id_associacao
							LEFT JOIN associacao as asf on asf.id_associacao = se.federacao_id_associacao
							WHERE id_sinde = '{$idLista[0]}'{$whereId})tab_1
							
							GROUP BY tab_1.nome_centralsindical
				";

				$this->logger->debug($sqlGraf );

				$resultGraf = mysqli_query( $this->db, $sqlGraf );
				$listaGrafPizza = [];
				while ($obj = $resultGraf->fetch_object()) {
					$pizza = new stdClass;
					$pizza->central = $obj->nome_centralsindical;
					$pizza->qtd = $obj->qtd_centrais;

					array_push($listaGrafPizza, $pizza);
				}

				$response['response_data']['pizza'] = $listaGrafPizza;

				//TABELA DIRIGENTES SINDICAIS
				$dirigentes = [];
				for ($i=0; $i < count($lista) ; $i++) { 
					$sql = "SELECT
								dirigente_e as dirigente,
								funcao_e as cargo,
								situacao_e as situacao,
								termino_mandatoe as termino,
								cm.razao_social as empresa
							FROM sind_diremp as sd
							LEFT JOIN cliente_matriz as cm on cm.id_empresa = sd.cliente_matriz_id_empresa
							WHERE sind_emp_id_sinde = '{$lista[$i]->id_sinde}'
							ORDER BY dirigente ASC
					";

					$this->logger->debug($sql);

					$resultDir = mysqli_query( $this->db, $sql );

					while ($obj = $resultDir->fetch_object()) {
						$dirSind = new stdClass;
						$dirSind->dirigente = $obj->dirigente;
						$dirSind->cargo = $obj->cargo;
						$dirSind->situacao = $obj->situacao;
						$dirSind->termino = $obj->termino;
						$dirSind->empresa = $obj->empresa;

						array_push($dirigentes, $dirSind);
					}

				}

				$htmlDirig = "";
				$htmlDirig = "
					<thead>
						<th>Nome</th>
						<th>Cargo</th>
						<th>Cliente Matriz</th>
						<th>Situação</th>
						<th>Término do Mandato</th>
					</thead>
				";
				$htmlDirig .= "<tbody>";
				for ($i=0; $i < count($dirigentes) ; $i++) { 
					
					$htmlDirig .= "<tr class='tbl-item'>";
					$htmlDirig .= "<td class='title'>{$dirigentes[$i]->dirigente}</td>";
					$htmlDirig .= "<td class='desc'>{$dirigentes[$i]->cargo}</td>";
					$htmlDirig .= "<td>{$dirigentes[$i]->empresa}</td>";
					$htmlDirig .= "<td>{$dirigentes[$i]->situacao}</td>";
					$htmlDirig .= "<td>{$dirigentes[$i]->termino}</td>";
					$htmlDirig .= "</tr>";
				}
				$htmlDirig .= "</tbody>";
				$this->logger->debug($htmlDirig);
				$response['response_data']['dirigentes'] = $htmlDirig;

			}else{
				$response = $this->response;
			}

		}else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		return $response;
	}


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


	function getTable($data = null)
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
				SELECT distinct
				cu.*
				FROM cliente_grupo as grupo inner join
				cliente_matriz as cm ON cm.cliente_grupo_id_grupo_economico = grupo.id_grupo_economico 
				INNER JOIN cliente_unidades as cu on cu.cliente_matriz_id_empresa = cm.id_empresa 
				where grupo.id_grupo_economico = {$data['gec']} {$data['filtro']};				
				";

				$this->logger->debug($sql);
				if ($resultsql = mysqli_query($this->db, $sql)) {

					$html = null;

					while ($obj = $resultsql->fetch_object()) {



						$html .= '<tr class="tbl-item">';
						$html .= '<td class="desc">';
						$html .= $obj->codigo_unidade;
						$html .= '</td>';
						$html .= '<td class="title">';
						$html .= $obj->nome_unidade;
						$html .= '</td>';
						$html .= '<td>';
						$html .= formatCnpjCpf($obj->cnpj_unidade);
						$html .= '</td>';
						$html .= '<td>';
						$html .= 'JAN';
						$html .= '</td>';
						$html .= '<td>';
						$html .= 'SINDLAB';
						$html .= '</td>';
						$html .= '<td>';
						$html .= 'SINDPATR';
						$html .= '</td>';
						$html .= '</tr>';
					}

					$response['response_data']['tabf'] 	= $html;
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
	
}
?>
