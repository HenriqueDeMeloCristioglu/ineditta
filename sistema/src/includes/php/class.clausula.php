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

ini_set("memory_limit", "800M");
ini_set("max_execution_time", "800");

include_once __DIR__ . "/class.usuario.php";
include_once __DIR__ . "/class.disparoEmail.php";

class clausula
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

  function getLists($data = null)
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
							est.id_estruturaclausula,
							est.nome_clausula,
							sn.nome_sinonimo,
							sn.id_sinonimo
						FROM estrutura_clausula as est
						LEFT JOIN sinonimos as sn on sn.estrutura_clausula_id_estruturaclausula = est.id_estruturaclausula
						GROUP BY est.id_estruturaclausula		
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {
          $html = null;

          $response['response_data']['clausulas'] = $html;
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        //ESTRUTURA CLAUSULA OPTIONS
        $sql = "SELECT 
							id_estruturaclausula
							,nome_clausula
						FROM 
							estrutura_clausula;
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {
          $opt0 = null;
          while ($obj = $resultsql->fetch_object()) {
            $opt0 .= '<option value="';
            $opt0 .= $obj->id_estruturaclausula;
            $opt0 .= '">';
            $opt0 .= $obj->nome_clausula;
            $opt0 .= '</option>';
          }

          $response['response_data']['optEstClausula'] = $opt0;
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        //ASSUNTO OPTIONS
        $sql = "SELECT 
							idassunto
							,assunto
						FROM 
							assunto;									
				";

        $resultsql = mysqli_query($this->db, $sql);

        $optAss = null;
        while ($obj = $resultsql->fetch_object()) {
          $optAss .= '<option value="';
          $optAss .= $obj->idassunto;
          $optAss .= '">';
          $optAss .= $obj->assunto;
          $optAss .= '</option>';
        }

        $response['response_data']['assunto'] = $optAss;

        //LISTA DOC SIND
        $sql2 = "SELECT 
							ds.id_doc
							,tp.nome_doc
							,ds.validade_final
							,ds.data_assinatura
							,ds.data_reg_mtes
						FROM 
							doc_sind as ds
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = ds.tipo_doc_idtipo_doc
						";

        if ($resultsql = mysqli_query($this->db, $sql2)) {
          $html = null;
          $update = null;

          while ($obj = $resultsql->fetch_object()) {

            $html .= "<option value='{$obj->id_doc}'>Id: {$obj->id_doc} - {$obj->nome_doc}</option>";

            $update .= '<tr class="odd gradeX">';
            $update .= '<td><button onclick="selectDocSind( ' . $obj->id_doc . ');" data-toggle="modal" href="#myModalUpdate" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
            $update .= '<td>';
            $update .= $obj->id_doc;
            $update .= '</td>';
            $update .= '<td>';
            $update .= $obj->nome_doc;
            $update .= '</td>';
            $update .= '<td>';
            $update .= $obj->validade_final;
            $update .= '</td>';
            $update .= '<td id="date">';
            $update .= $obj->data_assinatura;
            $update .= '</td>';
            $update .= '<td id="date">';
            $update .= $obj->data_reg_mte;
            $update .= '</td>';
            $update .= '</tr>';
          }

          $response['response_data']['docsind'] = $html;
          $response['response_data']['docsindUpdate'] = $update;
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        //CLAUSULA GERAL LIST
        $sql4 = "SELECT 
							IFNULL(GROUP_CONCAT(cg.id_clau), GROUP_CONCAT(IFNULL( cg.id_clau,null))) as id_clau
							,cg.doc_sind_id_documento
							,(SELECT count(data_processamento) FROM clausula_geral  WHERE data_processamento IS NOT NULL AND data_processamento != '0000-00-00' AND cg.doc_sind_id_documento = doc_sind_id_documento) as processadas
							,(SELECT count(*) FROM clausula_geral  WHERE (data_processamento is null OR data_processamento = '0000-00-00') AND cg.doc_sind_id_documento = doc_sind_id_documento) as nao_processadas
							,(SELECT count(data_aprovacao) FROM clausula_geral  WHERE data_aprovacao IS NOT NULL AND data_aprovacao != '0000-00-00' AND cg.doc_sind_id_documento = doc_sind_id_documento) as aprovado
							,(SELECT count(*) FROM clausula_geral  WHERE (data_aprovacao is null OR data_aprovacao = '0000-00-00') AND cg.doc_sind_id_documento = doc_sind_id_documento) as nao_aprovado
							,tp.nome_doc
							,ds.data_scrap
							,ds.data_aprovacao
							
						FROM	clausula_geral as cg 
						LEFT JOIN doc_sind as ds on ds.id_doc = cg.doc_sind_id_documento
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = ds.tipo_doc_idtipo_doc
						GROUP BY cg.doc_sind_id_documento
				";
        $resultsql = mysqli_query($this->db, $sql4);
        $result2 = mysqli_query($this->db, $sql4);

        if ($resultsql) {
          // if ($resultsql->num_rows >= 1) {
          while ($obj = $resultsql->fetch_object()) {
            //OBTENDO LISTA DE CLIENTE UNIDADES AFETADOS
            $sqlUn = "SELECT 
										*
									FROM doc_sind_cliente_unidades
									WHERE doc_sind_id_doc = '{$obj->doc_sind_id_documento}'
							";
            $lista[$obj->doc_sind_id_documento] = [];

            if ($resultUn = mysqli_query($this->db, $sqlUn)) {
              while ($objUn = $resultUn->fetch_object()) {

                array_push($lista[$obj->doc_sind_id_documento], $objUn->cliente_unidades_id_unidade);
              }
            }
          }
          if (!empty(array_filter($lista))) {
            //OBTENDO LISTA DE MATRIZES AFETADAS
            $listaMatrizes = [];
            foreach ($lista as $key => $value) {
              for ($o = 0; $o < count($lista[$key]); $o++) {
                $sql = "SELECT
												*
											FROM cliente_unidades
											WHERE id_unidade = '{$value[$o]}'
									";

                $result = mysqli_query($this->db, $sql);
                $objMatriz = $result->fetch_object();
                array_push($listaMatrizes, $objMatriz->cliente_matriz_id_empresa);
              }
            }

            $listaMatrizesAfetadas = explode(",", implode(",", array_unique(array_filter($listaMatrizes))));

            //OBTENDO LISTA DE SLA ENTREGA DE CLIENTE MATRIZ
            $listaSla = [];
            for ($i = 0; $i < count($listaMatrizesAfetadas); $i++) {
              $sql = "SELECT 
											*
										FROM cliente_matriz
										WHERE id_empresa = '{$listaMatrizesAfetadas[$i]}'
								";

              $result = mysqli_query($this->db, $sql);

              $objSla = $result->fetch_object();

              array_push($listaSla, $objSla->sla_entrega);
            }
            $periodo = min($listaSla);
          }

          $html = "";
          while ($obj2 = $result2->fetch_object()) {
            $dataSla = "";
            if ($periodo) {
              $dataSla = ($obj2->data_aprovacao == "" ? '--' : date_format((new DateTime($obj2->data_aprovacao))->add(new DateInterval("P{$periodo}D")), 'd/m/Y'));
            }


            $today = new DateTimeImmutable('now');
            $lastDate = (!$obj2->data_scrap ? '--' : (new DateTimeImmutable($obj2->data_scrap)));

            $diasEmProcess = ($lastDate == '--' ? '--' : $today->diff($lastDate)->days);


            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td><a data-toggle="modal" href="#myModalListClausulasByDocSind" onclick="getClausulaByDoc(' . $obj2->doc_sind_id_documento . ');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>'; //onclick="getByIdClausula( '.$obj->doc_sind_id_documento.');" 
            $html .= '<td class="title">';
            $html .= $obj2->doc_sind_id_documento;
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= $obj2->nome_doc;
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= $obj2->processadas;
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= $obj2->nao_processadas;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj2->aprovado;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj2->nao_aprovado;
            $html .= '</td>';
            $html .= '<td>';
            $html .= ($obj2->data_scrap == "" ? '--' : date_format((new DateTime($obj2->data_scrap)), 'd/m/Y'));
            $html .= '</td>';
            $html .= '<td>';
            $html .= ($dataSla == "" ? '--' : $dataSla);
            $html .= '</td>';
            $html .= '<td>';
            $html .= $diasEmProcess;
            $html .= '</td>';
            $html .= '</tr>';

          }

          $response['response_data']['listaPrincipal'] = $html;

        }


        //LISTA DOCUMENTOS APROVADOS
        $sql = "SELECT 
							ds.id_doc,
							tp.nome_doc,
							date_format(ds.data_aprovacao, '%d/%m/%Y') as data_aprovacao,
							ds.sla_entrega,
							ds.data_scrap,
							ds.scrap_aprovado,
							ds.usuario_responsavel,
							dl.caminho
						FROM doc_sind as ds 
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = ds.tipo_doc_idtipo_doc
						LEFT JOIN documentos_localizados as dl on dl.id_documento = ds.documento_id_documento
						WHERE ds.data_aprovacao != '0000-00-00' AND ds.data_aprovacao is not null
				";

        $resultsql = mysqli_query($this->db, $sql);

        if ($resultsql->fetch_object()) {

          $html = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td class="title">';
            $html .= $obj->id_doc;
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= $obj->nome_doc;
            $html .= '</td>';
            $html .= '<td>';
            $html .= ($obj->data_aprovacao == '00/00/0000' ? '--' : $obj->data_aprovacao);
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= ($obj->sla_entrega == "" ? '--' : $obj->sla_entrega);
            $html .= '</td>';
            $html .= '<td class="desc" style="display: flex; justify-content: center;">';
            $html .= '<a id="rodar_scrap" onclick="setPdf(\'' . $obj->caminho . '\', ' . $obj->id_doc . ')" data-toggle="modal" href="#scrap" data-dismiss="modal" class="btn btn-primary">SCRAP</a>';
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= ($obj->data_scrap == '' ? '--' : $obj->data_scrap);
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= ($obj->scrap_aprovado == '' ? '--' : $obj->scrap_aprovado);
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= ($obj->usuario_responsavel == '' ? '--' : $obj->usuario_responsavel);
            $html .= '</td>';
            $html .= '</tr>';
          }

          $response['response_data']['listaDocsAprovados'] = $html;
        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }


    return $response;
  }


  function getByDocClausula($data = null)
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
							cg.id_clau as id_clau
							,cg.doc_sind_id_documento
							,gp.nome_grupo
							,est.nome_clausula
							,date_format(cg.data_processamento, '%d/%m/%Y') as data_processamento
							,cg.tex_clau as tex_clau
							,usa.nome_usuario as aprovador
							,date_format(cg.data_aprovacao, '%d/%m/%Y') as data_aprovacaos
							,doc.data_scrap,
							us.nome_usuario as resp_processamento
						FROM clausula_geral as cg 
						
						LEFT JOIN clausula_geral_estrutura_clausula as ad on cg.id_clau = ad.clausula_geral_id_clau
						LEFT JOIN estrutura_clausula as est on cg.estrutura_id_estruturaclausula = est.id_estruturaclausula
						LEFT JOIN grupo_clausula as gp on gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
						LEFT JOIN doc_sind as doc on doc.id_doc = cg.doc_sind_id_documento
						LEFT JOIN usuario_adm as us on us.id_user = cg.responsavel_processamento
						LEFT JOIN usuario_adm as usa on usa.id_user = cg.aprovador
						WHERE cg.doc_sind_id_documento = '{$data['id_doc']}'
						GROUP BY cg.id_clau;	
				";


        if ($resultsql = mysqli_query($this->db, $sql)) {


          // $listClausula = [];
          $html = "";
          $teste = '';
          while ($obj = $resultsql->fetch_object()) {


            $html .= "<tr class='tbl-item'>";
            $html .= $obj->data_scrap && $obj->data_processamento == "00/00/0000" ? "<td><a data-toggle='modal' data-dismiss='modal' href='#myModalUpdate' data-origem='scrap' onclick='getByIdClausula(1,{$obj->id_clau});' class='btn-default-alt'  id='bootbox-demo-5'><i class='fa fa-file-text'></i></a></td>" : "<td><a data-toggle='modal' data-dismiss='modal' href='#myModalUpdate' data-origem='sistema' onclick='getByIdClausula(2,{$obj->id_clau});' class='btn-default-alt'  id='bootbox-demo-5'><i class='fa fa-file-text'></i></a></td>";
            $html .= "<td>{$obj->doc_sind_id_documento}</td>";
            $html .= "<td class='title'>{$obj->nome_grupo}</td>";
            $html .= "<td class='desc'>{$obj->nome_clausula}</td>";
            $html .= "<td>" . mb_substr($obj->tex_clau, 0, 200) . " ...</td>";
            $html .= "<td>" . ($obj->data_processamento == "00/00/0000" ? "--" : $obj->data_processamento) . "</td>";
            $html .= "<td>" . (!$obj->resp_processamento ? "--" : $obj->resp_processamento) . "</td>";
            $html .= "<td>" . (!$obj->data_aprovacao ? "--" : $obj->data_aprovacao) . "</td>";
            $html .= "<td>" . (!$obj->aprovador ? "--" : $obj->aprovador) . "</td>";
            $html .= "</tr>";
          }

          // $response['response_data']['lista_clausulas'] = $listClausula;
          $response['response_data']['lista_clausulas_atribuidas'] = $html;


          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Query realizada com sucesso.';
        } else {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'A query não retornou dados, verificar!';
        }
      } else {

        $response['response_status']['status'] = 0;
        $response['response_status']['error_code'] = $this->error_code . __LINE__;
        $response['response_status']['msg'] = 'Não foi possível realizar a busca no banco de dados';
      }
    } else {
      $response['response_status']['status'] = 0;
      $response['response_status']['error_code'] = $this->error_code . __LINE__;
      $response['response_status']['msg'] = 'Não foi possível iniciar a requisição getByDocClausula';
    }


    return $response;
  }



  function getByIdClausula($data = null)
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
							cg.id_clau,
							cg.tex_clau,
							est.id_estruturaclausula, 
							est.nome_clausula as clausula,
							cg.doc_sind_id_documento,
							date_format(cg.data_aprovacao, '%d/%m/%Y') as data_aprovacao,
							cg.aprovador,
							tp.nome_doc,
							ds.validade_inicial,
							ds.validade_final,
							ds.id_doc,
							cg.numero_clausula,
							cg.assunto_idassunto
						FROM
							clausula_geral as cg
						LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = estrutura_id_estruturaclausula
						LEFT JOIN doc_sind as ds on ds.id_doc = doc_sind_id_documento
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = ds.tipo_doc_idtipo_doc
						WHERE id_clau = '{$data['id_clau']}'			
				";



        if ($resultsql = mysqli_query($this->db, $sql)) {
          $obj = $resultsql->fetch_object();

          $response['response_data']['id_clau'] = $obj->id_clau;
          $response['response_data']['id_doc'] = $obj->id_doc;
          $response['response_data']['estrutura'] = $obj->clausula;
          $response['response_data']['id_estrutura'] = $obj->id_estruturaclausula;
          $response['response_data']['doc_sind'] = $obj->doc_sind_id_documento;
          $response['response_data']['tex_clau'] = $obj->tex_clau;
          $response['response_data']['data_aprovacao'] = $obj->data_aprovacao;
          $response['response_data']['aprovador'] = $obj->aprovador;
          $response['response_data']['nome_doc'] = $obj->nome_doc;
          $response['response_data']['valIni'] = $obj->validade_inicial;
          $response['response_data']['valFinal'] = $obj->validade_final;
          $response['response_data']['numero_clausula'] = $obj->numero_clausula;
          $response['response_data']['assunto'] = $obj->assunto_idassunto;

        } else {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        $idClausulaGeral = $data['id_clau'];
        //LISTA TITULO E CAMPO INFORMACAO ADICIONAL
        $sql5 = "SELECT 
							cg.id_clau as id_clau
							,cg.doc_sind_id_documento
							,cg.tex_clau as tex_clau,	
							IFNULL(GROUP_CONCAT(DISTINCT ad.id_clausulageral_estrutura_clausula), GROUP_CONCAT(IFNULL( ad.id_clausulageral_estrutura_clausula, null))) as id_cg_est,
							IFNULL(GROUP_CONCAT(DISTINCT est.id_estruturaclausula), GROUP_CONCAT(IFNULL( est.id_estruturaclausula, null))) as id_estruturaclausula,
							IFNULL(GROUP_CONCAT(DISTINCT est.nome_clausula), GROUP_CONCAT(IFNULL( est.nome_clausula, null))) as nome_clausula,
							IFNULL(GROUP_CONCAT(DISTINCT info.cdtipoinformacaoadicional), GROUP_CONCAT(IFNULL( info.cdtipoinformacaoadicional, null))) as id_informacaoadicional,
							IFNULL(GROUP_CONCAT(DISTINCT ad.id_info_tipo_grupo), GROUP_CONCAT(IFNULL( ad.id_info_tipo_grupo, null))) as id_info_tipo_grupo,
							IFNULL(GROUP_CONCAT(DISTINCT ad.texto), GROUP_CONCAT(IFNULL( ad.texto, null))) as texto,
                            IFNULL(GROUP_CONCAT(DISTINCT ad.numerico), GROUP_CONCAT(IFNULL( ad.numerico, null))) as numerico,
                            IFNULL(GROUP_CONCAT(DISTINCT ad.descricao), GROUP_CONCAT(IFNULL( ad.descricao, null))) as descricao,
                            IFNULL(GROUP_CONCAT(DISTINCT ad.data), GROUP_CONCAT(IFNULL( ad.data, null))) as data_val,
							IFNULL(GROUP_CONCAT(DISTINCT ad.hora), GROUP_CONCAT(IFNULL( ad.hora, null))) as hora,
							IFNULL(GROUP_CONCAT(DISTINCT ad.percentual), GROUP_CONCAT(IFNULL( ad.percentual, null))) as percentual,
							info.idtipodado
						FROM	clausula_geral as cg 
						LEFT JOIN clausula_geral_estrutura_clausula as ad on cg.id_clau = ad.clausula_geral_id_clau
						LEFT JOIN estrutura_clausula as est on cg.estrutura_id_estruturaclausula = est.id_estruturaclausula
						LEFT JOIN doc_sind as ds on ds.id_doc = cg.doc_sind_id_documento
						LEFT JOIN estrutura_clausulas_ad_tipoinformacaoadicional as esi on esi.estrutura_clausula_id_estruturaclausula = est.id_estruturaclausula
						LEFT JOIN ad_tipoinformacaoadicional as info on info.cdtipoinformacaoadicional = esi.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
						WHERE cg.id_clau = '{$data['id_clau']}'
						GROUP BY cg.id_clau;
				";



        if ($resultsql5 = mysqli_query($this->db, $sql5)) {

          $htmlUpdate = '';

          $classificacao = "";
          $idClassificacao = "";
          while ($obj5 = $resultsql5->fetch_object()) {

            $idTableRelacao = explode(",", $obj5->id_cg_est);



            $idInfoList = explode(",", $obj5->id_informacaoadicional);





            if ($obj5->id_informacaoadicional != "") {
              $countOption = 1;
              for ($i = 0; $i < count($idTableRelacao); $i++) {

                $sqlRelacao = "SELECT * FROM clausula_geral_estrutura_clausula WHERE id_clausulageral_estrutura_clausula = '{$idTableRelacao[$i]}' AND id_info_tipo_grupo is null";
                $resultRel = mysqli_query($this->db, $sqlRelacao);
                $objRelacao = $resultRel->fetch_object();




                if ($objRelacao->ad_tipoinformacaoadicional_cdtipoinformacaoadicional) { //
                  //sql dados info adicional
                  $sql6 = "SELECT
												cdtipoinformacaoadicional
												,nmtipoinformacaoadicional
												,idtipodado
											FROM
												ad_tipoinformacaoadicional
											WHERE
												cdtipoinformacaoadicional = $objRelacao->ad_tipoinformacaoadicional_cdtipoinformacaoadicional;
									";

                  $resultSql6 = mysqli_query($this->db, $sql6);
                  $obj6 = $resultSql6->fetch_object();

                  $htmlUpdate .= '<tr>';
                  $htmlUpdate .= '<td class="tableCg" id="' . $objRelacao->id_clausulageral_estrutura_clausula . '">';
                  $htmlUpdate .= $obj6->nmtipoinformacaoadicional;
                  $htmlUpdate .= '</td>';
                  $htmlUpdate .= '<td>';




                  if ($obj6->idtipodado == "D") {
                    $data = $objRelacao->data == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($objRelacao->data));
                    $htmlUpdate .= '<input type="text" class="form-control info-adicionalUpdate date-input" id="data" value="' . $data . '">';
                  } elseif ($obj6->idtipodado == "N") {
                    $htmlUpdate .= '<input type="number" class="form-control info-adicionalUpdate number-input" id="numerico" value="' . $objRelacao->numerico . '">';
                  } elseif ($obj6->idtipodado == "T") {
                    $htmlUpdate .= '<input type="text" class="form-control info-adicionalUpdate" id="texto" value="' . $objRelacao->texto . '">';
                  } elseif ($obj6->idtipodado == "C") {

                    $sql2 = "SELECT 
													options 
												FROM informacao_adicional_combo 
												WHERE ad_tipoinformacaoadicional_id = '{$obj6->cdtipoinformacaoadicional}'";

                    $resultSql2 = mysqli_query($this->db, $sql2);
                    $obj2 = $resultSql2->fetch_object();



                    $optionsSelect = explode(", ", $obj2->options);
                    $options = "";

                    for ($o = 0; $o < count($optionsSelect); $o++) {

                      if ($objRelacao->combo == $optionsSelect[$o]) {
                        $options .= '<option class="optionUpdate' . $countOption . '" selected>' . $optionsSelect[$o] . '</option>';
                      } else {
                        $options .= '<option class="optionUpdate' . $countOption . '">' . $optionsSelect[$o] . '</option>';
                      }
                    }

                    $htmlUpdate .= '<select class="form-control info-adicionalUpdate combo" id="' . $obj6->cdtipoinformacaoadicional . '" value="' . $objRelacao->combo . '">';

                    $htmlUpdate .= $options;

                    $htmlUpdate .= '</select>';

                    $countOption++;
                  } elseif ($obj6->idtipodado == "CM") {
                    $sql2 = "SELECT 
													options 
												FROM informacao_adicional_combo 
												WHERE ad_tipoinformacaoadicional_id = '{$obj6->cdtipoinformacaoadicional}'";

                    $resultSql2 = mysqli_query($this->db, $sql2);
                    $obj2 = $resultSql2->fetch_object();



                    $optionsSelect = explode(", ", $obj2->options);
                    $options = "";
                    $multipleOptions = explode(",", $objRelacao->combo);
                    $count = 0;


                    for ($m = 0; $m <= count($multipleOptions); $m++) {
                      while ($count < count($optionsSelect)) {
                        if ($multipleOptions[$m] == $optionsSelect[$count]) {
                          $options .= '<option class="optionUpdate' . $countOption . '" selected="selected">' . $optionsSelect[$count] . '</option>';
                          $count++;
                          // break;
                          if (count($multipleOptions) >= 2)
                            break;
                        } else {
                          $options .= '<option class="optionUpdate' . $countOption . '">' . $optionsSelect[$count] . '</option>';
                          $count++;
                        }
                      }
                    }

                    $htmlUpdate .= '<select multiple data-placeholder="Selecione" tabindex="8" style="width: 200px;" class="form-control chzn-select info-adicionalUpdate combo-multiplo" id="' . $obj6->cdtipoinformacaoadicional . '" value="' . $objRelacao->combo . '">';

                    $htmlUpdate .= $options;

                    $htmlUpdate .= '</select>';

                    $countOption++;
                  } elseif ($obj6->idtipodado == "V") {
                    $htmlUpdate .= '<input type="text" class="form-control info-adicionalUpdate valor-input" id="numerico" value="' . $objRelacao->numerico . '">';
                  } elseif ($obj6->idtipodado == "H") {
                    $htmlUpdate .= '<input type="text" class="form-control info-adicionalUpdate hora-input" id="hora" value="' . $objRelacao->hora . '">';
                  } elseif ($obj6->idtipodado == "L") {
                    $percent = number_format($objRelacao->percentual, 2);
                    $htmlUpdate .= '<input type="text" class="form-control info-adicionalUpdate percent-input" id="percentual" value="' . $percent . '">';
                  } else {
                    $htmlUpdate .= '<textarea class="form-control info-adicionalUpdate" id="descricao">' . $objRelacao->descricao . '</textarea>';
                  }

                  $htmlUpdate .= '</td>';
                  $htmlUpdate .= '</tr>';
                }

                if ($obj5->id_info_tipo_grupo) { //|| $obj5->idtipodado == "G"

                  $tipoGrupo = 'G';

                  $sqlRelacao = "SELECT * FROM clausula_geral_estrutura_clausula WHERE id_clausulageral_estrutura_clausula = '{$idTableRelacao[$i]}'";
                  $resultRel = mysqli_query($this->db, $sqlRelacao);
                  $objRelacao = $resultRel->fetch_object();


                  //sql dados info adicional
                  $sql6 = "SELECT
												cdtipoinformacaoadicional
												,nmtipoinformacaoadicional
												,idtipodado
											FROM
												ad_tipoinformacaoadicional
											WHERE
												cdtipoinformacaoadicional = $objRelacao->ad_tipoinformacaoadicional_cdtipoinformacaoadicional;
									";

                  $resultSql6 = mysqli_query($this->db, $sql6);
                  $obj6 = $resultSql6->fetch_object();

                  //Obter nome do grupo
                  $sql7 = "SELECT
												nmtipoinformacaoadicional
											FROM
												ad_tipoinformacaoadicional
											WHERE
												cdtipoinformacaoadicional = $objRelacao->id_info_tipo_grupo;
									";
                  $resultSql7 = mysqli_query($this->db, $sql7);
                  $obj7 = $resultSql7->fetch_object();

                  $nomeGrupo = $obj7->nmtipoinformacaoadicional;

                  $sql = "SELECT 
												clausula_geral_id_clau,
												id_info_tipo_grupo,
												ad_tipoinformacaoadicional_cdtipoinformacaoadicional,
												sequencia,
												texto,
												numerico,
												date_format(data, '%d/%m/%Y') as data,
												hora,
												descricao,
												percentual,
												combo
											FROM clausula_geral_estrutura_clausula
											where id_info_tipo_grupo = '{$objRelacao->id_info_tipo_grupo}' AND clausula_geral_id_clau = '{$idClausulaGeral}'; 
									";



                  $resultSql = mysqli_query($this->db, $sql);

                  $grupo = [];

                  //Gerando títulos <th>
                  $th = "";
                  $sqlTotalInfo = "SELECT
														IFNULL(GROUP_CONCAT(DISTINCT ad_tipoinformacaoadicional_cdtipoinformacaoadicional), GROUP_CONCAT(IFNULL( ad_tipoinformacaoadicional_cdtipoinformacaoadicional, null))) as id_infoadicional
													FROM clausula_geral_estrutura_clausula
													WHERE id_info_tipo_grupo = '{$objRelacao->id_info_tipo_grupo}' AND clausula_geral_id_clau = '{$idClausulaGeral}';
									";
                  $resultTotalInfo = mysqli_query($this->db, $sqlTotalInfo);
                  $objTotal = $resultTotalInfo->fetch_object();
                  $totalInformacoes = explode(",", $objTotal->id_infoadicional);


                  $qtd = count($totalInformacoes) + 1;
                  $sql1 = "SELECT 
													ad_tipoinformacaoadicional_cdtipoinformacaoadicional,
													sequencia,
													ad.idtipodado,
													ad.nmtipoinformacaoadicional
												FROM clausula_geral_estrutura_clausula 
												LEFT JOIN ad_tipoinformacaoadicional as ad on ad.cdtipoinformacaoadicional = ad_tipoinformacaoadicional_cdtipoinformacaoadicional
												WHERE sequencia < '{$qtd}' AND clausula_geral_id_clau = '{$idClausulaGeral}'
									";
                  $resultSql1 = mysqli_query($this->db, $sql1);

                  while ($obj1 = $resultSql1->fetch_object()) {
                    $th .= '<th class="title-group-table ' . $obj1->idtipodado . '">' . $obj1->nmtipoinformacaoadicional . '</th>';
                  }


                  $countOption = 1;
                  while ($obj = $resultSql->fetch_object()) {

                    $sql1 = "SELECT cdtipoinformacaoadicional, nmtipoinformacaoadicional, idtipodado FROM ad_tipoinformacaoadicional WHERE cdtipoinformacaoadicional = '{$obj->ad_tipoinformacaoadicional_cdtipoinformacaoadicional}'";
                    $resultSql1 = mysqli_query($this->db, $sql1);

                    $obj1 = $resultSql1->fetch_object();


                    if ($obj1->idtipodado == "D") {
                      $input = '<td><input type="text" class="form-control info-adicional-grupo-update date-input sequencia' . $countOption . '" id="' . $obj1->cdtipoinformacaoadicional . '?' . $objRelacao->id_info_tipo_grupo . '*data" value="' . $obj->data . '"></td>'; //'.$obj1->cdtipoinformacaoadicional.'?'.$obj6->cdtipoinformacaoadicional.'
                      array_push($grupo, $input);
                    } elseif ($obj1->idtipodado == "N") {
                      $input = '<td><input type="number" class="form-control info-adicional-grupo-update sequencia' . $countOption . '" id="' . $obj1->cdtipoinformacaoadicional . '?' . $objRelacao->id_info_tipo_grupo . '*numerico" value="' . $obj->numerico . '"></td>';
                      array_push($grupo, $input);
                    } elseif ($obj1->idtipodado == "T") {
                      $input = '<td><input type="text" class="form-control info-adicional-grupo-update sequencia' . $countOption . '" id="' . $obj1->cdtipoinformacaoadicional . '?' . $objRelacao->id_info_tipo_grupo . '*texto" value="' . $obj->texto . '"></td>';
                      array_push($grupo, $input);
                    } elseif ($obj1->idtipodado == "C") {

                      $sql2 = "SELECT 
														options 
													FROM informacao_adicional_combo 
													WHERE ad_tipoinformacaoadicional_id = '{$obj1->cdtipoinformacaoadicional}'";

                      $resultSql2 = mysqli_query($this->db, $sql2);
                      $obj2 = $resultSql2->fetch_object();


                      $optionsSelect = explode(", ", $obj2->options);
                      $options = "";
                      $input = "";

                      for ($l = 0; $l < count($optionsSelect); $l++) {

                        if ($obj->combo == $optionsSelect[$l]) {
                          $options .= '<option class="optionUpdate' . $countOption . '" selected>' . $optionsSelect[$l] . '</option>';
                        } else {
                          $options .= '<option class="optionUpdate' . $countOption . '">' . $optionsSelect[$l] . '</option>';
                        }
                      }

                      $input .= '<td><select class="form-control info-adicional-grupo-update combo sequencia' . $countOption . '" id="' . $obj1->cdtipoinformacaoadicional . '?' . $objRelacao->id_info_tipo_grupo . '*combo" value="' . $obj->combo . '">';

                      $input .= $options;

                      $input .= '</select></td>';
                      array_push($grupo, $input);
                    } elseif ($obj1->idtipodado == "CM") {

                      $sql2 = "SELECT 
														options 
													FROM informacao_adicional_combo 
													WHERE ad_tipoinformacaoadicional_id = '{$obj1->cdtipoinformacaoadicional}'";

                      $resultSql2 = mysqli_query($this->db, $sql2);
                      $obj2 = $resultSql2->fetch_object();


                      $optionsSelect = explode(", ", $obj2->options);
                      $options = "";
                      $input = "";
                      $multipleOptions = explode(",", $obj->combo);
                      $count = 0;


                      for ($m = 0; $m <= count($multipleOptions); $m++) {
                        while ($count < count($optionsSelect)) {
                          if ($multipleOptions[$m] == $optionsSelect[$count]) {
                            $options .= '<option class="optionUpdate' . $countOption . '" selected="selected">' . $optionsSelect[$count] . '</option>';
                            $count++;
                            // break;
                            if (count($multipleOptions) >= 2)
                              break;
                          } else {
                            $options .= '<option class="optionUpdate' . $countOption . '">' . $optionsSelect[$count] . '</option>';
                            $count++;
                          }
                        }
                      }

                      $input .= '<td><select multiple data-placeholder="Selecione" class="form-control chzn-select info-adicional-grupo-update combo sequencia' . $countOption . '" tabindex="8" id="' . $obj1->cdtipoinformacaoadicional . '?' . $objRelacao->id_info_tipo_grupo . '*combo" value="' . $obj->combo . '">';

                      $input .= $options;

                      $input .= '</select></td>';
                      array_push($grupo, $input);
                    } elseif ($obj1->idtipodado == "V") {
                      $input = '<td><input type="text" class="form-control info-adicional-grupo-update valor-input sequencia' . $countOption . '" id="' . $obj1->cdtipoinformacaoadicional . '?' . $objRelacao->id_info_tipo_grupo . '*numerico" value="' . number_format($obj->numerico, 2) . '"></td>';
                      array_push($grupo, $input);
                    } elseif ($obj1->idtipodado == "H") {
                      $input = '<td><input type="text" class="form-control info-adicional-grupo-update hora-input sequencia' . $countOption . '" id="' . $obj1->cdtipoinformacaoadicional . '?' . $objRelacao->id_info_tipo_grupo . '*hora" value="' . $obj->hora . '"></td>';
                      array_push($grupo, $input);
                    } elseif ($obj1->idtipodado == "L") {
                      $input = '<td><input type="text" class="form-control info-adicional-grupo-update percent-input sequencia' . $countOption . '" id="' . $obj1->cdtipoinformacaoadicional . '?' . $objRelacao->id_info_tipo_grupo . '*percentual" value="' . number_format($obj->percentual, 2) . '"></td>';
                      array_push($grupo, $input);
                    } else {
                      $input = '<td><textarea class="form-control info-adicional-grupo-update sequencia' . $countOption . '" id="' . $obj1->cdtipoinformacaoadicional . '?' . $objRelacao->id_info_tipo_grupo . '*descricao" rows"1">' . $obj->descricao . '</textarea></td>';
                      array_push($grupo, $input);
                    }

                    $countOption++;
                  }

                  $linhas = array_chunk($grupo, ($qtd - 1));

                  $qtdLinhas = count($linhas);
                  $tr = "";

                  for ($q = 0; $q < $qtdLinhas; $q++) {
                    $tr .= '<tr>';
                    for ($m = 0; $m < count($linhas[$q]); $m++) {
                      $tr .= $linhas[$q][$m];
                    }

                    $tr .= '</tr>';
                  }


                  $response['response_data']['titulos'] = $th;
                  $response['response_data']['camposGrupo'] = $tr;
                  // $response['response_data']['camposGrupoArray'] = array_filter(explode("</tr>", $tr));
                  $response['response_data']['tipoGrupo'] = $tipoGrupo;
                  $response['response_data']['nomeGrupo'] = $nomeGrupo;
                }
              }

              $classificacao .= implode(", ", explode(",", $obj5->nome_clausula));
              $idClassificacao .= implode(", ", explode(",", $obj5->id_estruturaclausula));


              $response['response_data']['listUpdate'] = $htmlUpdate;
              $response['response_data']['classificacao'] = $classificacao;
              $response['response_data']['idClassificacao'] = $idClassificacao;


              //BOTÕES DE SELEÇÃO INFORMAÇÃO ADICIONAL UPDATE


              $html = "";

              $list = explode(" ", trim($idClassificacao));
              $listInfoToUpdate = "";
              for ($i = 0; $i < count($list); $i++) {
                $sql3 = "SELECT 
											estrutura_clausula_id_estruturaclausula, ad_tipoinformacaoadicional_cdtipoinformacaoadicional 
										FROM 
											estrutura_clausulas_ad_tipoinformacaoadicional
										WHERE
										estrutura_clausula_id_estruturaclausula = '{$list[$i]}'
										";


                if ($resultsql = mysqli_query($this->db, $sql3)) {

                  while ($obj = $resultsql->fetch_object()) {
                    $sql4 = "SELECT 
													nmtipoinformacaoadicional, cdtipoinformacaoadicional, idtipodado
												FROM
													ad_tipoinformacaoadicional
												WHERE
													cdtipoinformacaoadicional = '{$obj->ad_tipoinformacaoadicional_cdtipoinformacaoadicional}';
												";
                    $resultsql4 = mysqli_query($this->db, $sql4);

                    while ($obj4 = $resultsql4->fetch_object()) {

                      if (mb_strpos($listInfoToUpdate, $obj4->nmtipoinformacaoadicional) == false) {

                        $listInfoToUpdate .= '<tr>';

                        if ($obj4->idtipodado == "G") {
                          $listInfoToUpdate .= '<td><button type="button" class="btn btn-secondary" onclick="finalizarInfoUpdate(' . $obj4->cdtipoinformacaoadicional . ');">Selecionar</button></td>'; //data-toggle="modal" href="#modalInformacaoTipoGrupo" data-dismiss="modal"
                        } else {
                          $listInfoToUpdate .= '<td><button type="button" class="btn btn-secondary" onclick="finalizarInfoUpdate(' . $obj4->cdtipoinformacaoadicional . ');">Selecionar</button></td>';
                        }



                        $listInfoToUpdate .= '<td>';
                        $listInfoToUpdate .= $obj4->nmtipoinformacaoadicional;
                        $listInfoToUpdate .= '</td>';
                        $listInfoToUpdate .= '</tr>';
                      }
                    }
                  }
                }

                $response['response_data']['listBtnToUpdate'] = $listInfoToUpdate;
              }
            }
          }

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

  function getListInfo()
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
      }
    }
  }

  function getListClassificacao($data = null)
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

        //LISTA ESTRUTURA CLÁUSULA CHECKED / NO CHECKED

        if (key_exists('id_claus', $data)) {
          $sql7 = "SELECT
							id_estruturaclausula, nome_clausula, tipo_clausula, classe_clausula
						FROM
							estrutura_clausula
						;
						";
          $resultsql7 = mysqli_query($this->db, $sql7);

          $listaMod = [];

          while ($obj7 = $resultsql7->fetch_object()) {
            $checkBox = null;
            $sql8 = "SELECT estrutura_clausula_id_estruturaclausula, clausula_geral_id_clau FROM clausula_geral_estrutura_clausula";

            $resultsql8 = mysqli_query($this->db, $sql8);

            //est 1
            while ($obj8 = $resultsql8->fetch_object()) {
              if ($obj7->id_estruturaclausula == $obj8->estrutura_clausula_id_estruturaclausula && $data['id_claus'] == $obj8->clausula_geral_id_clau) {
                $checked = 'true';
                break;
              } else {
                $checked = "";
              }
            }

            $imprime = 'true';

            if ($checked == "true") {
              $checkBox .= '<input class="form-check-input check-update" onclick="saveModuleChange( ' . $obj7->id_estruturaclausula . ', ' . $data['id_claus'] . ');" type="checkbox" value="0" id="checkClass' . $obj7->id_estruturaclausula . '" checked>';
            } else {
              $checkBox .= '<input class="form-check-input check-update" onclick="saveModuleChange( ' . $obj7->id_estruturaclausula . ', ' . $data['id_claus'] . ');" type="checkbox" value="1" id="checkClass' . $obj7->id_estruturaclausula . '">';
            }
            if ($imprime == 'true') {
              $obj7->checkBox = $checkBox;
              array_push($listaMod, $obj7);
            }
          }
          $response['response_data']['listaClausulaUpdate'] = $listaMod;
        }
      }
    }

    return $response;
  }


  function addClausulas($data = null)
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

      $docSindId = $data['ge-input'];

      $dataAtual = (new DateTime('now'))->format('Y-m-d');

      $usuario = (new usuario())->getUserData($data['user'])['response_data']['user_data'];

      //VERIFICA SE JÁ POSSUI DATA DE PROCESSAMENTO
      $sqlVerify = "SELECT * FROM clausula_geral WHERE id_clau = '{$data['id_clausula_em_processamento']}'";
      $objVerify = mysqli_query($this->db, $sqlVerify)->fetch_object();

      if ($objVerify) {
        $today = (new DateTime('now'))->format("Y-m-d");
        $sql = "UPDATE clausula_geral
							SET 
								data_processamento = '{$today}',
								numero_clausula = '{$data['numero_clausula']}',
								estrutura_id_estruturaclausula = '{$data['idEstruturas']}',
								nome_informacao = '{$data['idEstruturas']}',
								responsavel_processamento = '{$usuario->id_user}',
								tex_clau = '{$data['info-input']}'
								assunto_idassunto = '{$data['assunto']}'
							WHERE id_clau = '{$data['id_clausula_em_processamento']}'
						";
      } else {
        $sql = "INSERT INTO clausula_geral 
							(tex  _clau, 
							estrutura_id_estruturaclausula, 
							doc_sind_id_documento,
							data_processamento,
							numero_clausula,
							responsavel_processamento,
							assunto_idassunto
							)
						VALUES
							('{$data['info-input']}', 
							'{$data['idEstruturas']}', 
							'{$docSindId}',
							'{$dataAtual}',
							'{$data['numero_clausula']}',
							'{$usuario->id_user}',
							'{$data['assunto']}')
				";
      }

      //RECEBENDO INFORMAÇÕES ADICIONAIS
      $array = $data['infoAdicionaisSimples'];
      $array2 = $data['InfoAdicionaisComId'];


      //ADD INFORMAÇÕES SIMPLES
      if (!empty($array)) {
        for ($i = 0; $i < count($array); $i++) {
          $content = strstr($array[$i], "+", true);
          $idInfo = strstr(substr(strstr($array[$i], "+"), 1), "?", true);
          $idEstrutura = strstr(substr(strstr($array[$i], "?"), 1), "*", true);
          $campo = substr(strstr($array[$i], "*"), 1);

          (mb_strpos($content, "%") == true ? $content = (float) strstr($content, "%", true) : "");

          if ($campo == "data" && $content != "") {
            $content = implode("-", array_reverse(explode("/", $content)));
          }

          if ($campo == "data" && $content == "") {
            $content = "0000-00-00";
          }

          $sqlSimples = "INSERT INTO clausula_geral_estrutura_clausula
										(estrutura_clausula_id_estruturaclausula, 
										nome_informacao, 
										clausula_geral_id_clau,
										doc_sind_id_doc,
										ad_tipoinformacaoadicional_cdtipoinformacaoadicional,
										{$campo})
									VALUES
										('{$idEstrutura}',
										'{$idEstrutura}',
										'{$lastId}',
										'{$docSindId}',
										'{$idInfo}',
										'{$content}')
					";
        }
      }

      //ADD INFORMAÇÃO QUANDO HÁ APENAS UMA LINHA NO GRUPO
      if (!empty($array2) && !$data['extraLines']) {
        $sequencia = 1;
        $nomeInformacao = $data['idEstruturas'];

        for ($i = 0; $i < count($array2); $i++) {
          $posInfo = strpos($array2[$i], "+");
          $posEst = strpos($array2[$i], "?");

          $idInfoAdicional = strstr(substr($array2[$i], $posInfo + 1), "?", true);
          $idEstrutura = substr(strstr(strstr($array2[$i], "?"), "*", true), 1); //substr($array2[$i], $posEst+1);
          $conteudoInfo = strstr($array2[$i], "+", true);
          $infoGrupo = substr(strstr($array2[$i], "*"), 1);

          $sqlSlct = "SELECT * FROM ad_tipoinformacaoadicional WHERE cdtipoinformacaoadicional = $idInfoAdicional";
          $resultSlct = mysqli_query($this->db, $sqlSlct);
          $obj = $resultSlct->fetch_object();

          //NOME INFORMAÇÃO -> campo: nome_informacao
          if ($obj->cdtipoinformacaoadicional == 170) {
            $info = "SELECT id_estruturaclausula, nome_clausula FROM estrutura_clausula WHERE nome_clausula = '{$conteudoInfo}'";
            $infoObj = mysqli_query($this->db, $info)->fetch_object();

            $nomeInformacao = $infoObj->id_estruturaclausula;
          }

          if ($obj->idtipodado == "D") {
            $conteudoInfo = $conteudoInfo == "" ? "0000-00-00" : date('Y-m-d', strtotime(implode("-", explode("/", $conteudoInfo))));
            $campo = 'data';
          } elseif ($obj->idtipodado == "N") {
            $campo = 'numerico';
            $conteudoInfo = ($conteudoInfo != "" ? $conteudoInfo : 0);
          } elseif ($obj->idtipodado == "L") {
            (mb_strpos($conteudoInfo, "%") == true ? $conteudoInfo = (float) strstr($conteudoInfo, "%", true) : $conteudoInfo = 0.00);
            $conteudoInfo = ($conteudoInfo != "" ? $conteudoInfo : 0.00);
            $campo = 'percentual';
          } elseif ($obj->idtipodado == "H") {
            $campo = 'hora';
            $conteudoInfo = ($conteudoInfo != "" ? $conteudoInfo : '000:00:00');
          } elseif ($obj->idtipodado == "T") {
            $campo = 'texto';
            $conteudoInfo = ($conteudoInfo != "" ? $conteudoInfo : 'null');
          } elseif ($obj->idtipodado == "V") {
            $campo = 'numerico';
            $conteudoInfo = ($conteudoInfo != "" ? $conteudoInfo : 0);
          } elseif ($obj->idtipodado == "C") {
            $campo = 'combo';
          } elseif ($obj->idtipodado == "CM") {
            $campo = 'combo';
            $conteudoInfo = ($conteudoInfo[0] == "," ? substr($conteudoInfo, 1) : $conteudoInfo);
            $conteudoInfo = ($conteudoInfo != "" ? $conteudoInfo : 'null');
          } else {
            $campo = 'descricao';
            $conteudoInfo = ($conteudoInfo != "" ? $conteudoInfo : 'null');
          }

          if ($obj->idtipodado != "G") {
            $conteudoInfo = $conteudoInfo == 'null' ? null : $conteudoInfo;
            $sql5 = $this->insertInto(
              "clausula_geral_estrutura_clausula",
              "estrutura_clausula_id_estruturaclausula, 
													nome_informacao,
													clausula_geral_id_clau,
													doc_sind_id_doc,
													ad_tipoinformacaoadicional_cdtipoinformacaoadicional,
													{$campo},
													id_info_tipo_grupo,
													sequencia,
													grupo_dados",
              "{$idEstrutura},
													{$nomeInformacao},
													{$lastId},
													{$docSindId},
													{$idInfoAdicional},
													'{$conteudoInfo}',
													{$infoGrupo},
													{$sequencia},
													1"
            );
            $response = $this->save($sql5, __LINE__);
          }

          $sequencia++;
        }
      }

      //Tratando dados para o registro - (Informações do grupo)
      if ($data['qtdRegistros'] && $data['infoGrupo']) {

        $qtdRegistros = $data['qtdRegistros'];

        if ($data['infoGrupo']) {
          $registros = array_chunk($data['infoGrupo'], $data['qtdCampos']);
        }

        $principal = $data['InfoAdicionaisComId'];

        $primeiraLinha = [];

        for ($i = 0; $i < count($principal); $i++) {
          $content = strstr($principal[$i], "+", true);
          $idInfo = substr(strstr(strstr($principal[$i], "?", true), "+"), 1);
          $idGrupo = substr(strstr($principal[$i], "?"), 1);
          $idInfoEGrupo = strstr($principal[$i], "+");
          $primeiraLinha[$i] = $content;

          for ($o = 0; $o < $qtdRegistros; $o++) {
            $registros[$o][$i] .= $idInfoEGrupo;
          }
        }

        if ($registros && !empty($registros)) {

          array_unshift($registros, $principal);
        } else {
          $registros = $principal;
        }
      }

      //ADD INFORMÇÃO TIPO GRUPO
      if ($registros) {
        $sequencia = 1;
        $linha = 1;
        for ($i = 0; $i < count($registros); $i++) {
          if ($data['extraLines'] != 0) {
            $itensPorLinha = count($registros[$i]);

            for ($l = 0; $l < count($registros[$i]); $l++) {
              if (!is_array($content)) {
                $content = strstr($registros[$i][$l], "+", true);
              } else {
                $content = $registros[$i][$l];
              }

              $idInfo = substr(strstr(strstr($registros[$i][$l], "?", true), "+"), 1);
              $idGrupo = substr(strstr(substr(strstr($registros[$i][$l], "?"), 1), "*"), 1);
              $idEstrutura = strstr(substr(strstr($registros[$i][$l], "?"), 1), "*", true);

              $sql = "SELECT nmtipoinformacaoadicional, cdtipoinformacaoadicional, idtipodado FROM ad_tipoinformacaoadicional WHERE cdtipoinformacaoadicional = {$idInfo}";
              $resultSql = mysqli_query($this->db, $sql);

              $obj = $resultSql->fetch_object();

              //NOME INFORMAÇÃO -> campo: nome_informacao
              if ($obj->cdtipoinformacaoadicional == 170) {
                $info = "SELECT id_estruturaclausula, nome_clausula FROM estrutura_clausula WHERE nome_clausula = '{$content}'";
                $infoObj = mysqli_query($this->db, $info)->fetch_object();

                $nomeInformacao = $infoObj->id_estruturaclausula;
              }

              if ($obj->idtipodado == "D") {
                $content = ($content != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $content)))) : "0000-00-00");
                $campo = 'data';
              } elseif ($obj->idtipodado == "N") {
                $content = ($content != "" ? $content : '0.00');
                $campo = 'numerico';
              } elseif ($obj->idtipodado == "L") {
                (mb_strpos($content, "%") != false ? $content = (float) strstr($content, "%", true) : $content = '0.00');
                $content = ($content != "" ? $content : '0.00');
                $campo = 'percentual';
              } elseif ($obj->idtipodado == "H") {
                $content = ($content != "" ? $content : '000:00:00');
                $campo = 'hora';
              } elseif ($obj->idtipodado == "T") {
                $content = ($content != "" ? $content : 'null');
                $campo = 'texto';
              } elseif ($obj->idtipodado == "V") {
                $content = ($content != "" ? $content : '0.00');
                $campo = 'numerico';
              } elseif ($obj->idtipodado == "C") {
                $campo = 'combo';
              } elseif ($obj->idtipodado == "CM") {
                $content = ($content[0] == "," ? substr($content, 1) : $content);
                $content = ($content != "" ? $content : 'null');
                $campo = 'combo';
              } else {
                $content = ($content != "" ? $content : 'null');
                $campo = 'descricao';
              }

              if ($content) {
                $content = $content == 'null' ? $content = null : $content;

                $sql = "INSERT INTO clausula_geral_estrutura_clausula
										(id_info_tipo_grupo,
										estrutura_clausula_id_estruturaclausula,
										nome_informacao,
										grupo_dados,
										clausula_geral_id_clau,
										doc_sind_id_doc,
										ad_tipoinformacaoadicional_cdtipoinformacaoadicional,
										sequencia,
										{$campo})
									VALUES
										('{$idGrupo}',
										'{$idEstrutura}',
										'{$nomeInformacao}',
										'{$linha}',
										'{$lastId}',
										'{$docSindId}',
										'{$idInfo}',
										'{$sequencia}',
										'{$content}')		
								";

                if ($sequencia == $itensPorLinha || $sequencia % $itensPorLinha == 0) {
                  $linha++;
                }

                $sequencia++;

                if (!mysqli_query($this->db, $sql)) {
                  $response['response_status']['status'] = 0;
                  $response['response_status']['error_code'] = $this->error_code . __LINE__;
                  $response['response_status']['msg'] = '';
                } else {
                  $response['response_status']['status'] = 1;
                  $response['response_status']['error_code'] = null;
                  $response['response_status']['msg'] = 'Cadastro de grupo realizado com sucesso';
                }
              }
            }
          }
        }
      }
    } else {
      $response = $this->response;
    }

    return $response;
  }

  function updateClausula($data = null)
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

      $idDocSindUpdate = $data['ge-input-update'];
      $dataAtual = (new DateTime('now'))->format('Y-m-d');

      if ($response['response_status']['status'] == 1) {
        $sql = "UPDATE 
							clausula_geral
						SET  
							tex_clau = '{$data['info-input-update']}', 
							doc_sind_id_documento = '{$idDocSindUpdate}',
							data_processamento = '{$dataAtual}',
							numero_clausula = {$data['numero_clausula']},
							assunto_idassunto = {$data['assunto']}
						WHERE 
							id_clau = {$data['id_clau']};
						";

        if (!mysqli_query($this->db, $sql)) {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        } else {

          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Registro atualizado com sucesso';
        }

        //ADD RELACIONAMENTO

        $array = $data['InfoAdicionaisUpdate'];

        if ($array) {
          for ($i = 0; $i < count($array); $i++) {


            $content = strstr($array[$i], "+", true);
            $column = strstr(substr($array[$i], (strlen($content) + 1)), "?", true);
            $idTable = substr(strstr($array[$i], "?"), 1);

            if (strpos($content, "/") > 0 && $content != "") {
              $content = date('Y-m-d', strtotime(implode("-", explode("/", $content))));
            } else if ($content == "") {
              $content = "0000-00-00";
            }

            (mb_strpos($content, "%") == true ? $content = (float) strstr($content, "%", true) : "");

            if ($content[0] == ",") {
              $content = substr($content, 1);
            }


            $sqlCol = "SELECT idtipodado, cdtipoinformacaoadicional FROM ad_tipoinformacaoadicional where cdtipoinformacaoadicional = '{$column}' ";
            $resultCol = mysqli_query($this->db, $sqlCol);
            $objCol = $resultCol->fetch_object();


            if ($objCol->idtipodado == "C" || $objCol->idtipodado == "CM")
              $col = "combo";
            if ($objCol->idtipodado == "D")
              $col = "data";
            if ($objCol->idtipodado == "T")
              $col = "texto";
            if ($objCol->idtipodado == "L")
              $col = "percentual";
            if ($objCol->idtipodado == "P")
              $col = "descricao";
            if ($objCol->idtipodado == "N" || $objCol->idtipodado == "V")
              $col = "numerico";

            $sql = "UPDATE
									clausula_geral_estrutura_clausula 
								SET
									{$col} = '{$content}'
								WHERE 
									id_clausulageral_estrutura_clausula = '{$idTable}'
								";


            if (!mysqli_query($this->db, $sql)) {

              $response['response_status']['status'] = 0;
              $response['response_status']['error_code'] = $this->error_code . __LINE__;
              $response['response_status']['msg'] = 'Falha na atualização do registro!';

            } else {

              $response['response_status']['status'] = 1;
              $response['response_status']['error_code'] = $this->error_code . __LINE__;
              $response['response_status']['msg'] = 'Registro atualizado com sucesso!';
            }
          }
        }


        $arrayData = $data['resultInfoExtra'];
        $idClassificacao = explode(", ", $data['id_classificacao']);

        if ($arrayData) {
          for ($i = 0; $i < count($arrayData); $i++) {
            $idInfoExtra = strstr($arrayData[$i], "?", true);
            $contInfoExtra = substr(strstr($arrayData[$i], "?"), 1);


            for ($p = 0; $p < count($idClassificacao); $p++) {
              $sql = "SELECT * FROM estrutura_clausulas_ad_tipoinformacaoadicional WHERE estrutura_clausula_id_estruturaclausula = '{$idClassificacao[$p]}' AND ad_tipoinformacaoadicional_cdtipoinformacaoadicional = '{$idInfoExtra}'";
              $result = mysqli_query($this->db, $sql);

              if ($obj = $result->fetch_object()) {
                $sqlSlct = "SELECT * FROM ad_tipoinformacaoadicional WHERE cdtipoinformacaoadicional = $idInfoExtra";
                $resultSlct = mysqli_query($this->db, $sqlSlct);
                $obj = $resultSlct->fetch_object();

                if ($obj->idtipodado == "D") {
                  $contInfoExtra = ($contInfoExtra != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $contInfoExtra)))) : '0000-00-00');
                  $campo = 'data';
                } elseif ($obj->idtipodado == "N") {
                  $contInfoExtra = ($contInfoExtra != "" ? $contInfoExtra : '0.00');
                  $campo = 'numerico';
                } elseif ($obj->idtipodado == "L") {
                  (mb_strpos($contInfoExtra, "%") == true ? $contInfoExtra = number_format((float) strstr($contInfoExtra, "%", true), 2) : "");
                  $contInfoExtra = ($contInfoExtra != "" ? $contInfoExtra : '0.00');
                  $campo = 'percentual';
                } elseif ($obj->idtipodado == "H") {
                  $contInfoExtra = ($contInfoExtra != "" ? $contInfoExtra : '000:00:00');
                  $campo = 'hora';
                } elseif ($obj->idtipodado == "T") {
                  $contInfoExtra = ($contInfoExtra != "" ? $contInfoExtra : 'null');
                  $campo = 'texto';
                } elseif ($obj->idtipodado == "V") {
                  $contInfoExtra = ($contInfoExtra != "" ? $contInfoExtra : '0');
                  $campo = 'numerico';
                } elseif ($obj->idtipodado == "C" || $obj->idtipodado == "CM") {
                  $campo = 'combo';
                } else {
                  $contInfoExtra = ($contInfoExtra != "" ? $contInfoExtra : 'null');
                  $campo = 'descricao';
                }

                $sql5 = $this->insertInto(
                  "clausula_geral_estrutura_clausula",
                  "estrutura_clausula_id_estruturaclausula, 
																clausula_geral_id_clau,
																doc_sind_id_doc,
																ad_tipoinformacaoadicional_cdtipoinformacaoadicional,
																{$campo}",
                  "{$idClassificacao[$p]},
																{$data['id_clau']},
																{$idDocSindUpdate},
																{$idInfoExtra},
																'{$contInfoExtra}'"
                );
                $response = $this->save($sql5, __LINE__);
              }
            }
          }
        }

        //Informação do tipo grupo já cadastrada
        if ($data['linhasCadastradas']) {


          $linhasCadastradas = $data['linhasCadastradas'];
          $linhasExtras = $data['linhasExtras'];

          $sql = "SELECT * FROM clausula_geral_estrutura_clausula WHERE clausula_geral_id_clau = '{$data['id_clau']}'";
          $result = mysqli_query($this->db, $sql);

          for ($i = 0; $i < count($linhasCadastradas); $i++) {

            $content = strstr($linhasCadastradas[$i], "+", true); //22/05/2022+14?30*data
            $idInfo = strstr(substr(strstr($linhasCadastradas[$i], "+"), 1), "?", true);
            $idGrupo = strstr(substr(strstr($linhasCadastradas[$i], "?"), 1), "*", true);
            $campo = substr(strstr($linhasCadastradas[$i], "*"), 1);
            $sequencia = $i + 1;


            if ($campo == "data" && $content == "") {
              $content = "0000-00-00";
            } elseif ($campo == "data" && $content != "") {
              $content = date('Y-m-d', strtotime(implode("-", explode("/", $content))));
            } elseif ($campo == "texto" && $content == "") {
              $content = "null";
            } elseif ($campo == "numerico" && $content == "") {
              $content = "0.00";
            } elseif ($campo == "percentual" && $content == "") {
              $content = "0.00";
            } elseif ($campo == "hora" && $content == "") {
              $content = "000:00:00";
            } elseif ($campo == "descricao" && $content == "") {
              $content = "null";
            }

            $sql2 = "UPDATE 
									clausula_geral_estrutura_clausula 
								SET $campo = '{$content}'
								WHERE sequencia = '{$sequencia}' AND clausula_geral_id_clau = '{$data['id_clau']}';
						";

            if (!mysqli_query($this->db, $sql2)) {

              $response['response_status']['status'] = 0;
              $response['response_status']['error_code'] = $this->error_code . __LINE__;
              $response['response_status']['msg'] = 'Falha na atualização dos registros do grupo!';

            } else {

              $response['response_status']['status'] = 1;
              $response['response_status']['error_code'] = $this->error_code . __LINE__;
              $response['response_status']['msg'] = 'Registros do grupo atualizados com sucesso!';
            }
          }
        }

        //Linhas adicionais da informação tipo grupo

        if ($data['linhasExtras']) {

          $linhasExtras = $data['linhasExtras'];

          $sql = "SELECT * FROM clausula_geral_estrutura_clausula WHERE clausula_geral_id_clau = '{$data['id_clau']}' AND sequencia > 0";
          $result = mysqli_query($this->db, $sql);
          $obj = $result->fetch_all();

          $qtd = count($obj);
          $sequencia = $qtd + 1;

          for ($i = 0; $i < count($linhasExtras); $i++) {

            $contentExtra = strstr($linhasExtras[$i], "+", true); //22/05/2022+14?30*data
            $idInfoExtra = strstr(substr(strstr($linhasExtras[$i], "+"), 1), "?", true);
            $idGrupoExtra = strstr(substr(strstr($linhasExtras[$i], "?"), 1), "*", true);
            $campoExtra = substr(strstr($linhasExtras[$i], "*"), 1);

            if ($campoExtra == "data" && $contentExtra == "") {
              $contentExtra = "0000-00-00";
            } elseif ($campoExtra == "data" && $contentExtra != "") {
              $contentExtra = date('Y-m-d', strtotime(implode("-", explode("/", $contentExtra))));
            } elseif ($campoExtra == "texto" && $contentExtra == "") {
              $contentExtra = "null";
            } elseif ($campoExtra == "numerico" && $contentExtra == "") {
              $contentExtra = "0.00";
            } elseif ($campoExtra == "percentual" && $contentExtra == "") {
              $contentExtra = '0.00';
            } elseif ($campoExtra == "valor" && $contentExtra == "") {
              $contentExtra = "0.00";
            } elseif ($campoExtra == "hora" && $contentExtra == "") {
              $contentExtra = "000:00:00";
            } elseif ($campoExtra == "descricao" && $contentExtra == "") {
              $contentExtra = "null";
            }


            $sql3 = "INSERT INTO clausula_geral_estrutura_clausula 
									(id_info_tipo_grupo,
									estrutura_clausula_id_estruturaclausula,
									clausula_geral_id_clau,
									doc_sind_id_doc,
									ad_tipoinformacaoadicional_cdtipoinformacaoadicional,
									sequencia,
									{$campoExtra})
								VALUES
									('{$idGrupoExtra}',
									'{$data['id_classificacao']}',
									'{$data['id_clau']}',
									'{$idDocSindUpdate}',
									'{$idInfoExtra}',
									'{$sequencia}',
									'{$contentExtra}');
						";


            if (!mysqli_query($this->db, $sql3)) {

              $response['response_status']['status'] = 0;
              $response['response_status']['error_code'] = $this->error_code . __LINE__;
              $response['response_status']['msg'] = 'Falha ao incluir novos registros no grupo!';

            } else {

              $response['response_status']['status'] = 1;
              $response['response_status']['error_code'] = $this->error_code . __LINE__;
              $response['response_status']['msg'] = 'Registros do grupo atualizados com sucesso!';
            }

            $sequencia++;
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

  function getDocSindById($data = null)
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


        $sql = "SELECT * FROM doc_sind WHERE id_doc = '{$data['id_doc']}'";

        if ($result = mysqli_query($this->db, $sql)) {

          $obj = $result->fetch_object();

          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Dados obtidos com sucesso!';

          $response['response_data']['data_doc'] = $obj;
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Não possível obter os dados!';

        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }


    return $response;
  }

  function getSinonimos($data = null)
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

        $sql = "select sin.nome_sinonimo, ec.nome_clausula, ec.id_estruturaclausula from sinonimos as sin inner join estrutura_clausula as ec on ec.id_estruturaclausula
				= sin.estrutura_clausula_id_estruturaclausula;								
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {


          $opt0 = null;
          $json_resp = '[{
						"id": 0,
						"nome": "XXXXXXXX",
						"sinonimos": [
							"XXXXXXXXX"
						';
          $anterior = "";
          while ($obj = $resultsql->fetch_object()) {

            if ($obj->nome_clausula == $anterior) {
              $json_resp .= '"' . $obj->nome_sinonimo . '",';
            } else {
              $json_resp = substr($json_resp, 0, -1);
              $json_resp .= ']
						},{
							    "id": "' . $obj->id_estruturaclausula . '",
								"nome": "' . $obj->nome_clausula . '",
								"sinonimos": [
									"' . $obj->nome_sinonimo . '",';
            }
            $anterior = $obj->nome_clausula;
          }
          $json_resp = substr($json_resp, 0, -1);
          $json_resp .= "]}
					]";

          $response['response_data']['json_resp'] = $json_resp;


          $jsn = '{
						"url": "https://ineditta.com/documentos_sistema/documentos_sindicais/CCT-Sao-Paulo-2022-2023-0065MTItMTItMjAyMi0xNToxMTozOQ==.pdf",
						"procure": [{
													"nome": "Adicional de horas extras",
													"sinonimos": [
														"DAS HORAS EXTRAS/BANCO DE HORAS","HORAS EXTRAS","TRABALHO EXTRAORDINÁRIO"]
											}
										]
					}';


          $ch = curl_init();

          curl_setopt_array(
            $ch,
            array(
              CURLOPT_URL => 'http://177.70.102.46:5001',
              CURLOPT_RETURNTRANSFER => true,
              CURLOPT_SSL_VERIFYPEER => 0,
              CURLOPT_ENCODING => "",
              CURLOPT_MAXREDIRS => 10,
              CURLOPT_HEADER => false,
              CURLOPT_TIMEOUT => 10,
              CURLOPT_FOLLOWLOCATION => true,
              CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
              CURLOPT_CUSTOMREQUEST => "POST",
              CURLOPT_POSTFIELDS => $jsn,
              CURLOPT_HTTPHEADER => array(
                "Content-Type: application/json",
              ),
            )
          );


          $wsresp = curl_exec($ch);



          // 	"url": "' . $data['caminho'] . '",
          // 	"procure": ' . $json_resp . '
          // }');





          if (!curl_errno($ch)) {

            $getinfo = curl_getinfo($ch);


            if ($getinfo['http_code'] == 200) {

              $response['response_data']['response_api'] = $wsresp;
            } else {
              $response['response_status']['status'] = 0;
              $response['response_status']['error_code'] = $this->error_code;
              $response['response_status']['msg'] = '';
            }
          } else {

            $getinfo = curl_getinfo($ch);


            // $error_message = curl_strerror(curl_errno($ch));

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code;
            $response['response_status']['msg'] = '';
          }
          curl_close($ch);
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

  function saveClausula($data = null)
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

      $dataAtual = (new DateTime('now'))->format('Y-m-d');


      //ADD CLAUSULA GERAL E QUANDO NÃO HÁ INFORMAÇÕES ADICIONAIS
      $sql = "INSERT INTO clausula_geral 
						(tex_clau, 
						estrutura_id_estruturaclausula, 
						doc_sind_id_documento,
						data_processamento
						)
					VALUES
						('{$data['texto']}', 
						{$data['idestru']}, 
						{$data['iddocu']},
						'0000-00-00')
			";



      if (!mysqli_query($this->db, $sql)) {

        $response['response_status']['status'] = 0;
        $response['response_status']['error_code'] = $this->error_code . __LINE__;
        $response['response_status']['msg'] = '';

      } else {
        $sqlUp = "UPDATE doc_sind
							SET data_scrap = '{$dataAtual}'
						WHERE id_doc = '{$data['iddocu']}'
				";

        mysqli_query($this->db, $sqlUp);

        $response['response_status']['status'] = 1;
        $response['response_status']['error_code'] = 'Scrap realizado com sucesso';
        $response['response_status']['msg'] = '';
      }
    } else {
      $response = $this->response;
    }

    return $response;
  }

  function saveModuleChange($data = null)
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

        if (($data['check']) == 0) {
          $sql = "DELETE 
								FROM 
									clausula_geral_estrutura_clausula 
								WHERE 
									clausula_geral_id_clau = {$data['id_clau']}
								AND 
								estrutura_clausula_id_estruturaclausula 	  = {$data['id_estruturaclausula']};
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

          $sql = "INSERT INTO clausula_geral_estrutura_clausula (estrutura_clausula_id_estruturaclausula, clausula_geral_id_clau)
							VALUES ( {$data['id_estruturaclausula']} , {$data['id_clau']} );
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

  function finalizarInfo($data = null)
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
        //INFORMAÇÕES ADICIONAIS
        if ($data['id_informacoes']) {
          $idInfo = $data['id_informacoes'];
          $list = explode(",", $data['id_estruturaclausula']);

          $infoList = "";
          $infoListGrupoCampo = "";
          $infoListGrupo = "";

          $sql4 = "SELECT 
								nmtipoinformacaoadicional, cdtipoinformacaoadicional, idtipodado
							FROM
								ad_tipoinformacaoadicional
							WHERE
								cdtipoinformacaoadicional = '{$idInfo}';
							";

          $resultsql4 = mysqli_query($this->db, $sql4);

          for ($i = 0; $i < count($list); $i++) {
            $sql3 = "SELECT 
									estrutura_clausula_id_estruturaclausula, ad_tipoinformacaoadicional_cdtipoinformacaoadicional 
								FROM 
									estrutura_clausulas_ad_tipoinformacaoadicional
								WHERE
								ad_tipoinformacaoadicional_cdtipoinformacaoadicional = '{$idInfo}' AND estrutura_clausula_id_estruturaclausula = '{$list[$i]}'
						";

            if ($resultsql3 = mysqli_query($this->db, $sql3)) {
              $objRel = $resultsql3->fetch_object();

              if ($objRel->estrutura_clausula_id_estruturaclausula) {
                $sql5 = "SELECT id_estruturaclausula, nome_clausula FROM estrutura_clausula WHERE id_estruturaclausula = '{$objRel->estrutura_clausula_id_estruturaclausula}'";
                $resultsql5 = mysqli_query($this->db, $sql5);

                $objEst = $resultsql4->fetch_object();
                $obj3 = $resultsql5->fetch_object();

                if ($objEst->cdtipoinformacaoadicional) {
                  $infoList .= $objEst->idtipodado == "G" ? '<tr id="' . $objEst->cdtipoinformacaoadicional . '" class="infoGrupo">' : '<tr id="' . $objEst->cdtipoinformacaoadicional . '">';

                  $infoList .= '<td>';
                  $infoList .= $objEst->nmtipoinformacaoadicional;
                  $infoList .= '</td>';
                  $infoList .= '<td>';

                  if ($objEst->idtipodado == "D") {
                    $infoList .= '<input type="text" style="width: 200px;" class="form-control info-adicional-simples date-input" id="' . $objEst->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*data" . '" placeholder="00/00/0000">'; //
                  } elseif ($objEst->idtipodado == "N") {
                    $infoList .= '<input type="number" style="width: 200px;" class="form-control info-adicional-simples number-input" id="' . $objEst->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*numerico" . '">';
                  } elseif ($objEst->idtipodado == "L") {
                    $infoList .= '<input type="text" style="width: 200px;" class="form-control info-adicional-simples percent-input" id="' . $objEst->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*percentual" . '">'; //
                  } elseif ($objEst->idtipodado == "H") {
                    $infoList .= '<input type="text" style="width: 200px;" placeholder="hhh:mm:ss" class="form-control info-adicional-simples hora-input" id="' . $objEst->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*hora" . '">'; //
                  } elseif ($objEst->idtipodado == "T") {
                    $infoList .= '<input type="text" style="width: 200px;" class="form-control info-adicional-simples" id="' . $objEst->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*texto" . '">'; //
                  } elseif ($objEst->idtipodado == "V") {
                    $infoList .= '<input type="text" style="width: 200px;" class="form-control info-adicional-simples valor-input" id="' . $objEst->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*valor" . '">'; //
                  } elseif ($objEst->idtipodado == "G") {
                    $tipo = $objEst->idtipodado;

                    $sqlGrupo = "SELECT 
														info.ad_tipoinformacaoadicional_id,
														info.informacaoadicional_no_grupo as id_info,
														info.sequencia
													FROM 
														informacao_adicional_grupo as info
													WHERE 
														ad_tipoinformacaoadicional_id = '{$objEst->cdtipoinformacaoadicional}' order by sequencia asc";
                    $result = mysqli_query($this->db, $sqlGrupo);

                    $infoListGrupo .= '<thead id="topo">';
                    $infoListGrupoCampo .= '<tr id="linha-grupo">';

                    $countOption = 1;

                    while ($obj = $result->fetch_object()) {
                      $sqlInfo = "SELECT 
															idtipodado, 
															nmtipoinformacaoadicional, 
															cdtipoinformacaoadicional 
														FROM 
															ad_tipoinformacaoadicional
														WHERE cdtipoinformacaoadicional = '{$obj->id_info}';
											";
                      $resultInfo = mysqli_query($this->db, $sqlInfo);
                      $objInfoName = $resultInfo->fetch_object();

                      $infoListGrupo .= '<th class="title">' . $objInfoName->nmtipoinformacaoadicional . '</th>';

                      if ($objInfoName->idtipodado == "D") {
                        $infoListGrupoCampo .= '<td><input type="text" class="form-control info-adicional date-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*" . $objEst->cdtipoinformacaoadicional . '" placeholder="00/00/0000"></td>'; //
                      } elseif ($objInfoName->idtipodado == "N") {
                        $infoListGrupoCampo .= '<td><input type="number" class="form-control info-adicional number-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*" . $objEst->cdtipoinformacaoadicional . '"></td>';
                      } elseif ($objInfoName->idtipodado == "L") {
                        $infoListGrupoCampo .= '<td><input type="text" class="form-control info-adicional percent-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*" . $objEst->cdtipoinformacaoadicional . '"></td>'; //
                      } elseif ($objInfoName->idtipodado == "H") {
                        $infoListGrupoCampo .= '<td><input type="text" placeholder="hhh:mm:ss" class="form-control info-adicional hora-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*" . $objEst->cdtipoinformacaoadicional . '"></td>'; //
                      } elseif ($objInfoName->idtipodado == "T") {
                        $infoListGrupoCampo .= '<td><input type="text" class="form-control info-adicional text-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*" . $objEst->cdtipoinformacaoadicional . '"></td>'; //
                      } elseif ($objInfoName->idtipodado == "V") {
                        $infoListGrupoCampo .= '<td><input type="text" class="form-control info-adicional valor-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*" . $objEst->cdtipoinformacaoadicional . '"></td>'; //
                      } elseif ($objInfoName->idtipodado == "C") {
                        $infoListGrupoCampo .= '<td><select class="form-control info-adicional combo" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*" . $objEst->cdtipoinformacaoadicional . '">';

                        $sqlCombo = "SELECT * FROM informacao_adicional_combo WHERE ad_tipoinformacaoadicional_id = $objInfoName->cdtipoinformacaoadicional";
                        $resultCombo = mysqli_query($this->db, $sqlCombo);
                        $obj = $resultCombo->fetch_object();

                        $optionArray = explode(", ", $obj->options);

                        for ($i = 0; $i < count($optionArray); $i++) {
                          $infoListGrupoCampo .= '<option class="option' . $countOption . '" value="' . $optionArray[$i] . '">' . $optionArray[$i] . '</option>';
                        }
                        $infoListGrupoCampo .= '</select></td>';
                      } elseif ($objInfoName->idtipodado == "CM") {
                        $infoListGrupoCampo .= '<td><select multiple data-placeholder="Selecione" class="form-control info-adicional combo-multiplo chzn-select" tabindex="8" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*" . $objEst->cdtipoinformacaoadicional . '">';

                        $sqlCombo = "SELECT * FROM informacao_adicional_combo WHERE ad_tipoinformacaoadicional_id = $objInfoName->cdtipoinformacaoadicional";
                        $resultCombo = mysqli_query($this->db, $sqlCombo);
                        $obj = $resultCombo->fetch_object();

                        $optionArray = explode(", ", $obj->options);

                        for ($i = 0; $i < count($optionArray); $i++) {
                          $infoListGrupoCampo .= '<option class="option' . $countOption . '" value="' . $optionArray[$i] . '">' . $optionArray[$i] . '</option>';
                        }
                        $infoListGrupoCampo .= '</select></td>';
                      } else {
                        $infoListGrupoCampo .= '<td><textarea class="form-control info-adicional" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*" . $objEst->cdtipoinformacaoadicional . '" rows="1"></textarea></td>'; //
                      }
                      $countOption++;
                    }

                    $infoListGrupoCampo .= '</tr>';

                    $infoListGrupo .= '</thead>';
                    $infoListGrupo .= '<tbody id="table-info-grupo">';
                    $infoListGrupo .= '</tbody>';
                  } elseif ($objEst->idtipodado == "C") {
                    $infoList .= '<select style="width: 200px;" class="form-control info-adicional-simples" id="' . $objEst->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*combo" . '">';

                    $sqlCombo = "SELECT * FROM informacao_adicional_combo WHERE ad_tipoinformacaoadicional_id = $objEst->cdtipoinformacaoadicional";
                    $resultCombo = mysqli_query($this->db, $sqlCombo);
                    $obj = $resultCombo->fetch_object();

                    $optionArray = explode(", ", $obj->options);

                    for ($i = 0; $i < count($optionArray); $i++) {
                      $infoList .= '<option value="' . $optionArray[$i] . '">' . $optionArray[$i] . '</option>';
                    }
                    $infoList .= '</select>';
                  } elseif ($objEst->idtipodado == "CM") {
                    $infoList .= '<td><select multiple data-placeholder="Selecione" style="width: 200px;" class="form-control info-adicional-simples combo-multiplo chzn-select" tabindex="8" id="' . $objEst->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*combo" . '">';

                    $sqlCombo = "SELECT * FROM informacao_adicional_combo WHERE ad_tipoinformacaoadicional_id = $objEst->cdtipoinformacaoadicional";
                    $resultCombo = mysqli_query($this->db, $sqlCombo);
                    $obj = $resultCombo->fetch_object();

                    $optionArray = explode(", ", $obj->options);

                    for ($i = 0; $i < count($optionArray); $i++) {
                      $infoList .= '<option value="' . $optionArray[$i] . '">' . $optionArray[$i] . '</option>';
                    }
                    $infoList .= '</select></td>';
                  } else {
                    $infoList .= '<textarea class="form-control info-adicional-simples" id="' . $objEst->cdtipoinformacaoadicional . "?" . $obj3->id_estruturaclausula . "*descricao" . '" rows="1"></textarea>'; //
                  }

                  $infoList .= '</td>';
                  $infoList .= '<td style="height: 55px; display: flex; justify-content:center; align-items:center;">';
                  $infoList .= '<button style="color: red; border:none; background-color: transparent;" type="button" onclick="deleteInfo(' . $objEst->cdtipoinformacaoadicional . ')"><i class="fa fa-times"></i></button>';
                  $infoList .= '</td>';
                  $infoList .= '<tr>';
                }
              }
            }
          }
        }

        $response['response_data']['infoAdicionalList'] = $infoList;
        $response['response_data']['tipoInfo'] = $tipo;
        $response['response_data']['tabelaGrupo'] = $infoListGrupo;
        $response['response_data']['camposTableGrupo'] = $infoListGrupoCampo;
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    return $response;
  }

  function selectInfo($data = null)
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
        $list = explode(",", $data['id_estruturaclausula']);

        $optionInfo = "";
        for ($i = 0; $i < count($list); $i++) {
          $sql3 = "SELECT 
								estrutura_clausula_id_estruturaclausula, ad_tipoinformacaoadicional_cdtipoinformacaoadicional 
							FROM 
								estrutura_clausulas_ad_tipoinformacaoadicional
							WHERE
							estrutura_clausula_id_estruturaclausula = '{$list[$i]}'
							";

          if ($resultsql = mysqli_query($this->db, $sql3)) {
            while ($obj = $resultsql->fetch_object()) {
              $sql4 = "SELECT 
				 						nmtipoinformacaoadicional, cdtipoinformacaoadicional, idtipodado
				 					FROM
				 						ad_tipoinformacaoadicional
				 					WHERE
				 						cdtipoinformacaoadicional = '{$obj->ad_tipoinformacaoadicional_cdtipoinformacaoadicional}';
				 					";
              $resultsql4 = mysqli_query($this->db, $sql4);

              while ($obj2 = $resultsql4->fetch_object()) {
                if (mb_strpos($optionInfo, $obj2->nmtipoinformacaoadicional) == false) {
                  $optionInfo .= '<tr>';

                  $optionInfo .= '<td><button type="button" class="btn btn-secondary finalizar-info-btn" onclick="finalizarInfo(' . $obj2->cdtipoinformacaoadicional . ');">Selecionar</button></td>';

                  $optionInfo .= '<td>';
                  $optionInfo .= $obj2->nmtipoinformacaoadicional;
                  $optionInfo .= '</td>';
                  $optionInfo .= '</tr>';
                }
              }
            }
          }
        }

        $response['response_data']['optionInfo'] = $optionInfo;
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }


    return $response;
  }

  function selectInfoUpdate($data = null)
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

        //INFORMAÇÕES ADICIONAIS
        $html = "";

        $list = explode(" ", trim($data['id_estruturaclausula']));
        $optionInfo = "";
        $countOption = 1;
        for ($i = 0; $i < count($list); $i++) {
          $sql3 = "SELECT 
								estrutura_clausula_id_estruturaclausula, ad_tipoinformacaoadicional_cdtipoinformacaoadicional 
							FROM 
								estrutura_clausulas_ad_tipoinformacaoadicional
							WHERE
							estrutura_clausula_id_estruturaclausula = '{$list[$i]}'
							";

          if ($resultsql = mysqli_query($this->db, $sql3)) {

            while ($obj = $resultsql->fetch_object()) {
              $sql4 = "SELECT 
				 						nmtipoinformacaoadicional, cdtipoinformacaoadicional, idtipodado
				 					FROM
				 						ad_tipoinformacaoadicional
				 					WHERE
				 						cdtipoinformacaoadicional = '{$obj->ad_tipoinformacaoadicional_cdtipoinformacaoadicional}';
				 					";
              $resultsql4 = mysqli_query($this->db, $sql4);

              while ($obj2 = $resultsql4->fetch_object()) {

                if (mb_strpos($optionInfo, $obj2->nmtipoinformacaoadicional) == false) {
                  $optionInfo .= '<tr>';

                  if ($obj2->idtipodado == "G") {
                    $optionInfo .= '<td><button type="button"  class="btn btn-secondary" onclick="finalizarInfoUpdate(' . $obj2->cdtipoinformacaoadicional . ');">Selecionar</button></td>';
                  } else {
                    $optionInfo .= '<td><button type="button" class="btn btn-secondary" onclick="finalizarInfoUpdate(' . $obj2->cdtipoinformacaoadicional . ');">Selecionar</button></td>';
                  }

                  $optionInfo .= '<td>';
                  $optionInfo .= $obj2->nmtipoinformacaoadicional;
                  $optionInfo .= '</td>';
                  $optionInfo .= '</tr>';
                }
              }
            }
          }
        }

        $infoList = "";
        $sql = "SELECT * FROM ad_tipoinformacaoadicional WHERE cdtipoinformacaoadicional = '{$data['id_informacoes']}'";
        $result = mysqli_query($this->db, $sql);

        $obj = $result->fetch_object();
        if ($obj->cdtipoinformacaoadicional) {
          $infoList .= '<tr class="campos" id="' . $obj->cdtipoinformacaoadicional . '">';
          $infoList .= '<td>';
          $infoList .= $obj->nmtipoinformacaoadicional;
          $infoList .= '</td>';
          $infoList .= '<td>';

          if ($obj->idtipodado == "D") {
            $infoList .= '<input type="text" class="form-control info-adicionalCampo date-input" id="' . $obj->cdtipoinformacaoadicional . '" placeholder="00/00/0000">'; //
          } elseif ($obj->idtipodado == "N") {

            $infoList .= '<input type="number" class="form-control info-adicionalCampo" id="' . $obj->cdtipoinformacaoadicional . '">';
            // $stringPercent = '<input type="text" class="form-control info-adicionalCampo percent-number" id="'.$obj->cdtipoinformacaoadicional.'">'; 
            // (mb_strpos(strtolower($obj->nmtipoinformacaoadicional), "percentual") !== false ? $infoList .= $stringPercent : $infoList .= $stringNormal);

          } elseif ($obj->idtipodado == "T") {
            $infoList .= '<input type="text" class="form-control info-adicionalCampo" id="' . $obj->cdtipoinformacaoadicional . '">'; //
          } elseif ($obj->idtipodado == "L") {
            $infoList .= '<input type="text" class="form-control info-adicionalCampo percent-input" id="' . $obj->cdtipoinformacaoadicional . '">'; //
          } elseif ($obj->idtipodado == "H") {
            $infoList .= '<input type="text" placeholder="hhh:mm:ss" class="form-control info-adicionalCampo hora-input" id="' . $obj->cdtipoinformacaoadicional . '">'; //
          } elseif ($obj->idtipodado == "C") {
            $infoList .= '<td><select class="form-control info-adicionalCampo combo" id="' . $obj->cdtipoinformacaoadicional . '">';

            $sqlCombo = "SELECT * FROM informacao_adicional_combo WHERE ad_tipoinformacaoadicional_id = $obj->cdtipoinformacaoadicional";
            $resultCombo = mysqli_query($this->db, $sqlCombo);
            $obj = $resultCombo->fetch_object();

            $optionArray = explode(", ", $obj->options);

            for ($i = 0; $i < count($optionArray); $i++) {
              $infoList .= '<option class="option' . $countOption . '" value="' . $optionArray[$i] . '">' . $optionArray[$i] . '</option>';
            }
            $infoList .= '</select></td>';
          } elseif ($obj->idtipodado == "G") {
            $tipo = $obj->idtipodado;
            // $infoList .= '<input type="hidden" class="form-control info-adicional-simples valor-input" id="'.$objEst->cdtipoinformacaoadicional."?".$obj3->id_estruturaclausula.'">';

            $sqlGrupo = "SELECT 
										info.ad_tipoinformacaoadicional_id,
										info.informacaoadicional_no_grupo as id_info,
										info.sequencia
									FROM 
										informacao_adicional_grupo as info
									WHERE 
										ad_tipoinformacaoadicional_id = '{$obj->cdtipoinformacaoadicional}' order by sequencia asc";
            $result = mysqli_query($this->db, $sqlGrupo);

            $infoListGrupo = '<thead id="topo">';

            $infoListGrupoCampo = '<tr id="linha-grupo">';

            $countOption = 1;

            while ($obj2 = $result->fetch_object()) {
              $sqlInfo = "SELECT 
											idtipodado, 
											nmtipoinformacaoadicional, 
											cdtipoinformacaoadicional 
										FROM 
											ad_tipoinformacaoadicional
										WHERE cdtipoinformacaoadicional = '{$obj2->id_info}';
							";

              $resultInfo = mysqli_query($this->db, $sqlInfo);
              $objInfoName = $resultInfo->fetch_object();

              $infoListGrupo .= '<th class="title">' . $objInfoName->nmtipoinformacaoadicional . '</th>';

              if ($objInfoName->idtipodado == "D") {
                $infoListGrupoCampo .= '<td><input type="text" class="form-control info-adicional-grupo-update sequencia' . $countOption . ' date-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj->cdtipoinformacaoadicional . "*data" . '" placeholder="00/00/0000"></td>'; //
              } elseif ($objInfoName->idtipodado == "N") {
                $infoListGrupoCampo .= '<td><input type="number" class="form-control info-adicional-grupo-update sequencia' . $countOption . ' number-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj->cdtipoinformacaoadicional . "*numerico" . '"></td>';
              } elseif ($objInfoName->idtipodado == "L") {
                $infoListGrupoCampo .= '<td><input type="text" class="form-control info-adicional-grupo-update sequencia' . $countOption . ' percent-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj->cdtipoinformacaoadicional . "*percentual" . '"></td>'; //
              } elseif ($objInfoName->idtipodado == "H") {
                $infoListGrupoCampo .= '<td><input type="text" placeholder="hhh:mm:ss" class="form-control info-adicional-grupo-update sequencia' . $countOption . ' hora-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj->cdtipoinformacaoadicional . "*hora" . '"></td>'; //
              } elseif ($objInfoName->idtipodado == "T") {
                $infoListGrupoCampo .= '<td><input type="text" class="form-control info-adicional-grupo-update sequencia' . $countOption . ' text-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj->cdtipoinformacaoadicional . "*texto" . '"></td>'; //
              } elseif ($objInfoName->idtipodado == "V") {
                $infoListGrupoCampo .= '<td><input type="text" class="form-control info-adicional-grupo-update sequencia' . $countOption . ' valor-input" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj->cdtipoinformacaoadicional . "*numerico" . '"></td>'; //
              } elseif ($objInfoName->idtipodado == "C") {
                $infoListGrupoCampo .= '<td><select class="form-control info-adicional-grupo-update sequencia' . $countOption . ' combo" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj->cdtipoinformacaoadicional . "*combo" . '">';

                $sqlCombo = "SELECT * FROM informacao_adicional_combo WHERE ad_tipoinformacaoadicional_id = {$objInfoName->cdtipoinformacaoadicional}";
                $resultCombo = mysqli_query($this->db, $sqlCombo);
                $obj = $resultCombo->fetch_object();
                $optionArray = explode(", ", $obj2->options);

                for ($i = 0; $i < count($optionArray); $i++) {
                  $infoListGrupoCampo .= '<option class="option' . $countOption . '" value="' . $optionArray[$i] . '">' . $optionArray[$i] . '</option>';
                }
                $infoListGrupoCampo .= '</select></td>';
              } elseif ($objInfoName->idtipodado == "CM") {
                $infoListGrupoCampo .= '<td><select multiple data-placeholder="Selecione" class="form-control info-adicional-grupo-update sequencia' . $countOption . ' combo-multiplo chzn-select" tabindex="8" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj->cdtipoinformacaoadicional . "*combo" . '">';

                $sqlCombo = "SELECT * FROM informacao_adicional_combo WHERE ad_tipoinformacaoadicional_id = $objInfoName->cdtipoinformacaoadicional";
                $resultCombo = mysqli_query($this->db, $sqlCombo);
                $obj = $resultCombo->fetch_object();

                $optionArray = explode(", ", $obj2->options);

                for ($i = 0; $i < count($optionArray); $i++) {
                  $infoListGrupoCampo .= '<option class="option' . $countOption . '" value="' . $optionArray[$i] . '">' . $optionArray[$i] . '</option>';
                }
                $infoListGrupoCampo .= '</select></td>';
              } else {
                $infoListGrupoCampo .= '<td><textarea class="form-control info-adicional-grupo-update sequencia' . $countOption . '" id="' . $objInfoName->cdtipoinformacaoadicional . "?" . $obj->cdtipoinformacaoadicional . "*descricao" . '" rows="1"></textarea></td>'; //
              }
              $countOption++;
              // }
            }

            $infoListGrupoCampo .= '</tr>';

            $infoListGrupo .= '</thead>';
            $infoListGrupo .= '<tbody id="table-info-grupo">';
            $infoListGrupo .= '</tbody>';


            // $btnEdit = '<button id="btn-edit" href="#modalInformacaoTipoGrupo" data-toggle="modal" data-dismiss="modal" style="color: #4f8edc; border:none; background-color: transparent;" type="button"><i class="fa fa-pencil"></i></button>';

          } else {
            $infoList .= '<textarea class="form-control info-adicionalCampo" id="' . $obj->cdtipoinformacaoadicional . '" rows="1"></textarea>'; //
          }

          $infoList .= '</td>';
          $infoList .= '<td style="height: 55px; display: flex; justify-content:center; align-items:center;">';
          $infoList .= '<button style="color: red; border:none; background-color: transparent;" type="button" onclick="deleteInfo(' . $obj->cdtipoinformacaoadicional . ')"><i class="fa fa-times"></i></button>';
          $infoList .= '<tr>';

          $countOption++;
        }
        // }

        $response['response_data']['infoListUpdate'] = $infoList;
        $response['response_data']['optionInfoToUpdate'] = $optionInfo;
        $response['response_data']['tabelaGrupo'] = $infoListGrupo;
        $response['response_data']['infoListGrupoCampo'] = $infoListGrupoCampo;
        $response['response_data']['tipoInfo'] = $tipo;
      }
    }

    return $response;
  }

  function aprovacao($data = null)
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

        $id = $data['id_clau'];
        $user = $data['id_user'];

        $now = (new DateTime('now'))->format("Y-m-d");

        $sql = "UPDATE clausula_geral 
						SET aprovador = {$user},
							data_aprovacao = '{$now}'
						WHERE id_clau = {$id}
				";


        if (!mysqli_query($this->db, $sql)) {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Erro ao registrar aprovador!';

        } else {
          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Registro aprovado!';
        }

      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }


    return $response;
  }

  function insertInto($table, $columns, $values)
  {
    $insert = "INSERT INTO {$table}  ({$columns}) VALUES ({$values})";

    return $insert;
  }

  function save($query, $line)
  {
    if (!mysqli_query($this->db, $query)) {

      $response['response_status']['status'] = 0;
      $response['response_status']['error_code'] = $this->error_code . $line;
      $response['response_status']['msg'] = 'Operação INSERT não realizada!';
      $response['response_status']['db_error'] = $this->db->error;
    } else {
      $response['response_status']['status'] = 1;
      $response['response_status']['error_code'] = null;
      $response['response_status']['msg'] = 'Cadastro realizado com sucesso';
    }

    return $response;
  }

  function liberaDoc($data = null)
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

        $sql = "UPDATE clausula_geral
													SET liberado = 'S'
													WHERE doc_sind_id_documento = '{$data['id_doc']}'";

        if (!mysqli_query($this->db, $sql)) {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Não foi possível liberar o documento!';
          $response['response_status']['db_error'] = $this->db->error;
          $response['response_status']['icon'] = 'error';
        } else {
          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = null;
          $response['response_status']['msg'] = 'Documento liberado com sucesso!';
          $response['response_status']['icon'] = 'success';
        }

        //BUSCANDO FILIAIS
        $sql = "SELECT 
						cg.id_clau,
						cg.data_aprovacao,
						cg.doc_sind_id_documento as id_doc,
						IFNULL(GROUP_CONCAT(DISTINCT de.sind_emp_id_sinde), GROUP_CONCAT(IFNULL( de.sind_emp_id_sinde,null))) as id_sinde,
						IFNULL(GROUP_CONCAT(DISTINCT dp.sind_patr_id_sindp), GROUP_CONCAT(IFNULL( dp.sind_patr_id_sindp,null))) as id_sp
					FROM clausula_geral as cg
					LEFT JOIN doc_sind as doc on doc.id_doc = cg.doc_sind_id_documento
					LEFT JOIN doc_sind_sind_emp as de on de.doc_sind_id_doc = doc.id_doc
					LEFT JOIN doc_sind_sind_patr as dp on dp.doc_sind_id_doc = doc.id_doc
					WHERE cg.doc_sind_id_documento = {$data['id_doc']}
			";


        $obj = mysqli_query($this->db, $sql)->fetch_object();

        $idDoc = $obj->id_doc;

        if ($obj->id_sinde) {
          $sqlEmp = "SELECT 
								IFNULL(GROUP_CONCAT(DISTINCT classe_cnae_idclasse_cnae), GROUP_CONCAT(IFNULL( classe_cnae_idclasse_cnae,null))) as cnae

							FROM base_territorialsindemp
							WHERE sind_empregados_id_sinde1 IN ({$obj->id_sinde})
				";


          $objEmp = mysqli_query($this->db, $sqlEmp)->fetch_object();

          $cnaeEmp = $objEmp->cnae;
        } else {
          $cnaeEmp = 0;
        }


        if ($obj->id_sp) {
          $sqlPatr = "SELECT 
								IFNULL(GROUP_CONCAT(DISTINCT classe_cnae_idclasse_cnae), GROUP_CONCAT(IFNULL( classe_cnae_idclasse_cnae,null))) as cnae

							FROM base_territorialsindpatro
							WHERE sind_patronal_id_sindp IN ({$obj->id_sp})
				";

          $objPatr = mysqli_query($this->db, $sqlPatr)->fetch_object();

          $cnaePatr = $objPatr->cnae;
        } else {
          $cnaePatr = 0;
        }


        //UNIDADES ABRANGIDAS PELA CLAUSULA
        $sqlCnae = "SELECT 
							IFNULL(GROUP_CONCAT(DISTINCT cliente_unidades_id_unidade), GROUP_CONCAT(IFNULL( cliente_unidades_id_unidade,null))) as clt
						FROM cnae_emp
						WHERE classe_cnae_idclasse_cnae IN ({$cnaeEmp},{$cnaePatr})
						ORDER BY clt ASC
			";

        $objClt = explode(",", mysqli_query($this->db, $sqlCnae)->fetch_object()->clt);

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
                array_push($listaDeEmails, $datas);
              }
            }
          }
        }

        array_push($listaDeEmails, "lcs.rodrigues16@hotmail.com");
        array_push($listaDeEmails, "paulo@prccbusiness.com");


        //VERIFICA SE TODAS AS CLÁUSULAS DO DOC FORAMA PROVADAS PARA EFETUAR ODISPARO DE E-MAILS
        $sqlVerify = "SELECT 
							(SELECT count(*) FROM clausula_geral  WHERE cg.doc_sind_id_documento = doc_sind_id_documento) as todas
							,(SELECT count(data_aprovacao) FROM clausula_geral  WHERE data_aprovacao IS NOT NULL AND data_aprovacao != '0000-00-00' AND cg.doc_sind_id_documento = doc_sind_id_documento) as aprovado
							
						FROM	clausula_geral as cg 
						WHERE cg.doc_sind_id_documento = '{$idDoc}'
						GROUP BY cg.doc_sind_id_documento
			";

        $objVerify = mysqli_query($this->db, $sqlVerify)->fetch_object();

        if ($objVerify->aprovado != $objVerify->todas) {

          $response['response_status']['msg'] = 'Ainda faltam ' . ($objVerify->todas - $objVerify->aprovado) . ' Cláuslas para serem aprovadas';
          $response['response_status']['icon'] = 'warning';
          $response['response_status']['status'] = 0;
        } else {

          //BUSCANDO DADOS DO DOCUMENTO/CLAUSULA
          $sql = "SELECT 
							tp.nome_doc,
							CONCAT(DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y'), ' - ', DATE_FORMAT(doc.validade_final, '%d/%m/%Y')) as periodo,
							cs.descricao_subclasse,
							IFNULL(GROUP_CONCAT(DISTINCT CONCAT(loc.municipio, '-', loc.uf)), GROUP_CONCAT(IFNULL( CONCAT(loc.municipio, '-', loc.uf),null))) as abrangencia,
							IFNULL(GROUP_CONCAT(DISTINCT sinde.denominacao_sinde), GROUP_CONCAT(IFNULL( sinde.denominacao_sinde,null))) as laboral,
							IFNULL(GROUP_CONCAT(DISTINCT sp.denominacao_sp), GROUP_CONCAT(IFNULL( sp.denominacao_sp,null))) as patronal
						FROM clausula_geral as cg
						LEFT JOIN doc_sind as doc on doc.id_doc = cg.doc_sind_id_documento
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
						LEFT JOIN classe_cnae_doc_sind as cn on cn.doc_sind_id_doc = doc.id_doc
						LEFT JOIN classe_cnae as cs on cs.id_cnae = cn.classe_cnae_id_cnae
						LEFT JOIN abrang_docsind as abr on abr.doc_sind_id_documento = doc.id_doc
						LEFT JOIN localizacao as loc on loc.id_localizacao = abr.localizacao_id_localizacao
						LEFT JOIN doc_sind_sind_emp as de on de.doc_sind_id_doc = doc.id_doc
						LEFT JOIN doc_sind_sind_patr as dp on dp.doc_sind_id_doc = doc.id_doc
						LEFT JOIN sind_emp as sinde on sinde.id_sinde = de.sind_emp_id_sinde
						LEFT JOIN sind_patr as sp on sp.id_sindp = dp.sind_patr_id_sindp
						WHERE cg.doc_sind_id_documento = {$data['id_doc']}
				";
          $obj = mysqli_query($this->db, $sql)->fetch_object();

          $message = "
					<div>
						<p>Olá,</p>
						<br>
						<p>Informamos que os módulos Cláusulas, Calendário sindical e Mapa Sindical do documento <b>{$obj->nome_doc}</b> descrito abaixo estão disponíveis para consulta.</p>
						<br>
						<p><b>Documento Sindical: </b>{$obj->nome_doc}</p>
						<p><b>Período documento sindical: </b>{$obj->periodo}</p>
						<p><b>Atividade Econômica: </b>{$obj->descricao_subclasse}</p>
						<p><b>Abrangência: </b>{$obj->abrangencia}</p>
						<p><b>Sindicato(s) Laboral(is): </b>{$obj->laboral}</p>
						<p><b>Sindicato(s) Patronal(is): </b>{$obj->patronal}</p>
						<br>
						<p>Em caso de dúvidas, entre em contato conosco através dos canais: </p>
						<br>
						<p>Atenciosamente,</p>
						<p><b>Ineditta Consultoria Sindical</b></p>
					</div>
				";

          $listaDestino = implode(",", $listaDeEmails);
          $mail = new disparo_email();
          $response['response_email'] = $mail->dispararEmails([
            "email_remetente" => "no-reply@ineditta.com.br",
            "senha" => "oysnduyjipawezbl",
            "cripto" => "tls",
            "porta" => 587,
            "nome" => "Ineditta Sistema",
            "to_multi" => $listaDestino,
            "assunto" => "Cláusulas Aprovadas",
            "msg" => $message
          ]);

          $response['response_data']['destinatarios'] = $listaDeEmails;
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
