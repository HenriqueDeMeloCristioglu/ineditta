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

include_once __DIR__ . "/helpers.php";
include_once __DIR__ . "/class.disparoEmail.php";
// date_default_timezone_set('America/Sao_Paulo');
class docsind
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
        Logger::configure($fileLogConfig);
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

  function getDocSind($data = null)
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
							ds.id_doc
							,tp.nome_doc
							,ds.uf
              ,REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(ds.cnae_doc, '$[*].id') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  AS id_cnae
              ,REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(ds.cnae_doc, '$[*].subclasse') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  AS descricao_subclasse
							,ds.validade_final
							,ds.data_assinatura
							,ds.data_reg_mte
							,REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(ds.sind_laboral, '$[*].sigla') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  AS sind_emp
							,REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(ds.sind_patronal, '$[*].sigla') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  AS sind_patr
							,ds.data_aprovacao
							,user.nome_usuario
						FROM 
							doc_sind as ds
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = ds.tipo_doc_idtipo_doc
						LEFT JOIN usuario_adm as user on user.id_user = ds.usuario_responsavel
						WHERE tp.processado = 'S'
						GROUP BY ds.id_doc
					";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;

          while ($obj = $resultsql->fetch_object()) {
            $html .= '<tr class="odd gradeX tbl-item">';
            $html .= '<td><a data-toggle="modal" href="#updateModal" onclick="getByIdDocSind( ' . $obj->id_doc . ');" class="btn-default-alt"  id="bootbox-demo-5"><i class="fa fa-file-text"></i></a></td>';
            $html .= '<td class="title">';
            $html .= $obj->id_doc;
            $html .= '</td>';
            $html .= '<td class="desc">';
            $html .= $obj->nome_doc;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->id_cnae;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->descricao_subclasse;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->sind_emp;
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->sind_patr;
            $html .= '</td>';
            $html .= '<td>';
            $html .= ($obj->validade_final == "0000-00-00" || $obj->validade_final == null ? "--" : date('d/m/Y', strtotime($obj->validade_final)));
            $html .= '</td>';
            $html .= '<td>';
            $html .= ($obj->data_aprovacao == "0000-00-00" || $obj->data_aprovacao == null ? "--" : date('d/m/Y', strtotime($obj->data_aprovacao)));
            $html .= '</td>';
            $html .= '<td>';
            $html .= $obj->nome_usuario ? ucwords($obj->nome_usuario) : "--";
            $html .= '</td>';
            $html .= '</tr>';

          }

          $response['response_data']['html'] = $html;

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

  function getDocSindCampos($data = null)
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

        //LISTA CLIENTE UNIDADE
        $sqlClt = "SELECT 
								cu.id_unidade
								,gp.nome_grupoeconomico
								,cm.nome_empresa
								,cu.nome_unidade
							FROM 
								cliente_unidades cu
							LEFT JOIN cliente_matriz cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							LEFT JOIN cliente_grupo as gp ON gp.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
				";

        $resultClt = mysqli_query($this->db, $sqlClt);

        $html = null;
        $selectCltUn = null;

        while ($obj = $resultClt->fetch_object()) {

          $selectCltUn .= "<tr class='tbl-item'>";
          $selectCltUn .= "<td><input type='checkbox' id='emp{$obj->id_unidade}' onclick='selectEmpresa(1,{$obj->id_unidade})'</td>";
          $selectCltUn .= "<td>{$obj->nome_grupoeconomico}</td>";
          $selectCltUn .= "<td class='desc'>{$obj->nome_empresa}</td>";
          $selectCltUn .= "<td class='title'>{$obj->nome_unidade}</td>";
          $selectCltUn .= "</tr>";

        }

        $response['response_data']['filial'] = $selectCltUn;

        //LISTA CNAE
        $sql = "SELECT 
							id_cnae
                            ,descricao_subclasse
							,categoria
						FROM 
							classe_cnae;								
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;

          while ($obj = $resultsql->fetch_object()) {

            // $html .= "<option value='{$obj->id_cnae}'>{$obj->descricao_subclasse} / {$obj->categoria}</option>";
            $html .= "<tr class='tbl-item'>";
            $html .= "<td><input type='checkbox' id='cnae{$obj->id_cnae}' onclick='selectCnae(1, {$obj->id_cnae})'</td>";
            $html .= "<td class='title'>{$obj->descricao_subclasse}</td>";
            $html .= "<td class='desc'>{$obj->categoria}</td>";
            $html .= "</tr>";

          }

          $response['response_data']['listaCnae'] = $html;

        } else {

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
						FROM sind_emp as sinde

						LEFT JOIN base_territorialsindemp as bem on bem.sind_empregados_id_sinde1 = sinde.id_sinde
						
						WHERE bem.sind_empregados_id_sinde1 = sinde.id_sinde
						GROUP BY sinde.id_sinde
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          while ($obj = $resultsql->fetch_object()) {
            $cnpj = formatCnpjCpf($obj->cnpj_sinde);
            $html .= "<option value='{$obj->id_sinde}'>{$obj->sigla_sinde} / {$obj->denominacao_sinde} / {$cnpj}</option>";

          }

          $response['response_data']['listaSindEmp'] = $html;

        } else {

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
						FROM sind_patr as sp
						LEFT JOIN base_territorialsindpatro as bp on bp.sind_patronal_id_sindp = sp.id_sindp

						WHERE bp.sind_patronal_id_sindp = sp.id_sindp

						GROUP BY sp.id_sindp
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $html = null;
          while ($obj = $resultsql->fetch_object()) {
            $cnpj = formatCnpjCpf($obj->cnpj_sp);
            $html .= "<option value='{$obj->id_sindp}'>{$obj->sigla_sp} / {$obj->denominacao_sp} / {$cnpj}</option>";

          }

          $response['response_data']['listaPatronal'] = $html;
        } else {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        // OPTION LOCALIZAÇÃO
        $sql = "SELECT 
							id_localizacao
						    ,municipio
						FROM 
							localizacao								
				";

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
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }


        // OPTION UNIDADE CLIENTE
        $sql = "SELECT 
							*
						FROM 
							tipounidade_cliente									
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = null;
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="' . $obj->id_tiponegocio . '">';
            $grupos .= $obj->tipo_negocio;
            $grupos .= '</option>';
          }

          $response['response_data']['unidadeCliente'] = $grupos;
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        // OPTION TIPO DOCUMENTO
        $sql = "SELECT 
							DISTINCT idtipo_doc,
							sigla_doc,
							nome_doc,
							processado
						FROM 
							tipo_doc
						WHERE processado = 'S'
						ORDER BY nome_doc ASC									
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $grupos = "<option value=''></option>";
          while ($obj = $resultsql->fetch_object()) {
            $grupos .= '<option value="' . $obj->idtipo_doc . '" data-processado="' . $obj->processado . '">';
            $grupos .= $obj->nome_doc;
            $grupos .= '</option>';
          }

          $response['response_data']['tipoDoc'] = $grupos;
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        // LISTA DE DOCUMENTOS (ARQUIVOS)
        $sql = "SELECT 
							id_documento,
							nome_documento,
							caminho,
							origem,
							date_format(data_registro, '%d/%m/%Y - %H:%i:%s') as data_registro
						FROM 
						documentos_localizados
						WHERE situacao = 'não aprovado'
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $lista = null;
          while ($obj = $resultsql->fetch_object()) {
            $btnId = 'see' . $obj->id_documento . '';
            $embedId = 'embed_pdf';
            $onClick = "onclick=seeFile('{$btnId}','embed_pdf',{$obj->id_documento})";
            $path = $obj->caminho;
            $lista .= '<tr class="tbl-item">';
            $lista .= '<td class="title">' . $obj->nome_documento . '</td>';
            $lista .= '<td>' . $obj->origem . '</td>';
            $lista .= '<td class="desc">' . $obj->data_registro . '</td>';
            $lista .= "<td><button id='{$btnId}' data-path='" . $path . "' type='button' class='btn btn-primary' {$onClick} title='Visualizar arquivo'><i id='icon" . $obj->id_documento . "' style='font-size: 1.2em;'' class='fa fa-eye eye_list'></i></button>";
            $lista .= '<button type="button" class="btn btn-danger" onclick="deleteFile(' . $obj->id_documento . ')" title="Excluir arquivo" style="margin: 0 10px;"><i style="font-size: 1.2em;" class="fa fa-trash-o"></i></button>';
            $lista .= '<button type="button" class="btn btn-success" onclick="approveFile(' . $obj->id_documento . ')" title="Aprovar arquivo"><i style="font-size: 1.2em;" class="fa fa-check-square-o"></i></button></td>';
            $lista .= '</tr>';
          }

          $response['response_data']['listaArquivosDoc'] = $lista;
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        // LISTA DE DOCUMENTOS (ARQUIVOS)
        $sql = "SELECT 
							id_documento,
							nome_documento,
							caminho,
							origem,
							date_format(data_registro, '%d/%m/%Y') as data_registro,
							date_format(data_aprovacao, '%d/%m/%Y') as aprovacao
						FROM 
							documentos_localizados
						WHERE situacao = 'aprovado' AND referenciado = 'não'
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {

          $opt = null;
          $opt .= "<option value='--'>--</option>";
          while ($obj = $resultsql->fetch_object()) {
            $opt .= "<option value='{$obj->id_documento}'>{$obj->nome_documento} / Aprovação: {$obj->aprovacao}</option>";

          }

          $response['response_data']['referenceList'] = $opt;
        } else {
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

  function getByIdDocSind($data = null)
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
							ds.id_doc
							,tp.nome_doc
							,tp.idtipo_doc
							,tp.sigla_doc
							,ds.uf
							,ds.versao_documento
							,ds.origem
							,ds.numero_solicitacao_mr
							,ds.num_reg_mte
							,ds.validade_inicial
							,ds.validade_final
							,ds.prorrogacao_doc
							,ds.data_assinatura
							,ds.data_reg_mte  
							,ds.permissao
							,ds.observacao
							,ds.tipounidade_cliente_id_tiponegocio,
							dc.nome_documento,
							dc.id_documento,
							dc.caminho,
							ds.doc_restrito,
							ds.database_doc,
							ds.usuario_responsavel,
							ds.cliente_estabelecimento,
							ds.referencia,
							ds.sind_laboral,
							ds.sind_patronal,
							ds.cnae_doc,
							ds.abrangencia
						FROM 
							doc_sind as ds
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = ds.tipo_doc_idtipo_doc
						LEFT JOIN documentos_localizados as dc on dc.id_documento = ds.documento_id_documento
						WHERE ds.id_doc = '{$data['id_doc']}'
						GROUP BY ds.id_doc
				";

        if ($resultsql = mysqli_query($this->db, $sql)) {
          $obj = $resultsql->fetch_object();
          $response['response_data']['id'] = $obj->id_doc;
          $response['response_data']['siglaDoc'] = $obj->sigla_doc;
          $response['response_data']['origem'] = $obj->origem;
          $response['response_data']['versao'] = $obj->versao_documento;
          $response['response_data']['numero'] = $obj->numero_solicitacao_mr;
          $response['response_data']['data_reg'] = ($obj->data_reg_mte == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($obj->data_reg_mte)));
          $response['response_data']['num_reg'] = $obj->num_reg_mte;
          $response['response_data']['vini'] = ($obj->validade_inicial == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($obj->validade_inicial)));
          $response['response_data']['vfim'] = ($obj->validade_final == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($obj->validade_final)));
          $response['response_data']['pro'] = ($obj->prorrogacao_doc == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($obj->prorrogacao_doc)));
          $response['response_data']['ass'] = ($obj->data_assinatura == "0000-00-00" ? "00/00/0000" : date('d/m/Y', strtotime($obj->data_assinatura)));
          $response['response_data']['permite'] = $obj->permissao;
          $response['response_data']['observa'] = $obj->observacao;
          $response['response_data']['documento'] = $obj->nome_documento;
          $response['response_data']['id_documento'] = $obj->id_documento;
          $response['response_data']['doc_restrito'] = $obj->doc_restrito;
          $response['response_data']['data_base'] = $obj->database_doc;
          $response['response_data']['path'] = $obj->caminho;
          $response['response_data']['usuario'] = $obj->usuario_responsavel;

          //JSON COLUMNS
          $clt = is_array(json_decode($obj->cliente_estabelecimento)) ? json_decode($obj->cliente_estabelecimento) : [];
          $referencia = is_array(json_decode($obj->referencia)) ? json_decode($obj->referencia) : [];
          $emp = is_array(json_decode($obj->sind_laboral)) ? json_decode($obj->sind_laboral) : [];
          $patr = is_array(json_decode($obj->sind_patronal)) ? json_decode($obj->sind_patronal) : [];
          $cnae = is_array(json_decode($obj->cnae_doc)) ? json_decode($obj->cnae_doc) : [];
          array_multisort($cnae);
          $abrangDoc = is_array(json_decode($obj->abrangencia)) ? json_decode($obj->abrangencia) : [];

          $newCnae = [];
          foreach ($cnae as $value) {
            array_push($newCnae, $value->id);
          }

          $uf = $obj->uf;
          //LISTA LABORAL

          $optEmp = "";
          $sqlEmp = 'SELECT id_sinde, denominacao_sinde, cnpj_sinde FROM sind_emp';
          $resultEmp = mysqli_query($this->db, $sqlEmp);

          for ($i = 0; $i <= count($emp); $i++) {
            while ($objEmp = $resultEmp->fetch_object()) {
              $cnpj = formatCnpjCpf($objEmp->cnpj_sinde);

              if (key_exists($i, $emp)) {
                if ($emp[$i]->id == $objEmp->id_sinde) {

                  $optEmp .= "<option value='{$objEmp->id_sinde}' selected='selected'>{$objEmp->denominacao_sinde} / {$cnpj}</option>";
                  break;
                } else {
                  $optEmp .= "<option value='{$objEmp->id_sinde}'>{$objEmp->denominacao_sinde} / {$cnpj}</option>";
                }
              } else {
                $optEmp .= "<option value='{$objEmp->id_sinde}'>{$objEmp->denominacao_sinde} / {$cnpj}</option>";
              }

            }
          }

          $response['response_data']['listaLaboral'] = $optEmp;

          //LISTA PATRONAL
          $optPatr = "";
          $sqlPatr = 'SELECT id_sindp, denominacao_sp, cnpj_sp FROM sind_patr';
          $resultPatr = mysqli_query($this->db, $sqlPatr);

          for ($i = 0; $i <= count($patr); $i++) {
            while ($objPatr = $resultPatr->fetch_object()) {
              $cnpj = formatCnpjCpf($objPatr->cnpj_sp);

              if (key_exists($i, $patr)) {
                if ($patr[$i]->id == $objPatr->id_sindp) {
                  $optPatr .= "<option value='{$objPatr->id_sindp}' selected='selected'>{$objPatr->denominacao_sp} / {$cnpj}</option>";
                  break;
                } else {
                  $optPatr .= "<option value='{$objPatr->id_sindp}'>{$objPatr->denominacao_sp} / {$cnpj}</option>";
                }
              } else {
                $optPatr .= "<option value='{$objPatr->id_sindp}'>{$objPatr->denominacao_sp} / {$cnpj}</option>";
              }

            }
          }
          $response['response_data']['listaPatronal'] = $optPatr;

          //LISTA CLIENTE UNIDADES
          $optClt = "";
          $sqlClt = 'SELECT 
								cu.id_unidade
								,gp.nome_grupoeconomico
								,cm.nome_empresa
								,cu.nome_unidade
							FROM 
								cliente_unidades cu
							LEFT JOIN cliente_matriz cm ON cm.id_empresa = cu.cliente_matriz_id_empresa
							LEFT JOIN cliente_grupo as gp ON gp.id_grupo_economico = cm.cliente_grupo_id_grupo_economico';
          $resultClt = mysqli_query($this->db, $sqlClt);
          // $selected = 'no_selected';
          $listaClt = [];
          $idsEmpresa = "";
          for ($i = 0; $i < count($clt); $i++) {
            while ($objClt = $resultClt->fetch_object()) {

              if ($objClt->id_unidade == $clt[$i]->u) {
                $campo = "<input type='checkbox' id='empUp{$objClt->id_unidade}' onclick='selectEmpresa(2,{$objClt->id_unidade})' checked>";
                $idsEmpresa .= " " . $objClt->id_unidade;
              } else {
                $campo = "<input type='checkbox' id='empUp{$objClt->id_unidade}' onclick='selectEmpresa(2,{$objClt->id_unidade})'>";
              }

              $lista = new stdClass();
              $lista->campo = $campo;
              $lista->nome = $objClt->nome_unidade;
              $lista->empresa = $objClt->nome_empresa;
              $lista->grupo = $objClt->nome_grupoeconomico;

              array_push($listaClt, $lista);

            }
          }


          $response['response_data']['lista_clt'] = $listaClt;
          $response['response_data']['ids_empresa'] = $idsEmpresa;

          //LISTA TIPO UNIDADE CLIENTE
          $tipoUnidadeCliente = explode(",", $obj->tipounidade_cliente_id_tiponegocio);
          $optTipoUn = "";
          $sqlTipo = 'SELECT id_tiponegocio, tipo_negocio FROM tipounidade_cliente';
          $resultTipo = mysqli_query($this->db, $sqlTipo);

          for ($i = 0; $i <= count($tipoUnidadeCliente); $i++) {
            while ($objTipo = $resultTipo->fetch_object()) {


              if (key_exists($i, $tipoUnidadeCliente)) {
                if ($tipoUnidadeCliente[$i] == $objTipo->id_tiponegocio) {
                  $optTipoUn .= '<option value="' . $objTipo->id_tiponegocio . '" selected="selected">' . $objTipo->tipo_negocio . '</option>';
                  break;
                } else {
                  $optTipoUn .= '<option value="' . $objTipo->id_tiponegocio . '">' . $objTipo->tipo_negocio . '</option>';
                }
              } else {
                $optTipoUn .= '<option value="' . $objTipo->id_tiponegocio . '">' . $objTipo->tipo_negocio . '</option>';
              }

            }
          }

          $response['response_data']['listaTipoUnidade'] = $optTipoUn;

          //LISTA TIPO DOCUMENTO
          $tipoDocumento = explode(",", $obj->idtipo_doc);
          $optTipoDoc = "";
          $sqlTipoDoc = 'SELECT idtipo_doc, tipo_doc, processado, nome_doc FROM tipo_doc WHERE processado = "S"';
          $resultTipoDoc = mysqli_query($this->db, $sqlTipoDoc);

          for ($i = 0; $i <= count($tipoDocumento); $i++) {
            while ($objTipoDoc = $resultTipoDoc->fetch_object()) {


              if (key_exists($i, $tipoDocumento)) {
                if ($tipoDocumento[$i] == $objTipoDoc->idtipo_doc) {
                  $optTipoDoc .= '<option value="' . $objTipoDoc->idtipo_doc . '" selected="selected" data-processado="' . $objTipoDoc->processado . '">' . $objTipoDoc->nome_doc . '</option>';
                  break;
                } else {
                  $optTipoDoc .= '<option value="' . $objTipoDoc->idtipo_doc . '" data-processado="' . $objTipoDoc->processado . '">' . $objTipoDoc->nome_doc . '</option>';
                }
              } else {
                $optTipoDoc .= '<option value="' . $objTipoDoc->idtipo_doc . '" data-processado="' . $objTipoDoc->processado . '">' . $objTipoDoc->nome_doc . '</option>';
              }

            }
          }

          $response['response_data']['listaTipoDoc'] = $optTipoDoc;

          //LISTA CNAE
          $optCnae = "";
          $sqlCnae = 'SELECT id_cnae, descricao_subclasse, categoria FROM classe_cnae';
          $resultCnae = mysqli_query($this->db, $sqlCnae);
          $listaCnae = [];
          $idsCnae = "";

          while ($objCnae = $resultCnae->fetch_object()) {
            if (in_array($objCnae->id_cnae, $newCnae)) {
              $input = "<input type='checkbox' id='cnaeUp{$objCnae->id_cnae}' onclick='selectCnae(2,{$objCnae->id_cnae})' checked>";
              $idsCnae .= " " . $objCnae->id_cnae;
            } else {
              $input = "<input type='checkbox' id='cnaeUp{$objCnae->id_cnae}' onclick='selectCnae(2,{$objCnae->id_cnae})'>";
            }

            $lista = new stdClass();
            $lista->campo = $input;
            $lista->subclasse = $objCnae->descricao_subclasse;
            $lista->categoria = $objCnae->categoria;

            array_push($listaCnae, $lista);
          }

          $response['response_data']['lista_cnae'] = $listaCnae;
          $response['response_data']['ids_cnae'] = $idsCnae;
          $response['response_data']['id_referenciamento'] = $referencia;

          //Obtendo lista de clausulas
          $sqlEst = "SELECT id_estruturaclausula, nome_clausula FROM estrutura_clausula"; //WHERE id_estruturaclausula = '{$obj->estrutura_clausula_id_estruturaclausula}'
          $resultEst = mysqli_query($this->db, $sqlEst);

          $optEst = "";
          while ($objEst = $resultEst->fetch_object()) {

            $optEst .= "<option value='{$objEst->id_estruturaclausula}'>{$objEst->nome_clausula}</option>";
          }

          $response['response_data']['lista_referenciamento'] = $optEst;

          $arr = [];
          for ($i = 0; $i < count($emp); $i++) {
            array_push($arr, $emp[$i]->id);
          }

          $idEmp = "'" . implode("','", $arr) . "'";

          $sql = "SELECT
								IFNULL(GROUP_CONCAT(DISTINCT be.localizacao_id_localizacao1), GROUP_CONCAT(IFNULL( be.localizacao_id_localizacao1, null))) as base_emp
								
							FROM base_territorialsindemp as be
							WHERE be.sind_empregados_id_sinde1 IN ({$idEmp})
							
					";

          $result = mysqli_query($this->db, $sql);

          $obj = $result->fetch_object();

          $sindEmp = $emp;
          $sindPatr = $patr;
          $baseEmp = explode(",", $obj->base_emp);

          $listaBasePatr = [];
          for ($i = 0; $i < count($sindPatr); $i++) {
            $sqlBasePatr = "SELECT 
										IFNULL(GROUP_CONCAT(DISTINCT localizacao_id_localizacao1), GROUP_CONCAT(IFNULL( localizacao_id_localizacao1, null))) as base_patr
									FROM base_territorialsindpatro
									WHERE sind_patronal_id_sindp = '{$sindPatr[$i]->id}'
						";

            $resultPatr = mysqli_query($this->db, $sqlBasePatr);
            $obj = $resultPatr->fetch_object();

            $array = explode(",", $obj->base_patr);
            $listaBasePatr = array_merge($listaBasePatr, $array);

          }

          //LOCALIZAÇÕES ABRANGENCIA
          $arrayAbrang = [];
          $idsAbrang = "";
          for ($i = 0; $i < count($abrangDoc); $i++) {
            $sqlAbrang = "SELECT 
										id_localizacao,
										uf,
										municipio
									FROM localizacao
									WHERE id_localizacao = '{$abrangDoc[$i]->id}'
						";

            $resultAbrang = mysqli_query($this->db, $sqlAbrang);

            $idsAbrang .= " " . $abrangDoc[$i]->id;

            while ($obj = $resultAbrang->fetch_object()) {
              array_push($arrayAbrang, "{$obj->municipio} - {$obj->uf}");
            }
          }

          $response['response_data']['ids_abrang'] = $idsAbrang;

          //LOCALIZAÇÕES BASE EMP
          $arrayEmp = [];
          for ($i = 0; $i < count($baseEmp); $i++) {
            $sqlEmp = "SELECT 
										id_localizacao,
										uf,
										municipio
									FROM localizacao
									WHERE id_localizacao = '{$baseEmp[$i]}'
						";

            $resultEmp = mysqli_query($this->db, $sqlEmp);


            while ($obj = $resultEmp->fetch_object()) {
              array_push($arrayEmp, "{$obj->municipio} - {$obj->uf}");
            }
          }

          //LOCALIZAÇÕES BASE PATR
          $arrayPatr = [];
          for ($i = 0; $i < count($listaBasePatr); $i++) {
            $sqlPatr = "SELECT 
										id_localizacao,
										uf,
										municipio
									FROM localizacao
									WHERE id_localizacao = '{$listaBasePatr[$i]}'
						";

            $resultPatr = mysqli_query($this->db, $sqlPatr);


            while ($obj = $resultPatr->fetch_object()) {
              array_push($arrayPatr, "{$obj->municipio} - {$obj->uf}");
            }
          }

          //COMPARAÇÃO ABRANG COM LISTA EMP E PATR
          for ($i = 0; $i < count($arrayAbrang); $i++) {

            if (in_array($arrayAbrang[$i], $arrayEmp) && in_array($arrayAbrang[$i], $arrayPatr)) {
              //UNSET EMP
              $indexEmp = array_search($arrayAbrang[$i], $arrayEmp);
              unset($arrayEmp[$indexEmp]);

              //UNSET PATR
              $indexPatr = array_search($arrayAbrang[$i], $arrayPatr);
              unset($arrayPatr[$indexPatr]);

              //UNSET ABRANG
//							unset($arrayAbrang[$i]);
            }
          }

          //Lista Abrangencia
          $listaAbrang = "";
          natcasesort($arrayAbrang);
          foreach ($arrayAbrang as $value) {
            $listaAbrang .= "<tr>";
            $listaAbrang .= "<td>{$value}</td>";
            $listaAbrang .= "</tr>";
          }

          $response['response_data']['lista_abrang'] = $listaAbrang;

          //Lista Emp
          $listaEmp = "";
          natcasesort($arrayEmp);
          foreach ($arrayEmp as $value) {
            $listaEmp .= "<tr>";
            $listaEmp .= "<td>{$value}</td>";
            $listaEmp .= "</tr>";
          }

          $response['response_data']['lista_emp'] = $listaEmp;

          //Lista Patr
          $listaPatr = "";
          natcasesort($arrayPatr);
          foreach ($arrayPatr as $value) {
            $listaPatr .= "<tr>";
            $listaPatr .= "<td>{$value}</td>";
            $listaPatr .= "</tr>";
          }

          $response['response_data']['lista_patr'] = $listaPatr;


        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = '';
        }

        //LISTA ABRANGENCIA: ABRANG CADASTRADA
        $listaAb = "";
        for ($i = 0; $i < count($abrangDoc); $i++) {
          $sqlAb = "SELECT id_localizacao, pais, regiao, uf, municipio FROM localizacao WHERE id_localizacao = '{$abrangDoc[$i]->id}'";
          $resultAb = mysqli_query($this->db, $sqlAb);

          while ($obj = $resultAb->fetch_object()) {
            $listaAb .= "<tr class='tbl-item'>";
            $listaAb .= "<td><input type='checkbox' data-doc='{$data['id_doc']}' data-id='{$obj->id_localizacao}'></td>";
            $listaAb .= "<td>{$obj->regiao}</td>";
            $listaAb .= "<td class='desc'>{$obj->uf}</td>";
            $listaAb .= "<td class='title'>{$obj->municipio}</td>";
            $listaAb .= "</tr>";
          }
        }

        $response['response_data']['lista_abrang_cadastrada'] = $listaAb;

        //LISTA DE MUNICIPIOS UPDATE

        $sql2 = "SELECT id_localizacao, pais, regiao, uf, municipio FROM localizacao WHERE uf = 'SP'";
        $resultsql2 = mysqli_query($this->db, $sql2);

        $listaMod = "";

        while ($obj2 = $resultsql2->fetch_object()) {
          $listaMod .= "<tr class='tbl-item'>";
          $listaMod .= '<td class="checkAbrang" id="' . $obj2->id_localizacao . '"><input class="form-check-input checkInput" type="checkbox" value="1" id="' . $obj2->id_localizacao . '"></td>';
          $listaMod .= '<td class="title">';
          $listaMod .= $obj2->municipio;
          $listaMod .= '</td>';
          $listaMod .= '</tr>';
        }


        $response['response_data']['abrangUpdate'] = $listaMod;
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    return $response;
  }

  function addDocSind($data = null)
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

      $dataBase = $data['data_base'];

      $dataIni = ($data['vini-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['vini-input'])))) : "0000-00-00");
      $dataFim = ($data['vfim-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['vfim-input'])))) : "0000-00-00");
      $dataPro = ($data['pro-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['pro-input'])))) : "0000-00-00");
      $dataReg = ($data['data-reg-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['data-reg-input'])))) : "0000-00-00");
      $dataAss = ($data['ass-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['ass-input'])))) : "0000-00-00");




      if ($response['response_status']['status'] == 1) {

        //OBTÉM CLIENTE UNIDADES


        $estabelecimentos = "[]";
        if (trim($data['selectCltUn']) != "") {
          $unidades = explode(" ", trim($data['selectCltUn']));

          $array = [];
          foreach ($unidades as $unidade) {

            $sql = "SELECT id_unidade, cliente_matriz_id_empresa as matriz, cliente_grupo_id_grupo_economico as grupo, nome_unidade FROM cliente_unidades WHERE id_unidade = '{$unidade}'";
            $resObj = mysqli_query($this->db, $sql)->fetch_object();

            array_push($array, "{" . '"g":' . $resObj->grupo . ',"m":' . $resObj->matriz . ',"u":' . $resObj->id_unidade . ',"nome_unidade":' . '"' . $resObj->nome_unidade . '"' . "}");
          }

          $lista = implode(",", $array);
          $estabelecimentos = "[" . $lista . "]";

        } else {
          $cnaes = explode(" ", trim($data['cnae-input']));
          //obtém cnae
          $array = [];
          foreach ($cnaes as $cnae) {
            $id = '{"id":' . $cnae . '}';

            array_push($array, " JSON_CONTAINS(cnae_unidade, '{$id}', '$') ");
          }
          $new = implode(" OR ", $array);
          $sql = "SELECT id_unidade, nome_unidade, cliente_matriz_id_empresa as matriz, cliente_grupo_id_grupo_economico as grupo, cnae_unidade FROM cliente_unidades WHERE {$new}";


          $res = mysqli_query($this->db, $sql);

          //clientes pelo cnae
          $listaUnidades = [];
          while ($obj = $res->fetch_object()) {
            $new = new stdClass();
            $new->u = $obj->id_unidade;
            $new->m = $obj->matriz;
            $new->g = $obj->grupo;
            $new->nome_unidade = $obj->nome_unidade;

            array_push($listaUnidades, $new);
          }
          $string = [];
          foreach ($listaUnidades as $unidade) {
            array_push($string, "{" . '"g":' . $unidade->g . ',"m":' . $unidade->m . ',"u":' . $unidade->u . ',"nome_unidade":' . '"' . $unidade->nome_unidade . '"' . "}");

          }

          $lista = implode(",", $string);
          $estabelecimentos = "[" . $lista . "]";

        }

        //OBTÉM REFERENCIAMENTO
        $referencia = "[]";
        if ($data['referenciamento']) {

          $ref = implode('","', $data['referenciamento']);

          $referencia = '["' . $ref . '"]';

        }

        //OBTÉM LABORAL
        $laboral = "[]";
        $uf = "";
        if ($data['emp-input'] != "") {
          $array = [];
          foreach ($data['emp-input'] as $emp) {
            $sql = "SELECT id_sinde, sigla_sinde, cnpj_sinde, municipio_sinde, uf_sinde FROM sind_emp WHERE id_sinde = '{$emp}'";
            $resObj = mysqli_query($this->db, $sql)->fetch_object();
            $uf = $resObj->uf_sinde;

            array_push($array, "{" . '"id":' . $resObj->id_sinde . ',"sigla":' . '"' . $resObj->sigla_sinde . '"' . ',"cnpj":' . '"' . $resObj->cnpj_sinde . '"' . ',"municipio":' . '"' . $resObj->municipio_sinde . '"' . ',"uf":' . '"' . $resObj->uf_sinde . '"' . "}");
          }

          $lista = implode(",", $array);
          $laboral = "[" . $lista . "]";

        }

        //OBTÉM PATRONAL
        $patronal = "[]";
        if ($data['patr-input'] != "") {
          $array = [];
          foreach ($data['patr-input'] as $patr) {
            $sql = "SELECT id_sindp, sigla_sp, cnpj_sp, uf_sp FROM sind_patr WHERE id_sindp = '{$patr}'";
            $resObj = mysqli_query($this->db, $sql)->fetch_object();

            array_push($array, "{" . '"id":' . $resObj->id_sindp . ',"sigla":' . '"' . $resObj->sigla_sp . '"' . ',"cnpj":' . '"' . $resObj->cnpj_sp . '"' . ',"uf":' . '"' . $resObj->uf_sp . '"' . "}");
          }

          $lista = implode(",", $array);
          $patronal = "[" . $lista . "]";

        }

        //OBTÉM CNAE
        $cnae = "[]";
        if ($data['cnae-input']) {
          $cnaes = explode(" ", trim($data['cnae-input']));
          $array = [];
          foreach ($cnaes as $cnae) {
            $sql = "SELECT id_cnae, descricao_subclasse FROM classe_cnae WHERE id_cnae = '{$cnae}'";
            $resObj = mysqli_query($this->db, $sql)->fetch_object();

            array_push($array, "{" . '"id":' . $resObj->id_cnae . ',"subclasse":' . '"' . $resObj->descricao_subclasse . '"' . "}");
          }

          $lista = implode(",", $array);
          $cnae = "[" . $lista . "]";

        }

        //OBTÉM ABRANGENCIA
        $abrang = "[]";
        if ($data['abrang-input']) {
          $array = [];
          $abrang = explode(" ", trim($data['abrang-input']));

          foreach ($abrang as $local) {
            $sql = "SELECT id_localizacao, municipio, uf FROM localizacao WHERE id_localizacao = '{$local}'";
            $resObj = mysqli_query($this->db, $sql)->fetch_object();

            array_push($array, "{" . '"id":' . $resObj->id_localizacao . ',"Municipio":' . '"' . $resObj->municipio . '"' . ',"Uf":' . '"' . $resObj->uf . '"' . "}");
          }

          $lista = implode(",", $array);
          $abrang = "[" . $lista . "]";

        }

        $restrito = ($data['doc_restrito'] == "restrito" ? "Sim" : "Não");

        $sql = "INSERT INTO doc_sind
				(tipounidade_cliente_id_tiponegocio
				,tipo_doc_idtipo_doc
				,origem
				,versao_documento
				,numero_solicitacao_mr
				,data_reg_mte
				,num_reg_mte
				,validade_inicial
				,validade_final
				,prorrogacao_doc
				,data_assinatura
				,permissao
				,observacao
				,documento_id_documento,
				database_doc,
				doc_restrito,
				cliente_estabelecimento,
				referencia,
				sind_laboral,
				sind_patronal,
				cnae_doc,
        abrangencia,
				modulo,
				uf 
				)
				VALUES (
					'{$data['type-un-input']}', 
					'{$data['tipodoc-input']}', 
					'{$data['ori-input']}', 
					'{$data['versao-input']}', 
					'{$data['num-input']}', 
					'{$dataReg}', 
					'{$data['num-reg-input']}',
					'{$dataIni}', 
					'{$dataFim}',  
					'{$dataPro}', 
					'{$dataAss}', 
					'{$data['perm-input']}', 
					'{$data['obs-input']}',
					'{$data['id_arquivo']}',
					'{$dataBase}',
					'{$restrito}',
				    '{$estabelecimentos}',
				    '{$referencia}',
				    '{$laboral}',
				    '{$patronal}',
				    '{$cnae}',
				    '{$abrang}',
					'SISAP',
					'{$uf}')
				";


        //INSERT DOC SIND

        if (!mysqli_query($this->db, $sql)) {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Erro ao cadastrar';

        } else {

          $lastId = mysqli_insert_id($this->db);

          $sqlDoc = "UPDATE documentos_localizados
							SET referenciado = 'sim'
							WHERE id_documento = '{$data['id_arquivo']}'
					";

          mysqli_query($this->db, $sqlDoc);


          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Cadastro realizado com sucesso!';
        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }


    return $response;
  }

  function updateDocSind($data = null)
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


      $dataIni = ($data['vini-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['vini-input'])))) : "0000-00-00");
      $dataFim = ($data['vfim-input'] != "" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['vfim-input'])))) : "0000-00-00");
      $dataPro = ($data['pro-input'] != "00/00/0000" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['pro-input'])))) : "0000-00-00");
      $dataReg = ($data['data-reg-input'] != "00/00/0000" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['data-reg-input'])))) : "0000-00-00");
      $dataAss = ($data['ass-input'] != "00/00/0000" ? date('Y-m-d', strtotime(implode("-", explode("/", $data['ass-input'])))) : "0000-00-00");
      $docRestrito = $data['doc_restrito'] == 'restrito' ? 'Sim' : 'Não';

      if ($response['response_status']['status'] == 1) {


        //CLIENTE UNIDADES
        $estabelecimentos = "[]";
        if ($data['cliente_unidade']) {
          $array = [];

          $unidades = explode(" ", trim($data['cliente_unidade']));
          foreach ($unidades as $unidade) {

            $sql = "SELECT id_unidade, cliente_matriz_id_empresa as matriz, cliente_grupo_id_grupo_economico as grupo FROM cliente_unidades WHERE id_unidade = '{$unidade}'";
            $resObj = mysqli_query($this->db, $sql)->fetch_object();


            array_push($array, "{" . '"g":' . $resObj->grupo . ',"m":' . $resObj->matriz . ',"u":' . $resObj->id_unidade . ',"nome_unidade":' . '"' . $resObj->nome_unidade . '"' . "}");
          }

          $lista = implode(",", $array);
          $estabelecimentos = "[" . $lista . "]";
        }


        //REFERENCIAMENTO
        $reference = "[]";
        if ($data['referenciamento']) {
          $reference = '["' . implode('","', $data['referenciamento']) . '"]';
        }

        //OBTÉM LABORAL
        $uf = "";
        $emp = "[]";
        if ($data['sind_emp']) {
          $array = [];
          foreach ($data['sind_emp'] as $emp) {
            $sql = "SELECT id_sinde, sigla_sinde, cnpj_sinde, uf_sinde, municipio_sinde FROM sind_emp WHERE id_sinde = '{$emp}'";
            $resObj = mysqli_query($this->db, $sql)->fetch_object();
            $uf = $resObj->uf_sinde;
            array_push($array, "{" . '"id":' . $resObj->id_sinde . ',"sigla":' . '"' . $resObj->sigla_sinde . '"' . ',"cnpj":' . '"' . $resObj->cnpj_sinde . '"' . ',"municipio":' . '"' . $resObj->municipio_sinde . '"' . ',"uf":' . '"' . $resObj->uf_sinde . '"' . "}");
          }

          $lista = implode(",", $array);
          $emp = "[" . $lista . "]";
        }

        //OBTÉM PATRONAL
        $patr = "[]";
        if ($data['sind_patr']) {
          $array = [];
          foreach ($data['sind_patr'] as $patr) {
            $sql = "SELECT id_sindp, sigla_sp, cnpj_sp, uf_sp FROM sind_patr WHERE id_sindp = '{$patr}'";
            $resObj = mysqli_query($this->db, $sql)->fetch_object();

            array_push($array, "{" . '"id":' . $resObj->id_sindp . ',"sigla":' . '"' . $resObj->sigla_sp . '"' . ',"cnpj":' . '"' . $resObj->cnpj_sp . '"' . ',"uf":' . '"' . $resObj->uf_sp . '"' . "}");
          }

          $lista = implode(",", $array);
          $patr = "[" . $lista . "]";
        }

        //CNAE
        $cnae = "[]";
        if ($data['cnae'] != "") {
          $array = [];
          $cnaes = explode(" ", trim($data['cnae']));
          foreach ($cnaes as $item) {
            $sql = "SELECT id_cnae, descricao_subclasse FROM classe_cnae WHERE id_cnae = '{$item}'";
            $resObj = mysqli_query($this->db, $sql)->fetch_object();

            array_push($array, "{" . '"id":' . $resObj->id_cnae . ',"subclasse":' . '"' . $resObj->descricao_subclasse . '"' . "}");
          }

          $lista = implode(",", $array);
          $cnae = "[" . $lista . "]";
        }

        //OBTÉM ABRANGENCIA
        $abrang = "[]";

        if ($data['abrang-input'] != "") {
          $array = [];
          $abrangs = explode(" ", trim($data['abrang-input']));
          foreach ($abrangs as $local) {
            $sql = "SELECT id_localizacao, municipio, uf FROM localizacao WHERE id_localizacao = '{$local}'";
            $resObj = mysqli_query($this->db, $sql)->fetch_object();

            array_push($array, "{" . '"id":' . $resObj->id_localizacao . ',"Municipio":' . '"' . $resObj->municipio . '"' . ',"Uf":' . '"' . $resObj->uf . '"' . "}");
          }

          $lista = implode(",", $array);
          $abrang = "[" . $lista . "]";

        }


        $sql = "UPDATE doc_sind
						SET 
							tipounidade_cliente_id_tiponegocio = '{$data['type-un-input']}'
							,tipo_doc_idtipo_doc = '{$data['tipo-doc-input']}'
							,uf = '{$data['uf-input']}'
							,origem = '{$data['ori-input']}'
							,versao_documento = '{$data['versao-input']}'
							,numero_solicitacao_mr = '{$data['num-input']}'
							,data_reg_mte = '{$dataReg}'
							,num_reg_mte = '{$data['num-reg-input']}'
							,validade_inicial = '{$dataIni}'
							,validade_final = '{$dataFim}'
							,prorrogacao_doc = '{$dataPro}'
							,data_assinatura = '{$dataAss}'
							,observacao = '{$data['obs-input']}'
							,permissao = '{$data['perm-input']}'
							,doc_restrito = '{$docRestrito}'
						    ,cliente_estabelecimento = '{$estabelecimentos}'
						    ,referencia = '{$reference}'
						    ,sind_laboral = '{$emp}'
						    ,sind_patronal = '{$patr}'
						    ,cnae_doc = '{$cnae}'
						    ,abrangencia = '{$abrang}'

						WHERE 
							id_doc = {$data['id_doc']};
						";

        if (!mysqli_query($this->db, $sql)) {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Erro ao atualizar registro!';
        } else {
          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Cadastrado atualizado com sucesso';
        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    return $response;
  }

  function getAbrangVerify($data = null)
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

        $sql = "SELECT * FROM abrang_docsind WHERE doc_sind_id_documento = '{$data['id_doc']}'";

        if (!$result = mysqli_query($this->db, $sql)) {
          $response['response_data']['abrangencia'] = 0;
        } else {
          $obj = $result->fetch_object();
          $response['response_data']['abrangencia'] = $obj->doc_sind_id_documento;
        }

      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

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

      if ($response['response_status']['status'] == 1) {

        //LISTA LOCALIZAÇÃO
        $sql = "SELECT 
							id_localizacao, 
							municipio
						FROM 
							localizacao
						WHERE uf = '{$data['uf-input']}';				
						";


        if ($resultsql = mysqli_query($this->db, $sql)) {

          $list = "";
          // $update = "";
          $listaTable = [];
          while ($obj = $resultsql->fetch_object()) {
            $list .= '<tr class="tbl-item">';
            $list .= '<td><input class="form-check-input check" type="checkbox" value="1" id="' . $obj->id_localizacao . '"></td>';
            $list .= '<td class="title">';
            $list .= $obj->municipio;
            $list .= '</td>';
            $list .= '</tr>';

            $newObj = new stdClass();
            $newObj->campo = '<input class="form-check-input checkUpdate" type="checkbox" value="1" id="' . $obj->id_localizacao . '">';
            $newObj->municipio = $obj->municipio;
            array_push($listaTable, $newObj);
          }


          $response['response_data']['list'] = $list;
          $response['response_data']['lista_update'] = $listaTable;

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


      if ($response['response_status']['status'] == 1) {

        //LISTA LOCALIZAÇÃO
        $sql = "DELETE FROM 
							abrang_docsind 
						WHERE 
							doc_sind_id_documento = '{$data['id_doc']}';				
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

  function deleteAbrangencia($data = null)
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

        $idLocal = $data['id_localizacao'];
        for ($i = 0; $i < count($idLocal); $i++) {

          $select = "select json_search(abrangencia, 'one', (select municipio from localizacao where id_localizacao = {$idLocal[$i]})) as path from doc_sind where id_doc = {$data['id_doc'][0]}";
          $result = mysqli_query($this->db, $select)->fetch_object();
          $path = str_replace('"', "", strstr($result->path, ".", true));

          $sql = "UPDATE doc_sind
                                SET abrangencia = JSON_REMOVE(abrangencia, '{$path}')
                            WHERE id_doc = {$data['id_doc'][0]}";


          if (!mysqli_query($this->db, $sql)) {

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Erro ao excluir o registro!';

          } else {
            $response['response_status']['status'] = 1;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Registro excluído com sucesso!';
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

  function getDataBase($data = null)
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

        if ($data['sind_emp'][0] != "") {
          $sql = "SELECT dataneg FROM base_territorialsindemp WHERE sind_empregados_id_sinde1 = '{$data['sind_emp'][0]}'";
          $result = mysqli_query($this->db, $sql);
          $obj = $result->fetch_object();

          $dataMes = $obj->dataneg;
          $ano = getYear($data['validade_inicial']);
          // $dataBase = $dataMes . "/" . date_format((new DateTime('now')),'Y');
          $dataBase = $dataMes . "/" . $ano;

          $response['response_data']['data_base'] = $dataBase;
        }



      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }


    return $response;
  }

  function getPermissaoDoc($data = null)
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

        $sql = "SELECT idtipo_doc, permissao FROM tipo_doc WHERE idtipo_doc = '{$data['id_tipo_doc']}'
				";

        $result = mysqli_query($this->db, $sql);
        $obj = $result->fetch_object();

        $response['response_data']['permissao'] = $obj->permissao;




      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }


    return $response;
  }

  function addDocumentFile($data = null)
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

        $fileName = strstr($data['file_name'], "-0065", true);
        $origem = ($data['origem'] == "" ? "Não informada" : $data['origem']);


        $path = "https://ineditta.com" . strstr($data['path'], "/documentos_sistema");

        $sql = "INSERT INTO documentos_localizados
							(nome_documento,
							origem,
							caminho)
						VALUES
							('{$fileName}',
							'{$origem}',
							'{$path}')
				";

        if (!mysqli_query($this->db, $sql)) {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Erro ao realizar o cadastro do documento!';

        } else {
          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Cadastrado com sucesso!';
        }

      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }


    return $response;
  }

  function deleteDocumentFile($data = null)
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

        $select = "SELECT * FROM documentos_localizados WHERE id_documento = '{$data['id_documento']}'";
        $result = mysqli_query($this->db, $select);

        $obj = $result->fetch_object();

        $filePath = $obj->caminho;

        $path = __DIR__ . "/../../../documentos" . strstr($filePath, "_");

        if (unlink($path)) {
          $sql = "DELETE FROM documentos_localizados
							WHERE id_documento = '{$data['id_documento']}'
					";

          if (!mysqli_query($this->db, $sql)) {

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Erro ao excluir registro!';
          } else {
            $response['response_status']['status'] = 1;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Registro excluido com sucesso!';
          }
        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Erro ao excluir arquivo!';
        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }


    return $response;
  }

  function approveDocumentFile($data = null)
  {
    if ($this->response['response_status']['status'] == 1) {
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
        $sql = "UPDATE documentos_localizados
						SET situacao = 'aprovado', data_aprovacao = '" . date_format(new DateTime('now'), "Y-m-d") . "'
						 WHERE id_documento = '{$data['id_documento']}'
				";

        if (!mysqli_query($this->db, $sql)) {

          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Erro ao atualizar registro!';
        } else {
          $response['response_status']['status'] = 1;
          $response['response_status']['error_code'] = $this->error_code . __LINE__;
          $response['response_status']['msg'] = 'Registro atualizado com sucesso!';

          $sql = "SELECT 
								* 
							FROM documentos_localizados 
							WHERE id_documento = '{$data['id_documento']}'
					";

          $result = mysqli_query($this->db, $sql);

          $obj = $result->fetch_object();

          $response['response_data']['objDoc'] = $obj;
          $response['response_data']['path'] = $obj->caminho;
        }
      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    return $response;
  }

  function getDocumentoById($data = null)
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

        $select = "SELECT * FROM documentos_localizados WHERE id_documento = '{$data['id_documento']}'";
        $result = mysqli_query($this->db, $select);

        $obj = $result->fetch_object();

        $response['response_data']['objDoc'] = $obj;
        $response['response_data']['path'] = $obj->caminho;


      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
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

        $id = $data['id_doc'];
        $user = $data['id_user'];

        $now = (new DateTime('now'))->format("Y-m-d");

        $sql = "UPDATE doc_sind 
						SET usuario_responsavel = {$user},
							data_aprovacao = '{$now}'
						WHERE id_doc = {$id}
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

        //BUSCANDO FILIAIS
        $sql = "SELECT 
							id_doc,
							data_aprovacao,
							REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_laboral, '$[*].id') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  AS id_sinde,
                            REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_patronal, '$[*].id') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  AS id_sp,
                            REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.cliente_estabelecimento, '$[*].u') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  AS clt
						FROM doc_sind as doc
						WHERE doc.id_doc = {$id}
				";

        $obj = mysqli_query($this->db, $sql)->fetch_object();

        $objClt = explode(",", mysqli_query($this->db, $sql)->fetch_object()->clt);

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

        //BUSCANDO DADOS DO DOCUMENTO
        $sql = "SELECT 
						tp.nome_doc,
						CONCAT(DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y'), ' - ', DATE_FORMAT(doc.validade_final, '%d/%m/%Y')) as periodo,
						REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.cnae_doc, '$[*].subclasse') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  AS descricao_subclasse,
						JSON_EXTRACT(doc.abrangencia, '$[*].Municipio')  AS abrangencia,
						REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_laboral, '$[*].sigla') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  AS laboral,
						REPLACE(REPLACE(REPLACE(GROUP_CONCAT(DISTINCT JSON_EXTRACT(doc.sind_patronal, '$[*].sigla') SEPARATOR ','), '\"', ''), '[', ''), ']', '')  AS patronal
					FROM doc_sind as doc
					LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
					WHERE doc.id_doc = {$id}
			";

        $obj = mysqli_query($this->db, $sql)->fetch_object();

        $abrang = implode(", ", json_decode($obj->abrangencia));

        $message = "
				<div>
					<p>Olá,</p>
					<br>
					<p>Informamos que o documento sindical <b>{$obj->nome_doc}</b> abaixo está disponível para consulta.</p>
					<br>
					<p><b>Documento Sindical: </b>{$obj->nome_doc}</p>
					<p><b>Período documento sindical: </b>{$obj->periodo}</p>
					<p><b>Atividade Econômica: </b>{$obj->descricao_subclasse}</p>
					<p><b>Abrangência: </b>{$abrang}</p>
					<p><b>Sindicato(s) Laboral(is): </b>{$obj->laboral}</p>
					<p><b>Sindicato(s) Patronal(is): </b>{$obj->patronal}</p>
					<p>O documento já está disponível no Sistema Ineditta no menu Documentos - Documentos Processados.</p>
					<br>
					<p>Os demais módulos Cláusulas, Comparativo de Cláusulas, Calendário sindical e Mapa Sindical serão disponibilizados em até 5 dias.</p>
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
          "senha" => "mmzpdogaqaqwnhij",
          "cripto" => "tls",
          "porta" => 587,
          "nome" => "Ineditta Sistema",
          "to_multi" => $listaDestino,
          "assunto" => "Documento Sindical Aprovado",
          "msg" => $message
        ]);

        if ($response['response_email']['response_status']['status'] == 1) {

          //ADD CALENDARIO GERAL
          $sqlCalendar = "INSERT INTO calendario_geral_novo
							(doc_sind_id_doc)
							VALUES (
								'{$id}'
							)
				";

          if (!mysqli_query($this->db, $sqlCalendar)) {

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code'] = "";
            $response['response_status']['msg'] = "Disparos enviados, mas não foi possivel enviar os dados ao calendário geral";
          } else {
            $response['response_status']['status'] = 1;
            $response['response_status']['error_code'] = $this->error_code . __LINE__;
            $response['response_status']['msg'] = 'Disparos enviados e dados encaminhados para o calendario geral com sucesso!';
          }


        } else {
          $response['response_status']['status'] = 0;
          $response['response_status']['error_code'] = $response['response_email']['response_status']['error_code'];
          $response['response_status']['msg'] = $response['response_email']['response_status']['msg'];
        }

        $response['response_data']['destinatarios'] = $listaDeEmails;

      } else {
        $response = $this->response;
      }
    } else {
      $response = $this->response;
    }

    return $response;
  }
}