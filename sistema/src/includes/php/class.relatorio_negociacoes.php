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


// inclui as classes do PHPMailer
require(__DIR__ . "/PHPMailer.php");
require(__DIR__ . '/SMTP.php');
require(__DIR__ . '/helpers.php');
include_once "class.model.php";

setlocale(LC_TIME, 'pt_BR', 'pt_BR.utf-8', 'pt_BR.utf-8', 'portuguese');
date_default_timezone_set('America/Sao_Paulo');

class relatorio_negociacoes extends model
{


	function __construct()
	{
        parent::__construct(__CLASS__);

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
        }
	}

	function getRelatorio($data = null)
	{

        // Carregando a resposta padrão da função
        $response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

		if ($this->response['response_status']['status'] == 1) {

            $this->logger->debug(  $data );

            $iduser = $data['iduser'];
            $tipo = $data['tipo'];

            $sql = "SELECT 
                        idacompanhanto_cct, 
                        sigla_sinde, 
                        cnpj_sinde, 
                        sigla_sp, 
                        cnpj_sp,
                        ids_cnaes,
                        REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                            DATE_FORMAT(periodo_data, '%b/%Y'),
                            'Jan', 'JAN'),
                            'Feb', 'FEV'),
                            'Mar', 'MAR'),
                            'Apr', 'ABR'),
                            'May', 'MAI'),
                            'Jun', 'JUN'),
                            'Jul', 'JUL'),
                            'Aug', 'AGO'),
                            'Sep', 'SET'),
                            'Oct', 'OUT'),
                            'Nov', 'NOV'),
                            'Dec', 'DEZ'
                            ) as periodo_anterior,
                        data_base,
                        fase, 
                        dado_real, 
                        observacoes_gerais,
                        date_format(ultima_atualizacao, '%d/%m/%Y') as ultima_atualizacao,
                        be.localizacao_id_localizacao1,
                        be.classe_cnae_idclasse_cnae,
                        IFNULL(GROUP_CONCAT(cu.id_unidade), GROUP_CONCAT(IFNULL(cu.id_unidade,null))) as id_unidade,
                        IFNULL(GROUP_CONCAT(distinct cu.cliente_grupo_id_grupo_economico), GROUP_CONCAT(IFNULL(cu.cliente_grupo_id_grupo_economico,null))) as grupoecon
                    FROM acompanhanto_cct as cct
                        LEFT JOIN sind_emp as sd on sd.id_sinde = sind_emp_id_sinde
                        LEFT JOIN sind_patr as sp on sp.id_sindp = sind_patr_id_sindp
                        LEFT JOIN base_territorialsindemp as be on be.sind_empregados_id_sinde1 = sd.id_sinde
                        LEFT JOIN base_territorialsindpatro as bp on bp.sind_patronal_id_sindp = sp.id_sindp
                        LEFT JOIN cliente_unidades as cu on cu.localizacao_id_localizacao = be.localizacao_id_localizacao1 and 
                            JSON_CONTAINS(cu.cnae_unidade, CONCAT('\{\"id\":', be.classe_cnae_idclasse_cnae, '\}')) and 
                            cu.localizacao_id_localizacao = bp.localizacao_id_localizacao1 and  
                            JSON_CONTAINS(cu.cnae_unidade, CONCAT('\{\"id\":', bp.classe_cnae_idclasse_cnae, '\}'))
                        LEFT JOIN indecon_real as ir ON ir.periodo_data IS NOT NULL AND
                            REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                            DATE_FORMAT(DATE_ADD(ir.periodo_data, INTERVAL 1 MONTH), '%b/%Y'),
                            'Jan', 'JAN'),
                            'Feb', 'FEV'),
                            'Mar', 'MAR'),
                            'Apr', 'ABR'),
                            'May', 'MAI'),
                            'Jun', 'JUN'),
                            'Jul', 'JUL'),
                            'Aug', 'AGO'),
                            'Sep', 'SET'),
                            'Oct', 'OUT'),
                            'Nov', 'NOV'),
                            'Dec', 'DEZ'
                            ) =  data_base
                    WHERE ir.indicador = 'INPC' AND IF('{$tipo}' like 'Ineditta', 1 = 1, (select id_grupoecon from usuario_adm WHERE id_user = {$iduser}) = cu.cliente_grupo_id_grupo_economico)
                    GROUP BY cct.idacompanhanto_cct 
                    ORDER BY ultima_atualizacao
                    
            ";

            $this->logger->debug($sql);
            if ($resultsql = mysqli_query($this->db, $sql)) {

                // $html = null;
                $lista = [];
                while ($obj = $resultsql->fetch_object()) {

                    //OBTEM CNAE
                    $ids = "'" . implode("','", json_decode($obj->ids_cnaes)) . "'";
                    $cnae = "SELECT id_cnae, descricao_subclasse FROM classe_cnae WHERE id_cnae IN ({$ids})";
                    $result = mysqli_query($this->db, $cnae);

                    $array = [];
                    while ($objCnae = $result->fetch_object()) {
                        array_push($array, $objCnae->descricao_subclasse);
                    }

                    $cnaes = implode(", ", $array);


                    //gera nova lista
                    $new = new stdClass();
                    $new->sigla_sinde = $obj->sigla_sinde;
                    $new->cnpj_sinde = formatCnpjCpf($obj->cnpj_sinde);
                    $new->sigla_sp = $obj->sigla_sp;
                    $new->cnpj_sp = formatCnpjCpf($obj->cnpj_sp);
                    $new->cnaes = $cnaes;
                    $new->data_base = $obj->data_base;
                    $new->periodo = $obj->periodo_anterior;
                    $new->fase = $obj->fase;
                    $new->dado_real = $obj->dado_real;
                    $new->observacoes_gerais = $obj->observacoes_gerais;
                    $new->ultima_atualizacao = $obj->ultima_atualizacao;

                    array_push($lista, $new);
                }
                $this->logger->debug($lista);

                $response['response_data']['listaPrincipal'] = $lista;
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

		$this->logger->debug($response['response_status']['status']);

		return $response;
	}

	function getById($data = null)
	{

		if ($this->response['response_status']['status'] == 1) {
			// Carregando a resposta padrão da função
			$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

			$sql = "SELECT ac.idacompanhanto_cct
				, date_format(ac.data_inicial, \"%d/%m/%Y\") AS data_inicial
				, date_format(ac.data_final, \"%d/%m/%Y\") AS data_final
				, date_format(ac.ultima_atualizacao, \"%d/%m/%Y\") AS ultima_atualizacao
				, ac.status
				, ac.fase
				, ac.data_base
				, sp.sigla_sp
				, se.sigla_sinde
				, cc.descricao_subclasse
				FROM acompanhanto_cct AS ac
				INNER JOIN sind_patr AS sp ON sp.id_sindp = ac.sind_patr_id_sindp
				INNER JOIN sind_emp AS se ON se.id_sinde = ac.sind_emp_id_sinde
				INNER JOIN classe_cnae AS cc ON cc.id_cnae = ac.classe_cnae_id_cnae
				WHERE ac.idacompanhanto_cct = '{$data['id_acompanhanto_cct']}';				
			";


			$this->logger->debug($sql);
			if ($resultsql = mysqli_query($this->db, $sql)) {

				$obj = $resultsql->fetch_object();

				$response['response_data']['idacompanhanto_cct']   = $obj->idacompanhanto_cct;
				$response['response_data']['data_inicial']		    = $obj->data_inicial;
				$response['response_data']['data_final']		    = $obj->data_final;
				$response['response_data']['ultima_atualizacao']    = $obj->ultima_atualizacao;
				$response['response_data']['status']		        = $obj->status;
				$response['response_data']['fase']		            = $obj->fase;
				$response['response_data']['data_base']		        = $obj->data_base;
				$response['response_data']['patr']		            = $obj->sigla_sp;
				$response['response_data']['emp']		            = $obj->sigla_sinde;
				$response['response_data']['cnae']		            = $obj->descricao_subclasse;

				$this->logger->debug($response);
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

		return $response;
	}
}
