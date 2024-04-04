<?php
// session_start();

/**
 * @author      {Enter5}
 * @package     {1.0.0}
 * @description { }
 * @historic    {
        2020-08-28 11:10 ( v1.0.0 ) - 
    }
 **/

// Exibir na tela os alertas e erro do PHP? SIM = 1 ou NÃO = 0


// Montando o código do erro que será apresentado
$localizar  = array( strtolower(__DIR__), "/", "\\", ".php", ".");
$substituir = array( "", "", "", "", "-" );
$error_code = strtoupper(str_replace($localizar, $substituir,  strtolower(__FILE__))) . "-";

// Declarando os caminhos principais do sistema.
$localizar     = array( "\\", "/includes/php" );    
$substituir    = array( "/", "" );
$path         = str_replace($localizar, $substituir, __DIR__);


// Carregando as informações da requisição
$request = $_REQUEST;
if (isset($_FILES) && !empty($_FILES) ) {
    
    $request['file'] = $_FILES['file'];
    
    //print '{"files":[{"name":"loading.gif","type":"image/gif","size":3897}]}';
    //exit();
}

$fileClassCadcatalogoServicoa = $path . '/includes/php/class.' . $request['module'] . '.php';

if (file_exists($fileClassCadcatalogoServicoa) ) {

    include_once $fileClassCadcatalogoServicoa;

    $classAjaxName  = $request['module']; //tagclausulas
    $classMethod    = $request['action']; //addtagClausulas


    if (class_exists($classAjaxName) ) {

        $classAjax = new $classAjaxName();
    
        if ($classAjax->response['response_status']['status'] == 1 ) {
            
            if (method_exists($classAjax, $classMethod) ) {
                
                $response = $classAjax->$classMethod($request);
                
                
            } else {
                $response['response_status']['status']     = 0;
                $response['response_status']['error_code'] = $error_code . __LINE__;
                $response['response_status']['msg']        = 'Não foi possível determinar a ação para sua solicitação.';
            }
        } else {
            $response['response_status']['status']     = 0;
            $response['response_status']['error_code'] = $error_code . __LINE__;
            $response['response_status']['msg']        = $classAjax->response['response_status']['error_code'] . '::' . $classAjax->response['response_status']['msg'];
        }
    } else {
        $response['response_status']['status']     = 0;
        $response['response_status']['error_code'] = $error_code . __LINE__;
        $response['response_status']['msg']        = 'Não foi possível determinar a ação para sua solicitação.';
    }
} else {
    $response['response_status']['status']     = 0;
    $response['response_status']['error_code'] = $error_code . __LINE__;
    $response['response_status']['msg']        = 'Não foi possível carregar as funcionalidades para execução da solicitação.';
    $response['response_status']['request'] = $request;
}

// Preparando a resposta


if (isset($response['response_status']['status']) ) {
    $responseJson = json_encode($response);
} else {
    $responseJson = '{ "response_status": { "status": "0", "error_code": "' . $error_code . __LINE__ . '", "msg": "O sistema não gerou uma resposta para sua solicitação." } }';
}

print $responseJson;
?>
