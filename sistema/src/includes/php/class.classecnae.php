<?php

include_once "class.model.php";
class classecnae extends model
{

  function __construct()
  {

    parent::__construct(__CLASS__);

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
    }
  }
}

?>