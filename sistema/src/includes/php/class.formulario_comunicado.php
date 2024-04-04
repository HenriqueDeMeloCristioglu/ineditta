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

include_once __DIR__ . "/helpers.php";

class formulario_comunicado
{
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

  function __construct()
  {

    //Iniciando resposta padrão do construtor.
    $this->response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Classe ' . __CLASS__ . ' iniciada com sucesso.'));

    // Montando o código do erro que será apresentado
    $localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
    $substituir = array("", "", "", "", "-");
    $this->error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

    // Declarando os caminhos principais do sistema.
    $localizar = array("\\", "/includes/php");
    $substituir = array("/", "");
    $this->path = str_replace($localizar, $substituir, __DIR__);

    $fileLogApi = $this->path . '/includes/php/log4php/Logger.php';

    if (file_exists($fileLogApi)) {

      include_once($fileLogApi);

      $fileLogConfig = $this->path . '/includes/config/config.log.xml';

      if (file_exists($fileLogConfig)) {
        //Informado as configuracoes do log4php
        Logger::configure($fileLogConfig);

        //Indica qual bloco do XML corresponde as configuracoes
        $this->logger = Logger::getLogger('config.log');
      } else {
        $this->response['response_status']['status'] = 0;
        $this->response['response_status']['error_code'] = $this->error_code . __LINE__;
        $this->response['response_status']['msg'] = "Não foi possível localizar as configurações do log.";
      }
    } else {
      $this->response['response_status']['status'] = 0;
      $this->response['response_status']['error_code'] = $this->error_code . __LINE__;
      $this->response['response_status']['msg'] = 'Não foi possível encontrar o plugins log4php.';
    }

    if ($this->response['response_status']['status'] == 1) {

      $fileGetConfig = $this->path . "/includes/config/config.get.php";

      // Carregando as configuração do Mirrada
      if (file_exists($fileGetConfig)) {

        include_once($fileGetConfig);

        $this->getconfig = new getconfig();

        if ($this->getconfig->response['response_status']['status'] == 0) {
          $this->response = $this->getconfig->response;
        }
      } else {
        $this->response['response_status']['status'] = 0;
        $this->response['response_status']['error_code'] = $this->error_code . __LINE__;
        $this->response['response_status']['msg'] = 'Não foi possível localizar o arquivo de configuração (mirada-config).';
      }
    }
  }

  function connectdb()
  {

    if ($this->response['response_status']['status'] == 1) {

      // Carregando a resposta padrão da função
      $response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

      $qualitor_db = $this->getconfig->searchConfigDatabase('ineditta');

      if ($qualitor_db['response_status']['status'] == 1) {

        $parameters = $qualitor_db['response_data'];

        if (file_exists($this->path . '/includes/php/db.mysql.php')) {

          include_once($this->path . '/includes/php/db.mysql.php');

          // Criando o objeto de conexão com o banco de dados Qualitor
          $apidbmysql = new apidbmysql();

          $db = $apidbmysql->connection($parameters);

          if ($db['response_status']['status'] == 1) {

            $this->db = $db['response_data']['connection'];

            $this->logger->debug($db['response_data']['connection']);
          } else {
            $response = $db;
          }
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Não foi possível encontrar o db.mysql.';
        }
      } else {
        $response = $qualitor_db;
      }
    } else {
      $response = $this->response;
    }

    return $response;
  }

  function getDocumentos($data = null)
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
        // $sql = "select doc.*,DATE_FORMAT(doc.data_upload, '%d/%m/%Y %H:%i:%s') as inclusao,
        // concat(DATE_FORMAT(doc.vigencia_inicial, '%d/%m/%Y'), ' até ',
        //  IF(DATE_FORMAT(doc.vigencia_final, '%d/%m/%Y') = '00/00/0000',
        //  '(ainda em vigor)', DATE_FORMAT(doc.vigencia_final, '%d/%m/%Y'))) as
        //  vigencia,td.nome_doc,se.sigla_sinde,sp.denominacao_sp from documentos as doc 
        // 				left join tipo_doc as td on td.idtipo_doc = doc.tipo_doc_idtipo_doc
        // 				left join sind_emp as se on se.id_sinde = doc.sind_emp_id_sinde
        // 				left  join sind_patr as sp on sp.id_sindp = doc.sind_patr_id_sindp;";

        // $html = "";
        // while ($obj = $resultsql->fetch_object()) {
        // 	$html .= '<tr class="odd gradeX tbl-item">';

        // 	$html .= '<td><input type="checkbox" onclick="selectDoc( \'' . $obj->iddocumentos . '\',\'' . $obj->caminho_arquivo . '\');" id="check' . $obj->iddocumentos . '"></td>';
        // 	$html .= '<td>' . $obj->nome_doc . '</td>';
        // 	$html .= '<td>' . $obj->sigla_sinde . '</td>';
        // 	$html .= '<td>' . $obj->inclusao . '</td>';
        // 	$html .= '<td>' . $obj->vigencia . '</td>';
        // 	$html .= '<td>' . $obj->comentario_legislacao . '</td>';
        // 	$html .= '<td><a href="' . $obj->caminho_arquivo . '" target="_blank" class="btn-default-alt"  ><i class="fa fa-file-text"></i></a></td>';
        // 	$html .= '</tr>';
        // }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

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


        //FÉ

        $filter = [];



        $filter = array_merge($data, $filter);
        unset($filter["module"]);
        unset($filter["action"]);

        $this->logger->debug($data);



        $where = ""; //main.data_aprovacao IS NOT NULL

        if ($filter['nome_doc'] != "") {
          if (count($filter['nome_doc']) > 1) {
            $string = "";
            foreach ($filter['nome_doc'] as $value) {

              $string .= "'{$value}'" . ',';
            }

            $string = implode(",", array_filter(explode(",", $string)));
            $where .= ($where != "" ? " AND doc.tipo_doc_idtipo_doc IN ({$string})" : " doc.tipo_doc_idtipo_doc IN ({$string})");
          } else {

            $string = "";
            foreach ($filter['nome_doc'] as $value) {

              $string .= "{$value}";
            }

            $where .= ($where != "" ? " AND td.nome_doc = {$string}" : " td.nome_doc = {$string}");
          }
        }
        $this->logger->debug($where);
        if ($filter['categoria'] != "") {
          $string = [];
          foreach ($filter['categoria'] as $value) {
            $id = '{"id":' . $value . '}';

            array_push($string, " JSON_CONTAINS(doc.cnae_doc, '{$id}', '$') ");
          }

          $newString = implode(" OR ", $string);
          $where .= $where != "" ? " AND {$newString}" : " {$newString}";
        }
        $this->logger->debug($where);
        if ($filter['localidade'] != "") {

          $string = [];
          foreach ($filter['localidade'] as $value) {
            $column = strstr($value, "+", true);
            $content = substr(strstr($value, "+"), 1);

            $id = '{"' . $column . '":"' . $content . '"}';

            array_push($string, " JSON_CONTAINS(doc.abrangencia, '{$id}', '$') ");
          }
          $newString = implode(" OR ", $string);
          $where .= $where != "" ? " AND {$newString}" : " {$newString}";
        }
        $this->logger->debug($where);

        //FILTRO GRUPO ECONOMICO
        if ($filter['grupo'] != "" && $filter['grupo'] != 0) {
          $string = [];
          $id = '{"g":' . $filter['grupo'] . '}';

          array_push($string, " JSON_CONTAINS(doc.cliente_estabelecimento, '{$id}', '$') ");


          $newString = implode(" OR ", $string);
          $where .= $where != "" ? " AND  ({$newString} OR IF(doc.modulo like 'COMERCIAL',isnull(doc.cliente_estabelecimento) OR doc.cliente_estabelecimento like '[]', 1= 0))" : "({$newString} OR IF(doc.modulo like 'COMERCIAL',isnull(doc.cliente_estabelecimento) OR doc.cliente_estabelecimento like '[]', 1= 0))";
        }


        $this->logger->debug($where);

        //FILTRO MATRIZ
        if ($filter['matriz'] != "") {
          $string = [];
          foreach ($filter['matriz'] as $value) {
            $id = '{"m":' . $value . '}';

            array_push($string, " JSON_CONTAINS(doc.cliente_estabelecimento, '{$id}', '$') ");
          }

          $newString = implode(" OR ", $string);
          $where .= $where != "" ? " AND   {$newString}" : " {$newString}";
        }

        $this->logger->debug($where);

        //FILTRO UNIDADE
        if ($filter['unidade'] != "") {
          $string = [];
          foreach ($filter['unidade'] as $value) {
            $id = '{"u":' . $value . '}';

            array_push($string, " JSON_CONTAINS(doc.cliente_estabelecimento, '{$id}', '$') ");
          }

          $newString = implode(" OR ", $string);
          $where .= $where != "" ? " AND   {$newString}" : "  {$newString}";
        }

        $this->logger->debug($where);

        $this->logger->debug($filter['sindicato_laboral']);

        if ($filter['sindicato_laboral'] != "") {
          $string = [];
          foreach ($filter['sindicato_laboral'] as $value) {
            $id = '{"id":' . $value . '}';

            array_push($string, " JSON_CONTAINS(doc.sind_laboral, '{$id}', '$') ");
          }

          $newString = implode(" OR ", $string);
          $where .= $where != "" ? " AND {$newString}" : " {$newString}";
        }

        $this->logger->debug($where);

        if ($filter['sindicato_patronal'] != "") {
          $string = [];
          foreach ($filter['sindicato_patronal'] as $value) {

            $id = '{"id":' . $value . '}';

            array_push($string, " JSON_CONTAINS(doc.sind_patronal, '{$id}', '$') ");
          }

          $newString = implode(" OR ", $string);
          $where .= $where != "" ? " AND {$newString}" : " {$newString}";
        }

        $this->logger->debug($where);


        $this->logger->debug($where);
        $this->logger->debug($filter['data_base'][0] == "todos" ? "verdade" : "falso");
        if (!empty($filter['data_base']) && $filter['data_base'][0] != "todos") {

          if (count($filter['data_base']) > 1) {
            $string = "";
            foreach ($filter['data_base'] as $value) {

              $string .= "'{$value}'" . ',';
            }

            $string = implode(",", array_filter(explode(",", $string)));
            $where .= ($where != "" ? " AND doc.database_doc IN ({$string})" : " doc.database_doc IN ({$string})");
          } else {

            $string = "";
            foreach ($filter['data_base'] as $value) {

              $string .= "'{$value}'";
            }

            $where .= ($where != "" ? " AND doc.database_doc = {$string}" : " doc.database_doc = {$string}");
          }
        }


        $vigencia = $data['vigencia'];


        if ($data['vigencia']) {
          //vigencia
          $vigenIniDate = strstr($vigencia, ' -', true);
          $vigenIniDate = date("Y-m-d", strtotime(implode("-", explode("/", $vigenIniDate))));

          $separator = mb_strpos($vigencia, "-");
          $vigenEndDate = trim(substr($vigencia, $separator + 1));
          $vigenFinalDate = date("Y-m-d", strtotime(implode("-", explode("/", $vigenEndDate))));

          $where .= ($where != "" ? " AND doc.validade_inicial >= '{$vigenIniDate}' AND doc.validade_final <= '{$vigenFinalDate}'" : " doc.validade_inicial >= '{$vigenIniDate}' AND doc.validade_final <= '{$vigenFinalDate}'");
        }












        if ($data['tipo_doc'] == 'geral') {
          $grupoecon = $data['gec'];
          $ANDDD = $where == "" ? "" : "AND";
          $sql = "SELECT 
								group_concat(distinct ec.nome_clausula) as nome_clausula, doc.*,DATE_FORMAT(doc.data_upload, '%d/%m/%Y %H:%i:%s') as inclusao,
								concat(DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y'), ' até ',
								IF(DATE_FORMAT(doc.validade_final, '%d/%m/%Y') = '00/00/0000',
								'(não informado)', DATE_FORMAT(doc.validade_final, '%d/%m/%Y'))) as
								vigencia,
								CASE WHEN fg.doc_sind_id_doc IS NOT NULL THEN 'Sim' ELSE 'Não' END AS has_form,
								group_concat(distinct td.nome_doc) as nome_doc,
								REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_laboral, '$[*].sigla') SEPARATOR ','), '\"', ''), '[', ''), ']', '') as sigla_sinde,
								REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_patronal, '$[*].sigla') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  as sigla_sp 
							FROM doc_sind as doc 
								left join tipo_doc as td on td.idtipo_doc = doc.tipo_doc_idtipo_doc
								left join formulario_grupo as fg on fg.doc_sind_id_doc = doc.id_doc
								left join estrutura_clausula as ec on JSON_CONTAINS(doc.referencia, CONCAT('\"',ec.id_estruturaclausula, '\"'),'$')
							WHERE doc.modulo like 'COMERCIAL'and (JSON_CONTAINS(doc.cliente_estabelecimento, '{\"g\":" . $grupoecon . "}', '$') OR
							 isnull(doc.cliente_estabelecimento) OR doc.cliente_estabelecimento like '[]' OR '" . $grupoecon . "' = 'cliente_grupo_id_grupo_economico')
							 " . $ANDDD . " " . $where . " 
							  GROUP BY id_doc;
					";

          $this->logger->debug($sql);

          if ($resultsql = mysqli_query($this->db, $sql)) {

            $html = "<thead>
						<th></th>
						<th>Formulário</th>
														<th>Nome</th>
														<th>Sigla Laboral</th>
														<th>Inclusão</th>
														<th>Vigência</th>
														<th>Descrição</th>
														<th>Ver</th>
					</thead>
					<tbody>";
            while ($obj = $resultsql->fetch_object()) {
              $html .= '<tr class="odd gradeX tbl-item">';

              $html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdForm( ' . $obj->id_doc . ');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
              $html .= '<td>' . $obj->has_form . '</td>';
              $html .= '<td>' . $obj->nome_doc . '</td>';
              $html .= '<td>' . $obj->sigla_sinde . '</td>';
              $html .= '<td>' . $obj->inclusao . '</td>';
              $html .= '<td>' . $obj->vigencia . '</td>';
              $html .= '<td>' . $obj->descricao_documento . '</td>';
              $html .= '<td><a href="' . $obj->caminho_arquivo . '" target="_blank" class="btn-default-alt"  ><i class="fa fa-file-text"></i></a></td>';
              $html .= '</tr>';
            }

            $html .= "
					</tbody>";

            $response['response_data']['doc_table'] = $html;

            $this->logger->debug($obj);

            //LISTA TIPO DOC GERAL
            $sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'N'  ORDER BY nome_doc ASC";
            $result = mysqli_query($this->db, $sqlTipo);

            $option = "<option value=''></option>";
            while ($obj = $result->fetch_object()) {
              $this->logger->debug($obj);

              $option .= '<option value=\'"' . $obj->nome_doc . '"\'>';
              $option .= $obj->nome_doc;
              $option .= '</option>';
            }

            $response['response_data']['nnn'] = $option;

            //LISTA TIPO DOC
            $sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'S' ORDER BY nome_doc ASC";
            $result = mysqli_query($this->db, $sqlTipo);

            $option = "<option value=''></option>";
            while ($obj = $result->fetch_object()) {
              $this->logger->debug($obj);

              $option .= '<option value=\'"' . $obj->nome_doc . '"\'>';
              $option .= $obj->nome_doc;
              $option .= '</option>';
            }

            $response['response_data']['nome_doc'] = $option;
          } else {
            $this->logger->debug($this->db->error);

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Falha ao efetuar SELECT.';
          }
        } else {
          $grupoecon = $data['gec'];
          $sn = $data['form_sn'];
          // $sql = "select DISTINCT group_concat(ec.nome_clausula separator ', ') as nome_clausula,se.sigla_sinde,sp.denominacao_sp,doc.*,ds.*,DATE_FORMAT(ds.data_aprovacao, '%d/%m/%Y %H:%i:%s') as registro,
          // concat(DATE_FORMAT(ds.validade_inicial, '%d/%m/%Y'), ' até ',
          //  IF(DATE_FORMAT(ds.validade_final, '%d/%m/%Y') = '00/00/0000',
          //  '(não informado)', DATE_FORMAT(ds.validade_final, '%d/%m/%Y'))) as
          //  validade,td.nome_doc as nome, tuc.tipo_negocio from documentos_localizados as doc 
          //  left join doc_sind as ds on ds.documento_id_documento = doc.id_documento
          //  left join doc_sind_referencia as dsr on dsr.doc_sind_id_doc = ds.id_doc
          //  left join estrutura_clausula as ec on ec.id_estruturaclausula = dsr.estrutura_clausula_id_estruturaclausula
          //  left join doc_sind_sind_emp as dsc on dsc.doc_sind_id_doc = ds.id_doc
          //  left join doc_sind_sind_patr as dsp on dsp.doc_sind_id_doc = ds.id_doc
          //  left join doc_sind_cliente_unidades as dccu on dccu.doc_sind_id_doc = ds.id_doc
          //  left join sind_emp as se on se.id_sinde = dsc.sind_emp_id_sinde
          //  left join sind_patr as sp on sp.id_sindp = dsp.sind_patr_id_sindp
          // 				left join tipo_doc as td on td.idtipo_doc = ds.tipo_doc_idtipo_doc
          // 				left join tipounidade_cliente as tuc on tuc.id_tiponegocio = ds.tipounidade_cliente_id_tiponegocio
          // 				WHERE
          //                 IF(td.processado = 'S', IF(td.permissao like 'não', dccu.cliente_unidades_id_unidade IN (select distinct cuni.id_unidade from cliente_unidades as cuni inner join cliente_matriz as cm ON cm.id_empresa = cuni.cliente_matriz_id_empresa
          //                 WHERE cm.cliente_grupo_id_grupo_economico = {$grupoecon}),
          // 				1=1), 1=1) 	
          // 				AND
          // 				 id_sinde IN (select distinct se.id_sinde from cliente_grupo as grupo left join
          // 				cliente_matriz as cm ON cm.cliente_grupo_id_grupo_economico = grupo.id_grupo_economico LEFT JOIN cliente_unidades as cu ON
          // 				 cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
          // 				 localizacao as loc on loc.id_localizacao = cu.localizacao_id_localizacao left join
          // 				 cnae_emp as ce ON ce.cliente_unidades_id_unidade = cu.id_unidade LEFT JOIN
          // 				 base_territorialsindemp AS bl on (bl.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
          // 				 AND bl.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
          // 				 sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1
          // 				 WHERE grupo.id_grupo_economico = {$grupoecon} AND  IF(td.processado = 'S', IF(td.permissao like 'não', grupo.id_grupo_economico = {$grupoecon}, 1=1), 1=1) {$data['filtro']})
          //  AND
          //  (id_sindp IN (  select distinct sp.id_sindp from cliente_grupo as grupo left join
          //  cliente_matriz as cm ON cm.cliente_grupo_id_grupo_economico = grupo.id_grupo_economico LEFT JOIN cliente_unidades as cu ON
          //   cu.cliente_matriz_id_empresa = cm.id_empresa LEFT JOIN
          //   localizacao as loc on loc.id_localizacao = cu.localizacao_id_localizacao left join
          //   cnae_emp as ce ON ce.cliente_unidades_id_unidade = cu.id_unidade LEFT JOIN
          //   base_territorialsindpatro AS bp on (bp.classe_cnae_idclasse_cnae = ce.classe_cnae_idclasse_cnae 
          //   AND bp.localizacao_id_localizacao1 = cu.localizacao_id_localizacao) LEFT Join 
          //   sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp 
          //   WHERE grupo.id_grupo_economico = {$grupoecon} AND  IF(td.processado = 'S', IF(td.permissao like 'não', grupo.id_grupo_economico = {$grupoecon}, 1=1), 1=1) {$data['filtro']})
          //   {$data['filtro2']} OR isnull(id_sindp)) group by doc.id_documento

          //   ;"
          $ANDDD = $where == "" ? "" : "AND";
          $sql = "SELECT 
					group_concat(distinct ec.nome_clausula) as nome_clausula, doc.*,DATE_FORMAT(doc.data_upload, '%d/%m/%Y %H:%i:%s') as inclusao,
					concat(DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y'), ' até ',
					IF(DATE_FORMAT(doc.validade_final, '%d/%m/%Y') = '00/00/0000',
					'(não informado)', DATE_FORMAT(doc.validade_final, '%d/%m/%Y'))) as
					vigencia,
					CASE WHEN fg.doc_sind_id_doc IS NOT NULL THEN 'Sim' ELSE 'Não' END AS has_form,
					group_concat(distinct td.nome_doc) as nome_doc,
					REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_laboral, '$[*].sigla') SEPARATOR ','), '\"', ''), '[', ''), ']', '') as sigla_sinde,
					REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_patronal, '$[*].sigla') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  as sigla_sp 
				FROM doc_sind as doc 
					left join tipo_doc as td on td.idtipo_doc = doc.tipo_doc_idtipo_doc
					left join formulario_grupo as fg on fg.doc_sind_id_doc = doc.id_doc
					left join estrutura_clausula as ec on JSON_CONTAINS(doc.referencia, CONCAT('\"',ec.id_estruturaclausula, '\"'),'$')
				WHERE doc.modulo like 'SISAP' AND (CASE WHEN fg.doc_sind_id_doc IS NOT NULL THEN 'Sim' ELSE 'Não' END) like '" . $sn . "' and doc.data_aprovacao IS NOT NULL and 
				(JSON_CONTAINS(doc.cliente_estabelecimento, '{\"g\":" . $grupoecon . "}', '$') OR '" . $grupoecon . "' = 'cliente_grupo_id_grupo_economico') 
				 " . $ANDDD . " " . $where . " 
				 GROUP BY id_doc;
					";

          $this->logger->debug($sql);

          if ($resultsql = mysqli_query($this->db, $sql)) {

            $html = "<thead>
						<th></th>
						<th>Formulário</th>
						<th>Nome</th>
						<th>Sigla Laboral</th>
						<th>Data Inclusão</th>
						<th>Validade</th>
						<th>Assunto</th>
						<th>Comentários</th>
						<th>Ver</th>
					</thead>
					<tbody>";
            while ($obj = $resultsql->fetch_object()) {
              $html .= '<tr class="odd gradeX tbl-item">';

              $html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdForm( ' . $obj->id_doc . ');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
              $html .= '<td>' . $obj->has_form . '</td>';
              $html .= '<td>' . $obj->nome_doc . '</td>';
              $html .= '<td>' . $obj->sigla_sinde . '</td>';
              $html .= '<td>' . $obj->inclusao . '</td>';
              $html .= '<td>' . $obj->vigencia . '</td>';
              $html .= '<td>' . $obj->nome_clausula . '</td>';
              $html .= '<td>' . $obj->observacao . '</td>';
              $html .= '<td><a href="' . $obj->caminho_arquivo . '" target="_blank" class="btn-default-alt"  ><i class="fa fa-file-text"></i></a></td>';
              $html .= '</tr>';
            }

            $html .= "
					</tbody>
					
					";

            $response['response_data']['doc_table'] = $html;

            $this->logger->debug($obj);

            //LISTA TIPO DOC GERAL
            $sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'N'  ORDER BY nome_doc ASC";
            $result = mysqli_query($this->db, $sqlTipo);

            $option = "<option value=''></option>";
            while ($obj = $result->fetch_object()) {
              $this->logger->debug($obj);

              $option .= '<option value=\'"' . $obj->nome_doc . '"\'>';
              $option .= $obj->nome_doc;
              $option .= '</option>';
            }

            $response['response_data']['nnn'] = $option;

            //LISTA TIPO DOC
            $sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'S' ORDER BY nome_doc ASC";
            $result = mysqli_query($this->db, $sqlTipo);

            $option = "<option value=''></option>";
            while ($obj = $result->fetch_object()) {
              $this->logger->debug($obj);

              $option .= '<option value=\'"' . $obj->nome_doc . '"\'>';
              $option .= $obj->nome_doc;
              $option .= '</option>';
            }

            $response['response_data']['nome_doc'] = $option;
          } else {
            $this->logger->debug($this->db->error);

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Falha ao efetuar SELECT.';
          }
        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    return $response;
  }


  function getById($data = null)
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

      if ($response['response_status']['status'] == 1) {

        if ($data['indicador'] == 1) {
          $indicador = "INPC";
        } else {
          $indicador = "IPCA";
        }
        $sql = "SELECT
							id_formulario_comunicado,
							indicador, 
							id_usuario,
							origem, 
							fonte, 
							data, 
							dado_projetado, 
							dado_real, 
							cliente_matriz_id_empresa as clt
						FROM
							formulario_comunicado
						WHERE
						id_usuario = '{$data['id_usuario']}' AND indicador = '{$indicador}'
									
					";

        if ($resultsql = mysqli_query($this->db, $sql)) {
          $label = '<label for="origem" class="control-label">Selecione o Período</label>';

          $select = '<select id="periodo-select" class="form-control">';
          $select .= '<option value="0">--</option>';

          while ($obj = $resultsql->fetch_object()) {
            $this->logger->debug($obj);

            $select .= '<option value="' . $obj->id_formulario_comunicado . '">' . $obj->data . '</option>';

            $response['response_data']['indicador'] = $obj->indicador;
            $response['response_data']['origem'] = $obj->origem;
            $response['response_data']['fonte'] = $obj->fonte;
            $response['response_data']['dado_projetado'] = $obj->dado_projetado;
            $response['response_data']['cliente'] = $obj->clt;
          }

          $select .= '</select>';



          $response['response_data']['label'] = $label;
          $response['response_data']['periodo'] = $select;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    $this->logger->debug($response['response_status']['status']);
    $this->logger->debug($response['response_data']);

    return $response;
  }

  function getRegisterById($data = null)
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

      if ($response['response_status']['status'] == 1) {

        $sql = "SELECT
							id_formulario_comunicado,
							indicador,
							dado_projetado, 
							dado_real,
							data as periodo
						FROM
							formulario_comunicado
						WHERE
						id_formulario_comunicado = '{$data['id_formulario_comunicado']}'
									
					";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          while ($obj = $resultsql->fetch_object()) {
            $this->logger->debug($obj);

            $response['response_data']['projetado'] = $obj->dado_projetado;


            $sqlReal = "SELECT * FROM formulario_comunicado_real WHERE periodo = '{$obj->periodo}' AND indicador = '{$obj->indicador}' ";
            $resultReal = mysqli_query($this->db, $sqlReal);
            $objReal = $resultReal->fetch_object();

            $response['response_data']['real'] = $objReal->dado_real;

            $this->logger->debug($objReal);
          }
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    $this->logger->debug($response['response_status']['status']);
    $this->logger->debug($response['response_data']);

    return $response;
  }


  function getByIdReal($data = null)
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

      if ($response['response_status']['status'] == 1) {


        $sql = "SELECT
							*
						FROM
							formulario_comunicado_real
						WHERE
							id = {$data['id_real']}
									
					";
        $this->logger->debug($sql);

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $obj = $resultsql->fetch_object();

          $response['response_data']['indicador_update'] = $obj->indicador;
          $response['response_data']['dado_real'] = $obj->dado_real;
          $response['response_data']['periodo_update'] = $obj->periodo;

          $response['response_status']['status'] = 1;


          $this->logger->debug($obj);
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    $this->logger->debug($response['response_status']['status']);
    $this->logger->debug($response['response_data']);

    return $response;
  }


  function addIndicadores($data = null)
  {
    $this->logger->debug('entrou na classe php');
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


      $this->logger->debug($this->db);

      $lineOne = $data['tabela-fixa'];
      $lineOthers = $data['tabela-extra'];

      if ($response['response_status']['status'] == 1) {

        if ($lineOne) {


          if ($data['origem-input'] == "Ineditta") {
            $id_usuario = 1;
          } else {
            $id_usuario = 2;
          }

          $sql = "INSERT INTO
							formulario_comunicado (indicador, id_usuario,  origem, fonte, data, dado_projetado, cliente_matriz_id_empresa)
						VALUES
							('{$data['indicador-input']}', 
							'{$id_usuario}',
							'{$data['origem-input']}', 
							'{$data['fonte-input']}', 
							'{$lineOne[0]}',
							'{$lineOne[1]}',
							'{$data['cliente-input']}')
					";

          $this->logger->debug($sql);

          if (!mysqli_query($this->db, $sql)) {

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Falha ao efetuar registro.';

            $this->logger->debug($this->db->error);
          } else {
            $response['response_status']['status'] = 1;
            $response['response_status']['error_code'] = null;
            $response['response_status']['msg'] = 'Cadastro realizado com sucesso';
          }
        }

        if ($lineOthers) {

          $list = array_chunk($lineOthers, 2);

          if ($data['origem-input'] == "Ineditta") {
            $id_usuario = 1;
          } else {
            $id_usuario = 2;
          }



          for ($i = 0; $i < count($list); $i++) {

            $sql2 = "INSERT INTO
									formulario_comunicado (indicador, id_usuario, origem, fonte, data, dado_projetado, cliente_matriz_id_empresa)
								VALUES
									('{$data['indicador-input']}', 
									'{$id_usuario}',
									'{$data['origem-input']}', 
									'{$data['fonte-input']}', 
									'{$list[$i][0]}',
									'{$list[$i][1]}',
									'{$data['cliente-input']}')
						";
            $this->logger->debug($sql2);

            if (!mysqli_query($this->db, $sql2)) {

              $response['response_status']['status'] = 0;
              $response['response_status']['error_code'] = $this->error_code . __LINE__;
              $response['response_status']['msg'] = 'Falha ao efetuar registro.';

              $this->logger->debug($this->db->error);
            } else {
              $response['response_status']['status'] = 1;
              $response['response_status']['error_code'] = null;
              $response['response_status']['msg'] = 'Cadastro realizado com sucesso';
            }
          }
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


  function addIndicadorReal($data = null)
  {
    $this->logger->debug('entrou na classe php');
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


      $this->logger->debug($this->db);

      $lineOne = $data['tabela-fixa'];
      $lineOthers = $data['tabela-extra'];

      if ($response['response_status']['status'] == 1) {

        if ($lineOne) {

          $sql = "INSERT INTO
							formulario_comunicado_real (indicador, periodo, dado_real)
						VALUES
							('{$data['indicador-input']}',
							'{$lineOne[0]}',
							'{$lineOne[1]}')
					";

          $this->logger->debug($sql);

          if (!mysqli_query($this->db, $sql)) {

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Falha ao efetuar registro.';

            $this->logger->debug($this->db->error);
          } else {
            $response['response_status']['status'] = 1;
            $response['response_status']['error_code'] = null;
            $response['response_status']['msg'] = 'Cadastro realizado com sucesso';
          }
        }

        if ($lineOthers) {

          $list = array_chunk($lineOthers, 2);

          for ($i = 0; $i < count($list); $i++) {

            $sql2 = "INSERT INTO
									formulario_comunicado_real (indicador, periodo, dado_real)
								VALUES
									('{$data['indicador-input']}', 
									'{$list[$i][0]}',
									'{$list[$i][1]}')
						";
            $this->logger->debug($sql2);

            if (!mysqli_query($this->db, $sql2)) {

              $response['response_status']['status'] = 0;
              $response['response_status']['error_code'] = $this->error_code . __LINE__;
              $response['response_status']['msg'] = 'Falha ao efetuar registro.';

              $this->logger->debug($this->db->error);
            } else {
              $response['response_status']['status'] = 1;
              $response['response_status']['error_code'] = null;
              $response['response_status']['msg'] = 'Cadastro realizado com sucesso';
            }
          }
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



  function updateIndicador($data = null)
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


      if ($response['response_status']['status'] == 1) {
        $sql = "UPDATE 
							formulario_comunicado
						SET  
							indicador = '{$data['indicador-up']}', 
							origem = '{$data['origem-up']}', 
							fonte = '{$data['fonte-up']}', 
							dado_projetado = '{$data['projetado-up']}',
							dado_real = '{$data['real-up']}',
							cliente_matriz_id_empresa = '{$data['cliente-up']}'
						WHERE 
							id_formulario_comunicado = {$data['id_formulario_comunicado']};
						";

        $this->logger->debug($sql);
        if (!mysqli_query($this->db, $sql)) {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';

          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);
          $this->logger->debug($response);
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Registro atualizado com sucesso';
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


  function updateIndicadorReal($data = null)
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


      if ($response['response_status']['status'] == 1) {
        $sql = "UPDATE 
							formulario_comunicado_real
						SET  
							indicador = '{$data['indicador-up']}',
							dado_real = '{$data['real-up']}',
							periodo = '{$data['periodo-up']}'
						WHERE 
							id = {$data['id_real']};
						";

        $this->logger->debug($sql);
        if (!mysqli_query($this->db, $sql)) {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';

          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);
          $this->logger->debug($response);
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Registro atualizado com sucesso';
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











  //CAMPOS





  function getConsultaClausulaCampos($data = null)
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

      //$this->logger->debug(  $connectdb );

      if ($response['response_status']['status'] == 1) {





        $sql = "
					SELECT 
					id_user as id_usuario
					,foto
                    ,nome_usuario
                    ,email_usuario
					,cargo
					,telefone
					,ramal
					,id_user_superior
					,departamento
					FROM 
						usuario_adm;				
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $htmlupdate = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX">';
            $html .= '<td><button onclick="selectSup( ' . $obj->id_usuario . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
            $html .= '<td> <img src="' . $obj->foto . '" height="100" alt="Image preview...">';
            $html .= '<td>';
            $html .= $obj->nome_usuario;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->email_usuario;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->departamento;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->cargo;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->telefone;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->ramal;
            $html .= '</td>';
            $html .= '</tr>';


            $htmlupdate .= '<tr class="odd gradeX">';
            $htmlupdate .= '<td><button onclick="selectSup( ' . $obj->id_usuario . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
            $htmlupdate .= '<td> <img src="' . $obj->foto . '" height="100" alt="Image preview...">';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->nome_usuario;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->email_usuario;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->departamento;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->cargo;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->telefone;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->ramal;
            $htmlupdate .= '</td>';
            $htmlupdate .= '</tr>';
          }

          $response['response_data']['listaSup'] = $html;
          $response['response_data']['listaSupupdate'] = $htmlupdate;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
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

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $htmlupdate = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX">';
            $html .= '<td><button onclick="selectLocalizacao( ' . $obj->id_localizacao . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
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
            $html .= '<td>';
            $html .= $obj->estado;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->uf;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->cod_municipio;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->municipio;
            $html .= '</td>';
            $html .= '</tr>';



            $htmlupdate .= '<tr class="odd gradeX">';
            $htmlupdate .= '<td><button onclick="selectLocalizacao( ' . $obj->id_localizacao . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
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
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->estado;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->uf;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->cod_municipio;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->municipio;
            $htmlupdate .= '</td>';
            $htmlupdate .= '</tr>';
          }

          $response['response_data']['listaLocalizacao'] = $html;
          $response['response_data']['listaLocalizacaoUpdate'] = $htmlupdate;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }






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
        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $htmlupdate = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX">';
            $html .= '<td><button onclick="selectCNAE( ' . $obj->id_cnae . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
            $html .= '<td>';
            $html .= $obj->id_cnae;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->divisao_cnae;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->descricao_divisão;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->subclasse_cnae;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->descricao_subclasse;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->categoria;
            $html .= '</td>';
            $html .= '</tr>';


            $htmlupdate .= '<tr class="odd gradeX">';
            $htmlupdate .= '<td><button onclick="selectCNAE( ' . $obj->id_cnae . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->id_cnae;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->divisao_cnae;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->descricao_divisão;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->subclasse_cnae;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->descricao_subclasse;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->categoria;
            $htmlupdate .= '</td>';
            $htmlupdate .= '</tr>';
          }

          $response['response_data']['listaCnaes'] = $html;
          $response['response_data']['listaCnaesU'] = $htmlupdate;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        // $sql = "
        // SELECT 
        // 			id_unidade
        // 			,nome_unidade
        // 			,cnpj_unidade
        // 		FROM 
        // 			cliente_unidades;								
        // ";

        // $this->logger->debug($sql);
        // if ($resultsql = mysqli_query($this->db, $sql)) {

        // 	$grupos = '<option value="null"></option>';
        // 	while ($obj = $resultsql->fetch_object()) {
        // 		$grupos .= '<option value="';
        // 		$grupos .= $obj->id_unidade;
        // 		$grupos .= '">';
        // 		$grupos .= $obj->nome_unidade;
        // 		$grupos .= ' | CNPJ: ';
        // 		$grupos .= $obj->cnpj_unidade;
        // 		$grupos .= '</option>';
        // 	}

        // 	$response['response_data']['unidade'] 	= $grupos;
        // } else {
        // 	$this->logger->debug($sql);
        // 	$this->logger->debug($this->db->error);

        // 	$response['response_status']['status']       = 0;
        // 	$response['response_status']['error_code']   = $this->error_code . __LINE__;
        // 	$response['response_status']['msg']          = '';
        // }



        // $sql = "
        // SELECT 
        // 			id_empresa
        // 			,nome_empresa
        // 			,cnpj_empresa
        // 		FROM 
        // 			cliente_matriz;								
        // ";

        // $this->logger->debug($sql);
        // if ($resultsql = mysqli_query($this->db, $sql)) {

        // 	$grupos = '<option value="null"></option>';
        // 	while ($obj = $resultsql->fetch_object()) {
        // 		$grupos .= '<option value="';
        // 		$grupos .= $obj->id_empresa;
        // 		$grupos .= '">';
        // 		$grupos .= $obj->nome_empresa;
        // 		$grupos .= ' | CNPJ: ';
        // 		$grupos .= $obj->cnpj_empresa;
        // 		$grupos .= '</option>';
        // 	}

        // 	$response['response_data']['matriz'] 	= $grupos;
        // } else {
        // 	$this->logger->debug($sql);
        // 	$this->logger->debug($this->db->error);

        // 	$response['response_status']['status']       = 0;
        // 	$response['response_status']['error_code']   = $this->error_code . __LINE__;
        // 	$response['response_status']['msg']          = '';
        // }



        //LISTA CLASSE CNAE (CATEGORIA)
        $sqlCat = 'SELECT * FROM classe_cnae';
        $resultCat = mysqli_query($this->db, $sqlCat);

        $optcat = "<option value=''></option>";
        while ($objCat = $resultCat->fetch_object()) {
          $this->logger->debug($objCat);

          $optcat .= '<option value="' . $objCat->id_cnae . '">';
          $optcat .= $objCat->descricao_subclasse;
          $optcat .= '</option>';
        }

        $response['response_data']['cnaes'] = $optcat;

        //LISTA GRUPO ECONOMICO
        $sql = "SELECT
							id_grupo_economico,
							nome_grupoeconomico
						FROM cliente_grupo
				";

        $result = mysqli_query($this->db, $sql);

        $optGrupo = "<option value=''></option>";
        while ($obj = $result->fetch_object()) {

          $optGrupo .= '<option value="' . $obj->id_grupo_economico . '">';
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

        $resultEmp = mysqli_query($this->db, $sqlEmp);

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

        $resultPatr = mysqli_query($this->db, $sqlPatr);
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

        $resultDate = mysqli_query($this->db, $sqlDate);
        $optDate = "<option value=''></option>";
        while ($objDate = $resultDate->fetch_object()) {
          $optDate .= "<option value='\"{$objDate->database_doc}\"'>{$objDate->database_doc}</option>";
        }

        $response['response_data']['data_base'] = $optDate;

        //LISTA TIPO DOC
        $sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'S' ORDER BY nome_doc ASC";
        $result = mysqli_query($this->db, $sqlTipo);

        $option = "<option value=''></option>";
        while ($obj = $result->fetch_object()) {
          $this->logger->debug($obj);

          $option .= '<option value=\'"' . $obj->nome_doc . '"\'>';
          $option .= $obj->nome_doc;
          $option .= '</option>';
        }

        $response['response_data']['nome_doc'] = $option;

        //LISTA TIPO DOC GERAL
        $sqlTipo = "SELECT idtipo_doc, tipo_doc, nome_doc FROM tipo_doc WHERE processado = 'N' ORDER BY nome_doc ASC";
        $result = mysqli_query($this->db, $sqlTipo);

        $option = "<option value=''></option>";
        $optionTipo = '<option value=""></option>';
        while ($obj = $result->fetch_object()) {
          $this->logger->debug($obj);

          $option .= '<option value=\'"' . $obj->nome_doc . '"\'>';
          $option .= $obj->nome_doc;
          $option .= '</option>';

          $optionTipo .= '<option value=\'"' . $obj->tipo_doc . '"\'>' . $obj->tipo_doc . '</option>';
        }

        $this->logger->debug($option);

        //LISTA TIPO DOC GERAL
        $sqlTipo = "SELECT distinct tipo_doc FROM tipo_doc WHERE processado = 'N' ORDER BY tipo_doc ASC";
        $result = mysqli_query($this->db, $sqlTipo);


        $optionTipo = '';
        while ($obj = $result->fetch_object()) {
          $this->logger->debug($obj);



          $optionTipo .= '<option value=\'"' . $obj->tipo_doc . '"\'>' . $obj->tipo_doc . '</option>';
        }

        $this->logger->debug($option);

        $response['response_data']['lista_tipo_doc'] = $optionTipo;

        //LISTA GRUPO CLÁUSULAS
        $sql = "SELECT 
							* 
						FROM
							grupo_clausula
						ORDER BY nome_grupo ASC
				";

        $resultsql = mysqli_query($this->db, $sql);

        $group = "<option value=''></option>";

        while ($obj = $resultsql->fetch_object()) {

          $group .= '<option value="' . $obj->idgrupo_clausula . '">';
          $group .= $obj->nome_grupo;
          $group .= '</option>';
        }

        $response['response_data']['grupo_clausulas'] = $group;

        //LISTA ASSUNTOS
        $sql = "SELECT 
							* 
						FROM
						estrutura_clausula
						ORDER BY nome_clausula ASC
				";

        $resultsql = mysqli_query($this->db, $sql);

        $group = "<option value=''></option>";

        while ($obj = $resultsql->fetch_object()) {

          $group .= '<option value=\'"' . $obj->nome_clausula . '"\'>';
          $group .= $obj->nome_clausula;
          $group .= '</option>';
        }

        $response['response_data']['assuntos'] = $group;


        //LISTA DE CLÁUSULAS PARA COMPARAÇÃO
        // $sqlCl = "SELECT
        // 			cg.id_clau,
        // 			cg.doc_sind_id_documento,
        // 			gc.nome_grupo,
        // 			est.nome_clausula,
        // 			cg.tex_clau,
        // 			sinde.denominacao_sinde,
        // 			sp.denominacao_sp
        // 		FROM clausula_geral as cg
        // 		LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cg.estrutura_id_estruturaclausula
        // 		LEFT JOIN grupo_clausula as gc on gc.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
        // 		LEFT JOIN doc_sind_sind_emp as de on de.doc_sind_id_doc = cg.doc_sind_id_documento
        // 		LEFT JOIN doc_sind_sind_patr as dp on dp.doc_sind_id_doc = cg.doc_sind_id_documento
        // 		LEFT JOIN sind_emp as sinde on sinde.id_sinde = de.sind_emp_id_sinde
        // 		LEFT JOIN sind_patr as sp on sp.id_sindp = dp.sind_patr_id_sindp
        // ";

        // $resultCl = mysqli_query($this->db, $sqlCl);
        // $clau = "";
        // while ($objCl = $resultCl->fetch_object()) {
        // 	$param = "clau" . $objCl->id_clau;
        // 	$clau .= "<tr class='tbl-item'>";
        // 	$clau .= "<td>
        // 				<a 
        // 					id='{$param}'
        // 					data-nomeCl='{$objCl->nome_clausula}'
        // 					data-grupoCl='{$objCl->nome_grupo}'
        // 					data-laboralCl='{$objCl->denominacao_sinde}'
        // 					data-patronalCl='{$objCl->denominacao_sp}'
        // 					data-textoCl='{$objCl->tex_clau}'
        // 					onclick='selecionaClausula({$objCl->id_clau})' class='btn btn-secondary' data-dismiss='modal' data-toggle='modal' href='#myModal'>Selecionar</a></td>";
        // 	$clau .= "<td class='desc'>{$objCl->nome_grupo}</td>";
        // 	$clau .= "<td class='title'>{$objCl->nome_clausula}</td>";
        // 	$clau .= "<td>{$objCl->denominacao_sinde}</td>";
        // 	$clau .= "<td>{$objCl->denominacao_sp}</td>";
        // 	$clau .= "</tr>";
        // }

        $response['response_data']['lista_clausulas'] = "";

        //LISTA DE ANOS CONFORME DATA BASE DE DOC SIND
        $sqlAno = "SELECT
								DISTINCT database_doc
							FROM doc_sind

				";

        $resultAno = mysqli_query($this->db, $sqlAno);

        $optionAno = "<option value=''></option>";
        $lista = [];
        while ($objAno = $resultAno->fetch_object()) {
          $ano = substr(strstr($objAno->database_doc, "/"), 1);
          (in_array($ano, $lista) ? "" : array_push($lista, $ano));
        }

        $listaFiltrada = array_filter($lista);

        for ($i = 0; $i < count($listaFiltrada); $i++) {
          //	$optionAno .= "<option value='{$listaFiltrada[$i]}'>{$listaFiltrada[$i]}</option>";
        }

        $response['response_data']['lista_ano'] = $optionAno;

        $this->logger->debug($optionAno);

        $sql = "SELECT  distinct
							cc.id_cnae
							,cc.descricao_subclasse
						FROM 
							classe_cnae as cc INNER JOIN
							 cnae_emp as ce ON ce.classe_cnae_idclasse_cnae = cc.id_cnae WHERE
                             ce.cliente_unidades_id_unidade IN (select cu.id_unidade from cliente_unidades  as cu INNER JOIN cliente_matriz as cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							 WHERE cu.cliente_matriz_id_empresa IN (SELECT id_empresa FROM 
							cliente_matriz WHERE cliente_grupo_id_grupo_economico = 100 ));								
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value="null"></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_cnae;
            $grupos .= '">';
            $grupos .= $obj->descricao_subclasse;
            $grupos .= '</option>';
          }

          $response['response_data']['cnaes_carre'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }









        $sql = "
				SELECT 
							cu.id_unidade
							,cu.nome_unidade
							,cm.nome_empresa
						FROM 
							cliente_unidades as cu INNER JOIN cliente_matriz as cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							 WHERE cu.cliente_matriz_id_empresa IN (SELECT id_empresa FROM 
							cliente_matriz WHERE cliente_grupo_id_grupo_economico = 100 );							
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value="null"></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_unidade;
            $grupos .= '">';
            $grupos .= $obj->nome_empresa;
            $grupos .= ' - ';
            $grupos .= $obj->nome_unidade;
            $grupos .= '</option>';
          }

          $response['response_data']['unidade_carre'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }







        $sql = "
				SELECT 
							id_empresa
							,nome_empresa
						FROM 
							cliente_matriz WHERE cliente_grupo_id_grupo_economico = 100;								
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value="null"></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_empresa;
            $grupos .= '">';
            $grupos .= $obj->nome_empresa;
            $grupos .= '</option>';
          }

          $response['response_data']['matriz_carre'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }










        $sql = "
				select idtipo_doc, nome_doc, tipo_doc from tipo_doc WHERE processado = 'S' ORDER BY nome_doc ASC;
							
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value="null"></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->idtipo_doc;
            $grupos .= '">';
            $grupos .= $obj->nome_doc;
            $grupos .= '</option>';
          }

          $response['response_data']['docs'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }





        $sql = "
				SELECT 
							cu.id_unidade
							,cu.nome_unidade
							,cm.nome_empresa
							,cg.nome_grupoeconomico
						FROM 
							cliente_unidades cu INNER JOIN cliente_matriz cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							INNER JOIN cliente_grupo cg WHERE cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
							ORDER BY cg.nome_grupoeconomico;								
				";
        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $htmlupdate = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX">';
            $html .= '<td><button onclick="selectUMGE( ' . $obj->id_unidade . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
            $html .= '<td>';
            $html .= $obj->nome_unidade;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->nome_empresa;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->nome_grupoeconomico;
            $html .= '</td>';
            $html .= '</tr>';


            $htmlupdate .= '<tr class="odd gradeX">';
            $htmlupdate .= '<td><button onclick="selectUMGE( ' . $obj->id_unidade . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->nome_unidade;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->nome_empresa;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->nome_grupoeconomico;
            $htmlupdate .= '</td>';
            $htmlupdate .= '</tr>';
          }

          $response['response_data']['listaUMGE'] = $html;
          $response['response_data']['listaUMGEupdate'] = $htmlupdate;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }




        $sql = "
				SELECT 
							id_modulos
							,modulos
						FROM 
							modulos WHERE tipo = 'SISAP';								
				";
        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $htmlupdate = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX">';
            $html .= '<td>';
            $html .= $obj->modulos;
            $html .= '</td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Criar\');" value="1" id="checkCriar' . $obj->id_modulos . '"></td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="checkConsultar' . $obj->id_modulos . '"></td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="checkComentar' . $obj->id_modulos . '"></td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="checkAlterar' . $obj->id_modulos . '"></td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="checkExcluir' . $obj->id_modulos . '"></td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModSISAP(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="checkAprovar' . $obj->id_modulos . '"></td>';
            $html .= '</tr>';


            $htmlupdate .= '<tr class="odd gradeX">';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->modulos;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Criar\');" value="1" id="updatecheckCriar' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="updatecheckConsultar' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="updatecheckComentar' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="updatecheckAlterar' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="updatecheckExcluir' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModSISAP(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="updatecheckAprovar' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '</tr>';
          }

          $response['response_data']['listaSISAP'] = $html;
          $response['response_data']['listaSISAPupdate'] = $htmlupdate;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }





        $sql = "
				SELECT 
							id_modulos
							,modulos
						FROM 
							modulos WHERE tipo = 'Comercial';								
				";
        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $htmlupdate = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX">';
            $html .= '<td>';
            $html .= $obj->modulos;
            $html .= '</td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Criar\');" value="1" id="comcheckCriar' . $obj->id_modulos . '"></td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="comcheckConsultar' . $obj->id_modulos . '"></td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="comcheckComentar' . $obj->id_modulos . '"></td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="comcheckAlterar' . $obj->id_modulos . '"></td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="comcheckExcluir' . $obj->id_modulos . '"></td>';
            $html .= '<td><input class="form-check-input" type="checkbox" onclick="addModComercial(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="comcheckAprovar' . $obj->id_modulos . '"></td>';
            $html .= '</tr>';


            $htmlupdate .= '<tr class="odd gradeX">';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->modulos;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Criar\');" value="1" id="comupdatecheckCriar' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Consultar\');" value="1" id="comupdatecheckConsultar' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Comentar\');" value="1" id="comupdatecheckComentar' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Alterar\');" value="1" id="comupdatecheckAlterar' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Excluir\');" value="1" id="comupdatecheckExcluir' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '<td><input class="form-check-input" type="checkbox" onclick="updateModComercial(' . $obj->id_modulos . ', \'Aprovar\');" value="1" id="comupdatecheckAprovar' . $obj->id_modulos . '"></td>';
            $htmlupdate .= '</tr>';
          }

          $response['response_data']['listaComercial'] = $html;
          $response['response_data']['listaComercialupdate'] = $htmlupdate;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }








        $sql = "
				SELECT 
				id_user as id_usuario
				,nome_usuario
				FROM 
					usuario_adm;				
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value="null"></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_usuario;
            $grupos .= '">';
            $grupos .= $obj->nome_usuario;
            $grupos .= '</option>';
          }

          $response['response_data']['superiores'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


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

        $sqlMuni = "SELECT
							distinct municipio
							
						FROM
							localizacao
						ORDER BY municipio ASC
				";
        $resultUf = mysqli_query($this->db, $sqlUf);
        $resultReg = mysqli_query($this->db, $sqlRegiao);
        $resultMun = mysqli_query($this->db, $sqlMuni);

        $opt = "<option value=''></option>";

        while ($obj = $resultReg->fetch_object()) {
          $opt .= "<option value='regiao+" . $obj->regiao . "'>" . $obj->regiao . "</option>";
        }

        while ($obj = $resultUf->fetch_object()) {
          $opt .= "<option value='uf+" . $obj->uf . "'>" . $obj->uf . "</option>";
        }

        while ($obj = $resultMun->fetch_object()) {
          $opt .= "<option value='municipio+" . $obj->municipio . "'>" . $obj->municipio . "</option>";
        }

        $response['response_data']['localizacao'] = $opt;



        $sql = "
				SELECT 
							id_localizacao
						    ,municipio
						FROM 
							localizacao WHERE id_localizacao IN
                            (SELECT localizacao_id_localizacao from cliente_unidades WHERE cliente_matriz_id_empresa IN (SELECT id_empresa FROM 
							cliente_matriz WHERE cliente_grupo_id_grupo_economico = 100));								
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value="null"></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_localizacao;
            $grupos .= '">';
            $grupos .= $obj->municipio;
            $grupos .= '</option>';
          }

          $response['response_data']['localizacao_carre'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }








        $sql = "
				SELECT 
				id_jornada
				,descricao
				FROM 
					jornada;				
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value="null"></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_jornada;
            $grupos .= '">';
            $grupos .= $obj->descricao;
            $grupos .= '</option>';
          }

          $response['response_data']['jornada'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
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



  // function getMatriz($data = null)
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


  // 			$sql = "SELECT
  // 						id_empresa,
  // 						razao_social,
  // 						cnpj_empresa,
  // 						codigo_empresa
  // 					FROM cliente_matriz
  // 					WHERE cliente_grupo_id_grupo_economico = '{$data['id_grupo']}'
  // 			";

  // 			$result = mysqli_query($this->db, $sql);

  // 			$opt = "<option value=''></option>";
  // 			while ($obj = $result->fetch_object()) {
  // 				$cnpj = formatCnpjCpf($obj->cnpj_empresa);
  // 				$this->logger->debug(formatCnpjCpf($obj->cnpj_empresa));
  // 				$opt .= "<option value='{$obj->id_empresa}'>{$obj->codigo_empresa} / {$cnpj} / {$obj->razao_social}</option>";
  // 			}

  // 			$response['response_data']['lista_matriz'] = $opt;
  // 		} else {
  // 			$response = $this->response;
  // 		}
  // 	} else {
  // 		$response = $this->response;
  // 	}

  // 	$this->logger->debug($response['response_status']['status']);
  // 	return $response;
  // }

  // function getUnidade($data = null)
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

  // 			$matriz = $data['id_matriz'];

  // 			$opt = "<option value=''></option>";
  // 			for ($i = 0; $i < count($matriz); $i++) {
  // 				$sql = "SELECT
  // 						id_unidade,
  // 						nome_unidade,
  // 						cnpj_unidade,
  // 						codigo_unidade,
  // 						cod_sindcliente,
  // 						regional
  // 					FROM cliente_unidades
  // 					WHERE cliente_matriz_id_empresa = '{$matriz[$i]}'
  // 				";

  // 				$this->logger->debug($sql);

  // 				$result = mysqli_query($this->db, $sql);


  // 				while ($obj = $result->fetch_object()) {
  // 					$cnpj = formatCnpjCpf($obj->cnpj_unidade);
  // 					$opt .= "<option value='{$obj->id_unidade}'>Cód: {$obj->codigo_unidade} / CNPJ: {$cnpj} / Nome: {$obj->nome_unidade} / Cod. Sind. Cliente: {$obj->cod_sindcliente} / Regional: {$obj->regional}</option>";
  // 				}
  // 			}

  // 			$this->logger->debug($opt);

  // 			$response['response_data']['lista_unidade'] = $opt;
  // 		} else {
  // 			$response = $this->response;
  // 		}
  // 	} else {
  // 		$response = $this->response;
  // 	}

  // 	$this->logger->debug($response['response_status']['status']);
  // 	return $response;
  // }

  function getNomeDoc($data = null)
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

      if ($response['response_status']['status'] == 1) {

        if ($data['doc'] == "geral") {
          $sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'N'  ORDER BY nome_doc ASC";
        } else {
          $sqlTipo = "SELECT idtipo_doc, nome_doc FROM tipo_doc WHERE processado = 'S' ORDER BY nome_doc ASC";
        }

        $result = mysqli_query($this->db, $sqlTipo);

        $option = "<option value=''></option>";
        while ($obj = $result->fetch_object()) {
          $this->logger->debug($obj);

          $option .= '<option value=\'"' . $obj->nome_doc . '"\'>';
          $option .= $obj->nome_doc;
          $option .= '</option>';
        }

        $response['response_data']['nome_doc'] = $option;
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    $this->logger->debug($response['response_status']['status']);
    return $response;
  }

  function saveForm($data = null)
  {
    $this->logger->debug('entrou na classe php');
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

        $sql = "INSERT INTO formulario_grupo (cliente_grupo_id_grupo_economico, doc_sind_id_doc, formulario_comunicado)
				VALUES ({$data['gec']}, {$data['id_doc']}, '{$data['form']}')
				ON DUPLICATE KEY UPDATE formulario_comunicado = '{$data['form']}'
									
						";


        $this->logger->debug($sql);
        if (!mysqli_query($this->db, $sql)) {

          mysqli_query($this->db, '');

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';

          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);
          $this->logger->debug($response);
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
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

  function getByIdForm($data = null)
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

      if ($response['response_status']['status'] == 1) {

        $this->logger->debug($data);

        $sql = "SELECT 
                            doc.id_doc,
                            cl.clausula_geral_id_clau as clau,
                            ad.nmtipoinformacaoadicional as nome_info,
                            est.nome_clausula,
                            gp.nome_grupo,
                            case 
                                when cl.texto != '' then cl.texto
                                when cl.numerico != 0 then cl.numerico
                                when cl.descricao != '' then cl.descricao
                                when cl.data then date_format(cl.data, '%d/%m/%Y')
                                when cl.percentual != 0 then concat(format(cl.percentual, 2), '%')
                                when cl.hora != '' then cl.hora
                                when cl.combo != '' then cl.combo else null end as value
                        FROM clausula_geral_estrutura_clausula AS cl
                        LEFT JOIN ad_tipoinformacaoadicional AS ad ON ad.cdtipoinformacaoadicional = cl.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
                        LEFT JOIN doc_sind AS doc ON doc.id_doc = cl.doc_sind_id_doc
                        LEFT JOIN estrutura_clausula AS est ON est.id_estruturaclausula = cl.estrutura_clausula_id_estruturaclausula
                        LEFT JOIN grupo_clausula AS gp ON gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
                        
                        WHERE doc.id_doc = {$data['id_doc']}
                        GROUP BY ad.nmtipoinformacaoadicional
                ";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {



          $array = [];
          $infos = [];
          $clausula = 0;
          $new = new stdClass();
          $cont = 0;
          while ($obj = $resultsql->fetch_object()) {
            $this->logger->debug($obj);

            inicio:
            if ($clausula == $obj->clau) {
              if (in_array($obj->nome_info, $infos)) {
                $campo = $obj->nome_info . "_" . $cont;
                $new->$campo = $obj->value == null ? "" : $obj->value;
                $new->nome_clausula = $obj->nome_clausula;
                $new->nome_grupo = $obj->nome_grupo;
                $this->logger->debug($new);
                array_push($infos, $obj->nome_info);
              } else {
                $campo = $obj->nome_info;
                $new->$campo = $obj->value == null ? "" : $obj->value;
                $new->nome_clausula = $obj->nome_clausula;
                $new->nome_grupo = $obj->nome_grupo;
                $this->logger->debug($new);
              }
            } else {
              $clausula = $obj->clau;
              array_push($array, $new);
              $new = new stdClass();
              goto inicio;
            }


            $cont++;
          }

          array_push($array, $new);


          array_shift($array);
          $this->logger->debug($array);
          $response['response_data']['lista_info'] = array_filter($array);
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }





        $sql = "SELECT 
                            doc.doc_sind_id_doc,
                            doc.formulario_comunicado
                            
                        FROM  formulario_grupo AS doc 
                        WHERE doc.doc_sind_id_doc = {$data['id_doc']} AND doc.cliente_grupo_id_grupo_economico = {$data['gec']};
                ";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {
          $json_array = "";

          while ($obj = $resultsql->fetch_object()) {
            $this->logger->debug($obj);

            $json_array = $obj->formulario_comunicado;

          }
          $this->logger->debug($json_array);
          $response['response_data']['form_db'] = json_decode($json_array, true);
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }









        $sql = "SELECT 
							doc.id_doc,
                            doc.database_doc,
                            DATE_FORMAT(doc.data_aprovacao,'%d/%m/%Y') as data_aprovacao,
							REPLACE(REPLACE(GROUP_CONCAT(JSON_UNQUOTE(JSON_EXTRACT(doc.cnae_doc, '$[*].subclasse')) SEPARATOR ';'), '[', ''), ']', '') AS atividades_ec,
							group_concat(concat(se.sigla_sinde, ' - ', se.denominacao_sinde)) as sind_laboral,
							group_concat(se.cnpj_sinde) as cnpj_sinde,
							group_concat(concat(sp.sigla_sp, ' - ', sp.denominacao_sp)) as sind_patronal,
							group_concat(sp.cnpj_sp) as cnpj_sp,
							DATE_FORMAT(doc.validade_inicial,'%d/%m/%Y') as validade_inicial,
							DATE_FORMAT(doc.validade_final,'%d/%m/%Y') as validade_final,
							YEAR(doc.validade_inicial) as ano_inicial,
							YEAR(doc.validade_final) as ano_final,
							td.nome_doc
							
						FROM  doc_sind AS doc
						LEFT JOIN tipo_doc as td ON td.idtipo_doc = doc.tipo_doc_idtipo_doc
						LEFT JOIN sind_emp as se  ON JSON_CONTAINS(doc.sind_laboral,CONCAT('{\"id\":',se.id_sinde,'}'), '$')
						LEFT JOIN sind_patr as sp  ON JSON_CONTAINS(doc.sind_patronal,CONCAT('{\"id\":',sp.id_sindp,'}'), '$')
						
					WHERE doc.id_doc = {$data['id_doc']}
					
					GROUP BY doc.id_doc";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          while ($obj = $resultsql->fetch_object()) {
            $this->logger->debug($obj);
            $labs = explode(",", $obj->sind_laboral);
            $labs_cnpj = explode(",", $obj->cnpj_sinde);
            $patrs = explode(",", $obj->sind_patronal);
            $patrs_cnpj = explode(",", $obj->cnpj_sp);

            $mergedArray = [];

            // Merge the arrays
            for ($i = 0; $i < count($labs); $i++) {
              $mergedArray[] = $labs[$i] . " - " . formatCnpjCpf($labs_cnpj[$i]);
            }

            // Convert the merged array to a string
            $result_lab = implode(",", $mergedArray);

            $mergedArrayPatr = [];

            // Merge the arrays
            for ($i = 0; $i < count($labs); $i++) {
              $mergedArrayPatr[] = $patrs[$i] . " - " . formatCnpjCpf($patrs_cnpj[$i]);
            }

            // Convert the merged array to a string
            $result_patr = implode(",", $mergedArrayPatr);


            $response['response_data']['sind_laboral'] = $result_lab;
            $response['response_data']['sind_patronal'] = $result_patr;
            $response['response_data']['nome_doc'] = $obj->nome_doc;
            $response['response_data']['ano_inicial'] = $obj->ano_inicial;
            $response['response_data']['ano_final'] = $obj->ano_final;
            $response['response_data']['validade_inicial'] = $obj->validade_inicial;
            $response['response_data']['validade_final'] = $obj->validade_final;
            $response['response_data']['atividades_ec'] = $obj->atividades_ec;
            $response['response_data']['database_doc'] = $obj->database_doc;
            $response['response_data']['data_aprovacao'] = $obj->data_aprovacao;
          }
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    $this->logger->debug($response['response_status']['status']);
    $this->logger->debug($response['response_data']);

    return $response;
  }
}
