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

include __DIR__ . "/helpers.php";

class clienteunidade
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

  function getClienteUnidade($data = null)
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

        $sql = "SELECT
							cu.id_unidade AS id_unidade
							,cu.nome_unidade AS nome_unidade
							,cu.cnpj_unidade AS cnpj_unidade
							,DATE_FORMAT(cu.data_inativo,'%d/%m/%Y') AS data_inativacao
							,cu.nome_empresa as nome_empresa
							,loc.municipio
							,loc.uf
							,cu.nome_grupoeconomico as grupo_economico
							,tun.tipo_negocio
						FROM
							cliente_unidades cu
						LEFT JOIN localizacao as loc on loc.id_localizacao = cu.localizacao_id_localizacao
						LEFT JOIN tipounidade_cliente as tun on tun.id_tiponegocio = cu.tipounidade_cliente_id_tiponegocio
					";

        $this->logger->debug($sql);
        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          $selectCltUn = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdClienteUnidade( ' . $obj->id_unidade . ');" class="btn-default-alt"  id="btn_novo bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
            $html .= '<td>';
            $html .= $obj->grupo_economico; //Grupo economico
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->nome_empresa; //Empresa
            $html .= '</td>';
            $html .= '<td class="title">';
            $html .= $obj->nome_unidade; //Estabelecimento
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= formatCnpjCpf($obj->cnpj_unidade);
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->tipo_negocio;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->uf;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->municipio;
            $html .= '</td>';
            $html .= '<td>';
            $html .= !$obj->data_inativacao || $obj->data_inativacao == "0000-00-00" ? "--" : $obj->data_inativacao;
            $html .= '</td>';
            $html .= '</tr>';

            $selectCltUn .= '<option value="' . $obj->id_unidade . '">' . $obj->nome_unidade . '</option>';

          }

          $response['response_data']['listaPrincipal'] = $html;
          $response['response_data']['selectCltUn'] = $selectCltUn;
          $this->logger->debug($response['response_data']);
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

    return $response;
  }

  function getClienteUnidadeCampos($data = null)
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
							id_cnae
							,divisao_cnae
							,descricao_divisão
              ,subclasse_cnae
              ,descricao_subclasse
							,categoria
						FROM
							classe_cnae;
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td><input data-id="' . $obj->id_cnae . '" class="form-check-input" type="checkbox" onclick="addCNAE(' . $obj->id_cnae . ');" value="1" id="inicheck' . $obj->id_cnae . '"></td>';
            $html .= '<td>';
            $html .= $obj->id_cnae;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->divisao_cnae;
            $html .= '</td>';
            $html .= '<td class="title">';
            $html .= $obj->descricao_divisão;
            $html .= '</td>';
            $html .= '<td class="desc2">';
            $html .= formatCnae($obj->subclasse_cnae);
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= $obj->descricao_subclasse;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->categoria;
            $html .= '</td>';
            $html .= '</tr>';

          }

          $response['response_data']['listaCNAEini'] = $html;

        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        $sql = "SELECT
							id_empresa
						  ,nome_empresa
						FROM
							cliente_matriz;
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value=""></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_empresa;
            $grupos .= '">';
            $grupos .= $obj->nome_empresa;
            $grupos .= '</option>';
          }
          $response['response_data']['matriz'] = $grupos;

        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        $sql = "SELECT
							id_tiponegocio
						  ,tipo_negocio
						FROM
							tipounidade_cliente;
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value=""></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_tiponegocio;
            $grupos .= '">';
            $grupos .= $obj->tipo_negocio;
            $grupos .= '</option>';

          }

          $response['response_data']['negocio'] = $grupos;

        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        $sql = "SELECT
							id_localizacao
						  ,municipio
							,uf
						FROM
							localizacao;
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = '<option value=""></option>';
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="';
            $grupos .= $obj->id_localizacao;
            $grupos .= '">';
            $grupos .= $obj->municipio . ' - ' . $obj->uf;
            $grupos .= '</option>';

          }

          $response['response_data']['localizacao'] = $grupos;
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

  function getByIdClienteUnidade($data = null)
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
							cls.id_cnae,
							cu.nome_unidade,
							cu.id_unidade,
							cls.categoria,
							cls.divisao_cnae,
							cls.subclasse_cnae,
							cls.descricao_subclasse,
							cls.descricao_divisão
						FROM cliente_unidades as cu
						LEFT JOIN classe_cnae as cls on cls.id_cnae JSON_CONTAINS(cu.cnae_unidade, CONCAT('{\"id\":', cls.id_cnae ,'}'), '$')
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {
          $iduni = $data['id_unidade'];
          $tabelaCnae = [];
          $html = '';
          $multiplo = [];
          while ($obj = $resultsql->fetch_object()) {
            if (!in_array($obj->id_cnae, $multiplo)) {
              if ($obj->id_unidade == $iduni) {
                $td = '<input data-idUp="' . $obj->id_cnae . '" class="form-check-input" onclick="saveCNAEChange( ' . $iduni . ', ' . $obj->id_cnae . ');" type="checkbox" value="0" id="check' . $obj->id_cnae . '" checked>';
                $cnae = formatCnae($obj->subclasse_cnae);
                $html .= "<tr>";
                $html .= "<td>{$obj->id_cnae}</td>";
                $html .= "<td>{$obj->descricao_divisão}</td>";
                $html .= "<td>{$cnae}</td>";
                $html .= "<td>{$obj->descricao_subclasse}</td>";
                $html .= "<td>{$obj->categoria}</td>";
                $html .= "</tr>";
              } else {
                $td = '<input data-idUp="' . $obj->id_cnae . '" class="form-check-input" onclick="saveCNAEChange( ' . $iduni . ', ' . $obj->id_cnae . ');"  type="checkbox" value="1" id="check' . $obj->id_cnae . '">';
              }

              $objCnae = new stdClass();
              $objCnae->id_cnae = $obj->id_cnae;
              $objCnae->divisao_cnae = $obj->divisao_cnae;
              $objCnae->descricao_divisão = $obj->descricao_divisão;
              $objCnae->subclasse_cnae = formatCnae($obj->subclasse_cnae);
              $objCnae->descricao_subclasse = $obj->descricao_subclasse;
              $objCnae->categoria = $obj->categoria;
              $objCnae->td = $td;

              array_push($multiplo, $obj->id_cnae);
              array_push($tabelaCnae, $objCnae);
            }
          }
          $response['response_data']['listaCnaeSelecionado'] = $html;
          $response['response_data']['tabelaCnae'] = $tabelaCnae;


        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        $sql = "SELECT id_unidade
							,codigo_unidade
							,nome_unidade
							,cnpj_unidade
							,logradouro
							,bairro
							,regional
							,cep
							,DATE_FORMAT(data_inclusao ,'%d/%m/%Y')  AS data_inclusao
							,DATE_FORMAT(data_inativo ,'%d/%m/%Y')  AS data_inativo
							,cod_sindcliente
							,cod_sindpatrocliente
							,cliente_matriz_id_empresa
							,tipounidade_cliente_id_tiponegocio
							,localizacao_id_localizacao

						FROM cliente_unidades

						WHERE id_unidade = {$data['id_unidade']};
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {
          $obj = $resultsql->fetch_object();
          $response['response_data']['id_unidade'] = $obj->id_unidade;
          $response['response_data']['codigo_unidade'] = $obj->codigo_unidade;
          $response['response_data']['nome_unidade'] = $obj->nome_unidade;
          $response['response_data']['cnpj_unidade'] = formatCnpjCpf($obj->cnpj_unidade);
          $response['response_data']['endereco'] = $obj->logradouro;
          $response['response_data']['regional'] = $obj->regional;
          $response['response_data']['bairro'] = $obj->bairro;
          $response['response_data']['cep'] = $obj->cep;
          $response['response_data']['cod_sindcliente'] = $obj->cod_sindcliente;
          $response['response_data']['cod_sindpatrocliente'] = $obj->cod_sindpatrocliente;
          $response['response_data']['data_inclusao'] = $obj->data_inclusao;
          $response['response_data']['data_inativo'] = $obj->data_inativo;
          $response['response_data']['cliente_matriz_id_empresa'] = $obj->cliente_matriz_id_empresa;
          $response['response_data']['tipounidade_cliente_id_tiponegocio'] = $obj->tipounidade_cliente_id_tiponegocio;
          $response['response_data']['localizacao_id_localizacao'] = $obj->localizacao_id_localizacao;


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

  function addClienteUnidade($data = null)
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
        $dataInativa = $data['data_inativa'] == "" ? '0000-00-00' : $data['data_inativa'];

        // $cnae = "[]";
        // if ($data['cnaes-input']) {

        //   $cnaes = strpos($data['cnaes-input'], ",") ? array_filter(explode(",", $data['cnaes-input'])) : array_filter(explode(" ", $data['cnaes-input']));
        //   $array = [];
        //   foreach ($cnaes as $cnae) {
        //     $sql = "SELECT id_cnae, descricao_subclasse FROM classe_cnae WHERE id_cnae = '{$cnae}'";
        //     $resObj = mysqli_query($this->db, $sql)->fetch_object();

        //     array_push($array, '{"id":' . '"' . $resObj->id_cnae . '"}');
        //   }

        //   $lista = implode(",", $array);
        //   $cnae = "[" . $lista . "]";
        // }

        $sql = "INSERT INTO cliente_unidades
							(codigo_unidade
							,nome_unidade
							,cnpj_unidade
							,logradouro
							,bairro
							,regional
							,cep
							,cnae_unidade
							,data_inativo
							,cod_sindcliente
							,cod_sindpatrocliente
							,cliente_matriz_id_empresa
							,tipounidade_cliente_id_tiponegocio
							,localizacao_id_localizacao
              ,cnae_filial)
						VALUES(
							'{$data['codigo']}',
							'{$data['nome']}',
							'{$data['cnpj']}',
							'{$data['endereco']}',
							'{$data['bairro']}',
							'{$data['regional']}',
							'{$data['cep']}',
							'{$data['cnaes-input']}',
							-- '{$dataInativa}',
							'{$data['cod_sind_cliente']}',
							'{$data['cod_sind_patronal']}',
							'{$data['matriz']}',
							'{$data['tipo_neg']}',
							'{$data['localizacao']}',
							'{$data['cnae_filial']}'
							)
				";

        if (!mysqli_query($this->db, $sql)) {

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


  function saveCNAEChange($data = null)
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
        $today = date('d/m/Y');
        if (($data['check']) == 0) {
          $select = "SELECT json_search(cnae_unidade, 'one', '{$data['id_cnaes']}') AS path FROM cliente_unidades WHERE id_unidade = {$data['id_unidade']}";
          $result = mysqli_query($this->db, $select)->fetch_object();
          $path = str_replace('"', "", strstr($result->path, ".", true));

          $sql = "UPDATE cliente_unidades
                                SET cnae_unidade = JSON_REMOVE(cnae_unidade, '{$path}')
                            WHERE id_unidade = {$data['id_unidade']}";

          if (!mysqli_query($this->db, $sql)) {

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Erro ao excluir o registro!';

            $this->logger->debug($sql);
            $this->logger->debug($this->db->error);
            $this->logger->debug($response);
          } else {
            $this->logger->debug($sql);
            $response['response_status']['status'] = 1;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Registro excluído com sucesso!';
          }

        } else {

          $id = '{"id":' . '"' . $data['id_cnaes'] . '"}';
          $sql = "UPDATE cliente_unidades
							SET cnae_unidade = JSON_ARRAY_APPEND(cnae_unidade, '$', CAST('{$id}' as JSON))
							WHERE id_unidade = {$data['id_unidade']}

					";

          if (!mysqli_query($this->db, $sql)) {

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

  function updateClienteUnidade($data = null)
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
        $sql = " UPDATE cliente_unidades as cu
				INNER JOIN cliente_matriz as cm ON cm.id_empresa = {$data['em-input']}
										SET
											cu.codigo_unidade = '{$data['cod-input']}'
											,cu.nome_unidade = '{$data['nome-input']}'
											,cu.cnpj_unidade = '{$data['cnpj-input']}'
											,cu.logradouro = '{$data['end-input']}'
											,cu.bairro = '{$data['bairro-input']}'
											,cu.regional = '{$data['reg-input']}'
											,cu.cep = '{$data['cep-input']}'
											,cu.data_inativo = STR_TO_DATE('{$data['dataina-input']}', '%d/%m/%Y')
											,cu.cod_sindcliente = '{$data['csc-input']}'
											,cu.cod_sindpatrocliente = '{$data['csp-input']}'
											,cu.cliente_matriz_id_empresa = {$data['em-input']}
											,cu.tipounidade_cliente_id_tiponegocio = {$data['tn-input']}
											,cu.localizacao_id_localizacao = {$data['loc-input']}
												WHERE
													id_unidade = {$data['id_unidade']};
						";
        if (!mysqli_query($this->db, $sql)) {

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
}

?>