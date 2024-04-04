<?php
/**
 * @author    {J. Venicio}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
		2022-06-21 16:40 ( v1.0.0 ) - 
	}
**/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


include_once "class.model.php";
include_once "class.usuario.php";
include_once "helpers.php";
include_once __DIR__ . "/class.disparoEmail.php";

class helpdesk extends model{

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

				//LISTA PRINCIPAL
				$sqlList = "SELECT 
								hd.idhelpdesk,
								hd.data_abertura,
								hd.data_vencimento,
								hd.status_chamado,
								hd.inicio_resposta,
								md.modulos,
								cl.nome_unidade,
								uadmc.nome_usuario as userC,
								uadmr.nome_usuario as userR
							FROM helpdesk as hd
							LEFT JOIN modulos as md on md.id_modulos = hd.tipo_chamado
							LEFT JOIN cliente_unidades as cl on cl.id_unidade = hd.estabelecimento
							LEFT JOIN usuario_adm as uadmc on uadmc.id_user = hd.id_userC
							LEFT JOIN usuario_adm as uadmr on uadmr.id_user = hd.id_userR
						";

				$resultList = mysqli_query( $this->db, $sqlList ) or die(mysqli_error($this->db));

				$html = "";
				while ($obj = $resultList->fetch_object()) {
					$html .= '<tr class="odd gradeX tbl-item">';
					
					if($obj->inicio_resposta == "" || $obj->inicio_resposta == NULL) {
						$html .= '<td class="title"><a style="color: #000;" data-toggle="modal" href="#myModalUpdate" onclick="getById('.$obj->idhelpdesk.');"><i class="fa fa-file-text"></i></a></td>';
					}else{
						$html .= '<td class="title"><a data-toggle="modal" href="#responsavelChamadoInfoModal" onclick="getByIdResposta('.$obj->idhelpdesk.');"><i class="fa fa-file-text"></i></a></td>';
					};

					$html .= '<td class="desc">' . $obj->idhelpdesk . '</td>';
					$html .= '<td>' . $obj->modulos . '</td>';
					$html .= '<td>' . $obj->userC . '</td>';
					$html .= '<td>' . $obj->nome_unidade . '</td>';
					$html .= '<td>' . $obj->data_abertura . '</td>';
					$html .= '<td>' . $obj->data_vencimento . '</td>';
					$html .= '<td>' . $obj->userR . '</td>';
					$html .= '<td>' . $obj->status_chamado . '</td>';
					$html .= '<td style="text-align: center;"><a data-toggle="modal" href="#modalTimeline"><svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-align-middle" viewBox="0 0 16 16">
								<path d="M6 13a1 1 0 0 0 1 1h2a1 1 0 0 0 1-1V3a1 1 0 0 0-1-1H7a1 1 0 0 0-1 1v10zM1 8a.5.5 0 0 0 .5.5H6v-1H1.5A.5.5 0 0 0 1 8zm14 0a.5.5 0 0 1-.5.5H10v-1h4.5a.5.5 0 0 1 .5.5z"/>
							</svg></a></td>';
					$html .= '</tr>';
				}

				$response['response_data']['listaPrincipal'] = $html;
				

				//LISTA TIPO CHAMADO
				$sqlModulo = "SELECT id_modulos, modulos FROM modulos WHERE tipo = 'Helpdesk'";
				$resultModulo = mysqli_query($this->db, $sqlModulo);

				$option = "<option value=''></option>";
				while ($obj = $resultModulo->fetch_object()) {
					$option .= '<option value="' . $obj->id_modulos . '">' . $obj->modulos . '</option>';
				}

				$response['response_data']['tipo_chamado'] = $option;

				
				//LISTA CLAUSULA
				$sqlCG = "SELECT DISTINCT cg.id_clau,
									est.id_estruturaclausula,
									est.nome_clausula
								FROM clausula_geral as cg
								LEFT JOIN estrutura_clausula as est on est.id_estruturaclausula = cg.estrutura_id_estruturaclausula";

				$resultCG = mysqli_query($this->db, $sqlCG) or die(mysqli_error($this->db));

				$option = "<option value=''></option>";
				while ($obj = $resultCG->fetch_object()) {
					$option .= '<option value="' . $obj->id_clau . '">' . $obj->nome_clausula . '</option>';
				}

				$response['response_data']['lista_clausula'] = $option;

				
			}else{
				$response = $this->response;
			}			
		}else{
			$response = $this->response;
		}
		
		mysqli_close($this->db);
		return $response;
	}

	function addHelpDesk( $data = null ){
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

			if( $response['response_status']['status'] == 1 ){
				$caminho = "https://ineditta.com/documentos_sistema/arquivos_helpdesk/" . $data['caminho_arquivo'];

				$comentario = "{" . '"id_userC":' . '"'.$data['userC'].'"' . ',"data_abertura":' . '"'.date("Y-m-d H:i:s").'"' . ',"comentario":' . '"'.$data['comentario'].'"' . "}";
				
				$sql = "INSERT INTO helpdesk (id_userC,
												data_abertura,
												data_vencimento,
												tipo_chamado,
												estabelecimento,
												sind_laboral,
												sind_patronal,
												clausula,
												comentario_chamado,
												status_chamado,
												caminho_arquivo)
								VALUES ('{$data['userC']}',
										CURRENT_TIMESTAMP(),
										CURRENT_TIMESTAMP(),
										'{$data['tipo_chamado']}',
										'{$data['estabelecimento']}',
										'{$data['sind_labo']}',
										'{$data['sind_patro']}',
										'{$data['clausula']}',
										'{$comentario}',
										'Aberto',
										'{$caminho}')
				";

				$this->logger->debug( $sql );

				if(mysqli_query( $this->db, $sql )){
					$idChamado=mysqli_insert_id($this->db);
					$this->logger->debug( $idChamado );


					// PEGANDO O EMAIL DO USUARIO
					$sqlUser = "SELECT
									id_user,
									nome_usuario,
									email_usuario,
									ids_fmge,
									tipo
								FROM usuario_adm
								WHERE id_user = '". $data['userC'] ."'
								";

					$result = mysqli_query($this->db, $sqlUser);
					$emailUser=$result['email_usuario'];

					$message = "
						<div>
							<p>Prezado(a) Usuário,</p>
							<br>
							<p>O seu chamado foi aberto com sucesso <b>{$idChamado}</b>, <b>Tipo do Chamado:{$data['tipo_chamado']}</b></p>
							<br>
							<p><b>Comentários: </b>{$comentario}</p>
							<p>Acompanhe o status do seu chamado em <b>Gerenciar chamados</b> ( link com a grid Gerenciar Chamados - http://localhost:8080/helpdesk.php )</p>
							<br>
							<p>Atenciosamente,</p>
							<p><b>Ineditta Consultoria Sindical</b></p>
						</div>
					";
					$mail = new disparo_email();

					$response['response_email'] = $mail->dispararEmails([
						"email_remetente" => "no-reply@ineditta.com.br",
						"senha" => "mmzpdogaqaqwnhij",
						"cripto" => "tls",
						"porta" => 587,
						"nome" => "Ineditta Sistema",
						"to_multi" => $emailUser,
						"assunto" => "Chamado aberto " . $idChamado,
						"msg" => $message
					]);

					
					$this->logger->debug( $response['response_email'] );


					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = null;
					$response['response_status']['msg']          = 'Cadastro realizado com sucesso';
				}else{
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Falha ao efetuar registro.';
					$response['response_status']['error']   	 = $this->db->error;
					
					$this->logger->debug( $this->db->error );
				}
			}else{
				$response = $this->response;
			}	
		}else{
			$response = $this->response;
		}
		
		mysqli_close($this->db);
		$this->logger->debug( $response['response_status']['status'] );
		
		return $response;
	}

	function getById( $data = null ){
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
 
			if( $response['response_status']['status'] == 1 ){

				$sqlId = "SELECT 
								uadmc.nome_usuario,
								hd.data_abertura,
								hd.data_vencimento,
								hd.tipo_chamado,
								hd.estabelecimento,
								hd.sind_laboral,
								hd.sind_patronal,
								hd.clausula,
								hd.comentario_chamado->'$.comentario' as comentario
							FROM helpdesk as hd
							LEFT JOIN usuario_adm as uadmc on uadmc.id_user = hd.id_userC
							WHERE idhelpdesk = {$data['id_helpdesk']};
				";

				$this->logger->debug(  $sqlId );

				if( $resultId = mysqli_query( $this->db, $sqlId ) or die (mysqli_error($this->db)) ){
					$obj = $resultId->fetch_object();

					$response['response_data']['userC'] 	= $obj->nome_usuario;
					$response['response_data']['abertura'] 	= $obj->data_abertura;
					$response['response_data']['vencimento'] 	= $obj->data_vencimento;
					$response['response_data']['tipo'] 	= $obj->tipo_chamado;
					$response['response_data']['estabelecimento'] 	= $obj->estabelecimento;
					$response['response_data']['sind_laboral'] 	= $obj->sind_laboral;
					$response['response_data']['sind_patronal'] 	= $obj->sind_patronal;
					$response['response_data']['clausula'] 	= $obj->clausula;
					$response['response_data']['comentario'] 	= $obj->comentario;
				}else{
					$this->logger->debug( $sqlId );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}
			}else{
				$response = $this->response;
			}			
		}else{
			$response = $this->response;
		}
		
		mysqli_close($this->db);
		
		return $response;
	}
	
	function updateHelpdesk( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

			if( $response['response_status']['status'] == 1 ){
				if($data['caminho_arquivo'] != "") {
					$sql = "UPDATE 
								helpdesk
							SET  
								tipo_chamado = '{$data['tipo_chamado']}',
								estabelecimento = '{$data['estabelecimento']}',
								sind_laboral = '{$data['sind_labo']}',
								sind_patronal = '{$data['sind_patro']}',
								clausula = '{$data['clausula']}',
								comentario_chamado = '{$data['comentario']}',
								caminho_arquivo = '{$data['caminho_arquivo']}'
							WHERE 
								idhelpdesk = {$data['id_helpdesk']};
							";
				}else{
					$sql = "UPDATE 
								helpdesk
							SET  
								tipo_chamado = '{$data['tipo_chamado']}',
								estabelecimento = '{$data['estabelecimento']}',
								sind_laboral = '{$data['sind_labo']}',
								sind_patronal = '{$data['sind_patro']}',
								clausula = '{$data['clausula']}',
								comentario_chamado = '{$data['comentario']}'
							WHERE 
								idhelpdesk = {$data['id_helpdesk']};
							";
				};

				$this->logger->debug( $sql );
				if( !mysqli_query( $this->db, $sql ) or die (mysqli_error($this->db)) ){
										
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Não foi possível atualizar o registro';
					$response['response_status']['error_sql']          = mysqli_query( $this->db, $sql ) or die (mysqli_error($this->db));
					
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Registro atualizado com sucesso';
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
		
		return $response;
	}	
	
	function responsavelHelpdesk( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

			if( $response['response_status']['status'] == 1 ){

				$sql = "UPDATE 
							helpdesk
						SET
							inicio_resposta = CURRENT_TIMESTAMP(),
							id_userR = '{$data['id_responsavel']}',
							status_chamado = 'Em análise'
						WHERE
							idhelpdesk = {$data['id_helpdesk']};
						";

				$this->logger->debug( $sql );
				if( !mysqli_query( $this->db, $sql ) or die (mysqli_error($this->db)) ){
										
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['error_sql']   = mysqli_query( $this->db, $sql ) or die (mysqli_error($this->db));
					$response['response_status']['msg']          = 'Não foi possível responder o registro';
					
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Registro respondido com sucesso';
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
		
		return $response;
	}	

	function getByIdResposta( $data = null ){
		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );
 
			if( $response['response_status']['status'] == 1 ){

				$sqlId = "SELECT 
								uadmc.nome_usuario,
								uadmr.nome_usuario as nome_userR,
								hd.id_userR,
								hd.data_abertura,
								hd.data_vencimento,
								hd.tipo_chamado,
								hd.estabelecimento,
								hd.sind_laboral,
								hd.sind_patronal,
								hd.clausula,
								hd.comentario_chamado->'$.comentario' as comentario,
								hd.comentario_chamado->'$.resposta' as resposta,
								hd.comentario_chamado->'$.data_resposta' as data_resposta,
								hd.status_chamado
							FROM helpdesk as hd
							LEFT JOIN usuario_adm as uadmc on uadmc.id_user = hd.id_userC
							LEFT JOIN usuario_adm as uadmr on uadmr.id_user = hd.id_userR
							WHERE idhelpdesk = {$data['id_helpdesk']};
				";

				if( $resultId = mysqli_query( $this->db, $sqlId ) or die (mysqli_error($this->db)) ){
					$obj = $resultId->fetch_object();

					$response['response_data']['userC'] 	= $obj->nome_usuario;
					$response['response_data']['userR'] 	= $obj->nome_userR;
					$response['response_data']['id_responsavel'] 	= $obj->id_userR;
					$response['response_data']['tipo'] 	= $obj->tipo_chamado;
					$response['response_data']['estabelecimento'] 	= $obj->estabelecimento;
					$response['response_data']['sind_laboral'] 	= $obj->sind_laboral;
					$response['response_data']['sind_patronal'] 	= $obj->sind_patronal;
					$response['response_data']['clausula'] 	= $obj->clausula;
					$response['response_data']['comentario'] 	= $obj->comentario;
					$response['response_data']['resposta'] 	= $obj->resposta;
					$response['response_data']['data_resposta'] 	= $obj->data_resposta;
					$response['response_data']['status_res'] 	= $obj->status_chamado;
				}else{
					$this->logger->debug( $sqlId );
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = '';
				}
			}else{
				$response = $this->response;
			}			
		}else{
			$response = $this->response;
		}
		
		mysqli_close($this->db);
		
		return $response;
	}
	
	function conclusaoHelpdesk( $data = null ){

		if( $this->response['response_status']['status'] == 1 ){
			
			// Carregando a resposta padrão da função
			$response = array( "response_status" => array( 'status' => 1, 'error_code' => null, 'msg' => 'Solicitação ' . __FUNCTION__ . ' iniciada com sucesso.' ) );

			if( $response['response_status']['status'] == 1 ){
				$sql_comentario = "SELECT hd.comentario_chamado->'$.id_userC' as id_userC,
										  hd.comentario_chamado->'$.comentario' as comentario,
										  hd.comentario_chamado->'$.data_abertura' as data_open
									FROM helpdesk as hd
									WHERE idhelpdesk = {$data['id_helpdesk']};
								";

				$result_comentario = mysqli_query( $this->db, $sql_comentario );
				$obj = $result_comentario->fetch_object();

				$comentario = "{" . '"id_userC":' . '"'.str_replace('"', '', $obj->id_userC).'"' . ',"data_abertura":' . '"'.str_replace('"', '', $obj->data_open).'"' . ',"comentario":' . '"'.str_replace('"', '', $obj->comentario).'"' . ',"data_resposta":' . '"'.date("Y-m-d H:i:s").'"' . ',"resposta":' . '"'.$data['resposta'].'"' . "}";

				$sql = "UPDATE 
							helpdesk
						SET
							status_chamado = 'Respondido',
							comentario_chamado = '$comentario'
						WHERE
							idhelpdesk = {$data['id_helpdesk']};
						";

				$this->logger->debug( $sql );
				if( !mysqli_query( $this->db, $sql ) or die (mysqli_error($this->db)) ){
										
					$response['response_status']['status']       = 0;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['error_sql']   = mysqli_query( $this->db, $sql ) or die (mysqli_error($this->db));
					$response['response_status']['msg']          = 'Não foi possível responder o registro';
					
					$this->logger->debug( $this->db->error );
					$this->logger->debug( $response );
				}
				else{
					$this->logger->debug( $this->db->error );
								
					$response['response_status']['status']       = 1;
					$response['response_status']['error_code']   = $this->error_code . __LINE__;
					$response['response_status']['msg']          = 'Registro respondido com sucesso';
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
		
		return $response;
	}	
}
?>