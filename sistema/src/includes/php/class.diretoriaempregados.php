<?php
/**
 * @author    {Rafael P. Cruz}
 * @package   {2.0.0}
 * @description	{ }
 * @historic {
		2021-07-02 15:39 ( v1.0.0 ) -
 *      2023-05-22 10:16 (v2.0.0) - Lucas Alcantara
	}
**/


include_once "class.model.php";
class diretoriaempregados extends model{

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

	function getDiretoriaEmpregadosCampos( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            $sql = "SELECT 
                        id_sinde
                        ,sigla_sinde
                        ,cnpj_sinde
                        ,logradouro_sinde
                        ,email1_sinde
                        ,fone1_sinde
                        ,site_sinde
                    FROM 
                        sind_emp;			";

            $this->logger->debug(  $sql );
            if( $resultsql = mysqli_query( $this->db, $sql ) ){

                $html = null;
                $htmlupdate = null;

                while($obj = $resultsql->fetch_object()){
                    $html .= '<tr class="odd gradeX tbl-item">';
                    $html .= '<td><button onclick="selectSindicato( '.$obj->id_sinde.');" data-toggle="modal" href="#myModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
                    $html .= '<td class="title">';
                    $html .= $obj->sigla_sinde;
                    $html .= '</td>';
                    $html .= '<td class="cnpj_format">';
                    $html .= $obj->cnpj_sinde;
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
                    $htmlupdate .= '<td><button onclick="selectSindicato( '.$obj->id_sinde.');" data-toggle="modal" href="#updateModal" type="button" class="btn btn-secondary" data-dismiss="modal">Selecionar</button></td>';
                    $htmlupdate .= '<td class="title">';
                    $htmlupdate .= $obj->sigla_sinde;
                    $htmlupdate .= '</td>';
                    $htmlupdate .= '<td class="cnpj_format">';
                    $htmlupdate .= $obj->cnpj_sinde;
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

            //LISTA EMPRESA
            $sql = "SELECT
                        id_unidade,
                        nome_unidade as filial,
                        nome_empresa as matriz,
                        nome_grupoeconomico as grupo
                    FROM cliente_unidades
                    WHERE cliente_grupo_id_grupo_economico = {$_SESSION['grupoecon']};	
            ";

            $result = mysqli_query( $this->db, $sql );
            $html = "";
            while ($obj = $result->fetch_object()) {
                $html .= "<tr class='tbl-item'>";
                $html .= "<td><input type='checkbox' id='emp{$obj->id_unidade}' onclick='selectEmpresa({$obj->id_unidade})'></td>";
                $html .= "<td>{$obj->grupo}</td>";
                $html .= "<td class='desc'>{$obj->matriz}</td>";
                $html .= "<td class='title'>{$obj->filial}</td>";
                $html .= "</tr>";
            }

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
                        id_sinde
                        ,sigla_sinde
                    FROM 
                        sind_emp;									
            ";

            $this->logger->debug(  $sql );
            if( $resultsql = mysqli_query( $this->db, $sql ) ){

                $grupos = '<option value=""></option>';
                while($obj = $resultsql->fetch_object()){
                    $grupos .= '<option value="';
                    $grupos .= $obj->id_sinde;
                    $grupos .= '">';
                    $grupos .= $obj->sigla_sinde;
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

                $grupos = '<option value=""></option>';
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

                $grupos = '<option value=""></option>';
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

	
	function getDiretoriaEmpregados( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){

			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            $sql = "SELECT 
                        SQL_CALC_FOUND_ROWS
						dp.id_diretoriae AS id 
						,dp.dirigente_e AS dirigente_p
						,DATE_FORMAT(dp.inicio_mandatoe,'%d/%m/%Y') AS inicio_mandatop
						,DATE_FORMAT(dp.termino_mandatoe,'%d/%m/%Y') AS termino_mandatop
						,dp.funcao_e AS funcao_p
						,dp.situacao_e AS situacao_p
						,sp.sigla_sinde AS sigla_sp
						,cl.nome_unidade
					FROM sind_diremp dp
					INNER JOIN sind_emp sp ON dp.sind_emp_id_sinde = sp.id_sinde
					LEFT JOIN cliente_unidades cl ON dp.cliente_unidades_id_unidade = cl.id_unidade	
					LIMIT {$data['length']} OFFSET {$data['start']}
			";

            $response = $this->renderPrincipalTable($sql,
                $data['draw'],
                'updateModal',
                'getByIdDiretoriaEmpregados',
                $data['columns']);

		}
		else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );

		return $response;
	}

	function getByIdDiretoriaEmpregados( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            $sql = "SELECT  
                        dp.id_diretoriae AS id_diretoriap
                        ,dp.dirigente_e AS dirigente_p
                        ,DATE_FORMAT(dp.inicio_mandatoe,'%d/%m/%Y') AS inicio_mandatop
                        ,DATE_FORMAT(dp.termino_mandatoe,'%d/%m/%Y') AS termino_mandatop
                        ,dp.funcao_e AS funcao_p
                        ,dp.situacao_e AS situacao_p
                        ,dp.sind_emp_id_sinde AS sind_patr_id_sindp
                        ,dp.cliente_unidades_id_unidade AS unidade
                    FROM sind_diremp dp
                        WHERE dp.id_diretoriae = {$data['id_diretoriae']};	
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
                $response['response_data']['cliente_matriz_id_empresa'] 	= $obj->unidade;

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
	
	function addDiretoriaEmpregados( $data = null ){
		$this->logger->debug(  'entrou na classe php' );
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            $sql = "INSERT INTO sind_diremp ( dirigente_e
                        ,inicio_mandatoe
                        ,termino_mandatoe
                        ,funcao_e
                        ,situacao_e
                        ,sind_emp_id_sinde
                        ,cliente_unidades_id_unidade
                       ) VALUES ('{$data['dir-input']}',STR_TO_DATE('{$data['dataini-input']}', '%Y-%m-%d'),
                        STR_TO_DATE('{$data['datafim-input']}', '%Y-%m-%d'),
                       '{$data['func-input']}' ,'{$data['sit-input']}',{$data['sind-input']},
                        {$data['emp-input']})
                    ";

            $response = $this->executeQuery($sql,
                "Cadastro atualizado com sucesso!",
                "Erro ao atualizar o cadastro!");

            if($lastId = mysqli_insert_id($this->db)) {
                //ADD CALENDARIO GERAL
                $sqlCalendar = "INSERT INTO calendario_geral_novo
                                (sind_diremp_id)
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
	
	function updateDiretoriaEmpregados( $data = null ){
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            $sql = " UPDATE sind_diremp  
                                SET 
                                dirigente_e = '{$data['dir-input']}'
                                ,inicio_mandatoe = STR_TO_DATE('{$data['dataini-input']}', '%Y-%m-%d')
                                ,termino_mandatoe = STR_TO_DATE('{$data['datafim-input']}', '%Y-%m-%d')
                                ,funcao_e = '{$data['func-input']}'
                                ,situacao_e = '{$data['sit-input']}'
                                ,sind_emp_id_sinde = {$data['sind-input']}
                                ,cliente_unidades_id_unidade = {$data['emp-input']}
                                WHERE 
                                id_diretoriae = {$data['id_diretoriap']}
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
