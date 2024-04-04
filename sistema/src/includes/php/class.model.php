<?php
/**
 * @author    {Lucas Alcantara}
 * @package   {1.0.0}
 * @description	{ }
 * @historic {
2023-05-17 17:17 ( v1.0.0 ) -
}
 **/

abstract class model
{
    /*** @var */
    public $response;

    /*** @var */
    public $error_code;

    /*** @var */
    protected $logger;

    /*** @var */
    protected $getconfig;

    /*** @var */
    protected $db;

    /*** @var */
    private $path;


    public function __construct($class)
    {
        //Iniciando resposta padrão do construtor.
        $this->response = array("response_status" => array('status' => 1, 'error_code' => null, 'msg' => 'Classe ' . $class . ' iniciada com sucesso.'));

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

    /**
     * @return array|array[]
     */
    protected function connectdb()
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

    protected function executeQuery(string $query, string $successMessage, string $errorMessage)
    {
        if(!$this->db->query($query)) {

            $this->logger->debug( $query );
            $this->logger->debug( $this->db->error );

            $response['response_status']['status'] = 0;
            $response['response_status']['error_code']  	= $this->error_code . __LINE__;
            $response['response_status']['msg']         	= $errorMessage;
        }else {
            $response['response_status']['status'] = 1;
            $response['response_status']['error_code']  	= null;
            $response['response_status']['msg']         	= $successMessage;
        }

        return $response;
    }

    protected function read(string $query)
    {
        return $this->db->query($query);
    }

    /**
     * @param string $sql
     * @param int $draw
     * @param array $columns
     * @return mixed
     */
    function renderPrincipalTable(string $sql, int $draw, string $modalToOpen, string $onClickFunction, array $columns) {

        $this->logger->debug(  $sql );

        if( $resultsql = mysqli_query( $this->db, $sql ) ){

            $count = "SELECT FOUND_ROWS() as count";
            $resultCount = mysqli_query( $this->db, $count )->fetch_object();

            //GET COLUMNS
            $columnsList = [];
            foreach ($columns as $item) {
                array_push($columnsList, $item['data']);
            }
            $this->logger->debug(  $columnsList );

            $array = [];
            while ($obj = $resultsql->fetch_assoc()) {
                $new = [];
//                $new['button'] =;
                for ($i = 0; $i < count($columnsList); $i++) {
                    if($columnsList[$i] == "button") {
                        $new[$columnsList[$i]] = "<td><a data-toggle='modal' href='#{$modalToOpen}' onclick='{$onClickFunction}({$obj['id']});' class='btn-default-alt'  id='bootbox-demo-5'><i class='fa fa-file-text'></i></a></td>";
                    }else {
                        $new[$columnsList[$i]] = $obj[$columnsList[$i]];
                    }
                }

                array_push($array, $new);
            }
            $response['draw'] =  $draw;
            $response['data'] = $array;
            $response['recordsTotal'] = $resultCount->count;
            $response['recordsFiltered'] = $resultCount->count;

            $response['response_status']['status']       = 1;
            $response['response_status']['msg']          = 'Busca realizada com sucesso!';

        }else{
            $this->logger->debug( $this->db->error );

            $response['response_status']['status']       = 0;
            $response['response_status']['error_code']   = $this->error_code . __LINE__;
            $response['response_status']['msg']          = 'Erro ao processar a query!';
        }
        $this->logger->debug(  $response );
        return $response;
    }
}
