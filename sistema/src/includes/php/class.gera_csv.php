<?php
/**
 * @author    {Lucas A. Rodrigues Volpati}
 * @package   {1.0.0}
 * @description	{ }
**/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


class gera_csv{
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

				//LISTA LOCALIDADE
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

				$opt = "<option value=''></option>";

				while ($obj = $resultReg->fetch_object()) {
					$opt .= "<option value='regiao+".$obj->regiao."'>".$obj->regiao."</option>";
				}

				while ($obj = $resultUf->fetch_object()) {
					$opt .= "<option value='uf+".$obj->uf."'>".$obj->uf."</option>";
				}

				while ($obj = $resultMun->fetch_object()) {
					$opt .= "<option value='municipio+".$obj->municipio."'>".$obj->municipio."</option>";
				}

				$response['response_data']['lista_local'] = $opt;

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

				//LISTA CLASSE CNAE (CATEGORIA)
				$sqlCat = 'SELECT * FROM classe_cnae';
				$resultCat = mysqli_query( $this->db, $sqlCat );

				$optcat = "<option value=''></option>";
				while ($objCat = $resultCat->fetch_object()) {
					$this->logger->debug(  $objCat );

					$optcat .= '<option value="'.$objCat->id_cnae.'">';
					$optcat .= $objCat->descricao_subclasse;
					$optcat .= '</option>';

				}

				$response['response_data']['optionCategoria'] = $optcat;

				//LISTA GRUPO ECONOMICO
				$sql = "SELECT
							id_grupo_economico,
							nome_grupoeconomico
						FROM cliente_grupo
				";

				$result = mysqli_query( $this->db, $sql );

				$optGrupo = "<option value=''></option>";
				while($obj = $result->fetch_object()){

					$optGrupo .= '<option value="'.$obj->id_grupo_economico.'">';
					$optGrupo .= $obj->nome_grupoeconomico;
					$optGrupo .= '</option>';
				}

				$response['response_data']['grupo_economico'] = $optGrupo;


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

				$optEmp = "<option value=''></option>";
				while ($objEmp = $resultEmp->fetch_object()) {
					$optEmp .= "<option value='{$objEmp->id_sinde}'>{$objEmp->razaosocial_sinde}</option>";
				}

				$response['response_data']['sind_emp'] = $optEmp;

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
				$optPatr = "<option value=''></option>";
				while ($objPatr = $resultPatr->fetch_object()) {
					$optPatr .= "<option value='{$objPatr->id_sindp}'>{$objPatr->razaosocial_sp}</option>";
				}

				$response['response_data']['sind_patr'] = $optPatr;


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


				//LISTA GRUPO CLÁUSULAS
				$sql = "SELECT
							*
						FROM
							grupo_clausula
						ORDER BY nome_grupo ASC
				";

				$resultsql = mysqli_query( $this->db, $sql );

				$group = "<option value=''></option>";

				while($obj = $resultsql->fetch_object()){

					$group .= '<option value="'.$obj->idgrupo_clausula.'">';
					$group .= $obj->nome_grupo;
					$group .= '</option>';
				}

				$response['response_data']['grupoClausulas'] = $group;


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





				$where = "";

				if ($filter['nome_doc'] != "") {
					if (count($filter['nome_doc']) > 1) {
						$string = "";
						foreach ($filter['nome_doc'] as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$where .= $where != "" ? " AND doc.tipo_doc_idtipo_doc IN ({$string})" : " doc.tipo_doc_idtipo_doc IN ({$string})";
					}else {

						$string = "";
						foreach ($filter['nome_doc'] as $value) {

							$string .= "'{$value}'";
						}

						$where .= $where != "" ? " AND doc.tipo_doc_idtipo_doc = {$string}" : " doc.tipo_doc_idtipo_doc = {$string}";
					}
				}
				$this->logger->debug( $where);

				if($filter['categoria'] != "") {
					$string = [];
					foreach ($filter['categoria'] as $value) {
						$id = '{"id":'.$value.'}';

						array_push($string, " JSON_CONTAINS(doc.cnae_doc, '{$id}', '$') ");
					}

					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND {$newString}" : " {$newString}";
				}

				$this->logger->debug( $where);
				//ALOU?

				if ($filter['localidade'] != "" ) {

					$string = [];
					foreach ($filter['localidade'] as $value) {
						$column = strstr($value, "+", true);
						$content = substr(strstr($value, "+"), 1);

						$id = '{"'.$column.'":'.$content.'}';

						array_push($string, " JSON_CONTAINS(doc.abrangencia, '{$id}', '$') ");

					}
					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND {$newString}" : " {$newString}";
				
				}
				$this->logger->debug( $where);

				//FILTRO GRUPO ECONOMICO
				if ($filter['grupo'] != "" && $filter['grupo'] != 0) {

					$id = '{"g":'.$filter['grupo'].'}';

					$where .= $where != "" ? " AND JSON_CONTAINS(doc.cliente_estabelecimento, '{$id}', '$') " : " JSON_CONTAINS(doc.cliente_estabelecimento, '{$id}', '$')";
					
				}

				$this->logger->debug($where);

				//FILTRO MATRIZ
				if ($filter['matriz'] != "") {
					$string = [];
					foreach ($filter['matriz'] as $value) {
						$id = '{"m":'.$value.'}';

						array_push($string, " JSON_CONTAINS(doc.cliente_estabelecimento, '{$id}', '$') ");
					}

					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND {$newString}" : " {$newString}";
				}

				$this->logger->debug($where);

				//FILTRO UNIDADE
				if ($filter['unidade'] != "") {

					$string = [];
					foreach ($filter['unidade'] as $value) {
						$id = '{"u":'.$value.'}';

						array_push($string, " JSON_CONTAINS(doc.cliente_estabelecimento, '{$id}', '$') ");
					}

					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND {$newString}" : " {$newString}";
					
				}


				$this->logger->debug( $filter['sindicato_laboral']);

				if($filter['sindicato_laboral'] != "") {
					$string = [];
					foreach ($filter['sindicato_laboral'] as $value) {
						$id = '{"id":'.$value.'}';

						array_push($string, " JSON_CONTAINS(doc.sind_laboral, '{$id}', '$') ");
					}

					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND {$newString}" : " {$newString}";
				}

				$this->logger->debug( $where);

				if ($filter['sindicato_patronal'] != "") {
					$string = [];
					foreach ($filter['sindicato_patronal'] as $value) {

						$id = '{"id":'.$value.'}';

						array_push($string, " JSON_CONTAINS(doc.sind_patronal, '{$id}', '$') ");
					}
					
					$newString = implode(" OR ", $string);
					$where .= $where != "" ? " AND {$newString}" : " {$newString}";
				}

				$this->logger->debug( $where);

				if ($filter['data_base'] != "") {

					if (count($filter['data_base']) > 0) {
						$string = "";
						foreach ($filter['data_base'] as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$where .= $where != "" ? " AND doc.database_doc IN ({$string})" : " doc.database_doc IN ({$string})";
					}
					else {

						$string = "";
						foreach ($filter['data_base'] as $value) {

							$string .= "'{$value}'";
						}

						$where .= $where != "" ? " AND doc.database_doc = {$string}" : " doc.database_doc = {$string}";
					}
				}
				$this->logger->debug( $where);

				if ($filter['grupo_clausula'] != "") {

					if (!is_array($filter['grupo_clausula'])) {
						$grupo = explode(",", $filter['grupo_clausula']);
					}else {
						$grupo = $filter['grupo_clausula'];
					}

					if (count($grupo) > 0) {
						$string = "";
						foreach ($grupo as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$where .= $where != "" ? " AND gp.idgrupo_clausula IN ({$string})" : " gp.idgrupo_clausula IN ({$string})";
					}else {

						$string = "";
						foreach ($grupo as $value) {

							$string .= "'{$value}'";
						}

						$where .= $where != "" ? " AND gp.idgrupo_clausula = {$string}" : " gp.idgrupo_clausula = {$string}";
					}

				}
				$this->logger->debug( $where);

				if ($filter['lista_clausula'] != "") {

					if (!is_array($filter['lista_clausula'])) {
						$clau = explode(",", $filter['lista_clausula']);
					}else {
						$clau = $filter['lista_clausula'];
					}

					if (count($clau) > 0) {
						$string = "";
						foreach ($clau as $value) {

							$string .= "'{$value}'" . ',';
						}

						$string = implode(",", array_filter(explode(",", $string)));
						$where .= $where != "" ? " AND cl.estrutura_clausula_id_estruturaclausula IN ({$string})" : " cl.estrutura_clausula_id_estruturaclausula IN ({$string})";
					}else {

						$string = "";
						foreach ($clau as $value) {

							$string .= "'{$value}'";
						}

						$where .= $where != "" ? " AND cl.estrutura_clausula_id_estruturaclausula = {$string}" : " cl.estrutura_clausula_id_estruturaclausula = {$string}";
					}

				}



				$this->logger->debug( $where);

				$dataAprovacao = $data['data_aprovacao'];
				$vigencia = $data['vigencia'];

				if ($dataAprovacao) {
					//Data inicial
					//Aprovacao
					$iniDate = strstr($dataAprovacao, ' -', true);
					$iniDate = date("Y-m-d", strtotime(implode("-", explode("/", $iniDate))));

					//Data final
					//aprovacao
					$separator = mb_strpos($dataAprovacao, "-");
					$endDate = trim(substr($dataAprovacao, $separator + 1));
					$finalDate = date("Y-m-d", strtotime(implode("-", explode("/", $endDate))));

					$where .= $where != "" ? " AND gr.data_aprovacao BETWEEN '{$iniDate}' AND '{$finalDate}'" : " gr.data_aprovacao BETWEEN '{$iniDate}' AND '{$finalDate}'";
				}

				if ($vigencia) {
					//vigencia
					$vigenIniDate = strstr($vigencia, ' -', true);
					$vigenIniDate = date("Y-m-d", strtotime(implode("-", explode("/", $vigenIniDate))));


					//vigencia
					$separator = mb_strpos($vigencia, "-");
					$vigenEndDate = trim(substr($vigencia, $separator + 1));
					$vigenFinalDate = date("Y-m-d", strtotime(implode("-", explode("/", $vigenEndDate))));

					$where .= $where != "" ? " AND doc.validade_inicial >= '{$vigenIniDate}' AND doc.validade_final <= '{$vigenFinalDate}'" : " doc.validade_inicial >= '{$vigenIniDate}' AND doc.validade_final <= '{$vigenFinalDate}'";

				}








				$this->logger->debug( $filter);

				//TITULOS DINAMICOS
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

							WHERE ad.nmtipoinformacaoadicional IS NOT NULL AND cl.clausula_geral_id_clau is not null
							GROUP BY ad.nmtipoinformacaoadicional
							ORDER BY CASE ad.nmtipoinformacaoadicional WHEN 'Nome da cláusula' THEN 0 ELSE 1 END;
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
					$column = str_replace(" ", "_", str_replace([".", "-", "?", "!", "&", "*", "=", "+", "<", ">", "(", ")", "/", "%", "#", "$", "@"], " ", $obj->nmtipoinformacaoadicional));

					$column = preg_replace($pattern, explode(" ","a A e E i I o O u U n N c C"),$column);

					array_push($query, 'max(case when src.nmtipoinformacaoadicional = "'.$obj->nmtipoinformacaoadicional.'" then value END) as '.strtolower($column).'');
					// array_push($query, 'IFNULL(GROUP_CONCAT(case when tab2.info_adicional = "'.$obj->nmtipoinformacaoadicional.'" then tab2.conteudo END), GROUP_CONCAT(IFNULL(case when tab2.info_adicional = "'.$obj->nmtipoinformacaoadicional.'" then tab2.conteudo END,null))) as '.strtolower($column).'');


					array_push($title, strtolower($column));
				}


				$query = implode(", ", $query);

				$this->logger->debug( $query);

				$ifExist = $where == "" ? "" : "WHERE";


				//GERA TABELA COMPLETA
				$sql = "SELECT
							src.grupo_dados,
							src.estrutura_clausula_id_estruturaclausula, src.nome_informacao,
							{$query},
							laboral,
							cnpj_sinde,
							patronal,
							cnpj_sp,
							categoria,
							nome_doc,
							data_base,
							src.validade_final,
							estrutura,
							vigencia,
							nome_grupo,
							id_doc,
							localidade
						FROM
						(
						select cl.clausula_geral_id_clau, ad.nmtipoinformacaoadicional, cl.grupo_dados, cl.estrutura_clausula_id_estruturaclausula, cl.nome_informacao,
							case 
								when cl.texto != '' then cl.texto
								when cl.numerico != 0 then cl.numerico
								when cl.descricao != '' then cl.descricao
								when cl.data then date_format(cl.data, '%d/%m/%Y')
								when cl.percentual != 0 then concat(format(cl.percentual, 2), '%')
								when cl.hora != '' then cl.hora
								when cl.combo != '' then cl.combo else null end as value,
							JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].sigla'))  AS laboral,
							JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].sigla'))  AS patronal,
							JSON_UNQUOTE(JSON_EXTRACT(doc.sind_laboral, '$[0].cnpj')) as cnpj_sinde,
							JSON_UNQUOTE(JSON_EXTRACT(doc.sind_patronal, '$[0].cnpj')) as cnpj_sp,
							JSON_UNQUOTE(JSON_EXTRACT(doc.cnae_doc, '$[0].subclasse')) as categoria,
							DATE_FORMAT(gr.data_aprovacao, '%d/%m/%Y') as vigencia,
							tp.nome_doc as nome_doc,
							doc.database_doc as data_base,
							doc.validade_final,
							doc.sind_laboral,
							cl.estrutura_clausula_id_estruturaclausula as estrutura,
							gp.nome_grupo,
							doc.id_doc,
							doc.uf as localidade
						FROM clausula_geral_estrutura_clausula as cl
						LEFT JOIN ad_tipoinformacaoadicional as ad on ad.cdtipoinformacaoadicional = cl.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
						LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cl.estrutura_clausula_id_estruturaclausula
						LEFT JOIN clausula_geral as gr on gr.id_clau = cl.clausula_geral_id_clau
						LEFT JOIN doc_sind as doc on doc.id_doc = cl.doc_sind_id_doc
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
						LEFT JOIN grupo_clausula as gp on gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
						
						{$ifExist} {$where}
						) src
						
						GROUP BY src.clausula_geral_id_clau, src.estrutura_clausula_id_estruturaclausula, src.nome_informacao, src.grupo_dados
				";

				$this->logger->debug( $sql);

				if ($result = mysqli_query( $this->db, $sql )) {

					$content = [];
					while ($obj = $result->fetch_assoc()) {

						array_push($content, $obj);

					}

					$response['response_data']['title'] = $title;
					$response['response_data']['table'] = $content;

					$this->logger->debug( $content );
				}else {

					$this->logger->debug( $this->db->error );
					$response['response_status']['status']     = 0;
					$response['response_status']['error_code'] = $this->error_code . __LINE__;
					$response['response_status']['msg']        = 'Não foi possível renderizar a tabela.';
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

	function getClausulas( $data = null ){

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

				$idGrupo = $data['id_grupo_clausula'];
				$opt = "<option value=''></option>";
				for ($i=0; $i < count($idGrupo) ; $i++) {
					$sql = "SELECT
							id_estruturaclausula,
							nome_clausula
						FROM estrutura_clausula
						WHERE grupo_clausula_idgrupo_clausula = '{$idGrupo[$i]}'
					";

					$result = mysqli_query( $this->db, $sql );


					while ($obj = $result->fetch_object()) {
						$opt .= "<option value='{$obj->id_estruturaclausula}'>{$obj->nome_clausula}</option>";
					}
				}

				$response['response_data']['lista_clausulas'] = $opt;

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


}
?>
