<?php
/**
 * @author    {Rafael P. Cruz}
 * @package   {2.0.0}
 * @description	{ }
 * @historic {
		2021-07-02 15:39 ( v1.0.0 ) -
*       2023-05-22 10:16 (v2.0.0) - Lucas Alcantara
	}
**/

include_once "class.model.php";
class diretoriapatronal extends model{

    function __construct() {

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

	function getDiretoriaPatronalCampos( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            $sql = "SELECT 
                        id_sindp
                        ,sigla_sp
                        ,cnpj_sp
                        ,logradouro_sp
                        ,email1_sp
                        ,fone1_sp
                        ,site_sp
                    FROM 
                        sind_patr;		";

            $this->logger->debug(  $sql );
            if( $resultsql = mysqli_query( $this->db, $sql ) ){

                $html = null;
                $htmlupdate = null;

                while($obj = $resultsql->fetch_object()){
                    $html .= '<tr class="odd gradeX tbl-item">';
                    $html .= '<td><button onclick="selectSindicato( '.$obj->id_sindp.');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
                    $html .= '<td class="title">';
                    $html .= $obj->sigla_sp;
                    $html .= '</td>';
                    $html .= '<td class="cnpj_format">';
                    $html .= $obj->cnpj_sp;
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
                    $htmlupdate .= '<td><button onclick="selectSindicato( '.$obj->id_sindp.');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
                    $htmlupdate .= '<td class="title">';
                    $htmlupdate .= $obj->sigla_sp;
                    $htmlupdate .= '</td>';
                    $htmlupdate .= '<td class="cnpj_format">';
                    $htmlupdate .= $obj->cnpj_sp;
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

                $response['response_data']['listaSindicato'] 	= $html;
                $response['response_data']['listaSindicatoUpdate'] 	= $htmlupdate;

            }
            else{
                $this->logger->debug( $sql );
                $this->logger->debug( $this->db->error );

                $response['response_status']['status']       = 0;
                $response['response_status']['error_code']   = $this->error_code . __LINE__;
                $response['response_status']['msg']          = '';
            }

            $sql = "SELECT 
                        id_tiponegocio
                        ,tipo_negocio
                    FROM 
                        tipounidade_cliente;							
            ";

            $this->logger->debug(  $sql );
            if( $resultsql = mysqli_query( $this->db, $sql ) ){

                $html = null;
                $htmlupdate = null;

                while($obj = $resultsql->fetch_object()){
                    $html .= '<tr class="odd gradeX">';
                    $html .= '<td><button onclick="selectNegocio( '.$obj->id_tiponegocio.');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
                    $html .= '<td>';
                    $html .= $obj->tipo_negocio;
                    $html .= '</td>';
                    $html .= '</tr>';

                    $htmlupdate .= '<tr class="odd gradeX">';
                    $htmlupdate .= '<td><button onclick="selectNegocio( '.$obj->id_tiponegocio.');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
                    $htmlupdate .= '<td>';
                    $htmlupdate .= $obj->tipo_negocio;
                    $htmlupdate .= '</td>';
                    $htmlupdate .= '</tr>';
                }

                $response['response_data']['listaNegocio'] 	= $html;
                $response['response_data']['listaNegocioUpdate'] 	= $htmlupdate;
            }
            else{
                $this->logger->debug( $sql );
                $this->logger->debug( $this->db->error );

                $response['response_status']['status']       = 0;
                $response['response_status']['error_code']   = $this->error_code . __LINE__;
                $response['response_status']['msg']          = '';
            }

            $sql = "SELECT
                        id_unidade,
                        nome_unidade as filial,
                        nome_empresa as matriz,
                        nome_grupoeconomico as grupo
                    FROM cliente_unidades
                    WHERE cliente_grupo_id_grupo_economico = {$_SESSION['grupoecon']};	
            ";

            $this->logger->debug(  $sql );
            if( $resultsql = mysqli_query( $this->db, $sql ) ){

                $html = null;
                $htmlupdate = null;

                while($obj = $resultsql->fetch_object()){
                    $html .= '<tr class="odd gradeX tbl-item">';
                    $html .= '<td><button onclick="selectEmpresa( '.$obj->id_unidade.');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
                    $html .= '<td class="title">';
                    $html .= $obj->filial;
                    $html .= '</td>';
                    $html .= '<td class="cnpj_format">';
                    $html .= $obj->cnpj_unidade;
                    $html .= '</td>';
                    $html .= '<td>';
                    $html .= $obj->matriz;
                    $html .= '</td>';
                    $html .= '</tr>';

                    $htmlupdate .= '<tr class="odd gradeX tbl-item">';
                    $htmlupdate .= '<td><button onclick="selectEmpresa( '.$obj->id_unidade.');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
                    $htmlupdate .= '<td class="title">';
                    $htmlupdate .= $obj->filial;
                    $htmlupdate .= '</td>';
                    $htmlupdate .= '<td class="cnpj_format">';
                    $htmlupdate .= $obj->cnpj_unidade;
                    $htmlupdate .= '</td>';
                    $htmlupdate .= '<td>';
                    $htmlupdate .= $obj->matriz;
                    $htmlupdate .= '</td>';
                    $htmlupdate .= '</tr>';
                }

                $response['response_data']['listaEmpresa'] 	= $html;
                $response['response_data']['listaEmpresaUpdate'] 	= $htmlupdate;

            }
            else{
                $this->logger->debug( $sql );
                $this->logger->debug( $this->db->error );

                $response['response_status']['status']       = 0;
                $response['response_status']['error_code']   = $this->error_code . __LINE__;
                $response['response_status']['msg']          = '';
            }


            $sql = "SELECT 
                        id_sindp
                        ,sigla_sp
                    FROM 
                        sind_patr;									
            ";

            $this->logger->debug(  $sql );
            if( $resultsql = mysqli_query( $this->db, $sql ) ){

                $grupos = '<option value="">';
                while($obj = $resultsql->fetch_object()){
                    $grupos .= '<option value="';
                    $grupos .= $obj->id_sindp;
                    $grupos .= '">';
                    $grupos .= $obj->sigla_sp;
                    $grupos .= '</option>';

                }

                $response['response_data']['sindicatos'] 	= $grupos;

            }
            else{
                $this->logger->debug( $sql );
                $this->logger->debug( $this->db->error );

                $response['response_status']['status']       = 0;
                $response['response_status']['error_code']   = $this->error_code . __LINE__;
                $response['response_status']['msg']          = '';
            }

            $sql = "SELECT 
                        id_unidade
                        ,nome_unidade
                    FROM 
                        cliente_unidades;									
            ";

            $this->logger->debug(  $sql );
            if( $resultsql = mysqli_query( $this->db, $sql ) ){

                $grupos = '<option value="">';
                $html = null;
                $htmlupdate = null;
                while($obj = $resultsql->fetch_object()){
                    $grupos .= '<option value="';
                    $grupos .= $obj->id_unidade;
                    $grupos .= '">';
                    $grupos .= $obj->nome_unidade;
                    $grupos .= '</option>';

                }

                $response['response_data']['empresas'] 	= $grupos;

                $response['response_data']['listaCentralSindical'] 	= $html;

                $response['response_data']['listaCentralSindicalUpdate'] 	= $htmlupdate;

            }
            else{
                $this->logger->debug( $sql );
                $this->logger->debug( $this->db->error );

                $response['response_status']['status']       = 0;
                $response['response_status']['error_code']   = $this->error_code . __LINE__;
                $response['response_status']['msg']          = '';
            }

            $sql = "SELECT 
                        id_localizacao
                        ,municipio
                    FROM 
                        localizacao;									
            ";

            $this->logger->debug(  $sql );
            if( $resultsql = mysqli_query( $this->db, $sql ) ){

                $grupos = '<option value="">';
                while($obj = $resultsql->fetch_object()){
                    $grupos .= '<option value="';
                    $grupos .= $obj->id_localizacao;
                    $grupos .= '">';
                    $grupos .= $obj->municipio;
                    $grupos .= '</option>';

                }

                $response['response_data']['localizacao'] 	= $grupos;

            }
            else{
                $this->logger->debug( $sql );
                $this->logger->debug( $this->db->error );

                $response['response_status']['status']       = 0;
                $response['response_status']['error_code']   = $this->error_code . __LINE__;
                $response['response_status']['msg']          = '';
            }
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}


	function getDiretoriaPatronal( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){

			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            $sql = "SELECT
                        SQL_CALC_FOUND_ROWS
                        dp.id_diretoriap AS id
                        ,dp.dirigente_p AS dirigente_p
                        ,DATE_FORMAT(dp.inicio_mandatop,'%d/%m/%Y') AS inicio_mandatop
                        ,DATE_FORMAT(dp.termino_mandatop,'%d/%m/%Y') AS termino_mandatop
                        ,dp.funcao_p AS funcao_p
                        ,dp.situacao_p AS situacao_p
                        ,sp.sigla_sp AS sigla_sp
                        ,cu.nome_unidade
                    FROM
                        sind_dirpatro dp
                    INNER JOIN sind_patr sp ON dp.sind_patr_id_sindp = sp.id_sindp
                    LEFT JOIN cliente_unidades cu ON dp.cliente_unidades_id_unidade = cu.id_unidade
                    LIMIT {$data['length']} OFFSET {$data['start']}
            ";



            $response = $this->renderPrincipalTable($sql,
                $data['draw'],
                'updateModal',
                'getByIdDiretoriaPatronal',
                $data['columns']);
		}
		else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );

		return $response;
	}

	function getByIdDiretoriaPatronal( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );


            $sql = "SELECT  
                        dp.id_diretoriap AS id_diretoriap
                        ,dp.dirigente_p AS dirigente_p
                        ,DATE_FORMAT(dp.inicio_mandatop,'%d/%m/%Y') AS inicio_mandatop
                        ,DATE_FORMAT(dp.termino_mandatop,'%d/%m/%Y') AS termino_mandatop
                        ,dp.funcao_p AS funcao_p
                        ,dp.situacao_p AS situacao_p
                        ,dp.sind_patr_id_sindp AS sind_patr_id_sindp
                        ,dp.cliente_unidades_id_unidade AS cliente_unidades_id_unidade
                    FROM 
                        sind_dirpatro dp
                        WHERE dp.id_diretoriap = {$data['id_diretoriap']};	
            ";

            $this->logger->debug(  $sql );
            if( $resultsql = mysqli_query( $this->db, $sql ) ){
                $obj = $resultsql->fetch_object();
                $response['response_data']['id_diretoriap'] 	= $obj->id_diretoriap;
                $response['response_data']['dirigente_p'] 	= $obj->dirigente_p;
                $response['response_data']['inicio_mandatop'] 	= $obj->inicio_mandatop;
                $response['response_data']['termino_mandatop'] 	= $obj->termino_mandatop;
                $response['response_data']['funcao_p'] 	= $obj->funcao_p;
                $response['response_data']['situacao_p'] 	= $obj->situacao_p;
                $response['response_data']['sind_patr_id_sindp'] 	= $obj->sind_patr_id_sindp;
                $response['response_data']['cliente_unidades_id_unidade'] 	= $obj->cliente_unidades_id_unidade;

            }
            else{
                $this->logger->debug( $sql );
                $this->logger->debug( $this->db->error );

                $response['response_status']['status']       = 0;
                $response['response_status']['error_code']   = $this->error_code . __LINE__;
                $response['response_status']['msg']          = '';
            }
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}	
	
	function addDiretoriaPatronal( $data = null ){
		$this->logger->debug(  'entrou na classe php' );
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            $sql = "INSERT INTO sind_dirpatro ( dirigente_p
                        ,inicio_mandatop
                        ,termino_mandatop
                        ,funcao_p
                        ,situacao_p
                        ,sind_patr_id_sindp
                        ,cliente_unidades_id_unidade) 
                    VALUES ('{$data['dir-input']}',STR_TO_DATE('{$data['dataini-input']}', '%d/%m/%Y'),
                        STR_TO_DATE('{$data['datafim-input']}', '%d/%m/%Y'),
                       '{$data['func-input']}' ,'{$data['sit-input']}',{$data['sind-input']},
                        {$data['emp-input']})
                    ";

            $response = $this->executeQuery($sql,
                "Cadastro atualizado com sucesso!",
                "Erro ao atualizar o cadastro!");

            if($lastId = mysqli_insert_id($this->db)) {
                //ADD CALENDARIO GERAL
                $sqlCalendar = "INSERT INTO calendario_geral_novo
                                (sind_dirpatro_id)
                                VALUES (
                                    '{$lastId}'
                                )
                ";

                $response = $this->executeQuery($sqlCalendar,
                    "Cadastro realizado com sucesso!",
                    "Erro ao realizar o cadastro!");
            }

		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}	
	
	function updateDiretoriaPatronal( $data = null ){
		$this->logger->debug(  'entrou na classe php' );
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            $sql = " UPDATE sind_dirpatro  
                                SET 
                                dirigente_p = '{$data['dir-input']}'
                                ,inicio_mandatop = STR_TO_DATE('{$data['dataini-input']}', '%d/%m/%Y')
                                ,termino_mandatop = STR_TO_DATE('{$data['datafim-input']}', '%d/%m/%Y')
                                ,funcao_p = '{$data['func-input']}'
                                ,situacao_p = '{$data['sit-input']}'
                                ,sind_patr_id_sindp = {$data['sind-input']}
                                ,cliente_unidades_id_unidade = {$data['emp-input']}
                                WHERE 
                                id_diretoriap = {$data['id_diretoriap']}
                    ";

            $response = $this->executeQuery($sql,
                "Cadastro atualizado com sucesso!",
                "Erro ao atualizar o cadastro!");

		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}	

}
