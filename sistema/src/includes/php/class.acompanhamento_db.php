<?php

/**
 * @author    {Rafael P. Cruz}
 * @package   {1.0.0}
 * @description	{ }
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


// inclui as classes do PHPMailer
// require(__DIR__ . '/PHPMailer.php');
// require(__DIR__ . '/SMTP.php');

include_once __DIR__ . "/class.disparoEmail.php";

include __DIR__ . "/helpers.php";


date_default_timezone_set('America/Sao_Paulo');

class acompanhamento_db
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



  function getMonthYear($monthInitials)
  {
    $months = array(
      "JAN" => 0,
      "FEV" => 1,
      "MAR" => 2,
      "ABR" => 3,
      "MAI" => 4,
      "JUN" => 5,
      "JUL" => 6,
      "AGO" => 7,
      "SET" => 8,
      "OUT" => 9,
      "NOV" => 10,
      "DEZ" => 11
    );

    $now = new DateTime();
    $currentYear = $now->format('Y');
    $currentMonth = $now->format('n') - 1; // Adjust to be 0-based
    $targetMonth = $months[$monthInitials];

    $year = $currentYear;
    $month = $targetMonth;

    if ($currentMonth >= $targetMonth) {
      $year += 1;
    }

    $month += 1; // Adjust month to be 1-based (January is 1, not 0)

    return sprintf('%02d/%d', $month, $year);
  }








  function getAcompanhamentoCampos($data = null)
  {

    function getMonthYear($monthInitials)
    {
      $months = array(
        "JAN" => 0,
        "FEV" => 1,
        "MAR" => 2,
        "ABR" => 3,
        "MAI" => 4,
        "JUN" => 5,
        "JUL" => 6,
        "AGO" => 7,
        "SET" => 8,
        "OUT" => 9,
        "NOV" => 10,
        "DEZ" => 11
      );

      $now = new DateTime();
      $currentYear = $now->format('Y');
      $currentMonth = $now->format('n') - 1; // Adjust to be 0-based
      $targetMonth = $months[$monthInitials];

      $year = $currentYear;
      $month = $targetMonth;

      if ($currentMonth >= $targetMonth) {
        $year += 1;
      }

      $month += 1; // Adjust month to be 1-based (January is 1, not 0)

      return sprintf('%02d/%d', $month, $year);
    }


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
							id_sinde
							,sigla_sinde
							,cnpj_sinde
                            ,logradouro_sinde
                            ,email1_sinde
                            ,fone1_sinde
                            ,site_sinde
						FROM 
							sind_emp;			";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $htmlupdate = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td><input type="checkbox"onclick="selectSindicato( ' . $obj->id_sinde . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal"/></td>';
            $html .= '<td class="title">';
            $html .= $obj->sigla_sinde;
            $html .= '</td>';
            $html .= '<td class="cnpj_format">';
            $html .= formatCnpjCpf($obj->cnpj_sinde);
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->logradouro_sinde;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->email1_sinde;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->fone1_sinde;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->site_sinde;
            $html .= '</td>';
            $html .= '</tr>';


            $htmlupdate .= '<tr class="odd gradeX tbl-item">';
            $htmlupdate .= '<td><input type="checkbox"onclick="selectSindicato( ' . $obj->id_sinde . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal"/></td>';
            $htmlupdate .= '<td class="title">';
            $htmlupdate .= $obj->sigla_sinde;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td class="cnpj_format">';
            $htmlupdate .= formatCnpjCpf($obj->cnpj_sinde);
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->logradouro_sinde;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->email1_sinde;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->fone1_sinde;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->site_sinde;
            $htmlupdate .= '</td>';
            $htmlupdate .= '</tr>';
          }

          $response['response_data']['listaSindicato'] = $html;
          $response['response_data']['listaSindicatoU'] = $htmlupdate;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }








        $sql = "
				SELECT 
							id_tiponegocio
							,tipo_negocio
						FROM 
							tipounidade_cliente;							
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $htmlupdate = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX">';
            $html .= '<td><input type="checkbox"onclick="selectNegocio( ' . $obj->id_tiponegocio . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal"/></td>';
            $html .= '<td>';
            $html .= $obj->tipo_negocio;
            $html .= '</td>';
            $html .= '</tr>';



            $htmlupdate .= '<tr class="odd gradeX">';
            $htmlupdate .= '<td><input type="checkbox"onclick="selectNegocio( ' . $obj->id_tiponegocio . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal"/></td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->tipo_negocio;
            $htmlupdate .= '</td>';
            $htmlupdate .= '</tr>';
          }

          $response['response_data']['listaNegocio'] = $html;
          $response['response_data']['listaNegocioU'] = $htmlupdate;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }






        $sql = "
				SELECT 
							id_sindp
							,sigla_sp
							,cnpj_sp
                            ,logradouro_sp
                            ,email1_sp
                            ,fone1_sp
                            ,site_sp
						FROM 
							sind_patr;									
	";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $htmlupdate = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td><input type="checkbox"onclick="selectSindicatoP( ' . $obj->id_sindp . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal"/></td>';
            $html .= '<td class="title">';
            $html .= $obj->sigla_sp;
            $html .= '</td>';
            $html .= '<td class="cnpj_format">';
            $html .= formatCnpjCpf($obj->cnpj_sp);
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->logradouro_sp;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->email1_sp;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->fone1_sp;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->site_sp;
            $html .= '</td>';
            $html .= '</tr>';


            $htmlupdate .= '<tr class="odd gradeX tbl-item">';
            $htmlupdate .= '<td><input type="checkbox"onclick="selectSindicatoP( ' . $obj->id_sindp . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal"/></td>';
            $htmlupdate .= '<td class="title">';
            $htmlupdate .= $obj->sigla_sp;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td class="cnpj_format">';
            $htmlupdate .= formatCnpjCpf($obj->cnpj_sp);
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->logradouro_sp;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->email1_sp;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->fone1_sp;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->site_sp;
            $htmlupdate .= '</td>';
            $htmlupdate .= '</tr>';
          }

          $response['response_data']['listaEmpresa'] = $html;
          $response['response_data']['listaEmpresaU'] = $htmlupdate;
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
            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td><input type="checkbox"onclick="selectResp( ' . $obj->id_usuario . ');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal"/></td>';
            $html .= '<td class="title">';
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

            $htmlupdate .= '<tr class="odd gradeX tbl-item">';
            $htmlupdate .= '<td><input type="checkbox"onclick="selectResp( ' . $obj->id_usuario . ');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal"/></td>';
            $htmlupdate .= '<td class="title">';
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

          $response['response_data']['listaResponsaveis'] = $html;
          $response['response_data']['listaResponsaveisU'] = $htmlupdate;
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
          $lista = [];
          //perder
          while ($obj = $resultsql->fetch_object()) {
            $input = '<input type="checkbox"onclick="selectCNAEMultiU( ' . $obj->id_cnae . ');" class="btn btn-secondary" id="checu' . $obj->id_cnae . '"/>';
            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td><input type="checkbox"onclick="selectCNAEMulti( ' . $obj->id_cnae . ');" class="btn btn-secondary" id="chec' . $obj->id_cnae . '"/></td>';

            $html .= '<td  class="title">';
            $html .= $obj->subclasse_cnae;
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= $obj->descricao_subclasse;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->categoria;
            $html .= '</td>';
            $html .= '</tr>';


            $htmlupdate .= '<tr class="odd gradeX tbl-item">';
            $htmlupdate .= '<td><input type="checkbox"onclick="selectCNAEMultiU( ' . $obj->id_cnae . ');" class="btn btn-secondary" id="checu' . $obj->id_cnae . '"/></td>';

            $htmlupdate .= '<td class="title">';
            $htmlupdate .= $obj->subclasse_cnae;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td class="desc">';
            $htmlupdate .= $obj->descricao_subclasse;
            $htmlupdate .= '</td>';
            $htmlupdate .= '<td>';
            $htmlupdate .= $obj->categoria;
            $htmlupdate .= '</td>';
            $htmlupdate .= '</tr>';

            $newObj = new stdClass();
            $newObj->subclasse = $input;
            $newObj->id_cnae = $obj->id_cnae;
            $newObj->subclasse_cnae = $obj->subclasse_cnae;
            $newObj->descricao_subclasse = $obj->descricao_subclasse;
            $newObj->categoria = $obj->categoria;

            array_push($lista, $newObj);
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










        $sql = "
				select distinct se.sigla_sinde, sp.sigla_sp, emp.sind_empregados_id_sinde1, patr.sind_patronal_id_sindp,emp.dataneg,
				emp.classe_cnae_idclasse_cnae, cc.descricao_subclasse, ifnull(usu.nome_usuario, '-') as responsavel,ifnull(usu.id_user, 0) as responsavel_id,
                date_format((SELECT DATE_FORMAT(DATE_SUB(STR_TO_DATE(Concat('01/',IF(emp.dataneg = 'JAN', 'Jan',
       IF(emp.dataneg = 'FEV', 'Feb',
       IF(emp.dataneg = 'MAR', 'Mar',
       IF(emp.dataneg = 'ABR', 'Apr',
       IF(emp.dataneg = 'MAI', 'May',
       IF(emp.dataneg = 'JUN', 'Jun',
       IF(emp.dataneg = 'JUL', 'Jul',
       IF(emp.dataneg = 'AGO', 'Aug',
       IF(emp.dataneg = 'SET', 'Sep',
       IF(emp.dataneg = 'OUT', 'Oct',
       IF(emp.dataneg = 'NOV', 'Nov',
       IF(emp.dataneg = 'DEZ', 'Dec', NULL)))))))))))),'/',YEAR(NOW())), '%d/%b/%Y'), INTERVAL 0 MONTH), '%Y-%m-01') AS first_day_of_prev_month), '%d/%m/%Y') as data_ini
				from base_territorialsindemp as emp INNER JOIN base_territorialsindpatro as patr 
				ON (emp.localizacao_id_localizacao1 = patr.localizacao_id_localizacao1 AND emp.classe_cnae_idclasse_cnae = patr.classe_cnae_idclasse_cnae)
				INNER JOIN sind_emp as se ON se.id_sinde = emp.sind_empregados_id_sinde1
				INNER JOIN sind_patr as sp ON sp.id_sindp = patr.sind_patronal_id_sindp
				INNER JOIN classe_cnae as cc ON cc.id_cnae = emp.classe_cnae_idclasse_cnae
                INNER JOIN localizacao as loc ON loc.id_localizacao = emp.localizacao_id_localizacao1
                LEFT JOIN usuario_adm as usu ON (JSON_CONTAINS(
			usu.ids_cnae,
			CONCAT('\"',cc.divisao_cnae, '\"'),'$') AND JSON_CONTAINS(
			usu.ids_localidade,
			CONCAT('\"',loc.cod_uf, '\"'),'$'))
				WHERE emp.dataneg like \"%%\"								
				ORDER BY data_ini";
        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td><input type="checkbox"
						onclick="selectFutMulti( \'' . $obj->sind_empregados_id_sinde1 . '-' . $obj->sind_patronal_id_sindp . '-' . $obj->dataneg . '-' . $obj->classe_cnae_idclasse_cnae . '-' . $obj->responsavel_id . '\');"
						 class="btn btn-secondary" id="chep' . $obj->sind_empregados_id_sinde1 . '-' . $obj->sind_patronal_id_sindp . '-' . $obj->dataneg . '-' . $obj->classe_cnae_idclasse_cnae . '-' . $obj->responsavel_id . '"/></td>';
            $html .= '<td class="title">';
            $html .= $obj->sigla_sinde;
            $html .= '</td>';
            $html .= '<td class="title1">';
            $html .= $obj->sigla_sp;
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= $obj->descricao_subclasse;
            $html .= '</td>';
            $html .= '<td class="desc1">';
            $html .= getMonthYear($obj->dataneg);
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->data_ini;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->responsavel;
            $html .= '</td>';
            $html .= '</tr>';
          }

          $response['response_data']['listaFuturas'] = $html;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }













        $sql = "
				SELECT 
							id_sinde
						    ,sigla_sinde
							,cnpj_sinde
							,denominacao_sinde
						FROM 
							sind_emp;									
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value=""></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_sinde;
            $grupos .= '">';
            $grupos .= $obj->sigla_sinde . " | CNPJ: " . formatCnpjCpf($obj->cnpj_sinde) . " | Denominação: " . $obj->denominacao_sinde;
            $grupos .= '</option>';
          }

          $response['response_data']['sindicatos'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }



        //CONTADORES




        $sql = "
				select (SELECT COUNT(*)
				FROM acompanhanto_cct
				WHERE fase not like \"%Fechada%\" AND fase not like \"%Concluída%\") as abertas,
				(SELECT COUNT(*)
				FROM acompanhanto_cct
				WHERE ultima_atualizacao not BETWEEN DATE_SUB(CURDATE(), INTERVAL 30 DAY) AND CURDATE()) as semmov, 
				(SELECT COUNT(*)
				FROM acompanhanto_cct
				WHERE proxima_ligacao like CURDATE()) as ligacoes;								
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $abertas = '';
          $semmov = '';
          $ligacoes = '';
          while ($obj = $resultsql->fetch_object()) {
            $semmov .= $obj->semmov;
            $abertas .= $obj->abertas;
            $ligacoes .= $obj->ligacoes;
          }

          $response['response_data']['cabertas'] = $abertas;
          $response['response_data']['csemmov'] = $semmov;
          $response['response_data']['ligacoes'] = $ligacoes;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }





        $sql = "
				select idtipo_doc, nome_doc from tipo_doc WHERE nome_doc like '%convenção coletiva%' and nome_doc not like '%especifica%';
									
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value=""></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->idtipo_doc;
            $grupos .= '">';
            $grupos .= $obj->nome_doc;
            $grupos .= '</option>';
          }

          $response['response_data']['tipo_doc'] = $grupos;
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
							,descricao_subclasse
						FROM 
							classe_cnae;								
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value=""></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_cnae;
            $grupos .= '">';
            $grupos .= $obj->descricao_subclasse;
            $grupos .= '</option>';
          }

          $response['response_data']['cnaes'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        $sql = "
				select fase_negociacao from fase_cct where tipo_fase like 'cct%';
								
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value=""></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->fase_negociacao;
            $grupos .= '">';
            $grupos .= $obj->fase_negociacao;
            $grupos .= '</option>';
          }

          $response['response_data']['fasesCct'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        $sql = "
				SELECT 
							id_sindp
						    ,sigla_sp
							,cnpj_sp
							,denominacao_sp
						FROM 
							sind_patr;									
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value=""></option>';
          $html = null;
          $htmlupdate = null;
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_sindp;
            $grupos .= '">';
            $grupos .= $obj->sigla_sp . " | CNPJ: " . formatCnpjCpf($obj->cnpj_sp) . " | Denominação: " . $obj->denominacao_sp;
            $grupos .= '</option>';
          }

          $response['response_data']['empresas'] = $grupos;

          $response['response_data']['listaCentralSindical'] = $html;

          $response['response_data']['listaCentralSindicalUpdate'] = $htmlupdate;
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

          $grupos = '<option value=""></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_usuario;
            $grupos .= '">';
            $grupos .= $obj->nome_usuario;
            $grupos .= '</option>';
          }

          $response['response_data']['responsaveis'] = $grupos;
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












  function getAcompanhamento($data = null)
  {

    function getMonthYear2($monthInitials)
    {
      $months = array(
        "JAN" => 0,
        "FEV" => 1,
        "MAR" => 2,
        "ABR" => 3,
        "MAI" => 4,
        "JUN" => 5,
        "JUL" => 6,
        "AGO" => 7,
        "SET" => 8,
        "OUT" => 9,
        "NOV" => 10,
        "DEZ" => 11
      );

      $now = new DateTime();
      $currentYear = $now->format('Y');
      $currentMonth = $now->format('n') - 1; // Adjust to be 0-based
      $targetMonth = $months[$monthInitials];

      $year = $currentYear;
      $month = $targetMonth;

      if ($currentMonth >= $targetMonth) {
        $year += 1;
      }

      $month += 1; // Adjust month to be 1-based (January is 1, not 0)

      return sprintf('%02d/%d', $month, $year);
    }
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

      $abertas = 0;

      $this->logger->debug($connectdb);
      //$this->logger->debug(phpinfo());

      if ($response['response_status']['status'] == 1) {





        $sql = "
				select ac.idacompanhanto_cct
				, date_format(ac.data_inicial, '%d/%m/%Y') as data_inicial
				, date_format(ac.data_final, '%d/%m/%Y') as data_final
				, date_format(ac.ultima_atualizacao, '%d/%m/%Y') as ultima_atualizacao
				, ac.status
				, ac.proxima_ligacao
				, ua.nome_usuario
				, ac.fase
				, ac.data_base
				, sp.sigla_sp
				, se.sigla_sinde
				, cc.descricao_subclasse from acompanhanto_cct as ac
				left join usuario_adm as ua on ua.id_user = ac.usuario_adm_id_usuario 
				left join sind_patr as sp on sp.id_sindp = ac.sind_patr_id_sindp
				left join sind_emp as se on se.id_sinde = ac.sind_emp_id_sinde
				left join classe_cnae as cc on cc.id_cnae = ac.ids_cnaes
				ORDER BY ac.proxima_ligacao, ac.status;							
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {


          $html_fases = array(
            "Assembleia Patronal" => null,
            "Paralisada" => null,
            "Dissídio Coletivo" => null,
            "Prorrogação CCT" => null,
            "Fechada Parcialmente" => null,
            "Fechada" => null
          );

          $this->logger->debug($html_fases);

          while ($obj = $resultsql->fetch_object()) {
            $html = null;


            $html .= '<div class="card col-md-12" draggable="true">
						<h2>#';
            $html .= $obj->idacompanhanto_cct;
            $html .= '</h2>
						<hr />
						<p>Status: <b>';
            $html .= substr($obj->status, 2);
            $html .= '</b>
						</p>
						<p>Partes envolvidas: ';
            $html .= $obj->sigla_sp;
            $html .= ', ';
            $html .= $obj->sigla_sinde;
            $html .= '</p>
						<p>CNAE: ';
            $html .= $obj->descricao_subclasse;
            $html .= '</p>
						<p>Ínicio: ';
            $html .= $obj->data_inicial;
            $html .= '</p>
						<p>Fim previsto: ';
            $html .= $obj->data_final;
            $html .= '</p>
						<p>Ultima atualização: ';
            $html .= $obj->ultima_atualizacao;
            $html .= '</p>
						<p>Data Base: ';
            $html .= $obj->data_base;
            $html .= '</p>
						<p>Responsável: <b>';
            $html .= $obj->nome_usuario;
            $html .= '</b></p>
						<button data-toggle="modal" href="#updateModal" onclick="getByIdAcompanhamento( ' . $obj->idacompanhanto_cct . ');">Editar <i class="fa fa-file-text"></i></button>
						</div>';
            $html_fases[$obj->fase] .= $html;
          }

          $this->logger->debug($html_fases);
          $response['response_data']['Assembleia Patronal'] = $html_fases['Assembleia Patronal'];

          $response['response_data']['Paralisada'] = $html_fases['Paralisada'];

          $response['response_data']['Dissídio Coletivo'] = $html_fases['Dissídio Coletivo'];
          $response['response_data']['Prorrogação CCT'] = $html_fases['Prorrogação CCT'];
          $response['response_data']['Fechada Parcialmente'] = $html_fases['Fechada Parcialmente'];

          $response['response_data']['Fechada'] = $html_fases['Fechada'];

          $this->logger->debug($response);
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        $sql = "
				select ac.idacompanhanto_cct
				, date_format(ac.data_inicial, '%d/%m/%Y') as data_inicial
				, date_format(ac.data_final, '%d/%m/%Y') as data_final
				, date_format(ac.ultima_atualizacao, '%d/%m/%Y') as ultima_atualizacao
				, ac.status
				, ua.nome_usuario
				, ac.fase
				, td.nome_doc
				, date_format(ac.proxima_ligacao, '%d/%m/%Y') as proxima_ligacao
				, ac.data_base
				, sp.sigla_sp
				, sp.uf_sp
				, se.sigla_sinde
				, se.uf_sinde
				, cc.descricao_subclasse from acompanhanto_cct as ac
				left join usuario_adm as ua on ua.id_user = ac.usuario_adm_id_usuario
				left join tipo_doc as td on td.idtipo_doc = ac.tipo_doc_idtipo_doc
				left join sind_patr as sp on sp.id_sindp = ac.sind_patr_id_sindp
				left join sind_emp as se on se.id_sinde = ac.sind_emp_id_sinde
				left join classe_cnae as cc on cc.id_cnae = ac.ids_cnaes
				ORDER BY proxima_ligacao DESC, ac.status;				
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = "";
          $notifica = '';

          //GRID ACOMPANHAMENTOS

          //INBOX

          //error_reporting(E_ALL);
          //

          /////////////////// CREDENCIAIS DO IMAP ERRADAS ///////////////////////////////////////////////////////


          // $connect_to = '{imap.gmail.com:993/imap/ssl}INBOX';
          //$connect_to = '{imap.gmail.com:993/imap/ssl/novalidate-cert}INBOX';
          // $user = 'cct@ineditta.com.br';
          //$password = 'aero13@marisa';
          // $password = 'posdopnqnpbhxwif';

          // $mail_box = imap_open($connect_to, $user, $password);

          //$this->logger->debug($emails);

          //END INBOX

          while ($obj = $resultsql->fetch_object()) {

            $notifica = '';
            // if ($mail_box) {
            //     $emails = imap_search($mail_box, 'UNSEEN');

            //     if ($emails) {
            //         foreach ($emails as &$mensagem) {
            //             $mail_info = (imap_headerinfo($mail_box, $mensagem));
            //             //$body = (imap_fetchbody($mail_box, $mensagem, 1)); // 0=> retorna o body da mensagem com o texto que o servidor recebe / 1=> retorna somente o conteudo da mensagem em plain-text / 2=> retorna o conteudo da mensagem em html

            //             //$this->logger->debug($body);
            //             $this->logger->debug($mail_info->toaddress);
            //             if (strpos($mail_info->toaddress, 'Contato Ineditta - Acompanhamento #' . $obj->idacompanhanto_cct) !== false) {
            //                 $this->logger->debug("TEM EMAIL: ". $obj->idacompanhanto_cct);
            //                 $notifica = ' <a href="https://mail.google.com/mail/u/0" target="_blank" class="btn btn-primary" style="padding: 0; background: transparent; box-shadow: none;"><img src="includes/img/email-icon.jpg" alt="" style="width: 35px;"></a>';
            //             }
            //         }
            //     }

            // }else {
            //     $response['response_status']['email_error'] = "Não foi possível conectar ao serviço de e-mail!";
            //     $response['response_status']['email_status'] = 0;
            // }
            // $this->logger->debug($response);

            ///////////////////////////////////// CREDENCIAIS ERRADAS ////////////////////////////////////////


            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdAcompanhamento( ' . $obj->idacompanhanto_cct . ');"><i class="fa fa-file-text"></i></a></td>';
            $html .= '<td>';
            $html .= $notifica;
            $html .= '</td>';
            $html .= '<td class="title">';
            $html .= $obj->fase;
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= $obj->nome_usuario;
            $html .= '</td>';
            $html .= '<td class="desc1">';
            $html .= $obj->sigla_sinde;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->uf_sinde;
            $html .= '</td>';
            $html .= '<td class="desc2">';
            $html .= $obj->sigla_sp;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->data_base;
            $html .= '</td>';
            $html .= '<td class="desc3">';
            $html .= substr($obj->status, 2);
            $html .= '</td>';
            $html .= '<td class="desc4">';
            $html .= $obj->proxima_ligacao;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->ultima_atualizacao;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->nome_doc;
            $html .= '</td>';
            $html .= '</tr>';
          }
          // imap_close($mail_box); NAO FUNCIONA CREDENCIAIS
          $response['response_data']['listaPrincipal'] = $html;
          $this->logger->debug($html);
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

  function getByIdAcompanhamento($data = null)
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
        $id_sinde = null;
        $id_patr = null;

        $sql = "
				select ac.idacompanhanto_cct
				, ac.data_inicial as data_inicial
				, ac.data_final as data_final
				, date_format(ac.ultima_atualizacao, \"%d/%m/%Y\") as ultima_atualizacao
				, ac.status
				, ac.usuario_adm_id_usuario 
				, ac.fase
				, ac.observacoes_gerais as obs
				, ac.tipo_doc_idtipo_doc
				, ac.data_base
				, ac.sind_patr_id_sindp
				, ac.sind_emp_id_sinde
				, ac.ids_sindemp_adicionais as iemp
				, ac.ids_sindpatr_adicionais as ipatr
				, ac.ids_cnaes
                , se.municipio_sinde as cidade 
                , se.uf_sinde as estado
                , group_concat(DISTINCT cc.descricao_subclasse SEPARATOR \" | \") as atvs from acompanhanto_cct as ac
                LEFT JOIN sind_emp as se ON se.id_sinde = ac.sind_emp_id_sinde 
                left join classe_cnae as cc ON JSON_CONTAINS(
			ac.ids_cnaes,
			CONCAT('\"',cc.id_cnae, '\"'),'$') WHERE ac.idacompanhanto_cct = {$data['id']} GROUP BY ac.idacompanhanto_cct;
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $obj = $resultsql->fetch_object();

          $id_sinde = $obj->sind_emp_id_sinde;
          $id_patr = $obj->sind_patr_id_sindp;
          $response['response_data']['cidade'] = $obj->cidade;
          $response['response_data']['estado'] = $obj->estado;
          $response['response_data']['atvs'] = $obj->atvs;
          $response['response_data']['id'] = $obj->idacompanhanto_cct;
          $response['response_data']['status'] = $obj->status;
          $response['response_data']['tipo_doc_idtipo_doc'] = $obj->tipo_doc_idtipo_doc;
          $response['response_data']['dataini'] = $obj->data_inicial;
          $response['response_data']['datafim'] = $obj->data_final;
          $response['response_data']['resp'] = $obj->usuario_adm_id_usuario;
          $response['response_data']['fase'] = $obj->fase;
          $response['response_data']['db'] = $obj->data_base;
          $response['response_data']['obs'] = $obj->obs;
          $response['response_data']['emp'] = $obj->sind_patr_id_sindp;
          $response['response_data']['sind'] = $obj->sind_emp_id_sinde;
          $response['response_data']['cnae'] = is_array(json_decode($obj->ids_cnaes)) ? " " . implode(" ", json_decode($obj->ids_cnaes)) : "";
          $response['response_data']['iemp'] = is_array(json_decode($obj->iemp)) ? " " . implode(" ", json_decode($obj->iemp)) : "";
          $response['response_data']['ipatr'] = is_array(json_decode($obj->ipatr)) ? " " . implode(" ", json_decode($obj->ipatr)) : "";
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        //BUSCANDO FILIAIS

        if ($id_sinde) {
          $sqlEmp = "SELECT 
						IFNULL(GROUP_CONCAT(DISTINCT classe_cnae_idclasse_cnae), GROUP_CONCAT(IFNULL( classe_cnae_idclasse_cnae,null))) as cnae

					FROM base_territorialsindemp
					WHERE sind_empregados_id_sinde1 IN ({$id_sinde})
		";

          $this->logger->debug($sqlEmp);

          $objEmp = mysqli_query($this->db, $sqlEmp)->fetch_object();
          $this->logger->debug($objEmp);

          $cnaeEmp = $objEmp->cnae;
        } else {
          $cnaeEmp = 0;
        }


        if ($id_patr) {
          $sqlPatr = "SELECT 
						IFNULL(GROUP_CONCAT(DISTINCT classe_cnae_idclasse_cnae), GROUP_CONCAT(IFNULL( classe_cnae_idclasse_cnae,null))) as cnae

					FROM base_territorialsindpatro
					WHERE sind_patronal_id_sindp IN ({$id_patr})
		";

          $objPatr = mysqli_query($this->db, $sqlPatr)->fetch_object();
          $this->logger->debug($objPatr);

          $cnaePatr = $objPatr->cnae;
        } else {
          $cnaePatr = 0;
        }





        //UNIDADES ABRANGIDAS PELO DOCUMENTO
        $sqlCnae = "SELECT 
			IFNULL(GROUP_CONCAT(DISTINCT cliente_unidades_id_unidade), GROUP_CONCAT(IFNULL( cliente_unidades_id_unidade,null))) as clt
					FROM cnae_emp
					WHERE classe_cnae_idclasse_cnae IN ({$cnaeEmp},{$cnaePatr})
					ORDER BY clt ASC
			";

        $objClt = explode(",", mysqli_query($this->db, $sqlCnae)->fetch_object()->clt);

        $this->logger->debug($objClt);






        //BUSCANDO USUARIOS
        $sqlUser = "SELECT
							id_user,
							nome_usuario,
							email_usuario,
							ids_fmge,
							tipo
						FROM usuario_adm
				";

        $result = mysqli_query($this->db, $sqlUser);

        $listaDeEmails = [];
        while ($objUser = $result->fetch_object()) {
          $clDoUsuario = explode(",", str_replace('"', '', trim(str_replace(["[", "]"], [" ", " "], $objUser->ids_fmge))));

          //OBTEM A LISTA DOS DESTINATÁRIOS
          if ($clDoUsuario) {
            foreach ($clDoUsuario as $value) {
              $datas = $objUser->email_usuario; //"{$objUser->nome_usuario} + {
              if (in_array($value, $objClt) && $objUser->tipo == "Cliente" && !in_array($datas, $listaDeEmails)) {
                $this->logger->debug('Contém -> ' . $value . 'No usuario ' . $objUser->nome_usuario . "-- " . $objUser->email_usuario);
                array_push($listaDeEmails, $datas);
              }
            }
          }
        }

        $this->logger->debug($listaDeEmails);

        $response['response_data']['listaEmails'] = implode(",", $listaDeEmails);















        $sql = "SELECT distinct
				sp.id_sindp
				,sp.sigla_sp
				,sp.cnpj_sp
				,group_concat(distinct cc.descricao_subclasse SEPARATOR ' ; ') as atv_econo
				,concat(sp.email1_sp,' ; ',sp.email2_sp,' ; ',sp.email3_sp) as emails
				,concat(sp.fone1_sp,' ; ',sp.fone2_sp,' ; ',sp.fone3_sp) as telefones
				,sp.site_sp
				,group_concat(distinct nc.comentario SEPARATOR ' ; ') as comentarios
				,JSON_CONTAINS(
					(select ids_sindpatr_adicionais from acompanhanto_cct WHERE idacompanhanto_cct = 26),
					CONCAT('\"',sp.id_sindp, '\"'),'$') as checked
			FROM 
				sind_patr as sp 
				left JOIN base_territorialsindpatro as base ON base.sind_patronal_id_sindp = sp.id_sindp
				left join classe_cnae as cc on cc.id_cnae = base.classe_cnae_idclasse_cnae
				left join note_cliente as nc ON nc.id_tipo_comentario = sp.id_sindp AND tipo_comentario like 'patronal'
				GROUP BY sp.id_sindp ORDER BY sp.sigla_sp;	";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;

          $listIds = null;

          // 	$html .= '<div class="panel panel-sky" >
          // 	<div class="panel-heading">
          // 	<h4>Seleção Sindicato</h4>
          // 	<div class="options">
          // 		<a href="javascript:;" class="panel-collapse"><i class="fa fa-chevron-down"></i></a>
          // 	</div>
          // </div>
          // <div class="panel-body collapse in">

          // 	<table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered datatables" id="example">
          // 		<thead>
          // 			<tr>
          // 				<th></th>
          // 				<th>Sigla</th>
          // 				<th>CNPJ</th>
          // 				<th>Código</th>
          // 			</tr>
          // 		</thead>
          // 		<tbody >';

          $lista = [];
          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX">';
            if ($obj->checked) {
              $listIds .= ' ' . $obj->id_sindp;
              $html = '<td><input class="form-check-input" onclick="saveChange(' . $obj->id_sindp . ');" type="checkbox" value="0" id="check' . $obj->id_sindp . '" checked></td>';
            } else {
              $html = '<td><input class="form-check-input" onclick="saveChange(' . $obj->id_sindp . ');"  type="checkbox" value="1" id="check' . $obj->id_sindp . '"></td>';
            }
            // $html .= '<td>';
            // $html .= $obj->sigla_sp;
            // $html .= '</td>';
            // $html .= '<td>';
            // $html .= $obj->cnpj_sp;
            // $html .= '</td>';
            // $html .= '<td>';
            // $html .= $obj->codigo_sp;
            // $html .= '</td>';
            // $html .= '</tr>';

            $newObj = new stdClass();
            $newObj->campo = $html;
            $newObj->sigla = $obj->sigla_sp;
            $newObj->cnpj = formatCnpjCpf($obj->cnpj_sp);
            $newObj->atv_econo = $obj->atv_econo;
            $newObj->emails = $obj->emails;
            $newObj->telefones = $obj->telefones;
            $newObj->site = $obj->site_sp;
            $newObj->comentarios = $obj->comentarios;

            array_push($lista, $newObj);
          }

          // 		$html .= '  </tbody>
          // 		</table>

          // 	</div>
          // </div>'
          // ;

          $response['response_data']['sindspatro'] = $lista;
          $response['response_data']['listIds'] = $listIds;
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
							,JSON_CONTAINS(
								(select ids_cnaes from acompanhanto_cct WHERE idacompanhanto_cct = {$data['id']}),
								CONCAT('\"',id_cnae, '\"'),'$') as checked
						FROM 
							classe_cnae;								
				";
        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $listIds = null;
          $lista = [];
          //perder
          while ($obj = $resultsql->fetch_object()) {
            $input = '<tr class="odd gradeX">';
            if ($obj->checked) {
              $listIds .= ' ' . $obj->id_cnae;
              $input = '<td><input type="checkbox"onclick="selectCNAEMultiU( ' . $obj->id_cnae . ');" class="btn btn-secondary" id="checu' . $obj->id_cnae . '" checked></td>';
            } else {
              $input = '<td><input type="checkbox"onclick="selectCNAEMultiU( ' . $obj->id_cnae . ');" class="btn btn-secondary" id="checu' . $obj->id_cnae . '"></td>';
            }
            // $input = '<input type="checkbox"onclick="selectCNAEMultiU( ' . $obj->id_cnae . ');" class="btn btn-secondary" id="checu' . $obj->id_cnae . '"/>';
            // $html .= '<tr class="odd gradeX tbl-item">';
            // $html .= '<td><input type="checkbox"onclick="selectCNAEMulti( ' . $obj->id_cnae . ');" class="btn btn-secondary" id="chec' . $obj->id_cnae . '"/></td>';
            // $html .= '<td class="title">';
            // $html .= $obj->id_cnae;
            // $html .= '</td>';
            // $html .= '<td>';
            // $html .= $obj->subclasse_cnae;
            // $html .= '</td>';
            // $html .= '<td class="desc">';
            // $html .= $obj->descricao_subclasse;
            // $html .= '</td>';
            // $html .= '<td>';
            // $html .= $obj->categoria;
            // $html .= '</td>';
            // $html .= '</tr>';


            // $htmlupdate .= '<tr class="odd gradeX tbl-item">';
            // $htmlupdate .= '<td><input type="checkbox"onclick="selectCNAEMultiU( ' . $obj->id_cnae . ');" class="btn btn-secondary" id="checu' . $obj->id_cnae . '"/></td>';
            // $htmlupdate .= '<td class="title">';
            // $htmlupdate .= $obj->id_cnae;
            // $htmlupdate .= '</td>';
            // $htmlupdate .= '<td>';
            // $htmlupdate .= $obj->subclasse_cnae;
            // $htmlupdate .= '</td>';
            // $htmlupdate .= '<td class="desc">';
            // $htmlupdate .= $obj->descricao_subclasse;
            // $htmlupdate .= '</td>';
            // $htmlupdate .= '<td>';
            // $htmlupdate .= $obj->categoria;
            // $htmlupdate .= '</td>';
            // $htmlupdate .= '</tr>';

            $newObj = new stdClass();
            $newObj->campo = $input;
            $newObj->id_cnae = $obj->id_cnae;
            $newObj->subclasse_cnae = $obj->subclasse_cnae;
            $newObj->descricao_subclasse = $obj->descricao_subclasse;
            $newObj->categoria = $obj->categoria;

            array_push($lista, $newObj);
          }

          $response['response_data']['listaAtvEcono'] = $lista;
          $response['response_data']['listaAtvEconoIds'] = $listIds;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


































        $sql = "SELECT distinct
							se.id_sinde
							,se.sigla_sinde
							,se.cnpj_sinde
                            ,group_concat(distinct cc.descricao_subclasse SEPARATOR ' ; ') as atv_econo
                            ,concat(se.email1_sinde,' ; ',se.email2_sinde,' ; ',se.email3_sinde) as emails
                            ,concat(se.fone1_sinde,' ; ',se.fone2_sinde,' ; ',se.fone3_sinde) as telefones
                            ,se.site_sinde
                            ,group_concat(distinct nc.comentario SEPARATOR ' ; ') as comentarios
                            ,JSON_CONTAINS(
								(select ids_sindemp_adicionais from acompanhanto_cct WHERE idacompanhanto_cct = {$data['id']}),
								CONCAT('\"',se.id_sinde, '\"'),'$') as checked
						FROM 
							sind_emp as se 
                            left JOIN base_territorialsindemp as base ON base.sind_empregados_id_sinde1 = se.id_sinde
                            left join classe_cnae as cc on cc.id_cnae = base.classe_cnae_idclasse_cnae
                            left join note_cliente as nc ON nc.id_tipo_comentario = se.id_sinde AND tipo_comentario like 'laboral'
                            GROUP BY se.id_sinde ORDER BY se.sigla_sinde;							
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;

          $listIds = null;
          $lista = [];

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX">';
            if ($obj->checked) {
              $listIds .= ' ' . $obj->id_sinde;
              $html = '<td><input class="form-check-input" onclick="saveChange1(' . $obj->id_sinde . ');" type="checkbox" value="0" id="checks' . $obj->id_sinde . '" checked></td>';
            } else {
              $html = '<td><input class="form-check-input" onclick="saveChange1(' . $obj->id_sinde . ');"  type="checkbox" value="1" id="checks' . $obj->id_sinde . '"></td>';
            }
            // $html .= '<td class="title">';
            // $html .= $obj->sigla_sinde;
            // $html .= '</td>';
            // $html .= '<td>';
            // $html .= $obj->cnpj_sinde;
            // $html .= '</td>';
            // $html .= '<td>';
            // $html .= $obj->codigo_sinde;
            // $html .= '</td>';
            // $html .= '</tr>';

            $newObj = new stdClass();
            $newObj->campo = $html;
            $newObj->sigla = $obj->sigla_sinde;
            $newObj->cnpj = formatCnpjCpf($obj->cnpj_sinde);
            $newObj->atv_econo = $obj->atv_econo;
            $newObj->emails = $obj->emails;
            $newObj->telefones = $obj->telefones;
            $newObj->site = $obj->site_sinde;
            $newObj->comentarios = $obj->comentarios;

            array_push($lista, $newObj);
          }

          $html .= '';

          $response['response_data']['sindsemp'] = $lista;
          $response['response_data']['listIdse'] = $listIds;
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





  function getByIdEnvolvidos($data = null)
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
				select group_concat(distinct cc.descricao_subclasse separator \";\") as atvs_emp,
 group_concat(distinct ccc.descricao_subclasse separator \";\") as atvs_patr,sp.sigla_sp
				, sp.fone1_sp
				, sp.fone2_sp
				, sp.fone3_sp
				, sp.email1_sp
				, sp.email2_sp
				, sp.email3_sp
				, sp.site_sp
				, se.sigla_sinde
				, se.fone1_sinde
				, se.fone2_sinde
				, se.fone3_sinde
				, se.email1_sinde
				, se.email2_sinde
				, se.email3_sinde
				, se.site_sinde
				, j.fase as jform
				from sind_patr as sp left join jfase as j on fase->>\"$.fase\" = \"{$data['fase']}\" left join
				 sind_emp as se on se.id_sinde = {$data['id-sind']}
                 left join base_territorialsindemp as be ON be.sind_empregados_id_sinde1 = {$data['id-sind']}
                 left join classe_cnae as cc on cc.id_cnae = be.classe_cnae_idclasse_cnae
                 left join base_territorialsindpatro as bp ON bp.sind_patronal_id_sindp = {$data['id-emp']}
                 left join classe_cnae as ccc on ccc.id_cnae = bp.classe_cnae_idclasse_cnae
				 where sp.id_sindp = {$data['id-emp']};
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {
          //$obj = $resultsql->fetch_object();
          $response['response_data']['dados'] = $resultsql->fetch_object();
          // $response['response_data']['status'] 	= $obj->status;
          // $response['response_data']['dataini'] 	= $obj->data_inicial;
          // $response['response_data']['datafim'] 	= $obj->data_final;
          // $response['response_data']['resp'] 	= $obj->usuario_adm_id_usuario;
          // $response['response_data']['fase'] 	= $obj->fase;
          // $response['response_data']['emp'] 	= $obj->sind_patr_id_sindp;
          // $response['response_data']['sind'] 	= $obj->sind_emp_id_sinde;
          // $response['response_data']['cnae'] 	= $obj->ids_cnaes;

        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        //PEGANDO OS COMENTÁRIOS


        $sql = "
				select * from note_cliente where id_tipo_comentario = {$data['id-emp']} AND tipo_comentario like 'patronal';
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {
          $comentspatr = '';


          while ($obj = $resultsql->fetch_object()) {
            $comentspatr .= $obj->data_registro;

            $comentspatr .= "\n\n";

            $comentspatr .= $obj->comentario;

            $comentspatr .= "\n\n";
          }

          $response['response_data']['coment_patr'] = $comentspatr;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        $sql = "
				select * from note_cliente where id_tipo_comentario = {$data['id-sind']} AND tipo_comentario like 'laboral';
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {
          $comentslab = '';


          while ($obj = $resultsql->fetch_object()) {
            $comentslab .= $obj->data_registro;

            $comentslab .= "\n\n";

            $comentslab .= $obj->comentario;

            $comentslab .= "\n\n";
          }

          $response['response_data']['coment_lab'] = $comentslab;
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





  function addAcompanhamento($data = null)
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

      $CNAES = json_encode(explode(" ", trim($data['cnae-input'])));

      $di = $data['dataini-input'] ? $data['dataini-input'] : '0000-00-00';
      $df = $data['datafim-input'] ? $data['datafim-input'] : '0000-00-00';

      $si = $data['sind-input'] ? $data['sind-input'] : '0';
      $sp = $data['emp-input'] ? $data['emp-input'] : '0';

      $this->logger->debug($connectdb);
      if ($response['response_status']['status'] == 1) {

        $today = date('d/m/Y');
        $sql = "insert into acompanhanto_cct (
				tipo_doc_idtipo_doc
				,data_inicial
				,data_final
				,ultima_atualizacao
				,status
				,usuario_adm_id_usuario
				,fase
				,data_base
				,sind_patr_id_sindp
				,sind_emp_id_sinde
				,ids_cnaes
				,ids_sindemp_adicionais
				,ids_sindpatr_adicionais) values ({$data['tipodoc-input']}
				,\"{$di}\"
				,\"{$df}\"
				,str_to_date(\"{$today}\", '%d/%m/%Y')
				,\"{$data['status-input']}\"
				,{$data['resp-input']}
				,\"{$data['fase-input']}\"
				,\"{$data['db-input']}\"
				,{$sp}
				,{$si}
				,'{$CNAES}'
					,\"[]\"
					,\"[]\"
				); 
				";
        $this->logger->debug($sql);
        if (!mysqli_query($this->db, $sql)) {

          mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

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

  function updateAcompanhamento($data = null)
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

      $ademp = json_encode(explode(" ", trim($data['sindemps-input'])));
      $adpatr = json_encode(explode(" ", trim($data['sindpatrs-input'])));

      $cnaesu = json_encode(explode(" ", trim($data['cnae-input'])));

      $di = $data['dataini-input'] ? $data['dataini-input'] : '0000-00-00';
      $df = $data['datafim-input'] ? $data['datafim-input'] : '0000-00-00';


      if ($response['response_status']['status'] == 1) {
        $today = date('d/m/Y');
        $sql = " UPDATE acompanhanto_cct
						SET  fase = '{$data['fase-input']}'
							,tipo_doc_idtipo_doc = {$data['tipodoc-input']} 
							,status = '{$data['status-input']}'
							,fase = '{$data['fase-input']}'
							,data_base = '{$data['db-input']}'
							,data_inicial = \"{$di}\"
							,data_final = \"{$df}\"
							,ultima_atualizacao = str_to_date(\"{$today}\", '%d/%m/%Y')
							,usuario_adm_id_usuario = {$data['resp-input']}
							,sind_patr_id_sindp = {$data['emp-input']}
							,sind_emp_id_sinde = {$data['sind-input']}
							,ids_cnaes = '{$cnaesu}'
							,ids_sindemp_adicionais = '{$ademp}'
							,ids_sindpatr_adicionais = '{$adpatr}'
							,observacoes_gerais = '{$data['client-report']}'
								WHERE 
								idacompanhanto_cct = {$data['id']};
						";
        $this->logger->debug($sql);
        if (!mysqli_query($this->db, $sql)) {

          mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

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


        $telaspas = explode(" ", $data['sindpatrs-input']);
        $bancospas = explode(" ", $data['bancospa-input']);

        foreach ($telaspas as &$telaspa) {
          if (in_array($telaspa, $bancospas)) {
            $this->logger->debug($telaspa . "JÁ ESTÁ NO BANCO");
          } else {
            $sql = " insert into acompanhamento_envolvidos_patr 
								(sind_patr_id_sindp, acompanhamento_cct_idacompanhamento_cct)
								values ({$telaspa}, {$data['id']});
								";
            $this->logger->debug($sql);
            if (!mysqli_query($this->db, $sql)) {

              mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

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
          }
        }

        foreach ($bancospas as &$bancospa) {
          if (in_array($bancospa, $telaspas)) {
            $this->logger->debug($bancospa . "PODE PERMANECER NO BANCO");
          } else {
            $sql = " delete from acompanhamento_envolvidos_patr
						 where id_acompanhamento_envolvidos > 0
						  and sind_patr_id_sindp = {$bancospa} and acompanhamento_cct_idacompanhamento_cct = {$data['id']};";
            $this->logger->debug($sql);
            if (!mysqli_query($this->db, $sql)) {

              mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

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
          }
        }

        $telaseas = explode(" ", $data['sindemps-input']);
        $bancoseas = explode(" ", $data['bancosea-input']);

        foreach ($telaseas as &$telasea) {
          if (in_array($telasea, $bancoseas)) {
            $this->logger->debug($telasea . "JÁ ESTÁ NO BANCO");
          } else {
            $sql = " insert into acompanhamento_envolvidos_emp 
								(sind_emp_id_sinde, acompanhamento_cct_idacompanhamento_cct)
								values ({$telasea}, {$data['id']});
								";
            $this->logger->debug($sql);
            if (!mysqli_query($this->db, $sql)) {

              mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

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
          }
        }

        foreach ($bancoseas as &$bancosea) {
          if (in_array($bancosea, $telaseas)) {
            $this->logger->debug($bancosea . "PODE PERMANECER NO BANCO");
          } else {
            $sql = " delete from acompanhamento_envolvidos_emp
						 where id_acompanhamento_envolvidos > 0
						  and sind_emp_id_sinde = {$bancosea} and acompanhamento_cct_idacompanhamento_cct = {$data['id']};";
            $this->logger->debug($sql);
            if (!mysqli_query($this->db, $sql)) {

              mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

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

  function salvarScript($data = null)
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
        //$respostas = "'Sim','Laboral','E-mail','Não','Não','OBS'";
        $sql = " update acompanhanto_cct set
				 scripts_salvos = JSON_ARRAY_INSERT(scripts_salvos,
				  '$[last]', JSON_OBJECT('horario', NOW(), 'fase',
				   '{$data['fase']} (Usuário: {$data['resp']})', 'respostas',
				    JSON_ARRAY({$data['respostas']}))) where idacompanhanto_cct = {$data['id']};
				";
        if (!mysqli_query($this->db, $sql)) {

          mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        $today = date('d/m/Y');

        $sql = " UPDATE acompanhanto_cct
						    SET  ultima_atualizacao = str_to_date(\"{$today}\", '%d/%m/%Y')	
								WHERE 
								idacompanhanto_cct = {$data['id']};
						";
        if (!mysqli_query($this->db, $sql)) {

          mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        function getDateByPriority($priority)
        {
          $now = new DateTime();

          switch ($priority) {
            case "2 Alta":
            case "1 Cliente":
              $interval = new DateInterval("P7D");
              break;
            case "3 Média":
              $interval = new DateInterval("P15D");
              break;
            case "4 Baixa":
              $interval = new DateInterval("P30D");
              break;
            default:
              return false;
          }

          $dueDate = $now->add($interval);

          return $dueDate->format("d/m/Y");
        }

        if ($data['att_ligacao'] == 0) {
          $tomorrow = date('d/m/Y', strtotime('+1 day'));
          $sql = " UPDATE acompanhanto_cct SET  proxima_ligacao = str_to_date(\"{$tomorrow}\", '%d/%m/%Y') WHERE idacompanhanto_cct = {$data['id']};
						";
          if (!mysqli_query($this->db, $sql)) {
            mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = '';
          } else {
            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = '';
          }
        } else {
          $day = getDateByPriority($data['pri']);
          $sql = " UPDATE acompanhanto_cct
						SET proxima_ligacao = str_to_date(\"{$day}\", '%d/%m/%Y') WHERE idacompanhanto_cct = {$data['id']};
						";
          if (!mysqli_query($this->db, $sql)) {

            mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = '';
          } else {
            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = '';
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



  function getByIdHist($data = null)
  {
    "select ac.scripts_salvos from acompanhanto_cct as ac where idacompanhanto_cct = {$data['id']};";
  }

  function getPerguntas($data = null)
  {
    "select  fase->>'$.perguntas' as perguntas from jfase where fase->>'$.fase' = '{$data['fase']}';";
  }


  function updateAcompanhamentoFase($data = null)
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
        $sql = " UPDATE acompanhanto_cct
						SET  fase = '{$data['fase']}'
								WHERE 
								idacompanhanto_cct = {$data['id']}; 
						";
        if (!mysqli_query($this->db, $sql)) {
          mysqli_query($this->db, 'TRUNCATE TABLE discounts;');

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        } else {
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

    return $response;
  }








  function dispararEmailsAcompanhamento($data = null)
  {
    if ($this->response['response_status']['status'] == 1) {
      // Carregando a resposta padrão da função
      $response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

      // Montando o c??o do erro que ser?presentado
      $localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
      $substituir = array("", "", "", "", "-");
      $error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

      // Declarando os caminhos principais do sistema.
      $localizar = array("\\", "/includes/php");
      $substituir = array("/", "");
      $path = str_replace($localizar, $substituir, __DIR__);

      $anexo = $path . '/anexos/';

      if ($response['response_status']['status'] == 1) {
        $enviar = 0;
        $mail = new PHPMailer\PHPMailer\PHPMailer();

        try {
          $mail->SMTPDebug = 1;

          // utilizar SMTP
          $mail->isSMTP();

          // habilita autenticação SMTP
          $mail->SMTPAuth = true;

          // servidor SMTP
          $mail->Host = 'smtp.gmail.com';

          // usuário, senha e porta do SMTP
          $mail->Username = 'cct@ineditta.com.br';
          $mail->Password = 'posdopnqnpbhxwif';
          $mail->Port = 465;

          // tipo de criptografia: "tls" ou "ssl"
          $mail->SMTPSecure = 'ssl';

          $mensagem = utf8_decode("{$data['nome']}");

          $mail->setFrom('cct@ineditta.com.br', "{$mensagem}");

          if ($enviar == 0) {
            $enviar = 1;
          }

          $mail->addAddress("{$data['to']}", '');

          //$mail->addReplyTo('muralha77de77concreto@gmail.com', 'ULTIMATE PHP');
          $mail->isHTML(true);

          // codificação UTF-8
          $mail->Charset = 'UTF-8';

          // assunto do email
          $mail->Subject = utf8_decode("{$data['assunto']}");
          $mail->Body = utf8_decode("

					<p>{$data['msg']}</p>

				");
          $mail->send();

          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = null;
          $response['response_status']['msg'] = 'Disparos enviados com sucesso!';
        } catch (Exception $e) {
          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = null;
          $response['response_status']['msg'] = 'Falha ao enviar email. Entre em contato com o administrador do sistema!';
        }
      }
    } else {
      $response = $this->response;
    }
    $r = json_encode($response);

    return $r;
  }

  function dispararE($data = null)
  {

    if ($this->response['response_status']['status'] == 1) {
      // Carregando a resposta padrão da função
      $response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

      // Montando o c??o do erro que ser?presentado
      $localizar = array(strtolower(__DIR__), "/", "\\", ".php", ".");
      $substituir = array("", "", "", "", "-");
      $error_code = strtoupper(str_replace($localizar, $substituir, strtolower(__FILE__))) . "-";

      // Declarando os caminhos principais do sistema.
      $localizar = array("\\", "/includes/php");
      $substituir = array("/", "");
      $path = str_replace($localizar, $substituir, __DIR__);

      //Incluindo descrição do erro no log do sistema.
      $mail = new disparo_email();
      $response['response_email'] = $mail->dispararEmails([
        "email_remetente" => $data["email_remetente"],
        "senha" => $data["senha"],
        "cripto" => $data["cripto"],
        "porta" => $data["porta"],
        "nome" => $data["nome"],
        "to_multi" => $data["to_multi"],
        "assunto" => $data["assunto"],
        "msg" => $data["msg"]
      ]);
    } else {
      $response = $this->response;
    }

    return $response;
  }
}