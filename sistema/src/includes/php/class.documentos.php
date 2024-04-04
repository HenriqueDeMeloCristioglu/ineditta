<?php

/**
 * @author    {Rafael P. Cruz}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
    2021-07-02 15:39 ( v1.0.0 ) - 
  }
**/

ini_set('memory_limit', '256M');

include_once __DIR__ . "/helpers.php";

class documentos
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

  function getLista($data)
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

        //LISTA CNAE - ATIVIDADE ECONOMICA
        $sql = "SELECT 
							id_cnae
                            ,descricao_subclasse
							,categoria
						FROM 
							classe_cnae;								
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;

          while ($obj = $resultsql->fetch_object()) {

            $html .= "<tr class='tbl-item'>";
            $html .= "<td><input type='checkbox' id='check{$obj->id_cnae}' onclick='selectCnae({$obj->id_cnae})'></td>";
            $html .= "<td class='title'>{$obj->descricao_subclasse}</td>";
            $html .= "<td class='desc'>{$obj->categoria}</td>";
            $html .= "</tr>";

          }

          $response['response_data']['listaCnae'] = $html;

        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        //LISTA USUARIOS
        $sqlUser = "SELECT id_user, nome_usuario, departamento FROM usuario_adm WHERE id_grupoecon = {$data} OR {$data} = 0";

        if ($resultsql = mysqli_query($this->db, $sqlUser)) {

          $user = "";
          while ($obj = $resultsql->fetch_object()) {
            $nome = ucwords(strtolower($obj->nome_usuario));
            $firstName = strstr(ucwords(strtolower($obj->nome_usuario)), " ", true);
            $user .= "<tr class='tbl-item'>";
            $user .= "<td><input type='checkbox' data-firstName='{$firstName}' data-depto='{$obj->departamento}' id='user{$obj->id_user}' onclick='addUser({$obj->id_user})'></td>";
            $user .= "<td class='title'>{$nome}</td>";
            $user .= "<td class='desc'>{$obj->departamento}</td>";
            $user .= "</tr>";

          }

          $response['response_data']['listaUsuarios'] = $user;

        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        //LISTA EMPREGADOS
        $sql = "SELECT 
							id_sinde
							,denominacao_sinde
							,sigla_sinde
							,cnpj_sinde
                            ,logradouro_sinde
                            ,email1_sinde
                            ,fone1_sinde
                            ,site_sinde
						FROM 
							sind_emp		
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = "<option value=''></option>";
          while ($obj = $resultsql->fetch_object()) {
            $cnpj = formatCnpjCpf($obj->cnpj_sinde);
            $html .= "<option value='{$obj->id_sinde}'>{$obj->sigla_sinde} / {$cnpj} / {$obj->denominacao_sinde}</option>";

          }

          $response['response_data']['listaSindEmp'] = $html;

        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        //LISTA PATRONAL
        $sql = "SELECT 
							id_sindp
							,denominacao_sp
							,sigla_sp
							,cnpj_sp
                            ,logradouro_sp
                            ,email1_sp
                            ,fone1_sp
                            ,site_sp
						FROM 
							sind_patr
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = "<option value=''></option>";
          while ($obj = $resultsql->fetch_object()) {
            $cnpj = formatCnpjCpf($obj->cnpj_sp);
            $html .= "<option value='{$obj->id_sindp}'>{$obj->sigla_sp} / {$cnpj} / {$obj->denominacao_sp}</option>";
          }

          $response['response_data']['listaPatronal'] = $html;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        //LISTA EMPRESA
        $sql = "SELECT
							id_unidade,
							nome_unidade as filial,
							nome_empresa as matriz,
							nome_grupoeconomico as grupo
						FROM cliente_unidades
						WHERE cliente_grupo_id_grupo_economico = {$_SESSION['grupoecon']};	
				";

        $result = mysqli_query($this->db, $sql);
        $html = "";
        while ($obj = $result->fetch_object()) {
          $html .= "<tr class='tbl-item'>";
          $html .= "<td><input type='checkbox' id='emp{$obj->id_unidade}' onclick='selectEmpresa({$obj->id_unidade})'></td>";
          $html .= "<td>{$obj->grupo}</td>";
          $html .= "<td class='desc'>{$obj->matriz}</td>";
          $html .= "<td class='title'>{$obj->filial}</td>";
          $html .= "</tr>";
        }

        $response['response_data']['lista_empresa'] = $html;


        // OPTION LOCALIZAÇÃO
        $sql = "SELECT 
							id_localizacao
						    ,municipio
						FROM 
							localizacao								
				";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = null;
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_localizacao;
            $grupos .= '">';
            $grupos .= $obj->municipio;
            $grupos .= '</option>';
          }

          $response['response_data']['local'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        // OPTION TIPO DOCUMENTO
        $sql = "SELECT 
							idtipo_doc,
							nome_doc
						FROM 
							tipo_doc
						WHERE processado = 'N'		
						ORDER BY nome_doc ASC							
				";

        $this->logger->debug(mysqli_query($this->db, $sql));
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = "<option value=''></option>";
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="' . $obj->idtipo_doc . '">';
            $grupos .= $obj->nome_doc;
            $grupos .= '</option>';
          }

          $response['response_data']['tipoDoc'] = $grupos;
        } else {
          $this->logger->debug($sql);
          $this->logger->debug($this->db->error);

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        //LISTA DE CLAUSULAS PARA REFERENCIAMENTO
        $sqlRef = "SELECT 
								id_estruturaclausula,
								nome_clausula
							FROM estrutura_clausula
				";

        if ($resultRef = mysqli_query($this->db, $sqlRef)) {

          $opt = null;
          $opt .= "<option value=''></option>";
          while ($objRef = $resultRef->fetch_object()) {
            $opt .= "<option value='{$objRef->id_estruturaclausula}'>{$objRef->nome_clausula}</option>";

          }

          $response['response_data']['listaEstruturaClausula'] = $opt;
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

    $this->logger->debug($response['response_data']);

    return $response;
  }


  function addDocumentos($data = null)
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


      $this->logger->debug($data);

      $validadeInicial = !$data['vigencia_ini'] ? "00/00/0000" : $data['vigencia_ini'];
      $validadeFinal = !$data['vigencia_fim'] ? "00/00/0000" : $data['vigencia_fim'];
      $caminho = "https://ineditta.com/documentos_sistema/documentos_sindicais/" . $data['caminho_arquivo'];

      //SIND LABORAL
      $data['laboral'] = json_decode($data['laboral']);
      if (isset($data['laboral']) && is_array($data['laboral']) && sizeof($data['laboral']) > 0) {
        $ids = implode(',', array_map('intval', $data['laboral']));
        $sql = "SELECT id_sinde, sigla_sinde, cnpj_sinde, uf_sinde FROM sind_emp WHERE id_sinde IN ($ids)";
        $result = mysqli_query($this->db, $sql);

        $sindEmp = [];
        while ($resObj = $result->fetch_assoc()) {
          $uf = $resObj['uf_sinde'];
          array_push($sindEmp, "{" . '"id":' . $resObj['id_sinde'] . ',"sigla":' . '"' . $resObj['sigla_sinde'] . '"' . ',"cnpj":' . '"' . $resObj['cnpj_sinde'] . '"' . "}");
        }
        $sindEmp = "[" . implode(",", $sindEmp) . "]";

        $this->logger->debug($sindEmp);
      } else {
        $sindEmp = "[]";
      }

      //SIND PATRONAL
      $data['patronal'] = json_decode($data['patronal']);
      if (isset($data['patronal']) && is_array($data['patronal']) && sizeof($data['patronal']) > 0) {
        $ids = implode(',', array_map('intval', $data['patronal']));
        $sql = "SELECT id_sindp, sigla_sp, cnpj_sp FROM sind_patr WHERE id_sindp IN ($ids)";
        $result = mysqli_query($this->db, $sql);

        $sindPatr = [];
        while ($resObj = $result->fetch_assoc()) {
          array_push($sindPatr, "{" . '"id":' . $resObj['id_sindp'] . ',"sigla":' . '"' . $resObj['sigla_sp'] . '"' . ',"cnpj":' . '"' . $resObj['cnpj_sp'] . '"' . "}");
        }
        $sindPatr = "[" . implode(",", $sindPatr) . "]";

        $this->logger->debug($sindPatr);
      } else {
        $sindPatr = "[]";
      }


      //CLIENTE UNIDADES
      $unidades = explode(" ", trim($data['empresa']));
      $estabelecimentos = "[]";
      $this->logger->debug($unidades);
      if ($unidades[0] != "") {
        $array = [];
        foreach ($unidades as $unidade) {

          $sql = "SELECT id_unidade, cliente_matriz_id_empresa as matriz, cliente_grupo_id_grupo_economico as grupo FROM cliente_unidades WHERE id_unidade = '{$unidade}'";
          $resObj = mysqli_query($this->db, $sql)->fetch_object();

          array_push($array, "{" . '"g":' . $resObj->grupo . ',"m":' . $resObj->matriz . ',"u":' . $resObj->id_unidade . "}");
        }

        $lista = implode(",", $array);
        $estabelecimentos = "[" . $lista . "]";

        $this->logger->debug($estabelecimentos);
      }

      //OBTÉM ABRANGENCIA
      $abrang = "[]";
      $array = [];
      $abrangs = explode(" ", trim($data['abrangencia']));
      $this->logger->debug($abrang);
      if ($abrangs[0] != "") {
        foreach ($abrangs as $local) {
          $sql = "SELECT id_localizacao, municipio, uf FROM localizacao WHERE id_localizacao = '{$local}'";
          $resObj = mysqli_query($this->db, $sql)->fetch_object();

          array_push($array, "{" . '"id":' . '"' . $resObj->id_localizacao . '"' . ',"Municipio":' . '"' . $resObj->municipio . '"' . ',"Uf":' . '"' . $resObj->uf . '"' . "}");
        }

        $lista = implode(",", $array);
        $abrang = "[" . $lista . "]";

        $this->logger->debug($abrang);
      }

      //OBTÉM CNAE
      $cnae = "[]";
      $cnaes = explode(" ", trim($data['cnae']));
      if ($cnaes[0] != "") {
        $array = [];
        foreach ($cnaes as $cnae) {
          $sql = "SELECT id_cnae, descricao_subclasse FROM classe_cnae WHERE id_cnae = '{$cnae}'";
          $resObj = mysqli_query($this->db, $sql)->fetch_object();

          array_push($array, "{" . '"id":' . '"' . $resObj->id_cnae . '"' . ',"subclasse":' . '"' . $resObj->descricao_subclasse . '"' . "}");
        }

        $lista = implode(",", $array);
        $cnae = "[" . $lista . "]";

        $this->logger->debug($cnae);
      }

      $fileName = basename($caminho);

      $abrangEscapado = mysqli_real_escape_string($this->db, $abrang);
      $descricao = mysqli_real_escape_string($this->db, $data['descricao']);
      $origem = mysqli_real_escape_string($this->db, $data['origem']);
      $numero_lei = mysqli_real_escape_string($this->db, $data['numero_lei']);
      $anuencia = mysqli_real_escape_string($this->db, $data['anuencia']);
      $restrito = mysqli_real_escape_string($this->db, $data['restrito']);
      $nome = mysqli_real_escape_string($this->db, $data['nome']);
      $sindEmpEscapado = mysqli_real_escape_string($this->db, $sindEmp);
      $sindPatrEscapado = mysqli_real_escape_string($this->db, $sindPatr);
      $fonte_site = mysqli_real_escape_string($this->db, $data['fonte_site']);
      $comentarios = mysqli_real_escape_string($this->db, $data['comentarios']);
      $fileNameEscapado = mysqli_real_escape_string($this->db, $fileName);
      $cnaeEscapado = mysqli_real_escape_string($this->db, $cnae);
      $assunto = mysqli_real_escape_string($this->db, $data['assunto']);
      $estabelecimentosEscapado = mysqli_real_escape_string($this->db, $estabelecimentos);
      $nomeArquivo = mysqli_real_escape_string($this->db, $data['nome_arquivo']);

      //INSERT DOCUMENTOS
      $sql = "INSERT INTO doc_sind
						(descricao_documento,
						origem,
						numero_lei,
						anuencia,
						doc_restrito,
						tipo_doc_idtipo_doc,
						sind_laboral,
						sind_patronal,
						validade_inicial,
						validade_final,
						fonte_web,
						status,
						observacao,
						usuario_cadastro,
						caminho_arquivo,
						cnae_doc,
						abrangencia,
						referencia,
						cliente_estabelecimento,
						data_upload,
						modulo,
            nome_arquivo
						)
					VALUES (
						'$descricao',
						'$origem',
						'$numero_lei',
						'$anuencia',
						'$restrito',
						'$nome',
						'$sindEmpEscapado',
						'$sindPatrEscapado',
						STR_TO_DATE('{$validadeInicial}', '%d/%m/%Y'),
						STR_TO_DATE('{$validadeFinal}', '%d/%m/%Y'),
						'$fonte_site',
						'ativo',
						'$comentarios',
						{$data['usuario_cad']},
						'$fileNameEscapado',
						'$cnaeEscapado',
						'$abrangEscapado',
						'$assunto',
						'$estabelecimentosEscapado',
						curdate(),
						'COMERCIAL',
            '$nomeArquivo'
					)
			";

      $this->logger->debug($sql);
      if (!mysqli_query($this->db, $sql)) {

        $response['response_status']['status'] = 0;
        $response['response_status']['error_code'] = $this->error_code . __LINE__;
        $response['response_status']['msg'] = 'Erro ao cadastrar';

        $this->logger->debug($sql);
        $this->logger->debug($this->db->error);
        $this->logger->debug($response);
      } else {

        $response['response_status']['status'] = 1;
        $response['response_status']['error_code'] = $this->error_code . __LINE__;
        $response['response_status']['msg'] = 'Cadastro realizado com sucesso!';
      }

      $lastId = mysqli_insert_id($this->db);
      $response['response_status']['created_document_id'] = $lastId;

      if ($lastId) {
        //REGISTRO USUARIOS-ANUENCIA
        if ($data['anuencia'] == 'sim') {
          $users = explode(" ", trim($data['usuarios']));

          foreach ($users as $user) {
            $userId = intval($user);
            $sqlUser = "INSERT INTO anuencia_usuarios
										(documentos_iddocumentos,
										usuario_adm_id_user)
									VALUES (
										{$lastId},
										{$userId}
									)
						";

            if (!mysqli_query($this->db, $sqlUser)) {

              $response['response_status']['status'] = 0;
              $response['response_status']['error_code'] = $this->error_code . __LINE__;
              $response['response_status']['msg'] = 'Erro ao cadastrar usuários';

              $this->logger->debug($sql);
              $this->logger->debug($this->db->error);
              $this->logger->debug($response);
            } else {

              $response['response_status']['status'] = 1;
              $response['response_status']['error_code'] = $this->error_code . __LINE__;
              $response['response_status']['msg'] = 'Cadastro de usuários/anuencia realizado com sucesso!';
            }
          }
        }

        // //INSERT ABRANGENCIA TABLE
        // $abrang = explode(" ", trim($data['abrangencia']));

        // if ($abrang) {
        // 	for ($i=0; $i < count($abrang) ; $i++) { 
        // 		$sql2 = "INSERT INTO documentos_abrangencia 
        // 					(localizacao_id_localizacao, 
        // 					documentos_iddocumentos) 
        // 				VALUES (
        // 					'{$abrang[$i]}', 
        // 					'{$lastId}' )
        // 		";

        // 		$this->logger->debug( $sql2 );

        // 		if( !mysqli_query( $this->db, $sql2 ) ){

        // 			$response['response_status']['status']       = 0;
        // 			$response['response_status']['error_code']   = $this->error_code . __LINE__;
        // 			$response['response_status']['msg']          = 'Erro ao cadastrar abrangencia do documento.';

        // 			$this->logger->debug( $this->db->error );
        // 			$this->logger->debug( $response );
        // 		}
        // 		else{

        // 			$response['response_status']['status']       = 1;
        // 			$response['response_status']['error_code']   = $this->error_code . __LINE__;
        // 			$response['response_status']['msg']          = 'Abrangencia registrada com sucesso';
        // 		}
        // 	}
        // }


        // //ADD ASSUNTO
        // $assunto = $data['assunto'];
        // if ($assunto) {
        // 	for ($i=0; $i < count($assunto) ; $i++) { 
        // 		$sqlRef = "INSERT INTO documento_assunto 
        // 					(documentos_iddocumentos, 
        // 					estrutura_clausula_id_estruturaclausula)
        // 				VALUES
        // 					('{$lastId}', 
        // 					'{$assunto[$i]}')
        // 		";

        // 		if( !mysqli_query( $this->db, $sqlRef ) ){

        // 			$response['response_status']['status']       = 0;
        // 			$response['response_status']['error_code']   = $this->error_code . __LINE__;
        // 			$response['response_status']['msg']          = 'Erro ao cadastrar assunto!';

        // 			$this->logger->debug( $this->db->error );
        // 			$this->logger->debug( $response );
        // 		}
        // 		else{
        // 			$response['response_status']['status']       = 1;
        // 			$response['response_status']['error_code']   = $this->error_code . __LINE__;
        // 			$response['response_status']['msg']          = 'Assunto cadastrado com sucesso';
        // 		}
        // 	}
        // }

        // //ADD ATIVIDADE ECONOMICA
        // if ($data['cnae']) {
        // 	$cnaes = explode(" ", trim($data['cnae']));
        // 	foreach ($cnaes as $cnae) {
        // 		$sqlCnae = "INSERT INTO documentos_cnae
        // 					(documentos_iddocumentos,
        // 					classe_cnae_id_cnae)
        // 				VALUES (
        // 					'{$lastId}',
        // 					'{$cnae}'
        // 				)

        // 		";

        // 		if( !mysqli_query( $this->db, $sqlCnae ) ){									
        // 			$response['response_status']['status']       = 0;
        // 			$response['response_status']['error_code']   = $this->error_code . __LINE__;
        // 			$response['response_status']['msg']          = 'Erro ao cadastrar atividade economica!';

        // 			$this->logger->debug( $this->db->error );
        // 			$this->logger->debug( $response );
        // 		}
        // 		else{
        // 			$response['response_status']['status']       = 1;
        // 			$response['response_status']['error_code']   = $this->error_code . __LINE__;
        // 			$response['response_status']['msg']          = 'Atividade economica cadastrado com sucesso';
        // 		}
        // 	}
        // }

        //ADD EMPRESA
        // if ($data['empresa']) {
        // 	$empresas = explode(" ", trim($data['empresa']));

        // 	foreach ($empresas as $emp) {
        // 		$sqlEmp = "INSERT INTO documentos_empresa
        // 					(cliente_unidades_id_unidade,
        // 					documentos_id_documento)
        // 				VALUES (
        // 					'{$emp}',
        // 					'{$lastId}'
        // 				)

        // 		";

        // 		if( !mysqli_query( $this->db, $sqlEmp ) ){									
        // 			$response['response_status']['status']       = 0;
        // 			$response['response_status']['error_code']   = $this->error_code . __LINE__;
        // 			$response['response_status']['msg']          = 'Erro ao cadastrar empresa!';

        // 			$this->logger->debug( $this->db->error );
        // 			$this->logger->debug( $response );
        // 		}
        // 		else{
        // 			$response['response_status']['status']       = 1;
        // 			$response['response_status']['error_code']   = $this->error_code . __LINE__;
        // 			$response['response_status']['msg']          = 'Empresa cadastrado com sucesso';
        // 		}
        // 	}
        // }


      } else {
        $response['response_status']['status'] = 0;
        $response['response_status']['error_code'] = $this->error_code . __LINE__;
        $response['response_status']['msg'] = 'Não foi possível realizar o cadastro de abrangencia e assunto, pois o documento não foi cadastrado!';
      }
    } else {
      $response = $this->response;
    }

    $this->logger->debug($response['response_status']['status']);

    return $response;
  }

  function setUfAbrang($data = null)
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
      $this->logger->debug($data['uf-input']);

      if ($response['response_status']['status'] == 1) {

        //LISTA LOCALIZAÇÃO
        $sql = "SELECT 
							id_localizacao, 
							municipio,
							pais
						FROM 
							localizacao
						WHERE uf = '{$data['uf-input']}';				
						";

        $this->logger->debug($sql);

        if ($resultsql = mysqli_query($this->db, $sql)) {
          $this->logger->debug($resultsql);

          $list = "";
          // $update = "";
          $listaTable = [];
          while ($obj = $resultsql->fetch_object()) {
            $this->logger->debug($obj);
            $list .= '<tr class="tbl-item">';
            $list .= '<td><input class="form-check-input check" type="checkbox" value="1" id="' . $obj->id_localizacao . '"></td>';
            $list .= '<td class="title">';
            $list .= $obj->municipio;
            $list .= '</td>';
            $list .= '</tr>';

            $newObj = new stdClass();
            $newObj->campo = '<input class="form-check-input checkUpdate" type="checkbox" value="1" id="' . $obj->id_localizacao . '">';
            $newObj->municipio = $obj->municipio;
            $newObj->pais = $obj->pais;
            array_push($listaTable, $newObj);
          }


          $response['response_data']['list'] = $list;
          $response['response_data']['lista_update'] = $listaTable;

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

    $this->logger->debug($response);
    $this->logger->debug($this->db->error);

    return $response;
  }

  function deleteAbrang($data = null)
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

        //LISTA LOCALIZAÇÃO
        $sql = "DELETE FROM 
							abrang_docsind 
						WHERE 
							doc_sind_id_documento = '{$data['id_doc']}';				
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

    $this->logger->debug($response);
    $this->logger->debug($this->db->error);

    return $response;
  }

  public function verifyDocument()
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

        $today = (new DateTime('now'))->format("Y-m-d");

        $sql = "SELECT iddocumentos FROM documentos WHERE vigencia_final != '0000-00-00' AND vigencia_final <= '{$today}'";
        $result = mysqli_query($this->db, $sql);

        while ($obj = $result->fetch_object()) {
          $sqlUp = "UPDATE documentos
							SET status = 'inativo'
							WHERE iddocumentos = '{$obj->iddocumentos}'
					";

          if (!mysqli_query($this->db, $sqlUp)) {

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Não foi possível atualizar o status!';

            $this->logger->debug($sql);
            $this->logger->debug($this->db->error);
            $this->logger->debug($response);
          } else {
            $response['response_status']['status'] = 1;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Status atualizado com sucesso!';
          }

        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    $this->logger->debug($response);
    $this->logger->debug($this->db->error);

    return $response;
  }
}
