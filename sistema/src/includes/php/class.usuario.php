<?php
/**
 * @author    {Lucas A. R. Volpati}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
    2022-12-21 14:53 ( v1.0.0 ) - 
  }
**/

// 

class usuario
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

      include_once ($fileLogApi);

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

        include_once ($fileGetConfig);

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

          include_once ($this->path . '/includes/php/db.mysql.php');

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



  function validateUser($data = null)
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
						id_user,
						nome_usuario,
						tipo,
						nivel,
						modulos_sisap,
						modulos_comercial,
						id_grupoecon,
						ifnull(cg.nome_grupoeconomico,\"Ineditta\") as nome_gp,
						is_blocked,
						ausencia_inicio,
						ausencia_fim
					FROM usuario_adm LEFT JOIN cliente_grupo as cg ON cg.id_grupo_economico = id_grupoecon
						WHERE email_usuario = '{$data}'
				";


        $this->logger->debug($sql);

        if ($result = mysqli_query($this->db, $sql)) {
          $obj = $result->fetch_object();

          $response['response_data']['user'] = $obj;
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

  function validateUserAPI($data = null)
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
							id_user,
							nome_usuario,
							tipo,
							nivel,
							modulos_sisap,
							modulos_comercial,
							id_grupoecon,
							is_blocked,
							ausencia_inicio,
							ids_fmge,
							ids_gruc,
							ids_matrizes,
							nivel,
							ausencia_fim
						FROM usuario_adm
						WHERE email_usuario = '{$data['email']}'
				";

        $this->logger->debug($sql);

        if ($result = mysqli_query($this->db, $sql)) {
          $obj = $result->fetch_object();

          $response['response_data']['user'] = $obj;

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

  function validateModulos($data = null)
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

        $moduloComercial = $data['comercial'];
        $moduloSisap = $data['sisap'];
        $this->logger->debug($data);


        //MODULOS COMERCIAIS
        if (count($moduloComercial) > 0) {
          $lista = [];
          $listaUri = [];
          foreach ($moduloComercial as $modulos) {
            $sql = "SELECT
									id_modulos,
									modulos,
									tipo,
									uri
									

								FROM modulos
								WHERE id_modulos = '{$modulos->id}' AND ({$modulos->Criar} != 0
								OR {$modulos->Consultar} != 0
								OR {$modulos->Comentar} != 0
								OR {$modulos->Alterar} != 0
								OR {$modulos->Excluir} != 0
								OR {$modulos->Aprovar} != 0);
						";

            $result = mysqli_query($this->db, $sql);

            if ($result->num_rows > 0) {
              $obj = $result->fetch_object();
              array_push($lista, $obj->modulos);
              array_push($listaUri, "/desenvolvimento/" . $obj->uri);
            }
          }

          $response['response_data']['comercial_permitido'] = $lista;
          $response['response_data']['comercial_uri'] = $listaUri;


          $this->logger->debug($lista);
        }

        //MODULOS SISAP
        if (count($moduloSisap) > 0) {

          $listaSisap = [];
          $listaSisapUri = [];
          $listaComId = [];
          foreach ($moduloSisap as $modulos) {
            $sqlSisap = "SELECT
									id_modulos,
									modulos,
									tipo,
									uri

								FROM modulos
								WHERE id_modulos = '{$modulos->id}' AND ({$modulos->Criar} != 0
								OR {$modulos->Consultar} != 0
								OR {$modulos->Comentar} != 0
								OR {$modulos->Alterar} != 0
								OR {$modulos->Excluir} != 0
								OR {$modulos->Aprovar} != 0);
						";

            $resultSisap = mysqli_query($this->db, $sqlSisap);

            if ($resultSisap->num_rows > 0) {
              $objSisap = $resultSisap->fetch_object();

              array_push($listaSisap, $objSisap->modulos);
              array_push($listaSisapUri, "/desenvolvimento/" . $objSisap->uri);
              array_push($listaComId, $objSisap->id_modulos . "+" . $objSisap->modulos);
            }
          }

          $response['response_data']['sisap_permitido'] = $listaSisap;
          $response['response_data']['sisap_uri'] = $listaSisapUri;
          $response['response_data']['comercial_modulos_id'] = $listaComId;
          $this->logger->debug($listaSisap);
          $this->logger->debug("WWWWWWWWWWWWWW");
          $this->logger->debug($listaComId);

        }



      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    // $this->logger->debug( $response['response_data'] );

    return $response;
  }

  function getUserData($data = null)
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
							id_user,
							nome_usuario,
							ids_fmge,
							ids_gruc,
							ids_matrizes,
							id_grupoecon,
							nivel
						FROM usuario_adm
						WHERE email_usuario = '{$data}'
				";

        $this->logger->debug($sql);

        if ($result = mysqli_query($this->db, $sql)) {
          $obj = $result->fetch_object();

          $response['response_data']['user_data'] = $obj;

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

  function verificaAnuencia($email)
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
						FROM anuencia_inicial
						WHERE usuario_adm_email = '{$email}'
				";

        $this->logger->debug($sql);

        $result = mysqli_query($this->db, $sql);

        $obj = $result->fetch_object();

        $response['response_data']['anuencia'] = $obj ? "existente" : "inexistente";


      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    $this->logger->debug($response['response_data']);

    return $response;
  }

  function acceptTerms($data)
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

        if ($data['user']) {

          $sql = "INSERT INTO anuencia_inicial
								(usuario_adm_email)
							VALUES (
								'{$data['user']}'
							)
					";

          $this->logger->debug($sql);

          if (!mysqli_query($this->db, $sql)) {

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Erro ao realizar cadastro de anuencia';

            $this->logger->debug($sql);
            $this->logger->debug($this->db->error);
            $this->logger->debug($response);
          } else {
            $response['response_status']['status'] = 1;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Anuencia cadastrada com sucesso';
          }

          $user = $this->validateUser($data['user'])['response_data']['user'];

          $path = "perfil_grupo_economico.php";

          $response['response_data']['path'] = $path;


        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Email não informado';
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
}