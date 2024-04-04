<?php
/**
 * @author    {J. Vinicio} - {Lucas Alcantara}
 * @package   {2.0.0}
 * @description	{ }
 * @historic {
		2022-06-21 16:40 ( v1.0.0 ) - 
	}
**/

include_once "class.model.php";
include_once "class.usuario.php";
include_once "helpers.php";

class comparativo extends model{

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
	

	function getLists( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
 
			if( $response['response_status']['status'] == 1 ){

				$user = (new usuario())->getUserData($data)['response_data']['user_data'];

				$idsFiliais = json_decode(($user->ids_fmge == '""') ? '["0"]' : $user->ids_fmge);
				//$idsMatrizes = json_decode(($user->ids_matrizes == '""') ? '["0"]' : $user->ids_matrizes);
				$idGrupo = json_decode($user->id_grupoecon);

				//DEFININDO LISTA PARA USUARIO UNIDADE
				$this->logger->debug($user);

				$index = "";
				if ($user->nivel == "Unidade") {
					$index = "u";
				}else if($user->nivel == "Matriz") {
					$index = "m";
				}else if($user->nivel == "Grupo Econômico") {
					$index = "g";
				}
				
				if ($user->nivel == "Unidade" || $user->nivel == "Matriz") {
					foreach ($idsFiliais as $id) {

						$where = '{"'.$index.'":'.$id.'}';
	
						$sql = "SELECT 
									doc.id_doc,
									doc.abrangencia,
									doc.cnae_doc,
									doc.database_doc,
									cg.id_clau,
									est.id_estruturaclausula,
									est.nome_clausula,
									gp.idgrupo_clausula,
									gp.nome_grupo,
									tp.nome_doc
								FROM doc_sind as doc
								LEFT JOIN clausula_geral as cg on cg.doc_sind_id_documento = doc.id_doc
								LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cg.estrutura_id_estruturaclausula
								LEFT JOIN grupo_clausula as gp on gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
								LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
	
								WHERE JSON_CONTAINS(doc.cliente_estabelecimento, '{$where}', '$')
								GROUP BY doc.id_doc
						";

					}
				}else {
					$whereClause = $user->nivel == "Grupo Econômico" ? '{"' . $index . '":' . $idGrupo . '}' : "";

					$where = $whereClause == "" ? "" : "WHERE JSON_CONTAINS(doc.cliente_estabelecimento, '{$whereClause}', '$')";

					$sql = "SELECT 
								doc.id_doc,
								doc.abrangencia,
								doc.cnae_doc,
								doc.database_doc,
								cg.id_clau,
								est.id_estruturaclausula,
								est.nome_clausula,
								gp.idgrupo_clausula,
								gp.nome_grupo,
								tp.idtipo_doc,
								tp.nome_doc
							FROM doc_sind as doc
							LEFT JOIN clausula_geral as cg on cg.doc_sind_id_documento = doc.id_doc
							LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cg.estrutura_id_estruturaclausula
							LEFT JOIN grupo_clausula as gp on gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
							LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc

							{$where}
							GROUP BY doc.id_doc
					";
				}
					$this->logger->debug($sql);

					$result = mysqli_query($this->db, $sql);

					$optionDatabase = '';
					$optionGrupoClausula = '';
					$optionNomeClausula = '';
					$optionAbrangencia = '';
					$optionCnae = '';
					$optionNomedoc = '';
					$optionUf = '';
					$listaMultiplo = [];
	
					while ($obj = $result->fetch_object()) {

						//LISTA DATABASE
						if (!in_array($obj->database_doc, $listaMultiplo) && $obj->database_doc != "") {
							$optionDatabase .= "<option value='{$obj->database_doc}'>{$obj->database_doc}</option>";
							array_push($listaMultiplo, $obj->database_doc);
						}

						//LISTA NOMEDOC
						if (!in_array($obj->nome_doc, $listaMultiplo) && $obj->nome_doc != "") {
							$optionNomedoc .= "<option value='{$obj->idtipo_doc}'>{$obj->nome_doc}</option>";
							array_push($listaMultiplo, $obj->nome_doc);
						}

						//LISTA GRUPO CLÁUSULA
						if (!in_array($obj->nome_grupo, $listaMultiplo) && $obj->nome_grupo != "") {
							$optionGrupoClausula .= "<option value='{$obj->idgrupo_clausula}'>{$obj->nome_grupo}</option>";
							array_push($listaMultiplo, $obj->nome_grupo);
						}

						//LISTA NOME CLÁUSULA
						if (!in_array($obj->nome_clausula, $listaMultiplo) && $obj->nome_clausula != "") {
							$optionNomeClausula .= "<option value='{$obj->id_estruturaclausula}'>{$obj->nome_clausula}</option>";
							array_push($listaMultiplo, $obj->nome_clausula);
						}
						

						//LISTA LOCALIDADE
						$abrang = json_decode($obj->abrangencia);
						$listaLocal = '';
						$listaUf = '';

						foreach ($abrang as $local) {

							if (!in_array($local->Municipio, $listaMultiplo)) {
								$listaLocal .= "<option value='{$local->id}'>{$local->Municipio} / {$local->Uf}</option>";
								array_push($listaMultiplo, $local->Municipio);
							}

							if (!in_array($local->Uf, $listaMultiplo)) {
								$listaUf .= "<option value='uf+{$local->Uf}'>{$local->Uf}</option>";
								array_push($listaMultiplo, $local->Uf);
							}
							
						}

						$optionAbrangencia .= $listaLocal;
						$optionUf .= $listaUf;

						//LISTA CNAE
						$cnaes = json_decode($obj->cnae_doc);
						$listaCnae = '';

						foreach ($cnaes as $cnae) {

							if (!in_array($cnae->id, $listaMultiplo)) {
								$listaCnae .= "<option value='{$cnae->id}'>{$cnae->subclasse}</option>";
								array_push($listaMultiplo, $cnae->id);
							}
							
						}

						$optionCnae .= $listaCnae;
					}

					$response['response_status']['status'] = 1;
					$response['response_data']['lista_abrangencia'] = $optionAbrangencia;
					$response['response_data']['lista_uf'] = $optionUf;
					$response['response_data']['lista_clausula'] = $optionNomeClausula;
					$response['response_data']['lista_grupo'] = $optionGrupoClausula;
					$response['response_data']['lista_cnae'] = $optionCnae;
					$response['response_data']['lista_database'] = $optionDatabase;
					$response['response_data']['lista_nomedoc'] = $optionNomedoc;

                    //LISTA CABECALHO NOMES DAS INFORMAÇÕES
                    $sqlInf = "select
                                id_info_tipo_grupo as id_nome_clausula,
                                GROUP_CONCAT(distinct gp.informacaoadicional_no_grupo) as informacoes_da_clausula
                            from clausula_geral_estrutura_clausula as cl
                            LEFT JOIN ad_tipoinformacaoadicional as ad on ad.cdtipoinformacaoadicional = cl.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
                            LEFT JOIN informacao_adicional_grupo as gp on gp.informacaoadicional_no_grupo = cl.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
                            WHERE ad.dado_ms = 'S'
                    ";

                    $result = mysqli_query($this->db, $sqlInf);


                    $obj = $result->fetch_object();
                    $this->logger->debug($obj);

                    $sql = "select
                                GROUP_CONCAT(CONCAT(cdtipoinformacaoadicional, ';', nmtipoinformacaoadicional)) infos
                            from ad_tipoinformacaoadicional
                            where cdtipoinformacaoadicional IN ({$obj->informacoes_da_clausula})
                            ORDER BY CASE nmtipoinformacaoadicional WHEN 'Nome da cláusula' THEN 0 ELSE 1 END
                    ";

                    $this->logger->debug($sql);
                    $result = mysqli_query($this->db, $sql)->fetch_object();

                    $array = explode(",", $result->infos);
                    $this->logger->debug($array);
//                    foreach ($array as $item) {
//                        $nome = mb_substr(strstr($item, ';'), 1);
//                        $id = strstr($item, ';', true);
//                        $this->logger->debug($nome);
//                        $this->logger->debug($id);
//                        $lista .= "<tr id='{$id}' data-id='{$id}' class='linha_info'>";
//                        $lista .=   "<th  id='coll-{$id}'>{$nome}</th>";
//                        $lista .= "</tr>";
//
//                        $this->logger->debug('terminou');
//                    }
                    $lista = "";
                    $primeiroDaLista = "";
                    $position170 = array_search('170;Nome da cláusula', $array);
                    for ($i = 0; $i < count($array) ; $i++) {
                        $nome = mb_substr(strstr($array[$i], ';'), 1);
                        $id = strstr($array[$i], ';', true);
                        $this->logger->debug($nome);
                        $this->logger->debug($id);
                        if ($i != $position170) {
                            $lista .= "<tr id='{$id}' data-id='{$id}' class='linha_info'>";
                            $lista .=   "<th  id='coll-{$id}'>{$nome}</th>";
                            $lista .= "</tr>";
                        }else {
                            $primeiroDaLista .= "<tr id='{$id}' data-id='{$id}' class='linha_info linha_nome'>";
                            $primeiroDaLista .=   "<th  id='coll-{$id}'>{$nome}</th>";
                            $primeiroDaLista .= "</tr>";
                        }

                        $this->logger->debug('terminou');
                    }
                    $this->logger->debug($lista);
                    $final = $primeiroDaLista . $lista;
                    $response['response_data']['cabecalho_informacoes'] = $final;

			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		return $response;
	}

	function getListsWithoutUser(){

		if( $this->response['response_status']['status'] == 1 ){

			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

			if( $response['response_status']['status'] == 1 ){

				$sql = "SELECT 
							doc.id_doc,
							doc.abrangencia,
							doc.cnae_doc,
							doc.database_doc,
							cg.id_clau,
							est.id_estruturaclausula,
							est.nome_clausula,
							gp.idgrupo_clausula,
							gp.nome_grupo,
							tp.nome_doc
						FROM doc_sind as doc
						LEFT JOIN clausula_geral as cg on cg.doc_sind_id_documento = doc.id_doc
						LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cg.estrutura_id_estruturaclausula
						LEFT JOIN grupo_clausula as gp on gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
						GROUP BY doc.id_doc
				";

				$this->logger->debug($sql);

				$result = mysqli_query($this->db, $sql);
//				$this->logger->debug($result);

				$optionDatabase = '';
				$optionGrupoClausula = '';
				$optionNomeClausula = '';
				$optionAbrangencia = '';
				$optionCnae = '';
				$optionNomedoc = '';
				$optionUf = '';
				$listaMultiplo = [];

				while ($obj = $result->fetch_object()) {
//					$this->logger->debug($obj);
					//LISTA DATABASE
					if (!in_array($obj->database_doc, $listaMultiplo) && $obj->database_doc != "") {
						$optionDatabase .= "<option value='{$obj->database_doc}'>{$obj->database_doc}</option>";
						array_push($listaMultiplo, $obj->database_doc);
					}
//					$this->logger->debug($optionDatabase);

					//LISTA NOMEDOC
					if (!in_array($obj->nome_doc, $listaMultiplo) && $obj->nome_doc != "") {
						$optionNomedoc .= "<option value='{$obj->idtipo_doc}'>{$obj->nome_doc}</option>";
						array_push($listaMultiplo, $obj->nome_doc);
					}
//					$this->logger->debug($optionNomedoc);

					//LISTA GRUPO CLÁUSULA
					if (!in_array($obj->nome_grupo, $listaMultiplo) && $obj->nome_grupo != "") {
						$optionGrupoClausula .= "<option value='{$obj->idgrupo_clausula}'>{$obj->nome_grupo}</option>";
						array_push($listaMultiplo, $obj->nome_grupo);
					}
//					$this->logger->debug($optionGrupoClausula);

					//LISTA NOME CLÁUSULA
					if (!in_array($obj->nome_clausula, $listaMultiplo) && $obj->nome_clausula != "") {
						$optionNomeClausula .= "<option value='{$obj->id_estruturaclausula}'>{$obj->nome_clausula}</option>";
						array_push($listaMultiplo, $obj->nome_clausula);
					}
//					$this->logger->debug($optionNomeClausula);

					//LISTA LOCALIDADE
					$abrang = json_decode($obj->abrangencia);
					$listaLocal = '';
					$listaUf = '';

					foreach ($abrang as $local) {

						if (!in_array($local->Municipio, $listaMultiplo)) {
							$listaLocal .= "<option value='{$local->id}'>{$local->Municipio} / {$local->Uf}</option>";
							array_push($listaMultiplo, $local->Municipio);
						}
//						$this->logger->debug($listaLocal);

						if (!in_array($local->Uf, $listaMultiplo)) {
							$listaUf .= "<option value='uf+{$local->Uf}'>{$local->Uf}</option>";
							array_push($listaMultiplo, $local->Uf);
						}
//						$this->logger->debug($listaUf);

					}

					$optionAbrangencia .= $listaLocal;
					$optionUf .= $listaUf;

//					$this->logger->debug($optionAbrangencia);
//					$this->logger->debug($optionUf);

					//LISTA CNAE
					$cnaes = json_decode($obj->cnae_doc);
					$listaCnae = '';

					foreach ($cnaes as $cnae) {

						if (!in_array($cnae->id, $listaMultiplo)) {
							$listaCnae .= "<option value='{$cnae->id}'>{$cnae->subclasse}</option>";
							array_push($listaMultiplo, $cnae->id);
						}

					}

					$optionCnae .= $listaCnae;
//					$this->logger->debug($optionCnae);
				}

				$response['response_status']['status'] = 1;
				$response['response_data']['lista_abrangencia'] = $optionAbrangencia;
				$response['response_data']['lista_uf'] = $optionUf;
				$response['response_data']['lista_cnae'] = $optionCnae;
				$response['response_data']['lista_database'] = $optionDatabase;
				$response['response_data']['lista_nomedoc'] = $optionNomedoc;

			}
			else{
				$response = $this->response;
			}
		}
		else{
			$response = $this->response;
		}

		mysqli_close($this->db);
		return $response;
	}

	function firstFilter( $data = null ){
 
		if( $this->response['response_status']['status'] == 1 ){

			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

			if( $response['response_status']['status'] == 1 ){

				$this->logger->debug( $data );

				//FILTROS
				$where = '';
				if ($data['localidade']) {
					if (mb_substr($data['localidade'], 0, 2) == "uf") {
						$uf = mb_substr(mb_strstr($data['localidade'], "uf+"), 3);
						$this->logger->debug( $uf );
						$id = '{"Uf":"'.$uf.'"}';
						$where .= $where != "" ? " AND JSON_CONTAINS(doc.abrangencia, '{$id}', '$') " : " JSON_CONTAINS(doc.abrangencia, '{$id}', '$') ";
					}else {
						$id = '{"id":'.$data['localidade'].'}';

						$where .= $where != "" ? " AND JSON_CONTAINS(doc.abrangencia, '{$id}', '$') " : " JSON_CONTAINS(doc.abrangencia, '{$id}', '$') ";
					}
					
				}

				if ($data['cnae']) {
					$id = '{"id":'.$data['cnae'].'}';

					$where .= $where != "" ? " AND JSON_CONTAINS(doc.cnae_doc, '{$id}', '$') " : " JSON_CONTAINS(doc.cnae_doc, '{$id}', '$') ";
				}

				if ($data['nome_doc']) {

					$where .= $where != "" ? " AND tp.idtipo_doc = {$data['nome_doc']} " : " tp.idtipo_doc = {$data['nome_doc']} ";
				}

				if ($data['data_base']) {
					$where .= $where != "" ? " AND doc.database_doc = '{$data['data_base']}' " : " doc.database_doc = '{$data['data_base']}' ";
				}

				$this->logger->debug( $where );

				$whereClause = $where != "" ? "WHERE " : "";
                $sql = $this->returnQuery(["whereClause" => $whereClause, "where" => $where], 500);

				$this->logger->debug( $sql );

                mysqli_query($this->db, "SET SESSION group_concat_max_len = 1500000;");
				if( $result = mysqli_query( $this->db, $sql ) ){

                    $lista = [];
                    while($obj = $result->fetch_object()) {
                        $new = new stdClass();
                        $new->button = "<button type='button' class='btn btn-primary' onclick='getRefereceDoc({$obj->id_doc})' data-dismiss='modal' style='margin: 4px 0;'>Selecionar</button>";
                        $new->id_doc = $obj->id_doc;
                        $new->sindicatos = "<span title='Laboral'>{$obj->laboral}</span> X <span title='Patronal'>{$obj->patronal}</span>";
                        array_push($lista, $new);
                    }

                    $response['response_data']['obj'] = $lista;

					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Busca realizada com sucesso!';


				}
				else{
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Erro ao realizar a busca!';

				}


			}
			else{
				$response = $this->response;
			}			
		}
		else{
			$response = $this->response;
		}
		
		$this->logger->debug( $response['response_status']['status'] );
		$this->logger->debug( $response['response_data'] );
		
		mysqli_close($this->db);
		return $response;
	}

    function getRefereceDoc( $data = null ){

        if( $this->response['response_status']['status'] == 1 ){

            // Carregando a resposta padrão da função
            $response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            if( $response['response_status']['status'] == 1 ){

                $this->logger->debug( $data );

                //FILTROS
                $where = '';

                if ($data['doc']) {

                    $where .= $where != "" ? " AND id_doc = {$data['doc']} " : " id_doc = {$data['doc']} ";
                }

                $this->logger->debug( $where );

                $whereClause = $where != "" ? "WHERE " : "";
                $sql = $this->returnQuery(["whereClause" => $whereClause, "where" => $where], 1);

                $this->logger->debug( $sql );

                mysqli_query($this->db, "SET SESSION group_concat_max_len = 1500000;");
                if( $result = mysqli_query( $this->db, $sql ) ){

                    $lista = [];
                    while($obj = $result->fetch_object()) {
                        $this->logger->debug($obj );
                        $new = new stdClass();
                        $new->sindicatos = $obj->laboral . '<br> X <br>' . $obj->patronal;
                        $new->cnae = $obj->cnae;
                        $new->local = $obj->abrang;
                        $new->qtd = $obj->qtd;
                        $new->nome_doc = $obj->nome_doc;
                        $new->vigencia = $obj->vigencia;
                        $new->data_base = $obj->data_base;
                        $new->inpc = $obj->inpc . "%";


                        $array = explode(";", $obj->value);
                        $this->logger->debug($array );
                        function getId($item) {
                            $id = strstr($item, ':', true);

                            return  $id;
                        }

                        function getValue($item) {
                            $valorInfo = mb_substr(strstr($item, ':'), 1);

                            return  $valorInfo;
                        }

                        $new->id_info = array_map('getId', $array);
                        $this->logger->debug($new->id_info );
                        $new->value = array_map('getValue', $array);
                        $this->logger->debug($new->value );



                        array_push($lista, $new);
                    }

                    $response['response_data']['obj'] = $lista;

                    $response['response_status']['status']       = 1;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Busca realizada com sucesso!';


                }
                else{
                    $response['response_status']['status']       = 0;
                    $response['response_status']['error_code']   = $this->error_code . __LINE__;
                    $response['response_status']['msg']          = 'Erro ao realizar a busca!';

                    $this->logger->debug( $sql );
                    $this->logger->debug( $this->db->error );
                    $this->logger->debug( $response );
                }


            }
            else{
                $response = $this->response;
            }
        }
        else{
            $response = $this->response;
        }

        $this->logger->debug( $response['response_status']['status'] );
        $this->logger->debug( $response['response_data'] );

        mysqli_close($this->db);
        return $response;
    }

	function secondFilter( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){

			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

			$this->logger->debug( $data );

			//FILTROS
			$where = '';
			if ($data['localidade']) {
				foreach ($data['localidade'] as $local) {
					if (mb_substr($local, 0, 2) == "uf") {
						$uf = mb_substr(mb_strstr($local, "uf+"), 3);
						$this->logger->debug( $uf );
						$id = '{"Uf":"'.$uf.'"}';
						$where .= $where != "" ? " OR JSON_CONTAINS(doc.abrangencia, '{$id}', '$') " : " JSON_CONTAINS(doc.abrangencia, '{$id}', '$') ";
					}else {
						$id = '{"id":'.$local.'}';

						$where .= $where != "" ? " OR JSON_CONTAINS(doc.abrangencia, '{$id}', '$') " : " JSON_CONTAINS(doc.abrangencia, '{$id}', '$') ";
					}
				}


			}
			$this->logger->debug( $where );
			if ($data['cnae']) {
				foreach ($data['cnae'] as $cnae) {
					$id = '{"id":'.$cnae.'}';

					$where .= $where != "" ? " OR JSON_CONTAINS(doc.cnae_doc, '{$id}', '$') " : " JSON_CONTAINS(doc.cnae_doc, '{$id}', '$') ";
				}

			}
			$this->logger->debug( $where );
			if ($data['nome_doc']) {
				foreach ($data['nome_doc'] as $doc) {
					$where .= $where != "" ? " OR tp.idtipo_doc = {$doc} " : " tp.idtipo_doc = {$doc} ";
				}

			}
			$this->logger->debug( $where );
			if ($data['data_base']) {
				foreach ($data['data_base'] as $date) {
					$where .= $where != "" ? " OR doc.database_doc = {$date} " : " doc.database_doc = '{$date}' ";
				}

			}

			$this->logger->debug( $where );

			$whereClause = $where != "" ? "WHERE ia.dado_ms = 'S' AND " : "";
			$sql = $this->returnQuery(["whereClause" => $whereClause, "where" => $where], 19);

			$this->logger->debug( $sql );
            mysqli_query($this->db, "SET SESSION group_concat_max_len = 1500000;");
			if ($result = mysqli_query($this->db, $sql)) {

				$this->logger->debug('gg' );
                $lista = [];
                while($obj = $result->fetch_object()) {
                    $new = new stdClass();
                    $new->check = "<input id='doc{$obj->id_doc}' type='checkbox' onclick='selectDocsToCompare({$obj->id_doc})'>";
                    $new->id_doc = $obj->id_doc;
                    $new->sindicatos = "<span title='Laboral'>{$obj->laboral}</span> X <span title='Patronal'>{$obj->patronal}</span>";
                    array_push($lista, $new);
                }

                $response['response_data']['docs_to_compare'] = $lista;

			}else{
				$this->logger->debug($this->db->error );
				$response['response_status']['status'] = 0;
				$response['response_status']['msg'] = "A busca não encontrou nenhum documento!";
			}
		}
		else{
			$response = $this->response;
		}

		$this->logger->debug( $response['response_status']['status'] );
		$this->logger->debug( $response['response_data'] );

		mysqli_close($this->db);
		return $response;
	}

    function getDocsToCompare( $data = null ){

        if( $this->response['response_status']['status'] == 1 ){

            // Carregando a resposta padrão da função
            $response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

            $this->logger->debug( $data );

            $ids = explode(" ", trim($data['docs']));

            $where = '';
            foreach ($ids as $id) {
                $where .= $where != "" ? " OR id_doc = {$id} " : "  id_doc = {$id} ";
            }

            $this->logger->debug( $where );

            $whereClause = $where != "" ? "WHERE ia.dado_ms = 'S' AND " : "";
            $sql = $this->returnQuery(["whereClause" => $whereClause, "where" => $where], 19);

            $this->logger->debug( $sql );
            mysqli_query($this->db, "SET SESSION group_concat_max_len = 1500000;");
            if ($result = mysqli_query($this->db, $sql)) {

                $this->logger->debug('gg' );
                $lista = [];
                while($obj = $result->fetch_object()) {
                    $this->logger->debug($obj );
                    $new = new stdClass();
                    $new->sindicatos = $obj->laboral . '<br> X <br>' . $obj->patronal;
                    $new->cnae = $obj->cnae;
                    $new->local = $obj->abrang;
                    $new->qtd = $obj->qtd;
                    $new->nome_doc = $obj->nome_doc;
                    $new->vigencia = $obj->vigencia;
                    $new->data_base = $obj->data_base;
                    $new->inpc = $obj->inpc . "%";


                    $array = explode(";", $obj->value);
                    $this->logger->debug($array );
                    $idInfo = [];
                    $value = [];
                    foreach ($array as $item) {
                        $id = strstr($item, ':', true);
                        $valorInfo = mb_substr(strstr($item, ':'), 1);
                        array_push($idInfo, $id);
                        array_push($value, $valorInfo);

                    }

                    $new->id_info = $idInfo; //array_map('getId', $array);
                    $this->logger->debug($new->id_info );
                    $new->value = $value; //array_map('getValue', $array);
                    $this->logger->debug($new->value );

                    array_push($lista, $new);
                    $this->logger->debug($lista);
                }

                $response['response_data']['obj'] = $lista;

            }else{
                $this->logger->debug($this->db->error );
                $response['response_status']['status'] = 0;
                $response['response_status']['msg'] = "A busca não encontrou nenhum documento!";
            }
        }
        else{
            $response = $this->response;
        }

        $this->logger->debug( $response['response_status']['status'] );
        $this->logger->debug( $response['response_data'] );

        mysqli_close($this->db);
        return $response;
    }

	function getCnae($data = null)
	{

		if ($this->response['response_status']['status'] == 1) {

			// Carregando a resposta padrão da função
			$response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.'));

			$localidade = $data['localidade'];
            $where = '';
            if ($localidade) {
                foreach ($localidade as $local) {
                    if (mb_substr($local, 0, 2) == "uf") {
                        $uf = mb_substr(mb_strstr($local, "uf+"), 3);
                        $this->logger->debug( $uf );
                        $where .= $where != "" ? " AND lc.uf IN ('{$uf}')" : " lc.uf IN ('{$uf}')" ;
                    }else {
                        $where .= $where != "" ? " AND be.localizacao_id_localizacao1 IN ('{$local}')" : " be.localizacao_id_localizacao1 IN ('{$local}')";
                    }
                }

            }

            $whereClause = $where == "" ? "" : "WHERE";

			$opt = "<option value=''></option>";
			$sql = "SELECT
						distinct be.classe_cnae_idclasse_cnae as id_cnae,
						be.localizacao_id_localizacao1,
						cn.descricao_subclasse  as cnae
					FROM base_territorialsindemp as be
					LEFT JOIN classe_cnae as cn on cn.id_cnae = be.classe_cnae_idclasse_cnae
					LEFT JOIN localizacao as lc on lc.id_localizacao = be.localizacao_id_localizacao1
					{$whereClause} {$where}
			";

			$this->logger->debug($sql);

			$result = mysqli_query($this->db, $sql);


			while ($obj = $result->fetch_object()) {
				$opt .= "<option value='{$obj->id_cnae}'>{$obj->cnae}</option>";
			}

			$response['response_data']['lista_cnae'] = $opt;

		} else {
			$response = $this->response;
		}

		$this->logger->debug($response['response_status']['status']);
		return $response;
	}

    private function returnQuery(array $where, int $limit) {
        $query = "SELECT 
							doc.id_doc,
							REPLACE(REPLACE(REPLACE(JSON_EXTRACT(doc.abrangencia, '$[*].Municipio') , '\"', ''), '[', ''), ']', '')  AS abrang,
							REPLACE(REPLACE(REPLACE(JSON_EXTRACT(doc.cnae_doc, '$[*].subclasse') , '\"', ''), '[', ''), ']', '')  AS cnae,
							REPLACE(REPLACE(REPLACE(JSON_EXTRACT(doc.sind_laboral, '$[*].sigla'), '\"', ''), '[', ''), ']', '')  AS laboral,
							REPLACE(REPLACE(REPLACE(JSON_EXTRACT(doc.sind_patronal, '$[*].sigla'), '\"', ''), '[', ''), ']', '')  AS patronal,
							doc.database_doc as data_base,
							cg.id_clau,
							est.id_estruturaclausula,
							est.nome_clausula,
							gp.idgrupo_clausula,
							gp.nome_grupo,
							CONCAT(DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y'), ' até ', DATE_FORMAT(doc.validade_final, '%d/%m/%Y')) as vigencia,
							JSON_LENGTH(doc.cliente_estabelecimento) as qtd,
							tp.nome_doc,
							ir.periodo_data,
							ir.dado_real as inpc,
							ia.dado_ms,
							nmtipoinformacaoadicional,
                            GROUP_CONCAT(CONCAT(cdtipoinformacaoadicional,': ',
                                case 
                                    when cl.texto != '' then cl.texto
                                    when cl.numerico != 0 then cl.numerico
                                    when cl.descricao != '' then cl.descricao
                                    when cl.data then date_format(cl.data, '%d/%m/%Y')
                                    when cl.percentual != 0 then concat(format(cl.percentual, 2), '%')
                                    when cl.hora != '' then cl.hora
                                    when cl.combo != '' then cl.combo else null end)SEPARATOR ';') as value
						FROM clausula_geral_estrutura_clausula as cl
                        LEFT JOIN doc_sind as doc on doc.id_doc = cl.doc_sind_id_doc
						LEFT JOIN clausula_geral as cg on cg.id_clau = cl.clausula_geral_id_clau
                        LEFT JOIN ad_tipoinformacaoadicional as ia on cl.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = ia.cdtipoinformacaoadicional
						LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cl.estrutura_clausula_id_estruturaclausula
						LEFT JOIN grupo_clausula as gp on gp.idgrupo_clausula = est.grupo_clausula_idgrupo_clausula
						LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
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
								) =  database_doc AND ir.indicador = 'INPC'

					{$where['whereClause']} {$where['where']}
					GROUP BY id_doc
                    LIMIT {$limit}";

        $this->logger->debug($query);

        return $query;
    }

}
